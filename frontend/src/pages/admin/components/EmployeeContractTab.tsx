import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Button, Grid, TextField, Typography } from "@mui/material";
import {
  ArrowDownward,
  ArrowUpward,
  CancelOutlined,
  HistoryOutlined,
  PauseCircleOutline,
  RestartAlt,
  Save,
} from "@mui/icons-material";
import dayjs, { Dayjs } from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormDatePicker } from "@/components/FormDatePicker";
import { StyledTabs } from "@/components/StyledTabs";
import { EmployeePayrollTab } from "./EmployeePayrollTab";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService, PostService, PutService } from "@/constants/globalItems";

const FAR_FUTURE = dayjs("9998-12-31").toISOString();

type Id = number | string;
type LookupType = "employmentType" | "department" | "designation" | "grade";
type ActionTab = "promotion" | "demotion" | "suspension" | "reinstatement";
type ContractTab = ActionTab | "history";

type Row = {
  id: Id;
  employmentId?: number;
  positionId?: number;
  isLegacy?: boolean;
  employmentTypeId: number | "";
  employmentType?: string;
  actionId: number | "";
  employmentAction?: string;
  departmentId: number | "";
  department?: string;
  designationId: number | "";
  designation?: string;
  gradeId: number | "";
  grade?: string;
  fromDate: string;
  tillDate: string;
  employmentRemarks: string;
  jobResponsibility: string;
};

const actionTabs: Array<{
  key: ActionTab;
  label: string;
  actionName: string;
  icon: React.ReactElement;
}> = [
  {
    key: "promotion",
    label: "Promote",
    actionName: "Promotion",
    icon: <ArrowUpward fontSize="small" />,
  },
  {
    key: "demotion",
    label: "Demote",
    actionName: "Demotion",
    icon: <ArrowDownward fontSize="small" />,
  },
  {
    key: "suspension",
    label: "Suspend",
    actionName: "Suspension",
    icon: <PauseCircleOutline fontSize="small" />,
  },
  {
    key: "reinstatement",
    label: "Re-instate",
    actionName: "Reinstatement",
    icon: <RestartAlt fontSize="small" />,
  },
];

const tabKeys: ContractTab[] = [
  ...actionTabs.map((tab) => tab.key),
  "history",
];

const unpack = (response: any): any[] => {
  const data = response?.data ?? response ?? {};
  return (
    data.items ??
    data.Items ??
    data.data ??
    data.Data ??
    (Array.isArray(data) ? data : [])
  );
};

const pick = (response: any, ids: string[], names: string[]) =>
  unpack(response)
    .map((x: any) => ({
      id: Number(ids.map((key) => x[key]).find((value) => value != null) ?? 0),
      name: String(
        names.map((key) => x[key]).find((value) => value != null) ?? "",
      ),
    }))
    .filter((item: any) => item.id > 0 || item.name);

