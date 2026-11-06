import React, { useState, useEffect, useMemo } from "react";
import {
  Grid,
  Box,
  Typography,
  Button,
  IconButton,
  MenuItem,
  Collapse,
  InputAdornment,
  Paper,
} from "@mui/material";
import {
  Delete as DeleteIcon,
  Search as SearchIcon,
  Policy as PolicyIcon,
  EventBusy as EventBusyIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Assessment as AssessmentIcon,
} from "@mui/icons-material";
import { AttendancePolicyReportModal } from "./AttendancePolicyReportModal";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";
import { useAppDispatch, useCachedRows, useHrLookups, useHrLovs } from "@/hooks";
import { attendancePolicySlice } from "@/store/humanResourceSlices";
import {
  GetService,
} from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';

const getLovId = (opt: any) => opt?.id ?? opt?.Id ?? opt?.lovid ?? opt?.lovId ?? opt?.value;
const getLovCode = (opt: any) => opt?.code ?? opt?.Code ?? "";
const getLovLabel = (opt: any) => opt?.stxt ?? opt?.Stxt ?? opt?.name ?? opt?.Name ?? opt?.title ?? opt?.Title ?? opt?.code ?? opt?.Code ?? "";
const getLovLongLabel = (opt: any) => opt?.ltxt ?? opt?.Ltxt ?? opt?.longName ?? opt?.LongName ?? opt?.description ?? opt?.Description ?? getLovLabel(opt);
const normalizeLov = (opt: any) => ({
  ...opt,
  id: Number(getLovId(opt)),
  code: getLovCode(opt),
  stxt: getLovLabel(opt),
  ltxt: getLovLongLabel(opt),
});

const getAttendancePolicyKind = (policy: any) => {
  const code = String(policy?.code ?? policy?.Code ?? "").trim().toUpperCase();
  const name = String(policy?.stxt ?? policy?.Stxt ?? policy?.name ?? policy?.Name ?? "").trim().toUpperCase();
  const text = `${code} ${name}`;

  if (text.includes("LATE")) return "late";
  if (text.includes("EARLY")) return "early";
  if (text.includes("OVERTIME") || code === "OT") return "overtime";
  if (text.includes("ABSENT")) return "absent";
  return "";
};

interface AttendancePolicyProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const initialAttendancePolicyFormData = {
  id: null,
  code: "",
  policyName: "",
  policyTypeId: null,
  policyTypeName: "",
  applyOffDays: false,
  noOfExcuse: "",
  isActive: true,
  policyDetails: [],
  absentDetails: [],
};

