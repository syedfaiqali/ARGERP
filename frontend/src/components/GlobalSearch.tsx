import { useEffect, useMemo, useState } from 'react'
import { Alert, Box, Button, CircularProgress, Dialog, IconButton, InputAdornment, List, ListItemButton, ListItemIcon, ListItemText, Stack, Tab, Tabs, TextField, Typography } from '@mui/material'
import { ArrowOutward, Close, KeyboardReturnOutlined, Monitor, PersonOutline, Search, TravelExploreOutlined } from '@mui/icons-material'
import { PortalEmptyState, PortalPanelHeader, portalColors, portalInput, portalRow, portalScroll, portalSurface } from './portalUi'
import { useNavigate } from 'react-router-dom'
import { screens } from '@/screens'
import { modules } from '@/modules'
import { useAppSelector } from '@/hooks'
import api from '@/services/api'

export const searchDestinations = [
  ...screens.map(item => ({ name: item.label, group: item.group, path: item.path })),
  ...modules.map(item => ({ name: item.name, group: 'Modules', path: '/modules/' + item.key })),
  { name: 'Home', group: 'Portal', path: '/' },
  { name: 'HR Dashboard', group: 'Human Resources', path: '/hr/attrition' },
]
export const findDestinations = (query: string) => {
  const terms = query.toLowerCase().trim().split(/\s+/).filter(Boolean)
  return terms.length ? searchDestinations.filter(item => terms.every(term => `${item.name} ${item.group}`.toLowerCase().includes(term)))
    .sort((a, b) => Number(b.name.toLowerCase().startsWith(query.toLowerCase())) - Number(a.name.toLowerCase().startsWith(query.toLowerCase())) || a.name.localeCompare(b.name)) : []
}
type Employee = { id: number; name: string; code: string }
export function GlobalSearch({ open, onClose }: { open: boolean; onClose: () => void }) {
  const navigate = useNavigate(), campus = useAppSelector(state => state.auth.selectedCampusId)
  const [mode, setMode] = useState(0), [query, setQuery] = useState('')
  const [rows, setRows] = useState<Employee[]>([]), [total, setTotal] = useState(0), [page, setPage] = useState(1)
  const [loading, setLoading] = useState(false), [error, setError] = useState(''), [retry, setRetry] = useState(0)
  const destinations = useMemo(() => findDestinations(query), [query])
  useEffect(() => { if (open) { setQuery(''); setPage(1); setRows([]); setError('') } }, [open, campus])
  useEffect(() => {
    const abort = new AbortController()
    setLoading(false); setError('')
    if (!open || mode !== 1 || !query.trim()) { setRows([]); setTotal(0); return }
    if (page === 1) { setRows([]); setTotal(0) }
    setLoading(true)
    const timer = window.setTimeout(async () => {
      try {
        const { data } = await api.get('/api/HumanResource/Portal/v3/employees/search', { signal: abort.signal, params: { searchTerm: query.trim(), pageNumber: page } })
        if (!abort.signal.aborted) { setRows(previous => page === 1 ? data.items : [...previous, ...data.items]); setTotal(data.totalCount) }
      } catch { if (!abort.signal.aborted) setError('Unable to search employees. Please try again.') }
      finally { if (!abort.signal.aborted) setLoading(false) }
    }, 300)
    return () => { window.clearTimeout(timer); abort.abort() }
  }, [open, mode, query, page, campus, retry])
  const go = (path: string) => { onClose(); navigate(path) }
  const quickLinks = ['Employee Profile', 'Leave Request', 'Employee Daily Attendance', 'Change Password'].map(name => searchDestinations.find(item => item.name === name)).filter((item): item is typeof searchDestinations[number] => !!item)
  const resultRow = (name: string, group: string, path: string, employee = false) => <ListItemButton key={path} onClick={() => go(path)} sx={portalRow}><ListItemIcon sx={{ minWidth: 36 }}><Box sx={{ width: 36, height: 36, borderRadius: '11px', bgcolor: '#f1f5eb', color: '#91a076', display: 'grid', placeItems: 'center' }}>{employee ? <PersonOutline sx={{ fontSize: 19 }} /> : <Monitor sx={{ fontSize: 18 }} />}</Box></ListItemIcon><ListItemText primary={name} secondary={group} primaryTypographyProps={{ sx: { fontSize: 12.5, fontWeight: 650, color: portalColors.ink } }} secondaryTypographyProps={{ sx: { fontSize: 10, color: '#97a08d', mt: .25 } }} /><ArrowOutward sx={{ fontSize: 16, color: '#b7c0ac' }} /></ListItemButton>
  return <Dialog open={open} onClose={onClose} aria-label="Search workspace" fullWidth maxWidth="sm" BackdropProps={{ sx: { bgcolor: 'rgba(29,43,22,.28)', backdropFilter: 'blur(5px)' } }} PaperProps={{ sx: { ...portalSurface, m: 2, width: 'calc(100% - 32px)', maxWidth: 600, maxHeight: 'calc(100dvh - 40px)' } }}>
    <PortalPanelHeader eyebrow="YOUR WORKSPACE, AT A GLANCE" title="Find what you need" icon={<TravelExploreOutlined />} onClose={onClose} subtitle="A shortcut to your screens, people and everyday work." />
    <Box sx={{ px: 2.5, pt: 2, pb: 1.5 }}>
      <Tabs value={mode} onChange={(_, value) => { setMode(value); setPage(1) }} variant="fullWidth" aria-label="Search category" sx={{ p: .5, mb: 2, minHeight: 40, bgcolor: '#f5f7f1', border: '1px solid #ecf0e5', borderRadius: '12px', '& .MuiTabs-indicator': { display: 'none' }, '& .MuiTab-root': { minHeight: 34, fontSize: 11, fontWeight: 700, textTransform: 'none', color: '#929d87', borderRadius: '9px', '&.Mui-selected': { bgcolor: '#fff', color: portalColors.ink, boxShadow: '0 2px 6px #30432b0c' } } }}><Tab label="Menus & Screens" /><Tab label="Employees" /></Tabs>
      <TextField autoFocus fullWidth placeholder={mode ? 'Search employee name or code?' : 'Search any menu or screen?'} inputProps={{ maxLength: 100, 'aria-label': mode ? 'Search employee name or code' : 'Search any menu or screen' }} value={query} onChange={event => { setQuery(event.target.value); setPage(1) }} onKeyDown={event => { if (event.key === 'Enter' && !loading) { const first = mode ? rows[0] && '/hr/employee-profile?employeeId=' + rows[0].id : destinations[0]?.path; if (first) go(first) } }} InputProps={{ startAdornment: <InputAdornment position="start"><Search sx={{ fontSize: 19, color: '#91a07f' }} /></InputAdornment>, endAdornment: query ? <InputAdornment position="end"><IconButton size="small" aria-label="Clear search" onClick={() => { setQuery(''); setPage(1) }}><Close sx={{ fontSize: 16 }} /></IconButton></InputAdornment> : undefined }} sx={portalInput} />
    </Box>
    <Box sx={{ px: 1.25, pb: 1.25, minHeight: 240, maxHeight: '45dvh', overflow: 'auto', ...portalScroll }}>
      {!query.trim() ? <Box sx={{ px: 1.25, pt: 1, pb: 2 }}><Typography sx={{ fontSize: 9, letterSpacing: '1.3px', fontWeight: 800, color: '#9aa38f', mb: 1.5 }}>{mode ? 'EMPLOYEE DIRECTORY' : 'QUICK LINKS'}</Typography>{mode ? <PortalEmptyState icon={<PersonOutline />} title="Find someone on your team" description="Search by name or employee code in the selected campus or branch." /> : <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: .75 }}>{quickLinks.map(item => <Button key={item.path} onClick={() => go(item.path)} endIcon={<ArrowOutward sx={{ fontSize: '14px !important', color: '#a7b397' }} />} sx={{ justifyContent: 'space-between', px: 1.75, py: 1.6, borderRadius: '12px', color: '#66765a', fontSize: 11, textAlign: 'left', bgcolor: '#fafbf8', border: '1px solid #ecf0e5', '&:hover': { bgcolor: '#f0f5e8', borderColor: '#dce7ce' } }}>{item.name}</Button>)}</Box>}</Box> : <>
        {error && <Alert sx={{ m: 1.25, borderRadius: '12px', fontSize: 12 }} severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert>}
        {mode === 0 ? <List disablePadding>{destinations.map(item => resultRow(item.name, item.group, item.path))}{!destinations.length && <PortalEmptyState icon={<Search />} title="No matching screens" description="Try a shorter name or search for a module, such as HR or Organization." />}</List> : <>
          <List disablePadding>{rows.map(item => resultRow(item.name || item.code, item.code, '/hr/employee-profile?employeeId=' + item.id, true))}</List>
          {loading && <Stack direction="row" alignItems="center" justifyContent="center" role="status" spacing={1.25} sx={{ p: 3 }}><CircularProgress size={17} sx={{ color: '#8ba55d' }} /><Typography sx={{ fontSize: 12, color: '#95a086' }}>Finding employees?</Typography></Stack>}
          {!loading && !error && !rows.length && <PortalEmptyState icon={<PersonOutline />} title="No matching employees" description="Try a different name or employee code for this campus or branch." />}
          {rows.length < total && <Button fullWidth size="small" sx={{ fontSize: 11, my: 1 }} disabled={loading} onClick={() => setPage(value => value + 1)}>Show more employees</Button>}
        </>}
      </>}
    </Box>
    <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ px: 2.5, py: 1.5, bgcolor: '#fafbf8', borderTop: '1px solid ' + portalColors.line }}><Typography sx={{ fontSize: 10, color: '#96a08b' }}>{query.trim() ? (mode ? rows.length + ' of ' + total + ' employees' : destinations.length + ' screens found') : 'HR ? Organization ? Auth'}</Typography><Stack direction="row" alignItems="center" spacing={.65} sx={{ display: { xs: 'none', sm: 'flex' } }}><KeyboardReturnOutlined sx={{ fontSize: 14, color: '#a6af9c' }} /><Typography sx={{ fontSize: 9, color: '#a6af9c' }}>to open first result</Typography></Stack></Stack>
  </Dialog>
}
