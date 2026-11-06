import { useEffect, useState } from 'react'
import { Avatar, Box, Chip, InputAdornment, List, ListItemButton, ListItemText, Popover, TextField, Typography } from '@mui/material'
import { ApartmentOutlined, Check, Search } from '@mui/icons-material'
import { PortalEmptyState, PortalPanelHeader, portalColors, portalInput, portalRow, portalScroll, portalSurface } from './portalUi'

export type WorkspaceOption = { id: number; name: string; address?: string }
export function WorkspaceList({ items, selected, onSelect, currentLabel = 'Current', query = '' }: { items: WorkspaceOption[]; selected: number | null; onSelect: (id: number) => void; currentLabel?: string; query?: string }) {
  const filtered = items.filter(item => (item.name || '').toLowerCase().includes(query.trim().toLowerCase()))
  return filtered.length ? <List disablePadding aria-label="Available campuses" sx={{ p: 1 }}>
    {filtered.map(item => <ListItemButton key={item.id} selected={item.id === selected} onClick={() => onSelect(item.id)} sx={portalRow}>
      <Avatar variant="rounded" sx={{ width: 38, height: 38, borderRadius: '11px', bgcolor: item.id === selected ? '#fff' : '#f5f7f1', color: item.id === selected ? '#75924d' : '#98a38d', border: '1px solid #e8eddf' }}><ApartmentOutlined sx={{ fontSize: 19 }} /></Avatar>
      <ListItemText sx={{ m: 0, minWidth: 0 }} primary={item.name} secondary={item.address || 'Campus / Branch'} primaryTypographyProps={{ sx: { fontSize: 12.5, fontWeight: item.id === selected ? 750 : 600, lineHeight: 1.5, color: portalColors.ink, overflowWrap: 'anywhere' } }} secondaryTypographyProps={{ sx: { fontSize: 10, color: '#929b88', mt: .25, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' } }} />
      {item.id === selected && <Box sx={{ display: 'flex', alignItems: 'center', gap: .5, color: '#77934f', flexShrink: 0 }}><Typography sx={{ fontSize: 9, fontWeight: 750, display: { xs: 'none', sm: 'block' } }}>{currentLabel}</Typography><Check sx={{ fontSize: 17 }} /></Box>}
    </ListItemButton>)}
  </List> : <PortalEmptyState icon={<Search />} title="No workspaces found" description={query ? 'Try a different campus or branch name.' : 'No active campuses or branches are available.'} />
}

export function CampusSwitcher({ anchorEl, onClose, branches, selected, onSelect }: { anchorEl: HTMLElement | null; onClose: () => void; branches: WorkspaceOption[]; selected: number; onSelect: (id: number) => void }) {
  const [query, setQuery] = useState('')
  useEffect(() => { if (anchorEl) setQuery('') }, [anchorEl])
  const current = branches.find(item => item.id === selected)
  const count = branches.filter(item => (item.name || '').toLowerCase().includes(query.trim().toLowerCase())).length
  return <Popover open={!!anchorEl} anchorEl={anchorEl} onClose={onClose} marginThreshold={16} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }} transformOrigin={{ vertical: 'top', horizontal: 'right' }} PaperProps={{ role: 'dialog', 'aria-label': 'Switch workspace', sx: { ...portalSurface, mt: 1.5, width: 410, maxWidth: 'calc(100vw - 32px)', maxHeight: 'calc(100dvh - 100px)', display: 'flex', flexDirection: 'column' } }}>
    <PortalPanelHeader eyebrow="AL RAHIM GROUP" title="Your workspaces" icon={<ApartmentOutlined />} onClose={onClose} />
    <Box sx={{ px: 2.5, pt: 2, pb: 1.5 }}>
      <Typography sx={{ fontSize: 9, fontWeight: 800, letterSpacing: '1.2px', color: '#919d85', mb: .6 }}>CURRENT WORKSPACE</Typography>
      <Typography sx={{ fontSize: 12, fontWeight: 700, color: portalColors.ink, mb: 1.75, overflowWrap: 'anywhere' }}>{current?.name || 'Select a campus/branch'}</Typography>
      <TextField autoFocus fullWidth size="small" placeholder="Find a campus or branch…" value={query} onChange={event => setQuery(event.target.value)} inputProps={{ 'aria-label': 'Search campus/branch' }} InputProps={{ startAdornment: <InputAdornment position="start"><Search sx={{ fontSize: 18, color: '#8f9b82' }} /></InputAdornment> }} sx={portalInput} />
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 1.75 }}><Typography sx={{ fontSize: 9, fontWeight: 800, letterSpacing: '1.1px', color: '#939c8c' }}>{query ? 'SEARCH RESULTS' : 'SWITCH WORKSPACE'}</Typography><Chip label={count} size="small" sx={{ height: 19, fontSize: 10, bgcolor: '#f2f5ed', color: '#81956b', fontWeight: 750 }} /></Box>
    </Box>
    <Box sx={{ overflowY: 'auto', minHeight: 0, maxHeight: 310, px: 1.25, pb: 1.25, ...portalScroll }}><WorkspaceList items={branches} selected={selected} query={query} onSelect={onSelect} /></Box>
    <Box sx={{ px: 2.5, py: 1.3, bgcolor: '#fafbf8', borderTop: `1px solid ${portalColors.line}` }}><Typography sx={{ fontSize: 10, color: '#959e8d' }}>Your data follows the workspace you select.</Typography></Box>
  </Popover>
}
