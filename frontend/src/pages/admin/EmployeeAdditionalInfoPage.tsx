import Select from "@/components/ClearableSelect";
import React, { useState, useEffect } from "react";
import type { ReactElement, ReactNode } from "react";
import { Box, Typography, Paper, IconButton, Button, FormControl, InputLabel, Menu, MenuItem, ListItemIcon, ListItemText } from "@mui/material";
import {
  NavigateNext as NavigateNextIcon,
  ArrowBack as ArrowBackIcon, Work as EmploymentIcon,
  FormatListBulleted as RoasterIcon,
  Payments as PayrollIcon, AccountBalance as BankIcon, Place as AddressIcon,
  School as EducationIcon, ContactPhone as ContactIcon, Psychology as SkillIcon,
  Star as RatingIcon, History as ExperienceIcon, CreditCard as CardsIcon,
  MedicalServices as DiseasesIcon, EventNote as AttendanceIcon,
  Gavel as DisciplinaryIcon,
  WorkspacePremium as RecognitionIcon,
  Download as DownloadIcon,
  ArrowDropDown as ArrowDropDownIcon,
  PictureAsPdf as PdfIcon,
  TableView as ExcelIcon,
} from "@mui/icons-material";
import { useNavigate, useParams } from "react-router-dom";
import { Layout } from "@/components/Layout";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { StyledTabs } from "@/components/StyledTabs";
import { LookupDropdown } from "@/components/LookupDropdown";
import { EmployeeSearchModal } from "@/components/lookups/EmployeeSearchModal";
import { GetService } from "@/constants/globalItems";
import api from "@/services/api";
import { EmployeeHeaderCard } from "./components/EmployeeHeaderCard";
import { EmployeeContractTab } from "./components/EmployeeContractTab";
import { PositionAssignmentTab } from "./components/PositionAssignmentTab";
import { EmployeeRosterTab } from "./components/EmployeeRosterTab";
import { EmployeePayrollTab } from "./components/EmployeePayrollTab";
import { EmployeeBankAccountTab } from "./components/EmployeeBankAccountTab";
import { EmployeeAddressTab } from "./components/EmployeeAddressTab";
import { EmployeeEducationTab } from "./components/EmployeeEducationTab";
import { EmployeeContactInformationTab } from "./components/EmployeeContactInformationTab";
import { EmployeeSkillTab } from "./components/EmployeeSkillTab";
import { EmployeeSubjectRatingTab } from "./components/EmployeeSubjectRatingTab";
import { EmployeeExperienceTab } from "./components/EmployeeExperienceTab";
import { EmployeeCardDetailsTab } from "./components/EmployeeCardDetailsTab";
import { EmployeeDiseaseTab } from "./components/EmployeeDiseaseTab";
import { EmployeeAttendanceTab } from "./components/EmployeeAttendanceTab";
import {
  EmployeeDevelopmentRecognitionTab,
  EmployeeDisciplinaryLegalTab,
} from "./components/EmployeeInformationRecordsTab";

// API controls section labels/order/visibility; this registry only maps backend codes to implemented UI components.
const STAFF_ADDITIONAL_INFO_FOR_ID = 1041;
// Temporary feature override: true always shows the two employee record tabs.
// Set to false to use only the visibility configured through Additional Registration.
const FORCE_SHOW_EMPLOYEE_RECORD_TABS = false;
const CURRENT_YEAR = new Date().getFullYear();
const ACR_YEAR_OPTIONS = Array.from({ length: 31 }, (_, index) => CURRENT_YEAR - index);

type EmployeeAdditionalTabDefinition = {
  label: string;
  code: string;
  icon: ReactElement;
  render: (employeeId?: string, readOnly?: boolean, onContractSaved?: () => void) => ReactNode;
};

type EmployeeAdditionalTabComponent = {
  icon: ReactElement;
  render: (employeeId?: string, readOnly?: boolean, onContractSaved?: () => void) => ReactNode;
};

