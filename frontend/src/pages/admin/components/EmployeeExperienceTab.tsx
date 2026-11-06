import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Box, Grid, Typography } from "@mui/material";
import dayjs from "dayjs";
import { SchemaTable } from "@/components/SchemaTable";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";

interface LookupOption {
  id: number;
  code?: string;
  name: string;
}

interface EmployeeExperienceRow {
  id: number | string | null;
  code: string;
  employeeId?: number;
  businessNatureId: number | "";
  businessNature: string;
  company: string;
  department: string;
  designation: string;
  startDate: string;
  endDate: string;
  salary: number | "";
  address: string;
  phone: string;
  email: string;
  web: string;
  responsibility: string;
  contactPerson: string;
  personDesignation: string;
  personAddress: string;
}

interface EmployeeExperienceTabProps {
  employeeId: string | undefined;
}

const API = "/api/HumanResource/EREmpExperience/v3";
const PHONE_DIGIT_LENGTH = 11;

const onlyDigits = (value: string) => value.replace(/\D/g, "");

const formatPhoneNo = (value: string) => {
  const digits = onlyDigits(value).slice(0, PHONE_DIGIT_LENGTH);
  if (digits.length <= 4) return digits;
  return `${digits.slice(0, 4)}-${digits.slice(4)}`;
};

const createEmptyRow = (): EmployeeExperienceRow => ({
  id: null,
  code: "",
  businessNatureId: "",
  businessNature: "",
  company: "",
  department: "",
  designation: "",
  startDate: "",
  endDate: "",
  salary: "",
  address: "",
  phone: "",
  email: "",
  web: "",
  responsibility: "",
  contactPerson: "",
  personDesignation: "",
  personAddress: "",
});

const itemsFrom = (payload: any): any[] =>
  payload?.items ??
  payload?.Items ??
  payload?.data?.items ??
  payload?.data?.Items ??
  payload?.data ??
  payload?.Data ??
  (Array.isArray(payload) ? payload : []);

const normalizeLookup = (payload: any): LookupOption[] =>
  itemsFrom(payload)
    .map((item: any) => ({
      id: Number(item.id ?? item.Id ?? 0),
      code: item.code ?? item.Code ?? "",
      name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? "",
    }))
    .filter((item: LookupOption) => item.id > 0);

const normalizeRow = (item: any): EmployeeExperienceRow => ({
  id: item.id ?? item.Id ?? null,
  code: item.code ?? item.Code ?? "",
  employeeId: item.employeeId ?? item.EmployeeId ?? item.employeeid,
  businessNatureId:
    item.businessNatureId ??
    item.BusinessNatureId ??
    item.busnatureid ??
    "",
  businessNature:
    item.businessNature ??
    item.BusinessNature ??
    item.lov?.stxt ??
    item.Lov?.Stxt ??
    "",
  company: item.company ?? item.Company ?? "",
  department: item.department ?? item.Department ?? "",
  designation: item.designation ?? item.Designation ?? "",
  startDate: item.startDate ?? item.StartDate ?? item.startdate ?? "",
  endDate: item.endDate ?? item.EndDate ?? item.enddate ?? "",
  salary: item.salary ?? item.Salary ?? "",
  address: item.address ?? item.Address ?? "",
  phone: formatPhoneNo(item.phone ?? item.Phone ?? ""),
  email: item.email ?? item.Email ?? "",
  web: item.web ?? item.Web ?? "",
  responsibility: item.responsibility ?? item.Responsibility ?? "",
  contactPerson: item.contactPerson ?? item.ContactPerson ?? item.contactper ?? "",
  personDesignation:
    item.personDesignation ??
    item.PersonDesignation ??
    item.persondesignation ??
    "",
  personAddress:
    item.personAddress ?? item.PersonAddress ?? item.personaddress ?? "",
});

