import TextField from "@/components/ClearableTextField";
import React from "react";
import { Box, Button, Chip, Collapse, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, MenuItem, Popover, Skeleton, Stack, Typography } from "@mui/material";
import { Add as AddIcon, Close as CloseIcon, Remove as RemoveIcon } from "@mui/icons-material";
import dayjs from "dayjs";
import { GetService, PostService, PutService, toDateOnly } from "@/constants/globalItems";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { SchemaTable } from "@/components/SchemaTable";
import { useNavigate } from "react-router-dom";

interface RequestNotificationSummaryPopoverProps {
  anchorEl: HTMLElement | null;
  onClose: () => void;
  onMouseEnter?: () => void;
  onMouseLeave?: () => void;
  onShowList: () => void;
}

interface RequestNotificationsScreenProps {
  open?: boolean;
  onClose?: () => void;
  sidebarOpen?: boolean;
  isAnyPinned?: boolean;
  drawerWidth?: number;
  pageMode?: boolean;
}

type LeaveStatusFilter = "all" | "pending" | "approved" | "rejected";

type LeaveNotificationRow = {
  id: number;
  transactionNo: string;
  requestId: number | null;
  requestNo: string;
  transactionDate: string | null;
  employeeId: number | null;
  employeeName: string;
  leaveUnitId: number | null;
  leaveTypeId: number | null;
  leaveType: string;
  fromDate: string | null;
  tillDate: string | null;
  leaveDays: number;
  paidLeave: boolean;
  statusId: number | null;
  status: string;
  admApproval: number | null;
  remarks: string;
  ceoApprovalRequired: boolean;
  lineManagerApprovalCompleted: boolean;
  ceoApprovalCompleted: boolean;
  isFullyApproved: boolean;
  source?: "request" | "transaction";
};

const APPROVED_STATUS_ID = 582;
const REJECTED_STATUS_ID = 583;
const LONG_LEAVE_DAYS = 7;
// Login only needs notification metadata, not attachment payloads or unbounded lists.
const SCOPED_LEAVE_API = "/api/HumanResource/LeaveTransaction/v3/scoped?includeAttachments=false";
const LEAVE_REQUEST_API = "/api/HumanResource/LeaveRequest/v3?pageNumber=1&pageSize=250";
const EMPLOYEE_API = "/api/HumanResource/EmployeeProfile/v3?pageNumber=1&pageSize=250";
const LEAVE_TRANSACTION_API = "/api/HumanResource/LeaveTransaction/v3";
const LOAN_WORKFLOW_QUEUE_API = "/api/HumanResource/LoanWorkflow/v3/queue?view=pending";

const normalizeLoanWorkflowRows = (response: any) => {
  const payload = response?.data ?? response ?? [];
  return (Array.isArray(payload) ? payload : []).map((item: any) => ({
    requestId: Number(item?.requestId ?? item?.RequestId ?? 0),
    requestNo: item?.requestNo ?? item?.RequestNo ?? "",
    employeeName: item?.employeeName ?? item?.EmployeeName ?? "",
    loanType: item?.loanType ?? item?.LoanType ?? "",
    appliedAmount: item?.appliedAmount ?? item?.AppliedAmount ?? 0,
    stage: item?.stage ?? item?.Stage ?? "",
  }));
};

const fetchLoanWorkflowNotifications = async () => {
  if (!canLoadLoanWorkflowNotifications()) return [];
  return normalizeLoanWorkflowRows(await GetService(LOAN_WORKFLOW_QUEUE_API));
};

const useLoanWorkflowNotifications = (enabled: boolean) => {
  const [loanRows, setLoanRows] = React.useState<any[]>([]);
  const [loanLoading, setLoanLoading] = React.useState(false);
  React.useEffect(() => {
    if (!enabled || !hasAuthenticatedUser()) return;
    let active = true;
    setLoanLoading(true);
    fetchLoanWorkflowNotifications()
      .then((rows: any[]) => {
        if (!active) return;
        setLoanRows(rows);
        if (rows.length > 0) setRequestNotificationAccessCache(true);
      })
      .catch(() => { if (active) setLoanRows([]); })
      .finally(() => { if (active) setLoanLoading(false); });
    return () => { active = false; };
  }, [enabled]);
  return { loanRows, loanLoading };
};

const parseNumber = (value: any) => {
  const parsed = Number(value ?? 0);
  return Number.isFinite(parsed) ? parsed : 0;
};

const normalizeDate = (value: any) => value ? String(value).slice(0, 10) : null;

const getPayload = (response: any) => response?.data?.data || response?.data || response || {};
const getItems = (payload: any) => payload?.items || payload?.Items || [];

