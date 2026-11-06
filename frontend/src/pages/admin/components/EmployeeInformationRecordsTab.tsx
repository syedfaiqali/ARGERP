import { EditableImageInput } from "@/components/EditableImageInput";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Box,
  Button,
  Grid,
  IconButton,
  MenuItem,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  DeleteOutline as RemoveFileIcon,
  Download as DownloadIcon,
  UploadFile as UploadFileIcon,
} from "@mui/icons-material";
import dayjs from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { FormInput } from "@/components/FormInput";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { StyledTabs } from "@/components/StyledTabs";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { DeleteService, GetService, PostService, PutService, toDateOnly } from "@/constants/globalItems";
import api from "@/services/api";

type RecordGroup = "DISCIPLINARY" | "DEVELOPMENT";
type RecordCategory = "INQUIRY" | "PUNISHMENT" | "COURT_CASE" | "AWARD" | "TRAINING";

interface EmployeeInformationRecordRow {
  id: number | null;
  employeeId?: number;
  group: RecordGroup;
  categoryId: number | null;
  category: RecordCategory;
  title: string;
  referenceNumber: string;
  recordDate: string;
  endDate: string;
  status: string;
  authority: string;
  subCategory: string;
  mode: string;
  details: string;
  outcome: string;
  remarks: string;
  hasAttachment: boolean;
  fileName: string;
  file: File | null;
  removeAttachment: boolean;
}

interface EmployeeInformationRecordsTabProps {
  employeeId?: string;
  readOnly?: boolean;
  group: RecordGroup;
}

const API = "/api/HumanResource/EmployeeInformationRecords/v3";
const MAX_FILE_SIZE = 10 * 1024 * 1024;

const GROUP_CATEGORIES: Record<RecordGroup, RecordCategory[]> = {
  DISCIPLINARY: ["INQUIRY", "PUNISHMENT", "COURT_CASE"],
  DEVELOPMENT: ["AWARD", "TRAINING"],
};

const CATEGORY_LABELS: Record<RecordCategory, string> = {
  INQUIRY: "Inquiries",
  PUNISHMENT: "Punishments",
  COURT_CASE: "Court Cases",
  AWARD: "Awards",
  TRAINING: "Trainings",
};

const CATEGORY_SINGULAR_LABELS: Record<RecordCategory, string> = {
  INQUIRY: "Inquiry",
  PUNISHMENT: "Punishment",
  COURT_CASE: "Court Case",
  AWARD: "Award",
  TRAINING: "Training",
};

const STATUS_OPTIONS: Partial<Record<RecordCategory, string[]>> = {
  INQUIRY: ["Initiated", "In Progress", "Completed", "Closed"],
  PUNISHMENT: ["Issued", "Active", "Completed", "Withdrawn"],
  COURT_CASE: ["Pending", "In Progress", "Decided", "Closed"],
  TRAINING: ["Planned", "In Progress", "Completed", "Cancelled"],
};

const TRAINING_MODES = ["In-person", "Online", "Hybrid"];
const LOV_CATEGORY_CODES: Record<string, RecordCategory> = {
  INQ: "INQUIRY", PUN: "PUNISHMENT", CRT: "COURT_CASE", AWD: "AWARD", TRN: "TRAINING",
};

const itemsFrom = (payload: any): any[] =>
  payload?.items ?? payload?.Items ?? payload?.data?.items ?? payload?.data?.Items ?? payload?.data ?? (Array.isArray(payload) ? payload : []);

const makeEmptyRow = (group: RecordGroup, category: RecordCategory, categoryId: number | null): EmployeeInformationRecordRow => ({
  id: null,
  group,
  categoryId,
  category,
  title: "",
  referenceNumber: "",
  recordDate: "",
  endDate: "",
  status: "",
  authority: "",
  subCategory: "",
  mode: "",
  details: "",
  outcome: "",
  remarks: "",
  hasAttachment: false,
  fileName: "",
  file: null,
  removeAttachment: false,
});