export const EmployeeExperienceTab: React.FC<EmployeeExperienceTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeExperienceRow[]>([]);
  const [businessNatures, setBusinessNatures] = useState<LookupOption[]>([]);
  const [businessLookupOpen, setBusinessLookupOpen] = useState(false);
  const [activeFormSetter, setActiveFormSetter] = useState<React.Dispatch<
    React.SetStateAction<any>
  > | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    void GetService(`${API}/lookups/business-natures`)
      .then((response: any) =>
        setBusinessNatures(normalizeLookup(response?.data ?? response)),
      )
      .finally(() => setLoading(false));
  }, []);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `${API}?employeeId=${employeeId}&pageSize=1000`,
      );
      setRows(itemsFrom(response).map(normalizeRow));
    } finally {
      setLoading(false);
    }
  }, [employeeId]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  const loadRow = async (row: EmployeeExperienceRow) => {
    if (!row.id) return row;
    try {
      setLoading(true);
      const response: any = await GetService(`${API}/${row.id}`);
      return normalizeRow(response?.data ?? response);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async (data: EmployeeExperienceRow) => {
    if (!employeeId) throw new Error(translate("Employee context is missing"));
    if (
      data.startDate &&
      data.endDate &&
      dayjs(data.startDate).isAfter(dayjs(data.endDate), "day")
    ) {
      throw new Error(translate("Start Date cannot be greater than End Date"));
    }
    if (data.salary !== "" && Number(data.salary) < 0) {
      throw new Error(translate("Salary cannot be negative"));
    }

    const payload = {
      id: typeof data.id === "number" ? data.id : undefined,
      code: data.code?.trim() || "",
      employeeId: Number(employeeId),
      businessNatureId: data.businessNatureId ? Number(data.businessNatureId) : null,
      company: data.company?.trim() || "",
      department: data.department?.trim() || "",
      designation: data.designation?.trim() || "",
      startDate: data.startDate || null,
      endDate: data.endDate || null,
      salary: data.salary === "" ? null : Number(data.salary),
      address: data.address?.trim() || "",
      phone: data.phone?.trim() || "",
      email: data.email?.trim() || "",
      web: data.web?.trim() || "",
      responsibility: data.responsibility?.trim() || "",
      contactPerson: data.contactPerson?.trim() || "",
      personDesignation: data.personDesignation?.trim() || "",
      personAddress: data.personAddress?.trim() || "",
      personPhone: null,
      personEmail: null,
      personWeb: null,
    };

    try {
      setLoading(true);
      if (typeof data.id === "number") {
        await PutService(API, payload);
      } else {
        await PostService(API, payload);
      }

      await fetchRows();
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (row: EmployeeExperienceRow) => {
    if (typeof row.id !== "number") return;
    try {
      setLoading(true);
      await DeleteService(`${API}/${row.id}`);
      await fetchRows();
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () => [
      {
        id: "sno",
        label: translate("S.No"),
        minWidth: 70,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "businessNature",
        label: translate("Business Nature"),
        minWidth: 160,
      },
      { id: "company", label: translate("Company"), minWidth: 180 },
      { id: "department", label: translate("Department"), minWidth: 150 },
      { id: "designation", label: translate("Designation"), minWidth: 150 },
      {
        id: "startDate",
        label: translate("Start Date"),
        minWidth: 130,
        format: (value: any) => (value ? dayjs(value).format("DD MMM YYYY") : "-"),
      },
      {
        id: "endDate",
        label: translate("End Date"),
        minWidth: 130,
        format: (value: any) => (value ? dayjs(value).format("DD MMM YYYY") : "-"),
      },
      {
        id: "salary",
        label: translate("Salary"),
        minWidth: 120,
        align: "right" as const,
      },
    ],
    [translate],
  );

  const renderSectionTitle = (label: string) => (
    <Grid item xs={12}>
      <Typography
        sx={{
          mt: 1,
          mb: 0.5,
          fontSize: "0.9rem",
          fontWeight: 900,
          color: "var(--lms-text-main)",
        }}
      >
        {translate(label)}
      </Typography>
    </Grid>
  );

  const renderForm = (
    formData: EmployeeExperienceRow,
    setFormData: React.Dispatch<React.SetStateAction<EmployeeExperienceRow>>,
    _mode: "new" | "edit" | "view",
    isReadOnly: boolean,
    isSubmitted: boolean,
  ) => {
    const patch = (updates: Partial<EmployeeExperienceRow>) =>
      setFormData((prev) => ({ ...prev, ...updates }));

    return (
      <Box sx={{ pt: 1 }}>
        {loading && <LmsOverlayLoader />}
        <Grid container spacing={2}>
          {renderSectionTitle("Basic Information")}
          <Grid item xs={12} md={4}>
            <LookupDropdown
              label={translate("Business Nature")}
              required
              disabled={isReadOnly}
              value={formData.businessNatureId}
              options={businessNatures}
              getOptionLabel={(option) => option.name || ""}
              onSearchClick={() => {
                setActiveFormSetter(() => setFormData);
                setBusinessLookupOpen(true);
              }}
              onSelect={(option) =>
                patch({
                  businessNatureId: option?.id ?? "",
                  businessNature: option?.name ?? "",
                })
              }
              error={isSubmitted && !formData.businessNatureId}
              placeholder={translate("Select Business Nature")}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Company")}
              required
              disabled={isReadOnly}
              value={formData.company}
              onChange={(event) =>
                patch({ company: event.target.value.slice(0, 150) })
              }
              error={isSubmitted && !formData.company?.trim()}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Department")}
              required
              disabled={isReadOnly}
              value={formData.department}
              onChange={(event) =>
                patch({ department: event.target.value.slice(0, 50) })
              }
              error={isSubmitted && !formData.department?.trim()}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Designation")}
              disabled={isReadOnly}
              value={formData.designation}
              onChange={(event) =>
                patch({ designation: event.target.value.slice(0, 50) })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormDatePicker
              label={translate("Start Date")}
              required
              disabled={isReadOnly}
              value={formData.startDate ? dayjs(formData.startDate) : null}
              onChange={(value) =>
                patch({ startDate: value ? value.toISOString() : "" })
              }
              maxDate={formData.endDate ? dayjs(formData.endDate) : undefined}
              format="DD-MMM-YYYY"
              error={isSubmitted && (!formData.startDate || (
                !!formData.endDate && dayjs(formData.startDate).isAfter(dayjs(formData.endDate), "day")
              ))}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormDatePicker
              label={translate("End Date")}
              required
              disabled={isReadOnly}
              value={formData.endDate ? dayjs(formData.endDate) : null}
              onChange={(value) =>
                patch({ endDate: value ? value.toISOString() : "" })
              }
              minDate={formData.startDate ? dayjs(formData.startDate) : undefined}
              format="DD-MMM-YYYY"
              error={isSubmitted && (!formData.endDate || (
                !!formData.startDate && dayjs(formData.startDate).isAfter(dayjs(formData.endDate), "day")
              ))}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Salary")}
              required
              type="number"
              disabled={isReadOnly}
              value={formData.salary}
              onChange={(event) =>
                patch({
                  salary:
                    event.target.value === "" ? "" : Math.max(0, Number(event.target.value)),
                })
              }
              inputProps={{ min: 0 }}
              error={isSubmitted && (formData.salary === "" || Number(formData.salary) < 0)}
            />
          </Grid>
          <Grid item xs={12} md={8}>
            <FormInput
              label={translate("Job Responsibility")}
              multiline
              rows={3}
              disabled={isReadOnly}
              value={formData.responsibility}
              onChange={(event) =>
                patch({ responsibility: event.target.value.slice(0, 1500) })
              }
            />
          </Grid>

          {renderSectionTitle("Company Contact")}
          <Grid item xs={12} md={8}>
            <FormInput
              label={translate("Address")}
              multiline
              rows={3}
              disabled={isReadOnly}
              value={formData.address}
              onChange={(event) =>
                patch({ address: event.target.value.slice(0, 500) })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Phone")}
              disabled={isReadOnly}
              value={formData.phone}
              inputMode="tel"
              debounceMs={0}
              onChange={(event) =>
                patch({ phone: formatPhoneNo(event.target.value) })
              }
              maxLength={12}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Email")}
              type="email"
              disabled={isReadOnly}
              value={formData.email}
              onChange={(event) =>
                patch({ email: event.target.value.slice(0, 50) })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Web")}
              disabled={isReadOnly}
              value={formData.web}
              onChange={(event) =>
                patch({ web: event.target.value.slice(0, 50) })
              }
            />
          </Grid>

          {renderSectionTitle("Contact Person")}
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Contact Person")}
              disabled={isReadOnly}
              value={formData.contactPerson}
              onChange={(event) =>
                patch({ contactPerson: event.target.value.slice(0, 150) })
              }
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <FormInput
              label={translate("Person Designation")}
              disabled={isReadOnly}
              value={formData.personDesignation}
              onChange={(event) =>
                patch({ personDesignation: event.target.value.slice(0, 150) })
              }
            />
          </Grid>
        </Grid>
      </Box>
    );
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Experience")}
        columns={columns}
        rows={rows}
        hideTitle
        stickyHeader
        pagination={false}
        filtersEnabled={false}
        initialFormData={createEmptyRow()}
        renderForm={renderForm as any}
        onSave={handleSave as any}
        onDelete={handleDelete as any}
        loadRow={loadRow as any}
        maxWidth="lg"
        mandatoryFields={[
          { key: "businessNatureId", label: translate("Business Nature") },
          { key: "company", label: translate("Company") },
          { key: "department", label: translate("Department") },
          { key: "startDate", label: translate("Start Date") },
          { key: "endDate", label: translate("End Date") },
          { key: "salary", label: translate("Salary") },
        ]}
        showToaster
      />

      <GenericSearchModal
        open={businessLookupOpen}
        onClose={() => setBusinessLookupOpen(false)}
        title={translate("Business Nature")}
        columns={[
          { id: "id", label: translate("Id"), minWidth: 90 },
          { id: "code", label: translate("Code"), minWidth: 110 },
          { id: "name", label: translate("Name"), minWidth: 240 },
        ]}
        data={businessNatures}
        selectedField="id"
        onSelect={(row: LookupOption) => {
          activeFormSetter?.((prev: EmployeeExperienceRow) => ({
            ...prev,
            businessNatureId: row?.id ?? "",
            businessNature: row?.name ?? "",
          }));
          setBusinessLookupOpen(false);
        }}
      />
    </>
  );
};
