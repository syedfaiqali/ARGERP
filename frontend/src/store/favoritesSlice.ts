import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import type { RootState } from './store';

export interface FavoriteItem {
  favoriteId?: number;
  menuId?: number | null;
  mMenuId?: number | null;
  name: string;
  path: string;
  icon?: string;
  category?: string;
}

interface FavoritesState {
  items: FavoriteItem[];
  pendingToggleCount: number;
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  loadedForUserId: number | null;
  error: string | null;
}

const decodeJwtPayload = (token: string | null): any => {
  if (!token) return null;
  try {
    const payload = token.split('.')[1];
    if (!payload) return null;
    const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
    const paddedPayload =
      normalizedPayload + '='.repeat((4 - (normalizedPayload.length % 4)) % 4);
    return JSON.parse(atob(paddedPayload));
  } catch {
    return null;
  }
};

const resolveCurrentUserId = (state: RootState) => {
  const user = state.auth.user as any;
  const tokenPayload = decodeJwtPayload(localStorage.getItem('accessToken'));
  const rawUserId =
    user?.userid ??
    user?.userId ??
    user?.UserId ??
    user?.userID ??
    tokenPayload?.userid ??
    tokenPayload?.userId ??
    tokenPayload?.UserId ??
    tokenPayload?.nameid ??
    tokenPayload?.sub;

  const userId = Number(rawUserId);
  return Number.isFinite(userId) && userId > 0 ? userId : null;
};

const normalizeRouteKey = (value: string) =>
  value
    .trim()
    .toLowerCase()
    .replace(/&/g, 'and')
    .replace(/[^a-z0-9]+/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();

const getFavoriteViewName = (path: string | undefined) => {
  if (!path) return "";

  const [, queryString = ""] = path.split("?");
  const params = new URLSearchParams(queryString);
  return params.get("view") ?? "";
};

const resolveFavoriteMenuId = (state: RootState, item: FavoriteItem) => {
  const rawMenuId = Number(item.menuId);
  if (Number.isFinite(rawMenuId) && rawMenuId > 0) return rawMenuId;

  const normalizedName = normalizeRouteKey(item.name);
  const normalizedView = normalizeRouteKey(getFavoriteViewName(item.path));
  const normalizedCategory = normalizeRouteKey(item.category ?? "");

  const matchedItem = state.menuPermissions.visibleScreenMenuItems.find((menuItem) => {
    const menuName = normalizeRouteKey(menuItem.name);
    if (menuName !== normalizedName && menuName !== normalizedView) return false;
    if (!normalizedCategory) return true;

    const pathParts = menuItem.path
      .split("/")
      .map((part) => normalizeRouteKey(part));

    return (
      normalizeRouteKey(menuItem.parentName) === normalizedCategory ||
      pathParts.includes(normalizedCategory)
    );
  }) ?? state.menuPermissions.visibleScreenMenuItems.find((menuItem) =>
    normalizeRouteKey(menuItem.name) === normalizedName ||
    normalizeRouteKey(menuItem.name) === normalizedView
  );

  const resolvedMenuId = Number(matchedItem?.menuId);
  return Number.isFinite(resolvedMenuId) && resolvedMenuId > 0 ? resolvedMenuId : null;
};

const normalizeFavorite = (item: any): FavoriteItem => {
  const name = String(item?.name ?? item?.Name ?? '').trim();
  const menuPath = String(item?.path ?? item?.Path ?? '').trim();
  const category = String(item?.category ?? item?.Category ?? 'General').trim() || 'General';

  return {
    favoriteId: Number(item?.favoriteId ?? item?.FavoriteId) || undefined,
    menuId: item?.menuId ?? item?.MenuId ?? null,
    mMenuId: item?.mMenuId ?? item?.MMenuId ?? null,
    name,
    // Resolve the intact server menu path against current permissions when opening.
    path: menuPath,
    category,
  };
};

const writeCache = (items: FavoriteItem[]) => {
  localStorage.setItem('sidebar_favorites', JSON.stringify(items));
};

const initialState: FavoritesState = {
  items: JSON.parse(localStorage.getItem('sidebar_favorites') || '[]'),
  pendingToggleCount: 0,
  status: 'idle',
  loadedForUserId: null,
  error: null,
};

export const fetchFavorites = createAsyncThunk(
  'favorites/fetch',
  async (_, { getState }) => {
    const state = getState() as RootState;
    const userId = resolveCurrentUserId(state);
    if (!userId) return { userId: null, items: [] as FavoriteItem[] };

    const items = state.favorites.items;

    return { userId, items };
  },
  {
    condition: (_, { getState }) => {
      const state = getState() as RootState;
      const userId = resolveCurrentUserId(state);
      const favorites = state.favorites;
      return Boolean(userId) && !(favorites.status === 'loading' || favorites.loadedForUserId === userId);
    },
  },
);

export const toggleFavorite = createAsyncThunk(
  'favorites/toggle',
  async (item: FavoriteItem, { getState }) => {
    const state = getState() as RootState;
    const userId = resolveCurrentUserId(state);
    const menuId = resolveFavoriteMenuId(state, item);
    if (!userId) {
      throw new Error('User is required to update favorites.');
    }

    const exists = state.favorites.items.some(favorite => favorite.path === item.path);
    const items = exists ? state.favorites.items.filter(favorite => favorite.path !== item.path) : [...state.favorites.items, { ...item, menuId }];

    return { userId, items };
  },
);

const favoritesSlice = createSlice({
  name: 'favorites',
  initialState,
  reducers: {
    removeFavorite: (state, action) => {
      state.items = state.items.filter(item => item.path !== action.payload);
      writeCache(state.items);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchFavorites.pending, (state) => {
        state.status = 'loading';
        state.error = null;
      })
      .addCase(fetchFavorites.fulfilled, (state, action) => {
        state.items = action.payload.items;
        state.loadedForUserId = action.payload.userId;
        state.status = 'succeeded';
        state.error = null;
        writeCache(state.items);
      })
      .addCase(fetchFavorites.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message ?? 'Failed to load favorites';
      })
      .addCase(toggleFavorite.pending, (state) => {
        state.pendingToggleCount += 1;
        state.error = null;
      })
      .addCase(toggleFavorite.fulfilled, (state, action) => {
        state.pendingToggleCount = Math.max(0, state.pendingToggleCount - 1);
        state.items = action.payload.items;
        state.loadedForUserId = action.payload.userId;
        state.status = 'succeeded';
        state.error = null;
        writeCache(state.items);
      })
      .addCase(toggleFavorite.rejected, (state, action) => {
        state.pendingToggleCount = Math.max(0, state.pendingToggleCount - 1);
        state.status = 'failed';
        state.error = action.error.message ?? 'Failed to update favorites';
      });
  },
});

export const { removeFavorite } = favoritesSlice.actions;
export default favoritesSlice.reducer;
