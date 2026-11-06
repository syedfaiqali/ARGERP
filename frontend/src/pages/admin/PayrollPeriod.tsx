import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import React, { useState, useEffect, useCallback, useMemo, useRef } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
  Grid,
  Box,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Collapse,
  Menu,
  MenuItem,
} from "@mui/material";
import {
  Search as SearchIcon,
  Settings as SettingsIcon,
  ArrowBack as ArrowBackIcon,
  Assessment as AssessmentIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { PayrollPeriodReportModal } from "./PayrollPeriodReportModal";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormDatePicker } from "@/components/FormDatePicker";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService } from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { payrollPeriodSlice, payrollRegisterSlice } from "@/store/humanResourceSlices";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";

const normalizeLookupRows = (rows: any[] = []) =>
  rows.map((row) => ({
    ...row,
    id: row.id ?? row.Id,
    code: row.code ?? row.Code ?? "",
    name: row.name ?? row.Name ?? row.stxt ?? row.Stxt ?? "",
    stxt: row.stxt ?? row.Stxt ?? row.name ?? row.Name ?? "",
    ltxt: row.ltxt ?? row.Ltxt ?? row.longName ?? row.LongName ?? "",
  }));

const payrollTypeIds = new Set([259, 260, 261, 264]);

const getCurrentEntityContext = () => {
  const selectedEntityId = localStorage.getItem("selectedEntity")
    || localStorage.getItem("selectedEntityId")
    || localStorage.getItem("selectedEntityID")
    || localStorage.getItem("selectedCampusId")
    || "";
  const selectedEntityName = localStorage.getItem("selectedEntityName")
    || localStorage.getItem("selectedCampusName")
    || "";
  let user: any = {};
  try { user = JSON.parse(localStorage.getItem("user") || "{}"); } catch { /* use local scope values */ }

  const id = Number(selectedEntityId || user.entityId || user.EntityId || 0);
  const name = selectedEntityName || user.entityName || user.EntityName || user.campusName || user.CampusName || "";
  return id > 0 ? { id, name, stxt: name, instituteName: name, shortName: name } : null;
};

const normalizePayrollDetail = (detail: any = {}) => ({
  ...detail,
  id: detail.id ?? detail.Id ?? 0,
  periodstxt: detail.periodstxt ?? detail.Periodstxt ?? "",
  year: detail.year ?? detail.Year ?? null,
  startDate: detail.startDate ?? detail.StartDate ?? detail.startdate ?? detail.Startdate ?? null,
  endDate: detail.endDate ?? detail.EndDate ?? detail.enddate ?? detail.Enddate ?? null,
  noOfDays: detail.noOfDays ?? detail.NoOfDays ?? detail.noofdays ?? detail.Noofdays ?? null,
  disbDate: detail.disbDate ?? detail.DisbDate ?? detail.disbdate ?? detail.Disbdate ?? null,
  statusid: detail.statusid ?? detail.Statusid ?? detail.statusId ?? detail.StatusId ?? null,
  statusId: detail.statusId ?? detail.StatusId ?? detail.statusid ?? detail.Statusid ?? null,
  statusName: detail.statusName ?? detail.StatusName ?? detail.status ?? detail.Status ?? "",
});

