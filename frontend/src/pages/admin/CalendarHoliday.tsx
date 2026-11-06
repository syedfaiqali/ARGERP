import React, { useState, useEffect, useCallback } from "react";
import {
  Grid,
  Box,
  Typography,
  Collapse,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
} from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Delete as DeleteIcon,
} from "@mui/icons-material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { FormDatePicker } from "@/components/FormDatePicker";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LookupDropdown } from "@/components/LookupDropdown";
import dayjs from "dayjs";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService, PostService, PutService, DeleteService } from "@/constants/globalItems";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { calendarHolidaySlice } from "@/store/humanResourceSlices";

interface CalendarHolidayProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const APPLY_ON_DEPARTMENT = 584;
const APPLY_ON_EMPLOYEE_TYPE = 585;
const SHARED_WITH_ENTITY = 520;

const normalizeLookupRows = (rows: any[] = []) =>
  rows.map((row) => ({
    ...row,
    id: row.id ?? row.Id,
    code: row.code ?? row.Code ?? "",
    name:
      row.name ??
      row.Name ??
      row.stxt ??
      row.Stxt ??
      row.departmentName ??
      row.DepartmentName ??
      row.instituteName ??
      row.InstituteName ??
      row.shortName ??
      row.ShortName ??
      "",
  }));

const getLookupLabel = (row: any) =>
  String(row?.name ?? row?.Name ?? row?.stxt ?? row?.Stxt ?? row?.ltxt ?? row?.Ltxt ?? row?.code ?? row?.Code ?? "");

const normalizeLookupKey = (value: any) =>
  String(value ?? "")
    .toLowerCase()
    .replace(/[^a-z0-9]/g, "");

const isDepartmentApplyOnOption = (row: any, id?: any) => {
  const value = id ?? row?.id ?? row?.Id;
  const key = normalizeLookupKey(getLookupLabel(row));
  return Number(value) === APPLY_ON_DEPARTMENT || key.includes("department") || key.includes("depart");
};

const isEmployeeTypeApplyOnOption = (row: any, id?: any) => {
  const value = id ?? row?.id ?? row?.Id;
  const key = normalizeLookupKey(getLookupLabel(row));
  return Number(value) === APPLY_ON_EMPLOYEE_TYPE || key.includes("employeetype") || key.includes("emptype");
};

const isEntitySharedWithOption = (row: any, id?: any) => {
  const value = id ?? row?.id ?? row?.Id;
  const key = normalizeLookupKey(getLookupLabel(row));
  return Number(value) === SHARED_WITH_ENTITY || key === "entity" || key.includes("entity");
};

