import { getPolicyTimezoneOffset, getCurrencyDecimalPlaces, formatRegionalAmount } from "@/utils/regionalSettings";
import api from '@/services/api';
import dayjs from 'dayjs';

const GET_CACHE_TTL_MS = 60 * 1000;
const getResponseCache = new Map<string, { expiresAt: number; response: any }>();
const inFlightGetRequests = new Map<string, Promise<any>>();

export const invalidateGetCache = () => {
    getResponseCache.clear();
    inFlightGetRequests.clear();
};

export interface Campus {
    id: number;
    /** Institution row ID retained only to translate legacy saved selections. */
    institutionId?: number;
    name: string;
    address: string;
    entityType?: string;
    entityTypeId?: number;
    institutionType?: string;
    institutionTypeId?: number;
    imageId?: number | null;
    imageUrl?: string;
}

const toImageUrl = (imageId: number) => imageId > 0 ? `/api/Student/v3/image/${imageId}` : "";

export const mapInstitutionToCampus = (item: any): Campus => {
    const imageId = Number(item?.imageId ?? item?.ImageId ?? item?.imageid ?? item?.ImageID ?? 0);
    // Institution `Id` identifies the organization record.  Course extensions
    // are scoped by the Campus EntityId, which is a different value for campus
    // records such as Gulshan.  Sending the institution ID incorrectly creates
    // the extension under another entity.
    const institutionId = Number(item?.id ?? item?.Id ?? 0);
    const entityId = Number(item?.entityId ?? item?.EntityId ?? 0);
    return {
        id: entityId > 0 ? entityId : institutionId,
        institutionId: institutionId > 0 ? institutionId : undefined,
        name:
            item?.instituteName ??
            item?.InstituteName ??
            item?.stxt ??
            item?.STXT ??
            item?.name ??
            item?.Name ??
            "Unknown Campus",
        address: item?.address ?? item?.Address ?? "",
        entityType: item?.entityType ?? item?.EntityType ?? "",
        entityTypeId: Number(item?.entityTypeId ?? item?.EntityTypeId ?? 0),
        institutionType: item?.institutionType ?? item?.InstitutionType ?? "",
        institutionTypeId: Number(item?.institutionTypeId ?? item?.InstitutionTypeId ?? 0),
        imageId: Number.isFinite(imageId) && imageId > 0 ? imageId : null,
        imageUrl: item?.imageUrl ?? item?.ImageUrl ?? item?.imageurl ?? item?.ImageURL ?? toImageUrl(imageId),
    };
};

export const fetchGlobalCampuses = async (): Promise<Campus[]> => {
    try {
        const response: any = await GetService(
            "/api/organization/Institution/v3?pageNumber=1&pageSize=1000&lightweight=true",
        );
        const payload = response.data ?? response ?? {};
        const items = payload.items ?? payload.Items ?? [];

        if (!ToCheckArrayTypeAndLength(items)) return [];

        return items
            .map(mapInstitutionToCampus)
            .filter((campus: Campus) => Number.isFinite(campus.id) && campus.id > 0);
    } catch (e) {
        console.error("Failed to fetch global campuses", e);
        return [];
    }
};

/*
  getToken()
  Purpose: Retrieve authentication token from parameter or localStorage
  Parameters: token (optional)
  Returns: string
*/
export const getToken = (token?: string): string => {
    if (token) return token;
    return localStorage.getItem('accessToken') || '';
};

const shouldUseCampusAsEntityScope = (method?: string) => {
    const normalizedMethod = (method || '').toLowerCase();
    return normalizedMethod.includes('/api/humanresource/employeeprofile/v3');
};

// Chart of Accounts is client-wide; it must not inherit the selected entity.
const shouldExcludeEntityScope = (method?: string) =>
    (method || '').toLowerCase().includes('/api/financialmanagement/chartofaccount/v3');

