import React, { useState, useEffect, useCallback } from "react";
import dayjs from "dayjs";
import {
  Grid,
  Box,
  Typography,
  IconButton,
  Dialog,
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
  Visibility as VisibilityIcon,
  Download as DownloadIcon,
} from "@mui/icons-material";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { useTranslation, T } from "@/components/AdaptiveTranslationProvider";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LookupDropdown } from "@/components/LookupDropdown";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
  ToCheckArrayTypeAndLength,
  toDateOnly,
} from "@/constants/globalItems";
import { LeaveTransactionReportModal } from "./LeaveTransactionReportModal";
import { toAbsoluteUrl } from "@/utils/toAbsoluteUrl";
import api from "@/services/api";

interface LeaveTransactionProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const initialFormData = {
  id: null,
  transactionNo: "",
  transactionDate: dayjs().toISOString(),
  requestId: null,
  requestNo: "",
  requestDate: null,
  employeeId: null,
  employeeName: "",
  employeeCode: "",
  statusId: null,
  leaveUnitId: null,
  leaveTypeId: null,
  inLieuDate: null,
  allocated: "",
  availed: "",
  balance: "",
  fromDate: null,
  tillDate: null,
  leaveDays: "",
  admApproval: null,
  paidLeave: false,
  attachmentDocumentName: "",
  attachmentUploadPath: "",
  attachmentExternalLink: "",
  attachments: [] as any[],
};

const LONG_LEAVE_DAYS = 3;
const REGULAR_APPROVAL_STATUS_ID = 582;
const SPECIAL_APPROVAL_STATUS_ID = 1217;
const MONTHLY_ATTENDANCE_PROCESS_API =
  "https://sm.edu-man.com/sm/api/HRMonthlyAttendance/v2/MonthlyAttendanceProcess/3304/6583/null/null/null/null";

