import api from './api'
import { invalidateGetCache } from '@/constants/globalItems'

export function apiErrorMessage(error: unknown, fallback: string): string {
  const e = error as { response?: { data?: unknown }; message?: string }
  const data = e?.response?.data
  if (typeof data === 'string' && data.trim()) return data
  const body = (data ?? {}) as { error?: string; message?: string; Message?: string }
  return body.error || body.message || body.Message || e?.message || fallback
}

/** Save directly through the screen controller and refresh cached lists. */
export function saveRecord(method: 'post' | 'put' | 'delete', url: string, data?: unknown) {
  return api.request({ method, url, data }).finally(invalidateGetCache)
}
