import api from '@/services/api'
import { invalidateGetCache } from '@/constants/globalItems'
import type { AxiosResponse } from 'axios'
import toast from 'react-hot-toast'
import { randomUUId } from '@/utils/uuid'

export const workflowBase = '/api/HumanResource/Workflow/v3'
export const workflowEnabled = import.meta.env.VITE_WORKFLOW_ENABLED === 'true'
export type Selector = { kind: 'Group' | 'Role' | 'User'; id: string }
export type RelatedUser = 'Parent' | 'ReportTo' | 'Teacher'
export const relatedUserLabels: Record<RelatedUser, string> = { Parent: 'Parent', ReportTo: 'Report To', Teacher: 'Teacher' }
export type Step = {
  name: string; decisionKind: 'Approval' | 'Recommendation'; quorum: 'Any' | 'All'
  negativeBehavior: 'Continue' | 'Terminate'; assigneeScope: 'Campus' | 'Tenant'
  allowSelfAction: boolean; requirePositiveComment: boolean; requireNegativeComment: boolean; assignees: Selector[]
  // Resolved from the requester's records when the step starts; assignees are then only its fallback.
  relatedUser: RelatedUser | null; endOnPositiveDecision: boolean
}
export type Definition = { id: string; clientId: number; name: string; screenKey: string; actionKey: string; status: string; version: number; previousVersionId?: string; steps: Step[] }
export type Registration = { screenKey: string; name: string; module: string; route: string; entityScoped: boolean; permissionControllers?: string[] }
export type ScreenCatalogue = { controllerId: string; name: string; module: string }
export type Identity = { userId: number; clientId: number; name: string; isTenantAdmin: boolean; isPlatformAdmin: boolean; entityIds: number[] }
export type WorkflowTenant = { id: number; name: string }
export type Directory = { users: { userId: number; name: string; roleId: number; roleName: string; isTenantAdmin: boolean; entityIds: number[] }[]; roles: { id: number; name: string }[]; entities: { id: number; name: string }[] }
export type Policy = { clientId: number; screenKey: string; actionKey: string; mode: string; definitionId?: string; version: number }
export type InstanceRow = { id: string; screenKey: string; operation: string; subject: string; status: string; submitterId: number; submitterName: string; entityId?: number; recordKey?: string; stepIndex: number; definitionName: string; createdAtUtc: string; updatedAtUtc: string; version: number; failureCode?: string }
export type Detail = InstanceRow & {
  generation: number; previousInstanceId?: string; route: string; screenName: string; steps: Step[]; attempts: number
  tasks: { id: string; userId: number; userName: string; roleName: string; stepIndex: number; generation: number; status: string; decision?: string }[]
  // stepName: for a decision, the step it was taken on.
  events: { id: number; userId: number; userName: string; roleName?: string; stepName?: string; kind: string; comment: string; atUtc: string }[]
  before?: Record<string, unknown>; after?: Record<string, unknown>; payload?: Record<string, unknown>; payloadError?: string
  // canAmend: the user's role may edit this screen and the request still awaits approval; otherwise
  // amendBlocked says why its values cannot change.
  canDecide: boolean; canAmend: boolean; amendBlocked?: string; canComment: boolean; canWithdraw: boolean; canCancel: boolean; canResolve: boolean; canRetry: boolean; canResubmit: boolean
}
// A screen's own form, opened on a pending request. It shows detail.payload and, unless read-only,
// hands its edited body (the same body the screen would save) to onSave; onSave reports success.
export type WorkflowFormProps = { detail: Detail; readOnly: boolean; saving: boolean; onSave: (payload: Record<string, unknown>) => Promise<boolean> }
export const wfGet = async <T>(path: string, signal?: AbortSignal) => (await api.get<T>(workflowBase + path, { signal })).data
export const wfPost = async <T>(path: string, body?: unknown) => (await api.post<T>(workflowBase + path, body)).data
export const wfPut = async <T>(path: string, body: unknown) => (await api.put<T>(workflowBase + path, body)).data
export const wfDelete = async (path: string) => api.delete(workflowBase + path)
export function workflowError(error: unknown): string {
  const e = error as { response?: { data?: { error?: string; instanceId?: string }; status?: number } }
  return e.response?.data?.error || 'Workflow service unavailable. Please retry; no approval has been bypassed.'
}
// A screen's own controller answers errors with a plain string; a save that went through the
// workflow adapter answers { error, code }. Show the business message either way.
export function apiErrorMessage(error: unknown, fallback: string): string {
  const e = error as { response?: { data?: unknown }; message?: string }
  const data = e?.response?.data
  if (typeof data === 'string' && data.trim()) return data
  const body = (data ?? {}) as { error?: string; message?: string; Message?: string }
  return body.error || body.message || body.Message || e?.message || fallback
}
export const workflowChanged = () => window.dispatchEvent(new Event('workflowChanged'))

// A manually retried, unchanged request reuses its key after a network failure.
// Keys live in memory only and are reset on account/session change. No draft is persisted here.
let mutationSession = ''
const mutationKeys = new Map<string, string>()
export async function workflowMutation(method: 'post' | 'put' | 'delete', url: string, data?: unknown, previousInstanceId?: string, entityId?: number) {
  const session = `${localStorage.getItem('authSessionStartedAt') || ''}:${localStorage.getItem('user') || ''}`
  if (session !== mutationSession) { mutationKeys.clear(); mutationSession = session }
  const fingerprint = JSON.stringify([method, url, data, previousInstanceId, entityId ?? localStorage.getItem('selectedCampusId')])
  const key = mutationKeys.get(fingerprint) || randomUUId()
  mutationKeys.set(fingerprint, key)
  // Delete the key only after success: a gateway error can hide a committed result.
  const response = await api.request({ method, url, data, headers: {
    'Idempotency-Key': key,
    ...(previousInstanceId ? { 'X-Workflow-Previous-Instance': previousInstanceId } : {}),
    ...(entityId ? { 'X-Entity-Id': String(entityId) } : {}),
  // Like PostService/PutService/DeleteService, a mutation must not leave stale cached GETs behind.
  } }).finally(invalidateGetCache)
  mutationKeys.delete(fingerprint)
  return response
}
export function pendingWorkflow(response: AxiosResponse): boolean {
  if (response.status !== 202 || response.data?.disposition !== 'PendingApproval') return false
  toast(`Submitted for approval. Track this request in Workflow Inbox.`, { duration: 6500 })
  workflowChanged()
  return true
}
export const newStep = (): Step => ({ name: 'Approval', decisionKind: 'Approval', quorum: 'Any', negativeBehavior: 'Terminate', assigneeScope: 'Campus', allowSelfAction: false, requirePositiveComment: false, requireNegativeComment: true, assignees: [], relatedUser: null, endOnPositiveDecision: false })