const TAB_COMPONENTS_BY_CODE: Record<string, EmployeeAdditionalTabComponent> = {
  "023": { icon: <EmploymentIcon />, render: (employeeId, _readOnly, onContractSaved) => <EmployeeContractTab employeeId={employeeId} onContractSaved={onContractSaved} /> },
  "024": { icon: <EmploymentIcon />, render: (employeeId) => <PositionAssignmentTab employeeId={employeeId} /> },
  "025": { icon: <RoasterIcon />, render: (employeeId) => <EmployeeRosterTab employeeId={employeeId} /> },
  "026": { icon: <PayrollIcon />, render: (employeeId, readOnly) => <EmployeePayrollTab employeeId={employeeId} readOnly={readOnly} /> },
  "027": { icon: <BankIcon />, render: (employeeId, readOnly) => <EmployeeBankAccountTab employeeId={employeeId} readOnly={readOnly} /> },
  "028": { icon: <AddressIcon />, render: (employeeId) => <EmployeeAddressTab employeeId={employeeId} /> },
  "029": { icon: <EducationIcon />, render: (employeeId) => <EmployeeEducationTab employeeId={employeeId} /> },
  "030": { icon: <ContactIcon />, render: (employeeId, _readOnly, onSaved) => <EmployeeContactInformationTab employeeId={employeeId} onSaved={onSaved} /> },
  "032": { icon: <SkillIcon />, render: (employeeId) => <EmployeeSkillTab employeeId={employeeId} /> },
  "033": { icon: <ExperienceIcon />, render: (employeeId) => <EmployeeExperienceTab employeeId={employeeId} /> },
  "034": { icon: <CardsIcon />, render: (employeeId) => <EmployeeCardDetailsTab employeeId={employeeId} /> },
  "035": { icon: <DiseasesIcon />, render: (employeeId) => <EmployeeDiseaseTab employeeId={employeeId} /> },
  "036": { icon: <AttendanceIcon />, render: (employeeId) => <EmployeeAttendanceTab employeeId={employeeId} /> },
  "HRDLR": { icon: <DisciplinaryIcon />, render: (employeeId, readOnly) => <EmployeeDisciplinaryLegalTab employeeId={employeeId} readOnly={readOnly} /> },
  "HRDVR": { icon: <RecognitionIcon />, render: (employeeId, readOnly) => <EmployeeDevelopmentRecognitionTab employeeId={employeeId} readOnly={readOnly} /> },
};

const TAB_COMPONENTS_BY_CONFIG_ID: Record<number, EmployeeAdditionalTabComponent> = {
  32: { icon: <RatingIcon />, render: (employeeId) => <EmployeeSubjectRatingTab employeeId={employeeId} /> },
};

const FORCED_EMPLOYEE_RECORD_TABS: EmployeeAdditionalTabDefinition[] = [
  {
    label: "Disciplinary & Legal Records",
    code: "HRDLR",
    icon: <DisciplinaryIcon />,
    render: (employeeId, readOnly) => (
      <EmployeeDisciplinaryLegalTab employeeId={employeeId} readOnly={readOnly} />
    ),
  },
  {
    label: "Development & Recognition",
    code: "HRDVR",
    icon: <RecognitionIcon />,
    render: (employeeId, readOnly) => (
      <EmployeeDevelopmentRecognitionTab employeeId={employeeId} readOnly={readOnly} />
    ),
  },
];

const addForcedEmployeeRecordTabs = (tabs: EmployeeAdditionalTabDefinition[]) => {
  if (!FORCE_SHOW_EMPLOYEE_RECORD_TABS) return tabs;

  const existingCodes = new Set(tabs.map((tab) => tab.code.toUpperCase()));
  return [
    ...tabs,
    ...FORCED_EMPLOYEE_RECORD_TABS.filter(
      (tab) => !existingCodes.has(tab.code.toUpperCase()),
    ),
  ];
};

const getTabComponent = (row: any) => {
  const configId = Number(row?.configid ?? row?.id ?? 0);
  const code = String(row?.code ?? "").trim();
  return TAB_COMPONENTS_BY_CONFIG_ID[configId] ?? TAB_COMPONENTS_BY_CODE[code];
};

const extractItems = <T,>(response: any): T[] => {
  const data = response?.data ?? response;
  if (Array.isArray(data?.items)) return data.items;
  if (Array.isArray(data?.Items)) return data.Items;
  if (Array.isArray(data)) return data;
  return [];
};

