import { GetService } from '@/constants/globalItems';
import { createCrudSlice } from './createCrudSlice';

/**
 * Cached slices for the Human Resource setup screens.
 *
 * Each of these screens held its rows in `useState` and refetched on every mount, so
 * moving between them re-issued the same GETs and every save reloaded the whole list.
 * The rows live in the store instead: fetched once per scope, then patched in place by
 * the CRUD handlers on each screen.
 *
 * Only the *setup* screens are cached here. The transactional screens (attendance,
 * payroll runs, leave requests/transactions, loan requests, payslips) are deliberately
 * left fetching on mount, because their rows change server-side between visits and a
 * stale grid there would be acted on as if it were current.
 *
 * `sNo` is never stored: it is a display index derived at render time, and caching it
 * would leave it wrong as soon as a row is inserted or removed.
 */

const num = (v: any, fallback = 0) => Number(v ?? fallback) || fallback;
const str = (v: any, fallback = '') => v ?? fallback;
const byText = (key: string) => (a: any, b: any) =>
  String(a?.[key] ?? '').localeCompare(String(b?.[key] ?? ''));

/**
 * These screens map API fields at render time rather than at fetch time, so the raw
 * record is cached as-is. Reshaping it here would change what their grids display.
 */
const identity = (item: any) => item;

/** Most HR v3 list endpoints key the record on `id`. */
const byId = (r: any) => r?.id;

/* ------------------------------------------------- Human Resource Setup */

export const qualificationSlice = createCrudSlice<any>({
  name: 'hrQualification',
  url: '/api/HumanResource/Qualification/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    // The Qualification form picks a type from this list.
    qualificationTypes:
      '/api/HumanResource/QualificationType/v3?pageNumber=1&pageSize=1000&lovtypeId=368',
  },
  normalize: identity,
});

export const qualificationTypeSlice = createCrudSlice<any>({
  name: 'hrQualificationType',
  url: '/api/HumanResource/QualificationType/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const skillTypeSlice = createCrudSlice<any>({
  name: 'hrSkillType',
  url: '/api/HumanResource/SkillType/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const skillSlice = createCrudSlice<any>({
  name: 'hrSkill',
  url: '/api/HumanResource/Skill/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    skillTypes: '/api/HumanResource/SkillType/v3?pageSize=1000',
    ratingModels: '/api/HumanResource/Skill/v3/RatingModelLookup',
  },
  normalize: identity,
});

export const positionTypeSlice = createCrudSlice<any>({
  name: 'hrPositionType',
  url: '/api/HumanResource/PositionType/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    employeeTypes: '/api/HumanResource/PositionType/v3/EmployeeTypeLookup',
  },
  normalize: identity,
});

export const ratingModelSlice = createCrudSlice<any>({
  name: 'hrRatingModel',
  url: '/api/HumanResource/RatingModel/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const documentTypeSlice = createCrudSlice<any>({
  name: 'hrDocumentType',
  // Reached through humanResourceAdministrationService on the screen; the same
  // endpoint is stated here so the cache owns the request.
  url: '/api/HumanResource/HRDocumentType/v3?pageSize=1000',
  getId: byId,
  normalize: identity,
});

/* -------------------------------------------------------------- Employee */

export const gradeSlice = createCrudSlice<any>({
  name: 'hrGrade',
  url: '/api/HumanResource/Grade/v3',
  getId: byId,
  normalize: identity,
});

export const authoritySlice = createCrudSlice<any>({
  name: 'hrAuthority',
  url: '/api/HumanResource/Authority/v3?pageSize=1000',
  getId: byId,
  // The screen formats contactNo/phone for display; it re-applies that to the
  // cached rows, so the raw record is stored here.
  normalize: identity,
});

/* ------------------------------------------------- Time Management Setup */

export const holidayTypeSlice = createCrudSlice<any>({
  name: 'hrHolidayType',
  url: '/api/HumanResource/HolidayType/v3?pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const attendancePolicySlice = createCrudSlice<any>({
  name: 'hrAttendancePolicy',
  url: '/api/HumanResource/AttendancePolicy/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: (item) => ({
    ...item,
    id: num(item?.id),
    code: str(item?.code ?? item?.Code),
    policyName: str(item?.stxt ?? item?.Stxt),
    policyTypeId:
      item?.policyTypeId ?? item?.PolicyTypeId
        ? num(item?.policyTypeId ?? item?.PolicyTypeId)
        : null,
    policyTypeName: str(item?.policyTypeName ?? item?.PolicyTypeName),
    applyOffDays: !!(item?.applyOffDays ?? item?.ApplyOffDays),
  }),
});

export const shiftSlice = createCrudSlice<any>({
  name: 'hrShift',
  url: '/api/HumanResource/Shift/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: (item) => ({
    id: num(item?.id),
    code: str(item?.code),
    stxt: str(item?.stxt),
    shifttypeid: item?.shiftTypeId ?? null,
    shifttypename: str(item?.shiftTypeName),
    lovsharedid: item?.lovSharedId ?? null,
    lovsharedname: str(item?.lovSharedName),
    isActive: item?.isActive ?? true,
  }),
});

export const calendarHolidaySlice = createCrudSlice<any>({
  name: 'hrCalendarHoliday',
  url: '/api/HumanResource/CalendarHoliday/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const scheduleSlice = createCrudSlice<any>({
  name: 'hrSchedule',
  url: '/api/HumanResource/Schedule/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const customShiftSlice = createCrudSlice<any>({
  name: 'hrCustomShift',
  url: '/api/HumanResource/CustomShift/v3?pageNumber=1&pageSize=1000',
  // The grid builds a synthetic per-row key, keeping the record id in `realId`.
  getId: (r) => r?.realId ?? r?.id,
  normalize: identity,
});

