import { getDefaultTimeZone } from '@/utils/regionalSettings'

// Older API responses omit the offset after SQL materializes a UTC datetime2.
// Treat these workflow *Utc fields as UTC; preserve explicit offsets in newer responses.
export function formatWorkflowTime(value: string): string {
  const timestamp = /(?:Z|[+-]\d{2}:?\d{2})$/i.test(value) ? value : `${value}Z`
  const date = new Date(timestamp)
  if (!Number.isFinite(date.getTime())) return ''
  try { return date.toLocaleString(undefined, { timeZone: getDefaultTimeZone() }) }
  catch { return date.toLocaleString() }
}
