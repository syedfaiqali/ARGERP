import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import React, { useEffect, useMemo, useState } from "react";
import { Grid, Box, Typography, Button, Collapse } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon, Add as AddIcon, Delete as DeleteIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService } from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';
import { useAppDispatch, useCachedRows } from "@/hooks";
import { deductionGroupSlice } from "@/store/humanResourceSlices";

type LookupOption = { id: number; code: string; shortName: string };

const DeductionGroup: React.FC<{ onSubViewChange?: (subView: string | null) => void; resetKey?: number }> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the lookups payload both come from the cache.
  const {
    rows: cachedRows,
    lookups: cachedLookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrDeductionGroup", deductionGroupSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isSubTableExpanded, setIsSubTableExpanded] = useState(true);
  const [formData, setFormData] = useState<any>({ id: 0, code: "", deductionGroup: "" });
  const [deductionDetailRows, setDeductionDetailRows] = useState<any[]>([]);
  const [selectedDeductionDetailIds, setSelectedDeductionDetailIds] = useState<any[]>([]);

  const toErrorMessage = (error: any, fallback: string) => apiErrorMessage(error, fallback);

  const mapLookup = (items: any[]) =>
    (Array.isArray(items) ? items : [])
      .map((x: any) => ({
        id: Number(x?.id ?? x?.Id ?? 0),
        code: x?.code ?? x?.Code ?? "",
        shortName: x?.shortName ?? x?.ShortName ?? x?.stxt ?? x?.Stxt ?? x?.name ?? x?.Name ?? "",
      }))
      .filter((x: LookupOption) => x.id > 0 && x.shortName);

  // sNo and the display aliases are derived here so the cache keeps raw records.
  const rows = useMemo<any[]>(
    () =>
      (cachedRows || []).map((x: any, index: number) => ({
        ...x,
        id: x.id,
        sNo: index + 1,
        code: x.code,
        deductionGroup: x.deductionGroup,
      })),
    [cachedRows],
  );

  // The lookups endpoint returns one payload holding every list.
  const lookups = useMemo<{
    deductionTypes: LookupOption[];
    earningTypes: LookupOption[];
    rateBasis: LookupOption[];
    statuses: LookupOption[];
  }>(
    () => {
      const payload = (cachedLookups?.all ?? [])[0] ?? {};
      return {
        deductionTypes: mapLookup(payload?.deductionTypes),
        earningTypes: mapLookup(payload?.earningTypes),
        rateBasis: mapLookup(payload?.rateBasis),
        statuses: mapLookup(payload?.statuses),
      };
    },
    [cachedLookups],
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Deduction Group"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Deduction Group"));
    else if (viewMode === "view") onSubViewChange(translate("View Deduction Group"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) setViewMode("list");
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "deductionGroup", label: translate("Deduction Group"), minWidth: 300 },
  ];

  const updateDetailRow = (id: any, patch: Record<string, any>) =>
    setDeductionDetailRows((prev) => prev.map((row) => (row.id === id ? { ...row, ...patch } : row)));

  const valueRegex = /^\d*\.?\d{0,2}$/;
  const payslipRegex = /^\d*$/;

  const detailColumns: Column[] = useMemo(
    () => [
      { id: "lineIndex", label: translate("S.No"), minWidth: 70 },
      {
        id: "deductionTypeId",
        label: translate("Deduction Type"),
        minWidth: 180,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.deductionTypeId ?? ""}
            options={lookups.deductionTypes}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { deductionTypeId: opt?.id ?? null, deductionTypeName: opt?.shortName ?? "" })}
            placeholder={translate("Select Deduction Type")}
          />
        ),
      },
      {
        id: "rateBasisId",
        label: translate("Rate Basis"),
        minWidth: 150,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.rateBasisId ?? ""}
            options={lookups.rateBasis}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { rateBasisId: opt?.id ?? null, rateBasisName: opt?.shortName ?? "" })}
            placeholder={translate("Select Rate Basis")}
          />
        ),
      },
      {
        id: "basisEarningTypeId",
        label: translate("Basis Earning Type"),
        minWidth: 180,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.basisEarningTypeId ?? ""}
            options={lookups.earningTypes}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { basisEarningTypeId: opt?.id ?? null, basisEarningTypeName: opt?.shortName ?? "" })}
            placeholder={translate("Select Basis Earning Type")}
          />
        ),
      },
      {
        id: "value",
        label: translate("Value"),
        minWidth: 100,
        format: (_: any, row?: any) => (
          <FormInput
            hideLabel
            size="small"
            value={row?.value ?? ""}
            onChange={(e: any) => {
              const nextValue = e.target.value;
              if (valueRegex.test(nextValue)) updateDetailRow(row?.id, { value: nextValue });
            }}
          />
        ),
      },
      {
        id: "statusId",
        label: translate("Status"),
        minWidth: 120,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.statusId ?? ""}
            options={lookups.statuses}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { statusId: opt?.id ?? null, statusName: opt?.shortName ?? "" })}
            placeholder={translate("Select Status")}
          />
        ),
      },
      {
        id: "paySlipOrder",
        label: translate("Pay Slip Order"),
        minWidth: 130,
        format: (_: any, row?: any) => (
          <FormInput
            hideLabel
            size="small"
            value={row?.paySlipOrder ?? ""}
            onChange={(e: any) => {
              const nextValue = e.target.value;
              if (payslipRegex.test(nextValue)) updateDetailRow(row?.id, { paySlipOrder: nextValue });
            }}
          />
        ),
      },
      {
        id: "paySlipDescription",
        label: translate("Pay Slip Description"),
        minWidth: 190,
        format: (_: any, row?: any) => (
          <FormInput hideLabel size="small" value={row?.paySlipDescription ?? ""} onChange={(e: any) => updateDetailRow(row?.id, { paySlipDescription: e.target.value })} />
        ),
      },
    ],
    [lookups.deductionTypes, lookups.earningTypes, lookups.rateBasis, lookups.statuses, translate],
  );

  const handleAddDetailRow = () => {
    const active = lookups.statuses.find((x) => String(x.shortName).toLowerCase() === "active");
    setDeductionDetailRows((prev) => [
      ...prev,
      {
        id: Date.now() + Math.floor(Math.random() * 1000),
        lineIndex: prev.length + 1,
        deductionTypeId: null,
        rateBasisId: null,
        basisEarningTypeId: null,
        value: "",
        statusId: active?.id ?? null,
        paySlipOrder: "",
        paySlipDescription: "",
      },
    ]);
  };

  const handleDeleteSelectedDetailRows = () => {
    if (selectedDeductionDetailIds.length === 0) return;
    setDeductionDetailRows((prev) =>
      prev
        .filter((row) => !selectedDeductionDetailIds.includes(row.id))
        .map((row, index) => ({ ...row, lineIndex: index + 1 })),
    );
    setSelectedDeductionDetailIds([]);
  };

  const openForEditOrView = async (row: any, mode: "edit" | "view") => {
    setApiLoading(true);
    try {
      const response: any = await GetService(`/api/HumanResource/DeductionGroup/v3/${row.id}`);
      const loaded = response?.data ?? row;
      setFormData({ id: loaded.id ?? 0, code: loaded.code ?? "", deductionGroup: loaded.deductionGroup ?? "" });
      setDeductionDetailRows(
        Array.isArray(loaded?.details)
          ? loaded.details.map((d: any, index: number) => ({
            id: d.id,
            lineIndex: d.lineIndex ?? index + 1,
            deductionTypeId: d.deductionTypeId,
            deductionTypeName: d.deductionTypeName,
            rateBasisId: d.rateBasisId,
            rateBasisName: d.rateBasisName,
            basisEarningTypeId: d.basisEarningTypeId,
            basisEarningTypeName: d.basisEarningTypeName,
            value: d.value?.toString() ?? "",
            statusId: d.statusId,
            statusName: d.statusName,
            paySlipOrder: d.paySlipOrder?.toString() ?? "",
            paySlipDescription: d.paySlipDescription ?? "",
          }))
          : [],
      );
      setSelectedDeductionDetailIds([]);
      setIsSubmitted(false);
      setViewMode(mode);
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to load deduction group"))]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setApiLoading(false);
    }
  };

  const validateBeforeSave = () => {
    const errors: string[] = [];
    if (!formData.deductionGroup) errors.push(translate("Deduction Group is required"));
    if (deductionDetailRows.length === 0) errors.push(translate("At least one detail row is required"));
    const seenDeductionType = new Set<number>();
    const seenPaySlip = new Set<number>();
    deductionDetailRows.forEach((d, idx) => {
      const line = `${translate("Line")} ${idx + 1}: `;
      if (!d.deductionTypeId) errors.push(line + translate("Deduction Type is required"));
      if (!d.rateBasisId) errors.push(line + translate("Rate Basis is required"));
      if (!d.statusId) errors.push(line + translate("Status is required"));
      const value = Number(d.value);
      if (!Number.isFinite(value) || value <= 0) errors.push(line + translate("Value must be greater than 0"));
      const paySlip = Number(d.paySlipOrder);
      if (!Number.isInteger(paySlip) || paySlip <= 0) errors.push(line + translate("Pay Slip Order must be greater than 0"));

      if (d.deductionTypeId) {
        if (seenDeductionType.has(d.deductionTypeId)) errors.push(line + translate("Deduction Type already exists"));
        seenDeductionType.add(d.deductionTypeId);
      }

      if (Number.isInteger(paySlip) && paySlip > 0) {
        if (seenPaySlip.has(paySlip)) errors.push(line + translate("Pay Slip Order already exists"));
        seenPaySlip.add(paySlip);
      }
    });

    return errors;
  };

  const handleSave = async () => {
    const errors = validateBeforeSave();
    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    const isPersistedIntId = (value: unknown) =>
      typeof value === "number" &&
      Number.isInteger(value) &&
      value > 0 &&
      value <= 2147483647;

    const payload = {
      id: formData.id || 0,
      code: formData.code || "",
      deductionGroup: formData.deductionGroup,
      isActive: true,
      details: deductionDetailRows.map((d: any, idx: number) => ({
        id: isPersistedIntId(d.id) ? d.id : null,
        lineIndex: idx + 1,
        deductionTypeId: d.deductionTypeId ?? null,
        rateBasisId: d.rateBasisId ?? null,
        basisEarningTypeId: d.basisEarningTypeId ?? null,
        value: Number(d.value || 0),
        paySlipOrder: Number(d.paySlipOrder || 0),
        paySlipDescription: d.paySlipDescription || "",
        statusId: d.statusId ?? null,
      })),
    };

    setApiLoading(true);
    try {
      const response = formData.id
        ? await saveRecord("put", `/api/HumanResource/DeductionGroup/v3/${formData.id}`, payload)
        : await saveRecord("post", "/api/HumanResource/DeductionGroup/v3", payload);
      // The list row carries the server-shaped deductionGroup label, so refetch
      // rather than reconstruct the cached shape from the form payload.
      dispatch(deductionGroupSlice.fetchAll(true));
      setToasterMessages([formData.id ? translate("Record updated successfully") : translate("Record saved successfully")]);
      setToasterSeverity("success");
      setShowToaster(true);
      return true;
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to save deduction group"))]);
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
      const response = await saveRecord("delete", `/api/HumanResource/DeductionGroup/v3/${row.id}`);
      dispatch(deductionGroupSlice.actions.removeRow(row.id));
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to delete deduction group"))]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  const renderExpandIcon = (expanded: boolean) => (
    <Box sx={{ width: 22, height: 22, borderRadius: "50%", bgcolor: "rgba(var(--lms-primary-rgb), 0.1)", display: "flex", alignItems: "center", justifyContent: "center", color: "var(--lms-primary)", position: "relative" }}>
      <Box sx={{ width: 10, height: "2px", bgcolor: "currentColor" }} />
      <Box sx={{ width: "2px", height: 10, bgcolor: "currentColor", position: "absolute", transform: expanded ? "rotate(90deg)" : "rotate(0deg)", opacity: expanded ? 0 : 1 }} />
    </Box>
  );

  const renderForm = (isReadOnly?: boolean, submitted?: boolean) => (
    <FormThemeWrapper>
      <Box sx={{ p: 0, pointerEvents: isReadOnly ? "none" : "auto", opacity: isReadOnly ? 0.9 : 1 }}>
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, cursor: "pointer" }} onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
            <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>Basic Information</T></Typography>
          </Box>
          {renderExpandIcon(isGeneralExpanded)}
        </Box>
        <Collapse in={isGeneralExpanded}>
          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid item xs={12} md={4}>
              <FormInput label={translate("Code")} disabled value={formData.code || ""} />
            </Grid>
            <Grid item xs={12} md={8} />
            <Grid item xs={12} md={6}>
              <FormInput
                label={translate("Deduction Group*")}
                value={formData.deductionGroup || ""}
                onChange={(e: any) => setFormData({ ...formData, deductionGroup: e.target.value })}
                error={submitted && !formData.deductionGroup}
                helperText={submitted && !formData.deductionGroup ? translate("Deduction Group is required") : ""}
              />
            </Grid>
          </Grid>
        </Collapse>

        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, cursor: "pointer" }} onClick={() => setIsSubTableExpanded(!isSubTableExpanded)}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
            <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>General</T></Typography>
          </Box>
          {renderExpandIcon(isSubTableExpanded)}
        </Box>
        <Collapse in={isSubTableExpanded}>
          <SchemaTable
            title=""
            hideTitle
            columns={detailColumns}
            rows={deductionDetailRows}
            showAddButton={false}
            actions={false}
            showToaster={false}
            checkboxSelection
            selectedIds={selectedDeductionDetailIds}
            onSelectionChange={setSelectedDeductionDetailIds}
            maxHeight={320}
            extraToolbarItems={
              <Box sx={{ display: "flex", gap: 1 }}>
                <Button startIcon={<AddIcon />} onClick={handleAddDetailRow} sx={{ bgcolor: "var(--lms-primary)", color: "#fff", "&:hover": { bgcolor: "var(--lms-primary)", boxShadow: "0 2px 10px var(--lms-primary)" } }}>
                  <T>Add Row</T>
                </Button>
                <Button startIcon={<DeleteIcon />} onClick={handleDeleteSelectedDetailRows} sx={{ textTransform: "none", color: "#1E293B", fontWeight: 700, fontSize: "0.8rem" }}>
                  <T>Delete Selected</T>
                </Button>
              </Box>
            }
          />
        </Collapse>
      </Box>
    </FormThemeWrapper>
  );

  return (
    <>
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box sx={{ p: 2, display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 1.5 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Deduction Group") : viewMode === "edit" ? translate("Edit Deduction Group") : translate("View Deduction Group")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={() => setViewMode("list")} sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}>
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={async () => {
                    setIsSubmitted(true);
                    const ok = await handleSave();
                    if (ok) {
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
          <Box sx={{ px: 2, pb: 2 }}>{renderForm(viewMode === "view", isSubmitted)}</Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Deduction Group")}
          addButtonLabel={translate("New Deduction Group")}
          breadcrumbPath={""}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          onAddClick={() => {
            setFormData({ id: 0, code: "", deductionGroup: "" });
            setDeductionDetailRows([]);
            setSelectedDeductionDetailIds([]);
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => openForEditOrView(row, "edit")}
          onViewClick={(row: any) => openForEditOrView(row, "view")}
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

export default DeductionGroup;

