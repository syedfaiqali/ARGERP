import { useEffect, useRef, useState } from 'react'
import { Alert, Avatar, Box, Button, CircularProgress, Drawer, IconButton, Link, Paper, Stack, TextField, Tooltip, Typography } from '@mui/material'
import { Close, ContentCopyOutlined, DeleteOutline, Send, SmartToyOutlined } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { useAppSelector } from '@/hooks'
import api from '@/services/api'
import { searchDestinations } from './GlobalSearch'
import { randomUUId } from '@/utils/uuid'

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
  useEffect(() => { if (open) bottom.current?.scrollIntoView({ behavior: 'smooth' }) }, [messages, loading, open])
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
  return <Drawer anchor="right" open={open} onClose={onClose} PaperProps={{ sx: { width: { xs: '100%', sm: 440 }, maxWidth: '100vw', bgcolor: 'var(--lms-bg)' } }}>
    <Stack direction="row" alignItems="center" spacing={1} sx={{ p: 2, borderBottom: '1px solid var(--lms-border)', bgcolor: '#fff' }}><Avatar sx={{ bgcolor: '#f6eafa', color: '#9c27b0' }}><SmartToyOutlined /></Avatar><Box sx={{ flex: 1 }}><Typography fontWeight={800}>AI Assistant</Typography><Typography variant="caption" color="text.secondary">Al Rahim Group</Typography></Box><Tooltip title="Clear conversation"><IconButton aria-label="Clear conversation" disabled={loading} onClick={() => { save([welcome()]); setError(''); setInput('') }}><DeleteOutline /></IconButton></Tooltip><IconButton aria-label="Close AI Assistant" onClick={onClose}><Close /></IconButton></Stack>
    <Box role="log" aria-label="AI conversation" aria-live="polite" sx={{ flex: 1, overflow: 'auto', p: 2 }}>
      {messages.map(item => <Paper key={item.id} elevation={0} sx={{ p: 1.8, mb: 1.5, ml: item.role === 'user' ? 4 : 0, mr: item.role === 'assistant' ? 3 : 0, borderRadius: 3, bgcolor: item.role === 'user' ? 'rgba(var(--lms-primary-rgb),.1)' : '#fff', border: '1px solid var(--lms-border)' }}><Typography variant="caption" color="text.secondary">{item.role === 'user' ? 'You' : 'AI Assistant'}</Typography><Typography sx={{ whiteSpace: 'pre-wrap', overflowWrap: 'anywhere', fontSize: '.88rem' }}>{renderReply(item.text)}</Typography>{item.role === 'assistant' && <Tooltip title="Copy reply"><IconButton size="small" aria-label="Copy reply" onClick={() => navigator.clipboard?.writeText(item.text).catch(() => {})}><ContentCopyOutlined sx={{ fontSize: 15 }} /></IconButton></Tooltip>}</Paper>)}
      {messages.length === 1 && <Stack spacing={1}>{['How do I request leave?', 'Where is employee attendance?', 'How do I change my password?'].map(text => <Button key={text} size="small" variant="outlined" onClick={() => setInput(text)}>{text}</Button>)}</Stack>}
      {loading && <Stack role="status" direction="row" spacing={1}><CircularProgress size={18} /><Typography variant="body2">Thinking…</Typography></Stack>}
      {error && <Alert severity="error">{error}</Alert>}<div ref={bottom} />
    </Box>
    <Box component="form" onSubmit={event => { event.preventDefault(); void send() }} sx={{ p: 2, bgcolor: '#fff', borderTop: '1px solid var(--lms-border)' }}><Stack direction="row" spacing={1}><TextField fullWidth multiline maxRows={4} size="small" label="Ask AI Assistant" inputProps={{ maxLength: 2000 }} value={input} onChange={event => setInput(event.target.value)} onKeyDown={event => { if (event.key === 'Enter' && !event.shiftKey && !event.nativeEvent.isComposing) { event.preventDefault(); void send() } }} /><IconButton color="primary" type="submit" aria-label="Send message" disabled={loading || !input.trim()}><Send /></IconButton></Stack></Box>
  </Drawer>
}
