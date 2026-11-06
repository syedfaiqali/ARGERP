import React, { useEffect, useState } from "react";
import {
  Grid,
  Box,
  Typography,
  Collapse,
  Button,
} from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { deductionTypeSlice } from "@/store/humanResourceSlices";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";

const DeductionType: React.FC<{ onSubViewChange?: (subView: string | null) => void; resetKey?: number }> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const dispatch = useAppDispatch();
  const {
    rows: cachedRows,
    lookups: cachedLookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrDeductionType", deductionTypeSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [isSubmitted, setIsSubmitted] = useState(false);
  // sNo is a display index, derived here rather than cached.
  const rows = React.useMemo<any[]>(
    () => (cachedRows || []).map((x: any, index: number) => ({ ...x, sNo: index + 1 })),
    [cachedRows],
  );
  const [activeLookup, setActiveLookup] = useState<"paymentRunType" | null>(null);
  // The lookups endpoint returns one payload holding every list.
  const lookups = React.useMemo<any>(() => {
    const root = (cachedLookups?.all ?? [])[0] ?? {};
    const data = root?.data ?? root?.Data ?? root;
    return {
      paymentRunTypes: data?.paymentRunTypes ?? data?.PaymentRunTypes ?? [],
    };
  }, [cachedLookups]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "deductionType", label: translate("Deduction Type"), minWidth: 300 },
    {
      id: "paymentRunType",
      label: translate("Payment Run Type"),
      minWidth: 200,
    },
  ];

  const initialFormData = {
    id: 0,
    code: "",
    deductionType: "",
    description: "",
    paymentRunTypeId: null,
    paymentRunType: "",
    reimbursed: false,
    isActive: true,
  };
  const [formData, setFormData] = useState<any>(initialFormData);

  const [isBasicExpanded, setIsBasicExpanded] = useState(true);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);

  React.useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Deduction Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Deduction Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Deduction Type"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  React.useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
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
        shortName: x?.name ?? x?.Name ?? x?.stxt ?? x?.Stxt ?? x?.label ?? "",
      }))
      .filter((x: any) => !!x.shortName);


  const handleSave = async (currentFormData: any) => {
    const errors: string[] = [];
    if (!currentFormData.deductionType)
      errors.push(translate("Deduction Type is required"));
    if (!currentFormData.paymentRunTypeId)
      errors.push(translate("Payment Run Type is required"));

    if (errors.length > 0) {
      showError(errors);
      return false;
    }

    const payload = {
      id: currentFormData.id || 0,
      code: currentFormData.code || null,
      deductionType: currentFormData.deductionType,
      description: currentFormData.description || "",
      paymentRunTypeId: currentFormData.paymentRunTypeId,
      reimbursed: !!currentFormData.reimbursed,
      isActive: currentFormData.isActive !== false,
    };

    setApiLoading(true);
    try {
      const response: any = currentFormData.id && currentFormData.id > 0
        ? await workflowMutation("put", `/api/HumanResource/DeductionType/v3/${currentFormData.id}`, payload)
        : await workflowMutation("post", "/api/HumanResource/DeductionType/v3", payload);
      // Sent for approval: the record itself does not change yet, so the list is left as it is.
      if (pendingWorkflow(response)) return true;
      const saved: any = response?.data;
      // Patch the cached row instead of reloading the list; the lookups payload
      // is reference data unaffected by this save.
      const savedId = Number(saved?.id ?? currentFormData.id ?? 0);
      if (savedId > 0) {
        dispatch(
          deductionTypeSlice.actions.upsertRow({ ...currentFormData, ...payload, id: savedId }),
        );
      } else {
        dispatch(deductionTypeSlice.fetchAll(true));
      }
      setToasterMessages([
        currentFormData.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      return true;
    } catch (error: any) {
      showError([apiErrorMessage(error, translate("Failed to save deduction type"))]);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      const response = await workflowMutation("delete", `/api/HumanResource/DeductionType/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      dispatch(deductionTypeSlice.actions.removeRow(row.id));
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } catch (error: any) {
      showError([apiErrorMessage(error, translate("Failed to delete deduction type"))]);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

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

  const renderForm = (
    formData: any,
    setFormData: any,
    isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    return (
      <FormThemeWrapper>
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {/* Basic Information Section */}
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
                  fontWeight: 800,
                  color: "var(--lms-text-main)",
                  fontSize: "0.95rem",
                }}
              >
                <T>Basic Information</T>
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
            <Grid container spacing={3}>
              {/* Code */}
              <Grid item xs={12} md={4}>
                <FormInput
                  label={translate("Code")}
                  disabled
                  value={formData.code || ""}
                  placeholder={translate("Auto Generated")}
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} md={8} />

              {/* Deduction Type */}
              <Grid item xs={12} md={6}>
                <FormInput
                  label={translate("Deduction Type")}
                  required
                  placeholder={translate("Enter Deduction Type")}
                  value={formData.deductionType || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, deductionType: e.target.value })
                  }
                  error={isSubmitted && !formData.deductionType}
                  helperText={
                    isSubmitted && !formData.deductionType
                      ? translate("Deduction Type is required")
                      : ""
                  }
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} md={6} />

              {/* Description */}
              <Grid item xs={12} md={6}>
                <FormInput
                  label={translate("Description")}
                  placeholder={translate("Enter Description")}
                  multiline
                  rows={2}
                  value={formData.description || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                  size="small"
                  fullWidth
                />
              </Grid>
            </Grid>
          </Collapse>

          {/* General Section */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 3,
              mt: 2,
              cursor: "pointer",
              userSelect: "none",
              "&:hover": {
                "& .expand-icon-container": {
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
                },
              },
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
                <T>General</T>
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
              {renderExpandIcon(isGeneralExpanded)}
            </Box>
          </Box>

          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={3}>
              {/* Payment Run Type */}
              <Grid item xs={12} md={4}>
                <LookupDropdown
                  label={translate("Payment Run Type")}
                  required
                  value={formData.paymentRunType || ""}
                  options={mapLookup(lookups.paymentRunTypes)}
                  getOptionLabel={(opt: any) => opt.shortName || ""}
                  placeholder={translate("Select Payment Run Type")}
                  onSearchClick={() => !isReadOnly && setActiveLookup("paymentRunType")}
                  onSelect={(row: any) =>
                    setFormData({
                      ...formData,
                      paymentRunTypeId: row?.id ?? null,
                      paymentRunType: row?.shortName || "",
                    })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !formData.paymentRunTypeId}
                />
              </Grid>
              <Grid item xs={12} md={8} />

              <Grid item xs={12}>
                <LmsCheckbox
                  checked={formData.reimbursed || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      reimbursed: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  label={
                    <Typography
                      sx={{
                        fontSize: "0.8rem",
                        fontWeight: 600,
                        color: "#475569",
                      }}
                    >
                      <T>Re-imbursed</T>
                    </Typography>
                  }
                />
              </Grid>
            </Grid>
          </Collapse>
          <GenericSearchModal
            open={!!activeLookup}
            onClose={() => setActiveLookup(null)}
            title={translate("Select Payment Run Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 100 },
              { id: "shortName", label: translate("Name"), minWidth: 220 },
            ]}
            data={activeLookup === "paymentRunType" ? mapLookup(lookups.paymentRunTypes) : []}
            selectedValue={activeLookup === "paymentRunType" ? formData.paymentRunTypeId : ""}
            selectedField="id"
            onSelect={(row: any) => {
              setFormData({
                ...formData,
                paymentRunTypeId: row?.id ?? null,
                paymentRunType: row?.shortName || "",
              });
              setActiveLookup(null);
            }}
          />
        </Box>
      </FormThemeWrapper>
    );
  };

  return (
    <>
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
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
            <Typography
              variant="h5"
              sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
            >
              {viewMode === "new"
                ? translate("New Deduction Type")
                : viewMode === "edit"
                  ? translate("Edit Deduction Type")
                  : translate("View Deduction Type")}
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
            {renderForm(formData, setFormData, viewMode === "view", isSubmitted)}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Deduction Type")}
          addButtonLabel={translate("New Deduction Type")}
          breadcrumbPath={""}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
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

export default DeductionType;
