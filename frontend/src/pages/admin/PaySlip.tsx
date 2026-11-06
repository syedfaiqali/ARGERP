import React, { useState, useEffect, useMemo } from "react";
import {
  Box,
  Typography,
  Button,
  MenuItem,
  Grid,
  Paper,
  IconButton,
  Collapse,
  Divider,
  Menu,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import {
  PlayArrow as ProcessIcon,
  CheckCircleOutline as ApprovalIcon,
  Clear as ClearIcon,
  KeyboardArrowUp,
  KeyboardArrowDown,
  FilterAlt as FetchIcon,
  Assessment as ReportIcon,
  Menu as MenuIcon,
} from "@mui/icons-material";
import { DashboardTable } from "@/components/DashboardTable";
import type { Column } from "@/components/DashboardTable";
import { ReportsPage, type ReportSectionData } from "@/pages/ReportsPage";
import {
  getViewableReportMenuItems,
  normalizeMenuName,
  useHiddenMenuPermissions,
} from "@/hooks/useHiddenMenuPermissions";

const RowActionMenu = ({
  row,
  onAction,
  canViewPaySlipNewReport,
}: {
  row: any;
  onAction: (action: string, row: any) => void;
  canViewPaySlipNewReport: boolean;
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
  };

  const handleClose = (event?: React.MouseEvent<HTMLElement>) => {
    if (event) event.stopPropagation();
    setAnchorEl(null);
  };

  return (
    <Box>
      <IconButton size="small" onClick={handleClick} sx={{ color: "#94a3b8" }}>
        <MenuIcon fontSize="small" />
      </IconButton>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={() => handleClose()}
        elevation={0}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        MenuListProps={{
          "aria-labelledby": "basic-button",
          sx: { py: 0 }
        }}
        PaperProps={{
          sx: { 
            minWidth: "220px", 
            borderRadius: "16px", 
            border: "1px solid #e2e8f0",
            boxShadow: "0 10px 15px -3px rgba(0, 0, 0, 0.05), 0 4px 6px -2px rgba(0, 0, 0, 0.025)",
            mt: 0.5,
            overflow: "hidden"
          }
        }}
      >
        {canViewPaySlipNewReport && (
          <>
            <Box sx={{ pt: 2, pb: 1, px: 2.5 }}>
              <Typography sx={{ fontSize: "0.8rem", color: "#64748b", fontWeight: 800, fontFamily: "Inter, sans-serif", letterSpacing: "0.5px", textTransform: "uppercase" }}>
                Reports
              </Typography>
            </Box>
            <Divider sx={{ borderColor: "#f1f5f9" }} />
            <MenuItem
              onClick={(e) => { handleClose(e); onAction("report-payslip", row); }}
              sx={{
                px: 2.5,
                py: 1.5,
                bgcolor: "#fff",
                "&:hover": { bgcolor: "#f8fafc" }
              }}
            >
              <ListItemIcon sx={{ minWidth: "36px" }}>
                <Box sx={{ bgcolor: "#0ea5e9", color: "#fff", borderRadius: "6px", width: "24px", height: "24px", display: "flex", alignItems: "center", justifyContent: "center" }}>
                  <ReportIcon sx={{ fontSize: "16px" }} />
                </Box>
              </ListItemIcon>
              <ListItemText primaryTypographyProps={{ sx: { fontSize: "0.95rem", color: "#0ea5e9", fontWeight: 700, fontFamily: "Inter, sans-serif" } }}>
                Pay Slip New
              </ListItemText>
            </MenuItem>
          </>
        )}

        <Box sx={{ pt: canViewPaySlipNewReport ? 1 : 2, pb: 1, px: 2.5, mt: canViewPaySlipNewReport ? 0.5 : 0 }}>
          <Typography sx={{ fontSize: "0.8rem", color: "#64748b", fontWeight: 800, fontFamily: "Inter, sans-serif", letterSpacing: "0.5px", textTransform: "uppercase" }}>
            Options
          </Typography>
        </Box>
        <Divider sx={{ borderColor: "#f1f5f9" }} />
        <MenuItem 
          onClick={(e) => { handleClose(e); onAction("options-approval", row); }} 
          sx={{ 
            px: 2.5, 
            py: 1.5,
            pb: 2,
            bgcolor: "#fff",
            "&:hover": { bgcolor: "#f8fafc" }
          }}
        >
          <ListItemIcon sx={{ minWidth: "36px" }}>
            <Box sx={{ bgcolor: "#0ea5e9", color: "#fff", borderRadius: "6px", width: "24px", height: "24px", display: "flex", alignItems: "center", justifyContent: "center" }}>
              <ApprovalIcon sx={{ fontSize: "16px" }} />
            </Box>
          </ListItemIcon>
          <ListItemText primaryTypographyProps={{ sx: { fontSize: "0.95rem", color: "#0ea5e9", fontWeight: 700, fontFamily: "Inter, sans-serif" } }}>
            Approval
          </ListItemText>
        </MenuItem>
      </Menu>
    </Box>
  );
};
import { FormDatePicker } from "@/components/FormDatePicker";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsSelect } from "@/components/LmsSelect";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GetService, PostService } from "@/constants/globalItems";
import dayjs from "dayjs";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";

