import React, { useEffect, useState, useMemo} from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";
import {
  getGlobalCountries,
  fetchGlobalCountries,
  CountrySearchModal,
} from "@/components/CountrySearchModal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { stateSlice } from "@/store/organizationSlices";

interface StateProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

type StateRow = {
  id: number;
  stateid: number;
  sNo: number;
  statecode: string;
  statename: string;
  countryid: number;
  countryname: string;
  isactive: boolean;
};

const makeInitial = () => ({
  stateid: 0,
  statecode: "",
  statename: "",
  countryid: 0,
  countryname: "",
  isactive: true,
});

const STATE_URL = '/api/organization/State/v3';
const normalizeState = (row: any) => ({
  ...makeInitial(), ...row,
  id: Number(row.stateid ?? row.stateId ?? row.StateId ?? row.id ?? 0),
  stateid: Number(row.stateid ?? row.stateId ?? row.StateId ?? row.id ?? 0),
  statecode: String(row.statecode ?? row.stateCode ?? row.StateCode ?? ''),
  statename: String(row.statename ?? row.stateName ?? row.StateName ?? ''),
  countryid: Number(row.countryid ?? row.countryId ?? row.CountryId ?? 0),
  countryname: String(row.countryname ?? row.countryName ?? row.CountryName ?? ''),
});
const validateState = (data: any, rows: StateRow[]): Record<string, string> => {
  const errors: Record<string, string> = {};
  const code = String(data.statecode || '').trim();
  const name = String(data.statename || '').trim();
  if (!code) errors.statecode = 'Code is required';
  else if (code.length > 5) errors.statecode = 'State Code must be 5 characters or less';
  if (!name) errors.statename = 'State is required';
  else if (name.length > 150) errors.statename = 'State Name must be 150 characters or less';
  if (!(Number(data.countryid) > 0)) errors.countryid = 'Country is required';
  if (code && rows.some(row => row.stateid !== Number(data.stateid) && row.countryid === Number(data.countryid) && row.statecode.trim().toLowerCase() === code.toLowerCase()))
    errors.statecode = 'State Code already exists for the selected country';
  if (name && rows.some(row => row.stateid !== Number(data.stateid) && row.countryid === Number(data.countryid) && row.statename.trim().toLowerCase() === name.toLowerCase()))
    errors.statename = "State Name already exists for the selected country";
  return errors;
};
const stateApiErrors = (error: any, fallback: string) => {
  const payload = error?.response?.data ?? error?.data ?? error;
  const details = payload?.errors ?? payload?.Errors;
  const fields: Record<string, string> = {};
  const messages: string[] = [];
  const fieldNames: Record<string, string> = {statecode: 'statecode', statename: 'statename', countryid: 'countryid'};
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
    if (/state code|^code\b/i.test(message)) fields.statecode ||= message;
    else if (/state name|state is required/i.test(message)) fields.statename ||= message;
    else if (/country/i.test(message)) fields.countryid ||= message;
  }
  return { fields, messages };
};
const stateResponse = (response: any) => {
  const body = response?.data ?? response;
  if (response?.status >= 400 || body === false || body?.isSuccessCall === false || body?.IsSuccessCall === false)
    throw Object.assign(new Error('State request failed'), { response });
  return body?.data ?? body?.Data ?? body;
};

