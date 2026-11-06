import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button, Checkbox, FormControlLabel } from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import dayjs, { Dayjs } from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

const FAR_FUTURE = dayjs("9998-12-31");
const EMPLOYEE_ROSTER_PERMISSION_SCREEN_NAME = "Employee Roster";
const EMPLOYEE_ROSTER_PERMISSION_PARENT_NAME = "Employee";

interface LookupOption {
  id: number;
  name: string;
  code?: string;
}

interface EmployeeRosterRow {
  id: number | string;
  shiftId: number | "";
  shiftName?: string;
  leaveGroupId: number | "";
  leaveGroupName?: string;
  lateAllowed: boolean;
  earlyDeparture: boolean;
  overtimeAllowed: boolean;
  startDate: string;
  endDate: string;
  taxExempt: boolean;
}

interface EmployeeRosterTabProps {
  employeeId: string | undefined;
}

const createEmptyRow = (): EmployeeRosterRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  shiftId: "",
  shiftName: "",
  leaveGroupId: "",
  leaveGroupName: "",
  lateAllowed: false,
  earlyDeparture: false,
  overtimeAllowed: false,
  startDate: dayjs().toISOString(),
  endDate: FAR_FUTURE.toISOString(),
  taxExempt: false,
});

const normalizeLookup = (payload: any): LookupOption[] => {
  const items: any[] =
    payload?.items ??
    payload?.Items ??
    payload?.data ??
    payload?.Data ??
    (Array.isArray(payload) ? payload : []);

  return items
    .map((item: any) => ({
      id: Number(item.id ?? item.Id ?? 0),
      code: item.code ?? item.Code ?? "",
      name:
        item.name ??
        item.Name ??
        item.stxt ??
        item.Stxt ??
        item.shiftName ??
        item.leaveGroup ??
        item.leaveGroupName ??
        "",
    }))
    .filter((item) => item.id > 0);
};

