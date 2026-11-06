import React, { useEffect, useState } from "react";
import { Box, Grid, Button, Typography, Collapse, IconButton } from "@mui/material";
import {
  Settings as SettingsIcon,
  ClearAll as ClearFilterIcon,
  Add as AddIcon,
  Remove as RemoveIcon,
  Assessment as AssessmentIcon,
} from "@mui/icons-material";
import { Modal } from "@/components/Modal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { FastReportViewer } from "@/components/reporting/FastReportViewer";
import { LookupDropdown } from "@/components/LookupDropdown";

interface LeaveRequestReportModalProps {
  open: boolean;
  onClose: () => void;
  initialFilters?: {
    entityId?: number | null;
    entityName?: string;
    employeeId?: number | null;
    employeeCode?: string;
    genderId?: number | null;
    genderName?: string;
    designationId?: number | null;
    designationName?: string;
  };
  lookups?: {
    employees: any[];
    genders: any[];
    designations: any[];
    leaveTypes: any[];
    leaveUnits: any[];
    statuses: any[];
    entities: any[];
    requestNos: any[];
  };
  isFullScreenView?: boolean;
}

export const LeaveRequestReportModal: React.FC<LeaveRequestReportModalProps> = ({
  open,
  onClose,
  initialFilters,
  lookups,
  isFullScreenView,
}) => {
  const { translate } = useTranslation();
  const [criteriaExpanded, setCriteriaExpanded] = useState(true);
  const [viewerExpanded, setViewerExpanded] = useState(false);
  const [reportParams, setReportParams] = useState<any>(null);
  const [filters, setFilters] = useState({
    entityId: null as number | null,
    entityName: "",
    employeeId: null as number | null,
    employeeCode: "",
    genderId: null as number | null,
    genderName: "",
    designationId: null as number | null,
    designationName: "",
    institutionName: "",
    campusName: "",
    address: "",
    city: "",
    country: "",
    imageId: null as any,
  });

  useEffect(() => {
    if (open && lookups?.entities?.length && !filters.entityId) {
      const entity = lookups.entities[0];
      setFilters((prev) => ({
        ...prev,
        entityId: entity.id,
        entityName: entity.instituteName || entity.name || entity.stxt || "",
        institutionName: entity.shortName || entity.instituteName || entity.name || entity.stxt || "",
        campusName: entity.instituteName || entity.name || entity.stxt || "",
        address: entity.address || "",
        city: entity.city || "",
        country: entity.country || "",
        imageId: entity.imageId || null,
      }));
    }
  }, [open, lookups, filters.entityId]);

  useEffect(() => {
    if (!open || !initialFilters) return;
    setFilters((prev) => ({
      ...prev,
      entityId: initialFilters.entityId ?? prev.entityId,
      entityName: initialFilters.entityName ?? prev.entityName,
      employeeId: initialFilters.employeeId ?? prev.employeeId,
      employeeCode: initialFilters.employeeCode ?? prev.employeeCode,
      genderId: initialFilters.genderId ?? prev.genderId,
      genderName: initialFilters.genderName ?? prev.genderName,
      designationId: initialFilters.designationId ?? prev.designationId,
      designationName: initialFilters.designationName ?? prev.designationName,
    }));
  }, [open, initialFilters]);

  const handleGenerate = () => {
    setReportParams({
      EntityId: filters.entityId || 0,
      EmployeeId: filters.employeeId || 0,
      EmployeeCode: filters.employeeCode || "",
      GenderId: filters.genderId || 0,
      DesignationId: filters.designationId || 0,
      InstitutionName: filters.institutionName,
      CampusName: filters.campusName,
      Address: filters.address,
      City: filters.city,
      Country: filters.country,
      LogoId: filters.imageId,
      ReportName: "LeaveRequestSummary",
    });
    setCriteriaExpanded(false);
    setViewerExpanded(true);
  };

  const handleClear = () => {
    setFilters((prev) => ({
      ...prev,
      employeeId: null,
      employeeCode: "",
      genderId: null,
      genderName: "",
      designationId: null,
      designationName: "",
    }));
    setReportParams(null);
    setViewerExpanded(false);
    setCriteriaExpanded(true);
  };

  const renderLookupRow = (
    label: string,
    value: string,
    placeholder: string,
    options: any[],
    getOptionLabel: (option: any) => string,
    onSelect: (row: any) => void,
  ) => (
    <Grid item xs={12} sx={{ display: "flex", alignItems: "center" }}>
      <Typography sx={{ width: "180px", fontWeight: 600, color: "#475569", fontSize: "0.9rem" }}>
        <T>{label}</T>
      </Typography>
      <LookupDropdown
        hideLabel
        value={value}
        placeholder={placeholder}
        options={options}
        getOptionLabel={getOptionLabel}
        onSelect={onSelect}
        onSearchClick={() => {}}
        hideDropdownArrow={true}
        sx={{
          flex: 1,
          "& .MuiOutlinedInput-root": {
            bgcolor: "#f8fafc",
            borderRadius: "10px",
            "& fieldset": { borderColor: "#e2e8f0" },
            "&:hover fieldset": { borderColor: "#cbd5e1" },
          },
        }}
      />
    </Grid>
  );

  const renderContent = () => (
    <Box sx={{ minHeight: "500px" }}>
      <Box sx={{ display: "flex", gap: 2, mb: 2, p: 1.5, bgcolor: "#fff", borderBottom: "1px solid var(--lms-border)", alignItems: "center" }}>
        <Button
          variant="contained"
          onClick={handleGenerate}
          startIcon={<SettingsIcon />}
          sx={{
            textTransform: "none",
            fontWeight: 700,
            bgcolor: "var(--lms-primary)",
            borderRadius: "8px",
            px: 3,
            "&:hover": { bgcolor: "var(--lms-primary-dark)" },
          }}
        >
          <T>Generate</T>
        </Button>
        <Button
          onClick={handleClear}
          startIcon={<ClearFilterIcon />}
          sx={{ textTransform: "none", fontWeight: 700, color: "#64748b" }}
        >
          <T>Clear Filter</T>
        </Button>
      </Box>

      <Box sx={{ border: "1px solid var(--lms-border)", borderRadius: "12px", overflow: "hidden", bgcolor: "#fff" }}>
        <Box
          onClick={() => setCriteriaExpanded(!criteriaExpanded)}
          sx={{ p: 1.5, bgcolor: "#f8fafc", borderBottom: criteriaExpanded ? "1px solid var(--lms-border)" : "none", display: "flex", justifyContent: "space-between", alignItems: "center", cursor: "pointer", "&:hover": { bgcolor: "#f1f5f9" } }}
        >
          <Typography sx={{ fontSize: "0.9rem", fontWeight: 800, color: "#334155" }}><T>Report Criteria</T></Typography>
          <IconButton size="small">{criteriaExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}</IconButton>
        </Box>

        <Collapse in={criteriaExpanded}>
          <Box sx={{ p: 4 }}>
            <Grid container spacing={3} sx={{ maxWidth: "850px" }}>
              {renderLookupRow(
                "School",
                filters.entityName,
                translate("Select School"),
                lookups?.entities || [],
                (option) => option.instituteName || option.name || option.stxt || "",
                (row: any) =>
                  setFilters((prev) => ({
                    ...prev,
                    entityId: row.id,
                    entityName: row.instituteName || row.name || row.stxt || "",
                    institutionName: row.shortName || row.instituteName || row.name || row.stxt || "",
                    campusName: row.instituteName || row.name || row.stxt || "",
                    address: row.address || "",
                    city: row.city || "",
                    country: row.country || "",
                    imageId: row.imageId || null,
                  }))
              )}
              {renderLookupRow(
                "Employee ID",
                filters.employeeCode,
                translate("Select Employee ID"),
                lookups?.employees || [],
                (option) => option.employeeCode || option.name || "",
                (row: any) =>
                  setFilters((prev) => ({
                    ...prev,
                    employeeId: row.id,
                    employeeCode: row.employeeCode || "",
                  }))
              )}
              {renderLookupRow(
                "Gender",
                filters.genderName,
                translate("Select Gender"),
                lookups?.genders || [],
                (option) => option.stxt || option.name || "",
                (row: any) =>
                  setFilters((prev) => ({
                    ...prev,
                    genderId: row.id ?? null,
                    genderName: row.stxt || row.name || "",
                  }))
              )}
              {renderLookupRow(
                "Designation",
                filters.designationName,
                translate("Select Designation"),
                lookups?.designations || [],
                (option) => option.stxt || option.name || "",
                (row: any) =>
                  setFilters((prev) => ({
                    ...prev,
                    designationId: row.id ?? null,
                    designationName: row.stxt || row.name || "",
                  }))
              )}
            </Grid>
          </Box>
        </Collapse>

        <Box
          onClick={() => setViewerExpanded(!viewerExpanded)}
          sx={{ p: 1.5, bgcolor: "#f8fafc", borderTop: "1px solid var(--lms-border)", borderBottom: viewerExpanded ? "1px solid var(--lms-border)" : "none", display: "flex", justifyContent: "space-between", alignItems: "center", cursor: "pointer", "&:hover": { bgcolor: "#f1f5f9" } }}
        >
          <Typography sx={{ fontSize: "0.9rem", fontWeight: 800, color: "#334155" }}><T>Report Viewer</T></Typography>
          <IconButton size="small">{viewerExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}</IconButton>
        </Box>

        <Collapse in={viewerExpanded}>
          <Box sx={{ height: "700px", bgcolor: "#f1f5f9" }}>
            {reportParams ? (
              <FastReportViewer reportName="LeaveRequestSummary" parameters={reportParams} />
            ) : (
              <Box sx={{ height: "100%", display: "flex", alignItems: "center", justifyContent: "center", color: "#94a3b8", flexDirection: "column", gap: 2 }}>
                <AssessmentIcon sx={{ fontSize: "4rem", opacity: 0.2 }} />
                <Typography sx={{ fontWeight: 600 }}><T>Click Generate to preview report</T></Typography>
              </Box>
            )}
          </Box>
        </Collapse>
      </Box>
    </Box>
  );

  if (isFullScreenView) return renderContent();

  return (
    <Modal open={open} onClose={onClose} title={translate("Leave Request Report")} maxWidth="lg" onSave={handleGenerate} onReset={handleClear} saveLabel={translate("Generate")}>
      {renderContent()}
    </Modal>
  );
};
