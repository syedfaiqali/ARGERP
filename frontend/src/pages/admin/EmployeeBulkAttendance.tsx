import React, { useEffect, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
  Box,
  Typography,
  Button,
  Grid,
} from "@mui/material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { DepartmentSearchModal } from "@/components/lookups/DepartmentSearchModal";
import { DesignationSearchModal } from "@/components/lookups/DesignationSearchModal";
import { SiteSearchModal } from "@/components/lookups/SiteSearchModal";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GetService } from "@/constants/globalItems";
import {
  FilterList as FetchIcon,
} from "@mui/icons-material";
import { Modal } from "@/components/Modal";

const LegendItem = ({
  color,
  label,
  abbr,
}: {
  color: string;
  label: string;
  abbr: string;
}) => (
  <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1 }}>
    <Box
      sx={{
        width: 24,
        height: 24,
        bgcolor: color,
        borderRadius: "4px",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color: "#fff",
        fontSize: "0.75rem",
        fontWeight: 800,
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
      }}
    >
      {abbr}
    </Box>
    <Typography sx={{ fontSize: "0.85rem", fontWeight: 600, color: "var(--lms-text-main)" }}>
      <T>{label}</T>
    </Typography>
  </Box>
);

const EmployeeBulkAttendance: React.FC = () => {
  const { translate } = useTranslation();
  const [backendRows, setBackendRows] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");

  const [employeeLookupOpen, setEmployeeLookupOpen] = useState(false);
  const [departmentLookupOpen, setDepartmentLookupOpen] = useState(false);
  const [designationLookupOpen, setDesignationLookupOpen] = useState(false);
  const [siteLookupOpen, setSiteLookupOpen] = useState(false);

  const [employees, setEmployees] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [sites, setSites] = useState<any[]>([]);

  const [isFetchModalOpen, setIsFetchModalOpen] = useState(false);

  const [fetchFilters, setFetchFilters] = useState<{
    fromDate: Dayjs | null;
    tillDate: Dayjs | null;
    employeeId: number;
    employeeCode: string;
    employeeName: string;
    siteId: number;
    siteName: string;
    departmentId: number;
    departmentName: string;
    designationId: number;
    designationName: string;
  }>({
    fromDate: dayjs(),
    tillDate: dayjs(),
    employeeId: 0,
    employeeCode: "",
    employeeName: "",
    siteId: 0,
    siteName: "",
    departmentId: 0,
    departmentName: "",
    designationId: 0,
    designationName: "",
  });

  const [appliedFilters, setAppliedFilters] = useState(fetchFilters);

  const showToast = (
    messages: string[],
    severity: "success" | "error" | "info" | "warning",
  ) => {
    setToasterMessages(messages);
    setToasterSeverity(severity);
    setShowToaster(true);
  };

  const loadLookups = async () => {
    const getWithFallback = async (urls: string[]) => {
      for (const url of urls) {
        try {
          return await GetService(url);
        } catch {
          // try next endpoint
        }
      }
      throw new Error("All lookup endpoints failed");
    };

    const toArray = (resp: any) =>
      resp?.data?.items ?? resp?.data?.Items ?? resp?.data?.data ?? resp?.data ?? [];

    try {
      setLoading(true);
      const [empResp, deptResp, desigResp] = await Promise.allSettled([
        getWithFallback([
          "/api/HumanResource/EmployeeAttendance/v3/employees",
          "/api/HumanResource/EmployeeProfile/v3?pageNumber=1&pageSize=1000",
        ]),
        getWithFallback([
          "/api/HumanResource/Lookups/v3/departments",
          "/api/HumanResource/Lookups/v3/departments",
        ]),
        getWithFallback([
          "/api/HumanResource/Lookups/v3/designations",
          "/api/HumanResource/Lookups/v3/designations",
        ]),
      ]);

      const empItems = empResp.status === "fulfilled" ? toArray(empResp.value) : [];
      setEmployees(Array.isArray(empItems) ? empItems : []);

      const deptItems = deptResp.status === "fulfilled" ? toArray(deptResp.value) : [];
      setDepartments(
        (deptItems || []).map((item: any) => ({
          id: Number(item.id ?? item.departmentId ?? item.departid ?? 0),
          code: item.code ?? "",
          name: item.departmentName ?? item.department ?? item.stxt ?? item.name ?? "",
          description: item.description ?? "",
        }))
      );

      const desigItems = desigResp.status === "fulfilled" ? toArray(desigResp.value) : [];
      setDesignations(
        (desigItems || []).map((item: any) => ({
          id: Number(item.id ?? item.designationId ?? 0),
          code: item.code ?? item.designationCode ?? "",
          name: item.designationName ?? item.designation ?? item.stxt ?? item.name ?? "",
          description: item.description ?? "",
          account: item.account ?? "",
        }))
      );

      // Load site lookup from Site APIs first, fallback to employee-derived sites.
      const siteMap = new Map<number, any>();
      try {
        const siteResp: any = await getWithFallback([
          "/api/HumanResource/Lookups/v3/sites",
        ]);
        const siteItems = toArray(siteResp);
        (Array.isArray(siteItems) ? siteItems : []).forEach((item: any) => {
          const id = Number(item.siteid ?? item.siteId ?? item.id ?? 0);
          const name = item.stxt ?? item.siteName ?? item.name ?? "";
          if (id > 0 && name && !siteMap.has(id)) {
            siteMap.set(id, {
              id,
              code: item.code ?? "",
              stxt: name,
              sitetypename: item.sitetypename ?? item.siteTypeName ?? "",
              siteforname: item.siteforname ?? item.siteForName ?? "",
            });
          }
        });
      } catch {
        // fallback below
      }

      if (siteMap.size === 0) {
        (Array.isArray(empItems) ? empItems : []).forEach((item: any) => {
          const id = Number(item.siteid ?? item.siteId ?? item.id_site ?? 0);
          const name = item.stxt ?? item.siteName ?? item.site ?? item.name ?? "";
          if (id > 0 && name && !siteMap.has(id)) {
            siteMap.set(id, {
              id,
              code: item.code ?? "",
              stxt: name,
              sitetypename: item.sitetypename ?? item.siteTypeName ?? "",
              siteforname: item.siteforname ?? item.siteForName ?? "",
            });
          }
        });
      }

      setSites(Array.from(siteMap.values()));
    } catch (error: any) {
      showToast([translate("Failed to load lookup data")], "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadLookups();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleFetchData = async () => {
    if (!fetchFilters.fromDate || !fetchFilters.tillDate) {
      showToast([translate("Start Date and End Date are required")], "error");
      return;
    }

    try {
      setLoading(true);
      setAppliedFilters(fetchFilters);

      const params = new URLSearchParams();
      params.set("fromDate", dayjs(fetchFilters.fromDate).format("YYYY-MM-DD"));
      params.set("tillDate", dayjs(fetchFilters.tillDate).format("YYYY-MM-DD"));
      if (fetchFilters.employeeId > 0) {
        params.set("employeeId", String(fetchFilters.employeeId));
      }
      if (fetchFilters.departmentId > 0) {
        params.set("departmentId", String(fetchFilters.departmentId));
      }
      if (fetchFilters.designationId > 0) {
        params.set("designationId", String(fetchFilters.designationId));
      }
      if (fetchFilters.siteId > 0) {
        params.set("siteId", String(fetchFilters.siteId));
      }

      const response: any = await GetService(
        `/api/HumanResource/EmployeeDailyAttendance/v3?${params.toString()}`,
      );
      const items = Array.isArray(response?.data) ? response.data : [];

      const fromLabel = dayjs(fetchFilters.fromDate).format("DD-MMM");
      const employeeMeta = new Map<number, any>();
      (employees || []).forEach((emp: any) => {
        const id = Number(emp?.id ?? emp?.employeeId ?? 0);
        if (id > 0) employeeMeta.set(id, emp);
      });

      const grouped = new Map<number, any>();
      items.forEach((row: any) => {
        const empId = Number(row?.employeeId ?? 0);
        if (!empId) return;

        const meta = employeeMeta.get(empId) || {};
        const employeeCode = row?.employeeCode || meta?.employeeCode || "--";
        const employeeName =
          row?.employeeName ||
          meta?.employeeName ||
          `${meta?.firstName || ""} ${meta?.lastName || ""}`.trim() ||
          "--";
        const department =
          row?.department || meta?.department || meta?.departmentName || "--";
        const designation =
          row?.designation || meta?.designation || meta?.designationName || "--";
        const rowDate = row?.atnDate || row?.attendanceDate;
        const rowLabel = rowDate ? dayjs(rowDate).format("DD-MMM") : "";
        const mood = row?.isHoliday ? "H" : row?.isAbsent ? "A" : "P";

        if (!grouped.has(empId)) {
          grouped.set(empId, {
            id: empId,
            employeeId: empId,
            employeeCode,
            employee: employeeName,
            department,
            designation,
            dynamicDate: rowLabel === fromLabel ? mood : "SE",
            days: 0,
            present: 0,
            absents: 0,
            leaves: 0,
            holidays: 0,
          });
        }

        const current = grouped.get(empId);
        current.days += 1;
        if (mood === "P") current.present += 1;
        if (mood === "A") current.absents += 1;
        if (mood === "H") current.holidays += 1;
        if (rowLabel === fromLabel) current.dynamicDate = mood;
      });

      const aggregatedRows = Array.from(grouped.values()).map((row, index) => ({
        ...row,
        sNo: index + 1,
      }));
      setBackendRows(aggregatedRows);
      setIsFetchModalOpen(false);
    } catch (error: any) {
      showToast([translate("Failed to generate attendance")], "error");
      setBackendRows([]);
    } finally {
      setLoading(false);
    }
  };

  const dynamicDateColumnLabel = appliedFilters.fromDate
    ? appliedFilters.fromDate.format("DD-MMM")
    : dayjs().format("DD-MMM");

  const columns: Column[] = [
    { id: "sNo", label: "S.No", minWidth: 60 },
    { id: "employee", label: "Employee", minWidth: 200 },
    { id: "department", label: "Department", minWidth: 150 },
    { id: "designation", label: "Designation", minWidth: 150 },
    { 
      id: "dynamicDate", 
      label: dynamicDateColumnLabel, 
      minWidth: 100,
      align: "center",
      format: (val) => (
        <Box sx={{ 
          bgcolor: "#E1B05D", 
          color: "#fff", 
          borderRadius: "4px", 
          px: 1.5, 
          py: 0.5, 
          fontWeight: 800, 
          fontSize: "0.75rem",
          display: "inline-block" 
        }}>
          {val || "SE"}
        </Box>
      )
    },
    { id: "days", label: "Days", minWidth: 80, align: "center", format: () => "1" },
    { id: "present", label: "Present", minWidth: 80, align: "center", format: () => "0" },
    { id: "absents", label: "Absents", minWidth: 80, align: "center", format: () => "0" },
    { id: "leaves", label: "Leaves", minWidth: 80, align: "center", format: () => "0" },
    { id: "holidays", label: "Holidays", minWidth: 80, align: "center", format: () => "0" },
  ];

  const renderTopFilterSummary = () => (
    <Box sx={{ 
      p: 2, 
      mb: 2, 
      borderRadius: "12px",
      color: "#fff",
      boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
      position: "relative",
      overflow: "hidden"
    }}>
      {/* Background Gradient */}
      <Box sx={{
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        background: "linear-gradient(135deg, rgba(var(--lms-primary-rgb), 0.9) 0%, rgba(var(--lms-primary-rgb), 0.7) 100%)",
        zIndex: 0
      }} />

      <Box sx={{ position: "relative", zIndex: 1 }}>
        <Grid container spacing={4} alignItems="center">
          <Grid item xs={12} md={7}>
            <Grid container rowSpacing={1.5} columnSpacing={2}>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>Start Date: {appliedFilters.fromDate ? appliedFilters.fromDate.format("YYYY-MM-DD") : "--"}</Typography>
              </Grid>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>End Date: {appliedFilters.tillDate ? appliedFilters.tillDate.format("YYYY-MM-DD") : "--"}</Typography>
              </Grid>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>Designation: {appliedFilters.designationName || "--"}</Typography>
              </Grid>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>Site: {appliedFilters.siteName || "--"}</Typography>
              </Grid>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>Department: {appliedFilters.departmentName || "--"}</Typography>
              </Grid>
              <Grid item xs={6} md={4}>
                <Typography sx={{ fontSize: "0.85rem", opacity: 0.9 }}>Employee: {appliedFilters.employeeName || "--"}</Typography>
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={12} md={5}>
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 2, justifyContent: "flex-end" }}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#7DBD5B", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>P</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>Present</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#F5938D", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>A</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>Absent</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#F3A953", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>H</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>Holiday</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#F6EC63", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>L</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>Leave</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#E1B05D", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>SE</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>ScheduleNotExist</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                 <Box sx={{ width: 16, height: 16, bgcolor: "#FFD54F", borderRadius: "3px", color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: "0.6rem", fontWeight: "bold" }}>PE</Box>
                 <Typography sx={{ fontSize: "0.75rem", fontWeight: 600 }}>Posted Entry</Typography>
              </Box>
            </Box>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );

  const extraToolbarItems = (
    <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
      <Button
        variant="contained"
        disableElevation
        startIcon={<FetchIcon />}
        onClick={() => setIsFetchModalOpen(true)}
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
          "&:active": {
            transform: "translateY(0)",
          },
          borderRadius: "10px",
          px: 3,
          py: 0.8,
        }}
      >
        {translate("Fetch")}
      </Button>
    </Box>
  );

  return (
    <FormThemeWrapper>
      {loading && <LmsOverlayLoader />}
      
      {renderTopFilterSummary()}

      <SchemaTable
        title="Employee Bulk Attendance"
        breadcrumbPath=""
        columns={columns}
        rows={backendRows}
        initialFormData={{}}
        onSave={async () => true}
        onDelete={async () => {}}
        renderForm={() => null}
        extraToolbarItems={extraToolbarItems}
        showModalActions={false}
        actions={false}
        showAddButton={false}
      />

      <Modal
        open={isFetchModalOpen}
        onClose={() => setIsFetchModalOpen(false)}
        title={translate("Fetch")}
        maxWidth="lg"
        saveLabel={translate("Generate")}
        saveAndCloseLabel={translate("Generate")}
        showSaveButton={false}
        onReset={() => {
          const resetFilters = {
            fromDate: dayjs(),
            tillDate: dayjs(),
            employeeId: 0,
            employeeCode: "",
            employeeName: "",
            siteId: 0,
            siteName: "",
            departmentId: 0,
            departmentName: "",
            designationId: 0,
            designationName: "",
          };
          setFetchFilters(resetFilters);
        }}
        onSave={handleFetchData}
      >
        <Box sx={{ mb: 2 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 1.5,
              mb: 3,
              pb: 1,
              borderBottom: "1px solid #F1F5F9",
            }}
          >
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
          <Grid container spacing={4}>
            <Grid item xs={12} md={8}>
              <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                  <FormDatePicker
                    label={translate("Start Date")}
                    required
                    value={fetchFilters.fromDate ? dayjs(fetchFilters.fromDate) : null}
                    onChange={(val) => setFetchFilters({ ...fetchFilters, fromDate: val })}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <FormDatePicker
                    label={translate("End Date")}
                    required
                    value={fetchFilters.tillDate ? dayjs(fetchFilters.tillDate) : null}
                    onChange={(val) => setFetchFilters({ ...fetchFilters, tillDate: val })}
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <LookupDropdown
                    label={translate("Site")}
                    placeholder={translate("Select Site")}
                    value={fetchFilters.siteName}
                    onSearchClick={() => setSiteLookupOpen(true)}
                    options={sites}
                    getOptionLabel={(opt: any) =>
                      opt.stxt || opt.siteName || opt.name || ""
                    }
                    onSelect={(opt: any) =>
                      setFetchFilters((prev) => ({
                        ...prev,
                        siteId: Number(opt?.id ?? 0),
                        siteName: opt?.stxt || opt?.siteName || opt?.name || "",
                      }))
                    }
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <LookupDropdown
                    label={translate("Department")}
                    placeholder={translate("Select Department")}
                    value={fetchFilters.departmentName}
                    onSearchClick={() => setDepartmentLookupOpen(true)}
                    options={departments}
                    getOptionLabel={(opt: any) =>
                      opt.name || opt.stxt || opt.departmentName || ""
                    }
                    onSelect={(opt: any) =>
                      setFetchFilters((prev) => ({
                        ...prev,
                        departmentId: Number(opt?.id ?? 0),
                        departmentName:
                          opt?.name || opt?.stxt || opt?.departmentName || "",
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
                <Grid item xs={12} md={6}>
                  <LookupDropdown
                    label={translate("Designation")}
                    placeholder={translate("Select Designation")}
                    value={fetchFilters.designationName}
                    onSearchClick={() => setDesignationLookupOpen(true)}
                    options={designations}
                    getOptionLabel={(opt: any) =>
                      opt.name || opt.stxt || opt.designationName || ""
                    }
                    onSelect={(opt: any) =>
                      setFetchFilters((prev) => ({
                        ...prev,
                        designationId: Number(opt?.id ?? 0),
                        designationName:
                          opt?.name || opt?.stxt || opt?.designationName || "",
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
                <Grid item xs={12} md={6}>
                  <LookupDropdown
                    label={translate("Employee")}
                    placeholder={translate("Select Employee")}
                    value={fetchFilters.employeeName || fetchFilters.employeeCode}
                    onSearchClick={() => setEmployeeLookupOpen(true)}
                    options={employees}
                    getOptionLabel={(opt: any) =>
                      opt.employeeName ||
                      `${opt.firstName || ""} ${opt.lastName || ""}`
                        .trim()
                        .toUpperCase() ||
                      opt.employeeCode ||
                      ""
                    }
                    onSelect={(emp: any) =>
                      setFetchFilters((prev) => ({
                        ...prev,
                        employeeId: Number(emp?.id ?? 0),
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
            </Grid>
            <Grid item xs={12} md={4}>
              <Box sx={{ 
                p: 3, 
                bgcolor: "rgba(0,0,0,0.02)", 
                borderRadius: "12px", 
                border: "1px solid #F1F5F9",
                height: "100%"
              }}>
                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <LegendItem color="#7DBD5B" label={translate("Present")} abbr="P" />
                    <LegendItem color="#F5938D" label={translate("Absent")} abbr="A" />
                    <LegendItem color="#F3A953" label={translate("Holiday")} abbr="H" />
                  </Grid>
                  <Grid item xs={6}>
                    <LegendItem color="#F6EC63" label={translate("Leave")} abbr="L" />
                    <LegendItem color="#FFD54F" label={translate("Posted Entry")} abbr="PE" />
                    <LegendItem color="#E1B05D" label={translate("Schedule Not Exist")} abbr="SE" />
                  </Grid>
                </Grid>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </Modal>

      <EmployeeSearchModal
        open={employeeLookupOpen}
        onClose={() => setEmployeeLookupOpen(false)}
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
          setEmployeeLookupOpen(false);
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
      <SiteSearchModal
        open={siteLookupOpen}
        onClose={() => setSiteLookupOpen(false)}
        data={sites}
        selectedValue={fetchFilters.siteName}
        onSelect={(row) => {
          setFetchFilters((prev) => ({
            ...prev,
            siteId: Number(row?.id ?? 0),
            siteName: row?.stxt || row?.name || "",
          }));
          setSiteLookupOpen(false);
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

export default EmployeeBulkAttendance;
