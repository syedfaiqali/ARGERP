import { useEffect, useState } from 'react'
import { Alert, Avatar, Badge, Box, Button, Chip, IconButton, List, ListItemButton, ListItemText, Popover, Skeleton, Stack, Tooltip, Typography } from '@mui/material'
import { CakeOutlined, CelebrationOutlined, Refresh, SmartToyOutlined, TravelExploreOutlined, ArrowOutward } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '@/hooks'
import { setBirthdays, type BirthdayPerson } from '@/store/authSlice'
import { fetchBirthdayNotifications } from '@/services/birthdayService'
import { PortalEmptyState, PortalPanelHeader, portalColors, portalIconButton, portalRow, portalScroll, portalSurface } from './portalUi'
import { GlobalSearch } from './GlobalSearch'
import { AiAssistant } from './AiAssistant'

export function HeaderActions() {
  const dispatch = useAppDispatch(), navigate = useNavigate()
  const campus = useAppSelector(state => state.auth.selectedCampusId)
  const [birthdayAnchor, setBirthdayAnchor] = useState<HTMLElement | null>(null)
  const [searchOpen, setSearchOpen] = useState(false), [assistantOpen, setAssistantOpen] = useState(false)
  const [employees, setEmployees] = useState<BirthdayPerson[]>([]), [loading, setLoading] = useState(true), [error, setError] = useState(''), [retry, setRetry] = useState(0)
  const [day, setDay] = useState(new Date().toDateString())
  useEffect(() => { const timer = window.setInterval(() => setDay(new Date().toDateString()), 60000); return () => clearInterval(timer) }, [])
  useEffect(() => {
    let active = true
    setEmployees([]); setError(''); setLoading(true)
    fetchBirthdayNotifications(campus || 0, { useCache: retry === 0 }).then(result => { if (active) { setEmployees(result.employees); dispatch(setBirthdays(result)) } })
      .catch(() => { if (active) setError('Unable to load birthdays. Please try again.') })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [campus, dispatch, retry, day])
  const campusName = useAppSelector(state => state.auth.campuses.find(item => item.id === campus)?.name)
  const closeBirthdays = () => setBirthdayAnchor(null)
  return <>
    <Stack direction="row" alignItems="center" spacing={{ xs: .25, sm: .5 }}>
      <Tooltip title="Search your workspace"><IconButton aria-label="Global search" sx={portalIconButton} onClick={() => setSearchOpen(true)}><TravelExploreOutlined /></IconButton></Tooltip>
      <Tooltip title="AI Assistant"><IconButton aria-label="Open AI Assistant" sx={{ ...portalIconButton, bgcolor: '#f0f5e9', borderColor: '#e4ebda', '&:hover': { bgcolor: '#e5eed9', borderColor: '#cedebc' } }} onClick={() => setAssistantOpen(true)}><SmartToyOutlined /></IconButton></Tooltip>
      <Tooltip title="Employee birthdays"><IconButton aria-label="Employee birthdays" aria-haspopup="dialog" aria-expanded={!!birthdayAnchor} sx={{ ...portalIconButton, ...(birthdayAnchor ? { bgcolor: '#f7f3e8', color: '#a58a4d' } : {}) }} onClick={event => setBirthdayAnchor(event.currentTarget)}><Badge badgeContent={employees.length} sx={{ '& .MuiBadge-badge': { bgcolor: '#d7b771', color: '#fff', border: '2px solid #fff', fontSize: 9, fontWeight: 800, height: 18, minWidth: 18, right: -2 } }}><CelebrationOutlined /></Badge></IconButton></Tooltip>
    </Stack>
    <Popover open={!!birthdayAnchor} anchorEl={birthdayAnchor} onClose={closeBirthdays} marginThreshold={16} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }} transformOrigin={{ vertical: 'top', horizontal: 'right' }} PaperProps={{ role: 'dialog', 'aria-label': 'Employee birthdays', sx: { ...portalSurface, mt: 1.5, width: 360, maxWidth: 'calc(100vw - 32px)', maxHeight: 'calc(100dvh - 100px)', display: 'flex', flexDirection: 'column' } }}>
      <PortalPanelHeader eyebrow="A MOMENT TO CELEBRATE" title="Birthdays" icon={<CakeOutlined />} onClose={closeBirthdays} extra={<Chip label="Today" size="small" sx={{ fontSize: 9, fontWeight: 750, height: 22, bgcolor: '#faf5e9', color: '#b19658', border: '1px solid #f0e5ca' }} />} subtitle={new Date().toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long' })} />
      <Box sx={{ overflowY: 'auto', minHeight: 0, ...portalScroll }}>
        {loading ? <Box role="status" aria-label="Loading birthdays" sx={{ p: 2.5 }}>{[0, 1, 2].map(item => <Stack direction="row" spacing={1.5} sx={{ mb: 1.5 }} key={item}><Skeleton variant="rounded" width={40} height={40} sx={{ borderRadius: '12px' }} /><Box sx={{ flex: 1 }}><Skeleton width="65%" /><Skeleton width="40%" /></Box></Stack>)}</Box> : error ? <Alert sx={{ m: 2, fontSize: 12, borderRadius: '12px' }} severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert> : employees.length ? <List disablePadding sx={{ p: 1.5 }}>{employees.map(item => <ListItemButton key={item.id} sx={portalRow} onClick={() => { closeBirthdays(); navigate('/hr/employee-profile?employeeId=' + item.id) }}><Avatar sx={{ width: 40, height: 40, borderRadius: '13px', bgcolor: '#f7f1e4', color: '#ad9256', fontWeight: 750, fontSize: 15 }}>{item.name.slice(0, 1)}</Avatar><ListItemText primary={item.name} secondary={item.subtitle || 'Happy birthday!'} primaryTypographyProps={{ sx: { fontSize: 13, fontWeight: 700, color: portalColors.ink } }} secondaryTypographyProps={{ sx: { fontSize: 11, color: '#929b88', mt: .3 } }} /><ArrowOutward sx={{ fontSize: 15, color: '#b5beaa' }} /></ListItemButton>)}</List> : <PortalEmptyState icon={<CakeOutlined />} title="No birthdays today" description="Employee birthdays for this campus will appear here on their special day." />}
      </Box>
      <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={1} sx={{ px: 2.5, py: 1.4, bgcolor: '#fafbf8', borderTop: '1px solid ' + portalColors.line }}><Typography noWrap title={campusName} sx={{ fontSize: 10, color: '#939e89', minWidth: 0 }}>{campusName || 'Selected campus'}</Typography><Button size="small" startIcon={<Refresh sx={{ fontSize: '15px !important' }} />} disabled={loading} onClick={() => setRetry(value => value + 1)} sx={{ flexShrink: 0, fontSize: 10, color: '#779052', borderRadius: '8px', px: 1 }}>Refresh</Button></Stack>
    </Popover>
    <GlobalSearch open={searchOpen} onClose={() => setSearchOpen(false)} />
    <AiAssistant open={assistantOpen} onClose={() => setAssistantOpen(false)} />
  </>
}
