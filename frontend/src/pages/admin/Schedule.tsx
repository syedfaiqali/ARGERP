import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Grid, Box, Typography, IconButton, Paper, Button } from "@mui/material";
import { Settings as SettingsIcon } from "@mui/icons-material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { FormTimePicker } from "@/components/FormTimePicker";
import { LmsToaster } from "@/components/LmsToaster";
import { Collapse } from "@mui/material";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { 
  ArrowBack as ArrowBackIcon, 
  Save as SaveIcon,
  Assessment as AssessmentIcon
} from "@mui/icons-material";
import {
  GetService,
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLookups, useHrLovs } from "@/hooks";
import { scheduleSlice } from "@/store/humanResourceSlices";
import dayjs from "dayjs";
import { ScheduleReportModal } from "./ScheduleReportModal";

const getLovLabel = (opt: any) => opt?.name || opt?.stxt || opt?.code || "";

interface ScheduleProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const Schedule: React.FC<ScheduleProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrSchedule",
    scheduleSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;

  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );
  // SCHFR is global LOV reference data, shared with Shift.
  const { lov } = useHrLovs(["SCHFR"]);
  const scheduleForOptions = lov("SCHFR");
  const [shiftOptions, setShiftOptions] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">("list");
  const [formData, setFormData] = useState<any>(null);
  const [isSubmitted, setIsSubmitted] = useState(false);

  const resolveScheduleForMode = useCallback(
    (scheduleForId: number | null | undefined) => {
      if (!scheduleForId) return null;
      const opt = scheduleForOptions.find((o: any) => o.id === scheduleForId);
      const values = [opt?.code, opt?.stxt, opt?.name, opt?.ltxt]
        .filter(Boolean)
        .map((value) => String(value).trim().toLowerCase());
      if (values.some((value) => value === "ent" || value.includes("entity"))) return "entity";
      if (values.some((value) => value === "dpt" || value.includes("department"))) return "department";
      if (values.some((value) => value === "emp" || value.includes("employee") || value.includes("individual"))) return "employee";
      return null;
    },
    [scheduleForOptions],
  );

  // The grid shape (display index, formatted dates) is derived here so the
  // cache can keep the raw records.
  const rows = useMemo<any[]>(
    () =>
      (cachedRows || []).map((item: any, index: number) => ({
        id: item.id,
        sNo: index + 1,
        scheduleName: item.stxt ?? "",
        fromDate: item.fromDate ? dayjs(item.fromDate).format("DD-MMM-YYYY") : "",
        toDate: item.tillDate ? dayjs(item.tillDate).format("DD-MMM-YYYY") : "",
        fromDateValue: item.fromDate ?? null,
        tillDateValue: item.tillDate ?? null,
        shift: item.shiftName ?? "",
        scheduleFor: item.scheduleForName ?? "",
        scheduleForId: item.scheduleForId ?? item.scheduleForId,
        shiftId: item.shiftId ?? null,
        assignToId: item.assignToId ?? null,
      })),
    [cachedRows],
  );

  // Institutions and departments are shared across the HR screens and come from
  // the scoped lookup cache. Shifts and the employee roster are fetched here:
  // they are screen-specific lists that no slice models.
  const { lookup } = useHrLookups(["institutions", "departments"]);

  const entities = useMemo<any[]>(
    () =>
      (lookup("institutions") || []).map((item: any) => ({
        id: Number(item.id ?? item.entityId ?? 0),
        code: item.code ?? "",
        name: item.instituteName ?? item.entityName ?? item.stxt ?? item.name ?? "",
        description: item.description ?? item.ltxt ?? "",
      })),
    [lookup],
  );

  const departments = useMemo<any[]>(
    () =>
      (lookup("departments") || []).map((item: any) => ({
        id: Number(item.id ?? item.departmentId ?? item.depid ?? 0),
        code: item.code ?? item.departmentcode ?? item.departmentCode ?? "",
        name: item.departmentName ?? item.department ?? item.depstxt ?? item.name ?? "",
        description: item.description ?? item.ltxt ?? "",
      })),
    [lookup],
  );

  const fetchLookups = useCallback(async () => {
    try {
      const [shiftResp, employeeResp] = await Promise.all([
        GetService("/api/HumanResource/Shift/v3?pageNumber=1&pageSize=1000"),
        GetService("/api/HumanResource/EmployeeAttendance/v3/employees"),
      ]);

      const shiftData = (shiftResp as any)?.data || {};
      const shiftItems = shiftData.items || shiftData.Items || shiftData || [];
      setShiftOptions(shiftItems || []);

      const empItems = Array.isArray((employeeResp as any)?.data)
        ? (employeeResp as any).data
        : [];
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
  }, []);

  useEffect(() => {
    fetchLookups();
  }, [fetchLookups]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Schedule"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Schedule"));
    else if (viewMode === "view") onSubViewChange(translate("View Schedule"));
    else if (viewMode === "report") onSubViewChange(translate("Schedule Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: "S.No", minWidth: 50 },
    { id: "scheduleName", label: "Schedule Name", minWidth: 200 },
    { id: "fromDate", label: "From Date", minWidth: 120 },
    { id: "toDate", label: "To Date", minWidth: 120 },
    { id: "shift", label: "Shift", minWidth: 150 },
    { id: "scheduleFor", label: "Schedule For", minWidth: 150 },
  ];

  const initialFormData = {
    id: null as number | null,
    scheduleId: "",
    scheduleForId: null as number | null,
    scheduleForName: "",
    scheduleName: "",
    fromDate: null as any,
    tillDate: null as any,
    shiftId: null as number | null,
    shiftName: "",
    assignToId: null as number | null,
    assignToName: "",
    details: [] as any[],
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!data.scheduleForId) errors.push("Schedule For is required");
    if (!data.scheduleName) errors.push("Schedule Name is required");
    if (!data.fromDate) errors.push("From Date is required");
    if (!data.tillDate) errors.push("Till Date is required");
    if (!data.shiftId) errors.push("Shift is required");
    if (data.fromDate && data.tillDate) {
      const fromDate = dayjs(data.fromDate).startOf("day");
      const tillDate = dayjs(data.tillDate).startOf("day");
      const duplicateDateRange = rows.some((row: any) => {
        if (String(row.id) === String(data.id)) return false;

        const existingFromDate = dayjs(row.fromDateValue).startOf("day");
        const existingTillDate = dayjs(row.tillDateValue).startOf("day");
        if (!existingFromDate.isValid() || !existingTillDate.isValid()) return false;

        return (
          !fromDate.isAfter(existingTillDate, "day") &&
          !tillDate.isBefore(existingFromDate, "day")
        );
      });

      if (duplicateDateRange) {
        errors.push(translate("A schedule already exists for the selected date range."));
      }
    }
    const scheduleForMode = resolveScheduleForMode(data.scheduleForId);
    if (scheduleForMode && !data.assignToId) {
      errors.push(
        scheduleForMode === "entity"
          ? "Entity is required"
          : scheduleForMode === "department"
            ? "Department is required"
            : "Employee is required",
      );
    }
    if (!data.details || data.details.length === 0)
      errors.push("Schedule Detail is required");
    const detailDates = new Set<string>();
    const hasDuplicateDetailDate = (data.details || []).some((detail: any) => {
      if (!detail.schDate || !dayjs(detail.schDate).isValid()) return false;
      const dateKey = dayjs(detail.schDate).format("YYYY-MM-DD");
      if (detailDates.has(dateKey)) return true;
      detailDates.add(dateKey);
      return false;
    });
    if (hasDuplicateDetailDate) {
      errors.push(translate("Duplicate schedule dates are not allowed."));
    }

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    }

    const payload = {
      id: data.id ?? null,
      code: data.scheduleId || null,
      stxt: data.scheduleName?.trim(),
      fromDate: data.fromDate ? dayjs(data.fromDate).toISOString() : null,
      tillDate: data.tillDate ? dayjs(data.tillDate).toISOString() : null,
      scheduleForId: data.scheduleForId,
      shiftId: data.shiftId,
      assignToId: data.assignToId,
      details: (data.details || []).map((d: any, index: number) => ({
        id: d.id ?? null,
        lineIndex: d.lineIndex ?? index + 1,
        schDate: d.schDate ? dayjs(d.schDate).toISOString() : null,
        holiday: !!d.holiday,
        shiftId: data.shiftId,
        timeIn: d.timeIn ? `1970-01-01T${d.timeIn}:00.000Z` : null,
        timeOut: d.timeOut ? `1970-01-01T${d.timeOut}:00.000Z` : null,
        duration: d.duration ?? null,
      })),
    };

    try {
      if (data.id) {
        await PutService("/api/HumanResource/Schedule/v3", payload);
        setErrorMessages(["Schedule updated successfully"]);
      } else {
        await PostService("/api/HumanResource/Schedule/v3", payload);
        setErrorMessages(["Schedule saved successfully"]);
      }
      setToasterSeverity("success");
      setShowError(true);
      dispatch(scheduleSlice.fetchAll(true));
      return true;
    } catch (error: any) {
      const message =
        error?.response?.data.message || error?.message || "Failed to save schedule";
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    }
  };

  const handleDelete = async (row: any) => {
    try {
      await DeleteService(`/api/HumanResource/Schedule/v3/${row.id}`);
      setErrorMessages(["Record deleted successfully"]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(scheduleSlice.fetchAll(true));
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to delete schedule";
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
      throw error;
    }
  };

  const extractTime = (value?: string | null) => {
    if (!value) return "";
    const d = dayjs(value);
    return d.isValid() ? d.format("HH:mm") : "";
  };

  const loadSchedule = async (row: any) => {
    if (!row?.id) return row;
    const response: any = await GetService(`/api/HumanResource/Schedule/v3/${row.id}`);
    const data = response?.data;
    if (!data) return row;

    return {
      id: data.id,
      scheduleId: data.code || "",
      scheduleForId: data.scheduleForId ?? null,
      scheduleForName: data.scheduleForName ?? "",
      scheduleName: data.stxt ?? "",
      fromDate: data.fromDate ? dayjs(data.fromDate) : null,
      tillDate: data.tillDate ? dayjs(data.tillDate) : null,
      shiftId: data.shiftId ?? null,
      shiftName: data.shiftName ?? "",
      assignToId: data.assignToId ?? null,
      assignToName: "",
      details: (data.details || []).map((d: any, idx: number) => ({
        id: d.id ?? null,
        lineIndex: d.lineIndex ?? idx + 1,
        schDate: d.schDate ? dayjs(d.schDate) : null,
        holiday: !!d.holiday,
        timeIn: extractTime(d.timeIn),
        timeOut: extractTime(d.timeOut),
        duration: d.duration ?? null,
        dayName: d.dayName ?? (d.schDate ? dayjs(d.schDate).format("dddd") : ""),
      })),
    };
  };

  return (
    <>
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
              {viewMode === "new"
                ? translate("New Schedule")
                : viewMode === "edit"
                  ? translate("Edit Schedule")
                  : viewMode === "view"
                    ? translate("View Schedule")
                    : translate("Schedule Report")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                sx={{
                  borderRadius: "10px",
                  textTransform: "none",
                  fontWeight: 700,
                }}
              >
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && viewMode !== "report" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={async () => {
                    if (!formData) return;
                    setIsSubmitted(true);
                    const ok = await handleSave(formData);
                    if (ok !== false) {
                      setViewMode("list");
                      setIsSubmitted(false);
                    }
                  }}
                  sx={{
                    borderRadius: "10px",
                    textTransform: "none",
                    fontWeight: 700,
                    px: 4,
                  }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>
          <Box sx={{ px: 2, pb: 2 }}>
            {viewMode === "report" ? (
              <ScheduleReportModal
                open={true}
                onClose={() => setViewMode("list")}
                lookups={{
                  schedules: rows,
                  shifts: shiftOptions,
                  scheduleFor: scheduleForOptions,
                  entities: entities,
                  departments: departments,
                  employees: employees,
                }}
                isFullScreenView={true}
              />
            ) : (
              <ScheduleForm
                formData={formData ?? initialFormData}
                setFormData={setFormData}
                isReadOnly={viewMode === "view"}
                isSubmitted={isSubmitted}
                scheduleForOptions={scheduleForOptions}
                shiftOptions={shiftOptions}
                entities={entities}
                departments={departments}
                employees={employees}
                resolveScheduleForMode={resolveScheduleForMode}
                onValidationError={(messages) => {
                  setErrorMessages(messages);
                  setToasterSeverity("error");
                  setShowError(true);
                }}
              />
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title="Schedule"
          addButtonLabel={translate("New Schedule")}
          breadcrumbPath=""
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          actionsPosition="right"
          onAddClick={() => {
            setFormData({ ...initialFormData });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={async (row: any) => {
            setApiLoading(true);
            try {
              const loaded = await loadSchedule(row);
              setFormData({ ...initialFormData, ...loaded });
              setIsSubmitted(false);
              setViewMode("edit");
            } catch (error: any) {
              const message =
                error?.response?.data || error?.message || "Failed to load schedule";
              setErrorMessages([message]);
              setToasterSeverity("error");
              setShowError(true);
            } finally {
              setApiLoading(false);
            }
          }}
          onViewClick={async (row: any) => {
            setApiLoading(true);
            try {
              const loaded = await loadSchedule(row);
              setFormData({ ...initialFormData, ...loaded });
              setIsSubmitted(false);
              setViewMode("view");
            } catch (error: any) {
              const message =
                error?.response?.data || error?.message || "Failed to load schedule";
              setErrorMessages([message]);
              setToasterSeverity("error");
              setShowError(true);
            } finally {
              setApiLoading(false);
            }
          }}
          showAddButton={true}
          showToaster={false}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              onClick={() => setViewMode("report")}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9", // vibrant sky blue
                px: 3,
                py: 0.8,
                boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
                "&:hover": {
                  bgcolor: "#0284c7",
                  boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
                },
              }}
            >
              {translate("Schedule Report")}
            </Button>
          }
        />
      )}
      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {loading && <LmsOverlayLoader />}
    </>
  );
};

type LookupType = "scheduleFor" | "shift" | "entity" | "department" | "employee";

const ScheduleForm: React.FC<{
  formData: any;
  setFormData: React.Dispatch<React.SetStateAction<any>>;
  isReadOnly?: boolean;
  isSubmitted?: boolean;
  scheduleForOptions: any[];
  shiftOptions: any[];
  entities: any[];
  departments: any[];
  employees: any[];
  resolveScheduleForMode: (
    scheduleForId: number | null | undefined,
  ) => "entity" | "department" | "employee" | null;
  onValidationError: (messages: string[]) => void;
}> = ({
  formData,
  setFormData,
  isReadOnly,
  isSubmitted,
  scheduleForOptions,
  shiftOptions,
  entities,
  departments,
  employees,
  resolveScheduleForMode,
  onValidationError,
}) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [lookupType, setLookupType] = useState<LookupType | null>(null);
  const [scopedShiftOptions, setScopedShiftOptions] = useState<any[]>([]);

  const scheduleForMode = resolveScheduleForMode(formData.scheduleForId);
  const selectedDepartmentId =
    scheduleForMode === "department" && Number(formData.assignToId) > 0
      ? Number(formData.assignToId)
      : null;
  const selectedEmployeeId =
    scheduleForMode === "employee" && Number(formData.assignToId) > 0
      ? Number(formData.assignToId)
      : null;
  const requiresScopedShift = scheduleForMode === "department" || scheduleForMode === "employee";
  const selectedShiftScopeId = selectedDepartmentId ?? selectedEmployeeId;
  const normalizedScheduleForOptions = useMemo(
    () =>
      (scheduleForOptions || []).map((o: any) => ({
        ...o,
        stxt: o?.stxt ?? o?.name ?? o?.code ?? "",
        ltxt: o?.ltxt ?? o?.description ?? o?.name ?? o?.stxt ?? o?.code ?? "",
      })),
    [scheduleForOptions],
  );

  const normalizedShiftOptions = useMemo(
    () =>
      (requiresScopedShift
        ? scopedShiftOptions
        : shiftOptions || []
      ).map((s: any) => ({
        ...s,
        stxt: s?.name ?? s?.stxt ?? s?.code ?? "",
        code: s?.code ?? "",
      })),
    [requiresScopedShift, scopedShiftOptions, shiftOptions],
  );

  useEffect(() => {
    if (!selectedShiftScopeId) {
      setScopedShiftOptions([]);
      return;
    }

    const scopeQuery = selectedDepartmentId
      ? `departmentId=${selectedDepartmentId}`
      : `employeeId=${selectedEmployeeId}`;
    let cancelled = false;
    void GetService(
      `/api/HumanResource/EmployeeProfile/v3/lookups/shifts?${scopeQuery}`,
    )
      .then((response: any) => {
        if (cancelled) return;
        const data = response?.data;
        setScopedShiftOptions(
          Array.isArray(data) ? data : data?.items ?? data?.Items ?? [],
        );
      })
      .catch(() => {
        if (!cancelled) setScopedShiftOptions([]);
      });

    return () => {
      cancelled = true;
    };
  }, [selectedDepartmentId, selectedEmployeeId, selectedShiftScopeId]);

  const assignOptions = useMemo(() => {
    if (scheduleForMode === "entity") return entities;
    if (scheduleForMode === "department") return departments;
    if (scheduleForMode === "employee") return employees;
    return [];
  }, [departments, employees, entities, scheduleForMode]);

  const assignLabel = useCallback(
    (opt: any) => {
      if (!opt) return "";
      if (scheduleForMode === "employee") {
        const name = opt.employeeName ?? opt.name ?? opt.stxt ?? "";
        const code = opt.employeeCode ?? opt.code ?? "";
        return code ? `${name} (${code})` : name;
      }
      return opt.name ?? opt.stxt ?? opt.code ?? "";
    },
    [scheduleForMode],
  );

  const selectedEntityId = useMemo(() => {
    const storedId =
      localStorage.getItem("selectedEntity") ||
      localStorage.getItem("selectedEntityId") ||
      localStorage.getItem("selectedEntityID") ||
      localStorage.getItem("selectedCampusId") ||
      "";
    const id = Number(storedId);
    return Number.isFinite(id) && id > 0 ? id : null;
  }, []);

  useEffect(() => {
    if (scheduleForMode !== "entity" || !selectedEntityId) return;

    const entity = entities.find((item) => Number(item.id) === selectedEntityId);
    setFormData((current: any) => {
      if (
        current.assignToId === selectedEntityId &&
        current.assignToName === (entity?.name || entity?.code || "")
      ) {
        return current;
      }

      return {
        ...current,
        assignToId: selectedEntityId,
        assignToName: entity?.name || entity?.code || "",
      };
    });
  }, [entities, scheduleForMode, selectedEntityId, setFormData]);

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
    if (s == null || e == null || e < s) return null;
    const diff = e - s;
    return Number((diff / 60).toFixed(2));
  };

  const timeStrToDayjs = (timeStr?: string | null) => {
    if (!timeStr) return null;
    const dt = dayjs(`1970-01-01T${timeStr}`);
    return dt.isValid() ? dt : null;
  };

  const updateDetailRow = (id: number, patch: Partial<any>) => {
    setFormData((prev: any) => ({
      ...prev,
      details: (prev.details || []).map((d: any) =>
        d.id === id ? { ...d, ...patch } : d,
      ),
    }));
  };

  const addDetailRow = () => {
    setFormData((prev: any) => {
      const lastDate = (prev.details || []).length > 0 
        ? dayjs(prev.details[prev.details.length - 1].schDate).add(1, 'day')
        : dayjs(prev.fromDate || undefined);
      
      return {
        ...prev,
        details: [
          ...(prev.details || []),
          {
            id: -Math.floor(Math.random() * 1000000),
            lineIndex: (prev.details || []).length + 1,
            schDate: lastDate,
            dayName: lastDate.format("dddd"),
            holiday: false,
            timeIn: "",
            timeOut: "",
            duration: null,
          },
        ],
      };
    });
  };

  const handleProcess = async () => {
    const errors: string[] = [];
    if (!formData.fromDate) errors.push(translate("From Date is required"));
    if (!formData.tillDate) errors.push(translate("Till Date is required"));
    if (!formData.shiftId) errors.push(translate("Shift is required"));
    if (formData.fromDate && formData.tillDate && dayjs(formData.tillDate).isBefore(dayjs(formData.fromDate), "day")) {
      errors.push(translate("Till Date cannot be less than From Date"));
    }
    if (errors.length > 0) {
      onValidationError(errors);
      return;
    }

    try {
      // 1. Fetch full shift details
      const response: any = await GetService(`/api/HumanResource/Shift/v3/${formData.shiftId}`);
      const shift = response?.data;
      if (!shift) return;

      const details = [];
      let current = dayjs(formData.fromDate);
      const end = dayjs(formData.tillDate);
      let idx = 1;

      while (current.isBefore(end) || current.isSame(end, "day")) {
        const dayName = current.format("dddd");
        // Check if it's a weekly off
        const isWeeklyOff = 
          (dayName === "Monday" && shift.woMonday) ||
          (dayName === "Tuesday" && shift.woTuesday) ||
          (dayName === "Wednesday" && shift.woWednesday) ||
          (dayName === "Thursday" && shift.woThuresday) ||
          (dayName === "Friday" && shift.woFriday) ||
          (dayName === "Saturday" && shift.woSaturday) ||
          (dayName === "Sunday" && shift.woSunday);

        // Find shift detail for this day (or use first one if generic)
        const shiftDetail = (shift.details || []).find((d: any) => d.dayName === dayName) || (shift.details || [])[0];

        details.push({
          id: -Math.floor(Math.random() * 10000000),
          lineIndex: idx++,
          schDate: current,
          dayName: dayName,
          holiday: isWeeklyOff,
          timeIn: shiftDetail ? dayjs(shiftDetail.inTime).format("HH:mm") : "00:00",
          timeOut: shiftDetail ? dayjs(shiftDetail.outTime).format("HH:mm") : "00:00",
          duration: shiftDetail ? shiftDetail.duration : null,
        });

        current = current.add(1, "day");
      }

      setFormData({ ...formData, details });
    } catch (error) {
      console.error("Process failed", error);
    }
  };

  const detailRows = Array.isArray(formData.details) ? formData.details : [];

  const detailColumns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 60,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    {
      id: "schDate",
      label: translate("Date"),
      minWidth: 140,
      format: (_: any, row: any) => (
        <FormDatePicker
          label=""
          value={row.schDate ?? null}
          onChange={(date) =>
            updateDetailRow(row.id, {
              schDate: date,
              dayName: date ? dayjs(date).format("dddd") : "",
            })
          }
          disabled
          slotProps={{
            textField: {
              size: "small",
              fullWidth: true,
            },
          }}
        />
      ),
    },
    {
      id: "dayName",
      label: translate("Day"),
      minWidth: 120,
      format: (_: any, row: any) => (
        <FormInput size="small" value={row.dayName || ""} disabled />
      ),
    },
    {
      id: "holiday",
      label: translate("Holiday"),
      minWidth: 90,
      align: "center",
      format: (_: any, row: any) => (
        <LmsCheckbox
          checked={!!row.holiday}
          onChange={(checked) => updateDetailRow(row.id, { holiday: checked })}
          hideLabel
          disabled
        />
      ),
    },
    {
      id: "timeIn",
      label: translate("Time In"),
      minWidth: 140,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={timeStrToDayjs(row.timeIn)}
          onChange={(val) => {
            const timeIn = val ? val.format("HH:mm") : "";
            const duration = computeDuration(timeIn, row.timeOut);
            updateDetailRow(row.id, { timeIn, duration });
          }}
          disabled={!!isReadOnly}
        />
      ),
    },
    {
      id: "timeOut",
      label: translate("Time Out"),
      minWidth: 140,
      format: (_: any, row: any) => (
        <FormTimePicker
          value={timeStrToDayjs(row.timeOut)}
          onChange={(val) => {
            const timeOut = val ? val.format("HH:mm") : "";
            const duration = computeDuration(row.timeIn, timeOut);
            updateDetailRow(row.id, { timeOut, duration });
          }}
          disabled={!!isReadOnly}
        />
      ),
    },
    {
      id: "duration",
      label: translate("Duration"),
      minWidth: 100,
      format: (_: any, row: any) => (
        <FormInput size="small" value={row.duration ?? ""} disabled />
      ),
    },
  ];

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

  return (
    <FormThemeWrapper>
      <Box sx={{ p: 0 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 3,
            cursor: "pointer",
            userSelect: "none",
            "&:hover": {
              "& .expand-icon-container": {
                bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
              },
            },
          }}
          onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}
        >
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
            <Box
              sx={{
                width: 4,
                height: 18,
                bgcolor: "var(--lms-primary)",
                borderRadius: 1,
              }}
            />
            <Typography
              sx={{
                fontWeight: 800,
                color: "var(--lms-text-main)",
                fontSize: "0.95rem",
              }}
            >
              {translate("Basic Information")}
            </Typography>
          </Box>
          <Box
            className="expand-icon-container"
            sx={{
              transition: "background-color 0.3s ease",
              borderRadius: "50%",
              p: 0.5,
            }}
          >
            {renderExpandIcon(isGeneralExpanded)}
          </Box>
        </Box>

        <Collapse in={isGeneralExpanded}>
          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Schedule ID")}
                  value={formData.scheduleId || ""}
                  disabled
                />
              </Box>
            </Grid>

            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <LookupDropdown
                  label={translate("Schedule For")}
                  value={formData.scheduleForId ?? ""}
                  options={normalizedScheduleForOptions}
                  getOptionLabel={(opt) => getLovLabel(opt)}
                  onSelect={(row) =>
                    setFormData({
                      ...formData,
                      scheduleForId: row.id,
                      scheduleForName: row.stxt || getLovLabel(row),
                      assignToId: null,
                      assignToName: "",
                      shiftId: null,
                      shiftName: "",
                    })
                  }
                  onSearchClick={() => setLookupType("scheduleFor")}
                  placeholder={translate("Select Schedule For")}
                  hideDropdownArrow
                  disabled={!!isReadOnly}
                  error={!!isSubmitted && !formData.scheduleForId}
                />
              </Box>
            </Grid>

            <Grid item xs={12}>
              <Box sx={{ mb: 1.5 }}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Schedule Name")}
                  required
                  placeholder={translate("Enter Schedule Name")}
                  value={formData.scheduleName || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, scheduleName: e.target.value })
                  }
                  error={!!isSubmitted && !formData.scheduleName}
                  disabled={!!isReadOnly}
                />
              </Box>
            </Grid>

            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <FormDatePicker
                  label={translate("From Date")}
                  required
                  value={formData.fromDate}
                  onChange={(date) =>
                    setFormData({ ...formData, fromDate: date })
                  }
                  disabled={!!isReadOnly}
                  slotProps={{
                    textField: {
                      size: "small",
                      fullWidth: true,
                      error: !!isSubmitted && !formData.fromDate,
                      sx: {
                        "& .MuiOutlinedInput-root": {
                          bgcolor: "#fff",
                          borderRadius: "15px",
                          "& fieldset": {
                            borderColor:
                              isSubmitted && !formData.fromDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                          "&:hover fieldset": {
                            borderColor:
                              isSubmitted && !formData.fromDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                          "&.Mui-focused fieldset": {
                            borderColor:
                              isSubmitted && !formData.fromDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                        },
                        "& .MuiInputLabel-root": {
                          color: "var(--lms-primary) !important",
                          fontWeight: 600,
                          fontSize: "0.75rem",
                        },
                        "& .MuiSvgIcon-root": {
                          color: "var(--lms-primary)",
                        },
                      },
                    },
                  }}
                />
              </Box>
            </Grid>

            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <FormDatePicker
                  label={translate("Till Date")}
                  required
                  value={formData.tillDate}
                  onChange={(date) =>
                    setFormData({ ...formData, tillDate: date })
                  }
                  disabled={!!isReadOnly}
                  slotProps={{
                    textField: {
                      size: "small",
                      fullWidth: true,
                      error: !!isSubmitted && !formData.tillDate,
                      sx: {
                        "& .MuiOutlinedInput-root": {
                          bgcolor: "#fff",
                          borderRadius: "15px",
                          "& fieldset": {
                            borderColor:
                              isSubmitted && !formData.tillDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                          "&:hover fieldset": {
                            borderColor:
                              isSubmitted && !formData.tillDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                          "&.Mui-focused fieldset": {
                            borderColor:
                              isSubmitted && !formData.tillDate
                                ? "#d32f2f"
                                : "var(--lms-primary)",
                          },
                        },
                        "& .MuiInputLabel-root": {
                          color: "var(--lms-primary) !important",
                          fontWeight: 600,
                          fontSize: "0.75rem",
                        },
                        "& .MuiSvgIcon-root": {
                          color: "var(--lms-primary)",
                        },
                      },
                    },
                  }}
                />
              </Box>
            </Grid>

            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <LookupDropdown
                  label={translate("Shift")}
                  value={formData.shiftId ?? ""}
                  options={normalizedShiftOptions}
                  getOptionLabel={(opt) => opt?.stxt || opt?.name || opt?.code || ""}
                  onSelect={(row) =>
                    setFormData({
                      ...formData,
                      shiftId: row.id,
                      shiftName: row.stxt || row.name || row.code || "",
                    })
                  }
                  onSearchClick={() => setLookupType("shift")}
                  placeholder={translate("Select Shift")}
                  hideDropdownArrow
                  disabled={
                    !!isReadOnly ||
                    (requiresScopedShift && !selectedShiftScopeId)
                  }
                  error={!!isSubmitted && !formData.shiftId}
                />
              </Box>
            </Grid>

            <Grid item xs={12} md={6}>
              <Box sx={{ mb: 1.5 }}>
                <LookupDropdown
                  label={
                    scheduleForMode === "entity"
                      ? translate("Entity")
                      : scheduleForMode === "department"
                        ? translate("Department")
                        : scheduleForMode === "employee"
                          ? translate("Employee")
                          : translate("Assign To")
                  }
                  value={formData.assignToId ?? ""}
                  options={assignOptions}
                  getOptionLabel={(opt) => assignLabel(opt)}
                  onSelect={(row) =>
                    setFormData({
                      ...formData,
                      assignToId:
                        row.id ??
                        row.employeeId ??
                        row.departmentId ??
                        row.entityId ??
                        null,
                      assignToName: assignLabel(row),
                      shiftId:
                        requiresScopedShift ? null : formData.shiftId,
                      shiftName:
                        requiresScopedShift ? "" : formData.shiftName,
                    })
                  }
                  onSearchClick={() => {
                    if (scheduleForMode === "entity") setLookupType("entity");
                    else if (scheduleForMode === "department") setLookupType("department");
                    else if (scheduleForMode === "employee") setLookupType("employee");
                  }}
                  placeholder={translate("Select")}
                  hideDropdownArrow
                  disabled={!!isReadOnly || !scheduleForMode || scheduleForMode === "entity"}
                  error={!!isSubmitted && !!scheduleForMode && !formData.assignToId}
                />
              </Box>
            </Grid>
          </Grid>
        </Collapse>

        <Box sx={{ mt: 3 }}>
          <Paper
            elevation={0}
            sx={{
              border: "1px solid var(--lms-border)",
              borderRadius: "12px",
              overflow: "hidden",
              bgcolor: "#fff",
            }}
          >
            <Box sx={{ p: 2 }}>
              <SchemaTable
                title={translate("Schedule Details")}
                addButtonLabel={translate("Add Detail")}
                columns={detailColumns}
                rows={detailRows}
                onAddClick={addDetailRow}
                actions={false}
                showEditAction={false}
                hideTitle={true}
                hideBreadcrumbs={true}
                pagination={false}
                filtersEnabled={false}
                downloadEnabled={false}
                stickyHeader={true}
                rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                showAddButton={false}
                extraToolbarItemsLeft={
                  !isReadOnly && (
                    <Button
                      variant="contained"
                      startIcon={<SettingsIcon />}
                      onClick={handleProcess}
                      sx={{
                        borderRadius: "10px",
                        textTransform: "none",
                        fontWeight: 700,
                        bgcolor: "var(--lms-primary)",
                        color: "#fff",
                        "&:hover": {
                          bgcolor: "var(--lms-primary-dark)",
                        },
                      }}
                    >
                      {translate("Process")}
                    </Button>
                  )
                }
              />
            </Box>
          </Paper>
        </Box>
      </Box>

      <GenericSearchModal
        open={lookupType === "scheduleFor"}
        onClose={() => setLookupType(null)}
        title={translate("Schedule For")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 80 },
          { id: "stxt", label: translate("Name"), minWidth: 200 },
          { id: "ltxt", label: translate("Description"), minWidth: 220 },
        ]}
        data={normalizedScheduleForOptions}
        onSelect={(row) => {
          setFormData({
            ...formData,
            scheduleForId: row.id,
            scheduleForName: row.stxt || getLovLabel(row),
            assignToId: null,
            assignToName: "",
            shiftId: null,
            shiftName: "",
          });
          setLookupType(null);
        }}
        selectedValue={formData.scheduleForId ?? ""}
        selectedField="id"
      />

      <GenericSearchModal
        open={lookupType === "shift"}
        onClose={() => setLookupType(null)}
        title={translate("Shift")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 80 },
          { id: "stxt", label: translate("Name"), minWidth: 200 },
        ]}
        data={normalizedShiftOptions}
        onSelect={(row) => {
          setFormData({
            ...formData,
            shiftId: row.id,
            shiftName: row.stxt || row.name || row.code || "",
          });
          setLookupType(null);
        }}
        selectedValue={formData.shiftId ?? ""}
        selectedField="id"
      />

      <GenericSearchModal
        open={lookupType === "entity"}
        onClose={() => setLookupType(null)}
        title={translate("Select Entity")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 80 },
          { id: "name", label: translate("Entity"), minWidth: 200 },
          { id: "description", label: translate("Description"), minWidth: 200 },
        ]}
        data={entities}
        onSelect={(row) => {
          setFormData({
            ...formData,
            assignToId: row.id,
            assignToName: row.name || row.code || "",
            shiftId: null,
            shiftName: "",
          });
          setLookupType(null);
        }}
        selectedValue={formData.assignToId ?? ""}
        selectedField="id"
      />

      <GenericSearchModal
        open={lookupType === "department"}
        onClose={() => setLookupType(null)}
        title={translate("Select Department")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 80 },
          { id: "name", label: translate("Department"), minWidth: 200 },
          { id: "description", label: translate("Description"), minWidth: 200 },
        ]}
        data={departments}
        onSelect={(row) => {
          setFormData({
            ...formData,
            assignToId: row.id,
            assignToName: row.name || row.code || "",
            shiftId: null,
            shiftName: "",
          });
          setLookupType(null);
        }}
        selectedValue={formData.assignToId ?? ""}
        selectedField="id"
      />

      <GenericSearchModal
        open={lookupType === "employee"}
        onClose={() => setLookupType(null)}
        title={translate("Select Employee")}
        columns={[
          { id: "employeeCode", label: translate("Code"), minWidth: 100 },
          { id: "employeeName", label: translate("Employee"), minWidth: 200 },
          { id: "department", label: translate("Department"), minWidth: 160 },
          { id: "designation", label: translate("Designation"), minWidth: 160 },
        ]}
        data={employees}
        onSelect={(row) => {
          const label = assignLabel(row);
          setFormData({
            ...formData,
            assignToId: row.id,
            assignToName: label,
            shiftId: null,
            shiftName: "",
          });
          setLookupType(null);
        }}
        selectedValue={formData.assignToId ?? ""}
        selectedField="id"
      />
    </FormThemeWrapper>
  );
};

export default Schedule;
