import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button, TextField } from "@mui/material";
import {
  Add as AddIcon,
  DeleteOutline as DeleteIcon,
  School as QualificationIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { Modal } from "@/components/Modal";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

interface LookupOption {
  id: number;
  code?: string;
  name: string;
  qualificationTypeId?: number;
}

interface EmployeeEducationRow {
  id: number | string;
  employeeId?: number;
  qualificationTypeId: number | "";
  qualificationType: string;
  qualificationId: number | "";
  qualification: string;
  institute: string;
  cgpa: number | "";
  percentage: number | "";
  grade: string;
  division: string;
  passingYear: number | "";
}

interface EmployeeEducationTabProps {
  employeeId: string | undefined;
}

type LookupKind = "qualificationType" | "qualification";
type SetupKind = "qualificationType" | "qualification";
const QUALIFICATION_TYPE_ENDPOINT = "/api/HumanResource/QualificationType/v3";
const QUALIFICATION_ENDPOINT = "/api/HumanResource/Qualification/v3";
const EDUCATION_PERMISSION_SCREEN_NAME = "Education";
const EDUCATION_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeEducationRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  qualificationTypeId: "",
  qualificationType: "",
  qualificationId: "",
  qualification: "",
  institute: "",
  cgpa: "",
  percentage: "",
  grade: "",
  division: "",
  passingYear: "",
});

const itemsFrom = (payload: any): any[] =>
  payload?.items ??
  payload?.Items ??
  payload?.data?.items ??
  payload?.data?.Items ??
  payload?.data ??
  payload?.Data ??
  (Array.isArray(payload) ? payload : []);

const normalizeLookup = (payload: any): LookupOption[] =>
  itemsFrom(payload)
    .map((item: any) => ({
      id: Number(item.id ?? item.Id ?? 0),
      code: item.code ?? item.Code ?? "",
      name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? item.qualification ?? item.Qualification ?? "",
      qualificationTypeId:
        Number(
          item.qualificationTypeId ??
            item.QualificationTypeId ??
            item.qualificationtypeid ??
            item.Qualificationtypeid ??
            item.quatypeid ??
            item.Quatypeid ??
            item.quaTypeId ??
            item.QuaTypeId ??
            0,
        ) || undefined,
    }))
    .filter((item: LookupOption) => item.id > 0);

const emptyQualificationTypeForm = () => ({
  code: "",
  name: "",
  description: "",
});

const emptyQualificationForm = () => ({
  code: "",
  qualificationTypeId: "" as number | "",
  qualificationTypeName: "",
  qualification: "",
  description: "",
});

const ensureSuccessResponse = (response: any, fallbackMessage: string) => {
  const status = Number(response?.status ?? 200);
  if (status >= 200 && status < 300) return;

  throw new Error(
    response?.data?.error ||
      response?.data ||
      fallbackMessage,
  );
};

const normalizeRow = (item: any): EmployeeEducationRow => ({
  id: item.id ?? item.Id,
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  qualificationTypeId: item.qualificationTypeId ?? item.QualificationTypeId ?? item.qualificationtypeid ?? "",
  qualificationType: item.qualificationType ?? item.QualificationType ?? "",
  qualificationId: item.qualificationId ?? item.QualificationId ?? item.qualificationid ?? "",
  qualification: item.qualification ?? item.Qualification ?? "",
  institute: item.institute ?? item.Institute ?? item.insitute ?? "",
  cgpa: item.cgpa ?? item.Cgpa ?? item.totalmarks ?? "",
  percentage: item.percentage ?? item.Percentage ?? "",
  grade: item.grade ?? item.Grade ?? "",
  division: item.division ?? item.Division ?? "",
  passingYear: item.passingYear ?? item.PassingYear ?? item.year ?? "",
});

