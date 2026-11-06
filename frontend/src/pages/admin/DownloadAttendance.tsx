import React, { useEffect, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
  Box,
  Typography,
  Grid,
  Button,
} from "@mui/material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LmsSelect } from "@/components/LmsSelect";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GetService } from "@/constants/globalItems";
import {
  FilterList as FetchIcon,
  Refresh as RefreshIcon,
} from "@mui/icons-material";

const DownloadAttendance: React.FC = () => {
  const { translate } = useTranslation();
  const [backendRows, setBackendRows] = useState<any[]>([]);
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [loading, setLoading] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");

  const [fetchEmployeeLookupOpen, setFetchEmployeeLookupOpen] = useState(false);
  const [employees, setEmployees] = useState<any[]>([]);
  const [downloadFromOptions, setDownloadFromOptions] = useState<
    { label: string; value: string }[]
  >([]);
  const [fileTypeOptions, setFileTypeOptions] = useState<
    { label: string; value: string }[]
  >([]);

  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const [fetchFilters, setFetchFilters] = useState<{
    downloadFrom: string;
    employeeId: number;
    employeeCode: string;
    employeeName: string;
    fromDate: Dayjs | null;
    tillDate: Dayjs | null;
    fileType: string;
  }>({
    downloadFrom: "Manual",
    employeeId: 0,
    employeeCode: "",
    employeeName: "",
    fromDate: dayjs(),
    tillDate: dayjs(),
    fileType: "Excel File"
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
      setLoading(true);
      const response: any = await GetService(
        "/api/HumanResource/EmployeeAttendance/v3/employees",
      );
      const items = response?.data ?? [];
      setEmployees(Array.isArray(items) ? items : []);
    } catch (error: any) {
      showToast([translate("Failed to load employees")], "error");
    } finally {
      setLoading(false);
    }
  };

  const loadSelectLookups = async () => {
    const mapLovOptions = (items: any[]) =>
      (Array.isArray(items) ? items : [])
        .map((item: any) => ({
          label: item?.name || item?.stxt || item?.shortName || item?.code || "",
          value: String(
            item?.code ||
              item?.id ||
              item?.value ||
              item?.stxt ||
              item?.name ||
              "",
          ),
        }))
        .filter((x) => x.label);

    const getLovWithFallback = async (vtype: string) => {
      try {
        setLoading(true);
        return await GetService(`/api/HumanResource/Lov/v3/${vtype}`);
      } catch {
        return { data: [] };
      }
    };

    try {
      // Legacy flow: ATFRM = Download From, FTYPE = File Type
      const [downloadFromResp, fileTypeResp]: any = await Promise.all([
        getLovWithFallback("ATFRM"),
        getLovWithFallback("FTYPE"),
      ]);

      const fromOptions = mapLovOptions(
        downloadFromResp?.data?.items ??
          downloadFromResp?.data?.Items ??
          downloadFromResp?.data?.data ??
          downloadFromResp?.data,
      );
      const typeOptions = mapLovOptions(
        fileTypeResp?.data?.items ??
          fileTypeResp?.data?.Items ??
          fileTypeResp?.data?.data ??
          fileTypeResp?.data,
      );

      setDownloadFromOptions(fromOptions);
      setFileTypeOptions(typeOptions);

      if (fromOptions.length > 0) {
        setFetchFilters((prev) => ({
          ...prev,
          downloadFrom: fromOptions.some((o) => o.value === prev.downloadFrom)
            ? prev.downloadFrom
            : fromOptions[0].value,
        }));
      }
      if (typeOptions.length > 0) {
        setFetchFilters((prev) => ({
          ...prev,
          fileType: typeOptions.some((o) => o.value === prev.fileType)
            ? prev.fileType
            : typeOptions[0].value,
        }));
      }
    } catch (error: any) {
      showToast([translate("Failed to load dropdown data")], "error");
    } finally {
      setLoading(false);
    }
  };

  const loadAttendanceRows = async (filters = fetchFilters) => {
    try {
      setLoading(true);
      const params = new URLSearchParams();

      if (filters.employeeId > 0) {
        params.set("employeeId", String(filters.employeeId));
      }
      if (filters.fromDate) {
        params.set("fromDate", dayjs(filters.fromDate).format("YYYY-MM-DD"));
      }
      if (filters.tillDate) {
        params.set("tillDate", dayjs(filters.tillDate).format("YYYY-MM-DD"));
      }

      const response: any = await GetService(
        `/api/HumanResource/EmployeeAttendance/v3${params.toString() ? `?${params.toString()}` : ""}`,
      );

      const items = response?.data?.items ?? response?.data ?? [];
      const mappedRows = (Array.isArray(items) ? items : []).map(
        (item: any, index: number) => ({
          id: item.id ?? index + 1,
          sNo: index + 1,
          employeeId: item.employeeCode || item.employeeId || "--",
          employee: item.employeeName || "--",
          department: item.department || "--",
          designation: item.designation || "--",
          attendanceDate: item.attendanceDate
            ? dayjs(item.attendanceDate).format("DD/MM/YYYY")
            : "--",
          checkIn: item.attendanceTime
            ? dayjs(item.attendanceTime).format("hh:mm A")
            : "--",
          checkOut: item.checkOutTime
            ? dayjs(item.checkOutTime).format("hh:mm A")
            : "--",
          remarks: item.remarks || "--",
          status: item.activity || "--",
        }),
      );

      setBackendRows(mappedRows);
    } catch (error: any) {
      showToast([translate("Failed to load attendance records")], "error");
      setBackendRows([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEmployees();
    loadSelectLookups();
    loadAttendanceRows();
  }, []);

  const columns: Column[] = [
    {
      id: "select",
      label: "",
      minWidth: 40,
      format: (_value, row) => (
        <LmsCheckbox
          checked={selectedRowIds.includes(row.id)}
          onChange={(checked) =>
            setSelectedRowIds((prev) =>
              checked
                ? prev.includes(row.id)
                  ? prev
                  : [...prev, row.id]
                : prev.filter((id) => id !== row.id),
            )
          }
          hideLabel
        />
      ),
    },
    { id: "sNo", label: "S.No", minWidth: 60 },
    {
      id: "employeeId",
      label: "Employee ID",
      minWidth: 140,
      format: (value) => (
        <Typography sx={{ fontWeight: 700, fontSize: "0.85rem" }}>
          {value}
        </Typography>
      ),
    },
    { id: "employee", label: "Employee", minWidth: 180 },
    { id: "department", label: "Department", minWidth: 150 },
    { id: "designation", label: "Designation", minWidth: 150 },
    { id: "attendanceDate", label: "Attendance Date", minWidth: 140 },
    { id: "checkIn", label: "Check In", minWidth: 110 },
    { id: "checkOut", label: "Check Out", minWidth: 110 },
    { id: "remarks", label: "Remarks", minWidth: 180 },
    {
      id: "status",
      label: "Status",
      minWidth: 130,
      type: "status",
    },
  ];

  const resetFetchFilters = () => {
    setFetchFilters({
      downloadFrom: downloadFromOptions[0]?.value || "",
      employeeId: 0,
      employeeCode: "",
      employeeName: "",
      fromDate: dayjs(),
      tillDate: dayjs(),
      fileType: fileTypeOptions[0]?.value || "",
    });
    setSelectedFile(null);
  };

  const handleImport = () => {
    loadAttendanceRows(fetchFilters);
    showToast([translate("Data imported successfully")], "success");
  };

  const handleDownloadExcelTemplate = () => {
    const headers = [
      "employeeid",
      "employeecode",
      "fullname",
      "departmentstxt",
      "designationstxt",
      "atndate",
      "CheckIN",
      "CheckOut",
      "remarks",
    ];
    const csvContent = `${headers.join(",")}\n`;
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", "DownloadAttendanceTemplate.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const extraToolbarItems = (
    <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
      <Button
        variant="outlined"
        startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
        onClick={() => {
          resetFetchFilters();
          setBackendRows([]);
          setSelectedRowIds([]);
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
  );

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
        <Box sx={{ mb: 4 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              gap: 1.5,
              mb: 2,
              pb: 1,
              borderBottom: "1px solid #F1F5F9",
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
                {translate("Download From")}
              </Typography>
            </Box>
            <Box sx={{ display: "flex", gap: 1.5 }}>
              <Button
                variant="outlined"
                onClick={handleDownloadExcelTemplate}
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
                {translate("Download Excel Template")}
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
              <Button
                variant="contained"
                disableElevation
                startIcon={<FetchIcon />}
                onClick={handleImport}
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
                {translate("Import")}
              </Button>
            </Box>
          </Box>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <LmsSelect
                label={translate("Download From")}
                required
                value={fetchFilters.downloadFrom}
                onChange={(value) =>
                  setFetchFilters({
                    ...fetchFilters,
                    downloadFrom: String(value || ""),
                  })
                }
                options={downloadFromOptions}
                placeholder={translate("Select Download From")}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Employee")}
                placeholder={translate("Select Employee")}
                value={fetchFilters.employeeName || fetchFilters.employeeCode}
                onSearchClick={() => setFetchEmployeeLookupOpen(true)}
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
            <Grid item xs={12} md={6}>
              <FormDatePicker
                label={translate("From Date")}
                required
                value={fetchFilters.fromDate ? dayjs(fetchFilters.fromDate) : null}
                onChange={(val) =>
                  setFetchFilters({ ...fetchFilters, fromDate: val })
                }
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <FormDatePicker
                label={translate("Till Date")}
                required
                value={fetchFilters.tillDate ? dayjs(fetchFilters.tillDate) : null}
                onChange={(val) =>
                  setFetchFilters({ ...fetchFilters, tillDate: val })
                }
              />
            </Grid>
          </Grid>
        </Box>

        <Box sx={{ mb: 2 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 1.5,
              mb: 2,
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
              {translate("File Information")}
            </Typography>
          </Box>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <LmsSelect
                label={translate("File Type")}
                value={fetchFilters.fileType}
                onChange={(value) =>
                  setFetchFilters({
                    ...fetchFilters,
                    fileType: String(value || ""),
                  })
                }
                options={fileTypeOptions}
                placeholder={translate("Select File Type")}
              />

              <Box sx={{ display: "flex", alignItems: "center", gap: 1, mt: 2 }}>
                <Button
                  variant="contained"
                  component="label"
                  sx={{
                    textTransform: "none",
                    fontWeight: 700,
                    bgcolor: "var(--lms-primary)",
                    transition: "all 0.3s ease",
                    boxShadow: "0 2px 4px rgba(var(--lms-primary-rgb), 0.15)",
                    "&:hover": {
                      bgcolor: "var(--lms-primary)",
                      boxShadow: "0 4px 12px rgba(var(--lms-primary-rgb), 0.3)",
                    },
                    borderRadius: "8px",
                    px: 3,
                    py: 0.8,
                    fontSize: "0.85rem",
                  }}
                >
                  {translate("Choose Image")}
                  <input
                    type="file"
                    hidden
                    onChange={(e) => {
                      if (e.target.files && e.target.files.length > 0) {
                        setSelectedFile(e.target.files[0]);
                      }
                    }}
                  />
                </Button>
                <Typography
                  sx={{
                    fontSize: "0.85rem",
                    fontWeight: 500,
                    color: "var(--lms-text-secondary)",
                  }}
                >
                  {selectedFile ? selectedFile.name : translate("No file chosen")}
                </Typography>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </Box>
      <SchemaTable
        title="Download Attendance"
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

export default DownloadAttendance;
