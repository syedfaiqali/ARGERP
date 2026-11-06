import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from "react";
import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  TextField,
  Typography,
} from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import dayjs from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { PayrollAmountInput } from "./PayrollAmountInput";
import { calculateAllowanceAmount, formatPayrollAmount, serializeAllowanceAmount } from "./employeeAllowanceCalculation";
import { LookupDropdown } from "@/components/LookupDropdown";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService, PostService } from "@/constants/globalItems";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";

const FAR_FUTURE = dayjs("9998-12-31");

interface LookupOption {
  id: number;
  code?: string;
  name: string;
}

interface PayrollLine {
  id: number | string;
  typeId: number | "";
  typeName: string;
  startDate: string;
  endDate: string;
  value: number | "";
  toValue?: number | "";
  isPercentage?: boolean;
  isAllowed?: boolean;
}

interface PayrollState {
  id?: number;
  loanAllowed: boolean;
  advanceAllowed: boolean;
  payFullSalary: boolean;
  overtimeAllowed: boolean;
  baseSalaries: PayrollLine[];
  allowances: PayrollLine[];
  deductions: PayrollLine[];
  loanTypeIds: number[];
}

export interface EmployeePayrollTabHandle {
  hasPayrollSetup: () => boolean;
  validateConfiguredPayroll: () => string | null;
  save: (ownerId?: number) => Promise<void>;
}

interface EmployeePayrollTabProps {
  employeeId: string | undefined;
  designationId?: number | null;
  /** Changes only when a user explicitly selects a different designation. */
  applyDesignationTemplateKey?: number;
  payrollFor?: "employee" | "designation";
  readOnly?: boolean;
  hideSaveButton?: boolean;
}

interface DesignationSalaryRange {
  minimumSalary: number;
  maximumSalary: number;
}

const SalarySectionHeading: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1.75 }}>
    <Box
      sx={{
        width: 4,
        height: 18,
        bgcolor: "var(--lms-primary)",
        borderRadius: 1,
        flexShrink: 0,
      }}
    />
    <Typography
      sx={{
        fontSize: "0.95rem",
        fontWeight: 800,
        color: "var(--lms-text-main)",
      }}
    >
      {children}
    </Typography>
  </Box>
);

type LineSection = "baseSalaries" | "allowances" | "deductions";
type LookupType = "baseSalary" | "allowance" | "deduction";

const emptyPayroll = (): PayrollState => ({
  loanAllowed: false,
  advanceAllowed: false,
  payFullSalary: false,
  overtimeAllowed: false,
  baseSalaries: [],
  allowances: [],
  deductions: [],
  loanTypeIds: [],
});

const createLine = (): PayrollLine => ({
  id: `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  typeId: "",
  typeName: "",
  startDate: dayjs().toISOString(),
  endDate: FAR_FUTURE.toISOString(),
  value: "",
  toValue: "",
  isPercentage: false,
});

const createMasterLine = (type: LookupOption, section: "allowances" | "deductions"): PayrollLine => ({
  ...createLine(),
  id: `master-${section}-${type.id}`,
  typeId: type.id,
  typeName: type.name,
  isAllowed: false,
});

const mergeDesignationMasterTypes = (
  current: PayrollLine[],
  types: LookupOption[],
  section: "allowances" | "deductions",
) => {
  const sortAllowedFirst = (lines: PayrollLine[]) => [...lines].sort(
    (left, right) => Number(Boolean(right.isAllowed)) - Number(Boolean(left.isAllowed)) ||
      left.typeName.localeCompare(right.typeName),
  );
  if (types.length === 0) return sortAllowedFirst(current);

  return types.map((type) => {
  const saved = current.find((line) => Number(line.typeId) === type.id);
  return saved
    ? { ...saved, typeId: type.id, typeName: type.name, isAllowed: Boolean(saved.isAllowed) }
    : createMasterLine(type, section);
  }).sort((left, right) => Number(Boolean(right.isAllowed)) - Number(Boolean(left.isAllowed)) ||
    left.typeName.localeCompare(right.typeName));
};

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
      name: item.name ?? item.Name ?? item.loanType ?? item.LoanType ?? item.stxt ?? item.Stxt ?? "",
    }))
    .filter((item) => item.id > 0);
};

const normalizeLine = (item: any): PayrollLine => ({
  id: item.id ?? item.Id ?? `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
  typeId: item.typeId ?? item.TypeId ?? "",
  typeName: item.typeName ?? item.TypeName ?? "",
  startDate: item.startDate ?? item.StartDate ?? "",
  endDate: item.endDate ?? item.EndDate ?? FAR_FUTURE.toISOString(),
  value: item.value ?? item.Value ?? 0,
  toValue: item.toValue ?? item.ToValue ?? item.value ?? item.Value ?? 0,
  isPercentage: Boolean(item.isPercentage ?? item.IsPercentage ?? false),
});

const ensureSingleBaseSalary = (lines: PayrollLine[]) => {
  const activeLine =
    lines.find((line) => line.endDate && dayjs(line.endDate).year() === 9998) ??
    lines[0];
  return [activeLine ?? createLine()];
};

