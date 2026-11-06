import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsCheckbox } from "@/components/LmsCheckbox";
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
import { earningGroupSlice } from "@/store/humanResourceSlices";

type LookupOption = { id: number; code: string; shortName: string };

const EarningGroup: React.FC<{ onSubViewChange?: (subView: string | null) => void; resetKey?: number }> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the lookups payload both come from the cache.
  const {
    rows: cachedRows,
    lookups: cachedLookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrEarningGroup", earningGroupSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isSubTableExpanded, setIsSubTableExpanded] = useState(true);
  const [formData, setFormData] = useState<any>({ id: 0, code: "", stxt: "" });
  const [earningDetailRows, setEarningDetailRows] = useState<any[]>([]);
  const [selectedEarningDetailIds, setSelectedEarningDetailIds] = useState<any[]>([]);

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
        stxt: x.earningGroup ?? x.stxt,
      })),
    [cachedRows],
  );

  // The lookups endpoint returns one payload holding every list.
  const lookups = useMemo<{ earningTypes: LookupOption[]; rateBasis: LookupOption[]; statuses: LookupOption[] }>(
    () => {
      const payload = (cachedLookups?.all ?? [])[0] ?? {};
      return {
        earningTypes: mapLookup(payload?.earningTypes),
        rateBasis: mapLookup(payload?.rateBasis),
        statuses: mapLookup(payload?.statuses),
      };
    },
    [cachedLookups],
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Earning Group"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Earning Group"));
    else if (viewMode === "view") onSubViewChange(translate("View Earning Group"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) setViewMode("list");
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "stxt", label: translate("Earning Group"), minWidth: 300 },
  ];

  const updateDetailRow = (id: any, patch: Record<string, any>) =>
    setEarningDetailRows((prev) => prev.map((row) => (row.id === id ? { ...row, ...patch } : row)));

  const valueRegex = /^\d*\.?\d{0,2}$/;
  const payslipRegex = /^\d*$/;

  const detailColumns: Column[] = useMemo(
    () => [
      { id: "lineIndex", label: translate("S.No"), minWidth: 70 },
      {
        id: "earnTypeId",
        label: translate("Earning Type"),
        minWidth: 170,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.earnTypeId ?? ""}
            options={lookups.earningTypes}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { earnTypeId: opt?.id ?? null, earnTypeName: opt?.shortName ?? "" })}
            placeholder={translate("Select Earning Type")}
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
        id: "basisEarnTypeId",
        label: translate("Basis Earning Type"),
        minWidth: 180,
        format: (_: any, row?: any) => (
          <LookupDropdown
            hideLabel
            value={row?.basisEarnTypeId ?? ""}
            options={lookups.earningTypes}
            getOptionLabel={(opt: any) => opt.shortName || ""}
            onSearchClick={() => {}}
            onSelect={(opt: any) => updateDetailRow(row?.id, { basisEarnTypeId: opt?.id ?? null, basisEarnTypeName: opt?.shortName ?? "" })}
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
        id: "paySlipIndex",
        label: translate("Pay Slip Order"),
        minWidth: 130,
        format: (_: any, row?: any) => (
          <FormInput
            hideLabel
            size="small"
            value={row?.paySlipIndex ?? ""}
            onChange={(e: any) => {
              const nextValue = e.target.value;
              if (payslipRegex.test(nextValue)) updateDetailRow(row?.id, { paySlipIndex: nextValue });
            }}
          />
        ),
      },
      {
        id: "paySlipText",
        label: translate("Pay Slip Description"),
        minWidth: 180,
        format: (_: any, row?: any) => (
          <FormInput hideLabel size="small" value={row?.paySlipText ?? ""} onChange={(e: any) => updateDetailRow(row?.id, { paySlipText: e.target.value })} />
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
        id: "absNotEffective",
        label: translate("Absent Not Effective"),
        minWidth: 150,
        format: (_: any, row?: any) => <LmsCheckbox hideLabel checked={!!row?.absNotEffective} onChange={(checked) => updateDetailRow(row?.id, { absNotEffective: checked })} />,
      },
      {
        id: "levNotEffective",
        label: translate("Leave Not Effective"),
        minWidth: 150,
        format: (_: any, row?: any) => <LmsCheckbox hideLabel checked={!!row?.levNotEffective} onChange={(checked) => updateDetailRow(row?.id, { levNotEffective: checked })} />,
      },
      {
        id: "isOtPart",
        label: translate("OT Part"),
        minWidth: 90,
        format: (_: any, row?: any) => <LmsCheckbox hideLabel checked={!!row?.isOtPart} onChange={(checked) => updateDetailRow(row?.id, { isOtPart: checked })} />,
      },
    ],
    [lookups.earningTypes, lookups.rateBasis, lookups.statuses, translate],
  );

  const handleAddDetailRow = () => {
    const active = lookups.statuses.find((x) => String(x.shortName).toLowerCase() === "active");
    setEarningDetailRows((prev) => [
      ...prev,
      {
        id: Date.now() + Math.floor(Math.random() * 1000),
        lineIndex: prev.length + 1,
        earnTypeId: null,
        rateBasisId: null,
        basisEarnTypeId: null,
        value: "0",
        paySlipIndex: "",
        paySlipText: "",
        statusId: active?.id ?? null,
        absNotEffective: false,
        levNotEffective: false,
        isOtPart: false,
      },
    ]);
  };

  const handleDeleteSelectedDetailRows = () => {
    if (selectedEarningDetailIds.length === 0) return;
    setEarningDetailRows((prev) =>
      prev
        .filter((row) => !selectedEarningDetailIds.includes(row.id))
        .map((row, index) => ({ ...row, lineIndex: index + 1 })),
    );
    setSelectedEarningDetailIds([]);
  };

  const openForEditOrView = async (row: any, mode: "edit" | "view") => {
    setApiLoading(true);
    try {
      const response: any = await GetService(`/api/HumanResource/EarningGroup/v3/${row.id}`);
      const loaded = response?.data ?? row;
      setFormData({ id: loaded.id ?? 0, code: loaded.code ?? "", stxt: loaded.earningGroup ?? "" });
      setEarningDetailRows(
        Array.isArray(loaded?.details)
          ? loaded.details.map((d: any, index: number) => ({
              id: d.id,
              lineIndex: d.lineIndex ?? index + 1,
              earnTypeId: d.earnTypeId,
              earnTypeName: d.earnTypeName,
              rateBasisId: d.rateBasisId,
              rateBasisName: d.rateBasisName,
              basisEarnTypeId: d.basisEarnTypeId,
              basisEarnTypeName: d.basisEarnTypeName,
              value: d.value?.toString() ?? "",
              paySlipIndex: d.paySlipIndex?.toString() ?? "",
              paySlipText: d.paySlipText ?? "",
              statusId: d.statusId,
              statusName: d.statusName,
              absNotEffective: !!d.absNotEffective,
              levNotEffective: !!d.levNotEffective,
              isOtPart: !!d.isOtPart,
            }))
          : [],
      );
      setSelectedEarningDetailIds([]);
      setIsSubmitted(false);
      setViewMode(mode);
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to load earning group"))]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setApiLoading(false);
    }
  };

  const validateBeforeSave = () => {
    const errors: string[] = [];
    if (!formData.stxt) errors.push(translate("Earning Group is required"));
    if (earningDetailRows.length === 0) errors.push(translate("At least one detail row is required"));
    const seenEarnType = new Set<number>();
    const seenPaySlip = new Set<number>();
    earningDetailRows.forEach((d, idx) => {
      const line = `${translate("Line")} ${idx + 1}: `;
      if (!d.earnTypeId) errors.push(line + translate("Earning Type is required"));
      if (!d.rateBasisId) errors.push(line + translate("Rate Basis is required"));
      if (!d.statusId) errors.push(line + translate("Status is required"));
      const value = Number(d.value);
      if (!Number.isFinite(value) || value <= 0) errors.push(line + translate("Value must be greater than 0"));
      const slip = Number(d.paySlipIndex);
      if (!Number.isInteger(slip) || slip <= 0) errors.push(line + translate("Pay Slip Order must be greater than 0"));
      if (d.earnTypeId) {
        if (seenEarnType.has(d.earnTypeId)) errors.push(line + translate("Earning Type already exists"));
        seenEarnType.add(d.earnTypeId);
      }
      if (Number.isInteger(slip) && slip > 0) {
        if (seenPaySlip.has(slip)) errors.push(line + translate("Pay Slip Order already exists"));
        seenPaySlip.add(slip);
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
      earningGroup: formData.stxt,
      isActive: true,
      details: earningDetailRows.map((d: any, idx: number) => ({
        id: isPersistedIntId(d.id) ? d.id : null,
        lineIndex: idx + 1,
        earnTypeId: d.earnTypeId ?? null,
        rateBasisId: d.rateBasisId ?? null,
        basisEarnTypeId: d.basisEarnTypeId ?? null,
        value: Number(d.value || 0),
        paySlipIndex: Number(d.paySlipIndex || 0),
        paySlipText: d.paySlipText || "",
        statusId: d.statusId ?? null,
        absNotEffective: !!d.absNotEffective,
        levNotEffective: !!d.levNotEffective,
        isOtPart: !!d.isOtPart,
      })),
    };

    setApiLoading(true);
    try {
      const response = formData.id
        ? await saveRecord("put", `/api/HumanResource/EarningGroup/v3/${formData.id}`, payload)
        : await saveRecord("post", "/api/HumanResource/EarningGroup/v3", payload);
      // The list row carries the server-shaped earningGroup label, so refetch
      // rather than reconstruct the cached shape from the form payload.
      dispatch(earningGroupSlice.fetchAll(true));
      setToasterMessages([formData.id ? translate("Record updated successfully") : translate("Record saved successfully")]);
      setToasterSeverity("success");
      setShowToaster(true);
      return true;
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to save earning group"))]);
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
      const response = await saveRecord("delete", `/api/HumanResource/EarningGroup/v3/${row.id}`);
      dispatch(earningGroupSlice.actions.removeRow(row.id));
    } catch (error: any) {
      setToasterMessages([toErrorMessage(error, translate("Failed to delete earning group"))]);
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
                label={translate("Earning Group*")}
                value={formData.stxt || ""}
                onChange={(e: any) => setFormData({ ...formData, stxt: e.target.value })}
                error={submitted && !formData.stxt}
                helperText={submitted && !formData.stxt ? translate("Earning Group is empty") : ""}
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
            rows={earningDetailRows}
            showAddButton={false}
            actions={false}
            showToaster={false}
            checkboxSelection
            selectedIds={selectedEarningDetailIds}
            onSelectionChange={setSelectedEarningDetailIds}
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
              {viewMode === "new" ? translate("New Earning Group") : viewMode === "edit" ? translate("Edit Earning Group") : translate("View Earning Group")}
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
          title={translate("Earning Group")}
          addButtonLabel={translate("New Earning Group")}
          breadcrumbPath={""}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          onAddClick={() => {
            setFormData({ id: 0, code: "", stxt: "" });
            setEarningDetailRows([]);
            setSelectedEarningDetailIds([]);
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

export default EarningGroup;