const getApiErrorMessage = (error: any, fallback: string) => {
  const data = error?.response?.data;
  if (typeof data === "string" && data.trim()) return data.trim();
  if (typeof data?.error === "string" && data.error.trim()) return data.error.trim();
  if (typeof data?.message === "string" && data.message.trim()) return data.message.trim();
  if (Array.isArray(data?.errors)) return data.errors.filter(Boolean).join("\n");
  if (typeof error?.message === "string" && error.message.trim()) return error.message.trim();
  return fallback;
};
const normalizeLeaveRow = (row: any): LeaveNotificationRow => ({
  id: parseNumber(row?.id ?? row?.Id),
  transactionNo: row?.transactionNo ?? row?.TransactionNo ?? "",
  requestId: row?.requestId ?? row?.RequestId ?? null,
  requestNo: row?.requestNo ?? row?.RequestNo ?? "",
  transactionDate: normalizeDate(row?.transactionDate ?? row?.TransactionDate),
  employeeId: row?.employeeId ?? row?.EmployeeId ?? null,
  employeeName: row?.employeeName ?? row?.EmployeeName ?? "",
  leaveUnitId: row?.leaveUnitId ?? row?.LeaveUnitId ?? null,
  leaveTypeId: row?.leaveTypeId ?? row?.LeaveTypeId ?? null,
  leaveType: row?.leaveType ?? row?.LeaveType ?? "",
  fromDate: normalizeDate(row?.fromDate ?? row?.FromDate),
  tillDate: normalizeDate(row?.tillDate ?? row?.TillDate),
  leaveDays: parseNumber(row?.leaveDays ?? row?.LeaveDays),
  paidLeave: Boolean(row?.paidLeave ?? row?.PaidLeave),
  statusId: row?.statusId ?? row?.StatusId ?? null,
  status: row?.status ?? row?.Status ?? "",
  admApproval: row?.admApproval ?? row?.AdmApproval ?? row?.admapproval ?? null,
  remarks: row?.remarks ?? row?.Remarks ?? "",
  ceoApprovalRequired: Boolean(row?.ceoApprovalRequired ?? row?.CeoApprovalRequired),
  lineManagerApprovalCompleted: Boolean(row?.lineManagerApprovalCompleted ?? row?.LineManagerApprovalCompleted),
  ceoApprovalCompleted: Boolean(row?.ceoApprovalCompleted ?? row?.CeoApprovalCompleted),
  isFullyApproved: Boolean(row?.isFullyApproved ?? row?.IsFullyApproved),
  source: "transaction",
});

const normalizeLeaveRequestRow = (row: any): LeaveNotificationRow => ({
  id: parseNumber(row?.id ?? row?.Id),
  transactionNo: "",
  requestId: parseNumber(row?.id ?? row?.Id),
  requestNo: row?.requestNo ?? row?.RequestNo ?? row?.code ?? row?.Code ?? "",
  transactionDate: normalizeDate(row?.requestDate ?? row?.RequestDate),
  employeeId: row?.employeeId ?? row?.EmployeeId ?? null,
  employeeName: row?.employeeName ?? row?.EmployeeName ?? "",
  leaveUnitId: row?.leaveUnitId ?? row?.LeaveUnitId ?? null,
  leaveTypeId: row?.leaveTypeId ?? row?.LeaveTypeId ?? null,
  leaveType: row?.leaveType ?? row?.LeaveType ?? row?.leaveTypeTxt ?? row?.LeaveTypeTxt ?? "",
  fromDate: normalizeDate(row?.fromDate ?? row?.FromDate),
  tillDate: normalizeDate(row?.tillDate ?? row?.TillDate),
  leaveDays: parseNumber(row?.leaveDays ?? row?.LeaveDays),
  paidLeave: Boolean(row?.paidLeave ?? row?.PaidLeave),
  statusId: row?.statusId ?? row?.StatusId ?? null,
  status: row?.status ?? row?.Status ?? row?.statusTxt ?? row?.StatusTxt ?? "Pending",
  admApproval: null,
  remarks: row?.remarks ?? row?.Remarks ?? "",
  ceoApprovalRequired: parseNumber(row?.leaveDays ?? row?.LeaveDays) > LONG_LEAVE_DAYS,
  lineManagerApprovalCompleted: false,
  ceoApprovalCompleted: false,
  isFullyApproved: false,
  source: "request",
});

const getEmployeeId = (employee: any) => employee?.id ?? employee?.Id;
const getEmployeeDepartmentId = (employee: any) => employee?.departmentId ?? employee?.DepartmentId ?? employee?.deptId ?? employee?.DeptId;
const getEmployeeDesignationText = (employee: any) => String(
  employee?.designationName ?? employee?.DesignationName ?? employee?.designation ?? employee?.Designation ?? ""
).trim().toLowerCase();
const isHrDesignationText = (value: string) => value.includes("human resource") || value === "hr" || value.includes(" hr") || value.includes("hr ");
const getStatusText = (row: LeaveNotificationRow) => String(row.status || "").trim().toLowerCase();
const isRequestPendingForApproval = (row: LeaveNotificationRow) => {
  const statusText = getStatusText(row);
  return row.statusId !== APPROVED_STATUS_ID &&
    row.statusId !== REJECTED_STATUS_ID &&
    !statusText.includes("approved") &&
    !statusText.includes("reject") &&
    !statusText.includes("cancel") &&
    !statusText.includes("closed");
};

