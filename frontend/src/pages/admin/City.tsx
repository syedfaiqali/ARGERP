import { syncRegionalLocations } from "@/services/regionalService";
import React, { useEffect, useState, useMemo} from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsToaster } from "@/components/LmsToaster";
import {
  getGlobalCountries,
  fetchGlobalCountries,
  CountrySearchModal,
} from "@/components/CountrySearchModal";
import {
  GetService,
  getGlobalStates,
  fetchGlobalStates,
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { citySlice } from "@/store/organizationSlices";

interface CityProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const makeInitial = () => ({
  cityid: 0,
  citycode: "",
  cityname: "",
  stateid: 0,
  statename: "",
  countryid: 0,
  countryname: "",
  dialcode: "",
  isactive: true,
});

const CITY_URL = "/api/organization/City/v3";
const normalizeCity = (row: any) => ({
  ...makeInitial(), ...row,
  id: Number(row.cityid ?? row.cityId ?? row.CityId ?? row.id ?? 0),
  cityid: Number(row.cityid ?? row.cityId ?? row.CityId ?? row.id ?? 0),
  citycode: String(row.citycode ?? row.cityCode ?? row.CityCode ?? ""),
  cityname: String(row.cityname ?? row.cityName ?? row.CityName ?? ""),
  countryid: Number(row.countryid ?? row.countryId ?? row.CountryId ?? 0),
  countryname: String(row.countryname ?? row.countryName ?? row.CountryName ?? ""),
  stateid: Number(row.stateid ?? row.stateId ?? row.StateId ?? 0),
  statename: String(row.statename ?? row.stateName ?? row.StateName ?? ""),
  dialcode: String(row.dialcode ?? row.dialCode ?? row.DialCode ?? ""),
});

const validateCity = (data: any, rows: any[], states: any[]): Record<string, string> => {
  const errors: Record<string, string> = {};
  const code = String(data.citycode || "").trim();
  const name = String(data.cityname || "").trim();
  if (!code) errors.citycode = "Code is required";
  else if (code.length > 10) errors.citycode = "City Code must be 10 characters or less";
  if (!name) errors.cityname = "City is required";
  else if (name.length > 100) errors.cityname = "City Name must be 100 characters or less";
  if (!(Number(data.countryid) > 0)) errors.countryid = "Country is required";
  if (!(Number(data.stateid) > 0)) errors.stateid = "State is required";
  const state = states.find(item => Number(item.stateid ?? item.stateId ?? item.id) === Number(data.stateid));
  if (state && Number(state.countryid ?? state.countryId) !== Number(data.countryid)) errors.stateid = "Selected state does not belong to the selected country";
  const others = rows.filter(row => Number(row.cityid ?? row.id) !== Number(data.cityid));
  if (code && others.some(row => String(row.citycode || "").trim().toLowerCase() === code.toLowerCase())) errors.citycode = "City Code already exists";
  if (name && others.some(row => Number(row.countryid) === Number(data.countryid) && Number(row.stateid) === Number(data.stateid) && String(row.cityname || "").trim().toLowerCase() === name.toLowerCase())) errors.cityname = "City already exists against selected country and state";
  return errors;
};

const cityApiErrors = (error: any, fallback: string) => {
  const payload = error?.response?.data ?? error?.data ?? error;
  const details = payload?.errors ?? payload?.Errors;
  const fields: Record<string, string> = {};
  const messages: string[] = [];
  const fieldNames: Record<string, string> = {citycode: 'citycode', cityname: 'cityname', countryid: 'countryid', stateid: 'stateid', dialcode: 'dialcode'};
  if (details && !Array.isArray(details) && typeof details === 'object') {
    for (const [key, value] of Object.entries(details)) {
      const text = (Array.isArray(value) ? value : [value]).map(String).join(' ');
      messages.push(text);
      const field = fieldNames[key.split('.').pop()!.toLowerCase()];
      if (field) fields[field] = text;
    }
  } else if (Array.isArray(details)) {
    messages.push(...details.map(item => typeof item === 'string' ? item : item?.description ?? item?.Description ?? item?.message ?? item?.Message).filter(Boolean));
  }
  if (!messages.length) messages.push(payload?.error ?? payload?.description ?? payload?.Description ?? payload?.message ?? payload?.Message ?? fallback);
  for (const message of messages) {
    if (/city code|^code\b/i.test(message)) fields.citycode ||= message;
    else if (/city name|city already exists/i.test(message)) fields.cityname ||= message;
    else if (/state/i.test(message)) fields.stateid ||= message;
    else if (/country/i.test(message)) fields.countryid ||= message;
  }
  return { fields, messages };
};

const cityResponse = (response: any) => {
  const body = response?.data ?? response;
  if (response?.status >= 400 || body === false || body?.isSuccessCall === false || body?.IsSuccessCall === false) {
    throw Object.assign(new Error('City request failed'), { response });
  }
  return body?.data ?? body?.Data ?? body;
};

const City: React.FC<CityProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [countryModalOpen, setCountryModalOpen] = useState(false);
  const [stateModalOpen, setStateModalOpen] = useState(false);
  const [globalCountries, setGlobalCountries] = useState<any[]>(getGlobalCountries());
  const [globalStates, setGlobalStates] = useState<any[]>(getGlobalStates());
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const {
    rows: cachedRows,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgCity", citySlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [serverErrors, setServerErrors] = useState<Record<string, string>>({});

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  useEffect(() => {
    const load = async () => {
      if (globalCountries.length === 0) {
        const fetched = await fetchGlobalCountries();
        setGlobalCountries(fetched);
      }
      if (globalStates.length === 0) {
        const fetchedStates = await fetchGlobalStates();
        setGlobalStates(fetchedStates);
      }
    };
    load();
  }, []);

  useEffect(() => {
    if (!["new", "edit"].includes(viewMode) || !formData.countryid) return;
    let active = true;
    void syncRegionalLocations(formData.countryid).then(() => GetService("/api/organization/State/v3?PageNumber=1&PageSize=5000&countryId=" + formData.countryid))
      .then(response => { if (active) { const body = response?.data ?? response; setGlobalStates(body?.items ?? body?.Items ?? body?.data ?? body?.Data ?? []); } })
      .catch(() => { if (active) { setToasterMessages([translate("Could not load states from provider")]); setToasterSeverity("error"); setShowToaster(true); } });
    return () => { active = false; };
  }, [viewMode, formData.countryid]);
  // sNo is a display index, derived here rather than cached.
  const rows = useMemo<any[]>(
    () => cachedRows.map((row: any, index: number) => ({ ...row, sNo: index + 1 })),
    [cachedRows],
  );

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New City"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit City"));
    else if (viewMode === "view") onSubViewChange(translate("View City"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
      setFormData(makeInitial());
      setIsSubmitted(false);
      setServerErrors({});
    }
  }, [resetKey]);

  const fail = (msg: string | string[]) => {
    setToasterMessages(Array.isArray(msg) ? msg : [msg]);
    setToasterSeverity("error");
    setShowToaster(true);
  };

  const success = (msg: string) => {
    setToasterMessages([msg]);
    setToasterSeverity("success");
    setShowToaster(true);
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_, __, i) => (i ?? 0) + 1 },
    { id: "citycode", label: translate("Code"), minWidth: 100 },
    { id: "cityname", label: translate("City"), minWidth: 200 },
    { id: "countryname", label: translate("Country"), minWidth: 150 },
    { id: "statename", label: translate("State"), minWidth: 150 },
    { id: "dialcode", label: translate("DialCode"), minWidth: 100 },
  ];

  const handleSave = async () => {
    if (apiLoading || viewMode === 'view') return;
    setIsSubmitted(true); setServerErrors({});
    const errors = validateCity(formData, rows, globalStates);
    if (Object.keys(errors).length) { setIsGeneralExpanded(true); fail(Object.values(errors).map(translate)); return; }
    try {
      setApiLoading(true);
      const payload = {
        citycode: formData.citycode.trim(), cityname: formData.cityname.trim(),
        countryid: Number(formData.countryid), stateid: Number(formData.stateid),
        dialcode: String(formData.dialcode || '').trim(), isactive: formData.isactive ?? true,
      };
      const saved = cityResponse(formData.cityid
        ? await PutService(`${CITY_URL}/${formData.cityid}`, payload)
        : await PostService(CITY_URL, payload));
      const record = normalizeCity({ ...formData, ...payload, ...saved, cityid: Number(saved?.cityid ?? saved?.cityId ?? saved?.CityId ?? saved?.id ?? formData.cityid) });
      if (record.cityid > 0) dispatch(citySlice.actions.upsertRow(record));
      else await dispatch(citySlice.fetchAll(true)).unwrap();
      setViewMode("list");
      setIsSubmitted(false);
      success(translate(formData.cityid ? "Record updated successfully" : "Record saved successfully"));
    } catch (error: any) {
      const details = cityApiErrors(error, translate('Failed to save city'));
      setServerErrors(details.fields); setIsGeneralExpanded(true); fail(details.messages);
    }
    finally { setApiLoading(false); }
  };

  const handleDelete = async (row: any) => {
    const cityId = Number(row.cityid ?? row.id ?? 0);
    if (!cityId || apiLoading) return;
    try {
      setApiLoading(true);
      cityResponse(await DeleteService(`${CITY_URL}/${cityId}`));
      dispatch(citySlice.actions.removeRow(Number(cityId)));
      fail(translate("Record deleted successfully"));
    } catch (error: any) {
      fail(cityApiErrors(error, translate("Failed to delete city")).messages);
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
          transform: expanded ? "rotate(0deg)" : "rotate(90deg)",
        }}
      />
      {!expanded && <Box sx={{ width: "2px", height: 8, bgcolor: "var(--lms-primary)", position: "absolute" }} />}
    </Box>
  );

  const isReadOnly = viewMode === "view";
  const fieldErrors = { ...(isSubmitted ? validateCity(formData, rows, globalStates) : {}), ...serverErrors };
  const changeFields = (changes: Record<string, any>) => {
    setFormData((current: any) => ({ ...current, ...changes }));
    setServerErrors(current => Object.fromEntries(Object.entries(current).filter(([key]) => !(key in changes))));
  };
  const openForm = (row: any, mode: 'new' | 'edit' | 'view') => {
    setFormData(normalizeCity(row)); setViewMode(mode); setIsSubmitted(false); setServerErrors({}); setIsGeneralExpanded(true);
  };

  const states = globalStates
    .filter((s: any) => !formData.countryid || Number(s.countryid ?? s.countryId ?? 0) === Number(formData.countryid))
    .map((s: any) => ({
      stateid: Number(s.stateid ?? s.stateId ?? 0),
      statename: s.statename || s.stateName || "",
      countryid: Number(s.countryid ?? s.countryId ?? 0),
      countryname: s.countryname || s.countryName || "",
      statecode: s.statecode || s.code || "",
    }))
    .filter((s: any, index: number, arr: any[]) => {
      const firstIdx = arr.findIndex((x) => x.stateid === s.stateid);
      return firstIdx === index && s.stateid > 0;
    });

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable
          title={translate("City")}
          addButtonLabel={translate("New City")}
          columns={columns}
          rows={rows}
          onAddClick={() => openForm(makeInitial(), 'new')}
          onEditClick={(row) => openForm(row, 'edit')}
          onViewClick={(row) => openForm(row, 'view')}
          onDelete={handleDelete}
          showToaster={false}
          hideTitle
          hideBreadcrumbs
        />
      ) : (
        <Box sx={{ mt: 0 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New City") : viewMode === "edit" ? translate("Edit City") : translate("View City")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
              >
                {translate("Back to List")}
              </Button>
              {!isReadOnly && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSave}
                  disabled={loading}
                  sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>

          <FormThemeWrapper>
            <Box sx={{ p: 0 }}>
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  mb: 2,
                  cursor: "pointer",
                  userSelect: "none",
                  "&:hover": {
                    "& .expand-icon-container": { bgcolor: "rgba(var(--lms-primary-rgb), 0.05)" },
                  },
                }}
                onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}
              >
                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                  <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
                  <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", fontSize: "0.95rem" }}>
                    <T>General Information</T>
                  </Typography>
                </Box>
                <Box className="expand-icon-container" sx={{ transition: "background-color 0.3s ease", borderRadius: "50%", p: 0.5 }}>
                  {renderExpandIcon(isGeneralExpanded)}
                </Box>
              </Box>

              <Collapse in={isGeneralExpanded}>
                <Grid container spacing={3}>
                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Code")}
                      required
                      maxLength={10}
                      debounceMs={0}
                      placeholder={translate("Enter Code")}
                      value={formData.citycode || ""}
                      onChange={(e) => changeFields({ citycode: e.target.value })}
                      disabled={isReadOnly}
                      error={Boolean(fieldErrors.citycode)}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("City")}
                      required
                      maxLength={100}
                      debounceMs={0}
                      placeholder={translate("Enter City")}
                      value={formData.cityname || ""}
                      onChange={(e) => changeFields({ cityname: e.target.value })}
                      disabled={isReadOnly}
                      error={Boolean(fieldErrors.cityname)}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <LookupDropdown
                      label={translate("Country")}
                      required
                      value={formData.countryname || ""}
                      options={globalCountries}
                      getOptionLabel={(o) => o.countryname || o.countryName || o}
                      onSelect={(o) => changeFields({ countryid: Number(o.countryid ?? o.countryId ?? 0), countryname: o.countryname || o.countryName || "", stateid: 0, statename: "" })}
                      onClear={() => changeFields({ countryid: 0, countryname: '', stateid: 0, statename: '' })}
                      onSearchClick={() => setCountryModalOpen(true)}
                      error={Boolean(fieldErrors.countryid)}
                      disabled={isReadOnly}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <LookupDropdown
                      label={translate("State")}
                      required
                      value={formData.statename || ""}
                      options={states}
                      getOptionLabel={(o) => o.statename || o.stateName || o}
                      onSelect={(o) => changeFields({ stateid: Number(o.stateid ?? o.stateId ?? 0), statename: o.statename || o.stateName || "", countryid: Number(o.countryid ?? formData.countryid ?? 0), countryname: o.countryname || formData.countryname || "" })}
                      onClear={() => changeFields({ stateid: 0, statename: '' })}
                      onSearchClick={() => setStateModalOpen(true)}
                      error={Boolean(fieldErrors.stateid)}
                      disabled={isReadOnly}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Dial Code")}
                      placeholder={translate("Enter Dial Code")}
                      value={formData.dialcode || ""}
                      onChange={(e) => changeFields({ dialcode: e.target.value })}
                      disabled={isReadOnly}
                      debounceMs={0}
                      error={Boolean(fieldErrors.dialcode)}
                    />
                  </Grid>
                </Grid>
              </Collapse>
            </Box>
          </FormThemeWrapper>
        </Box>
      )}
      <CountrySearchModal
        open={countryModalOpen}
        onClose={() => setCountryModalOpen(false)}
        onSelect={(val) => {
          changeFields({ countryid: Number(val.countryid ?? val.countryId ?? 0), countryname: val.countryname || val.countryName || "", stateid: 0, statename: "" });
          setCountryModalOpen(false);
        }}
      />
      <GenericSearchModal
        open={stateModalOpen}
        onClose={() => setStateModalOpen(false)}
        title={translate("State")}
        columns={[{ id: "statecode", label: translate("Code"), minWidth: 100 }, { id: "statename", label: translate("State"), minWidth: 200 }, { id: "countryname", label: translate("Country"), minWidth: 200 }]}
        data={globalStates.map(s => ({ stateid: Number(s.stateid ?? s.stateId ?? 0), statecode: s.statecode || s.code || "", statename: s.statename || s.stateName || "", countryid: Number(s.countryid ?? s.countryId ?? 0), countryname: s.countryname || s.countryName || "" })).filter(row => !formData.countryid ? true : row.countryid === formData.countryid)}
        onSelect={(val) => {
          changeFields({ stateid: Number(val.stateid ?? 0), statename: val.statename || "", countryid: Number(val.countryid ?? formData.countryid ?? 0), countryname: val.countryname || formData.countryname || "" });
          setStateModalOpen(false);
        }}
      />
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default City;
