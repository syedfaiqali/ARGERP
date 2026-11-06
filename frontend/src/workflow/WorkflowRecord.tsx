import Autocomplete from "@/components/ClearableAutocomplete";
import { lazy, Suspense, useCallback, useEffect, useRef, useState, type ComponentType } from 'react'
import { Alert, Box, Button, Chip, Stack, TextField, Typography } from "@mui/material";
import toast from 'react-hot-toast'
import api from '@/services/api'
import { Layout } from '@/components/Layout'
import { randomUUId } from '@/utils/uuid'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { Detail, pendingWorkflow, wfGet, wfPost, workflowChanged, workflowError, workflowMutation, type WorkflowFormProps } from './api'
import { WorkflowDetail, WorkflowEdits, WorkflowHistory } from './WorkflowDetail'
import { formatWorkflowTime } from './time'

type Field = { key: string; label: string; multiline?: boolean; number?: boolean; required?: boolean; max?: number; lookupLabel?: string; readOnly?: boolean }
type Lookup = { id: number; name: string }
function RevisionLookup({ field, detail, values, onChange }: { field: Field; detail: Detail; values: Record<string, unknown>; onChange: (value: Record<string, unknown>) => void }) {
  const [search, setSearch] = useState('')
  const [options, setOptions] = useState<Lookup[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  useEffect(() => {
    const abort = new AbortController()
    const timer = window.setTimeout(async () => {
      setLoading(true); setError('')
      try {
        const endpoint = field.key === 'countryId'
          ? '/api/organization/Country/v3?pageNumber=1&pageSize=1000' : '/api/organization/Department/v3/ficoa-lookup'
        const response = await api.get(endpoint, { signal: abort.signal, params: { searchTerm: search } })
        const rows = Array.isArray(response.data) ? response.data : response.data?.items ?? response.data?.Items ?? []
        const normalized = rows.map((row: Record<string, unknown>) => ({ id: Number(row.id ?? row.Id ?? row.countryid), name: String(row.name ?? row.Name ?? row.title ?? row.countryName ?? row.countryname ?? '') }))
        if (!abort.signal.aborted) setOptions(normalized)
      } catch { if (!abort.signal.aborted) setError('Unable to load choices. Try searching again.') }
      finally { if (!abort.signal.aborted) setLoading(false) }
    }, 250)
    return () => { abort.abort(); clearTimeout(timer) }
  }, [field.key, search])
  const id = Number(values[field.key]) || null
  const selected = id ? options.find(o => o.id === id) || { id, name: String(values[field.lookupLabel!] ?? detail.after?.[field.lookupLabel!] ?? `Selected #${id}`) } : null
  const choices = selected && !options.some(o => o.id === selected.id) ? [selected, ...options] : options
  return <Autocomplete options={choices} value={selected} loading={loading} getOptionLabel={o => o.name} isOptionEqualToValue={(a, b) => a.id === b.id}
    onInputChange={(_, value, reason) => { if (reason === 'input') setSearch(value) }}
    onChange={(_, value) => onChange({ ...values, [field.key]: value?.id ?? null, [field.lookupLabel!]: value?.name ?? '' })}
    renderInput={props => <TextField {...props} label={field.label} required={field.required} error={Boolean(error)} helperText={error || 'Choose an active record; validation is repeated on submission.'} />} />
}
const fields: Record<string, Field[]> = {
  SMAuthor: [
    { key: 'code', label: 'Code', required: true, max: 10 }, { key: 'name', label: 'Name', required: true, max: 50 },
    { key: 'authorTypeId', label: 'Author Type', number: true, required: true, lookupLabel: 'authorTypeName' },
    { key: 'nickname', label: 'Nickname', max: 50 }, { key: 'countryId', label: 'Country', number: true, required: true, lookupLabel: 'countryName' },
    { key: 'yearBorn', label: 'Year Born' }, { key: 'yearDied', label: 'Year Died' }, { key: 'bio', label: 'Biography', multiline: true, max: 150 },
    { key: 'publications', label: 'Publications', multiline: true, max: 500 }, { key: 'awards', label: 'Awards', multiline: true, max: 500 }, { key: 'references', label: 'References', multiline: true, max: 500 },
  ],
  Department: [
    { key: 'code', label: 'Code', readOnly: true }, { key: 'departmentName', label: 'Department', required: true, max: 50 },
    { key: 'description', label: 'Description', multiline: true, max: 500 },
    { key: 'chartOfAccountId', label: 'Chart of Account', number: true, lookupLabel: 'chartOfAccount' },
    { key: 'receivingChartOfAccountId', label: 'Receiving Chart of Account', number: true, lookupLabel: 'receivingChartOfAccount' },
  ],
  Vehicletype: [
    { key: 'vehicleTypeCode', label: 'Code', required: true, max: 3 }, { key: 'vehicleTypeName', label: 'Vehicle Type', required: true, max: 50 },
    { key: 'description', label: 'Description', multiline: true, max: 150 },
  ],
  ERQualification: [
    { key: 'code', label: 'Code', required: true }, { key: 'qualificationTypeId', label: 'Qualification Type', number: true, required: true },
    { key: 'qualification', label: 'Qualification', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  ERQualificationtype: [
    { key: 'code', label: 'Code', required: true }, { key: 'name', label: 'Qualification Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  ERSkill: [
    { key: 'code', label: 'Code', required: true }, { key: 'skillTypeId', label: 'Skill Type', number: true, required: true },
    { key: 'name', label: 'Skill', required: true }, { key: 'ratingModelId', label: 'Rating Model', number: true, required: true },
    { key: 'description', label: 'Description', multiline: true },
  ],
  ERSkilltype: [
    { key: 'code', label: 'Code', required: true }, { key: 'name', label: 'Skill Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  ERPositiontype: [
    { key: 'code', label: 'Code', required: true }, { key: 'employeeTypeId', label: 'Employee Type', number: true, required: true },
    { key: 'name', label: 'Position Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  HRDocType: [
    { key: 'code', label: 'Code', required: true }, { key: 'documentType', label: 'Document Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  // Group and period detail lines are carried unchanged by "revise and resubmit"; only headers are shown here.
  EPEarningtype: [
    { key: 'code', label: 'Code' }, { key: 'earningType', label: 'Earning Type', required: true }, { key: 'description', label: 'Description', multiline: true },
    { key: 'payslipIndex', label: 'Payslip Index' }, { key: 'payslipDescription', label: 'Payslip Description' },
  ],
  EPEarninggrphdr: [{ key: 'code', label: 'Code' }, { key: 'earningGroup', label: 'Earning Group', required: true }],
  EPDeductiontype: [
    { key: 'code', label: 'Code' }, { key: 'deductionType', label: 'Deduction Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  EPDeductiongrphdr: [{ key: 'code', label: 'Code' }, { key: 'deductionGroup', label: 'Deduction Group', required: true }],
  ETPayperioddtl: [
    { key: 'code', label: 'Code' }, { key: 'ltxt', label: 'Description', multiline: true }, { key: 'noofperiod', label: 'No. of Periods', number: true },
  ],
  ETHolidaytypeList: [
    { key: 'code', label: 'Code', required: true }, { key: 'holidayType', label: 'Holiday Type', required: true },
    { key: 'description', label: 'Description', multiline: true }, { key: 'colorCode', label: 'Color Code' },
  ],
  HRAtnactivity: [
    { key: 'orderId', label: 'Order', number: true }, { key: 'activity', label: 'Activity', required: true }, { key: 'value', label: 'Value', number: true },
  ],
  DeviceType: [
    { key: 'code', label: 'Code', required: true }, { key: 'name', label: 'Device Type', required: true }, { key: 'description', label: 'Description', multiline: true },
  ],
  SMMachineRegistration: [
    { key: 'code', label: 'Code', required: true }, { key: 'deviceName', label: 'Device Name', required: true },
    { key: 'serialNumber', label: 'Serial Number' }, { key: 'deviceIp', label: 'Device IP' },
  ],
  ETPolicyhdr: [
    { key: 'code', label: 'Code' }, { key: 'stxt', label: 'Attendance Policy', required: true }, { key: 'noOfExcuse', label: 'No. of Excuses', number: true },
  ],
}
export function WorkflowRecordFields({ detail, values = detail.after ?? {}, onChange }: { detail: Detail; values?: Record<string, unknown>; onChange?: (values: Record<string, unknown>) => void }) {
  // Submitted values are unavailable when their encryption key is missing; the request's own pages still open.
  if (detail.payloadError) return <Alert severity="info" sx={{ my: 2 }}>{detail.payloadError}</Alert>
  const previous = detail.before ?? {}
  return <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2, py: 2 }}>
    {(fields[detail.screenKey] || []).map(field => {
      if (onChange && field.lookupLabel) return <RevisionLookup key={field.key} field={field} detail={detail} values={values} onChange={onChange} />
      const changed = detail.operation === 'Update' && String(previous[field.key] ?? '') !== String(values[field.key] ?? '')
      const value = !onChange && field.lookupLabel ? values[field.lookupLabel] ?? values[field.key] : values[field.key]
      const before = field.lookupLabel ? previous[field.lookupLabel] ?? previous[field.key] : previous[field.key]
      return <TextField key={field.key} label={field.label} value={value ?? ''} required={field.required} multiline={field.multiline} minRows={field.multiline ? 3 : undefined}
        type={onChange && field.number ? 'number' : 'text'} InputProps={{ readOnly: !onChange || field.readOnly }} disabled={field.readOnly} placeholder={field.readOnly ? 'Auto generated' : undefined} inputProps={{ maxLength: field.max, min: field.number ? 1 : undefined }}
        helperText={changed ? `Changed · Previously: ${String(before ?? '—')}` : undefined}
        color={changed ? 'warning' : 'primary'} focused={changed || undefined}
        onChange={e => onChange?.({ ...values, [field.key]: field.number ? (e.target.value ? Number(e.target.value) : null) : e.target.value })} />
    })}
  </Box>
}
export function WorkflowRecordPage() {
  const [params] = useSearchParams()
  const id = params.get('workflowInstance') || ''
  const [detail, setDetail] = useState<Detail | null>(null)
  const [values, setValues] = useState<Record<string, unknown>>({})
  const [editing, setEditing] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const navigate = useNavigate()
  useEffect(() => {
    const abort = new AbortController(); setDetail(null); setEditing(false); setError('')
    wfGet<Detail>(`/instances/${id}`, abort.signal).then(row => { if (!abort.signal.aborted) { setDetail(row); setValues(row.payload ?? {}) } }).catch(e => { if (!abort.signal.aborted) setError(workflowError(e)) })
    return () => abort.abort()
  }, [id])
  const resubmit = async () => {
    if (!detail) return
    setBusy(true); setError('')
    try {
      const endpoints: Record<string, string> = { Department: '/api/organization/Department/v3',
        ERQualification: '/api/HumanResource/Qualification/v3', ERQualificationtype: '/api/HumanResource/QualificationType/v3', ERSkill: '/api/HumanResource/Skill/v3',
        ERSkilltype: '/api/HumanResource/SkillType/v3', ERPositiontype: '/api/HumanResource/PositionType/v3', HRDocType: '/api/HumanResource/HRDocumentType/v3',
        EPEarningtype: '/api/HumanResource/EarningType/v3', EPEarninggrphdr: '/api/HumanResource/EarningGroup/v3', EPDeductiontype: '/api/HumanResource/DeductionType/v3',
        EPDeductiongrphdr: '/api/HumanResource/DeductionGroup/v3', ETPayperioddtl: '/api/HumanResource/PayrollPeriod/v3',
        ETHolidaytypeList: '/api/HumanResource/HolidayType/v3', HRAtnactivity: '/api/HumanResource/AttendanceActivity/v3', DeviceType: '/api/HumanResource/DeviceType/v3',
        SMMachineRegistration: '/api/HumanResource/DeviceRegistration/v3', ETPolicyhdr: '/api/HumanResource/AttendancePolicy/v3' }
      const endpoint = endpoints[detail.screenKey]
      if (!endpoint) throw new Error('Unsupported screen')
      const response = await workflowMutation(detail.operation === 'Create' ? 'post' : detail.operation === 'Update' ? 'put' : 'delete',
        endpoint + (detail.operation === 'Create' ? '' : `/${detail.recordKey}`),
        detail.operation === 'Delete' ? undefined : { ...values, ...(detail.recordKey ? { id: Number(detail.recordKey) } : {}) }, detail.id, detail.entityId)
      if (pendingWorkflow(response)) navigate(`/RequestNotifications?instance=${response.data.instanceId}`)
      else navigate(detail.route)
    } catch (e) { setError(workflowError(e)) }
    finally { setBusy(false) }
  }
  return <Box sx={{ p: 3 }}>
    <Stack direction="row" spacing={1}>
      <Button onClick={() => navigate('/RequestNotifications')}>Back to workflow inbox</Button>
      {detail && detail.operation !== 'Delete' && <Button component={Link} to={`${detail.route}?workflowForm=${detail.id}`}>Open form record</Button>}
    </Stack>
    {error && <Alert severity="error">{error}</Alert>}
    {!detail && !error && <Typography role="status">Loading authorized record preview…</Typography>}
    {detail && <>
      <Typography variant="h5">{detail.subject}</Typography>
      <Stack direction="row" spacing={1} sx={{ my: 2 }}><Chip label={detail.operation} /><Chip label={detail.status} /><Chip label={detail.entityId ? `Campus ${detail.entityId}` : 'Tenant-wide record'} /></Stack>
      <Alert severity={detail.operation === 'Delete' ? 'warning' : 'info'}>
        {detail.operation === 'Delete' ? 'Deletion request: this record remains unchanged until final approval.' : 'Workflow preview: these are submitted values, not a live editable business record.'}
      </Alert>
      <WorkflowRecordFields detail={detail} values={editing ? values : detail.after ?? {}} onChange={editing && detail.operation !== 'Delete' ? setValues : undefined} />
      {detail.canResubmit && !detail.payloadError && !false && <Stack direction="row" spacing={1}>
        {!editing ? <Button onClick={() => setEditing(true)}>Revise and resubmit</Button> : <>
          <Button disabled={busy} variant="contained" onClick={resubmit}>{detail.operation === 'Delete' ? 'Resubmit deletion for approval' : 'Submit a new revision'}</Button>
          <Button disabled={busy} onClick={() => { setEditing(false); setValues(detail.payload ?? {}) }}>Discard edits</Button>
        </>}
      </Stack>}
      {editing && <Alert sx={{ mt: 2 }} severity="info">A new revision restarts the entire approval chain. Earlier decisions and the original submission remain in history.</Alert>}
      <WorkflowDetail id={id} showRecordLink={false} />
    </>}
  </Box>
}
// Screens whose own form opens a pending request (it takes a workflowForm prop). Any other screen
// falls back to its submitted fields, so every governed screen has a form record.
const screenForms: Record<string, ComponentType<{ workflowForm: WorkflowFormProps }>> = {}
function WorkflowFieldsForm({ detail, readOnly, saving, onSave }: WorkflowFormProps) {
  const [values, setValues] = useState<Record<string, unknown>>(detail.payload ?? {})
  useEffect(() => setValues(detail.payload ?? {}), [detail])
  return <>
    <WorkflowRecordFields detail={detail} values={readOnly ? detail.after ?? {} : values} onChange={readOnly ? undefined : setValues} />
    {!readOnly && <Stack direction="row" spacing={1}>
      <Button variant="contained" disabled={saving} onClick={() => onSave(values)}>Save changes</Button>
      <Button disabled={saving} onClick={() => setValues(detail.payload ?? {})}>Discard changes</Button>
    </Stack>}
  </>
}
/** The requester's screen form on a request. A user whose role may edit that screen changes its values in place. */
export function WorkflowFormPage() {
  const [params] = useSearchParams()
  const id = params.get('workflowForm') || ''
  const [detail, setDetail] = useState<Detail | null>(null)
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)
  const keys = useRef(new Map<string, string>())
  const navigate = useNavigate()
  const load = useCallback((signal?: AbortSignal) => wfGet<Detail>(`/instances/${id}`, signal)
    .then(row => { if (!signal?.aborted) setDetail(row) })
    .catch(e => { if (!signal?.aborted) setError(workflowError(e)) }), [id])
  useEffect(() => {
    const abort = new AbortController(); setDetail(null); setError(''); keys.current.clear()
    void load(abort.signal)
    return () => abort.abort()
  }, [load])
  const save = async (payload: Record<string, unknown>) => {
    if (!detail || saving) return false
    setSaving(true); setError('')
    // A save retried after a network failure reuses its key, so the change is recorded once.
    const fingerprint = JSON.stringify([detail.version, payload])
    const key = keys.current.get(fingerprint) || randomUUId(); keys.current.set(fingerprint, key)
    try {
      await wfPost(`/instances/${id}/amend`, { payload, expectedVersion: detail.version, idempotencyKey: key })
      keys.current.delete(fingerprint); workflowChanged()
      toast.success(`Your changes to ${detail.submitterName}'s request were saved.`)
      await load(); return true
    } catch (e) {
      // The form sits below the page's error banner, so the reason is also raised where the user is looking.
      const message = workflowError(e); setError(message); toast.error(message); return false
    }
    finally { setSaving(false) }
  }
  const Form = detail ? screenForms[detail.screenKey] : undefined
  const editable = Boolean(detail?.canAmend)
  return <Box sx={{ p: 3 }}>
    <Stack direction="row" spacing={1}>
      <Button onClick={() => navigate(`/RequestNotifications?instance=${id}`)}>Back to workflow request</Button>
      {detail && <Button component={Link} to={`${detail.route}?workflowInstance=${id}`}>Audit trail</Button>}
    </Stack>
    {error && <Alert severity="error" sx={{ my: 1 }}>{error}</Alert>}
    {!detail && !error && <Typography role="status">Loading form record…</Typography>}
    {detail && <Stack spacing={2} sx={{ mt: 1 }}>
      <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
        <Typography variant="h5">{detail.subject}</Typography><Chip label={detail.operation} /><Chip label={detail.status} />
      </Stack>
      <Typography color="text.secondary">{detail.screenName} · Submitted by {detail.submitterName} · {formatWorkflowTime(detail.createdAtUtc)}</Typography>
      <Alert severity="info">{editable
        ? `Your edit rights on this screen let you change ${detail.submitterName}'s request. Your name and every changed value are recorded in its history, and ${detail.submitterName} is notified. The record itself changes only after the final approval.`
        : `View only. ${detail.amendBlocked ?? ''}`}</Alert>
      <WorkflowEdits detail={detail} />
      {detail.payloadError ? <Alert severity="info">{detail.payloadError}</Alert> : Form
        ? <Suspense fallback={<Typography role="status">Loading form…</Typography>}><Form workflowForm={{ detail, readOnly: !editable, saving, onSave: save }} /></Suspense>
        : <WorkflowFieldsForm detail={detail} readOnly={!editable} saving={saving} onSave={save} />}
      {/* A screen's own form pads its sections; the history lines up with them. */}
      <Box sx={{ px: Form ? 4 : 0 }}><WorkflowHistory detail={detail} /></Box>
    </Stack>}
  </Box>
}
/** Same screen URL, with a record-scoped grant instead of list-screen permissions. */
export function WorkflowAwareScreen({ children }: { children: React.ReactNode }) {
  const [params] = useSearchParams()
  if (params.has('workflowForm')) return <Layout breadcrumb="Workflow Form Record"><WorkflowFormPage /></Layout>
  return params.has('workflowInstance') ? <Layout breadcrumb="Workflow Record Preview"><WorkflowRecordPage /></Layout> : <>{children}</>
}
