import type { BirthdayPerson } from '@/store/authSlice'
import api from '@/services/api'

const requests = new Map<string, Promise<{ students: BirthdayPerson[]; employees: BirthdayPerson[] }>>()
const today = () => new Date().toLocaleDateString('en-CA')
export async function fetchBirthdayNotifications(campusId: number, { useCache = true } = {}) {
  const key = 'argerp-birthdays:' + campusId + ':' + today()
  if (!campusId) return { students: [], employees: [] }
  if (useCache) {
    try {
      const cached = JSON.parse(sessionStorage.getItem(key) || 'null')
      if (cached && Date.now() - cached.at < 30 * 60 * 1000) return { students: [], employees: cached.employees as BirthdayPerson[] }
    } catch { /* Fetch again when cached data is unavailable. */ }
  }
  const active = requests.get(key)
  if (active) return active
  const pending = api.get('/api/HumanResource/EmployeeProfile/v3/EmployeeBirthdays', {
    params: { entityId: campusId }, headers: { 'X-Entity-Id': String(campusId), 'X-Campus-Id': String(campusId) },
  }).then(({ data }) => {
    const employees: BirthdayPerson[] = (Array.isArray(data) ? data : []).map(item => ({
      id: item.id, name: item.firstname || item.employeecode || 'Employee', subtitle: item.designationstxt || '', dateOfBirth: item.dob ?? item.DOB ?? item.dateOfBirth, imageId: item.imageId,
    }))
    try { sessionStorage.setItem(key, JSON.stringify({ at: Date.now(), employees })) } catch { /* Results still display without a cache. */ }
    return { students: [], employees }
  }).finally(() => requests.delete(key))
  requests.set(key, pending)
  return pending
}