const CalendarHoliday: React.FC<CalendarHolidayProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();

  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrCalendarHoliday",
    calendarHolidaySlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    id: null,
    code: "",
    calendarHoliday: "",
    payrollPeriodId: null,
    payrollPeriod: "",
    applyOnId: null,
    applyOn: "",
    sharedWithId: null,
    sharedWith: "",
    isActive: true,
    details: [],
    periodRows: [],
    applyOnDetails: [],
    sharedDetails: [],
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  // sNo is a display index, derived here rather than cached.
  const rows = React.useMemo<any[]>(
    () =>
      (cachedRows || []).map((item: any, index: number) => ({
        id: item.id,
        sNo: index + 1,
        code: item.code,
        calendarHoliday: item.calendarHoliday,
        payrollPeriodId: item.payrollPeriodId,
        payrollPeriod: item.payrollPeriod,
        applyOnId: item.applyOnId,
        applyOn: item.applyOn,
        sharedWithId: item.sharedWithId,
        sharedWith: item.sharedWith,
        isActive: item.isActive,
        details: item.details || [],
        periodRows: item.periodRows || [],
        applyOnDetails: item.applyOnDetails || [],
        sharedDetails: item.sharedDetails || [],
      })),
    [cachedRows],
  );

  useEffect(() => {
    // Rows are served from the cache.
  }, []);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Calendar Holiday"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Calendar Holiday"));
    else if (viewMode === "view") onSubViewChange(translate("View Calendar Holiday"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleSave = async (data: any) => {
    const validationErrors: string[] = [];
    if (!data.calendarHoliday?.trim()) validationErrors.push(translate("Calendar Holiday is required"));
    if (!data.payrollPeriodId) validationErrors.push(translate("Payroll Period is required"));
    if (!data.applyOnId) validationErrors.push(translate("Apply On is required"));
    if (!data.sharedWithId) validationErrors.push(translate("Shared With is required"));
    if (!(data.details || []).length) validationErrors.push(translate("Holiday Details cannot be empty"));
    if (!(data.applyOnDetails || []).length) validationErrors.push(translate("Apply On Detail(s) cannot be empty"));
    if (!(data.sharedDetails || []).length) validationErrors.push(translate("Shared With Detail(s) cannot be empty"));
    if (validationErrors.length) {
      setErrorMessages(validationErrors);
      setShowError(true);
      return false;
    }

    setApiLoading(true);
    try {
      const saveData = { ...data };
      delete saveData.periodRows;
      const payload = {
        ...saveData,
        details: data.details || [],
        applyOnDetails: data.applyOnDetails || [],
        sharedDetails: data.sharedDetails || [],
      };

      let response: any;
      if (data.id) {
        response = await PutService("/api/HumanResource/CalendarHoliday/v3", payload);
      } else {
        response = await PostService("/api/HumanResource/CalendarHoliday/v3", payload);
      }

      if (response && (response.status === 200 || response.status === 201)) {
        setErrorMessages([
          data.id
            ? translate("Record updated successfully")
            : translate("Record saved successfully"),
        ]);
        setShowError(true);
        dispatch(calendarHolidaySlice.fetchAll(true));
        return true;
      }
    } catch (error: any) {
      console.error("Save failed", error);
      const responseData = error.response?.data;
      const message = responseData?.message ?? responseData?.Message ?? responseData ?? "Operation failed";
      setErrorMessages(Array.isArray(message) ? message.map(String) : [String(message)]);
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
    return false;
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      const response: any = await DeleteService(`/api/HumanResource/CalendarHoliday/v3/${row.id}`);
      if (response && (response.status === 204 || response.status === 200)) {
        setErrorMessages([translate("Record deleted successfully")]);
        setShowError(true);
        dispatch(calendarHolidaySlice.fetchAll(true));
      }
    } catch (error) {
      console.error("Delete failed", error);
    } finally {
      setApiLoading(false);
    }
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Code"), minWidth: 80 },
    { id: "calendarHoliday", label: translate("Calendar Holiday"), minWidth: 200 },
    { id: "payrollPeriod", label: translate("Payroll Period"), minWidth: 150 },
    { id: "applyOn", label: translate("Apply On"), minWidth: 120 },
    { id: "sharedWith", label: translate("Shared With"), minWidth: 120 },
  ];

  const initialFormData = {
    id: null,
    code: "",
    calendarHoliday: "",
    payrollPeriodId: null,
    payrollPeriod: "",
    applyOnId: null,
    applyOn: "",
    sharedWithId: null,
    sharedWith: "",
    isActive: true,
    details: [],
    periodRows: [],
    applyOnDetails: [],
    sharedDetails: [],
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
                ? translate("New Calendar Holiday")
                : viewMode === "edit"
                  ? translate("Edit Calendar Holiday")
                  : translate("View Calendar Holiday")}
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
            <CalendarHolidayForm
              formData={formData}
              setFormData={setFormData}
              isReadOnly={viewMode === "view"}
              isSubmitted={isSubmitted}
            />
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Calendar Holiday")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          actionsPosition="right"
          onAddClick={() => {
            setFormData({ ...initialFormData });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => {
            setFormData({ ...row });
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({ ...row });
            setIsSubmitted(false);
            setViewMode("view");
          }}
          hideTitle
          showAddButton={true}
          showToaster={false}
        />
      )}
      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
      />
    </>
  );
};

