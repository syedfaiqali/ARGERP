import React, { useEffect, useState } from "react";
import { Box, Button, Grid, Typography } from "@mui/material";
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
} from "@mui/icons-material";
import { FormInput } from "@/components/FormInput";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { SchemaTable } from "@/components/SchemaTable";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import {
  DeleteService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { useAppDispatch, useCachedRows } from "@/hooks";
import { gradeSlice } from "@/store/humanResourceSlices";

interface Props {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const emptyGrade = { id: 0, code: "", minimumSalary: "", maximumSalary: "" };

const Grade: React.FC<Props> = ({ onSubViewChange, resetKey }) => {
  const { translate } = useTranslation();
  const dispatch = useAppDispatch();
  // Rows come from the store, so returning to this screen reuses the cache.
  const { rows, loading: rowsLoading } = useCachedRows<any>(
    "hrGrade",
    gradeSlice.fetchAll,
  );
  const [form, setForm] = useState<any>(emptyGrade);
  const [mode, setMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [apiLoading, setApiLoading] = useState(false);
  const loading = rowsLoading || apiLoading;
  const [toast, setToast] = useState({
    open: false,
    severity: "error" as "error" | "success",
    messages: [] as string[],
  });

  useEffect(() => {
    onSubViewChange?.(
      mode === "list"
        ? null
        : translate(
            `${mode === "new" ? "New" : mode === "edit" ? "Edit" : "View"} Grade`,
          ),
    );
    return () => onSubViewChange?.(null);
  }, [mode, onSubViewChange, translate]);
  useEffect(() => {
    if (resetKey) setMode("list");
  }, [resetKey]);

  const save = async () => {
    const minimumSalary = Number(form.minimumSalary);
    const maximumSalary = Number(form.maximumSalary);
    if (
      !form.code.trim() ||
      form.minimumSalary === "" ||
      form.maximumSalary === ""
    ) {
      setToast({
        open: true,
        severity: "error",
        messages: [
          translate(
            "Grade Code, Minimum Salary and Maximum Salary are required",
          ),
        ],
      });
      return;
    }
    if (
      !Number.isFinite(minimumSalary) ||
      !Number.isFinite(maximumSalary) ||
      minimumSalary > maximumSalary
    ) {
      setToast({
        open: true,
        severity: "error",
        messages: [translate("Minimum Salary cannot exceed Maximum Salary")],
      });
      return;
    }
    try {
      setApiLoading(true);
      const payload = {
        id: Number(form.id || 0),
        code: form.code.trim(),
        minimumSalary,
        maximumSalary,
      };
      let saved: any;
      if (payload.id) {
        const response: any = await PutService(
          `/api/HumanResource/Grade/v3/${payload.id}`,
          payload,
        );
        saved = response?.data;
      } else {
        const response: any = await PostService("/api/HumanResource/Grade/v3", payload);
        saved = response?.data;
      }
      // Patch the cached row instead of reloading the whole list.
      const savedId = Number(saved?.id ?? payload.id ?? 0);
      if (savedId > 0) {
        dispatch(gradeSlice.actions.upsertRow({ ...payload, id: savedId }));
      } else {
        dispatch(gradeSlice.fetchAll(true));
      }
      setMode("list");
      setToast({
        open: true,
        severity: "success",
        messages: [translate("Grade saved successfully")],
      });
    } catch (error: any) {
      setToast({
        open: true,
        severity: "error",
        messages: [error?.response?.data ?? translate("Failed to save grade")],
      });
    } finally {
      setApiLoading(false);
    }
  };

  if (mode !== "list")
    return (
      <FormThemeWrapper>
        <Box sx={{ p: 2 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
            <Typography variant="h6" sx={{ fontWeight: 800 }}>
              {translate(
                mode === "new"
                  ? "New Grade"
                  : mode === "edit"
                    ? "Edit Grade"
                    : "View Grade",
              )}
            </Typography>
            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => setMode("list")}
            >
              {translate("Back")}
            </Button>
          </Box>
          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("Grade Code")}
                required
                disabled={mode === "view"}
                value={form.code}
                onChange={(e) => setForm({ ...form, code: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("Minimum Salary")}
                required
                type="number"
                disabled={mode === "view"}
                value={form.minimumSalary}
                onChange={(e) =>
                  setForm({ ...form, minimumSalary: e.target.value })
                }
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <FormInput
                label={translate("Maximum Salary")}
                required
                type="number"
                disabled={mode === "view"}
                value={form.maximumSalary}
                onChange={(e) =>
                  setForm({ ...form, maximumSalary: e.target.value })
                }
              />
            </Grid>
          </Grid>
          {mode !== "view" && (
            <Box sx={{ mt: 2, display: "flex", justifyContent: "flex-end" }}>
              <Button
                variant="contained"
                startIcon={<SaveIcon />}
                onClick={() => void save()}
              >
                {translate("Save")}
              </Button>
            </Box>
          )}
        </Box>
        <LmsToaster
          open={toast.open}
          severity={toast.severity}
          messages={toast.messages}
          onClose={() => setToast({ ...toast, open: false })}
        />
      </FormThemeWrapper>
    );

  return (
    <>
      <>{loading && <LmsOverlayLoader />}</>
      <SchemaTable
        title={translate("Grade")}
        addButtonLabel={translate("New")}
        columns={[
          { id: "code", label: translate("Grade Code"), minWidth: 160 },
          {
            id: "minimumSalary",
            label: translate("Min Salary"),
            minWidth: 150,
          },
          {
            id: "maximumSalary",
            label: translate("Max Salary"),
            minWidth: 150,
          },
        ]}
        rows={rows}
        onAddClick={() => {
          setForm(emptyGrade);
          setMode("new");
        }}
        onEditClick={(row: any) => {
          setForm({ ...row });
          setMode("edit");
        }}
        onViewClick={(row: any) => {
          setForm({ ...row });
          setMode("view");
        }}
        onDelete={async (row: any) => {
          await DeleteService(`/api/HumanResource/Grade/v3/${row.id}`);
          dispatch(gradeSlice.actions.removeRow(row.id));
        }}
        // Grade is intentionally frontend-managed until its UT menu record is created.
        useActionRights={false}
      />
      <LmsToaster
        open={toast.open}
        severity={toast.severity}
        messages={toast.messages}
        onClose={() => setToast({ ...toast, open: false })}
      />
    </>
  );
};

export default Grade;
