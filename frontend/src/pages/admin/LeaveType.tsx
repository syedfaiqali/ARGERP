import React, { useState } from "react";
import {
  Grid,
  Box,
  Typography,
  Collapse,
} from "@mui/material";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Assessment as AssessmentIcon
} from "@mui/icons-material";
import { Button } from "@mui/material";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LeaveTypeReportModal } from "./LeaveTypeReportModal";
import { useEffect, useCallback } from "react";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLookups, useHrLovs } from "@/hooks";
import { leaveTypeSlice } from "@/store/humanResourceSlices";
import { GenericSearchModal } from "@/components/GenericSearchModal";

const initialFormData = {
  code: "",
  renewOn: "",
  leaveType: "",
  leaveUnit: "",
  leaves: "",
  maxAvailUnit: "",
  maxAvail: "",
  maritalStatus: "",
  carryForwardVal: "",
  carryForwardLookup: "",
  encashmentVal: "",
  encashmentLookup: "",
  gender: "",
  entitleOn: "",
  accrualUnit: "",
  entitleLeaves: "",
  allowInProb: false,
  quotaValidate: false,
  lateAdjustable: false,
  earlyDepAdjustable: false,
  specialLeave: false,
  includeHolidays: false,
  excludeWeeklyHolidays: false,
  paidLeave: false,
  requestBefore: "",
  requestUnit: "",
  noOfMonth: "",
};

