import React, { useState } from 'react'
import { Alert, Box, Button, CircularProgress, Stack, TextField, Typography } from '@mui/material'
import dayjs from 'dayjs'
import api from '@/services/api'
import { SchemaTable } from '@/components/SchemaTable'
export interface ReportItemData { name: string; menuId?: number | null; reportId?: number | null; reportCode?: string }
export interface ReportSectionData { title: string; id: string; items: Array<string | ReportItemData> }
type Props = { category: string; sections: ReportSectionData[]; initialSelectedReport?: ReportItemData | null; embedded?: boolean; onBack?: () => void; reportConfigs?: Record<string, unknown> }
export const ReportsPage: React.FC<Props> = ({ initialSelectedReport, sections, onBack }) => {
  const [selected, setSelected] = useState(initialSelectedReport?.reportCode || 'employees')
  const [from, setFrom] = useState(dayjs().startOf('month').format('YYYY-MM-DD')), [to, setTo] = useState(dayjs().endOf('month').format('YYYY-MM-DD'))
  const [rows, setRows] = useState<Record<string, unknown>[]>([]), [error, setError] = useState(''), [busy, setBusy] = useState(false), [generated, setGenerated] = useState(false)
  const title = initialSelectedReport?.name || 'HR Reports'
  const columns = Object.keys(rows[0] || {}).map(key => ({ id: key, label: key.replace(/([a-z])([A-Z])/g, '$1 $2').replace(/^./, c => c.toUpperCase()) }))
  return <Box><Stack direction="row" alignItems="center" gap={2} sx={{ mb: 3 }}><Button onClick={onBack}>Back</Button><Typography variant="h6">{title}</Typography></Stack>
    {!initialSelectedReport && <Stack direction="row" gap={1} sx={{ mb: 2 }}>{sections.flatMap(s => s.items).map(item => { const report = typeof item === 'string' ? { name: item, reportCode: 'employees' } : item; return <Button key={report.name} onClick={() => setSelected(report.reportCode || 'employees')}>{report.name}</Button> })}</Stack>}
    <Stack component="form" direction={{ xs: 'column', sm: 'row' }} gap={2} sx={{ mb: 3 }} onSubmit={async event => { event.preventDefault(); setBusy(true); setError(''); try { const response = await api.get('/api/HumanResource/Reports/v3/' + selected, { params: { from, to } }); setRows(response.data); setGenerated(true) } catch { setError('The report could not be generated. Please retry.') } finally { setBusy(false) } }}>
      {selected !== 'employees' && <><TextField required type="date" label="From" InputLabelProps={{ shrink: true }} value={from} onChange={e => setFrom(e.target.value)}/><TextField required type="date" label="To" InputLabelProps={{ shrink: true }} value={to} onChange={e => setTo(e.target.value)}/></>}
      <Button variant="contained" type="submit" disabled={busy || from > to}>Generate report</Button>
    </Stack>
    {busy && <CircularProgress size={24}/>}
    {error && <Alert severity="error">{error}</Alert>}
    {generated && <SchemaTable title={title} columns={columns} rows={rows.map((row, id) => ({ ...row, id }))} actions={false} showAddButton={false} hideBreadcrumbs useActionRights={false} downloadEnabled pagination filtersEnabled/>}
  </Box>
}
