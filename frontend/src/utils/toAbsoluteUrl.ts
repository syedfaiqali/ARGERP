/// <reference types="vite/client" />

const API_BASE_URL: string =
  import.meta.env.VITE_API_URL || import.meta.env.VITE_API_BASE_URL || (typeof window === 'undefined' ? '' : window.location.origin);

/**
 * Converts a possibly-relative URL (e.g. "/api/...") into an absolute URL using VITE_API_URL.
 * Falls back to the original value when base URL is not configured.
 */
export function toAbsoluteUrl(input?: string | null): string {
  const raw = (input ?? "").trim();
  if (!raw) return "";

  // Already absolute / browser-managed
  if (/^(https?:)?\/\//i.test(raw) || /^data:/i.test(raw) || /^blob:/i.test(raw)) {
    return raw;
  }

  const base = (API_BASE_URL || "").trim().replace(/\/+$/, "");
  if (!base) return raw;

  if (raw.startsWith("/")) return `${base}${raw}`;
  return `${base}/${raw}`;
}