interface EmployeeAdditionalInfoPageProps {
  employeeId?: string;
  embedded?: boolean;
  onBack?: () => void;
  readOnly?: boolean;
}

export const EmployeeAdditionalInfoPage: React.FC<EmployeeAdditionalInfoPageProps> = ({
  employeeId,
  embedded = false,
  onBack,
  readOnly = false,
}) => {
  const navigate = useNavigate();
  const { id: routeEmployeeId } = useParams<{ id: string }>();
  const externalEmployeeId = employeeId ?? routeEmployeeId;
  const [selectedEmployeeId, setSelectedEmployeeId] = useState(externalEmployeeId);
  const id = selectedEmployeeId;
  const { translate } = useTranslation();

  const [activeTab, setActiveTab] = useState(0);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [employeeData, setEmployeeData] = useState<any>(null);
  const [employeeHeaderRefreshKey, setEmployeeHeaderRefreshKey] = useState(0);
  const [employeeOptions, setEmployeeOptions] = useState<any[]>([]);
  const [employeeOptionsLoading, setEmployeeOptionsLoading] = useState(false);
  const [employeeLookupOpen, setEmployeeLookupOpen] = useState(false);
  const [visibleTabs, setVisibleTabs] = useState<EmployeeAdditionalTabDefinition[]>([]);
  const [loading, setLoading] = useState(false);
  const [acrYear, setAcrYear] = useState(CURRENT_YEAR);
  const [acrExportAnchor, setAcrExportAnchor] = useState<HTMLElement | null>(null);
  const [acrExporting, setAcrExporting] = useState(false);
  const [acrExportError, setAcrExportError] = useState<string[]>([]);

  useEffect(() => {
    setSelectedEmployeeId(externalEmployeeId);
  }, [externalEmployeeId]);

  useEffect(() => {
    const selectedCampusId = Number(localStorage.getItem("selectedCampusId"));
    const campusQuery = Number.isFinite(selectedCampusId) && selectedCampusId > 0
      ? `&entityId=${selectedCampusId}`
      : "";

    setEmployeeOptionsLoading(true);
    void GetService(
      `/api/HumanResource/EmployeeProfile/v3?pageNumber=1&pageSize=1000${campusQuery}`,
    )
      .then((response: any) => {
        const employees = extractItems<any>(response)
          .map((row) => {
            const firstName = String(row?.firstName ?? row?.FirstName ?? "").trim();
            const lastName = String(row?.lastName ?? row?.LastName ?? "").trim();
            const employeeName = String(
              row?.employeeName ??
              row?.EmployeeName ??
              `${firstName} ${lastName}`.trim(),
            ).trim();

            return {
              ...row,
              id: Number(row?.id ?? row?.Id ?? row?.employeeId ?? row?.EmployeeId ?? 0),
              employeeCode: String(
                row?.employeeCode ?? row?.EmployeeCode ?? row?.code ?? row?.Code ?? "",
              ).trim(),
              employeeName,
              department: String(
                row?.department ?? row?.Department ??
                row?.departmentName ?? row?.DepartmentName ?? "",
              ).trim(),
              designation: String(
                row?.designation ?? row?.Designation ??
                row?.designationName ?? row?.DesignationName ?? "",
              ).trim(),
              machineCode: String(row?.machineCode ?? row?.MachineCode ?? "").trim(),
              shift: String(
                row?.shift ?? row?.Shift ?? row?.shiftName ?? row?.ShiftName ?? "",
              ).trim(),
            };
          })
          .filter((row) => row.id > 0)
          .sort((left, right) => {
            const leftLabel = `${left.employeeCode} ${left.employeeName}`.trim();
            const rightLabel = `${right.employeeCode} ${right.employeeName}`.trim();
            return leftLabel.localeCompare(rightLabel, undefined, {
              numeric: true,
              sensitivity: "base",
            });
          });

        setEmployeeOptions(employees);
      })
      .catch((error: any) => {
        console.error("Employee options load failed:", error);
        setEmployeeOptions([]);
      })
      .finally(() => setEmployeeOptionsLoading(false));
  }, []);

  // ── Load employee header data ───────────────────────────────────────────────
  useEffect(() => {
    if (!id) {
      setEmployeeData(null);
      return;
    }

    let cancelled = false;
    setEmployeeData(null);
    setLoading(true);
    void GetService(`/api/HumanResource/EmployeeProfile/v3/${id}`)
      .then((r: any) => {
        if (!cancelled) setEmployeeData(r?.data ?? r ?? null);
      })
      .catch((e: any) => console.error("Employee load failed:", e))
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [id, employeeHeaderRefreshKey]);

  useEffect(() => {
    setLoading(true);
    void GetService(`/api/organization/AdditionalRegistration/v3?infoforid=${STAFF_ADDITIONAL_INFO_FOR_ID}`)
      .then((response: any) => {
        const registeredRows = extractItems<any>(response);
        const tabsFromApi = registeredRows
          .filter((row) => row?.visible === true)
          .map((row) => {
            const component = getTabComponent(row);
            if (!component) return null;

            const code = String(row.code ?? "").trim();

            return {
              label: code === "023"
                ? "Employment Contract"
                : String(row.caption || row.stxt || row.code || "").trim(),
              code,
              icon: component.icon,
              render: component.render,
            };
          })
          .filter((tab): tab is EmployeeAdditionalTabDefinition => Boolean(tab?.label));

        setVisibleTabs(addForcedEmployeeRecordTabs(tabsFromApi));
      })
      .catch((error: any) => {
        console.error("Employee additional registration load failed:", error);
        setVisibleTabs(addForcedEmployeeRecordTabs([]));
      })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (activeTab >= visibleTabs.length) {
      setActiveTab(0);
    }
  }, [activeTab, visibleTabs.length]);

  // ── Sidebar tab navigation ─────────────────────────────────────────────────
  const handleTabChange = (_: React.SyntheticEvent, v: number) => {
    setActiveTab(v);
  };

  const handleBack = () => {
    if (embedded && onBack) {
      onBack();
      return;
    }

    navigate(-1);
  };

  const handleEmployeeSelect = (employee: any) => {
    const nextEmployeeId = String(
      employee?.id ?? employee?.Id ?? employee?.employeeId ?? employee?.EmployeeId ?? "",
    ).trim();

    if (!nextEmployeeId || nextEmployeeId === String(id ?? "")) return;

    setSelectedEmployeeId(nextEmployeeId);
    setEmployeeData(null);

    if (!embedded) {
      navigate(`/hr/employee/additional-info/${nextEmployeeId}`, { replace: true });
    }
  };

  const handleAcrExport = async (format: "pdf" | "xlsx") => {
    setAcrExportAnchor(null);
    if (!id) {
      setAcrExportError([translate("Please select an employee before exporting the ACR")]);
      return;
    }

    setAcrExporting(true);
    setAcrExportError([]);
    try {
      const entityId = Number(localStorage.getItem("selectedCampusId"));
      const response = await api.get("/api/report/export/EmployeePerformanceEvaluation", {
        params: {
          EmployeeId: id,
          FromDate: `${acrYear}-01-01`,
          TillDate: `${acrYear}-12-31`,
          format,
          ...(Number.isFinite(entityId) && entityId > 0 ? { EntityId: entityId } : {}),
        },
        responseType: "blob",
      });

      const blob = response.data as Blob;
      if (blob?.type?.includes("json")) {
        const payload = JSON.parse(await blob.text());
        throw new Error(payload?.detail || payload?.message || "Failed to export ACR");
      }

      const employeeCode = String(
        employeeData?.employeeCode ?? employeeData?.EmployeeCode ?? id,
      ).replace(/[^a-zA-Z0-9_-]+/g, "_");
      const downloadUrl = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = downloadUrl;
      link.download = `Employee_ACR_${employeeCode}_${acrYear}.${format}`;
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(downloadUrl);
    } catch (error: any) {
      let message = error?.response?.data?.detail || error?.response?.data?.message || error?.message;
      const errorBlob = error?.response?.data;
      if (errorBlob instanceof Blob) {
        try {
          const payload = JSON.parse(await errorBlob.text());
          message = payload?.detail || payload?.message || message;
        } catch {
          // Keep the request error when the response is not JSON.
        }
      }
      setAcrExportError([translate(String(message || "Failed to export ACR"))]);
    } finally {
      setAcrExporting(false);
    }
  };

  // ── Translated tabs array ──────────────────────────────────────────────────
  const tabs = visibleTabs.map(t => ({ ...t, label: translate(t.label) }));

  const renderActiveTab = () => {
    return visibleTabs[activeTab]?.render(
      id,
      readOnly,
      () => setEmployeeHeaderRefreshKey((key) => key + 1),
    ) ?? null;
  };

  const content = (
      <Box sx={{ flexGrow: 1, mt: 0 }}>
        {(loading || acrExporting) && <LmsOverlayLoader />}
        <Box sx={{ p: 2, pt: 0 }}>
          {/* Employee header card */}
          <EmployeeHeaderCard employeeData={employeeData} refreshKey={employeeHeaderRefreshKey} onPrint={() => window.print()} />

          {/* Main content panel */}
          <Paper elevation={0} sx={{ border: "1px solid var(--lms-border)", borderRadius: 3, bgcolor: "#fff", overflow: "hidden" }}>
            <Box sx={{ display: "flex", alignItems: "stretch" }}>
              {/* Sidebar */}
              <Box sx={{ width: isSidebarCollapsed ? 68 : 260, flexShrink: 0, transition: "width 0.3s cubic-bezier(0.4,0,0.2,1)", borderRight: "1px solid var(--lms-border)", bgcolor: "#fafafa", display: "flex", flexDirection: "column", position: "relative", zIndex: 2 }}>
                <IconButton size="small" onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)} sx={{ position: "absolute", top: 10, right: -14, zIndex: 10, bgcolor: "#fff", border: "1px solid var(--lms-border)", boxShadow: "0 2px 4px rgba(0,0,0,0.05)", "&:hover": { bgcolor: "#f1f1f1" } }}>
                  <NavigateNextIcon fontSize="small" sx={{ transform: isSidebarCollapsed ? "rotate(0deg)" : "rotate(180deg)", transition: "transform 0.3s ease", color: "var(--lms-primary)" }} />
                </IconButton>

                <Box sx={{ display: "flex", flexDirection: "column" }}>
                  {!isSidebarCollapsed && (
                    <Box sx={{ px: 2, pt: 2, pb: 0, flexShrink: 0 }}>
                      <LookupDropdown
                        label={translate("Employee")}
                        value={id ?? ""}
                        options={employeeOptions}
                        getOptionLabel={(option) =>
                          String(option?.employeeName ?? "").trim()
                        }
                        onSelect={handleEmployeeSelect}
                        onSearchClick={() => setEmployeeLookupOpen(true)}
                        openSearchOnFieldClick
                        hideDropdownArrow
                        isLoading={employeeOptionsLoading}
                        placeholder={translate("Select Employee")}
                      />
                    </Box>
                  )}
                  <Box sx={{ p: isSidebarCollapsed ? 1 : 2, pb: 0, display: "flex", justifyContent: "center", flexShrink: 0 }}>
                    <Button fullWidth={!isSidebarCollapsed} variant="outlined" startIcon={!isSidebarCollapsed ? <ArrowBackIcon /> : undefined} onClick={handleBack}
                      sx={{ borderRadius: "10px", textTransform: "none", fontWeight: 700, justifyContent: "center", minWidth: isSidebarCollapsed ? "40px" : undefined, width: isSidebarCollapsed ? "40px" : "100%", height: isSidebarCollapsed ? "40px" : "auto", px: isSidebarCollapsed ? 0 : 2, p: isSidebarCollapsed ? 0 : undefined, color: "var(--lms-primary)", borderColor: "var(--lms-primary)", "&:hover": { borderColor: "var(--lms-primary)", bgcolor: "rgba(var(--lms-primary-rgb), 0.04)" } }}>
                      {isSidebarCollapsed ? <ArrowBackIcon /> : <T>Back to List</T>}
                    </Button>
                  </Box>
                  <Box sx={{ pt: 1 }}>
                    <StyledTabs orientation="vertical" value={activeTab} onChange={handleTabChange} tabs={tabs} isCollapsed={isSidebarCollapsed} sx={{ overflow: "visible", "& .MuiTabs-root": { height: "auto", maxHeight: "none", width: "100%", maxWidth: "100%", boxSizing: "border-box" }, "&& .MuiTabs-scroller": { overflowY: "visible !important", overflowX: "visible !important" }, "& .MuiTabs-flexContainer": { p: 1 }, "& .MuiTab-root": { mb: 0.5, borderRadius: 2 } }} />
                  </Box>
                </Box>
              </Box>

              {/* Tab content */}
              <Box sx={{ flex: 1, minWidth: 0, position: "relative" }}>
                <Box sx={{ p: 2 }}>
                  <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", flexWrap: "wrap", gap: 1.5, mb: 2 }}>
                    <Typography variant="h6" sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}>
                      <T>{visibleTabs[activeTab]?.label ?? "Additional Information"}</T>
                    </Typography>
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                      <FormControl size="small" sx={{ minWidth: 112 }}>
                        <InputLabel id="acr-year-label"><T>ACR Year</T></InputLabel>
                        <Select
                          labelId="acr-year-label"
                          value={acrYear}
                          label={translate("ACR Year")}
                          onChange={(event) => setAcrYear(Number(event.target.value))}
                          disabled={acrExporting}
                        >
                          {ACR_YEAR_OPTIONS.map((year) => (
                            <MenuItem key={year} value={year}>{year}</MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                      <Button
                        variant="contained"
                        startIcon={<DownloadIcon />}
                        endIcon={<ArrowDropDownIcon />}
                        disabled={!id || acrExporting}
                        onClick={(event) => setAcrExportAnchor(event.currentTarget)}
                        sx={{
                          minHeight: 40,
                          borderRadius: "9px",
                          textTransform: "none",
                          fontWeight: 700,
                          bgcolor: "var(--lms-primary)",
                          "&:hover": { bgcolor: "var(--lms-primary-dark, var(--lms-primary-dark))" },
                        }}
                      >
                        <T>Export ACR</T>
                      </Button>
                      <Menu
                        anchorEl={acrExportAnchor}
                        open={Boolean(acrExportAnchor)}
                        onClose={() => setAcrExportAnchor(null)}
                        MenuListProps={{ dense: true }}
                      >
                        <MenuItem onClick={() => void handleAcrExport("pdf")}>
                          <ListItemIcon><PdfIcon fontSize="small" color="error" /></ListItemIcon>
                          <ListItemText primary={translate("Download PDF")} />
                        </MenuItem>
                        <MenuItem onClick={() => void handleAcrExport("xlsx")}>
                          <ListItemIcon><ExcelIcon fontSize="small" color="success" /></ListItemIcon>
                          <ListItemText primary={translate("Download Excel")} />
                        </MenuItem>
                      </Menu>
                    </Box>
                  </Box>
                  <Box sx={{ pointerEvents: readOnly ? "none" : "auto" }}>
                    {visibleTabs.length > 0 ? (
                      <Box key={id}>
                        {renderActiveTab()}
                      </Box>
                    ) : (
                      <Typography sx={{ color: "var(--lms-text-secondary)", fontWeight: 600 }}>
                        <T>No additional information sections are visible</T>
                      </Typography>
                    )}
                  </Box>
                </Box>
              </Box>
            </Box>
          </Paper>
        </Box>

        <EmployeeSearchModal
          open={employeeLookupOpen}
          onClose={() => setEmployeeLookupOpen(false)}
          data={employeeOptions}
          selectedValue={id}
          selectedField="id"
          title={translate("Employee")}
          breadcrumbPath="Human Resource / Employee Additional Information / Employee"
          onSelect={(employee) => {
            handleEmployeeSelect(employee);
            setEmployeeLookupOpen(false);
          }}
        />
        <LmsToaster
          open={acrExportError.length > 0}
          onClose={() => setAcrExportError([])}
          messages={acrExportError}
          severity="error"
          disableDeleteSuccessAutoSeverity={true}
        />
      </Box>
  );

  if (embedded) {
    return content;
  }

  return (
    <Layout>
      {content}
    </Layout>
  );
};