export const EmployeeContractTab: React.FC<{
  employeeId?: string;
  onContractSaved?: () => void;
}> = ({ employeeId, onContractSaved }) => {
  const { translate } = useTranslation();
  // Additional Information is editable independently of UT action rights.
  const canCreate = true;

  const [rows, setRows] = useState<Row[]>([]);
  const [types, setTypes] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [grades, setGrades] = useState<any[]>([]);
  const [actions, setActions] = useState<any[]>([]);
  const [activeTab, setActiveTab] = useState<ContractTab>("promotion");
  const [draft, setDraft] = useState<Row | null>(null);
  const [draftAction, setDraftAction] = useState<ActionTab | null>(null);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [lookupType, setLookupType] = useState<LookupType | null>(null);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<any>({
    open: false,
    messages: [],
    severity: "info",
  });

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;
    setLoading(true);
    try {
      const [employmentResponse, positionResponse] = await Promise.all([
        GetService(
          `/api/HumanResource/EREmployment/v3?employeeId=${employeeId}&pageSize=1000`,
        ),
        GetService(
          `/api/HumanResource/EREmpPosition/v3?employeeId=${employeeId}&pageSize=1000`,
        ),
      ]);
      const employment = unpack(employmentResponse);
      const positions = unpack(positionResponse);

      setRows(
        positions.map((position: any) => {
          const employmentRow = employment.find((item: any) =>
            dayjs(item.startDate ?? item.startdate).isSame(
              dayjs(position.fromDate ?? position.fromdate),
              "day",
            ),
          );
          return {
            id: position.id,
            employmentId: employmentRow?.id,
            positionId: position.id,
            isLegacy: !employmentRow,
            employmentTypeId:
              employmentRow?.employmentTypeId ??
              employmentRow?.employmenttypeid ??
              "",
            employmentType:
              employmentRow?.employmentType ?? employmentRow?.employmenttype,
            actionId: position.actionId ?? "",
            employmentAction: position.actionName ?? "",
            departmentId: position.departmentId ?? "",
            department: position.departmentName ?? "",
            designationId: position.designationId ?? "",
            designation: position.designationName ?? "",
            gradeId: position.gradeId ?? "",
            grade: position.gradeName ?? "",
            fromDate: position.fromDate ?? "",
            tillDate: position.tillDate ?? FAR_FUTURE,
            employmentRemarks: position.remarks ?? "",
            jobResponsibility: position.responsibility ?? "",
          } satisfies Row;
        }),
      );
    } catch {
      setToast({
        open: true,
        messages: [translate("Failed to load contract history")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  useEffect(() => {
    void Promise.all([
      GetService("/api/HumanResource/PositionType/v3/EmployeeTypeLookup"),
      GetService("/api/HumanResource/Lookups/v3/departments"),
      GetService("/api/HumanResource/Lookups/v3/designations"),
      GetService("/api/HumanResource/EmployeeProfile/v3/lookups/grades"),
      GetService("/api/HumanResource/Lov/v3/EMACT"),
    ])
      .then(([typeResponse, departmentResponse, designationResponse, gradeResponse, actionResponse]) => {
        setTypes(pick(typeResponse, ["id"], ["stxt", "name"]));
        setDepartments(
          pick(
            departmentResponse,
            ["id", "departmentId", "depid"],
            ["departmentName", "department", "depstxt", "name"],
          ),
        );
        setDesignations(
          pick(
            designationResponse,
            ["id", "designationId"],
            ["designationName", "designation", "stxt", "name"],
          ),
        );
        setGrades(pick(gradeResponse, ["id"], ["code", "name"]));
        setActions(
          unpack(actionResponse).map((item: any) => ({
            id: Number(item.id ?? item.Id),
            code: String(item.code ?? item.Code ?? ""),
            name: String(item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? ""),
          })),
        );
      })
      .catch(() =>
        setToast({
          open: true,
          messages: [translate("Failed to load Contract lookups")],
          severity: "error",
        }),
      );
  }, [translate]);

  const current = () =>
    rows.find((row) => dayjs(row.tillDate).year() === 9998) ?? rows[0];

  const getAction = (action: ActionTab) => {
    const name = actionTabs.find((tab) => tab.key === action)?.actionName ?? "";
    return actions.find(
      (item) =>
        String(item.code).toLowerCase() === name.toLowerCase() ||
        String(item.name).toLowerCase() === name.toLowerCase(),
    );
  };

  const buildDraft = (action: ActionTab): Row => {
    const previous = current();
    const configuredAction = getAction(action);
    return {
      id: `draft-${action}-${Date.now()}`,
      employmentTypeId: previous?.employmentTypeId ?? "",
      employmentType: previous?.employmentType,
      actionId: configuredAction?.id ?? "",
      employmentAction:
        configuredAction?.name ??
        actionTabs.find((tab) => tab.key === action)?.actionName ??
        "",
      departmentId: previous?.departmentId ?? "",
      department: previous?.department,
      designationId: previous?.designationId ?? "",
      designation: previous?.designation,
      gradeId: previous?.gradeId ?? "",
      grade: previous?.grade,
      fromDate: dayjs().toISOString(),
      tillDate: FAR_FUTURE,
      employmentRemarks: "",
      jobResponsibility: previous?.jobResponsibility ?? "",
    };
  };

  const editingAction: ActionTab =
    draftAction ?? (activeTab === "history" ? "promotion" : activeTab);
  const editingRow = draft ?? buildDraft(editingAction);

  const updateDraft = (patch: Partial<Row>) => {
    if (!canCreate) return;
    setDraft((previous) => ({ ...(previous ?? buildDraft(editingAction)), ...patch }));
    setDraftAction(editingAction);
    setHasUnsavedChanges(true);
  };

  const closeLookup = () => setLookupType(null);

  const handleTabChange = (_event: React.SyntheticEvent, nextIndex: number) => {
    const nextTab = tabKeys[nextIndex];
    if (!nextTab) return;

    if (nextTab === "history") {
      setActiveTab("history");
      return;
    }

    if (hasUnsavedChanges && draftAction && nextTab !== draftAction) return;
    setActiveTab(nextTab);
    if (!hasUnsavedChanges) {
      setDraft(null);
      setDraftAction(null);
    }
  };

  const cancelDraft = () => {
    setDraft(null);
    setDraftAction(null);
    setHasUnsavedChanges(false);
  };

  const save = async () => {
    const row = draft ?? buildDraft(editingAction);
    if (!employeeId) return;
    if (
      !row.employmentTypeId ||
      !row.departmentId ||
      !row.designationId ||
      !row.gradeId ||
      !row.actionId ||
      !row.fromDate
    ) {
      setToast({
        open: true,
        messages: [
          translate(
            "Employment Type, Department, Designation, Grade, and From Date are required",
          ),
        ],
        severity: "error",
      });
      return;
    }
    if (
      editingAction === "suspension" &&
      (!row.tillDate || dayjs(row.tillDate).isBefore(dayjs(row.fromDate), "day"))
    ) {
      setToast({
        open: true,
        messages: [translate("Suspension Till Date must be on or after From Date")],
        severity: "error",
      });
      return;
    }

    try {
      setLoading(true);
      const previous = current();
      const endDate = dayjs(row.fromDate).subtract(1, "day").toISOString();

      if (previous?.employmentId && dayjs(previous.tillDate).year() === 9998) {
        await PutService("/api/HumanResource/EREmployment/v3", {
          id: previous.employmentId,
          employeeId: Number(employeeId),
          employmentTypeId: previous.employmentTypeId,
          startDate: previous.fromDate,
          endDate,
        });
      }

      await PostService("/api/HumanResource/EREmployment/v3", {
        employeeId: Number(employeeId),
        employmentTypeId: Number(row.employmentTypeId),
        startDate: row.fromDate,
        endDate: row.tillDate || FAR_FUTURE,
      });
      await PostService("/api/HumanResource/EREmpPosition/v3", {
        employeeId: Number(employeeId),
        departmentId: Number(row.departmentId),
        designationId: Number(row.designationId),
        gradeId: Number(row.gradeId),
        actionId: Number(row.actionId),
        remarks: row.employmentRemarks || null,
        fromDate: row.fromDate,
        tillDate: row.tillDate || FAR_FUTURE,
        responsibility: row.jobResponsibility || null,
      });

      await fetchRows();
      cancelDraft();
      onContractSaved?.();
      setToast({
        open: true,
        messages: [translate("Contract action saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToast({
        open: true,
        messages: [
          error?.response?.data ??
            error?.message ??
            translate("Failed to save Contract action"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const historyColumns = useMemo(
    () => [
      { id: "employmentAction", label: translate("Employment Action"), minWidth: 180 },
      { id: "employmentType", label: translate("Employment Type"), minWidth: 180 },
      { id: "department", label: translate("Department"), minWidth: 220 },
      { id: "designation", label: translate("Designation"), minWidth: 220 },
      { id: "grade", label: translate("Grade"), minWidth: 150 },
      {
        id: "fromDate",
        label: translate("From Date"),
        minWidth: 150,
        format: (value: string) =>
          value ? dayjs(value).format("MM/DD/YYYY") : "-",
      },
      {
        id: "tillDate",
        label: translate("Till Date"),
        minWidth: 150,
        format: (value: string) =>
          value ? dayjs(value).format("MM/DD/YYYY") : "-",
      },
      {
        id: "employmentRemarks",
        label: translate("Employment Remarks"),
        minWidth: 280,
      },
      {
        id: "jobResponsibility",
        label: translate("Job Responsibility"),
        minWidth: 280,
      },
    ],
    [translate],
  );

  const lookupOptions =
    lookupType === "employmentType"
      ? types
      : lookupType === "department"
        ? departments
        : lookupType === "designation"
          ? designations
          : grades;
  const lookupTitle =
    lookupType === "employmentType"
      ? translate("Employment Type")
      : lookupType === "department"
        ? translate("Department")
        : lookupType === "designation"
          ? translate("Designation")
          : translate("Grade");
  const isSuspension = editingAction === "suspension";

  const lookupField = (
    label: string,
    type: LookupType,
    value: number | "",
    options: any[],
  ) => (
    <LookupDropdown
      label={translate(label)}
      required
      value={value}
      options={options}
      disabled={!canCreate}
      getOptionLabel={(option: any) => option.name || ""}
      onSearchClick={() => setLookupType(type)}
      onSelect={(option: any) => {
        const id = option?.id ?? "";
        const name = option?.name ?? "";
        const patches: Record<LookupType, Partial<Row>> = {
          employmentType: { employmentTypeId: id, employmentType: name },
          department: { departmentId: id, department: name },
          designation: { designationId: id, designation: name },
          grade: { gradeId: id, grade: name },
        };
        updateDraft(patches[type]);
      }}
    />
  );

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <StyledTabs
        value={tabKeys.indexOf(activeTab)}
        onChange={handleTabChange}
        variant="scrollable"
        scrollButtons="auto"
        allowScrollButtonsMobile
        tabs={[
          ...actionTabs.map((tab) => ({
            label: translate(tab.label),
            icon: tab.icon,
            disabled: Boolean(
              hasUnsavedChanges && draftAction && draftAction !== tab.key,
            ),
          })),
          {
            label: translate("History"),
            icon: <HistoryOutlined fontSize="small" />,
          },
        ]}
      />

      {activeTab === "history" ? (
        <SchemaTable
          title={translate("Employment Contract History")}
          columns={historyColumns}
          rows={rows}
          stickyHeader
          pagination={false}
          filtersEnabled={false}
          actions={false}
          showAddButton={false}
          showToaster={false}
        />
      ) : (
        <Box sx={{ display: "grid", gap: 3 }}>
          <Box>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 2,
                gap: 2,
              }}
            >
              <Box>
                <Typography variant="h6" sx={{ fontWeight: 800 }}>
                  {translate(
                    actionTabs.find((tab) => tab.key === editingAction)?.label ??
                      "Contract Action",
                  )}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  <T>Complete the contract details and save the action.</T>
                </Typography>
              </Box>
              {hasUnsavedChanges && (
                <Button
                  variant="outlined"
                  color="inherit"
                  startIcon={<CancelOutlined />}
                  onClick={cancelDraft}
                  sx={{ textTransform: "none", fontWeight: 700 }}
                >
                  <T>Cancel</T>
                </Button>
              )}
            </Box>

            <Grid container spacing={2}>
              <Grid item xs={12} md={4}>
                {lookupField(
                  "Employment Type",
                  "employmentType",
                  editingRow.employmentTypeId,
                  types,
                )}
              </Grid>
              <Grid item xs={12} md={4}>
                {lookupField(
                  "Department",
                  "department",
                  editingRow.departmentId,
                  departments,
                )}
              </Grid>
              <Grid item xs={12} md={4}>
                {lookupField(
                  "Designation",
                  "designation",
                  editingRow.designationId,
                  designations,
                )}
              </Grid>
              <Grid item xs={12} md={4}>
                {lookupField("Grade", "grade", editingRow.gradeId, grades)}
              </Grid>
              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("From Date")}
                  required
                  value={editingRow.fromDate ? dayjs(editingRow.fromDate) : null}
                  disabled={!canCreate}
                  onChange={(value: Dayjs | null) =>
                    updateDraft({ fromDate: value?.toISOString() || "" })
                  }
                  format="MM/DD/YYYY"
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <FormDatePicker
                  label={translate("Till Date")}
                  required={isSuspension}
                  value={editingRow.tillDate ? dayjs(editingRow.tillDate) : null}
                  disabled={!canCreate || !isSuspension}
                  onChange={(value: Dayjs | null) =>
                    updateDraft({ tillDate: value?.toISOString() || "" })
                  }
                  format="MM/DD/YYYY"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label={translate("Employment Remarks")}
                  multiline
                  minRows={3}
                  disabled={!canCreate}
                  value={editingRow.employmentRemarks}
                  onChange={(event) =>
                    updateDraft({ employmentRemarks: event.target.value })
                  }
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label={translate("Job Responsibility")}
                  multiline
                  minRows={3}
                  disabled={!canCreate}
                  value={editingRow.jobResponsibility}
                  onChange={(event) =>
                    updateDraft({ jobResponsibility: event.target.value })
                  }
                />
              </Grid>
            </Grid>

            <Box sx={{ display: "flex", justifyContent: "flex-end", mt: 2 }}>
              <Button
                variant="contained"
                startIcon={<Save />}
                onClick={() => void save()}
                disabled={!canCreate}
                sx={{ textTransform: "none", fontWeight: 800 }}
              >
                <T>Save</T>
              </Button>
            </Box>
          </Box>

          <Box
            onChangeCapture={() => {
              setHasUnsavedChanges(true);
              // This editor is rendered only for action tabs; history is not
              // reachable here, so retain the currently selected action.
              setDraftAction((currentAction) => currentAction ?? activeTab);
            }}
          >
            <Typography variant="h6" sx={{ fontWeight: 800, mb: 1 }}>
              <T>Salary</T>
            </Typography>
            <EmployeePayrollTab
              employeeId={employeeId}
              designationId={Number(editingRow.designationId || 0) || undefined}
            />
          </Box>
        </Box>
      )}

      <GenericSearchModal
        open={lookupType !== null}
        onClose={closeLookup}
        title={lookupTitle}
        columns={[
          { id: "id", label: translate("Id"), minWidth: 100 },
          { id: "name", label: translate("Name"), minWidth: 260 },
        ]}
        data={lookupOptions}
        selectedValue={
          lookupType === "employmentType"
            ? editingRow.employmentTypeId
            : lookupType === "department"
              ? editingRow.departmentId
              : lookupType === "designation"
                ? editingRow.designationId
                : editingRow.gradeId
        }
        selectedField="id"
        onSelect={(selected: any) => {
          if (!lookupType) return;
          const id = selected?.id ?? "";
          const name = selected?.name ?? "";
          const patches: Record<LookupType, Partial<Row>> = {
            employmentType: { employmentTypeId: id, employmentType: name },
            department: { departmentId: id, department: name },
            designation: { designationId: id, designation: name },
            grade: { gradeId: id, grade: name },
          };
          updateDraft(patches[lookupType]);
          closeLookup();
        }}
      />
      <LmsToaster
        open={toast.open}
        messages={toast.messages}
        severity={toast.severity}
        onClose={() => setToast((currentToast: any) => ({ ...currentToast, open: false }))}
      />
    </>
  );
};
