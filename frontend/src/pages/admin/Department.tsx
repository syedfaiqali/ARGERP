import { organizationTextErrors } from "@/utils/organizationValidation";
import React, { useState, useEffect, useMemo} from "react";
import { pendingWorkflow, workflowMutation } from '@/workflow/api';
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  PostService,
  PutService,
  DeleteService,
  CheckemptyString,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { departmentSlice } from "@/store/organizationSlices";

type FicoaLookup = {
  id: number;
  title: string;
  code: string;
};

interface DepartmentProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const makeInitial = () => ({
  id: 0,
  code: "",
  departmentName: "",
  chartOfAccountId: null,
  chartOfAccount: "",
  receivingChartOfAccountId: null,
  receivingChartOfAccount: "",
  description: "",
});

const Department: React.FC<DepartmentProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const dispatch = useAppDispatch();
  // Rows and the ficoa lookup are fetched together once and then served from cache.
  const {
    rows: backendRows,
    lookups,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgDepartment", departmentSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  const [coaModalOpen, setCoaModalOpen] = useState(false);
  const [coaMode, setCoaMode] = useState<"standard" | "receiving">("standard");

  const ficoas = useMemo<FicoaLookup[]>(
    () => (Array.isArray(lookups?.ficoa) ? lookups.ficoa : []),
    [lookups],
  );

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Department"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Department"));
    else if (viewMode === "view") onSubViewChange(translate("View Department"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
      setFormData(makeInitial());
      setIsSubmitted(false);
    }
  }, [resetKey]);

  const fail = (msg: string | string[]) => {
    setToasterMessages(Array.isArray(msg) ? msg : [msg]);
    setToasterSeverity("error");
    setShowToaster(true);
  };

  const success = (msg: string) => {
    setToasterMessages([msg]);
    setToasterSeverity("success");
    setShowToaster(true);
  };

  const columns = React.useMemo<Column[]>(
    () => [
      { id: "sNo", label: translate("S.No"), minWidth: 60, format: (_, __, i) => (i ?? 0) + 1 },
      { id: "code", label: translate("Code"), minWidth: 100 },
      { id: "departmentName", label: translate("Department"), minWidth: 200 },
      { id: "description", label: translate("Description"), minWidth: 250 },
      {
        id: "chartOfAccount",
        label: translate("Chart of Account"),
        minWidth: 200,
      },
    ],
    [translate],
  );

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = organizationTextErrors(formData, backendRows, [
      {field: "departmentName", label: "Department", maxLength: 50, duplicate: true},
      {field: "description", label: "Description", maxLength: 500},
    ]).map(message => translate(message));
    if (!CheckemptyString(formData.departmentName)) errors.push(translate("Department is required"));
    if ((formData.departmentName || "").trim().length > 50) errors.push(translate("Department cannot exceed 50 characters"));
    if ((formData.description || "").trim().length > 500) errors.push(translate("Description cannot exceed 500 characters"));

    if (errors.length > 0) {
      fail(errors);
      return;
    }

    try {
      setApiLoading(true);
      const payload = {
        id: formData.id,
        code: formData.code,
        departmentName: formData.departmentName,
        description: formData.description,
        chartOfAccountId: formData.chartOfAccountId,
        receivingChartOfAccountId: formData.receivingChartOfAccountId,
      };

      let saved: any;
      if (formData.id && formData.id > 0) {
        const response = await workflowMutation('put', `/api/organization/Department/v3/${formData.id}`, payload);
        if (pendingWorkflow(response)) { setViewMode('list'); return; }
        if (response?.status >= 400) throw { response };
        saved = response?.data?.data ?? response?.data;
        success(translate("Updated successfully"));
      } else {
        const response = await workflowMutation('post', "/api/organization/Department/v3", payload);
        if (pendingWorkflow(response)) { setViewMode('list'); return; }
        if (response?.status >= 400) throw { response };
        saved = response?.data?.data ?? response?.data;
        success(translate("Saved successfully"));
      }

      // Patch the cached row; chartOfAccount names are display-only on the grid.
      await dispatch(departmentSlice.fetchAll(true));
      const savedId = Number(saved?.id ?? saved?.Id ?? formData.id ?? 0);
      if (savedId > 0) {
        dispatch(departmentSlice.actions.upsertRow({
          ...payload,
          ...saved,
          id: savedId,
          chartOfAccount: formData.chartOfAccount ?? "",
          receivingChartOfAccount: formData.receivingChartOfAccount ?? "",
        }));
      } else {
        dispatch(departmentSlice.fetchAll(true));
      }
      setViewMode("list");
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to save department"));
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    if (!row.id) return;
    try {
      setApiLoading(true);
      if (pendingWorkflow(await workflowMutation('delete', `/api/organization/Department/v3/${row.id}`))) return;
      success(translate("Deleted successfully"));
      dispatch(departmentSlice.actions.removeRow(Number(row.id)));
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to delete department"));
    } finally {
      setApiLoading(false);
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
      {!expanded && <Box sx={{ width: "2px", height: 8, bgcolor: "var(--lms-primary)", position: "absolute" }} />}
    </Box>
  );

  const isReadOnly = viewMode === "view";

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable showToaster={false}
          title={translate("Department")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={backendRows}
          onDelete={handleDelete}
          onAddClick={() => { setFormData(makeInitial()); setViewMode("new"); setIsSubmitted(false); }}
          onEditClick={(row) => { setFormData(row); setViewMode("edit"); setIsSubmitted(false); }}
          onViewClick={(row) => { setFormData(row); setViewMode("view"); setIsSubmitted(false); }}
          hideTitle
          hideBreadcrumbs
        />
      ) : (
        <Box sx={{ mt: 0 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Department") : viewMode === "edit" ? translate("Edit Department") : translate("View Department")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
              >
                {translate("Back to List")}
              </Button>
              {!isReadOnly && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSave}
                  disabled={apiLoading}
                  sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>

          <FormThemeWrapper>
            <Box sx={{ p: 0 }}>
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  mb: 2,
                  cursor: "pointer",
                  userSelect: "none",
                  "&:hover": {
                    "& .expand-icon-container": { bgcolor: "rgba(var(--lms-primary-rgb), 0.05)" },
                  },
                }}
                onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}
              >
                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                  <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                  <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                    <T>Basic Information</T>
                  </Typography>
                </Box>
                <Box className="expand-icon-container" sx={{ transition: "background-color 0.3s ease", borderRadius: "50%", p: 0.5 }}>
                  {renderExpandIcon(isGeneralExpanded)}
                </Box>
              </Box>

              <Collapse in={isGeneralExpanded}>
                <Grid container spacing={3}>
                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Code")}
                      placeholder={translate("Auto generated")}
                      value={formData.code || ""}
                      disabled
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Department*")}
                      placeholder={translate("Department")}
                      value={formData.departmentName || ""}
                      onChange={(e) => setFormData({ ...formData, departmentName: e.target.value })}
                      disabled={isReadOnly}
                      inputProps={{ maxLength: 50 }}
                      error={isSubmitted && (!formData.departmentName || (formData.departmentName || "").trim().length > 50)}
                      helperText={
                        isSubmitted && !formData.departmentName
                          ? translate("Department is required")
                          : (formData.departmentName || "").trim().length > 50
                            ? translate("Department cannot exceed 50 characters")
                            : ""
                      }
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <LookupDropdown
                      label={translate("Chart of Account")}
                      value={formData.chartOfAccount || ""}
                      options={ficoas}
                      getOptionLabel={(o: any) => typeof o === "string" ? o : o?.title || ""}
                      onSelect={(o: any) => setFormData({ ...formData, chartOfAccountId: o.id, chartOfAccount: o.title })}
                      onSearchClick={() => { setCoaMode("standard"); setCoaModalOpen(true); }}
                      disabled={isReadOnly}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <LookupDropdown
                      label={translate("Receiving Chart of Account")}
                      value={formData.receivingChartOfAccount || ""}
                      options={ficoas}
                      getOptionLabel={(o: any) => typeof o === "string" ? o : o?.title || ""}
                      onSelect={(o: any) => setFormData({ ...formData, receivingChartOfAccountId: o.id, receivingChartOfAccount: o.title })}
                      onSearchClick={() => { setCoaMode("receiving"); setCoaModalOpen(true); }}
                      disabled={isReadOnly}
                    />
                  </Grid>

                  <Grid item xs={12}>
                    <FormInput
                      label={translate("Description")}
                      placeholder={translate("Description")}
                      multiline
                      rows={3}
                      value={formData.description || ""}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                      disabled={isReadOnly}
                      inputProps={{ maxLength: 500 }}
                    />
                  </Grid>
                </Grid>
              </Collapse>
            </Box>
          </FormThemeWrapper>
        </Box>
      )}
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
      <GenericSearchModal
        open={coaModalOpen}
        onClose={() => setCoaModalOpen(false)}
        title={coaMode === "receiving" ? translate("Receiving Chart of Account") : translate("Chart of Account")}
        columns={[{ id: "code", label: translate("Account Code"), minWidth: 120 }, { id: "title", label: translate("Account Title"), minWidth: 260 }]}
        data={ficoas}
        onSelect={(val) => {
          setFormData({
            ...formData,
            [coaMode === "receiving" ? "receivingChartOfAccountId" : "chartOfAccountId"]: val.id,
            [coaMode === "receiving" ? "receivingChartOfAccount" : "chartOfAccount"]: val.title,
          });
          setCoaModalOpen(false);
        }}
      />
    </>
  );
};

export default Department;
