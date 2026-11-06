import { FormInput } from "@/components/FormInput";
import { EmptyState } from "@/components/EmptyState";
import { LmsSelect } from "@/components/LmsSelect";
import React, { useEffect, useRef, useState } from "react";
import {
  Paper,
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Button,
  Tooltip,
} from "@mui/material";
import {
  EditOutlined as EditIcon,
  DeleteOutline as DeleteIcon,
  Menu as MenuIcon,
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  FirstPage as FirstPageIcon,
  LastPage as LastPageIcon,
  GetApp as DownloadIcon,
  FilterList as FilterIcon,
  Add as AddIcon,
  Visibility as ViewIcon,
} from "@mui/icons-material";
import { AnimatePresence, motion } from "framer-motion";
import { T, useTranslation } from "./AdaptiveTranslationProvider";
import { LmsCheckbox } from "./LmsCheckbox";
import { useGridPageSize, GRID_PAGING_OPTIONS } from "@/hooks/useGridPageSize";
import { useHiddenMenuPermissions } from "@/hooks/useHiddenMenuPermissions";
import { useLocation } from 'react-router-dom';
import { canOpenGridRowOnDoubleClick } from '@/utils/screenActionRights';
import { getGridRowPermissionMenuId } from '@/utils/gridRowAccess';

export interface Column {
  id: string;
  label: string;
  headerFormat?: React.ReactNode;
  minWidth?: number;
  align?: "right" | "left" | "center";
  type?: "status" | "text" | "number";
  /** Keeps a column fixed beside the right-side Actions column. */
  sticky?: "right";
  format?: (value: any, row?: any, index?: number) => React.ReactNode;
  filterable?: boolean;
}

const reactNodeToText = (node: React.ReactNode): string => {
  if (node == null || typeof node === "boolean") return "";
  if (typeof node === "string" || typeof node === "number") return String(node);
  if (Array.isArray(node)) return node.map(reactNodeToText).join(" ");
  if (React.isValidElement<{ children?: React.ReactNode }>(node)) {
    return reactNodeToText(node.props.children);
  }
  return "";
};

interface DashboardTableProps {
  title?: string;
  addButtonLabel?: string;
  columns: Column[];
  rows: any[];
  actions?: boolean;
  maxHeight?: number | string;
  stickyHeader?: boolean;
  pagination?: boolean;
  filtersEnabled?: boolean;
  // Opt-in: report column filters so a server-paginated caller can search the whole
  // dataset instead of only the page it has loaded. Without it the grid keeps its
  // existing client-side filtering.
  onFiltersChange?: (filters: Record<string, string>) => void;
  // Opt-in: on a server-paginated grid the header checkbox can only see the page it
  // has loaded. A caller that can fetch every matching id supplies it here, so
  // "select all" means the whole result set rather than the current ten rows.
  onSelectAllRequest?: () => Promise<any[]>;
  downloadEnabled?: boolean;
  onRowClick?: (row: any) => void;
  onRowDoubleClick?: (row: any) => void;
  onAdd?: () => void;
  onEdit?: (row: any) => void;
  onDelete?: (row: any) => void;
  onView?: (row: any) => void;
  onDownload?: () => void;
  disableAddAction?: boolean;
  disableViewAction?: boolean;
  disableEditAction?: boolean;
  disableDeleteAction?: boolean;
  permissionMenuId?: number;
  permissionScreenName?: string;
  permissionParentName?: string;
  useActionRights?: boolean;
  extraToolbarItemsLeft?: React.ReactNode;
  extraToolbarItems?: React.ReactNode;
  extraToolbarItemsRight?: React.ReactNode;
  actionsPosition?: "left" | "right";
  checkboxSelection?: boolean;
  showMenuAction?: boolean;
  showDeleteAction?: boolean;
  onMenuAction?: (row: any, event?: React.MouseEvent<HTMLElement>) => void;
  rowHoverBg?: string;
  selectedRowId?: any;
  selectedRowIds?: any[];
  resizableColumns?: boolean;
  onSelectionChange?: (selectedIds: any[]) => void;
  selectedIds?: any[];
  disableRowSelectionOnClick?: boolean;
  readOnly?: boolean;
  compactLegacy?: boolean;
  hideStandardActions?: (row: any) => boolean;
  /** Hides only the edit and delete actions for a row (view and double-click stay), e.g. system generated vouchers. */
  hideEditDeleteActions?: (row: any) => boolean;
  extraActions?: (row: any) => React.ReactNode;
  currentPage?: number;
  rowsPerPage?: number;
  totalCount?: number;
  onPageChange?: (page: number) => void;
  onRowsPerPageChange?: (rowsPerPage: number) => void;
  rowsPerPageOptions?: number[];
  disableHorizontalScroll?: boolean;
  actionColumnWidth?: number;
  toolbarBorder?: string;
  translateLabels?: boolean;
  hideToolbar?: boolean;
}