const filterRequestsByScope = (rows: LeaveNotificationRow[], scope: any, employees: any[]) => {
  const isAdmin = Boolean(scope?.isAdmin ?? scope?.IsAdmin) || String(scope?.scope ?? scope?.Scope ?? "").toLowerCase().includes("admin");
  if (isAdmin) return rows;

  const isManagement = Boolean(scope?.isManagement ?? scope?.IsManagement) || String(scope?.scope ?? scope?.Scope ?? "").toLowerCase().includes("management");
  if (!isManagement) return rows.filter((row) => String(row.employeeId ?? "") === String(scope?.referenceEmployeeId ?? scope?.ReferenceEmployeeId ?? ""));

  const departmentId = scope?.departmentId ?? scope?.DepartmentId ?? null;
  const scopeDesignation = String(scope?.designation ?? scope?.Designation ?? "").trim().toLowerCase();

  return rows.filter((row) => {
    const employee = employees.find((item) => String(getEmployeeId(item)) === String(row.employeeId ?? ""));
    if (!employee) return false;
    if (isHrDesignationText(scopeDesignation)) return isHrDesignationText(getEmployeeDesignationText(employee));
    if (departmentId !== null && departmentId !== undefined && departmentId !== "") {
      return String(getEmployeeDepartmentId(employee)) === String(departmentId);
    }
    return true;
  });
};
const getCurrentUser = () => {
  try { return JSON.parse(localStorage.getItem("user") || "{}"); }
  catch { return {}; }
};

const canLoadLoanWorkflowNotifications = () => {
  if (!hasAuthenticatedUser()) return false;

  // Menu permissions are cached per user after login.  Once they confirm that
  // Human Resource is not available for this user/client, do not call the
  // optional HR-only queue from the shared notification popover.
  const user = getCurrentUser();
  const userId = Number(user?.userid ?? user?.userId ?? user?.UserId ?? 0);
  if (!Number.isFinite(userId) || userId <= 0) return true;

  try {
    const cachedGroups = localStorage.getItem(`edxs_visible_menu_card_groups_v3_${userId}`);
    if (!cachedGroups) return true;
    const groups = JSON.parse(cachedGroups);
    if (!Array.isArray(groups) || groups.length === 0) return true;
    return groups.some((group) => String(group?.moduleName ?? group?.ModuleName ?? "")
      .trim()
      .toLowerCase() === "human resource");
  } catch {
    // A malformed local cache must never block the normal notification flow.
    return true;
  }
};

const getScopeReferenceEmployeeId = (scope: any) => {
  const employeeId = Number(scope?.referenceEmployeeId ?? scope?.ReferenceEmployeeId ?? 0);
  return Number.isFinite(employeeId) && employeeId > 0 ? employeeId : null;
};

const isCurrentUserCeo = () => {
  const user = getCurrentUser();
  const roles = Array.isArray(user?.roles) ? user.roles : Array.isArray(user?.Roles) ? user.Roles : [];
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
  ].filter(Boolean).join(" ").toLowerCase();
  return roleText.includes("ceo") || roleText.includes("chief executive");
};

const hasAuthenticatedUser = () => Boolean(localStorage.getItem("accessToken"));
const getScopedResponseScope = (payload: any) => String(payload?.scope ?? payload?.Scope ?? payload?.data?.scope ?? payload?.Data?.Scope ?? "").trim();
export const canShowRequestNotificationsForScope = (scope: any) => {
  const normalizedScope = String(scope ?? "").trim().toLowerCase();
  return normalizedScope === "admin" || normalizedScope === "management";
};

let requestNotificationAccessCache: boolean | null = null;
const requestNotificationAccessListeners = new Set<(canView: boolean) => void>();
const setRequestNotificationAccessCache = (canView: boolean) => {
  requestNotificationAccessCache = canView;
  requestNotificationAccessListeners.forEach((listener) => listener(canView));
};
export const getCachedRequestNotificationAccess = () => requestNotificationAccessCache;
export const subscribeRequestNotificationAccess = (listener: (canView: boolean) => void) => {
  requestNotificationAccessListeners.add(listener);
  return () => { requestNotificationAccessListeners.delete(listener); };
};
const isLongLeave = (row: LeaveNotificationRow) => (row.leaveDays || 0) > LONG_LEAVE_DAYS || row.ceoApprovalRequired;
const isApproved = (row: LeaveNotificationRow) => row.statusId === APPROVED_STATUS_ID || String(row.status).toLowerCase().includes("approved");
const isRejected = (row: LeaveNotificationRow) => row.statusId === REJECTED_STATUS_ID || String(row.status).toLowerCase().includes("reject");
const getLeaveStatusKey = (row: LeaveNotificationRow): Exclude<LeaveStatusFilter, "all"> => {
  if (isRejected(row)) return "rejected";
  if (row.isFullyApproved || (!isLongLeave(row) && isApproved(row))) return "approved";
  return "pending";
};
const getLeaveStatusText = (row: LeaveNotificationRow, translate: (value: string) => string) => {
  const statusKey = getLeaveStatusKey(row);
  if (statusKey === "rejected") return translate("Rejected");
  if (statusKey === "approved") return translate("Approved");
  return translate("Pending");
};
const isNewRequest = (row: LeaveNotificationRow) => getLeaveStatusKey(row) === "pending";
type LeaveNotificationsResult = { scope: any; rows: LeaveNotificationRow[] };

let leaveNotificationsCache: (LeaveNotificationsResult & { timestamp: number }) | null = null;
let leaveNotificationsRequest: Promise<LeaveNotificationsResult> | null = null;
const leaveNotificationsListeners = new Set<(result: LeaveNotificationsResult) => void>();

const emitLeaveNotificationsChange = (result: LeaveNotificationsResult) => {
  leaveNotificationsListeners.forEach((listener) => listener(result));
};

const subscribeLeaveNotifications = (listener: (result: LeaveNotificationsResult) => void) => {
  leaveNotificationsListeners.add(listener);
  return () => { leaveNotificationsListeners.delete(listener); };
};

