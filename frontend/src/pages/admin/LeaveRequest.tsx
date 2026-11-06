import { EditableImageInput } from "@/components/EditableImageInput";
import React, { useState, useEffect, useCallback } from "react";
import dayjs from "dayjs";
import {
  Grid,
  Box,
  Typography,
  IconButton,
  Dialog,
  Menu,
  MenuItem,
  InputAdornment,
  Collapse,
  Button,
} from "@mui/material";
import {
  Search as SearchIcon,
  Close as CloseIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Assessment as AssessmentIcon,
  History as HistoryIcon,
  Add as AddIcon,
  UploadFile as UploadFileIcon,
} from "@mui/icons-material";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
  toDateOnly,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { refreshRequestNotifications } from "@/components/RequestNotificationPopover";
import customParseFormat from "dayjs/plugin/customParseFormat";
import { LeaveRequestReportModal } from "./LeaveRequestReportModal";
import { humanResourceAdministrationService } from "@/services/admin/administrationService";
import { EmployeeHeaderCard } from "./components/EmployeeHeaderCard";

dayjs.extend(customParseFormat);

interface LeaveRequestProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const initialFormData = {
  id: null,
  requestNo: "",
  requestDate: dayjs().format("YYYY-MM-DD"),
  employeeId: null,
  employeeName: "",
  employeeCode: "",
  reason: "",
  leaveUnitId: null,
  leaveUnitTxt: "",
  leaveTypeId: null,
  leaveTypeTxt: "",
  childBirthDate: null,
  inLieuDate: null,
  fromDate: null,
  tillDate: null,
  fromTime: "",
  tillTime: "",
  leaveDays: 0,
  statusId: 2, // Pending
  remarks: "",
  attachmentFile: null as File | null,
  attachmentDocumentName: "",
  attachmentUploadPath: "",
  attachmentExternalLink: "",
  attachmentChanged: false,
  attachments: [] as any[],
};

const getErrorMessage = (error: any, fallback: string) => {
  const data = error?.response?.data;
  if (typeof data === "string") return data;
  if (data?.message) return data.message;
  if (data?.title) return data.title;
  return error?.message || fallback;
};

const readFileAsDataUrl = (file: File) =>
  new Promise<string>((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(String(reader.result || ""));
    reader.onerror = () => reject(reader.error);
    reader.readAsDataURL(file);
  });

const getTimeInputValue = (value: any) => {
  if (!value) return "";
  const text = String(value);
  if (/^\d{2}:\d{2}/.test(text)) return text.slice(0, 5);
  const parsed = dayjs(value);
  return parsed.isValid() ? parsed.format("HH:mm") : "";
};

const toDateTime = (date: any, time: string) =>
  date && time ? dayjs(`${toDateOnly(date)}T${time}`).toISOString() : null;

