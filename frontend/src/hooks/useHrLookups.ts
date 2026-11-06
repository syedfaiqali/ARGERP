import { useEffect, useMemo } from 'react';
import { useAppDispatch } from './useAppDispatch';
import { useAppSelector } from './useAppSelector';
import {
  fetchHrLookups,
  fetchHrLovs,
  type HrLookupName,
  type HrLovRow,
} from '@/store/hrLookupsSlice';

const EMPTY_ROWS: HrLovRow[] = [];
const EMPTY_LIST: any[] = [];

/**
 * Reads the shared HR LOV cache, fetching only the vtypes that are not cached yet.
 *
 * Screens list every vtype they need in one call; on a warm cache this dispatches
 * nothing, so the dropdowns are populated on the first paint.
 */
export const useHrLovs = (vtypes: string[]) => {
  const dispatch = useAppDispatch();
  const lovs = useAppSelector((state) => state.hrLookups.lovs);

  // The caller usually passes an inline array literal, so key the effect on the
  // contents rather than the identity to avoid redispatching on every render.
  const key = vtypes.join(',');

  useEffect(() => {
    const wanted = key ? key.split(',') : [];
    if (wanted.length > 0) dispatch(fetchHrLovs({ vtypes: wanted }));
  }, [dispatch, key]);

  return useMemo(
    () => ({
      /** Rows for one vtype; an empty array until that type has loaded. */
      lov: (vtype: string): HrLovRow[] => lovs[vtype] ?? EMPTY_ROWS,
      lovs,
      refresh: () =>
        dispatch(fetchHrLovs({ vtypes: key ? key.split(',') : [], force: true })),
    }),
    [dispatch, key, lovs],
  );
};

/**
 * Reads the shared, client/entity-scoped HR lookup lists (departments, institutions,
 * designations and friends), fetching only what is missing for the current scope.
 */
export const useHrLookups = (names: HrLookupName[]) => {
  const dispatch = useAppDispatch();
  const lookups = useAppSelector((state) => state.hrLookups.lookups);

  const key = names.join(',');

  useEffect(() => {
    const wanted = (key ? key.split(',') : []) as HrLookupName[];
    if (wanted.length > 0) dispatch(fetchHrLookups({ names: wanted }));
  }, [dispatch, key]);

  return useMemo(
    () => ({
      /** Rows for one lookup route; an empty array until it has loaded. */
      lookup: (name: HrLookupName): any[] => lookups[name] ?? EMPTY_LIST,
      lookups,
      refresh: () =>
        dispatch(
          fetchHrLookups({
            names: (key ? key.split(',') : []) as HrLookupName[],
            force: true,
          }),
        ),
    }),
    [dispatch, key, lookups],
  );
};
