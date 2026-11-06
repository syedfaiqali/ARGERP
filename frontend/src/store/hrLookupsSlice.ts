import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { GetService } from '@/constants/globalItems';
import { extractItems, getScopeKey } from './createCrudSlice';

/**
 * Shared cache for the Human Resource reference lookups.
 *
 * Almost every HR screen opens by fetching the same handful of lists: `Lov/v3/<VTYPE>`
 * for the dropdown enumerations and `Lookups/v3/<name>` for departments, institutions and
 * designations. LeaveType alone issued eleven of these on mount, and departments was
 * re-fetched by twelve different screens. They are cached here once and shared.
 *
 * The two families have different scoping rules, so they cache differently:
 *
 *  - LOV rows are global. LovV3Controller filters on `Vtype` and `UtBlock` only, with no
 *    client or entity predicate, so the same rows are served to every tenant. They are
 *    therefore kept for the whole session and deliberately survive a campus switch.
 *  - `Lookups/v3/*` IS client/entity-scoped, so those entries are tagged with the scope
 *    they were fetched under and refetched when it changes.
 */

/**
 * A LOV row as LovV3Controller projects it: { Id, Code, Name, LongName }.
 *
 * The HR screens read these rows under either naming convention — some use
 * `name`/`longName`, others the legacy `stxt`/`ltxt` (see `getLovLabel` on
 * AttendancePolicy and CustomShift). Both are populated so a row renders the
 * same whichever field a screen reaches for.
 */
export interface HrLovRow {
  id: number;
  code: string;
  name: string;
  longName: string;
  stxt: string;
  ltxt: string;
  /** PascalCase and shortName aliases: several screens fall back through
   *  `name || Name || shortName || stxt || ltxt` when labelling an option. */
  Name: string;
  shortName: string;
}

interface HrLookupsState {
  /** LOV rows keyed by vtype (e.g. "GEND", "LVUNT"). Global; no scope key. */
  lovs: Record<string, HrLovRow[]>;
  /** Scoped lookup rows keyed by route name (e.g. "departments"). */
  lookups: Record<string, any[]>;
  /** Scope the `lookups` entries were fetched under. */
  lookupScopeKey: string;
  /** In-flight//loaded bookkeeping keyed the same way, so two screens mounting
   *  together don't each dispatch the same request. */
  pending: Record<string, string>;
  error: string | null;
}

const initialState: HrLookupsState = {
  lovs: {},
  lookups: {},
  lookupScopeKey: '',
  pending: {},
  error: null,
};

const normalizeLovRow = (raw: any): HrLovRow => {
  const name = raw?.name ?? raw?.Name ?? raw?.stxt ?? raw?.Stxt ?? '';
  const longName = raw?.longName ?? raw?.LongName ?? raw?.ltxt ?? raw?.Ltxt ?? '';

  return {
    id: Number(raw?.id ?? raw?.Id ?? 0),
    code: raw?.code ?? raw?.Code ?? '',
    name,
    longName,
    // Legacy aliases: screens that read stxt/ltxt get the same value.
    stxt: name,
    ltxt: longName,
    Name: name,
    shortName: name,
  };
};

/**
 * Loads one or more LOV types, skipping any already cached.
 *
 * Screens pass every vtype they need in a single dispatch, so the uncached ones go out
 * in parallel and the cached ones cost nothing.
 */
export const fetchHrLovs = createAsyncThunk(
  'hrLookups/fetchLovs',
  async (
    args: { vtypes: string[]; force?: boolean },
    { getState, rejectWithValue, requestId },
  ) => {
    const { vtypes, force = false } = args;
    const state = (getState() as any).hrLookups as HrLookupsState;

    const wanted = Array.from(new Set(vtypes.filter(Boolean)));
    const missing = force
      ? wanted
      : wanted.filter((vtype) => !state.lovs[vtype] && state.pending[`lov:${vtype}`] === requestId);

    if (missing.length === 0) return { lovs: {} as Record<string, HrLovRow[]> };

    try {
      const responses = await Promise.allSettled(
        missing.map((vtype) => GetService(`/api/HumanResource/Lov/v3/${vtype}`)),
      );

      const lovs: Record<string, HrLovRow[]> = {};
      missing.forEach((vtype, index) => {
        const result = responses[index];
        // A failing vtype must not blank the others; it stays uncached and retries later.
        if (result.status !== 'fulfilled') return;
        lovs[vtype] = extractItems(result.value).map(normalizeLovRow);
      });

      return { lovs };
    } catch (e: any) {
      return rejectWithValue(e?.message || 'Failed to load HR lookups');
    }
  },
);