interface PaySlipProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const PaySlip: React.FC<PaySlipProps> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const { visibleMenuCardGroups } = useHiddenMenuPermissions();
  const [exportType, setExportType] = useState("None");
  const [showFilters, setShowFilters] = useState(true);
  const [loading, setLoading] = useState(false);
  const [rows, setRows] = useState<any[]>([]);
  const [selectedRows, setSelectedRows] = useState<any[]>([]);
  const [viewMode, setViewMode] = useState<"list" | "report">("list");

  // Toaster state
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("success");

  // Lookups state
  const [registers, setRegisters] = useState<any[]>([]);
  const [periods, setPeriods] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [employees, setEmployees] = useState<any[]>([]);
  const [salaryTypes, setSalaryTypes] = useState<any[]>([]);
  const [processTypes, setProcessTypes] = useState<any[]>([]);

  const [activeLookup, setActiveLookup] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    registerid: "" as string | number,
    periodid: "" as string | number,
    salarytypeid: 261 as string | number, // Default Monthly
    deptid: "" as string | number,
    desgid: "" as string | number,
    employeeid: "" as string | number,
    fromdate: dayjs() as dayjs.Dayjs | null,
    tilldate: dayjs() as dayjs.Dayjs | null,
    processtypeid: 1075 as string | number, // Default Base On Attendance
  });

  useEffect(() => {
    fetchInitialLookups();
  }, []);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "report") {
      onSubViewChange(translate("Pay Slip New"));
    } else {
      onSubViewChange(null);
    }
  }, [viewMode, onSubViewChange, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const paySlipNewReportContext = useMemo(() => {
    for (const group of visibleMenuCardGroups) {
      const reportItem = getViewableReportMenuItems(group.menuItems).find(
        (item) => normalizeMenuName(item.name) === "pay slip new",
      );

      if (reportItem) {
        return {
          category: group.moduleName,
          selectedReport: {
            name: reportItem.name,
            menuId: reportItem.menuId,
            reportId: reportItem.reportId,
            reportCode: reportItem.reportCode,
          },
          sections: [
            {
              title: group.cardName,
              id: normalizeMenuName(group.cardName).replace(/\s+/g, "-"),
              items: [
                {
                  name: reportItem.name,
                  menuId: reportItem.menuId,
                  reportId: reportItem.reportId,
                  reportCode: reportItem.reportCode,
                },
              ],
            },
          ] as ReportSectionData[],
        };
      }
    }

    return null;
  }, [visibleMenuCardGroups]);

  const canViewPaySlipNewReport = Boolean(
    paySlipNewReportContext?.selectedReport.reportId ||
      paySlipNewReportContext?.selectedReport.reportCode,
  );

  useEffect(() => {
    if (viewMode === "report" && !canViewPaySlipNewReport) {
      setViewMode("list");
    }
  }, [canViewPaySlipNewReport, viewMode]);

  const fetchInitialLookups = async () => {
    try {
      setLoading(true);
      const lookupResults = await Promise.allSettled([
        GetService("/api/HumanResource/PayrollRegister/v3"),
        GetService("/api/HumanResource/Lookups/v3/departments"),
        GetService("/api/HumanResource/Lookups/v3/designations"),
        GetService("/api/HumanResource/EmployeeProfile/v3?pageSize=1000"),
        GetService("/api/HumanResource/Lov/v3/ACUNT"), // Salary Type
        GetService("/api/HumanResource/Lov/v3/PROTY"), // Process Type
      ]);

      const [regRes, deptRes, desgRes, empRes, salRes, procRes] = lookupResults.map((result) =>
        result.status === "fulfilled" ? result.value : null
      );

      setRegisters((regRes?.data?.items || regRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.stxt ?? x.name ?? x.Name })));
      setDepartments((deptRes?.data?.items || deptRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.departmentName ?? x.stxt ?? x.name ?? x.Name })));
      setDesignations((desgRes?.data?.items || desgRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.designationName ?? x.stxt ?? x.name ?? x.Name })));
      setEmployees((empRes?.data?.items || empRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.firstName ? `${x.firstName} ${x.lastName ?? ""}` : (x.stxt ?? x.name ?? x.Name) })));
      setSalaryTypes((salRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.stxt ?? x.name ?? x.Name })));
      setProcessTypes((procRes?.data || []).map((x: any) => ({ ...x, id: x.id ?? x.Id, stxt: x.stxt ?? x.name ?? x.Name })));

      if (lookupResults.some((result) => result.status === "rejected")) {
        setToasterMessages(["Some lookup data could not be loaded"]);
        setToasterSeverity("error");
        setShowToaster(true);
      }
    } catch (error) {
      console.error("Failed to fetch lookups", error);
    } finally {
      setLoading(false);
    }
  };

  const fetchPeriods = async (registerId: number) => {
    try {
      const response = await GetService(`/api/HumanResource/EmployeeMonthlyAttendance/v3/pay-period-details?registerId=${registerId}`);
      setPeriods(response.data?.items || response.data || []);
    } catch (err) {
      console.error("Failed to fetch periods", err);
    }
  };

  const handleFetch = async () => {
    if (!formData.registerid || !formData.periodid) {
      setToasterMessages(["Payroll Register and Period are required"]);
      setToasterSeverity("error");
      setShowToaster(true);
      return;
    }

    setLoading(true);
    try {
      const response = await PostService("/api/HumanResource/PaySlip/v3/fetch", {
        registerid: Number(formData.registerid),
        periodid: Number(formData.periodid),
        salarytypeid: Number(formData.salarytypeid),
        deptid: formData.deptid ? Number(formData.deptid) : null,
        desgid: formData.desgid ? Number(formData.desgid) : null,
        employeeid: formData.employeeid ? Number(formData.employeeid) : null,
        processtypeid: Number(formData.processtypeid),
        fromdate: formData.fromdate?.toISOString(),
        tilldate: formData.tilldate?.toISOString(),
      });
      const payload = response.data;
      const items = Array.isArray(payload)
        ? payload
        : Array.isArray(payload?.items)
          ? payload.items
          : Array.isArray(payload?.data)
            ? payload.data
            : [];
      setSelectedRows([]);
      setRows(items.map((row: any) => ({
        ...row,
        rowKey: [
          row.Id ?? row.id ?? 0,
          row.employeeid ?? row.employeeId ?? row.EmployeeId ?? "",
          Number(formData.registerid),
          Number(formData.periodid),
        ].join("-"),
        name: row.name ?? row.Name ?? "",
        Name: row.Name ?? row.name ?? "",
      })));
    } catch (error) {
      console.error("Failed to fetch pay slip data", error);
      setToasterMessages(["Failed to fetch data"]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setLoading(false);
    }
  };

  const handleProcess = async () => {
    if (selectedRows.length === 0) {
      setToasterMessages(["Please select records to process"]);
      setToasterSeverity("error");
      setShowToaster(true);
      return;
    }

    setLoading(true);
    let successCount = 0;
    let failCount = 0;

    for (const row of selectedRows) {
      try {
        await PostService("/api/HumanResource/PaySlip/v3/process", {
          ...row,
          registerid: Number(formData.registerid),
          periodid: row.periodid ?? row.payperiodid ?? null,
          perioddtlid: Number(formData.periodid),
          salarytypeid: Number(formData.salarytypeid),
          processtypeid: Number(formData.processtypeid),
        });
        successCount++;
      } catch (error) {
        console.error("Failed to process row", row, error);
        failCount++;
      }
    }

    setLoading(false);
    setToasterMessages([
      `Processing complete: ${successCount} successful, ${failCount} failed.`,
    ]);
    setToasterSeverity(failCount === 0 ? "success" : "error");
    setShowToaster(true);

    if (successCount > 0) {
      handleFetch(); // Refresh grid
    }
  };

  const handleApproval = () => {
    console.log("Starting approval process...");
  };

  const handleClear = () => {
    setRows([]);
    setSelectedRows([]);
    setFormData({
      registerid: "",
      periodid: "",
      salarytypeid: 261,
      deptid: "",
      desgid: "",
      employeeid: "",
      fromdate: dayjs(),
      tilldate: dayjs(),
      processtypeid: 1075,
    });
  };

  const extraToolbar = (
    <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <Typography
          sx={{
            fontSize: "11px",
            fontWeight: 800,
            color: "var(--lms-text-secondary)",
            letterSpacing: "0.05em",
          }}
        >
          EXPORT TYPE :
        </Typography>
        <LmsSelect
          value={exportType}
          options={[
            { label: translate("None"), value: "None" },
            { label: translate("Excel"), value: "Excel" },
            { label: translate("PDF"), value: "PDF" }
          ]}
          onChange={(val) => setExportType(String(val))}
          hideLabel
          sx={{ width: 120 }}
        />
      </Box>

      <Button
        variant="contained"
        startIcon={<ProcessIcon />}
        onClick={handleProcess}
        sx={{
          textTransform: "none",
          fontWeight: 700,
          bgcolor: "var(--lms-primary)",
          transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
          boxShadow: "0 2px 4px rgba(var(--lms-primary-rgb), 0.15)",
          "&:hover": {
            bgcolor: "var(--lms-primary)",
            boxShadow: "0 2px 10px var(--lms-primary)",
          },
          "&:active": {
            transform: "translateY(0)",
          },
          borderRadius: "10px",
          px: 3,
          py: 0.8,
        }}
      >
        <T>Process</T>
      </Button>

      <Button
        variant="outlined"
        startIcon={<ApprovalIcon />}
        onClick={handleApproval}
        sx={{
          textTransform: "none",
          fontWeight: 700,
          borderRadius: "10px",
          px: 3,
          py: 0.8,
          borderColor: "var(--lms-primary)",
          color: "var(--lms-primary)",
          "&:hover": {
            bgcolor: "rgba(var(--lms-primary-rgb), 0.04)",
            borderColor: "var(--lms-primary)",
          },
        }}
      >
        <T>Approval Process</T>
      </Button>

      <Button
        variant="outlined"
        color="error"
        startIcon={<ClearIcon />}
        onClick={handleClear}
        sx={{
          textTransform: "none",
          fontWeight: 700,
          borderRadius: "10px",
          px: 3,
          py: 0.8,
        }}
      >
        <T>Clear</T>
      </Button>
    </Box>
  );

  const columns: Column[] = [
    { id: "sNo", label: "S.No", minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "name", label: "Employee Name", minWidth: 200 },
    { id: "departmentstxt", label: "Department", minWidth: 150 },
    { id: "designationstxt", label: "Designation", minWidth: 150 },
    { id: "days", label: "Total Days", minWidth: 80, align: "center" },
    { id: "present", label: "Working Days", minWidth: 80, align: "center" },
    { id: "absentscount", label: "Absents", minWidth: 80, align: "center" },
    { id: "holidays", label: "Holidays", minWidth: 80, align: "center" },
    { id: "leaves", label: "Leaves", minWidth: 80, align: "center" },
    { id: "salary", label: "Salary", minWidth: 100, align: "right" },
    { id: "allowance", label: "Allowance", minWidth: 100, align: "right" },
    { id: "loanddeduction", label: "Loan Deduction", minWidth: 120, align: "right" },
    { id: "othded", label: "Other Deduction", minWidth: 120, align: "right" },
    { id: "deduction", label: "Deduction", minWidth: 100, align: "right" },
    { id: "tax", label: "Tax", minWidth: 100, align: "right" },
    { id: "arrears", label: "Arears", minWidth: 100, align: "right" },
    { id: "grosssalary", label: "Net Salary", minWidth: 120, align: "right" },
    { id: "remarks", label: "Remarks", minWidth: 150 },
    {
      id: "actions",
      label: "Action",
      minWidth: 80,
      align: "center",
      format: (_: any, row: any) => (
        <RowActionMenu
          row={row}
          canViewPaySlipNewReport={canViewPaySlipNewReport}
          onAction={(action, selectedRow) => {
            console.log("Action triggered:", action, selectedRow);
            if (action === "report-payslip") {
              setViewMode("report");
            } else if (action === "options-approval") {
              // Open single approval logic
            }
          }}
        />
      ),
    },
  ];

  return (
    <Box sx={{ p: 2, height: "100%", display: "flex", flexDirection: "column", gap: 2, position: "relative" }}>
      {loading && <LmsOverlayLoader variant="container" />}

      {viewMode === "report" ? (
        <ReportsPage
          category={paySlipNewReportContext?.category || ""}
          sections={paySlipNewReportContext?.sections || []}
          initialSelectedReport={paySlipNewReportContext?.selectedReport || null}
          embedded
          onBack={() => setViewMode("list")}
        />
      ) : (
        <>
          <Paper
        elevation={0}
        sx={{
          border: "1px solid var(--lms-border)",
          borderRadius: "12px",
          overflow: "hidden",
        }}
      >
        <Box
          sx={{
            px: 2,
            py: 1,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            bgcolor: "#fff",
          }}
        >
          <Typography sx={{ fontWeight: 800, fontSize: "13px", color: "var(--lms-text-main)" }}>
            Filter
          </Typography>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Button
              size="small"
              variant="contained"
              startIcon={<FetchIcon />}
              onClick={handleFetch}
              sx={{ textTransform: "none", fontWeight: 700, fontSize: "11px", borderRadius: "8px" }}
            >
              Fetch
            </Button>
            <IconButton size="small" onClick={() => setShowFilters(!showFilters)}>
              {showFilters ? <KeyboardArrowUp /> : <KeyboardArrowDown />}
            </IconButton>
          </Box>
        </Box>
        <Divider />
        <Collapse in={showFilters}>
          <Box sx={{ p: 2, bgcolor: "#fcfcfc" }}>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Payroll Register*"
                  value={formData.registerid}
                  options={registers}
                  onChange={(val) => {
                    setFormData({ ...formData, registerid: Number(val), periodid: "" });
                    fetchPeriods(Number(val));
                  }}
                  onSearchClick={() => setActiveLookup("register")}
                  required
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Period*"
                  value={formData.periodid}
                  options={periods}
                  getOptionLabel={(o) => o.periodText || o.periodstxt || o.name || ""}
                  onChange={(val) => {
                    const sel = periods.find(p => p.id === Number(val));
                    setFormData({
                      ...formData,
                      periodid: Number(val),
                      fromdate: (sel?.startDate || sel?.startdate) ? dayjs(sel.startDate || sel.startdate) : formData.fromdate,
                      tilldate: (sel?.endDate || sel?.enddate) ? dayjs(sel.endDate || sel.enddate) : formData.tilldate
                    });
                  }}
                  onSearchClick={() => setActiveLookup("period")}
                  disabled={!formData.registerid}
                  required
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Salary Type*"
                  value={formData.salarytypeid}
                  options={salaryTypes}
                  getOptionLabel={(o) => o.name || o.stxt || ""}
                  onChange={(val) => setFormData({ ...formData, salarytypeid: Number(val) })}
                  onSearchClick={() => setActiveLookup("salaryType")}
                  disabled
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Department"
                  value={formData.deptid}
                  options={departments}
                  getOptionLabel={(o) => o.departmentName || o.stxt || ""}
                  onChange={(val) => setFormData({ ...formData, deptid: Number(val) })}
                  onSearchClick={() => setActiveLookup("dept")}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Designation"
                  value={formData.desgid}
                  options={designations}
                  getOptionLabel={(o) => o.designationName || o.stxt || ""}
                  onChange={(val) => setFormData({ ...formData, desgid: Number(val) })}
                  onSearchClick={() => setActiveLookup("desg")}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Employee"
                  value={formData.employeeid}
                  options={employees}
                  getOptionLabel={(o) => o.firstName ? `${o.firstName} ${o.lastName || ""}` : (o.stxt || o.name || "")}
                  onChange={(val) => setFormData({ ...formData, employeeid: Number(val) })}
                  onSearchClick={() => setActiveLookup("employee")}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <FormDatePicker
                  label="Start Date"
                  value={formData.fromdate}
                  onChange={(date) => setFormData({ ...formData, fromdate: date })}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <FormDatePicker
                  label="End Date"
                  value={formData.tilldate}
                  onChange={(date) => setFormData({ ...formData, tilldate: date })}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <LookupDropdown
                  label="Process Type*"
                  value={formData.processtypeid}
                  options={processTypes}
                  getOptionLabel={(o) => o.name || o.stxt || ""}
                  onChange={(val) => setFormData({ ...formData, processtypeid: Number(val) })}
                  onSearchClick={() => setActiveLookup("procType")}
                  disabled
                />
              </Grid>
            </Grid>
          </Box>
        </Collapse>
      </Paper>

      <DashboardTable
        title="Pay Slip"
        columns={columns}
        rows={rows}
        actions={false}
        extraToolbarItemsLeft={canViewPaySlipNewReport ? (
          <Button
            variant="contained"
            startIcon={<ReportIcon />}
            sx={{
              textTransform: "none",
              fontWeight: 700,
              color: "#fff",
              bgcolor: "#0ea5e9",
              borderRadius: "10px",
              px: 3,
              py: 0.8,
              boxShadow: "0 4px 12px rgba(14, 165, 233, 0.2)",
              "&:hover": {
                bgcolor: "#0284c7",
                boxShadow: "0 6px 16px rgba(14, 165, 233, 0.3)",
              },
            }}
            onClick={() => setViewMode("report")}
          >
            <T>Pay Slip New</T>
          </Button>
        ) : null}
        extraToolbarItems={extraToolbar}
        pagination={true}
        checkboxSelection
        onSelectionChange={(selected) => {
          const selectedIdSet = new Set(selected.map((id) => String(id)));
          setSelectedRows(rows.filter((row) => selectedIdSet.has(String(row.rowKey ?? row.id ?? row.sNo ?? row.code))));
        }}
      />

      <GenericSearchModal
        open={!!activeLookup}
        onClose={() => setActiveLookup(null)}
        title={`Select ${activeLookup?.charAt(0).toUpperCase()}${activeLookup?.slice(1)}`}
        data={
          activeLookup === "register" ? registers :
            activeLookup === "period" ? periods :
              activeLookup === "dept" ? departments :
                activeLookup === "desg" ? designations :
                  activeLookup === "employee" ? employees :
                    activeLookup === "salaryType" ? salaryTypes :
                      activeLookup === "procType" ? processTypes : []
        }
        columns={[
          {
            id: activeLookup === "dept" ? "departmentName" :
              activeLookup === "desg" ? "designationName" :
                activeLookup === "employee" ? "firstName" :
                  activeLookup === "period" ? "periodText" :
                    (activeLookup === "salaryType" || activeLookup === "procType") ? "name" : "stxt",
            label: "Name",
            minWidth: 200
          },
          { id: activeLookup === "employee" ? "employeeCode" : "code", label: "Code", minWidth: 100 }
        ]}
        onSelect={(row) => {
          const val = row.id || row.Id;
          if (activeLookup === "register") {
            setFormData({ ...formData, registerid: val, periodid: "" });
            fetchPeriods(val);
          } else if (activeLookup === "period") {
            setFormData({
              ...formData,
              periodid: val,
              fromdate: (row.startDate || row.startdate) ? dayjs(row.startDate || row.startdate) : formData.fromdate,
              tilldate: (row.endDate || row.enddate) ? dayjs(row.endDate || row.enddate) : formData.tilldate
            });
          } else if (activeLookup === "salaryType") {
            setFormData({ ...formData, salarytypeid: val });
          } else if (activeLookup === "procType") {
            setFormData({ ...formData, processtypeid: val });
          } else {
            setFormData({ ...formData, [`${activeLookup}id`]: val });
          }
          setActiveLookup(null);
        }}
      />

      <LmsToaster
        open={showToaster}
        messages={toasterMessages}
        severity={toasterSeverity}
        onClose={() => setShowToaster(false)}
      />
        </>
      )}
    </Box>
  );
};

export default PaySlip;