export const EmployeeEducationTab: React.FC<EmployeeEducationTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeEducationRow[]>([]);
  const [qualificationTypes, setQualificationTypes] = useState<LookupOption[]>([]);
  const [qualifications, setQualifications] = useState<LookupOption[]>([]);
  const [selectedIds, setSelectedIds] = useState<any[]>([]);
  const [setupKind, setSetupKind] = useState<SetupKind | null>(null);
  const [qualificationTypeForm, setQualificationTypeForm] = useState(
    emptyQualificationTypeForm,
  );
  const [qualificationForm, setQualificationForm] = useState(
    emptyQualificationForm,
  );
  const [setupSubmitted, setSetupSubmitted] = useState(false);
  const [setupLookupOpen, setSetupLookupOpen] = useState(false);
  const [lookupState, setLookupState] = useState<{
    open: boolean;
    kind: LookupKind | null;
    rowId: number | string | null;
  }>({ open: false, kind: null, rowId: null });
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({ open: false, messages: [], severity: "info" });
  const [loading, setLoading] = useState(false);
  const canCreate = true;
  const canUpdate = true;
  const canDelete = true;

  const loadEducationLookups = useCallback(async () => {
    setLoading(true);
    try {
      const [typeResponse, qualificationResponse] = await Promise.all([
        GetService(`${QUALIFICATION_TYPE_ENDPOINT}?pageNumber=1&pageSize=1000`),
        GetService(`${QUALIFICATION_ENDPOINT}?pageNumber=1&pageSize=1000&lovtypeId=368`),
      ]);
      ensureSuccessResponse(typeResponse, translate("Failed to load qualification types"));
      ensureSuccessResponse(qualificationResponse, translate("Failed to load qualifications"));
      setQualificationTypes(normalizeLookup(typeResponse?.data ?? typeResponse));
      setQualifications(normalizeLookup(qualificationResponse?.data ?? qualificationResponse));
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to load education lookups")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [translate]);

  useEffect(() => {
    void loadEducationLookups();
  }, [loadEducationLookups]);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `/api/HumanResource/ERAcademic/v3?employeeId=${employeeId}&pageSize=1000`,
      );
      setRows(itemsFrom(response).map(normalizeRow));
      setSelectedIds([]);
    } catch {
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load education records")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const updateRow = (
    rowId: number | string,
    patch: Partial<EmployeeEducationRow>,
  ) => {
    setRows((prev) =>
      prev.map((row) => (row.id === rowId ? { ...row, ...patch } : row)),
    );
  };

  const openLookup = (rowId: number | string, kind: LookupKind) => {
    setLookupState({ open: true, rowId, kind });
  };

  const closeLookup = () => {
    setLookupState({ open: false, rowId: null, kind: null });
  };

  const handleAddRow = () => {
    if (!canCreate) return;
    setRows((prev) => [...prev, createEmptyRow()]);
  };

  const openSetupModal = (kind: SetupKind) => {
    if (!canCreate) return;
    setSetupKind(kind);
    setSetupSubmitted(false);
    setSetupLookupOpen(false);
    if (kind === "qualificationType") {
      setQualificationTypeForm(emptyQualificationTypeForm());
    } else {
      setQualificationForm(emptyQualificationForm());
    }
  };

  const closeSetupModal = () => {
    setSetupKind(null);
    setSetupSubmitted(false);
    setSetupLookupOpen(false);
    void loadEducationLookups();
  };

  const resetSetupForm = () => {
    setSetupSubmitted(false);
    if (setupKind === "qualificationType") {
      setQualificationTypeForm(emptyQualificationTypeForm());
    }
    if (setupKind === "qualification") {
      setQualificationForm(emptyQualificationForm());
    }
  };

  const saveQualificationTypeSetup = async (closeAfterSave = false) => {
    if (!canCreate) return;

    setSetupSubmitted(true);
    const errors = [
      !qualificationTypeForm.code.trim() ? translate("Code is required") : null,
      !qualificationTypeForm.name.trim()
        ? translate("Qualification Type is required")
        : null,
    ].filter(Boolean) as string[];

    if (errors.length > 0) {
      setToaster({ open: true, messages: errors, severity: "error" });
      return;
    }

    try {
      setLoading(true);
      const response = await PostService(QUALIFICATION_TYPE_ENDPOINT, {
        code: qualificationTypeForm.code.trim(),
        name: qualificationTypeForm.name.trim(),
        description: qualificationTypeForm.description.trim(),
        isActive: true,
      });
      ensureSuccessResponse(response, translate("Failed to save qualification type"));
      await loadEducationLookups();
      setToaster({
        open: true,
        messages: [translate("Qualification Type saved successfully")],
        severity: "success",
      });
      setQualificationTypeForm(emptyQualificationTypeForm());
      setSetupSubmitted(false);
      if (closeAfterSave) setSetupKind(null);
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save qualification type"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const saveQualificationSetup = async (closeAfterSave = false) => {
    if (!canCreate) return;

    setSetupSubmitted(true);
    const errors = [
      !qualificationForm.code.trim() ? translate("Code is required") : null,
      !qualificationForm.qualificationTypeId
        ? translate("Qualification Type is required")
        : null,
      !qualificationForm.qualification.trim()
        ? translate("Qualification is required")
        : null,
    ].filter(Boolean) as string[];

    if (errors.length > 0) {
      setToaster({ open: true, messages: errors, severity: "error" });
      return;
    }

    try {
      setLoading(true);
      const response = await PostService(QUALIFICATION_ENDPOINT, {
        code: qualificationForm.code.trim(),
        qualificationTypeId: Number(qualificationForm.qualificationTypeId),
        qualification: qualificationForm.qualification.trim(),
        description: qualificationForm.description.trim(),
        lovtypeId: 368,
        isActive: true,
      });
      ensureSuccessResponse(response, translate("Failed to save qualification"));
      await loadEducationLookups();
      setToaster({
        open: true,
        messages: [translate("Qualification saved successfully")],
        severity: "success",
      });
      setQualificationForm(emptyQualificationForm());
      setSetupSubmitted(false);
      if (closeAfterSave) setSetupKind(null);
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save qualification"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const saveSetup = (closeAfterSave = false) => {
    if (setupKind === "qualificationType") {
      void saveQualificationTypeSetup(closeAfterSave);
    }
    if (setupKind === "qualification") {
      void saveQualificationSetup(closeAfterSave);
    }
  };

  const handleDeleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      setLoading(true);
      for (const row of rows.filter((item) => selectedIds.includes(item.id))) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/ERAcademic/v3/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected education rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected education rows")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSaveAll = async () => {
    if (!canUpdate) return;

    if (!employeeId) {
      setToaster({
        open: true,
        messages: [translate("Employee context is missing")],
        severity: "error",
      });
      return;
    }

    const requiredError =
      rows.some((row) => !row.qualificationTypeId) ? "Please select Qualification Type for all rows" :
      rows.some((row) => !row.qualificationId) ? "Please select Qualification for all rows" :
      rows.some((row) => !row.institute.trim()) ? "Institute is required for all rows" :
      rows.some((row) => !row.passingYear) ? "Passing Year is required for all rows" :
      rows.some((row) => Number(row.cgpa) > 100) ? "CGPA cannot be greater than 100" :
      rows.some((row) => Number(row.percentage) > 100) ? "Percentage cannot be greater than 100" :
      null;

    if (requiredError) {
      setToaster({
        open: true,
        messages: [translate(requiredError)],
        severity: "error",
      });
      return;
    }

    try {
      setLoading(true);
      for (const row of rows) {
        const payload = {
          id: typeof row.id === "number" ? row.id : undefined,
          employeeId: Number(employeeId),
          qualificationTypeId: Number(row.qualificationTypeId),
          qualificationId: Number(row.qualificationId),
          institute: row.institute.trim(),
          cgpa: row.cgpa === "" ? null : Number(row.cgpa),
          percentage: row.percentage === "" ? null : Number(row.percentage),
          grade: row.grade.trim(),
          division: row.division.trim(),
          passingYear: Number(row.passingYear),
        };

        if (typeof row.id === "number") {
          await PutService("/api/HumanResource/ERAcademic/v3", payload);
        } else {
          await PostService("/api/HumanResource/ERAcademic/v3", payload);
        }
      }

      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Education records saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save education records"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const getQualificationOptions = (row?: EmployeeEducationRow) =>
    row?.qualificationTypeId
      ? qualifications.filter((item) => item.qualificationTypeId === Number(row.qualificationTypeId))
      : qualifications;

  const renderTextInput = (
    row: EmployeeEducationRow | undefined,
    field: keyof Pick<EmployeeEducationRow, "institute" | "grade" | "division">,
    maxLength: number,
  ) =>
    row ? (
      <Box onClick={(event) => event.stopPropagation()}>
        <FormInput
          hideLabel
          size="small"
          value={row[field]}
          onChange={(event) =>
            updateRow(row.id, { [field]: event.target.value.slice(0, maxLength) })
          }
        />
      </Box>
    ) : (
      ""
    );

  const renderNumberInput = (
    row: EmployeeEducationRow | undefined,
    field: keyof Pick<EmployeeEducationRow, "cgpa" | "percentage" | "passingYear">,
    max?: number,
  ) =>
    row ? (
      <TextField
        type="number"
        size="small"
        value={row[field]}
        onClick={(event) => event.stopPropagation()}
        onChange={(event) =>
          updateRow(row.id, {
            [field]: event.target.value === ""
              ? ""
              : max === undefined
                ? Number(event.target.value)
                : Math.min(Number(event.target.value), max),
          })
        }
        inputProps={{ min: 0, max, style: { textAlign: "right" } }}
        sx={{ width: 130 }}
      />
    ) : (
      ""
    );

  const columns = useMemo(
    () => [
      {
        id: "sno",
        label: translate("S.No"),
        minWidth: 70,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "qualificationType",
        label: translate("Qualification Type"),
        minWidth: 220,
        format: (_: any, row?: EmployeeEducationRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.qualificationTypeId}
              options={qualificationTypes}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "qualificationType")}
              onSelect={(option) =>
                updateRow(row.id, {
                  qualificationTypeId: option?.id ?? "",
                  qualificationType: option?.name ?? "",
                  qualificationId: "",
                  qualification: "",
                })
              }
              placeholder={translate("Select Qualification Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "qualification",
        label: translate("Qualification"),
        minWidth: 220,
        format: (_: any, row?: EmployeeEducationRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.qualificationId}
              options={getQualificationOptions(row)}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "qualification")}
              onSelect={(option) =>
                updateRow(row.id, {
                  qualificationId: option?.id ?? "",
                  qualification: option?.name ?? "",
                })
              }
              placeholder={translate("Select Qualification")}
              disabled={!row.qualificationTypeId}
            />
          ) : (
            ""
          ),
      },
      {
        id: "institute",
        label: translate("Institution"),
        minWidth: 220,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderTextInput(row, "institute", 50),
      },
      {
        id: "cgpa",
        label: translate("CGPA"),
        minWidth: 140,
        align: "right" as const,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderNumberInput(row, "cgpa", 100),
      },
      {
        id: "percentage",
        label: translate("Percentage"),
        minWidth: 150,
        align: "right" as const,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderNumberInput(row, "percentage", 100),
      },
      {
        id: "grade",
        label: translate("Grade"),
        minWidth: 120,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderTextInput(row, "grade", 3),
      },
      {
        id: "division",
        label: translate("Division"),
        minWidth: 130,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderTextInput(row, "division", 3),
      },
      {
        id: "passingYear",
        label: translate("Passing Year"),
        minWidth: 150,
        align: "right" as const,
        format: (_: any, row?: EmployeeEducationRow) =>
          renderNumberInput(row, "passingYear", 9999),
      },
    ],
    [qualificationTypes, qualifications, translate],
  );

  const lookupRows = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (lookupState.kind === "qualification") return getQualificationOptions(row);
    if (lookupState.kind === "qualificationType") return qualificationTypes;
    return [];
  }, [qualificationTypes, qualifications, lookupState, rows]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    if (lookupState.kind === "qualificationType") return row.qualificationTypeId;
    if (lookupState.kind === "qualification") return row.qualificationId;
    return undefined;
  }, [lookupState, rows]);

  const lookupTitle =
    lookupState.kind === "qualificationType"
      ? translate("Qualification Type")
      : translate("Qualification");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Education")}
        columns={columns}
        rows={rows}
        hideTitle
        stickyHeader
        pagination={false}
        filtersEnabled={false}
        actions={false}
        checkboxSelection
        selectedIds={selectedIds}
        onSelectionChange={setSelectedIds}
        disableRowSelectionOnClick
        showToaster={false}
        onAddClick={handleAddRow}
        extraToolbarItemsLeft={
          <>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={handleDeleteSelected}
              disabled={!canDelete || selectedIds.length === 0}
              sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2 }}
            >
              <T>Delete Selected</T>
            </Button>
            <Button
              variant="outlined"
              startIcon={<QualificationIcon />}
              onClick={() => openSetupModal("qualificationType")}
              disabled={!canCreate}
              sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2, ml: 1 }}
            >
              <T>Qualification Type</T>
            </Button>
            <Button
              variant="outlined"
              startIcon={<AddIcon />}
              onClick={() => openSetupModal("qualification")}
              disabled={!canCreate}
              sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2, ml: 1 }}
            >
              <T>Qualification</T>
            </Button>
          </>
        }
        extraToolbarItemsRight={
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={handleSaveAll}
            disabled={!canUpdate || rows.length === 0}
            sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2 }}
          >
            <T>Save</T>
          </Button>
        }
      />

      <GenericSearchModal
        open={lookupState.open}
        onClose={closeLookup}
        title={lookupTitle}
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={lookupRows}
        selectedValue={selectedValue}
        selectedField="id"
        onSelect={(selectedRow: LookupOption) => {
          if (lookupState.rowId === null || !lookupState.kind) return;

          if (lookupState.kind === "qualificationType") {
            updateRow(lookupState.rowId, {
              qualificationTypeId: selectedRow?.id ?? "",
              qualificationType: selectedRow?.name ?? "",
              qualificationId: "",
              qualification: "",
            });
          }
          if (lookupState.kind === "qualification") {
            updateRow(lookupState.rowId, {
              qualificationId: selectedRow?.id ?? "",
              qualification: selectedRow?.name ?? "",
            });
          }
          closeLookup();
        }}
      />

      <Modal
        open={setupKind !== null}
        onClose={closeSetupModal}
        title={
          setupKind === "qualificationType"
            ? translate("Qualification Type")
            : translate("Qualification")
        }
        maxWidth="md"
        onReset={resetSetupForm}
        resetLabel={translate("New")}
        onSave={() => saveSetup(false)}
        onSaveAndClose={() => saveSetup(true)}
        saveLabel={translate("Save")}
        saveAndCloseLabel={translate("Save & Close")}
        contentPadding={3}
      >
        <Box
          sx={{
            bgcolor: "#fff",
            border: "1px solid var(--lms-border)",
            borderRadius: "16px",
            overflow: "hidden",
          }}
        >
          <Box
            sx={{
              px: 2.5,
              py: 2,
              borderBottom: "1px solid var(--lms-border)",
              bgcolor: "rgba(var(--lms-primary-rgb), 0.06)",
              fontWeight: 800,
              color: "var(--lms-text-main)",
            }}
          >
            <T>Basic Information</T>
          </Box>

          {setupKind === "qualificationType" && (
            <Box sx={{ maxWidth: 560, p: 2.5, display: "grid", gap: 2 }}>
              <FormInput
                label={translate("Code*")}
                required
                debounceMs={0}
                placeholder={translate("Enter Code")}
                value={qualificationTypeForm.code}
                onChange={(event) =>
                  setQualificationTypeForm((prev) => ({
                    ...prev,
                    code: event.target.value.slice(0, 10),
                  }))
                }
                error={setupSubmitted && !qualificationTypeForm.code.trim()}
              />
              <FormInput
                label={translate("Qualification Type*")}
                required
                debounceMs={0}
                placeholder={translate("Enter Qualification Type")}
                value={qualificationTypeForm.name}
                onChange={(event) =>
                  setQualificationTypeForm((prev) => ({
                    ...prev,
                    name: event.target.value.slice(0, 150),
                  }))
                }
                error={setupSubmitted && !qualificationTypeForm.name.trim()}
              />
              <FormInput
                label={translate("Description")}
                debounceMs={0}
                placeholder={translate("Enter Description")}
                value={qualificationTypeForm.description}
                multiline
                rows={3}
                onChange={(event) =>
                  setQualificationTypeForm((prev) => ({
                    ...prev,
                    description: event.target.value.slice(0, 500),
                  }))
                }
              />
            </Box>
          )}

          {setupKind === "qualification" && (
            <Box sx={{ maxWidth: 620, p: 2.5, display: "grid", gap: 2 }}>
              <FormInput
                label={translate("Code*")}
                required
                debounceMs={0}
                placeholder={translate("Enter Code")}
                value={qualificationForm.code}
                onChange={(event) =>
                  setQualificationForm((prev) => ({
                    ...prev,
                    code: event.target.value.slice(0, 10),
                  }))
                }
                error={setupSubmitted && !qualificationForm.code.trim()}
              />
              <LookupDropdown
                label={translate("Qualification Type*")}
                required
                value={qualificationForm.qualificationTypeId}
                options={qualificationTypes}
                getOptionLabel={(option) => option.name || ""}
                onSelect={(option) =>
                  setQualificationForm((prev) => ({
                    ...prev,
                    qualificationTypeId: option?.id ?? "",
                    qualificationTypeName: option?.name ?? "",
                  }))
                }
                onSearchClick={() => setSetupLookupOpen(true)}
                placeholder={translate("Select Qualification Type")}
                error={setupSubmitted && !qualificationForm.qualificationTypeId}
              />
              <FormInput
                label={translate("Qualification*")}
                required
                debounceMs={0}
                placeholder={translate("Enter Qualification")}
                value={qualificationForm.qualification}
                onChange={(event) =>
                  setQualificationForm((prev) => ({
                    ...prev,
                    qualification: event.target.value.slice(0, 150),
                  }))
                }
                error={setupSubmitted && !qualificationForm.qualification.trim()}
              />
              <FormInput
                label={translate("Description")}
                debounceMs={0}
                placeholder={translate("Enter Description")}
                value={qualificationForm.description}
                multiline
                rows={3}
                onChange={(event) =>
                  setQualificationForm((prev) => ({
                    ...prev,
                    description: event.target.value.slice(0, 500),
                  }))
                }
              />
            </Box>
          )}
        </Box>
      </Modal>

      <GenericSearchModal
        open={setupLookupOpen}
        onClose={() => setSetupLookupOpen(false)}
        title={translate("Qualification Type")}
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={qualificationTypes}
        selectedValue={qualificationForm.qualificationTypeId}
        selectedField="id"
        onSelect={(selectedRow: LookupOption) => {
          setQualificationForm((prev) => ({
            ...prev,
            qualificationTypeId: selectedRow?.id ?? "",
            qualificationTypeName: selectedRow?.name ?? "",
          }));
          setSetupLookupOpen(false);
        }}
      />

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />
    </>
  );
};