const normalizeRow = (item: any): EmployeeInformationRecordRow => ({
  id: Number(item.id ?? item.Id ?? 0) || null,
  employeeId: Number(item.employeeId ?? item.EmployeeId ?? 0) || undefined,
  group: (item.group ?? item.Group ?? "DEVELOPMENT") as RecordGroup,
  categoryId: Number(item.categoryId ?? item.CategoryId ?? 0) || null,
  category: (item.category ?? item.Category ?? "AWARD") as RecordCategory,
  title: item.title ?? item.Title ?? "",
  referenceNumber: item.referenceNumber ?? item.ReferenceNumber ?? "",
  recordDate: toDateOnly(item.recordDate ?? item.RecordDate),
  endDate: toDateOnly(item.endDate ?? item.EndDate),
  status: item.status ?? item.Status ?? "",
  authority: item.authority ?? item.Authority ?? "",
  subCategory: item.subCategory ?? item.SubCategory ?? "",
  mode: item.mode ?? item.Mode ?? "",
  details: item.details ?? item.Details ?? "",
  outcome: item.outcome ?? item.Outcome ?? "",
  remarks: item.remarks ?? item.Remarks ?? "",
  hasAttachment: Boolean(item.hasAttachment ?? item.HasAttachment),
  fileName: item.fileName ?? item.FileName ?? "",
  file: null,
  removeAttachment: false,
});

const readFileAsDataUrl = (file: File) => new Promise<string>((resolve, reject) => {
  const reader = new FileReader();
  reader.onload = () => resolve(String(reader.result ?? ""));
  reader.onerror = () => reject(new Error("Failed to read the selected attachment"));
  reader.readAsDataURL(file);
});

const titleLabel = (category: RecordCategory) => {
  if (category === "AWARD") return "Award Name";
  if (category === "TRAINING") return "Training/Course Name";
  return "Title/Subject";
};

const dateLabel = (category: RecordCategory) => {
  if (category === "AWARD") return "Award Date";
  if (category === "TRAINING") return "Start Date";
  if (category === "COURT_CASE") return "Filing Date";
  return "Record Date";
};

const authorityLabel = (category: RecordCategory) => {
  if (category === "COURT_CASE") return "Court Name";
  if (category === "AWARD") return "Awarding Organization";
  if (category === "TRAINING") return "Provider/Institute";
  return "Issuing Authority";
};

const detailsLabel = (category: RecordCategory) => {
  if (category === "AWARD") return "Reason/Achievement";
  if (category === "TRAINING") return "Training Details";
  return "Details";
};

