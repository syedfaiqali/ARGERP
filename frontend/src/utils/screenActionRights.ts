// Match the legacy create aliases accepted by Auth.Service's MenuController.
const createRights = new Set(["create", "creates", "add", "new", "save"]);

export const normalizeScreenActionRight = (right: string): string => {
  const normalized = right.trim().toLowerCase();
  return createRights.has(normalized) ? "create" : normalized;
};

export const hasGrantedScreenActionRight = (grantedRights: string[], requestedRight: string): boolean =>
  grantedRights.some((right) => normalizeScreenActionRight(right) === normalizeScreenActionRight(requestedRight));

/** Row double-click is reserved for users who can create or update. */
export const canOpenGridRowOnDoubleClick = ({ permissionsLoaded, canCreate, canUpdate, readOnly = false }: {
  permissionsLoaded: boolean; canCreate: boolean; canUpdate: boolean; readOnly?: boolean;
}): boolean => permissionsLoaded && !readOnly && (canCreate || canUpdate);
