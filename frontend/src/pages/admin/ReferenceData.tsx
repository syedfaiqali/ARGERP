import React, { useCallback, useEffect, useState } from 'react'
import { Alert, Grid, MenuItem, TextField } from '@mui/material'
import api from '@/services/api'
import { useDispatch } from 'react-redux'
import { clearHrScopedLookups } from '@/store/hrLookupsSlice'
import { SchemaTable } from '@/components/SchemaTable'
import { FormInput } from '@/components/FormInput'
import { invalidateGetCache } from '@/constants/globalItems'
const lists = [['classes', 'Teaching Classes'], ['teaching-subjects', 'Teaching Subjects'], ['banks', 'Bank Groups'], ['religions', 'Religions'], ['relations', 'Relationship Types'], ['diseases', 'Diseases'], ['employment-types', 'Employment Types'], ['income-tax', 'Income Tax Categories']] as const
export default function ReferenceData() {
  const dispatch = useDispatch()
  const [kind, setKind] = useState('banks'), [rows, setRows] = useState<any[]>([]), [error, setError] = useState('')
  const load = useCallback(async () => { try { setRows((await api.get('/api/HumanResource/Reference/v3/' + kind)).data); setError('') } catch { setError('Reference data could not be loaded. Please retry.') } }, [kind])
  useEffect(() => { setRows([]); void load() }, [load])
  return <><TextField select label="Reference list" value={kind} onChange={e => setKind(e.target.value)} sx={{ minWidth: 260, mb: 3 }}>{lists.map(([value, label]) => <MenuItem key={value} value={value}>{label}</MenuItem>)}</TextField>{error && <Alert severity="error">{error}</Alert>}<SchemaTable key={kind} title={lists.find(row => row[0] === kind)?.[1] || 'Reference Data'} columns={[{ id: 'code', label: 'Code' }, { id: 'name', label: 'Name' }, { id: 'description', label: 'Description' }]} rows={rows} initialFormData={{ id: 0, code: '', name: '', description: '', inactive: false }} useActionRights={false} mandatoryFields={[{ key: 'code', label: 'Code' }, { key: 'name', label: 'Name' }]}
    onSave={async data => { try { await api.request({ method: data.id ? 'put' : 'post', url: '/api/HumanResource/Reference/v3/' + kind + (data.id ? '/' + data.id : ''), data }); invalidateGetCache(); dispatch(clearHrScopedLookups()); await load(); return true } catch (e: any) { setError(e.response?.data?.error || 'Unable to save this record.'); return false } }}
    onDelete={async data => { await api.delete('/api/HumanResource/Reference/v3/' + kind + '/' + data.id); invalidateGetCache(); dispatch(clearHrScopedLookups()); await load() }}
    renderForm={(data, setData, _mode, readOnly) => <Grid container spacing={2}><Grid item xs={12} md={4}><FormInput label="Code" value={data.code} onChange={e => setData({ ...data, code: e.target.value })} disabled={readOnly} required inputProps={{ maxLength: 10 }}/></Grid><Grid item xs={12} md={8}><FormInput label="Name" value={data.name} onChange={e => setData({ ...data, name: e.target.value })} disabled={readOnly} required inputProps={{ maxLength: 150 }}/></Grid><Grid item xs={12}><FormInput label="Description" multiline value={data.description || ''} onChange={e => setData({ ...data, description: e.target.value })} disabled={readOnly} inputProps={{ maxLength: 500 }}/></Grid></Grid>} /></>
}
