import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button, TextField } from "@mui/material";
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
import { useHiddenMenuPermissions } from "@/hooks/useHiddenMenuPermissions";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

const FAR_FUTURE = dayjs("9998-12-31");

interface PositionAssignmentRow {
  id: number | string;
  departmentId: number | "";
  department?: string;
  designationId: number | "";
  designation?: string;
  fromDate: string;
  tillDate: string;
  responsibility: string;
}

interface PositionAssignmentTabProps {
  employeeId: string | undefined;
}

const POSITION_ASSIGNMENT_PERMISSION_SCREEN_NAME = "Position Assign";
const POSITION_ASSIGNMENT_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): PositionAssignmentRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  departmentId: "",
  department: "",
  designationId: "",
  designation: "",
  fromDate: dayjs().toISOString(),
  tillDate: FAR_FUTURE.toISOString(),
  responsibility: "",
});

export const PositionAssignmentTab: React.FC<PositionAssignmentTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const { screenPermissionsLoaded, hasScreenActionRightForParent } = useHiddenMenuPermissions();
  const [rows, setRows] = useState<PositionAssignmentRow[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
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
    type: "department" | "designation" | null;
    rowId: number | string | null;
  }>({
    open: false,
    type: null,
    rowId: null,
  });
  const [loading, setLoading] = useState(false);
  const canCreate = screenPermissionsLoaded && hasScreenActionRightForParent(
    POSITION_ASSIGNMENT_PERMISSION_SCREEN_NAME,
    POSITION_ASSIGNMENT_PERMISSION_PARENT_NAME,
    "create",
  );
  const canUpdate = screenPermissionsLoaded && hasScreenActionRightForParent(
    POSITION_ASSIGNMENT_PERMISSION_SCREEN_NAME,
    POSITION_ASSIGNMENT_PERMISSION_PARENT_NAME,
    "update",
  );
  const canDelete = screenPermissionsLoaded && hasScreenActionRightForParent(
    POSITION_ASSIGNMENT_PERMISSION_SCREEN_NAME,
    POSITION_ASSIGNMENT_PERMISSION_PARENT_NAME,
    "delete",
  );

  useEffect(() => {
    setLoading(true);
    void Promise.all([
      GetService("/api/HumanResource/Lookups/v3/departments"),
      GetService("/api/HumanResource/Lookups/v3/designations"),
    ])
      .then(([departmentResponse, designationResponse]) => {
        const departmentPayload =
          departmentResponse?.data ?? departmentResponse ?? {};
        const designationPayload =
          designationResponse?.data ?? designationResponse ?? {};

        const departmentItems: any[] =
          departmentPayload?.items ??
          departmentPayload?.Items ??
          departmentPayload?.data ??
          departmentPayload?.Data ??
          (Array.isArray(departmentPayload) ? departmentPayload : []);

        const designationItems: any[] =
          designationPayload?.items ??
          designationPayload?.Items ??
          designationPayload?.data ??
          designationPayload?.Data ??
          (Array.isArray(designationPayload) ? designationPayload : []);

        setDepartments(
          departmentItems.map((item: any) => ({
            id: Number(item.id ?? item.departmentId ?? item.depid ?? 0),
            name:
              item.departmentName ?? item.department ?? item.depstxt ?? item.name ?? "",
          })),
        );

        setDesignations(
          designationItems.map((item: any) => ({
            id: Number(item.id ?? item.designationId ?? 0),
            name: item.designationName ?? item.designation ?? item.stxt ?? item.name ?? "",
          })),
        );
      })
      .catch(() => {
        setLoading(false);
        setToaster({
          open: true,
          messages: [translate("Failed to load Department/Designation lookups")],
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
        `/api/HumanResource/EREmpPosition/v3?employeeId=${employeeId}&pageSize=1000`,
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
          departmentId: item.departmentId ?? "",
          department: item.departmentName ?? "",
          designationId: item.designationId ?? "",
          designation: item.designationName ?? "",
          fromDate: item.fromDate ?? "",
          tillDate: item.tillDate ?? FAR_FUTURE.toISOString(),
          responsibility: item.responsibility ?? item.Responsibility ?? "",
        })),
      );
      setSelectedIds([]);
    } catch {
      setLoading(false);
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load position assignments")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const openLookup = (rowId: number | string, type: "department" | "designation") => {
    setLookupState({
      open: true,
      type,
      rowId,
    });
  };

  const closeLookup = () => {
    setLookupState({
      open: false,
      type: null,
      rowId: null,
    });
  };

  const updateRow = (
    rowId: number | string,
    patch: Partial<PositionAssignmentRow>,
  ) => {
    setRows((prev) =>
      prev.map((row) => (row.id === rowId ? { ...row, ...patch } : row)),
    );
  };

  const handleAddRow = () => {
    if (!canCreate) return;

    const nextRow = createEmptyRow();
    const previousTillDate = dayjs(nextRow.fromDate).subtract(1, "day").toISOString();
    setRows((prev) => [
      ...prev.map((row) =>
        row.tillDate && dayjs(row.tillDate).year() === 9998
          ? { ...row, tillDate: previousTillDate }
          : row,
      ),
      nextRow,
    ]);
  };

  const handleDeleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      const selectedRows = rows.filter((row) => selectedIds.includes(row.id));

      for (const row of selectedRows) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/EREmpPosition/v3/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected position assignments deleted successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            translate("Failed to delete selected position assignments"),
        ],
        severity: "error",
      });
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

    if (rows.some((row) => !row.departmentId)) {
      setToaster({
        open: true,
        messages: [translate("Please select Department for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.designationId)) {
      setToaster({
        open: true,
        messages: [translate("Please select Designation for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.fromDate)) {
      setToaster({
        open: true,
        messages: [translate("Please set From Date for all rows")],
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
          departmentId: Number(row.departmentId),
          designationId: Number(row.designationId),
          fromDate: row.fromDate,
          tillDate: row.tillDate,
          responsibility: row.responsibility?.trim().slice(0, 1500) || null,
        };

        if (typeof row.id === "number") {
          await PutService("/api/HumanResource/EREmpPosition/v3", payload);
        } else {
          await PostService("/api/HumanResource/EREmpPosition/v3", payload);
        }
      }

      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Position assignments saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setLoading(false);
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
          error?.message ||
          translate("Failed to save position assignments"),
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
        minWidth: 80,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "department",
        label: translate("Department"),
        minWidth: 240,
        format: (_: any, row?: PositionAssignmentRow) =>
          row ? (
            <Box>
              <LookupDropdown
                hideLabel
                value={row.departmentId}
                options={departments}
                getOptionLabel={(option) => option.name || ""}
                onSelect={(option) =>
                  updateRow(row.id, {
                    departmentId: option?.id ?? "",
                    department: option?.name ?? "",
                  })
                }
                placeholder={translate("Select Department")}
                onSearchClick={() => openLookup(row.id, "department")}
              />
            </Box>
          ) : (
            ""
          ),
      },
      {
        id: "designation",
        label: translate("Designation"),
        minWidth: 240,
        format: (_: any, row?: PositionAssignmentRow) =>
          row ? (
            <Box>
              <LookupDropdown
                hideLabel
                value={row.designationId}
                options={designations}
                getOptionLabel={(option) => option.name || ""}
                onSelect={(option) =>
                  updateRow(row.id, {
                    designationId: option?.id ?? "",
                    designation: option?.name ?? "",
                  })
                }
                placeholder={translate("Select Designation")}
                onSearchClick={() => openLookup(row.id, "designation")}
              />
            </Box>
          ) : (
            ""
          ),
      },
      {
        id: "fromDate",
        label: translate("From Date"),
        minWidth: 180,
        format: (_: any, row?: PositionAssignmentRow) =>
          row ? (
            <Box
              onClick={(e) => e.stopPropagation()}
              onMouseDown={(e) => e.stopPropagation()}
            >
              <FormDatePicker
                hideLabel
                value={row.fromDate ? dayjs(row.fromDate) : null}
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    fromDate: value ? value.toISOString() : "",
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
        id: "tillDate",
        label: translate("Till Date"),
        minWidth: 180,
        format: (_: any, row?: PositionAssignmentRow) =>
          row ? (
            <Box
              onClick={(e) => e.stopPropagation()}
              onMouseDown={(e) => e.stopPropagation()}
            >
              <FormDatePicker
                hideLabel
                value={row.tillDate ? dayjs(row.tillDate) : null}
                disabled
                onChange={(value: Dayjs | null) =>
                  updateRow(row.id, {
                    tillDate: value ? value.toISOString() : "",
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
        id: "responsibility",
        label: translate("Job Responsibility"),
        minWidth: 280,
        format: (_: any, row?: PositionAssignmentRow) =>
          row ? (
            <TextField
              multiline
              minRows={2}
              size="small"
              value={row.responsibility || ""}
              onClick={(e) => e.stopPropagation()}
              onChange={(event) =>
                updateRow(row.id, { responsibility: event.target.value })
              }
              placeholder={translate("Enter Responsibility")}
              inputProps={{ maxLength: 1500 }}
              sx={{ width: "100%" }}
            />
          ) : (
            ""
          ),
      },
    ],
    [departments, designations, translate],
  );

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Position Assignment")}
        columns={columns}
        rows={rows}
        permissionScreenName={POSITION_ASSIGNMENT_PERMISSION_SCREEN_NAME}
        permissionParentName={POSITION_ASSIGNMENT_PERMISSION_PARENT_NAME}
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
          lookupState.type === "department"
            ? translate("Department")
            : translate("Designation")
        }
        columns={[
          { id: "id", label: translate("Id"), minWidth: 100 },
          {
            id: "name",
            label:
              lookupState.type === "department"
                ? translate("Department")
                : translate("Designation"),
            minWidth: 220,
          },
        ]}
        data={lookupState.type === "department" ? departments : designations}
        selectedValue={
          lookupState.rowId !== null
            ? lookupState.type === "department"
              ? rows.find((row) => row.id === lookupState.rowId)?.departmentId
              : rows.find((row) => row.id === lookupState.rowId)?.designationId
            : undefined
        }
        selectedField="id"
        onSelect={(selectedRow: any) => {
          if (lookupState.rowId !== null) {
            if (lookupState.type === "department") {
              updateRow(lookupState.rowId, {
                departmentId: selectedRow?.id ?? "",
                department: selectedRow?.name ?? "",
              });
            } else {
              updateRow(lookupState.rowId, {
                designationId: selectedRow?.id ?? "",
                designation: selectedRow?.name ?? "",
              });
            }
          }
          closeLookup();
        }}
      />
    </>
  );
};
