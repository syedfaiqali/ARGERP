export type RegionalSettings = {
  countryId: number; countryCode: string; currencyCode: string; currencyName: string;
  currencySymbol: string; timeZone: string | null; currencyOverridden: boolean; timeZones: string[];
  decimalPlaces?: number;
};
export const REGIONAL_CHANGED = "regional-settings-changed";

// Session cache is scoped by both client and entity; switching scope cannot reuse another tenant's defaults.
export const regionalScope = () => {
  try {
    const user = JSON.parse(localStorage.getItem("user") || "{}");
    const client = localStorage.getItem("ClientId") || localStorage.getItem("selectedClientId") ||
      localStorage.getItem("clientId") || localStorage.getItem("clientid") || user.clientId || user.ClientId || user.clientid;
    const entity = localStorage.getItem("selectedCampusId") || localStorage.getItem("selectedEntity") || localStorage.getItem("selectedEntityId") || user.entityId || user.EntityId;
    return Number(client) > 0 && Number(entity) > 0 ? `${client}:${entity}` : "";
  } catch { return ""; }
};

export const getRegionalSettings = (): RegionalSettings | null => {
  try {
    const scope = regionalScope();
    if (!scope) return null;
    return JSON.parse(sessionStorage.getItem(`regional:${scope}`) || "null");
  } catch { return null; }
};

export const setRegionalSettings = (value: RegionalSettings | null, scope = regionalScope()) => {
  if (!scope || scope !== regionalScope()) return;
  try {
    if (value) sessionStorage.setItem(`regional:${scope}`, JSON.stringify(value));
    else sessionStorage.removeItem(`regional:${scope}`);
  } catch { /* Storage restrictions must not stop the app. */ }
  window.dispatchEvent(new Event(REGIONAL_CHANGED));
};

export const getDefaultCurrencyCode = () => getRegionalSettings()?.currencyCode || "PKR";
export const getDefaultCurrencySymbol = () => getRegionalSettings()?.currencySymbol || getDefaultCurrencyCode();
export const getCurrencyDecimalPlaces = () => {
  const places = getRegionalSettings()?.decimalPlaces;
  return typeof places === "number" && Number.isInteger(places) && places >= 0 && places <= 6 ? places : 2;
};
export const formatRegionalAmount = (value: number, places = getCurrencyDecimalPlaces(), grouping = true) => {
  const safePlaces = Number.isFinite(places) ? Math.min(100, Math.max(0, Math.trunc(places))) : 2;
  const fixed = (Number.isFinite(value) ? value : 0).toFixed(safePlaces);
  if (!grouping) return fixed;
  const [whole, fraction] = fixed.split(".");
  const formatted = whole.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  return fraction == null ? formatted : `${formatted}.${fraction}`;
};
export const getDefaultTimeZone = () => getRegionalSettings()?.timeZone || Intl.DateTimeFormat().resolvedOptions().timeZone;

export const getPolicyTimezoneOffset = (date = new Date()): string => {
  const zone = getRegionalSettings()?.timeZone;
  if (!zone) return String(date.getTimezoneOffset());
  try {
    const parts = new Intl.DateTimeFormat("en-US", { timeZone: zone, year: "numeric", month: "2-digit",
      day: "2-digit", hour: "2-digit", minute: "2-digit", second: "2-digit", hourCycle: "h23" }).formatToParts(date);
    const part = (type: string) => Number(parts.find(p => p.type === type)?.value);
    const wall = Date.UTC(part("year"), part("month") - 1, part("day"), part("hour"), part("minute"), part("second"));
    return String(Math.round((date.getTime() - wall) / 60_000));
  } catch { return String(date.getTimezoneOffset()); }
};

// Calendar date ("yyyy-MM-dd") in the campus' policy timezone. Use this for "today" instead of
// `new Date().toISOString().slice(0, 10)`, which is the UTC date (yesterday in Pakistan before 5 AM).
export const getPolicyDateString = (date = new Date()): string => {
  try {
    const parts = new Intl.DateTimeFormat("en-US", { timeZone: getDefaultTimeZone(), year: "numeric", month: "2-digit", day: "2-digit" }).formatToParts(date);
    const part = (type: string) => parts.find(p => p.type === type)?.value;
    return `${part("year")}-${part("month")}-${part("day")}`;
  } catch {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
  }
};
