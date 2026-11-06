import { useCallback, useEffect, useRef, useState, useSyncExternalStore } from 'react';
import type { Dispatch, SetStateAction } from 'react';
import { loadGridPaging } from '@/services/gridPagingService';
import { DEFAULT_GRID_PAGING, getGridPaging, getGridPagingSnapshot, subscribeGridPaging } from '@/utils/systemPolicyGridPaging';
export { GRID_PAGING_OPTIONS } from '@/utils/systemPolicyGridPaging';

export function useGridPagingPolicy() {
  const snapshot = useSyncExternalStore(subscribeGridPaging, getGridPagingSnapshot, () => `/${DEFAULT_GRID_PAGING}`);
  const scope = snapshot.split('/')[0];
  useEffect(() => { void loadGridPaging(); }, [scope]);
  return { snapshot, pageSize: getGridPaging() };
}

/** A manual choice lasts for this grid until its school or policy changes. */
export function useGridPageSize(resetPage?: () => void): [number, Dispatch<SetStateAction<number>>] {
  const { snapshot, pageSize: policySize } = useGridPagingPolicy();
  const [choice, setChoice] = useState<{ snapshot: string; size: number } | null>(null);
  const size = choice?.snapshot === snapshot ? choice.size : policySize;
  const reset = useRef(resetPage);
  reset.current = resetPage;
  const currentPolicy = useRef({ snapshot, policySize });
  currentPolicy.current = { snapshot, policySize };
  useEffect(() => { reset.current?.(); }, [size, snapshot]);
  const setSize: Dispatch<SetStateAction<number>> = useCallback(value => {
    const { snapshot, policySize } = currentPolicy.current;
    setChoice(previous => {
      const current = previous?.snapshot === snapshot ? previous.size : policySize;
      return { snapshot, size: typeof value === 'function' ? value(current) : value };
    });
  }, []);
  return [size, setSize];
}

