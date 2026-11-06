import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button, Dialog, DialogContent, DialogTitle, IconButton, TextField } from "@mui/material";
import {
  Close as CloseIcon,
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import SkillType from "../SkillType";
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
  skillTypeId?: number;
  skillRatingId?: number;
}

interface EmployeeSkillRow {
  id: number | string;
  employeeId?: number;
  skillTypeId: number | "";
  skillType: string;
  skillId: number | "";
  skill: string;
  skillRatingId: number | "";
  skillRating: string;
  skillRatingDetailId: number | "";
  skillRatingDetail: string;
  periodUnitId: number | "";
  periodUnit: string;
  period: number | "";
  remarks: string;
}

interface EmployeeSkillTabProps {
  employeeId: string | undefined;
}

type LookupKind = "skillType" | "skill" | "rating" | "periodUnit";

const API = "/api/HumanResource/EREmpSkill/v3";
const SKILL_PERMISSION_SCREEN_NAME = "Skill";
const SKILL_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeSkillRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  skillTypeId: "",
  skillType: "",
  skillId: "",
  skill: "",
  skillRatingId: "",
  skillRating: "",
  skillRatingDetailId: "",
  skillRatingDetail: "",
  periodUnitId: "",
  periodUnit: "",
  period: "",
  remarks: "",
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
      skillTypeId: Number(item.skillTypeId ?? item.SkillTypeId ?? item.skilltypeid ?? 0) || undefined,
      skillRatingId: Number(item.skillRatingId ?? item.SkillRatingId ?? item.skillratingid ?? 0) || undefined,
    }))
    .filter((item: LookupOption) => item.id > 0);

const normalizeRow = (item: any): EmployeeSkillRow => ({
  id: item.id ?? item.Id,
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  skillTypeId: item.skillTypeId ?? item.SkillTypeId ?? item.skilltypeid ?? "",
  skillType: item.skillType ?? item.SkillType ?? item.ERSkilltype?.stxt ?? "",
  skillId: item.skillId ?? item.SkillId ?? item.skillid ?? "",
  skill: item.skill ?? item.Skill ?? item.ERSkill?.stxt ?? "",
  skillRatingId: item.skillRatingId ?? item.SkillRatingId ?? item.skillratingid ?? "",
  skillRating: item.skillRating ?? item.SkillRating ?? item.ERSkillratinghdr?.stxt ?? "",
  skillRatingDetailId: item.skillRatingDetailId ?? item.SkillRatingDetailId ?? item.skillratingdtlid ?? "",
  skillRatingDetail: item.skillRatingDetail ?? item.SkillRatingDetail ?? item.ERSkillratingdtl?.stxt ?? "",
  periodUnitId: item.periodUnitId ?? item.PeriodUnitId ?? item.punitid ?? "",
  periodUnit: item.periodUnit ?? item.PeriodUnit ?? item.lov?.stxt ?? "",
  period: item.period ?? item.Period ?? "",
  remarks: item.remarks ?? item.Remarks ?? "",
});

