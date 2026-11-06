import { useEffect, useState } from 'react'
import { Alert, Avatar, Box, Button, CircularProgress, InputAdornment, List, ListItemAvatar, ListItemButton, ListItemText, Paper, TextField, Typography } from '@mui/material'
import { Apartment, CheckCircle, Search } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '@/hooks'
import { logout, setCampus, setCampusesRedux, type CampusOption } from '@/store/authSlice'
import api from '@/services/api'
import { invalidateGetCache } from '@/constants/globalItems'
import argLogo from '@/assets/ARG.png'

export default function CampusSelection() {
  const dispatch = useAppDispatch(), navigate = useNavigate()
  const user = useAppSelector(state => state.auth.user)
  const [campuses, setCampuses] = useState<CampusOption[]>([])
  const [selected, setSelected] = useState<number | null>(null)
  const [query, setQuery] = useState(''), [loading, setLoading] = useState(true), [error, setError] = useState(''), [retry, setRetry] = useState(0)
  useEffect(() => {
    const abort = new AbortController()
    setLoading(true); setError('')
    api.get<CampusOption[]>('/api/auth/Legalentity/v3/Legalentity', { signal: abort.signal }).then(({ data }) => {
      if (abort.signal.aborted) return
      setCampuses(data); dispatch(setCampusesRedux(data))
      setSelected(data.some(item => item.id === user?.entityId) ? user!.entityId! : data[0]?.id ?? null)
    }).catch(() => { if (!abort.signal.aborted) setError('Unable to load campuses. Please try again.') })
      .finally(() => { if (!abort.signal.aborted) setLoading(false) })
    return () => abort.abort()
  }, [dispatch, user?.entityId, retry])
  return <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center', p: 2, background: 'linear-gradient(140deg, #eef7ef, #fff 60%, #e8f2e7)' }}>
    <Paper elevation={0} sx={{ width: '100%', maxWidth: 460, p: { xs: 3, sm: 4 }, borderRadius: 4, border: '1px solid var(--lms-border)', boxShadow: '0 20px 70px rgba(31,65,49,.08)' }}>
      <Box component="img" src={argLogo} alt="Al Rahim Group" sx={{ width: 100, height: 100, objectFit: 'contain', mb: 2 }} />
      <Typography variant="h5" fontWeight={800}>Select Campus/Branch</Typography>
      <Typography color="text.secondary" sx={{ mt: 1, mb: 2 }}>Authentication successful. Please select the workspace to enter.</Typography>
      <Typography fontWeight={700} sx={{ mb: 2 }}>{user?.fullName || user?.username}</Typography>
      {loading ? <Box role="status" sx={{ p: 4, textAlign: 'center' }}><CircularProgress size={28} /><Typography>Loading campuses…</Typography></Box> : error ? <Alert severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert> : <>
        <TextField fullWidth size="small" label="Search campuses/branches" value={query} onChange={event => setQuery(event.target.value)} InputProps={{ startAdornment: <InputAdornment position="start"><Search /></InputAdornment> }} />
        <List aria-label="Available campuses" sx={{ maxHeight: '38vh', overflow: 'auto', my: 1 }}>
          {campuses.filter(item => item.name.toLowerCase().includes(query.toLowerCase())).map(item => <ListItemButton key={item.id} selected={selected === item.id} onClick={() => setSelected(item.id)} sx={{ borderRadius: 2, my: .5, border: '1px solid var(--lms-border)' }}>
            <ListItemAvatar><Avatar sx={{ bgcolor: 'rgba(var(--lms-primary-rgb),.1)', color: 'var(--lms-primary)' }}><Apartment /></Avatar></ListItemAvatar><ListItemText primary={item.name} secondary={item.address} />{selected === item.id && <CheckCircle color="primary" />}
          </ListItemButton>)}
          {!campuses.length && <Alert severity="info">No campuses/branches are available. Contact your administrator.</Alert>}
          {!!campuses.length && !campuses.some(item => item.name.toLowerCase().includes(query.toLowerCase())) && <Typography sx={{ p: 2 }}>No matching campuses.</Typography>}
        </List>
        <Button fullWidth variant="contained" disabled={!selected} onClick={() => { if (selected && campuses.some(item => item.id === selected)) { invalidateGetCache(); dispatch(setCampus(selected)); navigate('/', { replace: true }) } }}>Enter Workspace</Button>
      </>}
      <Button fullWidth sx={{ mt: 1 }} onClick={() => { dispatch(logout()); navigate('/login', { replace: true }) }}>Sign out</Button>
    </Paper>
  </Box>
}
