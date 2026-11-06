import { organizationTextErrors } from "@/utils/organizationValidation";
import React, { useEffect, useMemo, useState } from "react";
import { Grid, Box, Typography, Collapse, Button } from "@mui/material";
import { ArrowBack as ArrowBackIcon, Save as SaveIcon } from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  PostService,
  PutService,
  DeleteService,
  CheckemptyString,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { orgSiteSlice } from "@/store/organizationSlices";

interface SiteProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

type LookupItem = {
  id: number;
  code: string;
  name: string;
  shortname?: string;
  description?: string;
  countryname?: string;
};

type SiteRow = {
  id: number;
  siteid: number;
  sNo: number;
  code: string;
  stxt: string;
  sitetypeid: number;
  sitetypename: string;
  siteforid: number;
  siteforname: string;
  regionid: number;
  regionname: string;
  forid?: number | null;
  forname?: string;
};

const makeInitial = () => ({
  siteid: 0,
  code: "",
  stxt: "",
  sitetypeid: 0,
  sitetypename: "",
  siteforid: 0,
  siteforname: "",
  regionid: 0,
  regionname: "",
  forid: 0,
  forname: "",
});

const Site: React.FC<SiteProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const dispatch = useAppDispatch();
  // The list and all four lookups are fetched in one parallel batch and then cached.
  const {
    rows: cachedRows,
    lookups,
    loading: rowsLoading,
    error: rowsError,
  } = useCachedRows<any>("orgSite", orgSiteSlice.fetchAll);
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);

  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  const [siteTypeModalOpen, setSiteTypeModalOpen] = useState(false);
  const [siteForModalOpen, setSiteForModalOpen] = useState(false);
  const [regionModalOpen, setRegionModalOpen] = useState(false);
  const [siteIdModalOpen, setSiteIdModalOpen] = useState(false);

  const buildSiteRows = (items: any[]): SiteRow[] => {
    return items.map((item: any, index: number) => ({
      id: Number(item.siteid ?? item.siteId ?? item.id ?? 0),
      siteid: Number(item.siteid ?? item.siteId ?? item.id ?? 0),
      sNo: index + 1,
      code: item.code ?? "",
      stxt: item.stxt ?? item.siteName ?? "",
      sitetypeid: Number(item.sitetypeid ?? item.siteTypeId ?? 0),
      sitetypename: item.sitetypename ?? item.siteTypeName ?? "",
      siteforid: Number(item.siteforid ?? item.siteForId ?? 0),
      siteforname: item.siteforname ?? item.siteForName ?? "",
      regionid: Number(item.regionid ?? item.regionId ?? 0),
      regionname: item.regionname ?? item.regionName ?? "",
      forid: item.forid ?? item.forId ?? null,
      forname: item.forname ?? item.forName ?? "",
    }));
  };

  const rows = useMemo<SiteRow[]>(
    () => buildSiteRows(cachedRows),
    [cachedRows],
  );

  // Lookups are cached raw; the same mapping as before is applied here.
  const siteTypes = useMemo<LookupItem[]>(
    () => (lookups?.siteTypes ?? []).map((x: any) => ({ id: Number(x.id ?? 0), code: x.code ?? "", name: x.name ?? "", shortname: x.shortname ?? x.shortName ?? x.name ?? "", description: x.description ?? x.name ?? "" })),
    [lookups],
  );
  const siteForOptions = useMemo<LookupItem[]>(
    () => (lookups?.siteFor ?? []).map((x: any) => ({ id: Number(x.id ?? 0), code: x.code ?? "", name: x.name ?? "", shortname: x.shortname ?? x.shortName ?? x.name ?? "", description: x.description ?? x.name ?? "" })),
    [lookups],
  );
  const regions = useMemo<LookupItem[]>(
    () => (lookups?.regions ?? []).map((x: any) => ({ id: Number(x.regionid ?? x.regionId ?? x.id ?? 0), code: x.regioncode ?? x.regionCode ?? "", name: x.regionname ?? x.regionName ?? "", countryname: x.countryname ?? x.countryName ?? "" })),
    [lookups],
  );
  const parentSites = useMemo<LookupItem[]>(
    () => (lookups?.parentSites ?? []).map((x: any) => ({ id: Number(x.id ?? 0), code: x.code ?? "", name: x.name ?? "" })),
    [lookups],
  );

  useEffect(() => {
    if (rowsError) fail(rowsError);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowsError]);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Site"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Site"));
    else if (viewMode === "view") onSubViewChange(translate("View Site"));
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
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "stxt", label: translate("Site Name"), minWidth: 220 },
    { id: "sitetypename", label: translate("Site Type"), minWidth: 160 },
    { id: "siteforname", label: translate("Site For"), minWidth: 160 },
    { id: "regionname", label: translate("Region"), minWidth: 180 },
  ];

  const handleSave = async () => {
    setIsSubmitted(true);
    const errors = organizationTextErrors(formData, rows, [
      {field: "code", label: "Code", maxLength: 10, duplicate: true},
      {field: "stxt", label: "Site Name", maxLength: 500, duplicate: true},
    ], "siteid").map(message => translate(message));
    if (!CheckemptyString(formData.code)) errors.push(translate("Code is required"));
    if (!CheckemptyString(formData.stxt)) errors.push(translate("Site Name is required"));
    if (!formData.sitetypeid || Number(formData.sitetypeid) <= 0) errors.push(translate("Site Type is required"));
    if (!formData.siteforid || Number(formData.siteforid) <= 0) errors.push(translate("Site For is required"));
    if (!formData.regionid || Number(formData.regionid) <= 0) errors.push(translate("Region is required"));

    if (errors.length > 0) {
      fail(errors);
      return;
    }

    const siteId = Number(formData.siteid ?? formData.id ?? 0);
    const payload = {
      siteid: siteId || 0,
      code: formData.code?.trim(),
      stxt: formData.stxt?.trim(),
      sitetypeid: Number(formData.sitetypeid),
      siteforid: Number(formData.siteforid),
      regionid: Number(formData.regionid),
      forid: formData.forid ? Number(formData.forid) : null,
    };

    try {
      setApiLoading(true);
      if (siteId && siteId > 0) {
        await PutService(`/api/organization/Site/v3/${siteId}`, payload);
        success(translate("Updated successfully"));
      } else {
        await PostService("/api/organization/Site/v3", payload);
        success(translate("Saved successfully"));
      }
      dispatch(orgSiteSlice.fetchAll(true));
      setViewMode("list");
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to save site"));
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    const siteId = Number(row.siteid ?? row.id ?? 0);
    if (!siteId) return;
    try {
      setApiLoading(true);
      await DeleteService(`/api/organization/Site/v3/${siteId}`);
      dispatch(orgSiteSlice.fetchAll(true));
      success(translate("Deleted successfully"));
    } catch (error: any) {
      fail(error?.response?.data?.error || translate("Failed to delete site"));
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
          title={translate("Site")}
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
              {viewMode === "new" ? translate("New Site") : viewMode === "edit" ? translate("Edit Site") : translate("View Site")}
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
                      value={formData.code || ""}
                      onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.code}
                      helperText={isSubmitted && !formData.code ? translate("Code is required") : ""}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormInput
                      label={translate("Site Name*")}
                      placeholder={translate("Enter Site Name")}
                      value={formData.stxt || ""}
                      onChange={(e) => setFormData({ ...formData, stxt: e.target.value })}
                      disabled={isReadOnly}
                      error={isSubmitted && !formData.stxt}
                      helperText={isSubmitted && !formData.stxt ? translate("Site Name is required") : ""}
                    />
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <LookupDropdown
                      label={translate("Site Type*")}
                      value={formData.sitetypename || ""}
                      options={siteTypes}
                      getOptionLabel={(o) => o?.name || o?.stxt || o || ""}
                      onSelect={(o) => setFormData({ ...formData, sitetypeid: Number(o.id ?? 0), sitetypename: o.shortname ?? o.name ?? "" })}
                      onSearchClick={() => setSiteTypeModalOpen(true)}
                      error={isSubmitted && (!formData.sitetypeid || Number(formData.sitetypeid) <= 0)}
                      helperText={isSubmitted && (!formData.sitetypeid || Number(formData.sitetypeid) <= 0) ? translate("Site Type is required") : ""}
                      disabled={isReadOnly}
                    />
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <LookupDropdown
                      label={translate("Site For*")}
                      value={formData.siteforname || ""}
                      options={siteForOptions}
                      getOptionLabel={(o) => o?.name || o?.stxt || o || ""}
                      onSelect={(o) => setFormData({ ...formData, siteforid: Number(o.id ?? 0), siteforname: o.shortname ?? o.name ?? "" })}
                      onSearchClick={() => setSiteForModalOpen(true)}
                      error={isSubmitted && (!formData.siteforid || Number(formData.siteforid) <= 0)}
                      helperText={isSubmitted && (!formData.siteforid || Number(formData.siteforid) <= 0) ? translate("Site For is required") : ""}
                      disabled={isReadOnly}
                    />
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <LookupDropdown
                      label={translate("Region*")}
                      value={formData.regionname || ""}
                      options={regions}
                      getOptionLabel={(o) => o?.name || o?.stxt || o || ""}
                      onSelect={(o) => setFormData({ ...formData, regionid: Number(o.id ?? 0), regionname: o.name ?? "" })}
                      onSearchClick={() => setRegionModalOpen(true)}
                      error={isSubmitted && (!formData.regionid || Number(formData.regionid) <= 0)}
                      helperText={isSubmitted && (!formData.regionid || Number(formData.regionid) <= 0) ? translate("Region is required") : ""}
                      disabled={isReadOnly}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <LookupDropdown
                      label={translate("Site ID")}
                      value={formData.forname || ""}
                      options={parentSites}
                      getOptionLabel={(o) => o?.name || o?.stxt || o || ""}
                      onSelect={(o) => setFormData({ ...formData, forid: Number(o.id ?? 0), forname: o.name ?? "" })}
                      onSearchClick={() => setSiteIdModalOpen(true)}
                      disabled={isReadOnly}
                    />
                  </Grid>
                </Grid>
              </Collapse>
            </Box>
          </FormThemeWrapper>
        </Box>
      )}
      <GenericSearchModal
        open={siteTypeModalOpen}
        onClose={() => setSiteTypeModalOpen(false)}
        title={translate("Site Type")}
        columns={[{ id: "code", label: translate("Code"), minWidth: 120 }, { id: "shortname", label: translate("Short Name"), minWidth: 220 }, { id: "description", label: translate("Description"), minWidth: 260 }]}
        data={siteTypes}
        onSelect={(val) => { setFormData({ ...formData, sitetypeid: Number(val.id ?? 0), sitetypename: val.shortname ?? val.name ?? "" }); setSiteTypeModalOpen(false); }}
      />
      <GenericSearchModal
        open={siteForModalOpen}
        onClose={() => setSiteForModalOpen(false)}
        title={translate("Site For")}
        columns={[{ id: "code", label: translate("Code"), minWidth: 120 }, { id: "shortname", label: translate("Short Name"), minWidth: 220 }, { id: "description", label: translate("Description"), minWidth: 260 }]}
        data={siteForOptions}
        onSelect={(val) => { setFormData({ ...formData, siteforid: Number(val.id ?? 0), siteforname: val.shortname ?? val.name ?? "" }); setSiteForModalOpen(false); }}
      />
      <GenericSearchModal
        open={regionModalOpen}
        onClose={() => setRegionModalOpen(false)}
        title={translate("Region")}
        columns={[{ id: "code", label: translate("Code"), minWidth: 120 }, { id: "name", label: translate("Region"), minWidth: 220 }, { id: "countryname", label: translate("Country"), minWidth: 220 }]}
        data={regions}
        onSelect={(val) => { setFormData({ ...formData, regionid: Number(val.id ?? 0), regionname: val.name ?? "" }); setRegionModalOpen(false); }}
      />
      <GenericSearchModal
        open={siteIdModalOpen}
        onClose={() => setSiteIdModalOpen(false)}
        title={translate("Site ID")}
        columns={[{ id: "code", label: translate("Code"), minWidth: 140 }, { id: "name", label: translate("Site Name"), minWidth: 260 }]}
        data={parentSites}
        onSelect={(val) => { setFormData({ ...formData, forid: Number(val.id ?? 0), forname: val.name ?? "" }); setSiteIdModalOpen(false); }}
      />
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity={toasterSeverity} disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Site;
