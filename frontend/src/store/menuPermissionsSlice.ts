import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import api from "@/services/api";
import type { RootState } from "./store";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";

export interface VisibleScreenMenuItem {
  menuId: number | null;
  /** Stable UT controller identity. Name is an editable display caption. */
  screenKey?: string;
  reportId?: number | null;
  reportCode?: string;
  name: string;
  path: string;
  parentName: string;
  menuCatId: number | null;
  actionRights: string[];
}

export interface VisibleMenuCardGroup {
  moduleName: string;
  cardName: string;
  menuItems: VisibleScreenMenuItem[];
}

interface MenuPermissionsState {
  userId: number | null;
  hiddenMenuNames: string[];
  hiddenScreenNames: string[];
  visibleScreenNames: string[];
  visibleScreenLabels: string[];
  visibleScreenMenuItems: VisibleScreenMenuItem[];
  visibleMenuCardGroups: VisibleMenuCardGroup[];
  actionRightMenuItems: VisibleScreenMenuItem[];
  menuPermissionsLoaded: boolean;
  screenPermissionsLoaded: boolean;
  status: LoadStatus;
  error: string | null;
}

const HIDDEN_MENUS_KEY_PREFIX = "edxs_sidebar_hidden_menus_v4";
const HIDDEN_SCREENS_KEY_PREFIX = "edxs_hidden_screens";
const VISIBLE_SCREENS_KEY_PREFIX = "edxs_visible_screens";
const VISIBLE_SCREEN_LABELS_KEY_PREFIX = "edxs_visible_screen_labels";
const VISIBLE_SCREEN_MENU_ITEMS_KEY_PREFIX = "edxs_visible_screen_menu_items_v5";
const VISIBLE_MENU_CARD_GROUPS_KEY_PREFIX = "edxs_visible_menu_card_groups_v3";
// v3 retains the caption, code and ID; an editable caption must not hide
// a stable create code from permission checks.
const ACTION_RIGHT_MENU_ITEMS_KEY_PREFIX = "edxs_action_right_menu_items_v3";

const getHiddenMenusCacheKey = (userId: number) =>
  `${HIDDEN_MENUS_KEY_PREFIX}_${userId}`;

const getHiddenScreensCacheKey = (userId: number) =>
  `${HIDDEN_SCREENS_KEY_PREFIX}_${userId}`;

const getVisibleScreensCacheKey = (userId: number) =>
  `${VISIBLE_SCREENS_KEY_PREFIX}_${userId}`;

const getVisibleScreenLabelsCacheKey = (userId: number) =>
  `${VISIBLE_SCREEN_LABELS_KEY_PREFIX}_${userId}`;

const getVisibleScreenMenuItemsCacheKey = (userId: number) =>
  `${VISIBLE_SCREEN_MENU_ITEMS_KEY_PREFIX}_${userId}`;

const getVisibleMenuCardGroupsCacheKey = (userId: number) =>
  `${VISIBLE_MENU_CARD_GROUPS_KEY_PREFIX}_${userId}`;

const getActionRightMenuItemsCacheKey = (userId: number) =>
  `${ACTION_RIGHT_MENU_ITEMS_KEY_PREFIX}_${userId}`;

/**
 * Drops a user's stored menus so the next load reads them from the API again.
 * Called on login: a session that starts fresh must pick up whatever the
 * administrator changed since, rather than replaying the previous session's
 * permissions. Between logins the cache is what keeps the menu API from being
 * re-requested on every refresh.
 */
export const clearCachedMenuPermissions = (userId: number) => {
  if (!userId) return;
  [
    getHiddenMenusCacheKey(userId),
    getHiddenScreensCacheKey(userId),
    getVisibleScreensCacheKey(userId),
    getVisibleScreenLabelsCacheKey(userId),
    getVisibleScreenMenuItemsCacheKey(userId),
    getVisibleMenuCardGroupsCacheKey(userId),
    getActionRightMenuItemsCacheKey(userId),
  ].forEach((key) => {
    try {
      localStorage.removeItem(key);
    } catch {
      // A storage-disabled browser simply has no cache to clear.
    }
  });
};