const LeaveTransaction: React.FC<LeaveTransactionProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">(
    "list"
  );
  const [formData, setFormData] = useState<any>({ ...initialFormData });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isLeaveDetailExpanded, setIsLeaveDetailExpanded] = useState(true);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );
  const [originalApprovalSnapshot, setOriginalApprovalSnapshot] = useState<any>(null);
  const [showRequestNoField, setShowRequestNoField] = useState(false);
  const [leaveTransactionScope, setLeaveTransactionScope] = useState<any>(null);

  // Lookups
  const [leaveRequests, setLeaveRequests] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);
  const [leaveTypes, setLeaveTypes] = useState<any[]>([]);
  const [leaveUnits, setLeaveUnits] = useState<any[]>([]);
  const [statuses, setStatuses] = useState<any[]>([]);
  const [entities, setEntities] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [employeeCategories, setEmployeeCategories] = useState<any[]>([]);
  const [genders, setGenders] = useState<any[]>([]);

  const [lookupRows, setLookupRows] = useState<any[]>([]);
  const [lookupSearchTerm, setLookupSearchTerm] = useState("");

  const getItems = (res: any) => res?.data?.items || res?.data?.Items || res?.data?.data || res?.data || [];
  const optionId = (row: any) => row?.id ?? row?.Id ?? row?.lovId ?? row?.lovid ?? row?.value;
  const optionCode = (row: any) => row?.code ?? row?.Code ?? row?.value ?? row?.Value ?? "";
  const optionName = (row: any) => row?.name ?? row?.Name ?? row?.stxt ?? row?.Stxt ?? row?.title ?? row?.Title ?? row?.longName ?? row?.LongName ?? "";
  const normalizeOption = (row: any) => ({
    ...row,
    id: optionId(row),
    code: optionCode(row),
    name: optionName(row),
  });
  const normalizeTransaction = (row: any) => ({
    ...row,
    id: row?.id ?? row?.Id ?? null,
    transactionNo: row?.transactionNo ?? row?.TransactionNo ?? "",
    transactionDate: row?.transactionDate ?? row?.TransactionDate ?? null,
    requestId: row?.requestId ?? row?.RequestId ?? null,
    requestNo: row?.requestNo ?? row?.RequestNo ?? "",
    requestDate: row?.requestDate ?? row?.RequestDate ?? null,
    employeeId: row?.employeeId ?? row?.EmployeeId ?? null,
    employeeCode: row?.employeeCode ?? row?.EmployeeCode ?? "",
    employeeName: row?.employeeName ?? row?.EmployeeName ?? "",
    department: row?.department ?? row?.Department ?? "",
    designation: row?.designation ?? row?.Designation ?? "",
    leaveTypeId: row?.leaveTypeId ?? row?.LeaveTypeId ?? null,
    leaveType: row?.leaveType ?? row?.LeaveType ?? row?.leaveTypeTxt ?? row?.LeaveTypeTxt ?? "",
    leaveUnitId: row?.leaveUnitId ?? row?.LeaveUnitId ?? null,
    inLieuDate: row?.inLieuDate ?? row?.InLieuDate ?? row?.inleuedate ?? row?.Inleuedate ?? null,
    allocated: row?.allocated ?? row?.Allocated ?? "",
    availed: row?.availed ?? row?.Availed ?? "",
    balance: row?.balance ?? row?.Balance ?? "",
    fromDate: row?.fromDate ?? row?.FromDate ?? null,
    tillDate: row?.tillDate ?? row?.TillDate ?? null,
    status: row?.status ?? row?.Status ?? row?.statusTxt ?? row?.StatusTxt ?? "",
    statusId:
      String(row?.statusId ?? row?.StatusId ?? row?.lovStatusId ?? row?.LovStatusId ?? "") === String(SPECIAL_APPROVAL_STATUS_ID) &&
      !(row?.admApproval ?? row?.AdmApproval ?? row?.admapproval)
        ? null
        : row?.statusId ?? row?.StatusId ?? row?.lovStatusId ?? row?.LovStatusId ?? null,
    leaveDays: row?.leaveDays ?? row?.LeaveDays ?? row?.leaveDuration ?? row?.LeaveDuration ?? "",
    leaveDuration: row?.leaveDuration ?? row?.LeaveDuration ?? row?.leaveDays ?? row?.LeaveDays ?? "",
    paidLeave: row?.paidLeave ?? row?.PaidLeave ?? false,
    // admApproval maps to ETLeavetrans.admApproval, which is an employee foreign
    // key. Do not substitute the Special Approval LOV/status ID here.
    admApproval:
      row?.admApproval ??
      row?.AdmApproval ??
      row?.admapproval ??
      row?.ceoApproval ??
      row?.CeoApproval ??
      null,
    attachmentDocumentName: row?.attachmentDocumentName ?? row?.AttachmentDocumentName ?? "",
    attachmentUploadPath: row?.attachmentUploadPath ?? row?.AttachmentUploadPath ?? "",
    attachmentExternalLink: row?.attachmentExternalLink ?? row?.AttachmentExternalLink ?? "",
    attachments: row?.attachments ?? row?.Attachments ?? [],
  });
  const normalizeRequest = (row: any) => ({
    ...row,
    id: row?.id ?? row?.Id ?? row?.requestId ?? row?.RequestId,
    requestNo: row?.requestNo ?? row?.RequestNo ?? row?.code ?? row?.Code ?? "",
    requestDate: row?.requestDate ?? row?.RequestDate ?? null,
    employeeId: row?.employeeId ?? row?.EmployeeId ?? null,
    employeeCode: row?.employeeCode ?? row?.EmployeeCode ?? "",
    employeeName: row?.employeeName ?? row?.EmployeeName ?? "",
    leaveTypeId: row?.leaveTypeId ?? row?.LeaveTypeId ?? null,
    leaveUnitId: row?.leaveUnitId ?? row?.LeaveUnitId ?? null,
    inLieuDate: row?.inLieuDate ?? row?.InLieuDate ?? row?.inleuedate ?? row?.Inleuedate ?? null,
    leaveDays: row?.leaveDays ?? row?.LeaveDays ?? "",
    fromDate: row?.fromDate ?? row?.FromDate ?? null,
    tillDate: row?.tillDate ?? row?.TillDate ?? null,
    allocated: row?.allocated ?? row?.Allocated ?? 15,
    availed: row?.availed ?? row?.Availed ?? 5,
    balance: row?.balance ?? row?.Balance ?? 10,
    statusId: row?.statusId ?? row?.StatusId ?? null,
    attachmentDocumentName: row?.attachmentDocumentName ?? row?.AttachmentDocumentName ?? "",
    attachmentUploadPath: row?.attachmentUploadPath ?? row?.AttachmentUploadPath ?? "",
    attachmentExternalLink: row?.attachmentExternalLink ?? row?.AttachmentExternalLink ?? "",
    attachments: row?.attachments ?? row?.Attachments ?? [],
  });

  const getRequestStatusText = (request: any) => String(
    request?.status ??
    request?.Status ??
    request?.statusTxt ??
    request?.StatusTxt ??
    request?.statusName ??
    request?.StatusName ??
    ""
  ).trim().toLowerCase();

  const isPendingLeaveRequest = (request: any) => {
    const statusText = getRequestStatusText(request);
    return !statusText.includes("approved") &&
      !statusText.includes("reject") &&
      !statusText.includes("cancel") &&
      !statusText.includes("closed");
  };
  const decodeJwtPayload = (token: string | null): any => {
    if (!token) return null;
    try {
      const payload = token.split(".")[1];
      if (!payload) return null;
      const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
      const paddedPayload =
        normalizedPayload + "=".repeat((4 - (normalizedPayload.length % 4)) % 4);
      return JSON.parse(atob(paddedPayload));
    } catch {
      return null;
    }
  };

  const getCurrentUser = () => {
    try {
      return JSON.parse(localStorage.getItem("user") || "{}");
    } catch {
      return {};
    }
  };

  const isCurrentUserCeo = () => {
    const user = getCurrentUser();
    const roles = Array.isArray(user?.roles)
      ? user.roles
      : Array.isArray(user?.Roles)
        ? user.Roles
        : [];
    const roleText = [
      ...roles,
      user?.displayRole,
      user?.DisplayRole,
      user?.roleName,
      user?.RoleName,
      user?.role,
      user?.Role,
      user?.designation,
      user?.Designation,
    ]
      .filter(Boolean)
      .join(" ")
      .toLowerCase();
    return roleText.includes("ceo") || roleText.includes("chief executive");
  };

  const getLeaveDuration = (data: any) => {
    const rawDuration =
      data?.leaveDuration ??
      data?.LeaveDuration ??
      data?.leaveDays ??
      data?.LeaveDays ??
      "";
    const duration = Number(rawDuration);
    if (Number.isFinite(duration) && duration > 0) return duration;

    const from = data?.fromDate ? dayjs(data.fromDate) : null;
    const till = data?.tillDate ? dayjs(data.tillDate) : null;
    if (from?.isValid() && till?.isValid() && (till.isAfter(from) || till.isSame(from))) {
      return till.diff(from, "day") + 1;
    }
    return 0;
  };

  const isLongLeave = (data: any) => getLeaveDuration(data) >= LONG_LEAVE_DAYS;

  const getStatusOptionText = (status: any) =>
    `${status?.code ?? status?.Code ?? ""} ${status?.name ?? status?.Name ?? status?.stxt ?? status?.Stxt ?? ""} ${status?.ltxt ?? status?.Ltxt ?? ""}`
      .trim()
      .toLowerCase();

  const isAdminScope = () => {
    const explicitFlag = leaveTransactionScope?.isAdmin ?? leaveTransactionScope?.IsAdmin;
    if (explicitFlag === true || String(explicitFlag).toLowerCase() === "true") {
      return true;
    }

    const scopeText = [
      leaveTransactionScope?.scope,
      leaveTransactionScope?.Scope,
      leaveTransactionScope?.employeeCategory,
      leaveTransactionScope?.EmployeeCategory,
    ]
      .filter(Boolean)
      .join(" ")
      .toLowerCase();

    return scopeText.includes("admin");
  };

  const isManagementScope = () => {
    const explicitFlag = leaveTransactionScope?.isManagement ?? leaveTransactionScope?.IsManagement;
    if (explicitFlag === true || String(explicitFlag).toLowerCase() === "true") {
      return true;
    }

    const scopeText = [
      leaveTransactionScope?.scope,
      leaveTransactionScope?.Scope,
      leaveTransactionScope?.employeeCategory,
      leaveTransactionScope?.EmployeeCategory,
    ]
      .filter(Boolean)
      .join(" ")
      .toLowerCase();

    return scopeText.includes("management");
  };

  const isSpecialApprovedStatus = (status: any) => {
    const text = getStatusOptionText(status);
    return isStatusId(status, SPECIAL_APPROVAL_STATUS_ID) ||
      (text.includes("special") && (text.includes("approved") || text.includes("approval")));
  };

  const isRejectedStatus = (status: any) => getStatusOptionText(status).includes("reject");

  const isStatusId = (status: any, id: number) =>
    String(status?.id ?? status?.Id ?? status?.lovId ?? status?.LovId ?? status?.lovid ?? "") === String(id);

  const isRegularApprovedStatus = (status: any) => {
    const text = getStatusOptionText(status);
    return isStatusId(status, REGULAR_APPROVAL_STATUS_ID) ||
      ((text.includes("approved") || text.includes("approval")) && !text.includes("special"));
  };

  const getSelectedStatus = (statusId: any) =>
    statuses.find((status: any) => String(status.id ?? status.Id) === String(statusId ?? ""));

  const getEffectiveStatusId = (data: any) => {
    const statusId = data?.statusId ?? data?.StatusId;
    const admApproval = data?.admApproval ?? data?.AdmApproval ?? data?.admapproval;
    if (statusId !== null && statusId !== undefined && statusId !== "") return statusId;
    return isSpecialApprovedStatus(getSelectedStatus(admApproval)) ? admApproval : null;
  };

  const getStatusText = (data: any) => {
    const status = getSelectedStatus(getEffectiveStatusId(data));
    return String(
      status?.name ??
        status?.Name ??
        status?.stxt ??
        status?.Stxt ??
        data?.status ??
        data?.Status ??
        "",
    ).toLowerCase();
  };

  const getDisplayStatus = (data: any) => {
    const status = getSelectedStatus(getEffectiveStatusId(data));
    return (
      status?.name ??
      status?.Name ??
      status?.stxt ??
      status?.Stxt ??
      data?.status ??
      data?.Status ??
      ""
    );
  };

  const getApprovalStatuses = () => {
    const explicitApprovalStatuses = statuses.filter((status: any) =>
      isSpecialApprovedStatus(status) || isRegularApprovedStatus(status)
    );
    return explicitApprovalStatuses.length > 0
      ? explicitApprovalStatuses
      : statuses.filter((status: any) => !isRejectedStatus(status));
  };

  const getVisibleStatuses = () =>
    statuses.filter((status: any) =>
      isAdminScope()
        ? !isStatusId(status, REGULAR_APPROVAL_STATUS_ID)
        : !isStatusId(status, SPECIAL_APPROVAL_STATUS_ID)
    );

  const isLineManagerApproved = (data: any) => {
    const statusId = Number(data?.statusId ?? data?.StatusId ?? 0);
    return statusId === REGULAR_APPROVAL_STATUS_ID;
  };

  const hasApprovalValue = (approval: any) =>
    approval !== null && approval !== undefined && approval !== "" && Number(approval) !== 0;

  const isCeoApproved = (data: any) => {
    const approval = data?.admApproval ?? data?.AdmApproval ?? data?.admapproval;
    const statusId = data?.statusId ?? data?.StatusId ?? data?.lovStatusId ?? data?.LovStatusId;
    return isStatusId({ id: statusId }, SPECIAL_APPROVAL_STATUS_ID) || hasApprovalValue(approval);
  };

  const isLeaveFullyApproved = (data: any) =>
    isLongLeave(data)
      ? isLineManagerApproved(data) && isCeoApproved(data)
      : isLineManagerApproved(data);

  const getPendingApprovalMessage = (data: any) => {
    if (!isLongLeave(data)) return "";
    const lineManagerApproved = isLineManagerApproved(data);
    const ceoApproved = isCeoApproved(data);
    if (lineManagerApproved && !ceoApproved) {
      return translate("CEO approval is required to complete this leave approval.");
    }
    if (ceoApproved && !lineManagerApproved) {
      return translate("Line Manager approval is required to complete this leave approval.");
    }
    return "";
  };

  const getEmployeeFullName = (employee: any) =>
    employee?.employeeName ??
    employee?.EmployeeName ??
    `${employee?.firstName ?? employee?.FirstName ?? ""} ${employee?.lastName ?? employee?.LastName ?? ""}`.trim();

  const getEmployeeCode = (employee: any) =>
    employee?.employeeCode ?? employee?.EmployeeCode ?? employee?.code ?? employee?.Code ?? "";

  const getEmployeeDepartmentId = (employee: any) =>
    employee?.departmentId ?? employee?.DepartmentId ?? employee?.depid ?? employee?.DepId ?? null;

  const getEmployeeDepartmentText = (employee: any) =>
    String(
      employee?.departmentName ??
        employee?.DepartmentName ??
        employee?.department ??
        employee?.Department ??
        getLookupText(departments, getEmployeeDepartmentId(employee), ""),
    )
      .trim()
      .toLowerCase();

  const getEmployeeDesignationId = (employee: any) =>
    employee?.designationId ?? employee?.DesignationId ?? employee?.desgid ?? employee?.DesgId ?? null;

  const getEmployeeDesignationText = (employee: any) =>
    String(
      employee?.designationName ??
        employee?.DesignationName ??
        employee?.designationstxt ??
        employee?.DesignationStxt ??
        employee?.designation ??
        employee?.Designation ??
        getLookupText(designations, getEmployeeDesignationId(employee), ""),
    )
      .trim()
      .toLowerCase();

  const isHrDesignationText = (value: any) => {
    const text = String(value ?? "").trim().toLowerCase();
    return /\bhr\b/.test(text) || text.includes("human resource");
  };

  const getScopeReferenceEmployeeId = () => {
    const referenceEmployeeId = Number(
      leaveTransactionScope?.referenceEmployeeId ?? leaveTransactionScope?.ReferenceEmployeeId ?? null,
    );
    return Number.isFinite(referenceEmployeeId) && referenceEmployeeId > 0
      ? referenceEmployeeId
      : null;
  };

  const getScopeEmployee = () => {
    const referenceEmployeeId = getScopeReferenceEmployeeId();
    if (!referenceEmployeeId) return null;
    return employees.find((employee: any) =>
      String(employee.id ?? employee.Id) === String(referenceEmployeeId),
    );
  };

  const getScopeDesignationText = () => {
    const scopeEmployee = getScopeEmployee();
    const designationId =
      leaveTransactionScope?.designationId ??
      leaveTransactionScope?.DesignationId ??
      getEmployeeDesignationId(scopeEmployee);
    const scopeDesignation = leaveTransactionScope?.designation ?? leaveTransactionScope?.Designation ?? "";
    const employeeDesignation = getEmployeeDesignationText(scopeEmployee);
    return String(
      scopeDesignation ||
        employeeDesignation ||
        getLookupText(designations, designationId, ""),
    )
      .trim()
      .toLowerCase();
  };

  const getFilteredLeaveRequests = () => {
    if (!isManagementScope()) return leaveRequests;

    const isHrScope = isHrDesignationText(getScopeDesignationText());
    const departmentId = leaveTransactionScope?.departmentId ?? leaveTransactionScope?.DepartmentId ?? null;
    const departmentText = String(leaveTransactionScope?.department ?? leaveTransactionScope?.Department ?? "")
      .trim()
      .toLowerCase();

    if (!isHrScope && !departmentId && !departmentText) return [];

    return leaveRequests.filter((request: any) => {
      const employee = employees.find((x: any) =>
        String(x.id ?? x.Id) === String(request.employeeId ?? request.EmployeeId ?? ""),
      );
      if (!employee) return false;
      if (isHrScope) {
        return isHrDesignationText(getEmployeeDesignationText(employee));
      }
      if (departmentId) {
        return String(getEmployeeDepartmentId(employee)) === String(departmentId);
      }
      return getEmployeeDepartmentText(employee) === departmentText;
    });
  };

  const getLookupText = (items: any[], id: any, fallback = "-") => {
    if (id === null || id === undefined || id === "") return fallback;
    const item = items.find((x: any) => String(x.id ?? x.Id ?? x.lovid ?? x.lovId) === String(id));
    return item?.name ?? item?.Name ?? item?.stxt ?? item?.Stxt ?? item?.departmentName ?? item?.designationName ?? fallback;
  };

  const getAttachmentExternalUrl = (attachment: any) => {
    const raw =
      attachment?.externalLink ??
      attachment?.ExternalLink ??
      "";
    const normalized = String(raw || "").trim().replace(/\\/g, "/");
    return toAbsoluteUrl(normalized);
  };

  const getAttachmentDownloadUrl = (attachment: any) => {
    const explicitUrl = attachment?.downloadUrl ?? attachment?.DownloadUrl;
    if (explicitUrl) {
      return toAbsoluteUrl(String(explicitUrl).trim().replace(/\\/g, "/"));
    }

    const attachmentId = attachment?.attachmentId ?? attachment?.AttachmentId ?? attachment?.id ?? attachment?.Id;
    const looksLikeAttachment =
      attachment?.attachmentNo !== undefined ||
      attachment?.AttachmentNo !== undefined ||
      attachment?.documentName !== undefined ||
      attachment?.DocumentName !== undefined ||
      attachment?.uploadPath !== undefined ||
      attachment?.UploadPath !== undefined ||
      attachment?.externalLink !== undefined ||
      attachment?.ExternalLink !== undefined;

    const raw =
      (attachmentId && looksLikeAttachment
        ? `/api/HumanResource/LeaveRequest/v3/attachments/${attachmentId}/download`
        : attachment?.uploadPath ?? attachment?.UploadPath ?? "");
    const normalized = String(raw || "").trim().replace(/\\/g, "/");
    return toAbsoluteUrl(normalized);
  };

  const hasAttachmentUrl = (attachment: any) =>
    Boolean(getAttachmentExternalUrl(attachment) || getAttachmentDownloadUrl(attachment));

  const getAttachmentFileName = (attachment: any) => {
    const documentName =
      attachment?.documentName ??
      attachment?.DocumentName ??
      "";
    const fallbackName =
      attachment?.uploadPath ??
      attachment?.UploadPath ??
      attachment?.externalLink ??
      attachment?.ExternalLink ??
      "attachment";
    const fallbackFileName = String(fallbackName).split(/[\\/]/).pop()?.split("?")[0] || "attachment";
    let fileName = String(documentName || fallbackFileName).split(/[\\/]/).pop()?.split("?")[0] || fallbackFileName;
    const hasExtension = /\.[a-z0-9]+$/i.test(fileName);
    const extension = fallbackFileName.match(/(\.[a-z0-9]+)$/i)?.[1] || "";
    if (!hasExtension && extension) {
      fileName += extension;
    }
    return fileName || "attachment";
  };

  const fetchAttachmentBlobUrl = async (attachment: any) => {
    const url = getAttachmentDownloadUrl(attachment);
    if (!url) return "";
    const response = await api.get(url, { responseType: "blob" });
    return URL.createObjectURL(response.data);
  };

  const viewAttachment = async (attachment: any) => {
    try {
      const externalUrl = getAttachmentExternalUrl(attachment);
      if (externalUrl) {
        window.open(externalUrl, "_blank", "noopener,noreferrer");
        return;
      }

      const blobUrl = await fetchAttachmentBlobUrl(attachment);
      if (!blobUrl) return;
      window.open(blobUrl, "_blank", "noopener,noreferrer");
      setTimeout(() => URL.revokeObjectURL(blobUrl), 60000);
    } catch (error: any) {
      setErrorMessages([getErrorMessage(error, "Attachment file was not found")]);
      setToasterSeverity("error");
      setShowError(true);
    }
  };

  const downloadAttachment = async (attachment: any) => {
    try {
      const externalUrl = getAttachmentExternalUrl(attachment);
      const href = externalUrl || await fetchAttachmentBlobUrl(attachment);
      if (!href) return;

      const link = document.createElement("a");
      link.href = href;
      link.target = "_blank";
      link.rel = "noopener noreferrer";
      link.download = getAttachmentFileName(attachment);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      if (!externalUrl) {
        setTimeout(() => URL.revokeObjectURL(href), 60000);
      }
    } catch (error: any) {
      setErrorMessages([getErrorMessage(error, "Attachment file was not found")]);
      setToasterSeverity("error");
      setShowError(true);
    }
  };

  const getErrorMessage = (error: any, fallback: string) => {
    const data = error?.response?.data;
    if (typeof data === "string" && data.trim()) return data;
    if (Array.isArray(data)) return data.filter(Boolean).join(" ");
    if (data?.message) return data.message;
    if (data?.Message) return data.Message;
    if (data?.title) return data.title;
    if (data?.errors) {
      const messages = Object.values(data.errors).flat().filter(Boolean);
      if (messages.length > 0) return messages.join(" ");
    }
    return error?.message || fallback;
  };

  const getEmployeeDetails = (employeeId?: any, employeeCode?: string) => {
    const employee = employees.find((x: any) =>
      String(x.id ?? x.Id) === String(employeeId ?? "") ||
      (!!employeeCode && String(getEmployeeCode(x)) === String(employeeCode))
    );
    return employee
      ? {
          code: getEmployeeCode(employee),
          name: getEmployeeFullName(employee),
          category: employee.employeeCategory ?? employee.EmployeeCategory ?? employee.category ?? employee.Category ?? getLookupText(employeeCategories, employee.employeeCategoryId ?? employee.EmployeeCategoryId),
          gender: employee.gender ?? employee.Gender ?? employee.genderName ?? employee.GenderName ?? getLookupText(genders, employee.genderId ?? employee.GenderId),
          designation: employee.designationName ?? employee.DesignationName ?? employee.designation ?? employee.Designation ?? getLookupText(designations, employee.designationId ?? employee.DesignationId),
          department: employee.departmentName ?? employee.DepartmentName ?? employee.department ?? employee.Department ?? getLookupText(departments, employee.departmentId ?? employee.DepartmentId),
          contact: employee.cellNo ?? employee.CellNo ?? employee.contactNo ?? employee.ContactNo ?? "",
          email: employee.emailId ?? employee.EmailId ?? employee.email ?? employee.Email ?? "",
        }
      : null;
  };

  const renderEmployeeDetailsCard = (data = formData) => {
    const details = getEmployeeDetails(data.employeeId, data.employeeCode);
    if (!data.employeeId && !details) return null;

    const code = details?.code || data.employeeCode || "";
    const name = details?.name || data.employeeName || "";
    const initial = (name || code || "E").trim().charAt(0).toUpperCase();

    return (
      <Box
        sx={{
          border: "1px solid var(--lms-primary)",
          borderRadius: "12px",
          p: 2,
          display: "flex",
          alignItems: "center",
          gap: 3,
          minHeight: 132,
          bgcolor: "#fff",
        }}
      >
        <Box
          sx={{
            width: 92,
            height: 92,
            borderRadius: "50%",
            border: "1px solid #cbd5e1",
            bgcolor: "#eef8fb",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "var(--lms-primary)",
            fontWeight: 900,
            fontSize: "2rem",
            flex: "0 0 auto",
          }}
        >
          {initial}
        </Box>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, mb: 1.5 }}>
            [{code}] - {name}
          </Typography>
          <Grid container spacing={1}>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Category</T>: <b>{details?.category || "-"}</b></Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Gender</T>: <b>{details?.gender || "-"}</b></Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Designation</T>: <b>{details?.designation || "-"}</b></Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Contact</T>: <b>{details?.contact || ""}</b></Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Department</T>: <b>{details?.department || "-"}</b></Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="body2"><T>Email</T>: <b>{details?.email || ""}</b></Typography>
            </Grid>
          </Grid>
        </Box>
      </Box>
    );
  };

  const applyLeaveRequest = (request: any) => {
    const req = normalizeRequest(request);
    setFormData({
      ...formData,
      requestId: req.id,
      requestNo: req.requestNo,
      requestDate: req.requestDate,
      employeeId: req.employeeId,
      employeeName: req.employeeName,
      employeeCode: req.employeeCode,
      leaveTypeId: req.leaveTypeId,
      leaveUnitId: req.leaveUnitId,
      inLieuDate: req.inLieuDate,
      leaveDays: req.leaveDays,
      admApproval: null,
      fromDate: req.fromDate,
      tillDate: req.tillDate,
      allocated: req.allocated,
      availed: req.availed,
      balance: req.balance,
      statusId: req.statusId ?? formData.statusId,
      attachmentDocumentName: req.attachmentDocumentName,
      attachmentUploadPath: req.attachmentUploadPath,
      attachmentExternalLink: req.attachmentExternalLink,
      attachments: req.attachments || req.Attachments || [],
    });
  };

  const fetchTransactions = useCallback(async () => {
    setLoading(true);
    try {
      const response: any = await GetService("/api/HumanResource/LeaveTransaction/v3");
      const items = response.data?.items || response?.data?.data || response?.data || [];
      if (ToCheckArrayTypeAndLength(items)) {
        setRows(items.map((x: any, index: number) => ({
          ...normalizeTransaction(x),
          sNo: index + 1,
        })));
      } else {
        setRows([]);
      }
    } catch (error: any) {
      console.error("Failed to fetch leave transactions", error);
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchRequestNoVisibility = useCallback(async () => {
    try {
      const response: any = await GetService("/api/HumanResource/LeaveTransaction/v3/scoped");
      const scopedData = response?.data?.data || response?.data || {};
      setLeaveTransactionScope(scopedData);
      setShowRequestNoField(Boolean(scopedData?.showRequestNo ?? scopedData?.ShowRequestNo));
    } catch (error) {
      console.error("Failed to fetch leave transaction request visibility", error);
      setLeaveTransactionScope(null);
      setShowRequestNoField(false);
    }
  }, []);

  const fetchLookups = useCallback(async () => {
    try {
      const [lr, tx, emp, lt, lu, st, depRes, desgRes, empCatRes, genderRes] = await Promise.all([
        GetService("/api/HumanResource/LeaveRequest/v3?pageSize=1000"),
        GetService("/api/HumanResource/LeaveTransaction/v3"),
        GetService("/api/HumanResource/EmployeeProfile/v3?pageSize=1000"),
        GetService("/api/HumanResource/LeaveType/v3"),
        GetService("/api/HumanResource/Lov/v3/LVUNT"),
        GetService("/api/HumanResource/Lov/v3/LVSTS"),
        GetService("/api/HumanResource/Lookups/v3/departments"),
        GetService("/api/HumanResource/Lookups/v3/designations"),
        GetService("/api/HumanResource/Lov/v3/ECAT"),
        GetService("/api/HumanResource/Lov/v3/GEND"),
      ]);
      const usedRequestIds = new Set(getItems(tx).map((item: any) => String(item?.requestId ?? item?.RequestId ?? "")).filter(Boolean));
      setLeaveRequests(getItems(lr).map(normalizeRequest).filter((request: any) => isPendingLeaveRequest(request) && !usedRequestIds.has(String(request.id ?? ""))));
      setEmployees(getItems(emp).map((x: any) => ({ ...x, id: x.id ?? x.Id })));
      setLeaveTypes(getItems(lt).map(normalizeOption));
      setLeaveUnits(getItems(lu).map(normalizeOption));
      setStatuses(getItems(st).map(normalizeOption));
      setDepartments((depRes?.data?.items || depRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, departmentName: x.departmentName ?? x.depstxt ?? x.stxt ?? x.name ?? x.Name })));
      setDesignations((desgRes?.data?.items || desgRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, designationName: x.designationName ?? x.stxt ?? x.name ?? x.Name })));
      setEmployeeCategories(getItems(empCatRes).map(normalizeOption));
      setGenders(getItems(genderRes).map(normalizeOption));
      const entityResp: any = await GetService("/api/HumanResource/Lookups/v3/institutions");
      const entityData = entityResp?.data;
      setEntities(entityData?.items || (Array.isArray(entityData) ? entityData : []));
    } catch (error) {
      console.error("Failed to fetch lookups", error);
    }
  }, []);

  useEffect(() => {
    fetchTransactions();
    fetchRequestNoVisibility();
    fetchLookups();
  }, [fetchTransactions, fetchRequestNoVisibility, fetchLookups]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new")
      onSubViewChange(translate("New Leave Transaction"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Leave Transaction"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Leave Transaction"));
    else if (viewMode === "report")
      onSubViewChange(translate("Leave Transaction Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  useEffect(() => {
    if (!formData.statusId || statuses.length === 0) return;
    const isVisible = getVisibleStatuses().some((status: any) =>
      String(status.id ?? status.Id) === String(formData.statusId)
    );
    if (!isVisible) {
      setFormData((prev: any) => ({ ...prev, statusId: null }));
    }
  }, [formData.statusId, leaveTransactionScope, statuses]);

  useEffect(() => {
    if (!isAdminScope() || statuses.length === 0) return;
    const specialApproved =
      statuses.find((status: any) => isStatusId(status, SPECIAL_APPROVAL_STATUS_ID)) ??
      getApprovalStatuses().find(isSpecialApprovedStatus);
    if (!specialApproved) return;

    const specialApprovedId = specialApproved.id ?? specialApproved.Id;
    const selectedStatus = getSelectedStatus(formData.statusId);
    const shouldUseSpecialApproval =
      !formData.statusId || isRegularApprovedStatus(selectedStatus);

    if (shouldUseSpecialApproval && String(formData.statusId ?? "") !== String(specialApprovedId ?? "")) {
      setFormData((prev: any) => ({ ...prev, statusId: specialApprovedId }));
    }
  }, [formData.statusId, leaveTransactionScope, statuses]);

  const handleLookupOpen = (lookupKey: string) => {
    if (lookupKey === "leaveRequest") {
      setLookupRows(getFilteredLeaveRequests().map(x => ({
        ...x,
        id: x.id,
        code: x.requestNo,
        date: x.requestDate ? dayjs(x.requestDate).format("MM/DD/YYYY") : "",
        employeeName: x.employeeName,
      })));
    } else if (lookupKey === "employee") {
      setLookupRows(employees.map(x => ({ id: x.id, code: x.employeeCode, name: `${x.firstName || ""} ${x.lastName || ""}`.trim() })));
    } else if (lookupKey === "leaveType") {
      setLookupRows(leaveTypes.map(x => ({ id: x.id, code: x.code, name: x.leaveType || x.name })));
    } else if (lookupKey === "leaveUnit") {
      setLookupRows(leaveUnits.map(x => ({ id: x.id, code: x.code, name: x.name })));
    } else if (lookupKey === "status") {
      setLookupRows(getVisibleStatuses().map(x => ({ id: x.id, code: x.code, name: x.name })));
    }
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
    setLookupSearchTerm("");
  };

  const LOOKUP_CONFIG: any = {
    leaveRequest: {
      title: translate("Leave Request"),
      subtitle: translate("Search / Leave Request"),
      columns: [
        { id: "code", label: translate("Request No"), minWidth: 100 },
        { id: "date", label: translate("Date"), minWidth: 120 },
        { id: "employeeName", label: translate("Employee"), minWidth: 150 },
      ],
    },
    employee: {
      title: translate("Employee"),
      subtitle: translate("Search / Employee"),
      columns: [
        { id: "code", label: translate("Employee ID"), minWidth: 100 },
        { id: "name", label: translate("Name"), minWidth: 200 },
      ],
    },
    status: {
      title: translate("Status"),
      subtitle: translate("Search / Status"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Status"), minWidth: 150 },
      ],
    },
    leaveType: {
      title: translate("Leave Type"),
      subtitle: translate("Search / Leave Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Leave Type"), minWidth: 200 },
      ],
    },
    leaveUnit: {
      title: translate("Leave Unit"),
      subtitle: translate("Search / Leave Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Leave Unit"), minWidth: 150 },
      ],
    },
  };

  const renderLookupDialog = () => {
    if (!activeLookup || !LOOKUP_CONFIG[activeLookup]) return null;
    const config = LOOKUP_CONFIG[activeLookup];

    const filteredRows = lookupRows.filter((row: any) => {
      const term = lookupSearchTerm.toLowerCase();
      return (
        (row.name && row.name.toLowerCase().includes(term)) ||
        (row.code && row.code.toString().toLowerCase().includes(term)) ||
        (row.employeeName && row.employeeName.toLowerCase().includes(term))
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
        <Box sx={{ px: 3, py: 2, bgcolor: "#fff" }}>
          <SchemaTable
            title={config.title}
            columns={config.columns}
            rows={filteredRows}
            onRowClick={(row) => {
              if (activeLookup === "leaveRequest") {
                const req = leaveRequests.find(x => String(x.id) === String(row.id));
                applyLeaveRequest(req || row);
              } else if (activeLookup === "employee") {
                setFormData({ ...formData, employeeId: row.id, employeeCode: row.code, employeeName: row.name });
              } else if (activeLookup === "status") {
                setFormData({ ...formData, statusId: row.id });
              } else if (activeLookup === "leaveType") {
                setFormData({ ...formData, leaveTypeId: row.id });
              } else if (activeLookup === "leaveUnit") {
                setFormData({ ...formData, leaveUnitId: row.id });
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
    { id: "transactionNo", label: translate("Transaction No"), minWidth: 150 },
    { id: "employeeName", label: translate("Employee Name"), minWidth: 200 },
    { id: "department", label: translate("Department"), minWidth: 160 },
    { id: "designation", label: translate("Designation"), minWidth: 160 },
    { id: "leaveType", label: translate("Leave Type"), minWidth: 150 },
    { id: "status", label: translate("Status"), minWidth: 150 },
  ];

  if (showRequestNoField) {
    columns.splice(2, 0, {
      id: "requestNo",
      label: translate("Request No"),
      minWidth: 150,
    });
  }

  const runMonthlyAttendanceProcess = async () => {
    try {
      await GetService(MONTHLY_ATTENDANCE_PROCESS_API);
      return true;
    } catch (error) {
      console.error("Monthly attendance process failed", error);
      return false;
    }
  };

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = [];
    const selectedStatusId = getEffectiveStatusId(formData);
    const selectedStatus = getSelectedStatus(selectedStatusId);
    const isSpecialApproval = isSpecialApprovedStatus(selectedStatus);
    const approvingEmployeeId = getScopeReferenceEmployeeId();

    if (!formData.transactionDate) errors.push(translate("Transaction Date is required"));
    if (!formData.employeeId) errors.push(translate("Employee is required"));
    if (!formData.leaveUnitId) errors.push(translate("Leave Unit is required"));
    if (!formData.leaveTypeId) errors.push(translate("Leave Type is required"));
    if (!formData.fromDate) errors.push(translate("From Date is required"));
    if (!formData.tillDate) errors.push(translate("Till Date is required"));
    if (!formData.leaveDays) errors.push(translate("Leave Days is required"));
    if (!selectedStatusId) errors.push(translate("Status is required"));
    if (isLongLeave(formData) && isSpecialApproval && !approvingEmployeeId) {
      errors.push(translate("CEO approval requires a valid employee profile for the approving user"));
    }

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return;
    }

    setLoading(true);
    try {
      const wasFullyApproved = originalApprovalSnapshot
        ? isLeaveFullyApproved(originalApprovalSnapshot)
        : false;
      const nextAdmApproval =
        isLongLeave(formData) && isSpecialApproval
          ? approvingEmployeeId
          : isLongLeave(formData) && isCurrentUserCeo() && isCeoApproved(formData)
            ? approvingEmployeeId ?? formData.admApproval ?? null
            : formData.admApproval ?? null;
      const nextStatusId = isSpecialApproval ? null : formData.statusId;
      const payload = {
        ...formData,
        statusId: nextStatusId,
        transactionDate: formData.transactionDate ? toDateOnly(formData.transactionDate) : null,
        fromDate: formData.fromDate ? toDateOnly(formData.fromDate) : null,
        tillDate: formData.tillDate ? toDateOnly(formData.tillDate) : null,
        leaveDays: Number(formData.leaveDays) || 0,
        leaveDuration: getLeaveDuration(formData),
        admApproval: nextAdmApproval,
        ceoApprovalRequired: isLongLeave(formData),
        lineManagerApprovalCompleted: isLineManagerApproved({ ...formData, statusId: nextStatusId }),
        ceoApprovalCompleted: isCeoApproved({ statusId: nextStatusId, admApproval: nextAdmApproval }),
        isFullyApproved:
          isLongLeave({ ...formData, statusId: nextStatusId, admApproval: nextAdmApproval })
            ? isLineManagerApproved({ ...formData, statusId: nextStatusId }) && Boolean(nextAdmApproval)
            : isLineManagerApproved({ ...formData, statusId: nextStatusId }),
      };
      const savedApprovalState = {
        ...formData,
        statusId: nextStatusId,
        admApproval: nextAdmApproval,
        leaveDuration: payload.leaveDuration,
      };

      if (formData.id) {
        await PutService(`/api/HumanResource/LeaveTransaction/v3/${formData.id}`, payload);
        setErrorMessages([translate("Leave Transaction updated successfully")]);
      } else {
        await PostService("/api/HumanResource/LeaveTransaction/v3", payload);
        setErrorMessages([translate("Leave Transaction saved successfully")]);
      }
      const isFullyApprovedAfterSave = isLeaveFullyApproved(savedApprovalState);
      const attendanceProcessCompleted =
        isFullyApprovedAfterSave && !wasFullyApproved
          ? await runMonthlyAttendanceProcess()
          : true;
      const pendingApprovalMessage = getPendingApprovalMessage(savedApprovalState);
      if (pendingApprovalMessage && !isSpecialApproval) {
        setErrorMessages([pendingApprovalMessage]);
        setToasterSeverity("error");
        setShowError(true);
      } else {
        setToasterSeverity("success");
        setShowError(true);
      }
      if (isFullyApprovedAfterSave && !attendanceProcessCompleted) {
        setErrorMessages([
          translate("Leave Transaction saved successfully"),
          translate("Monthly attendance process could not be completed."),
        ]);
        setToasterSeverity("success");
        setShowError(true);
      }
      setOriginalApprovalSnapshot(savedApprovalState);
      setViewMode("list");
      fetchTransactions();
    } catch (error: any) {
      const message = getErrorMessage(error, "Failed to save record");
      if (message.includes("CEO approval is required to complete this leave approval")) {
        setFormData((prev: any) => ({
          ...prev,
          statusId: originalApprovalSnapshot?.statusId ?? null,
          admApproval: originalApprovalSnapshot?.admApproval ?? null,
        }));
      }
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
      await DeleteService(`/api/HumanResource/LeaveTransaction/v3/${row.id}`);
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      fetchTransactions();
    } catch (error: any) {
      const message = getErrorMessage(error, "Failed to delete record");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setLoading(false);
    }
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
        position: "relative",
      }}
    >
      <Box
        sx={{
          width: 8,
          height: "2px",
          bgcolor: expanded ? "#fff" : "var(--lms-primary)",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(0deg)" : "rotate(90deg)",
        }}
      />
      {!expanded && (
        <Box
          sx={{
            width: "2px",
            height: 8,
            bgcolor: "var(--lms-primary)",
            position: "absolute",
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
  ) => {
    const hasRequest = !!formData.requestId;
    const lockRequestFields = isReadOnly || hasRequest;
    const longLeave = isLongLeave(formData);
    const approvingEmployeeId = getScopeReferenceEmployeeId();
    const canEditCeoApproval = longLeave && isCurrentUserCeo() && !!approvingEmployeeId && !isReadOnly;
    const selectedApprovalStatus = getSelectedStatus(getEffectiveStatusId(formData));
    const savedApprovalState = originalApprovalSnapshot ?? (
      isAdminScope() && isSpecialApprovedStatus(selectedApprovalStatus)
        ? { ...formData, statusId: null, admApproval: approvingEmployeeId }
        : formData
    );
    const lineManagerApproved = isLineManagerApproved(savedApprovalState);
    const ceoApproved = isCeoApproved(savedApprovalState);
    const requestOptions = getFilteredLeaveRequests();
    const selectedRequest = requestOptions.find(
      (x: any) => String(x.id) === String(formData.requestId ?? ""),
    );
    const inLieuDate = formData.inLieuDate || selectedRequest?.inLieuDate || null;
    const attachmentText =
      formData.attachmentDocumentName ||
      formData.attachmentUploadPath ||
      formData.attachmentExternalLink ||
      "";
    const attachmentRows = formData.attachments || formData.Attachments || [];
    const lookupBtn = (key: string, disabled = false) => (
      <InputAdornment position="end">
        <IconButton size="small" onClick={(e) => { e.stopPropagation(); handleLookupOpen(key); }} disabled={isReadOnly || disabled}>
          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
        </IconButton>
      </InputAdornment>
    );

    return (
      <FormThemeWrapper>
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {renderEmployeeDetailsCard(formData)}

          {/* General Section */}
          {renderSectionHeader(
            "General",
            isGeneralExpanded,
            () => setIsGeneralExpanded(!isGeneralExpanded),
            true,
          )}

          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Transaction No")}
                  placeholder={translate("Auto-generated if empty")}
                  disabled={isReadOnly || !!formData.id}
                  value={formData.transactionNo || ""}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      transactionNo: e.target.value,
                    })
                  }
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormDatePicker
                  label={translate("Transaction Date")}
                  required
                  disabled={isReadOnly}
                  value={
                    formData.transactionDate
                      ? dayjs(formData.transactionDate)
                      : null
                  }
                  onChange={(date) =>
                    setFormData({ ...formData, transactionDate: date })
                  }
                />
              </Grid>
              {showRequestNoField && (
                <>
                  <Grid item xs={12} md={6}>
                    <LookupDropdown
                      label={translate("Request No")}
                      placeholder={translate("Select Request No")}
                      fullWidth
                      disabled={isReadOnly}
                      value={formData.requestId || ""}
                      options={requestOptions}
                      getOptionLabel={(row: any) => row?.requestNo || row?.RequestNo || ""}
                      onSelect={(row) => {
                        const req = requestOptions.find(x => String(x.id) === String(row?.id));
                        applyLeaveRequest(req || row);
                      }}
                      onChange={(val) => {
                        const req = requestOptions.find(x => String(x.id) === String(val));
                        if (req) applyLeaveRequest(req);
                      }}
                      onSearchClick={() => handleLookupOpen("leaveRequest")}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Request Date")}
                      placeholder={translate("mm/dd/yyyy")}
                      disabled
                      value={formData.requestDate ? dayjs(formData.requestDate).format("MM/DD/YYYY") : ""}
                    />
                  </Grid>
                </>
              )}
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Employee ID")}
                  required
                  disabled={lockRequestFields}
                  value={formData.employeeId || ""}
                  onChange={(e) => {
                    const selected = employees.find(x => x.id === e.target.value);
                    setFormData({
                      ...formData,
                      employeeId: e.target.value,
                      employeeCode: selected ? getEmployeeCode(selected) : "",
                      employeeName: selected ? getEmployeeFullName(selected) : "",
                    });
                  }}
                  error={isSubmitted && !formData.employeeId}
                  InputProps={{ endAdornment: lookupBtn("employee", hasRequest) }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {employees.map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>
                      {getEmployeeCode(row)}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Status")}
                  required
                  disabled={isReadOnly}
                  value={(isAdminScope() ? getEffectiveStatusId(formData) : formData.statusId) || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, statusId: e.target.value })
                  }
                  error={isSubmitted && !(isAdminScope() ? getEffectiveStatusId(formData) : formData.statusId)}
                  InputProps={{ endAdornment: lookupBtn("status") }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {getVisibleStatuses().map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>
                      {row.name}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              {hasRequest && (
                <Grid item xs={12}>
                  <Box
                    sx={{
                      border: "1px solid var(--lms-border)",
                      borderRadius: "8px",
                      p: 2,
                      bgcolor: "#fff",
                    }}
                  >
                    <Typography sx={{ fontSize: "0.9rem", fontWeight: 800, mb: 1.5 }}>
                      <T>Attachment</T>
                    </Typography>
                    <Box sx={{ border: "1px solid var(--lms-border)", borderRadius: "6px", overflow: "hidden" }}>
                      <Grid container sx={{ bgcolor: "#f8f9fa", py: 1, px: 1, borderBottom: "1px solid var(--lms-border)", fontWeight: 700, fontSize: "0.75rem" }}>
                        <Grid item xs={1}><T>Action</T></Grid>
                        {/* <Grid item xs={1}><T>S.No</T></Grid> */}
                        <Grid item xs={2}><T>Document Type</T></Grid>
                        <Grid item xs={3}><T>Document Name</T></Grid>
                        <Grid item xs={3}><T>Upload Path</T></Grid>
                        <Grid item xs={2}><T>External Link</T></Grid>
                      </Grid>
                      {attachmentRows.length === 0 ? (
                        <Box sx={{ py: 3, textAlign: "center", color: "var(--lms-text-secondary)" }}>
                          <Typography sx={{ fontSize: "0.85rem" }}>
                            {attachmentText || translate("No attachments found")}
                          </Typography>
                        </Box>
                      ) : (
                        attachmentRows.map((attachment: any, index: number) => (
                          <Grid key={attachment.id ?? attachment.Id ?? index} container sx={{ px: 1, py: 1, borderTop: index === 0 ? "none" : "1px solid var(--lms-border)" }}>
                            <Grid item xs={1}>
                              <Box sx={{ display: "flex", gap: 0.5 }}>
                                <IconButton
                                  size="small"
                                  disabled={!hasAttachmentUrl(attachment)}
                                  onClick={() => viewAttachment(attachment)}
                                  title={translate("View")}
                                >
                                  <VisibilityIcon sx={{ fontSize: 18, color: hasAttachmentUrl(attachment) ? "var(--lms-primary)" : "#94a3b8" }} />
                                </IconButton>
                                <IconButton
                                  size="small"
                                  disabled={!hasAttachmentUrl(attachment)}
                                  onClick={() => downloadAttachment(attachment)}
                                  title={translate("Download")}
                                >
                                  <DownloadIcon sx={{ fontSize: 18, color: hasAttachmentUrl(attachment) ? "#0284c7" : "#94a3b8" }} />
                                </IconButton>
                              </Box>
                            </Grid>
                            {/* <Grid item xs={1}><Typography sx={{ fontSize: "0.85rem", fontWeight: 700 }}>{index + 1}</Typography></Grid> */}
                            <Grid item xs={2}><Typography sx={{ fontSize: "0.85rem" }}>{attachment.documentType ?? attachment.DocumentType ?? "-"}</Typography></Grid>
                            <Grid item xs={3}><Typography sx={{ fontSize: "0.85rem" }}>{attachment.documentName ?? attachment.DocumentName ?? "-"}</Typography></Grid>
                            <Grid item xs={3}><Typography sx={{ fontSize: "0.85rem" }}>{attachment.uploadPath ?? attachment.UploadPath ?? "-"}</Typography></Grid>
                            <Grid item xs={2}><Typography sx={{ fontSize: "0.85rem" }}>{attachment.externalLink ?? attachment.ExternalLink ?? "-"}</Typography></Grid>
                          </Grid>
                        ))
                      )}
                    </Box>
                  </Box>
                </Grid>
              )}
            </Grid>
          </Collapse>

          {/* Leave Detail Section */}
          {renderSectionHeader(
            "Leave Detail",
            isLeaveDetailExpanded,
            () => setIsLeaveDetailExpanded(!isLeaveDetailExpanded),
          )}
          <Collapse in={isLeaveDetailExpanded}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Allocated")}
                  disabled
                  value={formData.allocated ?? ""}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Availed")}
                  disabled
                  value={formData.availed ?? ""}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Balance")}
                  disabled
                  value={formData.balance ?? ""}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Leave Unit")}
                  required
                  disabled={isReadOnly}
                  value={formData.leaveUnitId || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, leaveUnitId: e.target.value })
                  }
                  error={isSubmitted && !formData.leaveUnitId}
                  InputProps={{ endAdornment: lookupBtn("leaveUnit") }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {leaveUnits.map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>
                      {row.name}
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
                  disabled={lockRequestFields}
                  value={formData.leaveTypeId || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, leaveTypeId: e.target.value })
                  }
                  error={isSubmitted && !formData.leaveTypeId}
                  InputProps={{ endAdornment: lookupBtn("leaveType", hasRequest) }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {leaveTypes.map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>
                      {row.leaveType || row.name || row.stxt}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>
              {inLieuDate && (
                <Grid item xs={12} md={6}>
                  <FormDatePicker
                    label={translate("In Lieu Date")}
                    disabled
                    value={dayjs(inLieuDate)}
                  />
                </Grid>
              )}
              <Grid item xs={12} md={6}>
                <FormDatePicker
                  label={translate("From Date")}
                  required
                  disabled={lockRequestFields}
                  value={formData.fromDate ? dayjs(formData.fromDate) : null}
                  onChange={(date) =>
                    setFormData({ ...formData, fromDate: date })
                  }
                  error={isSubmitted && !formData.fromDate}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormDatePicker
                  label={translate("Till Date")}
                  required
                  disabled={lockRequestFields}
                  value={formData.tillDate ? dayjs(formData.tillDate) : null}
                  onChange={(date) =>
                    setFormData({ ...formData, tillDate: date })
                  }
                  error={isSubmitted && !formData.tillDate}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormInput
                  fullWidth
                  size="small"
                  type="number"
                  label={translate("Leave Days")}
                  disabled={lockRequestFields}
                  value={formData.leaveDays ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, leaveDays: e.target.value })
                  }
                  error={isSubmitted && !formData.leaveDays}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.paidLeave || false}
                  disabled={isReadOnly}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      paidLeave: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  label={
                    <Typography sx={{ fontSize: "0.85rem", fontWeight: 500 }}>
                      <T>Paid Leave</T>
                    </Typography>
                  }
                />
              </Grid>
              {longLeave && (
                <>
                  <Grid item xs={12} md={4}>
                    <Typography variant="body2" sx={{ fontWeight: 700, color: "var(--lms-text-main)" }}>
                      <T>Line Manager Approval</T>
                    </Typography>
                    <Typography variant="body2" sx={{ color: lineManagerApproved ? "#166534" : "#92400e", fontWeight: 800 }}>
                      {lineManagerApproved ? translate("Approved") : translate("Pending")}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Typography variant="body2" sx={{ fontWeight: 700, color: "var(--lms-text-main)" }}>
                      <T>CEO Approval</T>
                    </Typography>
                    <Typography variant="body2" sx={{ color: ceoApproved ? "#166534" : "#92400e", fontWeight: 800 }}>
                      {ceoApproved ? translate("Approved") : translate("Pending")}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <LmsCheckbox
                      checked={ceoApproved}
                      disabled
                      onChange={(checked) =>
                        setFormData({
                          ...formData,
                          admApproval: checked ? approvingEmployeeId : null,
                        })
                      }
                      formControlSx={{ m: 0 }}
                      label={
                        <Typography sx={{ fontSize: "0.85rem", fontWeight: 500 }}>
                          <T>CEO Approved</T>
                        </Typography>
                      }
                    />
                  </Grid>
                </>
              )}
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
                ? translate("New Leave Transaction")
                : viewMode === "edit"
                  ? translate("Edit Leave Transaction")
                  : viewMode === "report"
                    ? translate("Leave Transaction Report")
                    : translate("View Leave Transaction")}
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
              <LeaveTransactionReportModal
                open={true}
                onClose={() => setViewMode("list")}
                isFullScreenView={true}
                lookups={{
                  employees: employees.map((x: any) => ({ id: x.id, employeeCode: x.employeeCode || "", stxt: x.employeeCode || "" })),
                  departments,
                  designations,
                  entities,
                }}
              />
            ) : (
              renderForm(formData, setFormData, viewMode === "view", isSubmitted)
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title="Leave Transaction"
          addButtonLabel={translate("New Leave Transaction")}
          breadcrumbPath=""
          columns={columns}
          rows={rows.map((row: any) => ({ ...row, status: getDisplayStatus(row) }))}
          onDelete={handleDelete}
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
              <T>Leave Transaction Report</T>
            </Button>
          }
          showAddButton={true}
          onAddClick={() => {
            setFormData({ ...initialFormData });
            setOriginalApprovalSnapshot(null);
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => {
            const normalizedRow = normalizeTransaction(row);
            setFormData({
              ...normalizedRow,
              transactionDate: dayjs(normalizedRow.transactionDate),
            });
            setOriginalApprovalSnapshot(normalizedRow);
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            const normalizedRow = normalizeTransaction(row);
            setFormData({
              ...normalizedRow,
              transactionDate: dayjs(normalizedRow.transactionDate),
            });
            setOriginalApprovalSnapshot(normalizedRow);
            setIsSubmitted(false);
            setViewMode("view");
          }}
        />
      )}
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

export default LeaveTransaction;
