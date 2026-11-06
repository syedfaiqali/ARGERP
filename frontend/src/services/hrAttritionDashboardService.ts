import { GetService } from '@/constants/globalItems'
import { isAxiosError } from 'axios'

export type HrLookup = { id: number; name: string }
export type HrMetricGroup = { id: string; name: string; total: number; averageStaff: number; exits: number; rate: number | null }
export type HrDetailKind = 'staff' | 'cohort' | 'retained' | 'joiners' | 'exits' | 'trend' | 'positions' | 'comparison'
export type HrFilters = { from: string; to: string; entityId?: number; departmentId?: number }
export type HrSummary = {
  total: number; openingStaff: number; averageStaff: number; joiners: number; exits: number; retained: number; cohortExits: number
  rate: number | null; retentionRate: number | null; previousRate: number | null; changePp: number | null; openPositions: number | null
  departmentRows: HrMetricGroup[]; campusRows: HrMetricGroup[]; staffTypeRows: HrMetricGroup[]
  months: { name: string; joiners: number; exits: number; rate: number | null }[]
  tenures: { name: string; value: number }[]; reasons: { name: string; value: number }[]
  comparison: { name: string; from: string; to: string; joiners: number; exits: number; rate: number | null }[]
}
export type HrDashboardResponse = { entities: HrLookup[]; departments: HrLookup[]; summary: HrSummary; dataNote: string }
export type HrDetailResponse = { rows: Record<string, unknown>[]; totalCount: number; pageNumber: number; pageSize: number; dataNote: string }
export const emptyHrSummary: HrSummary = {
  total: 0, openingStaff: 0, averageStaff: 0, joiners: 0, exits: 0, retained: 0, cohortExits: 0,
  rate: null, retentionRate: null, previousRate: null, changePp: null, openPositions: null,
  departmentRows: [], campusRows: [], staffTypeRows: [], months: [], tenures: [], reasons: [], comparison: [],
}
function queryFor(filters: HrFilters) {
  const query = new URLSearchParams({ from: filters.from, to: filters.to })
  if (filters.entityId) query.set('entityId', String(filters.entityId))
  if (filters.departmentId) query.set('departmentId', String(filters.departmentId))
  return query
}
async function get<T>(path: string, signal: AbortSignal): Promise<T> {
  try {
    const response = await GetService(path, undefined, undefined, signal)
    if (!response || response.status < 200 || response.status >= 300) {
      throw new Error(response?.status === 403 ? 'You do not have permission to view HR dashboard data for this campus.' : 'HR dashboard could not be loaded. Please retry.')
    }
    return response.data as T
  } catch (error) {
    if (isAxiosError(error) && error.code !== 'ERR_CANCELED') {
      throw new Error(error.response?.status === 403 ? 'You do not have permission to view HR dashboard data for this campus.' : 'HR dashboard could not be loaded. Please retry.')
    }
    throw error
  }
}
export async function getHrDashboard(filters: HrFilters, signal: AbortSignal) {
  const data = await get<HrDashboardResponse>(`/api/auth/HR/v3/attrition?${queryFor(filters)}`, signal)
  if (!data?.summary || !Array.isArray(data.entities) || !Array.isArray(data.summary.months)) throw new Error('Invalid HR dashboard response.')
  return data
}
export async function getHrDetails(filters: HrFilters, kind: HrDetailKind, pageNumber: number, pageSize: number, columnFilters: Record<string, string>, signal: AbortSignal) {
  const query = queryFor(filters)
  query.set('kind', kind)
  query.set('pageNumber', String(pageNumber))
  query.set('pageSize', String(pageSize))
  const activeFilters = Object.fromEntries(Object.entries(columnFilters).filter(([, value]) => value.trim()))
  if (Object.keys(activeFilters).length) query.set('filters', JSON.stringify(activeFilters))
  const data = await get<HrDetailResponse>(`/api/auth/HR/v3/attrition/details?${query}`, signal)
  if (!Array.isArray(data?.rows) || typeof data.totalCount !== 'number') throw new Error('Invalid HR detail response.')
  return data
}
