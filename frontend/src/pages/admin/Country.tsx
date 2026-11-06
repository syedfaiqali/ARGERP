import React, { useEffect, useMemo, useState } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  PostService,
  PutService,
  DeleteService,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import {
  setGlobalCountries,
} from "@/components/CountrySearchModal";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { countrySlice } from "@/store/organizationSlices";
import type { CountryRow as CachedCountryRow } from "@/store/organizationSlices";

type CountryRow = {
  id: number;
  countryid: number;
  sNo: number;
  countrycode: string;
  countryname: string;
  isocode?: string;
  dialcode?: string;
  currencycode?: string;
  currencyname?: string;
  currencysymbol?: string;
  isactive: boolean;
};

interface CountryProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const makeInitial = () => ({
  countryid: 0,
  countrycode: "",
  countryname: "",
  isocode: "",
  dialcode: "",
  currencycode: "",
  currencyname: "",
  currencysymbol: "",
  isactive: true,
});

const COUNTRY_URL = '/api/organization/Country/v3';
const normalizeCountry = (row: any) => ({
  ...makeInitial(), ...row,
  id: Number(row.countryid ?? row.countryId ?? row.CountryId ?? row.id ?? 0),
  countryid: Number(row.countryid ?? row.countryId ?? row.CountryId ?? row.id ?? 0),
  countrycode: String(row.countrycode ?? row.countryCode ?? row.CountryCode ?? ''),
  countryname: String(row.countryname ?? row.countryName ?? row.CountryName ?? ''),
  isocode: String(row.isocode ?? row.isoCode ?? row.IsoCode ?? ''),
  dialcode: String(row.dialcode ?? row.dialCode ?? row.DialCode ?? ''),
});
const validateCountry = (data: any, rows: CountryRow[]): Record<string, string> => {
  const errors: Record<string, string> = {};
  for (const [key, label, limit] of [['countrycode', 'Code', 2], ['countryname', 'Country Name', 100], ['isocode', 'ISO Code', 5], ['dialcode', 'Dial Code', 10]] as const) {
    const value = String(data[key] || '').trim();
    if (!value && (key === 'countrycode' || key === 'countryname')) errors[key] = `${label} is required`;
    else if (value.length > limit) errors[key] = `${label} must be ${limit} characters or less`;
  }
  const others = rows.filter(row => row.countryid !== Number(data.countryid));
  for (const [key, label] of [['countrycode', 'Country Code'], ['countryname', 'Country Name']] as const) {
    const value = String(data[key] || '').trim().toLowerCase();
    if (value && others.some(row => String(row[key] || '').trim().toLowerCase() === value)) errors[key] = `${label} already exists`;
  }
  return errors;
};
const countryApiErrors = (error: any, fallback: string) => {
  const payload = error?.response?.data ?? error?.data ?? error;
  const details = payload?.errors ?? payload?.Errors;
  const fields: Record<string, string> = {};
  const messages: string[] = [];
  const fieldNames: Record<string, string> = {countrycode: 'countrycode', countryname: 'countryname', isocode: 'isocode', dialcode: 'dialcode'};
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
    if (/country code|^code\b/i.test(message)) fields.countrycode ||= message;
    else if (/country name/i.test(message)) fields.countryname ||= message;
  }
  return { fields, messages };
};
const countryResponse = (response: any) => {
  const body = response?.data ?? response;
  if (response?.status >= 400 || body === false || body?.isSuccessCall === false || body?.IsSuccessCall === false)
    throw Object.assign(new Error('Country request failed'), { response });
  return body?.data ?? body?.Data ?? body;
};

