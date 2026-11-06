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
import { LmsCheckbox } from "@/components/LmsCheckbox";
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
}

interface EmployeeContactInformationRow {
  id: number | string;
  code: string;
  employeeId?: number;
  relationTypeId: number | "";
  relationType: string;
  firstName: string;
  lastName: string;
  address: string;
  phoneNo: string;
  mobileNo: string;
  email: string;
  emergency: boolean;
  nextOfKin: boolean;
}

interface EmployeeContactInformationTabProps {
  employeeId: string | undefined;
  onSaved?: () => void;
}

type LookupKind = "relationType";

const CONTACT_PERMISSION_SCREEN_NAME = "Relation";

const createEmptyRow = (): EmployeeContactInformationRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  code: "",
  relationTypeId: "",
  relationType: "",
  firstName: "",
  lastName: "",
  address: "",
  phoneNo: "",
  mobileNo: "",
  email: "",
  emergency: false,
  nextOfKin: false,
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

const getRelationTypesFromPayload = (payload: any): LookupOption[] => {
  const data = payload?.data ?? payload;
  const directItems = normalizeLookup(data);
  if (directItems.length > 0) return directItems;

  return normalizeLookup(data?.relationTypes ?? data?.RelationTypes ?? []);
};

const onlyDigits = (value: string) => value.replace(/\D/g, "");

const formatContactNo = (value: string) => {
  const digits = onlyDigits(value).slice(0, 11);
  if (!digits) return "";

  const firstPart = digits.slice(0, 4).padEnd(4, "_");
  const secondPart = digits.slice(4, 11).padEnd(7, "_");
  return `${firstPart}-${secondPart}`;
};

const CONTACT_DIGIT_LENGTH = 11;

const normalizeRow = (item: any): EmployeeContactInformationRow => ({
  id: item.id ?? item.Id,
  code: item.code ?? item.Code ?? "",
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  relationTypeId: item.relationTypeId ?? item.RelationTypeId ?? item.relationtypeid ?? "",
  relationType: item.relationType ?? item.RelationType ?? item.SMRelationtype?.stxt ?? "",
  firstName: item.firstName ?? item.FirstName ?? item.firstname ?? "",
  lastName: item.lastName ?? item.LastName ?? item.lastname ?? "",
  address: item.address ?? item.Address ?? "",
  phoneNo: formatContactNo(item.phoneNo ?? item.PhoneNo ?? item.phoneno ?? ""),
  mobileNo: formatContactNo(item.mobileNo ?? item.MobileNo ?? item.mobileno ?? ""),
  email: item.email ?? item.Email ?? "",
  emergency: Boolean(item.emergency ?? item.Emergency),
  nextOfKin: Boolean(item.nextOfKin ?? item.NextOfKin ?? item.nextofkin),
});

