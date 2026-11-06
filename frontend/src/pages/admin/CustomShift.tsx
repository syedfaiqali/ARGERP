import React, { useEffect, useMemo, useState } from "react";
import { Grid, Box, Typography, IconButton, Button, Paper } from "@mui/material";
import { DeleteOutline as DeleteIcon, ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { FormTimePicker } from "@/components/FormTimePicker";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { Collapse } from "@mui/material";
import dayjs from "dayjs";
import { GetService, PostService, PutService, DeleteService } from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { customShiftSlice } from "@/store/humanResourceSlices";

interface CustomShiftProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const getLovLabel = (opt: any) => opt?.name || opt?.stxt || opt?.code || "";

const formatTime = (value?: string | null) => {
  if (!value) return "";
  const d = dayjs(value);
  return d.isValid() ? d.format("hh:mm A") : "";
};

const extractTime = (value?: string | null) => {
  if (!value) return "";
  const d = dayjs(value);
  return d.isValid() ? d.format("HH:mm") : "";
};

const parseTimeValue = (value?: string | null) => {
  if (!value) return null;
  const parts = value.split(":");
  if (parts.length < 2) return null;
  const hrs = Number(parts[0]);
  const mins = Number(parts[1]);
  if (Number.isNaN(hrs) || Number.isNaN(mins)) return null;
  return dayjs().hour(hrs).minute(mins).second(0).millisecond(0);
};

const toDateTime = (timeStr?: string | null) => {
  if (!timeStr) return null;
  // Use local time format to avoid UTC conversion issues in shifts
  return `1970-01-01T${timeStr}:00`;
};

const parseTimeMinutes = (timeStr?: string) => {
  if (!timeStr) return null;
  const parts = timeStr.split(":");
  if (parts.length < 2) return null;
  const hrs = Number(parts[0]);
  const mins = Number(parts[1]);
  if (Number.isNaN(hrs) || Number.isNaN(mins)) return null;
  return hrs * 60 + mins;
};

const computeDuration = (start?: string, end?: string) => {
  const s = parseTimeMinutes(start);
  const e = parseTimeMinutes(end);
  if (s == null || e == null) return null;
  if (e < s) return null;
  const diff = e - s;
  return Number((diff / 60).toFixed(2));
};


const CustomShift: React.FC<CustomShiftProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrCustomShift",
    customShiftSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(null);
  const [isSubmitted, setIsSubmitted] = useState(false);

  const [showError, setShowError] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">(
    "success",
  );

  // SFTT is global LOV reference data, shared with Shift and Schedule.
  const { lov } = useHrLovs(["SFTT"]);
  const shiftTypeOptions = lov("SFTT");

  const [lookupType, setLookupType] = useState<"shiftType" | null>(null);

  // The grid shape (synthetic key, display index, formatted times) is derived
  // here so the cache can keep the raw records.
  const rows = useMemo(
    () =>
      (cachedRows || []).map((item: any, index: number) => ({
        id: `${item.id}-${index}`,
        realId: item.id,
        sNo: index + 1,
        code: item.code ?? "",
        shiftName: item.stxt ?? "",
        shiftType: item.shiftTypeName ?? "",
        startTime: formatTime(item.startTime),
        endTime: formatTime(item.endTime),
        graceTime: item.graceTime ?? "",
        graceTimeEd: item.graceTimeEd ?? "",
      })),
    [cachedRows],
  );


  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Custom Shift"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Custom Shift"));
    else if (viewMode === "view") onSubViewChange(translate("View Custom Shift"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "shiftName", label: translate("Shift Name"), minWidth: 200 },
    { id: "shiftType", label: translate("Shift Type"), minWidth: 150 },
    { id: "startTime", label: translate("Start Time"), minWidth: 130 },
    { id: "endTime", label: translate("End Time"), minWidth: 130 },
    { id: "graceTime", label: translate("Grace Time (Late)"), minWidth: 160 },
    {
      id: "graceTimeEd",
      label: translate("Grace Time (Early Departure)"),
      minWidth: 200,
    },
  ];


  const initialFormData = {
    id: null as number | null,
    code: "",
    stxt: "",
    shifttypeid: null as number | null,
    shifttypename: "",
    starttime: "",
    endtime: "",

    gracetime: "",
    gracetimeed: "",
    womonday: false,
    wotuesday: false,
    wowednesday: false,
    wothuresday: false,
    wofriday: false,
    wosaturday: false,
    wosunday: false,
    isActive: true,
    breaks: [] as any[],
  };


  const validateForm = (data: any) => {
    const errors: string[] = [];
    if (!data.stxt) errors.push(translate("Shift Name is required"));
    if (!data.shifttypeid) errors.push(translate("Shift Type is required"));

    if (!data.starttime) errors.push(translate("Start Time is required"));


    if (!data.endtime) errors.push(translate("End Time is required"));
    const s = parseTimeMinutes(data.starttime);
    const e = parseTimeMinutes(data.endtime);
    if (s != null && e != null && e <= s) {
      errors.push(translate("End Time cannot be less than or equal to Start Time"));
    }
    if (!data.gracetime || Number(data.gracetime) <= 0) {
      errors.push(translate("Grace Time (Late) must be greater than zero"));
    }
    if (!data.gracetimeed || Number(data.gracetimeed) <= 0) {
      errors.push(translate("Grace Time (Early Departure) must be greater than zero"));
    }
    if (!data.breaks || data.breaks.length === 0) {
      errors.push(translate("Break Schedule is required"));
    }
    return errors;
  };

  const handleSave = async (data: any) => {
    const errors = validateForm(data);
    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    }

    const payload = {
      id: data.id ?? null,
      code: data.code || null,
      stxt: data.stxt?.trim(),
      shiftTypeId: data.shifttypeid,
      entityId: data.entityid ?? null,

      startTime: toDateTime(data.starttime),

      endTime: toDateTime(data.endtime),
      graceTime: data.gracetime ? Number(data.gracetime) : null,
      graceTimeEd: data.gracetimeed ? Number(data.gracetimeed) : null,
      woMonday: !!data.womonday,
      woTuesday: !!data.wotuesday,
      woWednesday: !!data.wowednesday,
      woThuresday: !!data.wothuresday,
      woFriday: !!data.wofriday,
      woSaturday: !!data.wosaturday,
      woSunday: !!data.wosunday,

      isActive: data.isActive ?? true,
      breaks: (data.breaks || []).map((b: any, index: number) => ({
        id: b.id ?? null,
        lineIndex: b.lineIndex ?? index + 1,
        stxt: b.stxt?.trim(),
        startTime: toDateTime(b.starttime),
        endTime: toDateTime(b.endtime),
        duration: b.duration ?? null,
      })),
    };

    try {
      if (data.id) {
        await PutService("/api/HumanResource/CustomShift/v3", payload);
        setErrorMessages([translate("Custom Shift updated successfully")]);
      } else {
        await PostService("/api/HumanResource/CustomShift/v3", payload);
        setErrorMessages([translate("Custom Shift saved successfully")]);
      }
      setToasterSeverity("success");
      setShowError(true);
      dispatch(customShiftSlice.fetchAll(true));
      return true;
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to save custom shift";
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
      return false;
    }
  };

  const handleDelete = async (row: any) => {
    try {
      await DeleteService(`/api/HumanResource/CustomShift/v3/${row.realId || row.id}`);

      setErrorMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowError(true);
      dispatch(customShiftSlice.fetchAll(true));
    } catch (error: any) {
      const message =
        error?.response?.data || error?.message || "Failed to delete custom shift";
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowError(true);
      throw error;
    }
  };

  const loadCustomShift = async (row: any) => {
    const id = row.realId || row.id;
    if (!id) return row;
    const response: any = await GetService(`/api/HumanResource/CustomShift/v3/${id}`);

    const data = response?.data;
    if (!data) return row;

    return {
      id: data.id,
      code: data.code ?? "",
      stxt: data.stxt ?? "",
      shifttypeid: data.shiftTypeId ?? null,
      shifttypename: data.shiftTypeName ?? "",
      entityid: data.entityId ?? null,
      entityname: data.entityName ?? "",

      starttime: extractTime(data.startTime),

      endtime: extractTime(data.endTime),
      gracetime: data.graceTime ?? "",
      gracetimeed: data.graceTimeEd ?? "",

      womonday: !!data.woMonday,
      wotuesday: !!data.woTuesday,
      wowednesday: !!data.woWednesday,
      wothuresday: !!data.woThuresday,
      wofriday: !!data.woFriday,
      wosaturday: !!data.woSaturday,
      wosunday: !!data.woSunday,
      isActive: data.isActive ?? true,
      breaks: (data.breaks || []).map((b: any, idx: number) => ({
        id: b.id ?? null,
        lineIndex: b.lineIndex ?? idx + 1,
        stxt: b.stxt ?? "",
        starttime: extractTime(b.startTime),
        endtime: extractTime(b.endTime),
        duration: b.duration ?? null,
      })),
    };
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
                ? translate("New Custom Shift")
                : viewMode === "edit"
                  ? translate("Edit Custom Shift")
                  : translate("View Custom Shift")}
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
                    if (!formData) return;
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
            <CustomShiftForm
              formData={formData ?? initialFormData}
              setFormData={setFormData}
              isReadOnly={viewMode === "view"}
              isSubmitted={isSubmitted}
              shiftTypeOptions={shiftTypeOptions}
              lookupType={lookupType}
              setLookupType={setLookupType}
            />



          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Custom Shift")}
          addButtonLabel={translate("New Custom Shift")}
          breadcrumbPath=""
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          actionsPosition="right"
          onAddClick={() => {
            setFormData({ ...initialFormData });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={async (row: any) => {
            setApiLoading(true);
            try {
              const loaded = await loadCustomShift(row);
              setFormData({ ...initialFormData, ...loaded });
              setIsSubmitted(false);
              setViewMode("edit");
            } catch (error: any) {
              const message =
                error?.response?.data || error?.message || "Failed to load custom shift";
              setErrorMessages([message]);
              setToasterSeverity("error");
              setShowError(true);
            } finally {
              setApiLoading(false);
            }
          }}
          onViewClick={async (row: any) => {
            setApiLoading(true);
            try {
              const loaded = await loadCustomShift(row);
              setFormData({ ...initialFormData, ...loaded });
              setIsSubmitted(false);
              setViewMode("view");
            } catch (error: any) {
              const message =
                error?.response?.data || error?.message || "Failed to load custom shift";
              setErrorMessages([message]);
              setToasterSeverity("error");
              setShowError(true);
            } finally {
              setApiLoading(false);
            }
          }}
          showAddButton={true}
          showToaster={false}
        />
      )}

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

