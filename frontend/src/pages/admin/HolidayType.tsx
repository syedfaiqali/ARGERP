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
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
} from "@/constants/globalItems";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { holidayTypeSlice } from "@/store/humanResourceSlices";

interface HolidayTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const HolidayType: React.FC<HolidayTypeProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrHolidayType",
    holidayTypeSlice.fetchAll,
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
    holidayType: "",
    description: "",
    colorCode: "var(--lms-primary)",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  const normalizeColorValue = (value?: string) => {
    if (!value) return "var(--lms-primary)";
    const trimmed = value.trim();
    if (trimmed.startsWith("#")) {
      return trimmed.length === 4
        ? `#${trimmed[1]}${trimmed[1]}${trimmed[2]}${trimmed[2]}${trimmed[3]}${trimmed[3]}`
        : trimmed;
    }
    const rgbMatch = trimmed.match(/rgb\s*\(\s*(\d+)\s*[,\s]+\s*(\d+)\s*[,\s]+\s*(\d+)\s*\)/i);
    if (rgbMatch) {
      const r = Math.min(255, Math.max(0, parseInt(rgbMatch[1], 10)));
      const g = Math.min(255, Math.max(0, parseInt(rgbMatch[2], 10)));
      const b = Math.min(255, Math.max(0, parseInt(rgbMatch[3], 10)));
      return `#${r.toString(16).padStart(2, "0")}${g
        .toString(16)
        .padStart(2, "0")}${b.toString(16).padStart(2, "0")}`;
    }

    if (typeof document !== "undefined") {
      const canvas = document.createElement("canvas");
      const context = canvas.getContext("2d");
      if (context) {
        context.fillStyle = "var(--lms-primary)";
        context.fillStyle = trimmed;
        const normalized = context.fillStyle;
        if (normalized.startsWith("#")) return normalized;
      }
    }

    return "var(--lms-primary)";
  };

  const getErrorMessage = (error: any, fallback: string) => {
    const data = error?.response?.data;
    if (typeof data === "string") return data;
    if (Array.isArray(data)) return data.join(", ");
    return data?.message || data?.error || error?.message || fallback;
  };

  const normalizeText = (value: any) => String(value ?? "").trim().toLowerCase();


  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Holiday Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Holiday Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Holiday Type"));
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
    { id: "holidayType", label: translate("Holiday Type"), minWidth: 200 },
    { id: "description", label: translate("Description"), minWidth: 250 },
    {
      id: "colorCode",
      label: translate("Color Code"),
      minWidth: 150,
      format: (value: string) => (
        <Box
          sx={{
            width: "100%",
            height: 24,
            bgcolor: value || "#eee",
            borderRadius: "4px",
          }}
        />
      ),
    },
  ];

  const initialFormData = {
    code: "",
    holidayType: "",
    description: "",
    colorCode: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.holidayType)) errors.push(translate("Holiday Type is required"));
    if (!CheckemptyString(data.colorCode)) errors.push(translate("Color Code is required"));
    const duplicateHolidayType = rows.some((row: any) =>
      normalizeText(row.holidayType ?? row.HolidayType ?? row.stxt ?? row.Stxt) === normalizeText(data.holidayType) &&
      String(row.id ?? row.Id ?? "") !== String(data.id ?? data.Id ?? "")
    );
    if (duplicateHolidayType) errors.push(translate("Holiday Type already exists"));
    if (CheckemptyString(data.code) && rows.some((row: any) =>
      normalizeText(row.code ?? row.Code) === normalizeText(data.code) &&
      String(row.id ?? row.Id ?? "") !== String(data.id ?? data.Id ?? "")
    )) errors.push(translate("Code already exists"));

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    let saved: any;
    try {
      const response: any = data.id
        ? await workflowMutation("put", `/api/HumanResource/HolidayType/v3/${data.id}`, data)
        : await workflowMutation("post", "/api/HumanResource/HolidayType/v3", data);
      // Sent for approval: the record itself does not change yet, so the list is left as it is.
      if (pendingWorkflow(response)) return true;
      saved = response?.data;
      setToasterMessages([
        data.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      // Patch the cached row instead of reloading the whole list.
      {
        const savedId = Number(saved?.id ?? data.id ?? 0);
        if (savedId > 0) {
          dispatch(holidayTypeSlice.actions.upsertRow({ ...data, ...saved, id: savedId }));
        } else {
          dispatch(holidayTypeSlice.fetchAll(true));
        }
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save holiday type");
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
      const response = await workflowMutation("delete", `/api/HumanResource/HolidayType/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(holidayTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete holiday type");
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
    formData: any,
    setFormData: any,
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
            <Grid container spacing={2}>
              {/* Code */}
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Code")}
                    value={formData.code || ""}
                    disabled
                    sx={{
                      maxWidth: "200px",
                    }}
                  />
                </Box>
              </Grid>

              {/* Status */}
              <Grid item xs={12} md={6}>
                <Box
                  sx={{ mb: 1.5, display: "flex", justifyContent: "flex-end" }}
                >
                  <LmsCheckbox
                    checked={formData.isActive}
                    onChange={(checked) =>
                      setFormData({
                        ...formData,
                        isActive: checked,
                      })
                    }
                    label={translate("Mark as Active")}
                  />
                </Box>
              </Grid>

              {/* Holiday Type */}
              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Holiday Type")}
                    placeholder={translate("Enter Holiday Type")}
                    value={formData.holidayType || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, holidayType: e.target.value })
                    }
                    disabled={isReadOnly}
                    required
                    error={isSubmitted && !formData.holidayType}
                  />
                </Box>
              </Grid>

              {/* Description */}
              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Description")}
                    multiline
                    rows={3}
                    placeholder={translate("Enter Description")}
                    value={formData.description || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, description: e.target.value })
                    }
                    disabled={isReadOnly}
                  />
                </Box>
              </Grid>

              {/* Color Code */}
              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Color Code")}
                      placeholder={translate("Select Color")}
                      value={formData.colorCode || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, colorCode: e.target.value })
                      }
                      disabled={isReadOnly}
                      required
                      error={isSubmitted && !formData.colorCode}
                      sx={{
                        maxWidth: "200px",
                      }}
                    />
                    <Box sx={{ display: "flex", alignItems: "center" }}>
                      <input
                        type="color"
                        value={normalizeColorValue(formData.colorCode)}
                        onChange={(e) =>
                          setFormData({ ...formData, colorCode: e.target.value })
                        }
                        disabled={isReadOnly}
                        style={{
                          width: 34,
                          height: 34,
                          padding: 0,
                          border: "1px solid #dfe3e8",
                          borderRadius: 6,
                          background: "#fff",
                          boxShadow: "0 1px 2px rgba(16, 24, 40, 0.06)",
                          cursor: isReadOnly ? "not-allowed" : "pointer",
                          WebkitAppearance: "none",
                          appearance: "none",
                        }}
                      />
                    </Box>
                  </Box>
                </Box>
              </Grid>
            </Grid>
          </Collapse>
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
            }}
          >
            <Typography
              variant="h5"
              sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
            >
              {viewMode === "new"
                ? translate("New Holiday Type")
                : viewMode === "edit"
                  ? translate("Edit Holiday Type")
                  : translate("View Holiday Type")}
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
          title={translate("Holiday Type")}
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

export default HolidayType;