export const deviceTypeSlice = createCrudSlice<any>({
  name: 'hrDeviceType',
  url: '/api/HumanResource/DeviceType/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const deviceRegistrationSlice = createCrudSlice<any>({
  name: 'hrDeviceRegistration',
  url: '/api/HumanResource/DeviceRegistration/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    all: '/api/HumanResource/DeviceRegistration/v3/lookups',
  },
  normalize: identity,
});

export const attendanceActivitySlice = createCrudSlice<any>({
  name: 'hrAttendanceActivity',
  url: '/api/HumanResource/AttendanceActivity/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    activityTypes: '/api/HumanResource/EmployeeAttendance/v3/activities',
  },
  normalize: identity,
});

/* ---------------------------------------------------- Employee Leave Setup */

export const leaveTypeSlice = createCrudSlice<any>({
  name: 'hrLeaveType',
  url: '/api/HumanResource/LeaveType/v3?pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const leaveGroupSlice = createCrudSlice<any>({
  name: 'hrLeaveGroup',
  url: '/api/HumanResource/LeaveGroup/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  lookups: {
    adjustmentPolicies: '/api/HumanResource/LeaveAdjustmentPolicy/v3?pageSize=1000',
    leaveTypes: '/api/HumanResource/LeaveType/v3?pageSize=1000',
  },
  normalize: identity,
});

export const leaveAdjustmentPolicySlice = createCrudSlice<any>({
  name: 'hrLeaveAdjustmentPolicy',
  url: '/api/HumanResource/LeaveAdjustmentPolicy/v3?pageSize=1000',
  getId: byId,
  lookups: {
    leaveTypes: '/api/HumanResource/LeaveType/v3?pageSize=1000',
    attendancePolicies: '/api/HumanResource/AttendancePolicy/v3?pageNumber=1&pageSize=1000',
  },
  normalize: identity,
});

/* ------------------------------------------------------------ Payroll Setup */

export const earningTypeSlice = createCrudSlice<any>({
  name: 'hrEarningType',
  url: '/api/HumanResource/EarningType/v3',
  getId: byId,
  sort: byText('earningType'),
  lookups: {
    all: async () => {
      const response = await GetService('/api/HumanResource/EarningType/v3/lookups');
      // This endpoint returns an object containing several lists. The shared CRUD
      // cache extracts arrays, and the form reads this bundle from all[0].
      return { data: [response?.data ?? {}] };
    },
  },
  normalize: (item) => ({
    ...item,
    earningType: str(item?.earningType),
    description: str(item?.description),
    baseSalary: !!item?.baseSalary,
    isActive: item?.isActive !== false,
  }),
});

export const earningGroupSlice = createCrudSlice<any>({
  name: 'hrEarningGroup',
  url: '/api/HumanResource/EarningGroup/v3',
  getId: byId,
  lookups: {
    all: '/api/HumanResource/EarningGroup/v3/lookups',
  },
  normalize: (item) => ({
    ...item,
    id: item?.id,
    code: str(item?.code),
    stxt: str(item?.earningGroup),
  }),
});

export const deductionTypeSlice = createCrudSlice<any>({
  name: 'hrDeductionType',
  url: '/api/HumanResource/DeductionType/v3',
  getId: byId,
  sort: byText('deductionType'),
  lookups: {
    all: async () => {
      const response = await GetService('/api/HumanResource/DeductionType/v3/lookups');
      // Preserve the lookup bundle for the array-based cache and the form's all[0].
      return { data: [response?.data ?? {}] };
    },
  },
  normalize: (item) => ({
    ...item,
    deductionType: str(item?.deductionType),
    description: str(item?.description),
    paymentRunTypeId: item?.paymentRunTypeId ?? null,
    paymentRunType: str(item?.paymentRunType),
    reimbursed: !!item?.reimbursed,
    isActive: item?.isActive !== false,
  }),
});

export const deductionGroupSlice = createCrudSlice<any>({
  name: 'hrDeductionGroup',
  url: '/api/HumanResource/DeductionGroup/v3',
  getId: byId,
  lookups: {
    all: '/api/HumanResource/DeductionGroup/v3/lookups',
  },
  normalize: (item) => ({
    ...item,
    id: item?.id,
    code: str(item?.code),
    deductionGroup: str(item?.deductionGroup),
  }),
});

export const payrollPeriodSlice = createCrudSlice<any>({
  name: 'hrPayrollPeriod',
  url: '/api/HumanResource/PayrollPeriod/v3?pageNumber=1&pageSize=1000',
  getId: byId,
  normalize: identity,
});

export const payrollRegisterSlice = createCrudSlice<any>({
  name: 'hrPayrollRegister',
  url: '/api/HumanResource/PayrollRegister/v3',
  getId: byId,
  normalize: identity,
});

/* --------------------------------------------------------------- Loan Setup */



/**
 * Fetches one HR record by id, for the screens whose edit/view mode loads the full
 * record (header plus detail rows) rather than reusing the grid row.
 */
export const fetchHrRecordById = async (baseUrl: string, id: number | string) => {
  const response: any = await GetService(`${baseUrl}/${id}`);
  return response?.data?.data ?? response?.data?.Data ?? response?.data ?? null;
};
