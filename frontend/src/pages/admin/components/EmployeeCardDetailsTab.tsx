import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button } from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import dayjs, { Dayjs } from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormDatePicker } from "@/components/FormDatePicker";
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

interface EmployeeCardDetailsRow {
  id: number | string;
  employeeId?: number;
  cardTypeId: number | "";
  cardType: string;
  authorityId: number | "";
  authority: string;
  cardNo: string;
  validFrom: string;
  validTill: string;
}

interface EmployeeCardDetailsTabProps {
  employeeId: string | undefined;
}

type LookupKind = "cardType" | "authority";

const API = "/api/HumanResource/EREmpCards/v3";
const FAR_FUTURE = dayjs("9998-12-31");
const CARD_PERMISSION_SCREEN_NAME = "Card";
const CARD_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeCardDetailsRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  cardTypeId: "",
  cardType: "",
  authorityId: "",
  authority: "",
  cardNo: "",
  validFrom: dayjs().toISOString(),
  validTill: FAR_FUTURE.toISOString(),
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

const normalizeRow = (item: any): EmployeeCardDetailsRow => ({
  id: item.id ?? item.Id,
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  cardTypeId: item.cardTypeId ?? item.CardTypeId ?? item.cardtypeid ?? "",
  cardType:
    item.cardType ??
    item.CardType ??
    item.lov1?.stxt ??
    item.Lov1?.Stxt ??
    item.cardTypeLov?.stxt ??
    "",
  authorityId: item.authorityId ?? item.AuthorityId ?? item.authorityid ?? "",
  authority:
    item.authority ??
    item.Authority ??
    item.lov?.stxt ??
    item.Lov?.Stxt ??
    item.authorityLov?.stxt ??
    "",
  cardNo: item.cardNo ?? item.CardNo ?? item.cardno ?? "",
  validFrom: item.validFrom ?? item.ValidFrom ?? item.validfrom ?? "",
  validTill: item.validTill ?? item.ValidTill ?? item.validtill ?? "",
});

export const EmployeeCardDetailsTab: React.FC<EmployeeCardDetailsTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeCardDetailsRow[]>([]);
  const [cardTypes, setCardTypes] = useState<LookupOption[]>([]);
  const [authorities, setAuthorities] = useState<LookupOption[]>([]);
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
      GetService(`${API}/lookups/card-types`),
      GetService(`${API}/lookups/authorities`),
    ])
      .then(([cardTypeResponse, authorityResponse]) => {
        setCardTypes(normalizeLookup(cardTypeResponse?.data ?? cardTypeResponse));
        setAuthorities(normalizeLookup(authorityResponse?.data ?? authorityResponse));
      })
      .catch(() => {
        setToaster({
          open: true,
          messages: [translate("Failed to load card detail lookups")],
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
        messages: [translate("Failed to load card details")],
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
    patch: Partial<EmployeeCardDetailsRow>,
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
        messages: [translate("Selected card detail rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected card detail rows")],
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
      rows.some((row) => !row.cardTypeId)
        ? "Please select Card Type for all rows"
        : rows.some((row) => !row.cardNo?.trim())
          ? "Please enter Card No for all rows"
          : rows.some((row) => !row.validFrom)
            ? "Please set Valid From for all rows"
            : rows.some((row) => !row.validTill)
              ? "Please set Valid Till for all rows"
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
          cardTypeId: Number(row.cardTypeId),
          authorityId: row.authorityId === "" ? null : Number(row.authorityId),
          cardNo: row.cardNo.trim(),
          validFrom: row.validFrom || null,
          validTill: row.validTill || null,
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
        messages: [translate("Card details saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save card details"),
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
        id: "cardType",
        label: translate("Card Type"),
        minWidth: 240,
        format: (_: any, row?: EmployeeCardDetailsRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.cardTypeId}
              options={cardTypes}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "cardType")}
              onSelect={(option) =>
                updateRow(row.id, {
                  cardTypeId: option?.id ?? "",
                  cardType: option?.name ?? "",
                })
              }
              placeholder={translate("Select Card Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "authority",
        label: translate("Authority"),
        minWidth: 220,
        format: (_: any, row?: EmployeeCardDetailsRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.authorityId}
              options={authorities}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "authority")}
              onSelect={(option) =>
                updateRow(row.id, {
                  authorityId: option?.id ?? "",
                  authority: option?.name ?? "",
                })
              }
              placeholder={translate("Select Authority")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "cardNo",
        label: translate("Card No"),
        minWidth: 190,
        format: (_: any, row?: EmployeeCardDetailsRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()}>
              <FormInput
                hideLabel
                value={row.cardNo}
                onChange={(event) =>
                  updateRow(row.id, { cardNo: event.target.value.slice(0, 25) })
                }
                placeholder={translate("Card No")}
              />
            </Box>
          ) : (
            ""
          ),
      },
      {
        id: "validFrom",
        label: translate("Valid From"),
        minWidth: 180,
        format: (_: any, row?: EmployeeCardDetailsRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()} onMouseDown={(event) => event.stopPropagation()}>
              <FormDatePicker
                hideLabel
                value={row.validFrom ? dayjs(row.validFrom) : null}
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    validFrom: value ? value.toISOString() : "",
                  })
                }
                format="DD-MMM-YYYY"
              />
            </Box>
          ) : (
            ""
          ),
      },
      {
        id: "validTill",
        label: translate("Valid Till"),
        minWidth: 180,
        format: (_: any, row?: EmployeeCardDetailsRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()} onMouseDown={(event) => event.stopPropagation()}>
              <FormDatePicker
                hideLabel
                value={row.validTill ? dayjs(row.validTill) : null}
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    validTill: value ? value.toISOString() : "",
                  })
                }
                format="DD-MMM-YYYY"
              />
            </Box>
          ) : (
            ""
          ),
      },
    ],
    [authorities, cardTypes, translate],
  );

  const lookupRows = useMemo(() => {
    if (lookupState.kind === "cardType") return cardTypes;
    if (lookupState.kind === "authority") return authorities;
    return [];
  }, [authorities, cardTypes, lookupState.kind]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    return lookupState.kind === "cardType" ? row.cardTypeId : row.authorityId;
  }, [lookupState, rows]);

  const lookupTitle =
    lookupState.kind === "cardType"
      ? translate("Card Type")
      : translate("Authority");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Cards Detail")}
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

          if (lookupState.kind === "cardType") {
            updateRow(lookupState.rowId, {
              cardTypeId: selectedRow?.id ?? "",
              cardType: selectedRow?.name ?? "",
            });
          }

          if (lookupState.kind === "authority") {
            updateRow(lookupState.rowId, {
              authorityId: selectedRow?.id ?? "",
              authority: selectedRow?.name ?? "",
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