const getScopeHeaders = (method?: string) => {
    const headers: Record<string, string> = {};
    const excludeEntityScope = shouldExcludeEntityScope(method);
    const selectedCampusId = localStorage.getItem('selectedCampusId') || '';
    // Legacy dashboards are entity-scoped. A campus ID is only a fallback when no entity scope exists.
    const selectedEntity =
        localStorage.getItem('selectedEntity') ||
        localStorage.getItem('selectedEntityId') ||
        localStorage.getItem('selectedEntityID') ||
        selectedCampusId ||
        '';
    const entityScopeOverride = shouldUseCampusAsEntityScope(method) ? selectedCampusId : '';
    const userStr = localStorage.getItem('user');
    if (userStr) {
        try {
            const user = JSON.parse(userStr);
            const localClientId =
                localStorage.getItem('ClientId') ||
                localStorage.getItem('selectedClientId') ||
                localStorage.getItem('clientId') ||
                localStorage.getItem('clientid') ||
                '';
            const clientId =
                localClientId || user?.clientId || user?.ClientId || user?.clientID || user?.clientid;
            if (clientId != null && clientId !== '') {
                headers['X-Client-Id'] = clientId.toString();
                headers['Client-Id'] = clientId.toString();
            }

            const userEntityId =
                user?.entityId ?? user?.EntityId ?? user?.entityID ?? user?.entityid;
            const entityId = entityScopeOverride || selectedEntity || userEntityId;
            if (!excludeEntityScope && entityId != null && entityId !== '' && entityId !== 'null' && entityId !== 'undefined') {
                headers['X-Entity-Id'] = entityId.toString();
                headers['Entity-Id'] = entityId.toString();
            }

            const employeeId =
                user?.empId ?? user?.EmpId ?? user?.empid ?? user?.employeeId ?? user?.EmployeeId ?? user?.employeeid;
            if (employeeId != null && employeeId !== '' && employeeId !== 'null' && employeeId !== 'undefined') {
                headers['X-Employee-Id'] = employeeId.toString();
                headers['Employee-Id'] = employeeId.toString();
            }

            // Student-scoped endpoints must not infer the learner from EmpId: for staff that
            // value is the employee record. Send only an explicitly linked/selected student.
            const roleText = [user?.displayRole, user?.roleName, user?.role, ...(Array.isArray(user?.roles) ? user.roles : [])]
                .filter(Boolean)
                .join(' ')
                .toLowerCase();
            const isStudentAccount = roleText.includes('student') || roleText.includes('learner')
                || user?.userCatId === 11 || user?.userCatId === 12;
            const studentId =
                user?.studentId ?? user?.StudentId ?? user?.studentid
                ?? user?.selectedStudentId ?? user?.SelectedStudentId
                ?? (isStudentAccount ? employeeId : null);
            if (studentId != null && studentId !== '' && studentId !== 'null' && studentId !== 'undefined') {
                headers['X-Student-Id'] = studentId.toString();
            }
        } catch (e) {
            console.error("Failed to parse user for scope headers", e);
        }
    }

    if (!excludeEntityScope && !headers['X-Entity-Id']) {
        const entityId = entityScopeOverride || selectedEntity;
        if (entityId && entityId !== 'null' && entityId !== 'undefined') {
            headers['X-Entity-Id'] = entityId.toString();
            headers['Entity-Id'] = entityId.toString();
        }
    }

    const campusId = selectedCampusId;
    if (campusId != null && campusId !== '' && campusId !== 'null') {
        headers['X-Campus-Id'] = campusId;
        headers['Campus-Id'] = campusId;
    }

    return headers;
};

const shouldCacheGet = (method: string, signal?: AbortSignal) => {
    if (signal) return false;

    const normalizedMethod = method.toLowerCase();
    return normalizedMethod.includes('/lookups') ||
        normalizedMethod.includes('/lookup') ||
        normalizedMethod.includes('/timetable/v3/dashboard') ||
        normalizedMethod.includes('pagesize=1000');
};

const getRequestCacheKey = (method: string, token?: string) => {
    const scope = {
        token: token || localStorage.getItem('accessToken') || '',
        selectedEntity: localStorage.getItem('selectedEntity') || localStorage.getItem('selectedEntityId') || '',
        selectedCampusId: localStorage.getItem('selectedCampusId') || '',
    };

    return `${method}::${JSON.stringify(scope)}`;
};

