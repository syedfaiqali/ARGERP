import { DeleteService, GetService, PostService, PutService } from "@/constants/globalItems";
import type { RegionalSettings } from "@/utils/regionalSettings";
import { setStoredDateFormat } from "@/utils/systemPolicyDateFormat";
import { workflowMutation } from "@/workflow/api";
import { getGridPagingRevision, gridPagingScope, setGridPaging } from "@/utils/systemPolicyGridPaging";

const HR_DOCUMENT_TYPE_BASE = "/api/HumanResource/HRDocumentType/v3";
const SYSTEM_POLICY_ATTACHMENT_BASE = "/api/Administration/SystemPolicyAttachment/v3";
const SYSTEM_POLICY_GENERAL_BASE = "/api/Administration/SystemPolicyGeneral/v3";
const SYSTEM_POLICY_ATTENDANCE_BASE = "/api/Administration/SystemPolicyAttendance/v3";
const SYSTEM_POLICY_FEE_MANAGEMENT_BASE = "/api/Administration/SystemPolicyFeeManagement/v3";
const SYSTEM_POLICY_ACADEMIC_BASE = "/api/Administration/SystemPolicyAcademic/v3";
const SYSTEM_POLICY_FINANCE_BASE = "/api/Administration/SystemPolicyFinance/v3";
const SYSTEM_POLICY_COMMUNICATION_BASE = "/api/Administration/SystemPolicyCommunication/v3";
const SYSTEM_POLICY_EXTERNAL_DRIVES_BASE = "/api/Administration/SystemPolicyExternalDrives/v3";
const SYSTEM_POLICY_DEFAULT_REPORTS_BASE = "/api/Administration/SystemPolicyDefaultReports/v3";
const SYSTEM_POLICY_NOTIFICATIONS_BASE = "/api/Administration/SystemPolicyNotifications/v3";

export type LookupItem = {
  id: number;
  parentId?: number;
  code: string;
  name: string;
};

export type SystemPolicyAttachmentRow = {
  id?: number;
  sNo?: number;
  lovDocumentTypeId: number;
  documentTypeCode?: string;
  documentTypeName?: string;
  lovStorageTypeId: number;
  storageTypeCode?: string;
  storageTypeName?: string;
  sizeKb: number | string;
  location?: string;
  downloadDriveId?: number | null;
  downloadDriveName?: string;
  uploadDriveId?: number | null;
  uploadDriveName?: string;
};

export type SystemPolicyAttachmentLookups = {
  documentTypes: LookupItem[];
  storageTypes: LookupItem[];
  downloadDrives: LookupItem[];
  uploadDrives: LookupItem[];
};

export type SystemPolicyAttendanceTypeRow = {
  id?: number | null;
  policyId?: number | null;
  attendanceTypeId: number;
  attendanceTypeName: string;
  orderId?: number | string | null;
  selected: boolean;
};

export type SystemPolicyGeneralRow = {
  regional?: RegionalSettings | null;
  timeZone?: string | null;
  id?: number;
  defaultCountryId?: number | null;
  defaultCountryName?: string;
  defaultStateId?: number | null;
  defaultStateName?: string;
  defaultCityId?: number | null;
  defaultCityName?: string;
  dateFormat: string;
  timeFormat: string;
  gridPaging: number | string;
  guardianBasedOnEntity: boolean;
  userNameBasedOnId?: number | null;
  userNameBasedOnName?: string;
  allowIntegration: boolean;
  meetingTypeId?: number | null;
  meetingTypeName?: string;
  markAttendance: boolean;
  attendanceTypes: SystemPolicyAttendanceTypeRow[];
};

export type SystemPolicyGeneralLookups = {
  countries: LookupItem[];
  states: LookupItem[];
  cities: LookupItem[];
  userNameBasedOnTypes: LookupItem[];
  meetingTypes: LookupItem[];
  attendanceTypes: LookupItem[];
};

const responseData = (response: any) => response?.data?.data ?? response?.data?.Data ?? response?.data ?? response;

