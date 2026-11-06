import Select from "@/components/ClearableSelect";
import React, { useState, useMemo, useEffect } from "react";
import { Box, Typography, MenuItem } from "@mui/material";
import { DashboardTable } from "./DashboardTable";
import type { Column } from "./DashboardTable";
import { Modal } from "./Modal";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";

interface GenericSearchModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  columns: Column[];
  data: any[];
  onSelect: (row: any) => void;
  variant?: "modern" | "legacy";
  showFooter?: boolean;
  totalRecords?: number;
  breadcrumbPath?: string;
  selectedValue?: string | number;
  selectedValues?: Array<string | number>;
  selectedField?: string;
  showColumnFilters?: boolean;
  showSearchToolbar?: boolean;
  loading?: boolean;
  loadingText?: string;
}

export const GenericSearchModal: React.FC<GenericSearchModalProps> = ({
  open,
  onClose,
  title,
  columns,
  data,
  onSelect,
  variant = "legacy",
  breadcrumbPath,
  selectedValue,
  selectedValues,
  selectedField = "name",
  showColumnFilters = true,
  loading = false,
  loadingText,
  totalRecords,
  showSearchToolbar = true,
}) => {
  const { translate } = useTranslation();
  const [exportType, setExportType] = useState("None");
  const [globalSearch] = useState("");

  useEffect(() => {
    if (!open) return;
    // Reset transient modal state on each open so stale filters do not hide rows.
    setExportType("None");
  }, [open]);
  const sourceData = useMemo(() => (Array.isArray(data) ? data : []), [data]);
  const totalRecordCount = typeof totalRecords === "number" ? totalRecords : sourceData.length;

  // Quick search stays modal-level; column filters are handled by DashboardTable,
  // matching the SchemaTable list experience.
  const normalizeFieldKey = (value: string) =>
    String(value || "").replace(/[^a-zA-Z0-9]/g, "").toLowerCase();

  const getRowValue = (row: any, field: string): any => {
    const exactValue = row?.[field] ?? row?.[field ? `${field[0].toUpperCase()}${field.slice(1)}` : field];
    if (exactValue !== undefined) return exactValue;

    if (field.includes(".")) {
      const nestedValue: any = field
        .split(".")
        .reduce((current: any, part: string): any => {
          if (current === undefined || current === null) return undefined;
          return getRowValue(current, part);
        }, row);
      if (nestedValue !== undefined) return nestedValue;
    }

    const normalizedField = normalizeFieldKey(field);
    const matchingKey = Object.keys(row || {}).find(
      (key) => normalizeFieldKey(key) === normalizedField,
    );

    return matchingKey ? row[matchingKey] : undefined;
  };

  const filteredData = useMemo(() => {
    if (!sourceData.length) return [];
    const searchLow = variant === "modern" ? globalSearch.trim().toLowerCase() : "";
    if (!searchLow) return sourceData;
    return sourceData.filter((row) => {
      if (!row) return false;
      return columns.some((col) => {
        const val = getRowValue(row, col.id) ?? "";
        return String(val).toLowerCase().includes(searchLow);
      });
    });
  }, [sourceData, globalSearch, columns, variant]);

  const selectedRowId = useMemo(() => {
    if (
      selectedValue === undefined ||
      selectedValue === null ||
      selectedValue === ""
    )
      return undefined;
    const normalizedSelected = String(selectedValue).toLowerCase();
    const altField =
      selectedField && selectedField.length > 0
        ? `${selectedField[0].toUpperCase()}${selectedField.slice(1)}`
        : selectedField;

    const matchedRow = Array.isArray(data) ? data.find((row) => {
      const raw = getRowValue(row, selectedField) ?? (altField ? getRowValue(row, altField) : undefined);
      if (raw === undefined || raw === null || raw === "") return false;

      const rawNumber = Number(raw);
      const selectedNumber = Number(selectedValue);
      if (!Number.isNaN(rawNumber) && !Number.isNaN(selectedNumber)) {
        return rawNumber === selectedNumber;
      }

      return String(raw).toLowerCase() === normalizedSelected;
    }) : undefined;
    if (!matchedRow) return undefined;
    const id = matchedRow.id ?? matchedRow.Id;
    const sNo = matchedRow.sNo ?? matchedRow.SNo;
    const code = matchedRow.code ?? matchedRow.Code;
    return id ?? sNo ?? code;
  }, [data, selectedField, selectedValue]);
  const selectedRowIds = useMemo(() => {
    if (!Array.isArray(selectedValues) || selectedValues.length === 0) return undefined;
    const normalizedSelected = new Set(selectedValues.map((value) => String(value).toLowerCase()));
    const altField =
      selectedField && selectedField.length > 0
        ? `${selectedField[0].toUpperCase()}${selectedField.slice(1)}`
        : selectedField;

    return (Array.isArray(data) ? data : [])
      .filter((row) => {
        const raw = getRowValue(row, selectedField) ?? (altField ? getRowValue(row, altField) : undefined);
        if (raw === undefined || raw === null || raw === "") return false;
        return normalizedSelected.has(String(raw).toLowerCase());
      })
      .map((row) => row.id ?? row.Id ?? row.sNo ?? row.SNo ?? row.code ?? row.Code)
      .filter((value) => value !== undefined && value !== null);
  }, [data, selectedField, selectedValues]);

  const exportTypeControl = (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
      <Typography
        variant="caption"
        sx={{
          fontWeight: 800,
          color: "var(--lms-text-secondary)",
          fontSize: "0.7rem",
          textTransform: "uppercase",
          letterSpacing: "0.5px",
          whiteSpace: "nowrap",
        }}
      >
        <T>Export Type :</T>
      </Typography>
      <Select
        size="small"
        value={exportType}
        onChange={(e) => setExportType(e.target.value)}
        sx={{
          height: 32,
          fontSize: "0.75rem",
          fontWeight: 700,
          minWidth: 110,
          bgcolor: "rgba(0,0,0,0.02)",
          borderRadius: "8px",
          "& .MuiOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-border)",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-border)",
          },
        }}
      >
        <MenuItem value="None" sx={{ fontSize: "0.75rem", fontWeight: 600 }}>
          <T>None</T>
        </MenuItem>
        <MenuItem value="Excel" sx={{ fontSize: "0.75rem", fontWeight: 600 }}>
          <T>Excel</T>
        </MenuItem>
        <MenuItem value="PDF" sx={{ fontSize: "0.75rem", fontWeight: 600 }}>
          <T>PDF</T>
        </MenuItem>
      </Select>
    </Box>
  );

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={title}
      breadcrumbPath={breadcrumbPath}
      maxWidth="xl"
      showActions={false}
      contentPadding={0}
      disableContentScroll
      paperSx={{
        // Lookup tables frequently have several descriptive columns. Give the
        // shared search dialog workspace width instead of constraining it to
        // the default medium modal breakpoint.
        width: "min(96vw, 1600px)",
        maxWidth: "none",
      }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          position: "relative",
          // Keep the dialog fixed. DashboardTable is the only vertical scroller.
          height: "min(760px, calc(92vh - 120px))",
          minHeight: 0,
          overflow: "hidden",
        }}
      >
        {loading && (
          <LmsOverlayLoader
            variant="container"
            zIndex={2}
            message={loadingText ?? translate("Loading...")}
          />
        )}

        {/* Unified Table Content */}
        <Box
          sx={{
            flex: 1,
            bgcolor: "#fbfbfb",
            p: { xs: 1.5, sm: 2 },
            overflow: "hidden",
            minHeight: 0,
            "&::-webkit-scrollbar": { width: "6px" },
            "&::-webkit-scrollbar-thumb": {
              bgcolor: "rgba(var(--lms-primary-rgb), 0.3)",
              borderRadius: "10px",
            },
            "&::-webkit-scrollbar-track": { bgcolor: "transparent" },
          }}
        >
          <DashboardTable
            key={open ? `search-open-${title}` : `search-closed-${title}`}
            columns={columns}
            rows={filteredData}
            onRowClick={onSelect}
            actions={false}
            maxHeight="calc(87vh - 240px)"
            rowHoverBg="rgba(var(--lms-primary-rgb), 0.24)"
            selectedRowId={selectedRowId}
            selectedRowIds={selectedRowIds}
            filtersEnabled={showColumnFilters}
            downloadEnabled={false}
            extraToolbarItemsRight={
              showSearchToolbar ? exportTypeControl : undefined
            }
          />
        </Box>
        <Box
          sx={{
            px: 2,
            py: 1,
            bgcolor: "#fff",
            borderTop: "1px solid var(--lms-border)",
          }}
        >
          <Typography
            variant="caption"
            sx={{ fontWeight: 800, color: "var(--lms-text-secondary)" }}
          >
            <T>TOTAL RECORDS :</T> {totalRecordCount}
          </Typography>
        </Box>
      </Box>
    </Modal>
  );
};




