import { regionalScope } from './regionalSettings';

export const GRID_PAGING_OPTIONS = [5, 10, 15, 20, 50, 100];
export const DEFAULT_GRID_PAGING = 20;
export const GRID_PAGING_CHANGED = 'system-policy-grid-paging-changed';
const storagePrefix = 'systemPolicyGridPaging:';
const values = new Map<string, number>();
const revisions = new Map<string, number>();
const listeners = new Set<() => void>();

export const gridPagingScope = regionalScope;
export function normalizeGridPaging(value: unknown): number {
  const parsed = Number(value);
  return GRID_PAGING_OPTIONS.includes(parsed) ? parsed : DEFAULT_GRID_PAGING;
}

export function getGridPaging(scope = gridPagingScope()): number {
  if (!scope) return DEFAULT_GRID_PAGING;
  if (values.has(scope)) return values.get(scope)!;
  try { return normalizeGridPaging(localStorage.getItem(storagePrefix + scope)); }
  catch { return DEFAULT_GRID_PAGING; }
}

export const getGridPagingRevision = (scope: string) => revisions.get(scope) || 0;
export const getGridPagingSnapshot = () => `${gridPagingScope()}/${getGridPaging()}`;
export const notifyGridPaging = () => listeners.forEach(listener => listener());

export function setGridPaging(value: unknown, scope = gridPagingScope()): void {
  if (!scope) return;
  const size = normalizeGridPaging(value);
  values.set(scope, size);
  revisions.set(scope, getGridPagingRevision(scope) + 1);
  try { localStorage.setItem(storagePrefix + scope, String(size)); }
  catch { /* The in-memory policy still works when browser storage is disabled. */ }
  notifyGridPaging();
}

export function subscribeGridPaging(listener: () => void): () => void {
  if (listeners.size === 0) {
    window.addEventListener('storage', storageChanged);
    window.addEventListener(GRID_PAGING_CHANGED, notifyGridPaging);
  }
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
    if (listeners.size === 0) {
      window.removeEventListener('storage', storageChanged);
      window.removeEventListener(GRID_PAGING_CHANGED, notifyGridPaging);
    }
  };
}

function storageChanged(event: StorageEvent) {
  if (event.key === null || event.key === storagePrefix + gridPagingScope()) {
    values.delete(gridPagingScope());
    revisions.set(gridPagingScope(), getGridPagingRevision(gridPagingScope()) + 1);
    notifyGridPaging();
  }
}