const toLookup = (item: any): LookupItem => ({
  id: Number(item?.id ?? item?.Id ?? 0),
  parentId: Number(item?.parentId ?? item?.ParentId ?? 0) || undefined,
  code: String(item?.code ?? item?.Code ?? ""),
  name: String(item?.name ?? item?.Name ?? item?.stxt ?? item?.Stxt ?? item?.shortText ?? item?.ShortText ?? ""),
});

// Older Administration deployments have /lookups but not the scoped /locations route.
// Use the existing paginated Organization APIs only for a missing route, never for auth/server errors.
const getOrganizationPolicyLocations = async (countryId: number, stateId?: number | null) => {
  const read = async (kind: 'State' | 'City'): Promise<LookupItem[]> => {
    const result: LookupItem[] = [];
    for (let page = 1; page <= 100; page++) {
      const query = new URLSearchParams({ countryId: String(countryId), pageNumber: String(page), pageSize: '1000' });
      if (kind === 'City' && stateId) query.set('stateId', String(stateId));
      const response = await GetService(`/api/organization/${kind}/v3?${query}`);
      if (response?.status >= 400) throw new Error(response?.data?.error || 'Failed to load policy locations');
      throwIfFailed(response, 'Failed to load policy locations');
      const data = responseData(response) || {};
      const items: any[] = Array.isArray(data) ? data : data.items ?? data.Items ?? [];
      for (const item of items) {
        const parentCountry = Number(item.countryid ?? item.countryId ?? item.CountryId ?? countryId);
        const parentState = Number(item.stateid ?? item.stateId ?? item.StateId ?? stateId ?? 0);
        if (parentCountry !== countryId || (kind === 'City' && parentState !== stateId)) continue;
        result.push(toLookup(kind === 'State' ? {
          id: item.stateid ?? item.stateId ?? item.StateId ?? item.id ?? item.Id,
          parentId: parentCountry, code: item.statecode ?? item.stateCode ?? item.StateCode ?? item.code,
          name: item.statename ?? item.stateName ?? item.StateName ?? item.name ?? item.stxt,
        } : {
          id: item.cityid ?? item.cityId ?? item.CityId ?? item.id ?? item.Id,
          parentId: parentState, code: item.citycode ?? item.cityCode ?? item.CityCode ?? item.code,
          name: item.cityname ?? item.cityName ?? item.CityName ?? item.name,
        }));
      }
      const total = Number(data.totalCount ?? data.TotalCount ?? items.length);
      const pageSize = Number(data.pageSize ?? data.PageSize ?? 1000);
      if (Array.isArray(data) || !items.length || page * pageSize >= total) return result;
    }
    throw new Error('Too many location records. Narrow the location selection and try again.');
  };
  const [states, cities] = await Promise.all([read('State'), stateId ? read('City') : Promise.resolve([])]);
  return { states, cities };
};

const toAttachmentRow = (item: any): SystemPolicyAttachmentRow => ({
  id: Number(item?.id ?? item?.Id ?? 0),
  sNo: Number(item?.sNo ?? item?.SNo ?? 0),
  lovDocumentTypeId: Number(item?.lovDocumentTypeId ?? item?.LovDocumentTypeId ?? 0),
  documentTypeCode: item?.documentTypeCode ?? item?.DocumentTypeCode ?? "",
  documentTypeName: item?.documentTypeName ?? item?.DocumentTypeName ?? "",
  lovStorageTypeId: Number(item?.lovStorageTypeId ?? item?.LovStorageTypeId ?? 0),
  storageTypeCode: item?.storageTypeCode ?? item?.StorageTypeCode ?? "",
  storageTypeName: item?.storageTypeName ?? item?.StorageTypeName ?? "",
  sizeKb: item?.sizeKb ?? item?.SizeKb ?? "",
  location: item?.location ?? item?.Location ?? "",
  downloadDriveId: item?.downloadDriveId ?? item?.DownloadDriveId ?? null,
  downloadDriveName: item?.downloadDriveName ?? item?.DownloadDriveName ?? "",
  uploadDriveId: item?.uploadDriveId ?? item?.UploadDriveId ?? null,
  uploadDriveName: item?.uploadDriveName ?? item?.UploadDriveName ?? "",
});