const PayrollPeriod: React.FC<{ onSubViewChange?: (subView: string | null) => void; resetKey?: number }> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: isListLoading } = useCachedRows<any>(
    "hrPayrollPeriod",
    payrollPeriodSlice.fetchAll,
  );
  // The payroll register list is a cached HR list with its own screen.
  const { rows: payrollRegisters } = useCachedRows<any>(
    "hrPayrollRegister",
    payrollRegisterSlice.fetchAll,
  );
  const [isLookupLoading, setIsLookupLoading] = useState(false);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [isFormLoading, setIsFormLoading] = useState(false);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">("list");
  const [formData, setFormData] = useState<any>({
    id: null,
    code: "",
    payrollRegisterName: "",
    payrollRegisterId: null,
    payrollPeriod: "",
    fromDate: null as Dayjs | null,
    noOfPeriod: "",
    payrollType: "",
    payrollTypeId: null,
    status: "",
    statusId: null,
    isActive: true,
    details: [] as any[],
  });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isReportModalOpen, setIsReportModalOpen] = useState(false);
  const [reportFilters, setReportFilters] = useState<any>(null);
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

  const handleSingleReport = () => {
    const entity = entities[0];
    const entityId = entity?.id ?? null;

    const regId = selectedRow?.payrollregisterid ?? selectedRow?.payrollRegisterId;
    const regName = payrollRegisters.find((r: any) => String(r.id) === String(regId))?.stxt || selectedRow?.payrollRegisterName || "";

    const pTypeId = selectedRow?.atntypeid ?? selectedRow?.payrollTypeId;
    const pTypeName = attendanceTypes.find((a: any) => String(a.id) === String(pTypeId))?.stxt || selectedRow?.attendaneTypeName || selectedRow?.attendanceTypeName || "";

    const sId = selectedRow?.statusid ?? selectedRow?.statusId;
    const sName = statuses.find((s: any) => String(s.id) === String(sId))?.stxt || selectedRow?.statusName || "";

    setReportFilters({
      payrollRegisterId: regId,
      payrollRegisterName: regName,
      payrollPeriodId: selectedRow?.id,
      payrollPeriodName: selectedRow?.payrollPeriod || selectedRow?.ltxt || "",
      payrollTypeId: pTypeId,
      payrollTypeName: pTypeName,
      statusId: sId,
      statusName: sName,
      entityId: entityId,
      entityName: entity?.instituteName || entity?.name || entity?.stxt || entity?.ltxt || "",
      institutionName: entity?.shortName || entity?.instituteName || entity?.name || entity?.stxt || "",
      campusName: entity?.instituteName || entity?.name || entity?.stxt || "",
      address: entity?.address || "",
      city: entity?.city || "",
      country: entity?.country || "",
      imageId: entity?.imageId || null,
      reportType: 'Detail'
    });
    setViewMode("report");
    handleMenuClose();
  };

  // ACUNT/STATS/SType are global LOV reference data shared across HR screens.
  const { lov } = useHrLovs(["ACUNT", "STATS", "SType"]);
  const accrualUnits = useMemo<any[]>(
    () => normalizeLookupRows(lov("ACUNT")).filter((x: any) => payrollTypeIds.has(Number(x.id))),
    [lov],
  );
  const statuses = useMemo<any[]>(() => normalizeLookupRows(lov("STATS")), [lov]);
  const detailStatuses = useMemo<any[]>(() => normalizeLookupRows(lov("SType")), [lov]);
  // The attendance-type list is the same accrual-unit subset the legacy screen used.
  const attendanceTypes = accrualUnits;
  const [entities] = useState<any[]>(() => {
    const currentEntity = getCurrentEntityContext();
    return currentEntity ? [currentEntity] : [];
  });
  const [payrollPeriodItems, setPayrollPeriodItems] = useState<any[]>([]);

  const openStatus = statuses.find((status: any) =>
    String(status.code || "").trim().toUpperCase() === "OPN" ||
    String(status.stxt || status.name || "").trim().toLowerCase() === "open"
  );

  // sNo and the display alias are derived here so the cache keeps raw records.
  const rows = useMemo<any[]>(
    () =>
      (cachedRows || []).map((item: any, index: number) => ({
        ...item,
        sNo: index + 1,
        payrollPeriod: item.ltxt,
      })),
    [cachedRows],
  );

  // The period picker reuses the cached rows rather than its own copy.
  useEffect(() => {
    setPayrollPeriodItems(
      rows.map((m: any) => ({ id: m.id, name: m.payrollPeriod, stxt: m.payrollPeriod })),
    );
  }, [rows]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Payroll Period"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Payroll Period"));
    else if (viewMode === "view") onSubViewChange(translate("View Payroll Period"));
    else if (viewMode === "report") onSubViewChange(translate("Payroll Period Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);


  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Period ID"), minWidth: 100 },
    { id: "ltxt", label: translate("Payroll Period"), minWidth: 200 },
    { id: "payrollRegisterName", label: translate("Payroll Register"), minWidth: 150 },
    { id: "attendaneTypeName", label: translate("Payroll Type"), minWidth: 100 },
    {
      id: "fromDate",
      label: translate("From Date"),
      minWidth: 100,
      format: (val) => val ? dayjs(val).format("DD-MMM-YYYY") : ""
    },
    { id: "noOfPeriod", label: translate("No of Period"), minWidth: 80 },
    { id: "statusName", label: translate("Status"), minWidth: 80 },
  ];

  const initialFormData = {
    id: null,
    code: "",
    payrollRegisterName: "",
    payrollRegisterId: null,
    payrollPeriod: "",
    fromDate: null as Dayjs | null,
    noOfPeriod: "",
    payrollType: "",
    payrollTypeId: null,
    status: "",
    statusId: null,
    isActive: true,
    details: [] as any[],
  };

  const handleSave = async (formData: any) => {
    setIsFormLoading(true);
    try {
      const payload = {
        id: formData.id,
        code: formData.code ?? formData.Code ?? "",
        date: new Date().toISOString(),
        atntypeid: formData.payrollTypeId,
        fromyear: formData.fromDate ? formData.fromDate.toISOString() : null,
        toyear: formData.details && formData.details.length > 0 ? formData.details[formData.details.length - 1].endDate : null,
        ltxt: formData.payrollPeriod,
        startfrom: formData.fromDate ? dayjs(formData.fromDate).date() : 1,
        statusid: formData.statusId,
        lovsharedid: 520, 
        startdate: formData.fromDate ? formData.fromDate.toISOString() : null,
        payrollregisterid: formData.payrollRegisterId,
        noofperiod: parseInt(formData.noOfPeriod) || 0,
        isActive: formData.isActive,
        details: (formData.details || []).map((d: any, index: number) => ({
          id: d.id > 0 ? d.id : null,
          periodstxt: d.periodstxt,
          year: d.year,
          startDate: d.startDate,
          endDate: d.endDate,
          noOfDays: d.noOfDays,
          disbDate: d.disbDate,
          statusId: d.statusId ?? d.statusid ?? formData.statusId,
          lineIndex: index + 1
        }))
      };

      const response: any = formData.id
        ? await saveRecord("put", `/api/HumanResource/PayrollPeriod/v3/${formData.id}`, payload)
        : await saveRecord("post", "/api/HumanResource/PayrollPeriod/v3", payload);
      if (response && (response.status === 200 || response.status === 201 || response.status === 204)) {
        dispatch(payrollPeriodSlice.fetchAll(true));
        return true;
      }
    } catch (error: any) {
      console.error("Save failed", error);
      setErrorMessages([apiErrorMessage(error, "Operation failed")]);
      setShowError(true);
    } finally {
      setIsFormLoading(false);
    }
    return false;
  };

  const handleDelete = async (row: any) => {
    setIsFormLoading(true);
    try {
      const response: any = await saveRecord("delete", `/api/HumanResource/PayrollPeriod/v3/${row.id}`);
      if (response && (response.status === 204 || response.status === 200)) {
        dispatch(payrollPeriodSlice.fetchAll(true));
      }
    } catch (error) {
      console.error("Delete failed", error);
    } finally {
      setIsFormLoading(false);
    }
  };

  const handleGenerateDetails = async (formData: any, setFormData: any) => {
    if (!formData.payrollTypeId || !formData.fromDate || !formData.noOfPeriod) {
      setErrorMessages(["Please fill Payroll Type, From Date and No of Period first."]);
      setShowError(true);
      return;
    }

    setIsFormLoading(true);
    try {
      const fromDateStr = formData.fromDate.toISOString();
      const response: any = await GetService(`/api/HumanResource/PayrollPeriod/v3/generate?atnTypeId=${formData.payrollTypeId}&fromDate=${fromDateStr}&noOfPeriod=${formData.noOfPeriod}`);
      if (response && response.data) {
        setFormData({
          ...formData,
          details: (response.data || []).map(normalizePayrollDetail)
        });
      }
    } catch (error) {
      console.error("Generation failed");
    } finally {
      setIsFormLoading(false);
    }
  };

  const renderForm = (data: any, setFn: any, mode: string, isRO: boolean, isSub: boolean) => (
    <PayrollPeriodForm
      formData={data}
      setFormData={setFn}
      mode={mode}
      isReadOnly={isRO}
      isSubmitted={isSub}
      payrollRegisters={payrollRegisters}
      attendanceTypes={attendanceTypes}
      accrualUnits={accrualUnits}
      statuses={statuses}
      detailStatuses={detailStatuses}
      isFormLoading={isFormLoading}
      setIsFormLoading={setIsFormLoading}
      handleGenerateDetails={handleGenerateDetails}
    />
  );

  return (
    <>
      {(isFormLoading || isListLoading || isLookupLoading) && <LmsOverlayLoader />}
      {viewMode !== "list" ? (
        <Box sx={{ p: viewMode === "report" ? 0 : 2 }}>
          {viewMode !== "report" && (
            <Box
              sx={{
                p: 2,
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                flexWrap: "wrap",
                gap: 1.5,
              }}
            >
              <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
                {viewMode === "new" ? translate("New Payroll Period") : viewMode === "edit" ? translate("Edit Payroll Period") : translate("View Payroll Period")}
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
                    sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}
                  >
                    {translate("Save")}
                  </Button>
                )}
              </Box>
            </Box>
          )}

          <Box sx={{ px: viewMode === "report" ? 0 : 2, pb: 2 }}>
            {viewMode === "report" ? (
              <PayrollPeriodReportModal
                open={true}
                onClose={() => setViewMode("list")}
                lookups={{
                  entities,
                  payrollRegisters: payrollRegisters.map((x: any) => ({ id: x.id, stxt: x.stxt || x.name || x.shortName || "" })),
                  payrollPeriods: rows.map((x: any) => ({ id: x.id, stxt: x.payrollPeriod || x.ltxt || x.stxt || x.name || "" })),
                  payrollTypes: attendanceTypes.map((x: any) => ({ id: x.id, stxt: x.stxt || x.name || "" })),
                  statuses: statuses.map((x: any) => ({ id: x.id, stxt: x.stxt || x.name || "" }))
                }}
                initialFilters={reportFilters}
                isFullScreenView={false}
              />
            ) : (
              renderForm(formData, setFormData, viewMode, viewMode === "view", isSubmitted)
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Payroll Period")}
          addButtonLabel={translate("New Payroll Period")}
          breadcrumbPath={''}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          actionsPosition="right"
          onAddClick={() => {
            setFormData({
              ...initialFormData,
              statusId: openStatus?.id ?? null,
              status: openStatus?.stxt || openStatus?.name || "Open",
            });
            setIsSubmitted(false);
            setViewMode("new");
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
              onClick={() => {
                setReportFilters(null);
                setViewMode("report");
              }}
            >
              <T>Payroll Period Report</T>
            </Button>
          }
          onEditClick={(row: any) => {
            setFormData({
              ...row,
              code: row?.code ?? row?.Code ?? "",
              payrollRegisterId: row?.payrollregisterid ?? row?.payrollRegisterId ?? null,
              payrollRegisterName: row?.payrollRegisterName ?? "",
              payrollPeriod: row?.ltxt ?? row?.payrollPeriod ?? "",
              payrollTypeId: row?.atntypeid ?? row?.payrollTypeId ?? null,
              payrollType: row?.attendaneTypeName ?? row?.payrollType ?? "",
              statusId: row?.statusid ?? row?.statusId ?? null,
              noOfPeriod: String(row?.noofperiod ?? row?.noOfPeriod ?? ""),
              fromDate: row?.fromDate ? dayjs(row.fromDate) : null,
              details: Array.isArray(row?.details) ? row.details.map(normalizePayrollDetail) : [],
            });
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({
              ...row,
              code: row?.code ?? row?.Code ?? "",
              payrollRegisterId: row?.payrollregisterid ?? row?.payrollRegisterId ?? null,
              payrollRegisterName: row?.payrollRegisterName ?? "",
              payrollPeriod: row?.ltxt ?? row?.payrollPeriod ?? "",
              payrollTypeId: row?.atntypeid ?? row?.payrollTypeId ?? null,
              payrollType: row?.attendaneTypeName ?? row?.payrollType ?? "",
              statusId: row?.statusid ?? row?.statusId ?? null,
              noOfPeriod: String(row?.noofperiod ?? row?.noOfPeriod ?? ""),
              fromDate: row?.fromDate ? dayjs(row.fromDate) : null,
              details: Array.isArray(row?.details) ? row.details.map(normalizePayrollDetail) : [],
            });
            setIsSubmitted(false);
            setViewMode("view");
          }}
          showAddButton={true}
          showToaster={false}
          showMenuAction={true}
          onMenuAction={handleMenuOpen}
        />
      )}

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
          onClick={handleSingleReport}
          sx={{ 
            py: 1.5,
            fontSize: '0.875rem',
            color: '#0ea5e9',
            fontWeight: 500,
            '&:hover': { bgcolor: '#f0f9ff' }
          }}
        >
          <AssessmentIcon sx={{ fontSize: '1.25rem', mr: 1.5, color: '#0ea5e9' }} />
          <T>Payroll Period Detail</T>
        </MenuItem>
      </Menu>

      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
        severity="error"
      />
    </>
  );
};

