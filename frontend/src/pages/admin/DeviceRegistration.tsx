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
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsToaster } from "@/components/LmsToaster";
import { FormInput } from "@/components/FormInput";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
} from "@/constants/globalItems";
import { apiErrorMessage, pendingWorkflow, workflowMutation } from "@/workflow/api";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { deviceRegistrationSlice } from "@/store/humanResourceSlices";

interface DeviceRegistrationProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const DeviceRegistration: React.FC<DeviceRegistrationProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows plus the device/site/connectivity lookups all come from the cache.
  const {
    rows,
    lookups: cachedLookups,
    loading: rowsLoading,
  } = useCachedRows<any>("hrDeviceRegistration", deviceRegistrationSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isMachineExpanded, setIsMachineExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">(
    "list",
  );

  // The lookups endpoint returns one payload holding all three lists.
  const lookups = React.useMemo<any>(
    () =>
      (cachedLookups?.all ?? [])[0] ?? {
        deviceTypes: [],
        sites: [],
        connectivityTypes: [],
      },
    [cachedLookups],
  );

  const [formData, setFormData] = useState<any>({
    id: 0,
    code: "",
    deviceTypeId: 0,
    deviceName: "",
    siteId: 0,
    deviceIp: "",
    machinePort: "",
    connectivityTypeId: 0,
    deviceId: 0,
    isActive: true,
  });

  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new")
      onSubViewChange(translate("New Device Registration"));
    else if (viewMode === "edit")
      onSubViewChange(translate("Edit Device Registration"));
    else if (viewMode === "view")
      onSubViewChange(translate("View Device Registration"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleLookupOpen = (lookupKey: string) => {
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  const getLookupData = (lookupKey: string) => {
    if (lookupKey === "deviceType") {
      return {
        title: translate("Device Type"),
        subtitle: translate("Search / Device Type"),
        columns: [
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: translate("Device Type"), minWidth: 200 },
        ],
        rows: lookups.deviceTypes || [],
      };
    } else if (lookupKey === "site") {
      return {
        title: translate("Site"),
        subtitle: translate("Search / Site"),
        columns: [
          { id: "code", label: translate("Code"), minWidth: 100 },
          { id: "name", label: translate("Site"), minWidth: 200 },
        ],
        rows: lookups.sites || [],
      };
    } else if (lookupKey === "connectivityType") {
      return {
        title: translate("Connectivity Type"),
        subtitle: translate("Search / Connectivity Type"),
        columns: [
          { id: "code", label: translate("Code"), minWidth: 100 },
          {
            id: "name",
            label: translate("Connectivity Type"),
            minWidth: 200,
          },
        ],
        rows: lookups.connectivityTypes || [],
      };
    }
    return null;
  };

  const renderLookupDialog = () => {
    if (!activeLookup) return null;
    const data = getLookupData(activeLookup);
    if (!data) return null;

    return (
      <Dialog
        open={!!activeLookup}
        onClose={handleLookupClose}
        maxWidth="lg"
        fullWidth
        PaperProps={{
          sx: { borderRadius: "12px", overflow: "hidden" },
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
            {data.title}
          </Typography>
          <IconButton
            size="small"
            onClick={handleLookupClose}
            sx={{ bgcolor: "#fff", color: "#aaa" }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>
        <Box sx={{ px: 3, py: 1, bgcolor: "#fff" }}>
          <Typography
            variant="caption"
            sx={{ color: "var(--lms-primary)", fontWeight: 700 }}
          >
            {data.subtitle}
          </Typography>
        </Box>
        <Box sx={{ px: 3, py: 2 }}>
          <TableContainer
            sx={{ border: "1px solid #E2E8F0", borderRadius: "8px" }}
          >
            <Table size="small">
              <TableHead>
                <TableRow sx={{ bgcolor: "#F8FAFC" }}>
                  {data.columns.map((col: any) => (
                    <TableCell key={col.id} sx={{ fontWeight: 700 }}>
                      {col.label}
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                {data.rows.map((row: any, idx: number) => (
                  <TableRow
                    key={idx}
                    hover
                    sx={{ cursor: "pointer" }}
                    onClick={() => {
                      if (activeLookup === "deviceType") {
                        setFormData({
                          ...formData,
                          deviceTypeId: row.id,
                          deviceTypeName: row.name,
                        });
                      } else if (activeLookup === "site") {
                        setFormData({
                          ...formData,
                          siteId: row.id,
                          siteName: row.name,
                        });
                      } else if (activeLookup === "connectivityType") {
                        setFormData({
                          ...formData,
                          connectivityTypeId: row.id,
                          connectivityTypeName: row.name,
                        });
                      }
                      handleLookupClose();
                    }}
                  >
                    {data.columns.map((col: any) => (
                      <TableCell key={col.id}>{row[col.id]}</TableCell>
                    ))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
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
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "deviceName", label: translate("Device Name"), minWidth: 200 },
    { id: "serialNumber", label: translate("Serial Number"), minWidth: 150 },
    { id: "deviceTypeName", label: translate("Device Type"), minWidth: 150 },
    { id: "siteName", label: translate("Site"), minWidth: 150 },
    { id: "deviceIp", label: translate("Device IP"), minWidth: 150 },
  ];

  const handleSave = async (dataToSave: any) => {
    setIsSubmitted(true);
    const errors = [];
    if (!CheckemptyString(dataToSave.code))
      errors.push(translate("Code is required"));
    if (!dataToSave.deviceTypeId || dataToSave.deviceTypeId <= 0)
      errors.push(translate("Device Type is required"));
    if (!CheckemptyString(dataToSave.deviceName))
      errors.push(translate("Device Name is required"));
    if (!CheckemptyString(dataToSave.serialNumber))
      errors.push(translate("Serial Number is required"));
    if (!dataToSave.siteId || dataToSave.siteId <= 0)
      errors.push(translate("Site is required"));
    if (!CheckemptyString(dataToSave.deviceIp))
      errors.push(translate("Device IP is required"));
    if (!dataToSave.connectivityTypeId || dataToSave.connectivityTypeId <= 0)
      errors.push(translate("Connectivity Type is required"));

    const portVal = parseInt(dataToSave.machinePort, 10);
    if (isNaN(portVal) || portVal <= 0)
      errors.push(translate("Machine Port must be greater than 0"));

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = {
        code: dataToSave.code,
        deviceTypeId: Number(dataToSave.deviceTypeId) || 0,
        deviceName: dataToSave.deviceName,
        serialNumber: dataToSave.serialNumber?.trim(),
        siteId: Number(dataToSave.siteId) || 0,
        deviceIp: dataToSave.deviceIp,
        machinePort: portVal,
        connectivityTypeId: Number(dataToSave.connectivityTypeId) || 0,
        deviceId:
          dataToSave.deviceId != null && Number(dataToSave.deviceId) > 0
            ? Number(dataToSave.deviceId)
            : null,
        isActive: dataToSave.isActive,
      };

      const updating = Boolean(dataToSave.id && dataToSave.id > 0);
      const response = updating
        ? await workflowMutation("put", `/api/HumanResource/DeviceRegistration/v3/${dataToSave.id}`, payload)
        : await workflowMutation("post", "/api/HumanResource/DeviceRegistration/v3", payload);
      // Sent for approval: the record itself does not change yet, so the list is left as it is.
      if (pendingWorkflow(response)) {
        setViewMode("list");
        return true;
      }
      setToasterMessages([
        updating ? translate("Record updated successfully") : translate("Record saved successfully"),
      ]);

      setToasterSeverity("success");
      setShowToaster(true);
      setViewMode("list");
      // The saved row carries server-derived display names, so refetch rather
      // than reconstruct the cached shape from the form payload.
      dispatch(deviceRegistrationSlice.fetchAll(true));
      return true;
    } catch (error: any) {
      setToasterMessages([apiErrorMessage(error, translate("An error occurred while saving the record"))]);
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
      const response = await workflowMutation("delete", `/api/HumanResource/DeviceRegistration/v3/${row.id}`);
      if (pendingWorkflow(response)) return;
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(deviceRegistrationSlice.actions.removeRow(row.id));
    } catch (error: any) {
      setToasterMessages([apiErrorMessage(error, translate("An error occurred while deleting the record"))]);
      setToasterSeverity("error");
      setShowToaster(true);
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

  const initialFormData = {
    id: 0,
    code: "",
    deviceTypeId: 0,
    deviceName: "",
    serialNumber: "",
    siteId: 0,
    deviceIp: "",
    machinePort: "",
    connectivityTypeId: 0,
    deviceId: 0,
    isActive: true,
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable
          title={translate("Device Registration")}
          addButtonLabel={translate("New Device Registration")}
          breadcrumbPath={''}
          columns={columns}
          rows={rows}
          onAddClick={() => {
            setFormData(initialFormData);
            setViewMode("new");
            setIsSubmitted(false);
          }}
          onEditClick={(row) => {
            setFormData(row);
            setViewMode("edit");
            setIsSubmitted(false);
          }}
          onViewClick={(row) => {
            setFormData(row);
            setViewMode("view");
            setIsSubmitted(false);
          }}
          onDelete={handleDelete}
          showAddButton={true}
          showToaster={false}
          actionsPosition="right"
        />
      ) : (
        <Paper
          elevation={0}
          sx={{
            p: 3,
            borderRadius: "12px",
            border: "1px solid #E2E8F0",
            bgcolor: "#fff",
          }}
        >
          <FormThemeWrapper>
            <Box>
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "flex-end",
                  gap: 2,
                  mb: 3,
                }}
              >
                <Button
                  variant="outlined"
                  startIcon={<ArrowBackIcon />}
                  onClick={() => setViewMode("list")}
                  sx={{
                    borderRadius: "8px",
                    textTransform: "none",
                    fontWeight: 600,
                  }}
                >
                  {translate("Back to List")}
                </Button>
                {viewMode !== "view" && (
                  <Button
                    variant="contained"
                    startIcon={<SaveIcon />}
                    onClick={() => handleSave(formData)}
                    sx={{
                      borderRadius: "8px",
                      textTransform: "none",
                      fontWeight: 700,
                      bgcolor: "var(--lms-primary)",
                      "&:hover": { bgcolor: "var(--lms-primary-dark)" },
                    }}
                  >
                    {translate("Save")}
                  </Button>
                )}
              </Box>
              {/* Basic Information Section */}
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
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Code")}
                      required
                      disabled={viewMode === "view"}
                      value={formData.code || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, code: e.target.value })
                      }
                      error={isSubmitted && !CheckemptyString(formData.code)}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      select
                      fullWidth
                      size="small"
                      label={translate("Device Type")}
                      required
                      disabled={viewMode === "view"}
                      value={formData.deviceTypeId || ""}
                      onChange={(e) => {
                        const deviceTypeId = Number(e.target.value) || 0;
                        const selected = lookups.deviceTypes.find(
                          (t: any) => Number(t.id) === deviceTypeId,
                        );
                        setFormData({
                          ...formData,
                          deviceTypeId,
                          deviceTypeName: selected?.name || "",
                        });
                      }}
                      error={
                        isSubmitted &&
                        (!formData.deviceTypeId || formData.deviceTypeId <= 0)
                      }
                      InputProps={{
                        endAdornment: viewMode !== "view" && (
                          <InputAdornment position="end">
                            <IconButton
                              size="small"
                              onClick={() => handleLookupOpen("deviceType")}
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
                      SelectProps={{ IconComponent: () => null }}
                    >
                      {lookups.deviceTypes.map((dt: any) => (
                        <MenuItem key={dt.id} value={dt.id}>
                          {dt.name}
                        </MenuItem>
                      ))}
                    </FormInput>
                  </Grid>
                  <Grid item xs={12}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Device Name")}
                      required
                      disabled={viewMode === "view"}
                      placeholder={translate("Enter Device Name")}
                      value={formData.deviceName || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, deviceName: e.target.value })
                      }
                      error={
                        isSubmitted && !CheckemptyString(formData.deviceName)
                      }
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      select
                      fullWidth
                      size="small"
                      label={translate("Site")}
                      required
                      disabled={viewMode === "view"}
                      value={formData.siteId || ""}
                      onChange={(e) => {
                        const siteId = Number(e.target.value) || 0;
                        const selected = lookups.sites.find(
                          (s: any) => Number(s.id) === siteId,
                        );
                        setFormData({
                          ...formData,
                          siteId,
                          siteName: selected?.name || "",
                        });
                      }}
                      error={
                        isSubmitted &&
                        (!formData.siteId || formData.siteId <= 0)
                      }
                      InputProps={{
                        endAdornment: viewMode !== "view" && (
                          <InputAdornment position="end">
                            <IconButton
                              size="small"
                              onClick={() => handleLookupOpen("site")}
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
                      SelectProps={{ IconComponent: () => null }}
                    >
                      {lookups.sites.map((site: any) => (
                        <MenuItem key={site.id} value={site.id}>
                          {site.name}
                        </MenuItem>
                      ))}
                    </FormInput>
                  </Grid>
                </Grid>
              </Collapse>

              {/* Machine Information Section */}
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  mb: 3,
                  cursor: "pointer",
                  userSelect: "none",
                  borderTop: "1px solid #E2E8F0",
                  pt: 3,
                  "&:hover": {
                    "& .expand-icon-container": {
                      bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
                    },
                  },
                }}
                onClick={() => setIsMachineExpanded(!isMachineExpanded)}
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
                    <T>Machine Information</T>
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
                  {renderExpandIcon(isMachineExpanded)}
                </Box>
              </Box>

              <Collapse in={isMachineExpanded}>
                <Grid container spacing={3}>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Device Ip")}
                      required
                      disabled={viewMode === "view"}
                      placeholder={translate("Enter Device IP")}
                      value={formData.deviceIp || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, deviceIp: e.target.value })
                      }
                      error={
                        isSubmitted && !CheckemptyString(formData.deviceIp)
                      }
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      size="small"
                      label={translate("Serial Number")}
                      required
                      disabled={viewMode === "view"}
                      placeholder={translate("Enter Serial Number")}
                      value={formData.serialNumber || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, serialNumber: e.target.value })
                      }
                      error={
                        isSubmitted && !CheckemptyString(formData.serialNumber)
                      }
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      type="number"
                      size="small"
                      label={translate("Machine Port")}
                      required
                      disabled={viewMode === "view"}
                      value={formData.machinePort || ""}
                      onChange={(e) =>
                        setFormData({ ...formData, machinePort: e.target.value })
                      }
                      error={
                        isSubmitted &&
                        (!formData.machinePort ||
                          parseInt(formData.machinePort, 10) <= 0)
                      }
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      fullWidth
                      type="number"
                      size="small"
                      label={translate("Device ID")}
                      disabled={viewMode === "view"}
                      value={formData.deviceId || ""}
                      onChange={(e) =>
                        setFormData({
                          ...formData,
                          deviceId: parseInt(e.target.value, 10) || 0,
                        })
                      }
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      select
                      fullWidth
                      size="small"
                      label={translate("Connectivity Type")}
                      required
                      disabled={viewMode === "view"}
                      value={formData.connectivityTypeId || ""}
                      onChange={(e) => {
                        const connectivityTypeId = Number(e.target.value) || 0;
                        const selected = lookups.connectivityTypes.find(
                          (c: any) => Number(c.id) === connectivityTypeId,
                        );
                        setFormData({
                          ...formData,
                          connectivityTypeId,
                          connectivityTypeName: selected?.name || "",
                        });
                      }}
                      error={
                        isSubmitted &&
                        (!formData.connectivityTypeId ||
                          formData.connectivityTypeId <= 0)
                      }
                      InputProps={{
                        endAdornment: viewMode !== "view" && (
                          <InputAdornment position="end">
                            <IconButton
                              size="small"
                              onClick={() =>
                                handleLookupOpen("connectivityType")
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
                      SelectProps={{ IconComponent: () => null }}
                    >
                      {lookups.connectivityTypes.map((ct: any) => (
                        <MenuItem key={ct.id} value={ct.id}>
                          {ct.name}
                        </MenuItem>
                      ))}
                    </FormInput>
                  </Grid>
                </Grid>
              </Collapse>

            </Box>
          </FormThemeWrapper>
        </Paper>
      )}
      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {renderLookupDialog()}
    </>
  );
};

export default DeviceRegistration;
