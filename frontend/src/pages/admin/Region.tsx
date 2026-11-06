import { organizationTextErrors } from "@/utils/organizationValidation";
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
  CheckemptyString,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { regionSlice } from "@/store/organizationSlices";

interface RegionProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

type RegionRow = {
  id: number;
  regionid: number;
  sNo: number;
  regioncode: string;
  regionname: string;
  countryid: number;
  countryname: string;
  isactive: boolean;
};

const makeInitial = () => ({
  regionid: 0,
  regioncode: "",
  regionname: "",
  countryid: 0,
  countryname: "",
  isactive: true,
});

const Region: React.FC<RegionProps> = ({ onSubViewChange, resetKey }) => {
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
  } = useCachedRows<any>("orgRegion", regionSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);

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
  const rows = useMemo<RegionRow[]>(
    () => cachedRows.map((row: any, index: number) => ({ ...row, sNo: index + 1 })),
    [cachedRows],
  );

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Region"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Region"));
    else if (viewMode === "view") onSubViewChange(translate("View Region"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
      setFormData(makeInitial());
      setIsSubmitted(false);
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
    { id: "regioncode", label: translate("Code"), minWidth: 100 },
    { id: "regionname", label: translate("Region"), minWidth: 200 },
    { id: "countryname", label: translate("Country"), minWidth: 150 },
  ];

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = organizationTextErrors(formData, rows, [
      {field: "regioncode", label: "Region Code", maxLength: 20, duplicate: true},
      {field: "regionname", label: "Region Name", maxLength: 150, duplicate: true},
    ], "regionid").map(message => translate(message));
    if (!CheckemptyString(formData.regioncode)) errors.push(translate("Code is required"));
    if (!CheckemptyString(formData.regionname)) errors.push(translate("Region is required"));
    if (!formData.countryid || Number(formData.countryid) <= 0) errors.push(translate("Country is required"));

    if (errors.length > 0) {
      fail(errors);
      return;
    }

    const regionId = Number(formData.regionid ?? formData.id ?? 0);
    const payload = {
      regionid: regionId || 0,
      regioncode: formData.regioncode?.trim(),
      regionname: formData.regionname?.trim(),
      countryid: Number(formData.countryid),
      isactive: !!formData.isactive,
    };

    try {
      setApiLoading(true);
      let saved: any;
      if (regionId && regionId > 0) {
        const response: any = await PutService(`/api/organization/Region/v3/${regionId}`, payload);
        saved = response?.data;
        success(translate("Record updated successfully"));
      } else {
        const response: any = await PostService("/api/organization/Region/v3", payload);
        saved = response?.data;
        success(translate("Record saved successfully"));
      }

      // Patch the cached row instead of reloading the whole list. countryname is a
      // denormalized display field the payload does not carry, so resolve it locally.
      const savedId = Number(saved?.regionid ?? saved?.id ?? regionId ?? 0);
      if (savedId > 0) {
        const country = globalCountries.find(
          (c: any) => Number(c.countryid ?? c.id) === payload.countryid,
        );
        dispatch(regionSlice.actions.upsertRow({
          ...payload,
          id: savedId,
          regionid: savedId,
          countryname: country?.countryname ?? "",
        }));
      } else {
        dispatch(regionSlice.fetchAll(true));
      }
      setViewMode("list");
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to save region"));
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    const regionId = Number(row.regionid ?? row.id ?? 0);
    if (!regionId || apiLoading) return;
    try {
      setApiLoading(true);
      const response: any = await DeleteService(`/api/organization/Region/v3/${regionId}`);
      const body = response?.data ?? response;
      if (response?.status >= 400 || body === false || body?.isSuccessCall === false || body?.IsSuccessCall === false)
        throw Object.assign(new Error('Failed to delete region'), { response });
      dispatch(regionSlice.actions.removeRow(Number(regionId)));
      fail(translate("Record deleted successfully"));
    } catch (error: any) {
      const body = error?.response?.data;
      fail(body?.error || body?.message || body?.Message || translate(error?.response?.status === 403
        ? "You do not have permission to delete this region" : "Failed to delete region"));
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

  return (
    <>
      {loading && <LmsOverlayLoader />}
      {viewMode === "list" ? (
        <SchemaTable
          title={translate("Region")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          onAddClick={() => { setFormData(makeInitial()); setViewMode("new"); setIsSubmitted(false); }}
          onEditClick={(row) => { setFormData(row); setViewMode("edit"); setIsSubmitted(false); }}
          onViewClick={(row) => { setFormData(row); setViewMode("view"); setIsSubmitted(false); }}
          showToaster={false}
          hideTitle
          hideBreadcrumbs
        />
      ) : (
        <Box sx={{ mt: 0 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? translate("New Region") : viewMode === "edit" ? translate("Edit Region") : translate("View Region")}
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
                      label={translate("Code*")}
                      placeholder={translate("Enter Code")}
                      value={formData.regioncode || ""}
                      onChange={(e) => setFormData({ ...formData, regioncode: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.regioncode}
                      helperText={isSubmitted && !formData.regioncode ? translate("Code is required") : ""}
                    />
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <FormInput
                      label={translate("Region*")}
                      placeholder={translate("Enter Region")}
                      value={formData.regionname || ""}
                      onChange={(e) => setFormData({ ...formData, regionname: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.regionname}
                      helperText={isSubmitted && !formData.regionname ? translate("Region is required") : ""}
                    />
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <LookupDropdown
                      label={translate("Country*")}
                      value={formData.countryname || ""}
                      options={globalCountries}
                      getOptionLabel={(o) => o.countryname || o.countryName || o}
                      onSelect={(o) => setFormData({ ...formData, countryid: Number(o.countryid ?? o.countryId ?? 0), countryname: o.countryname || o.countryName || "" })}
                      onSearchClick={() => setCountryModalOpen(true)}
                      error={isSubmitted && (!formData.countryid || Number(formData.countryid) <= 0)}
                      helperText={isSubmitted && (!formData.countryid || Number(formData.countryid) <= 0) ? translate("Country is required") : ""}
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
          setFormData({ ...formData, countryid: Number(val.countryid ?? val.countryId ?? 0), countryname: val.countryname || val.countryName || "" });
          setCountryModalOpen(false);
        }}
      />
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Region;