const normalizeMenuName = (value: string) =>
  value
    .trim()
    .toLowerCase()
    .replace(/&/g, "and")
    .replace(/[^a-z0-9]+/g, " ")
    .replace(/\s+/g, " ")
    .trim();

const LIBRARY_CARD_LABELS: Record<string, string> = {
  cataloging: "Setup",
  circulation: "Transaction",
};

const LIBRARY_SCREEN_LABELS: Record<string, string> = {
  "book type": "Media Type",
  "book subject": "Media Category",
  "school library": "Library",
  catalog: "Media",
};

const mapLibraryPermissionLabel = (
  value: string,
  moduleName: string,
  type: "card" | "screen",
) => {
  if (normalizeMenuName(moduleName) !== "library") return value;

  const map = type === "card" ? LIBRARY_CARD_LABELS : LIBRARY_SCREEN_LABELS;
  return map[normalizeMenuName(value)] ?? value;
};

const readNameList = (key: string) => {
  try {
    const cached = localStorage.getItem(key);
    const names = cached ? JSON.parse(cached) : [];
    return Array.isArray(names)
      ? names.filter((name) => typeof name === "string")
      : [];
  } catch {
    return [];
  }
};

const readVisibleMenuItems = (key: string): VisibleScreenMenuItem[] => {
  try {
    const cached = localStorage.getItem(key);
    const items = cached ? JSON.parse(cached) : [];
    return Array.isArray(items)
      ? items.filter(
          (item) =>
            (item?.menuId == null || Number.isFinite(Number(item.menuId))) &&
            typeof item?.name === "string" &&
            typeof item?.path === "string" &&
            typeof item?.parentName === "string",
        ).map((item) => ({
          ...item,
          actionRights: Array.isArray(item.actionRights)
            ? item.actionRights.filter((right: any) => typeof right === "string")
            : [],
        }))
      : [];
  } catch {
    return [];
  }
};

const readVisibleMenuCardGroups = (key: string): VisibleMenuCardGroup[] => {
  try {
    const cached = localStorage.getItem(key);
    const groups = cached ? JSON.parse(cached) : [];
    return Array.isArray(groups)
      ? groups.filter(
          (group) =>
            typeof group?.moduleName === "string" &&
            typeof group?.cardName === "string" &&
            Array.isArray(group?.menuItems),
        )
      : [];
  } catch {
    return [];
  }
};

const writePermissionCache = (key: string, value: unknown) => {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch {
    // The API response remains usable in Redux when browser storage is full
    // or disabled. Cache persistence must never reject a successful menu load.
    try {
      localStorage.removeItem(key);
    } catch {
      // Storage may be entirely unavailable.
    }
  }
};

const writeNameList = (key: string, names: string[]) => {
  writePermissionCache(key, names);
};

const writeVisibleMenuItems = (key: string, items: VisibleScreenMenuItem[]) => {
  writePermissionCache(key, items);
};

const writeVisibleMenuCardGroups = (key: string, groups: VisibleMenuCardGroup[]) => {
  writePermissionCache(key, groups);
};

const normalizeNameList = (items: any[]) => {
  const names = new Set<string>();
  items.forEach((name) => {
    if (typeof name === "string" && name.trim()) {
      names.add(normalizeMenuName(name));
    }
  });
  return Array.from(names);
};

const normalizeActionRights = (item: any) => {
  const permissions = item?.permissions ?? item?.Permissions ?? {};
  const rawRights = Array.isArray(permissions?.actionRights)
    ? permissions.actionRights
    : Array.isArray(permissions?.ActionRights)
      ? permissions.ActionRights
      : [];

  return rawRights
    .flatMap((right: any) => [
      right?.rightText ?? right?.RightText,
      right?.rightCode ?? right?.RightCode,
      right?.rightId ?? right?.RightId ?? (typeof right === "string" ? right : null),
    ].map((value) => String(value ?? "").trim().toLowerCase()))
    .filter(Boolean);
};

const isPermissionVisible = (item: any) => {
  const permissions = item?.permissions ?? item?.Permissions ?? {};
  return permissions?.isVisible === true || permissions?.IsVisible === true;
};

