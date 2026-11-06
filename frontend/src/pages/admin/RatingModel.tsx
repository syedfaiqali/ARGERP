import React, { useState, useEffect } from "react";
import {
  Grid,
  Box,
  Typography,
  Button,
  Collapse,
} from "@mui/material";
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
  DeleteService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { ratingModelSlice } from "@/store/humanResourceSlices";

interface RatingModelProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const RatingModel: React.FC<RatingModelProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrRatingModel",
    ratingModelSlice.fetchAll,
  );
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;

  const [isGeneralExpanded, setIsGeneralExpanded] = useState(true);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<"success" | "error">("error");
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<any>({
    code: "",
    ratingModel: "",
    isActive: true,
    items: [],
    selectedItemIds: [],
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Rating Model"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Rating Model"));
    else if (viewMode === "view") onSubViewChange(translate("View Rating Model"));
    else onSubViewChange(null);
  }, [onSubViewChange, translate, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 50, format: (_: any, __: any, index?: number) => (index ?? 0) + 1 },
    { id: "code", label: translate("Code"), minWidth: 100 },
    { id: "ratingModel", label: translate("Rating Model"), minWidth: 250 },
  ];

  const initialFormData = {
    code: "",
    ratingModel: "",
    isActive: true,
    items: [],
    selectedItemIds: [],
  };

  const handleSave = async (data: any) => {
    const errors = [];
    if (!CheckemptyString(data.code?.trim())) errors.push(translate("Code is required"));
    if (!CheckemptyString(data.ratingModel?.trim())) errors.push(translate("Rating Model is required"));
    if (!(data.items || []).some((item: any) => !item.delete)) {
      errors.push(translate("At least one Rating Detail is required"));
    }

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    setApiLoading(true);
    try {
      const payload = { ...data };

      let saved: any;
      if (data.id) {
        const response: any = await PutService("/api/HumanResource/RatingModel/v3", payload);
        saved = response?.data;
      } else {
        const response: any = await PostService("/api/HumanResource/RatingModel/v3", payload);
        saved = response?.data;
      }
      setToasterMessages([
        data.id
          ? translate("Record updated successfully")
          : translate("Record saved successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
      // The grid row carries nested rating details, so refetch rather than
      // reconstruct the cached shape from the form payload.
      dispatch(ratingModelSlice.fetchAll(true));
      return true;
    } catch (error: any) {
      const message = error?.response?.data || error?.message || "Failed to save rating model";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    } finally {
      setApiLoading(false);
    }
  };

  const handleDelete = async (row: any) => {
    setApiLoading(true);
    try {
      await DeleteService(`/api/HumanResource/RatingModel/v3/${row.id}`);
      setToasterMessages([translate("Record deleted successfully")]);
      setToasterSeverity("error");
      setShowToaster(true);
      dispatch(ratingModelSlice.actions.removeRow(row.id));
    } catch (error: any) {
      const message = error?.response?.data || error?.message || "Failed to delete rating model";
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw error;
    } finally {
      setApiLoading(false);
    }
  };

  const renderExpandIcon = (expanded: boolean) => (
    <Box
      sx={{
        width: 22,
        height: 22,
        borderRadius: "50%",
        bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color: "var(--lms-primary)",
        position: "relative",
        transition: "transform 0.3s ease",
      }}
    >
      <Box sx={{ width: 10, height: "2px", bgcolor: "currentColor" }} />
      <Box
        sx={{
          width: "2px",
          height: 10,
          bgcolor: "currentColor",
          position: "absolute",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(90deg)" : "rotate(0deg)",
          opacity: expanded ? 0 : 1,
        }}
      />
    </Box>
  );

  const renderForm = (
    data: any,
    setData: any,
    _mode: string,
    isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    const selectedItemIds = data.selectedItemIds || [];
    const itemRows = data.items || [];

    const handleAddRow = () => {
      const newItems = [
        ...itemRows,
        {
          id: -Date.now(),
          code: "",
          key: "",
          description: "",
          factor: 0,
        },
      ];
      setData({ ...data, items: newItems });
    };

    const handleDeleteRows = () => {
      const newItems = itemRows
        .map((item: any) => {
          if (selectedItemIds.includes(item.id)) {
            return { ...item, delete: true };
          }
          return item;
        })
        .filter((item: any) => !(item.delete && item.id < 0));

      setData({ ...data, items: newItems, selectedItemIds: [] });
    };

    const updateItemById = (id: any, key: string, value: any) => {
      const newItems = itemRows.map((item: any) =>
        item.id === id ? { ...item, [key]: value } : item
      );
      setData({ ...data, items: newItems });
    };

    const displayItems = itemRows.filter((row: any) => !row.delete);

    return (
      <FormThemeWrapper>
        <Box sx={{ p: 0 }}>
          {/* Section Header */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 3,
              cursor: "pointer",
              userSelect: "none",
              "&:hover": {
                "& .expand-icon-container": {
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
                },
              },
            }}
            onClick={() => setIsGeneralExpanded(!isGeneralExpanded)}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
              <Box
                sx={{
                  width: 4,
                  height: 18,
                  bgcolor: "var(--lms-primary)",
                  borderRadius: 1,
                }}
              />
              <Typography
                sx={{
                  fontWeight: 800,
                  color: "var(--lms-text-main)",
                  fontSize: "0.95rem",
                }}
              >
                <T>Basic Information</T>
              </Typography>
            </Box>
            <Box
              className="expand-icon-container"
              sx={{
                transition: "background-color 0.3s ease",
                borderRadius: "50%",
                p: 0.5,
              }}
            >
              {renderExpandIcon(isGeneralExpanded)}
            </Box>
          </Box>

          <Collapse in={isGeneralExpanded}>
            <Grid container spacing={3} sx={{ mb: 4 }}>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Code")}
                  placeholder={translate("Enter Code")}
                  value={data.code || ""}
                  onChange={(e) => setData({ ...data, code: e.target.value })}
                  disabled={isReadOnly}
                  error={isSubmitted && !data.code}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormInput
                  fullWidth
                  size="small"
                  label={translate("Rating Model")}
                  placeholder={translate("Enter Rating Model")}
                  value={data.ratingModel || ""}
                  onChange={(e) =>
                    setData({ ...data, ratingModel: e.target.value })
                  }
                  disabled={isReadOnly}
                  error={isSubmitted && !data.ratingModel}
                  required
                />
              </Grid>

              <Grid item xs={12}>
                <LmsCheckbox
                  checked={data.isActive}
                  onChange={(checked) =>
                    setData({
                      ...data,
                      isActive: checked,
                    })
                  }
                  formControlSx={{ m: 0 }}
                  label={
                    <Typography
                      sx={{
                        fontSize: "0.85rem",
                        fontWeight: 600,
                        color: "var(--lms-text-main)",
                      }}
                    >
                      <T>Mark as Active Rating Model</T>
                    </Typography>
                  }
                />
              </Grid>
            </Grid>
          </Collapse>

          {/* Rating Items Section */}
          <Box sx={{ mt: 4 }}>
            <SchemaTable
              title={""}
              columns={[
                {
                  id: "sNo",
                  label: translate("S.No"),
                  minWidth: 50,
                  format: (_: any, __: any, index?: number) =>
                    (index ?? 0) + 1,
                },
                {
                  id: "code",
                  label: translate("Code"),
                  minWidth: 100,
                  format: (val: any, row: any) => (
                    <FormInput
                      fullWidth
                      size="small"
                      variant="standard"
                      value={val || ""}
                      onChange={(e) =>
                        updateItemById(row.id, "code", e.target.value)
                      }
                      disabled={isReadOnly}
                      InputProps={{ disableUnderline: true }}
                      sx={{ fontSize: "0.75rem" }}
                    />
                  ),
                },
                {
                  id: "key",
                  label: translate("Key"),
                  minWidth: 150,
                  format: (val: any, row: any) => (
                    <FormInput
                      fullWidth
                      size="small"
                      variant="standard"
                      value={val || ""}
                      onChange={(e) =>
                        updateItemById(row.id, "key", e.target.value)
                      }
                      disabled={isReadOnly}
                      InputProps={{ disableUnderline: true }}
                      sx={{ fontSize: "0.75rem" }}
                    />
                  ),
                },
                {
                  id: "description",
                  label: translate("Description"),
                  minWidth: 250,
                  format: (val: any, row: any) => (
                    <FormInput
                      fullWidth
                      size="small"
                      variant="standard"
                      value={val || ""}
                      onChange={(e) =>
                        updateItemById(row.id, "description", e.target.value)
                      }
                      disabled={isReadOnly}
                      InputProps={{ disableUnderline: true }}
                      sx={{ fontSize: "0.75rem" }}
                    />
                  ),
                },
                {
                  id: "factor",
                  label: translate("Factor"),
                  minWidth: 100,
                  format: (val: any, row: any) => (
                    <FormInput
                      fullWidth
                      size="small"
                      type="number"
                      variant="standard"
                      value={val || 0}
                      onChange={(e) =>
                        updateItemById(
                          row.id,
                          "factor",
                          parseFloat(e.target.value),
                        )
                      }
                      disabled={isReadOnly}
                      InputProps={{ disableUnderline: true }}
                      sx={{ fontSize: "0.75rem" }}
                    />
                  ),
                },
              ]}
              rows={displayItems}
              hideTitle
              hideBreadcrumbs
              pagination={false}
              filtersEnabled={false}
              downloadEnabled={false}
              actions={false}
              checkboxSelection={!isReadOnly}
              selectedIds={selectedItemIds}
              onSelectionChange={(ids) =>
                setData({ ...data, selectedItemIds: ids })
              }
              maxHeight={350}
              onAddClick={handleAddRow}
              extraToolbarItemsLeft={
                !isReadOnly && (
                  <Box sx={{ display: "flex", gap: 1 }}>
                    <Button
                      size="small"
                      startIcon={<DeleteIcon />}
                      onClick={handleDeleteRows}
                      disabled={selectedItemIds.length === 0}
                      sx={{
                        textTransform: "none",
                        fontWeight: 700,
                        color: "#EF4444",
                        "&:hover": { bgcolor: "rgba(239, 68, 68, 0.05)" },
                      }}
                    >
                      <T>Delete</T>
                    </Button>
                  </Box>
                )
              }
            />
          </Box>
        </Box>
      </FormThemeWrapper>
    );
  };

  return (
    <>
      {viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box
            sx={{
              p: 2,
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <Typography
              variant="h5"
              sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
            >
              {viewMode === "new"
                ? translate("New Rating Model")
                : viewMode === "edit"
                  ? translate("Edit Rating Model")
                  : translate("View Rating Model")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2 }}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => setViewMode("list")}
                sx={{
                  borderRadius: "10px",
                  textTransform: "none",
                  fontWeight: 700,
                }}
              >
                {translate("Back to List")}
              </Button>
              {viewMode !== "view" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={async () => {
                    setIsSubmitted(true);
                    const ok = await handleSave(formData);
                    if (ok !== false) {
                      setViewMode("list");
                      setIsSubmitted(false);
                    }
                  }}
                  sx={{
                    borderRadius: "10px",
                    textTransform: "none",
                    fontWeight: 700,
                    px: 4,
                  }}
                >
                  {translate("Save")}
                </Button>
              )}
            </Box>
          </Box>
          <Box sx={{ px: 2, pb: 2 }}>
            {renderForm(formData, setFormData, viewMode, viewMode === "view", isSubmitted)}
          </Box>
        </Box>
      ) : (
        <SchemaTable
          title={translate("Rating Model")}
          addButtonLabel={translate("New")}
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          maxWidth="lg"
          onAddClick={() => {
            setFormData({ ...initialFormData });
            setIsSubmitted(false);
            setViewMode("new");
          }}
          onEditClick={(row: any) => {
            setFormData({ ...row });
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            setFormData({ ...row });
            setIsSubmitted(false);
            setViewMode("view");
          }}
          hideTitle
          showAddButton={true}
          showToaster={false}
          rowHoverBg="rgba(var(--lms-primary-rgb), 0.05)"
        />
      )}
      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
      {loading && <LmsOverlayLoader />}
    </>
  );
};

export default RatingModel;