const LeaveRequest: React.FC<LeaveRequestProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [employees, setEmployees] = useState<any[]>([]);
  const [leaveTypes, setLeaveTypes] = useState<any[]>([]);
  const [leaveUnits, setLeaveUnits] = useState<any[]>([]);
  const [statuses, setStatuses] = useState<any[]>([]);
  const [genders, setGenders] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [entities, setEntities] = useState<any[]>([]);
  const [documentTypes, setDocumentTypes] = useState<any[]>([]);
  const [lookupRows, setLookupRows] = useState<any[]>([]);
  const [lookupSearchTerm, setLookupSearchTerm] = useState("");

  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">(
    "list"
  );
  const [formData, setFormData] = useState<any>({ ...initialFormData });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [attachmentDocTypeLookupLocalId, setAttachmentDocTypeLookupLocalId] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isLeaveDetailExpanded, setIsLeaveDetailExpanded] = useState(true);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );
  const [filterStatus, setFilterStatus] = useState<number | string>("");
  const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedMenuRow, setSelectedMenuRow] = useState<any | null>(null);
  const [reportInitialFilters, setReportInitialFilters] = useState<any>(null);

  const currentUser = (() => {
    try {
      return JSON.parse(localStorage.getItem("user") || "{}");
    } catch {
      return {};
    }
  })();
  // User Category 9 is the System Admin category (UCAT).
  const isLeaveRequestAdmin = Number(
    currentUser?.userCatId ?? currentUser?.UserCatId ?? currentUser?.usercatid ?? 0,
  ) === 9;
  const currentEmployeeId =
    currentUser?.empId ??
    currentUser?.EmpId ??
    currentUser?.empid ??
    currentUser?.employeeId ??
    currentUser?.EmployeeId ??
    currentUser?.employeeid ??
    null;
  const visibleEmployees = isLeaveRequestAdmin
    ? employees
    : employees.filter((employee: any) =>
        String(employee.id ?? employee.Id) === String(currentEmployeeId ?? ""),
      );

  const fetchRequests = useCallback(async () => {
    setLoading(true);
    try {
      let url = "/api/HumanResource/LeaveRequest/v3";
      if (filterStatus) {
        url += `?statusId=${filterStatus}`;
      }
      const response: any = await GetService(url);
      const items = response.data.items || [];
      setRows(items.map((x: any, index: number) => ({
        ...x,
        sNo: index + 1,
        entityId: x.entityId || x.EntityId || null,
        employeeName: x.employeeName || x.EmployeeName,
        employeeCode: x.employeeCode || x.EmployeeCode || "",
        genderId: x.genderId || x.GenderId || null,
        genderName: x.genderName || x.GenderName || x.gender || x.Gender || "",
        designationId: x.designationId || x.DesignationId || null,
        designationName: x.designationName || x.DesignationName || x.designation || x.Designation || "",
        leaveType: x.leaveTypeTxt || x.LeaveTypeTxt,
        leaveUnit: x.leaveUnitTxt || x.LeaveUnitTxt,
        leaveDays: x.leaveDays || x.LeaveDays,
        childBirthDate: x.childBirthDate || x.ChildBirthDate || x.dob || x.DOB || null,
        inLieuDate: x.inLieuDate || x.InLieuDate || x.inleuedate || x.Inleuedate || null,
        attachmentDocumentName: x.attachmentDocumentName || x.AttachmentDocumentName || "",
        attachmentUploadPath: x.attachmentUploadPath || x.AttachmentUploadPath || "",
        attachmentExternalLink: x.attachmentExternalLink || x.AttachmentExternalLink || "",
        attachments: x.attachments || x.Attachments || [],
      })));
    } catch (error: any) {
      console.error("Failed to fetch requests", error);
    } finally {
      setLoading(false);
    }
  }, [filterStatus]);

  const fetchLookups = useCallback(async () => {
    try {
      const [emp, lt, lu, st, genderResp, designationResp, departmentResp, entityResp, attachmentPolicyResp, attachmentLookupResp] = await Promise.all([
        GetService("/api/HumanResource/EmployeeProfile/v3?pageSize=1000&leaveRequestLookup=true"),
        GetService("/api/HumanResource/LeaveType/v3"),
        GetService("/api/HumanResource/Lov/v3/LVUNT"),
        GetService("/api/HumanResource/Lov/v3/LRST"), // Leave Request Status
        GetService("/api/HumanResource/Lov/v3/GEND"),
        GetService("/api/HumanResource/Lookups/v3/designations"),
        GetService("/api/HumanResource/Lookups/v3/departments"),
        GetService("/api/HumanResource/Lookups/v3/institutions"),
        humanResourceAdministrationService.getSystemPolicyAttachmentPolicies(),
        humanResourceAdministrationService.getSystemPolicyAttachmentLookups(),
      ]);
      setEmployees(emp?.data?.items || emp?.data?.Items || []);
      setLeaveTypes(lt?.data?.items || lt?.data?.Items || lt?.data || []);
      setLeaveUnits(lu?.data?.items || lu?.data?.Items || lu?.data || []);
      setStatuses(st?.data?.items || st?.data?.Items || st?.data || []);
      setGenders(genderResp?.data?.items || genderResp?.data?.Items || genderResp?.data || []);
      setDesignations(
        designationResp?.data?.items ||
          designationResp?.data?.Items ||
          designationResp?.data ||
          [],
      );
      setDepartments(departmentResp?.data?.items || departmentResp?.data?.Items || departmentResp?.data || []);
      const entityData = entityResp?.data;
      setEntities(entityData?.items || entityData?.Items || (Array.isArray(entityData) ? entityData : []));
      const normalizeDocumentTypeOption = (item: any) => {
        const id = item?.lovDocumentTypeId ?? item?.LovDocumentTypeId ?? item?.id ?? item?.Id;
        return {
          id,
          code: item?.documentTypeCode ?? item?.DocumentTypeCode ?? item?.code ?? item?.Code ?? "",
          name:
            item?.documentTypeName ??
            item?.DocumentTypeName ??
            item?.documentType ??
            item?.DocumentType ??
            item?.name ??
            item?.Name ??
            item?.stxt ??
            item?.Stxt ??
            item?.code ??
            item?.Code ??
            "",
        };
      };
      const policyDocumentTypes = (attachmentPolicyResp || [])
        .map(normalizeDocumentTypeOption)
        .filter((item: any) => item.id);
      const lookupDocumentTypes = (attachmentLookupResp?.documentTypes || [])
        .map(normalizeDocumentTypeOption)
        .filter((item: any) => item.id);
      const mergedDocumentTypes = [...policyDocumentTypes, ...lookupDocumentTypes].filter(
        (item, index, list) => list.findIndex((x) => String(x.id) === String(item.id)) === index,
      );
      setDocumentTypes(mergedDocumentTypes);
    } catch (error: any) {
      console.error("Failed to fetch lookups", error);
    }
  }, []);

  useEffect(() => {
    fetchRequests();
    fetchLookups();
  }, [fetchRequests, fetchLookups]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Leave Request"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Leave Request"));
    else if (viewMode === "view") onSubViewChange(translate("View Leave Request"));
    else if (viewMode === "report") onSubViewChange(translate("Leave Request Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);
  useEffect(() => {
    if (viewMode !== "new" || isLeaveRequestAdmin || !currentEmployeeId) return;

    const employee = employees.find((item: any) =>
      String(item.id ?? item.Id) === String(currentEmployeeId),
    );
    if (!employee) return;

    setFormData((previous: any) => {
      if (previous.employeeId) return previous;
      return {
        ...previous,
        employeeId: employee.id ?? employee.Id,
        employeeName:
          employee.shortName ||
          employee.employeeName ||
          employee.EmployeeName ||
          `${employee.firstName || employee.FirstName || ""} ${employee.lastName || employee.LastName || ""}`.trim(),
        employeeCode: employee.employeeCode ?? employee.EmployeeCode ?? employee.code ?? "",
      };
    });
  }, [currentEmployeeId, employees, isLeaveRequestAdmin, viewMode]);

  const isHourlyLeaveUnit = (leaveUnitId: any) =>
    /hours?/i.test(getLeaveUnitText(leaveUnitId));

  useEffect(() => {
    if (isHourlyLeaveUnit(formData.leaveUnitId)) return;
    if (formData.fromDate && formData.tillDate) {
      const from = dayjs(formData.fromDate);
      const till = dayjs(formData.tillDate);
      if (till.isValid() && from.isValid()) {
        if (till.isAfter(from) || till.isSame(from)) {
          const days = till.diff(from, "day") + 1;
          setFormData((prev: any) => ({ ...prev, leaveDays: days }));
        }
      }
    }
  }, [formData.fromDate, formData.tillDate, formData.leaveUnitId, leaveUnits]);

  useEffect(() => {
    if (!isHourlyLeaveUnit(formData.leaveUnitId) || !formData.fromTime || !formData.tillTime) return;

    const start = dayjs(`2000-01-01T${formData.fromTime}`);
    const end = dayjs(`2000-01-01T${formData.tillTime}`);
    if (start.isValid() && end.isValid() && end.isAfter(start)) {
      const durationInMinutes = end.diff(start, "minute");
      setFormData((prev: any) =>
        prev.leaveDays === durationInMinutes ? prev : { ...prev, leaveDays: durationInMinutes },
      );
    }
  }, [formData.fromTime, formData.tillTime, formData.leaveUnitId, leaveUnits]);

  const getLeaveUnitText = (leaveUnitId: any) => {
    const selectedUnit = leaveUnits.find((x: any) =>
      String(x.id ?? x.Id) === String(leaveUnitId ?? "")
    );
    return selectedUnit?.name || selectedUnit?.Name || selectedUnit?.stxt || selectedUnit?.Stxt || "";
  };

  const getLeaveTypeUnitId = (leaveType: any) => {
    const rawUnitId =
      leaveType?.leaveUnitId ??
      leaveType?.LeaveUnitId ??
      leaveType?.lvunitid ??
      leaveType?.LvUnitId ??
      leaveType?.leaveUnit ??
      leaveType?.LeaveUnit ??
      null;

    const selectedUnit = leaveUnits.find((x: any) =>
      String(x.id ?? x.Id) === String(rawUnitId ?? "")
    );
    return selectedUnit ? selectedUnit.id ?? selectedUnit.Id : rawUnitId;
  };

  const isSpecialLeaveType = (leaveType: any) =>
    leaveType?.specialLeave === true ||
    leaveType?.SpecialLeave === true ||
    leaveType?.halfadjust === true ||
    leaveType?.Halfadjust === true ||
    leaveType?.halfAdjust === true ||
    leaveType?.HalfAdjust === true;

  const getSelectedLeaveType = (leaveTypeId: any) =>
    leaveTypes.find((x: any) => String(x.id ?? x.Id) === String(leaveTypeId ?? ""));

  const getGenderText = (genderId: any) => {
    const gender = genders.find((x: any) =>
      String(x.id ?? x.Id) === String(genderId ?? "") ||
      String(x.code ?? x.Code ?? "").toLowerCase() === String(genderId ?? "").trim().toLowerCase() ||
      String(x.name ?? x.Name ?? x.stxt ?? x.Stxt ?? "").toLowerCase() === String(genderId ?? "").trim().toLowerCase()
    );

    return (
      gender?.code ??
      gender?.Code ??
      gender?.name ??
      gender?.Name ??
      gender?.stxt ??
      gender?.Stxt ??
      ""
    ).toString().trim().toLowerCase();
  };

  const isMaleLeaveType = (leaveType: any) => {
    const rawGender =
      leaveType?.gender ??
      leaveType?.Gender ??
      leaveType?.genderId ??
      leaveType?.GenderId ??
      leaveType?.genderTxt ??
      leaveType?.GenderTxt ??
      leaveType?.genderName ??
      leaveType?.GenderName;
    const normalized = getGenderText(rawGender) || rawGender?.toString().trim().toLowerCase() || "";

    return normalized === "m" || normalized === "male";
  };

  const showChildBirthDate = isMaleLeaveType(getSelectedLeaveType(formData.leaveTypeId));

  const normalizeAttachment = (row: any, index: number) => ({
    id: row?.id ?? row?.Id ?? null,
    attachmentNo: row?.attachmentNo ?? row?.AttachmentNo ?? "",
    documentTypeId: row?.documentTypeId ?? row?.DocumentTypeId ?? row?.lovreftypeid ?? row?.LovRefTypeId ?? "",
    documentType: row?.documentType ?? row?.DocumentType ?? "",
    documentName: row?.documentName ?? row?.DocumentName ?? row?.docname ?? row?.DocName ?? "",
    uploadPath: row?.uploadPath ?? row?.UploadPath ?? row?.uploadpath ?? row?.Uploadpath ?? "",
    externalLink: row?.externalLink ?? row?.ExternalLink ?? row?.externallink ?? row?.Externallink ?? "",
    file: row?.file ?? null,
    localId: row?.localId ?? `attachment-${row?.id ?? row?.Id ?? "new"}-${index}`,
  });

  const getAttachmentRows = (data: any = formData) =>
    (data.attachments || data.Attachments || []).map(normalizeAttachment);

  const getDocumentTypeText = (documentTypeId: any, fallback = "") => {
    const item = documentTypes.find((x: any) => String(x.id ?? x.Id) === String(documentTypeId ?? ""));
    return item?.documentType || item?.DocumentType || item?.name || item?.Name || item?.stxt || item?.Stxt || fallback;
  };

  const getDocumentTypeDisplayValue = (attachment: any) =>
    getDocumentTypeText(attachment.documentTypeId, attachment.documentType) ||
    attachment.documentType ||
    attachment.documentTypeId ||
    "";

  const getDocumentTypeCode = (documentTypeId: any) => {
    const item = documentTypes.find((x: any) => String(x.id ?? x.Id) === String(documentTypeId ?? ""));
    return item?.code || item?.Code || item?.documentTypeCode || item?.DocumentTypeCode || "";
  };

  const getAttachmentExtension = (attachment: any) => {
    const name =
      attachment?.file?.name ||
      attachment?.uploadPath ||
      attachment?.UploadPath ||
      attachment?.documentName ||
      attachment?.DocumentName ||
      "";
    const fileName = String(name).split(/[\\/]/).pop() || "";
    const match = fileName.match(/\.([a-z0-9]+)$/i);
    return match ? match[1].toLowerCase() : "";
  };

  const getAttachmentFileCategory = (extension: string) => {
    if (!extension) return "";
    if (["pdf"].includes(extension)) return "pdf";
    if (["xls", "xlsx", "csv"].includes(extension)) return "excel";
    if (["doc", "docx"].includes(extension)) return "word";
    if (["jpg", "jpeg", "png", "gif", "webp", "bmp", "svg"].includes(extension)) return "image";
    if (["txt", "rtf"].includes(extension)) return "text";
    if (["mp4", "mov", "avi", "mkv", "webm"].includes(extension)) return "video";
    return "";
  };

  const getDocumentTypeCategory = (attachment: any) => {
    const value = `${getDocumentTypeCode(attachment.documentTypeId)} ${getDocumentTypeText(attachment.documentTypeId, attachment.documentType)}`
      .toLowerCase();

    if (value.includes("pdf")) return "pdf";
    if (value.includes("excel") || value.includes("xls") || value.includes("spreadsheet") || value.includes("csv")) return "excel";
    if (value.includes("word") || value.includes("doc")) return "word";
    if (value.includes("image") || value.includes("photo") || value.includes("picture") || value.includes("jpg") || value.includes("jpeg") || value.includes("png")) return "image";
    if (value.includes("text") || value.includes("txt")) return "text";
    if (value.includes("video") || value.includes("mp4")) return "video";
    return "";
  };

  const getAttachmentTypeMismatchMessage = (attachment: any, rowNo?: number) => {
    const extension = getAttachmentExtension(attachment);
    const fileCategory = getAttachmentFileCategory(extension);
    const documentTypeCategory = getDocumentTypeCategory(attachment);

    if (!fileCategory || !documentTypeCategory || fileCategory === documentTypeCategory) {
      return "";
    }

    const prefix = rowNo ? `${translate("Attachment")} ${rowNo}: ` : "";
    return `${prefix}${translate("Uploaded file type does not match selected Document Type. Please select the correct Document Type.")}`;
  };

  const updateAttachmentRow = (localId: string, updates: any) => {
    const attachments = getAttachmentRows().map((row: any) =>
      row.localId === localId ? { ...row, ...updates } : row,
    );
    setFormData({ ...formData, attachments });
  };

  const selectAttachmentDocumentType = (localId: string, item: any) => {
    const documentTypeId = item?.id ?? item?.Id;
    updateAttachmentRow(localId, {
      documentTypeId,
      documentTypeCode: item?.code ?? item?.Code ?? item?.documentTypeCode ?? item?.DocumentTypeCode ?? "",
      documentType: item?.documentType || item?.DocumentType || item?.name || item?.Name || item?.stxt || item?.Stxt || "",
    });
  };

  const addAttachmentRow = () => {
    const attachments = getAttachmentRows();
    setFormData({
      ...formData,
      attachments: [
        ...attachments,
        normalizeAttachment({ localId: `attachment-new-${Date.now()}` }, attachments.length),
      ],
    });
  };

  const deleteAttachmentRow = (localId: string) => {
    setFormData({
      ...formData,
      attachments: getAttachmentRows().filter((row: any) => row.localId !== localId),
    });
  };

  const applyEmployeeSelection = (employee: any) => {
    const employeeId = employee?.id ?? employee?.Id;
    const employeeCode = employee?.employeeCode ?? employee?.EmployeeCode ?? employee?.code ?? "";
    const employeeName =
      employee?.shortName ||
      employee?.employeeName ||
      employee?.EmployeeName ||
      `${employee?.firstName || employee?.FirstName || ""} ${employee?.lastName || employee?.LastName || ""}`.trim();

    setFormData({
      ...formData,
      employeeId,
      employeeName,
      employeeCode,
    });
  };

  const applyLeaveUnitSelection = (leaveUnit: any) => {
    const leaveUnitId = leaveUnit?.id ?? leaveUnit?.Id;
    const leaveUnitTxt = leaveUnit?.name ?? leaveUnit?.Name ?? leaveUnit?.stxt ?? leaveUnit?.Stxt ?? "";
    const isHourlyUnit = /hours?/i.test(leaveUnitTxt);

    setFormData({
      ...formData,
      leaveUnitId,
      leaveUnitTxt,
      ...(isHourlyUnit ? {} : { fromTime: "", tillTime: "" }),
    });
  };

  const applyLeaveTypeSelection = (leaveType: any) => {
    const leaveTypeId = leaveType?.id ?? leaveType?.Id;
    const leaveTypeTxt =
      leaveType?.leaveType ||
      leaveType?.LeaveType ||
      leaveType?.name ||
      leaveType?.Name ||
      leaveType?.stxt ||
      leaveType?.Stxt ||
      leaveType?.shortName ||
      "";
    const specialLeave = isSpecialLeaveType(leaveType);
    const showChildBirthDateForLeaveType = isMaleLeaveType(leaveType);

    setFormData({
      ...formData,
      leaveTypeId,
      leaveTypeTxt,
      inLieuDate: specialLeave ? formData.inLieuDate : null,
      childBirthDate: showChildBirthDateForLeaveType ? formData.childBirthDate : null,
    });
  };

  const handleLookupOpen = (lookupKey: string) => {
    if (lookupKey === "employee") {
      setLookupRows(visibleEmployees.map(x => ({
        ...x,
        id: x.id ?? x.Id,
        code: x.employeeCode ?? x.EmployeeCode,
        shortName: x.employeeName || x.EmployeeName || `${x.firstName || x.FirstName || ""} ${x.lastName || x.LastName || ""}`.trim(),
      })));
    } else if (lookupKey === "leaveUnit") {
      setLookupRows(leaveUnits.map(x => ({ id: x.id, code: x.code, shortName: x.name })));
    } else if (lookupKey === "leaveType") {
      setLookupRows(leaveTypes.map(x => ({
        ...x,
        id: x.id ?? x.Id,
        code: x.code ?? x.Code,
        shortName: x.leaveType || x.LeaveType || x.name || x.Name || x.stxt || x.Stxt,
      })));
    } 
    else if (lookupKey === "status") {
      setLookupRows(statuses.map(x => ({ id: x.id, code: x.code, shortName: x.name })));
    }
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
    setLookupSearchTerm("");
  };

  const LOOKUP_CONFIG: any = {
    employee: {
      title: translate("Employee"),
      subtitle: translate("Search / Employee"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Employee Name"), minWidth: 200 },
      ],
    },
    leaveUnit: {
      title: translate("Leave Unit"),
      subtitle: translate("Search / Leave Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Leave Unit"), minWidth: 200 },
      ],
    },
    leaveType: {
      title: translate("Leave Type"),
      subtitle: translate("Search / Leave Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Leave Type"), minWidth: 200 },
      ],
    },
    status: {
      title: translate("Status"),
      subtitle: translate("Search / Status"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Status"), minWidth: 200 },
      ],
    },
  };

  const renderLookupDialog = () => {
    if (!activeLookup || !LOOKUP_CONFIG[activeLookup]) return null;
    const config = LOOKUP_CONFIG[activeLookup];

    const filteredRows = lookupRows.filter((row: any) => {
      const term = lookupSearchTerm.toLowerCase();
      return (
        (row.shortName && row.shortName.toLowerCase().includes(term)) ||
        (row.code && row.code.toString().toLowerCase().includes(term))
      );
    });

    return (
      <Dialog
        open={!!activeLookup}
        onClose={handleLookupClose}
        maxWidth="lg"
        fullWidth
        PaperProps={{
          sx: { borderRadius: "12px", overflow: "hidden" },
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
            {config.title}
          </Typography>
          <IconButton
            size="small"
            onClick={handleLookupClose}
            sx={{ bgcolor: "#fff", color: "#aaa" }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>
        <Box sx={{ px: 3, py: 1, bgcolor: "#fff" }}>
          <Typography
            variant="caption"
            sx={{ color: "var(--lms-primary)", fontWeight: 700 }}
          >
            {config.subtitle}
          </Typography>
        </Box>
        <Box sx={{ p: 3, bgcolor: "#fff" }}>
          <SchemaTable
            title={config.title}
            columns={config.columns}
            rows={filteredRows}
            onRowClick={(row) => {
              if (activeLookup === "employee") {
                applyEmployeeSelection(row);
              } else if (activeLookup === "leaveUnit") {
                applyLeaveUnitSelection(row);
              } else if (activeLookup === "leaveType") {
                applyLeaveTypeSelection(row);
              } else if (activeLookup === "status") {
                setFormData({ ...formData, statusTxt: row.shortName, statusId: row.id });
              }
              handleLookupClose();
            }}
            hideTitle={true}
            hideBreadcrumbs={true}
            showAddButton={false}
            actions={false}
            maxHeight={400}
            rowHoverBg="#f0fdf4"
          />
        </Box>
      </Dialog>
    );
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "employeeId", label: translate("Employee ID"), minWidth: 120 },
    { id: "employeeName", label: translate("Employee Name"), minWidth: 200 },
    { id: "leaveType", label: translate("Leave Type"), minWidth: 150 },
    { id: "leaveUnit", label: translate("Leave Unit"), minWidth: 100 },
    { id: "fromDate", label: translate("From Date"), minWidth: 120, format: (val: any) => val ? new Date(val).toLocaleDateString() : "" },
    { id: "tillDate", label: translate("Till Date"), minWidth: 120, format: (val: any) => val ? new Date(val).toLocaleDateString() : "" },
    { id: "leaveDays", label: translate("Leave Days"), minWidth: 100 },
  ];

  const handleSave = async () => {
    setIsSubmitted(true);
    const showInLieuDate = isSpecialLeaveType(getSelectedLeaveType(formData.leaveTypeId));
    const errors = [];
    if (!formData.employeeName)
      errors.push(translate("Employee Name is required"));
    if (!formData.reason) errors.push(translate("Reason is required"));
    if (!formData.leaveUnitId) errors.push(translate("Leave Unit is required"));
    if (!formData.leaveTypeId) errors.push(translate("Leave Type is required"));
    if (showInLieuDate && !formData.inLieuDate)
      errors.push(translate("In Lieu Date is required"));
    if (showChildBirthDate && !formData.childBirthDate)
      errors.push(translate("Child Birth Date is required"));
    if (!formData.fromDate) errors.push(translate("From Date is required"));
    if (!formData.tillDate) errors.push(translate("Till Date is required"));
    const isHourlyRequest = isHourlyLeaveUnit(formData.leaveUnitId);
    const startTime = getTimeInputValue(formData.fromTime);
    const endTime = getTimeInputValue(formData.tillTime);
    if (isHourlyRequest && !startTime) errors.push(translate("Start Time is required"));
    if (isHourlyRequest && !endTime) errors.push(translate("End Time is required"));
    if (isHourlyRequest && startTime && endTime && endTime <= startTime) {
      errors.push(translate("End Time must be after Start Time"));
    }
    const attachmentRows = formData.id ? getAttachmentRows() : [];
    attachmentRows.forEach((attachment: any, index: number) => {
      const rowNo = index + 1;
      if (!attachment.documentTypeId) {
        errors.push(`${translate("Attachment")} ${rowNo}: ${translate("Document Type is required")}`);
      }
      if (!attachment.documentName && !attachment.uploadPath && !attachment.externalLink && !attachment.file?.name) {
        errors.push(`${translate("Attachment")} ${rowNo}: ${translate("Document Name is required")}`);
      }
      const mismatchMessage = getAttachmentTypeMismatchMessage(attachment, rowNo);
      if (mismatchMessage) {
        errors.push(mismatchMessage);
      }
    });

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return;
    }

    setLoading(true);
    try {
      const attachmentPayload = await Promise.all(
        attachmentRows.map(async (attachment: any) => ({
          id: attachment.id || null,
          attachmentNo: attachment.attachmentNo || null,
          documentTypeId: attachment.documentTypeId ? Number(attachment.documentTypeId) : null,
          documentType: getDocumentTypeText(attachment.documentTypeId, attachment.documentType),
          documentName:
            attachment.documentName ||
            attachment.uploadPath ||
            attachment.externalLink ||
            attachment.file?.name ||
            null,
          uploadPath: attachment.file ? null : (attachment.uploadPath || null),
          externalLink: attachment.externalLink || null,
          fileName: attachment.file?.name || null,
          fileContentBase64: attachment.file ? await readFileAsDataUrl(attachment.file) : null,
        })),
      );
      const payload = {
        id: formData.id,
        employeeId: formData.employeeId,
        leaveTypeId: formData.leaveTypeId,
        leaveUnitId: formData.leaveUnitId,
        leaveDays: formData.leaveDays,
        reason: formData.reason,
        remarks: formData.remarks,
        statusId: formData.statusId,
        requestDate: formData.requestDate ? toDateOnly(formData.requestDate) : null,
        childBirthDate: showChildBirthDate && formData.childBirthDate
          ? toDateOnly(formData.childBirthDate)
          : null,
        inLieuDate: formData.inLieuDate ? toDateOnly(formData.inLieuDate) : null,
        fromDate: formData.fromDate ? toDateOnly(formData.fromDate) : null,
        tillDate: formData.tillDate ? toDateOnly(formData.tillDate) : null,
        fromTime: isHourlyRequest ? toDateTime(formData.fromDate, startTime) : null,
        tillTime: isHourlyRequest ? toDateTime(formData.tillDate, endTime) : null,
        attachments: attachmentPayload,
      };

      if (formData.id) {
        await PutService("/api/HumanResource/LeaveRequest/v3", payload);
        setErrorMessages([translate("Leave Request updated successfully")]);
      } else {
        await PostService("/api/HumanResource/LeaveRequest/v3", payload);
        setErrorMessages([translate("Leave Request saved successfully")]);
      }
      void refreshRequestNotifications();
      setToasterSeverity("success");
      setShowError(true);
      setViewMode("list");
      fetchRequests();
    } catch (error: any) {
      const message = getErrorMessage(error, "Failed to save record");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setLoading(true);
    try {
      await DeleteService(`/api/HumanResource/LeaveRequest/v3/${row.id}`);
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("success");
      setShowError(true);
      fetchRequests();
    } catch (error: any) {
      const message = getErrorMessage(error, "Failed to delete record");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setLoading(false);
    }
  };

  const handleMenuOpen = (row: any, event?: React.MouseEvent<HTMLElement>) => {
    setSelectedMenuRow(row);
    setMenuAnchorEl(event?.currentTarget || null);
  };

  const handleMenuClose = () => {
    setMenuAnchorEl(null);
    setSelectedMenuRow(null);
  };

  const handleHistoryClick = () => {
    setErrorMessages([translate("Leave Request history is not available yet")]);
    setToasterSeverity("success");
    setShowError(true);
    handleMenuClose();
  };

  const handleRowReportClick = () => {
    if (!selectedMenuRow) return;

    const entity = entities.find((item: any) => Number(item.id) === Number(selectedMenuRow.entityId));
    const gender = genders.find((item: any) => Number(item.id ?? item.Id) === Number(selectedMenuRow.genderId));
    const designation = designations.find(
      (item: any) => Number(item.id ?? item.designationId ?? item.Id) === Number(selectedMenuRow.designationId),
    );

    setReportInitialFilters({
      entityId: selectedMenuRow.entityId ?? entity?.id ?? null,
      entityName:
        entity?.instituteName ||
        entity?.name ||
        entity?.stxt ||
        "",
      employeeId: selectedMenuRow.employeeId ?? null,
      employeeCode: selectedMenuRow.employeeCode || "",
      genderId: selectedMenuRow.genderId ?? null,
      genderName: selectedMenuRow.genderName || gender?.stxt || gender?.name || gender?.Name || "",
      designationId: selectedMenuRow.designationId ?? null,
      designationName:
        selectedMenuRow.designationName ||
        designation?.stxt ||
        designation?.designationName ||
        designation?.name ||
        designation?.Name ||
        "",
    });
    setViewMode("report");
    handleMenuClose();
  };

  const renderExpandIcon = (expanded: boolean) => (
    <Box
      sx={{
        width: 18,
        height: 18,
        borderRadius: "4px",
        border: "1px solid #E2E8F0",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color: "var(--lms-primary)",
        transition: "all 0.3s ease",
        bgcolor: expanded ? "var(--lms-primary)" : "transparent",
      }}
    >
      <Box
        sx={{
          width: 8,
          height: "2px",
          bgcolor: expanded ? "#fff" : "var(--lms-primary)",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(0deg)" : "rotate(90deg)"
        }}
      />
      {!expanded && (
        <Box
          sx={{
            width: "2px",
            height: 8,
            bgcolor: "var(--lms-primary)",
            position: "absolute"
          }}
        />
      )}
    </Box>
  );

  const renderSectionHeader = (
    title: string,
    expanded: boolean,
    onToggle: () => void,
    isFirst: boolean = false,
  ) => (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        mb: 2,
        mt: isFirst ? 0 : 3,
        cursor: "pointer",
        userSelect: "none",
        borderTop: isFirst ? "none" : "1px solid #E2E8F0",
        pt: 2,
        "&:hover": {
          "& .expand-icon-container": {
            bgcolor: "rgba(var(--lms-primary-rgb), 0.05)",
          },
        },
      }}
      onClick={onToggle}
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
          {translate(title)}
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
        {renderExpandIcon(expanded)}
      </Box>
    </Box>
  );

  const renderForm = (
    formData: any,
    setFormData: any,
    isReadOnly?: boolean,
    isSubmitted?: boolean,
    isEdit?: boolean,
  ) => {
    const showInLieuDate = isSpecialLeaveType(getSelectedLeaveType(formData.leaveTypeId));
    const isHourlyRequest = isHourlyLeaveUnit(formData.leaveUnitId);
    const selectedEmployee = employees.find((employee: any) =>
      String(employee.id ?? employee.Id) === String(formData.employeeId ?? ""),
    );
    const getLookupName = (items: any[], id: any) => {
      const item = items.find((value: any) => String(value.id ?? value.Id) === String(id ?? ""));
      return item?.name ?? item?.Name ?? item?.stxt ?? item?.Stxt ?? "";
    };
    const employeeCardData = selectedEmployee
      ? {
          ...selectedEmployee,
          firstName: selectedEmployee.firstName ?? selectedEmployee.FirstName,
          lastName: selectedEmployee.lastName ?? selectedEmployee.LastName,
          employeeCode: selectedEmployee.employeeCode ?? selectedEmployee.EmployeeCode,
          imageId: selectedEmployee.imageId ?? selectedEmployee.ImageId,
          gender: getGenderText(selectedEmployee.genderId ?? selectedEmployee.GenderId),
          designation: getLookupName(designations, selectedEmployee.designationId ?? selectedEmployee.DesignationId),
          department: getLookupName(departments, selectedEmployee.departmentId ?? selectedEmployee.DepartmentId),
          emailId: selectedEmployee.emailId ?? selectedEmployee.EmailId,
          cellNo: selectedEmployee.cellNo ?? selectedEmployee.CellNo,
        }
      : null;

    const lookupBtn = (key: string) => (
      <InputAdornment position="end">
        <IconButton size="small" onClick={() => handleLookupOpen(key)} disabled={isReadOnly}>
          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
        </IconButton>
      </InputAdornment>
    );

    const attachmentColumns: Column[] = [
      { id: "sNo", label: translate("S.No"), minWidth: 60, align: "center" },
      {
        id: "documentTypeId",
        label: translate("Document Type"),
        minWidth: 220,
        format: (_value: any, attachment: any) => (
          <LookupDropdown
            value={getDocumentTypeDisplayValue(attachment)}
            options={documentTypes}
            getOptionLabel={(item: any) =>
              item?.documentType || item?.DocumentType || item?.name || item?.Name || item?.stxt || item?.Stxt || ""
            }
            onSelect={(item: any) => selectAttachmentDocumentType(attachment.localId, item)}
            onSearchClick={() => setAttachmentDocTypeLookupLocalId(attachment.localId)}
            hideDropdownArrow
            hideLabel
            disabled={isReadOnly}
            placeholder={translate("Select Document Type")}
            sx={attachmentCellControlSx}
          />
        ),
      },
      {
        id: "documentName",
        label: translate("Document Name"),
        minWidth: 220,
        format: (_value: any, attachment: any) => (
          <FormInput
            fullWidth
            size="small"
            disabled={isReadOnly}
            value={attachment.documentName || ""}
            onChange={(e) => updateAttachmentRow(attachment.localId, { documentName: e.target.value })}
            error={isSubmitted && !attachment.documentName && !attachment.uploadPath && !attachment.externalLink}
            sx={attachmentCellControlSx}
          />
        ),
      },
      {
        id: "uploadPath",
        label: translate("Upload Path"),
        minWidth: 260,
        format: (_value: any, attachment: any) => (
          <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
            <FormInput
              fullWidth
              size="small"
              disabled={isReadOnly}
              value={attachment.uploadPath || ""}
              onChange={(e) => updateAttachmentRow(attachment.localId, { uploadPath: e.target.value })}
              sx={attachmentCellControlSx}
            />
            {!isReadOnly && (
              <Button variant="outlined" component="label" size="small" sx={{ minWidth: 38, height: 36, px: 1 }}>
                <UploadFileIcon sx={{ fontSize: 18 }} />
                <EditableImageInput
                  type="file"
                  hidden
                  onChange={(event) => {
                    const file = event.target.files?.[0] ?? null;
                    const nextAttachment = {
                      ...attachment,
                      file,
                      documentName: file?.name || attachment.documentName,
                      uploadPath: file?.name || attachment.uploadPath,
                    };
                    const mismatchMessage = getAttachmentTypeMismatchMessage(nextAttachment);
                    if (mismatchMessage) {
                      setErrorMessages([mismatchMessage]);
                      setToasterSeverity("error");
                      setShowError(true);
                    }
                    updateAttachmentRow(attachment.localId, nextAttachment);
                  }}
                />
              </Button>
            )}
          </Box>
        ),
      },
      {
        id: "externalLink",
        label: translate("External Link"),
        minWidth: 220,
        format: (_value: any, attachment: any) => (
          <FormInput
            fullWidth
            size="small"
            disabled={isReadOnly}
            value={attachment.externalLink || ""}
            onChange={(e) => updateAttachmentRow(attachment.localId, { externalLink: e.target.value })}
            sx={attachmentCellControlSx}
          />
        ),
      },
    ];

    return (
      <FormThemeWrapper>
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {/* Basic Information */}
          {renderSectionHeader(
            "Basic Information",
            isGeneralExpanded,
            () => setIsGeneralExpanded(!isGeneralExpanded),
            true
          )}

          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Request No")}
                  disabled
                  value={formData.requestNo || ""}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Request Date")}
                  disabled
                  value={formData.requestDate || ""}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Employee Name")}
                  required
                  disabled={isReadOnly || isEdit || !isLeaveRequestAdmin}
                  value={formData.employeeId || ""}
                  onChange={(e) => {
                    const selected = employees.find((x: any) =>
                      String(x.id ?? x.Id) === String(e.target.value)
                    );
                    applyEmployeeSelection(selected);
                  }}
                  error={isSubmitted && !formData.employeeId}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          size="small"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleLookupOpen("employee");
                          }}
                          disabled={isReadOnly || isEdit || !isLeaveRequestAdmin}
                        >
                          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {visibleEmployees.map((row: any) => (
                    <MenuItem key={row.id ?? row.Id} value={row.id ?? row.Id}>
                      {row.employeeName || row.EmployeeName || `${row.firstName || row.FirstName || ""} ${row.lastName || row.LastName || ""}`.trim()}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Employee #")}
                  placeholder={translate("Enter Employee #")}
                  disabled={isReadOnly}
                  value={formData.employeeCode || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, employeeCode: e.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12}>
                <FormInput
                  fullWidth
                  multiline
                  rows={3}
                  label={translate("Reason")}
                  required
                  disabled={isReadOnly}
                  value={formData.reason || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, reason: e.target.value })
                  }
                  error={isSubmitted && !formData.reason}
                />
              </Grid>
              {employeeCardData && (
                <Grid item xs={12}>
                  <EmployeeHeaderCard employeeData={employeeCardData} />
                </Grid>
              )}
              {viewMode !== "new" && (
                <Grid item xs={12}>
                  <SchemaTable
                    title={translate("Attachment")}
                    addButtonLabel={translate("New")}
                    breadcrumbPath=""
                    columns={attachmentColumns}
                    rows={getAttachmentRows().map((attachment: any, index: number) => ({
                      ...attachment,
                      rowKey: attachment.localId,
                      sNo: index + 1,
                      documentType: getDocumentTypeText(attachment.documentTypeId, attachment.documentType),
                      documentTypeCode: getDocumentTypeCode(attachment.documentTypeId),
                    }))}
                    hideBreadcrumbs
                    showAddButton={!isReadOnly}
                    onAddClick={addAttachmentRow}
                    actions={!isReadOnly}
                    showEditAction={false}
                    showViewAction={false}
                    onDelete={(row: any) => deleteAttachmentRow(row.localId)}
                    pagination={false}
                    filtersEnabled={false}
                    downloadEnabled={false}
                    maxHeight={360}
                    defaultLatestFirst={false}
                    showToaster={false}
                  />
                </Grid>
              )}
            </Grid>
          </Collapse>

          {/* Leave Detail */}
          {renderSectionHeader("Leave Detail", isLeaveDetailExpanded, () =>
            setIsLeaveDetailExpanded(!isLeaveDetailExpanded),
          )}
          <Collapse in={isLeaveDetailExpanded}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Leave Unit")}
                  required
                  disabled={isReadOnly || isEdit}
                  value={formData.leaveUnitId || ""}
                  onChange={(e) => {
                    const selected = leaveUnits.find((x: any) =>
                      String(x.id ?? x.Id) === String(e.target.value),
                    );
                    applyLeaveUnitSelection(selected ?? { id: e.target.value });
                  }}
                  error={isSubmitted && !formData.leaveUnitId}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          size="small"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleLookupOpen("leaveUnit");
                          }}
                          disabled={isReadOnly || isEdit}
                        >
                          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {leaveUnits.map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>
                      {row.name || row.stxt}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Leave Type")}
                  required
                  disabled={isReadOnly || isEdit}
                  value={formData.leaveTypeId || ""}
                  onChange={(e) => {
                    const selected = leaveTypes.find((x: any) =>
                      String(x.id ?? x.Id) === String(e.target.value)
                    );
                    applyLeaveTypeSelection(selected);
                  }}
                  error={isSubmitted && !formData.leaveTypeId}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          size="small"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleLookupOpen("leaveType");
                          }}
                          disabled={isReadOnly || isEdit}
                        >
                          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {leaveTypes.map((row: any) => (
                    <MenuItem key={row.id ?? row.Id} value={row.id ?? row.Id}>
                      {row.leaveType || row.name || row.stxt}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              {showChildBirthDate && (
                <Grid item xs={12} md={6}>
                  <FormDatePicker
                    label={translate("Child Birth Date")}
                    required
                    disabled={isReadOnly}
                    value={formData.childBirthDate ? dayjs(formData.childBirthDate) : null}
                    error={Boolean(isSubmitted && !formData.childBirthDate)}
                    helperText={
                      isSubmitted && !formData.childBirthDate
                        ? translate("Child Birth Date is required")
                        : ""
                    }
                    onChange={(date) =>
                      setFormData({ ...formData, childBirthDate: date })
                    }
                  />
                </Grid>
              )}
              {showInLieuDate && (
                <Grid item xs={12} md={6}>
                  <FormDatePicker
                    label={translate("In Lieu Date")}
                    required
                    disabled={isReadOnly || isEdit}
                    value={formData.inLieuDate ? dayjs(formData.inLieuDate) : null}
                    onChange={(date) =>
                      setFormData({ ...formData, inLieuDate: date })
                    }
                    error={isSubmitted && !formData.inLieuDate}
                  />
                </Grid>
              )}
              <Grid item xs={12} md={6}>
                <FormDatePicker
                  label={translate("From Date")}
                  required
                  disabled={isReadOnly || isEdit}
                  value={formData.fromDate ? dayjs(formData.fromDate) : null}
                  onChange={(date) =>
                    setFormData({ ...formData, fromDate: date })
                  }
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormDatePicker
                  label={translate("Till Date")}
                  required
                  disabled={isReadOnly || isEdit}
                  value={formData.tillDate ? dayjs(formData.tillDate) : null}
                  onChange={(date) =>
                    setFormData({ ...formData, tillDate: date })
                  }
                />
              </Grid>
              {isHourlyRequest && (
                <>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      type="time"
                      label={translate("Start Time")}
                      required
                      disabled={isReadOnly || isEdit}
                      value={getTimeInputValue(formData.fromTime)}
                      onChange={(e) => setFormData({ ...formData, fromTime: e.target.value })}
                      error={Boolean(isSubmitted && !formData.fromTime)}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      type="time"
                      label={translate("End Time")}
                      required
                      disabled={isReadOnly || isEdit}
                      value={getTimeInputValue(formData.tillTime)}
                      onChange={(e) => setFormData({ ...formData, tillTime: e.target.value })}
                      error={Boolean(isSubmitted && !formData.tillTime)}
                    />
                  </Grid>
                </>
              )}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate(isHourlyRequest ? "Duration (In Minutes)" : "Leave Days")}
                  disabled={true}
                  value={formData.leaveDays || 0}
                />
              </Grid>
            </Grid>
          </Collapse>
        </Box>
      </FormThemeWrapper>
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
              {viewMode === "new"
                ? translate("New Leave Request")
                : viewMode === "edit"
                  ? translate("Edit Leave Request")
                  : viewMode === "report"
                    ? translate("Leave Request Report")
                    : translate("View Leave Request")}
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
                  onClick={handleSave}
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
              <LeaveRequestReportModal
                open={true}
                onClose={() => setViewMode("list")}
                initialFilters={reportInitialFilters}
                lookups={{
                  employees: employees.map((x: any) => ({
                    id: x.id,
                    employeeCode: x.employeeCode,
                    name: `${x.firstName || ""} ${x.lastName || ""}`.trim(),
                  })),
                  genders: genders.map((x: any) => ({
                    id: x.id ?? x.Id,
                    stxt: x.stxt || x.name || x.Name || "",
                  })),
                  designations: designations.map((x: any) => ({
                    id: x.id ?? x.designationId ?? x.Id,
                    stxt:
                      x.designationName ||
                      x.designation ||
                      x.stxt ||
                      x.name ||
                      x.Name ||
                      "",
                  })),
                  leaveTypes: leaveTypes.map((x: any) => ({
                    id: x.id,
                    stxt: x.leaveType || x.name || x.stxt || "",
                  })),
                  leaveUnits: leaveUnits.map((x: any) => ({
                    id: x.id,
                    stxt: x.name || x.stxt || "",
                  })),
                  statuses: statuses.map((x: any) => ({
                    id: x.id,
                    stxt: x.name || x.stxt || "",
                  })),
                  entities,
                  requestNos: rows.map((x: any) => ({
                    id: x.id,
                    code: x.requestNo || "",
                  })),
                }}
                isFullScreenView={true}
              />
            ) : (
              renderForm(formData, setFormData, viewMode === "view", isSubmitted, viewMode === "edit")
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Leave Request")}
          addButtonLabel={translate("New Leave Request")}
          breadcrumbPath={''}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          showAddButton={true}
          showMenuAction={true}
          onMenuAction={handleMenuOpen}
          onAddClick={() => {
            const daysUnit = leaveUnits.find((unit: any) =>
              /^days?$/i.test(unit?.name ?? unit?.Name ?? unit?.stxt ?? unit?.Stxt ?? ""),
            );
            setFormData({
              ...initialFormData,
              leaveUnitId: daysUnit?.id ?? daysUnit?.Id ?? null,
              leaveUnitTxt: daysUnit?.name ?? daysUnit?.Name ?? daysUnit?.stxt ?? daysUnit?.Stxt ?? "",
            });
            setIsSubmitted(false);
            setReportInitialFilters(null);
            setViewMode("new");
          }}
          onEditClick={(row: any) => {
            setFormData({
              ...row,
              attachmentFile: null,
              attachmentChanged: false,
              attachments: (row.attachments || row.Attachments || []).map(normalizeAttachment),
              inLieuDate: row.inLieuDate ? dayjs(row.inLieuDate) : null,
              childBirthDate: row.childBirthDate ? dayjs(row.childBirthDate) : null,
              fromDate: row.fromDate ? dayjs(row.fromDate) : null,
              tillDate: row.tillDate ? dayjs(row.tillDate) : null,
            });
            setIsSubmitted(false);
            setReportInitialFilters(null);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({
              ...row,
              attachmentFile: null,
              attachmentChanged: false,
              attachments: (row.attachments || row.Attachments || []).map(normalizeAttachment),
              inLieuDate: row.inLieuDate ? dayjs(row.inLieuDate) : null,
              childBirthDate: row.childBirthDate ? dayjs(row.childBirthDate) : null,
              fromDate: row.fromDate ? dayjs(row.fromDate) : null,
              tillDate: row.tillDate ? dayjs(row.tillDate) : null,
            });
            setIsSubmitted(false);
            setReportInitialFilters(null);
            setViewMode("view");
          }}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9",
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
              <T>Leave Request Report</T>
            </Button>
          }
          extraToolbarItems={
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <Typography sx={{ fontSize: "0.75rem", fontWeight: 700 }}>
                <T>STATUS</T>
              </Typography>
              <FormInput
                select
                size="small"
                value={filterStatus}
                onChange={(e) => setFilterStatus(e.target.value)}
                sx={{
                  "& .MuiInputBase-root": {
                    height: 32,
                    fontSize: "0.75rem",
                    borderRadius: "8px",
                    width: 120,
                    bgcolor: "#fff",
                  },
                }}
              >
                <MenuItem value="">{translate("All")}</MenuItem>
                <MenuItem value={1}>{translate("Approved")}</MenuItem>
                <MenuItem value={2}>{translate("Pending")}</MenuItem>
                <MenuItem value={3}>{translate("Rejected")}</MenuItem>
              </FormInput>
            </Box>
          }
        />
      )}
      <Menu
        anchorEl={menuAnchorEl}
        open={Boolean(menuAnchorEl)}
        onClose={handleMenuClose}
        PaperProps={{
          sx: {
            boxShadow: "0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)",
            borderRadius: "12px",
            minWidth: "190px",
            border: "1px solid #f1f5f9",
          },
        }}
      >
        <Box sx={{ px: 2, py: 1, borderBottom: "1px solid #f1f5f9" }}>
          <Typography sx={{ fontWeight: 700, fontSize: "0.75rem", color: "#64748b", textTransform: "uppercase", letterSpacing: "0.05em" }}>
            <T>Options</T>
          </Typography>
        </Box>
        <MenuItem
          onClick={handleHistoryClick}
          sx={{ py: 1.5, fontSize: "0.875rem", color: "#16a34a", fontWeight: 500, "&:hover": { bgcolor: "#f0fdf4" } }}
        >
          <HistoryIcon sx={{ fontSize: "1.15rem", mr: 1.5, color: "#16a34a" }} />
          <T>History</T>
        </MenuItem>
        <Box sx={{ px: 2, py: 1, borderTop: "1px solid #f1f5f9", borderBottom: "1px solid #f1f5f9" }}>
          <Typography sx={{ fontWeight: 700, fontSize: "0.75rem", color: "#111827" }}>
            <T>Report</T>
          </Typography>
        </Box>
        <MenuItem
          onClick={handleRowReportClick}
          sx={{ py: 1.5, fontSize: "0.875rem", color: "#16a34a", fontWeight: 500, "&:hover": { bgcolor: "#f0fdf4" } }}
        >
          <AssessmentIcon sx={{ fontSize: "1.15rem", mr: 1.5, color: "#16a34a" }} />
          <T>Leave Request</T>
        </MenuItem>
      </Menu>
      <GenericSearchModal
        open={!!attachmentDocTypeLookupLocalId}
        onClose={() => setAttachmentDocTypeLookupLocalId(null)}
        title={translate("Search Document Type")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: translate("Document Type"), minWidth: 220 },
        ]}
        data={documentTypes.map((item: any) => ({
          ...item,
          id: item.id ?? item.Id,
          code: item.code ?? item.Code ?? item.documentTypeCode ?? item.DocumentTypeCode ?? "",
          name: item.documentType || item.DocumentType || item.name || item.Name || item.stxt || item.Stxt || "",
        }))}
        selectedValue={
          attachmentDocTypeLookupLocalId
            ? getAttachmentRows().find((row: any) => row.localId === attachmentDocTypeLookupLocalId)?.documentTypeId
            : undefined
        }
        selectedField="id"
        onSelect={(row: any) => {
          if (attachmentDocTypeLookupLocalId) {
            selectAttachmentDocumentType(attachmentDocTypeLookupLocalId, row);
          }
          setAttachmentDocTypeLookupLocalId(null);
        }}
      />
      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {renderLookupDialog()}
    </>
  );
};

const attachmentCellControlSx = {
  "& .MuiOutlinedInput-root, & .MuiInputBase-root": {
    minHeight: 36,
    borderRadius: "8px",
    bgcolor: "#fff",
    fontSize: "0.82rem",
    fontWeight: 600,
  },
  "& .MuiInputBase-input": {
    py: "8px !important",
  },
  "& .MuiOutlinedInput-notchedOutline": {
    borderColor: "var(--lms-border)",
  },
  "&:hover .MuiOutlinedInput-notchedOutline": {
    borderColor: "var(--lms-primary)",
  },
};

export default LeaveRequest;
