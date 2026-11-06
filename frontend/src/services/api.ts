import axios from 'axios'
import { clearAuthSession, isAuthSessionValid } from '@/utils/authSession'
export const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || '', timeout: 60000 })
api.interceptors.request.use(config => {
  if (config.url?.includes('/api/auth/login')) return config
  const token = localStorage.getItem('accessToken')
  if (token && !isAuthSessionValid()) { clearAuthSession(); window.location.assign('/login'); throw new axios.Cancel('Session expired') }
  if (token) config.headers.Authorization = 'Bearer ' + token
  config.headers['X-Client-Id'] = localStorage.getItem('clientId') || '1'
  const entity = localStorage.getItem('selectedEntityId') || '1'
  config.headers['X-Entity-Id'] = entity
  config.headers['X-Campus-Id'] = entity
  return config
})
api.interceptors.response.use(response => response, error => {
  if (error.response?.status === 401 && !error.config?.url?.includes('/api/auth/login')) { clearAuthSession(); window.location.assign('/login') }
  return Promise.reject(error)
})
export default api
