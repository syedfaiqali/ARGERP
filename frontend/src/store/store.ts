import menuPermissionsReducer from './menuPermissionsSlice'
import { configureStore } from '@reduxjs/toolkit'
import authReducer from './authSlice'
import settingsReducer from './settingsSlice'
import layoutReducer from './layoutSlice'
import favoritesReducer from './favoritesSlice'
import hrLookupsReducer from './hrLookupsSlice'
import { countrySlice, regionSlice, stateSlice, citySlice, orgSiteSlice, departmentSlice, designationSlice, occupationSlice, institutionSlice } from './organizationSlices'
import {
  attendanceActivitySlice,
  attendancePolicySlice,
  authoritySlice,
  calendarHolidaySlice,
  customShiftSlice,
  deductionGroupSlice,
  deductionTypeSlice,
  deviceRegistrationSlice,
  deviceTypeSlice,
  documentTypeSlice,
  earningGroupSlice,
  earningTypeSlice,
  gradeSlice,
  holidayTypeSlice,
  leaveAdjustmentPolicySlice,
  leaveGroupSlice,
  leaveTypeSlice,
  payrollPeriodSlice,
  payrollRegisterSlice,
  positionTypeSlice,
  qualificationSlice,
  qualificationTypeSlice,
  ratingModelSlice,
  scheduleSlice,
  shiftSlice,
  skillSlice,
  skillTypeSlice,
} from './humanResourceSlices'
export const store = configureStore({reducer: { auth: authReducer, menuPermissions: menuPermissionsReducer, settings: settingsReducer, layout: layoutReducer, favorites: favoritesReducer,
    orgCountry: countrySlice.reducer,
    orgRegion: regionSlice.reducer,
    orgState: stateSlice.reducer,
    orgCity: citySlice.reducer,
    orgSite: orgSiteSlice.reducer,
    orgDepartment: departmentSlice.reducer,
    orgDesignation: designationSlice.reducer,
    orgOccupation: occupationSlice.reducer,
    orgInstitution: institutionSlice.reducer,

    // Human Resource module caches (see store/humanResourceSlices.ts).
    hrLookups: hrLookupsReducer,
    hrQualification: qualificationSlice.reducer,
    hrQualificationType: qualificationTypeSlice.reducer,
    hrSkillType: skillTypeSlice.reducer,
    hrSkill: skillSlice.reducer,
    hrPositionType: positionTypeSlice.reducer,
    hrRatingModel: ratingModelSlice.reducer,
    hrDocumentType: documentTypeSlice.reducer,
    hrGrade: gradeSlice.reducer,
    hrAuthority: authoritySlice.reducer,
    hrHolidayType: holidayTypeSlice.reducer,
    hrDeviceType: deviceTypeSlice.reducer,
    hrDeviceRegistration: deviceRegistrationSlice.reducer,
    hrAttendanceActivity: attendanceActivitySlice.reducer,
    hrAttendancePolicy: attendancePolicySlice.reducer,
    hrCalendarHoliday: calendarHolidaySlice.reducer,
    hrShift: shiftSlice.reducer,
    hrSchedule: scheduleSlice.reducer,
    hrCustomShift: customShiftSlice.reducer,
    hrLeaveType: leaveTypeSlice.reducer,
    hrLeaveGroup: leaveGroupSlice.reducer,
    hrLeaveAdjustmentPolicy: leaveAdjustmentPolicySlice.reducer,
    hrEarningType: earningTypeSlice.reducer,
    hrEarningGroup: earningGroupSlice.reducer,
    hrDeductionType: deductionTypeSlice.reducer,
    hrDeductionGroup: deductionGroupSlice.reducer,
    hrPayrollPeriod: payrollPeriodSlice.reducer,
    hrPayrollRegister: payrollRegisterSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
