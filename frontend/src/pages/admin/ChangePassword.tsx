import React, { useState } from 'react'
import { Alert, Button, Paper, Stack, TextField, Typography } from '@mui/material'
import api from '@/services/api'
import { useAppDispatch } from '@/hooks'
import { logout } from '@/store/authSlice'
export default function ChangePassword() {
 const dispatch=useAppDispatch(),[currentPassword,setCurrent]=useState(''),[newPassword,setNext]=useState(''),[confirm,setConfirm]=useState(''),[error,setError]=useState(''),[busy,setBusy]=useState(false)
 return <Paper component="form" onSubmit={async event=>{event.preventDefault();if(newPassword!==confirm){setError('New passwords do not match.');return}setBusy(true);setError('');try{await api.post('/api/auth/change-password',{currentPassword,newPassword});dispatch(logout());window.location.assign('/login')}catch(e:any){setError(e.response?.data?.error||'Password could not be changed.')}finally{setBusy(false)}}} sx={{p:3,maxWidth:520}}><Stack spacing={2}><Typography variant="h5" fontWeight={700}>Change Password</Typography><Typography color="text.secondary">Sign in again after changing your password.</Typography>{error&&<Alert severity="error">{error}</Alert>}<TextField label="Current Password" type="password" autoComplete="current-password" required value={currentPassword} onChange={e=>setCurrent(e.target.value)}/><TextField label="New Password" type="password" autoComplete="new-password" required inputProps={{minLength:12}} helperText="At least 12 characters." value={newPassword} onChange={e=>setNext(e.target.value)}/><TextField label="Confirm New Password" type="password" autoComplete="new-password" required value={confirm} onChange={e=>setConfirm(e.target.value)}/><Button type="submit" variant="contained" disabled={busy}>{busy?'Saving...':'Change Password'}</Button></Stack></Paper>
}
