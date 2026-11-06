import { FormInput } from "@/components/FormInput";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import React, { useEffect, useState } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import {
  CheckemptyString,
  DeleteService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { payrollRegisterSlice } from "@/store/humanResourceSlices";

const PayrollRegister: React.FC<{ onSubViewChange?: (subView: string | null) => void; resetKey?: number }> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();

  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrPayrollRegister",
    payrollRegisterSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "error",
  );
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">(
    "list",
  );
  const [formData, setFormData] = useState<any>({
    code: "",
    stxt: "",
    ltxt: "",
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Payroll Register"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Payroll Register"));
    else if (viewMode === "view") onSubViewChange(translate("View Payroll Register"));
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
    {
      id: "stxt",
      label: translate("Payroll Register"),
      minWidth: 250,
    },
    { id: "ltxt", label: translate("Description"), minWidth: 300 },
  ];

  const initialFormData = {
    code: "",
    stxt: "",
    ltxt: "",
  };

  const handleSave = async (formData: any) => {
    const errors = [];
    if (!CheckemptyString(formData.stxt)) {
      errors.push(translate("Payroll Register is required"));
    }

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = {
        id: formData.id,
        code: formData.code,
        stxt: formData.stxt,
        ltxt: formData.ltxt,
      };

      let saved: any;
      if (formData.id) {
        const response: any = await PutService(
          `/api/HumanResource/PayrollRegister/v3/${formData.id}`,
          payload,
        );
        saved = response?.data;
      } else {
        const response: any = await PostService(
          "/api/HumanResource/PayrollRegister/v3",
          payload,
        );
        saved = response?.data;
      }
      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? formData.id ?? 0);
      if (savedId > 0) {
        dispatch(payrollRegisterSlice.actions.upsertRow({ ...payload, id: savedId }));
      } else {
        dispatch(payrollRegisterSlice.fetchAll(true));
      }
      return true;
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to save payroll register";
      setToasterMessages([
        typeof message === "string" ? message : JSON.stringify(message),
      ]);
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
      await DeleteService(`/api/HumanResource/PayrollRegister/v3/${row.id}`);
      dispatch(payrollRegisterSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to delete payroll register";
      setToasterMessages([
        typeof message === "string" ? message : JSON.stringify(message),
      ]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);

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
              {/* Code (Read-only style as per image) */}
              <Grid item xs={12} md={12}>
                <Box sx={{ mb: 1 }}>
                  <Typography
                    sx={{
                      fontSize: "0.75rem",
                      fontWeight: 600,
                      color: "var(--lms-text-main)",
                      mb: 0.5,
                    }}
                  >
                    {translate("Code")}
                  </Typography>
                  <FormInput
                    fullWidth
                    size="small"
                    disabled
                    value={formData.code || ""}
                    placeholder={translate("Auto Generated")}
                    sx={{
                      "& .MuiInputBase-root": {
                        bgcolor: "#F1F5F9", // Lighter grey for read-only
                        borderRadius: "6px",
                      },
                    }}
                  />
                </Box>
              </Grid>

              {/* Payroll Register */}
              <Grid item xs={12} md={12}>
                <Box sx={{ mb: 1 }}>
                  <Typography
                    sx={{
                      fontSize: "0.75rem",
                      fontWeight: 600,
                      color: "var(--lms-text-main)",
                      mb: 0.5,
                    }}
                  >
                    {translate("Payroll Register")}*
                  </Typography>
                  <FormInput
                    fullWidth
                    size="small"
                    placeholder={translate("Enter Payroll Register")}
                    value={formData.stxt || ""}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        stxt: e.target.value,
                      })
                    }
                    disabled={isReadOnly}
                    error={isSubmitted && !formData.stxt}
                    helperText={
                      isSubmitted && !formData.stxt
                        ? translate("Payroll Register is empty")
                        : ""
                    }
                    sx={{
                      "& .MuiOutlinedInput-root": {
                        bgcolor: "#fff",
                        borderRadius: "12px",
                        "& fieldset": { borderColor: "#E2E8F0" },
                        "&:hover fieldset": { borderColor: "var(--lms-primary)" },
                        "&.Mui-focused fieldset": {
                          borderColor: "var(--lms-primary)",
                          borderWidth: "1.5px",
                        }, // Green focus as per image
                      },
                      "& input::placeholder": {
                        fontSize: "0.85rem",
                      },
                    }}
                  />
                </Box>
              </Grid>

              {/* Description */}
              <Grid item xs={12} md={12}>
                <Box sx={{ mb: 1 }}>
                  <Typography
                    sx={{
                      fontSize: "0.75rem",
                      fontWeight: 600,
                      color: "var(--lms-text-main)",
                      mb: 0.5,
                    }}
                  >
                    {translate("Description")}
                  </Typography>
                  <FormInput
                    fullWidth
                    size="small"
                    multiline
                    rows={2}
                    placeholder={translate("Enter Description")}
                    value={formData.ltxt || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, ltxt: e.target.value })
                    }
                    disabled={isReadOnly}
                    sx={{
                      "& .MuiOutlinedInput-root": {
                        bgcolor: "#fff",
                        borderRadius: "6px",
                        "& fieldset": { borderColor: "#E2E8F0" },
                        "&:hover fieldset": { borderColor: "var(--lms-primary)" },
                        "&.Mui-focused fieldset": {
                          borderColor: "var(--lms-primary)",
                        },
                      },
                      "& input::placeholder": {
                        fontSize: "0.85rem",
                      },
                    }}
                  />
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
              flexWrap: "wrap",
              gap: 1.5,
            }}
          >
            <Typography
              variant="h5"
              sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
            >
              {viewMode === "new"
                ? translate("New Payroll Register")
                : viewMode === "edit"
                  ? translate("Edit Payroll Register")
                  : translate("View Payroll Register")}
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
            {renderForm(
              formData,
              setFormData,
              viewMode,
              viewMode === "view",
              isSubmitted,
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Payroll Register")}
          addButtonLabel={translate("New Payroll Register")}
          breadcrumbPath={''}
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

export default PayrollRegister;