const toAttachmentPayload = (payload: SystemPolicyAttachmentRow) => ({
  id: payload.id || undefined,
  lovDocumentTypeId: Number(payload.lovDocumentTypeId || 0),
  lovStorageTypeId: Number(payload.lovStorageTypeId || 0),
  sizeKb: Number(payload.sizeKb || 0),
  location: payload.location ? String(payload.location).slice(0, 500) : null,
  downloadDriveId: payload.downloadDriveId || null,
  uploadDriveId: payload.uploadDriveId || null,
});

const toGeneralRow = (item: any): SystemPolicyGeneralRow => ({
  regional: item?.regional ?? item?.Regional ?? null,
  timeZone: item?.regional?.timeZone ?? item?.Regional?.TimeZone ?? null,
  id: item?.id ?? item?.Id ?? 0,
  defaultCountryId: item?.defaultCountryId ?? item?.DefaultCountryId ?? null,
  defaultCountryName: item?.defaultCountryName ?? item?.DefaultCountryName ?? "",
  defaultStateId: item?.defaultStateId ?? item?.DefaultStateId ?? null,
  defaultStateName: item?.defaultStateName ?? item?.DefaultStateName ?? "",
  defaultCityId: item?.defaultCityId ?? item?.DefaultCityId ?? null,
  defaultCityName: item?.defaultCityName ?? item?.DefaultCityName ?? "",
  dateFormat: item?.dateFormat ?? item?.DateFormat ?? "",
  timeFormat: item?.timeFormat ?? item?.TimeFormat ?? "",
  gridPaging: item?.gridPaging ?? item?.GridPaging ?? "20",
  guardianBasedOnEntity: Boolean(item?.guardianBasedOnEntity ?? item?.GuardianBasedOnEntity ?? false),
  userNameBasedOnId: item?.userNameBasedOnId ?? item?.UserNameBasedOnId ?? null,
  userNameBasedOnName: item?.userNameBasedOnName ?? item?.UserNameBasedOnName ?? "",
  allowIntegration: Boolean(item?.allowIntegration ?? item?.AllowIntegration ?? false),
  meetingTypeId: item?.meetingTypeId ?? item?.MeetingTypeId ?? null,
  meetingTypeName: item?.meetingTypeName ?? item?.MeetingTypeName ?? "",
  markAttendance: Boolean(item?.markAttendance ?? item?.MarkAttendance ?? false),
  attendanceTypes: (item?.attendanceTypes ?? item?.AttendanceTypes ?? []).map((row: any) => ({
    id: row?.id ?? row?.Id ?? null,
    policyId: row?.policyId ?? row?.PolicyId ?? null,
    attendanceTypeId: Number(row?.attendanceTypeId ?? row?.AttendanceTypeId ?? 0),
    attendanceTypeName: row?.attendanceTypeName ?? row?.AttendanceTypeName ?? "",
    orderId: row?.orderId ?? row?.OrderId ?? "",
    selected: Boolean(row?.selected ?? row?.Selected ?? false),
  })),
});

const toGeneralPayload = (payload: SystemPolicyGeneralRow) => ({
  timeZone: payload.timeZone || null,
  id: payload.id || undefined,
  defaultCountryId: payload.defaultCountryId || null,
  defaultStateId: payload.defaultStateId || null,
  defaultCityId: payload.defaultCityId || null,
  dateFormat: payload.dateFormat ? String(payload.dateFormat).slice(0, 50) : "",
  timeFormat: payload.timeFormat ? String(payload.timeFormat).slice(0, 50) : "",
  gridPaging: Number(payload.gridPaging || 0),
  guardianBasedOnEntity: Boolean(payload.guardianBasedOnEntity),
  userNameBasedOnId: payload.userNameBasedOnId || null,
  allowIntegration: Boolean(payload.allowIntegration),
  meetingTypeId: payload.meetingTypeId || null,
  markAttendance: Boolean(payload.markAttendance),
  attendanceTypes: (payload.attendanceTypes || []).map((row) => ({
    id: row.id || null,
    attendanceTypeId: Number(row.attendanceTypeId || 0),
    orderId: row.orderId === "" || row.orderId === null || row.orderId === undefined ? null : Number(row.orderId),
    selected: Boolean(row.selected),
  })),
});

