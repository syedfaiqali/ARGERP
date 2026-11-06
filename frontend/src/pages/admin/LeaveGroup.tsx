import React, { useState, useEffect, useMemo } from "react";
import {
  Grid,
  Box,
  Typography,
  IconButton,
  Dialog,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  MenuItem,
  InputAdornment,
  Button,
  Collapse,
  Checkbox,
} from "@mui/material";
import {
  Search as SearchIcon,
  Close as CloseIcon,
  Add as AddIcon,
  Delete as DeleteIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { leaveGroupSlice } from "@/store/humanResourceSlices";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

interface LeaveGroupProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const initialFormData = {
  id: 0,
  code: "",
  stxt: "",
  adjustmentPolicyId: null,
  adjustmentPolicyName: "",
  details: [],
};

const LeaveGroup: React.FC<LeaveGroupProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the adjustment-policy and leave-type lookups come from the cache.
  const {
    rows: cachedRows,
    lookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrLeaveGroup", leaveGroupSlice.fetchAll);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({ ...initialFormData });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isDetailsExpanded, setIsDetailsExpanded] = useState(true);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [lookupRows, setLookupRows] = useState<any[]>([]);
  const [activeDetailIndex, setActiveDetailIndex] = useState<number | null>(null);
  const [selectedDetailRows, setSelectedDetailRows] = useState<number[]>([]);

  const [lookupData, setLookupData] = useState<any>({
    adjustmentPolicy: {
      title: translate("Leave Adjustment Policy"),
      subtitle: translate("Search / Leave Adjustment Policy"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Policy Name"), minWidth: 200 },
      ],
      rows: []
    },
    leaveType: {
      title: translate("Leave Type"),
      subtitle: translate("Search / Leave Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Leave Type"), minWidth: 200 },
      ],
      rows: []
    },
  });

  // RNW/LVUNT are global LOV reference data shared with LeaveType.
  const { lov } = useHrLovs(["RNW", "LVUNT"]);

  // The dialog rows are derived from the cached lists rather than refetched, so
  // the lookup structure (titles, columns) stays exactly as the dialogs expect.
  useEffect(() => {
    const adjItems = lookups?.adjustmentPolicies ?? [];
    const typeItems = lookups?.leaveTypes ?? [];
    const renewItems = lov("RNW");
    const leaveUnitItems = lov("LVUNT");
    const getLookupName = (items: any[], id: any) => {
      const item = items.find((value: any) => String(value.id ?? value.Id) === String(id ?? ""));
      return item?.name ?? item?.Name ?? item?.stxt ?? item?.Stxt ?? "";
    };

    setLookupData((prev: any) => ({
      ...prev,
      adjustmentPolicy: {
        ...prev.adjustmentPolicy,
        rows: adjItems.map((x: any) => ({
          id: x.id ?? x.Id,
          code: x.code ?? x.Code,
          shortName: x.stxt ?? x.Stxt ?? x.shortName ?? x.ShortName,
        }))
      },
      leaveType: {
        ...prev.leaveType,
        rows: typeItems.map((x: any) => ({
          id: x.id ?? x.Id,
          code: x.code ?? x.Code,
          shortName: x.stxt ?? x.Stxt ?? x.shortName ?? x.ShortName ?? x.leaveType ?? x.LeaveType,
          renewalType: getLookupName(renewItems, x.renewOn ?? x.RenewOn),
          leaveUnit: getLookupName(leaveUnitItems, x.leaveUnit ?? x.LeaveUnit),
        }))
      }
    }));
  }, [lookups, lov]);

  // sNo and the display aliases are derived here so the cache keeps raw records.
  const rows = useMemo<any[]>(
    () =>
      (cachedRows || []).map((x: any, index: number) => ({
        ...x,
        sNo: index + 1,
        leaveGroup: x.stxt || x.Stxt,
        adjustmentPolicy: x.adjustmentPolicyName || x.AdjustmentPolicyName,
      })),
    [cachedRows],
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Leave Group"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Leave Group"));
    else if (viewMode === "view") onSubViewChange(translate("View Leave Group"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleLookupOpen = async (lookupKey: string, index: number | null = null) => {
    setActiveLookup(lookupKey);
    setActiveDetailIndex(index);
    setLookupRows(lookupData[lookupKey].rows);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
    setActiveDetailIndex(null);
  };

  const LOOKUP_DATA: any = {
    adjustmentPolicy: {
      title: translate("Leave Adjustment Policy"),
      subtitle: translate("Search / Leave Adjustment Policy"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Policy Name"), minWidth: 200 },
      ],
    },
    leaveType: {
      title: translate("Leave Type"),
      subtitle: translate("Search / Leave Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "shortName", label: translate("Leave Type"), minWidth: 200 },
      ],
    },
  };

  const handleLookupSelect = (item: any) => {
    if (activeLookup === "adjustmentPolicy") {
      setFormData({
        ...formData,
        adjustmentPolicyId: item.id,
        adjustmentPolicyName: item.shortName,
      });
    } else if (activeLookup === "leaveType" && activeDetailIndex !== null) {
      applyLeaveTypeToDetail(activeDetailIndex, item);
    }
    handleLookupClose();
  };

  const applyLeaveTypeToDetail = (index: number, leaveType: any) => {
    const newDetails = [...formData.details];
    newDetails[index] = {
      ...newDetails[index],
      leaveTypeId: leaveType.id ?? leaveType.Id,
      leaveTypeName: leaveType.shortName ?? leaveType.leaveType ?? leaveType.LeaveType ?? "",
      leaveTypeCode: leaveType.code ?? leaveType.Code ?? "",
      renewalType: leaveType.renewalType ?? leaveType.RenewalType ?? "",
      leaveUnit: leaveType.leaveUnit ?? leaveType.LeaveUnit ?? "",
    };
    setFormData({ ...formData, details: newDetails });
  };

  const addDetailRow = () => {
    setFormData({
      ...formData,
      details: [
        ...formData.details,
        {
          id: 0,
          leaveTypeId: null,
          leaveTypeName: "",
          leaveTypeCode: "",
          renewalType: "",
          leaveUnit: "",
          leaveDays: "",
        },
      ],
    });
  };

  const removeDetailRow = (index: number) => {
    const newDetails = [...formData.details];
    newDetails.splice(index, 1);
    setFormData({ ...formData, details: newDetails });
  };

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = [];
    if (!formData.stxt) errors.push(translate("Leave Group is required"));
    if (!formData.adjustmentPolicyId) errors.push(translate("Leave Adjustment Policy is required"));
    formData.details.forEach((detail: any, index: number) => {
      const rowNo = index + 1;
      if (!detail.leaveTypeId) {
        errors.push(`${translate("Row")} ${rowNo}: ${translate("Leave Type is required")}`);
      }
      if (detail.leaveDays === "" || detail.leaveDays === null || detail.leaveDays === undefined) {
        errors.push(`${translate("Row")} ${rowNo}: ${translate("Leave Days is required")}`);
      } else if (!Number.isFinite(Number(detail.leaveDays))) {
        errors.push(`${translate("Row")} ${rowNo}: ${translate("Leave Days must be a valid number")}`);
      }
    });

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return;
    }

    setApiLoading(true);
    try {
      const payload = {
        ...formData,
        details: formData.details.map((d: any) => ({
          id: d.id || 0,
          leaveTypeId: d.leaveTypeId,
          leaveDays: Number(d.leaveDays),
        })),
      };

      if (formData.id > 0) {
        await PutService("/api/HumanResource/LeaveGroup/v3", payload);
        setErrorMessages([translate("Leave Group updated successfully")]);
      } else {
        await PostService("/api/HumanResource/LeaveGroup/v3", payload);
        setErrorMessages([translate("Leave Group saved successfully")]);
      }
      setToasterSeverity("success");
      setShowError(true);
      setViewMode("list");
      // The saved row carries the server-resolved adjustment policy name, so
      // refetch rather than reconstruct the cached shape from the form payload.
      dispatch(leaveGroupSlice.fetchAll(true));
    } catch (error: any) {
      const message = error?.response?.data || error?.message || "Failed to save record";
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
      await DeleteService(`/api/HumanResource/LeaveGroup/v3/${row.id}`);
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(leaveGroupSlice.actions.removeRow(row.id));
    } catch (error) {
      setErrorMessages([translate("Failed to delete record")]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
  };

  const renderLookupDialog = () => {
    if (!activeLookup || !lookupData[activeLookup]) return null;
    const data = lookupData[activeLookup];

    return (
      <Dialog
        open={!!activeLookup}
        onClose={handleLookupClose}
        maxWidth="lg"
        fullWidth
        PaperProps={{ sx: { borderRadius: "12px", overflow: "hidden" } }}
      >
        <Box sx={{ bgcolor: "#f0fdf4", px: 3, py: 2, display: "flex", alignItems: "center", justifyContent: "space-between" }}>
          <Typography sx={{ fontWeight: 800, fontSize: "1.1rem", color: "#166534" }}>{data.title}</Typography>
          <IconButton size="small" onClick={handleLookupClose} sx={{ bgcolor: "#fff", color: "#aaa" }}><CloseIcon fontSize="small" /></IconButton>
        </Box>
        <Box sx={{ px: 3, py: 1, bgcolor: "#fff" }}>
          <Typography variant="caption" sx={{ color: "var(--lms-primary)", fontWeight: 700 }}>{data.subtitle}</Typography>
        </Box>
        <Box sx={{ px: 3, py: 2 }}>
          <TableContainer sx={{ border: "1px solid #E2E8F0", borderRadius: "8px" }}>
            <Table size="small">
              <TableHead><TableRow sx={{ bgcolor: "#F8FAFC" }}>{data.columns.map((col: any) => (<TableCell key={col.id} sx={{ fontWeight: 700 }}>{col.label}</TableCell>))}</TableRow></TableHead>
              <TableBody>
                {lookupRows.map((row: any, idx: number) => (
                  <TableRow key={idx} hover sx={{ cursor: "pointer" }} onClick={() => handleLookupSelect(row)}>
                    {data.columns.map((col: any) => (<TableCell key={col.id}>{row[col.id]}</TableCell>))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Box>
      </Dialog>
    );
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
        position: "relative",
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
      {!expanded && (
        <Box
          sx={{
            width: "2px",
            height: 8,
            bgcolor: "var(--lms-primary)",
            position: "absolute",
          }}
        />
      )}
    </Box>
  );

  const renderForm = (isReadOnly: boolean) => {
    const lookupBtn = (key: string, index: number | null = null) => (
      <InputAdornment position="end">
        <IconButton size="small" onClick={(e) => {
          e.stopPropagation();
          handleLookupOpen(key, index);
        }} disabled={isReadOnly}>
          <SearchIcon sx={{ color: "var(--lms-primary)", fontSize: "1.2rem" }} />
        </IconButton>
      </InputAdornment>
    );

    return (
      <FormThemeWrapper>
        <Box sx={{ p: 0 }}>
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, cursor: "pointer", userSelect: "none" }} onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
              <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
              <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>Basic Information</T></Typography>
            </Box>
            {renderExpandIcon(isGeneralExpanded)}
          </Box>
          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}><FormInput fullWidth size="small" label={translate("Code")} disabled value={formData.code || ""} /></Grid>
              <Grid item xs={12}><FormInput fullWidth size="small" label={translate("Leave Group")} required disabled={isReadOnly} value={formData.stxt || ""} onChange={(e) => setFormData({ ...formData, stxt: e.target.value })} error={isSubmitted && !formData.stxt} /></Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  select
                  fullWidth
                  size="small"
                  label={translate("Leave Adjustment Policy")}
                  required
                  disabled={isReadOnly}
                  value={formData.adjustmentPolicyId || ""}
                  onChange={(e) => {
                    const selected = lookupData.adjustmentPolicy.rows.find((r: any) => r.id === e.target.value);
                    setFormData({
                      ...formData,
                      adjustmentPolicyId: e.target.value,
                      adjustmentPolicyName: selected?.shortName || ""
                    });
                  }}
                  error={isSubmitted && !formData.adjustmentPolicyId}
                  InputProps={{ endAdornment: lookupBtn("adjustmentPolicy") }}
                  SelectProps={{ IconComponent: () => null }}
                >
                  {lookupData.adjustmentPolicy.rows.map((row: any) => (
                    <MenuItem key={row.id} value={row.id}>{row.shortName}</MenuItem>
                  ))}
                </FormInput>
              </Grid>
            </Grid>
          </Collapse>

          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 3, mt: 3, cursor: "pointer", userSelect: "none", borderTop: "1px solid #E2E8F0", pt: 2 }} onClick={() => setIsDetailsExpanded(!isDetailsExpanded)}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
              <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
              <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}><T>Leave Types</T></Typography>
            </Box>
            {renderExpandIcon(isDetailsExpanded)}
          </Box>
          <Collapse in={isDetailsExpanded}>
            <Box sx={{ border: "1px solid #E2E8F0", borderRadius: "8px", overflow: "hidden" }}>
              {/* Toolbar */}
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  px: 1.5,
                  py: 1,
                  gap: 1,
                  bgcolor: "#FAFAFA",
                  borderBottom: "1px solid #E2E8F0",
                  justifyContent: "space-between",
                }}
              >
                <Box sx={{ display: "flex", gap: 1 }}>
                  <Button
                    size="small"
                    startIcon={<AddIcon />}
                    disabled={isReadOnly}
                    onClick={addDetailRow}
                    sx={{
                      textTransform: "none",
                      fontWeight: 700,
                      color: "var(--lms-primary)",
                      border: "1px solid var(--lms-primary)",
                      borderRadius: "6px",
                      px: 1.5,
                      "&:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.06)" },
                    }}
                  >
                    <T>Add</T>
                  </Button>
                  <Button
                    size="small"
                    startIcon={<DeleteIcon />}
                    disabled={isReadOnly || selectedDetailRows.length === 0}
                    onClick={() => {
                      const newDetails = formData.details.filter(
                        (_: any, i: number) => !selectedDetailRows.includes(i)
                      );
                      setFormData({ ...formData, details: newDetails });
                      setSelectedDetailRows([]);
                    }}
                    sx={{
                      textTransform: "none",
                      fontWeight: 700,
                      color: selectedDetailRows.length > 0 ? "#dc2626" : "#aaa",
                      border: `1px solid ${selectedDetailRows.length > 0 ? "#dc2626" : "#E2E8F0"}`,
                      borderRadius: "6px",
                      px: 1.5,
                      "&:hover": {
                        bgcolor: selectedDetailRows.length > 0 ? "rgba(220,38,38,0.05)" : "transparent",
                      },
                    }}
                  >
                    <T>Delete</T>
                  </Button>
                </Box>
                {selectedDetailRows.length > 0 && (
                  <Typography variant="caption" sx={{ color: "#6b7280", fontWeight: 600 }}>
                    {selectedDetailRows.length} {translate("selected")}
                  </Typography>
                )}
              </Box>

              {/* Table */}
              <TableContainer>
                <Table size="small" sx={{ tableLayout: "fixed", minWidth: 980 }}>
                  <TableHead>
                    <TableRow sx={{ bgcolor: "#F8FAFC" }}>
                      <TableCell padding="checkbox" sx={{ width: 48 }}>
                        <Checkbox
                          size="small"
                          indeterminate={
                            selectedDetailRows.length > 0 &&
                            selectedDetailRows.length < formData.details.length
                          }
                          checked={
                            formData.details.length > 0 &&
                            selectedDetailRows.length === formData.details.length
                          }
                          onChange={(e) => {
                            if (e.target.checked) {
                              setSelectedDetailRows(
                                formData.details.map((_: any, i: number) => i)
                              );
                            } else {
                              setSelectedDetailRows([]);
                            }
                          }}
                          disabled={isReadOnly || formData.details.length === 0}
                          sx={{ color: "var(--lms-primary)", "&.Mui-checked": { color: "var(--lms-primary)" } }}
                        />
                      </TableCell>
                      <TableCell sx={{ fontWeight: 700, width: 50, textAlign: "center" }}>
                        {translate("S.No")}
                      </TableCell>
                      <TableCell sx={{ fontWeight: 700, width: "25%" }}>{translate("Leave Type")}</TableCell>
                      <TableCell sx={{ fontWeight: 700, width: 260 }}>{translate("Renewal Type")}</TableCell>
                      <TableCell sx={{ fontWeight: 700, width: 140 }}>{translate("Leave Unit")}</TableCell>
                      <TableCell sx={{ fontWeight: 700, width: 130 }}>{translate("Leave Days")}</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {formData.details.map((detail: any, index: number) => {
                      const isSelected = selectedDetailRows.includes(index);
                      return (
                        <TableRow
                          key={index}
                          selected={isSelected}
                          sx={{
                            bgcolor: isSelected ? "rgba(var(--lms-primary-rgb), 0.04)" : "inherit",
                            "&.Mui-selected": { bgcolor: "rgba(var(--lms-primary-rgb), 0.06)" },
                          }}
                        >
                          <TableCell padding="checkbox">
                            <Checkbox
                              size="small"
                              checked={isSelected}
                              disabled={isReadOnly}
                              onChange={(e) => {
                                if (e.target.checked) {
                                  setSelectedDetailRows((prev) => [...prev, index]);
                                } else {
                                  setSelectedDetailRows((prev) =>
                                    prev.filter((i) => i !== index)
                                  );
                                }
                              }}
                              sx={{ color: "var(--lms-primary)", "&.Mui-checked": { color: "var(--lms-primary)" } }}
                            />
                          </TableCell>
                          <TableCell sx={{ textAlign: "center", color: "#6b7280", fontWeight: 600 }}>
                            {index + 1}
                          </TableCell>
                          <TableCell>
                            <FormInput
                              select
                              fullWidth
                              size="small"
                              value={detail.leaveTypeId || ""}
                              disabled={isReadOnly}
                              placeholder={translate("Select Leave Type")}
                              onChange={(e) => {
                                const selected = lookupData.leaveType.rows.find(
                                  (r: any) => String(r.id) === String(e.target.value)
                                );
                                applyLeaveTypeToDetail(index, selected ?? { id: e.target.value });
                              }}
                              InputProps={{ endAdornment: lookupBtn("leaveType", index) }}
                              SelectProps={{ IconComponent: () => null }}
                            >
                              {lookupData.leaveType.rows.map((row: any) => (
                                <MenuItem key={row.id} value={row.id}>
                                  {row.shortName}
                                </MenuItem>
                              ))}
                            </FormInput>
                          </TableCell>
                          <TableCell sx={{ whiteSpace: "normal" }}>
                            <Typography sx={{ color: "var(--lms-text-main)", fontWeight: 600, lineHeight: 1.4 }}>
                              {detail.renewalType || lookupData.leaveType.rows.find((row: any) =>
                                String(row.id) === String(detail.leaveTypeId)
                              )?.renewalType || "-"}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography sx={{ color: "var(--lms-text-main)", fontWeight: 600 }}>
                              {detail.leaveUnit || lookupData.leaveType.rows.find((row: any) =>
                                String(row.id) === String(detail.leaveTypeId)
                              )?.leaveUnit || "-"}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <FormInput
                              fullWidth
                              size="small"
                              type="number"
                              value={detail.leaveDays ?? ""}
                              disabled={isReadOnly}
                              onChange={(e) => {
                                const newDetails = [...formData.details];
                                newDetails[index].leaveDays = e.target.value;
                                setFormData({ ...formData, details: newDetails });
                              }}
                            />
                          </TableCell>
                        </TableRow>
                      );
                    })}
                    {formData.details.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={6} sx={{ textAlign: "center", py: 4, color: "#9ca3af" }}>
                          <Typography variant="body2">
                            <T>No leave types added. Click Add to get started.</T>
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Box>
          </Collapse>
        </Box>
      </FormThemeWrapper>
    );
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "leaveGroup", label: translate("Leave Group"), minWidth: 200 },
    { id: "adjustmentPolicy", label: translate("Leave Adjustment Policy"), minWidth: 250 },
  ];

  return (
    <Box sx={{ position: "relative" }}>
      {loading && <LmsOverlayLoader />}
      <LmsToaster open={showError} onClose={() => setShowError(false)} messages={errorMessages} severity={toasterSeverity} />
      {renderLookupDialog()}
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box sx={{ p: 2, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>{viewMode === "new" ? translate("New Leave Group") : viewMode === "edit" ? translate("Edit Leave Group") : translate("View Leave Group")}</Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={() => setViewMode("list")} sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}>{translate("Back to List")}</Button>
              {viewMode !== "view" && <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave} sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}>{translate("Save")}</Button>}
            </Box>
          </Box>
          <Box sx={{ px: 2, pb: 2 }}>{renderForm(viewMode === "view")}</Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Leave Group")}
          addButtonLabel={translate("New Leave Group")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          showAddButton={true}
          onAddClick={() => { setFormData({ ...initialFormData }); setIsSubmitted(false); setViewMode("new"); }}
          onEditClick={async (row: any) => {
            setApiLoading(true);
            try {
              const res: any = await GetService(`/api/HumanResource/LeaveGroup/v3/${row.id}`);
              setFormData(res.data);
              setIsSubmitted(false);
              setViewMode("edit");
            } catch (error) { console.error(error); } finally { setApiLoading(false); }
          }}
          onViewClick={async (row: any) => {
            setApiLoading(true);
            try {
              const res: any = await GetService(`/api/HumanResource/LeaveGroup/v3/${row.id}`);
              setFormData(res.data);
              setIsSubmitted(false);
              setViewMode("view");
            } catch (error) { console.error(error); } finally { setApiLoading(false); }
          }}
        />
      )}
    </Box>
  );
};

export default LeaveGroup;
