import { useEffect } from 'react';
import { useAppDispatch } from './useAppDispatch';
import { useAppSelector } from './useAppSelector';
import { getScopeKey } from '@/store/createCrudSlice';
import type { CrudState } from '@/store/createCrudSlice';

/**
 * Reads a createCrudSlice cache and fetches only when it is actually cold.
 *
 * Screens call this instead of a `useEffect(() => { fetch() }, [])`, which is what made
 * navigating back to a screen re-issue the same GET every time. On a warm cache this
 * dispatches nothing, so the rows render from the store on the first paint.
 */
export const useCachedRows = <TRow>(
  sliceKey: string,
  // The slice thunks are created by createAsyncThunk, whose call signature is wider
  // than a plain function; accept it as-is rather than restating it at every call site.
  fetchAll: any,
) => {
  const dispatch = useAppDispatch();
  const state = useAppSelector(
    (root) => (root as any)[sliceKey] as CrudState<TRow>,
  );

  const { loaded, stale, scopeKey } = state;
  const activeScopeKey = getScopeKey();
  const matchesScope = scopeKey === activeScopeKey;

  useEffect(() => {
    if (!loaded || stale || !matchesScope) {
      dispatch(fetchAll(false));
    }
  }, [dispatch, fetchAll, loaded, stale, activeScopeKey, matchesScope]);

  return {
    rows: matchesScope ? state.rows : [],
    lookups: matchesScope ? state.lookups : {},
    loading: state.loading || !matchesScope,
    loaded: state.loaded && matchesScope,
    error: state.error,
    /** Explicit hard refresh, e.g. a toolbar reload button. */
    refresh: () => dispatch(fetchAll(true)),
  };
};
