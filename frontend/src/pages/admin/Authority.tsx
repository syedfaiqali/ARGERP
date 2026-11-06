import React, { useState, useEffect, useRef } from "react";
import { Grid, Box, Typography, IconButton, Dialog, Collapse, Button } from "@mui/material";
import {
  Close as CloseIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
  DeleteService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows, useHrLovs } from "@/hooks";
import { authoritySlice } from "@/store/humanResourceSlices";

interface AuthorityProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const Authority: React.FC<AuthorityProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows: cachedRows, loading: rowsLoading } = useCachedRows<any>(
    "hrAuthority",
    authoritySlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;

  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    code: "",
    authorityTypeId: null,
    authorityType: "",
    authority: "",
    description: "",
    contactNo: "",
    phone: "",
    web: "",
    fax: "",
    emailId: "",
    isActive: true,
  });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const caretRestoreFrameRef = useRef<number | null>(null);

  const onlyDigits = (value: string) => value.replace(/\D/g, "");

  const applyDigitMask = (value: string, groups: number[]) => {
    const digits = onlyDigits(value).slice(
      0,
      groups.reduce((sum, group) => sum + group, 0),
    );

    if (!digits) return "";

    let cursor = 0;
    return groups
      .map((length) => {
        const part = digits.slice(cursor, cursor + length);
        cursor += length;
        return part.padEnd(length, "_");
      })
      .join("-");
  };

  const formatContactNo = (value: string) => applyDigitMask(value, [4, 7]);

  const caretPositionForDigitCount = (formattedValue: string, digitCount: number) => {
    if (digitCount <= 0) return 0;
    let seenDigits = 0;
    for (let index = 0; index < formattedValue.length; index += 1) {
      if (/\d/.test(formattedValue[index])) {
        seenDigits += 1;
        if (seenDigits >= digitCount) return index + 1;
      }
    }
    return formattedValue.length;
  };

  const handleMaskedChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    field: "contactNo" | "phone",
    data: any,
    setData: any,
  ) => {
    const input = event.target;
    const caret = input.selectionStart ?? input.value.length;
    const digitsBeforeCaret = onlyDigits(input.value.slice(0, caret)).length;
    const nextValue = formatContactNo(input.value);
    const nextCaret = caretPositionForDigitCount(nextValue, digitsBeforeCaret);

    setData({ ...data, [field]: nextValue });
    if (caretRestoreFrameRef.current !== null) {
      window.cancelAnimationFrame(caretRestoreFrameRef.current);
    }
    caretRestoreFrameRef.current = window.requestAnimationFrame(() => {
      caretRestoreFrameRef.current = null;
      if (document.activeElement !== input) return;
      if (input.selectionStart === nextCaret && input.selectionEnd === nextCaret) return;
      input.setSelectionRange(nextCaret, nextCaret);
    });
  };

  // Contact numbers are formatted for display here so the cache keeps the raw record.
  const rows = React.useMemo<any[]>(
    () =>
      (cachedRows || []).map((item: any) => ({
        ...item,
        contactNo: formatContactNo(item.contactNo || ""),
        phone: formatContactNo(item.phone || ""),
      })),
    [cachedRows],
  );

  const normalizeAuthorityTypes = (items: any[]) =>
    (Array.isArray(items) ? items : []).map((item: any, index: number) => ({
      id: item.id ?? item.value ?? item.key ?? item.code ?? index + 1,
      code: item.code ?? item.Code ?? "",
      shortName: item.shortName ?? item.name ?? item.stxt ?? item.text ?? item.label ?? "",
      description: item.description ?? item.longName ?? item.desc ?? "",
    }));

  // AuthorityType is global LOV reference data. LovV3Controller exposes the same
  // rows by path segment as by ?lovType=, so the shared cache serves this screen.
  const { lov } = useHrLovs(["AuthorityType"]);
  const authorityTypes = React.useMemo<any[]>(
    () => normalizeAuthorityTypes(lov("AuthorityType")),
    [lov],
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Authority"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Authority"));
    else if (viewMode === "view") onSubViewChange(translate("View Authority"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  // Mock Lookup mapping
  // Note: For full integration, call /api/HumanResource/Lov/v3?lovType=AuthorityType 
  const LOOKUP_DATA: any = {
    authorityType: {
      title: "Authority Type",
      subtitle: "Search / Authority Type",
      columns: [
        { id: "code", label: "Code", minWidth: 120 },
        { id: "shortName", label: "Authority Type", minWidth: 200 },
        { id: "description", label: "Description", minWidth: 250 },
      ],
      rows: [
        {
          id: 144,
          code: "GOV",
          shortName: "Government",
          description: "Government Authority",
        },
        {
          id: 146,
          code: "SCH",
          shortName: "School",
          description: "School Authority",
        },
        { 
          id: 145,
          code: "PRI", 
          shortName: "Private", 
          description: "Private Authority" 
        },
      ],
    },
  };

  const renderLookupDialog = (formData: any, setFormData: any) => {
    if (!activeLookup || !LOOKUP_DATA[activeLookup]) return null;
    const lookupRows =
      activeLookup === "authorityType" && authorityTypes.length > 0
        ? authorityTypes
        : LOOKUP_DATA[activeLookup].rows;

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
            {LOOKUP_DATA[activeLookup].title}
          </Typography>
          <IconButton
            size="small"
            onClick={handleLookupClose}
            sx={{
              bgcolor: "#fff",
              color: "#aaa",
              "&:hover": { bgcolor: "#f5f5f5" },
              boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
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
            {LOOKUP_DATA[activeLookup].subtitle}
          </Typography>
        </Box>

        <Box sx={{ px: 3, pb: 2 }}>
          <SchemaTable
            title={LOOKUP_DATA[activeLookup].title}
            columns={LOOKUP_DATA[activeLookup].columns.map((col: any) => ({
              ...col,
              minWidth: col.minWidth || 120,
            }))}
            rows={lookupRows}
            actions
            actionsPosition="right"
            showAddButton={false}
            showEditAction={false}
            showToaster={false}
            hideTitle
            hideBreadcrumbs
            onViewClick={(row: any) => {
              if (activeLookup === "authorityType") {
                setFormData({
                  ...formData,
                  authorityTypeId: row.id,
                  authorityType: row.shortName || row.name,
                });
              }
              handleLookupClose();
            }}
          />
        </Box>

        <Box
          sx={{
            p: 2,
            bgcolor: "#fff",
            borderTop: "1px solid #E2E8F0",
          }}
        >
          <Typography
            sx={{
              fontWeight: 800,
              fontSize: "0.75rem",
              color: "#166534",
            }}
          >
            TOTAL RECORDS : {lookupRows.length}
          </Typography>
        </Box>
      </Dialog>
    );
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "authority", label: translate("Authority"), minWidth: 200 },
    { 
      id: "authorityType", 
      label: translate("Authority Type"), 
      minWidth: 150,
    },
    { id: "contactNo", label: translate("Contact No"), minWidth: 150 },
    { id: "phone", label: translate("Phone No"), minWidth: 120 },
    { id: "emailId", label: translate("Email ID"), minWidth: 200 },
  ];

  const initialFormData = {
    code: "",
    authorityTypeId: null,
    authorityType: "",
    authority: "",
    description: "",
    contactNo: "",
    phone: "",
    web: "",
    fax: "",
    emailId: "",
    isActive: true,
  };

  const handleSave = async (data: any) => {
    const errors = [];
    const contactDigits = onlyDigits(data.contactNo || "");
    const phoneDigits = onlyDigits(data.phone || "");

    if (!CheckemptyString(data.code)) errors.push(translate("Code is required"));
    if (!data.authorityTypeId && !data.authorityType) errors.push(translate("Authority Type is required"));
    if (!CheckemptyString(data.authority)) errors.push(translate("Authority is required"));
    if (contactDigits && contactDigits.length !== 11) errors.push(translate("Contact No is invalid"));
    if (phoneDigits && phoneDigits.length !== 11) errors.push(translate("Phone No is invalid"));

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = {
        ...data,
        contactNo: contactDigits.length === 11 ? formatContactNo(contactDigits) : "",
        phone: phoneDigits.length === 11 ? formatContactNo(phoneDigits) : "",
      };
      
      // Patch for legacy mocked dropdown data
      if (!payload.authorityTypeId && payload.authorityType) {
        const match = (authorityTypes.length > 0
          ? authorityTypes
          : LOOKUP_DATA.authorityType?.rows || []
        ).find(
          (opt: any) =>
            opt.shortName?.toLowerCase() ===
            String(payload.authorityType).toLowerCase(),
        );
        payload.authorityTypeId = match?.id || payload.authorityTypeId;
      }

      let saved: any;
      if (data.id) {
        const response: any = await PutService(`/api/HumanResource/Authority/v3/${data.id}`, payload);
        saved = response?.data;
      } else {
        const response: any = await PostService("/api/HumanResource/Authority/v3", payload);
        saved = response?.data;
      }
      setToasterMessages([
        data.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      // Patch the cached row instead of reloading the whole list. The authority
      // type name is display data the form already carries.
      const savedId = Number(saved?.id ?? data.id ?? 0);
      if (savedId > 0) {
        dispatch(authoritySlice.actions.upsertRow({ ...payload, id: savedId }));
      } else {
        dispatch(authoritySlice.fetchAll(true));
      }
      return true;
    } catch (error: any) {
      const data = error?.response?.data;
      const message = data?.Message || data?.message || (typeof data === "string" ? data : null) || error?.message || "Failed to save authority";
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
      await DeleteService(`/api/HumanResource/Authority/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(authoritySlice.actions.removeRow(row.id));
    } catch (error: any) {
      const data = error?.response?.data;
      const message = data?.Message || data?.message || (typeof data === "string" ? data : null) || error?.message || "Failed to delete authority";
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
    mode: "new" | "edit" | "view",
    _isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    const isReadOnly = mode === "view" || _isReadOnly;
    const authorityTypeOptions =
      authorityTypes.length > 0
        ? authorityTypes
        : LOOKUP_DATA.authorityType?.rows || [];
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
                <T>Authority Details</T>
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
              {/* Code */}
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

              {/* Authority Type */}
              <Grid item xs={12} md={6}>
                <LookupDropdown
                  label={translate("Authority Type")}
                  required
                  value={
                    data.authorityType ||
                    authorityTypeOptions.find(
                      (opt: any) => opt.id === data.authorityTypeId,
                    )?.shortName ||
                    ""
                  }
                  options={authorityTypeOptions}
                  getOptionLabel={(o) => o.shortName || o.name || ""}
                  onSelect={(val) => {
                    setData({
                      ...data,
                      authorityTypeId: val.id,
                      authorityType: val.shortName || val.name || "",
                    });
                  }}
                  onSearchClick={() => setActiveLookup("authorityType")}
                  error={isSubmitted && (!data.authorityTypeId && !data.authorityType)}
                  helperText={
                    isSubmitted && (!data.authorityTypeId && !data.authorityType)
                      ? translate("Authority Type is required")
                      : ""
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Authority */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Authority")}
                  placeholder={translate("Enter Authority")}
                  value={data.authority || ""}
                  onChange={(e) =>
                    setData({ ...data, authority: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.authority}
                  required
                />
              </Grid>

              {/* Description */}
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

              {/* Contact No */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Contact No")}
                  placeholder={translate("Enter Contact no")}
                  value={data.contactNo || ""}
                  type="tel"
                  inputMode="numeric"
                  debounceMs={0}
                  maxLength={20}
                  onChange={(e) => handleMaskedChange(e, "contactNo", data, setData)}
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Phone No */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Phone No")}
                  placeholder={translate("Enter Phone No")}
                  value={data.phone || ""}
                  type="tel"
                  inputMode="numeric"
                  debounceMs={0}
                  maxLength={20}
                  onChange={(e) => handleMaskedChange(e, "phone", data, setData)}
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Web */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Web")}
                  placeholder={translate("Enter Web")}
                  value={data.web || ""}
                  onChange={(e) =>
                    setData({ ...data, web: e.target.value })
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Fax */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Fax")}
                  placeholder={translate("Enter fax")}
                  value={data.fax || ""}
                  onChange={(e) =>
                    setData({ ...data, fax: e.target.value })
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Email ID */}
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Email ID")}
                  placeholder={translate("Enter Email")}
                  value={data.emailId || ""}
                  onChange={(e) =>
                    setData({ ...data, emailId: e.target.value })
                  }
                  disabled={isReadOnly}
                />
              </Grid>

              {/* Status Checkbox */}
              <Grid item xs={12}>
                <LmsCheckbox
                  checked={data.isActive !== false}
                  onChange={(checked) =>
                    setData({
                      ...data,
                      isActive: checked,
                    })
                  }
                  label={translate("Mark as Active Authority")}
                />
              </Grid>
            </Grid>
          </Collapse>
        </Box>
        {renderLookupDialog(data, setData)}
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
                ? translate("New Authority")
                : viewMode === "edit"
                  ? translate("Edit Authority")
                  : translate("View Authority")}
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
          title={translate("Authority")}
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
            setFormData({
              ...row,
              contactNo: formatContactNo(row.contactNo || ""),
              phone: formatContactNo(row.phone || ""),
            });
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({
              ...row,
              contactNo: formatContactNo(row.contactNo || ""),
              phone: formatContactNo(row.phone || ""),
            });
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

export default Authority;

