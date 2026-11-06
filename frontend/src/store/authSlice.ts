import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { CAMPUS_SESSION_KEY, clearAuthSession, isAuthSessionValid, startAuthSession } from '@/utils/authSession'

export interface User {
  userid?: number
  username?: string
  email?: string
  firstname?: string
  lastname?: string
  clientId?: number
  entityId?: number
  fullName?: string
  empId?: number
  studentId?: number
  selectedStudentId?: number
  userTypeId?: number
  userCatId?: number
  roles?: string[]
  displayRole?: string
  roleName?: string
  role?: string
  userType?: string
}

export interface CampusOption {
  id: number
  institutionId?: number
  name: string
  address: string
  entityType?: string
  entityTypeId?: number
  institutionType?: string
  institutionTypeId?: number
  imageId?: number | null
  imageUrl?: string
}

export interface BirthdayPerson {
  id: number
  name: string
  subtitle?: string
  dateOfBirth?: string
  imageId?: number | null
}

export interface AuthState {
  isAuthenticated: boolean
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  loading: boolean
  permissions: string[]
  selectedCampusId: number | null
  campuses: CampusOption[]
  studentBirthdays: BirthdayPerson[]
  employeeBirthdays: BirthdayPerson[]
  error: string | null
}

const savedUser = typeof window !== 'undefined' ? localStorage.getItem('user') : null
const savedCampuses = typeof window !== 'undefined' ? localStorage.getItem('campuses') : null
const savedStudentBirthdays = typeof window !== 'undefined' ? sessionStorage.getItem('studentBirthdays') : null
const savedEmployeeBirthdays = typeof window !== 'undefined' ? sessionStorage.getItem('employeeBirthdays') : null
const savedPermissions = typeof window !== 'undefined' ? localStorage.getItem('permissions') : null
const hasValidSession = typeof window !== 'undefined' ? isAuthSessionValid() : false
if (typeof window !== 'undefined' && localStorage.getItem('accessToken') && !hasValidSession) {
  clearAuthSession()
}
const initialState: AuthState = {
  isAuthenticated: hasValidSession,
  user: hasValidSession && savedUser ? JSON.parse(savedUser) : null,
  accessToken: hasValidSession && typeof window !== 'undefined' ? localStorage.getItem('accessToken') : null,
  refreshToken: hasValidSession && typeof window !== 'undefined' ? localStorage.getItem('refreshToken') : null,
  loading: false,
  selectedCampusId: hasValidSession && typeof window !== 'undefined' ? Number(sessionStorage.getItem(CAMPUS_SESSION_KEY)) || null : null,
  campuses: hasValidSession && savedCampuses ? JSON.parse(savedCampuses) : [],
  studentBirthdays: hasValidSession && savedStudentBirthdays ? JSON.parse(savedStudentBirthdays) : [],
  employeeBirthdays: hasValidSession && savedEmployeeBirthdays ? JSON.parse(savedEmployeeBirthdays) : [],
  error: null,
  permissions: hasValidSession && savedPermissions ? JSON.parse(savedPermissions) : []
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true
      state.error = null
    },
    loginSuccess: (state, action: PayloadAction<{ user: User; accessToken: string; refreshToken: string }>) => {
      state.loading = false
      state.isAuthenticated = true
      state.user = action.payload.user
      state.accessToken = action.payload.accessToken
      state.refreshToken = action.payload.refreshToken
      startAuthSession()
      localStorage.setItem('accessToken', action.payload.accessToken)
      localStorage.setItem('refreshToken', action.payload.refreshToken)
      localStorage.setItem('user', JSON.stringify(action.payload.user))

      state.selectedCampusId = null
      state.campuses = []
      state.studentBirthdays = []
      state.employeeBirthdays = []
      sessionStorage.removeItem(CAMPUS_SESSION_KEY)
      for (const key of ['selectedCampusId', 'selectedEntityId', 'selectedEntity', 'selectedEntityID', 'entityId', 'EntityId', 'campuses']) localStorage.removeItem(key)
    },
    loginFailure: (state, action: PayloadAction<string>) => {
      state.loading = false
      state.error = action.payload
      state.isAuthenticated = false
    },
    logout: (state) => {
      state.isAuthenticated = false
      state.user = null
      state.accessToken = null
      state.refreshToken = null
      state.selectedCampusId = null
      state.campuses = []
      state.studentBirthdays = []
      state.employeeBirthdays = []
      clearAuthSession()
    },
    setUser: (state, action: PayloadAction<User>) => {
      state.user = action.payload
    },
    setCampus: (state, action: PayloadAction<number>) => {
      state.selectedCampusId = action.payload
      const selectedEntityId = action.payload.toString()
      sessionStorage.setItem(CAMPUS_SESSION_KEY, selectedEntityId)
      localStorage.setItem('selectedCampusId', selectedEntityId)
      localStorage.setItem('selectedEntityId', selectedEntityId)
      localStorage.setItem('selectedEntity', selectedEntityId)
      localStorage.setItem('selectedEntityID', selectedEntityId)
      localStorage.setItem('entityId', selectedEntityId)
      localStorage.setItem('EntityId', selectedEntityId)
    },
    setCampusesRedux: (state, action: PayloadAction<CampusOption[]>) => {
      state.campuses = action.payload
      localStorage.setItem('campuses', JSON.stringify(action.payload))
    },
    setBirthdays: (state, action: PayloadAction<{ students: BirthdayPerson[]; employees: BirthdayPerson[] }>) => {
      state.studentBirthdays = action.payload.students
      state.employeeBirthdays = action.payload.employees
      sessionStorage.setItem('studentBirthdays', JSON.stringify(action.payload.students))
      sessionStorage.setItem('employeeBirthdays', JSON.stringify(action.payload.employees))
    },
    setPermissions:(state, action: PayloadAction<string[]>) => {
      state.permissions = action.payload;
      localStorage.setItem('permissions', JSON.stringify(action.payload));
    }
  },
})

export const { loginStart, loginSuccess, loginFailure, logout, setUser, setCampus,
   setCampusesRedux, setBirthdays, setPermissions } = authSlice.actions
export default authSlice.reducer