const getVisibleScreensFromMenuHeaders = (menuHeaders: any[]) => {
  const names = new Set<string>();
  const labels = new Set<string>();
  // Named apart from the per-header `menuItems` below: the inner declaration used to
  // shadow this accumulator, so collected screens were pushed back into the API
  // response and never reached the store.
  const screenMenuItems: VisibleScreenMenuItem[] = [];
  const seenMenuItems = new Set<string>();

  menuHeaders.forEach((header: any) => {
    const menuItems = Array.isArray(header?.menuItems)
      ? header.menuItems
      : Array.isArray(header?.MenuItems)
        ? header.MenuItems
        : [];

    menuItems.filter(isPermissionVisible).forEach((item: any) => {
      const name = item?.menuName ?? item?.MenuName;
      if (typeof name === "string" && name.trim()) {
        const rawModuleName = String(item?.moduleName ?? item?.ModuleName ?? "").trim();
        const trimmedName = mapLibraryPermissionLabel(name.trim(), rawModuleName, "screen");
        const path = String(item?.menuPath ?? item?.MenuPath ?? "").trim();
        const parentName = String(item?.parentMenuName ?? item?.ParentMenuName ?? "").trim();
        const rawMenuCatId = item?.menuCatId ?? item?.MenuCatId ?? null;
        const menuCatId = rawMenuCatId == null ? null : Number(rawMenuCatId);
        const rawMenuId = item?.menuId ?? item?.MenuId ?? null;
        const menuId = rawMenuId == null ? null : Number(rawMenuId);
        const rawReportId = item?.reportId ?? item?.ReportId ?? null;
        const reportId = rawReportId == null ? null : Number(rawReportId);
        const key = `${normalizeMenuName(trimmedName)}|${normalizeMenuName(path)}|${normalizeMenuName(parentName)}`;
        names.add(normalizeMenuName(trimmedName));
        labels.add(trimmedName);
        if (!seenMenuItems.has(key)) {
          seenMenuItems.add(key);
          screenMenuItems.push({
            menuId: Number.isFinite(menuId) ? menuId : null,
            screenKey: String(item?.screenKey ?? item?.ScreenKey ?? item?.controllerId ?? item?.ControllerId ?? "").trim(),
            reportId: Number.isFinite(reportId) ? reportId : null,
            reportCode: getReportCode(item),
            name: trimmedName,
            path,
            parentName,
            menuCatId: Number.isFinite(menuCatId) ? menuCatId : null,
            actionRights: normalizeActionRights(item),
          });
        }
      }
    });
  });

  return {
    names: Array.from(names),
    labels: Array.from(labels),
    menuItems: screenMenuItems,
  };
};

const getVisibleCardGroups = (cardGroups: any[]): VisibleMenuCardGroup[] =>
  cardGroups
    .map((group) => {
      const moduleName = String(group?.moduleName ?? group?.ModuleName ?? "").trim();
      const cardName = mapLibraryPermissionLabel(
        String(group?.cardName ?? group?.CardName ?? "").trim(),
        moduleName,
        "card",
      );
      const rawItems = Array.isArray(group?.menuItems)
        ? group.menuItems
        : Array.isArray(group?.MenuItems)
          ? group.MenuItems
          : [];
      const menuItems = rawItems
        .filter(isPermissionVisible)
        .map((item: any) => ({
          menuId: (() => {
            const rawMenuId = item?.menuId ?? item?.MenuId ?? null;
            const menuId = rawMenuId == null ? null : Number(rawMenuId);
            return Number.isFinite(menuId) ? menuId : null;
          })(),
          screenKey: String(item?.screenKey ?? item?.ScreenKey ?? item?.controllerId ?? item?.ControllerId ?? "").trim(),
          reportId: (() => {
            const rawReportId = item?.reportId ?? item?.ReportId ?? null;
            const reportId = rawReportId == null ? null : Number(rawReportId);
            return Number.isFinite(reportId) ? reportId : null;
          })(),
          reportCode: getReportCode(item),
          name: mapLibraryPermissionLabel(
            String(item?.menuName ?? item?.MenuName ?? "").trim(),
            moduleName,
            "screen",
          ),
          path: String(item?.menuPath ?? item?.MenuPath ?? "").trim(),
          parentName: String(item?.parentMenuName ?? item?.ParentMenuName ?? "").trim(),
          actionRights: normalizeActionRights(item),
          menuCatId: (() => {
            const rawMenuCatId = item?.menuCatId ?? item?.MenuCatId ?? null;
            const menuCatId = rawMenuCatId == null ? null : Number(rawMenuCatId);
            return Number.isFinite(menuCatId) ? menuCatId : null;
          })(),
        }))
        .filter((item: VisibleScreenMenuItem) => item.name);

      return { moduleName, cardName, menuItems };
    })
    .filter((group) => group.moduleName && group.cardName && group.menuItems.length > 0);

