import { FormInput } from "@/components/FormInput";
import React, { useEffect, useState } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows, useHrLookups } from "@/hooks";
import { earningTypeSlice } from "@/store/humanResourceSlices";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import AssessmentIcon from "@mui/icons-material/Assessment";
import { EarningTypeReportModal } from "./EarningTypeReportModal";

interface Props {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const EarningType: React.FC<Props> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the lookups payload both come from the cache.
  const {
    rows: cachedRows,
    lookups: cachedLookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrEarningType", earningTypeSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">("list");
  const [isSubmitted, setIsSubmitted] = useState(false);
  // The lookups endpoint returns one payload holding every list.
  const lookups = React.useMemo<any>(() => {
    const root = (cachedLookups?.all ?? [])[0] ?? {};
    const data = root?.data ?? root?.Data ?? root;
    const pickArray = (...keys: string[]) => {
      for (const key of keys) {
        const val = data?.[key];
        if (Array.isArray(val)) return val;
      }
      return [];
    };

    return {
      uoms: pickArray("uoms", "Uoms", "uom", "Uom"),
      paymentRunTypes: pickArray("paymentRunTypes", "PaymentRunTypes", "paymentTypes", "PaymentTypes"),
      taxMethods: pickArray("taxMethods", "TaxMethods"),
      rateBasis: pickArray("rateBasis", "RateBasis", "rateBases", "RateBases"),
      basisEarningTypes: pickArray("basisEarningTypes", "BasisEarningTypes"),
    };
  }, [cachedLookups]);
  // Institutions are shared with the other HR screens.
  const { lookup } = useHrLookups(["institutions"]);
  const entities = lookup("institutions");
  const [isReportModalOpen, setIsReportModalOpen] = useState(false);
  const [activeLookup, setActiveLookup] = useState<
    "uom" | "paymentRunType" | "taxMethod" | "rateBasis" | "basisEarningType" | null
  >(null);

  const initialFormData = {
    id: 0,
    code: "",
    earningType: "",
    description: "",
    baseSalary: false,
    payslipIndex: "",
    payslipDescription: "",
    uomId: null,
    uom: "",
    paymentRunTypeId: null,
    paymentRunType: "",
    taxMethodId: null,
    taxMethod: "",
    rateBasisId: null,
    rateBasis: "",
    basisEarningTypeId: null,
    basisEarningType: "",
    isActive: true,
  };
  const [formData, setFormData] = useState<any>(initialFormData);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Earning Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Earning Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Earning Type"));
    else if (viewMode === "report") onSubViewChange(translate("Earning Type Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) setViewMode("list");
  }, [resetKey]);

  const showError = (messages: string[]) => {
    setToasterMessages(messages);
    setToasterSeverity("error");
    setShowToaster(true);
  };

  const mapLookup = (items: any[]) =>
    (Array.isArray(items) ? items : [])
      .map((x: any) => ({
        id: Number(x?.id ?? x?.Id ?? x?.value ?? 0),
        code: x?.code ?? x?.Code ?? "",
        shortName: x?.name ?? x?.Name ?? x?.shortName ?? x?.ShortName ?? x?.stxt ?? x?.Stxt ?? x?.label ?? x?.earningType ?? x?.EarningType ?? "",
      }))
      .filter((x: any) => !!x.shortName);

  // sNo is a display index, derived here rather than cached.
  const rows = React.useMemo<any[]>(
    () => (cachedRows || []).map((x: any, index: number) => ({ ...x, sNo: index + 1 })),
    [cachedRows],
  );

  const handleSave = async (data: any) => {
    const errors: string[] = [];
    if (!data.code) errors.push(translate("Code is required"));
    if (!data.earningType) errors.push(translate("Earning Type is required"));
    if (!data.uomId) errors.push(translate("Unit of Measure is required"));
    if (!data.paymentRunTypeId) errors.push(translate("Payment Run Type is required"));
    if (!data.taxMethodId) errors.push(translate("Tax Method is required"));
    if (!data.rateBasisId) errors.push(translate("Rate Basis is required"));
    if (errors.length > 0) {
      showError(errors);
      return false;
    }

    const payload = {
      id: data.id || 0,
      code: data.code,
      earningType: data.earningType,
      description: data.description || "",
      baseSalary: !!data.baseSalary,
      payslipIndex: data.payslipIndex || "",
      payslipDescription: data.payslipDescription || "",
      uomId: data.uomId,
      paymentRunTypeId: data.paymentRunTypeId,
      taxMethodId: data.taxMethodId,
      rateBasisId: data.rateBasisId,
      basisEarningTypeId: data.basisEarningTypeId || null,
      isActive: data.isActive !== false,
    };

    try {
      setApiLoading(true);
      const response: any = data.id
        ? await workflowMutation("put", `/api/HumanResource/EarningType/v3/${data.id}`, payload)
        : await workflowMutation("post", "/api/HumanResource/EarningType/v3", payload);
      // Sent for approval: the record itself does not change yet, so the list is left as it is.
      if (pendingWorkflow(response)) return true;
      const saved: any = response?.data;
      // Patch the cached row instead of reloading the list; the lookups payload
      // is reference data unaffected by this save.
      const savedId = Number(saved?.id ?? data.id ?? 0);
      if (savedId > 0) {
        dispatch(earningTypeSlice.actions.upsertRow({ ...data, ...payload, id: savedId }));
      } else {
        dispatch(earningTypeSlice.fetchAll(true));
      }
      setToasterMessages([
        data.id ? translate("Record updated successfully") : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      return true;
    } catch (error: any) {
      showError([apiErrorMessage(error, translate("Failed to save earning type"))]);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    try {
      setApiLoading(true);
      const response = await workflowMutation("delete", `/api/HumanResource/EarningType/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      dispatch(earningTypeSlice.actions.removeRow(row.id));
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } catch (error: any) {
      showError([apiErrorMessage(error, translate("Failed to delete earning type"))]);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Code"), minWidth: 80 },
    { id: "earningType", label: translate("Earning Type"), minWidth: 200 },
    { id: "payslipIndex", label: translate("Payslip Index"), minWidth: 120 },
    { id: "payslipDescription", label: translate("Payslip Description"), minWidth: 200 },
    { id: "uom", label: translate("UOM"), minWidth: 80 },
    { id: "paymentRunType", label: translate("Payment Run Type"), minWidth: 150 },
    { id: "taxMethod", label: translate("Tax Method"), minWidth: 100 },
    { id: "rateBasis", label: translate("Rate Basis"), minWidth: 100 },
  ];

  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isSetupExpanded, setIsSetupExpanded] = useState(true);

  const renderExpandIcon = (expanded: boolean) => (
    <Box sx={{ width: 22, height: 22, borderRadius: "50%", bgcolor: "rgba(var(--lms-primary-rgb), 0.1)", display: "flex", alignItems: "center", justifyContent: "center", color: "var(--lms-primary)", position: "relative", transition: "transform 0.3s ease" }}>
      <Box sx={{ width: 10, height: "2px", bgcolor: "currentColor" }} />
      <Box sx={{ width: "2px", height: 10, bgcolor: "currentColor", position: "absolute", transition: "transform 0.3s ease", transform: expanded ? "rotate(90deg)" : "rotate(0deg)", opacity: expanded ? 0 : 1 }} />
    </Box>
  );

  const renderForm = (data: any, setData: any, _mode?: "new" | "edit" | "view", isReadOnly?: boolean, submitted?: boolean) => (
    <FormThemeWrapper>
      <Box sx={{ p: 0, pointerEvents: isReadOnly ? "none" : "auto", opacity: isReadOnly ? 0.9 : 1 }}>
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, cursor: "pointer", userSelect: "none" }} onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
            <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>Basic Information</T></Typography>
          </Box>
          {renderExpandIcon(isGeneralExpanded)}
        </Box>
        <Collapse in={isGeneralExpanded}>
          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid item xs={12} md={4}>
              <FormInput fullWidth size="small" label={translate("Code*")} value={data.code || ""} onChange={(e: any) => setData({ ...data, code: e.target.value })} disabled={isReadOnly} error={submitted && !data.code} />
            </Grid>
            <Grid item xs={12} md={6}>
              <FormInput fullWidth size="small" label={translate("Earning Type*")} value={data.earningType || ""} onChange={(e: any) => setData({ ...data, earningType: e.target.value })} disabled={isReadOnly} error={submitted && !data.earningType} />
            </Grid>
            <Grid item xs={12} md={6}>
              <FormInput fullWidth size="small" label={translate("Description")} multiline rows={2} value={data.description || ""} onChange={(e: any) => setData({ ...data, description: e.target.value })} disabled={isReadOnly} />
            </Grid>
            <Grid item xs={12}>
              <LmsCheckbox checked={data.baseSalary || false} onChange={(checked) => setData({ ...data, baseSalary: checked })} disabled={isReadOnly} label={<Typography sx={{ fontSize: "0.8rem", fontWeight: 600, color: "#475569" }}><T>Base Salary</T></Typography>} />
            </Grid>
          </Grid>
        </Collapse>

        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, cursor: "pointer", userSelect: "none" }} onClick={() => setIsSetupExpanded(!isSetupExpanded)}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
            <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>Setup</T></Typography>
          </Box>
          {renderExpandIcon(isSetupExpanded)}
        </Box>
        <Collapse in={isSetupExpanded}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={4}>
              <FormInput fullWidth size="small" label={translate("Payslip Index")} value={data.payslipIndex || ""} onChange={(e: any) => setData({ ...data, payslipIndex: e.target.value })} disabled={isReadOnly} />
            </Grid>
            <Grid item xs={12} md={6}>
              <FormInput fullWidth size="small" label={translate("Payslip Description")} multiline rows={2} value={data.payslipDescription || ""} onChange={(e: any) => setData({ ...data, payslipDescription: e.target.value })} disabled={isReadOnly} />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Unit of Measure")}
                value={data.uom || ""}
                placeholder={translate("Select Unit of Measure")}
                options={mapLookup(lookups.uoms)}
                getOptionLabel={(opt: any) => opt.shortName || ""}
                onSearchClick={() => !isReadOnly && setActiveLookup("uom")}
                onSelect={(row: any) =>
                  setData({ ...data, uomId: row?.id ?? null, uom: row?.shortName || "" })
                }
                disabled={isReadOnly}
                error={submitted && !data.uomId}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Payment Run Type")}
                value={data.paymentRunType || ""}
                placeholder={translate("Select Payment Run Type")}
                options={mapLookup(lookups.paymentRunTypes)}
                getOptionLabel={(opt: any) => opt.shortName || ""}
                onSearchClick={() => !isReadOnly && setActiveLookup("paymentRunType")}
                onSelect={(row: any) =>
                  setData({
                    ...data,
                    paymentRunTypeId: row?.id ?? null,
                    paymentRunType: row?.shortName || "",
                  })
                }
                disabled={isReadOnly}
                error={submitted && !data.paymentRunTypeId}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Tax Method")}
                value={data.taxMethod || ""}
                placeholder={translate("Select Tax Method")}
                options={mapLookup(lookups.taxMethods)}
                getOptionLabel={(opt: any) => opt.shortName || ""}
                onSearchClick={() => !isReadOnly && setActiveLookup("taxMethod")}
                onSelect={(row: any) =>
                  setData({
                    ...data,
                    taxMethodId: row?.id ?? null,
                    taxMethod: row?.shortName || "",
                  })
                }
                disabled={isReadOnly}
                error={submitted && !data.taxMethodId}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Rate Basis")}
                value={data.rateBasis || ""}
                placeholder={translate("Select Rate Basis")}
                options={mapLookup(lookups.rateBasis)}
                getOptionLabel={(opt: any) => opt.shortName || ""}
                onSearchClick={() => !isReadOnly && setActiveLookup("rateBasis")}
                onSelect={(row: any) =>
                  setData({
                    ...data,
                    rateBasisId: row?.id ?? null,
                    rateBasis: row?.shortName || "",
                  })
                }
                disabled={isReadOnly}
                error={submitted && !data.rateBasisId}
              />
            </Grid>
            {Number(data?.rateBasisId) === 708 && <Grid item xs={12} md={6}>
              <LookupDropdown
                label={translate("Basis Earning Type")}
                value={data.basisEarningType || ""}
                placeholder={translate("Select Basis Earning Type")}
                options={mapLookup(lookups.basisEarningTypes).filter(
                  (x: any) => Number(x.id) !== Number(data.id || 0),
                )}
                getOptionLabel={(opt: any) => opt.shortName || ""}
                onSearchClick={() => !isReadOnly && setActiveLookup("basisEarningType")}
                onSelect={(row: any) =>
                  setData({
                    ...data,
                    basisEarningTypeId: row?.id ?? null,
                    basisEarningType: row?.shortName || "",
                  })
                }
                disabled={isReadOnly}
              />
            </Grid>}
          </Grid>
        </Collapse>
      </Box>
      <GenericSearchModal
        open={!!activeLookup}
        onClose={() => setActiveLookup(null)}
        title={translate("Select Lookup")}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "shortName", label: translate("Name"), minWidth: 220 },
        ]}
        data={
          activeLookup === "uom"
            ? mapLookup(lookups.uoms)
            : activeLookup === "paymentRunType"
              ? mapLookup(lookups.paymentRunTypes)
              : activeLookup === "taxMethod"
                ? mapLookup(lookups.taxMethods)
                : activeLookup === "rateBasis"
                  ? mapLookup(lookups.rateBasis)
                  : activeLookup === "basisEarningType"
                    ? mapLookup(lookups.basisEarningTypes).filter(
                        (x: any) => Number(x.id) !== Number(data.id || 0),
                      )
                    : []
        }
        selectedValue={
          activeLookup === "uom"
            ? data.uomId
            : activeLookup === "paymentRunType"
              ? data.paymentRunTypeId
              : activeLookup === "taxMethod"
                ? data.taxMethodId
                : activeLookup === "rateBasis"
                  ? data.rateBasisId
                  : activeLookup === "basisEarningType"
                    ? data.basisEarningTypeId
                    : ""
        }
        selectedField="id"
        onSelect={(row: any) => {
          if (activeLookup === "uom") {
            setData({ ...data, uomId: row?.id ?? null, uom: row?.shortName || "" });
          } else if (activeLookup === "paymentRunType") {
            setData({
              ...data,
              paymentRunTypeId: row?.id ?? null,
              paymentRunType: row?.shortName || "",
            });
          } else if (activeLookup === "taxMethod") {
            setData({ ...data, taxMethodId: row?.id ?? null, taxMethod: row?.shortName || "" });
          } else if (activeLookup === "rateBasis") {
            setData({ ...data, rateBasisId: row?.id ?? null, rateBasis: row?.shortName || "" });
          } else if (activeLookup === "basisEarningType") {
            setData({
              ...data,
              basisEarningTypeId: row?.id ?? null,
              basisEarningType: row?.shortName || "",
            });
          }
          setActiveLookup(null);
        }}
      />
    </FormThemeWrapper>
  );

  return (
    <>
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box sx={{ p: 2, display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 1.5 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Earning Type") : viewMode === "edit" ? translate("Edit Earning Type") : viewMode === "view" ? translate("View Earning Type") : translate("Earning Type Report")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={() => setViewMode("list")} sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}>
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && viewMode !== "report" && (
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
          <Box sx={{ px: 2, pb: 2 }}>
            {viewMode === "report" ? (
              <EarningTypeReportModal
                open={true}
                onClose={() => setViewMode("list")}
                lookups={{
                  earningTypes: rows.map((x: any) => ({ id: x.id, stxt: x.earningType || x.name || x.Name || x.stxt || x.ltxt || x.shortName || x.code || "" })),
                  uoms: lookups.uoms.map((x: any) => ({ id: x.id, stxt: x.name || x.Name || x.shortName || x.stxt || x.ltxt || "" })),
                  payRunTypes: lookups.paymentRunTypes.map((x: any) => ({ id: x.id, stxt: x.name || x.Name || x.shortName || x.stxt || x.ltxt || "" })),
                  entities: entities
                }}
                isFullScreenView={true}
              />
            ) : (
              renderForm(formData, setFormData, viewMode, viewMode === "view", isSubmitted)
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Earning Type")}
          addButtonLabel={translate("New Earning Type")}
          breadcrumbPath={''}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
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
              onClick={() => setViewMode("report")}
            >
              <T>Earning Type Report</T>
            </Button>
          }
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
          showAddButton={true}
          showToaster={false}
        />
      )}

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

export default EarningType;
