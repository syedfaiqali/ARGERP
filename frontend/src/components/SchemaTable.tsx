import React, { useMemo, useState } from "react";
import { Box, Typography, Button } from "@mui/material";
import { DashboardTable } from "./DashboardTable";
import type { Column } from "./DashboardTable";
import { Modal } from "./Modal";
import { LmsToaster } from "./LmsToaster";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import { useLocation } from "react-router-dom";
import { useHiddenMenuPermissions } from "@/hooks/useHiddenMenuPermissions";
import { canOpenGridRowOnDoubleClick } from '@/utils/screenActionRights';
import { getGridRowPermissionMenuId } from '@/utils/gridRowAccess';

interface SchemaTableProps {
  title: string;
  addButtonLabel?: string;
  breadcrumbPath?: string;
  columns: Column[];
  rows: any[];
  onRowClick?: (row: any) => void;
  onRowDoubleClick?: (row: any) => void;
  initialFormData?: any;
  onSave?: (data: any) => void | boolean | Promise<void | boolean>;
  onDelete?: (row: any) => void | Promise<void>;
  loadRow?: (row: any, mode: "edit" | "view") => Promise<any>;
  renderForm?: (
    formData: any,
    setFormData: React.Dispatch<React.SetStateAction<any>>,
    modeOrIsReadOnly?: any,
    isReadOnlyOrIsSubmitted?: any,
    isSubmitted?: any,
    handleSave?: (shouldClose?: boolean) => void,
    handleClose?: () => void,
    handleReset?: () => void,
  ) => React.ReactNode;
  extraToolbarItemsLeft?: React.ReactNode;
  extraToolbarItems?: React.ReactNode;
  extraToolbarItemsRight?: React.ReactNode;
  showModalActions?: boolean;
  headerBgColor?: string;
  headerTextColor?: string;
  actions?: boolean;
  actionsPosition?: "left" | "right";
  maxWidth?: "xs" | "sm" | "md" | "lg" | "xl";
  mandatoryFields?: {
    key: string;
    label: string;
    editOptional?: boolean;
    condition?: (data: any) => boolean;
  }[];
  checkboxSelection?: boolean;
  showAddButton?: boolean;
  disableAddAction?: boolean;
  showMenuAction?: boolean;
  onMenuAction?: (row: any, event?: React.MouseEvent<HTMLElement>) => void;
  showDeleteAction?: boolean;
  resizableColumns?: boolean;
  /** Override the New button click — useful for navigating to a full-page form */
  onAddClick?: () => void;
  /** Override the Edit button click — useful for navigating to a full-page form */
  onEditClick?: (row: any) => void;
  /** Override the View button click — useful for navigating to a full-page form */
  onViewClick?: (row: any) => void;
  maxHeight?: number | string;
  hideBreadcrumbs?: boolean;
  hideTitle?: boolean;
  /** Whether to show the internal toaster — defaults to true */
  showToaster?: boolean;
  onSelectionChange?: (selectedIds: any[]) => void;
  selectedIds?: any[];
  selectedRowIds?: any[];
  disableRowSelectionOnClick?: boolean;
  /** Whether to show the Edit action in the list — defaults to true */
  showEditAction?: boolean;
  /** Whether to show the View action in the list — defaults to true */
  showViewAction?: boolean;
  pagination?: boolean;
  filtersEnabled?: boolean;
  onFiltersChange?: (filters: Record<string, string>) => void;
  onSelectAllRequest?: () => Promise<any[]>;
  downloadEnabled?: boolean;
  exportType?: string;
  stickyHeader?: boolean;
  rowHoverBg?: string;
  hideStandardActions?: (row: any) => boolean;
  /** Hides only the edit and delete actions for a row (view and double-click stay), e.g. system generated vouchers. */
  hideEditDeleteActions?: (row: any) => boolean;
  extraActions?: (row: any, rights?: { canCreate: boolean }) => React.ReactNode;
  currentPage?: number;
  rowsPerPage?: number;
  totalCount?: number;
  onPageChange?: (page: number) => void;
  onRowsPerPageChange?: (rowsPerPage: number) => void;
  rowsPerPageOptions?: number[];
  readOnly?: boolean;
  compactLegacy?: boolean;
  disableHorizontalScroll?: boolean;
  actionColumnWidth?: number;
  toolbarBorder?: string;
  defaultLatestFirst?: boolean;
  permissionMenuId?: number;
  permissionScreenName?: string;
  permissionParentName?: string;
  useActionRights?: boolean;
  translateLabels?: boolean;
  hideToolbar?: boolean;
}