const getLegacyLabel = (item: any) =>
  String(item?.label ?? item?.Label ?? item?.menuName ?? item?.MenuName ?? "").trim();

const getLegacyChildren = (item: any) => {
  const children = item?.children ?? item?.Children;
  return Array.isArray(children) ? children : [];
};

const getLegacyActionRights = (item: any) => {
  const actions = item?.actions ?? item?.Actions;
  return Array.isArray(actions)
    ? actions.map((right: any) => String(right).trim().toLowerCase()).filter(Boolean)
    : [];
};

const getLegacyNumber = (item: any, keys: string[]) => {
  for (const key of keys) {
    const value = item?.[key];
    if (value == null || value === "") continue;
    const numeric = Number(value);
    if (Number.isFinite(numeric)) return numeric;
  }
  return null;
};

const getLegacyReportId = (item: any) => {
  const directReportId = getLegacyNumber(item, ["reportid", "reportId", "ReportId"]);
  if (directReportId !== null) return directReportId;

  const reportHeader =
    item?.utreporthdr ??
    item?.utReportHdr ??
    item?.UTReportHdr ??
    item?.reportHeader ??
    item?.ReportHeader;

  if (reportHeader && typeof reportHeader === "object") {
    const headerReportId = getLegacyNumber(reportHeader, [
      "id",
      "Id",
      "PrimaryKeyValue",
      "primaryKeyValue",
      "reportid",
      "reportId",
      "ReportId",
    ]);
    if (headerReportId !== null) return headerReportId;
  }

  return null;
};

const getReportCode = (item: any) => {
  const directCode = String(item?.reportCode ?? item?.ReportCode ?? "").trim();
  if (directCode) return directCode;

  const documentSubTypeAssign =
    item?.utdocsubtypeassign ??
    item?.utDocSubtypeAssign ??
    item?.UTDocSubtypeAssign ??
    item?.docSubtypeAssign ??
    item?.DocSubtypeAssign;

  const reportHeader =
    item?.utreporthdr ??
    item?.utReportHdr ??
    item?.UTReportHdr ??
    item?.reportHeader ??
    item?.ReportHeader ??
    documentSubTypeAssign?.utreporthdr ??
    documentSubTypeAssign?.utReportHdr ??
    documentSubTypeAssign?.UTReportHdr ??
    documentSubTypeAssign?.reportHeader ??
    documentSubTypeAssign?.ReportHeader;

  if (reportHeader && typeof reportHeader === "object") {
    const headerCode = String(
      reportHeader.code ??
        reportHeader.Code ??
        reportHeader.reportcode ??
        reportHeader.Reportcode ??
        reportHeader.ReportCode ??
        "",
    ).trim();
    if (headerCode) return headerCode;
  }

  if (documentSubTypeAssign && typeof documentSubTypeAssign === "object") {
    const assignControllerId = String(
      documentSubTypeAssign.controllerid ??
        documentSubTypeAssign.controllerId ??
        documentSubTypeAssign.ControllerId ??
        "",
    ).trim();
    if (assignControllerId) return assignControllerId;
  }

  return "";
};