const CalendarHolidayForm: React.FC<{
  formData: any;
  setFormData: any;
  isReadOnly?: boolean;
  isSubmitted?: boolean;
}> = ({ formData, setFormData, isReadOnly, isSubmitted }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [payrollModalOpen, setPayrollModalOpen] = useState(false);
  const [applyOnModalOpen, setApplyOnModalOpen] = useState(false);
  const [sharedWithModalOpen, setSharedWithModalOpen] = useState(false);
  const [holidayModalOpen, setHolidayModalOpen] = useState(false);
  const [copyFromOpen, setCopyFromOpen] = useState(false);
  const [copySources, setCopySources] = useState<any[]>([]);
  const [calendarOpen, setCalendarOpen] = useState(false);
  const [selectedPeriodRow, setSelectedPeriodRow] = useState<any>(null);
  const [isApplyOnDetailExpanded, setIsApplyOnDetailExpanded] = useState(true);
  const [isSharedDetailExpanded, setIsSharedDetailExpanded] = useState(true);

  const [payrollPeriods, setPayrollPeriods] = useState<any[]>([]);
  const [applyOnOptions, setApplyOnOptions] = useState<any[]>([]);
  const [sharedWithOptions, setSharedWithOptions] = useState<any[]>([]);
  const [holidayTypes, setHolidayTypes] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [employeeTypes, setEmployeeTypes] = useState<any[]>([]);
  const [entities, setEntities] = useState<any[]>([]);

  useEffect(() => {
    const fetchLookups = async () => {
      try {
        const [ppResp, applyOnResp, sharedWithResp, holidayTypeResp, departmentResp, employeeTypeResp, entityResp] = await Promise.all([
          GetService("/api/HumanResource/PayrollPeriod/v3"),
          GetService("/api/HumanResource/Lov/v3/APLON"),
          GetService("/api/HumanResource/Lov/v3/SSD"),
          GetService("/api/HumanResource/HolidayType/v3"),
          GetService("/api/HumanResource/Lookups/v3/departments"),
          GetService("/api/HumanResource/Lov/v3/EMPTP"),
          // Restrict shared entities to the campuses assigned to the logged-in user.
          GetService("/api/auth/Legalentity/v3/Legalentity")
        ]);

        const ppData = (ppResp as any).data;
        const ppItems = ppData?.items ?? ppData?.Items ?? ppData ?? [];
        setPayrollPeriods(Array.isArray(ppItems) ? ppItems : []);
        setApplyOnOptions(normalizeLookupRows((applyOnResp as any).data || []));
        setSharedWithOptions(normalizeLookupRows((sharedWithResp as any).data || []));

        const htData = (holidayTypeResp as any).data;
        setHolidayTypes(htData?.items || htData || []);

        const deptData = (departmentResp as any).data;
        setDepartments(normalizeLookupRows(deptData?.items ?? deptData?.Items ?? deptData ?? []));

        const empTypeData = (employeeTypeResp as any).data;
        setEmployeeTypes(normalizeLookupRows(empTypeData?.items ?? empTypeData?.Items ?? empTypeData ?? []));

        const entityData = (entityResp as any).data;
        setEntities(normalizeLookupRows(entityData?.items ?? entityData?.Items ?? entityData ?? []));
      } catch (error) {
        console.error("Failed to fetch lookups", error);
      }
    };
    fetchLookups();
  }, []);

  // Legacy Calendar Holiday supports Entity sharing only.
  const visibleSharedWithOptions = sharedWithOptions.filter((option) => isEntitySharedWithOption(option));

  const getApplyOnDetailKey = () =>
    isDepartmentApplyOnOption({ name: formData.applyOn, id: formData.applyOnId }) ? "departId" : "empTypeId";

  const isApplyOnSelected = (id: any) => {
    const key = getApplyOnDetailKey();
    return (formData.applyOnDetails || []).some((detail: any) => String(detail[key]) === String(id));
  };

  const handleApplyOnSelect = (row: any) => {
    setFormData({
      ...formData,
      applyOn: row.name || row.code,
      applyOnId: row.id,
      applyOnDetails: [],
    });
  };

  const toggleApplyOnDetail = (id: any, checked: boolean) => {
    const key = getApplyOnDetailKey();
    const current = formData.applyOnDetails || [];
    const next = checked
      ? [
          ...current.filter((detail: any) => String(detail[key]) !== String(id)),
          {
            id: null,
            departId: key === "departId" ? id : null,
            empTypeId: key === "empTypeId" ? id : null,
          },
        ]
      : current.filter((detail: any) => String(detail[key]) !== String(id));

    setFormData({ ...formData, applyOnDetails: next });
  };

  const toggleAllApplyOnDetails = (rows: any[], checked: boolean) => {
    const key = getApplyOnDetailKey();
    const rowIds = new Set(rows.map((row) => String(row.id)));
    const current = formData.applyOnDetails || [];
    const retained = current.filter((detail: any) => !rowIds.has(String(detail[key])));
    const selected = checked
      ? rows.map((row) => ({
          id: null,
          departId: key === "departId" ? row.id : null,
          empTypeId: key === "empTypeId" ? row.id : null,
        }))
      : [];

    setFormData({ ...formData, applyOnDetails: [...retained, ...selected] });
  };

  const isSharedEntitySelected = (id: any) =>
    (formData.sharedDetails || []).some((detail: any) => String(detail.entityId ?? detail.EntityId) === String(id));

  const handleSharedWithSelect = (row: any) => {
    setFormData({
      ...formData,
      sharedWith: row.name || row.code,
      sharedWithId: row.id,
      sharedDetails: [],
    });
  };

  const toggleSharedEntity = (id: any, checked: boolean) => {
    const current = formData.sharedDetails || [];
    const next = checked
      ? [
          ...current.filter((detail: any) => String(detail.entityId ?? detail.EntityId) !== String(id)),
          {
            id: null,
            sharedWithId: formData.sharedWithId,
            clientId: null,
            entityId: id,
            regionId: null,
          },
        ]
      : current.filter((detail: any) => String(detail.entityId ?? detail.EntityId) !== String(id));

    setFormData({ ...formData, sharedDetails: next });
  };

  const renderApplyOnSelection = () => {
    const selectedApplyOn = applyOnOptions.find((option) => String(option.id) === String(formData.applyOnId)) ?? {
      id: formData.applyOnId,
      name: formData.applyOn,
    };
    const isDepartment = isDepartmentApplyOnOption(selectedApplyOn);
    const isEmployeeType = isEmployeeTypeApplyOnOption(selectedApplyOn);
    if (!isDepartment && !isEmployeeType) return null;

    const title = isDepartment ? translate("Department") : translate("Employee Type");
    const data = isDepartment ? departments : employeeTypes;
    const nameField = isDepartment ? "departmentName" : "name";
    const allApplyOnSelected = data.length > 0 && data.every((row: any) => isApplyOnSelected(row.id));
    const applyOnDetailColumns: Column[] = [
      {
        id: "select",
        label: translate("Select"),
        headerFormat: (
          <LmsCheckbox
            hideLabel
            checked={allApplyOnSelected}
            onChange={(checked) => toggleAllApplyOnDetails(data, checked)}
            disabled={isReadOnly || data.length === 0}
          />
        ),
        minWidth: 80,
        format: (_, row) => (
          <LmsCheckbox
            hideLabel
            checked={isApplyOnSelected(row.id)}
            onChange={(checked) => toggleApplyOnDetail(row.id, checked)}
            disabled={isReadOnly}
          />
        ),
      },
      { id: "code", label: translate("Code"), minWidth: 120 },
      {
        id: nameField,
        label: title,
        minWidth: 240,
        format: (_, row) => row[nameField] || row.name || row.departmentName || "",
      },
    ];

    return (
      <Box sx={{ mt: 3 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 1.5,
            cursor: "pointer",
            userSelect: "none",
          }}
          onClick={() => setIsApplyOnDetailExpanded(!isApplyOnDetailExpanded)}
        >
          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
            {title}
          </Typography>
          {renderExpandIcon(isApplyOnDetailExpanded)}
        </Box>
        <Collapse in={isApplyOnDetailExpanded}>
          <SchemaTable
            title={title}
            columns={applyOnDetailColumns}
            rows={data}
            pagination={false}
            filtersEnabled={false}
            downloadEnabled={false}
            showToaster={false}
            showAddButton={false}
            actions={false}
            hideTitle
            maxHeight={320}
            stickyHeader
          />
        </Collapse>
      </Box>
    );
  };

  const renderSharedWithSelection = () => {
    const selectedSharedWith = sharedWithOptions.find((option) => String(option.id) === String(formData.sharedWithId)) ?? {
      id: formData.sharedWithId,
      name: formData.sharedWith,
    };
    if (!isEntitySharedWithOption(selectedSharedWith)) return null;
    const sharedWithDetailColumns: Column[] = [
      {
        id: "select",
        label: translate("Select"),
        minWidth: 80,
        format: (_, row) => (
          <LmsCheckbox
            hideLabel
            checked={isSharedEntitySelected(row.id)}
            onChange={(checked) => toggleSharedEntity(row.id, checked)}
            disabled={isReadOnly}
          />
        ),
      },
      { id: "code", label: translate("Code"), minWidth: 120 },
      {
        id: "entity",
        label: translate("Entity"),
        minWidth: 260,
        format: (_, row) => row.instituteName || row.shortName || row.name || "",
      },
    ];

    return (
      <Box sx={{ mt: 3 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 1.5,
            cursor: "pointer",
            userSelect: "none",
          }}
          onClick={() => setIsSharedDetailExpanded(!isSharedDetailExpanded)}
        >
          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
            <T>Shared With Detail</T>
          </Typography>
          {renderExpandIcon(isSharedDetailExpanded)}
        </Box>
        <Collapse in={isSharedDetailExpanded}>
          <SchemaTable
            title={translate("Shared With Detail")}
            columns={sharedWithDetailColumns}
            rows={entities}
            pagination={false}
            filtersEnabled={false}
            downloadEnabled={false}
            showToaster={false}
            showAddButton={false}
            actions={false}
            hideTitle
            maxHeight={320}
            stickyHeader
          />
        </Collapse>
      </Box>
    );
  };

  const sortPeriodRows = (rows: any[] = []) => [...rows].sort((a: any, b: any) => {
    const startDateA = a.startDate ?? a.Startdate ?? a.startdate;
    const startDateB = b.startDate ?? b.Startdate ?? b.startdate;
    const valueA = startDateA && dayjs(startDateA).isValid()
      ? dayjs(startDateA).valueOf()
      : Number.MAX_SAFE_INTEGER;
    const valueB = startDateB && dayjs(startDateB).isValid()
      ? dayjs(startDateB).valueOf()
      : Number.MAX_SAFE_INTEGER;
    return valueA - valueB;
  });

  const mapPayrollPeriodRows = (periodDetails: any[] = []) =>
    sortPeriodRows(periodDetails.map((pd: any) => {
      const startDate = pd.startDate ?? pd.Startdate ?? pd.startdate ?? "";
      const endDate = pd.endDate ?? pd.Enddate ?? pd.enddate ?? "";
      return {
        id: pd.id ?? pd.Id,
        payPeriodDetailId: pd.id ?? pd.Id,
        year: pd.year ?? pd.Year ?? "",
        period: pd.periodstxt ?? pd.periodsTxt ?? pd.Periodstxt ?? (startDate ? dayjs(startDate).format("MMMM") : ""),
        startDate,
        endDate,
        days: pd.noofdays ?? pd.noOfDays ?? pd.Noofdays ?? 0,
        lineIndex: pd.lineIndex ?? pd.LineIndex ?? 0,
      };
    }));

  useEffect(() => {
    if (!formData.payrollPeriodId || (formData.periodRows || []).length > 0) return;

    let isActive = true;
    const fetchPeriodRows = async () => {
      try {
        const resp: any = await GetService(`/api/HumanResource/PayrollPeriod/v3/${formData.payrollPeriodId}/details`);
        if (!isActive) return;
        const rows = mapPayrollPeriodRows(resp.data || []);
        setFormData((prev: any) =>
          prev.payrollPeriodId === formData.payrollPeriodId && !(prev.periodRows || []).length
            ? { ...prev, periodRows: rows }
            : prev
        );
      } catch (error) {
        console.error("Failed to fetch payroll period rows", error);
      }
    };

    fetchPeriodRows();
    return () => {
      isActive = false;
    };
  }, [formData.payrollPeriodId, formData.periodRows]);

  const handlePayrollPeriodSelect = async (row: any) => {
    setPayrollModalOpen(false);

    try {
      const resp: any = await GetService(`/api/HumanResource/PayrollPeriod/v3/${row.id}/details`);
      const periodRows = mapPayrollPeriodRows(resp.data || []);

      setFormData({
        ...formData,
        payrollPeriod: row.name || row.ltxt || row.code,
        payrollPeriodId: row.id,
        periodRows,
        details: []
      });
    } catch (error) {
      console.error("Failed to fetch payroll details", error);
      setFormData({
        ...formData,
        payrollPeriod: row.name || row.ltxt || row.code,
        payrollPeriodId: row.id,
        periodRows: [],
        details: []
      });
    }
  };

  const getPayPeriodDetailId = (row: any) =>
    row?.payPeriodDetailId ?? row?.payperioddtlid ?? row?.payPeriodDtlId ?? row?.id;

  const getDayCount = (startDate: any, endDate: any) => {
    if (!startDate || !endDate) return 0;
    const days = dayjs(endDate).startOf("day").diff(dayjs(startDate).startOf("day"), "day") + 1;
    return Number.isFinite(days) && days > 0 ? days : 0;
  };

  const getPeriodRows = () => {
    const rows = formData.periodRows || [];
    if (rows.length > 0) return sortPeriodRows(rows);

    const uniqueRows = new Map<string, any>();
    (formData.details || []).forEach((detail: any) => {
      const periodId = getPayPeriodDetailId(detail);
      if (!periodId || uniqueRows.has(String(periodId))) return;
      uniqueRows.set(String(periodId), {
        id: periodId,
        payPeriodDetailId: periodId,
        year: detail.year ?? "",
        period: detail.period ?? "",
        startDate: detail.startDate ?? "",
        endDate: detail.endDate ?? "",
        days: detail.days ?? 0,
      });
    });
    return sortPeriodRows(Array.from(uniqueRows.values()));
  };

  const openCopyFrom = async () => {
    if (formData.id || !formData.payrollPeriodId || (formData.details || []).length) return;
    const response: any = await GetService("/api/HumanResource/CalendarHoliday/v3");
    setCopySources((response.data?.items || []).filter((item: any) => item.id !== formData.id));
    setCopyFromOpen(true);
  };

  const copyDetailsFrom = async (source: any) => {
    const response: any = await GetService(`/api/HumanResource/CalendarHoliday/v3/copy-from/${source.id}/payroll-period/${formData.payrollPeriodId}`);
    setFormData({ ...formData, details: response.data || [] });
    setCopyFromOpen(false);
  };

  const getHolidayRowsForSelectedPeriod = () => {
    const selectedPeriodId = getPayPeriodDetailId(selectedPeriodRow);
    return (formData.details || [])
      .filter(
        (detail: any) => String(getPayPeriodDetailId(detail)) === String(selectedPeriodId)
      )
      .slice()
      .sort((left: any, right: any) => {
        const leftDate = left.startDate && dayjs(left.startDate).isValid()
          ? dayjs(left.startDate).valueOf()
          : Number.MAX_SAFE_INTEGER;
        const rightDate = right.startDate && dayjs(right.startDate).isValid()
          ? dayjs(right.startDate).valueOf()
          : Number.MAX_SAFE_INTEGER;
        return leftDate - rightDate;
      });
  };

  const handleOpenHolidayDetails = (row: any) => {
    setSelectedPeriodRow(row);
    setHolidayModalOpen(true);
  };

  const handleAddHolidayDetail = () => {
    if (!selectedPeriodRow) return;
    const defaultDate = selectedPeriodRow.startDate || "";
    const newDetail = {
      id: -Math.floor(Math.random() * 1000000),
      holidayTypeId: null,
      description: "",
      startDate: defaultDate,
      endDate: defaultDate,
      days: defaultDate ? 1 : 0,
      payPeriodDetailId: getPayPeriodDetailId(selectedPeriodRow),
      year: selectedPeriodRow.year ?? "",
      period: selectedPeriodRow.period ?? "",
    };
    setFormData({
      ...formData,
      details: [...(formData.details || []), newDetail],
    });
  };

  const handleRemoveHolidayDetail = (row: any) => {
    setFormData({
      ...formData,
      details: (formData.details || []).filter((d: any) => d.id !== row.id),
    });
  };

  const handleDetailChange = (id: number, key: string, value: any) => {
    setFormData({
      ...formData,
      details: (formData.details || []).map((d: any) => {
        if (d.id !== id) return d;
        const next = { ...d, [key]: value };
        if (key === "startDate" || key === "endDate") {
          next.days = getDayCount(next.startDate, next.endDate);
        }
        return next;
      }),
    });
  };

  const periodColumns: Column[] = [
    {
      id: "index",
      label: translate("S.No"),
      minWidth: 60,
      format: (_, __, index) => index! + 1
    },
    {
      id: "year",
      label: translate("Year"),
      minWidth: 100,
    },
    {
      id: "period",
      label: translate("Period"),
      minWidth: 150,
    },
    {
      id: "startDate",
      label: translate("Start Date"),
      minWidth: 160,
      format: (val) => val ? dayjs(val).format("DD-MMM-YYYY") : ""
    },
    {
      id: "endDate",
      label: translate("End Date"),
      minWidth: 160,
      format: (val) => val ? dayjs(val).format("DD-MMM-YYYY") : ""
    },
    {
      id: "holidayDetails",
      label: translate("Holiday Details"),
      minWidth: 170,
      format: (_, row) => {
        const count = (formData.details || []).filter(
          (detail: any) => String(getPayPeriodDetailId(detail)) === String(getPayPeriodDetailId(row))
        ).length;
        return (
          <Button
            variant="text"
            size="small"
            onClick={() => handleOpenHolidayDetails(row)}
            sx={{
              minWidth: 0,
              p: 0,
              textTransform: "none",
              fontWeight: 700,
              textDecoration: "none",
              "&:hover": {
                bgcolor: "transparent",
                textDecoration: "underline",
              },
            }}
          >
            {count > 0 ? `${count} ${translate("Holiday(s)")}` : translate("Add")}
          </Button>
        );
      }
    }
  ];

  const holidayDetailColumns: Column[] = [
    {
      id: "index",
      label: translate("S.No"),
      minWidth: 60,
      format: (_, __, index) => index! + 1
    },
    {
      id: "holidayTypeId",
      label: translate("Holiday Type"),
      minWidth: 200,
      format: (val, row) => (
        <LookupDropdown
          value={holidayTypes.find(ht => ht.id == val)?.holidayType || ""}
          options={holidayTypes}
          getOptionLabel={(opt) => opt.holidayType || opt.code || ""}
          onSelect={(ht) => handleDetailChange(row.id, "holidayTypeId", ht.id)}
          placeholder={translate("Select Type")}
          disabled={isReadOnly}
          hideLabel
          hideDropdownArrow
        />
      )
    },
    {
      id: "description",
      label: translate("Description"),
      minWidth: 220,
      format: (val, row) => (
        <FormInput
          value={val || ""}
          placeholder={translate("Description")}
          onChange={(e) => handleDetailChange(row.id, "description", e.target.value)}
          disabled={isReadOnly}
          size="small"
          hideLabel
        />
      )
    },
    {
      id: "startDate",
      label: translate("Start Date"),
      minWidth: 160,
      format: (val, row) => (
        <FormDatePicker
          value={val ? dayjs(val) : null}
          onChange={(date) => handleDetailChange(row.id, "startDate", date ? date.toISOString() : "")}
          disabled={isReadOnly}
          format="DD-MMM-YYYY"
          hideLabel
        />
      )
    },
    {
      id: "endDate",
      label: translate("End Date"),
      minWidth: 160,
      format: (val, row) => (
        <FormDatePicker
          value={val ? dayjs(val) : null}
          onChange={(date) => handleDetailChange(row.id, "endDate", date ? date.toISOString() : "")}
          disabled={isReadOnly}
          format="DD-MMM-YYYY"
          hideLabel
        />
      )
    },
    {
      id: "days",
      label: translate("Days"),
      minWidth: 90,
    },
    {
      id: "actions",
      label: "",
      minWidth: 60,
      format: (_, row) => (
        <IconButton
          size="small"
          onClick={() => handleRemoveHolidayDetail(row)}
          disabled={isReadOnly}
          sx={{ color: "var(--lms-error, #d32f2f)" }}
        >
          <DeleteIcon fontSize="small" />
        </IconButton>
      )
    }
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
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("Code")}
                value={formData.code || ""}
                onChange={(e) =>
                  setFormData({ ...formData, code: e.target.value })
                }
                disabled
                placeholder={translate("Auto Generated")}
              />
            </Grid>
            <Grid item xs={12} md={8}>
              <FormInput
                label={translate("Calendar Holiday*")}
                placeholder={translate("Enter Calendar Holiday")}
                value={formData.calendarHoliday || ""}
                onChange={(e) =>
                  setFormData({ ...formData, calendarHoliday: e.target.value })
                }
                disabled={isReadOnly}
                error={isSubmitted && !formData.calendarHoliday}
                helperText={
                  isSubmitted && !formData.calendarHoliday
                    ? translate("Calendar Holiday is required")
                    : ""
                }
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <LookupDropdown
                label={translate("Payroll Period")}
                value={formData.payrollPeriod || ""}
                options={payrollPeriods}
                getOptionLabel={(opt) => opt.name || opt.ltxt || opt.code || ""}
                onSelect={handlePayrollPeriodSelect}
                onSearchClick={() => !isReadOnly && setPayrollModalOpen(true)}
                placeholder={translate("Select Period")}
                disabled={isReadOnly}
                hideDropdownArrow
              />
              <GenericSearchModal
                open={payrollModalOpen}
                onClose={() => setPayrollModalOpen(false)}
                title={translate("Select Payroll Period")}
                columns={[
                  { id: "code", label: translate("Code"), minWidth: 80 },
                  { id: "ltxt", label: translate("Name"), minWidth: 200 },
                ]}
                data={payrollPeriods}
                onSelect={handlePayrollPeriodSelect}
                selectedValue={formData.payrollPeriodId}
                selectedField="id"
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <LookupDropdown
                label={translate("Apply On")}
                value={formData.applyOn || ""}
                options={applyOnOptions}
                getOptionLabel={(opt) => opt.name || opt.code || ""}
                onSelect={(row) => {
                  handleApplyOnSelect(row);
                }}
                onSearchClick={() => !isReadOnly && setApplyOnModalOpen(true)}
                placeholder={translate("Select Apply On")}
                disabled={isReadOnly}
                hideDropdownArrow
              />
              <GenericSearchModal
                open={applyOnModalOpen}
                onClose={() => setApplyOnModalOpen(false)}
                title={translate("Select Apply On")}
                columns={[
                  { id: "code", label: translate("Code"), minWidth: 80 },
                  { id: "name", label: translate("Name"), minWidth: 200 },
                ]}
                data={applyOnOptions}
                onSelect={(row) => {
                  handleApplyOnSelect(row);
                  setApplyOnModalOpen(false);
                }}
                selectedValue={formData.applyOnId}
                selectedField="id"
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <LookupDropdown
                label={translate("Shared With")}
                value={formData.sharedWith || ""}
                options={visibleSharedWithOptions}
                getOptionLabel={(opt) => opt.name || opt.code || ""}
                onSelect={(row) => {
                  handleSharedWithSelect(row);
                }}
                onSearchClick={() => !isReadOnly && setSharedWithModalOpen(true)}
                placeholder={translate("Select Shared With")}
                disabled={isReadOnly}
                hideDropdownArrow
              />
              <GenericSearchModal
                open={sharedWithModalOpen}
                onClose={() => setSharedWithModalOpen(false)}
                title={translate("Select Shared With")}
                columns={[
                  { id: "code", label: translate("Code"), minWidth: 80 },
                  { id: "name", label: translate("Name"), minWidth: 200 },
                ]}
                data={visibleSharedWithOptions}
                onSelect={(row) => {
                  handleSharedWithSelect(row);
                  setSharedWithModalOpen(false);
                }}
                selectedValue={formData.sharedWithId}
                selectedField="id"
              />
            </Grid>
          </Grid>
          {renderApplyOnSelection()}
          {renderSharedWithSelection()}
        </Collapse>

        <Box sx={{ mt: 4 }}>
          <Box sx={{ display: "flex", gap: 1, mb: 1 }}>
            <Button size="small" onClick={() => setCalendarOpen(true)} disabled={!formData.details?.length}><T>Calendar</T></Button>
            <Button size="small" onClick={openCopyFrom} disabled={isReadOnly || !!formData.id || !formData.payrollPeriodId || !!formData.details?.length}><T>Copy From</T></Button>
          </Box>
          <SchemaTable
            title={translate("Holiday Details")}
            addButtonLabel={''}
            showAddButton={false}
            columns={periodColumns}
            rows={getPeriodRows()}
            defaultLatestFirst={false}
            pagination={false}
            filtersEnabled={false}
            downloadEnabled={false}
            showToaster={false}
            maxHeight={400}
            stickyHeader
            actions={false}
          />
        </Box>
        <Dialog
          open={holidayModalOpen}
          onClose={() => setHolidayModalOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogTitle sx={{ fontWeight: 900 }}>
            {translate("Holiday Details")}
            {selectedPeriodRow
              ? ` - ${selectedPeriodRow.period || ""}${selectedPeriodRow.startDate && selectedPeriodRow.endDate
                ? ` [${dayjs(selectedPeriodRow.startDate).format("DD-MMM-YYYY")} To ${dayjs(selectedPeriodRow.endDate).format("DD-MMM-YYYY")}]`
                : ""}`
              : ""}
          </DialogTitle>
          <DialogContent dividers>
            <SchemaTable
              title={translate("Holiday Details")}
              addButtonLabel={translate("Add")}
              showAddButton={!isReadOnly}
              columns={holidayDetailColumns}
              rows={getHolidayRowsForSelectedPeriod()}
              onAddClick={isReadOnly ? undefined : handleAddHolidayDetail}
              pagination={false}
              filtersEnabled={false}
              downloadEnabled={false}
              showToaster={false}
              maxHeight={360}
              stickyHeader
              actions={false}
              hideTitle
            />
          </DialogContent>
          <DialogActions sx={{ px: 3, py: 2 }}>
            <Button
              variant="contained"
              onClick={() => setHolidayModalOpen(false)}
              sx={{ borderRadius: "8px", textTransform: "none", fontWeight: 700 }}
            >
              {translate("Done")}
            </Button>
          </DialogActions>
        </Dialog>
        <Dialog open={copyFromOpen} onClose={() => setCopyFromOpen(false)} maxWidth="md" fullWidth>
          <DialogTitle><T>Copy From</T></DialogTitle>
          <DialogContent dividers>
            <SchemaTable title={translate("Calendar Holiday")} hideTitle showAddButton={false} actions={false} pagination={false} filtersEnabled={false} downloadEnabled={false} showToaster={false}
              columns={[{ id: "code", label: translate("Code"), minWidth: 100 }, { id: "calendarHoliday", label: translate("Calendar Holiday"), minWidth: 240 }, { id: "payrollPeriod", label: translate("Payroll Period"), minWidth: 180 }]}
              rows={copySources} onRowClick={(row: any) => copyDetailsFrom(row)} />
          </DialogContent>
        </Dialog>
        <Dialog open={calendarOpen} onClose={() => setCalendarOpen(false)} maxWidth="md" fullWidth>
          <DialogTitle><T>Calendar</T></DialogTitle>
          <DialogContent dividers>
            <SchemaTable title={translate("Holiday Details")} hideTitle showAddButton={false} actions={false} pagination={false} filtersEnabled={false} downloadEnabled={false} showToaster={false}
              columns={[{ id: "description", label: translate("Holiday"), minWidth: 240 }, { id: "startDate", label: translate("Start Date"), minWidth: 150, format: (v) => v ? dayjs(v).format("DD-MMM-YYYY") : "" }, { id: "endDate", label: translate("End Date"), minWidth: 150, format: (v) => v ? dayjs(v).format("DD-MMM-YYYY") : "" }]}
              rows={[...(formData.details || [])].sort((a, b) => dayjs(a.startDate).valueOf() - dayjs(b.startDate).valueOf())} />
          </DialogContent>
        </Dialog>
      </Box>
    </FormThemeWrapper>
  );
};

export default CalendarHoliday;
