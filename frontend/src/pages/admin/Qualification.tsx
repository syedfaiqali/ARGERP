import React, { useState, useEffect } from "react";
import { Grid, Box, Typography, IconButton, MenuItem, InputAdornment, Collapse, Button } from "@mui/material";
import {
  Search as SearchIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { CheckemptyString } from "@/constants/globalItems";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { qualificationSlice } from "@/store/humanResourceSlices";

interface QualificationProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const normalizeQualificationType = (item: any) => ({
  id: Number(item.id ?? item.Id ?? 0),
  code: item.code ?? item.Code ?? "",
  name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? "",
  description: item.description ?? item.Description ?? item.ltxt ?? item.Ltxt ?? "",
  isActive: item.isActive ?? item.IsActive ?? true,
});

const Qualification: React.FC<QualificationProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const dispatch = useAppDispatch();
  // Rows and the qualification-type lookup both come from the cache.
  const {
    rows,
    lookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrQualification", qualificationSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const qualificationTypes = React.useMemo<any[]>(
    () =>
      (lookups?.qualificationTypes ?? [])
        .map(normalizeQualificationType)
        .filter((item: any) => item.id > 0),
    [lookups],
  );
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const { translate } = useTranslation();
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    code: "",
    qualificationTypeId: null,
    qualificationTypeName: "",
    qualification: "",
    description: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Qualification"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Qualification"));
    else if (viewMode === "view") onSubViewChange(translate("View Qualification"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);


  const handleLookupOpen = (e: React.MouseEvent, lookupKey: string) => {
    e.stopPropagation();
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  const renderLookupDialog = (formData: any, setFormData: any) => {
    if (!activeLookup) return null;

    const lookupTitle = translate("Qualification Type");
    const lookupColumns = [
      { id: "code", label: translate("Code"), minWidth: 80 },
      { id: "name", label: translate("Qualification Type"), minWidth: 200 },
      { id: "description", label: translate("Description"), minWidth: 250 },
    ];

    return (
      <GenericSearchModal
        open={!!activeLookup}
        onClose={handleLookupClose}
        title={lookupTitle}
        columns={lookupColumns}
        data={qualificationTypes}
        selectedValue={formData.qualificationTypeId}
        selectedField="id"
        onSelect={(selectedRow: any) => {
          setFormData({
            ...formData,
            qualificationTypeId: selectedRow?.id ?? null,
            qualificationTypeName: selectedRow?.name ?? "",
          });
          handleLookupClose();
        }}
      />
    );
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    {
      id: "qualificationTypeName",
      label: translate("Qualification Type"),
      minWidth: 200,
    },
    { id: "qualification", label: translate("Qualification"), minWidth: 200 },
    { id: "description", label: translate("Description"), minWidth: 250 },
  ];

  const initialFormData = {
    code: "",
    qualificationTypeId: null,
    qualificationTypeName: "",
    qualification: "",
    description: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code)) errors.push(translate("Code is required"));
    if (!CheckemptyString(data.qualification)) errors.push(translate("Qualification is required"));
    if (!data.qualificationTypeId) errors.push(translate("Qualification Type is required"));

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
        ? await workflowMutation("put", `/api/HumanResource/Qualification/v3/${data.id}`, data)
        : await workflowMutation("post", "/api/HumanResource/Qualification/v3", data);
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
          dispatch(qualificationSlice.actions.upsertRow({ ...data, id: savedId }));
        } else {
          dispatch(qualificationSlice.fetchAll(true));
        }
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save qualification");
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
      const response = await workflowMutation("delete", `/api/HumanResource/Qualification/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(qualificationSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete qualification");
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
                <T>Qualification Information</T>
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
                  select
                  fullWidth
                  size="small"
                  label={translate("Qualification Type")}
                  value={data.qualificationTypeId || ""}
                  onChange={(e) => {
                    const selected = qualificationTypes.find(t => Number(t.id) === Number(e.target.value));
                    setData({
                      ...data,
                      qualificationTypeId: Number(e.target.value),
                      qualificationTypeName: selected?.name || ""
                    });
                  }}
                  disabled={isReadOnly}
                  error={isSubmitted && !data.qualificationTypeId}
                  required
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          size="small"
                          sx={{ cursor: "pointer" }}
                          onClick={(e) =>
                            handleLookupOpen(e, "qualificationType")
                          }
                        >
                          <SearchIcon
                            sx={{
                              color: "var(--lms-primary)",
                              fontSize: "1.2rem",
                            }}
                          />
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                  SelectProps={{
                    IconComponent: () => null,
                  }}
                >
                  {qualificationTypes.map(type => (
                    <MenuItem key={type.id} value={type.id}>{type.name}</MenuItem>
                  ))}
                </FormInput>
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Qualification")}
                  placeholder={translate("Enter Qualification")}
                  value={data.qualification || ""}
                  onChange={(e) =>
                    setData({ ...data, qualification: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.qualification}
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
                  label={translate("Mark as Active Qualification")}
                />
              </Grid>
            </Grid>
          </Collapse>
        </Box>
        {renderLookupDialog(data, setData)}
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
                ? translate("New Qualification")
                : viewMode === "edit"
                  ? translate("Edit Qualification")
                  : translate("View Qualification")}
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
          title={translate("Qualification")}
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

export default Qualification;