const getVisibleCardGroupsFromLegacyData = (legacyData: any[]): VisibleMenuCardGroup[] => {
  const groups = new Map<string, VisibleMenuCardGroup>();

  const addItem = (moduleName: string, cardName: string, item: any, pathParts: string[]) => {
    const name = getLegacyLabel(item);
    const actionRights = getLegacyActionRights(item);
    if (!name || actionRights.length === 0) return;

    const mappedItem: VisibleScreenMenuItem = {
      menuId: getLegacyNumber(item, ["id", "Id"]),
      screenKey: String(item?.controllerId ?? item?.ControllerId ?? "").trim(),
      reportId: getLegacyReportId(item),
      reportCode: getReportCode(item),
      name: mapLibraryPermissionLabel(name, moduleName, "screen"),
      path: pathParts.filter(Boolean).join("/"),
      parentName: cardName,
      menuCatId: getLegacyNumber(item, ["menucatid", "menuCatId", "MenuCatId"]),
      actionRights,
    };

    const groupKey = `${normalizeMenuName(moduleName)}|${normalizeMenuName(cardName)}`;
    const group = groups.get(groupKey) ?? {
      moduleName,
      cardName: mapLibraryPermissionLabel(cardName, moduleName, "card"),
      menuItems: [],
    };
    const itemKey = `${mappedItem.menuId ?? ""}|${normalizeMenuName(mappedItem.name)}|${normalizeMenuName(mappedItem.path)}`;
    if (!group.menuItems.some((existing) => `${existing.menuId ?? ""}|${normalizeMenuName(existing.name)}|${normalizeMenuName(existing.path)}` === itemKey)) {
      group.menuItems.push(mappedItem);
    }
    groups.set(groupKey, group);
  };

  const visit = (moduleName: string, item: any, ancestors: string[]) => {
    const name = getLegacyLabel(item);
    const children = getLegacyChildren(item);
    const nextAncestors = name ? [...ancestors, name] : ancestors;

    if (children.length === 0) {
      const cardName = ancestors.length > 1 ? ancestors[ancestors.length - 1] : moduleName;
      addItem(moduleName, cardName, item, nextAncestors);
      return;
    }

    const itemHasActions = getLegacyActionRights(item).length > 0;
    if (itemHasActions) {
      const cardName = ancestors.length > 1 ? ancestors[ancestors.length - 1] : moduleName;
      addItem(moduleName, cardName, item, nextAncestors);
    }

    children.forEach((child) => visit(moduleName, child, nextAncestors));
  };

  legacyData.forEach((moduleItem) => {
    const moduleName = getLegacyLabel(moduleItem);
    if (!moduleName) return;
    const children = getLegacyChildren(moduleItem);
    if (children.length === 0 && getLegacyActionRights(moduleItem).length > 0) {
      addItem(moduleName, moduleName, moduleItem, [moduleName]);
      return;
    }
    children.forEach((child) => visit(moduleName, child, [moduleName]));
  });

  return Array.from(groups.values()).filter((group) => group.menuItems.length > 0);
};

const mergeVisibleCardGroups = (groups: VisibleMenuCardGroup[]) => {
  const merged = new Map<string, VisibleMenuCardGroup>();

  groups.forEach((group) => {
    const groupKey = `${normalizeMenuName(group.moduleName)}|${normalizeMenuName(group.cardName)}`;
    const existing = merged.get(groupKey) ?? {
      moduleName: group.moduleName,
      cardName: group.cardName,
      menuItems: [],
    };

    group.menuItems.forEach((item) => {
      const itemKey = `${item.menuId ?? ""}|${normalizeMenuName(item.name)}|${normalizeMenuName(item.path)}`;
      const existingItem = existing.menuItems.find((current) =>
        `${current.menuId ?? ""}|${normalizeMenuName(current.name)}|${normalizeMenuName(current.path)}` === itemKey,
      );
      if (!existingItem) {
        existing.menuItems.push(item);
      } else if (!existingItem.screenKey && item.screenKey) {
        // A cached/full response can omit the new field while the legacy
        // response still contains controllerId. Preserve that stable key.
        existingItem.screenKey = item.screenKey;
      }
    });

    merged.set(groupKey, existing);
  });

  return Array.from(merged.values()).filter((group) => group.menuItems.length > 0);
};