interface LeaveTypeProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const LeaveType: React.FC<LeaveTypeProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrLeaveType",
    leaveTypeSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  // Institutions are client/entity-scoped and shared with the other HR screens.
  const { lookup } = useHrLookups(["institutions"]);
  const entities = React.useMemo<any[]>(
    () =>
      (lookup("institutions") || []).map((item: any) => ({
        id: Number(item.id ?? item.entityId ?? 0),
        code: item.code ?? "",
        name: item.instituteName || item.entityName || item.stxt || item.name || "",
        shortName: item.shortName || "",
        address: item.address || "",
        city: item.cityName || "",
        country: item.countryName || "",
        imageId: item.imageId || null,
        instituteName: item.instituteName || "",
      })),
    [lookup],
  );
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report">(
    "list"
  );
  const [formData, setFormData] = useState<any>({ ...initialFormData });
  const [isSubmitted, setIsSubmitted] = useState(false);

  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [isPolicyExpanded, setIsPolicyExpanded] = useState(true);
  const [isLeaveEntitleExpanded, setIsLeaveEntitleExpanded] = useState(true);
  const [isRestrictionExpanded, setIsRestrictionExpanded] = useState(true);
  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );

  const handleLookupOpen = (lookupKey: string) => {
    setActiveLookup(lookupKey);
  };

  const handleLookupClose = () => {
    setActiveLookup(null);
  };



  const [LOOKUP_DATA, setLOOKUP_DATA] = useState<any>({
    renewOn: {
      title: translate("Renew On"),
      subtitle: translate("Search / Renew On"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Renew On"), minWidth: 200 },
      ],
      rows: [],
    },
    leaveUnit: {
      title: translate("Leave Unit"),
      subtitle: translate("Search / Leave Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Leave Unit"), minWidth: 200 },
      ],
      rows: [],
    },
    maritalStatus: {
      title: translate("Marital Status"),
      subtitle: translate("Search / Marital Status"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Status"), minWidth: 200 },
      ],
      rows: [],
    },
    gender: {
      title: translate("Gender"),
      subtitle: translate("Search / Gender"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Gender"), minWidth: 200 },
      ],
      rows: [],
    },
    maxAvailUnit: {
      title: translate("Max Avail Unit"),
      subtitle: translate("Search / Max Avail Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Unit"), minWidth: 200 },
      ],
      rows: [],
    },
    entitleOn: {
      title: translate("Entitle On"),
      subtitle: translate("Search / Entitle On"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Entitle On"), minWidth: 200 },
      ],
      rows: [],
    },
    accrualUnit: {
      title: translate("Accrual Unit"),
      subtitle: translate("Search / Accrual Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Unit"), minWidth: 200 },
      ],
      rows: [],
    },
    requestUnit: {
      title: translate("Request Unit"),
      subtitle: translate("Search / Request Unit"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Unit"), minWidth: 200 },
      ],
      rows: [],
    },
    carryForward: {
      title: translate("Carry Forward"),
      subtitle: translate("Search / Carry Forward"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Policy"), minWidth: 200 },
      ],
      rows: [],
    },
    encashment: {
      title: translate("Encashment"),
      subtitle: translate("Search / Encashment"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Policy"), minWidth: 200 },
      ],
      rows: [],
    },
  });

  // Every one of these is global LOV reference data, shared with the other HR
  // screens. The dialog structure (titles, columns) is kept; only the rows are
  // filled from the shared cache instead of eight requests on every mount.
  const { lov } = useHrLovs([
    "RNW",
    "LVUNT",
    "PUNIT",
    "MRSTS",
    "GEND",
    "LVTIT",
    "LPCY",
    "ACUNT",
  ]);

  useEffect(() => {
    setLOOKUP_DATA((prev: any) => ({
      ...prev,
      renewOn:      { ...prev.renewOn,      rows: lov("RNW")   },
      leaveUnit:    { ...prev.leaveUnit,    rows: lov("LVUNT") },
      maxAvailUnit: { ...prev.maxAvailUnit, rows: lov("PUNIT") },
      requestUnit:  { ...prev.requestUnit,  rows: lov("PUNIT") },
      maritalStatus:{ ...prev.maritalStatus,rows: lov("MRSTS") },
      gender:       { ...prev.gender,       rows: lov("GEND")  },
      entitleOn:    { ...prev.entitleOn,    rows: lov("LVTIT") },
      carryForward: { ...prev.carryForward, rows: lov("LPCY")  },
      encashment:   { ...prev.encashment,   rows: lov("LPCY")  },
      accrualUnit:  { ...prev.accrualUnit,  rows: lov("ACUNT") },
    }));
  }, [lov]);



  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Leave Type"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Leave Type"));
    else if (viewMode === "view") onSubViewChange(translate("View Leave Type"));
    else if (viewMode === "report") onSubViewChange(translate("Leave Type Report"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleFetchById = async (id: number, mode: "edit" | "view") => {
    setApiLoading(true);
    try {
      const response: any = await GetService(`/api/HumanResource/LeaveType/v3/${id}`);
      setFormData(response.data);
      setViewMode(mode);
    } catch (error: any) {
      console.error("Failed to fetch details", error);
    } finally {
      setApiLoading(false);
    }
  };



  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    {
      id: "renewOn",
      label: translate("Renew On"),
      minWidth: 150,
      format: (val: any) => {
        const row = LOOKUP_DATA.renewOn.rows.find((r: any) => r.id.toString() === val?.toString());
        return row ? row.name : val;
      }
    },
    { id: "leaveType", label: translate("Leave Type"), minWidth: 200 },
    {
      id: "leaveUnit",
      label: translate("Leave Unit"),
      minWidth: 120,
      format: (val: any) => {
        const row = LOOKUP_DATA.leaveUnit.rows.find((r: any) => r.id.toString() === val?.toString());
        return row ? row.name : val;
      }
    },
    { id: "leaves", label: translate("Leaves"), minWidth: 100 },
    { id: "noOfMonth", label: translate("No of Month"), minWidth: 120 },
    {
      id: "entitleOn",
      label: translate("Entitle On"),
      minWidth: 120,
      format: (val: any) => {
        const row = LOOKUP_DATA.leaveUnit.rows.find((r: any) => r.id.toString() === val?.toString());
        return row ? row.name : val;
      }
    },
  ];


  const getCalculatedEntitleLeaves = (
    baseLeaves: string | number | undefined,
    entitleOnId: string | undefined,
    accrualUnitId: string | undefined
  ) => {
    if (!baseLeaves) return "";
    const numLeaves = Number(baseLeaves);
    if (isNaN(numLeaves)) return "";

    const entitleName = LOOKUP_DATA.entitleOn.rows.find((r: any) => r.id.toString() === entitleOnId?.toString())?.name?.toLowerCase();
    
    if (entitleName === "yearly") {
      return numLeaves.toString();
    } else if (entitleName === "accrual") {
      const accrualName = LOOKUP_DATA.accrualUnit.rows.find((r: any) => r.id.toString() === accrualUnitId?.toString())?.name?.toLowerCase();
      
      switch (accrualName) {
        case "daily":
          return ((numLeaves / 12) / 30).toFixed(3);
        case "weekly":
          return (((numLeaves / 12) / 30) * 7).toFixed(3);
        case "fortnightly":
          return (((numLeaves / 12) / 30) * 15).toFixed(3);
        case "monthly":
          return (numLeaves / 12).toFixed(3);
        case "quarterly":
          return ((numLeaves / 12) * 3).toFixed(3);
        case "half yearly":
        case "half-yearly":
        case "half_yearly":
          return ((numLeaves / 12) * 6).toFixed(3);
        case "yearly":
          return numLeaves.toString();
        default:
          return "";
      }
    }
    return "";
  };

  const handleSaveInternal = async () => {
    setIsSubmitted(true);
    const errors = [];
    if (!formData.leaveType) errors.push(translate("Leave Type is required"));
    if (!formData.renewOn) errors.push(translate("Renew On is required"));
    if (formData.leaves === "" || formData.leaves === null || isNaN(Number(formData.leaves))) {
      errors.push(translate("Leaves must be a valid number"));
    }

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return;
    }

    setApiLoading(true);
    try {
      // Ensure numerical fields are numbers, not empty strings
      const payload = {
        ...formData,
        leaves: Number(formData.leaves) || 0,
        carryForwardVal: formData.carryForwardVal ? formData.carryForwardVal.toString() : "0",
        encashmentVal: formData.encashmentVal ? formData.encashmentVal.toString() : "0",
        requestBefore: formData.requestBefore ? formData.requestBefore.toString() : "0",
        noOfMonth: formData.noOfMonth ? formData.noOfMonth.toString() : "0",
        entitleLeaves: getCalculatedEntitleLeaves(formData.leaves, formData.entitleOn, formData.accrualUnit),
      };

      let saved: any;
      if (formData.id) {
        const response: any = await PutService("/api/HumanResource/LeaveType/v3", payload);
        saved = response?.data;
      } else {
        const response: any = await PostService("/api/HumanResource/LeaveType/v3", payload);
        saved = response?.data;
      }
      setErrorMessages([translate("Record saved successfully")]);
      setToasterSeverity("success");
      setShowError(true);
      setViewMode("list");
      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? formData.id ?? 0);
      if (savedId > 0) {
        dispatch(leaveTypeSlice.actions.upsertRow({ ...formData, ...payload, id: savedId }));
      } else {
        dispatch(leaveTypeSlice.fetchAll(true));
      }
    } catch (error: any) {
      let messages = [translate("Failed to save record")];
      if (error?.response?.data?.errors) {
        messages = Object.values(error.response.data.errors).flat() as string[];
      } else if (error?.response?.data?.title) {
        messages = [error.response.data.title];
      } else if (error.message) {
        messages = [error.message];
      }
      setErrorMessages(messages);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      await DeleteService(`/api/HumanResource/LeaveType/v3/${row.id}`);
      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(leaveTypeSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const data = error?.response?.data;
      const message = typeof data === "string"
        ? data
        : data?.error || data?.title || error.message || "Failed to delete record";
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
    } finally {
      setApiLoading(false);
    }
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
      }}
    >
      <Box
        sx={{
          width: 8,
          height: "2px",
          bgcolor: expanded ? "#fff" : "var(--lms-primary)",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(0deg)" : "rotate(90deg)"
        }}
      />
      {!expanded && (
        <Box
          sx={{
            width: "2px",
            height: 8,
            bgcolor: "var(--lms-primary)",
            position: "absolute"
          }}
        />
      )}
    </Box>
  );

  const renderSectionHeader = (
    title: string,
    expanded: boolean,
    onToggle: () => void,
  ) => (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        mb: 2,
        mt: 0,
        cursor: "pointer",
        userSelect: "none",
        pt: 2,
        "&:hover": {
          "& .expand-icon-container": {
            bgcolor: "rgba(var(--lms-primary-rgb), 0.05)",
          },
        },
      }}
      onClick={onToggle}
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
          {translate(title)}
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
        {renderExpandIcon(expanded)}
      </Box>
    </Box>
  );

  const renderForm = (
    formData: any,
    setFormData: any,
    _isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    return (
      <FormThemeWrapper>
        <Box sx={{ p: 0 }}>
          {/* Basic Information */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 2,
              mt: 0,
              cursor: "pointer",
              userSelect: "none",
              pt: 2,
              "&:hover": {
                "& .expand-icon-container": {
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.05)",
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
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Code")}
                    disabled
                    value={formData.code || ""}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Renew On*</T>}
                    value={formData.renewOn || ""}
                    options={LOOKUP_DATA.renewOn.rows}
                    onSearchClick={() => handleLookupOpen("renewOn")}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, renewOn: option.id.toString() })
                    }
                    error={isSubmitted && !formData.renewOn}
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Leave Type")}
                    required
                    placeholder={translate("Enter Leave Type")}
                    value={formData.leaveType || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, leaveType: e.target.value })
                    }
                    error={isSubmitted && !formData.leaveType}
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Leave Unit*</T>}
                    value={formData.leaveUnit || ""}
                    onSearchClick={() => handleLookupOpen("leaveUnit")}
                    options={LOOKUP_DATA.leaveUnit.rows}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, leaveUnit: option.id.toString() })
                    }
                    error={isSubmitted && !formData.leaveUnit}
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    type="number"
                    size="small"
                    label={translate("Leaves")}
                    required
                    placeholder={translate("Enter Leaves")}
                    value={formData.leaves ?? ""}
                    onChange={(e) =>
                      setFormData({ ...formData, leaves: e.target.value })
                    }
                    error={isSubmitted && !formData.leaves}
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>

          {/* Policy */}
          {renderSectionHeader("Policy", isPolicyExpanded, () =>
            setIsPolicyExpanded(!isPolicyExpanded),
          )}
          <Collapse in={isPolicyExpanded}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Max Avail Unit</T>}
                    value={formData.maxAvailUnit || ""}
                    onSearchClick={() => handleLookupOpen("maxAvailUnit")}
                    options={LOOKUP_DATA.maxAvailUnit.rows}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, maxAvailUnit: option.id.toString() })
                    }
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    type="number"
                    label={translate("Max Avail")}
                    value={formData.maxAvail || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, maxAvail: e.target.value })
                    }
                    placeholder={translate("Enter Max Avail")}
                    disabled={_isReadOnly || !formData.maxAvailUnit}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Marital Status</T>}
                    value={formData.maritalStatus || ""}
                    options={LOOKUP_DATA.maritalStatus.rows}
                    getOptionLabel={(option) => option.name}
                    onSearchClick={() => handleLookupOpen("maritalStatus")}
                    onSelect={(option) =>
                      setFormData({ ...formData, maritalStatus: option.id.toString() })
                    }
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <Typography
                    component="label"
                    sx={{ display: "block", mb: 0.5, fontSize: "0.85rem", fontWeight: 600 }}
                  >
                    <T>Carry Forward</T>
                  </Typography>
                  <Box sx={{ display: "flex", alignItems: "stretch" }}>
                    <FormInput
                      size="small"
                      type="number"
                      hideLabel
                      placeholder={translate("Value")}
                      value={formData.carryForwardVal || ""}
                      onChange={(e) =>
                        setFormData({
                          ...formData,
                          carryForwardVal: e.target.value,
                        })
                      }
                      sx={{
                        flex: "0 0 34%",
                        "& .MuiOutlinedInput-root": { borderRadius: "9px 0 0 9px" },
                      }}
                      disabled={_isReadOnly}
                    />
                    <LookupDropdown
                      hideLabel
                      value={formData.carryForwardLookup || ""}
                      options={LOOKUP_DATA.carryForward.rows}
                      getOptionLabel={(option) => option.name}
                      onSearchClick={() => handleLookupOpen("carryForward")}
                      onSelect={(option) =>
                        setFormData({
                          ...formData,
                          carryForwardLookup: option.id.toString(),
                        })
                      }
                      disabled={_isReadOnly}
                      sx={{
                        flex: "1 1 auto",
                        "& .MuiOutlinedInput-root": { borderRadius: "0 9px 9px 0", marginLeft: "-1px" },
                      }}
                    />
                  </Box>
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <Typography
                    component="label"
                    sx={{ display: "block", mb: 0.5, fontSize: "0.85rem", fontWeight: 600 }}
                  >
                    <T>Encashment</T>
                  </Typography>
                  <Box sx={{ display: "flex", alignItems: "stretch" }}>
                    <FormInput
                      size="small"
                      type="number"
                      hideLabel
                      placeholder={translate("Value")}
                      value={formData.encashmentVal || ""}
                      onChange={(e) =>
                        setFormData({
                          ...formData,
                          encashmentVal: e.target.value,
                        })
                      }
                      sx={{
                        flex: "0 0 34%",
                        "& .MuiOutlinedInput-root": { borderRadius: "9px 0 0 9px" },
                      }}
                      disabled={_isReadOnly}
                    />
                    <LookupDropdown
                      hideLabel
                      value={formData.encashmentLookup || ""}
                      options={LOOKUP_DATA.encashment.rows}
                      getOptionLabel={(option) => option.name}
                      onSearchClick={() => handleLookupOpen("encashment")}
                      onSelect={(option) =>
                        setFormData({
                          ...formData,
                          encashmentLookup: option.id.toString(),
                        })
                      }
                      disabled={_isReadOnly}
                      sx={{
                        flex: "1 1 auto",
                        "& .MuiOutlinedInput-root": { borderRadius: "0 9px 9px 0", marginLeft: "-1px" },
                      }}
                    />
                  </Box>
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5, mt: 2.9}}>
                  <LookupDropdown
                    label={<T>Gender</T>}
                    value={formData.gender || ""}
                    onSearchClick={() => handleLookupOpen("gender")}
                    options={LOOKUP_DATA.gender.rows}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, gender: option.id.toString() })
                    }
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>

          {/* Leave Entitle Policy */}
          {renderSectionHeader(
            "Leave Entitle Policy",
            isLeaveEntitleExpanded,
            () => setIsLeaveEntitleExpanded(!isLeaveEntitleExpanded),
          )}
          <Collapse in={isLeaveEntitleExpanded}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Entitle On</T>}
                    value={formData.entitleOn || ""}
                    options={LOOKUP_DATA.entitleOn.rows}
                    onSearchClick={() => handleLookupOpen("entitleOn")}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({
                        ...formData,
                        entitleOn: option.id.toString(),
                        ...(option.name?.toLowerCase() !== "accrual" ? { accrualUnit: "" } : {}),
                      })
                    }
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Accrual Unit</T>}
                    value={formData.accrualUnit || ""}
                    options={LOOKUP_DATA.accrualUnit.rows}
                    onSearchClick={() => handleLookupOpen("accrualUnit")}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, accrualUnit: option.id.toString() })
                    }
                    disabled={
                      _isReadOnly ||
                      LOOKUP_DATA.entitleOn.rows.find(
                        (r: any) => r.id.toString() === formData.entitleOn?.toString()
                      )?.name?.toLowerCase() !== "accrual"
                    }
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={4}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Leaves")}
                    placeholder={translate("Enter Leaves")}
                    value={getCalculatedEntitleLeaves(formData.leaves, formData.entitleOn, formData.accrualUnit)}
                    disabled
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>

          {/* Restriction(s) */}
          {renderSectionHeader("Restriction(s)", isRestrictionExpanded, () =>
            setIsRestrictionExpanded(!isRestrictionExpanded),
          )}
          <Collapse in={isRestrictionExpanded}>
            <Grid container spacing={1}>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.allowInProb || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      allowInProb: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      <T>Allow in Prob</T>
                    </Typography>
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.quotaValidate || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      quotaValidate: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      <T>Quota Validate</T>
                    </Typography>
                  }
                />
              </Grid>
              <Grid item xs={12} md={6}></Grid>

              <Grid item xs={12} md={3}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    type="number"
                    label={translate("Request Before")}
                    placeholder={translate("Enter Request Before")}
                    value={formData.requestBefore || ""}
                    onChange={(e) => {
                      const val = e.target.value;
                      setFormData({
                        ...formData,
                        requestBefore: val,
                        ...(val ? {} : { requestUnit: "" }),
                      });
                    }}
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={3}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    type="number"
                    label={translate("No of Month")}
                    placeholder={translate("Enter No of Month")}
                    value={formData.noOfMonth || ""}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        noOfMonth: e.target.value,
                      })
                    }
                    disabled={_isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.lateAdjustable || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      lateAdjustable: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      <T>Late Adjustable</T>
                    </Typography>
                  }
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <LmsCheckbox
                  checked={formData.includeHolidays || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      includeHolidays: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      Include Holidays
                    </Typography>
                  }
                />
              </Grid>

              <Grid item xs={12} md={3}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={<T>Request Unit</T>}
                    value={formData.requestUnit || ""}
                    onSearchClick={() => handleLookupOpen("requestUnit")}
                    options={LOOKUP_DATA.requestUnit.rows}
                    getOptionLabel={(option) => option.name}
                    onSelect={(option) =>
                      setFormData({ ...formData, requestUnit: option.id.toString() })
                    }
                    disabled={_isReadOnly || !formData.requestBefore}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.earlyDepAdjustable || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      earlyDepAdjustable: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      Early Dep. Adjustable
                    </Typography>
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.specialLeave || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      specialLeave: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      Special Leave
                    </Typography>
                  }
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.excludeWeeklyHolidays || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      excludeWeeklyHolidays: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      Exclude Weekly Holidays
                    </Typography>
                  }
                />
              </Grid>

              <Grid item xs={12} md={3}>
                <LmsCheckbox
                  checked={formData.paidLeave || false}
                  onChange={(checked) =>
                    setFormData({
                      ...formData,
                      paidLeave: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  disabled={_isReadOnly}
                  label={
                    <Typography sx={{ fontSize: "0.85rem" }}>
                      Paid Leave
                    </Typography>
                  }
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
                ? translate("New Leave Type")
                : viewMode === "edit"
                  ? translate("Edit Leave Type")
                  : viewMode === "view"
                    ? translate("View Leave Type")
                    : translate("Leave Type Report")}
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
              {viewMode !== "view" && viewMode !== "report" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSaveInternal}
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
            {viewMode === "report" ? (
              <LeaveTypeReportModal
                open={true}
                onClose={() => setViewMode("list")}
                lookups={{
                  leaveTypes: rows,
                  entities: entities,
                  maritalStatus: LOOKUP_DATA.maritalStatus.rows,
                  gender: LOOKUP_DATA.gender.rows,
                  renewOn: LOOKUP_DATA.renewOn.rows
                }}
                isFullScreenView={true}
              />
            ) : (
              renderForm(formData, setFormData, viewMode === "view", isSubmitted)
            )}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Leave Type")}
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
          onEditClick={(row: any) => handleFetchById(row.id, "edit")}
          onViewClick={(row: any) => handleFetchById(row.id, "view")}
          showAddButton={true}
          showToaster={false}
          extraToolbarItemsLeft={
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              onClick={() => setViewMode("report")}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9", // vibrant sky blue
                px: 3,
                py: 0.8,
                boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
                "&:hover": {
                  bgcolor: "#0284c7",
                  boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
                },
              }}
            >
              {translate("Leave Type Report")}
            </Button>
          }
        />
      )}
      <LmsToaster
        open={showError}
        onClose={() => setShowError(false)}
        messages={errorMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      <GenericSearchModal
        open={activeLookup !== null}
        onClose={handleLookupClose}
        title={activeLookup ? LOOKUP_DATA[activeLookup]?.title : ""}
        columns={activeLookup ? LOOKUP_DATA[activeLookup]?.columns : []}
        data={activeLookup ? LOOKUP_DATA[activeLookup]?.rows : []}
        selectedValue={
          activeLookup === "carryForward"
            ? formData.carryForwardLookup
            : activeLookup === "encashment"
              ? formData.encashmentLookup
              : (formData[activeLookup as keyof typeof formData] as string)
        }
        selectedField="name"
        onSelect={(row) => {
          if (activeLookup === "carryForward") {
            setFormData((prev: any) => ({ ...prev, carryForwardLookup: row.id.toString() }));
          } else if (activeLookup === "encashment") {
            setFormData((prev: any) => ({ ...prev, encashmentLookup: row.id.toString() }));
          } else {
            setFormData((prev: any) => ({ ...prev, [activeLookup as string]: row.id.toString() }));
          }
          handleLookupClose();
        }}
        breadcrumbPath={''}
      />
      {loading && <LmsOverlayLoader />}
    </>
  );
};

export default LeaveType;


