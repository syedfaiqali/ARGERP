import { GetService } from '@/constants/globalItems';

// A single keyword swap, e.g. { from: "Teacher", to: { en: "Caretaker", ur: "نگران", ... } }.
// `to` is keyed by language code (en, ur, ar, fa, ps); `en` is always present.
export interface TerminologyMapping {
    from: string;
    to: Record<string, string>;
}

// Normalized institution-type key (e.g. "daycare") -> its keyword swaps.
export type TerminologyMap = Record<string, TerminologyMapping[]>;

const ENDPOINT = '/api/organization/InstitutionTerminology/v3';

// Cache the resolved map so it is fetched at most once per session — but only once we
// have a *valid* payload. GetService treats HTTP 404 as a resolved response (not a
// rejection), so a 404 (e.g. before the backend endpoint is deployed) must NOT be cached,
// otherwise the empty result would stick for the whole session.
let cached: Promise<TerminologyMap> | null = null;

const isValidMap = (data: unknown): data is TerminologyMap =>
    !!data && typeof data === 'object' && !Array.isArray(data) && Object.keys(data as object).length > 0;

export function getInstitutionTerminology(): Promise<TerminologyMap> {
    if (!cached) {
        cached = GetService(ENDPOINT)
            .then((res: any) => {
                const status = res?.status;
                const data = res?.data;
                const ok = (status === undefined || (status >= 200 && status < 300)) && isValidMap(data);
                if (ok) return data as TerminologyMap;
                cached = null; // 404 / empty / error body -> allow a later retry
                return {} as TerminologyMap;
            })
            .catch(() => {
                cached = null;
                return {} as TerminologyMap;
            });
    }
    return cached;
}
