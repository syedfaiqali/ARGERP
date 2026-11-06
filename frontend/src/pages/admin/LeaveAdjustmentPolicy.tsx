import React, { useState, useEffect, useMemo } from "react";
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
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";

import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { FormInput } from "@/components/FormInput";
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { leaveAdjustmentPolicySlice } from "@/store/humanResourceSlices";

interface LeaveAdjustmentPolicyProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const LeaveAdjustmentPolicy: React.FC<LeaveAdjustmentPolicyProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const dispatch = useAppDispatch();
  // Rows plus the leave-type and attendance-policy lookups come from the cache.
  const {
    rows,
    lookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrLeaveAdjustmentPolicy", leaveAdjustmentPolicySlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">(
    "list"
  );
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "error"
  );
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isDetailsExpanded, setIsDetailsExpanded] = useState(true);
  const [isSubmitted, setIsSubmitted] = useState(false);

  // Lookups
  const leaveTypes = useMemo<any[]>(() => lookups?.leaveTypes ?? [], [lookups]);
  const attendancePolicies = useMemo<any[]>(
    () => lookups?.attendancePolicies ?? [],
    [lookups],
  );
  // ADJP/PRIT are global LOV reference data shared across HR screens.
  const { lov } = useHrLovs(["ADJP", "PRIT"]);
  const adjustmentTypes = lov("ADJP");
  const priorities = lov("PRIT");
  const [detailLookup, setDetailLookup] = useState<{
    type: "basedOn" | "priority";
    data: any;
    setData: (data: any) => void;
  } | null>(null);



  const [formData, setFormData] = useState<any>({
    code: "",
    stxt: "",
    details: [],
  });



  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new")
      onSubViewChange(translate("New Leave Adjustment Policy"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Leave Adjustment Policy"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Leave Adjustment Policy"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const attendancePolicyAdjustmentType = useMemo(() => {
    return adjustmentTypes.find((item: any) =>
      String(item.code ?? item.name ?? item.stxt ?? "").toLowerCase().includes("attendance")
    ) ?? adjustmentTypes.find((item: any) => Number(item.id) === 738) ?? null;
  }, [adjustmentTypes]);

  const renderDetailLookupModal = () => {
    if (!detailLookup) return null;

    const isBasedOn = detailLookup.type === "basedOn";
    const options = isBasedOn ? attendancePolicies : priorities;
    const title = isBasedOn ? translate("Attendance Policy") : translate("Priority");
    const selectedValue = isBasedOn
      ? detailLookup.data.basedOnId ?? ""
      : detailLookup.data.priorityId ?? "";

    return (
      <GenericSearchModal
        open
        onClose={() => setDetailLookup(null)}
        title={title}
        columns={[
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: title, minWidth: 220 },
          { id: "description", label: translate("Description"), minWidth: 220 },
        ]}
        data={(options || []).map((item: any) => ({
          ...item,
          name: item.name ?? item.stxt ?? item.code ?? "",
          description: item.description ?? item.ltxt ?? "",
        }))}
        onSelect={(row) => {
          detailLookup.setData({
            ...detailLookup.data,
            ...(isBasedOn
              ? { basedOnId: row.id, basedOnName: row.name ?? row.stxt ?? row.code ?? "" }
              : { priorityId: row.id, priorityName: row.name ?? row.stxt ?? row.code ?? "" }),
          });
          setDetailLookup(null);
        }}
        selectedValue={selectedValue}
        selectedField="id"
      />
    );
  };

  const handleFetchById = async (id: number, mode: "edit" | "view") => {
    setApiLoading(true);
    try {
      const response: any = await GetService(
        `/api/HumanResource/LeaveAdjustmentPolicy/v3/${id}`
      );
      setFormData(response.data);
      setViewMode(mode);
    } catch (error: any) {
      console.error("Failed to fetch policy details", error);
      setToasterMessages([translate("Failed to fetch policy details")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setApiLoading(false);
    }
  };

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = [];
    if (!CheckemptyString(formData.stxt))
      errors.push(translate("Policy Name is required"));

    const activeDetails = formData.details.filter((d: any) => !d.delete);
    if (activeDetails.length === 0) {
      errors.push(translate("At least one detail record is required"));
    }

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return;
    }

    setApiLoading(true);
    try {
      if (formData.id) {
        await PutService("/api/HumanResource/LeaveAdjustmentPolicy/v3", formData);
      } else {
        await PostService("/api/HumanResource/LeaveAdjustmentPolicy/v3", formData);
      }
      setToasterMessages([translate("Record saved successfully")]);
      setToasterSeverity("success");
      setShowToaster(true);
      setViewMode("list");
      dispatch(leaveAdjustmentPolicySlice.fetchAll(true));
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to save record";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      await DeleteService(
        `/api/HumanResource/LeaveAdjustmentPolicy/v3/${row.id}`
      );
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(leaveAdjustmentPolicySlice.fetchAll(true));
    } catch (error: any) {
      const data = error?.response?.data;
      const message = typeof data === "string"
        ? data
        : data?.error || data?.title || error?.message || "Failed to delete record";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setApiLoading(false);
    }
  };



  const handleDetailSave = (detailData: any) => {
    const newDetails = [...formData.details];
    if (detailData.tempId !== undefined) {
      // Edit existing temp detail or existing detail
      const idx = newDetails.findIndex((d: any, i: number) =>
        (d.id > 0 && d.id === detailData.id) || (d.id === 0 && i === detailData.tempId)
      );
      if (idx !== -1) {
        newDetails[idx] = { ...detailData };
      }
    } else {
      // Add new detail
      newDetails.push({ ...detailData, id: 0, delete: false });
    }
    setFormData({ ...formData, details: newDetails });
    return true;
  };

  const handleDetailDelete = (detailRow: any) => {
    const newDetails = [...formData.details];
    const idx = newDetails.findIndex((d: any, i: number) =>
      (d.id > 0 && d.id === detailRow.id) || (d.id === 0 && d.lineIndex === detailRow.lineIndex && i === detailRow.tempId)
    );

    if (idx !== -1) {
      if (newDetails[idx].id > 0) {
        newDetails[idx].delete = true;
      } else {
        newDetails.splice(idx, 1);
      }
      setFormData({ ...formData, details: newDetails });
    }
  };

  const detailColumns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "leaveTypeName", label: translate("Leave Type"), minWidth: 150 },
    { id: "adjTypeName", label: translate("Adjustment Type"), minWidth: 150 },
    { id: "basedOnName", label: translate("Based On"), minWidth: 150 },
    { id: "priorityName", label: translate("Priority"), minWidth: 120 },
    { id: "days", label: translate("Days"), minWidth: 80 },
  ];

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

  const renderDetailForm = (
    data: any,
    setData: any,
    _mode: any,
    isReadOnly?: boolean,
    isSubmitted?: boolean
  ) => {
    return (
      <FormThemeWrapper>
        <Grid container spacing={3} sx={{ p: 2 }}>
          <Grid item xs={12} md={6}>
            <LookupDropdown
              label={translate("Leave Type")}
              value={data.leaveTypeId || ""}
              options={leaveTypes}
              getOptionLabel={(opt) => opt.leaveType || opt.stxt || ""}
              onSelect={(row) => {
                setData({
                  ...data,
                  leaveTypeId: row.id,
                  leaveTypeName: row.leaveType || row.stxt || ""
                });
              }}
              required
              error={isSubmitted && !data.leaveTypeId}
              disabled={isReadOnly}
              placeholder={translate("Select Leave Type")}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <LookupDropdown
              label={translate("Adjustment Type")}
              value={data.adjTypeId || attendancePolicyAdjustmentType?.id || ""}
              options={adjustmentTypes}
              getOptionLabel={(opt) => opt.name || opt.stxt || ""}
              onSelect={() => {}}
              disabled
              placeholder={translate("Attendance Policy")}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <LookupDropdown
              label={translate("Based On")}
              value={data.basedOnId || ""}
              options={attendancePolicies}
              getOptionLabel={(opt) => opt.stxt || opt.name || ""}
              onSelect={(row) => {
                setData({
                  ...data,
                  basedOnId: row.id,
                  basedOnName: row.stxt || row.name || ""
                });
              }}
              onSearchClick={() => setDetailLookup({ type: "basedOn", data, setData })}
              disabled={isReadOnly}
              hideDropdownArrow
              placeholder={translate("Select Attendance Policy")}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <LookupDropdown
              label={translate("Priority")}
              value={data.priorityId || ""}
              options={priorities}
              getOptionLabel={(opt) => opt.name || opt.stxt || ""}
              onSelect={(row) => {
                setData({
                  ...data,
                  priorityId: row.id,
                  priorityName: row.name || row.stxt || ""
                });
              }}
              onSearchClick={() => setDetailLookup({ type: "priority", data, setData })}
              disabled={isReadOnly}
              hideDropdownArrow
              placeholder={translate("Select Priority")}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <FormInput
              fullWidth
              size="small"
              type="number"
              label={translate("Days")}
              value={data.days || 0}
              onChange={(e) => setData({ ...data, days: parseInt(e.target.value) || 0 })}
              required
              error={isSubmitted && (!data.days || data.days <= 0)}
              disabled={isReadOnly}
            />
          </Grid>
        </Grid>
      </FormThemeWrapper>
    );
  };


  const renderForm = () => {
    const isReadOnly = viewMode === "view";
    const activeDetails = formData.details
      .map((d: any, index: number) => ({ ...d, tempId: index }))
      .filter((d: any) => !d.delete);

    return (
      <FormThemeWrapper>
        <Box
          sx={{
            p: 0,
            pointerEvents: isReadOnly ? "none" : "auto",
            opacity: isReadOnly ? 0.9 : 1,
          }}
        >
          {/* Header Section */}
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
            <Grid container spacing={3} sx={{ mb: 4 }}>
              <Grid item xs={12} md={4}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Policy ID")}
                  value={formData.code || ""}
                  disabled={true}
                  placeholder={translate("Auto Generated")}
                />
              </Grid>
              <Grid item xs={12} md={8}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Policy Name")}
                  placeholder={translate("Enter Policy Name")}
                  value={formData.stxt || ""}
                  onChange={(e) =>
                    setFormData({ ...formData, stxt: e.target.value })
                  }
                  required
                  error={isSubmitted && !formData.stxt}
                  disabled={isReadOnly}
                />
              </Grid>
            </Grid>
          </Collapse>

          {/* Details Section Using SchemaTable */}
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
            onClick={() => setIsDetailsExpanded(!isDetailsExpanded)}
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
                <T>Policy Details</T>
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
              {renderExpandIcon(isDetailsExpanded)}
            </Box>
          </Box>

          <Collapse in={isDetailsExpanded}>
            <Box sx={{ mb: 2 }}>
              <SchemaTable
                title={translate("Policy Details")}
                columns={detailColumns}
                rows={activeDetails}
                onSave={handleDetailSave}
                onDelete={handleDetailDelete}
                initialFormData={{
                  id: 0,
                  leaveTypeId: null,
                  leaveTypeName: "",
                  adjTypeId: attendancePolicyAdjustmentType?.id || 738,
                  adjTypeName: attendancePolicyAdjustmentType?.name || attendancePolicyAdjustmentType?.stxt || "Attendance Policy",
                  basedOnId: null,
                  basedOnName: "",
                  priorityId: null,
                  priorityName: "",
                  days: 0,
                  delete: false
                }}
                renderForm={renderDetailForm}
                hideTitle={true}
                hideBreadcrumbs={true}
                showAddButton={!isReadOnly}
                showEditAction={!isReadOnly}
                actions={!isReadOnly}
                maxWidth="md"
                mandatoryFields={[
                  { key: "leaveTypeId", label: translate("Leave Type") },
                ]}
                showToaster={false}
              />
            </Box>
          </Collapse>
        </Box>
      </FormThemeWrapper>
    );
  };

  const listColumns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "code", label: translate("Policy ID"), minWidth: 120 },
    { id: "stxt", label: translate("Policy Name"), minWidth: 250 },
  ];

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
                ? translate("New Leave Adjustment Policy")
                : viewMode === "edit"
                  ? translate("Edit Leave Adjustment Policy")
                  : translate("View Leave Adjustment Policy")}
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
                  onClick={handleSave}
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
          <Box sx={{ px: 2, pb: 2 }}>{renderForm()}</Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Leave Adjustment Policy")}
          addButtonLabel={translate("New")}
          columns={listColumns}
          rows={rows}
          onDelete={handleDelete}
          maxWidth="lg"
          onAddClick={() => {
            setFormData({ code: "", stxt: "", details: [] });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => handleFetchById(row.id, "edit")}
          onViewClick={(row: any) => handleFetchById(row.id, "view")}
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
      {renderDetailLookupModal()}
      {loading && <LmsOverlayLoader />}
    </>
  );
};

export default LeaveAdjustmentPolicy;
