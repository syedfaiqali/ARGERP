import { useEffect, useState } from 'react'
import { Alert, Avatar, Badge, Box, Button, IconButton, List, ListItemAvatar, ListItemButton, ListItemText, Popover, Stack, Tooltip, Typography } from '@mui/material'
import { CelebrationOutlined, SmartToyOutlined } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '@/hooks'
import { setBirthdays, type BirthdayPerson } from '@/store/authSlice'
import { fetchBirthdayNotifications } from '@/services/birthdayService'
import searchIcon from '@/assets/search-flaticon-8112604.gif'
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
  const closeBirthdays = () => setBirthdayAnchor(null)
  return <>
    <Stack direction="row" alignItems="center" spacing={{ xs: 0, sm: .5 }} sx={{ color: 'var(--lms-primary)' }}>
      <Tooltip title="Search menus and employees"><IconButton aria-label="Global search" color="inherit" onClick={() => setSearchOpen(true)}><Box component="img" src={searchIcon} alt="" sx={{ width: 24, height: 24 }} /></IconButton></Tooltip>
      <Tooltip title="AI Assistant"><IconButton aria-label="Open AI Assistant" color="inherit" onClick={() => setAssistantOpen(true)}><SmartToyOutlined /></IconButton></Tooltip>
      <Tooltip title="Employee birthdays"><IconButton aria-label="Employee birthdays" aria-haspopup="true" color="inherit" onClick={event => setBirthdayAnchor(event.currentTarget)}><Badge badgeContent={employees.length} color="primary"><CelebrationOutlined /></Badge></IconButton></Tooltip>
    </Stack>
    <Popover open={!!birthdayAnchor} anchorEl={birthdayAnchor} onClose={closeBirthdays} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }} transformOrigin={{ vertical: 'top', horizontal: 'right' }} PaperProps={{ sx: { mt: 1.5, width: 360, maxWidth: 'calc(100vw - 24px)', maxHeight: '65vh', borderRadius: 4 } }}>
      <Box sx={{ p: 2.5, background: 'linear-gradient(120deg, #f5f9ed, #fff6eb)', borderBottom: '1px solid var(--lms-border)' }}><Typography fontWeight={800}>Today's Birthdays 🎉</Typography><Typography variant="body2" color="text.secondary">Employees · {new Date().toLocaleDateString('en-GB', { day: 'numeric', month: 'long' })}</Typography></Box>
      {loading ? <Typography role="status" sx={{ p: 3 }}>Loading birthdays…</Typography> : error ? <Alert severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert> : employees.length ? <List>{employees.map(item => <ListItemButton key={item.id} onClick={() => { closeBirthdays(); navigate(`/hr/employee-profile?employeeId=${item.id}`) }}><ListItemAvatar><Avatar sx={{ bgcolor: 'rgba(var(--lms-primary-rgb),.1)', color: 'var(--lms-primary)' }}>{item.name.slice(0, 1)}</Avatar></ListItemAvatar><ListItemText primary={item.name} secondary={item.subtitle} /></ListItemButton>)}</List> : <Typography sx={{ p: 3 }} color="text.secondary">No employee birthdays today in this campus/branch.</Typography>}
      <Box sx={{ px: 2, pb: 1 }}><Button size="small" disabled={loading} onClick={() => setRetry(value => value + 1)}>Refresh birthdays</Button></Box>
    </Popover>
    <GlobalSearch open={searchOpen} onClose={() => setSearchOpen(false)} />
    <AiAssistant open={assistantOpen} onClose={() => setAssistantOpen(false)} />
  </>
}