export const EmployeePayrollTab = forwardRef<EmployeePayrollTabHandle, EmployeePayrollTabProps>(({
  employeeId,
  designationId,
  applyDesignationTemplateKey,
  payrollFor = "employee",
  readOnly = false,
  hideSaveButton = false,
}, ref) => {
  const { translate } = useTranslation();
  const [data, setData] = useState<PayrollState>(emptyPayroll);
  const [selectedIds, setSelectedIds] = useState<Record<LineSection, any[]>>({
    baseSalaries: [],
    allowances: [],
    deductions: [],
  });
  const [baseSalaryTypes, setBaseSalaryTypes] = useState<LookupOption[]>([]);
  const [allowanceTypes, setAllowanceTypes] = useState<LookupOption[]>([]);
  const [deductionTypes, setDeductionTypes] = useState<LookupOption[]>([]);  const [designationSalaryRange, setDesignationSalaryRange] = useState<DesignationSalaryRange | null>(null);
  const [lookupState, setLookupState] = useState<{
    open: boolean;
    type: LookupType | null;
    section: LineSection | null;
    rowId: number | string | null;
  }>({ open: false, type: null, section: null, rowId: null });
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({ open: false, messages: [], severity: "info" });
  const [loading, setLoading] = useState(false);
  const [salaryDetailTab, setSalaryDetailTab] = useState(0);
  const payrollLoadVersion = useRef(0);
  const appliedTemplateKey = useRef<number | undefined>(undefined);
  // Payroll is an Additional Information section and must not be locked by UT rights.
  const canCreate = true;
  const canUpdate = true;
  const canDelete = true;
  const isDesignationPayroll = payrollFor === "designation";
  // An employee with a designation uses the same master choices as the
  // designation. This keeps all master records visible and puts allowed ones first.
  const usesDesignationTemplate = isDesignationPayroll || Number(designationId || 0) > 0;
  const payrollApi = isDesignationPayroll
    ? "/api/HumanResource/DesignationPayroll/v3"
    : "/api/HumanResource/EREmppayroll/v3";
  const ownerQueryName = isDesignationPayroll ? "designationId" : "employeeId";
  const configuredOwnerId = isDesignationPayroll
    ? Number(designationId || 0)
    : Number(employeeId || 0);
  const numberValue = (value: any) => {
    const parsed = Number(value ?? 0);
    return Number.isFinite(parsed) ? parsed : 0;
  };

  const salarySummary = useMemo(() => {
    const baseSalary = data.baseSalaries.reduce(
      (sum, row) => sum + numberValue(row.value),
      0,
    );
    const maximumBaseSalary = data.baseSalaries.reduce(
      (sum, row) => sum + numberValue(
        isDesignationPayroll && row.toValue !== "" && row.toValue !== undefined
          ? row.toValue
          : row.value,
      ),
      0,
    );
    const allowances = data.allowances.filter((row) => !usesDesignationTemplate || row.isAllowed).reduce(
      (sum, row) => sum + calculateAllowanceAmount(row, baseSalary),
      0,
    );
    const deductions = data.deductions.filter((row) => !usesDesignationTemplate || row.isAllowed).reduce(
      (sum, row) => sum + numberValue(row.value),
      0,
    );

    return {
      baseSalary,
      maximumBaseSalary,
      allowances,
      deductions,
      grossSalary: baseSalary + allowances - deductions,
      netSalary: baseSalary + allowances - deductions,
    };
  }, [data.allowances, data.baseSalaries, data.deductions, isDesignationPayroll, usesDesignationTemplate]);

  const formatAmount = (value: number) =>
    formatPayrollAmount(value);

  useEffect(() => {
    const selectedDesignationId = Number(designationId || 0);
    if (isDesignationPayroll || !Number.isInteger(selectedDesignationId) || selectedDesignationId <= 0) {
      setDesignationSalaryRange(null);
      return;
    }

    setDesignationSalaryRange(null);
    let cancelled = false;
    void GetService(`/api/HumanResource/DesignationPayroll/v3?designationId=${selectedDesignationId}`)
      .then((response: any) => {
        const payload = response?.data ?? response ?? {};
        const baseSalary = (payload.baseSalaries ?? payload.BaseSalaries ?? [])[0];
        const minimumSalary = Number(baseSalary?.value ?? baseSalary?.Value);
        const maximumSalary = Number(baseSalary?.toValue ?? baseSalary?.ToValue);

        if (cancelled) return;
        setDesignationSalaryRange(
          baseSalary && Number.isFinite(minimumSalary) && Number.isFinite(maximumSalary)
            ? {
                minimumSalary,
                maximumSalary,
              }
            : null,
        );
      })
      .catch(() => {
        if (!cancelled) setDesignationSalaryRange(null);
      });

    return () => {
      cancelled = true;
    };
  }, [designationId, isDesignationPayroll]);

  useEffect(() => {
    setLoading(true);
    void Promise.all([
      GetService("/api/HumanResource/EREmppayroll/v3/lookups/earning-types?baseSalary=true"),
      GetService("/api/HumanResource/EREmppayroll/v3/lookups/earning-types?baseSalary=false"),
      GetService("/api/HumanResource/EREmppayroll/v3/lookups/deduction-types"),
    ])
      .then(([baseResponse, allowanceResponse, deductionResponse]) => {
        setBaseSalaryTypes(normalizeLookup(baseResponse?.data ?? baseResponse));
        setAllowanceTypes(normalizeLookup(allowanceResponse?.data ?? allowanceResponse));
        setDeductionTypes(normalizeLookup(deductionResponse?.data ?? deductionResponse));
      })
      .catch(() => {
        setLoading(false);
        setToaster({
          open: true,
          messages: [translate("Failed to load payroll lookups")],
          severity: "error",
        });
      })
      .finally(() => {
        setLoading(false);
      });
  }, [translate]);

  const applyDesignationTemplate = useCallback(async () => {
    const selectedDesignationId = Number(designationId || 0);
    if (isDesignationPayroll || !selectedDesignationId) return;

    const requestVersion = ++payrollLoadVersion.current;
    try {
      const response: any = await GetService(
        `/api/HumanResource/DesignationPayroll/v3?designationId=${selectedDesignationId}`,
      );
      if (requestVersion !== payrollLoadVersion.current) return;

      const payload = response?.data ?? response ?? {};
      const allowances = (payload.allowances ?? payload.Allowances ?? []).map((line: any) => ({
        ...normalizeLine(line),
        isAllowed: true,
      }));
      const deductions = (payload.deductions ?? payload.Deductions ?? []).map((line: any) => ({
        ...normalizeLine(line),
        isAllowed: true,
      }));

      setData({
        loanAllowed: Boolean(payload.loanAllowed ?? payload.LoanAllowed),
        advanceAllowed: Boolean(payload.advanceAllowed ?? payload.AdvanceAllowed),
        payFullSalary: Boolean(payload.payFullSalary ?? payload.PayFullSalary),
        overtimeAllowed: Boolean(payload.overtimeAllowed ?? payload.OvertimeAllowed),
        baseSalaries: ensureSingleBaseSalary(
          (payload.baseSalaries ?? payload.BaseSalaries ?? []).map(normalizeLine),
        ).map((line) => isDesignationPayroll ? line : { ...line, value: "", toValue: "" }),
        allowances: mergeDesignationMasterTypes(allowances, allowanceTypes, "allowances"),
        deductions: mergeDesignationMasterTypes(deductions, deductionTypes, "deductions"),
        loanTypeIds: (payload.loanTypeIds ?? payload.LoanTypeIds ?? [])
          .map((id: any) => Number(id))
          .filter((id: number) => Number.isInteger(id) && id > 0),
      });
      setSelectedIds({ baseSalaries: [], allowances: [], deductions: [] });
    } catch {
      if (requestVersion === payrollLoadVersion.current) {
        setToaster({
          open: true,
          messages: [translate("Failed to load designation salary")],
          severity: "error",
        });
      }
    }
  }, [allowanceTypes, deductionTypes, designationId, isDesignationPayroll, translate]);

  const fetchPayroll = useCallback(async (ownerId?: number) => {
    const payrollOwnerId = Number(ownerId || configuredOwnerId);
    if (!payrollOwnerId) {
      setData(emptyPayroll());
      setSelectedIds({ baseSalaries: [], allowances: [], deductions: [] });
      return;
    }

    try {
      const requestVersion = ++payrollLoadVersion.current;
      setLoading(true);
      const response: any = await GetService(
        `${payrollApi}?${ownerQueryName}=${payrollOwnerId}`,
      );
      const payload = response?.data ?? response ?? {};
      if (requestVersion !== payrollLoadVersion.current) return;
      const payrollExists = Boolean(payload.id ?? payload.Id);

      const allowances = (payload.allowances ?? payload.Allowances ?? []).map((line: any) => ({
        ...normalizeLine(line),
        isAllowed: usesDesignationTemplate ? true : undefined,
      }));
      const deductions = (payload.deductions ?? payload.Deductions ?? []).map((line: any) => ({
        ...normalizeLine(line),
        isAllowed: usesDesignationTemplate ? true : undefined,
      }));

      setData({
        id: payload.id ?? payload.Id,
        loanAllowed: Boolean(payload.loanAllowed ?? payload.LoanAllowed),
        advanceAllowed: Boolean(payload.advanceAllowed ?? payload.AdvanceAllowed),
        payFullSalary: Boolean(payload.payFullSalary ?? payload.PayFullSalary),
        overtimeAllowed: Boolean(payload.overtimeAllowed ?? payload.OvertimeAllowed),
        baseSalaries: ensureSingleBaseSalary(
          (payload.baseSalaries ?? payload.BaseSalaries ?? []).map(normalizeLine),
        ),
        allowances: usesDesignationTemplate
          ? mergeDesignationMasterTypes(allowances, allowanceTypes, "allowances")
          : allowances,
        deductions: usesDesignationTemplate
          ? mergeDesignationMasterTypes(deductions, deductionTypes, "deductions")
          : deductions,
        loanTypeIds: (payload.loanTypeIds ?? payload.LoanTypeIds ?? [])
          .map((id: any) => Number(id))
          .filter((id: number) => Number.isInteger(id) && id > 0),
      });
      setSelectedIds({ baseSalaries: [], allowances: [], deductions: [] });
      if (!isDesignationPayroll && !payrollExists && Number(designationId || 0) > 0) {
        void applyDesignationTemplate();
      }
    } catch {
      setLoading(false);
      setData(emptyPayroll());
      setToaster({
        open: true,
        messages: [translate("Failed to load payroll")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [allowanceTypes, applyDesignationTemplate, configuredOwnerId, deductionTypes, designationId, isDesignationPayroll, ownerQueryName, payrollApi, translate, usesDesignationTemplate]);

  useEffect(() => {
    void fetchPayroll();
  }, [fetchPayroll]);

  useEffect(() => {
    if (isDesignationPayroll || !applyDesignationTemplateKey || !Number(designationId || 0)) return;
    if (appliedTemplateKey.current === applyDesignationTemplateKey) return;
    appliedTemplateKey.current = applyDesignationTemplateKey;
    void applyDesignationTemplate();
  }, [applyDesignationTemplate, applyDesignationTemplateKey, designationId, isDesignationPayroll]);

  // Base Salary is a single amount field. Its earning type is supplied from
  // the configured Base Salary earning type, so users do not need another tab
  // or a type/date selection just to set an employee's base salary.
  useEffect(() => {
    const defaultType = baseSalaryTypes[0];
    if (!defaultType) return;

    setData((previous) => {
      const baseSalary = ensureSingleBaseSalary(previous.baseSalaries);
      const updated = baseSalary.map((line) =>
        line.typeId
          ? line
          : { ...line, typeId: defaultType.id, typeName: defaultType.name },
      );
      const changed = updated.some(
        (line, index) => line !== previous.baseSalaries[index],
      );
      return changed || previous.baseSalaries.length !== 1
        ? { ...previous, baseSalaries: updated }
        : previous;
    });
  }, [baseSalaryTypes, data.baseSalaries]);

  // A designation chooses from the master Earning and Deduction Types. New
  // master records therefore appear here automatically and stay unchecked
  // until this designation explicitly allows them.
  useEffect(() => {
    if (!usesDesignationTemplate) return;

    setData((previous) => ({
      ...previous,
      allowances: mergeDesignationMasterTypes(previous.allowances, allowanceTypes, "allowances"),
      deductions: mergeDesignationMasterTypes(previous.deductions, deductionTypes, "deductions"),
    }));
  }, [allowanceTypes, deductionTypes, usesDesignationTemplate]);

  const optionsFor = (type: LookupType | null) => {
    if (type === "baseSalary") return baseSalaryTypes;
    if (type === "allowance") return allowanceTypes;
    if (type === "deduction") return deductionTypes;
    return [];
  };

  const updateLine = (section: LineSection, rowId: number | string, patch: Partial<PayrollLine>) => {
    if (!canUpdate) return;

    setData((prev) => ({
      ...prev,
      [section]: prev[section].map((row) =>
        row.id === rowId ? { ...row, ...patch } : row,
      ),
    }));
  };

  const addLine = (section: LineSection) => {
    if (!canCreate) return;

    if (section === "baseSalaries" && data.baseSalaries.length >= 1) {
      setToaster({
        open: true,
        messages: [translate("Only one Base Salary row is allowed")],
        severity: "warning",
      });
      return;
    }

    setData((prev) => ({ ...prev, [section]: [...prev[section], createLine()] }));
  };

  const deleteSelected = (section: LineSection) => {
    const ids = selectedIds[section];
    if (!canDelete || ids.length === 0) return;

    setData((prev) => {
      const remaining = prev[section].filter((row) => !ids.includes(row.id));
      return {
        ...prev,
        [section]:
          section === "baseSalaries" && remaining.length === 0
            ? [createLine()]
            : remaining,
      };
    });
    setSelectedIds((prev) => ({ ...prev, [section]: [] }));
  };

  const openLookup = (
    section: LineSection,
    rowId: number | string,
    type: LookupType,
  ) => {
    setLookupState({ open: true, section, rowId, type });
  };

  const closeLookup = () => {
    setLookupState({ open: false, type: null, section: null, rowId: null });
  };

  const validate = () => {
    const hasSelectedSalaryAmounts =
      data.allowances.some((row) => !usesDesignationTemplate || row.isAllowed) ||
      data.deductions.some((row) => !usesDesignationTemplate || row.isAllowed);
    if (
      usesDesignationTemplate &&
      data.baseSalaries.every((row) => row.value === "") &&
      data.loanTypeIds.length > 0 &&
      !hasSelectedSalaryAmounts
    ) {
      return null;
    }
    if (data.baseSalaries.length === 0) {
      return translate("Base Salary cannot be empty");
    }

    const sections: Array<[LineSection, string]> = [
      ["baseSalaries", "Base Salary"],
      ["allowances", "Allowances"],
      ["deductions", "Deduction"],
    ];

    for (const [section, label] of sections) {
      for (const row of data[section]) {
        if (usesDesignationTemplate && section !== "baseSalaries" && !row.isAllowed) continue;
        // Base Salary type is optional; it is defaulted when a Base Salary earning type exists.
        if (section !== "baseSalaries" && !row.typeId) {
          return translate(`${label} type is required`);
        }
        const inputValue = row.value;
        if (inputValue === "" || inputValue === null || inputValue === undefined) {
          return translate(`${label} value is required`);
        }
        if (!Number.isFinite(Number(inputValue)) || Number(inputValue) < 0) {
          return translate(`${label} value must be a valid non-negative number`);
        }
        if (section === "baseSalaries") {
          if (!isDesignationPayroll && designationSalaryRange) {
            const salary = Number(inputValue);
            if (
              salary < designationSalaryRange.minimumSalary ||
              salary > designationSalaryRange.maximumSalary
            ) {
              return translate(
                `Base Salary must be between ${formatAmount(designationSalaryRange.minimumSalary)} and ${formatAmount(designationSalaryRange.maximumSalary)} for the selected designation`,
              );
            }
          }
          if (!isDesignationPayroll) continue;
          const toValue = row.toValue;
          if (toValue === "" || toValue === null || toValue === undefined) {
            return translate("Base Salary To Amount is required");
          }
          if (!Number.isFinite(Number(toValue)) || Number(toValue) < 0) {
            return translate("Base Salary To Amount must be a valid non-negative number");
          }
          if (Number(toValue) < Number(inputValue)) {
            return translate("Base Salary To Amount cannot be less than From Amount");
          }
        }
        if (section === "allowances" && row.isPercentage && Number(inputValue) > 100) {
          return translate("Allowance percentage must be between 0 and 100");
        }
      }
    }

    return null;
  };

  const hasPayrollSetup = () =>
    data.baseSalaries.some((line) => line.value !== "") ||
    data.allowances.some((line) => !usesDesignationTemplate || line.isAllowed) ||
    data.deductions.some((line) => !usesDesignationTemplate || line.isAllowed) ||
    data.loanAllowed ||
    data.advanceAllowed ||
    data.payFullSalary ||
    data.overtimeAllowed ||
    data.loanTypeIds.length > 0;

  const save = async (ownerId?: number) => {
    if (!canUpdate) return;
    const payrollOwnerId = Number(ownerId || configuredOwnerId);
    if (!Number.isInteger(payrollOwnerId) || payrollOwnerId <= 0) {
      setToaster({
        open: true,
        messages: [translate(isDesignationPayroll ? "Select a designation before saving salary" : "Save the employee profile before saving salary")],
        severity: "warning",
      });
      return;
    }

    const error = validate();
    if (error) {
      setToaster({ open: true, messages: [error], severity: "error" });
      return;
    }

    const serializeLine = (row: PayrollLine) => ({
      id: typeof row.id === "number" ? row.id : undefined,
      typeId: row.typeId ? Number(row.typeId) : null,
      startDate: row.startDate,
      endDate: row.endDate,
      value: Number(row.value),
      toValue: isDesignationPayroll
        ? (row.toValue === "" || row.toValue === undefined ? null : Number(row.toValue))
        : Number(row.value),
    });
    const hasSelectedSalaryAmounts =
      data.allowances.some((row) => !usesDesignationTemplate || row.isAllowed) ||
      data.deductions.some((row) => !usesDesignationTemplate || row.isAllowed);
    const loanOnlySetup =
      usesDesignationTemplate &&
      data.loanTypeIds.length > 0 &&
      !hasSelectedSalaryAmounts &&
      data.baseSalaries.every((row) => row.value === "");

    try {
      setLoading(true);
      await PostService(payrollApi, {
        id: data.id,
        [ownerQueryName]: payrollOwnerId,
        loanAllowed: false,
        advanceAllowed: false,
        payFullSalary: data.payFullSalary,
        overtimeAllowed: data.overtimeAllowed,
        loanTypeIds: [],
        baseSalaries: loanOnlySetup ? [] : data.baseSalaries.map(serializeLine),
        allowances: data.allowances
          .filter((row) => !usesDesignationTemplate || row.isAllowed)
          .map(row => ({
          ...serializeLine(row),
          ...serializeAllowanceAmount(row),
          })),
        deductions: data.deductions
          .filter((row) => !usesDesignationTemplate || row.isAllowed)
          .map(serializeLine),
      });

      await fetchPayroll(payrollOwnerId);
      setToaster({
        open: true,
        messages: [translate(isDesignationPayroll ? "Designation salary saved successfully" : "Payroll saved successfully")],
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
            translate("Failed to save payroll"),
        ],
        severity: "error",
      });
    } finally {      
      setLoading(false);
    }
  };

  useImperativeHandle(ref, () => ({
    hasPayrollSetup,
    validateConfiguredPayroll: () => (hasPayrollSetup() ? validate() : null),
    save,
  }), [data, configuredOwnerId, fetchPayroll, isDesignationPayroll, ownerQueryName, payrollApi, translate, usesDesignationTemplate]);

  const makeColumns = (
    section: LineSection,
    label: string,
    lookupType: LookupType,
    options: LookupOption[],
  ) => [
    ...(usesDesignationTemplate && section !== "baseSalaries" ? [{
      id: "isAllowed",
      label: translate("Allowed"),
      minWidth: 100,
      format: (_: any, row?: PayrollLine) => row ? (
        <Checkbox
          checked={Boolean(row.isAllowed)}
          disabled={readOnly || !canUpdate}
          inputProps={{ "aria-label": translate(`Allow ${row.typeName}`) }}
          onClick={(event) => event.stopPropagation()}
          onChange={(event) => updateLine(section, row.id, { isAllowed: event.target.checked })}
        />
      ) : "",
    }] : []),
    {
      id: "sno",
      label: translate("S.No"),
      minWidth: section === "allowances" ? 50 : 70,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    {
      id: "typeName",
      label: translate(label),
      minWidth: section === "allowances" ? 200 : 240,
      format: (_: any, row?: PayrollLine) =>
        row ? (
          usesDesignationTemplate && section !== "baseSalaries" ? (
            <Typography sx={{ fontWeight: 700, color: "var(--lms-text-main)" }}>
              {row.typeName}
            </Typography>
          ) : <LookupDropdown
            hideLabel
            value={row.typeId}
            options={options}
            getOptionLabel={(option) => option.name || ""}
            onSelect={(option) =>
              updateLine(section, row.id, {
                typeId: option?.id ?? "",
                typeName: option?.name ?? "",
              })
            }
            placeholder={translate(`Select ${label}`)}
            onSearchClick={() => openLookup(section, row.id, lookupType)}
          />
        ) : (
          ""
        ),
    },
    {
      id: "value",
      label: translate("Value"),
      minWidth: 160,
      align: "right" as const,
      format: (_: any, row?: PayrollLine) =>
        row ? section === "allowances" ? (
          <PayrollAmountInput
            key={`${row.id}-${Boolean(row.isPercentage)}`}
            label={translate(row.isPercentage ? "Percentage" : "Amount")}
            hideLabel
            value={row.value}
            percentage={Boolean(row.isPercentage)}
            disabled={readOnly || !canUpdate || (usesDesignationTemplate && !row.isAllowed)}
            onChange={(value) => updateLine(section, row.id, { value })}
            sx={{ width: 140 }}
          />
        ) : (
          <TextField
            type="number"
            size="small"
            value={row.value}
            onClick={(event) => event.stopPropagation()}
            onChange={(event) =>
              updateLine(section, row.id, {
                value:
                  event.target.value === "" ? "" : Number(event.target.value),
              })
            }
            disabled={readOnly || !canUpdate || (usesDesignationTemplate && !row.isAllowed)}
            inputProps={{ min: 0, style: { textAlign: "right" } }}
            sx={{ width: 140 }}
          />
        ) : (
          ""
        ),
    },
    ...(section === "allowances" ? [
      {
        id: "isPercentage",
        label: translate("Percentage"),
        minWidth: 100,
        format: (_: any, row?: PayrollLine) => row ? (
          <Checkbox
            checked={Boolean(row.isPercentage)}
            disabled={readOnly || !canUpdate || (usesDesignationTemplate && !row.isAllowed)}
            inputProps={{ "aria-label": translate("Calculate allowance as percentage of Base Salary") }}
            onClick={(event) => event.stopPropagation()}
            onChange={(event) => {
              const checked = event.target.checked;
              // Leave a new rate blank rather than interpreting a fixed amount as a rate.
              // Switching back to fixed keeps the currently calculated monetary amount.
              updateLine(section, row.id, checked
                ? { isPercentage: true, value: "" }
                : { isPercentage: false, value: calculateAllowanceAmount(row, salarySummary.baseSalary) });
            }}
          />
        ) : "",
      },
      {
        id: "calculatedAmount",
        label: translate("Calculated Amount"),
        minWidth: 160,
        align: "right" as const,
        format: (_: any, row?: PayrollLine) => {
          if (!row) return "";
          const fromAmount = calculateAllowanceAmount(row, salarySummary.baseSalary);
          const toAmount = calculateAllowanceAmount(row, salarySummary.maximumBaseSalary);
          return fromAmount !== toAmount
            ? `${formatPayrollAmount(fromAmount)} - ${formatPayrollAmount(toAmount)}`
            : formatPayrollAmount(fromAmount);
        },
      },
    ] : []),
  ];

  const tableConfigs = useMemo(
    () => ({
      baseSalaries: {
        title: translate("Base Salary"),
        label: "Earning Type",
        lookupType: "baseSalary" as LookupType,
        options: baseSalaryTypes,
      },
      allowances: {
        title: translate("Allowances"),
        label: "Earning Type",
        lookupType: "allowance" as LookupType,
        options: allowanceTypes,
      },
      deductions: {
        title: translate("Deduction"),
        label: "Deduction Type",
        lookupType: "deduction" as LookupType,
        options: deductionTypes,
      },
    }),
    [allowanceTypes, baseSalaryTypes, deductionTypes, translate],
  );

  const renderAmountGrid = (section: LineSection) => {
    const config = tableConfigs[section];
    const rows = usesDesignationTemplate && section !== "baseSalaries"
      ? [...data[section]].sort((left, right) =>
          Number(Boolean(right.isAllowed)) - Number(Boolean(left.isAllowed)) ||
          left.typeName.localeCompare(right.typeName),
        )
      : data[section];

    return (
      <SchemaTable
        title={config.title}
        columns={makeColumns(section, config.label, config.lookupType, config.options)}
        rows={rows}
        // SchemaTable defaults to newest-first and would otherwise reverse the
        // allowed-first order prepared above.
        defaultLatestFirst={false}
        hideTitle
        stickyHeader
        showAddButton={!(usesDesignationTemplate && section !== "baseSalaries")}
        onAddClick={usesDesignationTemplate && section !== "baseSalaries" ? undefined : () => addLine(section)}
        pagination={false}
        filtersEnabled={false}
        actions={false}
        checkboxSelection={!usesDesignationTemplate || section === "baseSalaries"}
        selectedIds={selectedIds[section]}
        onSelectionChange={(ids: any[]) =>
          setSelectedIds((prev) => ({ ...prev, [section]: ids }))
        }
        disableRowSelectionOnClick
        showToaster={false}
        extraToolbarItemsLeft={usesDesignationTemplate && section !== "baseSalaries" ? undefined :
          <>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={() => deleteSelected(section)}
              disabled={!canDelete || selectedIds[section].length === 0}
              sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2, ml: 1 }}
            >
              <T>Delete</T>
            </Button>
          </>
        }
      />
    );
  };

  const selectedValue =
    lookupState.section && lookupState.rowId !== null
      ? data[lookupState.section].find((row) => row.id === lookupState.rowId)
          ?.typeId
      : undefined;
  return (
    <>
      {loading && <LmsOverlayLoader />}
      {!hideSaveButton && <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 2.5 }}>
        <Button
          variant="contained"
          startIcon={<SaveIcon />}
          onClick={() => void save()}
          disabled={readOnly || !canUpdate}
          sx={{ textTransform: "none", fontWeight: 800, borderRadius: 2 }}
        >
          <T>Save</T>
        </Button>
      </Box>}

      <Box
        aria-disabled={readOnly}
        sx={{ display: "grid", gap: 3, pb: 1, pointerEvents: readOnly ? "none" : "auto", opacity: readOnly ? 0.58 : 1 }}
      >
        <Box>
          <SalarySectionHeading>
            <T>Pay Setup</T>
          </SalarySectionHeading>
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
              columnGap: 3,
              rowGap: 0.75,
              maxWidth: 780,
            }}
          >
            {[              ["payFullSalary", "Pay Full Salary"],              ["overtimeAllowed", "Over Time Allowed"],
            ].map(([field, label]) => (
              <FormControlLabel
                key={field}
                control={
                  <Checkbox
                    checked={Boolean(data[field as keyof PayrollState])}
                    disabled={!canUpdate}
                    onChange={(event) =>
                      canUpdate &&
                      setData((prev) => ({
                        ...prev,
                        [field]: event.target.checked,
                      }))
                    }
                  />
                }
                label={translate(label)}
                sx={{
                  "& .MuiFormControlLabel-label": {
                    fontWeight: 700,
                    color: "var(--lms-text-main)",
                  },
                }}
              />
            ))}
          </Box>
        </Box>

        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "1fr",
            gap: 3,
            alignItems: "stretch",
          }}
        >
          <Box>
            <SalarySectionHeading>
              <T>Base Salary</T>
            </SalarySectionHeading>
            <Box
              sx={{
                display: "flex",
                alignItems: { xs: "stretch", sm: "center" },
                flexDirection: { xs: "column", sm: "row" },
                flexWrap: "wrap",
                gap: 1.5,
              }}
            >
            <PayrollAmountInput
              label={translate(isDesignationPayroll ? "From Amount" : "Base Salary")}
              disabled={readOnly || !canUpdate}
              value={data.baseSalaries[0]?.value ?? ""}
              onChange={(value) => {
                if (!canUpdate) return;
                setData((previous) => {
                  const [baseSalary] = ensureSingleBaseSalary(previous.baseSalaries);
                  return {
                    ...previous,
                    baseSalaries: [{ ...baseSalary, value }],
                  };
                });
              }}
              sx={{ width: { xs: "100%", sm: 280 } }}
            />
            {isDesignationPayroll && (
              <PayrollAmountInput
                label={translate("To Amount")}
                disabled={readOnly || !canUpdate}
                value={data.baseSalaries[0]?.toValue ?? ""}
                onChange={(toValue) => {
                  if (!canUpdate) return;
                  setData((previous) => {
                    const [baseSalary] = ensureSingleBaseSalary(previous.baseSalaries);
                    return {
                      ...previous,
                      baseSalaries: [{ ...baseSalary, toValue }],
                    };
                  });
                }}
                sx={{ width: { xs: "100%", sm: 280 } }}
              />
            )}
            {!isDesignationPayroll && designationSalaryRange && (
              <Box
                sx={{
                  px: 1.5,
                  py: 0.85,
                  display: "flex",
                  alignItems: { xs: "flex-start", sm: "center" },
                  flexDirection: { xs: "column", md: "row" },
                  gap: { xs: 0.75, md: 1.75 },
                  width: { xs: "100%", sm: "fit-content" },
                  border: "1px solid #d7e2ef",
                  borderRadius: 1.5,
                  bgcolor: "#f8fafc",
                }}
              >
                <Typography sx={{ fontSize: 12, fontWeight: 900, color: "#5f7694", whiteSpace: "nowrap" }}>
                  {translate("Designation Salary Range")}
                </Typography>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1.75, flexWrap: "wrap" }}>
                  <Box sx={{ display: "flex", alignItems: "baseline", gap: 0.5 }}>
                    <Typography sx={{ fontSize: 11, fontWeight: 700, color: "#64748b" }}>
                      {translate("Min")}
                    </Typography>
                    <Typography sx={{ fontSize: 13, fontWeight: 900, color: "#1e293b" }}>
                      {formatAmount(designationSalaryRange.minimumSalary)}
                    </Typography>
                  </Box>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "baseline",
                      gap: 0.5,
                      pl: { xs: 0, sm: 1.75 },
                      borderLeft: { xs: "none", sm: "1px solid #d7e2ef" },
                    }}
                  >
                    <Typography sx={{ fontSize: 11, fontWeight: 700, color: "#64748b" }}>
                      {translate("Max")}
                    </Typography>
                    <Typography sx={{ fontSize: 13, fontWeight: 900, color: "#1e293b" }}>
                      {formatAmount(designationSalaryRange.maximumSalary)}
                    </Typography>
                  </Box>
                </Box>
              </Box>
            )}
            </Box>
          </Box>
        </Box>

        <Box sx={{ minWidth: 0 }}>
          <StyledTabs
            value={salaryDetailTab}
            onChange={(_, value) => setSalaryDetailTab(value)}
            variant="scrollable"
            scrollButtons="auto"
            tabs={[
              { label: translate("Allowances") },
              { label: translate("Deduction") },            ]}
            sx={{ mb: 1 }}
          />

          <TabPanel value={salaryDetailTab} index={0} padding={0}>
            <Typography variant="caption" sx={{ display: "block", mb: 1, color: "var(--lms-text-secondary)" }}>
              <T>Percentage allowances are calculated from Base Salary.</T>
            </Typography>
            {renderAmountGrid("allowances")}
          </TabPanel>

          <TabPanel value={salaryDetailTab} index={1} padding={0}>
            {renderAmountGrid("deductions")}
          </TabPanel>
        </Box>

        <Box>
          <SalarySectionHeading>
            <T>Payroll</T>
          </SalarySectionHeading>
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: {
                xs: "1fr",
                sm: "repeat(2, minmax(0, 1fr))",
                lg: "repeat(4, minmax(0, 1fr))",
              },
              gap: 2,
            }}
          >
            {[
              {
                label: "Gross Salary",
                value: salarySummary.baseSalary,
                color: "#334155",
              },
              {
                label: "Allowances",
                value: salarySummary.allowances,
                color: "#059669",
              },
              {
                label: "Deduction",
                value: salarySummary.deductions,
                color: "#dc2626",
              },
              {
                label: "Net Salary",
                value: salarySummary.netSalary,
                color: "#0284c7",
              },
            ].map((item) => (
              <Box
                key={item.label}
                sx={{
                  minHeight: 68,
                  px: 1.5,
                  py: 1.25,
                  border: "1px solid #dbe4ee",
                  borderRadius: 1.5,
                  bgcolor: "#f8fafc",
                  boxShadow: "0 5px 12px rgba(15, 23, 42, 0.08)",
                }}
              >
                <Typography
                  sx={{
                    fontSize: 12,
                    fontWeight: 900,
                    color: "#5f7694",
                    textTransform: "uppercase",
                  }}
                >
                  {translate(item.label)}
                </Typography>
                <Typography
                  sx={{
                    mt: 0.5,
                    fontSize: 18,
                    fontWeight: 900,
                    color: item.color,
                    textAlign: "right",
                  }}
                >
                  {formatAmount(item.value)}
                </Typography>
              </Box>
            ))}
          </Box>
        </Box>

      </Box>

      <GenericSearchModal
        open={lookupState.open}
        onClose={closeLookup}
        title={
          lookupState.type === "deduction"
            ? translate("Deduction Type")
            : translate("Earning Type")
        }
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={optionsFor(lookupState.type)}
        selectedValue={selectedValue}
        selectedField="id"
        onSelect={(selectedRow: any) => {
          if (lookupState.section && lookupState.rowId !== null) {
            updateLine(lookupState.section, lookupState.rowId, {
              typeId: selectedRow?.id ?? "",
              typeName: selectedRow?.name ?? "",
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
});