const mapPermissionItem = (item: any, moduleName: string): VisibleScreenMenuItem | null => {
  const name = mapLibraryPermissionLabel(
    String(item?.menuName ?? item?.MenuName ?? "").trim(),
    moduleName,
    "screen",
  );
  if (!name) return null;

  const rawMenuId = item?.menuId ?? item?.MenuId ?? null;
  const menuId = rawMenuId == null ? null : Number(rawMenuId);
  const rawReportId = item?.reportId ?? item?.ReportId ?? null;
  const reportId = rawReportId == null ? null : Number(rawReportId);
  const rawMenuCatId = item?.menuCatId ?? item?.MenuCatId ?? null;
  const menuCatId = rawMenuCatId == null ? null : Number(rawMenuCatId);

  return {
    menuId: Number.isFinite(menuId) ? menuId : null,
    screenKey: String(item?.screenKey ?? item?.ScreenKey ?? item?.controllerId ?? item?.ControllerId ?? "").trim(),
    reportId: Number.isFinite(reportId) ? reportId : null,
    reportCode: getReportCode(item),
    name,
    path: String(item?.menuPath ?? item?.MenuPath ?? "").trim(),
    parentName: String(item?.parentMenuName ?? item?.ParentMenuName ?? "").trim(),
    menuCatId: Number.isFinite(menuCatId) ? menuCatId : null,
    actionRights: normalizeActionRights(item),
  };
};

const getActionRightMenuItems = (menuHeaders: any[], cardGroups: any[]): VisibleScreenMenuItem[] => {
  const items: VisibleScreenMenuItem[] = [];
  const seen = new Set<string>();

  const addItem = (item: any, moduleName: string) => {
    const mappedItem = mapPermissionItem(item, moduleName);
    if (!mappedItem || mappedItem.actionRights.length === 0) return;

    const key = `${normalizeMenuName(mappedItem.name)}|${normalizeMenuName(mappedItem.path)}|${normalizeMenuName(mappedItem.parentName)}`;
    if (seen.has(key)) return;

    seen.add(key);
    items.push(mappedItem);
  };

  menuHeaders.forEach((header: any) => {
    const headerItems = Array.isArray(header?.menuItems)
      ? header.menuItems
      : Array.isArray(header?.MenuItems)
        ? header.MenuItems
        : [];

    headerItems.forEach((item: any) => {
      addItem(item, String(item?.moduleName ?? item?.ModuleName ?? "").trim());
    });
  });

  cardGroups.forEach((group: any) => {
    const moduleName = String(group?.moduleName ?? group?.ModuleName ?? "").trim();
    const groupItems = Array.isArray(group?.menuItems)
      ? group.menuItems
      : Array.isArray(group?.MenuItems)
        ? group.MenuItems
        : [];

    groupItems.forEach((item: any) => addItem(item, moduleName));
  });

  return items;
};

export const hydrateMenuPermissionsFromCache = createAsyncThunk(
  "menuPermissions/hydrateFromCache",
  async (userId: number) => {
    // Menus are per-user and only change when an administrator edits the role,
    // so the last successful response is replayed from localStorage on reload.
    // Redux itself does not survive a refresh; without this the menu API was
    // re-fetched on every page load.
    const hiddenMenuNames = readNameList(getHiddenMenusCacheKey(userId));
    const hiddenScreenNames = readNameList(getHiddenScreensCacheKey(userId));
    const visibleScreenNames = readNameList(getVisibleScreensCacheKey(userId));
    const visibleScreenLabels = readNameList(getVisibleScreenLabelsCacheKey(userId));
    const visibleScreenMenuItems = readVisibleMenuItems(getVisibleScreenMenuItemsCacheKey(userId));
    const visibleMenuCardGroups = readVisibleMenuCardGroups(getVisibleMenuCardGroupsCacheKey(userId));
    const actionRightMenuItems = readVisibleMenuItems(getActionRightMenuItemsCacheKey(userId));

    // The sidebar is driven by the card groups and the screen list, so a cache
    // only counts as usable when those actually carry entries. A hidden-name
    // list can legitimately be empty and is not evidence of a stored response.
    const hasMenuCache = visibleMenuCardGroups.length > 0;
    let hasActionRightsCache = false;
    try {
      hasActionRightsCache = Array.isArray(JSON.parse(localStorage.getItem(getActionRightMenuItemsCacheKey(userId)) ?? "null"));
    } catch {
      // A missing or invalid action-right cache must be reloaded from the API.
    }
    const hasScreenCache = visibleScreenNames.length > 0 && hasActionRightsCache;

    return {
      userId,
      hiddenMenuNames,
      hiddenScreenNames,
      visibleScreenNames,
      visibleScreenLabels,
      visibleScreenMenuItems,
      visibleMenuCardGroups,
      actionRightMenuItems,
      hasMenuCache,
      hasScreenCache,
    };
  },
);