export const EmployeeRosterTab: React.FC<EmployeeRosterTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeRosterRow[]>([]);
  const [shifts, setShifts] = useState<LookupOption[]>([]);
  const [leaveGroups, setLeaveGroups] = useState<LookupOption[]>([]);
  const [selectedIds, setSelectedIds] = useState<any[]>([]);
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({
    open: false,
    messages: [],
    severity: "info",
  });
  const [lookupState, setLookupState] = useState<{
    open: boolean;
    type: "shift" | "leaveGroup" | null;
    rowId: number | string | null;
  }>({
    open: false,
    type: null,
    rowId: null,
  });
  const [loading, setLoading] = useState(false);
  const canCreate = true;
  const canUpdate = true;
  const canDelete = true;

  useEffect(() => {
    setLoading(true);
    void Promise.all([
      GetService("/api/HumanResource/EmployeeProfile/v3/lookups/shifts"),
      GetService("/api/HumanResource/EmployeeProfile/v3/lookups/leave-groups"),
    ])
      .then(([shiftResponse, leaveGroupResponse]) => {
        setShifts(normalizeLookup(shiftResponse?.data ?? shiftResponse));
        setLeaveGroups(normalizeLookup(leaveGroupResponse?.data ?? leaveGroupResponse));
      })
      .catch(() => {
        setLoading(false);
        setToaster({
          open: true,
          messages: [translate("Failed to load roster lookups")],
          severity: "error",
        });
      })
      .finally(() => {
        setLoading(false);
      });
  }, [translate]);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `/api/HumanResource/EREmproaster/v3?employeeId=${employeeId}&pageSize=1000`,
      );
      const payload = response?.data ?? response ?? {};
      const items: any[] =
        payload?.items ??
        payload?.Items ??
        payload?.data ??
        payload?.Data ??
        (Array.isArray(payload) ? payload : []);

      setRows(
        items.map((item: any) => ({
          id: item.id,
          shiftId: item.shiftId ?? "",
          shiftName: item.shiftName ?? "",
          leaveGroupId: item.leaveGroupId ?? "",
          leaveGroupName: item.leaveGroupName ?? "",
          lateAllowed: Boolean(item.lateAllowed),
          earlyDeparture: Boolean(item.earlyDeparture),
          overtimeAllowed: Boolean(item.overtimeAllowed),
          startDate: item.startDate ?? "",
          endDate: item.endDate ?? FAR_FUTURE.toISOString(),
          taxExempt: Boolean(item.taxExempt),
        })),
      );
      setSelectedIds([]);
    } catch {
      setLoading(false);
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load employee roster")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const lookupOptions = useMemo(() => {
    if (lookupState.type === "shift") return shifts;
    if (lookupState.type === "leaveGroup") return leaveGroups;
    return [];
  }, [leaveGroups, lookupState.type, shifts]);

  const leaveGroupOptions = useMemo(() => {
    const byId = new Map<number, LookupOption>();
    leaveGroups.forEach((item) => byId.set(item.id, item));
    rows.forEach((row) => {
      const id = Number(row.leaveGroupId || 0);
      if (id > 0 && !byId.has(id)) {
        byId.set(id, {
          id,
          code: "",
          name: row.leaveGroupName || String(id),
        });
      }
    });
    return Array.from(byId.values());
  }, [leaveGroups, rows]);

  const updateRow = (
    rowId: number | string,
    patch: Partial<EmployeeRosterRow>,
  ) => {
    setRows((prev) =>
      prev.map((row) => (row.id === rowId ? { ...row, ...patch } : row)),
    );
  };

  const openLookup = (
    rowId: number | string,
    type: "shift" | "leaveGroup",
  ) => {
    setLookupState({ open: true, type, rowId });
  };

  const closeLookup = () => {
    setLookupState({ open: false, type: null, rowId: null });
  };

  const handleAddRow = () => {
    if (!canCreate) return;

    const nextRow = createEmptyRow();
    const previousEndDate = dayjs(nextRow.startDate).subtract(1, "day").toISOString();
    setRows((prev) => [
      ...prev.map((row) =>
        row.endDate && dayjs(row.endDate).year() === 9998
          ? { ...row, endDate: previousEndDate }
          : row,
      ),
      nextRow,
    ]);
  };

  const handleDeleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      setLoading(true);
      const selectedRows = rows.filter((row) => selectedIds.includes(row.id));

      for (const row of selectedRows) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/EREmproaster/v3/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected roster rows deleted successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setLoading(false);
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            translate("Failed to delete selected roster rows"),
        ],
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

    if (rows.some((row) => !row.shiftId)) {
      setToaster({
        open: true,
        messages: [translate("Please select Shift for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.startDate)) {
      setToaster({
        open: true,
        messages: [translate("Please set Start Date for all rows")],
        severity: "error",
      });
      return;
    }

    try {

      for (const row of rows) {
        const payload = {
          id: typeof row.id === "number" ? row.id : undefined,
          employeeId: Number(employeeId),
          shiftId: Number(row.shiftId),
          leaveGroupId: row.leaveGroupId ? Number(row.leaveGroupId) : null,
          lateAllowed: row.lateAllowed,
          earlyDeparture: row.earlyDeparture,
          overtimeAllowed: row.overtimeAllowed,
          startDate: row.startDate,
          endDate: row.endDate,
          taxId: null,
          taxExempt: row.taxExempt,
        };

        if (typeof row.id === "number") {
          await PutService("/api/HumanResource/EREmproaster/v3", payload);
        } else {
          await PostService("/api/HumanResource/EREmproaster/v3", payload);
        }
      }

      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Employee roster saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setLoading(false);
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
          error?.response?.data ||
          error?.message ||
          translate("Failed to save employee roster"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const renderCheckbox = (
    row: EmployeeRosterRow | undefined,
    field: "lateAllowed" | "earlyDeparture" | "overtimeAllowed" | "taxExempt",
    label: string,
  ) =>
    row ? (
      <Box onClick={(e) => e.stopPropagation()} sx={{ minWidth: 130 }}>
        <FormControlLabel
          control={
            <Checkbox
              size="small"
              checked={row[field]}
              onChange={(event) =>
                updateRow(row.id, { [field]: event.target.checked })
              }
            />
          }
          label={translate(label)}
          sx={{
            m: 0,
            "& .MuiFormControlLabel-label": {
              fontSize: "0.8125rem",
              fontWeight: 600,
              whiteSpace: "nowrap",
            },
          }}
        />
      </Box>
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
        id: "shiftName",
        label: translate("Shift"),
        minWidth: 220,
        format: (_: any, row?: EmployeeRosterRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.shiftId}
              options={shifts}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) =>
                updateRow(row.id, {
                  shiftId: option?.id ?? "",
                  shiftName: option?.name ?? "",
                })
              }
              placeholder={translate("Select Shift")}
              onSearchClick={() => openLookup(row.id, "shift")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "leaveGroupName",
        label: translate("Leave Group"),
        minWidth: 220,
        format: (_: any, row?: EmployeeRosterRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.leaveGroupId}
              options={leaveGroupOptions}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) =>
                updateRow(row.id, {
                  leaveGroupId: option?.id ?? "",
                  leaveGroupName: option?.name ?? "",
                })
              }
              placeholder={translate("Select Leave Group")}
              onSearchClick={() => openLookup(row.id, "leaveGroup")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "lateAllowed",
        label: translate("Late Allowed"),
        minWidth: 150,
        format: (_: any, row?: EmployeeRosterRow) =>
          renderCheckbox(row, "lateAllowed", "Late Allowed"),
      },
      {
        id: "earlyDeparture",
        label: translate("Early Departure"),
        minWidth: 170,
        format: (_: any, row?: EmployeeRosterRow) =>
          renderCheckbox(row, "earlyDeparture", "Early Departure"),
      },
      {
        id: "overtimeAllowed",
        label: translate("Overtime"),
        minWidth: 145,
        format: (_: any, row?: EmployeeRosterRow) =>
          renderCheckbox(row, "overtimeAllowed", "Overtime"),
      },
      {
        id: "startDate",
        label: translate("Start Date"),
        minWidth: 180,
        format: (_: any, row?: EmployeeRosterRow) =>
          row ? (
            <Box onClick={(e) => e.stopPropagation()}>
              <FormDatePicker
                hideLabel
                value={row.startDate ? dayjs(row.startDate) : null}
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    startDate: value ? value.toISOString() : "",
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
        id: "endDate",
        label: translate("End Date"),
        minWidth: 180,
        format: (_: any, row?: EmployeeRosterRow) =>
          row ? (
            <Box onClick={(e) => e.stopPropagation()}>
              <FormDatePicker
                hideLabel
                value={row.endDate ? dayjs(row.endDate) : null}
                disabled
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    endDate: value ? value.toISOString() : "",
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
        id: "taxExempt",
        label: translate("Tax Exempt"),
        minWidth: 150,
        format: (_: any, row?: EmployeeRosterRow) =>
          renderCheckbox(row, "taxExempt", "Tax Exempt"),
      },
    ],
    [leaveGroupOptions, shifts, translate],
  );

  const selectedValue =
    lookupState.rowId !== null
      ? lookupState.type === "shift"
        ? rows.find((row) => row.id === lookupState.rowId)?.shiftId
        : rows.find((row) => row.id === lookupState.rowId)?.leaveGroupId
      : undefined;

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Employee Roster")}
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

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />

      <GenericSearchModal
        open={lookupState.open}
        onClose={closeLookup}
        title={
          lookupState.type === "shift"
            ? translate("Shift")
            : translate("Leave Group")
        }
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={lookupOptions}
        selectedValue={selectedValue}
        selectedField="id"
        onSelect={(selectedRow: any) => {
          if (lookupState.rowId !== null) {
            if (lookupState.type === "shift") {
              updateRow(lookupState.rowId, {
                shiftId: selectedRow?.id ?? "",
                shiftName: selectedRow?.name ?? "",
              });
            } else if (lookupState.type === "leaveGroup") {
              updateRow(lookupState.rowId, {
                leaveGroupId: selectedRow?.id ?? "",
                leaveGroupName: selectedRow?.name ?? "",
              });
            }
          }
          closeLookup();
        }}
      />
    </>
  );
};