export const DashboardTable: React.FC<DashboardTableProps> = ({
  title,
  addButtonLabel,
  columns,
  rows,
  actions = true,
  stickyHeader = true,
  pagination = true,
  filtersEnabled = true,
  onFiltersChange,
  onSelectAllRequest,
  downloadEnabled = true,
  onRowClick,
  onRowDoubleClick,
  onAdd,
  onEdit,
  onDelete,
  onView,
  onDownload,
  disableAddAction = false,
  disableViewAction = false,
  disableEditAction = false,
  disableDeleteAction = false,
  permissionMenuId,
  permissionScreenName,
  permissionParentName,
  useActionRights = true,
  extraToolbarItemsLeft,
  extraToolbarItems,
  extraToolbarItemsRight,
  actionsPosition = "right",
  checkboxSelection = false,
  showMenuAction = false,
  showDeleteAction = true,
  onMenuAction,
  rowHoverBg = "rgba(0,0,0,0.01)",
  selectedRowId,
  selectedRowIds,
  resizableColumns = false,
  onSelectionChange,
  selectedIds,
  disableRowSelectionOnClick = false,
  maxHeight,
  readOnly = false,
  compactLegacy = false,
  extraActions,
  hideStandardActions,
  hideEditDeleteActions,
  currentPage,
  rowsPerPage: controlledRowsPerPage,
  totalCount,
  onPageChange,
  onRowsPerPageChange,
  rowsPerPageOptions = GRID_PAGING_OPTIONS,
  disableHorizontalScroll = false,
  actionColumnWidth: actionColumnWidthProp,
  toolbarBorder,
  translateLabels = true,
  hideToolbar = false,
}) => {
  const [internalPage, setInternalPage] = useState(1);
  const [internalRowsPerPage, setInternalRowsPerPage] = useGridPageSize(() => setInternalPage(1));
  const [filterVisible, setFilterVisible] = useState(filtersEnabled);
  const [filters, setFilters] = useState<Record<string, string>>({});
  const [selected, setSelected] = useState<any[]>([]);
  const [columnWidths, setColumnWidths] = useState<Record<string, number>>({});
  const resizingRef = useRef<{
    columnId: string;
    startX: number;
    startWidth: number;
  } | null>(null);
  const { translate } = useTranslation();
  const { screenPermissionsLoaded, hasScreenActionRight, hasScreenActionRightForParent, hasScreenActionRightForMenuId, getScreenMenuItem, visibleMenuCardGroups } = useHiddenMenuPermissions();
  const location = useLocation();
  const effectivePermissionScreenName = permissionScreenName ?? title ?? "";
  const shouldUseActionRights = useActionRights && (permissionMenuId != null || Boolean(effectivePermissionScreenName));
  const hasEffectiveActionRight = (actionRight: string) =>
    permissionMenuId != null
      ? hasScreenActionRightForMenuId(permissionMenuId, actionRight)
      : permissionParentName
      ? hasScreenActionRightForParent(effectivePermissionScreenName, permissionParentName, actionRight)
      : hasScreenActionRight(effectivePermissionScreenName, actionRight);
  const effectiveDisableAddAction = disableAddAction || (
    shouldUseActionRights &&
    (!screenPermissionsLoaded || !hasEffectiveActionRight("create"))
  );
  const effectiveDisableViewAction = disableViewAction || (
    shouldUseActionRights &&
    (!screenPermissionsLoaded || !hasEffectiveActionRight("view"))
  );
  const effectiveDisableEditAction = disableEditAction || (
    shouldUseActionRights &&
    (!screenPermissionsLoaded || !hasEffectiveActionRight("update"))
  );
  const effectiveDisableDeleteAction = disableDeleteAction || (
    shouldUseActionRights &&
    (!screenPermissionsLoaded || !hasEffectiveActionRight("delete"))
  );
  const routeScreenName = (() => {
    const parts = location.pathname.split('/').filter(Boolean);
    if (parts.length < 2) return '';
    try { return decodeURIComponent(parts[parts.length - 1]); } catch { return parts[parts.length - 1]; }
  })();
  const rowPermissionName = permissionScreenName || routeScreenName || title || '';
  const scopedRowMenuId = getGridRowPermissionMenuId(rowPermissionName, location.pathname, permissionParentName, visibleMenuCardGroups);
  const rowPermissionMenuId = permissionMenuId ?? (scopedRowMenuId === undefined ? getScreenMenuItem(rowPermissionName, permissionParentName)?.menuId : scopedRowMenuId);
  const hasRowActionRight = (right: string) => rowPermissionMenuId === null ? false : rowPermissionMenuId != null
    ? hasScreenActionRightForMenuId(rowPermissionMenuId, right)
    : permissionParentName ? hasScreenActionRightForParent(rowPermissionName, permissionParentName, right)
    : hasScreenActionRight(rowPermissionName, right);
  const rowDoubleClickHandler = onRowDoubleClick || (
    !effectiveDisableEditAction && hasRowActionRight('update') && onEdit
      ? onEdit
      : onView
  );
  const canOpenRowOnDoubleClick = Boolean(rowDoubleClickHandler) && canOpenGridRowOnDoubleClick({
    permissionsLoaded: screenPermissionsLoaded,
    canCreate: hasRowActionRight('create'),
    canUpdate: hasRowActionRight('update'),
    readOnly,
  });
  const page = currentPage ?? internalPage;
  const rowsPerPage = controlledRowsPerPage ?? internalRowsPerPage;

  useEffect(() => {
    if (!filtersEnabled) {
      setFilterVisible(false);
      setFilters({});
    } else {
      setFilterVisible(true);
    }
  }, [filtersEnabled]);

  useEffect(() => {
    if (!Array.isArray(selectedIds)) return;
    setSelected(selectedIds);
  }, [selectedIds]);

  useEffect(() => {
    setColumnWidths((prev) => {
      const next: Record<string, number> = {};
      columns.forEach((column) => {
        if (typeof prev[column.id] === "number") {
          next[column.id] = prev[column.id];
        } else if (typeof column.minWidth === "number") {
          next[column.id] = column.minWidth;
        }
      });
      return next;
    });
  }, [columns]);

  useEffect(() => {
    const handleMouseMove = (event: MouseEvent) => {
      if (!resizingRef.current) return;
      const { columnId, startX, startWidth } = resizingRef.current;
      const delta = event.clientX - startX;
      const nextWidth = Math.max(80, startWidth + delta);
      setColumnWidths((prev) => ({ ...prev, [columnId]: nextWidth }));
    };

    const handleMouseUp = () => {
      if (!resizingRef.current) return;
      resizingRef.current = null;
      document.body.style.userSelect = "";
      document.body.style.cursor = "";
    };

    window.addEventListener("mousemove", handleMouseMove);
    window.addEventListener("mouseup", handleMouseUp);
    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
      window.removeEventListener("mouseup", handleMouseUp);
    };
  }, []);

  const normalizeFieldKey = (value: string) =>
    String(value || "").replace(/[^a-zA-Z0-9]/g, "").toLowerCase();

  const getCellRawValue = (row: any, columnId: string): any => {
    const pascalColumnId = columnId
      ? `${columnId[0].toUpperCase()}${columnId.slice(1)}`
      : columnId;
    const exactValue = row[columnId] ?? row[pascalColumnId];
    if (exactValue !== undefined) return exactValue;

    if (columnId.includes(".")) {
      const nestedValue: any = columnId
        .split(".")
        .reduce((current: any, part: string): any => {
          if (current === undefined || current === null) return undefined;
          return getCellRawValue(current, part);
        }, row);
      if (nestedValue !== undefined) return nestedValue;
    }

    const normalizedColumnId = normalizeFieldKey(columnId);
    const matchingKey = Object.keys(row || {}).find(
      (key) => normalizeFieldKey(key) === normalizedColumnId,
    );

    return matchingKey ? row[matchingKey] : undefined;
  };

  const getCellFilterText = (row: any, column: Column) => {
    const rawValue = getCellRawValue(row, column.id);
    const formattedValue = column.format
      ? column.format(rawValue, row)
      : rawValue;
    const formattedText = reactNodeToText(formattedValue);
    const rawText = rawValue == null ? "" : String(rawValue);

    return `${rawText} ${formattedText}`.trim().toLowerCase();
  };

  const getRowId = (row: any) =>
    row.rowKey ?? row.id ?? row.sNo ?? row.code ?? JSON.stringify(row);

  // Filtering logic
  // With onFiltersChange the caller refetches filtered rows from the server, so the
  // rows already reflect the filters; re-applying them here would match against the
  // one page held in the browser and hide valid server results.
  const filteredRows = !filtersEnabled || onFiltersChange
    ? (Array.isArray(rows) ? rows : [])
    : (Array.isArray(rows) ? rows : []).filter((row) => {
        const matchesFilters = Object.keys(filters).every((key) => {
          if (!filters[key]) return true;
          const column = columns.find((item) => item.id === key);
          if (!column) return true;
          return getCellFilterText(row, column).includes(filters[key].toLowerCase());
        });
        return matchesFilters;
      });

  const hasActiveColumnFilters = filtersEnabled && !onFiltersChange && Object.values(filters).some((value) => value.trim() !== "");
  const isServerPaginated = typeof totalCount === "number" && (!!onPageChange || !!onRowsPerPageChange);
  const effectiveTotalCount =
    isServerPaginated && hasActiveColumnFilters
      ? filteredRows.length
      : typeof totalCount === "number"
        ? totalCount
        : filteredRows.length;
  const totalPages = Math.max(1, Math.ceil(effectiveTotalCount / rowsPerPage));
  const recordRangeStart = effectiveTotalCount === 0 ? 0 : (page - 1) * rowsPerPage + 1;
  const recordRangeEnd = effectiveTotalCount === 0
    ? 0
    : Math.min(page * rowsPerPage, effectiveTotalCount);

  useEffect(() => {
    if (page <= totalPages) return;
    if (onPageChange) onPageChange(totalPages);
    else setInternalPage(totalPages);
  }, [onPageChange, page, totalPages]);

  const displayRows = pagination
    ? isServerPaginated
      ? hasActiveColumnFilters
        ? filteredRows
        : (Array.isArray(rows) ? rows : [])
      : filteredRows.slice(
          (page - 1) * rowsPerPage,
          (page - 1) * rowsPerPage + rowsPerPage,
        )
    : filteredRows;
  const selectableRows =
    isServerPaginated && !hasActiveColumnFilters ? displayRows : filteredRows;
  const selectableRowIds = selectableRows.map((row) => getRowId(row));
  const selectedSelectableCount = selectableRowIds.filter((id) =>
    selected.includes(id),
  ).length;

  const changePage = (nextPage: number) => {
    const safePage = Math.min(Math.max(1, nextPage), totalPages);
    if (onPageChange) onPageChange(safePage);
    else setInternalPage(safePage);
  };

  const changeRowsPerPage = (nextRowsPerPage: number) => {
    if (onRowsPerPageChange) onRowsPerPageChange(nextRowsPerPage);
    else setInternalRowsPerPage(nextRowsPerPage);
    if (onPageChange) onPageChange(1);
    else setInternalPage(1);
  };

  const handleFilterChange = (columnId: string, value: string) => {
    setFilters((prev) => {
      const next = { ...prev, [columnId]: value };
      onFiltersChange?.(next);
      return next;
    });
    changePage(1);
  };

  const handleSelectAllClick = async (checked: boolean) => {
    if (readOnly) return;
    if (checked) {
      if (onSelectAllRequest) {
        // Merge rather than replace: ids already ticked on another page must survive.
        const allIds = await onSelectAllRequest();
        const merged = Array.from(new Set([...selected, ...allIds]));
        setSelected(merged);
        onSelectionChange?.(merged);
        return;
      }
      const newSelecteds = Array.from(new Set([...selected, ...selectableRowIds]));
      setSelected(newSelecteds);
      onSelectionChange?.(newSelecteds);
      return;
    }
    // Unticking the header clears the whole selection when it selected the whole
    // result set, otherwise only the rows this page can see.
    if (onSelectAllRequest) {
      setSelected([]);
      onSelectionChange?.([]);
      return;
    }
    const selectableIdSet = new Set(selectableRowIds);
    const newSelecteds = selected.filter((id) => !selectableIdSet.has(id));
    setSelected(newSelecteds);
    onSelectionChange?.(newSelecteds);
  };

  const handleClick = (_event: React.MouseEvent<unknown>, id: any) => {
    if (readOnly) return;
    const selectedIndex = selected.indexOf(id);
    let newSelected: any[] = [];

    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, id);
    } else if (selectedIndex === 0) {
      newSelected = newSelected.concat(selected.slice(1));
    } else if (selectedIndex === selected.length - 1) {
      newSelected = newSelected.concat(selected.slice(0, -1));
    } else if (selectedIndex > 0) {
      newSelected = newSelected.concat(
        selected.slice(0, selectedIndex),
        selected.slice(selectedIndex + 1),
      );
    }

    setSelected(newSelected);
    onSelectionChange?.(newSelected);
  };

  const isSelected = (id: any) => selected.indexOf(id) !== -1;
  const visibleActionCount =
    (showMenuAction ? 1 : 0) +
    (extraActions ? 1 : 0) +
    (onView ? 1 : 0) +
    (onEdit ? 1 : 0) +
    (showDeleteAction && onDelete ? 1 : 0);
  const actionColumnWidth = actionColumnWidthProp ?? Math.max(108, visibleActionCount * 34 + 16);
  const leftActionAreaSx =
    actionsPosition === "left"
      ? {
          position: "sticky",
          left: 0,
          zIndex: 2,
          borderRight: "1px solid var(--lms-border)",
          bgcolor: "rgba(0,0,0,0.01)",
          boxShadow: "4px 0 8px rgba(15, 23, 42, 0.05)",
        }
      : {};
  const rightActionAreaSx =
    actionsPosition === "right"
      ? {
          position: "sticky",
          right: 0,
          zIndex: 2,
          bgcolor: "#fff",
          borderLeft: "1px solid var(--lms-border)",
          boxShadow: "-4px 0 8px rgba(15, 23, 42, 0.05)",
        }
      : {};
  const getStickyColumnSx = (column: Column, zIndex: number) =>
    column.sticky === "right"
      ? {
          position: "sticky",
          right: actions && actionsPosition === "right" ? actionColumnWidth : 0,
          zIndex,
          bgcolor: "#fff",
          borderLeft: "1px solid var(--lms-border)",
          boxShadow: "-4px 0 8px rgba(15, 23, 42, 0.05)",
        }
      : {};

  const handleResizeStart = (
    event: React.MouseEvent<HTMLDivElement>,
    columnId: string,
  ) => {
    if (!resizableColumns) return;
    event.preventDefault();
    event.stopPropagation();
    const currentWidth = columnWidths[columnId];
    resizingRef.current = {
      columnId,
      startX: event.clientX,
      startWidth: typeof currentWidth === "number" ? currentWidth : 120,
    };
    document.body.style.userSelect = "none";
    document.body.style.cursor = "col-resize";
  };

  const renderStatus = (status: any) => {
    const normalizedStatus =
      typeof status === "string"
        ? status.trim().toLowerCase()
        : typeof status === "boolean"
          ? String(status)
          : status == null
            ? ""
            : String(status).trim().toLowerCase();

    const isActive =
      normalizedStatus === "active" ||
      normalizedStatus === "true" ||
      normalizedStatus === "published" ||
      normalizedStatus === "1";

    const statusLabel =
      normalizedStatus === "true"
        ? "ACTIVE"
        : normalizedStatus === "false"
          ? "INACTIVE"
          : normalizedStatus.toUpperCase();

    return (
      <Box
        sx={{
          display: "inline-flex",
          alignItems: "center",
          gap: 1,
          px: 1.5,
          py: 0.5,
          borderRadius: 1.5,
          bgcolor: isActive ? "rgba(76,175,80,0.1)" : "rgba(244,67,54,0.1)",
          border: `1px solid ${isActive ? "rgba(76,175,80,0.2)" : "rgba(244,67,54,0.2)"}`,
        }}
      >
        <Box
          sx={{
            width: 6,
            height: 6,
            borderRadius: "50%",
            bgcolor: isActive ? "#4CAF50" : "#F44336",
          }}
        />
        <Typography
          variant="caption"
          sx={{
            fontWeight: 800,
            color: isActive ? "#2E7D32" : "#C62828",
            letterSpacing: "0.02em",
            whiteSpace: "nowrap",
          }}
        >
          <T>
            {statusLabel}
          </T>
        </Typography>
      </Box>
    );
  };

  return (
    <Box
      component={motion.div}
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      sx={{ width: "100%", maxWidth: "100%", overflow: "hidden", position: "relative" }}
    >
      {/* Toolbar - Cloned from BankSetup */}
      {!hideToolbar && <Paper
        elevation={0}
        sx={{
          p: compactLegacy ? 0.35 : 2,
          mb: compactLegacy ? 0 : 2,
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          flexWrap: "wrap",
          gap: compactLegacy ? 0.5 : 1.5,
          borderRadius: compactLegacy ? 0 : 3,
          bgcolor: "#fff",
          border: toolbarBorder ?? (compactLegacy ? "1px solid var(--lms-primary)" : "1px solid var(--lms-border)"),
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: compactLegacy ? 0.5 : 1,
            flexWrap: "wrap",
            minWidth: 0,
            flex: "1 1 520px",
          }}
        >
          {extraToolbarItemsLeft}
          {onAdd && (
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={onAdd}
              disabled={effectiveDisableAddAction}
              sx={{
                textTransform: "none",
                fontWeight: 700,
                bgcolor: "var(--lms-primary)",
                transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                boxShadow: "none",
                whiteSpace: "nowrap",
                minHeight: 36,
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  boxShadow: "none",
                },
                "&:active": {
                  transform: "translateY(0)",
                },
                borderRadius: "8px",
                px: 1.5,
                py: 0.65,
              }}
            >
              {translateLabels ? <T>{addButtonLabel || `New`}</T> : (addButtonLabel || "New")}
            </Button>
          )}
          {extraToolbarItems}
        </Box>

        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
            flexWrap: "wrap",
            justifyContent: "flex-end",
            minWidth: 0,
            flex: "0 1 auto",
          }}
        >
          {extraToolbarItemsRight}
          {pagination && (
            <Typography
              variant="caption"
              sx={{
                color: "var(--lms-text-main)",
                fontWeight: 700,
                fontSize: "0.78rem",
                whiteSpace: "nowrap",
              }}
            >
              {`Showing records ${recordRangeStart}–${recordRangeEnd} of ${effectiveTotalCount}`}
            </Typography>
          )}
          {pagination && (
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 0.5,
                bgcolor: "#f8f9fa",
                borderRadius: "8px",
                px: 1,
                py: 0.5,
              }}
            >
              <IconButton
                size="small"
                onClick={() => changePage(1)}
                disabled={page === 1}
              >
                <FirstPageIcon fontSize="small" />
              </IconButton>
              <IconButton
                size="small"
                onClick={() => changePage(page - 1)}
                disabled={page === 1}
              >
                <ChevronLeftIcon fontSize="small" />
              </IconButton>
              <Typography
                variant="caption"
                sx={{
                  mx: 1.5,
                  fontWeight: 700,
                  color: "var(--lms-text-main)",
                  minWidth: 40,
                  textAlign: "center",
                }}
              >
                {page} / {totalPages}
              </Typography>
              <IconButton
                size="small"
                onClick={() => changePage(page + 1)}
                disabled={page === totalPages}
              >
                <ChevronRightIcon fontSize="small" />
              </IconButton>
              <IconButton
                size="small"
                onClick={() => changePage(totalPages)}
                disabled={page === totalPages}
              >
                <LastPageIcon fontSize="small" />
              </IconButton>
            </Box>
          )}

          <Box sx={{ display: "flex", gap: 1 }}>
          {filtersEnabled && rows?.length > 0 && (
              <IconButton
                size="small"
                onClick={() => setFilterVisible(!filterVisible)}
                sx={{
                  bgcolor: filterVisible
                    ? "rgba(var(--lms-primary-rgb), 0.1)"
                    : "rgba(0,0,0,0.02)",
                  color: filterVisible
                    ? "var(--lms-primary)"
                    : "var(--lms-text-secondary)",
                  border: "1px solid var(--lms-border)",
                }}
              >
                <FilterIcon fontSize="small" />
              </IconButton>
            )}
            {downloadEnabled && !!onDownload && (
              <IconButton
                size="small"
                onClick={onDownload}
                sx={{
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.05)",
                  color: "var(--lms-primary)",
                  border: "1px solid var(--lms-border)",
                  transition: "all 0.2s ease",
                  "&:hover": {
                    bgcolor: "var(--lms-primary)",
                    color: "#fff",
                    transform: "scale(1.05)",
                  },
                }}
              >
                <DownloadIcon fontSize="small" />
              </IconButton>
            )}
          </Box>

          {pagination && (
            <LmsSelect
              value={rowsPerPage}
              clearable={false}
              options={[...new Set([...GRID_PAGING_OPTIONS, ...rowsPerPageOptions, rowsPerPage])].sort((a, b) => a - b).map((value) => ({
                label: translate(`${value} Rows`),
                value,
              }))}
              onChange={(newValue) => {
                if (newValue) {
                  changeRowsPerPage(Number(newValue));
                }
              }}
              hideLabel
              sx={{
                width: 150,
                minWidth: 150,
                flexShrink: 0,
              }}
            />
          )}
        </Box>
      </Paper>}

      <TableContainer
        sx={{
          borderRadius: compactLegacy ? 0 : 3,
          border: "1px solid var(--lms-border)",
          overflowX: disableHorizontalScroll ? "hidden" : "auto",
          overflowY: "auto",
          bgcolor: "#fff",
          maxHeight: maxHeight,
        }}
      >
        <Table
          size="small"
          stickyHeader={stickyHeader}
          sx={{
            width: "100%",
            tableLayout: resizableColumns || disableHorizontalScroll ? "fixed" : "auto",
          }}
        >
          <TableHead>
            <TableRow>
              {checkboxSelection && (
                <TableCell padding="checkbox" sx={{ bgcolor: "#fff" }}>
                  <LmsCheckbox
                    indeterminate={
                      selectedSelectableCount > 0 &&
                      selectedSelectableCount < selectableRows.length
                    }
                    checked={
                      selectableRows.length > 0 &&
                      selectableRows.every((row) =>
                        selected.includes(getRowId(row)),
                      )
                    }
                    onChange={(checked) => handleSelectAllClick(checked)}
                    hideLabel
                    disabled={readOnly}
                  />
                </TableCell>
              )}
              {actions && actionsPosition === "left" && (
                <TableCell
                  align="left"
                  sx={{
                    width: actionColumnWidth,
                    minWidth: actionColumnWidth,
                    maxWidth: actionColumnWidth,
                    fontWeight: 800,
                    fontSize: "0.85rem",
                    color: "var(--lms-text-main)",
                    bgcolor: "#fff",
                    py: 1.5,
                    ...leftActionAreaSx,
                  }}
                >
                  <T>Actions</T>
                </TableCell>
              )}
              {columns.map((column) => (
                <TableCell
                  key={column.id}
                  align={column.align || "left"}
                    sx={{
                      minWidth: columnWidths[column.id] ?? column.minWidth,
                      width: columnWidths[column.id],
                      fontWeight: 800,
                      fontSize: "0.85rem",
                      color: "var(--lms-text-main)",
                      bgcolor: "#fff",
                      py: 1.5,
                      position: "relative",
                      overflow: disableHorizontalScroll ? "hidden" : "visible",
                      textOverflow: disableHorizontalScroll ? "ellipsis" : "clip",
                      whiteSpace: disableHorizontalScroll ? "nowrap" : "normal",
                      ...getStickyColumnSx(column, 4),
                    }}
                  >
                  {column.headerFormat ?? (translateLabels ? <T>{column.label}</T> : column.label)}
                  {resizableColumns && (
                    <Box
                      onMouseDown={(e) => handleResizeStart(e, column.id)}
                      sx={{
                        position: "absolute",
                        top: 0,
                        right: -3,
                        width: 8,
                        height: "100%",
                        cursor: "col-resize",
                        zIndex: 3,
                        "&:hover": {
                          bgcolor: "rgba(var(--lms-primary-rgb), 0.18)",
                        },
                      }}
                    />
                  )}
                </TableCell>
              ))}
              {actions && actionsPosition === "right" && (
                <TableCell
                  align="center"
                  sx={{
                    width: actionColumnWidth,
                    minWidth: actionColumnWidth,
                    maxWidth: actionColumnWidth,
                    fontWeight: 800,
                    fontSize: "0.85rem",
                    color: "var(--lms-text-main)",
                    bgcolor: "#fff",
                    py: 1.5,
                    ...rightActionAreaSx,
                  }}
                >
                  <T>Actions</T>
                </TableCell>
              )}
            </TableRow>
            <AnimatePresence>
              {filtersEnabled && filterVisible && (
                <TableRow
                  component={motion.tr}
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: "auto" }}
                  exit={{ opacity: 0, height: 0 }}
                >
                  {checkboxSelection && (
                    <TableCell padding="checkbox" sx={{ bgcolor: "#fff" }} />
                  )}
                  {actions && actionsPosition === "left" && (
                    <TableCell
                      sx={{
                        p: 1,
                        bgcolor: "#fff",
                        width: actionColumnWidth,
                        minWidth: actionColumnWidth,
                        maxWidth: actionColumnWidth,
                        ...leftActionAreaSx,
                      }}
                    />
                  )}
                  {columns.map((column) => (
                    <TableCell
                      key={column.id}
                    sx={{
                        p: 1,
                        bgcolor: "#fff",
                        minWidth: columnWidths[column.id] ?? column.minWidth,
                        width: columnWidths[column.id],
                        overflow: disableHorizontalScroll ? "hidden" : "visible",
                        textOverflow: disableHorizontalScroll ? "ellipsis" : "clip",
                        whiteSpace: disableHorizontalScroll ? "nowrap" : "normal",
                        ...getStickyColumnSx(column, 3),
                      }}
                    >
                      {column.filterable === false ? null : (
                        <FormInput
                          fullWidth
                          size="small"
                          placeholder={`${translate("Filter")} ${translateLabels ? translate(column.label) : column.label}...`}
                          value={filters[column.id] || ""}
                          onChange={(e) =>
                            handleFilterChange(column.id, e.target.value)
                          }
                          sx={{
                            "& .MuiInputBase-root": {
                              fontSize: "0.75rem",
                              bgcolor: "#fdfdfd",
                              borderRadius: "8px",
                              "& fieldset": {
                                borderColor: "var(--lms-border)",
                              },
                            },
                          }}
                        />
                      )}
                    </TableCell>
                  ))}
                  {actions && actionsPosition === "right" && (
                    <TableCell
                      sx={{
                        p: 1,
                        bgcolor: "#fff",
                        width: actionColumnWidth,
                        minWidth: actionColumnWidth,
                        maxWidth: actionColumnWidth,
                        ...rightActionAreaSx,
                      }}
                    />
                  )}
                </TableRow>
              )}
            </AnimatePresence>
          </TableHead>
          <TableBody>
            {displayRows.map((row, rowIndex) => {
              const rowId = getRowId(row);
              const isRowSelected =
                Array.isArray(selectedRowIds) && selectedRowIds.length > 0
                  ? selectedRowIds.some((selectedId) => String(selectedId) === String(rowId))
                  : selectedRowId !== undefined
                  ? rowId === selectedRowId
                  : isSelected(rowId);
              return (
                <TableRow
                  key={rowId}
                  hover
                  onClick={(event) =>
                    onRowClick
                      ? onRowClick(row)
                      : checkboxSelection && !disableRowSelectionOnClick
                        ? handleClick(event, rowId)
                        : undefined
                  }
                  onDoubleClick={canOpenRowOnDoubleClick && !hideStandardActions?.(row) ? (event) => {
                    if ((event.target as HTMLElement).closest('button, a, input, select, textarea, [role="button"], [contenteditable="true"]')) return;
                    rowDoubleClickHandler?.(row);
                  } : undefined}
                  role="checkbox"
                  aria-checked={isRowSelected}
                  tabIndex={-1}
                  selected={isRowSelected}
                  sx={{
                    cursor:
                      onRowClick || canOpenRowOnDoubleClick || checkboxSelection
                        ? "pointer"
                        : "default",
                    "&.Mui-selected": {
                      backgroundColor: "rgba(var(--lms-primary-rgb), 0.24) !important",
                    },
                    "&.Mui-selected:hover": {
                      backgroundColor: "rgba(var(--lms-primary-rgb), 0.24) !important",
                    },
                    "&:hover": {
                      backgroundColor: isRowSelected
                        ? "rgba(var(--lms-primary-rgb), 0.24) !important"
                        : `${rowHoverBg} !important`,
                    },
                    bgcolor: isRowSelected
                      ? "rgba(var(--lms-primary-rgb), 0.24) !important"
                      : rowIndex % 2 === 0
                        ? "#fff"
                        : "rgba(0,0,0,0.005)",
                    "& td": isRowSelected
                      ? { color: "var(--lms-primary-dark)", fontWeight: 700 }
                      : undefined,
                  }}
                >
                  {checkboxSelection && (
                    <TableCell
                      padding="checkbox"
                      onClick={(event) => event.stopPropagation()}
                    >
                      <LmsCheckbox
                        checked={isRowSelected}
                        onChange={(_, event) => {
                          event.stopPropagation();
                          handleClick(event as any, rowId);
                        }}
                        hideLabel
                        disabled={readOnly}
                      />
                    </TableCell>
                  )}
                  {actions && actionsPosition === "left" && (
                    <TableCell
                      align="left"
                      sx={{
                        py: 1.2,
                        width: actionColumnWidth,
                        minWidth: actionColumnWidth,
                        maxWidth: actionColumnWidth,
                        ...leftActionAreaSx,
                      }}
                    >
                      <Box
                        sx={{
                          display: "flex",
                          gap: 0.5,
                          justifyContent: "flex-start",
                          alignItems: "center",
                          flexWrap: "nowrap",
                          whiteSpace: "nowrap",
                        }}
                      >
                        {extraActions && extraActions(row)}
                        {showMenuAction && !hideStandardActions?.(row) && (
                          <Tooltip title={translate("Options")} arrow>
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                onMenuAction && onMenuAction(row, e);
                              }}
                              sx={{
                                color: "var(--lms-text-main)",
                                "&:hover": {
                                  bgcolor: "rgba(0, 0, 0, 0.05)",
                                },
                              }}
                            >
                              <MenuIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {onView && !hideStandardActions?.(row) && (
                          <Tooltip title={translate("View")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableViewAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onView(row);
                              }}
                              sx={{
                                color: "var(--lms-primary)",
                                "&:hover": {
                                  bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
                                },
                              }}
                            >
                              <ViewIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {onEdit && !hideStandardActions?.(row) && !hideEditDeleteActions?.(row) && (
                          <Tooltip title={translate("Edit")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableEditAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onEdit(row);
                              }}
                              sx={{
                                color: "#006699",
                                "&:hover": {
                                  bgcolor: "rgba(0, 102, 153, 0.05)",
                                },
                              }}
                            >
                              <EditIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {showDeleteAction && onDelete && !hideStandardActions?.(row) && !hideEditDeleteActions?.(row) && (
                          <Tooltip title={translate("Delete")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableDeleteAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onDelete(row);
                              }}
                              sx={{
                                color: "#EF4444",
                                "&:hover": { bgcolor: "rgba(239,68,68,0.1)" },
                              }}
                            >
                              <DeleteIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Box>
                    </TableCell>
                  )}
                  {columns.map((column) => {
                    const value = getCellRawValue(row, column.id);
                    const displayValue =
                      column.id === "sNo"
                        ? (pagination ? (page - 1) * rowsPerPage + rowIndex : rowIndex) + 1
                        : value;
                    return (
                      <TableCell
                        key={column.id}
                        align={column.align || "left"}
                      sx={{
                          minWidth: columnWidths[column.id] ?? column.minWidth,
                          width: columnWidths[column.id],
                          fontSize: "0.8rem",
                          fontWeight:
                            column.id === "courseName" || column.id === "course" || column.id === "sNo"
                              ? 700
                              : 500,
                          color:
                            column.id === "courseName" || column.id === "course" || column.id === "sNo"
                              ? "var(--lms-text-main)"
                              : "var(--lms-text-secondary)",
                          py: 1.2,
                          wordBreak: disableHorizontalScroll ? "normal" : "break-all",
                          overflowWrap: disableHorizontalScroll ? "normal" : "break-word",
                          overflow: disableHorizontalScroll ? "hidden" : "visible",
                          textOverflow: disableHorizontalScroll ? "ellipsis" : "clip",
                          whiteSpace: disableHorizontalScroll ? "nowrap" : "normal",
                          ...getStickyColumnSx(column, 2),
                      }}
                    >
                        {column.type === "status"
                          ? renderStatus(displayValue)
                          : column.format
                            ? column.format(displayValue, row, pagination ? (page - 1) * rowsPerPage + rowIndex : rowIndex)
                            : displayValue}

                      </TableCell>
                    );
                  })}
                  {actions && actionsPosition === "right" && (
                    <TableCell
                      align="center"
                      sx={{
                        py: 1.2,
                        width: actionColumnWidth,
                        minWidth: actionColumnWidth,
                        maxWidth: actionColumnWidth,
                        ...rightActionAreaSx,
                      }}
                    >
                      <Box
                        sx={{
                          display: "flex",
                          gap: 0.5,
                          justifyContent: "center",
                          alignItems: "center",
                          flexWrap: "nowrap",
                          whiteSpace: "nowrap",
                        }}
                      >
                        {extraActions && extraActions(row)}
                        {showMenuAction && !hideStandardActions?.(row) && (
                          <Tooltip title={translate("Options")} arrow>
                            <IconButton
                              size="small"
                              onClick={(e) => {
                                e.stopPropagation();
                                onMenuAction && onMenuAction(row, e);
                              }}
                              sx={{
                                color: "var(--lms-text-main)",
                                "&:hover": {
                                  bgcolor: "rgba(0, 0, 0, 0.05)",
                                },
                              }}
                            >
                              <MenuIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {onView && !hideStandardActions?.(row) && (
                          <Tooltip title={translate("View")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableViewAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onView(row);
                              }}
                              sx={{
                                color: "var(--lms-primary)",
                                "&:hover": {
                                  bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
                                },
                              }}
                            >
                              <ViewIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {onEdit && !hideStandardActions?.(row) && !hideEditDeleteActions?.(row) && (
                          <Tooltip title={translate("Edit")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableEditAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onEdit(row);
                              }}
                              sx={{
                                color: "#006699",
                                "&:hover": {
                                  bgcolor: "rgba(0, 102, 153, 0.05)",
                                },
                              }}
                            >
                              <EditIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {showDeleteAction && onDelete && !hideStandardActions?.(row) && !hideEditDeleteActions?.(row) && (
                          <Tooltip title={translate("Delete")} arrow>
                            <IconButton
                              size="small"
                              disabled={effectiveDisableDeleteAction}
                              onClick={(e) => {
                                e.stopPropagation();
                                onDelete(row);
                              }}
                              sx={{
                                color: "#EF4444",
                                "&:hover": { bgcolor: "rgba(239,68,68,0.1)" },
                              }}
                            >
                              <DeleteIcon sx={{ fontSize: "1.1rem" }} />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Box>
                    </TableCell>
                  )}
                </TableRow>
              );
            })}
            {displayRows.length === 0 && (
              <TableRow>
                <TableCell
                  colSpan={
                    columns.length +
                    (actions ? 1 : 0) +
                    (checkboxSelection ? 1 : 0)
                  }
                  sx={{ py: compactLegacy ? 16 : 12, textAlign: "center", border: "none" }}
                >
                  <EmptyState />
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};
