// Keep cache identity aligned with the client/entity headers in services/api.ts.
// Client selection can change without rewriting the stored login user.
export function getCrudScopeKey(storage: Pick<Storage, "getItem"> = localStorage): string {
  const readJson = (key: string): Record<string, unknown> => {
    try { return JSON.parse(storage.getItem(key) || "{}") ?? {}; }
    catch { return {}; }
  };
  const first = (sources: Record<string, unknown>[], keys: string[]): string => {
    for (const source of sources) {
      for (const key of keys) {
        const value = String(source[key] ?? "");
        if (/^\d+$/.test(value) && Number(value) > 0) return value;
      }
    }
    return "";
  };
  let token: Record<string, unknown> = {};
  try {
    const payload = (storage.getItem("accessToken") || "").split(".")[1];
    if (payload) {
      const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
      token = JSON.parse(atob(normalized + "=".repeat((4 - normalized.length % 4) % 4)));
    }
  } catch { /* A missing/invalid token has no scope. */ }
  const user = readJson("user");
  const localKeys = ["ClientId", "selectedClientId", "clientId", "clientid", "selectedCampusId", "selectedEntityId", "selectedEntity", "selectedEntityID", "entityId", "EntityId"];
  const local = Object.fromEntries(localKeys.map(key => [key, storage.getItem(key)]));
  const client = first([local], ["ClientId", "selectedClientId", "clientId", "clientid"])
    || first([user, token], ["clientId", "ClientId", "clientID", "clientid", "client_id", "client"]);
  const entity = first([local], ["selectedCampusId", "selectedEntityId", "selectedEntity", "selectedEntityID", "entityId", "EntityId"])
    || first([user, token], ["entityId", "EntityId", "entityID", "entityid", "entity_id", "entity"])
    || first([user], ["selectedCampusId", "campusId", "CampusId"]);
  return `${client}|${entity}`;
}
