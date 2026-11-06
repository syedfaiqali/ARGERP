/// <reference types="vite/client" />

const API_BASE_URL: string =
  import.meta.env.VITE_API_URL || import.meta.env.VITE_API_BASE_URL || "http://localhost:5001";

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

  // LMS syllabus thumbnails and teaser media are served by Academics.Service. Route
  // those paths through the Academics gateway prefix instead of the frontend dev server.
  if (/^\/uploads\/lms-syllabus\//i.test(raw)) {
    const academicsBase = /\/api\/Academics$/i.test(base) ? base : `${base}/api/Academics`;
    return `${academicsBase}${raw}`;
  }

  if (raw.startsWith("/")) return `${base}${raw}`;
  return `${base}/${raw}`;
}