const EmployeeInformationRecordsTab: React.FC<EmployeeInformationRecordsTabProps> = ({
  employeeId,
  readOnly = false,
  group,
}) => {
  const { translate } = useTranslation();
  const categories = GROUP_CATEGORIES[group];
  const [activeCategory, setActiveCategory] = useState<RecordCategory>(categories[0]);
  const [rows, setRows] = useState<EmployeeInformationRecordRow[]>([]);
  const [categoryLovs, setCategoryLovs] = useState<Array<{ id: number; code: RecordCategory; name: string }>>([]);
  const [loading, setLoading] = useState(false);
  const [toastOpen, setToastOpen] = useState(false);
  const [toastMessages, setToastMessages] = useState<string[]>([]);

  const showError = useCallback((error: any, fallback: string) => {
    const message = error?.response?.status === 403
      ? "You are not authorized to access disciplinary and legal records"
      : error?.response?.data?.error ?? error?.response?.data ?? error?.message ?? fallback;
    setToastMessages([String(message)]);
    setToastOpen(true);
  }, []);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;
    try {
      setLoading(true);
      const response: any = await GetService(`${API}?employeeId=${employeeId}&group=${group}&pageSize=1000`);
      if (response?.status === 404) throw new Error("Employee information record API was not found. Please restart the Human Resource service.");
      setRows(itemsFrom(response).map(normalizeRow));
    } catch (error: any) {
      setRows([]);
      showError(error, "Failed to load employee information records");
    } finally {
      setLoading(false);
    }
  }, [employeeId, group, showError]);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response: any = await GetService("/api/HumanResource/Lov/v3/HRRCA");
        const allowed = new Set<RecordCategory>(categories);
        setCategoryLovs(itemsFrom(response)
          .map((item: any) => ({
            id: Number(item.id ?? item.Id ?? 0),
            code: LOV_CATEGORY_CODES[String(item.code ?? item.Code ?? "").toUpperCase()],
            name: String(item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? ""),
          }))
          .filter((item: { id: number; code?: RecordCategory }) => item.id > 0 && !!item.code && allowed.has(item.code)) as Array<{ id: number; code: RecordCategory; name: string }>);
      } catch (error: any) {
        setCategoryLovs([]);
        showError(error, "Failed to load record categories");
      }
    };
    void fetchCategories();
  }, [categories, showError]);

  useEffect(() => {
    setActiveCategory(categories[0]);
  }, [categories, group]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const loadRow = async (row: EmployeeInformationRecordRow) => {
    if (!row.id) return row;
    try {
      setLoading(true);
      const response: any = await GetService(`${API}/${row.id}`);
      return normalizeRow(response?.data ?? response);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async (data: EmployeeInformationRecordRow) => {
    if (!employeeId) throw new Error(translate("Employee context is missing"));
    if (data.file && data.file.size > MAX_FILE_SIZE) throw new Error(translate("Attachment size cannot exceed 10 MB"));
    if (data.category === "TRAINING" && data.recordDate && data.endDate && dayjs(data.recordDate).isAfter(dayjs(data.endDate), "day")) {
      throw new Error(translate("Training Start Date cannot be greater than End Date"));
    }

    const payload = {
      employeeId: Number(employeeId),
      categoryId: data.categoryId,
      title: data.title.trim(),
      referenceNumber: data.referenceNumber.trim() || null,
      recordDate: data.recordDate || null,
      endDate: data.category === "TRAINING" ? data.endDate || null : null,
      status: data.category === "AWARD" ? null : data.status,
      authority: data.authority.trim() || null,
      subCategory: data.subCategory.trim() || null,
      mode: data.category === "TRAINING" ? data.mode || null : null,
      details: data.details.trim() || null,
      outcome: group === "DISCIPLINARY" ? data.outcome.trim() || null : null,
      remarks: data.remarks.trim() || null,
      fileName: data.file?.name || null,
      fileContentBase64: data.file ? await readFileAsDataUrl(data.file) : null,
      removeAttachment: data.removeAttachment,
    };

    try {
      setLoading(true);
      if (data.id) await PutService(`${API}/${data.id}`, payload);
      else await PostService(API, payload);
      await fetchRows();
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (row: EmployeeInformationRecordRow) => {
    if (!row.id) return;
    try {
      setLoading(true);
      await DeleteService(`${API}/${row.id}`);
      await fetchRows();
    } finally {
      setLoading(false);
    }
  };

  const downloadAttachment = async (row: EmployeeInformationRecordRow) => {
    if (!row.id || !row.hasAttachment) return;
    try {
      setLoading(true);
      const response = await api.get(`${API}/${row.id}/attachment`, { responseType: "blob" });
      const url = URL.createObjectURL(response.data);
      const anchor = document.createElement("a");
      anchor.href = url;
      anchor.download = row.fileName || "attachment";
      document.body.appendChild(anchor);
      anchor.click();
      anchor.remove();
      URL.revokeObjectURL(url);
    } catch (error: any) {
      showError(error, "Failed to download attachment");
    } finally {
      setLoading(false);
    }
  };

  const filteredRows = useMemo(
    () => rows.filter((row) => row.category === activeCategory),
    [activeCategory, rows],
  );

  const columns = useMemo(() => [
    {
      id: "sno",
      label: translate("S.No"),
      minWidth: 70,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "title", label: translate(titleLabel(activeCategory)), minWidth: 220 },
    { id: "referenceNumber", label: translate(activeCategory === "COURT_CASE" ? "Case Number" : "Reference Number"), minWidth: 150 },
    {
      id: "recordDate",
      label: translate(dateLabel(activeCategory)),
      minWidth: 135,
      format: (value: any) => value ? dayjs(value).format("DD MMM YYYY") : "-",
    },
    { id: "status", label: translate("Status"), minWidth: 130, format: (value: any) => value || "-" },
    { id: "authority", label: translate(authorityLabel(activeCategory)), minWidth: 190 },
    { id: "fileName", label: translate("Supporting Document"), minWidth: 190, format: (value: any) => value || "-" },
  ], [activeCategory, translate]);

  const renderForm = (
    formData: EmployeeInformationRecordRow,
    setFormData: React.Dispatch<React.SetStateAction<EmployeeInformationRecordRow>>,
    _mode: "new" | "edit" | "view",
    isReadOnly: boolean,
    isSubmitted: boolean,
  ) => {
    const patch = (updates: Partial<EmployeeInformationRecordRow>) => setFormData((previous) => ({ ...previous, ...updates }));
    const statusOptions = STATUS_OPTIONS[formData.category] ?? [];
    const disciplinary = group === "DISCIPLINARY";

    return (
      <Box sx={{ pt: 1 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <FormInput
              select
              label={translate("Record Category")}
              required
              disabled={isReadOnly}
              value={formData.categoryId ?? ""}
              onChange={(event) => {
                const selected = categoryLovs.find((item) => item.id === Number(event.target.value));
                patch({ categoryId: selected?.id ?? null, category: selected?.code ?? formData.category, status: "", mode: "", subCategory: "" });
              }}
              error={isSubmitted && !formData.categoryId}
            >
              {categoryLovs.map((category) => <MenuItem key={category.id} value={category.id}>{translate(category.name || CATEGORY_LABELS[category.code])}</MenuItem>)}
            </FormInput>
          </Grid>
          <Grid item xs={12} md={8}>
            <FormInput
              label={translate(titleLabel(formData.category))}
              required
              disabled={isReadOnly}
              value={formData.title}
              onChange={(event) => patch({ title: event.target.value.slice(0, 250) })}
              error={isSubmitted && !formData.title.trim()}
            />
          </Grid>

          {(disciplinary || formData.category === "AWARD") && (
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate(formData.category === "COURT_CASE" ? "Case Number" : formData.category === "AWARD" ? "Certificate Number" : "Reference Number")}
                disabled={isReadOnly}
                value={formData.referenceNumber}
                onChange={(event) => patch({ referenceNumber: event.target.value.slice(0, 100) })}
              />
            </Grid>
          )}

          <Grid item xs={12} md={4}>
            <FormDatePicker
              label={translate(dateLabel(formData.category))}
              required
              disabled={isReadOnly}
              value={formData.recordDate ? dayjs(formData.recordDate) : null}
              onChange={(value) => patch({ recordDate: value ? value.format("YYYY-MM-DD") : "" })}
              maxDate={formData.category === "TRAINING" && formData.endDate ? dayjs(formData.endDate) : undefined}
              format="DD-MMM-YYYY"
              error={isSubmitted && !formData.recordDate}
            />
          </Grid>

          {formData.category === "TRAINING" && (
            <Grid item xs={12} md={4}>
              <FormDatePicker
                label={translate("End Date")}
                disabled={isReadOnly}
                value={formData.endDate ? dayjs(formData.endDate) : null}
                onChange={(value) => patch({ endDate: value ? value.format("YYYY-MM-DD") : "" })}
                minDate={formData.recordDate ? dayjs(formData.recordDate) : undefined}
                format="DD-MMM-YYYY"
              />
            </Grid>
          )}

          {formData.category !== "AWARD" && (
            <Grid item xs={12} md={4}>
              <FormInput
                select
                label={translate("Status")}
                required
                disabled={isReadOnly}
                value={formData.status}
                onChange={(event) => patch({ status: event.target.value })}
                error={isSubmitted && !formData.status}
              >
                {statusOptions.map((status) => <MenuItem key={status} value={status}>{translate(status)}</MenuItem>)}
              </FormInput>
            </Grid>
          )}

          <Grid item xs={12} md={4}>
            <FormInput
              label={translate(authorityLabel(formData.category))}
              disabled={isReadOnly}
              value={formData.authority}
              onChange={(event) => patch({ authority: event.target.value.slice(0, 200) })}
            />
          </Grid>

          {formData.category === "AWARD" && (
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("Award Category")}
                disabled={isReadOnly}
                value={formData.subCategory}
                onChange={(event) => patch({ subCategory: event.target.value.slice(0, 100) })}
              />
            </Grid>
          )}

          {formData.category === "TRAINING" && (
            <Grid item xs={12} md={4}>
              <FormInput
                select
                label={translate("Training Mode")}
                disabled={isReadOnly}
                value={formData.mode}
                onChange={(event) => patch({ mode: event.target.value })}
              >
                <MenuItem value=""><em>{translate("Not specified")}</em></MenuItem>
                {TRAINING_MODES.map((mode) => <MenuItem key={mode} value={mode}>{translate(mode)}</MenuItem>)}
              </FormInput>
            </Grid>
          )}

          <Grid item xs={12} md={disciplinary ? 6 : 12}>
            <FormInput
              label={translate(detailsLabel(formData.category))}
              multiline
              rows={3}
              disabled={isReadOnly}
              value={formData.details}
              onChange={(event) => patch({ details: event.target.value.slice(0, 2000) })}
            />
          </Grid>

          {disciplinary && (
            <Grid item xs={12} md={6}>
              <FormInput
                label={translate("Outcome/Decision")}
                multiline
                rows={3}
                disabled={isReadOnly}
                value={formData.outcome}
                onChange={(event) => patch({ outcome: event.target.value.slice(0, 2000) })}
              />
            </Grid>
          )}

          <Grid item xs={12}>
            <FormInput
              label={translate("Remarks")}
              multiline
              rows={2}
              disabled={isReadOnly}
              value={formData.remarks}
              onChange={(event) => patch({ remarks: event.target.value.slice(0, 2000) })}
            />
          </Grid>

          <Grid item xs={12}>
            <Typography sx={{ fontSize: "0.78rem", fontWeight: 700, color: "var(--lms-text-secondary)", mb: 0.75 }}>
              <T>Supporting Document</T>
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1, flexWrap: "wrap", minHeight: 44, border: "1px solid var(--lms-border)", borderRadius: "10px", px: 1.25, py: 0.75 }}>
              <Button
                component="label"
                variant="contained"
                size="small"
                startIcon={<UploadFileIcon />}
                disabled={isReadOnly}
                sx={{ textTransform: "none", fontWeight: 700 }}
              >
                <T>Choose File</T>
                <EditableImageInput
                  hidden
                  type="file"
                  accept=".pdf,.png,.jpg,.jpeg,.doc,.docx"
                  onChange={(event) => {
                    const file = event.target.files?.[0] ?? null;
                    if (file && file.size > MAX_FILE_SIZE) {
                      showError(null, "Attachment size cannot exceed 10 MB");
                      event.target.value = "";
                      return;
                    }
                    patch({ file, fileName: file?.name ?? formData.fileName, removeAttachment: false });
                  }}
                />
              </Button>
              <Typography sx={{ flex: 1, minWidth: 180, fontSize: "0.88rem", color: "var(--lms-text-secondary)" }}>
                {formData.file?.name || (!formData.removeAttachment && formData.fileName) || translate("No file chosen")}
              </Typography>
              {!isReadOnly && (formData.file || (formData.hasAttachment && !formData.removeAttachment)) && (
                <Tooltip title={translate("Remove attachment")}>
                  <IconButton size="small" color="error" onClick={() => patch({ file: null, fileName: "", removeAttachment: true })}>
                    <RemoveFileIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              )}
            </Box>
            <Typography sx={{ mt: 0.5, fontSize: "0.72rem", color: "var(--lms-text-secondary)" }}>
              <T>PDF, image, or Word document; maximum 10 MB</T>
            </Typography>
          </Grid>
        </Grid>
      </Box>
    );
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate(CATEGORY_LABELS[activeCategory])}
        addButtonLabel={`${translate("New")} ${translate(CATEGORY_SINGULAR_LABELS[activeCategory])}`}
        columns={columns}
        rows={filteredRows}
        hideTitle
        stickyHeader
        pagination={false}
        filtersEnabled={false}
        initialFormData={makeEmptyRow(group, activeCategory, categoryLovs.find((item) => item.code === activeCategory)?.id ?? null)}
        loadRow={loadRow as any}
        onSave={handleSave as any}
        onDelete={handleDelete as any}
        renderForm={renderForm as any}
        readOnly={readOnly}
        useActionRights={false}
        maxWidth="lg"
        mandatoryFields={[
          { key: "categoryId", label: translate("Record Category") },
          { key: "title", label: translate(titleLabel(activeCategory)) },
          { key: "recordDate", label: translate(dateLabel(activeCategory)) },
          { key: "status", label: translate("Status"), condition: (data: EmployeeInformationRecordRow) => data.category !== "AWARD" },
        ]}
        extraToolbarItemsLeft={
          <StyledTabs
            value={categories.indexOf(activeCategory)}
            onChange={(_, value) => setActiveCategory(categories[value])}
            tabs={categories.map((category) => ({ label: translate(CATEGORY_LABELS[category]) }))}
            variant="scrollable"
            scrollButtons="auto"
            allowScrollButtonsMobile
            sx={{ mb: 0, minWidth: 0 }}
          />
        }
        extraActions={(row: EmployeeInformationRecordRow) => row.hasAttachment ? (
          <Tooltip title={translate("Download attachment")}>
            <IconButton size="small" color="primary" onClick={() => void downloadAttachment(row)}>
              <DownloadIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        ) : null}
        showToaster
      />
      <LmsToaster
        open={toastOpen}
        onClose={() => setToastOpen(false)}
        messages={toastMessages}
        severity="error"
        disableDeleteSuccessAutoSeverity
      />
    </>
  );
};

export const EmployeeDisciplinaryLegalTab: React.FC<Omit<EmployeeInformationRecordsTabProps, "group">> = (props) => (
  <EmployeeInformationRecordsTab {...props} group="DISCIPLINARY" />
);

export const EmployeeDevelopmentRecognitionTab: React.FC<Omit<EmployeeInformationRecordsTabProps, "group">> = (props) => (
  <EmployeeInformationRecordsTab {...props} group="DEVELOPMENT" />
);
