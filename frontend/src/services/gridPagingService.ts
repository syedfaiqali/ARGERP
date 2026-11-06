import { GetService } from '@/constants/globalItems';
import { getGridPaging, getGridPagingRevision, gridPagingScope, notifyGridPaging, setGridPaging } from '@/utils/systemPolicyGridPaging';

const pending = new Map<string, Promise<number>>();
const loaded = new Set<string>();

/** One policy request per school/session, shared by every mounted grid. */
export function loadGridPaging(force = false): Promise<number> {
  const scope = gridPagingScope();
  notifyGridPaging();
  if (!scope) return Promise.resolve(getGridPaging());
  const current = pending.get(scope);
  if (current) return current;
  if (!force && loaded.has(scope)) return Promise.resolve(getGridPaging(scope));
  const revision = getGridPagingRevision(scope);
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000);
  const url = '/api/Administration/SystemPolicyGeneral/v3/grid-paging' + (force ? `?gridPagingRefresh=${Date.now()}` : '');
  const request = GetService(url, undefined, undefined, controller.signal)
    .then((response: any) => {
      const body = response?.data ?? response;
      if (response?.status >= 400 || body?.isSuccessCall === false || body?.IsSuccessCall === false)
        throw new Error('Unable to load grid paging policy');
      const policy = body?.data ?? body?.Data ?? body;
      // A slow initial read must not overwrite a policy saved in the meantime.
      if (getGridPagingRevision(scope) === revision)
        setGridPaging(policy?.gridPaging ?? policy?.GridPaging, scope);
      loaded.add(scope);
      return getGridPaging(scope);
    })
    .catch(() => getGridPaging(scope))
    .finally(() => { clearTimeout(timeout); pending.delete(scope); });
  pending.set(scope, request);
  return request;
}
