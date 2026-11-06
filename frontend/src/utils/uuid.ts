/** Generate a UUID on HTTPS and HTTP hosts without weakening randomness. */
export function createUuid(): string {
  const cryptoApi = globalThis.crypto
  // getRandomValues is available on HTTP, unlike randomUUID.
  const bytes = cryptoApi?.getRandomValues(new Uint8Array(16))
  if (!bytes) {
    throw new Error('Secure random number generation is not available in this browser.')
  }
  bytes[6] = (bytes[6] & 0x0f) | 0x40
  bytes[8] = (bytes[8] & 0x3f) | 0x80
  const hex = Array.from(bytes, byte => byte.toString(16).padStart(2, '0')).join('')
  return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`
}

// Support both helper names used by existing callers after the branch merge.
export const randomUUId = createUuid