const CustomShiftForm: React.FC<{
  formData: any;
  setFormData: React.Dispatch<React.SetStateAction<any>>;
  isReadOnly?: boolean;
  isSubmitted?: boolean;
  shiftTypeOptions: any[];
  lookupType: "shiftType" | null;
  setLookupType: (value: "shiftType" | null) => void;



}> = ({
  formData,
  setFormData,
  isReadOnly,
  isSubmitted,
  shiftTypeOptions,
  lookupType,
  setLookupType,



}) => {
    const { translate } = useTranslation();
    const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);

    const normalizedShiftTypes = useMemo(
      () =>
        (shiftTypeOptions || []).map((o: any) => ({
          ...o,
          stxt: o?.stxt ?? o?.name ?? o?.code ?? "",
          ltxt: o?.ltxt ?? o?.description ?? o?.name ?? o?.stxt ?? o?.code ?? "",
        })),
      [shiftTypeOptions],
    );




    const updateBreakRow = (id: number, updater: Partial<any>) => {
      setFormData((prev: any) => ({
        ...prev,
        breaks: (prev.breaks || []).map((b: any) =>
          b.id === id ? { ...b, ...updater } : b,
        ),
      }));
    };

    const addBreakRow = () => {
      const newBreak = {
        id: -Math.floor(Math.random() * 1000000),
        lineIndex: (formData.breaks || []).length + 1,
        stxt: "",
        starttime: "",
        endtime: "",
        duration: null,
      };
      setFormData((prev: any) => ({
        ...prev,
        breaks: [...(prev.breaks || []), newBreak],
      }));
    };

    const removeBreakRow = (id: number) => {
      setFormData((prev: any) => ({
        ...prev,
        breaks: (prev.breaks || []).filter((b: any) => b.id !== id),
      }));
    };



    const breakColumns: Column[] = [
      {
        id: "sNo",
        label: translate("S.No"),
        minWidth: 60,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "stxt",
        label: translate("Break Name"),
        minWidth: 200,
        format: (_: any, row: any) => (
          <FormInput
            size="small"
            value={row.stxt || ""}
            onChange={(e) => updateBreakRow(row.id, { stxt: e.target.value })}
            disabled={!!isReadOnly}
          />
        ),
      },
      {
        id: "starttime",
        label: translate("Start Time"),
        minWidth: 130,
        format: (_: any, row: any) => (
          <FormTimePicker
            value={parseTimeValue(row.starttime)}
            onChange={(val) => {
              const starttime = val ? val.format("HH:mm") : "";
              const duration = computeDuration(starttime, row.endtime) ?? row.duration;
              updateBreakRow(row.id, { starttime, duration });
            }}
            disabled={!!isReadOnly}
            hideLabel
          />
        ),
      },
      {
        id: "endtime",
        label: translate("End Time"),
        minWidth: 130,
        format: (_: any, row: any) => (
          <FormTimePicker
            value={parseTimeValue(row.endtime)}
            onChange={(val) => {
              const endtime = val ? val.format("HH:mm") : "";
              const duration = computeDuration(row.starttime, endtime) ?? row.duration;
              updateBreakRow(row.id, { endtime, duration });
            }}
            disabled={!!isReadOnly}
            hideLabel
          />
        ),
      },
      {
        id: "duration",
        label: translate("Duration (hr)"),
        minWidth: 120,
        format: (_: any, row: any) => (
          <FormInput size="small" value={row.duration ?? ""} disabled />
        ),
      },
      {
        id: "delete",
        label: "",
        minWidth: 60,
        format: (_: any, row: any) => (
          <IconButton onClick={() => removeBreakRow(row.id)} size="small" disabled={!!isReadOnly}>
            <DeleteIcon fontSize="small" />
          </IconButton>
        ),
      },
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

    return (
      <FormThemeWrapper>
        <Box sx={{ p: 0 }}>
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
                {translate("Basic Information")}
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
                    value={formData.code || ""}
                    disabled
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <LookupDropdown
                    label={translate("Shift Type")}
                    value={formData.shifttypeid ?? ""}
                    options={normalizedShiftTypes}
                    getOptionLabel={(opt) => getLovLabel(opt)}
                    onSelect={(row) =>
                      setFormData({
                        ...formData,
                        shifttypeid: row.id,
                        shifttypename: row.stxt || getLovLabel(row),
                      })
                    }
                    onSearchClick={() => setLookupType("shiftType")}
                    placeholder={translate("Select Shift Type")}
                    hideDropdownArrow
                    disabled={!!isReadOnly}
                    error={!!isSubmitted && !formData.shifttypeid}
                  />
                </Box>
              </Grid>
              <Grid item xs={12}>


                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    label={translate("Shift Name")}
                    required
                    placeholder={translate("Enter Shift Name")}
                    value={formData.stxt || ""}
                    onChange={(e) => setFormData({ ...formData, stxt: e.target.value })}
                    error={!!isSubmitted && !formData.stxt}
                    disabled={!!isReadOnly}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormTimePicker
                    label={translate("Start Time")}
                    value={parseTimeValue(formData.starttime)}
                    onChange={(val) =>
                      setFormData({
                        ...formData,
                        starttime: val ? val.format("HH:mm") : "",
                      })
                    }
                    disabled={!!isReadOnly}
                    error={!!isSubmitted && !formData.starttime}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormTimePicker
                    label={translate("End Time")}
                    value={parseTimeValue(formData.endtime)}
                    onChange={(val) =>
                      setFormData({
                        ...formData,
                        endtime: val ? val.format("HH:mm") : "",
                      })
                    }
                    disabled={!!isReadOnly}
                    error={!!isSubmitted && !formData.endtime}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    type="number"
                    label={translate("Grace Time (Late)")}
                    value={formData.gracetime || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, gracetime: e.target.value })
                    }
                    disabled={!!isReadOnly}
                    error={!!isSubmitted && (!formData.gracetime || Number(formData.gracetime) <= 0)}
                  />
                </Box>
              </Grid>
              <Grid item xs={12} md={6}>
                <Box sx={{ mb: 1.5 }}>
                  <FormInput
                    fullWidth
                    size="small"
                    type="number"
                    label={translate("Grace Time (Early Departure)")}
                    value={formData.gracetimeed || ""}
                    onChange={(e) =>
                      setFormData({ ...formData, gracetimeed: e.target.value })
                    }
                    disabled={!!isReadOnly}
                    error={!!isSubmitted && (!formData.gracetimeed || Number(formData.gracetimeed) <= 0)}
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>

          <Box sx={{ mt: 3 }}>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                mb: 2,
                gap: 1.5,
                borderTop: "1px solid #E2E8F0",
                pt: 2,
              }}
            >
              <Typography
                sx={{
                  fontWeight: 800,
                  color: "var(--lms-text-main)",
                  fontSize: "0.95rem",
                }}
              >
                {translate("Weekly Off Day(s)")}
              </Typography>
            </Box>
            <Grid container spacing={1}>
              {[
                { key: "womonday", label: "Monday" },
                { key: "wotuesday", label: "Tuesday" },
                { key: "wowednesday", label: "Wednesday" },
                { key: "wothuresday", label: "Thursday" },
                { key: "wofriday", label: "Friday" },
                { key: "wosaturday", label: "Saturday" },
                { key: "wosunday", label: "Sunday" },
              ].map((day) => (
                <Grid item xs={12} sm={6} md={3} key={day.key}>
                  <LmsCheckbox
                    checked={!!formData[day.key]}
                    onChange={(checked) =>
                      setFormData({ ...formData, [day.key]: checked })
                    }
                    formControlSx={{ m: 0 }}
                    label={
                      <Typography sx={{ fontSize: "0.85rem", fontWeight: 500 }}>
                        {translate(day.label)}
                      </Typography>
                    }
                    disabled={!!isReadOnly}
                  />
                </Grid>
              ))}
            </Grid>
          </Box>

          <Box sx={{ mt: 3 }}>
            <Paper
              elevation={0}
              sx={{
                border: "1px solid var(--lms-border)",
                borderRadius: "12px",
                overflow: "hidden",
                bgcolor: "#fff",
              }}
            >
              <SchemaTable
                title={translate("Break Schedule")}
                addButtonLabel={translate("Add Break")}
                columns={breakColumns}
                rows={formData.breaks || []}
                onAddClick={addBreakRow}
                actions={false}
                showEditAction={false}
                hideBreadcrumbs={true}
                hideTitle={false}
                pagination={false}
                filtersEnabled={false}
                downloadEnabled={false}
                stickyHeader={true}
                rowHoverBg="rgba(var(--lms-primary-rgb), 0.04)"
                showAddButton={!isReadOnly}
              />
            </Paper>
          </Box>

          <GenericSearchModal
            open={lookupType === "shiftType"}
            onClose={() => setLookupType(null)}
            title={translate("Shift Type")}
            columns={[
              { id: "code", label: translate("Code"), minWidth: 80 },
              { id: "stxt", label: translate("Name"), minWidth: 200 },
              { id: "ltxt", label: translate("Description"), minWidth: 220 },
            ]}
            data={normalizedShiftTypes}
            onSelect={(row) => {
              setFormData({
                ...formData,
                shifttypeid: row.id,
                shifttypename: row.stxt || getLovLabel(row),
              });
              setLookupType(null);
            }}
            selectedValue={formData.shifttypeid ?? ""}
            selectedField="id"
          />


        </Box>
      </FormThemeWrapper>
    );
  };

export default CustomShift;