const normalize = (item: any): any => {
  if (!item || typeof item !== "object") return item;
  return Object.keys(item).reduce((acc: any, key) => {
    const nextKey = key.charAt(0).toLowerCase() + key.slice(1);
    acc[nextKey] = Array.isArray(item[key]) ? item[key].map(normalize) : normalize(item[key]);
    return acc;
  }, {});
};

const getPolicy = async <T = any>(base: string, fallback: string): Promise<T | null> => {
  const response = await GetService(base);
  throwIfFailed(response, fallback);
  const data = responseData(response);
  return data ? normalize(data) : null;
};

const getPolicyRows = async <T = any>(base: string, fallback: string): Promise<T[]> => {
  const response = await GetService(base);
  throwIfFailed(response, fallback);
  const rows = responseData(response);
  return Array.isArray(rows) ? rows.map(normalize) : [];
};

const getPolicyLookups = async <T = any>(base: string, fallback: string): Promise<T> => {
  const response = await GetService(`${base}/lookups`);
  throwIfFailed(response, fallback);
  return normalize(responseData(response) || {});
};

const savePolicy = async <T = any>(base: string, payload: any, fallback: string): Promise<T> => {
  const response = await PutService(base, payload);
  throwIfFailed(response, fallback);
  return normalize(responseData(response));
};

const savePolicyRows = async <T = any>(base: string, payload: any, fallback: string): Promise<T[]> => {
  const response = await PutService(base, payload);
  throwIfFailed(response, fallback);
  const rows = responseData(response);
  return Array.isArray(rows) ? rows.map(normalize) : [];
};

const savePolicyRow = async <T = any>(base: string, payload: any, fallback: string): Promise<T> => {
  const response = payload.id ? await PutService(`${base}/${payload.id}`, payload) : await PostService(base, payload);
  throwIfFailed(response, fallback);
  return normalize(responseData(response));
};

const throwIfFailed = (response: any, fallback: string) => {
  const body = response?.data;
  if (body?.isSuccessCall === false || body?.IsSuccessCall === false) {
    const errors = body?.errors ?? body?.Errors ?? [];
    const firstError = Array.isArray(errors) ? errors[0] : null;
    throw new Error(firstError?.description ?? firstError?.Description ?? fallback);
  }
};