export const fetchMenuPermissions = createAsyncThunk(
  "menuPermissions/fetch",
  async (userId: number) => {
    const [menuResponse, reportResponse] = await Promise.all([
      api.get("/api/auth/utmenu/v3/menu/null/24"),
      api.get("/api/auth/utmenu/v3/menu/null/25"),
    ]);
    const hiddenMenuNames = normalizeNameList(
      Array.isArray(menuResponse.data?.hiddenMenuNames) ? menuResponse.data.hiddenMenuNames : [],
    );
    const hiddenScreenNames = normalizeNameList(
      Array.isArray(menuResponse.data?.hiddenScreenNames) ? menuResponse.data.hiddenScreenNames : [],
    );
    const menuHeaders = Array.isArray(menuResponse.data?.menuHeaders) ? menuResponse.data.menuHeaders : [];
    const cardGroups = Array.isArray(menuResponse.data?.cardGroups) ? menuResponse.data.cardGroups : [];
    const legacyData = Array.isArray(menuResponse.data?.Data)
      ? menuResponse.data.Data
      : Array.isArray(menuResponse.data?.data)
        ? menuResponse.data.data
        : [];
    const legacyReportData = Array.isArray(reportResponse.data?.Data)
      ? reportResponse.data.Data
      : Array.isArray(reportResponse.data?.data)
        ? reportResponse.data.data
        : [];
    // This endpoint is the legacy UT menu endpoint. Its server tree has already
    // applied the role-detail/menu/controller authorization rule; a leaf is
    // visible when it has any granted action, not only View. Use its actions as
    // the source of truth when no full-response card groups were returned.
    const hasModernMenuCardGroups = Array.isArray(menuResponse.data?.cardGroups) ||
      Array.isArray(menuResponse.data?.CardGroups);
    const legacyMenuCardGroups = hasModernMenuCardGroups
      ? []
      : getVisibleCardGroupsFromLegacyData(legacyData);
    const legacyReportCardGroups = getVisibleCardGroupsFromLegacyData(legacyReportData);
    const visibleScreens = getVisibleScreensFromMenuHeaders(menuHeaders);
    const legacyScreenItems = [
      ...legacyMenuCardGroups.flatMap((group) => group.menuItems),
      ...legacyReportCardGroups.flatMap((group) => group.menuItems),
    ];
    const visibleScreenNames = Array.from(new Set([
      ...visibleScreens.names,
      ...legacyScreenItems.map((item) => normalizeMenuName(item.name)),
    ]));
    const visibleScreenLabels = Array.from(new Set([
      ...visibleScreens.labels,
      ...legacyScreenItems.map((item) => item.name),
    ]));
    const visibleScreenMenuItems = [
      ...visibleScreens.menuItems,
      ...legacyScreenItems,
    ];
    const visibleMenuCardGroups = mergeVisibleCardGroups([
      ...getVisibleCardGroups(cardGroups),
      ...legacyMenuCardGroups,
      ...legacyReportCardGroups,
    ]);
    const actionRightMenuItems = [
      ...getActionRightMenuItems(menuHeaders, cardGroups),
      ...visibleMenuCardGroups.flatMap((group) => group.menuItems.filter((item) => item.actionRights.length > 0)),
    ];

    writeNameList(getHiddenMenusCacheKey(userId), hiddenMenuNames);
    writeNameList(getHiddenScreensCacheKey(userId), hiddenScreenNames);
    writeNameList(getVisibleScreensCacheKey(userId), visibleScreenNames);
    writeNameList(getVisibleScreenLabelsCacheKey(userId), visibleScreenLabels);
    writeVisibleMenuItems(getVisibleScreenMenuItemsCacheKey(userId), visibleScreenMenuItems);
    writeVisibleMenuCardGroups(getVisibleMenuCardGroupsCacheKey(userId), visibleMenuCardGroups);
    writeVisibleMenuItems(getActionRightMenuItemsCacheKey(userId), actionRightMenuItems);

    return {
      userId,
      hiddenMenuNames,
      hiddenScreenNames,
      visibleScreenNames,
      visibleScreenLabels,
      visibleScreenMenuItems,
      visibleMenuCardGroups,
      actionRightMenuItems,
    };
  },
  {
    condition: (userId, { getState }) => {
      const state = getState() as RootState;
      const permissions = state.menuPermissions;
      if (permissions.status === "loading") return false;
      return (
        permissions.userId !== userId ||
        !permissions.menuPermissionsLoaded ||
        !permissions.screenPermissionsLoaded
      );
    },
  },
);

