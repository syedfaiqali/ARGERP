import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button } from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";

interface LookupOption {
  id: number;
  code?: string;
  name: string;
}

interface EmployeeDiseaseRow {
  id: number | string;
  employeeId?: number;
  diseaseCategoryId: number | "";
  diseaseCategory: string;
  diseaseTypeId: number | "";
  diseaseType: string;
  diseaseId: number | "";
  disease: string;
  diseaseName: string;
  description: string;
}

interface EmployeeDiseaseTabProps {
  employeeId: string | undefined;
}

type LookupKind = "category" | "type";

const API = "/api/HumanResource/EREmployeedisease/v3";
const DISEASE_PERMISSION_SCREEN_NAME = "Disease";
const DISEASE_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeDiseaseRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  diseaseCategoryId: "",
  diseaseCategory: "",
  diseaseTypeId: "",
  diseaseType: "",
  diseaseId: "",
  disease: "",
  diseaseName: "",
  description: "",
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
      name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? "",
    }))
    .filter((item: LookupOption) => item.id > 0);

const normalizeRow = (item: any): EmployeeDiseaseRow => ({
  id: item.id ?? item.Id,
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  diseaseCategoryId:
    item.diseaseCategoryId ?? item.DiseaseCategoryId ?? item.discatid ?? "",
  diseaseCategory:
    item.diseaseCategory ??
    item.DiseaseCategory ??
    item.lov1?.stxt ??
    item.Lov1?.Stxt ??
    "",
  diseaseTypeId: item.diseaseTypeId ?? item.DiseaseTypeId ?? item.distypeid ?? "",
  diseaseType:
    item.diseaseType ??
    item.DiseaseType ??
    item.lov?.stxt ??
    item.Lov?.Stxt ??
    "",
  diseaseId: item.diseaseId ?? item.DiseaseId ?? item.diseasesid ?? "",
  disease: item.disease ?? item.Disease ?? item.SMDisease?.stxt ?? "",
  diseaseName:
    item.diseaseName ??
    item.DiseaseName ??
    item.stxt ??
    item.Stxt ??
    item.SMDisease?.stxt ??
    "",
  description: item.description ?? item.Description ?? item.ltxt ?? item.Ltxt ?? "",
});

export const EmployeeDiseaseTab: React.FC<EmployeeDiseaseTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeDiseaseRow[]>([]);
  const [categories, setCategories] = useState<LookupOption[]>([]);
  const [types, setTypes] = useState<LookupOption[]>([]);
  const [selectedIds, setSelectedIds] = useState<any[]>([]);
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

  useEffect(() => {
    setLoading(true);
    void Promise.all([
      GetService(`${API}/lookups/categories`),
      GetService(`${API}/lookups/types`),
    ])
      .then(([categoryResponse, typeResponse]) => {
        setCategories(normalizeLookup(categoryResponse?.data ?? categoryResponse));
        setTypes(normalizeLookup(typeResponse?.data ?? typeResponse));
      })
      .catch(() => {
        setToaster({
          open: true,
          messages: [translate("Failed to load disease lookups")],
          severity: "error",
        });
      })
      .finally(() => setLoading(false));
  }, [translate]);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `${API}?employeeId=${employeeId}&pageSize=1000`,
      );
      setRows(itemsFrom(response).map(normalizeRow));
      setSelectedIds([]);
    } catch {
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load diseases")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const updateRow = (rowId: number | string, patch: Partial<EmployeeDiseaseRow>) => {
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

  const handleDeleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      setLoading(true);
      for (const row of rows.filter((item) => selectedIds.includes(item.id))) {
        if (typeof row.id === "number") {
          await DeleteService(`${API}/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected disease rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected disease rows")],
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
      rows.some((row) => !row.diseaseCategoryId)
        ? "Please select Category for all rows"
        : rows.some((row) => !row.diseaseTypeId)
          ? "Please select Type for all rows"
          : rows.some((row) => !row.diseaseName?.trim())
            ? "Please enter Disease Name for all rows"
            : null;

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
          diseaseCategoryId: Number(row.diseaseCategoryId),
          diseaseTypeId: Number(row.diseaseTypeId),
          diseaseId: row.diseaseId === "" ? null : Number(row.diseaseId),
          diseaseName: row.diseaseName.trim(),
          description: row.description?.trim() || "",
        };

        if (typeof row.id === "number") {
          await PutService(API, payload);
        } else {
          await PostService(API, payload);
        }
      }

      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Diseases saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save diseases"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () => [
      {
        id: "sno",
        label: translate("S.No"),
        minWidth: 70,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "diseaseCategory",
        label: translate("Category"),
        minWidth: 220,
        format: (_: any, row?: EmployeeDiseaseRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.diseaseCategoryId}
              options={categories}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "category")}
              onSelect={(option) =>
                updateRow(row.id, {
                  diseaseCategoryId: option?.id ?? "",
                  diseaseCategory: option?.name ?? "",
                })
              }
              placeholder={translate("Select Category")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "diseaseType",
        label: translate("Type"),
        minWidth: 220,
        format: (_: any, row?: EmployeeDiseaseRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.diseaseTypeId}
              options={types}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "type")}
              onSelect={(option) =>
                updateRow(row.id, {
                  diseaseTypeId: option?.id ?? "",
                  diseaseType: option?.name ?? "",
                })
              }
              placeholder={translate("Select Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "diseaseName",
        label: translate("Disease Name"),
        minWidth: 260,
        format: (_: any, row?: EmployeeDiseaseRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()}>
              <FormInput
                hideLabel
                value={row.diseaseName}
                onChange={(event) =>
                  updateRow(row.id, {
                    diseaseId: "",
                    disease: "",
                    diseaseName: event.target.value.slice(0, 150),
                  })
                }
                placeholder={translate("Enter Disease Name")}
              />
            </Box>
          ) : (
            ""
          ),
      },
      {
        id: "description",
        label: translate("Description"),
        minWidth: 320,
        format: (_: any, row?: EmployeeDiseaseRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()}>
              <FormInput
                hideLabel
                value={row.description}
                onChange={(event) =>
                  updateRow(row.id, {
                    description: event.target.value.slice(0, 500),
                  })
                }
                placeholder={translate("Description")}
              />
            </Box>
          ) : (
            ""
          ),
      },
    ],
    [categories, translate, types],
  );

  const lookupRows = useMemo(() => {
    if (lookupState.kind === "category") return categories;
    if (lookupState.kind === "type") return types;
    return [];
  }, [categories, lookupState.kind, types]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    if (lookupState.kind === "category") return row.diseaseCategoryId;
    return row.diseaseTypeId;
  }, [lookupState, rows]);

  const lookupTitle =
    lookupState.kind === "category" ? translate("Category") : translate("Type");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Diseases")}
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

          if (lookupState.kind === "category") {
            updateRow(lookupState.rowId, {
              diseaseCategoryId: selectedRow?.id ?? "",
              diseaseCategory: selectedRow?.name ?? "",
            });
          }

          if (lookupState.kind === "type") {
            updateRow(lookupState.rowId, {
              diseaseTypeId: selectedRow?.id ?? "",
              diseaseType: selectedRow?.name ?? "",
            });
          }

          closeLookup();
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
