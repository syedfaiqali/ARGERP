import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button } from "@mui/material";
import {
  AccountBalance as BankGroupIcon,
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { Modal } from "@/components/Modal";
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

interface EmployeeBankAccountRow {
  id: number | string;
  code: string;
  bankGroupId: number | "";
  bankGroupName: string;
  branchCode: string;
  branchName: string;
  accountNo: string;
  ibanNo: string;
  title: string;
  primaryAccount: boolean;
}

interface EmployeeBankAccountTabProps {
  employeeId: string | undefined;
  readOnly?: boolean;
}

const BANK_ACCOUNT_PERMISSION_SCREEN_NAME = "Bank Account";
const BANK_ACCOUNT_PERMISSION_PARENT_NAME = "Employee";

const createEmptyRow = (): EmployeeBankAccountRow => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  code: "",
  bankGroupId: "",
  bankGroupName: "",
  branchCode: "",
  branchName: "",
  accountNo: "",
  ibanNo: "",
  title: "",
  primaryAccount: false,
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
      name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? "",
    }))
    .filter((item) => item.id > 0);
};

const normalizeRow = (item: any): EmployeeBankAccountRow => ({
  id: item.id ?? item.Id,
  code: item.code ?? item.Code ?? "",
  bankGroupId: item.bankGroupId ?? item.BankGroupId ?? item.bankgrpid ?? "",
  bankGroupName:
    item.bankGroupName ??
    item.BankGroupName ??
    item.bankGroup ??
    item.BankGroup ??
    item.FMBankgrp?.stxt ??
    item.fMBankgrp?.stxt ??
    "",
  branchCode: item.branchCode ?? item.BranchCode ?? item.branchcode ?? "",
  branchName: item.branchName ?? item.BranchName ?? item.branchname ?? "",
  accountNo: item.accountNo ?? item.AccountNo ?? item.accountno ?? "",
  ibanNo: item.ibanNo ?? item.IbanNo ?? item.ibanno ?? "",
  title: item.title ?? item.Title ?? item.accountTitle ?? "",
  primaryAccount: Boolean(
    item.primaryAccount ??
      item.PrimaryAccount ??
      item.primaryacct ??
      item.PrimaryAcct ??
      false,
  ),
});

