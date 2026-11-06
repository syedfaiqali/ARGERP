import React, { useState, useEffect } from "react";
import {
  Grid,
  Box,
  Typography,
  Collapse,
  Button,
  Dialog,
} from "@mui/material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
} from "@/constants/globalItems";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { attendanceActivitySlice } from "@/store/humanResourceSlices";
import {
  ArrowBack as ArrowBackIcon,
  Close as CloseIcon,
  Save as SaveIcon,
} from "@mui/icons-material";

interface AttendanceActivityProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const AttendanceActivity: React.FC<AttendanceActivityProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the activity-type lookup both come from the cache.
  const {
    rows,
    lookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrAttendanceActivity", attendanceActivitySlice.fetchAll);
  const activityTypes = React.useMemo<any[]>(
    () =>
      (lookups?.activityTypes ?? []).map((item: any) => ({
        id: item.id,
        name: item.activity ?? item.Activity ?? "",
      })),
    [lookups],
  );
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [lookupData, setLookupData] = useState<any[]>([]);
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
    orderId: "",
    activity: "",
    value: "",
  });
  const [lookupSearch, setLookupSearch] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Attendance Activity"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Attendance Activity"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Attendance Activity"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleLookupOpen = () => {
    setLookupData(
      activityTypes.map((item: any) => ({
        id: item.id,
        name: item.name,
      })),
    );
    setActiveLookup("activityType");
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  const handleLookupRowClick = (row: any) => {
    setFormData({
      ...formData,
      value: row.id,
      valueName: row.name,
    });
    handleLookupClose();
  };

  const filteredLookupData = lookupData.filter((row: any) => {
    if (!lookupSearch) return true;
    return String(row.name ?? "").toLowerCase().includes(
      lookupSearch.trim().toLowerCase(),
    );
  });

  const renderLookupDialog = () => {
    if (!activeLookup) return null;

    return (
      <Dialog
        open={!!activeLookup}
        onClose={handleLookupClose}
        maxWidth="lg"
        fullWidth
      >
        <Box sx={{ bgcolor: "#f3faf4", borderBottom: "1px solid #d1e7dd" }}>
          <Box
            sx={{
              px: 4,
              py: 3,
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Box>
              <Typography sx={{ fontWeight: 800, fontSize: "1.1rem" }}>
                {translate("Activity Type")}
              </Typography>
              <Typography variant="caption" sx={{ color: "#4b5563" }}>
                {translate("Select an activity type from the list")}
              </Typography>
            </Box>
            <Button
              onClick={handleLookupClose}
              sx={{
                minWidth: "auto",
                width: 36,
                height: 36,
                borderRadius: "50%",
                color: "#1f2937",
                bgcolor: "rgba(0,0,0,0.04)",
                p: 0,
              }}
            >
              <CloseIcon fontSize="small" />
            </Button>
          </Box>
        </Box>
        <Box sx={{ px: 4, py: 3, display: "flex", alignItems: "center", gap: 2, bgcolor: "#fff" }}>
          <FormInput
            size="small"
            placeholder={translate("Search")}
            value={lookupSearch}
            onChange={(e) => setLookupSearch(e.target.value)}
            sx={{ width: 300, maxWidth: "100%" }}
          />
          <Typography sx={{ fontWeight: 700, color: "#1f2937" }}>
            {translate("TOTAL RECORDS :")} {filteredLookupData.length}
          </Typography>
        </Box>
        <Box sx={{ p: 2, bgcolor: "#fff" }}>
          <SchemaTable
            title={translate("Activity Type")}
            columns={lookupColumns}
            rows={filteredLookupData}
            onRowClick={handleLookupRowClick}
            actions={false}
            pagination={false}
            filtersEnabled={false}
            downloadEnabled={false}
            showAddButton={false}
            showMenuAction={false}
            showToaster={false}
            hideTitle={true}
            hideBreadcrumbs={true}
            maxHeight={400}
          />
        </Box>
      </Dialog>
    );
  };

  const columns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "orderId", label: translate("Sequence No"), minWidth: 120 },
    { id: "activity", label: translate("Activity"), minWidth: 200 },
    {
      id: "value",
      label: translate("Activity Type"),
      minWidth: 200,
      format: (value: any) =>
        activityTypes.find((opt) => String(opt.id) === String(value))?.name ||
        String(value ?? ""),
    },
  ];

  const lookupColumns: Column[] = [
    {
      id: "sNo",
      label: "#",
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    {
      id: "activity",
      label: translate("Activity Type"),
      minWidth: 200,
    },
  ];

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = [];
    if (!CheckemptyString(formData.activity))
      errors.push(translate("Activity is required"));

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return;
    }

    const dataToSend = {
      ...formData,
      orderId: formData.orderId ? parseInt(formData.orderId) : null,
      value: formData.value ? parseInt(formData.value) : null,
    };
    if (!formData.id) {
      delete dataToSend.id;
    }

    setApiLoading(true);
    try {
      const response: any = formData.id
        ? await workflowMutation("put", `/api/HumanResource/AttendanceActivity/v3/${formData.id}`, dataToSend)
        : await workflowMutation("post", "/api/HumanResource/AttendanceActivity/v3", dataToSend);
      // Sent for approval: the record itself does not change yet, so the list is left as it is.
      if (pendingWorkflow(response)) {
        setViewMode("list");
        return;
      }
      const saved: any = response?.data;
      setErrorMessages([
        formData.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowError(true);
      // Patch the cached row instead of reloading the whole list.
      {
        const savedId = Number(saved?.id ?? formData.id ?? 0);
        if (savedId > 0) {
          dispatch(
            attendanceActivitySlice.actions.upsertRow({ ...dataToSend, id: savedId }),
          );
        } else {
          dispatch(attendanceActivitySlice.fetchAll(true));
        }
      }
      setViewMode("list");
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save attendance activity");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      const response = await workflowMutation("delete", `/api/HumanResource/AttendanceActivity/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(attendanceActivitySlice.actions.removeRow(row.id));
    } catch (error: any) {
      console.error(error);
      setErrorMessages([translate("Failed to delete attendance activity")]);
      setToasterSeverity("error");
      setShowError(true);
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
                  label={translate("Sequence No")}
                  placeholder={translate("Enter Sequence No")}
                  value={localFormData.orderId || ""}
                  onChange={(e) =>
                    setLocalFormData({ ...localFormData, orderId: e.target.value })
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Activity")}
                  placeholder={translate("Enter Activity")}
                  value={localFormData.activity || ""}
                  onChange={(e) =>
                    setLocalFormData({ ...localFormData, activity: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !localFormData.activity}
                  required
                />
              </Grid>

              <Grid item xs={12}>
                <LookupDropdown
                  label={translate("Activity Type")}
                  placeholder={translate("Select Activity Type")}
                  value={localFormData.value ?? ""}
                  options={activityTypes}
                  getOptionLabel={(option) => option.name}
                  onSelect={(option) =>
                    setLocalFormData({
                      ...localFormData,
                      value: option.id,
                      valueName: option.name,
                    })
                  }
                  onSearchClick={handleLookupOpen}
                  required
                  error={isSubmitted && !localFormData.value}
                  helperText={
                    isSubmitted && !localFormData.value
                      ? translate("Activity Type is required")
                      : ""
                  }
                  disabled={isReadOnly}
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
                ? translate("New Attendance Activity")
                : viewMode === "edit"
                ? translate("Edit Attendance Activity")
                : translate("View Attendance Activity")}
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
                    await handleSave();
                    setIsSubmitted(false);
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
        title={translate("Attendance Activity")}
        addButtonLabel={translate("New")}
        columns={columns}
        rows={rows}
        onDelete={handleDelete}
        maxWidth="lg"
        onAddClick={() => {
          setFormData({ id: 0, orderId: "", activity: "", value: "" });
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
    );
  };

  return (
    <>
      {renderContent()}
      {renderLookupDialog()}
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

export default AttendanceActivity;