interface FormProps {
  formData: any;
  setFormData: any;
  mode: string;
  isReadOnly?: boolean;
  isSubmitted?: boolean;
  payrollRegisters: any[];
  attendanceTypes: any[];
  accrualUnits: any[];
  statuses: any[];
  detailStatuses: any[];
  isFormLoading: boolean;
  setIsFormLoading: (v: boolean) => void;
  handleGenerateDetails: (fd: any, sfd: any) => void;
}

const PayrollPeriodForm: React.FC<FormProps> = ({
  formData,
  setFormData,
  mode,
  isReadOnly,
  isSubmitted,
  payrollRegisters,
  attendanceTypes,
  statuses,
  detailStatuses,
  isFormLoading,
  setIsFormLoading,
  handleGenerateDetails,
}) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [registerModalOpen, setRegisterModalOpen] = useState(false);
  const [typeModalOpen, setTypeModalOpen] = useState(false);
  const [statusModalOpen, setStatusModalOpen] = useState(false);
  const [detailStatusModalIndex, setDetailStatusModalIndex] = useState<number | null>(null);
  const latestRegisterDateRequestRef = useRef(0);
  const lockGeneratedFields = isReadOnly || mode === "edit";
  const disabledCursorSx = lockGeneratedFields
    ? {
        "& .MuiInputBase-root, & .MuiOutlinedInput-root, & .MuiPickersOutlinedInput-root, & input, & button": {
          cursor: "not-allowed !important",
        },
        "& .Mui-disabled": {
          cursor: "not-allowed !important",
        },
      }
    : undefined;

  const selectDetailStatus = (index: number, row: any) => {
    const newDetails = [...(formData.details || [])];
    if (!newDetails[index]) return;
    newDetails[index].statusid = row.id;
    newDetails[index].statusId = row.id;
    newDetails[index].statusName = row.stxt || row.name || "";
    setFormData({ ...formData, details: newDetails });
  };

  const selectPayrollRegister = async (row: any) => {
    const registerId = row.id;
    const requestId = latestRegisterDateRequestRef.current + 1;
    latestRegisterDateRequestRef.current = requestId;
    const nextData = {
      ...formData,
      payrollRegisterId: registerId,
      payrollRegisterName: row.stxt || row.name || "",
      fromDate: null,
    };
    setFormData(nextData);

    if (formData.id || isReadOnly) return;

    try {
      const response: any = await GetService(`/api/HumanResource/PayrollPeriod/v3/next-register-date/${registerId}`);
      const startdate = response?.data?.startdate ?? response?.data?.Startdate;
      if (latestRegisterDateRequestRef.current !== requestId) return;
      setFormData((current: any) => {
        if (String(current.payrollRegisterId) !== String(registerId)) return current;
        return { ...current, fromDate: startdate ? dayjs(startdate) : null };
      });
    } catch (error) {
      console.error("Failed to fetch next payroll register date", error);
      if (latestRegisterDateRequestRef.current !== requestId) return;
      setFormData((current: any) => {
        if (String(current.payrollRegisterId) !== String(registerId)) return current;
        return { ...current, fromDate: null };
      });
    }
  };

    // Fetch full details if editing and details are missing
    useEffect(() => {
        const fetchDetails = async () => {
            if (formData.id && (!formData.details || formData.details.length === 0)) {
                setIsFormLoading(true);
                try {
                    const response: any = await GetService(`/api/HumanResource/PayrollPeriod/v3/${formData.id}`);
                    if (response && response.data) {
                        const data = response.data;
                        setFormData({
                            ...formData,
                            ...data,
                            code: data.code ?? data.Code ?? formData.code ?? formData.Code ?? "",
                            payrollRegisterId: data.payrollregisterid,
                            payrollRegisterName: payrollRegisters.find(r => r.id === data.payrollregisterid)?.stxt || "",
                            payrollTypeId: data.atntypeid,
                            statusId: data.statusid,
                            fromDate: data.startdate ? dayjs(data.startdate) : null,
                            noOfPeriod: data.noofperiod?.toString() || "",
                            details: (data.details || []).map(normalizePayrollDetail)
                        });
                    }
                } catch (error) {
                    console.error("Failed to fetch full record", error);
                } finally {
                    setIsFormLoading(false);
                }
            }
        };
        fetchDetails();
    }, [formData.id]);

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
      <Box
        sx={{
          p: 0,
          pointerEvents: isReadOnly ? "none" : "auto",
          opacity: isReadOnly ? 0.9 : 1,
        }}
      >
        {isFormLoading && <LmsOverlayLoader />}

        {/* Section Header: Basic Information */}
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 2,
            cursor: "pointer",
            userSelect: "none",
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
              <T>Basic Information</T>
            </Typography>
          </Box>
          <Box sx={{ borderRadius: "50%", p: 0.5 }}>
            {renderExpandIcon(isGeneralExpanded)}
          </Box>
        </Box>

        <Collapse in={isGeneralExpanded}>
          <Grid container spacing={3}>
            {/* Row 1: Code and Payroll Register */}
            <Grid item xs={12} md={6}>
              <FormInput
                label={translate("Period ID")}
                disabled
                value={formData.code ?? formData.Code ?? ""}
                placeholder={translate("Auto Generated")}
                sx={disabledCursorSx}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Payroll Register*")}
                value={formData.payrollRegisterName || ""}
                placeholder={translate("Select Payroll Register")}
                options={payrollRegisters}
                getOptionLabel={(opt) => opt.stxt || opt.code || ""}
                onSelect={(row) => {
                  selectPayrollRegister(row);
                }}
                onSearchClick={() => !lockGeneratedFields && setRegisterModalOpen(true)}
                disabled={lockGeneratedFields}
                error={isSubmitted && !formData.payrollRegisterId}
                helperText={
                  isSubmitted && !formData.payrollRegisterId
                    ? translate("Payroll Register is required")
                    : ""
                }
                hideDropdownArrow
              />
            </Grid>

            <Grid item xs={12}>
              <FormInput
                label={translate("Payroll Period*")}
                placeholder={translate("Enter Payroll Period")}
                value={formData.payrollPeriod || ""}
                onChange={(e: any) =>
                  setFormData({ ...formData, payrollPeriod: e.target.value })
                }
                error={isSubmitted && !formData.payrollPeriod}
                helperText={
                  isSubmitted && !formData.payrollPeriod
                    ? translate("Payroll Period is required")
                    : ""
                }
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <FormDatePicker
                label={translate("From Date*")}
                value={formData.fromDate ? dayjs(formData.fromDate) : null}
                onChange={(val) =>
                  setFormData({ ...formData, fromDate: val })
                }
                disabled={lockGeneratedFields}
                error={isSubmitted && !formData.fromDate}
                helperText={
                  isSubmitted && !formData.fromDate
                    ? translate("From Date is required")
                    : ""
                }
                sx={disabledCursorSx}
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("No of Period*")}
                placeholder={translate("Enter No of Period")}
                type="number"
                value={formData.noOfPeriod || ""}
                onChange={(e: any) =>
                  setFormData({ ...formData, noOfPeriod: e.target.value })
                }
                disabled={lockGeneratedFields}
                error={isSubmitted && !formData.noOfPeriod}
                helperText={
                  isSubmitted && !formData.noOfPeriod
                    ? translate("No of Period is required")
                    : ""
                }
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <LookupDropdown
                label={translate("Payroll Type*")}
                value={formData.payrollType || attendanceTypes.find(u => u.id === formData.payrollTypeId)?.stxt || ""}
                placeholder={translate("Select Payroll Type")}
                options={attendanceTypes}
                getOptionLabel={(opt) => opt.stxt || opt.name || opt.code || ""}
                onSelect={(row) => {
                  setFormData({ ...formData, payrollTypeId: row.id, payrollType: row.stxt || row.name || "" });
                }}
                onSearchClick={() => !lockGeneratedFields && setTypeModalOpen(true)}
                disabled={lockGeneratedFields}
                error={isSubmitted && !formData.payrollTypeId}
                helperText={
                  isSubmitted && !formData.payrollTypeId
                    ? translate("Payroll Type is required")
                    : ""
                }
                hideDropdownArrow
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <LookupDropdown
                label={translate("Status*")}
                value={statuses.find(s => s.id === formData.statusId)?.stxt || formData.status || ""}
                placeholder={translate("Select Status")}
                options={statuses}
                getOptionLabel={(opt) => opt.stxt || opt.name || opt.code || ""}
                onSelect={(row) => {
                  setFormData({ ...formData, statusId: row.id, status: row.stxt || row.name || "" });
                }}
                onSearchClick={() => !isReadOnly && setStatusModalOpen(true)}
                disabled={isReadOnly}
                error={isSubmitted && !formData.statusId}
                helperText={
                  isSubmitted && !formData.statusId
                    ? translate("Status is required")
                    : ""
                }
                hideDropdownArrow
              />
            </Grid>
          </Grid>
        </Collapse>

        {/* Period Details Section */}
        <Box sx={{ mt: 3 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 1.5,
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
              <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                <T>Period Details</T>
              </Typography>
            </Box>
          </Box>

          {!lockGeneratedFields && (
            <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 1.5 }}>
              <Button
                size="small"
                startIcon={<SettingsIcon />}
                onClick={() => handleGenerateDetails(formData, setFormData)}
                sx={{
                  fontWeight: 700,
                  textTransform: "none",
                  color: "var(--lms-primary)",
                  border: "1px solid var(--lms-primary)",
                  borderRadius: "8px",
                  px: 1.5,
                }}
              >
                <T>Generate</T>
              </Button>
            </Box>
          )}

          <Box sx={{ border: "1px solid #e0e0e0", borderRadius: "10px", overflow: "hidden" }}>
            <Table size="small">
              <TableHead>
                <TableRow sx={{ bgcolor: "rgba(0,0,0,0.02)", borderBottom: "1px solid #e0e0e0" }}>
                  <TableCell sx={{ fontWeight: 800, width: 60, color: "var(--lms-text-main)" }}><T>S.No</T></TableCell>
                  <TableCell sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}><T>Periods</T></TableCell>
                  <TableCell sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}><T>From Date</T></TableCell>
                  <TableCell sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}><T>Till Date</T></TableCell>
                  <TableCell sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}><T>Disbursement Date</T></TableCell>
                  <TableCell sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}><T>Status</T></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {formData.details && formData.details.length > 0 ? (
                  formData.details.map((detail: any, index: number) => (
                    <TableRow key={index} sx={{ borderBottom: "1px solid #f0f0f0" }}>
                      <TableCell>{index + 1}</TableCell>
                      <TableCell>
                        <FormInput
                          fullWidth
                          size="small"
                          value={detail.periodstxt || ""}
                          onChange={(e) => {
                            const newDetails = [...formData.details];
                            newDetails[index].periodstxt = e.target.value;
                            setFormData({ ...formData, details: newDetails });
                          }}
                          disabled={isReadOnly}
                        />
                      </TableCell>
                      <TableCell>{detail.startDate ? dayjs(detail.startDate).format("DD-MMM-YYYY") : ""}</TableCell>
                      <TableCell>{detail.endDate ? dayjs(detail.endDate).format("DD-MMM-YYYY") : ""}</TableCell>
                      <TableCell>
                        <FormDatePicker
                          hideLabel
                          value={detail.disbDate ? dayjs(detail.disbDate) : null}
                          onChange={(val) => {
                            const newDetails = [...formData.details];
                            newDetails[index].disbDate = val ? val.toISOString() : null;
                            setFormData({ ...formData, details: newDetails });
                          }}
                          disabled={isReadOnly}
                        />
                      </TableCell>
                      <TableCell>
                        <LookupDropdown
                          hideLabel
                          value={detail.statusid ?? detail.statusId ?? ""}
                          placeholder={translate("Select Status")}
                          options={detailStatuses}
                          getOptionLabel={(opt) => opt.stxt || opt.name || opt.code || ""}
                          onSelect={(row) => {
                            selectDetailStatus(index, row);
                          }}
                          disabled={isReadOnly}
                          onSearchClick={() => !isReadOnly && setDetailStatusModalIndex(index)}
                          sx={{
                            minWidth: 140,
                            "& .MuiInputBase-root": {
                              minHeight: 36,
                            },
                          }}
                        />
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={6} align="center" sx={{ py: 6, color: "text.secondary" }}>
                      <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 1, opacity: 0.5 }}>
                        <SearchIcon sx={{ fontSize: "3rem", color: "#90a4ae" }} />
                        <Typography sx={{ fontWeight: 700, fontSize: "0.95rem", color: "#607d8b" }}>
                          <T>No Record Found</T>
                        </Typography>
                      </Box>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </Box>
        </Box>

        <GenericSearchModal
          open={registerModalOpen}
          onClose={() => setRegisterModalOpen(false)}
          title={translate("Payroll Register")}
          columns={[
            { id: "code", label: translate("Code"), minWidth: 50 },
            { id: "stxt", label: translate("Payroll Register"), minWidth: 200 },
            { id: "ltxt", label: translate("Description"), minWidth: 300 }
          ]}
          data={payrollRegisters}
          onSelect={(row) => {
            selectPayrollRegister(row);
            setRegisterModalOpen(false);
          }}
        />

        <GenericSearchModal
          open={typeModalOpen}
          onClose={() => setTypeModalOpen(false)}
          title={translate("Payroll Type")}
          columns={[
            { id: "code", label: translate("Code"), minWidth: 80 },
            { id: "stxt", label: translate("Name") }
          ]}
          data={attendanceTypes}
          onSelect={(row) => {
            setFormData({ ...formData, payrollTypeId: row.id, payrollType: row.stxt || row.name || "" });
            setTypeModalOpen(false);
          }}
        />

        <GenericSearchModal
          open={statusModalOpen}
          onClose={() => setStatusModalOpen(false)}
          title={translate("Status")}
          columns={[
            { id: "code", label: translate("Code"), minWidth: 80 },
            { id: "stxt", label: translate("Name") }
          ]}
          data={statuses}
          onSelect={(row) => {
            setFormData({ ...formData, statusId: row.id, status: row.stxt || row.name || "" });
            setStatusModalOpen(false);
          }}
        />

        <GenericSearchModal
          open={detailStatusModalIndex !== null}
          onClose={() => setDetailStatusModalIndex(null)}
          title={translate("Status")}
          columns={[
            { id: "code", label: translate("Code"), minWidth: 80 },
            { id: "stxt", label: translate("Name") }
          ]}
          data={detailStatuses}
          onSelect={(row) => {
            if (detailStatusModalIndex !== null) {
              selectDetailStatus(detailStatusModalIndex, row);
            }
            setDetailStatusModalIndex(null);
          }}
        />
      </Box>
    </FormThemeWrapper>
  );
};

export default PayrollPeriod;