export const EmployeeSkillTab: React.FC<EmployeeSkillTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeSkillRow[]>([]);
  const [skillTypes, setSkillTypes] = useState<LookupOption[]>([]);
  const [skills, setSkills] = useState<LookupOption[]>([]);
  const [ratingsBySkill, setRatingsBySkill] = useState<Record<number, LookupOption[]>>({});
  const [periodUnits, setPeriodUnits] = useState<LookupOption[]>([]);
  const [skillTypeDialogOpen, setSkillTypeDialogOpen] = useState(false);
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

  const loadLookups = useCallback(async () => {
    const [typeResponse, skillResponse, periodResponse] = await Promise.all([
      GetService(`${API}/lookups/skill-types`),
      GetService(`${API}/lookups/skills`),
      GetService(`${API}/lookups/period-units`),
    ]);

    setSkillTypes(normalizeLookup(typeResponse?.data ?? typeResponse));
    setSkills(normalizeLookup(skillResponse?.data ?? skillResponse));
    setPeriodUnits(normalizeLookup(periodResponse?.data ?? periodResponse));
  }, []);

  useEffect(() => {
    setLoading(true);
    void loadLookups()
      .catch(() => {
        setToaster({
          open: true,
          messages: [translate("Failed to load skill lookups")],
          severity: "error",
        });
      })
      .finally(() => setLoading(false));
  }, [loadLookups, translate]);

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
        messages: [translate("Failed to load skill records")],
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
    patch: Partial<EmployeeSkillRow>,
  ) => {
    setRows((prev) =>
      prev.map((row) => (row.id === rowId ? { ...row, ...patch } : row)),
    );
  };

  const getSkillOptions = (row?: EmployeeSkillRow) =>
    row?.skillTypeId
      ? skills.filter((item) => item.skillTypeId === Number(row.skillTypeId))
      : skills;

  const loadRatingsForSkill = useCallback(async (skillId: number) => {
    if (ratingsBySkill[skillId]) return ratingsBySkill[skillId];

    const response: any = await GetService(`${API}/lookups/ratings?skillId=${skillId}`);
    const options = normalizeLookup(response?.data ?? response);
    setRatingsBySkill((prev) => ({ ...prev, [skillId]: options }));
    return options;
  }, [ratingsBySkill]);

  const getRatingOptions = (row?: EmployeeSkillRow) =>
    row?.skillId ? ratingsBySkill[Number(row.skillId)] ?? [] : [];

  const openLookup = async (rowId: number | string, kind: LookupKind) => {
    const row = rows.find((item) => item.id === rowId);
    if (kind === "rating" && row?.skillId) {
      try {
        setLoading(true);
        await loadRatingsForSkill(Number(row.skillId));
      } finally {
        setLoading(false);
      }
    }
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
        messages: [translate("Selected skill rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected skill rows")],
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
      rows.some((row) => !row.skillTypeId) ? "Please select Skill Type for all rows" :
      rows.some((row) => !row.skillId) ? "Please select Skill for all rows" :
      rows.some((row) => !row.skillRatingDetailId) ? "Please select Rating for all rows" :
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
        const selectedSkill = skills.find((item) => item.id === Number(row.skillId));
        const payload = {
          id: typeof row.id === "number" ? row.id : undefined,
          employeeId: Number(employeeId),
          skillTypeId: Number(row.skillTypeId),
          skillId: Number(row.skillId),
          skillRatingId: row.skillRatingId === "" ? selectedSkill?.skillRatingId ?? null : Number(row.skillRatingId),
          skillRatingDetailId: Number(row.skillRatingDetailId),
          periodUnitId: row.periodUnitId === "" ? null : Number(row.periodUnitId),
          period: row.period === "" ? null : Number(row.period),
          remarks: row.remarks.trim(),
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
        messages: [translate("Skill records saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save skill records"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const renderNumberInput = (row: EmployeeSkillRow | undefined) =>
    row ? (
      <TextField
        type="number"
        size="small"
        value={row.period}
        onClick={(event) => event.stopPropagation()}
        onChange={(event) =>
          updateRow(row.id, {
            period: event.target.value === "" ? "" : Number(event.target.value),
          })
        }
        inputProps={{ min: 0, style: { textAlign: "right" } }}
        sx={{ width: 120 }}
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
        id: "skillType",
        label: translate("Skill Type"),
        minWidth: 220,
        format: (_: any, row?: EmployeeSkillRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.skillTypeId}
              options={skillTypes}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => void openLookup(row.id, "skillType")}
              onSelect={(option) =>
                updateRow(row.id, {
                  skillTypeId: option?.id ?? "",
                  skillType: option?.name ?? "",
                  skillId: "",
                  skill: "",
                  skillRatingId: "",
                  skillRating: "",
                  skillRatingDetailId: "",
                  skillRatingDetail: "",
                })
              }
              placeholder={translate("Select Skill Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "skill",
        label: translate("Skill"),
        minWidth: 220,
        format: (_: any, row?: EmployeeSkillRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.skillId}
              options={getSkillOptions(row)}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => void openLookup(row.id, "skill")}
              onSelect={(option) => {
                updateRow(row.id, {
                  skillId: option?.id ?? "",
                  skill: option?.name ?? "",
                  skillRatingId: option?.skillRatingId ?? "",
                  skillRatingDetailId: "",
                  skillRatingDetail: "",
                });
                if (option?.id) void loadRatingsForSkill(option.id);
              }}
              placeholder={translate("Select Skill")}
              disabled={!row.skillTypeId}
            />
          ) : (
            ""
          ),
      },
      {
        id: "skillRatingDetail",
        label: translate("Rating"),
        minWidth: 200,
        format: (_: any, row?: EmployeeSkillRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.skillRatingDetail || row.skillRatingDetailId}
              options={getRatingOptions(row)}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => void openLookup(row.id, "rating")}
              onSelect={(option) =>
                updateRow(row.id, {
                  skillRatingDetailId: option?.id ?? "",
                  skillRatingDetail: option?.name ?? "",
                })
              }
              placeholder={translate("Select Rating")}
              disabled={!row.skillId}
            />
          ) : (
            ""
          ),
      },
      {
        id: "periodUnit",
        label: translate("Period Unit"),
        minWidth: 180,
        format: (_: any, row?: EmployeeSkillRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.periodUnitId}
              options={periodUnits}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => void openLookup(row.id, "periodUnit")}
              onSelect={(option) =>
                updateRow(row.id, {
                  periodUnitId: option?.id ?? "",
                  periodUnit: option?.name ?? "",
                })
              }
              placeholder={translate("Select Unit")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "period",
        label: translate("Period"),
        minWidth: 130,
        align: "right" as const,
        format: (_: any, row?: EmployeeSkillRow) => renderNumberInput(row),
      },
      {
        id: "remarks",
        label: translate("Remarks"),
        minWidth: 240,
        format: (_: any, row?: EmployeeSkillRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()}>
              <FormInput
                hideLabel
                size="small"
                value={row.remarks}
                onChange={(event) =>
                  updateRow(row.id, { remarks: event.target.value.slice(0, 500) })
                }
              />
            </Box>
          ) : (
            ""
          ),
      },
    ],
    [skillTypes, skills, ratingsBySkill, periodUnits, translate, loadRatingsForSkill],
  );

  const lookupRows = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (lookupState.kind === "skillType") return skillTypes;
    if (lookupState.kind === "skill") return getSkillOptions(row);
    if (lookupState.kind === "rating") return getRatingOptions(row);
    if (lookupState.kind === "periodUnit") return periodUnits;
    return [];
  }, [skillTypes, skills, ratingsBySkill, periodUnits, lookupState, rows]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    if (lookupState.kind === "skillType") return row.skillTypeId;
    if (lookupState.kind === "skill") return row.skillId;
    if (lookupState.kind === "rating") return row.skillRatingDetailId;
    if (lookupState.kind === "periodUnit") return row.periodUnitId;
    return undefined;
  }, [lookupState, rows]);

  const lookupTitle =
    lookupState.kind === "skillType"
      ? translate("Skill Type")
      : lookupState.kind === "skill"
        ? translate("Skill")
        : lookupState.kind === "rating"
          ? translate("Rating")
          : translate("Period Unit");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Skill")}
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
          <Box sx={{ display: "flex", gap: 1 }}>
            <Button
              variant="outlined"
              onClick={() => canCreate && setSkillTypeDialogOpen(true)}
              disabled={!canCreate}
              sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2 }}
            >
              <T>Skill Type</T>
            </Button>
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
          </Box>
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

          if (lookupState.kind === "skillType") {
            updateRow(lookupState.rowId, {
              skillTypeId: selectedRow?.id ?? "",
              skillType: selectedRow?.name ?? "",
              skillId: "",
              skill: "",
              skillRatingId: "",
              skillRating: "",
              skillRatingDetailId: "",
              skillRatingDetail: "",
            });
          }
          if (lookupState.kind === "skill") {
            updateRow(lookupState.rowId, {
              skillId: selectedRow?.id ?? "",
              skill: selectedRow?.name ?? "",
              skillRatingId: selectedRow?.skillRatingId ?? "",
              skillRatingDetailId: "",
              skillRatingDetail: "",
            });
            if (selectedRow?.id) void loadRatingsForSkill(selectedRow.id);
          }
          if (lookupState.kind === "rating") {
            updateRow(lookupState.rowId, {
              skillRatingDetailId: selectedRow?.id ?? "",
              skillRatingDetail: selectedRow?.name ?? "",
            });
          }
          if (lookupState.kind === "periodUnit") {
            updateRow(lookupState.rowId, {
              periodUnitId: selectedRow?.id ?? "",
              periodUnit: selectedRow?.name ?? "",
            });
          }
          closeLookup();
        }}
      />

      <Dialog
        open={skillTypeDialogOpen}
        onClose={() => {
          setSkillTypeDialogOpen(false);
          void loadLookups();
        }}
        fullWidth
        maxWidth="lg"
      >
        <DialogTitle sx={{ fontWeight: 900, pr: 6 }}>
          <T>Skill Type</T>
          <IconButton
            aria-label={translate("Close")}
            onClick={() => {
              setSkillTypeDialogOpen(false);
              void loadLookups();
            }}
            sx={{ position: "absolute", right: 12, top: 10 }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers sx={{ p: 0 }}>
          <SkillType
            legacyDialog
            onClose={() => {
              setSkillTypeDialogOpen(false);
              void loadLookups();
            }}
          />
        </DialogContent>
      </Dialog>

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />
    </>
  );
};
