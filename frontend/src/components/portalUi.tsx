import type { ReactNode } from 'react'
import { Box, IconButton, Stack, Typography } from '@mui/material'
import { Close } from '@mui/icons-material'

export const portalColors = { ink: '#263525', muted: '#6f7b65', line: '#e8ece5', wash: '#f7f9f5', green: 'var(--lms-primary)', forest: '#30432b' }
export const portalSurface = {
  borderRadius: '22px', border: `1px solid ${portalColors.line}`, bgcolor: '#fff', backgroundImage: 'none',
  boxShadow: '0 24px 80px -16px rgba(31,49,24,.24), 0 4px 16px rgba(31,49,24,.04)', overflow: 'hidden',
}
export const portalScroll = {
  scrollbarWidth: 'thin', scrollbarColor: '#d2dccb transparent', overscrollBehavior: 'contain',
  '&::-webkit-scrollbar': { width: 5 }, '&::-webkit-scrollbar-thumb': { bgcolor: '#d2dccb', borderRadius: 5 },
} as const
export const portalInput = {
  '& .MuiOutlinedInput-root': { borderRadius: '12px', bgcolor: portalColors.wash, fontSize: 13, minHeight: 44, '& fieldset': { borderColor: portalColors.line }, '&:hover fieldset': { borderColor: '#c4d0bb' }, '&.Mui-focused fieldset': { borderColor: 'var(--lms-primary)', borderWidth: 1 } },
  '& .MuiInputBase-input::placeholder': { opacity: 1, color: '#929b8c' },
}
export const portalIconButton = {
  width: { xs: 34, sm: 38 }, height: { xs: 34, sm: 38 }, borderRadius: '11px', color: '#658348', border: '1px solid transparent',
  transition: 'background-color .18s, border-color .18s, box-shadow .18s',
  '& svg': { fontSize: 21 }, '&:hover': { bgcolor: '#f2f6ee', borderColor: '#e0e8d9', boxShadow: '0 3px 10px #30432b08' },
  '&.Mui-focusVisible': { outline: '2px solid var(--lms-primary)', outlineOffset: 2 },
}
export const portalRow = {
  borderRadius: '12px', px: 1.5, py: 1.25, gap: 1.3, mb: .5, border: '1px solid transparent', alignItems: 'center',
  '&:hover': { bgcolor: '#f6f8f3', borderColor: '#e8ede1' },
  '&.Mui-selected': { bgcolor: '#f0f5e9', borderColor: '#dce7ce', '&:hover': { bgcolor: '#eaf1e1' } },
  '&.Mui-focusVisible': { outline: '2px solid var(--lms-primary)', outlineOffset: -2 },
}

export function PortalPanelHeader({ eyebrow, title, subtitle, icon, onClose, extra }: { eyebrow?: string; title: string; subtitle?: string; icon: ReactNode; onClose?: () => void; extra?: ReactNode }) {
  return <Box sx={{ px: 2.5, pt: 2.5, pb: 2, borderBottom: `1px solid ${portalColors.line}`, bgcolor: '#fff', flexShrink: 0 }}>
    <Stack direction="row" alignItems="center" spacing={1.5}>
      <Box sx={{ width: 44, height: 44, borderRadius: '14px', bgcolor: '#f0f5e9', border: '1px solid #e2ead9', color: '#6f8d48', display: 'grid', placeItems: 'center', flexShrink: 0 }}>{icon}</Box>
      <Box sx={{ flex: 1, minWidth: 0 }}>{eyebrow && <Typography sx={{ fontSize: 10, fontWeight: 750, letterSpacing: '1.2px', color: '#7a876d', mb: .35 }}>{eyebrow}</Typography>}<Typography sx={{ fontSize: 18, lineHeight: 1.3, letterSpacing: '-.45px', fontWeight: 800, color: portalColors.ink }}>{title}</Typography></Box>
      {extra}{onClose && <IconButton size="small" aria-label={`Close ${title}`} onClick={onClose} sx={{ color: '#8d9885', borderRadius: '9px', '&:hover': { bgcolor: portalColors.wash } }}><Close sx={{ fontSize: 18 }} /></IconButton>}
    </Stack>
    {subtitle && <Typography sx={{ fontSize: 12, color: portalColors.muted, mt: 1.4, lineHeight: 1.6 }}>{subtitle}</Typography>}
  </Box>
}

export function PortalEmptyState({ icon, title, description }: { icon: ReactNode; title: string; description: string }) {
  return <Box sx={{ px: 3, py: 4.5, textAlign: 'center' }}>
    <Box sx={{ position: 'relative', mx: 'auto', mb: 2.2, width: 82, height: 82, display: 'grid', placeItems: 'center', border: '1px solid #ecf0e6', borderRadius: '50%', bgcolor: '#fafbf7', '&::before': { content: '""', position: 'absolute', width: 57, height: 57, borderRadius: '19px', bgcolor: '#eff4e7', transform: 'rotate(-12deg)' }, '& > svg': { position: 'relative', color: '#8ba55d', fontSize: 30 } }}>{icon}</Box>
    <Typography sx={{ fontSize: 14, fontWeight: 750, color: portalColors.ink, mb: .8 }}>{title}</Typography>
    <Typography sx={{ fontSize: 12, lineHeight: 1.8, color: portalColors.muted, maxWidth: 260, mx: 'auto' }}>{description}</Typography>
  </Box>
}