/* 
  GetService()
  Purpose: Standardized GET request handler
*/
export function GetService(method: string, _baseUrl?: string, token?: string, signal?: AbortSignal, scopeHeaders?: Record<string, string>) {
    const authToken = getToken(token);
    const headers: any = {
        'User-Timezone-Offset': getPolicyTimezoneOffset(),
        ...getScopeHeaders(method),
        ...scopeHeaders,
    };
    if (authToken) {
        headers.Authorization = `Bearer ${authToken}`;
    }

    // A dashboard-only scope override must not reuse a response cached under
    // the user's default campus scope.
    const canCache = !scopeHeaders && shouldCacheGet(method, signal);
    const cacheKey = canCache ? getRequestCacheKey(method, authToken) : "";
    if (canCache) {
        const cached = getResponseCache.get(cacheKey);
        if (cached && cached.expiresAt > Date.now()) {
            return Promise.resolve(cached.response);
        }

        const pending = inFlightGetRequests.get(cacheKey);
        if (pending) return pending;
    }

    const request = api.get(method, {
        headers,
        signal,
        validateStatus: function (status) {
            return (status >= 200 && status < 300) || status === 404;
        }
    });

    if (!canCache) return request;

    const cachedRequest = request
        .then((response) => {
            getResponseCache.set(cacheKey, {
                expiresAt: Date.now() + GET_CACHE_TTL_MS,
                response,
            });
            return response;
        })
        .finally(() => {
            inFlightGetRequests.delete(cacheKey);
        });

    inFlightGetRequests.set(cacheKey, cachedRequest);
    return cachedRequest;
}

/* 
  PostService()
  Purpose: Standardized POST request handler
*/
export function PostService(method: string, payload: any, _baseUrl?: string, token?: string, signal?: AbortSignal, scopeHeaders?: Record<string, string>) {
    const authToken = getToken(token);
    const headers: any = {
        'User-Timezone-Offset': getPolicyTimezoneOffset(),
        ...getScopeHeaders(method),
        ...scopeHeaders,
    };
    if (authToken) {
        headers.Authorization = `Bearer ${authToken}`;
    }

    return api.post(method, payload, {
        headers,
        signal,
        validateStatus: function (status) {
            return (status >= 200 && status < 300) || status === 404;
        }
    }).finally(invalidateGetCache);
}

/* 
  PutService()
  Purpose: Standardized PUT request handler
*/
export function PutService(method: string, payload: any, _baseUrl?: string, token?: string, scopeHeaders?: Record<string, string>) {
    const authToken = getToken(token);
    const headers: any = {
        'User-Timezone-Offset': getPolicyTimezoneOffset(),
        ...getScopeHeaders(method),
        ...scopeHeaders,
    };
    if (authToken) {
        headers.Authorization = `Bearer ${authToken}`;
    }

    return api.put(method, payload, {
        headers,
        validateStatus: function (status) {
            return (status >= 200 && status < 300) || status === 404;
        }
    }).finally(invalidateGetCache);
}

/* 
  DeleteService()
  Purpose: Standardized DELETE request handler
*/
export function DeleteService(method: string, _baseUrl?: string, token?: string) {
    const authToken = getToken(token);
    const headers: any = {
        'User-Timezone-Offset': getPolicyTimezoneOffset(),
        ...getScopeHeaders(method),
    };
    if (authToken) {
        headers.Authorization = `Bearer ${authToken}`;
    }

    return api.delete(method, {
        headers,
        validateStatus: function (status) {
            return (status >= 200 && status < 300) || status === 404;
        }
    }).finally(invalidateGetCache);
}

/*
  CheckemptyString()
  Purpose: Helper function - to check falsy values in string type
  Parameters: string
  Returns: boolean
*/
export const CheckemptyString = (e: any): boolean => {
    switch (e) {
        case '':
        case '0':
        case null:
        case undefined:
            return false;
        default:
            return true;
    }
};

export const RemoveAmountCommas = (
    value: string | number | null | undefined,
): string => String(value ?? '').replace(/,/g, '');

export const AMOUNT_INPUT_MAX_DIGITS = 18;
export const AMOUNT_INPUT_DECIMAL_PLACES = 2;

export const NormalizeAmountInput = (
    value: string | number | null | undefined,
    decimalPlaces: number = getCurrencyDecimalPlaces(),
    maxDigits: number = AMOUNT_INPUT_MAX_DIGITS,
): string => {
    let normalized = RemoveAmountCommas(value).replace(/[^\d.]/g, '');
    const firstDotIndex = normalized.indexOf('.');

    if (firstDotIndex >= 0) {
        normalized =
            normalized.slice(0, firstDotIndex + 1) +
            normalized.slice(firstDotIndex + 1).replace(/\./g, '');
    }

    const [wholePart, decimalPart] = normalized.split('.');
    const limitedWhole = wholePart.slice(0, maxDigits);

    if (decimalPlaces === 0) return limitedWhole;
    if (decimalPart === undefined) return limitedWhole;
    return `${limitedWhole}.${decimalPart.slice(0, decimalPlaces)}`;
};

