import { useCallback, useEffect, useRef, useState } from 'react'
import { Alert, Box, Button, Chip, CircularProgress, Divider, Stack, TextField, Typography } from '@mui/material'
import { Link } from 'react-router-dom'
import { Detail, relatedUserLabels, wfGet, wfPost, workflowChanged, workflowError } from './api'
import { randomUUId } from '@/utils/uuid'
import { formatWorkflowTime } from './time'

// Other users' changes to the requester's values, newest first, so whoever opens the request sees them before acting.
export function WorkflowEdits({ detail }: { detail: Detail }) {
  const edits = detail.events.filter(event => event.kind === 'Edited').reverse()
  if (edits.length === 0) return null
  return <Alert severity="warning">
    <Typography variant="subtitle2">Changed after submission</Typography>
    {edits.map(event => <Box key={event.id} sx={{ mt: 1 }}>
      <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', overflowWrap: 'anywhere' }}>{event.comment}</Typography>
      <Typography variant="caption" color="text.secondary">{formatWorkflowTime(event.atUtc)}</Typography>
    </Box>)}
  </Alert>
}

const historyLabels: Record<string, string> = {
  Submitted: 'Created and submitted', Edited: 'Updated', Approve: 'Approved', Reject: 'Rejected', Recommend: 'Recommended', Decline: 'Declined',
  Return: 'Returned for changes', Escalated: 'Passed to the next step', StepsSkipped: 'Later steps skipped', Comment: 'Commented',
  withdraw: 'Withdrawn', cancel: 'Canceled', resolve: 'Assignments re-resolved', retry: 'Saving retried', Completed: 'Saved to the record',
  ExecutionAttemptFailed: 'Saving to the record failed', DirectoryRecovered: 'Assigned after the directory recovered',
  RelatedUserAssigned: 'Related user assigned', RelatedUserNotFound: 'Related user not found',
}
const historyColor = (kind: string) => ['Approve', 'Recommend', 'Completed'].includes(kind) ? 'success.main'
  : ['Reject', 'Decline', 'cancel', 'ExecutionAttemptFailed'].includes(kind) ? 'error.main'
  : ['Edited', 'Return', 'withdraw', 'Escalated'].includes(kind) ? 'warning.main' : 'primary.main'

// Who created the request, who changed it and who decided at which step, oldest first, under the record's form.
export function WorkflowHistory({ detail }: { detail: Detail }) {
  const events = false ? detail.events.filter(event => event.kind !== 'ExecutionAttemptFailed') : detail.events
  return <Box>
    <Stack direction="row" spacing={1.5} alignItems="center" sx={{ mb: 2 }}>
      <Box sx={{ width: 4, height: 18, bgcolor: 'var(--lms-primary)', borderRadius: 1 }} />
      <Typography sx={{ fontWeight: 800, fontSize: '0.95rem' }}>History</Typography>
    </Stack>
    {events.map((event, index) => <Box key={event.id} sx={{ display: 'flex', gap: 2 }}>
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', pt: 0.75 }}>
        <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: historyColor(event.kind), flexShrink: 0 }} />
        {index < events.length - 1 && <Box sx={{ flex: 1, width: 2, bgcolor: 'divider', mt: 0.5 }} />}
      </Box>
      <Box sx={{ pb: 2, minWidth: 0 }}>
        <Typography variant="body2" sx={{ fontWeight: 700 }}>{historyLabels[event.kind] ?? event.kind}{event.stepName ? ` · ${event.stepName}` : ''}</Typography>
        <Typography variant="caption" color="text.secondary">
          {event.userId === 0 ? event.userName : `by ${event.userName}${event.roleName ? ` (${event.roleName})` : ''}`} · {formatWorkflowTime(event.atUtc)}
        </Typography>
        {event.comment && <Typography variant="body2" sx={{ mt: 0.5, whiteSpace: 'pre-wrap', overflowWrap: 'anywhere' }}>{event.comment}</Typography>}
      </Box>
    </Box>)}
  </Box>
}

