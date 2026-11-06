import { useEffect, useState } from 'react'
import { Alert, Avatar, Box, Button, CircularProgress, InputAdornment, Paper, Stack, TextField, Typography } from '@mui/material'
import { ApartmentOutlined, ArrowForward, CheckCircleOutline, Search, LogoutOutlined } from '@mui/icons-material'
import { WorkspaceList } from '@/components/CampusSwitcher'
import { portalColors, portalInput, portalScroll } from '@/components/portalUi'
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
  const visibleCount = campuses.filter(item => (item.name || '').toLowerCase().includes(query.trim().toLowerCase())).length
  return <Box sx={{ minHeight: '100dvh', display: 'grid', placeItems: 'center', p: { xs: 1.5, sm: 4 }, bgcolor: '#f5f7f0', backgroundImage: 'radial-gradient(ellipse at 15% 15%, #e9efdf 0%, transparent 45%)' }}>
    <Paper elevation={0} sx={{ width: '100%', maxWidth: 980, display: 'grid', gridTemplateColumns: { xs: '1fr', md: '.8fr 1.2fr' }, borderRadius: '26px', overflow: 'hidden', border: '1px solid #e2e8d9', boxShadow: '0 30px 100px -25px #30432b30' }}>
      <Box sx={{ position: 'relative', overflow: 'hidden', bgcolor: '#30432b', p: { xs: 3, md: 5 }, display: 'flex', flexDirection: 'column', color: '#fff', '&::after': { content: '""', position: 'absolute', width: 340, height: 340, border: '1px solid #ffffff0a', borderRadius: '50%', right: -130, bottom: -130, boxShadow: '0 0 0 40px #ffffff03, 0 0 0 80px #ffffff02', pointerEvents: 'none' } }}>
        <Box component="img" src={argLogo} alt="Al Rahim Group" sx={{ width: 74, height: 74, objectFit: 'contain', p: .5, bgcolor: '#fff', borderRadius: '17px', mb: { xs: 2, md: 7 }, boxShadow: '0 8px 22px #00000010' }} />
        <Typography sx={{ fontSize: 9, letterSpacing: '2px', fontWeight: 700, color: '#b8c99c', mb: 1.2 }}>YOUR WORKDAY STARTS HERE</Typography>
        <Typography sx={{ fontSize: { xs: 27, md: 36 }, fontWeight: 750, lineHeight: 1.2, letterSpacing: '-1px', color: '#fff', mb: 2 }}>Welcome back.</Typography>
        <Typography sx={{ fontSize: 12, color: '#b8c5ab', lineHeight: 1.9, maxWidth: 250 }}>One workspace for your people, organization and everyday work.</Typography>
        <Stack direction="row" alignItems="center" spacing={1.2} sx={{ mt: { xs: 3, md: 'auto' }, pt: { xs: 0, md: 7 }, position: 'relative' }}><Avatar sx={{ width: 37, height: 37, bgcolor: '#ffffff12', color: '#dbe6cd', fontSize: 14, border: '1px solid #ffffff15' }}>{(user?.fullName || user?.username || 'A').slice(0, 1)}</Avatar><Box sx={{ minWidth: 0 }}><Typography noWrap sx={{ fontSize: 12, fontWeight: 700, color: '#edf2e5' }}>{user?.fullName || user?.username}</Typography><Typography sx={{ fontSize: 10, color: '#a7ba92', mt: .3 }}>Signed in successfully</Typography></Box><CheckCircleOutline sx={{ fontSize: 17, color: '#a5bf7d' }} /></Stack>
      </Box>
      <Box sx={{ p: { xs: 2.5, sm: 4, md: 4.5 }, bgcolor: '#fff', minWidth: 0 }}>
        <Stack direction="row" spacing={1.3} alignItems="center" sx={{ mb: 2.5 }}><Box sx={{ width: 42, height: 42, display: 'grid', placeItems: 'center', borderRadius: '13px', bgcolor: '#f1f5e9', color: '#7b9850', border: '1px solid #e6ecdc' }}><ApartmentOutlined sx={{ fontSize: 22 }} /></Box><Box><Typography sx={{ fontSize: 20, fontWeight: 800, color: portalColors.ink, letterSpacing: '-.6px' }}>Choose your workspace</Typography><Typography sx={{ fontSize: 11, color: '#89957a', mt: .5 }}>Select a campus or branch to continue.</Typography></Box></Stack>
        {loading ? <Box role="status" sx={{ py: 7, textAlign: 'center' }}><CircularProgress size={25} sx={{ color: '#8ba55d' }} /><Typography sx={{ fontSize: 12, color: '#92a07f', mt: 2 }}>Finding your workspaces?</Typography></Box> : error ? <Alert sx={{ borderRadius: '12px', my: 3, fontSize: 12 }} severity="error" action={<Button onClick={() => setRetry(value => value + 1)}>Retry</Button>}>{error}</Alert> : <>
          <TextField fullWidth size="small" placeholder="Find your campus or branch?" inputProps={{ 'aria-label': 'Search campuses/branches' }} value={query} onChange={event => setQuery(event.target.value)} InputProps={{ startAdornment: <InputAdornment position="start"><Search sx={{ fontSize: 18, color: '#8d9c7a' }} /></InputAdornment> }} sx={portalInput} />
          <Stack direction="row" justifyContent="space-between" sx={{ mt: 2.2, mb: 1 }}><Typography sx={{ fontSize: 9, fontWeight: 800, letterSpacing: '1.2px', color: '#929e83' }}>AVAILABLE WORKSPACES</Typography><Typography sx={{ fontSize: 10, color: '#89987a' }}>{visibleCount} {visibleCount === 1 ? 'workspace' : 'workspaces'}</Typography></Stack>
          <Box sx={{ maxHeight: { xs: '30dvh', md: 290 }, minHeight: 140, overflowY: 'auto', mx: -1, mb: 2, ...portalScroll }}><WorkspaceList items={campuses} selected={selected} onSelect={setSelected} currentLabel="Selected" query={query} /></Box>
          <Button fullWidth variant="contained" endIcon={<ArrowForward sx={{ fontSize: '17px !important' }} />} disabled={!selected} sx={{ py: 1.5, borderRadius: '12px', fontSize: 12, bgcolor: '#668343', '&:hover': { bgcolor: '#506c32' }, boxShadow: '0 5px 14px #66834320' }} onClick={() => { if (selected && campuses.some(item => item.id === selected)) { invalidateGetCache(); dispatch(setCampus(selected)); navigate('/', { replace: true }) } }}>Enter Workspace</Button>
        </>}
        <Button fullWidth startIcon={<LogoutOutlined sx={{ fontSize: '15px !important' }} />} sx={{ mt: 1.4, fontSize: 11, color: '#929f84', borderRadius: '10px' }} onClick={() => { dispatch(logout()); navigate('/login', { replace: true }) }}>Sign out</Button>
      </Box>
    </Paper>
  </Box>
}
