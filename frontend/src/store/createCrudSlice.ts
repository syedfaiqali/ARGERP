import { getCrudScopeKey } from "@/utils/crudScope";
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { Draft, PayloadAction } from '@reduxjs/toolkit';
import { GetService } from '@/constants/globalItems';

/**
 * Shared cache layer for the simple list+CRUD screens (Organization Setup and friends).
 *
 * Every screen used to `useState` its rows and refetch on mount, so navigating away and
 * back re-ran the GET. These slices keep the rows in the store instead and only hit the
 * API when the cache has never been filled, was marked stale, or belongs to a different
 * campus/client scope. CRUD handlers patch the cached rows in place via upsert/remove so
 * a save never costs a full reload.
 */

/** Identity of the logged-in scope. Rows cached for one campus must not leak into another. */
export const getScopeKey = (): string => getCrudScopeKey();

/** Unwraps the several response envelopes the v3 controllers return. */
export const extractItems = (res: any): any[] => {
  const data =
    res?.data?.items ??
    res?.data?.Items ??
    res?.data?.data?.items ??
    res?.data?.Data?.Items ??
    res?.data?.data ??
    res?.data?.Data ??
    res?.data ??
    [];

  return Array.isArray(data) ? data : [];
};

export interface CrudState<TRow> {
  rows: TRow[];
  /** Extra per-screen cached payloads (lookups/dropdowns) keyed by name. */
  lookups: Record<string, any>;
  loading: boolean;
  /** True once a fetch has succeeded; gates the "don't refetch on remount" behaviour. */
  loaded: boolean;
  /** Set when something outside this slice invalidates the cache. */
  stale: boolean;
  scopeKey: string;
  error: string | null;
  requestId: string | null;
}

interface CreateCrudSliceOptions<TRow> {
  /** Slice name; also the store key. */
  name: string;
  /** List endpoint, already including any query string. */
  url: string;
  /** Maps a raw API record onto the row shape the screen renders. */
  normalize: (raw: any) => TRow;
  /** Reads the primary key off a row so upsert/remove can match records. */
  getId: (row: TRow) => number | string | null | undefined;
  /** Optional ordering applied after every mutation so inserts land in the right place. */
  sort?: (a: TRow, b: TRow) => number;
  /** Optional extra requests resolved in parallel with the list (dropdowns etc.). */
  lookups?: Record<string, string | (() => Promise<any>)>;
}

export const createCrudSlice = <TRow>({
  name,
  url,
  normalize,
  getId,
  sort,
  lookups,
}: CreateCrudSliceOptions<TRow>) => {
  const initialState: CrudState<TRow> = {
    rows: [],
    lookups: {},
    loading: false,
    loaded: false,
    stale: false,
    scopeKey: '',
    error: null,
    requestId: null,
  };

  const fetchAll = createAsyncThunk(
    `${name}/fetchAll`,
    async (force: boolean = false, { getState, rejectWithValue }) => {
      const scopeKey = getScopeKey();
      const slice = (getState() as any)?.[name] as CrudState<TRow> | undefined;

      // Cache hit: serve what is already in the store and make no network call at all.
      if (!force && slice?.loaded && !slice.stale && slice.scopeKey === scopeKey) {
        return { rows: slice.rows, lookups: slice.lookups, scopeKey, cached: true };
      }

      try {
        const lookupEntries = Object.entries(lookups ?? {});

        // List + every lookup go out together instead of waterfalling one after another.
        const [listRes, ...lookupResults] = await Promise.all([
          GetService(force ? `${url}${url.includes('?') ? '&' : '?'}refresh=${Date.now()}` : url),
          ...lookupEntries.map(([, source]) =>
            typeof source === 'function' ? source() : GetService(source),
          ),
        ]);

        const resolvedLookups: Record<string, any> = {};
        lookupEntries.forEach(([key], index) => {
          resolvedLookups[key] = extractItems(lookupResults[index]);
        });

        const rows = extractItems(listRes).map(normalize);
        if (sort) rows.sort(sort);

        return { rows, lookups: resolvedLookups, scopeKey, cached: false };
      } catch (e: any) {
        return rejectWithValue(e?.response?.data?.error || e?.message || `Failed to load ${name}`);
      }
    },
  );

  const slice = createSlice({
    name,
    initialState,
    reducers: {
      /** Insert or replace one record after a create/update, keeping the cache authoritative. */
      upsertRow: (state, action: PayloadAction<TRow>) => {
        const row = action.payload as Draft<TRow>;
        const id = getId(action.payload);
        if (id === null || id === undefined || id === '') return;

        const index = state.rows.findIndex((item) => getId(item as TRow) === id);
        if (index >= 0) {
          state.rows[index] = row;
        } else {
          state.rows.push(row);
        }

        if (sort) (state.rows as TRow[]).sort(sort);
        state.loaded = true;
        state.stale = false;
        state.scopeKey = getScopeKey();
      },
      removeRow: (state, action: PayloadAction<number | string>) => {
        state.rows = state.rows.filter((item) => getId(item as TRow) !== action.payload);
        state.loaded = true;
        state.stale = false;
        state.scopeKey = getScopeKey();
      },
      setLookup: (state, action: PayloadAction<{ key: string; value: any }>) => {
        state.lookups[action.payload.key] = action.payload.value;
      },
      /** Flags the cache for reload on the next visit without clearing what is shown now. */
      markStale: (state) => {
        state.stale = true;
      },
      clear: () => initialState,
    },
    extraReducers: (builder) => {
      builder
        .addCase(fetchAll.pending, (state, action) => {
          state.requestId = action.meta.requestId;
          if (state.scopeKey !== getScopeKey()) {
            state.rows = [];
            state.lookups = {};
            state.loaded = false;
          }
          // A cache hit resolves synchronously; don't flash a spinner for it.
          if (!(action.meta.arg !== true && state.loaded && !state.stale)) {
            state.loading = true;
          }
          state.error = null;
        })
        .addCase(fetchAll.fulfilled, (state, action) => {
          if (state.requestId !== action.meta.requestId) return;
          if (action.payload.scopeKey !== getScopeKey()) return;
          state.loading = false;
          state.rows = action.payload.rows as Draft<TRow>[];
          state.lookups = action.payload.lookups;
          state.loaded = true;
          state.stale = false;
          state.scopeKey = action.payload.scopeKey;
        })
        .addCase(fetchAll.rejected, (state, action) => {
          if (state.requestId !== action.meta.requestId) return;
          state.loading = false;
          state.error = action.payload as string;
        });
    },
  });

  return {
    reducer: slice.reducer,
    fetchAll,
    actions: slice.actions,
  };
};
