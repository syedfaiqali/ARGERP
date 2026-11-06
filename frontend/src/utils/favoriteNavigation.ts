export function resolveFavoriteRoute(favorite:any,items:any[],_groups?:any[]) {
 const match=items.find(item=>favorite.menuId && Number(item.menuId)===Number(favorite.menuId)) || items.find(item=>item.name===favorite.name);
 return match?.path || null;
}