/** Route names under `/api/HumanResource/Lookups/v3/`. */
export type HrLookupName =
  | 'departments'
  | 'designations'
  | 'institutions'
  | 'entities'
  | 'countries'
  | 'states'
  | 'cities'
  | 'religions';

/** Loads one or more scoped lookup lists, skipping any already cached for this scope. */
export const fetchHrLookups = createAsyncThunk(
  'hrLookups/fetchLookups',
  async (
    args: { names: HrLookupName[]; force?: boolean },
    { getState, rejectWithValue, requestId },
  ) => {
    const { names, force = false } = args;
    const state = (getState() as any).hrLookups as HrLookupsState;
    const scopeKey = getScopeKey();
    // A scope change invalidates every cached entry, so refetch what was asked for.
    const scopeChanged = state.lookupScopeKey !== scopeKey;

    const wanted = Array.from(new Set(names.filter(Boolean)));
    const missing = wanted.filter(
      (name) => (force || scopeChanged || !state.lookups[name])
        && state.pending[`lookup:${name}`] === requestId,
    );

    if (missing.length === 0) return { lookups: {} as Record<string, any[]>, scopeKey };

    try {
      const responses = await Promise.allSettled(
        missing.map((name) => GetService(`/api/HumanResource/Lookups/v3/${name}`)),
      );

      const lookups: Record<string, any[]> = {};
      missing.forEach((name, index) => {
        const result = responses[index];
        if (result.status !== 'fulfilled') return;
        lookups[name] = extractItems(result.value);
      });

      return { lookups, scopeKey };
    } catch (e: any) {
      return rejectWithValue(e?.message || 'Failed to load HR lookups');
    }
  },
);

const hrLookupsSlice = createSlice({
  name: 'hrLookups',
  initialState,
  reducers: {
    /** Drops the scoped lookups but keeps the global LOVs, which no scope affects. */
    clearHrScopedLookups: (state) => {
      state.lookups = {};
      state.lookupScopeKey = '';
    },
    clearHrLookups: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchHrLovs.pending, (state, action) => {
        action.meta.arg.vtypes.forEach((vtype) => {
          // pending is dispatched before the thunk reads state. Track its owner
          // so the request can fetch its own rows without duplicating another fetch.
          if (action.meta.arg.force || (!state.lovs[vtype] && !state.pending[`lov:${vtype}`])) {
            state.pending[`lov:${vtype}`] = action.meta.requestId;
          }
        });
      })
      .addCase(fetchHrLovs.fulfilled, (state, action) => {
        Object.assign(state.lovs, action.payload.lovs);
        action.meta.arg.vtypes.forEach((vtype) => {
          if (state.pending[`lov:${vtype}`] === action.meta.requestId) delete state.pending[`lov:${vtype}`];
        });
      })
      .addCase(fetchHrLovs.rejected, (state, action) => {
        state.error = action.payload as string;
        action.meta.arg.vtypes.forEach((vtype) => {
          if (state.pending[`lov:${vtype}`] === action.meta.requestId) delete state.pending[`lov:${vtype}`];
        });
      })
      .addCase(fetchHrLookups.pending, (state, action) => {
        const scopeChanged = state.lookupScopeKey !== getScopeKey();
        action.meta.arg.names.forEach((name) => {
          if (action.meta.arg.force || ((scopeChanged || !state.lookups[name]) && !state.pending[`lookup:${name}`])) {
            state.pending[`lookup:${name}`] = action.meta.requestId;
          }
        });
      })
      .addCase(fetchHrLookups.fulfilled, (state, action) => {
        // Clear stale entries before merging when the scope moved underneath us.
        if (state.lookupScopeKey !== action.payload.scopeKey) {
          state.lookups = {};
          state.lookupScopeKey = action.payload.scopeKey;
        }
        Object.assign(state.lookups, action.payload.lookups);
        action.meta.arg.names.forEach((name) => {
          if (state.pending[`lookup:${name}`] === action.meta.requestId) delete state.pending[`lookup:${name}`];
        });
      })
      .addCase(fetchHrLookups.rejected, (state, action) => {
        state.error = action.payload as string;
        action.meta.arg.names.forEach((name) => {
          if (state.pending[`lookup:${name}`] === action.meta.requestId) delete state.pending[`lookup:${name}`];
        });
      });
  },
});

export const { clearHrLookups, clearHrScopedLookups } = hrLookupsSlice.actions;

export default hrLookupsSlice.reducer;
