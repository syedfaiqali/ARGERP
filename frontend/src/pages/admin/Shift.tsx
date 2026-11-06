import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Grid,
  Box,
  Typography,
  Button,
  IconButton,
  Collapse,
  Paper,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Delete as DeleteIcon,
  Close as CloseIcon,
} from "@mui/icons-material";
import AssessmentIcon from "@mui/icons-material/Assessment";
import dayjs from "dayjs";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";
import { LookupDropdown } from "@/components/LookupDropdown";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormTimePicker } from "@/components/FormTimePicker";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { ShiftReportModal } from "./ShiftReportModal";
import {
  GetService,
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { attendancePolicySlice, shiftSlice } from "@/store/humanResourceSlices";

interface ShiftProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const getAttendancePolicyKind = (policy: any) => {
  const code = String(policy?.policyTypeCode ?? policy?.code ?? policy?.Code ?? "").trim().toUpperCase();
  const name = String(
    policy?.policyTypeName ?? policy?.stxt ?? policy?.Stxt ?? policy?.name ?? policy?.Name ?? "",
  ).trim().toUpperCase();
  const text = `${code} ${name}`;

  if (text.includes("LATE")) return "late";
  if (text.includes("EARLY")) return "early";
  if (text.includes("OVERTIME") || code === "OT") return "overtime";
  if (text.includes("ABSENT")) return "absent";
  return "";
};

const defaultDaysFallback = [
  { id: 1, stxt: "Monday" },
  { id: 2, stxt: "Tuesday" },
  { id: 3, stxt: "Wednesday" },
  { id: 4, stxt: "Thursday" },
  { id: 5, stxt: "Friday" },
  { id: 6, stxt: "Saturday" },
  { id: 7, stxt: "Sunday" },
];

const weekdayOrder = [
  "monday",
  "tuesday",
  "wednesday",
  "thursday",
  "friday",
  "saturday",
  "sunday",
];

const orderWeekdays = (items: any[]) =>
  [...items].sort((left, right) => {
    const getOrder = (item: any) => {
      const text = String(item?.stxt ?? item?.name ?? item?.code ?? "").trim().toLowerCase();
      const index = weekdayOrder.findIndex((day) => text === day || text.startsWith(day.slice(0, 3)));
      return index === -1 ? Number.MAX_SAFE_INTEGER : index;
    };

    return getOrder(left) - getOrder(right);
  });

const toLookupItems = (response: any): any[] => {
  const data = response?.data ?? response;
  if (Array.isArray(data)) return data;
  const items = data?.items ?? data?.Items ?? data?.data ?? data?.Data;
  return Array.isArray(items) ? items : [];
};

const toLovOptions = (response: any): any[] =>
  toLookupItems(response).map((item: any) => ({
    ...item,
    id: Number(item.id ?? item.Id ?? 0),
    code: item.code ?? item.Code ?? "",
    name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? "",
    ltxt: item.ltxt ?? item.Ltxt ?? item.longName ?? item.LongName ?? "",
  }));

const DEFAULT_SCHEDULE_END_DATE = "9998-12-31";

const initialFormData = {
  id: null as number | null,
  code: "",
  stxt: "",
  shifttypeid: null as number | null,
  shifttypename: "",
  lovsharedid: null as number | null,
  lovsharedname: "",
  womonday: false,
  wotuesday: false,
  wowednesday: false,
  wothuresday: false,
  wofriday: false,
  wosaturday: false,
  wosunday: false,
  latearivpolicyid: null as number | null,
  earlydeppolicyid: null as number | null,
  otpolicyid: null as number | null,
  abspolicyid: null as number | null,
  latearivpolicymnt: "" as string | number,
  earlydeppolicymnt: "" as string | number,
  otpolicymnt: "" as string | number,
  isActive: true,
  shiftdetails: [] as any[],
  sharedDetails: [] as any[],
};

const Shift: React.FC<ShiftProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrShift",
    shiftSlice.fetchAll,
  );
  // Attendance policies are a cached HR list, shared with their own screen.
  const { rows: policyRows } = useCachedRows<any>(
    "hrAttendancePolicy",
    attendancePolicySlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "error",
  );

  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">(
    "list",
  );
  const [formData, setFormData] = useState<any>({ ...initialFormData });
  const [isSubmitted, setIsSubmitted] = useState(false);

  // SFTT/SCHFR/WDAY are global LOV reference data shared with CustomShift and
  // Schedule; policies come from the cached AttendancePolicy list.
  const { lov } = useHrLovs(["SFTT", "SCHFR", "WDAY"]);
  const shiftTypes = lov("SFTT");
  const sharedWithOptions = lov("SCHFR");
  const days = useMemo<any[]>(() => {
    const dayData = lov("WDAY");
    return Array.isArray(dayData) && dayData.length > 0
      ? orderWeekdays(dayData)
      : defaultDaysFallback;
  }, [lov]);
  const policies = useMemo<any[]>(
    () =>
      (policyRows || []).map((p: any) => ({
        id: p.id,
        code: p.code,
        stxt: p.stxt ?? p.policyName,
        policyTypeId: p.policyTypeId,
        policyTypeName: p.policyTypeName,
      })),
    [policyRows],
  );
  const [entities, setEntities] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);

  const [activeDayTab, setActiveDayTab] = useState(0);
  const [selectedDetailId, setSelectedDetailId] = useState<number | null>(null);
  const [breakModalOpen, setBreakModalOpen] = useState(false);
  const [isSharedDetailExpanded, setIsSharedDetailExpanded] = useState(true);
  const [shiftTypeModalOpen, setShiftTypeModalOpen] = useState(false);
  const [sharedWithModalOpen, setSharedWithModalOpen] = useState(false);
  const [activePolicyLookup, setActivePolicyLookup] = useState<
    "late" | "early" | "over" | "absent" | null
  >(null);

  const policyLate = policies.filter((p) => getAttendancePolicyKind(p) === "late");
  const policyEarly = policies.filter((p) => getAttendancePolicyKind(p) === "early");
  const policyOver = policies.filter((p) => getAttendancePolicyKind(p) === "overtime");
  const policyAbsent = policies.filter((p) => getAttendancePolicyKind(p) === "absent");

  const loadDepartments = useCallback(async () => {
    try {
      const response = await GetService("/api/HumanResource/Lookups/v3/departments");
      let items = toLookupItems(response);

      if (items.length === 0) {
        const organizationResponse = await GetService(
          "/api/organization/Department/v3?pageNumber=1&pageSize=1000",
        );
        items = toLookupItems(organizationResponse);
      }

      setDepartments(
        items.map((item: any) => ({
          id: Number(item.id ?? item.departmentId ?? item.depid ?? 0),
          code: item.code ?? item.departmentcode ?? item.departmentCode ?? "",
          name: item.departmentName ?? item.department ?? item.depstxt ?? item.name ?? "",
          description: item.description ?? item.ltxt ?? item.depltxt ?? "",
        })),
      );
    } catch (error) {
      console.error("Failed to load HR departments", error);
      setDepartments([]);
    }
  }, []);

  const resolveSharedMode = useCallback(
    (lovId: number | null | undefined) => {
      if (!lovId) return null;
      const opt = sharedWithOptions.find((o: any) => o.id === lovId);
      // SCHFR uses numeric legacy codes; the displayed LOV name determines
      // which shared-detail source legacy opens.
      const text = [opt?.name, opt?.stxt, opt?.ltxt, opt?.code]
        .filter((value) => value !== null && value !== undefined)
        .join(" ")
        .toLowerCase();
      if (text.includes("entity") || text.includes("entities")) return "entity";
      if (text.includes("department") || text.includes("departments")) return "department";
      if (text.includes("employee") || text.includes("employees")) return "employee";
      return null;
    },
    [sharedWithOptions],
  );

  const selectShiftType = (row: any) => {
    setFormData((current: any) => ({
      ...current,
      shifttypeid: row.id,
      shifttypename: row.stxt || row.name || row.code || "",
    }));
  };

  const selectSharedWith = (row: any) => {
    void loadDepartments();
    setFormData((current: any) => ({
      ...current,
      lovsharedid: row.id,
      lovsharedname: row.stxt || row.name || row.code || "",
      sharedDetails: [],
    }));
  };

  const policyLookupRows = useMemo(() => {
    const source =
      activePolicyLookup === "late"
        ? policyLate
        : activePolicyLookup === "early"
          ? policyEarly
          : activePolicyLookup === "over"
            ? policyOver
            : activePolicyLookup === "absent"
              ? policyAbsent
              : [];
    return (source || []).map((p: any) => ({
      id: p.id,
      code: p.code ?? "",
      stxt: p.stxt ?? p.name ?? p.code ?? "",
      policyTypeName: p.policyTypeName ?? "",
    }));
  }, [activePolicyLookup, policyLate, policyEarly, policyOver, policyAbsent]);

  const policyLookupTitle = useMemo(() => {
    switch (activePolicyLookup) {
      case "late":
        return translate("Late Arrival Policy");
      case "early":
        return translate("Early Departure Policy");
      case "over":
        return translate("Overtime Policy");
      case "absent":
        return translate("Absent Policy");
      default:
        return "";
    }
  }, [activePolicyLookup, translate]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Shift"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Shift"));
    else if (viewMode === "view") onSubViewChange(translate("View Shift"));
    else if (viewMode === "report") onSubViewChange(translate("Shift Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);
  // sNo is a display index, derived here rather than cached.
  const rows = useMemo<any[]>(
    () => (cachedRows || []).map((item: any, index: number) => ({ ...item, sNo: index + 1 })),
    [cachedRows],
  );

  // The shift-type, shared-with, weekday and attendance-policy lists are served
  // from their caches. Only the entity allocation and employee roster are
  // fetched here: neither endpoint is modelled by a slice.
  const fetchLookups = useCallback(async () => {
    try {
      const results = await Promise.allSettled([
        // Use the user's Legalentity allocation, matching the Switch Campus list,
        // instead of the client-wide HR entity lookup.
        GetService("/api/auth/Legalentity/v3/Legalentity"),
        GetService("/api/HumanResource/EmployeeAttendance/v3/employees"),
      ]);
      const [entityResult, employeeResult] = results;
      const getResultData = (result: PromiseSettledResult<any>) =>
        result.status === "fulfilled" ? result.value : null;

      const entityResp = getResultData(entityResult);
      const employeeResp = getResultData(employeeResult);

      const entityItems = toLookupItems(entityResp);
      setEntities(
        (entityItems || []).map((item: any) => ({
          id: Number(item.id ?? item.entityId ?? 0),
          code: item.code ?? item.Code ?? item.entityNo ?? item.EntityNo ?? "",
          name: item.instituteName ?? item.InstituteName ?? item.entityName ?? item.EntityName ?? item.campusName ?? item.stxt ?? item.name ?? "",
          description: item.description ?? item.Description ?? item.ltxt ?? item.Ltxt ?? "",
        })).filter((item: any) => item.id > 0 && item.name),
      );

      await loadDepartments();

      const empItems = toLookupItems(employeeResp);
      setEmployees(
        empItems.map((item: any) => ({
          id: Number(item.id ?? item.employeeId ?? 0),
          employeeCode: item.employeeCode ?? item.code ?? "",
          employeeName: item.employeeName ?? item.name ?? item.stxt ?? "",
          department: item.department ?? item.departmentName ?? "",
          designation: item.designation ?? item.designationName ?? "",
          machineCode: item.machineCode ?? "",
        })),
      );
    } catch (error) {
      console.error("Failed to fetch lookups", error);
    }
  }, [loadDepartments]);

  useEffect(() => {
    fetchLookups();
  }, [fetchLookups]);

  const loadShiftById = async (id: number) => {
    setApiLoading(true);
    try {
      const response: any = await GetService(`/api/HumanResource/Shift/v3/${id}`);
      const data = response?.data;
      if (!data) return;

      setFormData({
        id: data.id,
        code: data.code || "",
        stxt: data.stxt || "",
        shifttypeid: data.shiftTypeId ?? null,
        shifttypename: data.shiftTypeName ?? "",
        lovsharedid: data.lovSharedId ?? null,
        lovsharedname: data.lovSharedName ?? "",
        womonday: data.woMonday ?? false,
        wotuesday: data.woTuesday ?? false,
        wowednesday: data.woWednesday ?? false,
        wothuresday: data.woThuresday ?? false,
        wofriday: data.woFriday ?? false,
        wosaturday: data.woSaturday ?? false,
        wosunday: data.woSunday ?? false,
        latearivpolicyid: data.lateArrivPolicyId ?? null,
        earlydeppolicyid: data.earlyDepPolicyId ?? null,
        otpolicyid: data.otPolicyId ?? null,
        abspolicyid: data.absPolicyId ?? null,
        latearivpolicymnt: data.lateArrivPolicyMnt ?? "",
        earlydeppolicymnt: data.earlyDepPolicyMnt ?? "",
        otpolicymnt: data.otPolicyMnt ?? "",
        isActive: data.isActive ?? true,
        shiftdetails: (data.shiftDetails || []).map((detail: any) => ({
          ...detail,
          inTime: normalizeTimeString(detail.inTime),
          outTime: normalizeTimeString(detail.outTime),
          breaks: (detail.breaks || []).map((br: any) => ({
            ...br,
            startTime: normalizeTimeString(br.startTime),
            endTime: normalizeTimeString(br.endTime),
          })),
        })),
        sharedDetails: (data.sharedDetails || []).map((s: any) => ({
          id: s.id ?? s.Id ?? null,
          lineIndex: s.lineIndex ?? s.LineIndex ?? 0,
          lovsharedid: s.lovSharedId ?? s.LovSharedId ?? data.lovSharedId ?? null,
          entityId: s.entityId ?? s.EntityId ?? null,
          departmentId: s.departmentId ?? s.DepartmentId ?? null,
          employeeId: s.employeeId ?? s.EmployeeId ?? null,
          entityName: s.entityName ?? "",
          departmentName: s.departmentName ?? "",
          employeeName: s.employeeName ?? "",
        })),
      });
      setSelectedDetailId(null);
    } catch (error) {
      console.error("Failed to load shift", error);
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      await DeleteService(`/api/HumanResource/Shift/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(shiftSlice.fetchAll(true));
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to delete shift";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  const normalizeTimeString = (value: any) => {
    if (!value) return "";
    if (typeof value === "string") {
      const hhmm = value.match(/^(\d{1,2}):(\d{2})/) ?? value.match(/T(\d{1,2}):(\d{2})/);
      if (hhmm) {
        return `${hhmm[1].padStart(2, "0")}:${hhmm[2]}`;
      }
    }
    const parsed = dayjs(value);
    return parsed.isValid() ? parsed.format("HH:mm") : "";
  };

  const toDateTime = (timeValue: any, dateValue?: any) => {
    const time = normalizeTimeString(timeValue);
    if (!time) return null;
    const baseDate = dayjs(dateValue).isValid()
      ? dayjs(dateValue).format("YYYY-MM-DD")
      : "1970-01-01";
    const dt = dayjs(`${baseDate}T${time}:00`);
    return dt.isValid() ? `${baseDate}T${time}:00` : null;
  };

  const toDateOnly = (dateVal: any) => {
    if (!dateVal) return null;
    const d = dayjs(dateVal);
    return d.isValid() ? d.format("YYYY-MM-DD") : null;
  };

  const parseTimeMinutes = (timeStr?: string) => {
    if (!timeStr) return null;
    const parts = timeStr.split(":");
    if (parts.length < 2) return null;
    const hrs = Number(parts[0]);
    const mins = Number(parts[1]);
    if (Number.isNaN(hrs) || Number.isNaN(mins)) return null;
    return hrs * 60 + mins;
  };

  const computeDuration = (start?: string, end?: string) => {
    const s = parseTimeMinutes(start);
    const e = parseTimeMinutes(end);
    if (s == null || e == null) return null;
    if (e < s) return null;
    const diff = e - s;
    return Number((diff / 60).toFixed(2));
  };

  const hasOptionalNumberValue = (value: unknown) =>
    value !== "" && value !== null && value !== undefined;

  const isOptionalNumberNonPositive = (value: unknown) =>
    hasOptionalNumberValue(value) && Number(value) <= 0;

  const toOptionalNumber = (value: unknown) =>
    hasOptionalNumberValue(value) ? Number(value) : null;

  const toPersistedId = (value: unknown) => {
    const id = Number(value);
    return Number.isFinite(id) && id > 0 ? id : null;
  };
  const preventNegativeNumberInput = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "-") {
      event.preventDefault();
    }
  };

  const preventNegativeNumberPaste = (event: React.ClipboardEvent<HTMLInputElement>) => {
    if (event.clipboardData.getData("text").includes("-")) {
      event.preventDefault();
    }
  };

  const validateForm = (data: any) => {
    const errors: string[] = [];
    if (!data.stxt) errors.push(translate("Shift Name is required"));
    if (!data.shifttypeid) errors.push(translate("Shift Type is required"));
    if (!data.lovsharedid) errors.push(translate("Shared With is required"));

    if (isOptionalNumberNonPositive(data.otpolicymnt)) {
      errors.push(translate("Overtime Policy Minutes must be greater than zero"));
    }

    if (!data.shiftdetails || data.shiftdetails.length === 0) {
      errors.push(translate("Schedule is required"));
    }

    if (!data.sharedDetails || data.sharedDetails.length === 0) {
      errors.push(translate("Shared With Detail is required"));
    }

    const sharedMode = resolveSharedMode(data.lovsharedid);
    (data.sharedDetails || []).forEach((s: any, idx: number) => {
      const labelPrefix = translate(`Shared With Detail ${idx + 1}:`);
      if (sharedMode === "entity" && !s.entityId) {
        errors.push(`${labelPrefix} ${translate("Entity is required")}`);
      }
      if (sharedMode === "department" && !s.departmentId) {
        errors.push(`${labelPrefix} ${translate("Department is required")}`);
      }
      if (sharedMode === "employee" && !s.employeeId) {
        errors.push(`${labelPrefix} ${translate("Employee is required")}`);
      }
    });

    (data.shiftdetails || []).forEach((d: any, idx: number) => {
      if (!d.dayId) errors.push(translate(`Schedule ${idx + 1}: Day is required`));
      if (!d.inTime) errors.push(translate(`Schedule ${idx + 1}: In Time is required`));
      if (!d.outTime) errors.push(translate(`Schedule ${idx + 1}: Out Time is required`));
      if (!d.startDate) errors.push(translate(`Schedule ${idx + 1}: Start Date is required`));
      if (!d.endDate) errors.push(translate(`Schedule ${idx + 1}: End Date is required`));
    });

    const duplicateSchedule = (data.shiftdetails || []).some((detail: any, index: number, all: any[]) =>
      all.some((other: any, otherIndex: number) =>
        otherIndex < index &&
        other.dayId === detail.dayId &&
        dayjs(other.startDate).isValid() &&
        dayjs(detail.startDate).isValid() &&
        dayjs(other.startDate).isSame(dayjs(detail.startDate), "day"),
      ),
    );
    if (duplicateSchedule) {
      errors.push(translate("A schedule with the same Effective Date already exists for this weekday."));
    }

    const schedulesOverlap = (data.shiftdetails || []).some((detail: any, index: number, all: any[]) =>
      all.some((other: any, otherIndex: number) => {
        if (otherIndex >= index || other.dayId !== detail.dayId) return false;
        const start = dayjs(detail.startDate);
        const end = dayjs(detail.endDate);
        const otherStart = dayjs(other.startDate);
        const otherEnd = dayjs(other.endDate);
        return start.isValid() && end.isValid() && otherStart.isValid() && otherEnd.isValid() &&
          !end.isBefore(otherStart, "day") && !otherEnd.isBefore(start, "day");
      }),
    );
    if (schedulesOverlap) {
      errors.push(translate("Schedule date range overlaps another schedule for the same weekday."));
    }

    (data.shiftdetails || []).forEach((detail: any, scheduleIndex: number) => {
      const breaks = detail.breaks || [];
      const hasBreakOverlap = breaks.some((current: any, index: number) =>
        breaks.some((previous: any, previousIndex: number) => {
          if (previousIndex >= index) return false;
          const currentStart = parseTimeMinutes(current.startTime);
          const currentEnd = parseTimeMinutes(current.endTime);
          const previousStart = parseTimeMinutes(previous.startTime);
          const previousEnd = parseTimeMinutes(previous.endTime);
          return currentStart != null && currentEnd != null && previousStart != null && previousEnd != null &&
            currentStart <= previousEnd && previousStart <= currentEnd;
        }),
      );
      if (hasBreakOverlap) {
        errors.push(translate(`Schedule ${scheduleIndex + 1}: Break time overlaps another break.`));
      }
    });

    return errors;
  };

  const handleSave = async () => {
    const errors = validateForm(formData);
    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = {
        id: formData.id ?? null,
        code: formData.code || null,
        stxt: formData.stxt?.trim(),
        shiftTypeId: formData.shifttypeid,
        lovSharedId: formData.lovsharedid,
        woMonday: !!formData.womonday,
        woTuesday: !!formData.wotuesday,
        woWednesday: !!formData.wowednesday,
        woThuresday: !!formData.wothuresday,
        woFriday: !!formData.wofriday,
        woSaturday: !!formData.wosaturday,
        woSunday: !!formData.wosunday,
        lateArrivPolicyId: toPersistedId(formData.latearivpolicyid),
        earlyDepPolicyId: toPersistedId(formData.earlydeppolicyid),
        otPolicyId: toPersistedId(formData.otpolicyid),
        absPolicyId: toPersistedId(formData.abspolicyid),
        lateArrivPolicyMnt: toOptionalNumber(formData.latearivpolicymnt),
        earlyDepPolicyMnt: toOptionalNumber(formData.earlydeppolicymnt),
        otPolicyMnt: toOptionalNumber(formData.otpolicymnt),
        isActive: formData.isActive ?? true,
        shiftDetails: (formData.shiftdetails || []).map((d: any, index: number) => ({
          id: toPersistedId(d.id),
          lineIndex: d.lineIndex ?? index + 1,
          dayId: d.dayId,
          inTime: toDateTime(d.inTime, d.startDate),
          outTime: toDateTime(d.outTime, d.startDate),
          startDate: toDateOnly(d.startDate),
          endDate: toDateOnly(d.endDate),
          graceTime: null,
          graceTimeEnd: null,
          latePolicyApplicable: Number(formData.latearivpolicyid) > 0,
          earlyPolicyApplicable: false,
          duration: d.duration ?? null,
          breaks: (d.breaks || []).map((b: any, bIndex: number) => ({
            id: toPersistedId(b.id),
            lineIndex: b.lineIndex ?? bIndex + 1,
            stxt: b.stxt || null,
            startTime: toDateTime(b.startTime, d.startDate),
            endTime: toDateTime(b.endTime, d.startDate),
            duration: b.duration ?? null,
          })),
        })),
        sharedDetails: (formData.sharedDetails || []).map((s: any, sIndex: number) => ({
          id: toPersistedId(s.id),
          lineIndex: s.lineIndex ?? sIndex + 1,
          lovSharedId: s.lovsharedid ?? formData.lovsharedid,
          entityId: s.entityId ?? null,
          departmentId: s.departmentId ?? null,
          employeeId: s.employeeId ?? null,
        })),
      };

      if (formData.id) {
        await PutService("/api/HumanResource/Shift/v3", payload);
      } else {
        await PostService("/api/HumanResource/Shift/v3", payload);
      }

      setToasterMessages([
        formData.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      dispatch(shiftSlice.fetchAll(true));
      return true;
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to save shift";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const columns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "stxt", label: translate("Shift Name"), minWidth: 200 },
    { id: "shifttypename", label: translate("Shift Type"), minWidth: 150 },
    { id: "lovsharedname", label: translate("Shared With"), minWidth: 150 },
  ];

  const dayTabs = useMemo(
    () =>
      (days || defaultDaysFallback).map((d: any) => ({
        label: d.stxt || d.name || d.code || "",
      })),
    [days],
  );

  const activeDayId = useMemo(() => {
    const item = (days || defaultDaysFallback)[activeDayTab];
    return item?.id ?? activeDayTab + 1;
  }, [days, activeDayTab]);

  const scheduleRows = useMemo(() => {
    return (formData.shiftdetails || []).filter((d: any) => d.dayId === activeDayId);
  }, [formData.shiftdetails, activeDayId]);

  const updateScheduleRow = (id: number, updater: Partial<any>) => {
    setFormData((prev: any) => {
      const details = prev.shiftdetails || [];
      const updatedDetail = details.find((detail: any) => detail.id === id);
      const effectiveDate = updater.startDate ? dayjs(updater.startDate).startOf("day") : null;

      return {
        ...prev,
        shiftdetails: details.map((detail: any) => {
          if (detail.id === id) return { ...detail, ...updater };
          if (!effectiveDate?.isValid() || detail.dayId !== updatedDetail?.dayId || !dayjs(detail.startDate).isValid()) {
            return detail;
          }

          const detailStart = dayjs(detail.startDate).startOf("day");
          return detailStart.isBefore(effectiveDate) &&
            (!detail.endDate || !dayjs(detail.endDate).isBefore(effectiveDate, "day"))
            ? { ...detail, endDate: toDateOnly(effectiveDate.subtract(1, "day")) }
            : detail;
        }),
      };
    });
  };

  const addScheduleRow = () => {
    const startDate = dayjs().startOf("day");
    const newRow = {
      id: -Math.floor(Math.random() * 1000000),
      dayId: activeDayId,
      lineIndex: scheduleRows.length + 1,
      inTime: "",
      outTime: "",
      startDate: toDateOnly(startDate),
      endDate: toDateOnly(dayjs(DEFAULT_SCHEDULE_END_DATE)),
      duration: null,
      breaks: [],
    };
    setFormData((prev: any) => ({
      ...prev,
      shiftdetails: (prev.shiftdetails || []).map((detail: any) => {
        if (detail.dayId !== activeDayId || !dayjs(detail.startDate).isValid()) return detail;
        const detailStart = dayjs(detail.startDate).startOf("day");
        return detailStart.isBefore(startDate) &&
          (!detail.endDate || !dayjs(detail.endDate).isBefore(startDate, "day"))
          ? { ...detail, endDate: toDateOnly(startDate.subtract(1, "day")) }
          : detail;
      }).concat(newRow),
    }));
  };

  const removeScheduleRow = (id: number) => {
    setFormData((prev: any) => ({
      ...prev,
      shiftdetails: (prev.shiftdetails || []).filter((d: any) => d.id !== id),
    }));
    if (selectedDetailId === id) {
      setSelectedDetailId(null);
      setBreakModalOpen(false);
    }
  };
  const scheduleColumns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 60,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    {
      id: "inTime",
      label: translate("In Time"),
      minWidth: 130,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={parseTimeValue(row.inTime)}
          defaultTime={dayjs().hour(8).minute(0).second(0)}
          onChange={(val) => {
            const inTime = val ? val.format("HH:mm") : "";
            const duration = computeDuration(inTime, row.outTime) ?? row.duration;
            updateScheduleRow(row.id, { inTime, duration });
          }}
          hideLabel
        />
      ),
    },
    {
      id: "outTime",
      label: translate("Out Time"),
      minWidth: 130,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={parseTimeValue(row.outTime)}
          onChange={(val) => {
            const outTime = val ? val.format("HH:mm") : "";
            const duration = computeDuration(row.inTime, outTime) ?? row.duration;
            updateScheduleRow(row.id, { outTime, duration });
          }}
          hideLabel
        />
      ),
    },
    {
      id: "duration",
      label: translate("Duration (hr)"),
      minWidth: 120,
      format: (_: any, row: any) => (
        <FormInput size="small" value={row.duration ?? ""} disabled />
      ),
    },
    {
      id: "startDate",
      label: translate("Start Date"),
      minWidth: 150,
      format: (_: any, row: any) => (
        <FormDatePicker
          value={row.startDate ? dayjs(row.startDate) : null}
          onChange={(date) =>
            updateScheduleRow(row.id, {
              startDate: date ? toDateOnly(date) : "",
            })
          }
          hideLabel
          format="DD-MMM-YYYY"
        />
      ),
    },
    {
      id: "endDate",
      label: translate("End Date"),
      minWidth: 150,
      format: (_: any, row: any) => (
        <FormDatePicker
          value={row.endDate ? dayjs(row.endDate) : null}
          disabled
          hideLabel
          format="DD-MMM-YYYY"
        />
      ),
    },
    {
      id: "breaks",
      label: translate("Breaks"),
      minWidth: 90,
      format: (_: any, row: any) => {
        const count = (row.breaks || []).length;
        return (
          <Button
            size="small"
            variant="text"
            onClick={() => {
              setSelectedDetailId(row.id);
              setBreakModalOpen(true);
            }}
            sx={{
              minWidth: 0,
              p: 0,
              textTransform: "none",
              fontWeight: 700,
              textDecoration: "none",
              "&:hover": {
                bgcolor: "transparent",
                textDecoration: "underline",
              },
            }}
          >
            {count > 0 ? `${count} ${translate("Break(s)")}` : translate("Add")}
          </Button>
        );
      },
    },
    // {
    //   id: "delete",
    //   label: "",
    //   minWidth: 60,
    //   format: (_: any, row: any) => (
    //     <IconButton onClick={() => removeScheduleRow(row.id)} size="small">
    //       <DeleteIcon fontSize="small" />
    //     </IconButton>
    //   ),
    // },
  ];

  const selectedSchedule = (formData.shiftdetails || []).find(
    (d: any) => d.id === selectedDetailId,
  );

  const updateBreakRow = (id: number, updater: Partial<any>) => {
    if (!selectedSchedule) return;
    setFormData((prev: any) => ({
      ...prev,
      shiftdetails: (prev.shiftdetails || []).map((d: any) => {
        if (d.id !== selectedSchedule.id) return d;
        return {
          ...d,
          breaks: (d.breaks || []).map((b: any) =>
            b.id === id ? { ...b, ...updater } : b,
          ),
        };
      }),
    }));
  };

  const addBreakRow = () => {
    if (!selectedSchedule) return;
    const newBreak = {
      id: -Math.floor(Math.random() * 1000000),
      lineIndex: (selectedSchedule.breaks || []).length + 1,
      stxt: "",
      startTime: "",
      endTime: "",
      duration: null,
    };
    setFormData((prev: any) => ({
      ...prev,
      shiftdetails: (prev.shiftdetails || []).map((d: any) =>
        d.id === selectedSchedule.id
          ? { ...d, breaks: [...(d.breaks || []), newBreak] }
          : d,
      ),
    }));
  };

  const removeBreakRow = (id: number) => {
    if (!selectedSchedule) return;
    setFormData((prev: any) => ({
      ...prev,
      shiftdetails: (prev.shiftdetails || []).map((d: any) =>
        d.id === selectedSchedule.id
          ? { ...d, breaks: (d.breaks || []).filter((b: any) => b.id !== id) }
          : d,
      ),
    }));
  };

  const breakColumns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 60,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    {
      id: "stxt",
      label: translate("Break Name"),
      minWidth: 200,
      format: (_: any, row: any) => (
        <FormInput
          size="small"
          value={row.stxt || ""}
          onChange={(e) => updateBreakRow(row.id, { stxt: e.target.value })}
          inputProps={{ maxLength: 49 }}
        />
      ),
    },
    {
      id: "startTime",
      label: translate("Start Time"),
      minWidth: 130,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={parseTimeValue(row.startTime)}
          onChange={(val) => {
            const startTime = val ? val.format("HH:mm") : "";
            const duration = computeDuration(startTime, row.endTime) ?? row.duration;
            updateBreakRow(row.id, { startTime, duration });
          }}
          hideLabel
        />
      ),
    },
    {
      id: "endTime",
      label: translate("End Time"),
      minWidth: 130,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={parseTimeValue(row.endTime)}
          onChange={(val) => {
            const endTime = val ? val.format("HH:mm") : "";
            const duration = computeDuration(row.startTime, endTime) ?? row.duration;
            updateBreakRow(row.id, { endTime, duration });
          }}
          hideLabel
        />
      ),
    },
    {
      id: "duration",
      label: translate("Duration (hr)"),
      minWidth: 120,
      format: (_: any, row: any) => (
        <FormInput size="small" value={row.duration ?? ""} disabled />
      ),
    },
    {
      id: "delete",
      label: "",
      minWidth: 60,
      format: (_: any, row: any) => (
        <IconButton onClick={() => removeBreakRow(row.id)} size="small">
          <DeleteIcon fontSize="small" />
        </IconButton>
      ),
    },
  ];

  const getSharedDetailKey = () => {
    const sharedMode = resolveSharedMode(formData.lovsharedid);
    if (sharedMode === "department") return "departmentId";
    if (sharedMode === "employee") return "employeeId";
    return "entityId";
  };

  const getSharedDetailRows = () => {
    const sharedMode = resolveSharedMode(formData.lovsharedid);
    if (sharedMode === "department") return departments;
    if (sharedMode === "employee") return employees;
    if (sharedMode === "entity") return entities;
    return [];
  };

  const isSharedDetailSelected = (id: any) => {
    const key = getSharedDetailKey();
    return (formData.sharedDetails || []).some(
      (detail: any) => String(detail[key]) === String(id),
    );
  };

  const toggleSharedDetail = (row: any, checked: boolean) => {
    const sharedMode = resolveSharedMode(formData.lovsharedid);
    const key = getSharedDetailKey();
    const rowId = row.id ?? row.employeeId ?? row.departmentId ?? row.entityId;
    const current = formData.sharedDetails || [];
    const next = checked
      ? [
          ...current.filter((detail: any) => String(detail[key]) !== String(rowId)),
          {
            id: null,
            lineIndex: current.length + 1,
            lovsharedid: formData.lovsharedid,
            entityId: sharedMode === "entity" ? rowId : null,
            entityName: sharedMode === "entity" ? row.name || row.code || "" : "",
            departmentId: sharedMode === "department" ? rowId : null,
            departmentName: sharedMode === "department" ? row.name || row.code || "" : "",
            employeeId: sharedMode === "employee" ? rowId : null,
            employeeName:
              sharedMode === "employee"
                ? row.employeeName || row.employeeCode || row.name || ""
                : "",
          },
        ]
      : current.filter((detail: any) => String(detail[key]) !== String(rowId));

    setFormData({ ...formData, sharedDetails: next });
  };

  const toggleAllSharedDetails = (rowsToToggle: any[], checked: boolean) => {
    const sharedMode = resolveSharedMode(formData.lovsharedid);
    const key = getSharedDetailKey();
    const selectedIds = new Set(
      rowsToToggle.map((row: any) => String(row.id ?? row.employeeId ?? row.departmentId ?? row.entityId)),
    );
    const current = formData.sharedDetails || [];
    const retained = current.filter((detail: any) => !selectedIds.has(String(detail[key])));
    const selected = checked
      ? rowsToToggle.map((row: any, index: number) => {
          const rowId = row.id ?? row.employeeId ?? row.departmentId ?? row.entityId;
          return {
            id: null,
            lineIndex: retained.length + index + 1,
            lovsharedid: formData.lovsharedid,
            entityId: sharedMode === "entity" ? rowId : null,
            entityName: sharedMode === "entity" ? row.name || row.code || "" : "",
            departmentId: sharedMode === "department" ? rowId : null,
            departmentName: sharedMode === "department" ? row.name || row.code || "" : "",
            employeeId: sharedMode === "employee" ? rowId : null,
            employeeName:
              sharedMode === "employee"
                ? row.employeeName || row.employeeCode || row.name || ""
                : "",
          };
        })
      : [];

    setFormData({ ...formData, sharedDetails: [...retained, ...selected] });
  };

  const renderSharedWithSelection = () => {
    const sharedMode = resolveSharedMode(formData.lovsharedid);
    if (!sharedMode) return null;

    const title =
      sharedMode === "department"
        ? translate("Department")
        : sharedMode === "employee"
          ? translate("Employee")
          : translate("Entity");
    const detailRows = getSharedDetailRows();
    const nameColumnId = sharedMode === "employee" ? "employeeName" : "name";
    const codeColumnId = sharedMode === "employee" ? "employeeCode" : "code";
    const allSharedDetailsSelected =
      detailRows.length > 0 &&
      detailRows.every((row: any) => {
        const rowId = row.id ?? row.employeeId ?? row.departmentId ?? row.entityId;
        return isSharedDetailSelected(rowId);
      });
    const sharedDetailColumns: Column[] = [
      {
        id: "select",
        label: translate("Select"),
        headerFormat: (
          <LmsCheckbox
            hideLabel
            checked={allSharedDetailsSelected}
            onChange={(checked) => toggleAllSharedDetails(detailRows, checked)}
            disabled={viewMode === "view" || detailRows.length === 0}
          />
        ),
        minWidth: 80,
        format: (_: any, row: any) => {
          const rowId = row.id ?? row.employeeId ?? row.departmentId ?? row.entityId;
          return (
            <LmsCheckbox
              hideLabel
              checked={isSharedDetailSelected(rowId)}
              onChange={(checked) => toggleSharedDetail(row, checked)}
              disabled={viewMode === "view"}
            />
          );
        },
      },
      { id: codeColumnId, label: translate("Code"), minWidth: 120 },
      {
        id: nameColumnId,
        label: title,
        minWidth: 260,
        format: (_: any, row: any) =>
          sharedMode === "employee"
            ? row.employeeName || row.name || row.employeeCode || ""
            : row.name || row.departmentName || row.instituteName || row.code || "",
      },
    ];

    return (
      <Box sx={{ mt: 3 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 1.5,
            cursor: "pointer",
            userSelect: "none",
          }}
          onClick={() => setIsSharedDetailExpanded(!isSharedDetailExpanded)}
        >
          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
            <T>Shared With Detail</T>
          </Typography>
          {renderExpandIcon(isSharedDetailExpanded)}
        </Box>
        <Collapse in={isSharedDetailExpanded}>
          <SchemaTable
            title={translate("Shared With Detail")}
            columns={sharedDetailColumns}
            rows={detailRows}
            pagination={false}
            filtersEnabled={true}
            downloadEnabled={false}
            showToaster={false}
            showAddButton={false}
            actions={false}
            hideTitle
            maxHeight={320}
            stickyHeader
          />
        </Collapse>
      </Box>
    );
  };

  const renderExpandIcon = (expanded: boolean) => (
    <Box
      sx={{
        width: 22,
        height: 22,
        borderRadius: "50%",
        bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color: "var(--lms-primary)",
        position: "relative",
        transition: "transform 0.3s ease",
      }}
    >
      <Box sx={{ width: 10, height: "2px", bgcolor: "currentColor" }} />
      <Box
        sx={{
          width: "2px",
          height: 10,
          bgcolor: "currentColor",
          position: "absolute",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(90deg)" : "rotate(0deg)",
          opacity: expanded ? 0 : 1,
        }}
      />
    </Box>
  );

  const parseTimeValue = (value?: string) => {
    const time = normalizeTimeString(value);
    if (!time) return null;
    const candidate = dayjs(`1970-01-01T${time}:00`);
    return candidate.isValid() ? candidate : null;
  };
  const renderPolicyLookupDialog = () => {
    if (!activePolicyLookup) return null;

    const lookupColumns: Column[] = [
      { id: "code", label: translate("Code"), minWidth: 100 },
      { id: "stxt", label: translate("Policy"), minWidth: 220 },
      { id: "policyTypeName", label: translate("Policy Type"), minWidth: 180 },
    ];

    return (
      <Dialog
        open={!!activePolicyLookup}
        onClose={() => setActivePolicyLookup(null)}
        maxWidth="lg"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: "12px",
            overflow: "hidden",
            boxShadow: "0 10px 40px rgba(0,0,0,0.1)",
          },
        }}
      >
        <Box
          sx={{
            bgcolor: "#f0fdf4",
            px: 3,
            py: 2,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <Typography
            sx={{ fontWeight: 800, fontSize: "1.1rem", color: "#166534" }}
          >
            {policyLookupTitle}
          </Typography>
          <IconButton
            size="small"
            onClick={() => setActivePolicyLookup(null)}
            sx={{
              bgcolor: "#fff",
              color: "#aaa",
              "&:hover": { bgcolor: "#f5f5f5" },
            }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>

        <Box sx={{ p: 3, bgcolor: "#fff" }}>
          <SchemaTable
            title={policyLookupTitle}
            columns={lookupColumns}
            rows={policyLookupRows}
            actions={false}
            showAddButton={false}
            hideTitle={true}
            hideBreadcrumbs={true}
            pagination={false}
            filtersEnabled={true}
            downloadEnabled={false}
            stickyHeader={true}
            rowHoverBg="rgba(var(--lms-primary-rgb), 0.08)"
            onRowClick={(row: any) => {
              setFormData((prev: any) => {
                if (activePolicyLookup === "late") {
                  return { ...prev, latearivpolicyid: row.id };
                }
                if (activePolicyLookup === "early") {
                  return { ...prev, earlydeppolicyid: row.id };
                }
                if (activePolicyLookup === "over") {
                  return { ...prev, otpolicyid: row.id };
                }
                if (activePolicyLookup === "absent") {
                  return { ...prev, abspolicyid: row.id };
                }
                return prev;
              });
              setActivePolicyLookup(null);
            }}
          />
        </Box>
      </Dialog>
    );
  };
  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box
            sx={{
              p: 2,
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <Typography
              variant="h5"
              sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
            >
              {viewMode === "new" ? translate("New Shift") : viewMode === "edit" ? translate("Edit Shift") : viewMode === "view" ? translate("View Shift") : translate("Shift Report")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
              >
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && viewMode !== "report" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={async () => {
                    setIsSubmitted(true);
                    const ok = await handleSave();
                    if (ok !== false) {
                      setViewMode("list");
                      setIsSubmitted(false);
                    }
                  }}
                  sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>

          <Box sx={{ px: 2, pb: 2 }}>
            {viewMode === "report" ? (
              <ShiftReportModal
                open={true}
                onClose={() => setViewMode("list")}
                lookups={{
                  shifts: rows.map(x => ({ id: x.id, stxt: x.stxt || x.name || x.Name || x.stxt || x.ltxt || x.shortName || x.code || "" })),
                  shiftTypes: shiftTypes.map(x => ({ id: x.id, stxt: x.name || x.Name || x.shortName || x.stxt || x.ltxt || "" })),
                  sharedWith: sharedWithOptions,
                  entities: entities
                }}
                isFullScreenView={true}
              />
            ) : (
              <FormThemeWrapper>
                <Box
                  sx={{
                    p: 0,
                    pointerEvents: viewMode === "view" ? "none" : "auto",
                    opacity: viewMode === "view" ? 0.9 : 1,
                  }}
                >
                  {/* Basic Information */}
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "space-between",
                      mb: 3,
                      cursor: "pointer",
                      userSelect: "none",
                    }}
                    onClick={() => setFormData((prev: any) => ({ ...prev, _basicOpen: !prev._basicOpen }))}
                  >
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                      <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                      <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                        <T>Basic Information</T>
                      </Typography>
                    </Box>
                    <Box sx={{ borderRadius: "50%", p: 0.5 }}>{renderExpandIcon(formData._basicOpen ?? true)}</Box>
                  </Box>

                  <Collapse in={formData._basicOpen ?? true}>
                    <Grid container spacing={3}>
                      <Grid item xs={12} md={4}>
                        <FormInput
                          label={translate("Code")}
                          value={formData.code || ""}
                          disabled
                          placeholder={translate("Auto Generated")}
                        />
                      </Grid>
                      <Grid item xs={12} md={8}>
                        <FormInput
                          label={translate("Shift Name")}
                          placeholder={translate("Enter Shift Name")}
                          value={formData.stxt || ""}
                          onChange={(e) => setFormData({ ...formData, stxt: e.target.value })}
                          inputProps={{ maxLength: 50 }}
                          error={isSubmitted && !formData.stxt}
                          required
                        />
                      </Grid>

                      <Grid item xs={12} md={4}>
                        <LookupDropdown
                          label={translate("Shift Type")}
                          value={formData.shifttypename || ""}
                          options={shiftTypes}
                          getOptionLabel={(opt) => opt?.name || opt?.stxt || opt?.code || ""}
                          onSelect={selectShiftType}
                          onSearchClick={() => setShiftTypeModalOpen(true)}
                          placeholder={translate("Select Shift Type")}
                          hideDropdownArrow
                          error={isSubmitted && !formData.shifttypeid}
                        />
                      </Grid>

                      <Grid item xs={12} md={4}>
                        <LookupDropdown
                          label={translate("Shared With")}
                          value={formData.lovsharedname || ""}
                          options={sharedWithOptions}
                          getOptionLabel={(opt) => opt?.name || opt?.stxt || opt?.code || ""}
                          onSelect={selectSharedWith}
                          onSearchClick={() => setSharedWithModalOpen(true)}
                          placeholder={translate("Select Shared With")}
                          hideDropdownArrow
                          error={isSubmitted && !formData.lovsharedid}
                          disabled={viewMode === "edit" || viewMode === "view"}
                        />
                      </Grid>

                    </Grid>
                    {renderSharedWithSelection()}
                  </Collapse>

                  {/* Off Days */}
                  <Box sx={{ mt: 3 }}>
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 2 }}>
                      <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                      <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                        <T>Off Days</T>
                      </Typography>
                    </Box>
                    <Grid container spacing={2}>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.womonday} onChange={(checked) => setFormData({ ...formData, womonday: checked })} label={translate("Monday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wotuesday} onChange={(checked) => setFormData({ ...formData, wotuesday: checked })} label={translate("Tuesday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wowednesday} onChange={(checked) => setFormData({ ...formData, wowednesday: checked })} label={translate("Wednesday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wothuresday} onChange={(checked) => setFormData({ ...formData, wothuresday: checked })} label={translate("Thursday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wofriday} onChange={(checked) => setFormData({ ...formData, wofriday: checked })} label={translate("Friday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wosaturday} onChange={(checked) => setFormData({ ...formData, wosaturday: checked })} label={translate("Saturday")} /></Grid>
                      <Grid item xs={6} md={3}><LmsCheckbox checked={!!formData.wosunday} onChange={(checked) => setFormData({ ...formData, wosunday: checked })} label={translate("Sunday")} /></Grid>
                    </Grid>
                  </Box>

                  {/* Attendance Policy */}
                  <Box sx={{ mt: 3 }}>
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 2 }}>
                      <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                      <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                        <T>Attendance Policy</T>
                      </Typography>
                    </Box>
                    <Grid container spacing={3}>
                      <Grid item xs={12} md={6}>
                        <LookupDropdown
                          label={translate("Late Arrival Policy")}
                          value={formData.latearivpolicyid || ""}
                          options={policyLate}
                          getOptionLabel={(opt) => opt?.stxt || opt?.name || opt?.code || ""}
                          onSelect={(row) =>
                            setFormData({ ...formData, latearivpolicyid: toPersistedId(row.id) })
                          }
                          onSearchClick={() => setActivePolicyLookup("late")}
                          placeholder={translate("Select Late Arrival Policy")}
                          hideDropdownArrow
                        />
                      </Grid>
                      <Grid item xs={12} md={6}>
                        <FormInput
                          label={translate("Late Arrival (Grace Min)")}
                          type="number"
                          value={formData.latearivpolicymnt ?? ""}
                          onChange={(e) => setFormData({ ...formData, latearivpolicymnt: e.target.value })}
                          onKeyDown={preventNegativeNumberInput}
                          onPaste={preventNegativeNumberPaste}
                          inputProps={{ min: 0 }}
                          error={isSubmitted && isOptionalNumberNonPositive(formData.latearivpolicymnt)}
                        />
                      </Grid>

                      <Grid item xs={12} md={6}>
                        <LookupDropdown
                          label={translate("Early Departure Policy")}
                          value={formData.earlydeppolicyid || ""}
                          options={policyEarly}
                          getOptionLabel={(opt) => opt?.stxt || opt?.name || opt?.code || ""}
                          onSelect={(row) =>
                            setFormData({ ...formData, earlydeppolicyid: toPersistedId(row.id) })
                          }
                          onSearchClick={() => setActivePolicyLookup("early")}
                          placeholder={translate("Select Early Departure Policy")}
                          hideDropdownArrow
                        />
                      </Grid>
                      <Grid item xs={12} md={6}>
                        <FormInput
                          label={translate("Early Departure (Min)")}
                          type="number"
                          value={formData.earlydeppolicymnt ?? ""}
                          onChange={(e) => setFormData({ ...formData, earlydeppolicymnt: e.target.value })}
                          onKeyDown={preventNegativeNumberInput}
                          onPaste={preventNegativeNumberPaste}
                          inputProps={{ min: 0 }}
                          error={isSubmitted && isOptionalNumberNonPositive(formData.earlydeppolicymnt)}
                        />
                      </Grid>

                      <Grid item xs={12} md={6}>
                        <LookupDropdown
                          label={translate("Overtime Policy")}
                          value={formData.otpolicyid || ""}
                          options={policyOver}
                          getOptionLabel={(opt) => opt?.stxt || opt?.name || opt?.code || ""}
                          onSelect={(row) =>
                            setFormData({ ...formData, otpolicyid: toPersistedId(row.id) })
                          }
                          onSearchClick={() => setActivePolicyLookup("over")}
                          placeholder={translate("Select Overtime Policy")}
                          hideDropdownArrow
                        />
                      </Grid>
                      <Grid item xs={12} md={6}>
                        <FormInput
                          label={translate("Overtime (Min)")}
                          type="number"
                          value={formData.otpolicymnt ?? ""}
                          onChange={(e) => setFormData({ ...formData, otpolicymnt: e.target.value })}
                          onKeyDown={preventNegativeNumberInput}
                          onPaste={preventNegativeNumberPaste}
                          inputProps={{ min: 0 }}
                          error={isSubmitted && isOptionalNumberNonPositive(formData.otpolicymnt)}
                        />
                      </Grid>

                      <Grid item xs={12} md={6}>
                        <LookupDropdown
                          label={translate("Absent Policy")}
                          value={formData.abspolicyid || ""}
                          options={policyAbsent}
                          getOptionLabel={(opt) => opt?.stxt || opt?.name || opt?.code || ""}
                          onSelect={(row) =>
                            setFormData({ ...formData, abspolicyid: toPersistedId(row.id) })
                          }
                          onSearchClick={() => setActivePolicyLookup("absent")}
                          placeholder={translate("Select Absent Policy")}
                          hideDropdownArrow
                        />
                      </Grid>
                    </Grid>
                  </Box>

                  {/* Schedule Section */}
                  <Box sx={{ mt: 3 }}>
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 2 }}>
                      <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                      <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                        <T>Schedule</T>
                      </Typography>
                    </Box>

                    <StyledTabs
                      value={activeDayTab}
                      onChange={(_, v) => setActiveDayTab(v)}
                      variant="scrollable"
                      scrollButtons="auto"
                      allowScrollButtonsMobile
                      tabs={dayTabs}
                    />

                    <Paper
                      elevation={0}
                      sx={{
                        mt: -3,
                        border: "1px solid var(--lms-border)",
                        borderTop: "none",
                        borderRadius: "0 0 12px 12px",
                        overflow: "hidden",
                        bgcolor: "#fff",
                      }}
                    >
                      <TabPanel value={activeDayTab} index={activeDayTab} padding={2}>
                        <SchemaTable
                          title={translate("Schedule")}
                          addButtonLabel={translate("Add Schedule")}
                          columns={scheduleColumns}
                          showAddButton={true}
                          rows={scheduleRows}
                          onAddClick={addScheduleRow}
                          actions={false}
                          showEditAction={false}
                          hideTitle={true}
                          hideBreadcrumbs={true}
                          pagination={false}
                          filtersEnabled={false}
                          downloadEnabled={false}
                          stickyHeader={true}
                          rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                        />

                      </TabPanel>
                    </Paper>
                  </Box>

                  <Dialog
                    open={breakModalOpen}
                    onClose={() => setBreakModalOpen(false)}
                    maxWidth="lg"
                    fullWidth
                  >
                    <DialogTitle sx={{ fontWeight: 900, display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                      <Box>
                        <T>Break Details</T>
                        {selectedSchedule?.dayName ? ` - ${selectedSchedule.dayName}` : ""}
                      </Box>
                      <IconButton size="small" onClick={() => setBreakModalOpen(false)}>
                        <CloseIcon fontSize="small" />
                      </IconButton>
                    </DialogTitle>
                    <DialogContent dividers>
                      <SchemaTable
                        title={translate("Breaks")}
                        columns={breakColumns}
                        rows={selectedSchedule?.breaks || []}
                        actions={false}
                        onAddClick={addBreakRow}
                        addButtonLabel={translate("Add Break")}
                        showAddButton={true}
                        showEditAction={false}
                        hideTitle={true}
                        hideBreadcrumbs={true}
                        pagination={false}
                        filtersEnabled={false}
                        downloadEnabled={false}
                        stickyHeader={true}
                        rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                        maxHeight={360}
                      />
                    </DialogContent>
                    <DialogActions sx={{ px: 3, py: 2 }}>
                      <Button
                        variant="contained"
                        onClick={() => setBreakModalOpen(false)}
                        sx={{ borderRadius: "8px", textTransform: "none", fontWeight: 700 }}
                      >
                        {translate("Close")}
                      </Button>
                    </DialogActions>
                  </Dialog>
                </Box>
              </FormThemeWrapper>
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Shift")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9", // A vibrant sky blue for reports
                borderRadius: "10px",
                px: 3,
                py: 0.8,
                boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
                "&:hover": {
                  bgcolor: "#0284c7",
                  boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
                },
              }}
              onClick={() => setViewMode("report")}
            >
              <T>Shift Report</T>
            </Button>
          }
          actionsPosition="right"
          onAddClick={() => {
            setFormData({ ...initialFormData, _basicOpen: true });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={async (row: any) => {
            await loadShiftById(row.id);
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={async (row: any) => {
            await loadShiftById(row.id);
            setIsSubmitted(false);
            setViewMode("view");
          }}
          showAddButton={true}
          showToaster={false}
        />
      )}

      {renderPolicyLookupDialog()}

      <GenericSearchModal
        open={shiftTypeModalOpen}
        onClose={() => setShiftTypeModalOpen(false)}
        title={translate("Shift Type")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: translate("Name"), minWidth: 220 },
          { id: "ltxt", label: translate("Description"), minWidth: 220 },
        ]}
        data={shiftTypes}
        onSelect={(row) => {
          selectShiftType(row);
          setShiftTypeModalOpen(false);
        }}
        selectedValue={formData.shifttypeid ?? ""}
        selectedField="id"
      />

      <GenericSearchModal
        open={sharedWithModalOpen}
        onClose={() => setSharedWithModalOpen(false)}
        title={translate("Shared With")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: translate("Name"), minWidth: 220 },
          { id: "ltxt", label: translate("Description"), minWidth: 220 },
        ]}
        data={sharedWithOptions}
        onSelect={(row) => {
          selectSharedWith(row);
          setSharedWithModalOpen(false);
        }}
        selectedValue={formData.lovsharedid ?? ""}
        selectedField="id"
      />

      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
    </>
  );
};

export default Shift;