const State: React.FC<StateProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [countryModalOpen, setCountryModalOpen] = useState(false);
  const [globalCountries, setGlobalCountries] = useState<any[]>(getGlobalCountries());
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const {
    rows: cachedRows,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgState", stateSlice.fetchAll);
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
    };
    load();
  }, []);

  // sNo is a display index, derived here rather than cached.
  const rows = useMemo<StateRow[]>(
    () => cachedRows.map((row: any, index: number) => ({ ...normalizeState(row), sNo: index + 1 })),
    [cachedRows],
  );

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New State"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit State"));
    else if (viewMode === "view") onSubViewChange(translate("View State"));
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
    { id: "statecode", label: translate("Code"), minWidth: 100 },
    { id: "statename", label: translate("State"), minWidth: 200 },
    { id: "countryname", label: translate("Country"), minWidth: 150 },
  ];

  const handleSave = async () => {
    if (apiLoading || viewMode === 'view') return;
    setIsSubmitted(true); setServerErrors({});
    const errors = validateState(formData, rows);
    if (Object.keys(errors).length) { setIsGeneralExpanded(true); fail(Object.values(errors).map(translate)); return; }
    try {
      setApiLoading(true);
      const payload = { statecode: formData.statecode.trim(), statename: formData.statename.trim(), countryid: Number(formData.countryid), isactive: formData.isactive ?? true };
      const saved = stateResponse(formData.stateid
        ? await PutService(`${STATE_URL}/${formData.stateid}`, payload)
        : await PostService(STATE_URL, payload));
      const record = normalizeState({ ...formData, ...payload, ...saved, stateid: Number(saved?.stateid ?? saved?.stateId ?? saved?.StateId ?? saved?.id ?? formData.stateid) });
      if (record.stateid > 0) dispatch(stateSlice.actions.upsertRow(record));
      else await dispatch(stateSlice.fetchAll(true)).unwrap();
      setViewMode("list");
      setIsSubmitted(false);
      success(translate(formData.stateid ? 'Record updated successfully' : 'Record saved successfully'));
    } catch (error: any) {
      const details = stateApiErrors(error, translate('Failed to save state'));
      setServerErrors(details.fields); setIsGeneralExpanded(true); fail(details.messages);
    }
    finally { setApiLoading(false); }
  };

  const handleDelete = async (row: any) => {
    const stateId = Number(row.stateid ?? row.id ?? 0);
    if (!stateId || apiLoading) return;
    try {
      setApiLoading(true);
      stateResponse(await DeleteService(`${STATE_URL}/${stateId}`));
      dispatch(stateSlice.actions.removeRow(Number(stateId)));
      fail(translate("Record deleted successfully"));
    } catch (error: any) {
      fail(stateApiErrors(error, translate("Failed to delete state")).messages);
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
  const fieldErrors = { ...(isSubmitted ? validateState(formData, rows) : {}), ...serverErrors };
  const changeFields = (changes: Record<string, any>) => {
    setFormData((current: any) => ({ ...current, ...changes }));
    setServerErrors(current => Object.fromEntries(Object.entries(current).filter(([key]) => !(key in changes))));
  };
  const openForm = (row: any, mode: 'new' | 'edit' | 'view') => {
    setFormData(normalizeState(row)); setViewMode(mode); setIsSubmitted(false); setServerErrors({}); setIsGeneralExpanded(true);
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable showToaster={false}
          title={translate("State")}
          addButtonLabel={translate("New State")}
          columns={columns}
          rows={rows}
          onAddClick={() => openForm(makeInitial(), 'new')}
          onEditClick={(row) => openForm(row, 'edit')}
          onViewClick={(row) => openForm(row, 'view')}
          onDelete={handleDelete}
          hideTitle
          hideBreadcrumbs
        />
      ) : (
        <Box sx={{ mt: 0 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New State") : viewMode === "edit" ? translate("Edit State") : translate("View State")}
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
                  disabled={apiLoading}
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
                  <Grid item xs={12} md={6}>
                    <FormInput
                      label={translate("Code")}
                      required
                      placeholder={translate("Enter Code")}
                      value={formData.statecode || ""}
                      onChange={(e) => changeFields({ statecode: e.target.value })}
                      disabled={isReadOnly}
                      debounceMs={0}
                      maxLength={5}
                      error={Boolean(fieldErrors.statecode)}
                    />
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <FormInput
                      label={translate("State")}
                      required
                      placeholder={translate("Enter State")}
                      value={formData.statename || ""}
                      onChange={(e) => changeFields({ statename: e.target.value })}
                      disabled={isReadOnly}
                      debounceMs={0}
                      maxLength={150}
                      error={Boolean(fieldErrors.statename)}
                    />
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <LookupDropdown
                      label={translate("Country")}
                      required
                      value={formData.countryname || ""}
                      options={globalCountries}
                      getOptionLabel={(o) => o.countryname || o.countryName || o}
                      onSelect={(o) => changeFields({ countryid: Number(o.countryid ?? o.countryId ?? 0), countryname: o.countryname || o.countryName || "" })}
                      onClear={() => changeFields({ countryid: 0, countryname: '' })}
                      onSearchClick={() => setCountryModalOpen(true)}
                      error={Boolean(fieldErrors.countryid)}
                      disabled={isReadOnly}
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
          changeFields({ countryid: Number(val.countryid ?? val.countryId ?? 0), countryname: val.countryname || val.countryName || "" });
          setCountryModalOpen(false);
        }}
      />
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default State;