const initialState: MenuPermissionsState = {
  userId: null,
  hiddenMenuNames: [],
  hiddenScreenNames: [],
  visibleScreenNames: [],
  visibleScreenLabels: [],
  visibleScreenMenuItems: [],
  visibleMenuCardGroups: [],
  actionRightMenuItems: [],
  menuPermissionsLoaded: false,
  screenPermissionsLoaded: false,
  status: "idle",
  error: null,
};

const menuPermissionsSlice = createSlice({
  name: "menuPermissions",
  initialState,
  reducers: {
    clearMenuPermissions: () => initialState,
    markMenuPermissionsLoadedWithoutUser: (state) => {
      state.userId = null;
      state.hiddenMenuNames = [];
      state.hiddenScreenNames = [];
      state.visibleScreenNames = [];
      state.visibleScreenLabels = [];
      state.visibleScreenMenuItems = [];
      state.visibleMenuCardGroups = [];
      state.actionRightMenuItems = [];
      state.menuPermissionsLoaded = true;
      state.screenPermissionsLoaded = true;
      state.status = "succeeded";
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(hydrateMenuPermissionsFromCache.fulfilled, (state, action) => {
        state.userId = action.payload.userId;
        state.hiddenMenuNames = action.payload.hiddenMenuNames;
        state.hiddenScreenNames = action.payload.hiddenScreenNames;
        state.visibleScreenNames = action.payload.visibleScreenNames;
        state.visibleScreenLabels = action.payload.visibleScreenLabels;
        state.visibleScreenMenuItems = action.payload.visibleScreenMenuItems;
        state.visibleMenuCardGroups = action.payload.visibleMenuCardGroups;
        state.actionRightMenuItems = action.payload.actionRightMenuItems;
        state.menuPermissionsLoaded = action.payload.hasMenuCache;
        state.screenPermissionsLoaded = action.payload.hasScreenCache;
        if (state.status !== "loading") {
          state.status = "idle";
        }
        state.error = null;
      })
      .addCase(fetchMenuPermissions.pending, (state, action) => {
        state.userId = action.meta.arg;
        state.status = "loading";
        state.error = null;
      })
      .addCase(fetchMenuPermissions.fulfilled, (state, action: PayloadAction<{
        userId: number;
        hiddenMenuNames: string[];
        hiddenScreenNames: string[];
        visibleScreenNames: string[];
        visibleScreenLabels: string[];
        visibleScreenMenuItems: VisibleScreenMenuItem[];
        visibleMenuCardGroups: VisibleMenuCardGroup[];
        actionRightMenuItems: VisibleScreenMenuItem[];
      }>) => {
        state.userId = action.payload.userId;
        state.hiddenMenuNames = action.payload.hiddenMenuNames;
        state.hiddenScreenNames = action.payload.hiddenScreenNames;
        state.visibleScreenNames = action.payload.visibleScreenNames;
        state.visibleScreenLabels = action.payload.visibleScreenLabels;
        state.visibleScreenMenuItems = action.payload.visibleScreenMenuItems;
        state.visibleMenuCardGroups = action.payload.visibleMenuCardGroups;
        state.actionRightMenuItems = action.payload.actionRightMenuItems;
        state.menuPermissionsLoaded = true;
        state.screenPermissionsLoaded = true;
        state.status = "succeeded";
        state.error = null;
      })
      .addCase(fetchMenuPermissions.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message ?? "Failed to load menu permissions";
      });
  },
});

export const { clearMenuPermissions, markMenuPermissionsLoadedWithoutUser } = menuPermissionsSlice.actions;
export default menuPermissionsSlice.reducer;
