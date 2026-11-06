import React, { useEffect, useMemo, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
  Box,
  Typography,
  Grid,
  Collapse,
  Button,
} from "@mui/material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { DepartmentSearchModal } from "@/components/lookups/DepartmentSearchModal";
import { DesignationSearchModal } from "@/components/lookups/DesignationSearchModal";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import {
  GetService,
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import {
  Save as SaveIcon,
  Search as SearchIcon,
  Refresh as RefreshIcon,
} from "@mui/icons-material";

const EmployeeDailyAttendance: React.FC = () => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [backendRows, setBackendRows] = useState<any[]>([]);
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [loading, setLoading] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");

  const [employeeLookupOpen, setEmployeeLookupOpen] = useState(false);
  const [fetchEmployeeLookupOpen, setFetchEmployeeLookupOpen] = useState(false);
  const [departmentLookupOpen, setDepartmentLookupOpen] = useState(false);
  const [designationLookupOpen, setDesignationLookupOpen] = useState(false);
  const [employees, setEmployees] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);

  const [fetchFilters, setFetchFilters] = useState<{
    fromDate: Dayjs | null;
    tillDate: Dayjs | null;
    employeeId: number;
    employeeCode: string;
    employeeName: string;
    departmentId: number;
    departmentName: string;
    designationId: number;
    designationName: string;
  }>({
    fromDate: dayjs().startOf("month"),
    tillDate: dayjs(),
    employeeId: 0,
    employeeCode: "",
    employeeName: "",
    departmentId: 0,
    departmentName: "",
    designationId: 0,
    designationName: "",
  });

  const showToast = (
    messages: string[],
    severity: "success" | "error" | "info" | "warning",
  ) => {
    setToasterMessages(messages);
    setToasterSeverity(severity);
    setShowToaster(true);
  };

  const loadEmployees = async () => {
    try {
      const [empResp, deptResp, desigResp] = await Promise.all([
        GetService("/api/HumanResource/EmployeeAttendance/v3/employees"),
        GetService("/api/HumanResource/Lookups/v3/departments"),
        GetService("/api/HumanResource/Lookups/v3/designations"),
      ]);

      const empItems = empResp?.data ?? [];
      setEmployees(Array.isArray(empItems) ? empItems : []);

      const deptItems = deptResp?.data?.items ?? deptResp?.data?.Items ?? [];
      setDepartments(
        (deptItems || []).map((item: any) => ({
          id: Number(item.id ?? item.departmentId ?? 0),
          code: item.code ?? "",
          name: item.departmentName ?? item.department ?? item.name ?? "",
          description: item.description ?? "",
        }))
      );

      const desigItems = desigResp?.data?.items ?? desigResp?.data?.Items ?? [];
      setDesignations(
        (desigItems || []).map((item: any) => ({
          id: Number(item.id ?? item.designationId ?? 0),
          code: item.code ?? item.designationCode ?? "",
          name: item.designationName ?? item.designation ?? item.name ?? "",
          description: item.description ?? "",
          account: item.account ?? "",
        }))
      );

    } catch (error: any) {
      showToast([translate("Failed to load lookup data")], "error");
    }
  };

  const loadAttendance = async (filters: any = fetchFilters) => {
    try {
      setLoading(true);
      const params = new URLSearchParams();
      if (filters?.employeeId > 0)
        params.set("employeeId", String(filters.employeeId));
      if (filters?.fromDate)
        params.set("fromDate", dayjs(filters.fromDate).format("YYYY-MM-DD"));
      if (filters?.tillDate)
        params.set("tillDate", dayjs(filters.tillDate).format("YYYY-MM-DD"));
      if (filters?.departmentId > 0)
        params.set("departmentId", String(filters.departmentId));
      if (filters?.designationId > 0)
        params.set("designationId", String(filters.designationId));

      const response: any = await GetService(
        `/api/HumanResource/EmployeeDailyAttendance/v3${params.toString() ? "?" + params.toString() : ""}`,
      );
      const items = response?.data ?? [];
      setBackendRows(Array.isArray(items) ? items : []);
    } catch (error: any) {
      showToast([translate("Failed to load attendance data")], "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEmployees();
  }, []);

  const rows = useMemo(
    () =>
      (Array.isArray(backendRows) ? backendRows : []).map((x, index) => ({
        ...x,
        sNo: index + 1,
        attDate: x.atnDate ? dayjs(x.atnDate).format("DD/MM/YYYY") : "--",
        day: x.day || "--",
        schIn: x.schTimeIn ? dayjs(x.schTimeIn).format("hh:mm A") : "--",
        schOut: x.schTimeOut ? dayjs(x.schTimeOut).format("hh:mm A") : "--",
        actIn: x.actShiftIn ? dayjs(x.actShiftIn).format("hh:mm A") : "--",
        actOut: x.actShiftOut ? dayjs(x.actShiftOut).format("hh:mm A") : "--",
      })),
    [backendRows],
  );

  const initialFormData = useMemo(
    () => ({
      employeeId: 0,
      employeeCode: "",
      employeeName: "",
      atnDate: dayjs(),
      isAbsent: false,
      isHoliday: false,
      remarks: "",
      lateMin: 0,
      lateCount: 0,
      lateHalfDays: 0,
      edMin: 0,
      edCount: 0,
      edHalfDays: 0,
      otMin: 0,
      otCount: 0,
      otHalfDays: 0,
    }),
    [],
  );

  const toApiDateTime = (value: any) =>
    value ? dayjs(value).format("YYYY-MM-DDTHH:mm:ss") : null;

  const buildAttendancePayload = (source: any) => ({
    ...source,
    atnDate: source.atnDate
      ? dayjs(source.atnDate).format("YYYY-MM-DDTHH:mm:ss")
      : null,
    employeeId: Number(source.employeeId || 0),
    schTimeIn: toApiDateTime(source.schTimeIn),
    schTimeOut: toApiDateTime(source.schTimeOut),
    actShiftIn: toApiDateTime(source.actShiftIn),
    actShiftOut: toApiDateTime(source.actShiftOut),
    lateMin: Number(source.lateMin || 0),
    lateCount: Number(source.lateCount || 0),
    lateHalfDays: Number(source.lateHalfDays || 0),
    lateAbsent: Number(source.lateAbsent || 0),
    lateExcuse: Number(source.lateExcuse || 0),
    edMin: Number(source.edMin || 0),
    edCount: Number(source.edCount || 0),
    edHalfDays: Number(source.edHalfDays || 0),
    edAbsent: Number(source.edAbsent || 0),
    edExcuse: Number(source.edExcuse || 0),
    otMin: Number(source.otMin || 0),
    otCount: Number(source.otCount || 0),
    otHalfDays: Number(source.otHalfDays || 0),
    shiftId: source.shiftId ? Number(source.shiftId) : null,
    shiftDtlId: source.shiftDtlId ? Number(source.shiftDtlId) : null,
    dayId: source.dayId ? Number(source.dayId) : null,
    departmentId: source.departmentId ? Number(source.departmentId) : null,
    designationId: source.designationId ? Number(source.designationId) : null,
    remarks: source.remarks || "",
    isAbsent: Boolean(source.isAbsent),
    isHoliday: Boolean(source.isHoliday),
  });

  const columns: Column[] = [
    { id: "sNo", label: "S.No", minWidth: 60 },
    {
      id: "employeeCode",
      label: "Employee Code",
      minWidth: 140,
      format: (value) => (
        <Typography sx={{ fontWeight: 700, fontSize: "0.85rem" }}>
          {value}
        </Typography>
      ),
    },
    { id: "employeeName", label: "Employee Name", minWidth: 180 },
    { id: "attDate", label: "Attendance Date", minWidth: 130 },
    { id: "day", label: "Day", minWidth: 100 },
    { id: "schIn", label: "Sch.Time IN", minWidth: 110 },
    { id: "schOut", label: "Sch.Time Out", minWidth: 110 },
    { id: "actIn", label: "Act Time IN", minWidth: 110 },
    { id: "actOut", label: "Act Time Out", minWidth: 110 },
    { id: "lateMin", label: "Late (Min)", minWidth: 100 },
    { id: "lateCount", label: "Late Count", minWidth: 100 },
    { id: "lateHalfDays", label: "Late H/D", minWidth: 90 },
    { id: "edMin", label: "ED (Min)", minWidth: 90 },
    { id: "edCount", label: "ED Count", minWidth: 90 },
    { id: "edHalfDays", label: "ED H/D", minWidth: 90 },
    { id: "otMin", label: "OT Dur (Min)", minWidth: 110 },
    { id: "otCount", label: "OT Count", minWidth: 90 },
    { id: "otHalfDays", label: "OT H/D", minWidth: 90 },
    {
      id: "isAbsent",
      label: "Is Absent",
      minWidth: 110,
      format: (val) => (val ? "Yes" : "No"),
    },
    { id: "remarks", label: "Remarks", minWidth: 180 },
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

  const handleSave = async (formData: any) => {
    const errors = [];
    if (!formData.employeeId) errors.push(translate("Employee is required"));
    if (!formData.atnDate)
      errors.push(translate("Attendance Date is required"));

    if (errors.length > 0) {
      showToast(errors, "error");
      return false;
    }

    const payload = buildAttendancePayload(formData);

    try {
      setLoading(true);
      if (Number(formData.id) > 0) {
        await PutService(
          `/api/HumanResource/EmployeeDailyAttendance/v3/${formData.id}`,
          payload,
        );
        showToast([translate("Attendance updated successfully")], "success");
      } else {
        await PostService(
          "/api/HumanResource/EmployeeDailyAttendance/v3",
          payload,
        );
        showToast([translate("Attendance saved successfully")], "success");
      }
      await loadAttendance();
      return true;
    } catch (error: any) {
      const message =
        error?.response?.data ?? translate("Failed to save attendance");
      showToast([message], "error");
      return false;
    } finally {
      setLoading(false);
    }
  };

  const handleBulkSave = async () => {
    const selected = selectedRowIds.length
      ? rows.filter((row: any) => selectedRowIds.includes(Number(row.id)))
      : rows;
    const items = selected
      .filter((row: any) => row.employeeId && row.atnDate)
      .map(buildAttendancePayload);

    if (!items.length) {
      showToast([translate("No attendance rows available to save")], "warning");
      return;
    }

    try {
      setLoading(true);
      // The bulk API preloads related data and processes rows set-wise. Keep
      // requests bounded to avoid repeated setup overhead and gateway timeouts.
      const bulkBatchSize = 200;
      for (let offset = 0; offset < items.length; offset += bulkBatchSize) {
        await PostService(
          "/api/HumanResource/EmployeeDailyAttendance/v3/bulk",
          {
            items: items.slice(offset, offset + bulkBatchSize),
          },
        );
      }
      showToast([translate("Attendance saved successfully")], "success");
      await loadAttendance();
      setSelectedRowIds([]);
    } catch (error: any) {
      const message =
        error?.response?.data ?? translate("Failed to save attendance");
      showToast([message], "error");
    } finally {
      setLoading(false);
    }
  };

  const onDelete = async (row: any) => {
    try {
      setLoading(true);
      await DeleteService(
        `/api/HumanResource/EmployeeDailyAttendance/v3/${row.id}`,
      );
      showToast([translate("Record deleted successfully")], "error");
      await loadAttendance();
    } catch (error: any) {
      showToast([translate("Failed to delete record")], "error");
    } finally {
      setLoading(false);
    }
  };

  const resetFetchFilters = () => {
    setFetchFilters({
      fromDate: dayjs().startOf("month"),
      tillDate: dayjs(),
      employeeId: 0,
      employeeCode: "",
      employeeName: "",
      departmentId: 0,
      departmentName: "",
      designationId: 0,
      designationName: "",
    });
  };

  const renderForm = (
    formData: any,
    setFormData: any,
    _mode: "new" | "edit" | "view",
    isReadOnly: boolean = false,
    isSubmitted: boolean = false,
  ) => {
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
                    required
                    value={formData.employeeName || formData.employeeCode || ""}
                    placeholder={translate("Select Employee")}
                    onSearchClick={() => setEmployeeLookupOpen(true)}
                    error={isSubmitted && !formData.employeeId}
                    disabled={isReadOnly}
                    options={employees}
                    getOptionLabel={(opt: any) =>
                      opt?.employeeName ||
                      `${opt?.firstName || ""} ${opt?.lastName || ""}`.trim() ||
                      opt?.employeeCode ||
                      ""
                    }
                    onSelect={(emp: any) => {
                      setFormData({
                        ...formData,
                        employeeId: Number(emp?.id ?? emp?.employeeId ?? 0),
                        employeeCode: emp?.employeeCode || "",
                        employeeName:
                          emp?.employeeName ||
                          `${emp?.firstName || ""} ${emp?.lastName || ""}`
                            .trim()
                            .toUpperCase(),
                      });
                    }}
                    onClear={() =>
                      setFormData({
                        ...formData,
                        employeeId: 0,
                        employeeCode: "",
                        employeeName: "",
                      })
                    }
                  />
                  <EmployeeSearchModal
                    open={employeeLookupOpen}
                    onClose={() => setEmployeeLookupOpen(false)}
                    data={employees}
                    selectedValue={formData.employeeCode}
                    onSelect={(emp) => {
                      if (emp) {
                        setFormData({
                          ...formData,
                          employeeId: emp.id,
                          employeeCode: emp.employeeCode,
                          employeeName:
                            emp.employeeName ||
                            `${emp.firstName || ""} ${emp.lastName || ""}`
                              .trim()
                              .toUpperCase(),
                        });
                      }
                      setEmployeeLookupOpen(false);
                    }}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormDatePicker
                    label={translate("Attendance Date")}
                    required
                    value={formData.atnDate ? dayjs(formData.atnDate) : null}
                    disabled={isReadOnly}
                    onChange={(val) =>
                      setFormData({ ...formData, atnDate: val })
                    }
                    error={isSubmitted && !formData.atnDate}
                  />
                </Box>
              </Grid>

              {/* Attendance metrics */}
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Late Min")}
                  type="number"
                  value={formData.lateMin || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, lateMin: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Late Count")}
                  type="number"
                  value={formData.lateCount || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, lateCount: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Late Half Days")}
                  type="number"
                  value={formData.lateHalfDays || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, lateHalfDays: e.target.value })
                  }
                />
              </Grid>
              <Grid
                item
                xs={12}
                md={3}
                sx={{ display: "flex", alignItems: "center" }}
              >
                <LmsCheckbox
                  label={translate("Is Absent")}
                  checked={formData.isAbsent || false}
                  disabled={isReadOnly}
                  onChange={(checked) =>
                    setFormData({ ...formData, isAbsent: checked })
                  }
                />
              </Grid>

              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("ED Min")}
                  type="number"
                  value={formData.edMin || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, edMin: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("ED Count")}
                  type="number"
                  value={formData.edCount || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, edCount: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("ED Half Days")}
                  type="number"
                  value={formData.edHalfDays || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, edHalfDays: e.target.value })
                  }
                />
              </Grid>
              <Grid
                item
                xs={12}
                md={3}
                sx={{ display: "flex", alignItems: "center" }}
              >
                <LmsCheckbox
                  label={translate("Is Holiday")}
                  checked={formData.isHoliday || false}
                  disabled={isReadOnly}
                  onChange={(checked) =>
                    setFormData({ ...formData, isHoliday: checked })
                  }
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("OT Min")}
                  type="number"
                  value={formData.otMin || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, otMin: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("OT Count")}
                  type="number"
                  value={formData.otCount || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, otCount: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("OT Half Days")}
                  type="number"
                  value={formData.otHalfDays || 0}
                  disabled={isReadOnly}
                  onChange={(e) =>
                    setFormData({ ...formData, otHalfDays: e.target.value })
                  }
                />
              </Grid>

              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Remarks")}
                    multiline
                    rows={2}
                    value={formData.remarks || ""}
                    disabled={isReadOnly}
                    onChange={(e) =>
                      setFormData({ ...formData, remarks: e.target.value })
                    }
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>
        </Box>
      </FormThemeWrapper>
    );
  };

  return (
    <FormThemeWrapper>
      {loading && <LmsOverlayLoader />}
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
              onClick={() => loadAttendance(fetchFilters)}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                bgcolor: "var(--lms-primary)",
                transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                boxShadow: "0 2px 4px rgba(var(--lms-primary-rgb), 0.15)",
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  boxShadow: "0 2px 10px var(--lms-primary)",
                },
                borderRadius: "10px",
                px: 3,
                py: 0.8,
              }}
            >
              {translate("Fetch")}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
              onClick={() => {
                resetFetchFilters();
                loadAttendance({
                  fromDate: dayjs().startOf("month"),
                  tillDate: dayjs(),
                  employeeId: 0,
                  employeeCode: "",
                  employeeName: "",
                  departmentId: 0,
                  departmentName: "",
                  designationId: 0,
                  designationName: "",
                });
              }}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                borderRadius: "10px",
                px: 3,
                border: "1px solid var(--lms-primary)",
                borderColor: "var(--lms-primary)",
                color: "var(--lms-primary)",
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  border: "1px solid var(--lms-primary)",
                  borderColor: "var(--lms-primary)",
                  color: "#fff",
                  boxShadow: "0 2px 10px var(--lms-primary)",
                },
              }}
            >
              {translate("Clear")}
            </Button>
          </Box>
        </Box>
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <FormDatePicker
              label={translate("Start Date")}
              required
              value={fetchFilters.fromDate ? dayjs(fetchFilters.fromDate) : null}
              onChange={(val) =>
                setFetchFilters({ ...fetchFilters, fromDate: val })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormDatePicker
              label={translate("End Date")}
              required
              value={fetchFilters.tillDate ? dayjs(fetchFilters.tillDate) : null}
              onChange={(val) =>
                setFetchFilters({ ...fetchFilters, tillDate: val })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <LookupDropdown
              label={translate("Department")}
              placeholder={translate("Select Department")}
              value={fetchFilters.departmentName}
              onSearchClick={() => setDepartmentLookupOpen(true)}
              options={departments}
              getOptionLabel={(opt: any) => opt.name || ""}
              onSelect={(opt: any) =>
                setFetchFilters((prev) => ({
                  ...prev,
                  departmentId: Number(opt?.id ?? 0),
                  departmentName: opt?.name || "",
                }))
              }
              onClear={() =>
                setFetchFilters((prev) => ({
                  ...prev,
                  departmentId: 0,
                  departmentName: "",
                }))
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <LookupDropdown
              label={translate("Designation")}
              placeholder={translate("Select Designation")}
              value={fetchFilters.designationName}
              onSearchClick={() => setDesignationLookupOpen(true)}
              options={designations}
              getOptionLabel={(opt: any) => opt.name || ""}
              onSelect={(opt: any) =>
                setFetchFilters((prev) => ({
                  ...prev,
                  designationId: Number(opt?.id ?? 0),
                  designationName: opt?.name || "",
                }))
              }
              onClear={() =>
                setFetchFilters((prev) => ({
                  ...prev,
                  designationId: 0,
                  designationName: "",
                }))
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <LookupDropdown
              label={translate("Employee")}
              placeholder={translate("Select Employee")}
              value={fetchFilters.employeeName || fetchFilters.employeeCode}
              onSearchClick={() => setFetchEmployeeLookupOpen(true)}
              options={employees}
              getOptionLabel={(opt: any) =>
                opt?.employeeName ||
                `${opt?.firstName || ""} ${opt?.lastName || ""}`.trim() ||
                opt?.employeeCode ||
                ""
              }
              onSelect={(emp: any) =>
                setFetchFilters((prev) => ({
                  ...prev,
                  employeeId: Number(emp?.id ?? emp?.employeeId ?? 0),
                  employeeCode: emp?.employeeCode || "",
                  employeeName:
                    emp?.employeeName ||
                    `${emp?.firstName || ""} ${emp?.lastName || ""}`
                      .trim()
                      .toUpperCase(),
                }))
              }
              onClear={() =>
                setFetchFilters((prev) => ({
                  ...prev,
                  employeeId: 0,
                  employeeCode: "",
                  employeeName: "",
                }))
              }
            />
          </Grid>
        </Grid>
      </Box>
      <SchemaTable
        title="Employee Daily Attendance"
        addButtonLabel="New"
        breadcrumbPath=""
        columns={columns}
        rows={rows}
        initialFormData={initialFormData}
        onSave={handleSave}
        onDelete={onDelete}
        renderForm={renderForm}
        showModalActions={true}
        actions={false}
        actionsPosition="right"
        defaultLatestFirst={false}
        checkboxSelection
        selectedIds={selectedRowIds}
        onSelectionChange={(ids) =>
          setSelectedRowIds(ids.map((id) => Number(id)).filter(Number.isFinite))
        }
        showAddButton={false}
        showEditAction={false}
        showDeleteAction={false}
        showViewAction={false}
        extraToolbarItems={
          <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
            <Button
              variant="outlined"
              startIcon={<SaveIcon sx={{ fontSize: "1.1rem" }} />}
              onClick={handleBulkSave}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                borderRadius: "10px",
                px: 3,
                border: "1px solid var(--lms-primary)",
                borderColor: "var(--lms-primary)",
                color: "var(--lms-primary)",
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  border: "1px solid var(--lms-primary)",
                  borderColor: "var(--lms-primary)",
                  color: "#fff",
                  boxShadow: "0 2px 10px var(--lms-primary)",
                },
              }}
            >
              {translate("Save")}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
              onClick={() => {
                resetFetchFilters();
                loadAttendance({
                  fromDate: dayjs().startOf("month"),
                  tillDate: dayjs(),
                  employeeId: 0,
                  employeeCode: "",
                  employeeName: "",
                  departmentId: 0,
                  departmentName: "",
                  designationId: 0,
                  designationName: "",
                });
              }}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                borderRadius: "10px",
                px: 3,
                border: "1px solid var(--lms-primary)",
                borderColor: "var(--lms-primary)",
                color: "var(--lms-primary)",
                transition: "all 0.2s ease",
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  border: "1px solid var(--lms-primary)",
                  borderColor: "var(--lms-primary)",
                  color: "#fff",
                  boxShadow: "0 2px 10px var(--lms-primary)",
                },
              }}
            >
              {translate("Clear")}
            </Button>
          </Box>
        }
        mandatoryFields={[
          { key: "employeeId", label: "Employee" },
          { key: "atnDate", label: "Attendance Date" },
        ]}
      />

      <EmployeeSearchModal
        open={fetchEmployeeLookupOpen}
        onClose={() => setFetchEmployeeLookupOpen(false)}
        data={employees}
        selectedValue={fetchFilters.employeeCode}
        onSelect={(emp) => {
          if (emp) {
            setFetchFilters({
              ...fetchFilters,
              employeeId: emp.id,
              employeeCode: emp.employeeCode,
              employeeName:
                emp.employeeName ||
                `${emp.firstName || ""} ${emp.lastName || ""}`
                  .trim()
                  .toUpperCase(),
            });
          }
          setFetchEmployeeLookupOpen(false);
        }}
      />

      <DepartmentSearchModal
        open={departmentLookupOpen}
        onClose={() => setDepartmentLookupOpen(false)}
        data={departments}
        selectedValue={fetchFilters.departmentName}
        onSelect={(row) => {
          setFetchFilters((prev) => ({
            ...prev,
            departmentId: Number(row?.id ?? 0),
            departmentName: row?.name || "",
          }));
          setDepartmentLookupOpen(false);
        }}
      />

      <DesignationSearchModal
        open={designationLookupOpen}
        onClose={() => setDesignationLookupOpen(false)}
        data={designations}
        selectedValue={fetchFilters.designationName}
        onSelect={(row) => {
          setFetchFilters((prev) => ({
            ...prev,
            designationId: Number(row?.id ?? 0),
            designationName: row?.name || "",
          }));
          setDesignationLookupOpen(false);
        }}
      />

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

export default EmployeeDailyAttendance;
