import {
  MODULE_SCREEN_MENU_CAT_ID,
  getViewableReportMenuItems,
  menuPermissionAliases,
  normalizeMenuName,
} from "@/hooks/useHiddenMenuPermissions";
import type {
  VisibleMenuCardGroup,
  VisibleScreenMenuItem,
} from "@/store/menuPermissionsSlice";

export const moduleRouteMap: Record<string, string> = {
  academics: "/academics",
  academic: "/academics",
  student: "/student",
  "student information": "/student",
  "student management": "/student",
  "human resource": "/hr",
  "human resources": "/hr",
  hr: "/hr",
  "fee management": "/fees",
  "fees management": "/fees",
  transport: "/transport",
  library: "/library",
  communication: "/comm",
  "front office": "/front-office",
  "organization setup": "/org",
  organization: "/org",
  administration: "/org",
  "fixed asset": "/fixed-asset",
  "inventory management": "/inventory",
  inventory: "/inventory",
  "item management": "/inventory",
  "financial management": "/finance",
  "general ledger": "/finance",
  "cash management": "/finance",
  "account receivable": "/finance",
  freight: "/freight",
  reports: "/reports",
  "system settings": "/security",
  security: "/security",
  "user authorization": "/security",
};

export const toReportModuleSlug = (value: string) =>
  normalizeMenuName(value).replace(/\s+/g, "-");

export const buildReportRoute = (moduleName: string, report: VisibleScreenMenuItem) => {
  const slug = toReportModuleSlug(moduleName);
  if (!slug) return null;
  const params = new URLSearchParams({ report: report.name });
  if (report.reportId != null) params.set("reportId", String(report.reportId));
  else if (report.menuId != null) params.set("menuId", String(report.menuId));
  return `/reports/${slug}?${params.toString()}`;
};

/**
 * The screens a card actually holds, using the same rule every module landing page
 * applies: the card must own its items in the menu tree, and an item must be a module
 * screen parented to that card. Grouping nodes (an HR "Academic" card) fail it, which
 * is why routing to them lands on an empty module view.
 */
export const getCardScreenItems = (group: VisibleMenuCardGroup) => {
  const holdsModuleScreens = group.menuItems.some((item) =>
    item.path.trim().startsWith(`${group.moduleName}/${group.cardName}/`),
  );
  if (!holdsModuleScreens) return [];

  const cardKey = normalizeMenuName(group.cardName);
  return group.menuItems
    .filter((item) => Number(item.menuCatId) === MODULE_SCREEN_MENU_CAT_ID)
    .filter((item) => normalizeMenuName(item.parentName) === cardKey)
    .filter((item) => item.name.trim());
};

export const getScreenRoute = (moduleRoute: string, screenName: string) =>
  `${moduleRoute}/${encodeURIComponent(screenName)}`;

export const REPORTS_NODE_LABEL = "Reports";

/** One level of the sidebar hover fly-out. */
export interface MenuFlyoutNode {
  name: string;
  /** Where clicking this row goes. */
  route: string;
  children?: MenuFlyoutNode[];
}

const toSectionHash = (cardName: string) => normalizeMenuName(cardName).replace(/\s+/g, "-");

/**
 * The fly-out tree of one sidebar module: its cards with the screens they hold, plus a
 * "Reports" branch holding the module's report cards and, under each, its reports.
 */
export const getModuleFlyoutNodes = (
  groups: VisibleMenuCardGroup[],
  moduleText: string,
  modulePath: string,
): MenuFlyoutNode[] => {
  const aliases = (menuPermissionAliases[moduleText] ?? [moduleText]).map(normalizeMenuName);
  const moduleGroups = groups.filter((group) => aliases.includes(normalizeMenuName(group.moduleName)));
  const screenCards = new Map<string, MenuFlyoutNode>();
  const reportCards = new Map<string, MenuFlyoutNode>();
  let reportsRoute = "";

  moduleGroups.forEach((group) => {
    const cardKey = normalizeMenuName(group.cardName);

    const screens = getCardScreenItems(group);
    if (screens.length > 0) {
      const card = screenCards.get(cardKey) ?? { name: group.cardName, route: modulePath, children: [] };
      screens.forEach((item) => {
        if (card.children!.some((child) => normalizeMenuName(child.name) === normalizeMenuName(item.name))) return;
        card.children!.push({ name: item.name, route: getScreenRoute(modulePath, item.name) });
      });
      screenCards.set(cardKey, card);
    }

    const reports = getViewableReportMenuItems(group.menuItems);
    if (reports.length > 0) {
      const moduleReportsRoute = `/reports/${toReportModuleSlug(group.moduleName)}`;
      if (!reportsRoute) reportsRoute = moduleReportsRoute;

      const card = reportCards.get(cardKey) ?? {
        name: group.cardName,
        route: `${moduleReportsRoute}#${toSectionHash(group.cardName)}`,
        children: [],
      };
      reports.forEach((item) => {
        const route = buildReportRoute(group.moduleName, item);
        if (!route) return;
        if (card.children!.some((child) => normalizeMenuName(child.name) === normalizeMenuName(item.name))) return;
        card.children!.push({ name: item.name, route });
      });
      if (card.children!.length > 0) reportCards.set(cardKey, card);
    }
  });

  const nodes = Array.from(screenCards.values()).filter((card) => card.children!.length > 0);

  if (reportCards.size > 0) {
    nodes.push({
      name: REPORTS_NODE_LABEL,
      route: reportsRoute,
      children: Array.from(reportCards.values()),
    });
  }

  return nodes;
};

export interface SearchDestination {
  name: string;
  moduleName: string;
  cardName: string;
  kind: "screen" | "report";
  route: string;
}

/** Every screen and report the user can open, for master search. */
export const getSearchDestinations = (groups: VisibleMenuCardGroup[]): SearchDestination[] => {
  const found = new Map<string, SearchDestination>();

  const add = (destination: SearchDestination) => {
    const key = `${destination.kind}|${normalizeMenuName(destination.name)}|${destination.route.split("?")[0]}`;
    if (!found.has(key)) found.set(key, destination);
  };

  groups.forEach((group) => {
    const moduleName = group.moduleName.trim();
    const cardName = group.cardName.trim();

    getViewableReportMenuItems(group.menuItems).forEach((item) => {
      const route = buildReportRoute(moduleName, item);
      if (route) add({ name: item.name, moduleName, cardName, kind: "report", route });
    });

    const moduleRoute = moduleRouteMap[normalizeMenuName(moduleName)];
    if (!moduleRoute) return;

    getCardScreenItems(group).forEach((item) => {
      add({
        name: item.name,
        moduleName,
        cardName,
        kind: "screen",
        route: getScreenRoute(moduleRoute, item.name),
      });
    });
  });

  return Array.from(found.values());
};
