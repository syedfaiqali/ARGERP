import React, { useState, useEffect } from "react";
import { Grid, Box, Typography, Collapse, Button, Paper } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
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
import { humanResourceAdministrationService } from "@/services/admin/administrationService";
import { apiErrorMessage } from '@/services/mutations';
import { useAppDispatch, useCachedRows } from "@/hooks";
import { documentTypeSlice } from "@/store/humanResourceSlices";

interface DocumentTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const DocumentType: React.FC<DocumentTypeProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrDocumentType",
    documentTypeSlice.fetchAll,
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
    documentType: "",
    description: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Document Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Document Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Document Type"));
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
    { id: "documentType", label: translate("Document Type"), minWidth: 200 },
    { id: "description", label: translate("Description"), minWidth: 250 },
  ];

  const initialFormData = {
    code: "",
    documentType: "",
    description: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code)) errors.push(translate("Code is required"));
    if (!CheckemptyString(data.documentType)) errors.push(translate("Document Type is required"));

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = { ...data };

      const response: any = data.id
        ? await humanResourceAdministrationService.updateDocumentType(data.id, payload)
        : await humanResourceAdministrationService.createDocumentType(payload);
      const saved: any = response?.data;
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
          dispatch(documentTypeSlice.actions.upsertRow({ ...payload, id: savedId }));
        } else {
          dispatch(documentTypeSlice.fetchAll(true));
        }
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save document type");
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
      const response = await humanResourceAdministrationService.deleteDocumentType(row.id);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(documentTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete document type");
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
    mode: "new" | "edit" | "view",
    _isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    const isReadOnly = mode === "view" || _isReadOnly;
    return (
      <FormThemeWrapper>
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {/* Section Header with Premium Expand Icon */}
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
                <T>Document Type Details</T>
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
              {/* Code */}
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

              {/* Document Type */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Document Type")}
                  placeholder={translate("Enter Document Type")}
                  value={data.documentType || ""}
                  onChange={(e) =>
                    setData({ ...data, documentType: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.documentType}
                  required
                />
              </Grid>

              {/* Description */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Description")}
                  placeholder={translate("Enter Description")}
                  value={data.description || ""}
                  onChange={(e) =>
                    setData({ ...data, description: e.target.value })
                  }
                  disabled={isReadOnly}
                  multiline
                  rows={1}
                />
              </Grid>

              {/* Status Checkbox */}
              <Grid item xs={12}>
                <LmsCheckbox
                  checked={data.isActive !== false}
                  onChange={(checked) =>
                    setData({
                      ...data,
                      isActive: checked,
                    })
                  }
                  label={translate("Mark as Active Document Type")}
                />
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
                ? translate("New Document Type")
                : viewMode === "edit"
                  ? translate("Edit Document Type")
                  : translate("View Document Type")}
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
          title={translate("Document Type")}
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
          hideTitle
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

export default DocumentType;

