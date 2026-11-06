import Autocomplete from "@/components/ClearableAutocomplete";
import TextField from "@/components/ClearableTextField";
import { useCallback, useEffect, useState } from 'react'
import { Alert, Box, Button, Card, CardContent, Checkbox, Chip, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, FormGroup, ListSubheader, MenuItem, Stack, Tab, Tabs, Typography } from "@mui/material";
import ArrowDownward from '@mui/icons-material/ArrowDownward'
import { Link } from 'react-router-dom'
import { SchemaTable } from '@/components/SchemaTable'
import { useAppSelector } from '@/hooks'
import { Definition, Directory, Identity, newStep, Policy, Registration, RelatedUser, relatedUserLabels, ScreenCatalogue, Selector, Step, wfDelete, wfGet, wfPost, wfPut, workflowEnabled, workflowError, WorkflowTenant } from './api'

type Group = { key: string; name: string }
type Binding = { groupKey: string; version: number; selectors: Selector[] }
type Option = Selector & { label: string }
const emptyDirectory: Directory = { users: [], roles: [], entities: [] }
export function WorkflowManagementShortcut() {
  const token = useAppSelector(s => s.auth.accessToken)
  const [allowed, setAllowed] = useState(false)
  useEffect(() => {
    setAllowed(false)
    if (!workflowEnabled || !token) return
    const abort = new AbortController()
    wfGet<Identity>('/me', abort.signal).then(me => { if (!abort.signal.aborted) setAllowed(me.isPlatformAdmin) }).catch(() => {})
    return () => abort.abort()
  }, [token])
  return allowed ? <Box sx={{ px: 2, py: 1, display: 'flex', justifyContent: 'flex-end' }}><Button component={Link} to="/security/WorkflowManagement">Configure approval workflows</Button></Box> : null
}
function selectorOptions(groups: Group[], directory: Directory, global: boolean): Option[] {
  return [
    ...groups.map(g => ({ kind: 'Group' as const, id: g.key, label: `Group · ${g.name}` })),
    ...(global ? [] : directory.roles.map(r => ({ kind: 'Role' as const, id: String(r.id), label: `Role · ${r.name}` }))),
    ...(global ? [] : directory.users.map(u => ({ kind: 'User' as const, id: String(u.userId), label: `User · ${u.name}${u.isTenantAdmin ? ' (tenant admin)' : ''}` }))),
  ]
}
function Assignees({ value, options, onChange, disabled, label = 'Assignee groups, roles or users', helperText }: { value: Selector[]; options: Option[]; onChange: (next: Selector[]) => void; disabled?: boolean; label?: string; helperText?: string }) {
  return <Autocomplete multiple disabled={disabled} options={options} value={value.map(s => options.find(o => o.kind === s.kind && o.id === s.id) || { ...s, label: `${s.kind} · ${s.id} (unavailable)` })}
    isOptionEqualToValue={(a, b) => a.kind === b.kind && a.id === b.id} getOptionLabel={o => o.label}
    onChange={(_, selected) => onChange(selected.map(({ kind, id }) => ({ kind, id })))} renderInput={props => <TextField {...props} label={label} helperText={helperText} />} />
}
const relatedUserChoices: [RelatedUser, string][] = [
  ['Parent', 'Parent — guardians linked to the requesting student'],
  ['ReportTo', 'Report To — the requesting employee’s reporting manager'],
  ['Teacher', 'Teacher — class teacher of the requesting student, or of a requesting parent’s children'],
]
export function WorkflowManagement() {
  const token = useAppSelector(s => s.auth.accessToken)
  if (!workflowEnabled) return <Alert severity="info">Workflow Management is not enabled in this deployment.</Alert>
  return <WorkflowManagementSession key={token || 'anonymous'} />
}
function WorkflowManagementSession() {
  const [me, setMe] = useState<Identity | null>(null)
  const [tenants, setTenants] = useState<WorkflowTenant[]>([])
  const [ready, setReady] = useState(false)
  const [error, setError] = useState('')
  const [retry, setRetry] = useState(0)
  const [scope, setScope] = useState('global')
  const [tenantId, setTenantId] = useState<number | ''>('')
  const [tab, setTab] = useState('definitions')
  const [busy, setBusy] = useState(false)
  const [dirty, setDirty] = useState(false)
  useEffect(() => {
    const abort = new AbortController()
    setReady(false); setMe(null); setError('')
    const load = async () => {
      const identity = await wfGet<Identity>('/me', abort.signal)
      if (abort.signal.aborted) return
      setMe(identity)
      // Do not even request configuration data for a tenant administrator.
      if (!identity.isPlatformAdmin) return
      const available = await wfGet<WorkflowTenant[]>('/tenants', abort.signal)
      if (!abort.signal.aborted) { setTenants(available); setReady(true) }
    }
    void load().catch(e => { if (!abort.signal.aborted) setError(workflowError(e)) })
    return () => abort.abort()
  }, [retry])
  const tenant = tenants.find(t => t.id === tenantId)
  const confirmChange = () => !dirty || window.confirm('Changing tenant or configuration scope clears unsaved workflow and mapping edits. Continue?')
  return <Box sx={{ p: 3 }}>
    <Stack direction="row" justifyContent="space-between"><Typography variant="h5">Workflow Management</Typography><Button component={Link} to="/RequestNotifications">Workflow inbox</Button></Stack>
    {error ? <Alert severity="error" action={<Button onClick={() => setRetry(r => r + 1)}>Retry</Button>}>{error}</Alert>
      : me && !me.isPlatformAdmin ? <Alert severity="warning" sx={{ my: 2 }}>Workflow Management is restricted to superadmins. You can still review your assigned requests in the workflow inbox.</Alert>
      : !ready ? <Typography role="status">Checking superadmin access and loading tenants…</Typography> : null}
    {ready && me?.isPlatformAdmin && <>
      <Typography color="text.secondary" sx={{ my: 1 }}>Superadmins configure shared approval groups and map them to each tenant’s roles or users. Every chain must finish with a hard approval.</Typography>
      <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap sx={{ my: 2 }}>
        <TextField select disabled={busy} size="small" label="Configuration scope" value={scope} onChange={e => { if (e.target.value !== scope && confirmChange()) setScope(e.target.value) }} sx={{ minWidth: 220 }}><MenuItem value="global">Platform defaults</MenuItem><MenuItem value="tenant">Selected tenant override</MenuItem></TextField>
        <Autocomplete disabled={busy} options={tenants} value={tenant || null} sx={{ minWidth: 320 }}
          getOptionLabel={t => t.name} isOptionEqualToValue={(a, b) => a.id === b.id}
          renderOption={(props, t) => <li {...props} key={t.id}>{t.name}</li>}
          onChange={(_, next) => { if ((next?.id || '') !== tenantId && confirmChange()) setTenantId(next?.id || '') }}
          renderInput={props => <TextField {...props} size="small" label="Tenant for mappings and preview" />} />
      </Stack>
      {!tenants.length && <Alert severity="warning">No active tenants are available. Shared groups and platform defaults can still be configured.</Alert>}
      <Alert severity="info">{tenant ? `Mappings and assignee previews target ${tenant.name}.` : 'Select a tenant to map shared groups and preview approvers.'} {scope === 'global' ? 'Workflow definitions are platform-wide; tenant mappings remain separate.' : 'Workflow definitions and policies apply only to the selected tenant.'}</Alert>
      <WorkflowConfiguration key={`${scope}:${tenantId}`} me={me} scope={scope} tenantId={tenantId} tenantName={tenant?.name || ''} tab={tab} onTabChange={setTab} onBusyChange={setBusy} onDirtyChange={setDirty} />
    </>}
  </Box>
}
function WorkflowConfiguration({ me, scope, tenantId, tenantName, tab, onTabChange, onBusyChange, onDirtyChange }: { me: Identity; scope: string; tenantId: number | ''; tenantName: string; tab: string; onTabChange: (tab: string) => void; onBusyChange: (busy: boolean) => void; onDirtyChange: (dirty: boolean) => void }) {
  const [definitions, setDefinitions] = useState<Definition[]>([])
  const [policies, setPolicies] = useState<Policy[]>([])
  const [groups, setGroups] = useState<Group[]>([])
  const [bindings, setBindings] = useState<Binding[]>([])
  const [directory, setDirectory] = useState<Directory>(emptyDirectory)
  const [actions, setActions] = useState<Registration[]>([])
  const [screens, setScreens] = useState<ScreenCatalogue[]>([])
  const [selected, setSelected] = useState<Definition | null>(null)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')
  const [busy, setBusy] = useState(false)
  const [loading, setLoading] = useState(true)
  const [visual, setVisual] = useState(true)
  const [stepEditor, setStepEditor] = useState<{ index: number; step: Step } | null>(null)
  const [requester, setRequester] = useState<number | ''>('')
  const [campus, setCampus] = useState<number | ''>('')
  const [simulation, setSimulation] = useState<{ errors: string[]; steps: { name: string; blocked: boolean; users: Directory['users']; relatedUser?: RelatedUser | null; relatedUserMissing?: boolean }[] } | null>(null)
  const [policyScreen, setPolicyScreen] = useState('')
  const [policyAction, setPolicyAction] = useState('Save')
  const [groupKey, setGroupKey] = useState('')
  const [groupName, setGroupName] = useState('')
  const [mappingKey, setMappingKey] = useState('')
  const [mapping, setMapping] = useState<Selector[]>([])
  const [mappingVersion, setMappingVersion] = useState(0)
  useEffect(() => {
    const original = definitions.find(d => d.id === selected?.id)
    const draftChanged = selected?.status === 'Draft' && (selected.name !== original?.name || JSON.stringify(selected.steps) !== JSON.stringify(original?.steps))
    onDirtyChange(Boolean(draftChanged || mappingKey || stepEditor || groupKey || groupName))
    return () => onDirtyChange(false)
  }, [selected, definitions, mappingKey, stepEditor, groupKey, groupName, onDirtyChange])
  const canManage = me.isPlatformAdmin
  const clientId = scope === 'global' ? 0 : tenantId
  const canConfigure = scope === 'global' || Boolean(tenantId)
  const editable = selected?.status === 'Draft' && selected.clientId === clientId
  const options = selectorOptions(groups, directory, scope === 'global')
  // The catalogue holds every configurable screen of the installation, so a superadmin sees the
  // same list whatever their own menus are. Only screens whose service registered an adapter can
  // be configured; the rest are listed disabled, because the server rejects their drafts.
  const readyControllers = new Set(actions.flatMap(a => [a.screenKey, ...(a.permissionControllers || [])]).map(x => x.toLowerCase()))
  const pendingScreens = screens.filter(screen => screen.controllerId && !readyControllers.has(screen.controllerId.toLowerCase()))
    .map(screen => ({ key: screen.controllerId, label: `${screen.module} / ${screen.name}` }))
  const load = useCallback(async (signal?: AbortSignal) => {
    const query = `scope=${scope}${tenantId ? `&clientId=${tenantId}` : ''}`
    const [defs, rules, logicalGroups, mapped, users, catalog, allScreens] = await Promise.all([
      canConfigure ? wfGet<Definition[]>(`/definitions?${query}`, signal) : Promise.resolve([]),
      canConfigure ? wfGet<Policy[]>(`/policies?${query}`, signal) : Promise.resolve([]),
      wfGet<Group[]>('/groups', signal), tenantId ? wfGet<Binding[]>(`/bindings?clientId=${tenantId}`, signal) : Promise.resolve([]),
      tenantId ? wfGet<Directory>(`/directory?clientId=${tenantId}`, signal) : Promise.resolve(emptyDirectory), wfGet<Registration[]>('/actions', signal),
      wfGet<ScreenCatalogue[]>('/screens', signal),
    ])
    if (signal?.aborted) return
    setDefinitions(defs); setPolicies(rules); setGroups(logicalGroups); setBindings(mapped); setDirectory(users); setActions(catalog); setScreens(allScreens)
    setRequester(current => users.users.some(u => u.userId === current) ? current : users.users[0]?.userId || '')
    setLoading(false)
  }, [scope, tenantId, canConfigure])
  useEffect(() => {
    if (!workflowEnabled) return
    const abort = new AbortController(); setSelected(null); setSimulation(null); setError('')
    load(abort.signal).catch(e => { if (!abort.signal.aborted) { setError(workflowError(e)); setLoading(false) } })
    return () => abort.abort()
  }, [load])
  const run = async (action: () => Promise<void>) => {
    if (busy || loading) return
    setBusy(true); onBusyChange(true); setError(''); setMessage('')
    try { await action(); await load() } catch (e) { setError(workflowError(e)) }
    finally { setBusy(false); onBusyChange(false) }
  }
  const assigneeSummary = (step: Step) => {
    const selectors = step.assignees.map(s => options.find(o => o.kind === s.kind && o.id === s.id)?.label || `${s.kind}: ${s.id}`).join(' · ')
    if (!step.relatedUser) return selectors || 'No assignees configured'
    return `Related user · ${relatedUserLabels[step.relatedUser]} — ${selectors ? `fallback: ${selectors}` : 'no fallback; blocks if the relationship is missing'}`
  }
  const policy = (screen: string, action: string, client = clientId) => policies.find(p => p.clientId === client && p.screenKey === screen && p.actionKey === action)
  const draft = (definition: Definition) => ({ name: definition.name, screenKey: definition.screenKey, actionKey: definition.actionKey, steps: definition.steps })
  const save = async () => {
    if (!selected || !editable) return
    const saved = await wfPut<Definition>(`/definitions/${selected.id}`, { scope, clientId, draft: draft(selected), expectedVersion: selected.version })
    setSelected(saved); setMessage('Draft saved. Running workflows are unchanged.'); return saved
  }
  const create = () => run(async () => {
    if (!policyScreen || !canConfigure) { setError('Select a workflow-ready screen and configuration tenant first.'); return }
    const created = await wfPost<Definition>('/definitions', { scope, clientId, draft: { name: `${actions.find(a => a.screenKey === policyScreen)?.name} ${policyAction}`, screenKey: policyScreen, actionKey: policyAction, steps: [newStep()] } })
    setSelected(created)
  })
  const move = (index: number, delta: number) => {
    if (!selected || !editable || index + delta < 0 || index + delta >= selected.steps.length) return
    const next = [...selected.steps]; [next[index], next[index + delta]] = [next[index + delta], next[index]]
    setSelected({ ...selected, steps: next }); setSimulation(null)
  }
  const stepActions = (index: number) => <Stack direction="row" spacing={0.5}>
    <Button size="small" disabled={!editable} onClick={() => setStepEditor({ index, step: { ...newStep(), ...structuredClone(selected!.steps[index]) } })}>Edit</Button>
    <Button size="small" aria-label={`Move step ${index + 1} up`} disabled={!editable || index === 0} onClick={() => move(index, -1)}>↑</Button>
    <Button size="small" aria-label={`Move step ${index + 1} down`} disabled={!editable || index === (selected?.steps.length || 0) - 1} onClick={() => move(index, 1)}>↓</Button>
    <Button size="small" color="error" disabled={!editable} onClick={() => setSelected(s => s && ({ ...s, steps: s.steps.filter((_, i) => i !== index) }))}>Delete</Button>
  </Stack>
  if (!workflowEnabled) return <Alert severity="info">Workflow Management is not enabled in this deployment.</Alert>
  return <Box sx={{ py: 2 }}>
    {error && <Alert severity="error" sx={{ my: 1 }}>{error}</Alert>}{message && <Alert severity="success" sx={{ my: 1 }}>{message}</Alert>}
    {!canManage && !error && <Typography role="status">Checking administrator access…</Typography>}
    {canManage && <>
      <Button disabled={busy || loading} onClick={() => run(async () => { setMessage('Configuration refreshed.') })}>Refresh</Button>
      {loading && <Typography role="status">Loading workflow configuration…</Typography>}
      <Box component="fieldset" disabled={busy || loading} sx={{ border: 0, p: 0, m: 0, minWidth: 0 }}>
      <Tabs value={tab} onChange={(_, value) => onTabChange(value)}><Tab value="definitions" label="Definitions & visual creator" /><Tab value="groups" label="Approver groups & mappings" /></Tabs>
      {tab === 'definitions' ? <>
        <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap sx={{ my: 2 }}>
          <TextField select label="Screen" size="small" value={policyScreen} onChange={e => setPolicyScreen(e.target.value)} sx={{ minWidth: 230 }}>
            <MenuItem value="">Select screen</MenuItem>
            <ListSubheader>Workflow-ready</ListSubheader>
            {actions.map(a => <MenuItem key={a.screenKey} value={a.screenKey}>{a.module} / {a.name}</MenuItem>)}
            {pendingScreens.length > 0 && <ListSubheader>Not workflow-ready yet</ListSubheader>}
            {pendingScreens.map(screen => <MenuItem key={`pending-${screen.key}`} value={screen.key} disabled>{screen.label} — adapter pending</MenuItem>)}
          </TextField>
          <TextField select label="Action" size="small" value={policyAction} onChange={e => setPolicyAction(e.target.value)}><MenuItem value="Save">Save (create / update)</MenuItem><MenuItem value="Delete">Delete</MenuItem></TextField>
          <Button variant="contained" disabled={busy || !policyScreen || !canConfigure} onClick={create}>New draft</Button>
          <Button disabled={busy || !policyScreen || !canConfigure} onClick={() => run(async () => { await wfPut('/policies', { scope, clientId, screenKey: policyScreen, actionKey: policyAction, mode: 'Inherit', expectedVersion: policy(policyScreen, policyAction)?.version || 0 }); setMessage(scope === 'global' ? 'Global binding cleared for new submissions.' : `${tenantName} now inherits the global default.`) })}>{scope === 'global' ? 'Clear default' : 'Inherit default'}</Button>
          {scope === 'tenant' && <Button color="warning" disabled={busy || !policyScreen || !canConfigure} onClick={() => run(async () => { await wfPut('/policies', { scope, clientId, screenKey: policyScreen, actionKey: policyAction, mode: 'Disabled', expectedVersion: policy(policyScreen, policyAction)?.version || 0 }); setMessage('Workflow disabled for new submissions only. Running requests are unchanged.') })}>Disable for selected tenant</Button>}
        </Stack>
        {policyScreen && <Alert severity="info" sx={{ mb: 2 }}>Current setting: {policy(policyScreen, policyAction)?.mode || 'Inherit'}{scope === 'tenant' ? ` · Global default: ${policy(policyScreen, policyAction, 0)?.definitionId ? 'Configured' : 'None'}` : ''}. Publishing a draft replaces the current binding for new submissions.</Alert>}
        <SchemaTable title="Workflow definitions and versions" rows={definitions} columns={[
          { id: 'name', label: 'Name' }, { id: 'screenKey', label: 'Screen' }, { id: 'actionKey', label: 'Action' }, { id: 'clientId', label: 'Scope', format: value => value === 0 ? 'Platform default' : 'Tenant' }, { id: 'status', label: 'Status', format: (value, row) => value === 'Published' && policy(row.screenKey, row.actionKey, row.clientId)?.definitionId === row.id ? 'Published (live)' : value }, { id: 'version', label: 'Revision' },
        ]} showAddButton={false} showEditAction={false} showDeleteAction={false} useActionRights={false} onViewClick={row => { setSelected(structuredClone(row)); setSimulation(null) }} defaultLatestFirst={false} downloadEnabled={false} />
        {selected && <Box sx={{ my: 3, p: 2, border: 1, borderColor: 'divider', borderRadius: 2 }}>
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap alignItems="center">
            <Typography variant="h6">{selected.screenKey} / {selected.actionKey}</Typography><Chip label={`${selected.status} · revision ${selected.version}`} />
            <Button variant={selected.status === 'Draft' ? 'outlined' : 'contained'} disabled={busy || !canConfigure} onClick={() => run(async () => { const created = await wfPost<Definition>('/definitions', { scope, clientId, draft: draft(selected), previousVersionId: selected.id }); setSelected(created); setMessage('New editable version created.') })}>Create a new version</Button>
            {editable && <><Button disabled={busy} onClick={() => run(async () => { await save() })}>Save draft</Button><Button variant="contained" disabled={busy} onClick={() => run(async () => { const saved = await save(); if (!saved) return; const published = await wfPost<Definition>(`/definitions/${saved.id}/publish`, { expectedVersion: saved.version, expectedPolicyVersion: policy(saved.screenKey, saved.actionKey)?.version || 0 }); setSelected(published); setMessage('Published. New submissions will use only this version; the previously published version was archived.') })}>Publish</Button>
              <Button color="error" disabled={busy} onClick={() => run(async () => { await wfDelete(`/definitions/${selected.id}?expectedVersion=${selected.version}`); setSelected(null) })}>Delete draft</Button></>}
            {selected.status === 'Published' && <>
              <Button variant="outlined" disabled={busy || selected.clientId !== clientId} onClick={() => run(async () => { const restored = await wfPost<Definition>(`/definitions/${selected.id}/unpublish`, { expectedVersion: selected.version, expectedPolicyVersion: policy(selected.screenKey, selected.actionKey)?.version || 0 }); setSelected(restored); setMessage(scope === 'global' ? 'Unpublished and moved back to draft. New submissions no longer use a platform default workflow.' : `Unpublished and moved back to draft. ${tenantName} now inherits the platform default for new submissions.`) })}>Unpublish</Button>
              <Button variant="outlined" color="warning" disabled={busy || selected.clientId !== clientId} onClick={() => run(async () => { await wfPost(`/definitions/${selected.id}/archive`, { expectedVersion: selected.version, expectedPolicyVersion: policy(selected.screenKey, selected.actionKey)?.version || 0 }); setSelected(null); setMessage('Archived. Running requests retain their snapshot.') })}>Archive</Button>
            </>}
            {selected.status === 'Archived' && <Button variant="outlined" disabled={busy || selected.clientId !== clientId} onClick={() => run(async () => { const restored = await wfPost<Definition>(`/definitions/${selected.id}/unarchive`, { expectedVersion: selected.version }); setSelected(restored); setMessage('Unarchived as a draft. Publish it to make it the live workflow.') })}>Unarchive</Button>}
          </Stack>
          {!editable && <Alert severity="info" sx={{ my: 2 }}>{selected.clientId !== clientId
            ? 'This is a platform default and is read-only here. Manage it from Platform defaults, or create a new version to override it for this tenant.'
            : selected.status === 'Archived'
              ? 'This version is archived and not used for new submissions. Unarchive it to edit and publish it again, or create a new version from it.'
              : selected.status === 'Published'
                ? 'This is the live workflow for new submissions and is read-only. Unpublish it to edit it, archive it to retire it, or create a new version to change it.'
                : 'This draft is read-only here.'}</Alert>}
          <TextField fullWidth label="Workflow name" sx={{ my: 2 }} value={selected.name} disabled={!editable || busy} inputProps={{ maxLength: 150 }} onChange={e => setSelected({ ...selected, name: e.target.value })} />
          <Stack direction="row" spacing={1}><Button onClick={() => setVisual(!visual)}>{visual ? 'Show steps table' : 'Show visual chain'}</Button><Button disabled={!editable || selected.steps.length >= 30} onClick={() => setStepEditor({ index: selected.steps.length, step: newStep() })}>Add step</Button></Stack>
          {visual ? <Stack spacing={1} alignItems="stretch" sx={{ my: 2 }}>{selected.steps.map((step, index) => <Box key={index}>
            <Card variant="outlined"><CardContent><Stack direction="row" justifyContent="space-between" flexWrap="wrap"><Typography variant="h6">{index + 1}. {step.name}</Typography>{stepActions(index)}</Stack>
              <Typography>{step.decisionKind} · {step.quorum === 'Any' ? 'First decision wins' : 'Everyone must decide'} · {step.endOnPositiveDecision && index < selected.steps.length - 1 ? 'Approve ends the workflow · ' : ''}{step.negativeBehavior === 'Terminate' ? 'Hard rejection' : 'Soft negative continues'} · {step.assigneeScope}</Typography>
              <Typography variant="body2" color="text.secondary">{assigneeSummary(step)}</Typography>
              {index === selected.steps.length - 1 && <Chip size="small" sx={{ mt: 1 }} color={step.decisionKind === 'Approval' && step.negativeBehavior === 'Terminate' ? 'success' : 'error'} label="Final step must be hard approval" />}
            </CardContent></Card>{index < selected.steps.length - 1 && <Box sx={{ textAlign: 'center', pt: 1 }}><ArrowDownward aria-hidden /></Box>}
          </Box>)}</Stack> : <SchemaTable title="Ordered workflow steps" rows={selected.steps.map((s, index) => ({ ...s, id: index + 1, index }))} columns={[
            { id: 'id', label: 'Order' }, { id: 'name', label: 'Step' }, { id: 'decisionKind', label: 'Decisions' }, { id: 'quorum', label: 'Quorum' }, { id: 'negativeBehavior', label: 'Negative behavior' },
            { id: 'relatedUser', label: 'Related user', format: (value: RelatedUser | null) => value ? relatedUserLabels[value] : '—' },
            { id: 'endOnPositiveDecision', label: 'Ends on approval', format: (value: boolean) => value ? 'Yes' : 'No' },
          ]} showAddButton={false} showEditAction={false} showDeleteAction={false} showViewAction={false} useActionRights={false} extraActions={row => stepActions(row.index)} actionColumnWidth={300} defaultLatestFirst={false} pagination={false} downloadEnabled={false} />}
          <Typography variant="subtitle1" sx={{ mt: 2 }}>Test assignee resolution{tenantName ? ` for ${tenantName}` : ' — select a tenant first'} (no request is created)</Typography>
          <Stack direction="row" spacing={1} sx={{ my: 2 }} flexWrap="wrap" useFlexGap>
            <TextField select label="Requester" value={requester} onChange={e => setRequester(Number(e.target.value))} sx={{ minWidth: 240 }}><MenuItem value="">Select requester</MenuItem>{directory.users.map(u => <MenuItem key={u.userId} value={u.userId}>{u.name}</MenuItem>)}</TextField>
            <TextField select label="Record campus" value={campus} onChange={e => setCampus(e.target.value === '' ? '' : Number(e.target.value))} sx={{ minWidth: 210 }}><MenuItem value="">Tenant-wide record</MenuItem>{directory.entities.map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}</TextField>
            <Button disabled={busy || !requester || !tenantId} onClick={() => run(async () => { setSimulation(await wfPost('/simulate', { clientId: tenantId, draft: draft(selected), requesterId: requester, entityId: campus || null })) })}>Preview assignees</Button>
          </Stack>
          {simulation && <Stack spacing={1}>{simulation.errors.map(e => <Alert severity="error" key={e}>{e}</Alert>)}{simulation.steps.map((s, i) => <Alert key={i} severity={s.blocked ? 'warning' : 'success'}>{i + 1}. {s.name}{s.relatedUser ? ` (${relatedUserLabels[s.relatedUser]}${s.relatedUserMissing ? ' not found for this requester' + (s.blocked ? '' : ' — fallback assignees') : ''})` : ''}: {s.blocked ? 'Configuration blocked — no eligible assignees' : s.users.map(u => u.name).join(', ')}</Alert>)}</Stack>}
        </Box>}
      </> : <Box sx={{ py: 2 }}>
        <Alert severity="info">Shared groups are approval labels, not administrator permissions. {tenantName ? `Map each group to roles or users in ${tenantName}.` : 'Select a tenant above before mapping approvers.'} Empty bindings block approval rather than skip it.</Alert>
        {me?.isPlatformAdmin && <Stack direction="row" spacing={1} sx={{ my: 2 }}><TextField label="Stable group key" value={groupKey} onChange={e => setGroupKey(e.target.value)} /><TextField label="Group name" value={groupName} onChange={e => setGroupName(e.target.value)} /><Button disabled={busy || !groupKey || !groupName} onClick={() => run(async () => { await wfPost('/groups', { key: groupKey, name: groupName }); setGroupKey(''); setGroupName('') })}>Create shared group</Button></Stack>}
        <SchemaTable title={tenantName ? `Shared approver groups — ${tenantName}` : 'Shared approver groups'} rows={groups.map(g => ({ ...g, id: g.key, mapping: !tenantId ? 'Select a tenant' : bindings.find(b => b.groupKey === g.key)?.selectors.map(s => selectorOptions([], directory, false).find(o => o.kind === s.kind && o.id === s.id)?.label || s.id).join(' · ') || 'Unbound' }))} columns={[
          { id: 'key', label: 'Stable key' }, { id: 'name', label: 'Group' }, { id: 'mapping', label: 'Selected tenant’s binding' },
        ]} showAddButton={false} showEditAction={false} showViewAction={false} showDeleteAction={false} useActionRights={false} extraActions={row => <Stack direction="row"><Button disabled={busy || !tenantId} onClick={() => { const binding = bindings.find(b => b.groupKey === row.key); setMappingKey(row.key); setMapping(binding?.selectors || []); setMappingVersion(binding?.version || 0) }}>Map approvers</Button>{me?.isPlatformAdmin && <Button color="error" disabled={busy} onClick={() => run(async () => { await wfDelete(`/groups/${encodeURIComponent(row.key)}`) })}>Delete unused</Button>}</Stack>} actionColumnWidth={260} downloadEnabled={false} />
        {mappingKey && <Stack spacing={2} sx={{ my: 2 }}><Typography variant="h6">{tenantName} → {groups.find(g => g.key === mappingKey)?.name || mappingKey}</Typography><Assignees value={mapping} options={selectorOptions([], directory, false)} onChange={setMapping} /><Stack direction="row" spacing={1}><Button disabled={busy || !tenantId} onClick={() => run(async () => { await wfPut(`/bindings/${encodeURIComponent(mappingKey)}`, { clientId: tenantId, selectors: mapping, expectedVersion: mappingVersion }); setMessage(`Mapping saved for ${tenantName}. Other tenants are unchanged. Re-resolve any blocked active steps to use it.`); setMappingKey('') })}>Save tenant binding</Button><Button disabled={busy} onClick={() => setMappingKey('')}>Cancel mapping</Button></Stack></Stack>}
      </Box>}
      </Box>
    </>}
    <Dialog open={Boolean(stepEditor)} onClose={() => setStepEditor(null)} maxWidth="md" fullWidth>
      <DialogTitle>{stepEditor && selected && stepEditor.index >= selected.steps.length ? 'Add workflow step' : 'Edit workflow step'}</DialogTitle>
      <DialogContent>{stepEditor && <Stack spacing={2} sx={{ pt: 1 }}>
        <TextField label="Step name" value={stepEditor.step.name} inputProps={{ maxLength: 150 }} onChange={e => setStepEditor({ ...stepEditor, step: { ...stepEditor.step, name: e.target.value } })} />
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="flex-start">
          <Box sx={{ flex: 1, minWidth: 0, width: '100%' }}>
            <Assignees value={stepEditor.step.assignees} options={options} onChange={assignees => setStepEditor({ ...stepEditor, step: { ...stepEditor.step, assignees } })}
              label={stepEditor.step.relatedUser ? 'Fallback groups, roles or users (optional)' : undefined}
              helperText={stepEditor.step.relatedUser ? 'Used only when the related user cannot be found. Leave empty to block the request until the relationship is fixed.' : undefined} />
          </Box>
          <TextField select fullWidth label="Related user" value={stepEditor.step.relatedUser || ''} sx={{ flex: 1, minWidth: 0 }}
            helperText="Found from the requester’s own records when this step starts; takes precedence over the assignees."
            SelectProps={{ renderValue: value => relatedUserLabels[value as RelatedUser] }}
            onChange={e => setStepEditor({ ...stepEditor, step: { ...stepEditor.step, relatedUser: (e.target.value || null) as RelatedUser | null } })}>
            <MenuItem value="">None — use the assignees</MenuItem>
            {relatedUserChoices.map(([value, text]) => <MenuItem key={value} value={value}>{text}</MenuItem>)}
          </TextField>
        </Stack>
        {([
          ['decisionKind', 'Decision labels', [['Approval', 'Approve / Reject'], ['Recommendation', 'Recommend / Decline']]],
          ['quorum', 'Required decisions', [['Any', 'Any — first valid decision'], ['All', 'All — every assignee']]],
          ['negativeBehavior', 'Negative decision behavior', [['Terminate', 'Hard — reject the request'], ['Continue', 'Soft — record and continue']]],
          ['assigneeScope', 'Eligible assignee scope', [['Campus', 'Record campus + central tenant admins'], ['Tenant', 'Any assigned user in this tenant']]],
        ] as const).map(([key, label, choices]) => <TextField select key={key} label={label} value={stepEditor.step[key]}
          onChange={e => setStepEditor({ ...stepEditor, step: {
            ...stepEditor.step, [key]: e.target.value,
            // Only an approval may end the chain early, so switching to recommendations clears it.
            ...(key === 'decisionKind' && e.target.value !== 'Approval' ? { endOnPositiveDecision: false } : {}),
          } })}>{choices.map(([value, text]) => <MenuItem key={value} value={value}>{text}</MenuItem>)}</TextField>)}
        {/* FormGroup, not direct Stack children: Stack zeroes child margins, which drops FormControlLabel's -11px checkbox offset. */}
        <FormGroup sx={{ gap: 2 }}>
          {([['allowSelfAction', 'Allow requester to act on their own submission'], ['requirePositiveComment', 'Require comment for positive decisions'], ['requireNegativeComment', 'Require comment for negative decisions']] as const).map(([key, label]) => <FormControlLabel key={key} label={label} control={<Checkbox checked={stepEditor.step[key]} onChange={e => setStepEditor({ ...stepEditor, step: { ...stepEditor.step, [key]: e.target.checked } })} />} />)}
          <Box>
            <FormControlLabel label="End workflow on positive decision" disabled={stepEditor.step.decisionKind !== 'Approval'}
              control={<Checkbox checked={stepEditor.step.endOnPositiveDecision} onChange={e => setStepEditor({ ...stepEditor, step: { ...stepEditor.step, endOnPositiveDecision: e.target.checked } })} />} />
            <Typography variant="caption" color="text.secondary" component="p" sx={{ pl: 4 }}>
              {stepEditor.step.decisionKind === 'Approval'
                ? 'Approve completes the workflow and skips the later steps. Reject follows Negative decision behavior: Hard rejects the request, Soft passes it to the next step.'
                : 'Available on Approve / Reject steps only.'}
            </Typography>
          </Box>
        </FormGroup>
        <Alert severity="info">Return for changes always requires an explanation. The final step must use Approve / Reject and hard rejection.</Alert>
      </Stack>}</DialogContent>
      <DialogActions><Button onClick={() => setStepEditor(null)}>Cancel</Button><Button variant="contained" disabled={!stepEditor?.step.name.trim()} onClick={() => { if (!selected || !stepEditor) return; const steps = [...selected.steps]; steps[stepEditor.index] = stepEditor.step; setSelected({ ...selected, steps }); setSimulation(null); setStepEditor(null) }}>Apply to draft</Button></DialogActions>
    </Dialog>
  </Box>
}
