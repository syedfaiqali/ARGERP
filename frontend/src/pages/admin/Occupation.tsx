import { organizationTextErrors } from "@/utils/organizationValidation";
import React, { useState, useEffect } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
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
import { occupationSlice } from "@/store/organizationSlices";

interface OccupationProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const makeInitial = () => ({
  id: 0,
  code: "",
  occupationName: "",
  description: "",
});

const Occupation: React.FC<OccupationProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const {
    rows: backendRows,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgOccupation", occupationSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Occupation"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Occupation"));
    else if (viewMode === "view") onSubViewChange(translate("View Occupation"));
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
      { id: "code", label: translate("Code"), minWidth: 120 },
      { id: "occupationName", label: translate("Occupation"), minWidth: 220 },
      { id: "description", label: translate("Description"), minWidth: 250 },
    ],
    [translate],
  );

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = organizationTextErrors(formData, backendRows, [
      {field: "code", label: "Code", maxLength: 10, duplicate: true},
      {field: "occupationName", label: "Occupation", maxLength: 150, duplicate: true},
      {field: "description", label: "Description", maxLength: 500},
    ]).map(message => translate(message));
    if (!CheckemptyString(formData.code)) errors.push(translate("Code is required"));
    if (!CheckemptyString(formData.occupationName)) errors.push(translate("Occupation is required"));

    if (errors.length > 0) {
      fail(errors);
      return;
    }

    try {
      setApiLoading(true);
      const payload = {
        id: formData.id,
        code: formData.code,
        occupationName: formData.occupationName,
        description: formData.description,
      };

      let saved: any;
      if (formData.id && formData.id > 0) {
        const response: any = await PutService(`/api/organization/Occupation/v3/${formData.id}`, payload);
        saved = response?.data;
        success(translate("Updated successfully"));
      } else {
        const response: any = await PostService("/api/organization/Occupation/v3", payload);
        saved = response?.data;
        success(translate("Saved successfully"));
      }

      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? formData.id ?? 0);
      if (savedId > 0) {
        dispatch(occupationSlice.actions.upsertRow({ ...payload, id: savedId }));
      } else {
        dispatch(occupationSlice.fetchAll(true));
      }
      setViewMode("list");
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to save occupation"));
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    if (!row.id) return;
    try {
      setApiLoading(true);
      await DeleteService(`/api/organization/Occupation/v3/${row.id}`);
      success(translate("Deleted successfully"));
      dispatch(occupationSlice.fetchAll(true));
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to delete occupation"));
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
          title={translate("Occupation")}
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
              {viewMode === "new" ? translate("New Occupation") : viewMode === "edit" ? translate("Edit Occupation") : translate("View Occupation")}
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
                      label={translate("Code*")}
                      placeholder={translate("Enter Code")}
                      value={formData.code || ""}
                      onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.code}
                      helperText={isSubmitted && !formData.code ? translate("Code is required") : ""}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Occupation*")}
                      placeholder={translate("Enter Occupation")}
                      value={formData.occupationName || ""}
                      onChange={(e) => setFormData({ ...formData, occupationName: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.occupationName}
                      helperText={isSubmitted && !formData.occupationName ? translate("Occupation is required") : ""}
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
                </Grid>
              </Collapse>
            </Box>
          </FormThemeWrapper>
        </Box>
      )}
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Occupation;