export const EmployeeContactInformationTab: React.FC<EmployeeContactInformationTabProps> = ({
  employeeId,
  onSaved,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeContactInformationRow[]>([]);
  const [relationTypes, setRelationTypes] = useState<LookupOption[]>([]);
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
  const canDelete = true;

  useEffect(() => {
    const loadRelationTypes = async () => {
      const endpoints = [
        "/api/HumanResource/EREmpRelation/v3/lookups/relation-types",
        "/api/HumanResource/Relationtype/v3",
        "/api/HumanResource/Relationtype/v3/lookups",
      ];

      for (const endpoint of endpoints) {
        try {
          const response = await GetService(endpoint);
          const items = getRelationTypesFromPayload(response);
          if (items.length > 0) return items;
        } catch {
          // Try the next known relation-type source.
        }
      }

      return [];
    };

    setLoading(true);
    void loadRelationTypes()
      .then((items) => {
        setRelationTypes(items);
      })
      .catch(() => {
        setToaster({
          open: true,
          messages: [translate("Failed to load contact lookups")],
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
        `/api/HumanResource/EREmpRelation/v3?employeeId=${employeeId}&pageSize=1000`,
      );
      setRows(itemsFrom(response).map(normalizeRow));
      setSelectedIds([]);
    } catch {
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load contact information")],
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
    patch: Partial<EmployeeContactInformationRow>,
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
    setRows((prev) => [...prev, createEmptyRow()]);
  };

  const handleDeleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      setLoading(true);
      for (const row of rows.filter((item) => selectedIds.includes(item.id))) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/EREmpRelation/v3/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      onSaved?.();
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected contact rows deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected contact rows")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSaveAll = async () => {
    if (!employeeId) {
      setToaster({
        open: true,
        messages: [translate("Employee context is missing")],
        severity: "error",
      });
      return;
    }

    const requiredError =
      rows.some((row) => !row.relationTypeId) ? "Please select Relation Type for all rows" :
      rows.some((row) => !row.firstName.trim()) ? "First Name is required for all rows" :
      rows.some((row) => !row.mobileNo.trim()) ? "Mobile No is required for all rows" :
      rows.some((row) => onlyDigits(row.mobileNo).length !== CONTACT_DIGIT_LENGTH) ? "Mobile No is invalid" :
      rows.some((row) => row.phoneNo.trim() && onlyDigits(row.phoneNo).length !== CONTACT_DIGIT_LENGTH) ? "Phone No is invalid" :
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
          code: row.code.trim(),
          employeeId: Number(employeeId),
          relationTypeId: Number(row.relationTypeId),
          genderId: null,
          firstName: row.firstName.trim(),
          lastName: row.lastName.trim(),
          address: row.address.trim(),
          phoneNo: row.phoneNo.trim(),
          mobileNo: row.mobileNo.trim(),
          email: row.email.trim(),
          emergency: row.emergency,
          nextOfKin: row.nextOfKin,
        };

        if (typeof row.id === "number") {
          await PutService("/api/HumanResource/EREmpRelation/v3", payload);
        } else {
          await PostService("/api/HumanResource/EREmpRelation/v3", payload);
        }
      }

      await fetchRows();
      onSaved?.();
      setToaster({
        open: true,
        messages: [translate("Contact information saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save contact information"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const renderTextInput = (
    row: EmployeeContactInformationRow | undefined,
    field: keyof Pick<EmployeeContactInformationRow, "code" | "firstName" | "lastName" | "address" | "phoneNo" | "mobileNo" | "email">,
    maxLength: number,
    formatter?: (value: string) => string,
  ) =>
    row ? (
      <Box onClick={(event) => event.stopPropagation()}>
        <FormInput
          hideLabel
          size="small"
          debounceMs={formatter ? 0 : undefined}
          inputMode={formatter ? "tel" : undefined}
          value={row[field]}
          onChange={(event) =>
            updateRow(row.id, {
              [field]: (formatter
                ? formatter(event.target.value)
                : event.target.value
              ).slice(0, maxLength),
            })
          }
        />
      </Box>
    ) : (
      ""
    );

  const renderCheckbox = (
    row: EmployeeContactInformationRow | undefined,
    field: keyof Pick<EmployeeContactInformationRow, "emergency" | "nextOfKin">,
  ) =>
    row ? (
      <Box onClick={(event) => event.stopPropagation()}>
        <LmsCheckbox
          hideLabel
          checked={row[field]}
          onChange={(checked) => updateRow(row.id, { [field]: checked })}
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
        id: "relationType",
        label: translate("Relation Type"),
        minWidth: 190,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.relationTypeId}
              options={relationTypes}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => openLookup(row.id, "relationType")}
              onSelect={(option) =>
                updateRow(row.id, {
                  relationTypeId: option?.id ?? "",
                  relationType: option?.name ?? "",
                })
              }
              placeholder={translate("Select Relation Type")}
            />
          ) : (
            ""
          ),
      },
      {
        id: "firstName",
        label: translate("First Name"),
        minWidth: 170,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "firstName", 50),
      },
      {
        id: "lastName",
        label: translate("Last Name"),
        minWidth: 170,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "lastName", 50),
      },
      {
        id: "mobileNo",
        label: translate("Mobile No"),
        minWidth: 150,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "mobileNo", 12, formatContactNo),
      },
      {
        id: "phoneNo",
        label: translate("Phone No"),
        minWidth: 150,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "phoneNo", 12, formatContactNo),
      },
      {
        id: "email",
        label: translate("Email"),
        minWidth: 210,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "email", 100),
      },
      {
        id: "address",
        label: translate("Address"),
        minWidth: 260,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderTextInput(row, "address", 500),
      },
      {
        id: "emergency",
        label: translate("Emergency"),
        minWidth: 120,
        align: "center" as const,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderCheckbox(row, "emergency"),
      },
      {
        id: "nextOfKin",
        label: translate("Next of Kin"),
        minWidth: 130,
        align: "center" as const,
        format: (_: any, row?: EmployeeContactInformationRow) =>
          renderCheckbox(row, "nextOfKin"),
      },
    ],
    [relationTypes, translate],
  );

  const lookupRows = useMemo(() => {
    if (lookupState.kind === "relationType") return relationTypes;
    return [];
  }, [lookupState.kind, relationTypes]);

  const selectedValue = useMemo(() => {
    const row = rows.find((item) => item.id === lookupState.rowId);
    if (!row || !lookupState.kind) return undefined;
    if (lookupState.kind === "relationType") return row.relationTypeId;
    return undefined;
  }, [lookupState, rows]);

  const lookupTitle = translate("Relation Type");

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Contact Information")}
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
            disabled={rows.length === 0}
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

          if (lookupState.kind === "relationType") {
            updateRow(lookupState.rowId, {
              relationTypeId: selectedRow?.id ?? "",
              relationType: selectedRow?.name ?? "",
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