export const IsValidAmountInput = (
    value: string | number | null | undefined,
    decimalPlaces: number = getCurrencyDecimalPlaces(),
    maxDigits: number = AMOUNT_INPUT_MAX_DIGITS,
): boolean => {
    const normalizedValue = RemoveAmountCommas(value);
    if (normalizedValue === '') return true;
    if (decimalPlaces === 0) return new RegExp(`^\\d{0,${maxDigits}}$`).test(normalizedValue);

    const pattern = new RegExp(
        `^\\d{0,${maxDigits}}(\\.\\d{0,${decimalPlaces}})?$`,
    );
    return pattern.test(normalizedValue);
};

export const GetNextAmountInputValue = (
    currentValue: string,
    selectionStart: number | null | undefined,
    selectionEnd: number | null | undefined,
    nextText: string,
): string => {
    const valueWithoutCommas = RemoveAmountCommas(currentValue);
    const rawStart = selectionStart ?? currentValue.length;
    const rawEnd = selectionEnd ?? currentValue.length;
    const start = RemoveAmountCommas(currentValue.slice(0, rawStart)).length;
    const end = RemoveAmountCommas(currentValue.slice(0, rawEnd)).length;

    return `${valueWithoutCommas.slice(0, start)}${nextText}${valueWithoutCommas.slice(end)}`;
};

export const AmountToNumber = (
    value: string | number | null | undefined,
): number => {
    const numericValue =
        typeof value === 'number'
            ? value
            : parseFloat(RemoveAmountCommas(value));

    return Number.isFinite(numericValue) ? numericValue : 0;
};

export const AmountFormat = (
    value: string | number | null | undefined,
    decimals: number = getCurrencyDecimalPlaces(),
    comma: boolean = true,
): string => {
    return formatRegionalAmount(AmountToNumber(value), decimals, comma);
};

export const DateFormat = (
    value: string | number | Date | null | undefined,
    pattern: string = 'DD-MMM-YYYY',
): string => {
    if (!value) return '';

    const parsedDate = dayjs(value);
    return parsedDate.isValid() ? parsedDate.format(pattern) : '';
};

/* Digits-only input helper (allows empty while typing) */
export const NUMBER_INPUT_REGEX = /^\d*$/;

/*
  CheckemptyObject()
  Purpose: Helper function - to check empty object
  Parameters: object
  Returns: boolean
*/
export const CheckemptyObject = (obj: any): boolean => {
    if (Array.isArray(obj)) return false;
    if (!obj) return false;
    if (Object.keys(obj).length === 0) return false;
    return true;
};

/* To Check Array Type And Length */
export const ToCheckArrayTypeAndLength = (_arrayData: any): boolean => {
    if (Array.isArray(_arrayData) && _arrayData.length > 0) {
        return true;
    } else {
        return false;
    }
};

{/*to Resolve imeZone issue*/}
export const toDateOnly = (value: string | Date | null | undefined): string => {
  if (!value) return "";

  if (typeof value === "string") {
    const datePart = value.match(/^(\d{4}-\d{2}-\d{2})/);
    if (datePart) return datePart[1];
  }

  const parsedDate = dayjs(value);
  return parsedDate.isValid() ? parsedDate.format("YYYY-MM-DD") : "";
};
{/*to Resolve imeZone issue*/}

/* Global state helpers */
export const getGlobalStates = () => {
    const cached = localStorage.getItem('global_states');
    if (cached) {
        try {
            const parsed = JSON.parse(cached);
            if (ToCheckArrayTypeAndLength(parsed)) {
                return parsed;
            }
        } catch (e) {
            console.error("Failed to parse global_states from localStorage", e);
        }
    }
    return [];
};

export const setGlobalStates = (states: any[]) => {
    localStorage.setItem('global_states', JSON.stringify(states));
};

export const fetchGlobalStates = async () => {
    try {
        const response: any = await GetService(
            "/api/organization/State/v3?pageNumber=1&pageSize=1000&includeInactive=true",
        );
        const payload = response.data ?? response ?? {};
        const items = payload.items ?? payload.Items ?? [];

        if (ToCheckArrayTypeAndLength(items)) {
            const formatted = items.map((item: any) => ({
                stateid: item.stateid || item.stateId || item.id,
                code: item.statecode || item.stateCode || "",
                statename: item.statename || item.stateName || "",
                countryid: item.countryid || item.countryId || 0,
                countryname: item.countryname || item.countryName || "",
            }));
            setGlobalStates(formatted);
            return formatted;
        }
    } catch (e) {
        console.error("Failed to fetch global states", e);
    }

    return [];
};
