import api from "@/services/api";
import { toAbsoluteUrl } from "@/utils/toAbsoluteUrl";

const objectUrlCache = new Map<string, string>();

/**
 * Requests still in flight, keyed by URL.
 *
 * The resolved-value cache below is only consulted after a request finishes, so
 * every component asking for the same image before the first response arrived
 * used to issue its own request. On the course screen the same avatar and hero
 * image are requested from several places at once, which turned one image into
 * six identical downloads. Sharing the promise collapses them into one.
 */
const inFlight = new Map<string, Promise<string>>();

export async function loadImageObjectUrl(input?: string | null): Promise<string> {
  const url = toAbsoluteUrl(input);
  if (!url) return "";

  // Already suitable for <img src="...">
  if (/^data:/i.test(url) || /^blob:/i.test(url)) return url;

  const cached = objectUrlCache.get(url);
  if (cached) return cached;

  const pending = inFlight.get(url);
  if (pending) return pending;

  const request = api
    .get(url, { responseType: "blob" })
    .then((resp) => {
      const blobUrl = URL.createObjectURL(resp.data);
      objectUrlCache.set(url, blobUrl);
      return blobUrl;
    })
    .finally(() => {
      // Cleared either way: a failure must be retryable rather than pinned.
      inFlight.delete(url);
    });

  inFlight.set(url, request);
  return request;
}

