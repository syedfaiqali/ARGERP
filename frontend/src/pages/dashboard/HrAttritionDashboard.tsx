import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { Alert, Button, ButtonBase, IconButton, Paper } from '@mui/material'
import dayjs, { type Dayjs } from 'dayjs'
import { FormDatePicker } from '@/components/FormDatePicker'
import { LmsSelect } from '@/components/LmsSelect'
import { DashboardTable, type Column } from '@/components/DashboardTable'
import { Modal } from '@/components/Modal'
import { LmsOverlayLoader } from '@/components/LmsOverlayLoader'
import { useAppSelector } from '@/hooks'
import { useHiddenMenuPermissions } from '@/hooks/useHiddenMenuPermissions'
import { getPolicyDateString } from '@/utils/regionalSettings'
import { emptyHrSummary, getHrDashboard, getHrDetails, type HrDashboardResponse, type HrDetailKind, type HrDetailResponse, type HrFilters, type HrMetricGroup, type HrLookup } from '@/services/hrAttritionDashboardService'
import { RefreshRounded } from '@mui/icons-material'
import { GroupsRounded, PersonAddAlt1Rounded, LogoutRounded, AutorenewRounded, EventSeatRounded, TrendingDownRounded, SchoolOutlined, BadgeOutlined, SupportAgentRounded, CalendarMonthRounded, LightbulbOutlined, TrackChangesRounded, ChevronRightRounded } from '@mui/icons-material'
import { Bar, BarChart, CartesianGrid, Cell, ComposedChart, Legend, Line, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import './hrAttritionDashboard.css'

const GREEN = '#2dc467'
const RED = '#ff525b'
const BLUE = 'var(--lms-primary)'
const chartTooltipStyle = { backgroundColor: 'var(--lms-bg)', color: 'var(--lms-text-main)', border: '1px solid var(--lms-border)', borderRadius: 8 }
const tenureColors = ['#1598e5', '#2dc467', '#ffd447', '#9b6cf3', '#94a3b8']
const pct = (value: number, total: number) => total ? (value / total * 100).toFixed(1) : '0.0'
const rateText = (value: number | null) => value === null ? '—' : value.toFixed(1) + '%'
const employeeColumns: Column[] = [
  { id: 'employeeCode', label: 'Employee Code' }, { id: 'employee', label: 'Employee Name' },
  { id: 'campusName', label: 'Campus' }, { id: 'departmentName', label: 'Department' },
  { id: 'staffType', label: 'Staff Type' }, { id: 'status', label: 'Status' },
]
const exitColumns: Column[] = [...employeeColumns, { id: 'exitDate', label: 'Exit Date' }, { id: 'tenureName', label: 'Tenure' }, { id: 'reasonName', label: 'Exit Reason' }]
type DetailView = { title: string; kind: HrDetailKind; columns: Column[] }

function Panel({ title, children, onViewAll, className = '' }: { title: string; children: ReactNode; onViewAll: () => void; className?: string }) {
  return <Paper component="section" elevation={0} className={`hr-panel ${className}`}><h2>{title}</h2><div className="hr-panel-content">{children}</div><div className="hr-panel-footer"><Button size="small" onClick={onViewAll} endIcon={<ChevronRightRounded />} aria-label={`View all ${title} details`}>View All</Button></div></Paper>
}
function Rate({ value }: { value: number | null }) {
  return <span className={`hr-rate ${value === null ? 'moderate' : value < 12 ? 'good' : value < 12.5 ? 'moderate' : 'high'}`}>{rateText(value)}</span>
}
function Stat({ title, value, icon, onClick, color = 'green', children }: { title: string; value: string | number; icon: ReactNode; onClick: () => void; color?: string; children?: ReactNode }) {
  return <ButtonBase component="button" type="button" className="hr-stat" onClick={onClick} aria-label={`View ${title} details`} aria-haspopup="dialog"><div className={`hr-stat-icon ${color}`}>{icon}</div><div><span className="hr-stat-label">{title}</span><strong>{value}</strong>{children}</div></ButtonBase>
}

type AttritionRow = HrMetricGroup & { isTotal?: boolean }

function AttritionTable({ rows, kind }: { rows: AttritionRow[]; kind: 'staff' | 'department' | 'campus' }) {
  const columns: Column[] = [
    ...(kind === 'campus' ? [{ id: 'index', label: '#', align: 'center' as const, format: (_value: unknown, _row: unknown, index?: number) => (index ?? 0) + 1 }] : []),
    { id: 'name', label: kind === 'staff' ? 'Staff Type' : kind === 'campus' ? 'Campus' : 'Department', format: (value, row) => {
      if (row.isTotal) return <span className="hr-total-label">{kind === 'staff' ? 'Overall' : 'Total'}</span>
      if (kind !== 'staff') return value
      const index = ['Teachers', 'Admin Staff', 'Support Staff'].indexOf(value)
      return <span className="hr-cell-label">{index === 0 ? <SchoolOutlined /> : index === 1 ? <BadgeOutlined /> : <SupportAgentRounded />}{value}</span>
    } },
    ...(kind !== 'staff' ? [{ id: 'total', label: 'Total Staff', align: 'center' as const }, { id: 'exits', label: 'Exits', align: 'center' as const, format: (value: number) => <span className="hr-exits">{value}</span> }] : []),
    { id: 'rate', label: 'Attrition Rate', align: 'center', format: (_value, row) => kind === 'staff'
      ? <div className="hr-inline-rate"><b>{rateText(row.rate)}</b><span className="hr-mini-track"><i style={{ width: `${(row.rate ?? 0) * 3}%` }} /></span></div>
      : <Rate value={row.rate} /> },
    ...(kind === 'staff' ? [{ id: 'exitsTotal', label: 'Exits / Avg Staff', align: 'center' as const, format: (_value: unknown, row: AttritionRow) => `${row.exits} / ${row.averageStaff}` }] : []),
  ]
  return <div className="hr-summary-table"><DashboardTable columns={columns} rows={rows} maxHeight={256} stickyHeader actions={false} pagination={false} filtersEnabled={false} downloadEnabled={false} checkboxSelection={false} hideToolbar readOnly useActionRights={false} disableRowSelectionOnClick rowHoverBg="rgba(var(--lms-primary-rgb), .08)" /></div>
}

export function HrAttritionDashboard() {
  const today = useMemo(() => dayjs(getPolicyDateString()), [])
  const defaultStart = today.subtract(11, 'month').startOf('month')
  const { selectedCampusId, campuses: assignedCampuses, user } = useAppSelector(state => state.auth)
  const { canAccessCampusDropdown, menuPermissionsLoaded, screenPermissionsLoaded } = useHiddenMenuPermissions()
  const currentCampus = Number(selectedCampusId || user?.entityId || 0)
  const [campus, setCampus] = useState(currentCampus ? String(currentCampus) : 'all')
  const [department, setDepartment] = useState('all')
  const [period, setPeriod] = useState('12')
  const [startDate, setStartDate] = useState<Dayjs | null>(defaultStart)
  const [endDate, setEndDate] = useState<Dayjs | null>(today)
  const [dashboard, setDashboard] = useState<HrDashboardResponse | null>(null)
  const [lookups, setLookups] = useState<{ entities: HrLookup[]; departments: HrLookup[] }>({ entities: [], departments: [] })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [refresh, setRefresh] = useState(0)
  const [detailView, setDetailView] = useState<DetailView | null>(null)
  const [details, setDetails] = useState<HrDetailResponse | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [detailError, setDetailError] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [columnFilters, setColumnFilters] = useState<Record<string, string>>({})
  const [detailRefresh, setDetailRefresh] = useState(0)
  const invalidRange = Boolean(startDate && endDate && (startDate.isAfter(endDate, 'day') || endDate.diff(startDate, 'day') > 1096))
  const entityId = canAccessCampusDropdown ? (campus === 'all' ? undefined : Number(campus)) : currentCampus || undefined
  const filters = useMemo<HrFilters | null>(() => startDate?.isValid() && endDate?.isValid() && !invalidRange
    ? { from: startDate.format('YYYY-MM-DD'), to: endDate.format('YYYY-MM-DD'), entityId, departmentId: department === 'all' ? undefined : Number(department) } : null,
  [startDate, endDate, invalidRange, entityId, department])
  useEffect(() => { setCampus(currentCampus ? String(currentCampus) : 'all'); setDepartment('all') }, [currentCampus])
  useEffect(() => {
    if (!menuPermissionsLoaded || !screenPermissionsLoaded) return
    setDetailView(null)
    setDashboard(null)
    setError('')
    if (!filters || (!canAccessCampusDropdown && !currentCampus)) { setLoading(false); return }
    const controller = new AbortController()
    setLoading(true)
    getHrDashboard(filters, controller.signal).then(result => {
      if (controller.signal.aborted) return
      setDashboard(result)
      setLookups({ entities: result.entities, departments: result.departments })
    }).catch((reason: unknown) => {
      if (!controller.signal.aborted) setError(reason instanceof Error ? reason.message : 'HR dashboard could not be loaded.')
    }).finally(() => { if (!controller.signal.aborted) setLoading(false) })
    return () => controller.abort()
  }, [filters, canAccessCampusDropdown, currentCampus, menuPermissionsLoaded, screenPermissionsLoaded, refresh])
  useEffect(() => {
    if (!detailView || !filters) return
    const controller = new AbortController()
    setDetailLoading(true)
    setDetailError('')
    const timeout = window.setTimeout(() => {
      getHrDetails(filters, detailView.kind, page, pageSize, columnFilters, controller.signal).then(result => {
        if (!controller.signal.aborted) setDetails(result)
      }).catch((reason: unknown) => {
        if (!controller.signal.aborted) { setDetails(null); setDetailError(reason instanceof Error ? reason.message : 'HR details could not be loaded.') }
      }).finally(() => { if (!controller.signal.aborted) setDetailLoading(false) })
    }, Object.keys(columnFilters).length ? 250 : 0)
    return () => { window.clearTimeout(timeout); controller.abort() }
  }, [detailView, filters, page, pageSize, columnFilters, detailRefresh])
  const onDetailFilters = useCallback((next: Record<string, string>) => {
    setColumnFilters(previous => JSON.stringify(previous) === JSON.stringify(next) ? previous : next)
    setPage(1)
  }, [])
  const changePeriod = (value: string) => {
    setPeriod(value)
    if (value !== 'custom') { setStartDate(today.subtract(Number(value) - 1, 'month').startOf('month')); setEndDate(today) }
  }
  const resetFilters = () => {
    setCampus(currentCampus ? String(currentCampus) : 'all'); setDepartment('all'); setPeriod('12'); setStartDate(defaultStart); setEndDate(today)
  }
  const data = dashboard?.summary ?? emptyHrSummary
  const overall = [{ name: 'Starting staff who exited', value: data.cohortExits }, { name: 'Starting staff retained', value: data.retained }]
  const isDefault = campus === (currentCampus ? String(currentCampus) : 'all') && department === 'all' && period === '12'
  const highestDepartment = [...data.departmentRows].filter(row => row.rate !== null).sort((a, b) => (b.rate ?? 0) - (a.rate ?? 0))[0]
  const comparison = data.comparison.map(row => ({ ...row, name: dayjs(row.from).format('YYYY') + '-' + dayjs(row.to).format('YY') }))
  const totalRow: AttritionRow = { id: 'total', name: 'Total', total: data.total, averageStaff: data.averageStaff, exits: data.exits, rate: data.rate, isTotal: true }
  const entities = lookups.entities.length ? lookups.entities : assignedCampuses.map(item => ({ id: Number(item.id), name: item.name }))
  const scopeLabel = [entityId ? entities.find(item => item.id === entityId)?.name ?? 'Current campus' : 'All permitted campuses', department === 'all' ? 'All Departments' : lookups.departments.find(item => item.id === Number(department))?.name ?? 'Department', (startDate?.format('DD MMM YYYY') ?? '') + ' - ' + (endDate?.format('DD MMM YYYY') ?? '')].join(' / ')
  const openDetail = (title: string, kind: HrDetailKind, columns: Column[]) => {
    setPage(1); setColumnFilters({}); setDetails(null); setDetailLoading(true); setDetailError(''); setDetailView({ title, kind, columns })
  }
  const showExits = (title: string) => openDetail(title, 'exits', exitColumns)
  const showStaff = (title: string, kind: 'staff' | 'retained' | 'cohort' = 'staff') => openDetail(title, kind, employeeColumns)
  const showJoiners = () => openDetail('New Joiners', 'joiners', [...employeeColumns, { id: 'joiningDate', label: 'Joining Date' }])
  const showPositions = () => openDetail('Open Positions', 'positions', [{ id: 'positionNumber', label: 'Position Number' }, { id: 'position', label: 'Position' }, { id: 'campusName', label: 'Campus' }, { id: 'departmentName', label: 'Department' }, { id: 'status', label: 'Status' }])
  const showTrend = () => openDetail('Joiners & Exits', 'trend', [...employeeColumns, { id: 'date', label: 'Event Date' }])
  const showComparison = () => openDetail('Year-on-Year Comparison', 'comparison', [{ id: 'name', label: 'Period' }, { id: 'joiners', label: 'Joiners', align: 'center' }, { id: 'exits', label: 'Exits', align: 'center' }, { id: 'rate', label: 'Attrition Rate', format: value => rateText(value) }])

  return <main className="hr-attrition">
    {loading && <LmsOverlayLoader message="Loading HR dashboard" />}
    <header className="hr-dashboard-header">
      <div><h1>HR Attrition Dashboard</h1><nav aria-label="Breadcrumb"><Link to="/">Home</Link><ChevronRightRounded /><Link to="/hr">HR &amp; Staff</Link><ChevronRightRounded /><span>Attrition</span></nav></div>
      <div className="hr-filters">
        <LmsSelect label="Campus" value={canAccessCampusDropdown ? campus : String(currentCampus || '')} disabled={!canAccessCampusDropdown || loading} clearable={canAccessCampusDropdown} onChange={value => setCampus(String(value))} onClear={() => setCampus('all')} options={[...(canAccessCampusDropdown ? [{ label: 'All Campuses', value: 'all' }] : []), ...entities.map(item => ({ label: item.name, value: String(item.id) }))]} />
        <LmsSelect label="Department" value={department} disabled={loading} onChange={value => setDepartment(String(value))} onClear={() => setDepartment('all')} options={[{ label: 'All Departments', value: 'all' }, ...lookups.departments.map(item => ({ label: item.name, value: String(item.id) }))]} />
        <LmsSelect label="Period" value={period} onChange={value => changePeriod(String(value))} clearable={false} options={[{ label: 'Last 12 Months', value: '12' }, { label: 'Last 6 Months', value: '6' }, { label: 'Last 3 Months', value: '3' }, { label: 'Custom Range', value: 'custom' }]} />
        <div className="hr-date-field"><FormDatePicker label="From Date" value={startDate} onChange={value => { if (!value || value.isValid()) { setStartDate(value); setPeriod('custom') } }} minDate={dayjs('1903-01-01')} maxDate={endDate ?? today} error={invalidRange} helperText={invalidRange ? 'Select a valid range of up to three years.' : undefined} slotProps={{ textField: { size: 'small', fullWidth: true } }} /></div>
        <div className="hr-date-field"><FormDatePicker label="To Date" value={endDate} onChange={value => { if (!value || value.isValid()) { setEndDate(value); setPeriod('custom') } }} minDate={startDate ?? dayjs('1903-01-01')} maxDate={today} error={invalidRange} slotProps={{ textField: { size: 'small', fullWidth: true } }} /></div>
        <IconButton aria-label="Refresh HR dashboard" disabled={loading || !filters} onClick={() => setRefresh(value => value + 1)}><RefreshRounded /></IconButton>
      </div>
    </header>
    <div className="hr-preview-note"><span className="hr-preview-dot" />Live HR data<Button size="small" variant="text" onClick={resetFilters} disabled={isDefault || loading}>Reset filters</Button></div>
    {!filters && <Alert severity="warning">Select both dates and a valid range of up to three years.</Alert>}
    {!canAccessCampusDropdown && !currentCampus && <Alert severity="warning">Select a campus to view the HR dashboard.</Alert>}
    {error && <Alert severity="error" action={<Button onClick={() => setRefresh(value => value + 1)}>Retry</Button>}>{error}</Alert>}
    {dashboard && !loading && <>
    <p className="hr-data-note">{dashboard.dataNote}</p>
    <section className="hr-stats" aria-label="HR summary">
      <Stat title="Total Staff" onClick={() => showStaff('Total Staff')} value={data.total} icon={<GroupsRounded />} />
      <Stat title="New Joiners" onClick={showJoiners} value={data.joiners} icon={<PersonAddAlt1Rounded />} color="blue" />
      <Stat title="Exits (Left)" onClick={() => showExits('Exits (Left)')} value={data.exits} icon={<LogoutRounded />} color="red" />
      <Stat title="Attrition Rate" onClick={() => showExits('Attrition Details')} value={rateText(data.rate)} icon={<AutorenewRounded />}><small>Exits / average staff: {data.averageStaff}</small>{data.previousRate !== null && <small>Previous period: {rateText(data.previousRate)}</small>}{data.changePp !== null && <span className={data.changePp <= 0 ? 'hr-improvement' : 'hr-exits'}>{data.changePp > 0 ? '+' : ''}{data.changePp.toFixed(1)} pp</span>}</Stat>
      <Stat title="Retention Rate" onClick={() => showStaff('Retained Starting Staff', 'retained')} value={rateText(data.retentionRate)} icon={<GroupsRounded />}><small>{data.retained} / {data.openingStaff} starting staff</small></Stat>
    </section>
    <div className="hr-grid hr-grid-top">
      <Panel onViewAll={showTrend} title={period === 'custom' ? 'Attrition Trend (Selected Period)' : `Attrition Trend (Last ${period} Months)`}>
        <div className="hr-chart hr-trend"><ResponsiveContainer width="100%" height="100%"><ComposedChart data={data.months} margin={{ top: 12, right: 0, left: -22, bottom: 0 }}><CartesianGrid stroke="var(--lms-border)" vertical={false} /><XAxis dataKey="name" tick={{ fontSize: 10, fill: 'var(--lms-text-secondary)' }} tickFormatter={name => name.split(' ')[0]} axisLine={false} tickLine={false} /><YAxis yAxisId="count" allowDecimals={false} tick={{ fontSize: 11, fill: 'var(--lms-text-secondary)' }} axisLine={false} tickLine={false} /><YAxis yAxisId="rate" orientation="right" tickFormatter={value => `${value}%`} tick={{ fontSize: 10, fill: 'var(--lms-text-secondary)' }} axisLine={false} tickLine={false} /><Tooltip contentStyle={chartTooltipStyle} /><Legend verticalAlign="top" height={32} iconSize={10} wrapperStyle={{ fontSize: 11 }} /><Bar yAxisId="count" dataKey="joiners" name="Joiners" fill={GREEN} radius={[3, 3, 0, 0]} maxBarSize={16} /><Bar yAxisId="count" dataKey="exits" name="Exits" fill={RED} radius={[3, 3, 0, 0]} maxBarSize={16} /><Line yAxisId="rate" dataKey="rate" name="Attrition Rate" stroke={BLUE} strokeWidth={2.5} dot={{ r: 3 }} unit="%" /></ComposedChart></ResponsiveContainer></div>
      </Panel>
      <Panel title="Starting Staff Retention" onViewAll={() => showStaff('Starting Staff Retention', 'cohort')}>
        <div className="hr-overall"><div className="hr-donut"><ResponsiveContainer width="100%" height="100%"><PieChart><Pie data={overall} dataKey="value" innerRadius="66%" outerRadius="92%" startAngle={90} endAngle={-270} stroke="#fff" paddingAngle={1}>{overall.map((item, index) => <Cell key={item.name} fill={index === 0 ? RED : GREEN} />)}</Pie><Tooltip contentStyle={chartTooltipStyle} /></PieChart></ResponsiveContainer><div className="hr-donut-center"><strong>{rateText(data.retentionRate)}</strong><span>Retention Rate</span></div></div>
          <div className="hr-overall-details"><div><i style={{ background: RED }} />Exited ({data.cohortExits})<b>{pct(data.cohortExits, data.openingStaff)}%</b></div><div><i style={{ background: GREEN }} />Retained ({data.retained})<b>{rateText(data.retentionRate)}</b></div><aside><div>Starting headcount<b>{data.openingStaff}</b></div><div>Closing headcount<b>{data.total}</b></div></aside></div>
        </div>
      </Panel>
      <Panel title="Attrition by Staff Type" onViewAll={() => showExits('Attrition by Staff Type')}><AttritionTable kind="staff" rows={[...data.staffTypeRows, totalRow]} /></Panel>
    </div>
    <div className="hr-grid hr-grid-middle">
      <Panel title="Attrition by Department" onViewAll={() => showExits('Attrition by Department')}><AttritionTable kind="department" rows={[...data.departmentRows, totalRow]} /></Panel>
      <Panel title="Attrition by Tenure (Length of Service)" onViewAll={() => showExits('Attrition by Tenure')}><div className="hr-tenure"><div className="hr-donut"><ResponsiveContainer width="100%" height="100%"><PieChart><Pie data={data.tenures} dataKey="value" innerRadius="53%" outerRadius="94%" startAngle={90} endAngle={-270} paddingAngle={1}>{data.tenures.map((row, index) => <Cell key={row.name} fill={tenureColors[index]} />)}</Pie><Tooltip contentStyle={chartTooltipStyle} /></PieChart></ResponsiveContainer><div className="hr-donut-center"><CalendarMonthRounded /><strong>{data.exits}</strong><span>Exits</span></div></div><div className="hr-tenure-legend">{data.tenures.map((row, index) => <div key={row.name}><i style={{ background: tenureColors[index] }} /><span>{row.name}</span><b>{row.value} <small>({pct(row.value, data.exits)}%)</small></b></div>)}</div></div>{data.exits === 0 && <p className="hr-empty">No exits for the selected filters.</p>}</Panel>
      <Panel title="Attrition by Reason" onViewAll={() => showExits('Attrition by Reason')}><div className="hr-reasons">{data.reasons.map(row => <div key={row.name}><span>{row.name}</span><div className="hr-reason-track"><i style={{ width: `${pct(row.value, data.exits)}%` }} /></div><b>{pct(row.value, data.exits)}%</b></div>)}</div></Panel>
    </div>
    <div className="hr-grid hr-grid-bottom">
      <Panel title="Attrition by Campus" onViewAll={() => showExits('Attrition by Campus')}><AttritionTable kind="campus" rows={data.campusRows} /></Panel>
      <Panel title="Exits vs. Joiners (YoY Comparison)" onViewAll={showComparison}><p className="hr-panel-caption">Same selected dates in each year / selected campus and department</p><div className="hr-chart hr-comparison"><ResponsiveContainer width="100%" height="100%"><BarChart data={comparison} margin={{ top: 15, left: -24, right: 2, bottom: 0 }}><CartesianGrid vertical={false} stroke="var(--lms-border)" /><XAxis dataKey="name" tick={{ fontSize: 11, fill: 'var(--lms-text-secondary)' }} axisLine={false} tickLine={false} /><YAxis tick={{ fontSize: 11, fill: 'var(--lms-text-secondary)' }} axisLine={false} tickLine={false} /><Tooltip contentStyle={chartTooltipStyle} /><Legend verticalAlign="top" height={30} iconSize={10} wrapperStyle={{ fontSize: 11 }} /><Bar dataKey="joiners" name="Joiners" fill={GREEN} radius={[4, 4, 0, 0]} maxBarSize={40} label={{ position: 'top', fill: 'var(--lms-text-main)', fontSize: 12, fontWeight: 700 }} /><Bar dataKey="exits" name="Exits" fill={RED} radius={[4, 4, 0, 0]} maxBarSize={40} label={{ position: 'top', fill: 'var(--lms-text-main)', fontSize: 12, fontWeight: 700 }} /></BarChart></ResponsiveContainer></div></Panel>
      <Panel title="Key Insights & Actions" onViewAll={() => showExits('Supporting Exit Records')}><ul className="hr-insights">
        <li><TrendingDownRounded /><span>Attrition: <b>{rateText(data.rate)}</b>{data.changePp !== null && <> / {data.changePp > 0 ? '+' : ''}{data.changePp.toFixed(1)} pp compared with the same dates last year.</>}</span></li>
        <li><GroupsRounded /><span>{highestDepartment ? <>Highest department attrition: <b>{highestDepartment.name}</b> ({rateText(highestDepartment.rate)}).</> : 'No department comparison is available.'}</span></li>
        <li><CalendarMonthRounded /><span><b>{pct(data.tenures[0]?.value ?? 0, data.exits)}%</b> of exits occurred within the first year.</span></li>
        <li><TrackChangesRounded /><span>Structured exit reasons are not recorded in the current HR data.</span></li>
        <li><LightbulbOutlined /><span><b>Suggested Action:</b> {data.exits ? 'Review the exit records and onboarding process, and start recording exit reasons.' : 'Continue monitoring staff retention for the selected period.'}</span></li>
      </ul></Panel>
    </div>
    </>}
    <Modal open={Boolean(detailView)} onClose={() => setDetailView(null)} title={detailView ? detailView.title + (details ? ' (' + details.totalCount + ')' : '') : 'HR Details'} maxWidth="xl" showActions={false} contentPadding={2}>
      {detailView && <>
        <p className="hr-detail-context">{scopeLabel}{detailView.kind === 'positions' ? ' / Current vacancies (dates do not apply)' : ''}</p>
        <div className="hr-detail-body">
          {detailLoading && <LmsOverlayLoader variant="container" message="Loading HR details" />}
          {detailError ? <Alert severity="error" action={<Button onClick={() => setDetailRefresh(value => value + 1)}>Retry</Button>}>{detailError}</Alert> :
            <DashboardTable key={detailView.kind} title={detailView.title} columns={detailView.columns} rows={details?.rows ?? []} actions={false} readOnly useActionRights={false} checkboxSelection={false} downloadEnabled={false} maxHeight="55vh" rowHoverBg="rgba(var(--lms-primary-rgb), .08)" currentPage={page} rowsPerPage={pageSize} rowsPerPageOptions={[10, 25, 50, 100]} totalCount={details?.totalCount ?? 0} onPageChange={setPage} onRowsPerPageChange={setPageSize} onFiltersChange={onDetailFilters} />}
        </div>
        {details && <p className="hr-data-note">{details.dataNote}</p>}
      </>}
    </Modal>
  </main>
}
