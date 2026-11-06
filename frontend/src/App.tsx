import React, { Suspense, lazy, useState } from 'react'
import { Alert, Box, Button, CircularProgress, Grid, Paper, TextField, Typography } from '@mui/material'
import { Navigate, Route, Routes, Link, useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from './hooks'
import { loginSuccess, setCampus } from './store/authSlice'
import { Layout } from './components/Layout'
import api from './services/api'
import { screens } from './screens'
import { modules, moduleForGroup } from './modules'
import { WorkflowAwareScreen } from './workflow/WorkflowRecord'
import { WorkflowInbox } from './workflow/WorkflowInbox'
import { WorkflowManagement } from './workflow/WorkflowManagement'

import { screenLoaders } from './screenLoaders'
const pages = Object.fromEntries(screens.map(screen => [screen.component, lazy(screenLoaders[screen.component] as () => Promise<{ default: React.ComponentType<any> }>)]))
const AdditionalInfo = lazy(() => import('./pages/admin/EmployeeAdditionalInfoPage').then(module => ({ default: module.EmployeeAdditionalInfoPage })))
const Dashboard = lazy(() => import('./pages/dashboard/HrAttritionDashboard').then(module => ({ default: module.HrAttritionDashboard })))
class ScreenBoundary extends React.Component<{ children: React.ReactNode }, { error: boolean }> {
  state = { error: false }
  static getDerivedStateFromError() { return { error: true } }
  render() { return this.state.error ? <Alert severity="error" action={<Button onClick={() => window.location.reload()}>Reload</Button>}>This screen could not load. Reload to try again.</Alert> : this.props.children }
}
function Login() {
  const dispatch = useAppDispatch(), navigate = useNavigate()
  const authenticated = useAppSelector(state => state.auth.isAuthenticated)
  const [username, setUsername] = useState(''), [password, setPassword] = useState(''), [error, setError] = useState(''), [busy, setBusy] = useState(false)
  if (authenticated) return <Navigate to="/" replace/>
  return <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center', bgcolor: 'var(--lms-primary-soft)', p: 3 }}><Paper component="form" onSubmit={async event => {
    event.preventDefault(); setBusy(true); setError('')
    try { const { data } = await api.post('/api/auth/login', { username, password }); dispatch(loginSuccess({ ...data, refreshToken: '' })); dispatch(setCampus(data.user.entityId)); localStorage.setItem('clientName', 'ARG ERP'); localStorage.setItem('clientId', String(data.user.clientId)); navigate('/', { replace: true }) }
    catch (e: any) { setError(e.response?.data?.error || 'Unable to sign in. Check your credentials and connection.') }
    finally { setBusy(false) }
  }} sx={{ p: 4, width: '100%', maxWidth: 420, borderRadius: 3 }}><Typography variant="h4" fontWeight={800} color="primary.dark">ARG ERP</Typography><Typography color="text.secondary" sx={{ mt: 1, mb: 3 }}>Human Resources · Sign in</Typography>{error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}<TextField fullWidth required label="Username" autoComplete="username" value={username} onChange={e => setUsername(e.target.value)} sx={{ mb: 2 }}/><TextField fullWidth required label="Password" autoComplete="current-password" type="password" value={password} onChange={e => setPassword(e.target.value)} sx={{ mb: 3 }}/><Button fullWidth variant="contained" type="submit" disabled={busy}>{busy ? 'Signing in…' : 'Sign in'}</Button></Paper></Box>
}
function Home({ module }: { module?: string }) {
 const selected = modules.find(item => item.key === module)
 const visible = screens.filter(screen => !module || moduleForGroup(screen.group) === module)
 return <Layout><Typography variant="h5" fontWeight={800} sx={{ mb: 1 }}>{selected?.name || 'ARG ERP'}</Typography><Typography color="text.secondary" sx={{ mb: 3 }}>{selected?.description || 'Choose a module to open your workspace.'}</Typography>{!module ? <Grid container spacing={2}>{modules.map(item => <Grid item xs={12} md={4} key={item.key}><Paper component={Link} to={'/modules/'+item.key} sx={{ display:'block',p:3,minHeight:160,textDecoration:'none',color:'text.primary',border:'1px solid var(--lms-border)','&:hover':{bgcolor:'var(--lms-primary-soft)',borderColor:'primary.main'} }}><Typography variant="h5" fontWeight={800} color="primary.dark" sx={{mb:1}}>{item.name}</Typography><Typography color="text.secondary">{item.description}</Typography></Paper></Grid>)}</Grid> : [...new Set(visible.map(screen => screen.group))].map(group => <Box key={group} sx={{ mb: 3 }}><Typography variant="h6" sx={{ mb: 1 }}>{group}</Typography><Grid container spacing={1.5}>{visible.filter(screen => screen.group === group).map(screen => <Grid item xs={12} sm={6} lg={4} key={screen.path}><Paper component={Link} to={screen.path} sx={{ display:'block',p:2,textDecoration:'none',color:'text.primary',border:'1px solid var(--lms-border)','&:hover':{bgcolor:'var(--lms-primary-soft)',borderColor:'primary.main'} }}>{screen.label}</Paper></Grid>)}</Grid></Box>)}</Layout>
}
export default function App() {
  const authenticated = useAppSelector(state => state.auth.isAuthenticated)
  if (!authenticated) return <Routes><Route path="*" element={<Login/>}/></Routes>
  return <Suspense fallback={<Box sx={{ p: 6, textAlign: 'center' }}><CircularProgress/></Box>}><Routes><Route path="/login" element={<Navigate to="/" replace/>}/><Route path="/" element={<Home/>}/><Route path="/hr" element={<Home module="hr"/>}/>{modules.map(item => <Route key={item.key} path={"/modules/"+item.key} element={<Home module={item.key}/>}/>)}<Route path="/RequestNotifications" element={<Layout><WorkflowInbox/></Layout>}/><Route path="/security/WorkflowManagement" element={<Layout><WorkflowManagement/></Layout>}/><Route path="/hr/attrition" element={<Layout><Dashboard/></Layout>}/><Route path="/hr/employee/additional-info/:id" element={<ScreenBoundary><AdditionalInfo/></ScreenBoundary>}/>{screens.map(screen => { const Page = pages[screen.component]; return <Route key={screen.path} path={screen.path} element={<Layout><ScreenBoundary key={screen.path}><WorkflowAwareScreen><Page/></WorkflowAwareScreen></ScreenBoundary></Layout>}/> })}<Route path="*" element={<Navigate to="/" replace/>}/></Routes></Suspense>
}
