export const AUTH_SESSION_DURATION_MS = 24 * 60 * 60 * 1000;
export const AUTH_SESSION_EXPIRES_AT_KEY = "authSessionExpiresAt";
export const AUTH_SESSION_STARTED_AT_KEY = "authSessionStartedAt";
export const CAMPUS_SESSION_KEY = "selectedCampusId";

const AUTH_STORAGE_KEYS = [
  "accessToken",
  "refreshToken",
  "user",
  // The LMS persists a second copy of the token and profile under these keys and
  // reads them as a fallback when restoring its auth slice. Leaving them behind
  // when a session is cleared left the LMS reporting "authenticated" from a token
  // this client had already stopped sending, so the app looked signed in while
  // every learner action failed.
  "edxs_token",
  "edxs_user",
  "lms_token",
  "permissions",
  "campuses",
  "studentBirthdays",
  "employeeBirthdays",
  "birthdaysCampusId",
  "birthdaysFetchedAt",
  "selectedCampusId",
  "selectedCampusName",
  "selectedEntityId",
  "selectedEntity",
  "selectedEntityID",
  "entityId",
  "EntityId",
  "clientId",
  "ClientId",
  "selectedClientId",
  "clientLogoUrl",
  "clientName",
  AUTH_SESSION_STARTED_AT_KEY,
  AUTH_SESSION_EXPIRES_AT_KEY,
];

export const startAuthSession = (startedAt = Date.now()) => {
  localStorage.setItem(AUTH_SESSION_STARTED_AT_KEY, String(startedAt));
  localStorage.setItem(
    AUTH_SESSION_EXPIRES_AT_KEY,
    String(startedAt + AUTH_SESSION_DURATION_MS),
  );
};

export const clearAuthSession = () => {
  AUTH_STORAGE_KEYS.forEach((key) => localStorage.removeItem(key));
  sessionStorage.clear();
};

export const hasCampusSession = () =>
  typeof window !== "undefined" && Boolean(sessionStorage.getItem(CAMPUS_SESSION_KEY));

export const getAuthSessionExpiresAt = () => {
  const raw = localStorage.getItem(AUTH_SESSION_EXPIRES_AT_KEY);
  if (!raw) return null;
  const parsed = Number(raw);
  return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
};

export const isAuthSessionValid = () => {
  const token = localStorage.getItem("accessToken");
  if (!token) return false;

  const expiresAt = getAuthSessionExpiresAt();
  if (!expiresAt) return false;

  return Date.now() < expiresAt;
};
