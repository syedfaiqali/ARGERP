import React, { useEffect, useMemo, useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import { Box, Typography, Grid, Button } from "@mui/material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import {
  Save as SaveIcon,
  Refresh as RefreshIcon,
} from "@mui/icons-material";
import { Collapse } from "@mui/material";
import { FormDatePicker } from "@/components/FormDatePicker";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LookupDropdown } from "@/components/LookupDropdown";
import { DeviceSearchModal } from "@/components/lookups/DeviceSearchModal";
import { FileTypeSearchModal } from "@/components/lookups/FileTypeSearchModal";
import { GetService, PostService } from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

// Legacy GenEnum.MemberShipType — the MEMTP LOV ids that drive whether the
// screen uploads student or staff attendance.
const MEMBER_TYPE_STUDENT = 190;
const MEMBER_TYPE_STAFF = 191;

// Legacy GenEnum.File_Types.Text_File — the only pre-selected lookup on this
// screen.
const FILE_TYPE_TEXT_FILE = 766;

const UploadAttendance: React.FC = () => {
  const { translate } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [rows, setRows] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);
  const [students, setStudents] = useState<any[]>([]);
  const [memberTypes, setMemberTypes] = useState<any[]>([]);
  const [devices, setDevices] = useState<any[]>([]);
  const [fileTypes, setFileTypes] = useState<any[]>([]);
  const [parsedRows, setParsedRows] = useState<any[]>([]);
  const [formData, setFormData] = useState({
    typeId: 0,
    type: "",
    deviceId: 0,
    device: "",
    fileTypeId: 0,
    fileType: "",
    fromDate: dayjs(),
    tillDate: dayjs(),
    fileName: "No file chosen",
  });
  const [isBasicExpanded, setIsBasicExpanded] = useState(true);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterErrors, setToasterErrors] = useState<string[]>([]);

  const [deviceLookupOpen, setDeviceLookupOpen] = useState(false);
  const [fileTypeLookupOpen, setFileTypeLookupOpen] = useState(false);

  const showError = (errors: string[]) => {
    setToasterErrors(errors);
    setShowToaster(true);
  };

  const parseTime = (value: string) => {
    const v = (value || "").trim();
    if (!v) return "";
    const t1 = dayjs(v, "HH:mm:ss", true);
    if (t1.isValid()) return t1.format("HH:mm:ss");
    const t2 = dayjs(v, "HH:mm", true);
    if (t2.isValid()) return t2.format("HH:mm:ss");
    const t3 = dayjs(v);
    if (t3.isValid()) return t3.format("HH:mm:ss");
    return "";
  };

  const parseDate = (value: string) => {
    const v = (value || "").trim();
    if (!v) return "";
    const d1 = dayjs(v, "YYYY-MM-DD", true);
    if (d1.isValid()) return d1.format("YYYY-MM-DD");
    const d2 = dayjs(v, "DD/MM/YYYY", true);
    if (d2.isValid()) return d2.format("YYYY-MM-DD");
    const d3 = dayjs(v);
    if (d3.isValid()) return d3.format("YYYY-MM-DD");
    return "";
  };

  const parseCsvRows = (content: string) => {
    const lines = content
      .split(/\r?\n/)
      .map((x) => x.trim())
      .filter(Boolean);

    if (lines.length <= 1) return [];

    const detectDelimiter = (line: string) => {
      if (line.includes("\t")) return "\t";
      if (line.includes(";")) return ";";
      return ",";
    };
    const delimiter = detectDelimiter(lines[0]);
    const splitLine = (line: string) => line.split(delimiter).map((x) => x.trim());

    const headers = splitLine(lines[0]).map((h) => h.toLowerCase());
    const idx = {
      employeeCode: headers.findIndex((x) =>
        [
          "employeecode",
          "employee code",
          "employee_code",
          "empcode",
          "employeeid",
          "employee id",
        ].includes(x),
      ),
      attendanceDate: headers.findIndex((x) =>
        ["atndate", "attendance date", "attendancedate", "attendance_date", "date"].includes(x),
      ),
      checkIn: headers.findIndex((x) => ["checkin", "check in", "in", "timein"].includes(x)),
      checkOut: headers.findIndex((x) => ["checkout", "check out", "out", "timeout"].includes(x)),
      remarks: headers.findIndex((x) => ["remarks", "remark", "note"].includes(x)),
      studentNo: headers.findIndex((x) =>
        ["studentno", "student no", "student_no", "studentid", "student id", "grno", "gr no"].includes(x),
      ),
      // Legacy matches biometric rows to a student by the device enrolment
      // number held on the profile as machinecode.
      machineCode: headers.findIndex((x) =>
        ["enno", "en no", "machinecode", "machine code", "machine_code"].includes(x),
      ),
    };

    return lines.slice(1).map((line) => {
      const cols = splitLine(line);
      const employeeCode =
        idx.employeeCode >= 0
          ? cols[idx.employeeCode] || ""
          : cols[0] || "";
      const attendanceDate =
        idx.attendanceDate >= 0
          ? parseDate(cols[idx.attendanceDate] || "")
          : parseDate(cols[1] || "");
      const checkIn =
        idx.checkIn >= 0
          ? parseTime(cols[idx.checkIn] || "")
          : parseTime(cols[2] || "");
      const checkOut =
        idx.checkOut >= 0
          ? parseTime(cols[idx.checkOut] || "")
          : parseTime(cols[3] || "");
      const remarks =
        idx.remarks >= 0
          ? cols[idx.remarks] || ""
          : cols[4] || "";
      const studentNo =
        idx.studentNo >= 0 ? cols[idx.studentNo] || "" : "";
      const machineCode =
        idx.machineCode >= 0 ? cols[idx.machineCode] || "" : "";
      return {
        employeeCode,
        studentNo,
        machineCode,
        attendanceDate,
        checkIn,
        checkOut,
        remarks,
      };
    });
  };

  const loadLookups = async () => {
    try {
      setLoading(true);
      const response: any = await GetService("/api/HumanResource/UploadAttendance/v3/lookups");
      const payload = response?.data ?? {};
      const toOptions = (items: any[]) =>
        (Array.isArray(items) ? items : []).map((x: any) => ({
          id: Number(x?.id ?? 0),
          code: x?.code || "",
          shortName: x?.name || "",
          description: x?.description || "",
        }));

      const types = toOptions(payload?.memberTypes);
      const deviceItems = toOptions(payload?.deviceTypes);
      const fileItems = toOptions(payload?.fileTypes);

      setMemberTypes(types.filter((x:any) => x.id !== MEMBER_TYPE_STUDENT));
      setDevices(deviceItems);
      setFileTypes(fileItems);

      // Legacy leaves Type and Device empty so the user must pick them, and
      // defaults File Type to Text File only.
      const defaultFileType =
        fileItems.find((x) => x.id === FILE_TYPE_TEXT_FILE) ??
        fileItems.find((x) => /text/i.test(x.shortName || ""));

      setFormData((prev) => ({
        ...prev,
        fileTypeId: defaultFileType?.id || 0,
        fileType: defaultFileType?.shortName || "",
      }));
    } catch {
      showError([translate("Failed to load lookup data")]);
    } finally {
      setLoading(false);
    }
  };

  const loadEmployees = async () => {
    try {
      const response: any = await GetService("/api/HumanResource/UploadAttendance/v3/employees");
      const items = response?.data ?? [];
      setEmployees(Array.isArray(items) ? items : []);
    } catch {
      showError([translate("Failed to load employees")]);
    }
  };

  const loadStudents = async () => {
    try {
      const response: any = await GetService("/api/HumanResource/UploadAttendance/v3/employees");
      const items = response?.data ?? [];
      setStudents(Array.isArray(items) ? items : []);
    } catch {
      showError([translate("Failed to load students")]);
    }
  };

  const validate = (): boolean => {
    setIsSubmitted(true);
    const errors: string[] = [];
    if (!formData.typeId) errors.push("Type is required");
    if (!formData.deviceId) errors.push("Device is required");
    if (!formData.fileTypeId) errors.push("File Type is required");
    if (!formData.fromDate) errors.push("From Date is required");
    if (!formData.tillDate) errors.push("Till Date is required");
    if (formData.fileName === "No file chosen") {
      errors.push("File is required");
    } else if (parsedRows.length === 0) {
      errors.push("No valid rows found in selected file");
    }

    if (errors.length > 0) {
      showError(errors);
      return false;
    }
    return true;
  };

  const handleSave = async () => {
    if (!validate()) return;
    try {
      setLoading(true);
      const commonPayload = {
        typeId: formData.typeId || null,
        deviceId: formData.deviceId || null,
        fileTypeId: formData.fileTypeId || null,
        fromDate: formData.fromDate
          ? dayjs(formData.fromDate).format("YYYY-MM-DD")
          : null,
        tillDate: formData.tillDate
          ? dayjs(formData.tillDate).format("YYYY-MM-DD")
          : null,
      };

      const payload = isStudentType
        ? {
            ...commonPayload,
            items: parsedRows.map((x: any) => ({
              studentNo: x.studentNo || "",
              machineCode: x.machineCode || "",
              attendanceDate: x.attendanceDate || "",
              checkIn: x.checkIn || null,
              checkOut: x.checkOut || null,
              remarks: x.remarks || "",
            })),
          }
        : {
            ...commonPayload,
            items: parsedRows.map((x: any) => ({
              employeeCode: x.employeeCode || "",
              attendanceDate: x.attendanceDate || "",
              checkIn: x.checkIn || null,
              checkOut: x.checkOut || null,
              remarks: x.remarks || "",
            })),
          };

      const response: any = await PostService(
        isStudentType
          ? "/api/HumanResource/UploadAttendance/v3/process"
          : "/api/HumanResource/UploadAttendance/v3/process",
        payload,
      );
      const result = response?.data ?? {};
      const resultItems = Array.isArray(result?.items) ? result.items : [];

      const mappedRows = isStudentType
        ? resultItems.map((item: any, index: number) => {
            const source = parsedRows[index] || {};
            const key = String(item?.studentNo || "").trim().toUpperCase();
            const student =
              students.find(
                (x: any) =>
                  String(x?.studentNo || "").trim().toUpperCase() === key,
              ) ??
              students.find(
                (x: any) =>
                  String(x?.machineCode || "").trim().toUpperCase() === key,
              );
            return {
              id: item?.attendanceId ?? index + 1,
              sNo: index + 1,
              grNo: student?.grNo || "--",
              studentNo: student?.studentNo || item?.studentNo || "--",
              studentName: student?.studentName || "--",
              course: student?.course || "--",
              section: student?.section || "--",
              attendanceDate: source?.attendanceDate
                ? dayjs(source.attendanceDate).format("DD/MM/YYYY")
                : "--",
              checkIn: source?.checkIn || "--",
              checkOut: source?.checkOut || "--",
              remarks: source?.remarks || "--",
              status: item?.success ? "Processed" : item?.message || "Failed",
            };
          })
        : resultItems.map((item: any, index: number) => {
            const code = String(item?.employeeCode || "").trim().toUpperCase();
            const emp = employees.find(
              (x: any) =>
                String(x?.employeeCode || "").trim().toUpperCase() === code,
            );
            const source = parsedRows[index] || {};
            return {
              id: item?.attendanceId ?? index + 1,
              sNo: index + 1,
              employeeCode: code || "--",
              employeeName: emp?.employeeName || "--",
              department: emp?.department || "--",
              designation: emp?.designation || "--",
              attendanceDate: source?.attendanceDate
                ? dayjs(source.attendanceDate).format("DD/MM/YYYY")
                : "--",
              checkIn: source?.checkIn || "--",
              checkOut: source?.checkOut || "--",
              remarks: source?.remarks || "--",
              status: item?.success ? "Processed" : item?.message || "Failed",
            };
          });

      setRows(mappedRows);
      setShowToaster(true);
      setToasterErrors([
        `${translate("Processed")}: ${result?.successCount ?? 0}`,
        `${translate("Failed")}: ${result?.failedCount ?? 0}`,
      ]);
    } catch (error: any) {
      const message =
        error?.response?.data ||
        error?.response?.data?.message ||
        translate("Failed to upload attendance");
      showError([String(message)]);
    } finally {
      setLoading(false);
    }
  };

  const handleFileSelect = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const content = await file.text();
      const parsed = parseCsvRows(content).filter((x) =>
        isStudentType
          ? (x.studentNo || x.machineCode) && x.attendanceDate
          : x.employeeCode && x.attendanceDate,
      );
      setParsedRows(parsed);
      setFormData((prev) => ({ ...prev, fileName: file.name }));
      setRows([]);
      if (parsed.length === 0) {
        showError([
          translate("No valid rows found in selected file"),
          isStudentType
            ? translate("Expected columns: StudentNo or EnNo, AttendanceDate, CheckIn, CheckOut, Remarks")
            : translate("Expected columns: EmployeeCode, AttendanceDate, CheckIn, CheckOut, Remarks"),
        ]);
      }
    }
  };

  const resetForm = () => {
    // Legacy ClearScreen(): Type and Device go back to empty, File Type back to
    // Text File.
    const defaultFileType =
      fileTypes.find((x) => x.id === FILE_TYPE_TEXT_FILE) ??
      fileTypes.find((x) => /text/i.test(x.shortName || ""));

    setFormData({
      typeId: 0,
      type: "",
      deviceId: 0,
      device: "",
      fileTypeId: defaultFileType?.id || 0,
      fileType: defaultFileType?.shortName || "",
      fromDate: dayjs(),
      tillDate: dayjs(),
      fileName: "No file chosen",
    });
    setParsedRows([]);
    setRows([]);
    setIsSubmitted(false);
  };

  useEffect(() => {
    loadLookups();
    loadEmployees();
  }, []);

  // Legacy TypeConfig(): Student and Staff each drive their own lookup list and
  // their own grid.
  const isStudentType = useMemo(() => {
    if (formData.typeId === MEMBER_TYPE_STUDENT) return true;
    if (formData.typeId === MEMBER_TYPE_STAFF) return false;
    return /student/i.test(formData.type || "");
  }, [formData.type, formData.typeId]);

  useEffect(() => {
    if (isStudentType && students.length === 0) {
      loadStudents();
    }
  }, [isStudentType]);

  const typeOptions = useMemo(() => memberTypes, [memberTypes]);

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

  // Legacy renders a dedicated grid per member type (studentgridConfig vs
  // gridConfig).
  const columns: Column[] = isStudentType
    ? [
        { id: "sNo", label: "S.No", minWidth: 60 },
        { id: "grNo", label: "G.R #", minWidth: 120 },
        { id: "studentNo", label: "Student ID", minWidth: 120 },
        { id: "studentName", label: "Student", minWidth: 180 },
        { id: "course", label: "Course", minWidth: 150 },
        { id: "section", label: "Section", minWidth: 150 },
        { id: "attendanceDate", label: "Attendance Date", minWidth: 130 },
        { id: "checkIn", label: "Check In", minWidth: 100 },
        { id: "checkOut", label: "Check Out", minWidth: 100 },
        { id: "remarks", label: "Remarks", minWidth: 180 },
        { id: "status", label: "Status", minWidth: 130 },
      ]
    : [
        { id: "sNo", label: "S.No", minWidth: 60 },
        { id: "employeeCode", label: "Employee ID", minWidth: 120 },
        { id: "employeeName", label: "Employee", minWidth: 180 },
        { id: "department", label: "Department", minWidth: 150 },
        { id: "designation", label: "Designation", minWidth: 150 },
        { id: "attendanceDate", label: "Attendance Date", minWidth: 130 },
        { id: "checkIn", label: "Check In", minWidth: 100 },
        { id: "checkOut", label: "Check Out", minWidth: 100 },
        { id: "remarks", label: "Remarks", minWidth: 180 },
        { id: "status", label: "Status", minWidth: 130 },
      ];

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
              {translate("Upload Attendance")}
            </Typography>
          </Box>
          <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
            <Button
              variant="contained"
              disableElevation
              startIcon={<SaveIcon />}
              onClick={handleSave}
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
              {translate("Upload")}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon sx={{ fontSize: "1.1rem" }} />}
              onClick={resetForm}
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
              startIcon={<SaveIcon />}
              onClick={handleSave}
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
              {translate("Save")}
            </Button>
          </Box>
        </Box>

        <Box>
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
            onClick={() => setIsBasicExpanded(!isBasicExpanded)}
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
                <T>Attendance Upload Parameters</T>
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
              {renderExpandIcon(isBasicExpanded)}
            </Box>
          </Box>

          <Collapse in={isBasicExpanded}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={
                    <>
                      Type
                      <Typography component="span" sx={{ color: "error.main" }}>
                        *
                      </Typography>
                    </>
                  }
                  placeholder={translate("Select Type")}
                  value={formData.type}
                  options={typeOptions}
                  getOptionLabel={(opt: any) => opt.shortName || ""}
                  onSelect={(row: any) => {
                    setFormData((prev) => ({
                      ...prev,
                      typeId: Number(row?.id ?? 0),
                      type: row?.shortName || "",
                      fileName: "No file chosen",
                    }));
                    // Switching member type changes which grid and which file
                    // columns apply, so drop anything already parsed.
                    setParsedRows([]);
                    setRows([]);
                  }}
                  error={isSubmitted && !formData.typeId}
                  helperText={
                    isSubmitted && !formData.typeId ? "Type is required" : ""
                  }
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={<>Device<Typography component="span" sx={{ color: "error.main" }}>*</Typography></>}
                  placeholder={translate("Select Device")}
                  value={formData.device}
                  onSearchClick={() => setDeviceLookupOpen(true)}
                  options={devices}
                  getOptionLabel={(opt: any) => opt.shortName || ""}
                  onSelect={(row: any) =>
                    setFormData((prev) => ({
                      ...prev,
                      deviceId: Number(row?.id ?? 0),
                      device: row?.shortName || "",
                    }))
                  }
                  error={isSubmitted && !formData.deviceId}
                  helperText={
                    isSubmitted && !formData.deviceId ? "Device is required" : ""
                  }
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={<>File Type<Typography component="span" sx={{ color: "error.main" }}>*</Typography></>}
                  placeholder={translate("Select File Type")}
                  value={formData.fileType}
                  onSearchClick={() => setFileTypeLookupOpen(true)}
                  options={fileTypes}
                  getOptionLabel={(opt: any) => opt.shortName || ""}
                  onSelect={(row: any) =>
                    setFormData((prev) => ({
                      ...prev,
                      fileTypeId: Number(row?.id ?? 0),
                      fileType: row?.shortName || "",
                    }))
                  }
                  error={isSubmitted && !formData.fileTypeId}
                  helperText={
                    isSubmitted && !formData.fileTypeId ? "File Type is required" : ""
                  }
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("From Date")}
                  value={formData.fromDate}
                  onChange={(val) =>
                    setFormData({ ...formData, fromDate: val as Dayjs })
                  }
                  error={isSubmitted && !formData.fromDate}
                  helperText={
                    isSubmitted && !formData.fromDate
                      ? "From Date is required"
                      : ""
                  }
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("Till Date")}
                  value={formData.tillDate}
                  onChange={(val) =>
                    setFormData({ ...formData, tillDate: val as Dayjs })
                  }
                  error={isSubmitted && !formData.tillDate}
                  helperText={
                    isSubmitted && !formData.tillDate
                      ? "Till Date is required"
                      : ""
                  }
                />
              </Grid>

              {/* Upload Section */}
              <Grid item xs={12}>
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 2,
                    mt: 1,
                  }}
                >
                  <Button
                    component="label"
                    variant="contained"
                    sx={{
                      bgcolor: "var(--lms-primary)",
                      "&:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.92)" },
                      textTransform: "none",
                      borderRadius: "10px",
                      fontSize: "0.85rem",
                      fontWeight: 700,
                      px: 3,
                      height: "40px",
                      boxShadow: "none",
                    }}
                  >
                    + Choose File
                    <input type="file" hidden onChange={handleFileSelect} />
                  </Button>
                  <Typography
                    sx={{
                      fontSize: "0.85rem",
                      color: "var(--lms-primary)",
                      fontWeight: 600,
                      px: 1.5,
                      py: 1,
                      borderRadius: "8px",
                      border: "1px dashed rgba(var(--lms-primary-rgb), 0.35)",
                      bgcolor: "rgba(var(--lms-primary-rgb), 0.06)",
                    }}
                  >
                    {formData.fileName}
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          </Collapse>
        </Box>
      </Box>
      <SchemaTable
        title="Upload Attendance"
        breadcrumbPath=""
        columns={columns}
        rows={rows}
        initialFormData={{}}
        onSave={async () => true}
        onDelete={async () => { }}
        renderForm={() => null}
        showModalActions={false}
        actions={false}
        showAddButton={false}
      />
      <DeviceSearchModal
        open={deviceLookupOpen}
        onClose={() => setDeviceLookupOpen(false)}
        data={devices}
        selectedValue={formData.device}
        onSelect={(row) => {
          setFormData((prev) => ({
            ...prev,
            deviceId: Number(row?.id ?? 0),
            device: row?.shortName || "",
          }));
          setDeviceLookupOpen(false);
        }}
      />
      <FileTypeSearchModal
        open={fileTypeLookupOpen}
        onClose={() => setFileTypeLookupOpen(false)}
        data={fileTypes}
        selectedValue={formData.fileType}
        onSelect={(row) => {
          setFormData((prev) => ({
            ...prev,
            fileTypeId: Number(row?.id ?? 0),
            fileType: row?.shortName || "",
          }));
          setFileTypeLookupOpen(false);
        }}
      />
      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterErrors}
        severity="error"
        disableDeleteSuccessAutoSeverity={true}
      />
    </FormThemeWrapper>
  );
};

export default UploadAttendance;

