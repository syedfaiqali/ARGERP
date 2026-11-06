import type { VisibleScreenMenuItem } from '../store/menuPermissionsSlice'
import { screens } from '../screens'
export const normalizeMenuName = (value: string) => value.trim().toLowerCase().replace(/&/g, 'and').replace(/[^a-z0-9]+/g, ' ').trim()
export const hasViewActionRight = (_rights?: string[]) => true
export const isViewableReportMenuItem = (_item: unknown) => true
export const getViewableReportMenuItems = <T,>(items: T[]) => items
export const ALWAYS_VISIBLE_MENU_ITEMS = new Set(['Human Resource'])
export const MODULE_SCREEN_MENU_CAT_ID = 24
export const REPORT_MENU_CAT_ID = 25
export const menuPermissionAliases: Record<string, string[]> = {}
const allowed = (..._args: any[]) => true
const screenItems: VisibleScreenMenuItem[] = screens.map((s, i) => ({ menuId: i + 1, menuCatId: 24, reportId: null, reportCode: undefined, name: s.label, screenKey: s.component, controllerId: s.component, parentName: s.group, moduleName: 'Human Resource', path: s.path, actionRights: ['view', 'create', 'update', 'delete', 'print', 'export'] }))
const reports: VisibleScreenMenuItem[] = [
 { menuId: 1001, name: 'Employee Profile Report', screenKey: 'EmployeeProfileReport', path: '/hr/employee-profile', parentName: 'Reports', menuCatId: 25, actionRights: ['view'], reportCode: 'employees' },
 { menuId: 1002, name: 'Attendance Report Format', screenKey: 'AttendanceReport', path: '/hr/employee-monthly-attendance', parentName: 'Reports', menuCatId: 25, actionRights: ['view'], reportCode: 'attendance' },
 { menuId: 1003, name: 'Pay Slip New', screenKey: 'PayslipReport', path: '/hr/pay-slip', parentName: 'Reports', menuCatId: 25, actionRights: ['view'], reportCode: 'payslips' },
]
const items = [...screenItems, ...reports]
const access = {
  hiddenMenuNames: [], hiddenScreenNames: [], visibleScreenNames: items.map(i => i.name), visibleScreenLabels: items.map(i => i.name), visibleScreenMenuItems: items,
  visibleMenuCardGroups: [...new Set([...screens.map(s => s.group), 'Reports'])].map(group => ({ moduleName: 'Human Resource', cardName: group, menuItems: items.filter(i => i.parentName === group) })),
  menuPermissionsLoaded: true, screenPermissionsLoaded: true, menuPermissionsLoading: false,
  isMenuAllowed: allowed, hasOrgLegalEntityCreateRight: true, canAccessCampusDropdown: true, isScreenHidden: (..._args: any[]) => false,
  hasScreenActionRight: allowed, hasScreenActionRightForParent: allowed, hasScreenActionRightForMenuId: allowed,
  hasViewableReportModule: allowed, hasViewableReportCard: allowed, hasViewableReport: allowed,
  getScreenMenuItem: (name: string, ..._args: any[]) => items.find(i => normalizeMenuName(i.name) === normalizeMenuName(name) || normalizeMenuName(i.screenKey || '') === normalizeMenuName(name)) ?? items[0],
}
export const useHiddenMenuPermissions = () => access