export function WorkflowDetail({ id, showRecordLink = true, onChanged }: { id: string; showRecordLink?: boolean; onChanged?: () => void }) {
  const [detail, setDetail] = useState<Detail | null>(null)
  const [comment, setComment] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const [busyAction, setBusyAction] = useState('')
  const actionPending = useRef(false)
  const requestKeys = useRef(new Map<string, string>())
  const generation = useRef(0)
  const cancelPending = useCallback(() => { generation.current++ }, [])
  const load = useCallback(async () => {
    const current = ++generation.current
    try {
      const row = await wfGet<Detail>(`/instances/${id}`)
      if (generation.current === current) setDetail(row)
    } catch (e) { if (generation.current === current) setError(workflowError(e)) }
  }, [id])
  useEffect(() => {
    setDetail(null); setError(''); setComment(''); requestKeys.current.clear()
    void load()
    void wfPost(`/instances/${id}/read`).then(workflowChanged).catch(() => {})
    window.addEventListener('workflowChanged', load)
    return () => { cancelPending(); window.removeEventListener('workflowChanged', load) }
  }, [id, load, cancelPending])
  const act = async (action: string, kind: 'decisions' | 'commands' | 'comments') => {
    if (!detail || actionPending.current) return
    const step = detail.steps[detail.stepIndex]
    const required = kind === 'comments' || kind === 'commands' && action !== 'withdraw' || action === 'Return' ||
      (action === 'Reject' || action === 'Decline' ? step?.requireNegativeComment : step?.requirePositiveComment)
    if (required && !comment.trim()) { setError('Please add a comment explaining this action.'); return }
    actionPending.current = true
    setBusy(true); setBusyAction(action); setError('')
    const fingerprint = JSON.stringify([id, kind, action, detail.version, comment])
    const key = requestKeys.current.get(fingerprint) || randomUUId(); requestKeys.current.set(fingerprint, key)
    try {
      await wfPost(`/instances/${id}/${kind}${kind === 'commands' ? `/${action}` : ''}`, {
        decision: action, comment, expectedVersion: detail.version, idempotencyKey: key,
      })
      setComment(''); requestKeys.current.delete(fingerprint); workflowChanged(); onChanged?.()
    } catch (e) { setError(workflowError(e)) }
    finally { await load(); actionPending.current = false; setBusy(false); setBusyAction('') }
  }
  if (!detail) return <Box sx={{ p: 2 }}>{error ? <Alert severity="error">{error}</Alert> : <Typography role="status">Loading workflow…</Typography>}</Box>
  const step = detail.steps[detail.stepIndex]
  const finalLoanApproval = false
  const events = finalLoanApproval ? detail.events.filter(event => event.kind !== 'ExecutionAttemptFailed') : detail.events
  return <Stack spacing={2} sx={{ py: 2 }}>
    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap alignItems="center">
      <Typography variant="h6">{detail.definitionName}</Typography><Chip label={detail.status} color={finalLoanApproval ? 'success' : 'default'} />
      {showRecordLink && <Button component={Link} to={`${detail.route}?workflowInstance=${detail.id}`}>Audit trail{detail.canResubmit && !finalLoanApproval ? ' / revise' : ''}</Button>}
      {/* A deletion proposes no values, so it has no form to open. */}
      {showRecordLink && detail.operation !== 'Delete' && <Button component={Link} to={`${detail.route}?workflowForm=${detail.id}`}>Open form record</Button>}
    </Stack>
    <Typography color="text.secondary">{detail.subject} · Submitted by {detail.submitterName} · {formatWorkflowTime(detail.createdAtUtc)}</Typography>
    <WorkflowEdits detail={detail} />
    {detail.previousInstanceId && <Button component={Link} to={`/RequestNotifications?instance=${detail.previousInstanceId}`}>View previous revision</Button>}
    {finalLoanApproval && detail.status !== 'Completed' && <Alert severity="info">{'Record details are unavailable.'}</Alert>}
    {!finalLoanApproval && detail.payloadError && <Alert severity="info">{detail.payloadError}</Alert>}
    {!finalLoanApproval && detail.failureCode && <Alert severity="warning">{detail.failureCode}. {detail.status === 'Executing' ? 'Execution outcome needs reconciliation; cancellation is disabled to prevent an ambiguous result.' : 'An administrator can repair assignments, or the requester can revise a stale submission.'}</Alert>}
    <Stack spacing={1}>{detail.steps.map((s, index) => <Box key={index} sx={{ p: 1.5, border: 1, borderColor: index === detail.stepIndex ? 'primary.main' : 'divider', borderRadius: 1 }}>
      <Typography>{index + 1}. {s.name} · {s.decisionKind} · {s.quorum} · {s.endOnPositiveDecision && s.decisionKind === 'Approval' && index < detail.steps.length - 1 ? 'Approve ends the workflow · ' : ''}{s.negativeBehavior === 'Terminate' ? 'Hard negative' : 'Soft negative'}{s.relatedUser ? ` · Related user: ${relatedUserLabels[s.relatedUser]}` : ''}</Typography>
      <Typography variant="body2" color="text.secondary">{detail.tasks.filter(t => t.stepIndex === index).map(t => `${t.userName} (${t.roleName}): ${t.decision || t.status}${t.generation < detail.generation && index === detail.stepIndex ? ' (earlier assignment)' : ''}`).join(' · ')
        // Past the last step with no assignments means an earlier approval ended the workflow.
        || (detail.stepIndex >= detail.steps.length ? 'Skipped — an earlier approval ended the workflow' : 'Assignees resolve when this step starts')}</Typography>
    </Box>)}</Stack>
    {error && <Alert severity="error">{error}</Alert>}
    {detail.canComment && <TextField label="Comment / reason" multiline minRows={3} value={comment} onChange={e => setComment(e.target.value)} inputProps={{ maxLength: 4000 }} disabled={busy} />}
    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
      {detail.canDecide && step && <>
        <Button disabled={busy} variant="contained" startIcon={busy && ['Approve', 'Recommend'].includes(busyAction) ? <CircularProgress size={16} color="inherit" /> : undefined} onClick={() => act(step.decisionKind === 'Approval' ? 'Approve' : 'Recommend', 'decisions')}>{step.decisionKind === 'Approval' ? 'Approve' : 'Recommend'}</Button>
        <Button disabled={busy} color="error" variant="outlined" startIcon={busy && ['Reject', 'Decline'].includes(busyAction) ? <CircularProgress size={16} color="inherit" /> : undefined} onClick={() => act(step.decisionKind === 'Approval' ? 'Reject' : 'Decline', 'decisions')}>{step.decisionKind === 'Approval' ? 'Reject' : 'Decline'}</Button>
      </>}
      {detail.canComment && <Button disabled={busy || !comment.trim()} onClick={() => act('', 'comments')}>Add comment</Button>}
      {detail.canWithdraw && <Button disabled={busy} color="warning" onClick={() => act('withdraw', 'commands')}>Withdraw request</Button>}
      {detail.canRetry && <Button disabled={busy} onClick={() => act('retry', 'commands')}>Retry execution</Button>}
    </Stack>
    {busy && <Stack direction="row" spacing={1} alignItems="center" role="status" aria-live="polite">
      <CircularProgress size={20} /><Typography variant="body2">{['Approve', 'Recommend'].includes(busyAction) ? 'Saving approval…' : ['Reject', 'Decline'].includes(busyAction) ? 'Saving rejection…' : 'Saving workflow action…'}</Typography>
    </Stack>}
    <Divider /><Typography variant="subtitle1">Activity and comments</Typography>
    {events.map(event => <Box key={event.id}><Typography variant="body2">{event.kind} · {event.userName} · {formatWorkflowTime(event.atUtc)}</Typography>{event.comment && <Typography sx={{ whiteSpace: 'pre-wrap', overflowWrap: 'anywhere' }}>{event.comment}</Typography>}</Box>)}
  </Stack>
}
