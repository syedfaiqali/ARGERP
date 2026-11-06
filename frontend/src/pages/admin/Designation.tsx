import { organizationTextErrors } from "@/utils/organizationValidation";
import React, { useState, useEffect, useRef } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";
import { FormInput } from "@/components/FormInput";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  PostService,
  PutService,
  DeleteService,
  CheckemptyString,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { designationSlice } from "@/store/organizationSlices";
import { EmployeePayrollTab, type EmployeePayrollTabHandle } from "./components/EmployeePayrollTab";

interface DesignationProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const makeInitial = () => ({
  id: 0,
  code: "",
  designationName: "",
  description: "",
  inactive: false,
});

const Designation: React.FC<DesignationProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const {
    rows: backendRows,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgDesignation", designationSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [activeTab, setActiveTab] = useState(0);
  const payrollTabRef = useRef<EmployeePayrollTabHandle>(null);

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Designation"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Designation"));
    else if (viewMode === "view") onSubViewChange(translate("View Designation"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
      setFormData(makeInitial());
      setIsSubmitted(false);
      setActiveTab(0);
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
      { id: "code", label: translate("Designation Code"), minWidth: 150 },
      { id: "designationName", label: translate("Designation"), minWidth: 200 },
      { id: "description", label: translate("Description"), minWidth: 250 },
    ],
    [translate],
  );

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = organizationTextErrors(formData, backendRows, [
      {field: "designationName", label: "Designation", maxLength: 50, duplicate: true},
      {field: "description", label: "Description", maxLength: 500},
    ]).map(message => translate(message));
    if (!CheckemptyString(formData.designationName)) errors.push(translate("Designation is required"));

    const payrollError = payrollTabRef.current?.validateConfiguredPayroll();
    if (payrollError) errors.push(payrollError);

    if (errors.length > 0) {
      fail(errors);
      return;
    }

    try {
      setApiLoading(true);
      const payload = {
        id: formData.id,
        code: formData.code,
        designationName: formData.designationName,
        description: formData.description,
        inactive: !!formData.inactive,
      };

      let saved: any;
      if (formData.id && formData.id > 0) {
        const response: any = await PutService(`/api/organization/Designation/v3/${formData.id}`, payload);
        if (response?.status >= 400) throw { response };
        saved = response?.data?.data ?? response?.data;
      } else {
        const response: any = await PostService("/api/organization/Designation/v3", payload);
        if (response?.status >= 400) throw { response };
        saved = response?.data?.data ?? response?.data;
      }

      // Patch the cached row instead of reloading the whole list.
      await dispatch(designationSlice.fetchAll(true));
      const savedId = Number(saved?.id ?? saved?.Id ?? formData.id ?? 0);
      if (payrollTabRef.current?.hasPayrollSetup() && savedId > 0) {
        await payrollTabRef.current.save(savedId);
      }
      if (savedId > 0) {
        dispatch(designationSlice.actions.upsertRow({ ...payload, ...saved, id: savedId }));
      } else {
        dispatch(designationSlice.fetchAll(true));
      }
      success(translate(formData.id && formData.id > 0 ? "Updated successfully" : "Saved successfully"));
      setViewMode("list");
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to save designation"));
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    if (!row.id) return;
    try {
      setApiLoading(true);
      const response: any = await DeleteService(`/api/organization/Designation/v3/${row.id}`);
      if (response?.status >= 400) throw { response };
      success(translate("Deleted successfully"));
      dispatch(designationSlice.fetchAll(true));
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to delete designation"));
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
          title={translate("Designation")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={backendRows}
          onDelete={handleDelete}
          onAddClick={() => { setFormData(makeInitial()); setViewMode("new"); setIsSubmitted(false); setActiveTab(0); }}
          onEditClick={(row) => { setFormData(row); setViewMode("edit"); setIsSubmitted(false); setActiveTab(0); }}
          onViewClick={(row) => { setFormData(row); setViewMode("view"); setIsSubmitted(false); setActiveTab(0); }}
          hideTitle
          hideBreadcrumbs
        />
      ) : (
        <Box sx={{ mt: 0 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Designation") : viewMode === "edit" ? translate("Edit Designation") : translate("View Designation")}
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
              <StyledTabs
                value={activeTab}
                onChange={(_, value) => setActiveTab(value)}
                tabs={[
                  { label: translate("Basic Information") },
                  { label: translate("Salary") },
                ]}
              />
              <TabPanel value={activeTab} index={0} padding={3} keepMounted>
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
                      label={translate("Designation*")}
                      placeholder={translate("Enter Designation")}
                      value={formData.designationName || ""}
                      onChange={(e) => setFormData({ ...formData, designationName: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.designationName}
                      helperText={isSubmitted && !formData.designationName ? translate("Designation is required") : ""}
                    />
                  </Grid>

                  <Grid item xs={12}>
                    <FormInput
                      label={translate("Description")}
                      placeholder={translate("Enter Description")}
                      multiline
                      rows={3}
                      value={formData.description || ""}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                      disabled={isReadOnly}
                    />
                  </Grid>

                  <Grid item xs={12}>
                    <LmsCheckbox
                      label={translate("Inactive")}
                      checked={!!formData.inactive}
                      onChange={(checked) => setFormData({ ...formData, inactive: checked })}
                      disabled={isReadOnly}
                    />
                  </Grid>
                </Grid>
              </Collapse>
              </TabPanel>
              <TabPanel value={activeTab} index={1} padding={3} keepMounted>
                <EmployeePayrollTab
                  ref={payrollTabRef}
                  employeeId={undefined}
                  designationId={Number(formData.id || 0) || undefined}
                  payrollFor="designation"
                  readOnly={isReadOnly}
                  hideSaveButton
                />
              </TabPanel>
            </Box>
          </FormThemeWrapper>
        </Box>
      )}
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Designation;