export const EmployeeBankAccountTab: React.FC<EmployeeBankAccountTabProps> = ({
  employeeId,
  readOnly = false,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeBankAccountRow[]>([]);
  const [bankGroups, setBankGroups] = useState<LookupOption[]>([]);
  const [selectedIds, setSelectedIds] = useState<any[]>([]);
  const [lookupRowId, setLookupRowId] = useState<number | string | null>(null);
  const [showBankGroupSetup, setShowBankGroupSetup] = useState(false);
  const [bankGroupForm, setBankGroupForm] = useState({
    code: "",
    bankGroup: "",
    description: "",
  });
  const [bankGroupSubmitted, setBankGroupSubmitted] = useState(false);
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({ open: false, messages: [], severity: "info" });
  const [loading, setLoading] = useState(false);
  const canDelete = true;
  const canCreate = true;
  const canUpdate = true;

  const loadBankGroups = useCallback(async () => {
    setLoading(true);
    try {
      const response: any = await GetService("/api/HumanResource/EREmpBank/v3/lookups/bank-groups");
      setBankGroups(normalizeLookup(response?.data ?? response));
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to load bank groups")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [translate]);

  useEffect(() => {
    void loadBankGroups();
  }, [loadBankGroups]);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `/api/HumanResource/EREmpBank/v3?employeeId=${employeeId}&pageSize=1000`,
      );
      const payload = response?.data ?? response ?? {};
      const items: any[] =
        payload?.items ??
        payload?.Items ??
        payload?.data ??
        payload?.Data ??
        (Array.isArray(payload) ? payload : []);

      setRows(items.map(normalizeRow));
      setSelectedIds([]);
    } catch {
      setLoading(false);
      setRows([]);
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load bank accounts")],
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
    patch: Partial<EmployeeBankAccountRow>,
  ) => {
    if (readOnly) return;

    setRows((prev) =>
      prev.map((row) => {
        if (row.id !== rowId) return row;

        const nextRow = { ...row, ...patch };
        if (patch.primaryAccount) {
          return nextRow;
        }
        return nextRow;
      }),
    );

    if (patch.primaryAccount) {
      setRows((prev) =>
        prev.map((row) =>
          row.id === rowId
            ? { ...row, primaryAccount: true }
            : { ...row, primaryAccount: false },
        ),
      );
    }
  };

  const handleAddRow = () => {
    if (readOnly || !canCreate) return;
    setRows((prev) => [...prev, createEmptyRow()]);
  };

  const handleDeleteSelected = async () => {
    if (selectedIds.length === 0) return;
    if (readOnly || !canDelete) return;

    try {
      setLoading(true);
      const selectedRows = rows.filter((row) => selectedIds.includes(row.id));

      for (const row of selectedRows) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/EREmpBank/v3/${row.id}`);
        }
      }

      setRows((prev) => prev.filter((row) => !selectedIds.includes(row.id)));
      setSelectedIds([]);
      setToaster({
        open: true,
        messages: [translate("Selected bank accounts deleted successfully")],
        severity: "success",
      });
    } catch {
      setLoading(false);
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected bank accounts")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSaveAll = async () => {
    if (readOnly || !canUpdate) return;

    if (!employeeId) {
      setToaster({
        open: true,
        messages: [translate("Employee context is missing")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.bankGroupId)) {
      setToaster({
        open: true,
        messages: [translate("Bank Group is required for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.branchName.trim())) {
      setToaster({
        open: true,
        messages: [translate("Branch Name is required for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.accountNo.trim())) {
      setToaster({
        open: true,
        messages: [translate("Account No is required for all rows")],
        severity: "error",
      });
      return;
    }

    if (rows.some((row) => !row.title.trim())) {
      setToaster({
        open: true,
        messages: [translate("Account Title is required for all rows")],
        severity: "error",
      });
      return;
    }

    try {
      setLoading(true);
      const orderedRows = [
        ...rows.filter((row) => !row.primaryAccount),
        ...rows.filter((row) => row.primaryAccount),
      ];

      for (const row of orderedRows) {
        const payload = {
          id: typeof row.id === "number" ? row.id : undefined,
          employeeId: Number(employeeId),
          code: row.code.trim(),
          bankGroupId: row.bankGroupId ? Number(row.bankGroupId) : null,
          branchCode: row.branchCode.trim(),
          branchName: row.branchName.trim(),
          accountNo: row.accountNo.trim(),
          ibanNo: row.ibanNo.trim(),
          title: row.title.trim(),
          primaryAccount: row.primaryAccount,
        };

        if (typeof row.id === "number") {
          await PutService("/api/HumanResource/EREmpBank/v3", payload);
        } else {
          await PostService("/api/HumanResource/EREmpBank/v3", payload);
        }
      }

      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Bank accounts saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save bank accounts"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const renderTextInput = (
    row: EmployeeBankAccountRow | undefined,
    field: keyof Pick<
      EmployeeBankAccountRow,
      "code" | "branchCode" | "branchName" | "accountNo" | "ibanNo" | "title"
    >,
    maxLength: number,
  ) =>
    row ? (
      <Box onClick={(event) => event.stopPropagation()}>
        <FormInput
          hideLabel
          size="small"
          value={row[field]}
          disabled={readOnly}
          debounceMs={field === "accountNo" ? 0 : undefined}
          onChange={(event) => {
            const value = field === "accountNo"
              ? event.target.value.replace(/\D/g, "").slice(0, maxLength)
              : event.target.value.slice(0, maxLength);
            updateRow(row.id, {
              [field]: value,
            });
          }}
          inputProps={
            field === "accountNo" ? { inputMode: "numeric", pattern: "[0-9]*" } : undefined
          }
          onKeyDown={(event) => {
            if (
              field === "accountNo" &&
              !event.ctrlKey &&
              !event.metaKey &&
              !event.altKey &&
              event.key.length === 1 &&
              !/^[0-9]$/.test(event.key)
            ) {
              event.preventDefault();
            }
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
        id: "bankGroupName",
        label: translate("Bank Group"),
        minWidth: 220,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          row ? (
            <LookupDropdown
              hideLabel
              value={row.bankGroupId}
              options={bankGroups}
              disabled={readOnly}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) =>
                updateRow(row.id, {
                  bankGroupId: option?.id ?? "",
                  bankGroupName: option?.name ?? "",
                })
              }
              placeholder={translate("Select Bank Group")}
              onSearchClick={() => setLookupRowId(row.id)}
            />
          ) : (
            ""
          ),
      },
      {
        id: "branchCode",
        label: translate("Branch Code"),
        minWidth: 150,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          renderTextInput(row, "branchCode", 10),
      },
      {
        id: "branchName",
        label: translate("Branch Name"),
        minWidth: 220,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          renderTextInput(row, "branchName", 100),
      },
      {
        id: "accountNo",
        label: translate("Account No"),
        minWidth: 180,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          renderTextInput(row, "accountNo", 30),
      },
      {
        id: "ibanNo",
        label: translate("IBAN No"),
        minWidth: 180,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          renderTextInput(row, "ibanNo", 30),
      },
      {
        id: "title",
        label: translate("Account Title"),
        minWidth: 220,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          renderTextInput(row, "title", 100),
      },
      {
        id: "primaryAccount",
        label: translate("Primary"),
        minWidth: 120,
        format: (_: any, row?: EmployeeBankAccountRow) =>
          row ? (
            <Box onClick={(event) => event.stopPropagation()}>
              <LmsCheckbox
                hideLabel
                checked={row.primaryAccount}
                disabled={readOnly}
                onChange={(checked) =>
                  updateRow(row.id, { primaryAccount: checked })
                }
              />
            </Box>
          ) : (
            ""
          ),
      },
    ],
    [bankGroups, translate],
  );

  const selectedValue =
    lookupRowId !== null
      ? rows.find((row) => row.id === lookupRowId)?.bankGroupId
      : undefined;

  const closeBankGroupSetup = () => {
    setShowBankGroupSetup(false);
    setBankGroupSubmitted(false);
    void loadBankGroups();
  };

  const resetBankGroupForm = () => {
    setBankGroupForm({ code: "", bankGroup: "", description: "" });
    setBankGroupSubmitted(false);
  };

  const openBankGroupSetup = () => {
    if (!canCreate) return;
    resetBankGroupForm();
    setShowBankGroupSetup(true);
  };

  const saveBankGroup = async (closeAfterSave = false) => {
    setBankGroupSubmitted(true);
    if (!bankGroupForm.bankGroup.trim()) {
      setToaster({
        open: true,
        messages: [translate("Bank Group cannot be blank")],
        severity: "error",
      });
      return;
    }

    try {
      setLoading(true);
      await PostService("/api/HumanResource/Reference/v3/banks", {
        code: bankGroupForm.code.trim(),
        name: bankGroupForm.bankGroup.trim(),
        description: bankGroupForm.description.trim(),
      });
      await loadBankGroups();
      setToaster({
        open: true,
        messages: [translate("Record saved successfully")],
        severity: "success",
      });
      resetBankGroupForm();
      if (closeAfterSave) {
        setShowBankGroupSetup(false);
      }
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
            error?.response?.data ||
            error?.message ||
            translate("Failed to save bank group"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Bank Account")}
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
        onAddClick={readOnly ? undefined : handleAddRow}
        showAddButton={!readOnly}
        readOnly={readOnly}
        extraToolbarItemsLeft={
          readOnly ? null : (
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
                startIcon={<BankGroupIcon />}
                onClick={openBankGroupSetup}
                disabled={!canCreate}
                sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2, ml: 1 }}
              >
                <T>Bank Group</T>
              </Button>
            </>
          )
        }
        extraToolbarItemsRight={
          readOnly ? null : <Button
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
        open={lookupRowId !== null}
        onClose={() => setLookupRowId(null)}
        title={translate("Bank Group")}
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={bankGroups}
        selectedValue={selectedValue}
        selectedField="id"
        onSelect={(selectedRow: any) => {
          if (lookupRowId !== null) {
            updateRow(lookupRowId, {
              bankGroupId: selectedRow?.id ?? "",
              bankGroupName: selectedRow?.name ?? "",
            });
          }
          setLookupRowId(null);
        }}
      />

      <Modal
        open={showBankGroupSetup}
        onClose={closeBankGroupSetup}
        title={translate("Bank Group")}
        maxWidth="md"
        onReset={resetBankGroupForm}
        resetLabel={translate("New")}
        onSave={() => void saveBankGroup(false)}
        onSaveAndClose={() => void saveBankGroup(true)}
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
            }}
          >
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                fontWeight: 800,
                fontSize: "1rem",
                color: "var(--lms-text-main)",
              }}
            >
              <T>Basic Information</T>
            </Box>
          </Box>
          <Box sx={{ maxWidth: 560, p: 2.5 }}>
            <FormInput
              label={translate("Code")}
              value={bankGroupForm.code}
              disabled
              size="small"
              sx={{ mb: 1.5, maxWidth: 190 }}
            />
            <FormInput
              label={translate("Bank Group*")}
              required
              debounceMs={0}
              placeholder={translate("Enter Bank Group")}
              value={bankGroupForm.bankGroup}
              onChange={(event) =>
                setBankGroupForm((prev) => ({
                  ...prev,
                  bankGroup: event.target.value.slice(0, 150),
                }))
              }
              error={bankGroupSubmitted && !bankGroupForm.bankGroup.trim()}
              helperText={
                bankGroupSubmitted && !bankGroupForm.bankGroup.trim()
                  ? translate("Bank Group cannot be blank")
                  : ""
              }
              sx={{ mb: 1.5 }}
            />
            <FormInput
              label={translate("Description")}
              debounceMs={0}
              placeholder={translate("Enter Description")}
              value={bankGroupForm.description}
              multiline
              rows={3}
              onChange={(event) =>
                setBankGroupForm((prev) => ({
                  ...prev,
                  description: event.target.value.slice(0, 500),
                }))
              }
            />
          </Box>
        </Box>
      </Modal>

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />
    </>
  );
};
