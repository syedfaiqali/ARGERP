import React, { useEffect, useMemo, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
  Box,
  Typography,
  Button,
  Collapse,
  IconButton,
  Grid,
  Avatar,
  Paper,
  Divider,
  Menu,
  MenuItem,
} from "@mui/material";
import {
  Search as SearchIcon,
  Refresh as RefreshIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormTimePicker } from "@/components/FormTimePicker";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { ActivitySearchModal } from "@/components/lookups/ActivitySearchModal";
import { LookupDropdown } from "@/components/LookupDropdown";
import {
  GetService,
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { Assessment as AssessmentIcon } from "@mui/icons-material";
import { EmployeeAttendanceReportModal } from "./EmployeeAttendanceReportModal";
import { useAppSelector } from "@/hooks";
import { useAttendanceCampusPermission } from "@/hooks/useAttendanceCampusPermission";

type EmployeeLookup = {
  id: number;
  employeeCode: string;
  employeeName: string;
  department: string;
  designation: string;
  machineCode: string;
  shiftId?: number | null;
  shift: string;
};

type ActivityLookup = {
  id: number;
  activity: string;
};

interface EmployeeAttendanceProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}
const EmployeeAttendance: React.FC<EmployeeAttendanceProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const { canAccessCampusDropdown, screenPermissionsLoaded } = useAttendanceCampusPermission();
  const selectedCampusId = useAppSelector(state => state.auth.selectedCampusId);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [backendRows, setBackendRows] = useState<any[]>([]);
  const [employeeLookupOpen, setEmployeeLookupOpen] = useState(false);
  const [filterEmployeeLookupOpen, setFilterEmployeeLookupOpen] = useState(false);
  const [activityLookupOpen, setActivityLookupOpen] = useState(false);
  const [employees, setEmployees] = useState<EmployeeLookup[]>([]);
  const [activities, setActivities] = useState<ActivityLookup[]>([]);
  const [categories, setCategories] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">(
    "list"
  );
  const [entities, setEntities] = useState<any[]>([]);
  const [filters, setFilters] = useState({
    employeeId: 0,
    employeeCode: "",
    employeeName: "",
    fromDate: dayjs(),
    tillDate: dayjs(),
  });
  const [appliedFilters, setAppliedFilters] = useState(filters);
  const [reportFilters, setReportFilters] = useState(filters);

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");
  const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedRow, setSelectedRow] = useState<any>(null);

  const handleMenuOpen = (row: any, event: any) => {
    setMenuAnchorEl(event.currentTarget);
    setSelectedRow(row);
  };

  const handleMenuClose = () => {
    setMenuAnchorEl(null);
    setSelectedRow(null);
  };

  const handleIndividualReport = () => {
    if (selectedRow) {
      setReportFilters({
        ...appliedFilters,
        employeeId: selectedRow.empId || selectedRow.employeeId || 0,
        employeeCode: selectedRow.employeeCode || "",
        employeeName: selectedRow.employeeName || "",
      });
      setViewMode("report");
    }
    handleMenuClose();
  };

  const showToast = (
    messages: string[],
    severity: "success" | "error" | "info" | "warning",
  ) => {
    setToasterMessages(messages);
    setToasterSeverity(severity);
    setShowToaster(true);
  };

  const getBackendErrorMessage = (error: any, fallback: string) => {
    const payload = error?.response?.data;
    if (typeof payload === "string" && payload.trim()) return payload;
    if (typeof payload?.error === "string" && payload.error.trim()) return payload.error;
    if (typeof payload?.message === "string" && payload.message.trim()) return payload.message;
    if (typeof payload?.title === "string" && payload.title.trim()) return payload.title;
    if (Array.isArray(payload?.errors)) {
      const messages = payload.errors
        .map((item: any) =>
          typeof item === "string" ? item : item?.description ?? item?.message,
        )
        .filter(Boolean);
      if (messages.length) return messages.join(" ");
    }
    return fallback;
  };

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new")
      onSubViewChange(translate("New Attendance"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Attendance"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Attendance"));
    else if (viewMode === "report")
      onSubViewChange(translate("Employee Attendance Summary"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const loadEmployees = async () => {
    try {
      const response: any = await GetService(
        "/api/HumanResource/EmployeeAttendance/v3/employees",
      );
      const items = response?.data ?? [];
      setEmployees(Array.isArray(items) ? items : []);
    } catch (error: any) {
      const message =
        error?.response?.data?.error ?? translate("Failed to load employees");
      showToast([message], "error");
    }
  };

  useEffect(() => {
    if (!screenPermissionsLoaded) return;
    let cancelled = false;
    const loadEntities = async () => {
      try {
        const lookup = canAccessCampusDropdown ? "entities" : "institutions";
        const response: any = await GetService(`/api/HumanResource/Lookups/v3/${lookup}`);
        const items = response?.data?.items || response?.data || [];
        if (!cancelled) setEntities(Array.isArray(items) ? items : []);
      } catch (error) {
        if (!cancelled) setEntities([]);
        console.error("Failed to load entities", error);
      }
    };
    void loadEntities();
    return () => { cancelled = true; };
  }, [canAccessCampusDropdown, screenPermissionsLoaded, selectedCampusId]);

  const loadAttendance = async (currentFilters = filters) => {
    try {
      setLoading(true);
      const params = new URLSearchParams();
      if (currentFilters.employeeId && currentFilters.employeeId > 0) {
        params.set("employeeId", String(currentFilters.employeeId));
      }
      if (currentFilters.fromDate) {
        params.set(
          "fromDate",
          dayjs(currentFilters.fromDate).format("YYYY-MM-DD"),
        );
      }
      if (currentFilters.tillDate) {
        params.set(
          "tillDate",
          dayjs(currentFilters.tillDate).format("YYYY-MM-DD"),
        );
      }

      const query = params.toString();
      const response: any = await GetService(
        `/api/HumanResource/EmployeeAttendance/v3${query ? `?${query}` : ""}`,
      );
      const items = response?.data?.items ?? response?.data ?? [];
      setBackendRows(Array.isArray(items) ? items : []);
      setAppliedFilters({ ...currentFilters });
    } catch (error: any) {
      const message =
        error?.response?.data?.error ?? translate("Failed to load attendance");
      showToast([message], "error");
    } finally {
      setLoading(false);
    }
  };

  const loadActivities = async () => {
    try {
      const response: any = await GetService(
        "/api/HumanResource/EmployeeAttendance/v3/activities",
      );
      // Backend might return the array directly or inside a data property
      const data = response?.data;
      const items = Array.isArray(data) ? data : (data?.items ?? []);
      const apiActivities = Array.isArray(items) ? items : [];

      setActivities((prev) => {
        const combined: ActivityLookup[] = [];

        apiActivities.forEach((apiAct: any) => {
          if (apiAct && typeof apiAct.id !== "undefined") {
            const label = apiAct.activity || apiAct.stxt || "";
            combined.push({ id: apiAct.id, activity: label });
          }
        });
        return combined;
      });
    } catch (error: any) {
      const message =
        error?.response?.data?.error ?? translate("Failed to load activities");
      showToast([message], "error");
    }
  };

  const loadCategories = async () => {
    try {
      const response: any = await GetService("/api/HumanResource/Lov/v3/ECAT");
      setCategories(response?.data || []);
    } catch (error) {
      console.error("Failed to load categories", error);
    }
  };

  const loadDepartments = async () => {
    try {
      const response: any = await GetService(
        "/api/HumanResource/Lookups/v3/departments",
      );
      setDepartments(response?.data?.items || response?.data || []);
    } catch (error) {
      console.error("Failed to load departments", error);
    }
  };

  useEffect(() => {
    loadAttendance();
    loadEmployees();
    loadActivities();
    loadCategories();
    loadDepartments();
  }, []);

  const columns = useMemo<Column[]>(
    () => [
      { id: "sNo", label: translate("S.No"), minWidth: 60 },
      {
        id: "employeeCode",
        label: translate("Employee ID"),
        minWidth: 120,
        format: (value) => (
          <Typography sx={{ fontWeight: 700, fontSize: "0.85rem" }}>
            {value || "--"}
          </Typography>
        ),
      },
      { id: "employeeName", label: translate("Employee"), minWidth: 200 },
      { id: "department", label: translate("Department"), minWidth: 150 },
      { id: "designation", label: translate("Designation"), minWidth: 150 },
      {
        id: "attendanceDate",
        label: translate("Date"),
        minWidth: 130,
        format: (value) => (value ? dayjs(value).format("DD/MM/YYYY") : "--"),
      },
      {
        id: "attendanceTime",
        label: translate("Time"),
        minWidth: 110,
        format: (value) => (value ? dayjs(value).format("hh:mm A") : "--"),
      },
      { id: "activity", label: translate("Activity"), minWidth: 150 },
    ],
    [translate],
  );

  const rows = useMemo(
    () =>
      backendRows.map((x, index) => {
        const attendanceDate = x.attendanceDate
          ? dayjs(x.attendanceDate)
          : null;
        const attendanceTime = x.attendanceTime
          ? dayjs(x.attendanceTime)
          : attendanceDate;

        const atnActivityId = x.activityId;
        // Resolve activity name from the loaded list if it's missing
        const activityFromList = activities.find(
          (a) => a.id === atnActivityId,
        )?.activity;

        return {
          ...x,
          sNo: index + 1,
          attendanceDate,
          attendanceTime,
          atnActivityId,
          activity: x.activity || activityFromList || "",
        };
      }),
    [backendRows, activities],
  );

  const initialFormData = useMemo(
    () => ({
      id: 0,
      empId: 0,
      employeeCode: "",
      employeeName: "",
      designation: "",
      department: "",
      machineCode: "",
      shiftId: null,
      shift: "",
      atnActivityId: 0,
      activity: "",
      attendanceDate: dayjs(),
      attendanceTime: dayjs(),
      remarks: "",
    }),
    [],
  );
  const [formData, setFormData] = useState<any>(initialFormData);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [rosterStartDate, setRosterStartDate] = useState<dayjs.Dayjs | null>(null);

  useEffect(() => {
    if (!formData.empId) {
      setRosterStartDate(null);
      return;
    }

    let cancelled = false;
    const loadRosterStartDate = async () => {
      try {
        const response: any = await GetService(
          `/api/HumanResource/EREmproaster/v3?employeeId=${formData.empId}&pageSize=1000`,
        );
        const rosterRows = response?.data?.items ?? response?.data?.Items ?? response?.data ?? [];
        const earliestStart = (Array.isArray(rosterRows) ? rosterRows : [])
          .map((row: any) => row.startDate ?? row.StartDate)
          .filter((value: any) => value && dayjs(value).isValid())
          .sort((left: any, right: any) => dayjs(left).valueOf() - dayjs(right).valueOf())[0];
        const minimumDate = earliestStart ? dayjs(earliestStart).startOf("day") : null;

        if (cancelled) return;
        setRosterStartDate(minimumDate);
        if (minimumDate) {
          setFormData((previous: any) =>
            previous.empId === formData.empId &&
            (!previous.attendanceDate || dayjs(previous.attendanceDate).isBefore(minimumDate, "day"))
              ? { ...previous, attendanceDate: minimumDate }
              : previous,
          );
        }
      } catch {
        if (!cancelled) setRosterStartDate(null);
      }
    };

    void loadRosterStartDate();
    return () => { cancelled = true; };
  }, [formData.empId]);

  const resetFilters = () => {
    setFilters({
      employeeId: 0,
      employeeCode: "",
      employeeName: "",
      fromDate: dayjs(),
      tillDate: dayjs(),
    });
  };

  const handleSave = async (formData: any) => {
    const errors = [];
    if (!formData.empId) errors.push(translate("Employee is required"));
    if (!formData.atnActivityId) errors.push(translate("Activity is required"));
    if (!formData.attendanceDate)
      errors.push(translate("Attendance Date is required"));
    if (
      rosterStartDate &&
      formData.attendanceDate &&
      dayjs(formData.attendanceDate).isBefore(rosterStartDate, "day")
    ) {
      errors.push(translate("Attendance Date cannot be before the employee roster start date"));
    }
    if (!formData.attendanceTime)
      errors.push(translate("Attendance Time is required"));

    if (errors.length > 0) {
      showToast(errors, "error");
      return false;
    }

    const payload = {
      id: formData.id,
      empId: formData.empId,
      atnActivityId: formData.atnActivityId || 0,
      attendanceDate: dayjs(formData.attendanceDate).format(
        "YYYY-MM-DDTHH:mm:ss",
      ),
      attendanceTime: dayjs(formData.attendanceTime).format(
        "YYYY-MM-DDTHH:mm:ss",
      ),
      remarks: formData.remarks ?? "",
    };

    try {
      setLoading(true);
      if (formData.id && formData.id > 0) {
        await PutService(
          `/api/HumanResource/EmployeeAttendance/v3/${formData.id}`,
          payload,
        );
        showToast([translate("Record updated successfully")], "success");
      } else {
        await PostService("/api/HumanResource/EmployeeAttendance/v3", payload);
        showToast([translate("Record saved successfully")], "success");
      }
      await loadAttendance();
      return true;
    } catch (error: any) {
      const message = getBackendErrorMessage(
        error,
        translate("Failed to save attendance"),
      );
      showToast([message], "error");
      return false;
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    if (!row.id) return;
    try {
      setLoading(true);
      await DeleteService(`/api/HumanResource/EmployeeAttendance/v3/${row.id}`);
      showToast([translate("Record deleted successfully")], "error");
      await loadAttendance();
    } catch (error: any) {
      const message =
        error?.response?.data?.error ??
        translate("Failed to delete attendance");
      showToast([message], "error");
    } finally {
      setLoading(false);
    }
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

  const renderForm = (
    formData: any,
    setFormData: React.Dispatch<React.SetStateAction<any>>,
    mode: "new" | "edit" | "view",
    isReadOnly: boolean = false,
    isSubmitted: boolean = false,
  ) => {
    const isViewMode = mode === "view" || isReadOnly;

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
                  fontSize: "0.95rem",
                  fontWeight: 800,
                  color: "var(--lms-text-main)",
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
                  <LookupDropdown
                    label={translate("Employee")}
                    placeholder={translate("Select Employee")}
                    required
                    value={formData.employeeName || formData.employeeCode || ""}
                    options={employees}
                    getOptionLabel={(opt) =>
                      opt?.employeeName || opt?.employeeCode || ""
                    }
                    onSelect={(row: any) => {
                      setFormData({
                        ...formData,
                        empId: row.id,
                        employeeCode: row.employeeCode,
                        employeeName: row.employeeName,
                        department: row.department,
                        designation: row.designation,
                        machineCode: row.machineCode,
                        shiftId: row.shiftId ?? null,
                        shift: row.shift,
                      });
                    }}
                    onClear={() =>
                      setFormData({
                        ...formData,
                        empId: 0,
                        employeeCode: "",
                        employeeName: "",
                        department: "",
                        designation: "",
                        machineCode: "",
                        shiftId: null,
                        shift: "",
                      })
                    }
                    onSearchClick={() =>
                      !isViewMode && setEmployeeLookupOpen(true)
                    }
                    disabled={isViewMode}
                    error={isSubmitted && !formData.empId}
                    helperText={
                      isSubmitted && !formData.empId
                        ? translate("Employee is required")
                        : ""
                    }
                    allowSearchWhenDisabled={false}
                    hideDropdownArrow={false}
                  />
                </Box>
              </Grid>

              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Employee ID")}
                    placeholder={translate("Employee ID")}
                    value={formData.employeeCode || ""}
                    disabled
                  />
                </Box>
              </Grid>

              <Grid item xs={12}>
                <Paper
                  elevation={0}
                  sx={{
                    p: 2,
                    borderRadius: "16px",
                    border: "1px solid #E2E8F0",
                    bgcolor: "#f8fafc",
                    display: "flex",
                    gap: 3,
                    alignItems: "center",
                    mb: 2,
                  }}
                >
                  <Avatar
                    sx={{
                      width: 70,
                      height: 70,
                      bgcolor: "#0088cc",
                      fontSize: "1rem",
                      fontWeight: 800,
                    }}
                  >
                    <Box component="span" sx={{ opacity: 0.7 }}>
                      EMP
                    </Box>
                  </Avatar>

                  <Box sx={{ display: "flex", flexGrow: 1, gap: 4 }}>
                    <Box>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                        }}
                      >
                        {translate("Employee ID")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.employeeCode || "--"}
                      </Typography>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                          mt: 1,
                        }}
                      >
                        {translate("Employee")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.employeeName || "--"}
                      </Typography>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                          mt: 1,
                        }}
                      >
                        {translate("Designation")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.designation || "--"}
                      </Typography>
                    </Box>
                    <Divider
                      orientation="vertical"
                      flexItem
                      sx={{ borderStyle: "dashed" }}
                    />
                    <Box>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                        }}
                      >
                        {translate("Machine Code")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.machineCode || "--"}
                      </Typography>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                          mt: 1,
                        }}
                      >
                        {translate("Shift")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.shift || "--"}
                      </Typography>
                      <Typography
                        sx={{
                          fontSize: "0.7rem",
                          color: "#64748B",
                          fontWeight: 700,
                          mt: 1,
                        }}
                      >
                        {translate("Department")}:
                      </Typography>
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 800 }}>
                        {formData.department || "--"}
                      </Typography>
                    </Box>
                  </Box>
                </Paper>
              </Grid>

              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={translate("Activity")}
                    placeholder={translate("Select Activity")}
                    value={formData.activity || ""}
                    options={activities}
                    getOptionLabel={(opt) => opt.activity || ""}
                    onSelect={(opt) => {
                      setFormData({
                        ...formData,
                        atnActivityId: opt.id,
                        activity: opt.activity,
                      });
                    }}
                    onSearchClick={() =>
                      !isViewMode && setActivityLookupOpen(true)
                    }
                    required
                    disabled={isViewMode}
                    allowSearchWhenDisabled={false}
                    hideDropdownArrow={true}
                    error={isSubmitted && !formData.atnActivityId}
                    helperText={
                      isSubmitted && !formData.atnActivityId
                        ? translate("Activity is required")
                        : ""
                    }
                  />
                </Box>
              </Grid>

              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormDatePicker
                    label={translate("Attendance Date")}
                    required
                    value={formData.attendanceDate || null}
                    onChange={(val) =>
                      setFormData({ ...formData, attendanceDate: val })
                    }
                    minDate={rosterStartDate ?? undefined}
                    disabled={isViewMode}
                    error={isSubmitted && !formData.attendanceDate}
                    helperText={
                      isSubmitted && !formData.attendanceDate
                        ? translate("Date is required")
                        : ""
                    }
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormTimePicker
                    label={translate("Attendance Time")}
                    required
                    value={formData.attendanceTime || null}
                    onChange={(val) =>
                      setFormData({ ...formData, attendanceTime: val })
                    }
                    disabled={isViewMode}
                    error={isSubmitted && !formData.attendanceTime}
                    helperText={
                      isSubmitted && !formData.attendanceTime
                        ? translate("Time is required")
                        : ""
                    }
                  />
                </Box>
              </Grid>

              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Remarks")}
                    placeholder={translate("Remarks")}
                    value={formData.remarks || ""}
                    disabled={isViewMode}
                    onChange={(e) =>
                      setFormData({ ...formData, remarks: e.target.value })
                    }
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>
        </Box>

        <EmployeeSearchModal
          open={employeeLookupOpen}
          onClose={() => setEmployeeLookupOpen(false)}
          data={employees}
          selectedValue={formData.employeeCode}
          onSelect={(row) => {
            setFormData({
              ...formData,
              empId: row.id,
              employeeCode: row.employeeCode,
              employeeName: row.employeeName,
              department: row.department,
              designation: row.designation,
              machineCode: row.machineCode,
              shiftId: row.shiftId ?? null,
              shift: row.shift,
            });
            setEmployeeLookupOpen(false);
          }}
        />

        <ActivitySearchModal
          open={activityLookupOpen}
          onClose={() => setActivityLookupOpen(false)}
          data={activities}
          selectedValue={formData.atnActivityId}
          onSelect={(row) => {
            setFormData({
              ...formData,
              atnActivityId: row.id,
              activity: row.activity,
            });
            setActivityLookupOpen(false);
          }}
        />
      </FormThemeWrapper>
    );
  };

  return (
    <FormThemeWrapper>
      {viewMode === "report" ? (
        <Box sx={{ p: 0 }}>
          <Box sx={{
            px: 4,
            py: 3,
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center"
          }}>
            <Typography variant="h4" sx={{ fontWeight: 800, color: "#334155", fontSize: '1.75rem' }}>
              <T>Employee Attendance Report</T>
            </Typography>
            <Button
              variant="text"
              startIcon={<ArrowBackIcon />}
              onClick={() => setViewMode("list")}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                color: "var(--lms-primary)",
                fontSize: '1rem',
                '&:hover': { bgcolor: 'transparent', opacity: 0.8 }
              }}
            >
              <T>Back to List</T>
            </Button>
          </Box>
          <Box sx={{ px: 4, pb: 4 }}>
            <EmployeeAttendanceReportModal
              open={true}
              onClose={() => setViewMode("list")}
              lookups={{ employees, entities, activities, categories, departments }}
              initialFilters={reportFilters}
              isFullScreenView={true}
            />
          </Box>
        </Box>
      ) : viewMode !== "list" ? (
        <Box
          sx={{
            p: 2,
          }}
        >
          <Box
            sx={{
              p: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              gap: 2,
              flexWrap: "wrap",
            }}
          >
            <Box>
              <Typography sx={{ fontSize: "1rem", fontWeight: 800 }}>
                {viewMode === "new"
                  ? translate("New Attendance")
                  : viewMode === "edit"
                    ? translate("Edit Attendance")
                    : translate("View Attendance")}
              </Typography>
            </Box>
            <Box sx={{ display: "flex", gap: 1.5 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => {
                  setViewMode("list");
                  setIsSubmitted(false);
                }}
                sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
              >
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={async () => {
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
                    px: 3,
                    bgcolor: "var(--lms-primary)",
                    "&:hover": { bgcolor: "var(--lms-primary)" },
                  }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>
          <Box sx={{ px: 2, pb: 2 }}>
            {renderForm(
              formData,
              setFormData,
              viewMode === "new" ? "new" : viewMode === "edit" ? "edit" : "view",
              viewMode === "view",
              isSubmitted,
            )}
          </Box>
        </Box>
      ) : (
        <>
          <Box
            sx={{
              p: 2,
              mb: 2,
              border: "1px solid #E2E8F0",
              borderRadius: 2,
              bgcolor: "#fff",
            }}
          >
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                gap: 1.5,
                mb: 2,
                flexWrap: "wrap",
              }}
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
                <Typography sx={{ fontWeight: 800, fontSize: "0.95rem" }}>
                  {translate("Filter")}
                </Typography>
              </Box>
              <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
                <Button
                  variant="contained"
                  disableElevation
                  startIcon={<SearchIcon />}
                  onClick={() => loadAttendance(filters)}
                  sx={{
                    textTransform: "none",
                    fontWeight: 700,
                    bgcolor: "var(--lms-primary)",
                    borderRadius: "10px",
                    px: 3,
                    py: 0.8,
                    "&:hover": { bgcolor: "var(--lms-primary)" },
                  }}
                >
                  {translate("Search")}
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
                  onClick={() => {
                    const defaultFilters = {
                      employeeId: 0,
                      employeeCode: "",
                      employeeName: "",
                      fromDate: dayjs(),
                      tillDate: dayjs(),
                    };
                    resetFilters();
                    loadAttendance(defaultFilters);
                  }}
                  sx={{
                    textTransform: "none",
                    fontWeight: 700,
                    borderRadius: "10px",
                    px: 3,
                    borderColor: "var(--lms-primary)",
                    color: "var(--lms-primary)",
                    "&:hover": {
                      bgcolor: "var(--lms-primary)",
                      borderColor: "var(--lms-primary)",
                      color: "#fff",
                    },
                  }}
                >
                  {translate("Clear")}
                </Button>
              </Box>
            </Box>
            <Grid container spacing={2}>
              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={translate("Employee")}
                  placeholder={translate("Select Employee")}
                  value={filters.employeeName || filters.employeeCode}
                  onSearchClick={() => setFilterEmployeeLookupOpen(true)}
                  options={employees}
                  getOptionLabel={(opt: any) =>
                    opt?.employeeName || opt?.employeeCode || ""
                  }
                  onSelect={(emp: any) =>
                    setFilters((prev) => ({
                      ...prev,
                      employeeId: Number(emp?.id ?? 0),
                      employeeCode: emp?.employeeCode || "",
                      employeeName: emp?.employeeName || "",
                    }))
                  }
                  onClear={() =>
                    setFilters((prev) => ({
                      ...prev,
                      employeeId: 0,
                      employeeCode: "",
                      employeeName: "",
                    }))
                  }
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("From Date")}
                  value={filters.fromDate}
                  onChange={(val) =>
                    setFilters({ ...filters, fromDate: val as Dayjs })
                  }
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("Till Date")}
                  value={filters.tillDate}
                  onChange={(val) =>
                    setFilters({ ...filters, tillDate: val as Dayjs })
                  }
                />
              </Grid>
            </Grid>
          </Box>
          <SchemaTable
            title={translate("Employee Attendance")}
            extraToolbarItemsLeft={
              <Button
                variant="contained"
                startIcon={<AssessmentIcon />}
                onClick={() => {
                  setReportFilters({ ...appliedFilters });
                  setViewMode("report");
                }}
                sx={{
                  textTransform: "none",
                  fontWeight: 700,
                  color: "#fff",
                  bgcolor: "#0ea5e9", // A vibrant sky blue for reports
                  borderRadius: "10px",
                  px: 3,
                  py: 0.8,
                  boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
                  transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                  "&:hover": {
                    bgcolor: "#0284c7",
                    boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
                  },
                }}
              >
                <T>Employee Attendance Report</T>
              </Button>
            }
            addButtonLabel={translate("New Attendance")}
            breadcrumbPath={''}
            columns={columns}
            rows={rows}
            onDelete={handleDelete}
            onAddClick={() => {
              setFormData({ ...initialFormData });
              setIsSubmitted(false);
              setViewMode("new");
            }}
            onEditClick={(row: any) => {
              setFormData({
                ...row,
                attendanceDate: row.attendanceDate ? dayjs(row.attendanceDate) : dayjs(),
                attendanceTime: row.attendanceTime ? dayjs(row.attendanceTime) : dayjs(),
              });
              setIsSubmitted(false);
              setViewMode("edit");
            }}
            onViewClick={(row: any) => {
              setFormData({
                ...row,
                attendanceDate: row.attendanceDate ? dayjs(row.attendanceDate) : null,
                attendanceTime: row.attendanceTime ? dayjs(row.attendanceTime) : null,
              });
              setIsSubmitted(false);
              setViewMode("view");
            }}
            showAddButton={true}
            showToaster={false}
            showMenuAction={true}
            onMenuAction={handleMenuOpen}
          />

          <Menu
            anchorEl={menuAnchorEl}
            open={Boolean(menuAnchorEl)}
            onClose={handleMenuClose}
            PaperProps={{
              sx: {
                boxShadow: '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
                borderRadius: '12px',
                minWidth: '200px',
                border: '1px solid #f1f5f9'
              }
            }}
          >
            <Box sx={{ px: 2, py: 1, borderBottom: '1px solid #f1f5f9' }}>
              <Typography sx={{ fontWeight: 700, fontSize: '0.75rem', color: '#64748b', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                <T>Reports</T>
              </Typography>
            </Box>
            <MenuItem
              onClick={handleIndividualReport}
              sx={{
                py: 1.5,
                fontSize: '0.875rem',
                color: '#0ea5e9',
                fontWeight: 500,
                '&:hover': { bgcolor: '#f0f9ff' }
              }}
            >
              <AssessmentIcon sx={{ fontSize: '1.25rem', mr: 1.5, color: '#0ea5e9' }} />
              <T>Individual Summary Report</T>
            </MenuItem>
          </Menu>
          <EmployeeSearchModal
            open={filterEmployeeLookupOpen}
            onClose={() => setFilterEmployeeLookupOpen(false)}
            data={employees}
            selectedValue={filters.employeeCode}
            onSelect={(row: any) => {
              setFilters((prev) => ({
                ...prev,
                employeeId: Number(row?.id ?? 0),
                employeeCode: row?.employeeCode || "",
                employeeName: row?.employeeName || "",
              }));
              setFilterEmployeeLookupOpen(false);
            }}
          />
        </>
      )}

      {loading && <LmsOverlayLoader />}

      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
    </FormThemeWrapper>
  );
};

export default EmployeeAttendance;
