import React, { Suspense, lazy, useState } from 'react'
import { Alert, Box, Button, CircularProgress, Grid, Paper, TextField, Typography } from '@mui/material'
import { Navigate, Route, Routes, Link, useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from './hooks'
import { loginSuccess, setCampus } from './store/authSlice'
import { Layout } from './components/Layout'
import api from './services/api'
import { screens } from './screens'
import { modules, moduleForGroup } from './modules'
import Login from './pages/auth/LoginPage'
import CampusSelection from './pages/auth/CampusSelection'
import { HomePage } from './pages/HomePage'
import { MenuCardGrid } from './components/MenuCard'

import { screenLoaders } from './screenLoaders'
const pages = Object.fromEntries(screens.map(screen => [screen.component, lazy(screenLoaders[screen.component] as () => Promise<{ default: React.ComponentType<any> }>)]))
const AdditionalInfo = lazy(() => import('./pages/admin/EmployeeAdditionalInfoPage').then(module => ({ default: module.EmployeeAdditionalInfoPage })))
const Dashboard = lazy(() => import('./pages/dashboard/HrAttritionDashboard').then(module => ({ default: module.HrAttritionDashboard })))
class ScreenBoundary extends React.Component<{ children: React.ReactNode }, { error: boolean }> {
  state = { error: false }
  static getDerivedStateFromError() { return { error: true } }
  render() { return this.state.error ? <Alert severity="error" action={<Button onClick={() => window.location.reload()}>Reload</Button>}>This screen could not load. Reload to try again.</Alert> : this.props.children }
}
function Home({ module }: { module?: string }) {
 const selected = modules.find(item => item.key === module)
 const visible = screens.filter(screen => moduleForGroup(screen.group) === module)
 const navigate = useNavigate()
 if (!module) return <HomePage/>
 const cards = [...new Set(visible.map(screen => screen.group))].map(group => ({title:group,items:visible.filter(screen=>screen.group===group).map(screen=>screen.label),permissionsPreFiltered:true,onItemClick:(label:string)=>{const screen=visible.find(item=>item.label===label);if(screen)navigate(screen.path)}}))
 return <Layout><Box><Paper elevation={0} sx={{py:.8,px:{xs:1.5,sm:3},minHeight:66,mb:2,background:'linear-gradient(105deg, color-mix(in srgb, var(--lms-primary) 10%, white) 0%, rgba(255, 255, 255, 0.98) 72%)',border:'1px solid color-mix(in srgb, var(--lms-primary) 22%, transparent)',borderRadius:'18px',boxShadow:'0 5px 18px rgba(31,65,49,.045)',display:'flex',alignItems:'center',justifyContent:'space-between',gap:1,flexWrap:'wrap'}}><Typography variant="body2" sx={{fontWeight:700}}><Link to="/">Home</Link> / {selected?.name}</Typography>{module==='hr'&&<Button component={Link} to="/hr/attrition" size="small" variant="outlined">HR Dashboard</Button>}</Paper><Box sx={{p:{xs:1,md:3}}}><MenuCardGrid cards={cards}/></Box></Box></Layout>
}
export default function App() {
  const authenticated = useAppSelector(state => state.auth.isAuthenticated)
  const campusId = useAppSelector(state => state.auth.selectedCampusId)
  if (!authenticated) return <Routes><Route path="*" element={<Login/>}/></Routes>
  if (!campusId) return <Routes><Route path="/campus-selection" element={<CampusSelection/>}/><Route path="*" element={<Navigate to="/campus-selection" replace/>}/></Routes>
  return <Suspense fallback={<Box sx={{ p: 6, textAlign: 'center' }}><CircularProgress/></Box>}><Routes><Route path="/login" element={<Navigate to="/" replace/>}/><Route path="/" element={<Home/>}/><Route path="/hr" element={<Home module="hr"/>}/>{modules.map(item => <Route key={item.key} path={"/modules/"+item.key} element={<Home module={item.key}/>}/>)}<Route path="/hr/attrition" element={<Layout><Dashboard/></Layout>}/><Route path="/hr/employee/additional-info/:id" element={<ScreenBoundary><AdditionalInfo/></ScreenBoundary>}/>{screens.map(screen => { const Page = pages[screen.component]; return <Route key={screen.path} path={screen.path} element={<Layout><ScreenBoundary key={screen.path}><Page/></ScreenBoundary></Layout>}/> })}<Route path="*" element={<Navigate to="/" replace/>}/></Routes></Suspense>
}
