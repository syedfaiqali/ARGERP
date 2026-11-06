import React, { useState, useEffect } from "react";
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
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { CheckemptyString } from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';
import { useAppDispatch, useCachedRows } from "@/hooks";
import { skillTypeSlice } from "@/store/humanResourceSlices";

interface SkillTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
  legacyDialog?: boolean;
  onClose?: () => void;
}

const SkillType: React.FC<SkillTypeProps> = ({
  onSubViewChange,
  resetKey,
  legacyDialog = false,
  onClose,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrSkillType",
    skillTypeSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    code: "",
    name: "",
    description: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Skill Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Skill Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Skill Type"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    {
      id: "name",
      label: translate("Skill Type"),
      minWidth: 250,
    },
    { id: "description", label: translate("Description"), minWidth: 300 },
  ];

  const initialFormData = {
    code: "",
    name: "",
    description: "",
    isActive: true,
  };

  const resetForm = () => {
    setFormData({ ...initialFormData });
    setIsSubmitted(false);
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code)) errors.push(translate("Code is required"));
    if (!CheckemptyString(data.name)) errors.push(translate("Skill Type is required"));

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const response: any = data.id
        ? await saveRecord("put", `/api/HumanResource/SkillType/v3/${data.id}`, data)
        : await saveRecord("post", "/api/HumanResource/SkillType/v3", data);
      const saved: any = response?.data;
      setToasterMessages([
        data.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? data.id ?? 0);
      if (savedId > 0) {
        dispatch(skillTypeSlice.actions.upsertRow({ ...data, id: savedId }));
      } else {
        dispatch(skillTypeSlice.fetchAll(true));
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save skill type");
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
      const response = await saveRecord("delete", `/api/HumanResource/SkillType/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(skillTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete skill type");
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
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
    data: any,
    setData: any,
    _mode: string,
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
                <T>Skill Type Details</T>
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
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Code")}
                  placeholder={translate("Enter Code")}
                  value={data.code || ""}
                  onChange={(e) =>
                    setData({ ...data, code: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.code}
                  required
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Skill Type")}
                  placeholder={translate("Enter Skill Type")}
                  value={data.name || ""}
                  onChange={(e) =>
                    setData({
                      ...data,
                      name: e.target.value,
                    })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.name}
                  required
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Description")}
                  multiline
                  rows={1}
                  placeholder={translate("Enter Description")}
                  value={data.description || ""}
                  onChange={(e) =>
                    setData({ ...data, description: e.target.value })
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              <Grid item xs={12}>
                <LmsCheckbox
                  checked={data.isActive}
                  onChange={(checked) =>
                    setData({
                      ...data,
                      isActive: checked,
                    })
                  }
                  label={translate("Mark as Active Skill Type")}
                />
              </Grid>
            </Grid>
          </Collapse>
        </Box>
      </FormThemeWrapper>
    );
  };

  const renderLegacyDialogForm = () => {
    const save = async (closeAfterSave: boolean) => {
      setIsSubmitted(true);
      const ok = await handleSave(formData);
      if (ok !== false) {
        resetForm();
        if (closeAfterSave) onClose?.();
      }
    };

    return (
      <Box sx={{ p: 2 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-end",
            gap: 1.5,
            mb: 2,
          }}
        >
          <Button
            variant="outlined"
            onClick={resetForm}
            sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
          >
            <T>New</T>
          </Button>
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={() => void save(false)}
            sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
          >
            <T>Save</T>
          </Button>
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={() => void save(true)}
            sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
          >
            <T>Save & Close</T>
          </Button>
        </Box>

        <FormThemeWrapper>
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
            {renderExpandIcon(isGeneralExpanded)}
          </Box>

          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Code")}
                  placeholder={translate("Enter Code")}
                  value={formData.code || ""}
                  onChange={(event) =>
                    setFormData({ ...formData, code: event.target.value })
                  }
                  error={isSubmitted && !formData.code}
                  required
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Skill Type")}
                  placeholder={translate("Enter Skill Type")}
                  value={formData.name || ""}
                  onChange={(event) =>
                    setFormData({ ...formData, name: event.target.value })
                  }
                  error={isSubmitted && !formData.name}
                  required
                />
              </Grid>

              <Grid item xs={12}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Description")}
                  multiline
                  rows={2}
                  placeholder={translate("Enter Description")}
                  value={formData.description || ""}
                  onChange={(event) =>
                    setFormData({ ...formData, description: event.target.value })
                  }
                />
              </Grid>
            </Grid>
          </Collapse>
        </FormThemeWrapper>
      </Box>
    );
  };

  return (
    <>
      {legacyDialog ? (
        renderLegacyDialogForm()
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
                ? translate("New Skill Type")
                : viewMode === "edit"
                  ? translate("Edit Skill Type")
                  : translate("View Skill Type")}
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
            {renderForm(formData, setFormData, viewMode, viewMode === "view", isSubmitted)}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Skill Type")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          maxWidth="lg"
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

export default SkillType;