const fetchLeaveNotifications = async (force = false) => {
  if (!hasAuthenticatedUser()) {
    leaveNotificationsCache = null;
    leaveNotificationsRequest = null;
    setRequestNotificationAccessCache(false);
    return { scope: null, rows: [] };
  }

  if (!force && leaveNotificationsCache) {
    return { scope: leaveNotificationsCache.scope, rows: leaveNotificationsCache.rows };
  }

  if (!force && leaveNotificationsRequest) return leaveNotificationsRequest;

  // Resolve authorization/scope first. Most users do not need the two expensive
  // approval lookups, so do not issue them during login unless they can use them.
  leaveNotificationsRequest = GetService(SCOPED_LEAVE_API)
    .then(async (scopeResponse: any) => {
      const payload = getPayload(scopeResponse);
      const canView = canShowRequestNotificationsForScope(getScopedResponseScope(payload));
      if (canView) setRequestNotificationAccessCache(true);

      if (!canView) {
        const result = { scope: payload, rows: [] };
        leaveNotificationsCache = { ...result, timestamp: Date.now() };
        return result;
      }

      const [requestResponse, employeeResponse] = await Promise.all([
        GetService(LEAVE_REQUEST_API),
        GetService(EMPLOYEE_API),
      ]);

      const transactionRows: LeaveNotificationRow[] = getItems(payload).map(normalizeLeaveRow);
      const transactionRequestIds = new Set(
        transactionRows
          .map((row: LeaveNotificationRow) => row.requestId)
          .filter((id: number | null): id is number => id !== null && id !== undefined),
      );
      const employees = getItems(getPayload(employeeResponse));
      const rawRequestRows: LeaveNotificationRow[] = getItems(getPayload(requestResponse)).map(normalizeLeaveRequestRow);
      const pendingRequestRows = filterRequestsByScope(rawRequestRows, payload, employees)
        .filter((row: LeaveNotificationRow) => isRequestPendingForApproval(row) && !transactionRequestIds.has(row.requestId ?? 0));
      const decidedTransactionRows = transactionRows.filter((row: LeaveNotificationRow) => isApproved(row) || isRejected(row));
      const result = { scope: payload, rows: [...pendingRequestRows, ...decidedTransactionRows] };
      leaveNotificationsCache = { ...result, timestamp: Date.now() };
      return result;
    })
    .catch((error: any) => { throw error; })
    .finally(() => {
      leaveNotificationsRequest = null;
    });

  return leaveNotificationsRequest;
};
export const preloadRequestNotifications = async (force = false) => {
  const leaveResult = await fetchLeaveNotifications(force).catch(() => ({ scope: null, rows: [] as LeaveNotificationRow[] }));
  const loanRows = await fetchLoanWorkflowNotifications().catch(() => []);
  const canViewLeave = canShowRequestNotificationsForScope(getScopedResponseScope(leaveResult.scope));
  setRequestNotificationAccessCache(canViewLeave || loanRows.length > 0);
  return leaveResult;
};

export const refreshRequestNotifications = async () => {
  leaveNotificationsCache = null;
  const result = await preloadRequestNotifications(true);
  emitLeaveNotificationsChange(result);
  return result;
};

const useLeaveNotifications = (active: boolean) => {
  const { translate } = useTranslation();
  const [loading, setLoading] = React.useState(false);
  const [scope, setScope] = React.useState<any>(leaveNotificationsCache?.scope ?? null);
  const [leaveRows, setLeaveRows] = React.useState<LeaveNotificationRow[]>(leaveNotificationsCache?.rows ?? []);
  const [toaster, setToaster] = React.useState<{ show: boolean; messages: string[]; severity: "success" | "error" }>({ show: false, messages: [], severity: "success" });

  const openToaster = React.useCallback((messages: string[], severity: "success" | "error") => {
    setToaster({ show: true, messages, severity });
  }, []);

  const loadLeaveRequests = React.useCallback(async (force = false) => {
    if (!active || !hasAuthenticatedUser()) return;
    setLoading(true);
    try {
      const result = await fetchLeaveNotifications(force);
      setScope(result.scope);
      setLeaveRows(result.rows);
    } catch (error: any) {
      openToaster([getApiErrorMessage(error, translate("Failed to load leave requests"))], "error");
      setLeaveRows([]);
    } finally {
      setLoading(false);
    }
  }, [active, openToaster, translate]);

  React.useEffect(() => { void loadLeaveRequests(); }, [loadLeaveRequests]);

  React.useEffect(() => {
    if (!active) return undefined;
    return subscribeLeaveNotifications((result) => {
      setScope(result.scope);
      setLeaveRows(result.rows);
    });
  }, [active]);

  const updateLeaveRows = React.useCallback((updater: React.SetStateAction<LeaveNotificationRow[]>) => {
    setLeaveRows((prev) => {
      const next = typeof updater === "function" ? (updater as (rows: LeaveNotificationRow[]) => LeaveNotificationRow[])(prev) : updater;
      leaveNotificationsCache = { scope, rows: next, timestamp: Date.now() };
      return next;
    });
  }, [scope]);

  return { loading, setLoading, scope, leaveRows, setLeaveRows: updateLeaveRows, loadLeaveRequests, toaster, setToaster, openToaster };
};

