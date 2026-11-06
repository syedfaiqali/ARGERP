import React, { useState, useEffect } from "react";
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
  Collapse,
  Button,
  Paper,
} from "@mui/material";
import {
  Search as SearchIcon,
  Close as CloseIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { CheckemptyString } from "@/constants/globalItems";
import { apiErrorMessage, saveRecord } from '@/services/mutations';
import { useAppDispatch, useCachedRows } from "@/hooks";
import { positionTypeSlice } from "@/store/humanResourceSlices";

interface PositionTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const PositionType: React.FC<PositionTypeProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the employee-type lookup both come from the cache.
  const {
    rows,
    lookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrPositionType", positionTypeSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;

  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [lookupData, setLookupData] = useState<any[]>([]);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");

  const employeeTypes = React.useMemo<any[]>(() => lookups?.employeeTypes ?? [], [lookups]);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    code: "",
    employeeTypeId: null,
    employeeTypeName: "",
    name: "",
    description: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Position Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Position Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Position Type"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleLookupOpen = (e: React.MouseEvent, lookupKey: string) => {
    e.stopPropagation();
    if (lookupKey === "employeeType") {
      setLookupData(
        employeeTypes.map((et) => ({
          id: et.id,
          name: et.name,
        }))
      );
    }
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  const LOOKUP_CONFIG: any = {
    employeeType: {
      title: "Employee Type",
      subtitle: "Search / Employee Type",
      columns: [
        { id: "id", label: "ID", minWidth: 100 },
        { id: "name", label: "Employee Type", minWidth: 200 },
      ],
    },
  };

  const renderLookupDialog = (formData: any, setFormData: any) => {
    if (!activeLookup || !LOOKUP_CONFIG[activeLookup]) return null;

    return (
      <Dialog
        open={!!activeLookup}
        onClose={handleLookupClose}
        maxWidth="lg"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: "12px",
            overflow: "hidden",
            boxShadow: "0 10px 40px rgba(0,0,0,0.1)",
          },
        }}
      >
        <Box
          sx={{
            bgcolor: "#f0fdf4",
            px: 3,
            py: 2,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <Typography
            sx={{ fontWeight: 800, fontSize: "1.1rem", color: "#166534" }}
          >
            {LOOKUP_CONFIG[activeLookup].title}
          </Typography>
          <IconButton
            size="small"
            onClick={handleLookupClose}
            sx={{
              bgcolor: "#fff",
              color: "#aaa",
              "&:hover": { bgcolor: "#f5f5f5" },
            }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>

        <Box sx={{ px: 3, py: 1, bgcolor: "#fff" }}>
          <Typography
            variant="caption"
            sx={{
              color: "var(--lms-primary)",
              fontWeight: 700,
              fontSize: "0.75rem",
            }}
          >
            {LOOKUP_CONFIG[activeLookup].subtitle}
          </Typography>
        </Box>

        <Box sx={{ p: 3, bgcolor: "#fff" }}>
          <TableContainer
            sx={{ border: "1px solid #E2E8F0", borderRadius: "8px" }}
          >
            <Table size="small">
              <TableHead>
                <TableRow sx={{ bgcolor: "#F8FAFC" }}>
                  {LOOKUP_CONFIG[activeLookup].columns.map((col: any) => (
                    <TableCell
                      key={col.id}
                      sx={{
                        fontWeight: 700,
                        fontSize: "0.85rem",
                        color: "#64748B",
                        borderRight: "1px solid #E2E8F0",
                        py: 1,
                      }}
                    >
                      | {col.label}
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                {lookupData.map((row: any, idx: number) => (
                  <TableRow
                    key={idx}
                    hover
                    sx={{
                      cursor: "pointer",
                      "&:hover": { bgcolor: "#f0fdf4" },
                    }}
                    onClick={() => {
                      if (activeLookup === "employeeType") {
                        setFormData({
                          ...formData,
                          employeeTypeId: row.id,
                          employeeTypeName: row.name,
                        });
                      }
                      handleLookupClose();
                    }}
                  >
                    {LOOKUP_CONFIG[activeLookup].columns.map((col: any) => (
                      <TableCell
                        key={col.id}
                        sx={{
                          borderBottom: "1px solid #E2E8F0",
                          borderRight: "1px solid #E2E8F0",
                          fontSize: "0.85rem",
                          color: "#1E293B",
                          py: 1,
                        }}
                      >
                        {row[col.id]}
                      </TableCell>
                    ))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          <Box sx={{ mt: 2 }}>
            <Typography
              variant="caption"
              sx={{ fontWeight: 800, color: "#166534" }}
            >
              TOTAL RECORDS : {lookupData.length}
            </Typography>
          </Box>
        </Box>
      </Dialog>
    );
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "employeeTypeName", label: translate("Employee Type"), minWidth: 150 },
    { id: "name", label: translate("Position Type"), minWidth: 200 },
    { id: "description", label: translate("Description"), minWidth: 250 },
  ];

  const initialFormData = {
    code: "",
    employeeTypeId: null,
    employeeTypeName: "",
    name: "",
    description: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code?.trim())) errors.push(translate("Code is required"));
    if (!CheckemptyString(data.employeeTypeId)) errors.push(translate("Employee Type is required"));
    if (!CheckemptyString(data.name?.trim())) errors.push(translate("Position Type is required"));

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
        ? await saveRecord("put", `/api/HumanResource/PositionType/v3/${data.id}`, payload)
        : await saveRecord("post", "/api/HumanResource/PositionType/v3", payload);
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
        dispatch(positionTypeSlice.actions.upsertRow({ ...payload, id: savedId }));
      } else {
        dispatch(positionTypeSlice.fetchAll(true));
      }
      return true;
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to save position type");
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
      const response = await saveRecord("delete", `/api/HumanResource/PositionType/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(positionTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = apiErrorMessage(error, "Failed to delete position type");
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
    isSubmitted?: boolean
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
                <T>Position Type Details</T>
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
                  label={translate("Employee Type")}
                  value={data.employeeTypeId || ""}
                  onChange={(e) => {
                    const selected = employeeTypes.find(
                      (et) => et.id === e.target.value
                    );
                    setData({
                      ...data,
                      employeeTypeId: e.target.value,
                      employeeTypeName: selected ? selected.name : "",
                    });
                  }}
                  disabled={isReadOnly}
                  error={isSubmitted && !data.employeeTypeId}
                  required
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          size="small"
                          sx={{ cursor: "pointer" }}
                          onClick={(e) => handleLookupOpen(e, "employeeType")}
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
                  {employeeTypes.map((et) => (
                    <MenuItem key={et.id} value={et.id}>
                      {et.name}
                    </MenuItem>
                  ))}
                </FormInput>
              </Grid>

              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Position Type")}
                  placeholder={translate("Enter Position Type")}
                  value={data.name || ""}
                  onChange={(e) =>
                    setData({ ...data, name: e.target.value })
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
                  label={translate("Mark as Active Position Type")}
                />
              </Grid>
            </Grid>
            {renderLookupDialog(data, setData)}
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
                ? translate("New Position Type")
                : viewMode === "edit"
                  ? translate("Edit Position Type")
                  : translate("View Position Type")}
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
          title={translate("Position Type")}
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

export default PositionType;

