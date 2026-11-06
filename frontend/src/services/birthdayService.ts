import { regionalScope, setRegionalSettings } from "@/utils/regionalSettings";
import type { BirthdayPerson } from "@/store/authSlice";
import api from "@/services/api";

const BIRTHDAY_CACHE_TTL_MS = 30 * 60 * 1000;
const activeExecutiveDataRequests = new Map<number, Promise<BirthdayLoadResult>>();

interface BirthdayLoadOptions {
  useCache?: boolean;
}

interface BirthdayLoadResult {
  students: BirthdayPerson[];
  employees: BirthdayPerson[];
}

const readBirthdayCache = (campusId: number): BirthdayLoadResult | null => {
  const cachedCampusId = Number(sessionStorage.getItem("birthdaysCampusId"));
  const cachedAt = Number(sessionStorage.getItem("birthdaysFetchedAt"));
  const hasFreshBirthdayCache =
    cachedCampusId === campusId &&
    Number.isFinite(cachedAt) &&
    Date.now() - cachedAt < BIRTHDAY_CACHE_TTL_MS;

  if (!hasFreshBirthdayCache) return null;

  try {
    const students = JSON.parse(sessionStorage.getItem("studentBirthdays") || "[]");
    const employees = JSON.parse(sessionStorage.getItem("employeeBirthdays") || "[]");

    return {
      students: Array.isArray(students) ? students : [],
      employees: Array.isArray(employees) ? employees : [],
    };
  } catch {
    return null;
  }
};

export const fetchBirthdayNotifications = async (
  campusId: number,
  options: BirthdayLoadOptions = {},
): Promise<BirthdayLoadResult> => {
  const { useCache = true } = options;

  if (!campusId) {
    return { students: [], employees: [] };
  }

  const cached = useCache ? readBirthdayCache(campusId) : null;
  if (cached) return cached;

  // More than one component can ask for executive data during app startup.
  // Reuse that request so a hard refresh produces a single API call per campus.
  const activeRequest = activeExecutiveDataRequests.get(campusId);
  if (activeRequest) return activeRequest;

  const request = loadBirthdayNotifications(campusId);
  activeExecutiveDataRequests.set(campusId, request);

  try {
    return await request;
  } finally {
    activeExecutiveDataRequests.delete(campusId);
  }
};

const loadBirthdayNotifications = async (campusId: number): Promise<BirthdayLoadResult> => {
  const scope = regionalScope();
  const response = await api.get("/api/auth/executive-data", {
    params: { entityId: campusId },
  });
  const executiveData = response.data ?? {};
  const studentItems = Array.isArray(response.data?.studentBirthdays)
    ? response.data.studentBirthdays
    : [];
  const employeeItems = Array.isArray(response.data?.employeeBirthdays)
    ? response.data.employeeBirthdays
    : [];

  const students = studentItems.map((x: any) => ({
    id: Number(x.id ?? 0),
    name: [x.firstname, x.lastname].filter(Boolean).join(" ").trim() || x.studentno || "Student",
    subtitle: [x.Course, x.Section].filter(Boolean).join(" - "),
    dateOfBirth: x.dateofbirth,
    imageId: x.imageid ?? null,
  }));

  const employees = employeeItems.map((x: any) => ({
    id: Number(x.id ?? 0),
    name: x.firstname || x.employeecode || "Employee",
    subtitle: x.designationstxt || "",
    dateOfBirth: x.DOB,
    imageId: x.imageId ?? null,
  }));

  sessionStorage.setItem("studentBirthdays", JSON.stringify(students));
  sessionStorage.setItem("employeeBirthdays", JSON.stringify(employees));
  sessionStorage.setItem("birthdaysCampusId", String(campusId));
  sessionStorage.setItem("birthdaysFetchedAt", String(Date.now()));
  sessionStorage.setItem("executiveData", JSON.stringify(executiveData));
  sessionStorage.setItem("executiveDataCampusId", String(executiveData.entityId ?? campusId));
  sessionStorage.setItem("executiveLocation", JSON.stringify({
    entityId: executiveData.entityId ?? campusId,
    loadedAtUtc: executiveData.loadedAtUtc ?? null,
    country: executiveData.country ?? null,
    state: executiveData.state ?? null,
    city: executiveData.city ?? null,
    currency: executiveData.currency ?? null,
    entityType: executiveData.entityType ?? "",
    entityTypeId: executiveData.entityTypeId ?? null,
    institutionType: executiveData.institutionType ?? "",
    institutionTypeId: executiveData.institutionTypeId ?? null,
    identityDocumentLabel: executiveData.identityDocumentLabel ?? "ID",
  }));
  if (executiveData.regional) setRegionalSettings(executiveData.regional, scope);
  window.dispatchEvent(new Event("executiveDataUpdated"));

  return { students, employees };
};