const Country: React.FC<CountryProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const dispatch = useAppDispatch();
  // Rows come from the store: revisiting this screen reuses the cache instead of refetching.
  const {
    rows: cachedRows,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<CachedCountryRow>("orgCountry", countrySlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [serverErrors, setServerErrors] = useState<Record<string, string>>({});

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  // sNo is a display index, so it is derived here rather than stored in the cache.
  const rows = useMemo<CountryRow[]>(
    () => cachedRows.map(normalizeCountry).sort((a, b) => a.countryname.localeCompare(b.countryname)).map((row, index) => ({ ...row, sNo: index + 1 })),
    [cachedRows],
  );

  // Keep the localStorage copy other screens read in step with the cache.
  useEffect(() => {
    if (!rowsLoading && !rowsError) setGlobalCountries(cachedRows);
  }, [cachedRows, rowsLoading, rowsError]);

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Country"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Country"));
    else if (viewMode === "view") onSubViewChange(translate("View Country"));
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
    { id: "sNo", label: translate("S.No"), minWidth: 60, format: (_, __, i) => (i ?? 0) + 1 },
    { id: "countrycode", label: translate("Code"), minWidth: 100 },
    { id: "countryname", label: translate("Country"), minWidth: 200 },
    { id: "isocode", label: translate("ISO Code"), minWidth: 100 },
    { id: "dialcode", label: translate("Dial Code"), minWidth: 100 },
  ];

  const handleSave = async (closeAfterSave = false) => {
    if (apiLoading || viewMode === 'view') return;
    setIsSubmitted(true);
    setServerErrors({});
    const errors = validateCountry(formData, rows);
    if (Object.keys(errors).length > 0) {
      setIsGeneralExpanded(true); fail(Object.values(errors).map(translate));
      return;
    }

    const countryId = formData.countryid ?? formData.id;
    const payload = {
      countryid: countryId || 0,
      countrycode: formData.countrycode?.trim(),
      countryname: formData.countryname?.trim(),
      isocode: formData.isocode?.trim(),
      dialcode: formData.dialcode?.trim(),
      currencycode: formData.currencycode?.trim(),
      currencyname: formData.currencyname?.trim(),
      currencysymbol: formData.currencysymbol?.trim(),
      isactive: !!formData.isactive,
    };

    try {
      setApiLoading(true);
      let saved: any;
      if (countryId && countryId > 0) {
        const response: any = await PutService(`/api/organization/Country/v3/${countryId}`, payload);
        saved = countryResponse(response);
      } else {
        const response: any = await PostService("/api/organization/Country/v3", payload);
        saved = countryResponse(response);
      }

      // Patch the cached row in place; a full reload is no longer needed.
      const savedId = Number(saved?.countryid ?? saved?.countryId ?? saved?.CountryId ?? saved?.id ?? countryId ?? 0);
      const record = normalizeCountry({ ...formData, ...payload, ...saved, countryid: savedId });
      if (savedId > 0) {
        dispatch(countrySlice.actions.upsertRow(record));
      } else {
        // Server did not echo an id back, so refresh rather than cache a half record.
        await dispatch(countrySlice.fetchAll(true)).unwrap();
      }
      setIsSubmitted(false);
      if (closeAfterSave || !savedId) setViewMode('list');
      else { setFormData(record); setViewMode('edit'); }
      success(translate(countryId ? 'Record updated successfully' : 'Record saved successfully'));
    } catch (error: any) {
      const details = countryApiErrors(error, translate('Failed to save country'));
      setServerErrors(details.fields); setIsGeneralExpanded(true); fail(details.messages);
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    const countryId = row.countryid ?? row.id;
    if (!countryId || apiLoading) return;
    try {
      setApiLoading(true);
      countryResponse(await DeleteService(`/api/organization/Country/v3/${countryId}`));
      dispatch(countrySlice.actions.removeRow(Number(countryId)));
      fail(translate("Record deleted successfully"));
    } catch (error: any) {
      fail(countryApiErrors(error, translate('Failed to delete country')).messages);
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
  const fieldErrors = { ...(isSubmitted ? validateCountry(formData, rows) : {}), ...serverErrors };
  const changeFields = (changes: Record<string, any>) => {
    setFormData((current: any) => ({ ...current, ...changes }));
    setServerErrors(current => Object.fromEntries(Object.entries(current).filter(([key]) => !(key in changes))));
  };
  const openForm = (row: any, mode: 'new' | 'edit' | 'view') => {
    setFormData(normalizeCountry(row)); setViewMode(mode); setIsSubmitted(false); setServerErrors({}); setIsGeneralExpanded(true);
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable
          title={translate("Country")}
          addButtonLabel={translate("New Country")}
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
        <Box sx={{ p: 2 }}>
          <Box sx={{ p: 2, display: "flex", flexWrap: 'wrap', gap: 2, justifyContent: "space-between", alignItems: "center" }}>
            <Typography id="country-form-title" variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Country") : viewMode === "edit" ? translate("Edit Country") : translate("View Country")}
            </Typography>
            <Box sx={{ display: "flex", flexWrap: 'wrap', gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                disabled={apiLoading}
                sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700 }}
              >
                {translate("Back to List")}
              </Button>
              {!isReadOnly && (<>
                <Button variant="outlined" disabled={apiLoading} onClick={() => openForm(makeInitial(), 'new')}>{translate('New')}</Button>
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={() => handleSave(false)}
                  disabled={apiLoading}
                  sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, px: 4 }}
                >
                  {translate("Save")}
                </Button>
                <Button variant="contained" disabled={apiLoading} onClick={() => handleSave(true)}>{translate('Save & Close')}</Button>
              </>
              )}
            </Box>
          </Box>

          <Box sx={{ px: 2, pb: 2 }}>
          <FormThemeWrapper>
            <Box sx={{ p: 0 }}>
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  mb: 2,
                  pt: 2,
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
                      maxLength={2}
                      debounceMs={0}
                      placeholder={translate("Enter Code")}
                      value={formData.countrycode || ""}
                      onChange={(e) => changeFields({ countrycode: e.target.value })}
                      disabled={isReadOnly}
                      error={Boolean(fieldErrors.countrycode)}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Country Name")}
                      required
                      maxLength={100}
                      debounceMs={0}
                      placeholder={translate("Enter Country Name")}
                      value={formData.countryname || ""}
                      onChange={(e) => changeFields({ countryname: e.target.value })}
                      disabled={isReadOnly}
                      error={Boolean(fieldErrors.countryname)}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("ISO Code")}
                      maxLength={5}
                      debounceMs={0}
                      error={Boolean(fieldErrors.isocode)}
                      placeholder={translate("Enter ISO Code")}
                      value={formData.isocode || ""}
                      onChange={(e) => changeFields({ isocode: e.target.value })}
                      disabled={isReadOnly}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
                    <FormInput
                      label={translate("Dial Code")}
                      maxLength={10}
                      debounceMs={0}
                      error={Boolean(fieldErrors.dialcode)}
                      placeholder={translate("Enter Dial Code")}
                      value={formData.dialcode || ""}
                      onChange={(e) => changeFields({ dialcode: e.target.value })}
                      disabled={isReadOnly}
                    />
                  </Grid>
                </Grid>
              </Collapse>
            </Box>
          </FormThemeWrapper>
          </Box>
        </Box>
      )}
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Country;