const RequestSummaryCard: React.FC<{ title: string; newCount: number; approvedCount: number; rejectedCount: number }> = ({ title, newCount, approvedCount, rejectedCount }) => (
  <Box sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5, p: 1.25, bgcolor: "#fff" }}>
    <Typography sx={{ fontWeight: 900, fontSize: "0.86rem", color: "var(--lms-text-main)" }}>{title}</Typography>
    <Box sx={{ display: "flex", gap: 1, mt: 1, flexWrap: "wrap" }}>
      <Chip size="small" label={<><T>Pending</T>: {newCount}</>} sx={{ bgcolor: "#eef6e7", color: "var(--lms-primary)", fontWeight: 800 }} />
      <Chip size="small" label={<><T>Approved</T>: {approvedCount}</>} sx={{ bgcolor: "#e8f5e9", color: "#2e7d32", fontWeight: 800 }} />
      <Chip size="small" label={<><T>Rejected</T>: {rejectedCount}</>} sx={{ bgcolor: "#ffebee", color: "#c62828", fontWeight: 800 }} />
    </Box>
  </Box>
);
const RequestSummarySkeletonCard: React.FC = () => (
  <Box sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5, p: 1.25, bgcolor: "#fff" }}>
    <Skeleton variant="text" width="42%" height={24} sx={{ bgcolor: "rgba(148, 163, 184, 0.22)" }} />
    <Box sx={{ display: "flex", gap: 1, mt: 1 }}>
      <Skeleton variant="rounded" width={78} height={24} sx={{ borderRadius: 5, bgcolor: "rgba(148, 163, 184, 0.2)" }} />
      <Skeleton variant="rounded" width={112} height={24} sx={{ borderRadius: 5, bgcolor: "rgba(148, 163, 184, 0.2)" }} />
      <Skeleton variant="rounded" width={102} height={24} sx={{ borderRadius: 5, bgcolor: "rgba(148, 163, 184, 0.2)" }} />
    </Box>
  </Box>
);

const RequestNotificationsTableSkeleton: React.FC = () => (
  <Box sx={{ bgcolor: "#fff", border: "1px solid var(--lms-border)", borderRadius: 3, overflow: "hidden" }}>
    <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", gap: 2, px: 2.5, py: 2, borderBottom: "1px solid var(--lms-border)" }}>
      <Skeleton variant="rounded" width={190} height={46} sx={{ borderRadius: 2, bgcolor: "rgba(148, 163, 184, 0.2)" }} />
      <Box sx={{ display: "flex", gap: 1 }}>
        <Skeleton variant="circular" width={42} height={42} sx={{ bgcolor: "rgba(148, 163, 184, 0.2)" }} />
        <Skeleton variant="rounded" width={130} height={42} sx={{ borderRadius: 2.5, bgcolor: "rgba(148, 163, 184, 0.2)" }} />
      </Box>
    </Box>
    <Box sx={{ p: 2 }}>
      <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1.2fr 1.6fr 1fr 1fr 1.2fr 1.2fr", gap: 2, mb: 1.5 }}>
        {Array.from({ length: 7 }).map((_, index) => (
          <Skeleton key={`header-${index}`} variant="text" height={26} sx={{ bgcolor: "rgba(148, 163, 184, 0.22)" }} />
        ))}
      </Box>
      <Stack spacing={1.2}>
        {Array.from({ length: 5 }).map((_, rowIndex) => (
          <Box key={`row-${rowIndex}`} sx={{ display: "grid", gridTemplateColumns: "1fr 1.2fr 1.6fr 1fr 1fr 1.2fr 1.2fr", gap: 2, alignItems: "center", py: 1.25, borderTop: rowIndex === 0 ? "1px solid var(--lms-border)" : "1px solid rgba(15, 23, 42, 0.06)" }}>
            {Array.from({ length: 7 }).map((__, cellIndex) => (
              <Skeleton key={`row-${rowIndex}-${cellIndex}`} variant="rounded" height={28} sx={{ borderRadius: 1.5, bgcolor: "rgba(148, 163, 184, 0.18)" }} />
            ))}
          </Box>
        ))}
      </Stack>
    </Box>
  </Box>
);