const getSortableRecordId = (row: any) => {
  const value = row?.id ?? row?.Id ?? row?.ID ?? row?.rowId ?? row?.RowId;
  if (value === null || value === undefined || value === "") return null;

  const numericValue = Number(value);
  return Number.isFinite(numericValue) ? numericValue : null;
};

export const SchemaTable: React.FC<SchemaTableProps> = ({
  title,
  addButtonLabel,
  breadcrumbPath,
  columns,
  rows,
  onRowClick,
  onRowDoubleClick,
  initialFormData,
  onSave,
  onDelete,
  loadRow,
  renderForm,
  extraToolbarItemsLeft,
  extraToolbarItems,
  extraToolbarItemsRight,
  showModalActions = true,
  headerBgColor,
  headerTextColor,
  actions = true,
  actionsPosition = "right",
  showDeleteAction = true,
  maxWidth = "md",
  mandatoryFields,
  checkboxSelection = false,
  showAddButton = true,
  disableAddAction = false,
  showMenuAction = false,
  onMenuAction,
  resizableColumns = false,
  onAddClick,
  onEditClick,
  onViewClick,
  maxHeight,
  hideBreadcrumbs = false,
  hideTitle = false,
  showToaster = true,
  onSelectionChange,
  selectedIds,
  selectedRowIds,
  disableRowSelectionOnClick = false,
  showEditAction = true,
  showViewAction = true,
  pagination,
  filtersEnabled,
  onFiltersChange,
  onSelectAllRequest,
  downloadEnabled,
  exportType = "Excel",
  stickyHeader,
  rowHoverBg,
  extraActions,
  hideStandardActions,
  hideEditDeleteActions,
  currentPage,
  rowsPerPage,
  totalCount,
  onPageChange,
  onRowsPerPageChange,
  rowsPerPageOptions,
  readOnly = false,
  compactLegacy = false,
  disableHorizontalScroll = false,
  actionColumnWidth,
  toolbarBorder,
  defaultLatestFirst = true,
  permissionMenuId,
  permissionScreenName,
  permissionParentName,
  useActionRights = true,
  translateLabels = true,
  hideToolbar = false,
}) => {
  const { translate } = useTranslation();
  const { screenPermissionsLoaded, hasScreenActionRight, hasScreenActionRightForParent, hasScreenActionRightForMenuId, getScreenMenuItem, visibleMenuCardGroups } = useHiddenMenuPermissions();
  const location = useLocation();
  const [open, setOpen] = useState(false);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [rowToDelete, setRowToDelete] = useState<any>(null);
  const [formData, setFormData] = useState(initialFormData);
  const [mode, setMode] = useState<"new" | "edit" | "view">("new");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);
  const [showErrors, setShowErrors] = useState(false);
  const [toasterSeverity, setToasterSeverity] = useState<"error" | "success">(
    "error",
  );

  const handleOpen = () => {
    if (onAddClick) {
      onAddClick();
      return;
    }
    setFormData(initialFormData);
    setMode("new");
    setIsSubmitted(false);
    setOpen(true);
  };

  const handleEdit = (row: any) => {
    if (!canUpdate || readOnly) return;
    if (onEditClick) {
      onEditClick(row);
      return;
    }
    (async () => {
      try {
        const loaded = loadRow ? await loadRow(row, "edit") : row;
        setFormData(loaded);
        setMode("edit");
        setIsSubmitted(false);
        setOpen(true);
      } catch (error: any) {
        const message =
          error?.response?.data?.error ||
          error?.message ||
          translate("Failed to load record");
        setErrorMessages([message]);
        setToasterSeverity("error");
        setShowErrors(true);
      }
    })();
  };

  const handleView = (row: any) => {
    if (!canView && !canUseRowDoubleClick) return;
    if (onViewClick) {
      onViewClick(row);
      return;
    }
    (async () => {
      try {
        const loaded = loadRow ? await loadRow(row, "view") : row;
        setFormData(loaded);
        setMode("view");
        setIsSubmitted(false);
        setOpen(true);
      } catch (error: any) {
        const message =
          error?.response?.data?.error ||
          error?.message ||
          translate("Failed to load record");
        setErrorMessages([message]);
        setToasterSeverity("error");
        setShowErrors(true);
      }
    })();
  };

  const handleDeleteClick = (row: any) => {
    if (!onDelete) return;
    setRowToDelete(row);
    setDeleteConfirmOpen(true);
  };

  const confirmDelete = async () => {
    if (rowToDelete && onDelete) {
      const row = rowToDelete;
      setDeleteConfirmOpen(false);
      setRowToDelete(null);
      try {
        await onDelete(row);
        setErrorMessages([translate("Record deleted successfully")]);
        setToasterSeverity("success");
        setShowErrors(true);
      } catch (error: any) {
        const message =
          error?.response?.data?.error ||
          error?.message ||
          translate("Failed to delete record");
        setErrorMessages([message]);
        setToasterSeverity("error");
        setShowErrors(true);
      } finally {
        setDeleteConfirmOpen(false);
        setRowToDelete(null);
      }
    }
  };

  const handleClose = () => setOpen(false);

  const handleSaveInternal = async (shouldClose: boolean = false) => {
    setIsSubmitted(true);

    const errors: string[] = [];
    if (mandatoryFields) {
      mandatoryFields.forEach((field: any) => {
        const isOptionalInEditMode = mode === "edit" && field.editOptional;
        const conditionMet = field.condition ? field.condition(formData) : true;
        if (!formData[field.key] && !isOptionalInEditMode && conditionMet) {
          errors.push(`${field.label} ${translate("is empty")}`);
        }
      });
    }

    if (errors.length > 0) {
      setErrorMessages(errors);
      setToasterSeverity("error");
      setShowErrors(true);
      return;
    }

    try {
      if (!onSave) return;
      const result = await onSave(formData);
      if (result !== false) {
        const successMessage =
          mode === "edit"
            ? translate("Record updated successfully")
            : translate("Record saved successfully");

        setErrorMessages([successMessage]);
        setToasterSeverity("success");
        setShowErrors(true);

        if (shouldClose) {
          handleClose();
        } else {
          setFormData(initialFormData);
          setMode("new");
          setIsSubmitted(false);
        }
      }
    } catch (error: any) {
      const message =
        error?.response?.data?.error ||
        error?.message ||
        translate("Failed to save record");
      setErrorMessages([message]);
      setToasterSeverity("error");
      setShowErrors(true);
    }
  };

  const getRawCellValue = (row: any, columnId: string) => {
    const pascalColumnId = columnId
      ? `${columnId[0].toUpperCase()}${columnId.slice(1)}`
      : columnId;
    const value = row?.[columnId] ?? row?.[pascalColumnId];
    if (value === null || value === undefined) return "";
    if (value instanceof Date) return value.toLocaleDateString();
    if (typeof value === "object") return JSON.stringify(value);
    return String(value);
  };

  const sanitizeFileName = (value: string) =>
    value
      .replace(/[^a-z0-9-_]+/gi, "-")
      .replace(/^-+|-+$/g, "")
      .toLowerCase() || "table-export";

  const escapeHtml = (value: string) =>
    value
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");

  const downloadBlob = (blob: Blob, fileName: string) => {
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const buildExportMatrix = () => {
    const exportColumns = columns.filter((column) => column.id !== "actions");
    const header = exportColumns.map((column) => translate(column.label));
    const body = tableRows.map((row, rowIndex) =>
      exportColumns.map((column) =>
        column.id === "sNo" ? String(rowIndex + 1) : getRawCellValue(row, column.id),
      ),
    );
    return { header, body };
  };

  const buildSimplePdf = (lines: string[]) => {
    const pageWidth = 842;
    const pageHeight = 595;
    const margin = 28;
    const lineHeight = 12;
    const maxLinesPerPage = Math.floor((pageHeight - margin * 2) / lineHeight);
    const pages: string[][] = [];

    for (let index = 0; index < lines.length; index += maxLinesPerPage) {
      pages.push(lines.slice(index, index + maxLinesPerPage));
    }

    const escapePdfText = (value: string) =>
      value.replace(/\\/g, "\\\\").replace(/\(/g, "\\(").replace(/\)/g, "\\)");

    const objects: string[] = [];
    const pageObjectNumbers: number[] = [];

    objects.push("<< /Type /Catalog /Pages 2 0 R >>");
    objects.push("");

    pages.forEach((pageLines) => {
      const contentObjectNumber = objects.length + 2;
      const pageObjectNumber = objects.length + 3;
      pageObjectNumbers.push(pageObjectNumber);

      const text = [
        "BT",
        "/F1 8 Tf",
        `${margin} ${pageHeight - margin} Td`,
        ...pageLines.map((line, index) =>
          `${index === 0 ? "" : `0 -${lineHeight} Td `}(${escapePdfText(line.slice(0, 170))}) Tj`,
        ),
        "ET",
      ].join("\n");

      objects.push(`<< /Length ${text.length} >>\nstream\n${text}\nendstream`);
      objects.push(
        `<< /Type /Page /Parent 2 0 R /MediaBox [0 0 ${pageWidth} ${pageHeight}] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> >> >> /Contents ${contentObjectNumber} 0 R >>`,
      );
    });

    objects[1] = `<< /Type /Pages /Kids [${pageObjectNumbers
      .map((objectNumber) => `${objectNumber} 0 R`)
      .join(" ")}] /Count ${pageObjectNumbers.length} >>`;

    let pdf = "%PDF-1.4\n";
    const offsets: number[] = [0];
    objects.forEach((object, index) => {
      offsets.push(pdf.length);
      pdf += `${index + 1} 0 obj\n${object}\nendobj\n`;
    });

    const xrefOffset = pdf.length;
    pdf += `xref\n0 ${objects.length + 1}\n0000000000 65535 f \n`;
    offsets.slice(1).forEach((offset) => {
      pdf += `${String(offset).padStart(10, "0")} 00000 n \n`;
    });
    pdf += `trailer\n<< /Size ${objects.length + 1} /Root 1 0 R >>\nstartxref\n${xrefOffset}\n%%EOF`;
    return pdf;
  };

  const handleDownload = () => {
    const normalizedType = String(exportType || "Excel").trim().toLowerCase();
    if (!tableRows.length) {
      setErrorMessages([translate("No data available to download")]);
      setToasterSeverity("error");
      setShowErrors(true);
      return;
    }

    if (normalizedType === "none") {
      setErrorMessages([translate("Please select export type")]);
      setToasterSeverity("error");
      setShowErrors(true);
      return;
    }

    const { header, body } = buildExportMatrix();
    const fileBase = sanitizeFileName(title);

    if (normalizedType === "pdf") {
      const lines = [
        translate(title),
        "",
        header.join(" | "),
        "-".repeat(Math.min(170, header.join(" | ").length || 20)),
        ...body.map((row) => row.join(" | ")),
      ];
      downloadBlob(
        new Blob([buildSimplePdf(lines)], { type: "application/pdf" }),
        `${fileBase}.pdf`,
      );
      return;
    }

    if (normalizedType === "csv") {
      const csv = [header, ...body]
        .map((row) =>
          row
            .map((cell) => `"${String(cell).replace(/"/g, '""')}"`)
            .join(","),
        )
        .join("\r\n");
      downloadBlob(
        new Blob([`\uFEFF${csv}`], { type: "text/csv;charset=utf-8;" }),
        `${fileBase}.csv`,
      );
      return;
    }

    const html = `
      <html>
        <head><meta charset="UTF-8" /></head>
        <body>
          <table border="1">
            <thead>
              <tr>${header.map((cell) => `<th>${escapeHtml(cell)}</th>`).join("")}</tr>
            </thead>
            <tbody>
              ${body
                .map(
                  (row) =>
                    `<tr>${row
                      .map((cell) => `<td>${escapeHtml(String(cell))}</td>`)
                      .join("")}</tr>`,
                )
                .join("")}
            </tbody>
          </table>
        </body>
      </html>`;
    downloadBlob(
      new Blob([html], { type: "application/vnd.ms-excel;charset=utf-8;" }),
      `${fileBase}.xls`,
    );
  };

  const toTitleCase = (str: string) => {
    return str
      .toLowerCase()
      .split(" ")
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(" ");
  };

  const finalAddButtonLabel =
    !addButtonLabel ||
      addButtonLabel === translate("New") ||
      addButtonLabel === "+ New" ||
      addButtonLabel === `+ New ${title}`
      ? `${translate("New")} ${toTitleCase(translate(title))}`
      : toTitleCase(
        addButtonLabel
          .replace(/^\+?\s*New\s+/i, `${translate("New")} `)
          .replace(/^\+?\s*\+\s*/, ""),
      );
  const routeScreenName = (() => {
    const parts = location.pathname.split("/").map((part) => part.trim()).filter(Boolean);
    const lastPart = parts[parts.length - 1];
    if (!lastPart || parts.length < 2) return "";

    try {
      return decodeURIComponent(lastPart);
    } catch {
      return lastPart;
    }
  })();
  const effectivePermissionScreenName = permissionScreenName || routeScreenName || title;
  const resolvedPermissionMenuId =
    permissionMenuId ??
    getScreenMenuItem(effectivePermissionScreenName, permissionParentName)?.menuId ??
    null;
  const hasEffectiveActionRight = (actionRight: string) =>
    resolvedPermissionMenuId != null
      ? hasScreenActionRightForMenuId(resolvedPermissionMenuId, actionRight)
      : permissionParentName
      ? hasScreenActionRightForParent(effectivePermissionScreenName, permissionParentName, actionRight)
      : hasScreenActionRight(effectivePermissionScreenName, actionRight);
  const canCreate = !useActionRights || (
    screenPermissionsLoaded &&
    hasEffectiveActionRight("create")
  );
  const canView = !useActionRights || (
    screenPermissionsLoaded &&
    hasEffectiveActionRight("view")
  );
  const canUpdate = !useActionRights || (
    screenPermissionsLoaded &&
    hasEffectiveActionRight("update")
  );
  const canDelete = !useActionRights || (
    screenPermissionsLoaded &&
    hasEffectiveActionRight("delete")
  );

  const scopedRowMenuId = getGridRowPermissionMenuId(effectivePermissionScreenName, location.pathname, permissionParentName, visibleMenuCardGroups);
  const rowMenuId = permissionMenuId ?? (scopedRowMenuId === undefined ? resolvedPermissionMenuId : scopedRowMenuId);
  const hasRowActionRight = (right: string) => rowMenuId === null ? false : rowMenuId != null ? hasScreenActionRightForMenuId(rowMenuId, right) : hasEffectiveActionRight(right);
  const canUseRowDoubleClick = canOpenGridRowOnDoubleClick({
    permissionsLoaded: screenPermissionsLoaded,
    canCreate: hasRowActionRight('create'),
    canUpdate: hasRowActionRight('update'),
    readOnly,
  });
  const hasRowViewer = Boolean(onViewClick || renderForm);
  const hasRowEditor = Boolean(onEditClick || renderForm);
  const effectiveRowDoubleClick = canUseRowDoubleClick && (actions || onRowDoubleClick)
    ? onRowDoubleClick || (canUpdate && showEditAction && hasRowEditor
      ? handleEdit
      : hasRowViewer ? handleView : undefined)
    : undefined;

  const tableRows = useMemo(() => {
    const safeRows = Array.isArray(rows) ? rows : [];
    if (!defaultLatestFirst || safeRows.length <= 1) return safeRows;

    const rowsWithIndex = safeRows.map((row, index) => ({
      row,
      index,
      sortableId: getSortableRecordId(row),
    }));

    const hasSortableIds = rowsWithIndex.every((item) => item.sortableId !== null);
    if (hasSortableIds) {
      return [...rowsWithIndex]
        .sort((a, b) => {
          const idDifference = (b.sortableId ?? 0) - (a.sortableId ?? 0);
          return idDifference !== 0 ? idDifference : b.index - a.index;
        })
        .map((item) => item.row);
    }

    return [...safeRows].reverse();
  }, [defaultLatestFirst, rows]);

  return (
    <Box sx={{ width: "100%", px: hideTitle ? 0 : 2, mt: hideTitle ? 0 : 1 }}>
      {!hideTitle && (
        <Box sx={{ mb: 2 }}>
          {!hideBreadcrumbs && breadcrumbPath && (
            <Typography variant="caption" color="textSecondary">
              {breadcrumbPath}
            </Typography>
          )}
          <Typography variant="h5" fontWeight={800}>
            {title}
          </Typography>
        </Box>
      )}
      <DashboardTable
        title={title}
        addButtonLabel={finalAddButtonLabel}
        columns={columns}
        rows={tableRows}
        onAdd={showAddButton ? handleOpen : undefined}
        onEdit={showEditAction ? handleEdit : undefined}
        onView={showViewAction && hasRowViewer ? handleView : undefined}
        onDelete={handleDeleteClick}
        disableAddAction={disableAddAction || !canCreate}
        disableViewAction={!canView}
        disableEditAction={!canUpdate}
        disableDeleteAction={!canDelete}
        onDownload={handleDownload}
        extraToolbarItemsLeft={extraToolbarItemsLeft}
        extraToolbarItems={extraToolbarItems}
        extraToolbarItemsRight={extraToolbarItemsRight}
        actions={actions}
        actionsPosition={actionsPosition}
        checkboxSelection={checkboxSelection}
        hideStandardActions={hideStandardActions}
        hideEditDeleteActions={hideEditDeleteActions}
        showMenuAction={showMenuAction}
        onMenuAction={onMenuAction}
        onRowClick={onRowClick}
        onRowDoubleClick={effectiveRowDoubleClick}
        resizableColumns={resizableColumns}
        showDeleteAction={showDeleteAction}
        maxHeight={maxHeight}
        onSelectionChange={onSelectionChange}
        selectedIds={selectedIds}
        selectedRowIds={selectedRowIds}
        disableRowSelectionOnClick={disableRowSelectionOnClick}
        pagination={pagination}
        filtersEnabled={filtersEnabled}
        onFiltersChange={onFiltersChange}
        onSelectAllRequest={onSelectAllRequest}
        downloadEnabled={downloadEnabled}
        stickyHeader={stickyHeader}
        rowHoverBg={rowHoverBg}
        extraActions={extraActions ? (row) => extraActions(row, { canCreate }) : undefined}
        currentPage={currentPage}
        rowsPerPage={rowsPerPage}
        totalCount={totalCount}
        onPageChange={onPageChange}
        onRowsPerPageChange={onRowsPerPageChange}
        rowsPerPageOptions={rowsPerPageOptions}
        readOnly={readOnly}
        compactLegacy={compactLegacy}
        disableHorizontalScroll={disableHorizontalScroll}
        actionColumnWidth={actionColumnWidth}
        toolbarBorder={toolbarBorder}
        permissionScreenName={effectivePermissionScreenName}
        permissionMenuId={rowMenuId ?? undefined}
        permissionParentName={permissionParentName}
        useActionRights={useActionRights}
        translateLabels={translateLabels}
        hideToolbar={hideToolbar}
      />

      <Modal
        open={open}
        onClose={handleClose}
        title={`${mode === "view" ? translate("View") : mode === "edit" ? translate("Edit") : translate("Add")} ${title}`}
        breadcrumbPath={breadcrumbPath}
        onSave={() => handleSaveInternal(false)}
        onSaveAndClose={() => handleSaveInternal(true)}
        onReset={() => {
          setFormData(initialFormData);
          setIsSubmitted(false);
        }}
        showActions={showModalActions && mode !== "view"}
        headerBgColor={headerBgColor}
        headerTextColor={headerTextColor}
        maxWidth={maxWidth}
      >
        {renderForm
          ? renderForm.length <= 4
            ? (renderForm as any)(formData, setFormData, mode === "view", isSubmitted)
            : (renderForm as any)(
              formData,
              setFormData,
              mode,
              mode === "view",
              isSubmitted,
              handleSaveInternal,
              handleClose,
              () => {
                setFormData(initialFormData);
              },
            )
          : null}
      </Modal>

      {showToaster && (
        <LmsToaster
          open={showErrors}
          onClose={() => setShowErrors(false)}
          messages={errorMessages}
          severity={toasterSeverity}
        />
      )}

      <Modal
        open={deleteConfirmOpen}
        onClose={() => setDeleteConfirmOpen(false)}
        title={translate("Confirm Delete")}
        maxWidth="xs"
        showActions={false}
      >
        <Box sx={{ textAlign: "center", py: 2 }}>
          <Typography
            sx={{ mb: 3, fontWeight: 600, color: "var(--lms-text-main)" }}
          >
            <T>
              Are you sure you want to delete this record? This action cannot be
              undone.
            </T>
          </Typography>
          <Box sx={{ display: "flex", gap: 2, justifyContent: "center" }}>
            <Button
              variant="outlined"
              onClick={() => setDeleteConfirmOpen(false)}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
              }}
            >
              <T>No, Cancel</T>
            </Button>
            <Button
              variant="contained"
              onClick={confirmDelete}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
                bgcolor: "#EF4444",
                "&:hover": { bgcolor: "#DC2626" },
              }}
            >
              <T>Yes, Delete</T>
            </Button>
          </Box>
        </Box>
      </Modal>
    </Box>
  );
};
