import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Button } from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
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
  ratingId?: number;
  ratingValue?: number | null;
}

interface EmployeeSubjectRatingRow {
  id: number | string;
  employeeId?: number;
  subjectTypeId: number | "";
  subjectType: string;
  ratingDetailId: number | "";
  rating: string;
  ratingValue: number | "";
}

interface EmployeeSubjectRatingTabProps {
  employeeId: string | undefined;
}

type LookupKind = "subjectType" | "rating";

const API = "/api/HumanResource/EREmployeesubjectskill/v3";
const SUBJECT_RATING_PERMISSION_SCREEN_NAME = "Employee Subject Rating Skill";
const SUBJECT_RATING_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeSubjectRatingRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  subjectTypeId: "",
  subjectType: "",
  ratingDetailId: "",
  rating: "",
  ratingValue: "",
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
      code: item.code ?? item.Code ?? item.dtlcode ?? item.DtlCode ?? "",
      name:
        item.name ??
        item.Name ??
        item.stxt ??
        item.Stxt ??
        item.dtlstxt ??
        item.DtlStxt ??
        "",
      ratingId: Number(item.ratingId ?? item.RatingId ?? item.ratingid ?? 0) || undefined,
      ratingValue:
        item.ratingValue ??
        item.RatingValue ??
        item.dtlfactor ??
        item.DtlFactor ??
        null,
    }))
    .filter((item: LookupOption) => item.id > 0);

const normalizeRow = (item: any): EmployeeSubjectRatingRow => ({
  id: item.id ?? item.Id,
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  subjectTypeId:
    item.subjectTypeId ??
    item.SubjectTypeId ??
    item.subjecttypeid ??
    "",
  subjectType:
    item.subjectType ??
    item.SubjectType ??
    item.SMSubjecttype?.stxt ??
    item.SMSubjecttype?.Stxt ??
    "",
  ratingDetailId:
    item.ratingDetailId ??
    item.RatingDetailId ??
    item.ratingdtlid ??
    "",
  rating:
    item.rating ??
    item.Rating ??
    item.ERSkillratingdtl?.stxt ??
    item.ERSkillratingdtl?.Stxt ??
    "",
  ratingValue: item.ratingValue ?? item.RatingValue ?? item.ratingvalue ?? "",
});

export const EmployeeSubjectRatingTab: React.FC<EmployeeSubjectRatingTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeSubjectRatingRow[]>([]);
  const [subjectTypes, setSubjectTypes] = useState<LookupOption[]>([]);
  const [ratings, setRatings] = useState<LookupOption[]>([]);
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
      GetService(`${API}/lookups/subject-types`),
      GetService(`${API}/lookups/ratings`),
    ])
      .then(([subjectResponse, ratingResponse]) => {
        setSubjectTypes(normalizeLookup(subjectResponse?.data ?? subjectResponse));
        setRatings(normalizeLookup(ratingResponse?.data ?? ratingResponse));
      })
      .catch(() => {
        setToaster({
          open: true,
          messages: [translate("Failed to load subject rating lookups")],
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
        messages: [translate("Failed to load subject ratings")],
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
    patch: Partial<EmployeeSubjectRatingRow>,
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
        messages: [translate("Selected subject rating rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected subject rating rows")],
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
      rows.some((row) => !row.subjectTypeId) ? "Please select Subject Type for all rows" :
      rows.some((row) => !row.ratingDetailId) ? "Please select Rating for all rows" :
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
        const selectedRating = ratings.find((item) => item.id === Number(row.ratingDetailId));
        const payload = {
          id: typeof row.id === "number" ? row.id : undefined,
          employeeId: Number(employeeId),
          subjectTypeId: Number(row.subjectTypeId),
          ratingDetailId: Number(row.ratingDetailId),
          ratingValue:
            row.ratingValue === ""
              ? selectedRating?.ratingValue ?? null
              : Number(row.ratingValue),
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
        messages: [translate("Subject ratings saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save subject ratings"),
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
        id: "subjectType",
        label: translate("Subject Type"),
        minWidth: 260,
        format: (_: any, row?: EmployeeSubjectRatingRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.subjectTypeId}
              options={subjectTypes}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "subjectType")}
              onSelect={(option) =>
                updateRow(row.id, {
                  subjectTypeId: option?.id ?? "",
                  subjectType: option?.name ?? "",
                })
              }
              placeholder={translate("Select Subject Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "rating",
        label: translate("Rating"),
        minWidth: 240,
        format: (_: any, row?: EmployeeSubjectRatingRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.ratingDetailId}
              options={ratings}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "rating")}
              onSelect={(option) =>
                updateRow(row.id, {
                  ratingDetailId: option?.id ?? "",
                  rating: option?.name ?? "",
                  ratingValue: option?.ratingValue ?? "",
                })
              }
              placeholder={translate("Select Rating")}
            />
          ) : (
            ""
          ),
      },
    ],
    [ratings, subjectTypes, translate],
  );

  const lookupRows = useMemo(() => {
    if (lookupState.kind === "subjectType") return subjectTypes;
    if (lookupState.kind === "rating") return ratings;
    return [];
  }, [lookupState.kind, ratings, subjectTypes]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    if (lookupState.kind === "subjectType") return row.subjectTypeId;
    if (lookupState.kind === "rating") return row.ratingDetailId;
    return undefined;
  }, [lookupState, rows]);

  const lookupTitle =
    lookupState.kind === "subjectType"
      ? translate("Subject Type")
      : translate("Rating");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Subject Rating")}
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
        columns={
          lookupState.kind === "rating"
            ? [
                { id: "id", label: translate("Id"), minWidth: 90 },
                { id: "code", label: translate("Code"), minWidth: 110 },
                { id: "name", label: translate("Rating"), minWidth: 220 },
                { id: "ratingValue", label: translate("Factor"), minWidth: 100 },
              ]
            : [
                { id: "id", label: translate("Id"), minWidth: 90 },
                { id: "code", label: translate("Code"), minWidth: 110 },
                { id: "name", label: translate("Name"), minWidth: 240 },
              ]
        }
        data={lookupRows}
        selectedValue={selectedValue}
        selectedField="id"
        onSelect={(selectedRow: LookupOption) => {
          if (lookupState.rowId === null || !lookupState.kind) return;

          if (lookupState.kind === "subjectType") {
            updateRow(lookupState.rowId, {
              subjectTypeId: selectedRow?.id ?? "",
              subjectType: selectedRow?.name ?? "",
            });
          }

          if (lookupState.kind === "rating") {
            updateRow(lookupState.rowId, {
              ratingDetailId: selectedRow?.id ?? "",
              rating: selectedRow?.name ?? "",
              ratingValue: selectedRow?.ratingValue ?? "",
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
