import { FormInput } from "@/components/FormInput";
import React, { useEffect, useMemo, useState } from "react";
import { Box, Typography, Button, MenuItem, Grid } from "@mui/material";
import {
  Save as SaveIcon,
  Refresh as RefreshIcon,
  Delete as DeleteIcon,
  Assessment as AssessmentIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { PayrollRegisterSearchModal } from "@/components/lookups/PayrollRegisterSearchModal";
import { PeriodSearchModal } from "@/components/lookups/PeriodSearchModal";
import { DepartmentSearchModal } from "@/components/lookups/DepartmentSearchModal";
import { DesignationSearchModal } from "@/components/lookups/DesignationSearchModal";
import { ReportsPage, type ReportSectionData } from "@/pages/ReportsPage";
import { GetService, PostService, DeleteService } from "@/constants/globalItems";
import {
  getViewableReportMenuItems,
  normalizeMenuName,
  useHiddenMenuPermissions,
} from "@/hooks/useHiddenMenuPermissions";

const mapPeriodDetail = (item: any) => ({
  ...item,
  id: Number(item.id ?? item.Id ?? 0),
  payPeriodId: item.payPeriodId ?? item.PayPeriodId ?? null,
  periodText: item.periodText ?? item.PeriodText ?? item.periodstxt ?? item.Periodstxt ?? "",
  year:
    item.year ??
    item.Year ??
    (item.startDate || item.StartDate
      ? new Date(item.startDate ?? item.StartDate).getFullYear()
      : ""),
  month:
    item.month ??
    item.Month ??
    (item.startDate || item.StartDate
      ? new Date(item.startDate ?? item.StartDate).toLocaleString("default", { month: "long" })
      : ""),
  startDate: item.startDate ?? item.StartDate ?? null,
  endDate: item.endDate ?? item.EndDate ?? null,
  noOfDays: item.noOfDays ?? item.NoOfDays ?? item.numberOfDays ?? null,
  numberOfDays: item.numberOfDays ?? item.noOfDays ?? item.NoOfDays ?? null,
  disbDate: item.disbDate ?? item.DisbDate ?? item.disbursementDate ?? null,
  disbursementDate: item.disbursementDate ?? item.disbDate ?? item.DisbDate ?? null,
});

const EmployeeMonthlyAttendance: React.FC = () => {
  const { translate } = useTranslation();
  const { visibleMenuCardGroups } = useHiddenMenuPermissions();
  const [backendRows, setBackendRows] = useState<any[]>([]);
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [exportType, setExportType] = useState("None");
  const [viewMode, setViewMode] = useState<"list" | "report">("list");
  const [loading, setLoading] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");

  const [payrollRegisters, setPayrollRegisters] = useState<any[]>([]);
  const [payrollPeriodDetails, setPayrollPeriodDetails] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);
  const [employeeLookupOpen, setEmployeeLookupOpen] = useState(false);
  const [payrollRegisterLookupOpen, setPayrollRegisterLookupOpen] = useState(false);
  const [periodLookupOpen, setPeriodLookupOpen] = useState(false);
  const [departmentLookupOpen, setDepartmentLookupOpen] = useState(false);
  const [designationLookupOpen, setDesignationLookupOpen] = useState(false);

  const [fetchFilters, setFetchFilters] = useState({
    registerId: 0,
    registerName: "",
    periodDetailId: 0,
    periodName: "",
    departmentId: 0,
    departmentName: "",
    designationId: 0,
    designationName: "",
    employeeId: 0,
    employeeCode: "",
    employeeName: "",
  });

  const showToast = (
    messages: string[],
    severity: "success" | "error" | "info" | "warning",
  ) => {
    setToasterMessages(messages);
    setToasterSeverity(severity);
    setShowToaster(true);
  };

  const loadLookups = async () => {
    try {
      setLoading(true);
      const [deptResp, desigResp, registerResp, employeeResp] =
        await Promise.all([
          GetService("/api/HumanResource/Lookups/v3/departments"),
          GetService("/api/HumanResource/Lookups/v3/designations"),
          GetService("/api/HumanResource/PayrollRegister/v3"),
          GetService("/api/HumanResource/EmployeeAttendance/v3/employees"),
        ]);

      const departmentItems =
        deptResp?.data?.items ?? deptResp?.data?.Items ?? [];
      setDepartments(
        (departmentItems || []).map((item: any) => ({
          id: Number(item.id ?? item.departmentId ?? item.depid ?? 0),
          code: item.code ?? "",
          name: item.departmentName ?? item.department ?? item.depstxt ?? item.name ?? "",
          description: item.description ?? "",
        })),
      );

      const designationItems =
        desigResp?.data?.items ?? desigResp?.data?.Items ?? [];
      setDesignations(
        (designationItems || []).map((item: any) => ({
          id: Number(item.id ?? item.designationId ?? 0),
          code: item.code ?? item.designationCode ?? "",
          name: item.designationName ?? item.designation ?? item.stxt ?? item.name ?? "",
          description: item.description ?? "",
          account: item.account ?? "",
        })),
      );

      setPayrollRegisters(
        (registerResp?.data ?? []).map((item: any) => ({
          id: Number(item.id ?? 0),
          code: item.code ?? "",
          name: item.stxt ?? item.name ?? "",
          payrollSetup: item.stxt ?? item.name ?? "",
          description: item.ltxt ?? item.description ?? "",
        })),
      );

      setEmployees(Array.isArray(employeeResp?.data) ? employeeResp.data : []);
    } catch (error: any) {
      showToast([translate("Failed to load lookup data")], "error");
    } finally {
      setLoading(false);
    }
  };

  const loadPayrollPeriodDetails = async (registerId: number) => {
    if (!registerId) {
      setPayrollPeriodDetails([]);
      return;
    }

    try {
      setLoading(true);
      const resp: any = await GetService(
        `/api/HumanResource/EmployeeMonthlyAttendance/v3/pay-period-details?registerId=${registerId}`,
      );
      setPayrollPeriodDetails(Array.isArray(resp?.data) ? resp.data.map(mapPeriodDetail) : []);
    } catch (error: any) {
      setPayrollPeriodDetails([]);
      showToast([translate("Failed to load payroll periods")], "error");
    } finally {
      setLoading(false);
    }
  };

  const loadMonthlyAttendance = async (filters = fetchFilters) => {
    if (!filters.periodDetailId) {
      showToast([translate("Period is required")], "error");
      return;
    }

    try {
      setLoading(true);
      const params = new URLSearchParams();
      params.set("periodDetailId", String(filters.periodDetailId));
      if (filters.departmentId > 0)
        params.set("departmentId", String(filters.departmentId));
      if (filters.designationId > 0)
        params.set("designationId", String(filters.designationId));
      if (filters.employeeId > 0)
        params.set("employeeId", String(filters.employeeId));

      const response: any = await GetService(
        `/api/HumanResource/EmployeeMonthlyAttendance/v3/process?${params.toString()}`,
      );
      const items = response?.data ?? [];
      setBackendRows(Array.isArray(items) ? items : []);
    } catch (error: any) {
      showToast([translate("Failed to load monthly attendance")], "error");
    } finally {
      setLoading(false);
    }
  };

  const resetFetchFilters = () => {
    setFetchFilters({
      registerId: 0,
      registerName: "",
      periodDetailId: 0,
      periodName: "",
      departmentId: 0,
      departmentName: "",
      designationId: 0,
      designationName: "",
      employeeId: 0,
      employeeCode: "",
      employeeName: "",
    });
    setPayrollPeriodDetails([]);
  };

  const handleSave = async () => {
    if (!backendRows || backendRows.length === 0) {
      showToast([translate("No records to save")], "warning");
      return;
    }

    try {
      setLoading(true);
      const payload = {
        items: backendRows.map((row) => ({
          employeeId: row.employeeId,
          registerId: row.registerId,
          payPeriodId: row.payPeriodId,
          payPeriodDetailId: row.payPeriodDetailId,
          startDate: row.startDate,
          endDate: row.endDate,
          disbDate: row.disbDate,
          entityId: row.entityId,
          days: row.days,
          presentsCount: row.presentsCount,
          absentsCount: row.absentsCount,
          halfDaysCount: row.halfDaysCount,
          lateCount: row.lateCount,
          earlyCounts: row.earlyCounts,
          actualDays: row.actualDays,
          actualPresent: row.actualPresent,
          holidays: row.holidays,
          leaves: row.leaves,
        })),
      };

      await PostService(
        "/api/HumanResource/EmployeeMonthlyAttendance/v3/bulk",
        payload,
      );
      showToast([translate("Monthly attendance saved successfully")], "success");
    } catch (error: any) {
      showToast([translate("Failed to save monthly attendance")], "error");
    } finally {
      setLoading(false);
    }
  };

  const handleRevert = async () => {
    if (!selectedRowIds.length) {
      showToast([translate("Select at least one employee to revert")], "warning");
      return;
    }

    const selectedRows = backendRows.filter((row) =>
      selectedRowIds.includes(row.employeeId),
    );

    if (!selectedRows.length) {
      showToast([translate("Selected records are not available")], "error");
      return;
    }

    try {
      setLoading(true);
      const messages: string[] = [];

      for (const row of selectedRows) {
        if (!row.registerId || !row.payPeriodDetailId) continue;
        await DeleteService(
          `/api/HumanResource/EmployeeMonthlyAttendance/v3/revert?employeeId=${row.employeeId}&registerId=${row.registerId}&periodDetailId=${row.payPeriodDetailId}`,
        );
        messages.push(
          `${row.employeeName || translate("Employee")} ${translate("process deleted")}`,
        );
      }

      showToast(
        messages.length > 0 ? messages : [translate("Process reverted")],
        "success",
      );
      setSelectedRowIds([]);
    } catch (error: any) {
      showToast([translate("Failed to revert process")], "error");
    } finally {
      setLoading(false);
    }
  };

  const handleAttendanceReportFormat = () => {
    setViewMode("report");
  };

  useEffect(() => {
    loadLookups();
  }, []);

  const rows = useMemo(
    () =>
      (Array.isArray(backendRows) ? backendRows : []).map((x, index) => ({
        ...x,
        id: x.employeeId ?? x.id ?? index + 1,
        sNo: index + 1,
      })),
    [backendRows],
  );

  const attendanceReportContext = useMemo(() => {
    for (const group of visibleMenuCardGroups) {
      const reportItem = getViewableReportMenuItems(group.menuItems).find(
        (item) => normalizeMenuName(item.name) === "attendance report format",
      );

      if (reportItem) {
        return {
          category: group.moduleName,
          selectedReport: {
            name: reportItem.name,
            menuId: reportItem.menuId,
            reportId: reportItem.reportId,
            reportCode: reportItem.reportCode,
          },
          sections: [
            {
              title: group.cardName,
              id: normalizeMenuName(group.cardName).replace(/\s+/g, "-"),
              items: [
                {
                  name: reportItem.name,
                  menuId: reportItem.menuId,
                  reportId: reportItem.reportId,
                  reportCode: reportItem.reportCode,
                },
              ],
            },
          ] as ReportSectionData[],
        };
      }
    }

    return null;
  }, [visibleMenuCardGroups]);

  const canViewAttendanceReportFormat = Boolean(
    attendanceReportContext?.selectedReport.reportId ||
      attendanceReportContext?.selectedReport.reportCode,
  );

  useEffect(() => {
    if (viewMode === "report" && !canViewAttendanceReportFormat) {
      setViewMode("list");
    }
  }, [canViewAttendanceReportFormat, viewMode]);

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
    { id: "department", label: "Department", minWidth: 150 },
    { id: "designation", label: "Designation", minWidth: 150 },
    { id: "days", label: "Total Days", minWidth: 110 },
    { id: "presentsCount", label: "Working Days", minWidth: 110 },
    { id: "absentsCount", label: "Absents", minWidth: 100 },
    { id: "holidays", label: "Holidays", minWidth: 100 },
    { id: "leaves", label: "Leaves", minWidth: 100 },
    { id: "lateCount", label: "Late Count", minWidth: 100 },
    { id: "earlyCounts", label: "ED Count", minWidth: 100 },
    { id: "halfDaysCount", label: "Half Days", minWidth: 100 },
  ];

  const extraToolbarItems = (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        width: "100%",
        gap: 1.5,
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <Typography
            sx={{
              fontSize: "0.65rem",
              fontWeight: 800,
              color: "var(--lms-text-main)",
              whiteSpace: "nowrap",
            }}
          >
            EXPORT TYPE:
          </Typography>
          <FormInput
            select
            size="small"
            value={exportType}
            onChange={(e) => setExportType(e.target.value)}
            sx={{
              "& .MuiInputBase-root": {
                height: "32px",
                fontSize: "0.75rem",
                minWidth: "120px",
                borderRadius: "16px",
                bgcolor: "#fff",
                border: "1px solid #ced4da",
                "& fieldset": { display: "none" },
              },
            }}
          >
            <MenuItem value="None">None</MenuItem>
            <MenuItem value="Excel">Excel</MenuItem>
            <MenuItem value="PDF">PDF</MenuItem>
          </FormInput>
        </Box>

        <Button
          variant="outlined"
          startIcon={<SaveIcon sx={{ fontSize: "1.1rem" }} />}
          onClick={handleSave}
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
          startIcon={<DeleteIcon sx={{ fontSize: "1.1rem" }} />}
          onClick={handleRevert}
          sx={{
            textTransform: "none",
            fontWeight: 700,
            borderRadius: "10px",
            px: 3,
            border: "1px solid #d9534f",
            borderColor: "#d9534f",
            color: "#d9534f",
            transition: "all 0.2s ease",
            "&:hover": {
              bgcolor: "#d9534f",
              border: "1px solid #d9534f",
              color: "#fff",
              boxShadow: "0 2px 10px rgba(217, 83, 79, 0.4)",
            },
          }}
        >
          {translate("Delete Process")}
        </Button>

        {canViewAttendanceReportFormat && (
          <Button
            variant="outlined"
            startIcon={<AssessmentIcon sx={{ fontSize: "1.1rem" }} />}
            onClick={handleAttendanceReportFormat}
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
            {translate("Attendance Report Format")}
          </Button>
        )}

        <Button
          variant="outlined"
          startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
          onClick={() => {
            setBackendRows([]);
            setSelectedRowIds([]);
            resetFetchFilters();
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
  );

  return (
    <FormThemeWrapper>
      {loading && <LmsOverlayLoader />}
      {viewMode === "report" ? (
        <ReportsPage
          category={attendanceReportContext?.category || ""}
          sections={attendanceReportContext?.sections || []}
          initialSelectedReport={attendanceReportContext?.selectedReport || null}
          embedded
          onBack={() => setViewMode("list")}
        />
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
              onClick={() => loadMonthlyAttendance(fetchFilters)}
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
              {translate("Fetch")}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
              onClick={resetFetchFilters}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                borderRadius: "10px",
                px: 3,
                border: "1px solid var(--lms-primary)",
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
              label={translate("Payroll Register")}
              required
              placeholder={translate("Select Payroll Register")}
              value={fetchFilters.registerName}
              onSearchClick={() => setPayrollRegisterLookupOpen(true)}
              options={payrollRegisters}
              getOptionLabel={(opt: any) => opt.name || ""}
              onSelect={(opt: any) => {
                const registerId = Number(opt?.id ?? 0);
                setFetchFilters((prev) => ({
                  ...prev,
                  registerId,
                  registerName: opt?.name || "",
                  periodDetailId: 0,
                  periodName: "",
                }));
                loadPayrollPeriodDetails(registerId);
              }}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <LookupDropdown
              label={translate("Period")}
              required
              placeholder={translate("Select Period")}
              value={fetchFilters.periodName}
              onSearchClick={() => setPeriodLookupOpen(true)}
              options={payrollPeriodDetails}
              getOptionLabel={(opt: any) => opt.periodText || ""}
              onSelect={(opt: any) =>
                setFetchFilters((prev) => ({
                  ...prev,
                  periodDetailId: Number(opt?.id ?? 0),
                  periodName: opt?.periodText || "",
                }))
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
              onSearchClick={() => setEmployeeLookupOpen(true)}
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
        title="Employee Monthly Attendance"
        breadcrumbPath=""
        columns={columns}
        rows={rows}
        initialFormData={{}}
        onSave={() => {}}
        onDelete={() => {}}
        renderForm={() => null}
        extraToolbarItems={extraToolbarItems}
        showModalActions={false}
        actions={false}
        checkboxSelection
        selectedIds={selectedRowIds}
        onSelectionChange={(ids) =>
          setSelectedRowIds((ids || []).map((id) => Number(id)))
        }
        exportType={exportType}
        showAddButton={false}
      />

      <EmployeeSearchModal
        open={employeeLookupOpen}
        onClose={() => setEmployeeLookupOpen(false)}
        data={employees}
        selectedValue={fetchFilters.employeeCode}
        onSelect={(emp) => {
          if (emp) {
            setFetchFilters((prev) => ({
              ...prev,
              employeeId: emp.id,
              employeeCode: emp.employeeCode,
              employeeName:
                emp.employeeName ||
                `${emp.firstName || ""} ${emp.lastName || ""}`
                  .trim()
                  .toUpperCase(),
            }));
          }
          setEmployeeLookupOpen(false);
        }}
      />

      <PayrollRegisterSearchModal
        open={payrollRegisterLookupOpen}
        onClose={() => setPayrollRegisterLookupOpen(false)}
        data={payrollRegisters}
        selectedValue={fetchFilters.registerName}
        onSelect={(row) => {
          const registerId = Number(row?.id ?? 0);
          setFetchFilters((prev) => ({
            ...prev,
            registerId,
            registerName: row?.name || "",
            periodDetailId: 0,
            periodName: "",
          }));
          loadPayrollPeriodDetails(registerId);
          setPayrollRegisterLookupOpen(false);
        }}
      />

      <PeriodSearchModal
        open={periodLookupOpen}
        onClose={() => setPeriodLookupOpen(false)}
        data={payrollPeriodDetails}
        selectedValue={fetchFilters.periodName}
        onSelect={(row) => {
          setFetchFilters((prev) => ({
            ...prev,
            periodDetailId: Number(row?.id ?? 0),
            periodName: row?.periodText || "",
          }));
          setPeriodLookupOpen(false);
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
        </>
      )}
    </FormThemeWrapper>
  );
};

export default EmployeeMonthlyAttendance;
