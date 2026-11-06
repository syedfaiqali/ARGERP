import { useCallback, useEffect, useRef, useState } from 'react'
import { Alert, Box, Button, Chip, LinearProgress, Paper, Stack, Typography } from '@mui/material'
import {
  AccountTreeOutlined, AdminPanelSettingsOutlined, ArrowForwardRounded, HistoryRounded,
  InboxOutlined, RefreshRounded, ReplayRounded, SendOutlined, SettingsOutlined, TuneRounded,
} from '@mui/icons-material'
import { Link, useSearchParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { FormDatePicker } from '@/components/FormDatePicker'
import { FormInput } from '@/components/FormInput'
import { FormThemeWrapper } from '@/components/FormThemeWrapper'
import { LmsSelect } from '@/components/LmsSelect'
import { Modal } from '@/components/Modal'
import { SchemaTable } from '@/components/SchemaTable'
import { StyledTabs } from '@/components/StyledTabs'
import { wfGet, workflowEnabled, workflowError, workflowChanged } from './api'
import type { Identity, InstanceRow, Registration } from './api'
import { WorkflowDetail } from './WorkflowDetail'
import { formatWorkflowTime } from './time'
import { useGridPageSize } from '@/hooks/useGridPageSize'

const emptyFilters = { screenKey: '', action: '', status: '', submitterId: '', from: '', to: '' }
const statuses = ['Pending', 'ConfigurationBlocked', 'PendingExecution', 'Executing', 'Completed', 'Rejected', 'Returned', 'Withdrawn', 'Canceled', 'NeedsResubmission']
const statusColors = {
  Pending: 'warning', ConfigurationBlocked: 'error', PendingExecution: 'info', Executing: 'info',
  Completed: 'success', Rejected: 'error', Returned: 'warning', NeedsResubmission: 'warning',
} as const
const statusLabel = (value: string) => value.replace(/([a-z])([A-Z])/g, '$1 $2')
const panelSx = { border: '1px solid var(--lms-border, #e8ece5)', borderRadius: '16px', bgcolor: 'var(--lms-bg, #fff)' }
const buttonSx = { textTransform: 'none', borderRadius: '10px', fontWeight: 700, boxShadow: 'none' } as const

export function WorkflowInbox() {
  const [params, setParams] = useSearchParams()
  const [me, setMe] = useState<Identity | null>(null)
  const [catalog, setCatalog] = useState<Registration[]>([])
  const [view, setView] = useState('pending')
  const [filters, setFilters] = useState(emptyFilters)
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useGridPageSize(() => setPage(0))
  const [rows, setRows] = useState<InstanceRow[]>([])
  const [total, setTotal] = useState(0)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const generation = useRef(0)
  const cancelPending = useCallback(() => { generation.current++ }, [])
  useEffect(() => {
    if (!workflowEnabled) return
    const abort = new AbortController()
    Promise.all([wfGet<Identity>('/me', abort.signal), wfGet<Registration[]>('/actions', abort.signal)])
      .then(([identity, actions]) => { if (!abort.signal.aborted) { setMe(identity); setCatalog(actions) } }).catch(e => { if (!abort.signal.aborted) setError(workflowError(e)) })
    return () => abort.abort()
  }, [])
  const load = useCallback(async () => {
    if (!workflowEnabled) return
    const current = ++generation.current
    setLoading(true)
    const query = new URLSearchParams({ view, page: String(page + 1), pageSize: String(pageSize) })
    Object.entries(filters).forEach(([key, value]) => { if (value) query.set(key, key === 'to' ? `${value}T23:59:59.999` : value) })
    try {
      const data = await wfGet<{ items: InstanceRow[]; totalCount: number }>(`/instances?${query}`)
      if (generation.current === current) { setRows(data.items); setTotal(data.totalCount); setError('') }
    } catch (e) { if (generation.current === current) setError(workflowError(e)) }
    finally { if (generation.current === current) setLoading(false) }
  }, [view, filters, page, pageSize])
  useEffect(() => { void load(); window.addEventListener('workflowChanged', load); return () => { cancelPending(); window.removeEventListener('workflowChanged', load) } }, [load, cancelPending])

  const tabs = [
    { value: 'pending', label: 'Pending my action', icon: <AccountTreeOutlined />, description: 'Review requests waiting for your approval or recommendation.' },
    { value: 'mine', label: 'My requests', icon: <SendOutlined />, description: 'Track the progress of requests you have submitted.' },
    { value: 'history', label: 'History', icon: <HistoryRounded />, description: 'Review past workflow requests and their outcomes.' },
    ...((me?.isTenantAdmin || me?.isPlatformAdmin) ? [{ value: 'admin', label: 'Administration', icon: <AdminPanelSettingsOutlined />, description: 'Review workflow requests available to your administration role.' }] : []),
  ]
  const activeTab = Math.max(0, tabs.findIndex(tab => tab.value === view))
  const filterCount = Object.values(filters).filter(Boolean).length
  const changeFilter = (key: keyof typeof emptyFilters, value: string) => {
    setFilters(current => ({ ...current, [key]: value }))
    setPage(0)
  }

  if (!workflowEnabled) return <Alert severity="info">Workflow Management is not enabled in this deployment.</Alert>
  return <FormThemeWrapper>
    <Box sx={{ p: { xs: 1.5, md: 2.5 }, display: 'grid', gap: 2, minWidth: 0, color: 'var(--lms-text-main)' }}>
      <Paper elevation={0} sx={{ ...panelSx, p: { xs: 2, md: 2.5 } }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" alignItems={{ xs: 'stretch', sm: 'center' }} gap={2}>
          <Stack direction="row" alignItems="center" gap={1.5}>
            <Box sx={{ width: 46, height: 46, flexShrink: 0, display: 'grid', placeItems: 'center', borderRadius: '13px', bgcolor: 'var(--lms-primary-soft)', color: 'var(--lms-primary)' }}><InboxOutlined /></Box>
            <Box>
              <Typography component="h1" sx={{ fontSize: { xs: '1.25rem', md: '1.5rem' }, fontWeight: 850, lineHeight: 1.3 }}>Workflow Inbox</Typography>
              <Typography variant="body2" sx={{ mt: 0.5, color: 'var(--lms-text-secondary)', fontSize: '0.82rem' }}>{tabs[activeTab].description}</Typography>
            </Box>
          </Stack>
          <Stack direction="row" gap={1} flexWrap="wrap">
            <Button variant="outlined" startIcon={<RefreshRounded />} onClick={workflowChanged} disabled={loading} sx={buttonSx}>Refresh</Button>
            {me?.isPlatformAdmin && <Button variant="contained" startIcon={<SettingsOutlined />} component={Link} to="/security/WorkflowManagement" sx={buttonSx}>Manage workflows</Button>}
          </Stack>
        </Stack>
        <StyledTabs value={activeTab} onChange={(_, index) => { setView(tabs[index].value); setPage(0) }} tabs={tabs} variant="scrollable" scrollButtons="auto" allowScrollButtonsMobile
          sx={{ mb: 0, mt: 2, '& .MuiTab-root': { fontSize: '0.82rem', minHeight: 44, px: { xs: 1.5, md: 2 } } }} />
      </Paper>

      <Box role="tabpanel" id={`tabpanel-${activeTab}`} aria-labelledby={`tab-${activeTab}`} sx={{ display: 'grid', gap: 2, minWidth: 0 }}>
      <Paper component="section" aria-label="Workflow filters" elevation={0} sx={{ ...panelSx, p: 2 }}>
        <Stack direction="row" alignItems="center" justifyContent="space-between" gap={1} sx={{ mb: 2 }}>
          <Stack direction="row" alignItems="center" gap={1}>
            <TuneRounded sx={{ fontSize: 19, color: 'var(--lms-primary)' }} />
            <Typography variant="subtitle2" sx={{ fontWeight: 800 }}>Filters</Typography>
            {filterCount > 0 && <Chip size="small" label={`${filterCount} applied`} sx={{ height: 23, bgcolor: 'var(--lms-primary-soft)', color: 'var(--lms-primary-dark)', fontSize: '0.7rem', fontWeight: 700 }} />}
          </Stack>
          <Button size="small" startIcon={<ReplayRounded />} disabled={!filterCount} onClick={() => { setFilters(emptyFilters); setPage(0) }} sx={buttonSx}>Clear filters</Button>
        </Stack>
        <Box sx={{ display: 'grid', gap: 1.5, gridTemplateColumns: { xs: 'minmax(0, 1fr)', sm: 'repeat(2, minmax(0, 1fr))', lg: 'repeat(3, minmax(0, 1fr))', xl: '1.5fr 1fr 1.2fr 1fr 1.15fr 1.15fr' }, '& > *': { minWidth: 0 } }}>
          <LmsSelect label="Screen" shrinkLabel value={filters.screenKey} options={[{ value: '', label: 'All screens' }, ...catalog.map(action => ({ value: action.screenKey, label: action.name }))]} onChange={value => changeFilter('screenKey', String(value ?? ''))} />
          <LmsSelect label="Action" shrinkLabel value={filters.action} options={[{ value: '', label: 'All actions' }, { value: 'Save', label: 'Save' }, { value: 'Delete', label: 'Delete' }]} onChange={value => changeFilter('action', String(value ?? ''))} />
          <LmsSelect label="Status" shrinkLabel value={filters.status} options={[{ value: '', label: 'All statuses' }, ...statuses.map(status => ({ value: status, label: statusLabel(status) }))]} onChange={value => changeFilter('status', String(value ?? ''))} />
          <FormInput label="Requester ID" type="number" value={filters.submitterId} inputProps={{ min: 1 }} onChange={event => changeFilter('submitterId', event.target.value)} />
          <FormDatePicker label="Received from" value={filters.from ? dayjs(filters.from) : null} maxDate={filters.to ? dayjs(filters.to) : undefined} onChange={date => { if (!date || date.isValid()) changeFilter('from', date ? date.format('YYYY-MM-DD') : '') }} />
          <FormDatePicker label="Received through" value={filters.to ? dayjs(filters.to) : null} minDate={filters.from ? dayjs(filters.from) : undefined} onChange={date => { if (!date || date.isValid()) changeFilter('to', date ? date.format('YYYY-MM-DD') : '') }} />
        </Box>
      </Paper>

      {error && <Alert severity="error" action={<Button color="inherit" size="small" onClick={() => void load()}>Retry</Button>}>{error}</Alert>}


      <Box component="section" aria-label="Workflow requests" aria-busy={loading} sx={{ minWidth: 0 }}>
        <Stack direction="row" alignItems="center" gap={1} sx={{ px: 0.5, mb: 1.5 }}>
          <Typography component="h2" sx={{ fontSize: '1.05rem', fontWeight: 800 }}>Workflow requests</Typography>
          <Chip size="small" label={loading ? 'Loading…' : total.toLocaleString()} sx={{ height: 24, bgcolor: 'var(--lms-primary-soft)', color: 'var(--lms-primary-dark)', fontWeight: 700, fontSize: '0.72rem' }} />
        </Stack>
        {loading && <LinearProgress aria-label="Loading workflow requests" sx={{ mb: 1, borderRadius: 2 }} />}
        <SchemaTable title="Workflow requests" hideTitle hideBreadcrumbs rows={rows} columns={[
          { id: 'subject', label: 'Record / request', minWidth: 190, format: (value: string, row?: InstanceRow) => <Box><Typography variant="body2" sx={{ fontWeight: 700 }}>{value}</Typography>{row?.createdAtUtc && <Typography variant="caption" color="text.secondary">{formatWorkflowTime(row.createdAtUtc)}</Typography>}</Box> },
          { id: 'screenKey', label: 'Screen', minWidth: 125, format: (value: string) => catalog.find(action => action.screenKey === value)?.name || value },
          { id: 'operation', label: 'Action', minWidth: 90 },
          { id: 'status', label: 'Status', minWidth: 155, format: (value: string) => <Chip size="small" variant="outlined" color={statusColors[value as keyof typeof statusColors] ?? 'default'} label={statusLabel(value)} sx={{ fontSize: '0.72rem', fontWeight: 700 }} /> },
          { id: 'submitterName', label: 'Requester', minWidth: 120 },
          { id: 'definitionName', label: 'Workflow', minWidth: 150 },
        ]} showAddButton={false} showEditAction={false} showDeleteAction={false} showViewAction={true} useActionRights={false} downloadEnabled={false}
          onViewClick={row => setParams({ instance: row.id })} currentPage={page + 1} rowsPerPage={pageSize} totalCount={total} onPageChange={value => setPage(value - 1)} onRowsPerPageChange={setPageSize} defaultLatestFirst={false} maxHeight="clamp(320px, 52vh, 640px)" />
      </Box>
      </Box>
      <Modal open={Boolean(params.get('instance'))} onClose={() => setParams({})} title="Workflow request" maxWidth="md" showActions={false} contentPadding={2.5}>
        {params.get('instance') && <WorkflowDetail id={params.get('instance')!} onChanged={load} />}
      </Modal>
    </Box>
  </FormThemeWrapper>
}