const AttendancePolicy: React.FC<AttendancePolicyProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrAttendancePolicy",
    attendancePolicySlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "error",
  );

  // ATPOL/FNINC/PENTP are global LOV reference data shared across HR screens.
  const { lov } = useHrLovs(["ATPOL", "FNINC", "PENTP"]);
  const policyTypes = lov("ATPOL");
  const periodTypes = lov("FNINC");
  const penaltyTypes = lov("PENTP");
  const [viewMode, setViewMode] = useState<
    "list" | "new" | "edit" | "view" | "report"
  >("list");
  const [formData, setFormData] = useState<any>({
    ...initialAttendancePolicyFormData,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [showReportModal, setShowReportModal] = useState(false);
  // Institutions are client/entity-scoped and shared with the other HR screens.
  const { lookup } = useHrLookups(["institutions"]);
  const entities = lookup("institutions");

  // sNo is a display index, derived here rather than cached.
  const rows = useMemo(
    () => (cachedRows || []).map((row: any, index: number) => ({ ...row, sNo: index + 1 })),
    [cachedRows],
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Attendance Policy"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Attendance Policy"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Attendance Policy"));
    else if (viewMode === "report")
      onSubViewChange(translate("Attendance Policy Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const normalizedPolicyTypes = (policyTypes || []).filter((p) => p != null).map(normalizeLov);

  const columns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "policyName", label: translate("Policy Name"), minWidth: 200 },
    { id: "policyTypeName", label: translate("Policy Type"), minWidth: 200 },
  ];

  const validateForm = (formData: any) => {
    const errors: string[] = [];
    if (!formData.policyName) errors.push(translate("Policy Name is required"));
    if (!formData.policyTypeId)
      errors.push(translate("Policy Type is required"));

    const selectedPolicyType = normalizedPolicyTypes.find(
      (p) => Number(p.id) === Number(formData.policyTypeId),
    );
    const policyKind = getAttendancePolicyKind(selectedPolicyType);
    const needsPolicyDetails = ["late", "early", "overtime"].includes(policyKind);
    const needsAbsentDetails = ["late", "early", "absent"].includes(policyKind);

    if (
      needsPolicyDetails &&
      (!formData.policyDetails || formData.policyDetails.length === 0)
    ) {
      errors.push(translate("Policy Detail is required"));
    }
    if (
      needsAbsentDetails &&
      (!formData.absentDetails || formData.absentDetails.length === 0)
    ) {
      errors.push(translate("Absent Policy Detail is required"));
    }

    if (formData.policyDetails && formData.policyDetails.length > 0) {
      formData.policyDetails.forEach((d: any, idx: number) => {
        if (!d.periodTypeId)
          errors.push(
            translate(`Policy Detail ${idx + 1}: Period Type is required`),
          );
        if (!d.valTypeId)
          errors.push(
            translate(`Policy Detail ${idx + 1}: Type is required`),
          );
        if (d.value == null || d.value === "" || Number(d.value) <= 0)
          errors.push(
            translate(
              `Policy Detail ${idx + 1}: Value must be greater than zero`,
            ),
          );
        if (
          d.startTime == null ||
          d.startTime === "" ||
          d.endTime == null ||
          d.endTime === ""
        )
          errors.push(
            translate(
              `Policy Detail ${idx + 1}: Start Time and End Time are required`,
            ),
          );
        if (
          d.startTime != null &&
          d.startTime !== "" &&
          d.endTime != null &&
          d.endTime !== "" &&
          Number(d.endTime) < Number(d.startTime)
        )
          errors.push(
            translate(
              `Policy Detail ${idx + 1}: End Time cannot be less than Start Time`,
            ),
          );
      });
    }

    if (formData.absentDetails && formData.absentDetails.length > 0) {
      formData.absentDetails.forEach((d: any, idx: number) => {
        if (!d.periodTypeId)
          errors.push(
            translate(`Absent Detail ${idx + 1}: Period Type is required`),
          );
        if (
          d.policyCount == null ||
          d.policyCount === "" ||
          Number(d.policyCount) <= 0
        )
          errors.push(
            translate(
              `Absent Detail ${idx + 1}: Policy Count must be greater than zero`,
            ),
          );
        if (
          d.absentCount == null ||
          d.absentCount === "" ||
          Number(d.absentCount) <= 0
        )
          errors.push(
            translate(
              `Absent Detail ${idx + 1}: Absent Count must be greater than zero`,
            ),
          );
      });
    }

    return errors;
  };

  const handleSave = async (formData: any) => {
    const errors = validateForm(formData);
    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = {
        id: formData.id ?? null,
        code: formData.code || null,
        stxt: formData.policyName?.trim(),
        policyTypeId: Number(formData.policyTypeId),
        applyOffDays: !!formData.applyOffDays,
        noOfExcuse:
          formData.noOfExcuse === "" || formData.noOfExcuse == null
            ? null
            : Number(formData.noOfExcuse),
        isActive: formData.isActive ?? true,
        policyDetails: (formData.policyDetails || []).map(
          (d: any, index: number) => ({
            id: d.id && d.id > 0 ? d.id : null,
            lineIndex: d.lineIndex ?? index + 1,
            startTime:
              d.startTime === "" || d.startTime == null
                ? null
                : Number(d.startTime),
            endTime:
              d.endTime === "" || d.endTime == null ? null : Number(d.endTime),
            periodTypeId: d.periodTypeId ?? null,
            valTypeId: d.valTypeId ?? null,
            value: d.value === "" || d.value == null ? null : Number(d.value),
          }),
        ),
        absentDetails: (formData.absentDetails || []).map(
          (d: any, index: number) => ({
            id: d.id && d.id > 0 ? d.id : null,
            lineIndex: d.lineIndex ?? index + 1,
            periodTypeId: d.periodTypeId ?? null,
            policyCount:
              d.policyCount === "" || d.policyCount == null
                ? null
                : Number(d.policyCount),
            absentCount:
              d.absentCount === "" || d.absentCount == null
                ? null
                : Number(d.absentCount),
          }),
        ),
      };

      const response = formData.id
        ? await saveRecord("put", `/api/HumanResource/AttendancePolicy/v3/${formData.id}`, payload)
        : await saveRecord("post", "/api/HumanResource/AttendancePolicy/v3", payload);

      setToasterMessages([
        formData.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      // The saved row carries server-derived policy type names, so refetch
      // rather than reconstruct the cached shape from the form payload.
      dispatch(attendancePolicySlice.fetchAll(true));
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save attendance policy");
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      const response = await saveRecord("delete", `/api/HumanResource/AttendancePolicy/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(attendancePolicySlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete attendance policy");
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  return (
    <>
      {viewMode === "report" ? (
        <Box sx={{ p: 2 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3, px: 2 }}>
            <Typography variant="h5" fontWeight={800} sx={{ color: 'var(--lms-text-main)' }}>
              <T>Attendance Policy Report</T>
            </Typography>
            <Button
              onClick={() => setViewMode("list")}
              startIcon={<ArrowBackIcon sx={{ fontSize: '1rem !important' }} />}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                color: "var(--lms-primary)",
                fontSize: "0.9rem",
                "&:hover": { bgcolor: "transparent", opacity: 0.8 },
              }}
            >
              <T>Back to List</T>
            </Button>
          </Box>
          <Box sx={{ px: 2 }}>
            <AttendancePolicyReportModal
              isFullScreenView
              open={true}
              onClose={() => setViewMode("list")}
              lookups={{
                entities: entities,
                policyTypes: normalizedPolicyTypes,
                policies: rows,
              }}
            />
          </Box>
        </Box>
      ) : viewMode !== "list" ? (
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
                ? translate("New Attendance Policy")
                : viewMode === "edit"
                  ? translate("Edit Attendance Policy")
                  : translate("View Attendance Policy")}
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
            <AttendancePolicyForm
              formData={formData}
              setFormData={setFormData}
              isReadOnly={viewMode === "view"}
              isSubmitted={isSubmitted}
              policyTypes={policyTypes}
              periodTypes={periodTypes}
              penaltyTypes={penaltyTypes}
            />
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Attendance Policies")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          maxWidth="lg"
          onAddClick={() => {
            setFormData({ ...initialAttendancePolicyFormData });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => {
            setFormData({
              ...row,
              policyDetails: row.policyDetails || [],
              absentDetails: row.absentDetails || [],
            });
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({
              ...row,
              policyDetails: row.policyDetails || [],
              absentDetails: row.absentDetails || [],
            });
            setIsSubmitted(false);
            setViewMode("view");
          }}
          showAddButton={true}
          showToaster={false}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              onClick={() => setViewMode("report")}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9", // A vibrant sky blue for reports
                borderRadius: "10px",
                px: 3,
                py: 0.8,
                boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
                "&:hover": {
                  bgcolor: "#0284c7",
                  boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
                },
              }}
            >
              <T>Attendance Policy Report</T>
            </Button>
          }
        />
      )}
      <AttendancePolicyReportModal
        open={showReportModal}
        onClose={() => setShowReportModal(false)}
        lookups={{
          entities: entities,
          policyTypes: normalizedPolicyTypes,
          policies: rows,
        }}
      />
      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {loading && <LmsOverlayLoader />}
    </>
  );
};

const AttendancePolicyForm: React.FC<{
  formData: any;
  setFormData: any;
  isReadOnly?: boolean;
  isSubmitted?: boolean;
  policyTypes: any[];
  periodTypes: any[];
  penaltyTypes: any[];
}> = ({
  formData,
  setFormData,
  isReadOnly,
  isSubmitted,
  policyTypes,
  periodTypes,
  penaltyTypes,
}) => {
    const { translate } = useTranslation();
    const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
    const [detailsLoading, setDetailsLoading] = useState(false);
    const [policyTypeModalOpen, setPolicyTypeModalOpen] = useState(false);
    const [detailModal, setDetailModal] = useState<{
      kind: "policyPeriod" | "policyVal" | "absentPeriod";
      rowId: number;
    } | null>(null);
    const [detailTab, setDetailTab] = useState(0);
    const [policySelected, setPolicySelected] = useState<number[]>([]);
    const [absentSelected, setAbsentSelected] = useState<number[]>([]);
    const [editingMinutes, setEditingMinutes] = useState<Record<string, string>>(
      {},
    );

    const safePolicyTypes = (policyTypes || []).filter((p) => p != null);
    const normalizedPolicyTypes = safePolicyTypes.map(normalizeLov);
    const selectedPolicyTypeId = Number(formData.policyTypeId);
    const selectedPolicy = normalizedPolicyTypes.find((p) => Number(p.id) === selectedPolicyTypeId);
    const policyKind = getAttendancePolicyKind(selectedPolicy);

    const isLateArrival = policyKind === "late";
    const isEarlyDeparture = policyKind === "early";
    const isOvertime = policyKind === "overtime";
    const isAbsent = policyKind === "absent";

    const showNoOfExcuse = isLateArrival || isEarlyDeparture;
    const showPolicyDetails = isLateArrival || isEarlyDeparture || isOvertime;
    const showAbsentDetails = isLateArrival || isEarlyDeparture || isAbsent;

    const normalizedPeriodTypes = (periodTypes || []).filter((p) => p != null).map(normalizeLov);
    const normalizedPenaltyTypes = (penaltyTypes || []).filter((p) => p != null).map(normalizeLov);

    const resolvePeriodLabel = (id?: number | null) =>
      normalizedPeriodTypes.find((p) => Number(p.id) === Number(id))?.stxt || "";
    const resolvePenaltyLabel = (id?: number | null) =>
      normalizedPenaltyTypes.find((p) => Number(p.id) === Number(id))?.stxt || "";
    const formatMinutes = (value: any) => {
      if (value === null || value === undefined || value === "") return "";
      const digits = String(value).replace(/[^0-9]/g, "");
      if (digits === "") return "";
      const total = Number(digits);
      if (!Number.isFinite(total)) return String(value);
      const hrs = Math.floor(total / 60);
      const mins = total % 60;
      return `${hrs}hrs-${mins}mins`;
    };
    const normalizeMinutesInput = (raw: string) => {
      const digits = String(raw ?? "").replace(/[^0-9]/g, "");
      if (digits === "") return "";
      return String(Number(digits));
    };
    const getMinutesKey = (id: number, key: "startTime" | "endTime") =>
      `${id}:${key}`;

    useEffect(() => {
      if (!showPolicyDetails && showAbsentDetails) setDetailTab(1);
      if (!showAbsentDetails && showPolicyDetails) setDetailTab(0);
      if (!showAbsentDetails && !showPolicyDetails) setDetailTab(0);
    }, [showPolicyDetails, showAbsentDetails]);

    useEffect(() => {
      setPolicySelected((prev) =>
        prev.filter((id) =>
          (formData.policyDetails || []).some((d: any) => d.id === id),
        ),
      );
    }, [formData.policyDetails]);

    useEffect(() => {
      setAbsentSelected((prev) =>
        prev.filter((id) =>
          (formData.absentDetails || []).some((d: any) => d.id === id),
        ),
      );
    }, [formData.absentDetails]);

    const policyDetails = formData.policyDetails || [];
    const absentDetails = formData.absentDetails || [];
    useEffect(() => {
      const fetchFullRecord = async () => {
        if (!formData.id) return;
        const hasDetails =
          (formData.policyDetails?.length ?? 0) > 0 ||
          (formData.absentDetails?.length ?? 0) > 0;
        if (hasDetails) return;
        setDetailsLoading(true);
        try {
          const response: any = await GetService(
            `/api/HumanResource/AttendancePolicy/v3/${formData.id}`,
          );
          const data = response?.data || response;
          const policyDetails = data.policyDetails || data.PolicyDetails || [];
          const absentDetails = data.absentDetails || data.AbsentDetails || [];

          setFormData({
            ...formData,
            code: data.code ?? data.Code ?? formData.code,
            policyName: data.stxt ?? data.Stxt ?? formData.policyName,
            policyTypeId:
              data.policyTypeId ?? data.PolicyTypeId ?? formData.policyTypeId,
            policyTypeName:
              data.policyTypeName ??
              data.PolicyTypeName ??
              formData.policyTypeName,
            applyOffDays:
              data.applyOffDays ?? data.ApplyOffDays ?? formData.applyOffDays,
            noOfExcuse:
              data.noOfExcuse ?? data.NoOfExcuse ?? formData.noOfExcuse,
            isActive: data.isActive ?? data.IsActive ?? formData.isActive,
            policyDetails: policyDetails.map((d: any) => ({
              id: d.id ?? d.Id ?? null,
              lineIndex: d.lineIndex ?? d.LineIndex ?? 0,
              startTime: d.startTime ?? d.StartTime ?? "",
              endTime: d.endTime ?? d.EndTime ?? "",
              periodTypeId: d.periodTypeId ?? d.PeriodTypeId ?? null,
              periodTypeName: d.periodTypeName ?? d.PeriodTypeName ?? "",
              valTypeId: d.valTypeId ?? d.ValTypeId ?? null,
              valTypeName: d.valTypeName ?? d.ValTypeName ?? "",
              value: d.value ?? d.Value ?? "",
            })),
            absentDetails: absentDetails.map((d: any) => ({
              id: d.id ?? d.Id ?? null,
              lineIndex: d.lineIndex ?? d.LineIndex ?? 0,
              periodTypeId: d.periodTypeId ?? d.PeriodTypeId ?? null,
              periodTypeName: d.periodTypeName ?? d.PeriodTypeName ?? "",
              policyCount: d.policyCount ?? d.PolicyCount ?? "",
              absentCount: d.absentCount ?? d.AbsentCount ?? "",
            })),
          });
        } catch (error) {
          console.error("Failed to fetch attendance policy details", error);
        } finally {
          setDetailsLoading(false);
        }
      };
      fetchFullRecord();
    }, [formData.id]);

    const handleAddPolicyDetail = () => {
      const newDetail = {
        id: -Math.floor(Math.random() * 1000000),
        lineIndex: (formData.policyDetails || []).length + 1,
        startTime: "",
        endTime: "",
        periodTypeId: null,
        periodTypeName: "",
        valTypeId: null,
        valTypeName: "",
        value: "",
      };
      setFormData({
        ...formData,
        policyDetails: [...(formData.policyDetails || []), newDetail],
      });
    };

    const handlePolicyDetailChange = (id: number, key: string, value: any) => {
      setFormData({
        ...formData,
        policyDetails: (formData.policyDetails || []).map((d: any) =>
          d.id === id ? { ...d, [key]: value } : d,
        ),
      });
    };
    const updatePolicyDetail = (id: number, updates: Record<string, any>) => {
      setFormData((prev: any) => ({
        ...prev,
        policyDetails: (prev.policyDetails || []).map((d: any) =>
          d.id === id ? { ...d, ...updates } : d,
        ),
      }));
    };
    const handleRemoveSelectedPolicyDetails = () => {
      if (policySelected.length === 0) return;
      setFormData({
        ...formData,
        policyDetails: (formData.policyDetails || []).filter(
          (d: any) => !policySelected.includes(d.id),
        ),
      });
      setPolicySelected([]);
    };

    const handleAddAbsentDetail = () => {
      const newDetail = {
        id: -Math.floor(Math.random() * 1000000),
        lineIndex: (formData.absentDetails || []).length + 1,
        periodTypeId: null,
        periodTypeName: "",
        policyCount: "",
        absentCount: "",
      };
      setFormData({
        ...formData,
        absentDetails: [...(formData.absentDetails || []), newDetail],
      });
    };

    const handleAbsentDetailChange = (id: number, key: string, value: any) => {
      setFormData({
        ...formData,
        absentDetails: (formData.absentDetails || []).map((d: any) =>
          d.id === id ? { ...d, [key]: value } : d,
        ),
      });
    };
    const updateAbsentDetail = (id: number, updates: Record<string, any>) => {
      setFormData((prev: any) => ({
        ...prev,
        absentDetails: (prev.absentDetails || []).map((d: any) =>
          d.id === id ? { ...d, ...updates } : d,
        ),
      }));
    };
    const handleRemoveSelectedAbsentDetails = () => {
      if (absentSelected.length === 0) return;
      setFormData({
        ...formData,
        absentDetails: (formData.absentDetails || []).filter(
          (d: any) => !absentSelected.includes(d.id),
        ),
      });
      setAbsentSelected([]);
    };

    const policyDetailColumns: Column[] = [
      {
        id: "sNo",
        label: translate("S.No"),
        minWidth: 60,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "startTime",
        label: translate("Start Time (Minutes)"),
        minWidth: 180,
        format: (_: any, detail: any) => (
          <FormInput
            type="text"
            size="small"
            inputMode="numeric"
            value={
              editingMinutes[getMinutesKey(detail.id, "startTime")] ??
              formatMinutes(detail.startTime)
            }
            onFocus={() => {
              const key = getMinutesKey(detail.id, "startTime");
              setEditingMinutes((prev) => ({
                ...prev,
                [key]: normalizeMinutesInput(String(detail.startTime ?? "")),
              }));
            }}
            onChange={(e) =>
              handlePolicyDetailChange(
                detail.id,
                "startTime",
                (function () {
                  const next = normalizeMinutesInput(e.target.value);
                  const key = getMinutesKey(detail.id, "startTime");
                  setEditingMinutes((prev) => ({
                    ...prev,
                    [key]: next,
                  }));
                  return next;
                })(),
              )
            }
            onBlur={() => {
              const key = getMinutesKey(detail.id, "startTime");
              setEditingMinutes((prev) => {
                const next = { ...prev };
                delete next[key];
                return next;
              });
            }}
            disabled={isReadOnly}
          />
        ),
      },
      {
        id: "endTime",
        label: translate("End Time (Minutes)"),
        minWidth: 180,
        format: (_: any, detail: any) => (
          <FormInput
            type="text"
            size="small"
            inputMode="numeric"
            value={
              editingMinutes[getMinutesKey(detail.id, "endTime")] ??
              formatMinutes(detail.endTime)
            }
            onFocus={() => {
              const key = getMinutesKey(detail.id, "endTime");
              setEditingMinutes((prev) => ({
                ...prev,
                [key]: normalizeMinutesInput(String(detail.endTime ?? "")),
              }));
            }}
            onChange={(e) =>
              handlePolicyDetailChange(
                detail.id,
                "endTime",
                (function () {
                  const next = normalizeMinutesInput(e.target.value);
                  const key = getMinutesKey(detail.id, "endTime");
                  setEditingMinutes((prev) => ({
                    ...prev,
                    [key]: next,
                  }));
                  return next;
                })(),
              )
            }
            onBlur={() => {
              const key = getMinutesKey(detail.id, "endTime");
              setEditingMinutes((prev) => {
                const next = { ...prev };
                delete next[key];
                return next;
              });
            }}
            disabled={isReadOnly}
          />
        ),
      },
      {
        id: "periodTypeId",
        label: translate("Period Type"),
        minWidth: 180,
        format: (_: any, detail: any) => (
          <FormInput
            select
            size="small"
            value={detail.periodTypeId || ""}
            onChange={(e) => {
              const selected = normalizedPeriodTypes.find(
                (p) => Number(p.id) === Number(e.target.value),
              );
              updatePolicyDetail(detail.id, {
                periodTypeId: Number(e.target.value),
                periodTypeName: selected?.stxt || getLovLabel(selected),
              });
            }}
            disabled={isReadOnly}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    size="small"
                    sx={{ color: "var(--lms-primary)" }}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (!isReadOnly) {
                        setDetailModal({
                          kind: "policyPeriod",
                          rowId: detail.id,
                        });
                      }
                    }}
                    disabled={isReadOnly}
                  >
                    <SearchIcon fontSize="small" />
                  </IconButton>
                </InputAdornment>
              ),
            }}
            SelectProps={{
              renderValue: (val) => resolvePeriodLabel(val as number) || "",
              IconComponent: () => null,
            }}
          >
            {normalizedPeriodTypes.map((p) => (
              <MenuItem key={p.id} value={p.id}>
                {getLovLabel(p)}
              </MenuItem>
            ))}
          </FormInput>
        ),
      },
      {
        id: "valTypeId",
        label: translate("Type"),
        minWidth: 180,
        format: (_: any, detail: any) => (
          <FormInput
            select
            size="small"
            value={detail.valTypeId || ""}
            onChange={(e) => {
              const selected = normalizedPenaltyTypes.find(
                (p) => Number(p.id) === Number(e.target.value),
              );
              updatePolicyDetail(detail.id, {
                valTypeId: Number(e.target.value),
                valTypeName: selected?.stxt || getLovLabel(selected),
              });
            }}
            disabled={isReadOnly}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    size="small"
                    sx={{ color: "var(--lms-primary)" }}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (!isReadOnly) {
                        setDetailModal({
                          kind: "policyVal",
                          rowId: detail.id,
                        });
                      }
                    }}
                    disabled={isReadOnly}
                  >
                    <SearchIcon fontSize="small" />
                  </IconButton>
                </InputAdornment>
              ),
            }}
            SelectProps={{
              renderValue: (val) => resolvePenaltyLabel(val as number) || "",
              IconComponent: () => null,
            }}
          >
            {normalizedPenaltyTypes.map((p) => (
              <MenuItem key={p.id} value={p.id}>
                {getLovLabel(p)}
              </MenuItem>
            ))}
          </FormInput>
        ),
      },
      {
        id: "value",
        label: translate("Value"),
        minWidth: 120,
        format: (_: any, detail: any) => (
          <FormInput
            type="number"
            size="small"
            value={detail.value ?? ""}
            onChange={(e) =>
              handlePolicyDetailChange(detail.id, "value", e.target.value)
            }
            disabled={isReadOnly}
          />
        ),
      },
    ];

    const absentDetailColumns: Column[] = [
      {
        id: "sNo",
        label: translate("S.No"),
        minWidth: 60,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "periodTypeId",
        label: translate("Period Type"),
        minWidth: 200,
        format: (_: any, detail: any) => (
          <FormInput
            select
            size="small"
            value={detail.periodTypeId || ""}
            onChange={(e) => {
              const selected = normalizedPeriodTypes.find(
                (p) => Number(p.id) === Number(e.target.value),
              );
              updateAbsentDetail(detail.id, {
                periodTypeId: Number(e.target.value),
                periodTypeName: selected?.stxt || getLovLabel(selected),
              });
            }}
            disabled={isReadOnly}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    size="small"
                    sx={{ color: "var(--lms-primary)" }}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (!isReadOnly) {
                        setDetailModal({
                          kind: "absentPeriod",
                          rowId: detail.id,
                        });
                      }
                    }}
                    disabled={isReadOnly}
                  >
                    <SearchIcon fontSize="small" />
                  </IconButton>
                </InputAdornment>
              ),
            }}
            SelectProps={{
              renderValue: (val) => resolvePeriodLabel(val as number) || "",
              IconComponent: () => null,
            }}
          >
            {normalizedPeriodTypes.map((p) => (
              <MenuItem key={p.id} value={p.id}>
                {getLovLabel(p)}
              </MenuItem>
            ))}
          </FormInput>
        ),
      },
      {
        id: "policyCount",
        label: translate("Policy Count"),
        minWidth: 150,
        format: (_: any, detail: any) => (
          <FormInput
            type="number"
            size="small"
            value={detail.policyCount ?? ""}
            onChange={(e) =>
              handleAbsentDetailChange(detail.id, "policyCount", e.target.value)
            }
            disabled={isReadOnly}
          />
        ),
      },
      {
        id: "absentCount",
        label: translate("Absent Count"),
        minWidth: 150,
        format: (_: any, detail: any) => (
          <FormInput
            type="number"
            size="small"
            value={detail.absentCount ?? ""}
            onChange={(e) =>
              handleAbsentDetailChange(detail.id, "absentCount", e.target.value)
            }
            disabled={isReadOnly}
          />
        ),
      },
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
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {detailsLoading && <LmsOverlayLoader />}
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
                  disabled={isReadOnly}
                  error={isSubmitted && !formData.code}
                  required
                  placeholder={translate("Auto Generated")}
                />
              </Grid>
              <Grid item xs={12} md={8}>
                <FormInput
                  label={translate("Policy Name")}
                  placeholder={translate("Enter Policy Name")}
                  value={formData.policyName || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, policyName: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !formData.policyName}
                  required
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={translate("Policy Type*")}
                  value={
                    formData.policyTypeName ||
                    normalizedPolicyTypes.find((p) => Number(p.id) === Number(formData.policyTypeId))
                      ?.stxt ||
                    ""
                  }
                  placeholder={translate("Select Policy Type")}
                  options={normalizedPolicyTypes}
                  getOptionLabel={(opt) => getLovLabel(opt)}
                  onSelect={(row) => {
                    setFormData({
                      ...formData,
                      policyTypeId: Number(row.id),
                      policyTypeName: row.stxt || getLovLabel(row),
                    });
                  }}
                  onSearchClick={() => !isReadOnly && setPolicyTypeModalOpen(true)}
                  disabled={isReadOnly}
                  error={isSubmitted && !formData.policyTypeId}
                  helperText={
                    isSubmitted && !formData.policyTypeId
                      ? translate("Policy Type is required")
                      : ""
                  }
                  hideDropdownArrow
                />
              </Grid>

              {showNoOfExcuse && (
                <Grid item xs={12} md={4}>
                  <FormInput
                    label={translate("No. of Excuse")}
                    placeholder={translate("Enter No. of Excuse")}
                    type="number"
                    value={formData.noOfExcuse ?? ""}
                    onChange={(e) =>
                      setFormData({ ...formData, noOfExcuse: e.target.value })
                    }
                    disabled={isReadOnly}
                  />
                </Grid>
              )}

              <Grid item xs={12} md={4}>
                <LmsCheckbox
                  checked={!!formData.applyOffDays}
                  onChange={(checked) =>
                    setFormData({ ...formData, applyOffDays: checked })
                  }
                  label={translate("Apply on off days")}
                />
              </Grid>

            </Grid>
          </Collapse>

          {(showPolicyDetails || showAbsentDetails) && (
            <Box sx={{ mt: 3 }}>
              <StyledTabs
                value={detailTab}
                onChange={(_, v) => setDetailTab(v)}
                variant="scrollable"
                scrollButtons="auto"
                allowScrollButtonsMobile
                tabs={[
                  {
                    label: <T>Policy</T>,
                    icon: <PolicyIcon />,
                    disabled: !showPolicyDetails,
                  },
                  {
                    label: <T>Absent Policy</T>,
                    icon: <EventBusyIcon />,
                    disabled: !showAbsentDetails,
                  },
                ]}
              />

              <Paper
                elevation={0}
                sx={{
                  mt: -3,
                  border: "1px solid var(--lms-border)",
                  borderTop: "none",
                  borderRadius: "0 0 12px 12px",
                  overflow: "hidden",
                  bgcolor: "#fff",
                }}
              >
                <TabPanel value={detailTab} index={0} padding={2}>
                  {showPolicyDetails && (
                    <Box sx={{ borderTop: "1px solid #E2E8F0", pt: 1 }}>
                      <Box sx={{ display: "flex", gap: 1, mb: 1 }}>
                        {!isReadOnly && (
                          <Button
                            variant="outlined"
                            startIcon={<DeleteIcon />}
                            size="small"
                            onClick={handleRemoveSelectedPolicyDetails}
                            disabled={policySelected.length === 0}
                            sx={{
                              textTransform: "none",
                              fontWeight: 700,
                              fontSize: "0.75rem",
                              borderColor: "#E2E8F0",
                              color: "#333",
                              borderRadius: "8px",
                              "&:hover": {
                                borderColor: "#ef4444",
                                bgcolor: "rgba(239, 68, 68, 0.05)",
                              },
                            }}
                          >
                            {translate("Delete")}
                          </Button>
                        )}
                      </Box>

                      <SchemaTable
                        title={translate("Policy Details")}
                        columns={policyDetailColumns}
                        rows={policyDetails}
                        actions={false}
                        showAddButton={true}
                        onAddClick={handleAddPolicyDetail}
                        showEditAction={false}
                        hideTitle={true}
                        hideBreadcrumbs={true}
                        checkboxSelection={!isReadOnly}
                        selectedIds={policySelected}
                        onSelectionChange={(ids) => setPolicySelected(ids as number[])}
                        disableRowSelectionOnClick={true}
                        pagination={false}
                        filtersEnabled={false}
                        downloadEnabled={false}
                        stickyHeader={true}
                        rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                      />
                    </Box>
                  )}
                </TabPanel>

                <TabPanel value={detailTab} index={1} padding={2}>
                  {showAbsentDetails && (
                    <Box sx={{ borderTop: "1px solid #E2E8F0", pt: 1 }}>
                      <Box sx={{ display: "flex", gap: 1, mb: 1 }}>
                        {!isReadOnly && (
                          <Button
                            variant="outlined"
                            startIcon={<DeleteIcon />}
                            size="small"
                            onClick={handleRemoveSelectedAbsentDetails}
                            disabled={absentSelected.length === 0}
                            sx={{
                              textTransform: "none",
                              fontWeight: 700,
                              fontSize: "0.75rem",
                              borderColor: "#E2E8F0",
                              color: "#333",
                              borderRadius: "8px",
                              "&:hover": {
                                borderColor: "#ef4444",
                                bgcolor: "rgba(239, 68, 68, 0.05)",
                              },
                            }}
                          >
                            {translate("Delete")}
                          </Button>
                        )}
                      </Box>

                      <SchemaTable
                        title={translate("Absent Policy Details")}
                        columns={absentDetailColumns}
                        rows={absentDetails}
                        actions={false}
                        showAddButton={true}
                        onAddClick={handleAddAbsentDetail}
                        showEditAction={false}
                        hideTitle={true}
                        hideBreadcrumbs={true}
                        checkboxSelection={!isReadOnly}
                        selectedIds={absentSelected}
                        onSelectionChange={(ids) => setAbsentSelected(ids as number[])}
                        disableRowSelectionOnClick={true}
                        pagination={false}
                        filtersEnabled={false}
                        downloadEnabled={false}
                        stickyHeader={true}
                        rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                      />
                    </Box>
                  )}
                </TabPanel>
              </Paper>
            </Box>
          )}

          <GenericSearchModal
            open={policyTypeModalOpen}
            onClose={() => setPolicyTypeModalOpen(false)}
            title={translate("Policy Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 80 },
              { id: "stxt", label: translate("Name"), minWidth: 200 },
              { id: "ltxt", label: translate("Description"), minWidth: 240 },
            ]}
            data={normalizedPolicyTypes}
            onSelect={(row) => {
              setFormData({
                ...formData,
                policyTypeId: Number(row.id),
                policyTypeName: row.stxt || getLovLabel(row),
              });
              setPolicyTypeModalOpen(false);
            }}
          />

          <GenericSearchModal
            open={detailModal?.kind === "policyPeriod"}
            onClose={() => setDetailModal(null)}
            title={translate("Period Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 80 },
              { id: "stxt", label: translate("Short Name"), minWidth: 160 },
              { id: "ltxt", label: translate("Description"), minWidth: 220 },
            ]}
            data={normalizedPeriodTypes}
            onSelect={(row) => {
              if (!detailModal) return;
              updatePolicyDetail(detailModal.rowId, {
                periodTypeId: Number(row.id),
                periodTypeName: row.stxt || getLovLabel(row),
              });
              setDetailModal(null);
            }}
          />

          <GenericSearchModal
            open={detailModal?.kind === "policyVal"}
            onClose={() => setDetailModal(null)}
            title={translate("Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 80 },
              { id: "stxt", label: translate("Short Name"), minWidth: 160 },
              { id: "ltxt", label: translate("Description"), minWidth: 220 },
            ]}
            data={normalizedPenaltyTypes}
            onSelect={(row) => {
              if (!detailModal) return;
              updatePolicyDetail(detailModal.rowId, {
                valTypeId: Number(row.id),
                valTypeName: row.stxt || getLovLabel(row),
              });
              setDetailModal(null);
            }}
          />

          <GenericSearchModal
            open={detailModal?.kind === "absentPeriod"}
            onClose={() => setDetailModal(null)}
            title={translate("Period Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 80 },
              { id: "stxt", label: translate("Short Name"), minWidth: 160 },
              { id: "ltxt", label: translate("Description"), minWidth: 220 },
            ]}
            data={normalizedPeriodTypes}
            onSelect={(row) => {
              if (!detailModal) return;
              updateAbsentDetail(detailModal.rowId, {
                periodTypeId: Number(row.id),
                periodTypeName: row.stxt || getLovLabel(row),
              });
              setDetailModal(null);
            }}
          />
        </Box>
      </FormThemeWrapper>
    );
  };

export default AttendancePolicy;