export const humanResourceAdministrationService = {
  getDocumentTypes: (pageSize = 1000) =>
    GetService(`${HR_DOCUMENT_TYPE_BASE}?pageSize=${pageSize}`),
  getDocumentType: (id: number | string) => GetService(`${HR_DOCUMENT_TYPE_BASE}/${id}`),
  // Document type saves may go through an approval workflow, which answers 202 while pending.
  createDocumentType: (payload: any) => workflowMutation("post", HR_DOCUMENT_TYPE_BASE, payload),
  updateDocumentType: (id: number | string, payload: any) =>
    workflowMutation("put", `${HR_DOCUMENT_TYPE_BASE}/${id}`, payload),
  deleteDocumentType: (id: number | string) => workflowMutation("delete", `${HR_DOCUMENT_TYPE_BASE}/${id}`),
  getSystemPolicyAttachmentPolicies: async (): Promise<SystemPolicyAttachmentRow[]> => {
    const response = await GetService(SYSTEM_POLICY_ATTACHMENT_BASE);
    throwIfFailed(response, "Failed to load attachment policies");
    const rows = responseData(response);
    return Array.isArray(rows) ? rows.map(toAttachmentRow) : [];
  },
  getSystemPolicyAttachmentPolicy: async (id: number | string): Promise<SystemPolicyAttachmentRow> => {
    const response = await GetService(`${SYSTEM_POLICY_ATTACHMENT_BASE}/${id}`);
    throwIfFailed(response, "Failed to load attachment policy");
    return toAttachmentRow(responseData(response));
  },
  getSystemPolicyAttachmentLookups: async (): Promise<SystemPolicyAttachmentLookups> => {
    const response = await GetService(`${SYSTEM_POLICY_ATTACHMENT_BASE}/lookups`);
    throwIfFailed(response, "Failed to load attachment policy lookups");
    const data = responseData(response) || {};
    return {
      documentTypes: (data.documentTypes ?? data.DocumentTypes ?? []).map(toLookup),
      storageTypes: (data.storageTypes ?? data.StorageTypes ?? []).map(toLookup),
      downloadDrives: (data.downloadDrives ?? data.DownloadDrives ?? []).map(toLookup),
      uploadDrives: (data.uploadDrives ?? data.UploadDrives ?? []).map(toLookup),
    };
  },
  saveSystemPolicyAttachmentPolicy: async (payload: SystemPolicyAttachmentRow) => {
    const request = toAttachmentPayload(payload);
    const response = payload.id
      ? await PutService(`${SYSTEM_POLICY_ATTACHMENT_BASE}/${payload.id}`, request)
      : await PostService(SYSTEM_POLICY_ATTACHMENT_BASE, request);

    throwIfFailed(response, "Failed to save attachment policy");
    return toAttachmentRow(responseData(response));
  },
  deleteSystemPolicyAttachmentPolicy: async (id: number | string) => {
    const response = await DeleteService(`${SYSTEM_POLICY_ATTACHMENT_BASE}/${id}`);
    throwIfFailed(response, "Failed to delete attachment policy");
    return true;
  },
  getSystemPolicyGeneral: async (): Promise<SystemPolicyGeneralRow | null> => {
    const scope = gridPagingScope();
    const revision = getGridPagingRevision(scope);
    const response = await GetService(SYSTEM_POLICY_GENERAL_BASE);
    throwIfFailed(response, "Failed to load general policy");
    const data = responseData(response);
    const policy = data ? toGeneralRow(data) : null;
    if (getGridPagingRevision(scope) === revision) setGridPaging(policy?.gridPaging, scope);
    return policy;
  },
  getSystemPolicyGeneralLookups: async (includeLocations = true): Promise<SystemPolicyGeneralLookups> => {
    const response = await GetService(`${SYSTEM_POLICY_GENERAL_BASE}/lookups${includeLocations ? '' : '?includeLocations=false'}`);
    throwIfFailed(response, "Failed to load general policy lookups");
    const data = responseData(response) || {};
    return {
      countries: (data.countries ?? data.Countries ?? []).map(toLookup),
      states: (data.states ?? data.States ?? []).map(toLookup),
      cities: (data.cities ?? data.Cities ?? []).map(toLookup),
      userNameBasedOnTypes: (data.userNameBasedOnTypes ?? data.UserNameBasedOnTypes ?? []).map(toLookup),
      meetingTypes: (data.meetingTypes ?? data.MeetingTypes ?? []).map(toLookup),
      attendanceTypes: (data.attendanceTypes ?? data.AttendanceTypes ?? []).map(toLookup),
    };
  },
  getSystemPolicyGeneralLocations: async (countryId?: number | null, stateId?: number | null): Promise<Pick<SystemPolicyGeneralLookups, 'states' | 'cities'>> => {
    if (!countryId) return { states: [], cities: [] };
    const query = new URLSearchParams({ countryId: String(countryId) });
    if (stateId) query.set('stateId', String(stateId));
    let response: any;
    try { response = await GetService(`${SYSTEM_POLICY_GENERAL_BASE}/locations?${query}`); }
    catch (error: any) {
      if (error?.response?.status !== 404) throw error;
      return getOrganizationPolicyLocations(countryId, stateId);
    }
    if (response?.status === 404) return getOrganizationPolicyLocations(countryId, stateId);
    if (response?.status >= 400) throw new Error(response?.data?.error || 'Failed to load policy locations');
    throwIfFailed(response, "Failed to load policy locations");
    const data = responseData(response) || {};
    return { states: (data.states ?? data.States ?? []).map(toLookup), cities: (data.cities ?? data.Cities ?? []).map(toLookup) };
  },
  saveSystemPolicyGeneral: async (payload: SystemPolicyGeneralRow): Promise<SystemPolicyGeneralRow> => {
    const scope = gridPagingScope();
    const response = await PutService(SYSTEM_POLICY_GENERAL_BASE, toGeneralPayload(payload));
    throwIfFailed(response, "Failed to save general policy");
    const saved = toGeneralRow(responseData(response));
    setGridPaging(saved.gridPaging || payload.gridPaging, scope);
    setStoredDateFormat(saved.dateFormat || payload.dateFormat);
    return saved;
  },
  getSystemPolicyAttendance: () => getPolicy(SYSTEM_POLICY_ATTENDANCE_BASE, "Failed to load attendance policy"),
  saveSystemPolicyAttendance: (payload: any) =>
    savePolicy(SYSTEM_POLICY_ATTENDANCE_BASE, payload, "Failed to save attendance policy"),
  getSystemPolicyFeeManagement: () =>
    getPolicy(SYSTEM_POLICY_FEE_MANAGEMENT_BASE, "Failed to load fee management policy"),
  getSystemPolicyFeeManagementLookups: () =>
    getPolicyLookups(SYSTEM_POLICY_FEE_MANAGEMENT_BASE, "Failed to load fee management lookups"),
  saveSystemPolicyFeeManagement: (payload: any) =>
    savePolicy(SYSTEM_POLICY_FEE_MANAGEMENT_BASE, payload, "Failed to save fee management policy"),
  getSystemPolicyAcademic: () => getPolicy(SYSTEM_POLICY_ACADEMIC_BASE, "Failed to load academic policy"),
  getSystemPolicyAcademicLookups: () =>
    getPolicyLookups(SYSTEM_POLICY_ACADEMIC_BASE, "Failed to load academic lookups"),
  saveSystemPolicyAcademic: (payload: any) =>
    savePolicy(SYSTEM_POLICY_ACADEMIC_BASE, payload, "Failed to save academic policy"),
  getSystemPolicyFinance: () => getPolicy(SYSTEM_POLICY_FINANCE_BASE, "Failed to load finance policy"),
  getSystemPolicyFinanceLookups: () =>
    getPolicyLookups(SYSTEM_POLICY_FINANCE_BASE, "Failed to load finance lookups"),
  saveSystemPolicyFinance: (payload: any) =>
    savePolicy(SYSTEM_POLICY_FINANCE_BASE, payload, "Failed to save finance policy"),
  getSystemPolicyCommunication: () =>
    getPolicy(SYSTEM_POLICY_COMMUNICATION_BASE, "Failed to load communication policy"),
  saveSystemPolicyCommunication: (payload: any) =>
    savePolicy(SYSTEM_POLICY_COMMUNICATION_BASE, payload, "Failed to save communication policy"),
  getSystemPolicyExternalDrives: () =>
    getPolicyRows(SYSTEM_POLICY_EXTERNAL_DRIVES_BASE, "Failed to load external drives"),
  getSystemPolicyExternalDriveLookups: () =>
    getPolicyLookups(SYSTEM_POLICY_EXTERNAL_DRIVES_BASE, "Failed to load external drive lookups"),
  saveSystemPolicyExternalDrive: (payload: any) =>
    savePolicyRow(SYSTEM_POLICY_EXTERNAL_DRIVES_BASE, payload, "Failed to save external drive"),
  deleteSystemPolicyExternalDrive: async (id: number | string) => {
    const response = await DeleteService(`${SYSTEM_POLICY_EXTERNAL_DRIVES_BASE}/${id}`);
    throwIfFailed(response, "Failed to delete external drive");
    return true;
  },
  getSystemPolicyDefaultReports: () =>
    getPolicyRows(SYSTEM_POLICY_DEFAULT_REPORTS_BASE, "Failed to load default reports"),
  saveSystemPolicyDefaultReports: (rows: any[]) =>
    savePolicyRows(SYSTEM_POLICY_DEFAULT_REPORTS_BASE, { rows }, "Failed to save default reports"),
  getSystemPolicyNotifications: () =>
    getPolicyRows(SYSTEM_POLICY_NOTIFICATIONS_BASE, "Failed to load notifications"),
  getSystemPolicyNotificationLookups: () =>
    getPolicyLookups(SYSTEM_POLICY_NOTIFICATIONS_BASE, "Failed to load notification lookups"),
  saveSystemPolicyNotifications: (rows: any[]) =>
    savePolicyRows(SYSTEM_POLICY_NOTIFICATIONS_BASE, { rows }, "Failed to save notifications"),
};
