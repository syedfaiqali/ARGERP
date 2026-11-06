import { GetService, PostService } from "@/constants/globalItems";
import { regionalScope, setRegionalSettings, getRegionalSettings, type RegionalSettings } from "@/utils/regionalSettings";
const BASE = "/api/Administration/RegionalConfiguration/v3";
const unwrap = (response: any) => {
  const body = response?.data ?? response;
  if (body?.isSuccessCall !== true && body?.IsSuccessCall !== true) throw new Error(body?.message || body?.Message || "Regional service unavailable");
  return body?.data ?? body?.Data ?? null;
};
const pending = new Map<string, Promise<RegionalSettings | null>>();
const versions = new Map<string, number>();
export const refreshRegionalSettings = (force = false): Promise<RegionalSettings | null> => {
  const scope = regionalScope();
  if (!scope) return Promise.resolve(null);
  const current = pending.get(scope);
  if (current && !force) return current;
  const version = (versions.get(scope) || 0) + 1;
  versions.set(scope, version);
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000);
  const request = GetService(force ? `${BASE}?refresh=${Date.now()}` : BASE, undefined, undefined, controller.signal).then(response => {
    const value = unwrap(response) as RegionalSettings | null;
    if (versions.get(scope) === version) setRegionalSettings(value, scope);
    return value;
  }).catch(() => scope === regionalScope() ? getRegionalSettings() : null)
    .finally(() => { clearTimeout(timeout); if (pending.get(scope) === request) pending.delete(scope); });
  pending.set(scope, request); return request;
};
export type CurrencyReference = { code: string; name: string; symbol: string };
export type CountryReference = { code: string; name: string; currencyCode: string; currencyName: string; currencySymbol: string; timeZones: string[] };
export const getCountryReferences = async (): Promise<CountryReference[]> => unwrap(await GetService(`${BASE}/countries`)) || [];
export const resetCountryCurrency = async () => {
  unwrap(await PostService(`${BASE}/currency/reset`, {}));
  return refreshRegionalSettings(true);
};
export const getCurrencyReferences = async (): Promise<CurrencyReference[]> => unwrap(await GetService(`${BASE}/currencies`)) || [];
export const syncRegionalLocations = async (countryId?: number, stateId?: number) => {
  let response: any;
  try {
    response = await PostService(`${BASE}/sync`, { countryId: countryId || null, stateId: stateId || null });
  } catch (error: any) {
    // A 503 carries the server's reason (tables missing, no internet, folder not writable).
    const body = error?.response?.data;
    throw new Error(body?.message || body?.Message || "Regional service unavailable");
  }
  unwrap(response);
};
export type RegionalRate = { baseCode: string; quoteCode: string; rate: number | null; rateDate: string | null; source: string; stale: boolean; available: boolean };
export const getExchangeRate = async (quote: string, date?: string): Promise<RegionalRate> =>
  unwrap(await GetService(`${BASE}/rate?quote=${encodeURIComponent(quote)}${date ? `&date=${encodeURIComponent(date)}` : ""}`));
export const getRegionalReference = async (countryId: number, stateId?: number, cityId?: number): Promise<RegionalSettings | null> =>
  unwrap(await GetService(`${BASE}/reference?countryId=${countryId}${stateId ? `&stateId=${stateId}` : ""}${cityId ? `&cityId=${cityId}` : ""}`));

export const getReferenceStates = async (countryCode: string): Promise<{id:number;name:string}[]> => unwrap(await GetService(BASE + "/states?countryCode=" + encodeURIComponent(countryCode))) || [];
export const getReferenceCities = async (countryCode: string,stateId:number): Promise<{id:number;name:string}[]> => unwrap(await GetService(BASE + "/cities?countryCode=" + encodeURIComponent(countryCode) + "&stateId=" + stateId)) || [];
