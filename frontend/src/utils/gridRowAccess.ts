import { moduleRouteMap } from './menuNavigation';
import type { VisibleMenuCardGroup } from '@/store/menuPermissionsSlice';

const key = (value: string) => value.trim().toLowerCase().replace(/&/g, 'and').replace(/[^a-z0-9]+/g, ' ').trim();
const identity = (value: string) => key(value).replace(/ /g, '');

/** null denies an unresolved scoped screen; undefined retains legacy metadata fallback. */
export function getGridRowPermissionMenuId(screenName: string, pathname: string, parentName: string | undefined, groups: VisibleMenuCardGroup[]): number | null | undefined {
    if (!groups?.length) return undefined;
    const prefix = pathname.split('/').filter(Boolean)[0];
    const route = prefix === 'admin' ? '/security' : prefix === 'fee' ? '/fees' : `/${prefix}`;
    if (!Object.values(moduleRouteMap).includes(route)) return undefined;
    const ids = new Set(groups.filter(group => moduleRouteMap[key(group.moduleName)] === route)
        .flatMap(group => group.menuItems)
        .filter(item => Number(item.menuCatId) === 24 &&
            (identity(item.name) === identity(screenName) || identity(item.screenKey || '') === identity(screenName)) &&
            (!parentName || key(item.parentName) === key(parentName)))
        .map(item => item.menuId).filter((id): id is number => id != null));
    return ids.size === 1 ? [...ids][0] : null;
}
