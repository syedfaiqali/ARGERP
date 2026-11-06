import { useEffect, useMemo, useState } from 'react'
import { Alert, Box, Button, CircularProgress, Dialog, DialogContent, DialogTitle, IconButton, InputAdornment, List, ListItemButton, ListItemIcon, ListItemText, Tab, Tabs, TextField, Typography } from '@mui/material'
import { Close, Monitor, PersonOutline, Search } from '@mui/icons-material'
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
  return <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm" PaperProps={{ sx: { borderRadius: 4 } }}>
    <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>Search<IconButton aria-label="Close search" onClick={onClose}><Close /></IconButton></DialogTitle>
    <DialogContent>
      <Tabs value={mode} onChange={(_, value) => { setMode(value); setPage(1) }} aria-label="Search category" sx={{ mb: 2 }}><Tab label="Menus & Screens" /><Tab label="Employees" /></Tabs>
      <TextField autoFocus fullWidth label={mode ? 'Search employee name or code' : 'Search any menu or screen'} inputProps={{ maxLength: 100 }} value={query} onChange={event => { setQuery(event.target.value); setPage(1) }} InputProps={{ startAdornment: <InputAdornment position="start"><Search /></InputAdornment> }} />
      <Box sx={{ mt: 1, minHeight: 150, maxHeight: '55vh', overflow: 'auto' }}>
        {!query.trim() && <Typography color="text.secondary" sx={{ p: 3, textAlign: 'center' }}>Type to find {mode ? 'employees in the selected campus/branch' : 'HR, Organization and Auth screens'}.</Typography>}
        {error && <Alert severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert>}
        {mode === 0 ? <List>{destinations.map(item => <ListItemButton key={item.path} onClick={() => go(item.path)}><ListItemIcon><Monitor color="primary" /></ListItemIcon><ListItemText primary={item.name} secondary={item.group} /></ListItemButton>)}{query.trim() && !destinations.length && <Typography sx={{ p: 3 }}>No matching screens.</Typography>}</List> : <>
          <List>{rows.map(item => <ListItemButton key={item.id} onClick={() => go(`/hr/employee-profile?employeeId=${item.id}`)}><ListItemIcon><PersonOutline color="primary" /></ListItemIcon><ListItemText primary={item.name || item.code} secondary={item.code} /></ListItemButton>)}</List>
          {loading && <Box role="status" sx={{ p: 2, textAlign: 'center' }}><CircularProgress size={24} /><Typography>Searching employees…</Typography></Box>}
          {!loading && !error && !!query.trim() && !rows.length && <Typography sx={{ p: 3 }}>No matching employees in this campus/branch.</Typography>}
          {!!rows.length && <Typography variant="caption">{rows.length} of {total} employees</Typography>}
          {rows.length < total && <Button disabled={loading} onClick={() => setPage(value => value + 1)}>Load more</Button>}
        </>}
      </Box>
    </DialogContent>
  </Dialog>
}
