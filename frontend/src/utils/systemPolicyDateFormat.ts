import { GetService } from "@/constants/globalItems";

export const DEFAULT_DATE_FORMAT = "MM/dd/yyyy";
export const DATE_FORMAT_STORAGE_KEY = "systemPolicyDateFormat";
export const DATE_FORMAT_CHANGED_EVENT = "system-policy-date-format-changed";

export const DATE_FORMAT_OPTIONS = [
  "MM/dd/yyyy",
  "dd-MMM-yyyy",
  "dd/MM/yyyy",
  "yyyy-MM-dd",
  "dd-MM-yyyy",
  "MM-dd-yyyy",
];

export const normalizePolicyDateFormat = (
  value: unknown,
  fallback = DEFAULT_DATE_FORMAT,
) => {
  const format = typeof value === "string" ? value.trim() : "";
  return DATE_FORMAT_OPTIONS.includes(format) ? format : fallback;
};

export const toDayjsDateFormat = (format: string | undefined | null) => {
  const normalized = normalizePolicyDateFormat(format);
  return normalized
    .replace(/yyyy/g, "YYYY")
    .replace(/yy/g, "YY")
    .replace(/dd/g, "DD");
};

export const getStoredDateFormat = () => {
  if (typeof window === "undefined") return DEFAULT_DATE_FORMAT;
  return normalizePolicyDateFormat(window.localStorage.getItem(DATE_FORMAT_STORAGE_KEY));
};

export const setStoredDateFormat = (value: unknown) => {
  const format = normalizePolicyDateFormat(value);
  cachedDateFormat = format;
  if (typeof window === "undefined") return;
  window.localStorage.setItem(DATE_FORMAT_STORAGE_KEY, format);
  window.dispatchEvent(new Event(DATE_FORMAT_CHANGED_EVENT));
};

let cachedDateFormat: string | null = null;
let dateFormatRequest: Promise<string> | null = null;

export const loadPolicyDateFormat = async () => {
  if (cachedDateFormat !== null) return cachedDateFormat;
  if (dateFormatRequest) return dateFormatRequest;

  dateFormatRequest = GetService("/api/HumanResource/Settings/v3")
    .then((response: any) => {
      const data = response?.data?.data ?? response?.data?.Data ?? response?.data ?? response ?? {};
      const value = normalizePolicyDateFormat(
        data.dateFormat ?? data.DateFormat,
        getStoredDateFormat(),
      );
      cachedDateFormat = value;
      if (typeof window !== "undefined") {
        window.localStorage.setItem(DATE_FORMAT_STORAGE_KEY, value);
      }
      return value;
    })
    .catch(() => getStoredDateFormat())
    .finally(() => {
      dateFormatRequest = null;
    });

  return dateFormatRequest;
};