export const RequestNotificationSummaryPopover: React.FC<RequestNotificationSummaryPopoverProps> = ({ anchorEl, onClose, onMouseEnter, onMouseLeave, onShowList }) => {
  const open = Boolean(anchorEl);
  const { translate } = useTranslation();
  const { loading, leaveRows } = useLeaveNotifications(open);
  const { loanRows, loanLoading } = useLoanWorkflowNotifications(open);
  const approvedLeaveCount = leaveRows.filter(isApproved).length;
  const rejectedLeaveCount = leaveRows.filter(isRejected).length;
  const newLeaveCount = leaveRows.filter(isNewRequest).length;

  return (
    <Popover
      open={open}
      anchorEl={anchorEl}
      onClose={onClose}
      disableRestoreFocus
      anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
      transformOrigin={{ vertical: "top", horizontal: "right" }}
      slotProps={{
        paper: {
          onMouseEnter,
          onMouseLeave,
          sx: {
            mt: 1.5,
            width: 420,
            maxWidth: "calc(100vw - 24px)",
            borderRadius: 4,
            pointerEvents: "auto",
            boxShadow: "0 24px 60px rgba(22, 31, 18, 0.18)",
            border: "1px solid var(--lms-border)",
            overflow: "hidden",
            p: 2,
            background: "linear-gradient(180deg, #ffffff 0%, #fbfcf7 100%)",
          },
        },
      }}
      sx={{ pointerEvents: "none" }}
    >
      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", gap: 1.5, mb: 1.5 }}>
        <Box sx={{ minWidth: 0 }}>
          <Typography sx={{ fontWeight: 900, fontSize: "1rem", color: "var(--lms-text-main)" }}><T>Request Notifications</T></Typography>
          <Typography sx={{ color: "var(--lms-text-secondary)", fontSize: "0.78rem", fontWeight: 700 }}>
            {loading || loanLoading ? translate("Loading requests...") : translate("Summary of pending, approved and rejected requests")}
          </Typography>
        </Box>
        <Button
          variant="contained"
          size="small"
          onClick={onShowList}
          sx={{ bgcolor: "var(--lms-primary)", borderRadius: 5, px: 1.75, py: 0.65, textTransform: "none", fontWeight: 900, whiteSpace: "nowrap", "&:hover": { bgcolor: "var(--lms-primary-dark)" } }}
        >
          <T>Show List</T>
        </Button>
      </Box>
      <Stack spacing={1.2}>
        {(loading || loanLoading) && leaveRows.length === 0 && loanRows.length === 0 ? (
          <>
            <RequestSummarySkeletonCard />
            <RequestSummarySkeletonCard />
            <RequestSummarySkeletonCard />
          </>
        ) : (
          <>
            <RequestSummaryCard title={translate("Leave Request")} newCount={newLeaveCount} approvedCount={approvedLeaveCount} rejectedCount={rejectedLeaveCount} />
            <RequestSummaryCard title={translate("Loan Request")} newCount={loanRows.length} approvedCount={0} rejectedCount={0} />
            <RequestSummaryCard title={translate("Scholarship Request")} newCount={0} approvedCount={0} rejectedCount={0} />
          </>
        )}
      </Stack>
    </Popover>
  );
};
export const RequestNotificationsScreen: React.FC<RequestNotificationsScreenProps> = ({ open = true, onClose, sidebarOpen = false, isAnyPinned = false, drawerWidth = 400, pageMode = false }) => {
  const { translate } = useTranslation();
  const navigate = useNavigate();
  const { loading, setLoading, scope, leaveRows, setLeaveRows, toaster, setToaster, openToaster } = useLeaveNotifications(open);
  const { loanRows, loanLoading } = useLoanWorkflowNotifications(open);
  const [rejectRow, setRejectRow] = React.useState<LeaveNotificationRow | null>(null);
  const [rejectRemarks, setRejectRemarks] = React.useState("");
  const [leaveExpanded, setLeaveExpanded] = React.useState(true);
  const [leaveStatusFilter, setLeaveStatusFilter] = React.useState<LeaveStatusFilter>("all");
  const [loanExpanded, setLoanExpanded] = React.useState(true);
  const [scholarshipExpanded, setScholarshipExpanded] = React.useState(true);

  const saveLeaveDecision = async (row: LeaveNotificationRow, decision: "approve" | "reject", remarks = "") => {
    const currentApproverEmployeeId = getScopeReferenceEmployeeId(scope);
    const currentUserIsCeo = isCurrentUserCeo();
    const currentUserCanCompleteLongLeave = currentUserIsCeo || Boolean(scope?.isAdmin ?? scope?.IsAdmin);
    const longLeave = isLongLeave(row);
    const nextStatusId = decision === "reject" ? REJECTED_STATUS_ID : APPROVED_STATUS_ID;
    const nextAdmApproval = decision === "approve" && longLeave && currentUserCanCompleteLongLeave && currentApproverEmployeeId ? currentApproverEmployeeId : row.admApproval ?? null;
    const payload = {
      transactionNo: row.source === "request" ? "" : row.transactionNo,
      transactionDate: toDateOnly(dayjs().format("YYYY-MM-DD")),
      requestId: row.requestId,
      employeeId: row.employeeId,
      leaveUnitId: row.leaveUnitId,
      leaveTypeId: row.leaveTypeId,
      fromDate: row.fromDate ? toDateOnly(row.fromDate) : null,
      tillDate: row.tillDate ? toDateOnly(row.tillDate) : null,
      leaveDays: row.leaveDays || 0,
      paidLeave: row.paidLeave,
      statusId: nextStatusId,
      admApproval: decision === "reject" ? null : nextAdmApproval,
      remarks,
      ceoApprovalRequired: longLeave,
      lineManagerApprovalCompleted: nextStatusId === APPROVED_STATUS_ID,
      ceoApprovalCompleted: Boolean(nextAdmApproval),
      isFullyApproved: longLeave ? nextStatusId === APPROVED_STATUS_ID && Boolean(nextAdmApproval) : nextStatusId === APPROVED_STATUS_ID,
    };

    setLoading(true);
    try {
      const response: any = row.source === "request"
        ? await PostService(LEAVE_TRANSACTION_API, payload)
        : await PutService(`${LEAVE_TRANSACTION_API}/${row.id}`, payload);
      const savedId = response?.data?.data?.id ?? response?.data?.data?.Id ?? row.id;
      setLeaveRows((prev) => prev.map((item) => item.id === row.id && item.source === row.source ? {
        ...item,
        id: savedId,
        statusId: nextStatusId,
        status: decision === "approve" ? "Approved" : "Rejected",
        admApproval: payload.admApproval,
        remarks: decision === "approve" ? "Approved" : remarks,
        lineManagerApprovalCompleted: payload.lineManagerApprovalCompleted,
        ceoApprovalCompleted: payload.ceoApprovalCompleted,
        isFullyApproved: payload.isFullyApproved,
        source: "transaction",
      } : item));
      openToaster([decision === "approve" ? translate("Leave request approved successfully") : translate("Leave request rejected successfully")], "success");
    } catch (error: any) {
      openToaster([getApiErrorMessage(error, translate("Failed to save leave request"))], "error");
    } finally {
      setLoading(false);
    }
  };

  const handleRejectConfirm = async () => {
    if (!rejectRow) return;
    if (!rejectRemarks.trim()) {
      openToaster([translate("Rejection remarks are required")], "error");
      return;
    }
    const row = rejectRow;
    const remarks = rejectRemarks.trim();
    setRejectRow(null);
    setRejectRemarks("");
    await saveLeaveDecision(row, "reject", remarks);
  };

  const renderStatusAction = (row: LeaveNotificationRow) => {
    if (isRejected(row)) return <Chip size="small" color="error" label={translate("Rejected")} sx={{ "& .MuiChip-label": { fontWeight: 900 } }} />;
    if (row.isFullyApproved || (!isLongLeave(row) && isApproved(row))) return <Chip size="small" color="success" label={translate("Approved")} sx={{ "& .MuiChip-label": { fontWeight: 900 } }} />;
    return (
      <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
        <Button size="small" variant="contained" onClick={() => saveLeaveDecision(row, "approve")} sx={{ bgcolor: "#dff3df", color: "#1b7f35", boxShadow: "none", textTransform: "none", fontWeight: 900, borderRadius: 5, "&:hover": { bgcolor: "#c9ebcb", boxShadow: "none" } }}>
          <T>Approve</T>
        </Button>
        <Button size="small" variant="contained" onClick={() => { setRejectRow(row); setRejectRemarks(""); }} sx={{ bgcolor: "#fde2e2", color: "#c62828", boxShadow: "none", textTransform: "none", fontWeight: 900, borderRadius: 5, "&:hover": { bgcolor: "#facaca", boxShadow: "none" } }}>
          <T>Reject</T>
        </Button>
      </Box>
    );
  };

  const leaveColumns = React.useMemo(() => [
    { id: "request", label: "Leave Request", minWidth: 150 },
    { id: "leaveType", label: "Leave Type", minWidth: 180 },
    { id: "employeeName", label: "Employee Name", minWidth: 240 },
    { id: "fromDateDisplay", label: "From Date", minWidth: 150 },
    { id: "tillDateDisplay", label: "Till Date", minWidth: 150 },
    { id: "remarksDisplay", label: "Remarks", minWidth: 180 },
    { id: "statusText", label: "Status / Action", minWidth: 190, format: (_value: any, row: LeaveNotificationRow) => renderStatusAction(row) },
  ], [leaveRows, translate]);

  const filteredLeaveRows = React.useMemo(() => leaveRows.filter((row) => leaveStatusFilter === "all" || getLeaveStatusKey(row) === leaveStatusFilter), [leaveRows, leaveStatusFilter]);

  const leaveTableRows = React.useMemo(() => filteredLeaveRows.map((row) => ({
    ...row,
    request: row.requestNo || row.transactionNo || row.id,
    fromDateDisplay: row.fromDate ? dayjs(row.fromDate).format("DD-MMM-YYYY") : "-",
    tillDateDisplay: row.tillDate ? dayjs(row.tillDate).format("DD-MMM-YYYY") : "-",
    remarksDisplay: row.remarks || row.status || "-",
    statusText: getLeaveStatusText(row, translate),
  })), [filteredLeaveRows, translate]);

  const renderRequestSection = (
    title: string,
    expanded: boolean,
    onToggle: () => void,
    content: React.ReactNode,
  ) => (
    <Box sx={{ bgcolor: "#fff", border: "1px solid var(--lms-border)", borderRadius: 2, p: { xs: 2, md: 2.5 } }}>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          cursor: "pointer",
          userSelect: "none",
          "&:hover": { "& .expand-icon-container": { bgcolor: "rgba(var(--lms-primary-rgb), 0.08)" } },
        }}
        onClick={onToggle}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
          <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
          <Typography sx={{ fontWeight: 900, color: "var(--lms-text-main)", fontSize: "1.05rem" }}>
            {title}
          </Typography>
        </Box>
        <Box
          className="expand-icon-container"
          sx={{
            width: 22,
            height: 22,
            borderRadius: 1,
            border: "1px solid var(--lms-border)",
            color: "var(--lms-primary)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            transition: "background-color 0.2s ease",
          }}
        >
          {expanded ? <RemoveIcon sx={{ fontSize: "0.95rem" }} /> : <AddIcon sx={{ fontSize: "0.95rem" }} />}
        </Box>
      </Box>
      <Collapse in={expanded} timeout="auto" unmountOnExit>
        <Box sx={{ pt: 2 }}>{content}</Box>
      </Collapse>
    </Box>
  );
  if (!open) return null;

  return (
    <>
      {loading && leaveRows.length > 0 && <LmsOverlayLoader />}
      <Box
        sx={{
          position: pageMode ? "relative" : "fixed",
          top: pageMode ? "auto" : 64,
          bottom: pageMode ? "auto" : 0,
          left: pageMode ? "auto" : { xs: 0, md: sidebarOpen ? 240 : 64 },
          right: pageMode ? "auto" : isAnyPinned ? `${drawerWidth}px` : 0,
          zIndex: pageMode ? "auto" : (theme) => theme.zIndex.appBar - 1,
          bgcolor: "rgba(248, 251, 244, 0.98)",
          overflow: "auto",
          p: { xs: 1.5, md: 3 },
        }}
      >
        <Box sx={{ maxWidth: 1280, mx: "auto", display: "grid", gap: 2 }}>
          <Box sx={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between", gap: 2, bgcolor: "#fff", border: "1px solid var(--lms-border)", borderRadius: 2, px: { xs: 2, md: 2.5 }, py: 2 }}>
            <Box>
              <Typography sx={{ fontWeight: 900, color: "var(--lms-text-main)", fontSize: { xs: "1.15rem", md: "1.35rem" } }}><T>Request Notifications</T></Typography>
              <Typography sx={{ color: "var(--lms-text-secondary)", fontSize: "0.84rem", fontWeight: 700 }}>{scope?.scope || scope?.Scope || translate("Admin")}</Typography>
            </Box>
            {onClose && <IconButton size="small" onClick={onClose} sx={{ color: "var(--lms-text-secondary)" }}><CloseIcon fontSize="small" /></IconButton>}
          </Box>

          {renderRequestSection(
            translate("Leave Request"),
            leaveExpanded,
            () => setLeaveExpanded((current) => !current),
            loading && leaveRows.length === 0 ? (
              <RequestNotificationsTableSkeleton />
            ) : (
                <SchemaTable
                  title="Leave Request"
                  columns={leaveColumns}
                  rows={leaveTableRows}
                extraToolbarItemsRight={(
                  <TextField
                    select
                    size="small"
                    label={translate("Status")}
                    value={leaveStatusFilter}
                    onChange={(event) => setLeaveStatusFilter(event.target.value as LeaveStatusFilter)}
                    sx={{ width: 190, "& .MuiInputBase-root": { height: 44 } }}
                  >
                    <MenuItem value="all">{translate("All")}</MenuItem>
                    <MenuItem value="pending">{translate("Pending")}</MenuItem>
                    <MenuItem value="approved">{translate("Approved")}</MenuItem>
                    <MenuItem value="rejected">{translate("Rejected")}</MenuItem>
                  </TextField>
                )}
                  actions={false}
                  showAddButton={false}
                  hideBreadcrumbs
                  hideTitle
                  showToaster={false}
                  pagination
                  filtersEnabled
                  downloadEnabled={false}
                  stickyHeader
                  maxHeight="calc(100vh - 360px)"
                  rowsPerPageOptions={[10, 20, 50, 100]}
                  rowHoverBg="rgba(var(--lms-primary-rgb), 0.08)"
                />
            ),
          )}

          {renderRequestSection(
            translate("Loan Request"),
            loanExpanded,
            () => setLoanExpanded((current) => !current),
            loanLoading ? <Skeleton variant="rounded" height={90} /> : loanRows.length === 0 ? (
              <Typography sx={{ color: "var(--lms-text-secondary)", fontSize: "0.86rem", fontWeight: 700 }}>{translate("No loan requests pending your action")}</Typography>
            ) : (
              <SchemaTable
                title={translate("Loan Request")}
                columns={[
                  { id: "requestNo", label: translate("Request No"), minWidth: 110 },
                  { id: "employeeName", label: translate("Employee"), minWidth: 170 },
                  { id: "loanType", label: translate("Loan Type"), minWidth: 140 },
                  { id: "appliedAmount", label: translate("Amount"), minWidth: 110 },
                  { id: "stage", label: translate("Current Stage"), minWidth: 190 },
                ]}
                rows={loanRows}
                actions={false}
                showAddButton={false}
                hideBreadcrumbs
                hideTitle
                showToaster={false}
                onRowClick={(row: any) => {
                  onClose?.();
                  navigate(`/hr/${encodeURIComponent("Loan / Advance Issuance")}?loanWorkflowTab=pending&loanRequestId=${Number(row?.requestId ?? 0)}`);
                }}
              />
            ),
          )}
          {renderRequestSection(
            translate("Scholarship Request"),
            scholarshipExpanded,
            () => setScholarshipExpanded((current) => !current),
            <Typography sx={{ color: "var(--lms-text-secondary)", fontSize: "0.86rem", fontWeight: 700 }}>{translate("No scholarship requests configured yet")}</Typography>,
          )}
        </Box>
      </Box>

      <Dialog open={Boolean(rejectRow)} onClose={() => setRejectRow(null)} maxWidth="sm" fullWidth>
        <DialogTitle sx={{ fontWeight: 900 }}><T>Reject Leave Request</T></DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            fullWidth
            multiline
            minRows={3}
            label={translate("Rejection Remarks")}
            value={rejectRemarks}
            onChange={(event) => setRejectRemarks(event.target.value)}
            required
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setRejectRow(null)} sx={{ textTransform: "none", fontWeight: 800 }}><T>Cancel</T></Button>
          <Button variant="contained" color="error" onClick={handleRejectConfirm} sx={{ textTransform: "none", fontWeight: 800 }}><T>Reject</T></Button>
        </DialogActions>
      </Dialog>

      <LmsToaster open={toaster.show} onClose={() => setToaster({ show: false, messages: [], severity: "success" })} messages={toaster.messages} severity={toaster.severity} />
    </>
  );
};

