import { useEffect, useRef, useState } from 'react'
import { Alert, Avatar, Box, Button, CircularProgress, Drawer, IconButton, Link, Paper, Stack, TextField, Tooltip, Typography } from '@mui/material'
import { ArrowForward, CalendarMonthOutlined, Close, ContentCopyOutlined, DeleteOutline, LockOutlined, Send, SmartToyOutlined } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { useAppSelector } from '@/hooks'
import api from '@/services/api'
import { searchDestinations } from './GlobalSearch'
import { randomUUId } from '@/utils/uuid'
import { portalColors, portalScroll } from './portalUi'

type Message = { id: string; role: 'user' | 'assistant'; text: string }
const welcome = (): Message => ({ id: 'welcome', role: 'assistant', text: "Hello! I'm your AI Assistant. Ask me about HR, Organization or Auth, or tell me which screen you want to open." })
export function AiAssistant({ open, onClose }: { open: boolean; onClose: () => void }) {
  const campus = useAppSelector(state => state.auth.selectedCampusId), userId = useAppSelector(state => state.auth.user?.userid)
  const storageKey = `argerp-ai-chat:${userId}:${campus}`
  const [messages, setMessages] = useState<Message[]>([welcome()]), [input, setInput] = useState('')
  const [loading, setLoading] = useState(false), [error, setError] = useState('')
  const request = useRef<AbortController | null>(null), bottom = useRef<HTMLDivElement>(null)
  const navigate = useNavigate()
  useEffect(() => {
    try { const saved = JSON.parse(sessionStorage.getItem(storageKey) || 'null'); setMessages(Array.isArray(saved) && saved.length ? saved.filter(item => typeof item.text === 'string' && ['user', 'assistant'].includes(item.role)).slice(-30) : [welcome()]) }
    catch { setMessages([welcome()]) }
    return () => request.current?.abort()
  }, [storageKey])
  useEffect(() => { if (open && messages.some(item => item.id !== 'welcome')) bottom.current?.scrollIntoView({ behavior: 'smooth' }) }, [messages, loading, open])
  const save = (next: Message[]) => { setMessages(next); try { sessionStorage.setItem(storageKey, JSON.stringify(next.slice(-30))) } catch { /* Chat remains usable without storage. */ } }
  const send = async () => {
    if (!input.trim() || loading) return
    const text = input.trim(), next: Message[] = [...messages, { id: randomUUId(), role: 'user', text }]
    save(next); setInput(''); setError(''); setLoading(true)
    const abort = new AbortController(); request.current = abort
    try {
      const { data } = await api.post('/api/HumanResource/Assistant/v3/chat', { message: text, history: messages.filter(item => item.id !== 'welcome').slice(-8).map(({ role, text }) => ({ role, text })) }, { signal: abort.signal })
      if (!abort.signal.aborted) save([...next, { id: randomUUId(), role: 'assistant', text: data.reply }])
    } catch (e: any) { if (!abort.signal.aborted) { setError(e.response?.data?.message || 'Unable to reach AI Assistant. Please try again.'); setInput(text); save(messages) } }
    finally { if (!abort.signal.aborted) setLoading(false) }
  }
  const renderReply = (text: string) => text.split(/(\[[^\]]+\]\([^)]+\))/g).map((part, index) => {
    const link = /^\[([^\]]+)\]\(([^)]+)\)$/.exec(part)
    const destination = link && searchDestinations.find(item => item.path === link[2])
    return destination ? <Link key={index} component="button" onClick={() => { onClose(); navigate(destination.path) }} sx={{ verticalAlign: 'baseline', textAlign: 'left' }}>{link![1]}</Link> : <span key={index}>{link ? link[1] : part}</span>
  })
  const emptyConversation = messages.every(item => item.id === 'welcome')
  const suggestions = [
    { title: 'Leave & time off', text: 'How do I request leave?', icon: <CalendarMonthOutlined /> },
    { title: 'Employee attendance', text: 'Where is employee attendance?', icon: <SmartToyOutlined /> },
    { title: 'Account & access', text: 'How do I change my password?', icon: <LockOutlined /> },
  ]
  return <Drawer anchor="right" open={open} onClose={onClose} ModalProps={{ BackdropProps: { sx: { bgcolor: 'rgba(29,43,22,.25)', backdropFilter: 'blur(4px)' } } }} PaperProps={{ sx: { width: { xs: '100%', sm: 450 }, maxWidth: { xs: '100vw', sm: 'calc(100vw - 24px)' }, top: { xs: 0, sm: 12 }, bottom: { xs: 0, sm: 12 }, right: { xs: 0, sm: 12 }, height: 'auto', borderRadius: { xs: 0, sm: '24px' }, border: '1px solid #e6ecdf', boxShadow: '0 24px 80px #1f31183d', bgcolor: '#f8faf6', overflow: 'hidden' } }}>
    <Stack direction="row" alignItems="center" spacing={1.5} sx={{ p: 2.5, borderBottom: '1px solid #e8ede2', bgcolor: '#fff', flexShrink: 0 }}><Avatar variant="rounded" sx={{ width: 44, height: 44, borderRadius: '14px', bgcolor: '#30432b', color: '#dbe9cb', boxShadow: '0 5px 14px #30432b15' }}><SmartToyOutlined sx={{ fontSize: 24 }} /></Avatar><Box sx={{ flex: 1 }}><Typography sx={{ fontSize: 8, letterSpacing: '1.4px', fontWeight: 800, color: '#99a38c', mb: .3 }}>AL RAHIM GROUP</Typography><Typography sx={{ fontSize: 17, fontWeight: 800, color: portalColors.ink, letterSpacing: '-.4px' }}>AI Assistant</Typography></Box><Tooltip title="Clear conversation"><IconButton aria-label="Clear conversation" disabled={loading} onClick={() => { save([welcome()]); setError(''); setInput('') }} sx={{ color: '#9da790', borderRadius: '10px' }}><DeleteOutline sx={{ fontSize: 19 }} /></IconButton></Tooltip><IconButton aria-label="Close AI Assistant" onClick={onClose} sx={{ color: '#9da790', borderRadius: '10px' }}><Close sx={{ fontSize: 19 }} /></IconButton></Stack>
    <Box role="log" aria-label="AI conversation" aria-live="polite" sx={{ flex: 1, minHeight: 0, overflowY: 'auto', p: 2.5, ...portalScroll }}>
      {emptyConversation ? <Box sx={{ pt: { xs: 2, sm: 4 }, pb: 3 }}>
        <Box sx={{ width: 70, height: 70, mx: 'auto', mb: 2.5, display: 'grid', placeItems: 'center', borderRadius: '23px', bgcolor: '#edf3e3', color: '#829e54', border: '1px solid #dfe8d2', boxShadow: '0 8px 24px #627f3410' }}><SmartToyOutlined sx={{ fontSize: 34 }} /></Box>
        <Typography sx={{ fontSize: 24, fontWeight: 800, color: portalColors.ink, letterSpacing: '-.8px', textAlign: 'center', mb: 1 }}>How can I help?</Typography>
        <Typography sx={{ fontSize: 12, lineHeight: 1.9, color: '#8f9a83', textAlign: 'center', maxWidth: 280, mx: 'auto', mb: 3.5 }}>Find a screen, understand a process or get help with your everyday work.</Typography>
        <Typography sx={{ fontSize: 9, fontWeight: 800, letterSpacing: '1.2px', color: '#a0aa94', mb: 1.3 }}>A FEW PLACES TO START</Typography>
        <Stack spacing={1}>{suggestions.map(item => <Button key={item.title} onClick={() => setInput(item.text)} sx={{ justifyContent: 'flex-start', p: 1.6, gap: 1.3, borderRadius: '13px', bgcolor: '#fff', border: '1px solid #e6ecde', color: '#849a68', '&:hover': { borderColor: '#cedbbd', bgcolor: '#f3f7ed', '& .suggestion-arrow': { transform: 'translateX(3px)' } } }}><Box sx={{ width: 31, height: 31, display: 'grid', placeItems: 'center', borderRadius: '9px', bgcolor: '#f5f8ef', '& svg': { fontSize: 18 } }}>{item.icon}</Box><Typography sx={{ flex: 1, textAlign: 'left', fontSize: 12, fontWeight: 650, color: '#586a48' }}>{item.title}</Typography><ArrowForward className="suggestion-arrow" sx={{ fontSize: 15, color: '#afbc9d', transition: 'transform .2s' }} /></Button>)}</Stack>
      </Box> : messages.filter(item => item.id !== 'welcome').map(item => <Box key={item.id} sx={{ mb: 2.5, display: 'flex', gap: 1, flexDirection: item.role === 'user' ? 'row-reverse' : 'row', alignItems: 'flex-start' }}>
        {item.role === 'assistant' && <Avatar sx={{ width: 27, height: 27, borderRadius: '9px', bgcolor: '#e9f0dd', color: '#8aa361', mt: 2.5 }}><SmartToyOutlined sx={{ fontSize: 17 }} /></Avatar>}
        <Box sx={{ maxWidth: item.role === 'user' ? '87%' : 'calc(100% - 36px)', minWidth: 0 }}><Typography sx={{ fontSize: 9, fontWeight: 700, color: '#a0aa93', mb: .6, textAlign: item.role === 'user' ? 'right' : 'left' }}>{item.role === 'user' ? 'YOU' : 'AI ASSISTANT'}</Typography><Paper elevation={0} sx={{ px: 1.8, py: 1.5, borderRadius: item.role === 'user' ? '15px 15px 4px 15px' : '4px 15px 15px 15px', bgcolor: item.role === 'user' ? '#edf3e3' : '#fff', border: '1px solid ' + (item.role === 'user' ? '#dfe9d1' : '#e8ede2') }}><Typography sx={{ whiteSpace: 'pre-wrap', overflowWrap: 'anywhere', fontSize: 12.5, lineHeight: 1.85, color: '#536048' }}>{renderReply(item.text)}</Typography></Paper>{item.role === 'assistant' && <Tooltip title="Copy reply"><IconButton size="small" aria-label="Copy reply" sx={{ color: '#a4af98', mt: .5 }} onClick={() => navigator.clipboard?.writeText(item.text).catch(() => {})}><ContentCopyOutlined sx={{ fontSize: 13 }} /></IconButton></Tooltip>}</Box>
      </Box>)}
      {loading && <Stack role="status" direction="row" spacing={1} alignItems="center" sx={{ px: 1, py: 1.5 }}><CircularProgress size={14} sx={{ color: '#95aa72' }} /><Typography sx={{ fontSize: 11, color: '#99a68a' }}>Working on your answer?</Typography></Stack>}
      {error && <Alert severity="error" sx={{ fontSize: 12, borderRadius: '12px' }}>{error}</Alert>}<div ref={bottom} />
    </Box>
    <Box component="form" onSubmit={event => { event.preventDefault(); void send() }} sx={{ px: 2.5, pt: 1.8, pb: 'max(18px, env(safe-area-inset-bottom))', bgcolor: '#fff', borderTop: '1px solid #e8ede2', flexShrink: 0 }}>
      <Box sx={{ display: 'flex', alignItems: 'flex-end', gap: 1, p: 1, pl: 1.75, border: '1px solid #dfe7d5', borderRadius: '15px', bgcolor: '#fafcf7', transition: 'box-shadow .2s, border-color .2s', '&:focus-within': { borderColor: '#a9bd89', boxShadow: '0 0 0 3px #eff4e7' } }}><TextField fullWidth multiline maxRows={5} variant="standard" placeholder="Ask me anything about your workspace?" inputProps={{ maxLength: 2000, 'aria-label': 'Ask AI Assistant' }} InputProps={{ disableUnderline: true, sx: { fontSize: 12, lineHeight: 1.7, py: .7, color: '#536048' } }} value={input} onChange={event => setInput(event.target.value)} onKeyDown={event => { if (event.key === 'Enter' && !event.shiftKey && !event.nativeEvent.isComposing) { event.preventDefault(); void send() } }} /><IconButton type="submit" aria-label="Send message" disabled={loading || !input.trim()} sx={{ width: 34, height: 34, borderRadius: '10px', bgcolor: '#526d3c', color: '#fff', '&:hover': { bgcolor: '#3d572b' }, '&.Mui-disabled': { bgcolor: '#e9efdf', color: '#b6c3a4' } }}><Send sx={{ fontSize: 16 }} /></IconButton></Box>
      <Typography sx={{ textAlign: 'center', fontSize: 9, color: '#afb7a4', mt: 1.2, display: { xs: 'none', sm: 'block' } }}>Enter to send ? Shift + Enter for a new line</Typography>
    </Box>
  </Drawer>
}
