import React, { useState, useEffect } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";

import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
} from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { deviceTypeSlice } from "@/store/humanResourceSlices";

interface DeviceTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const DeviceType: React.FC<DeviceTypeProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrDeviceType",
    deviceTypeSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">(
    "list",
  );
  const [formData, setFormData] = useState<any>({
    id: 0,
    code: "",
    name: "",
    description: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Device Type"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Device Type"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Device Type"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const columns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "name", label: translate("Device Type"), minWidth: 200 },
    { id: "description", label: translate("Description"), minWidth: 250 },
  ];

  const initialFormData = {
    id: 0,
    code: "",
    name: "",
    description: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code))
      errors.push(translate("Code is required"));
    if (!CheckemptyString(data.name))
      errors.push(translate("Device Type is required"));

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    }

    setApiLoading(true);
    try {
      const response: any = data.id
        ? await saveRecord("put", `/api/HumanResource/DeviceType/v3/${data.id}`, data)
        : await saveRecord("post", "/api/HumanResource/DeviceType/v3", data);
      const saved: any = response?.data;
      setErrorMessages([
        data.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowError(true);
      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? data.id ?? 0);
      if (savedId > 0) {
        dispatch(deviceTypeSlice.actions.upsertRow({ ...data, id: savedId }));
      } else {
        dispatch(deviceTypeSlice.fetchAll(true));
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save device type");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      const response = await saveRecord("delete", `/api/HumanResource/DeviceType/v3/${row.id}`);
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(deviceTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      console.error(error);
      setErrorMessages([translate("Failed to delete device type")]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
  };

  const handleAddClick = () => {
    setFormData({ ...initialFormData });
    setIsSubmitted(false);
    setViewMode("new");
  };

  const handleEditClick = (row: any) => {
    setFormData({ ...row });
    setIsSubmitted(false);
    setViewMode("edit");
  };

  const handleViewClick = (row: any) => {
    setFormData({ ...row });
    setIsSubmitted(false);
    setViewMode("view");
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
    localFormData: any,
    setLocalFormData: React.Dispatch<React.SetStateAction<any>>,
    _mode: "new" | "edit" | "view",
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
                  value={localFormData.code || ""}
                  onChange={(e) =>
                    setLocalFormData({ ...localFormData, code: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !localFormData.code}
                  required
                  inputProps={{ maxLength: 10 }}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Device Type")}
                  placeholder={translate("Enter Device Type")}
                  value={localFormData.name || ""}
                  onChange={(e) =>
                    setLocalFormData({ ...localFormData, name: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !localFormData.name}
                  required
                  inputProps={{ maxLength: 50 }}
                />
              </Grid>

              <Grid item xs={12}>
                <FormInput
                  fullWidth
                  size="small"
                  multiline
                  rows={3}
                  label={translate("Description")}
                  placeholder={translate("Enter Description")}
                  value={localFormData.description || ""}
                  onChange={(e) =>
                    setLocalFormData({ ...localFormData, description: e.target.value })
                  }
                  disabled={isReadOnly}
                  inputProps={{ maxLength: 150 }}
                />
              </Grid>
            </Grid>
          </Collapse>
        </Box>
      </FormThemeWrapper>
    );
  };

  const renderContent = () => {
    if (viewMode !== "list") {
      return (
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
                ? translate("New Device Type")
                : viewMode === "edit"
                ? translate("Edit Device Type")
                : translate("View Device Type")}
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
                    if (ok) {
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
            {renderForm(
              formData,
              setFormData,
              viewMode,
              viewMode === "view",
              isSubmitted,
            )}
          </Box>
        </Box>
      );
    }

    return (
      <SchemaTable
        title={translate("Device Type")}
        addButtonLabel={translate("New")}
        columns={columns}
        rows={rows}
        onDelete={handleDelete}
        maxWidth="lg"
        onAddClick={handleAddClick}
        onEditClick={handleEditClick}
        onViewClick={handleViewClick}
        showAddButton={true}
        showToaster={false}
      />
    );
  };

  return (
    <>
      {renderContent()}
      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {loading && <LmsOverlayLoader />}
    </>
  );
};

export default DeviceType;
