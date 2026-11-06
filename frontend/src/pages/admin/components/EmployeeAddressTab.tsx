import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Box, Button, Chip, Typography } from "@mui/material";
import {
  DeleteOutline as DeleteIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { Modal } from "@/components/Modal";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { FormInput } from "@/components/FormInput";
import { LmsCheckbox } from "@/components/LmsCheckbox";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  DeleteService,
  GetService,
  PostService,
  PutService,
  ToCheckArrayTypeAndLength,
} from "@/constants/globalItems";

interface LookupOption {
  id: number;
  code?: string;
  name: string;
  countryId?: number;
  stateId?: number;
}

interface EmployeeAddressRow {
  id: number | string;
  code: string;
  name: string;
  addressTypeId: number | "";
  countryId: number | "";
  stateId: number | "";
  cityId: number | "";
  address: string;
  primaryAddress: boolean;
  contactName: string;
  landline: string;
  ext: string;
  mobile: string;
}

interface EmployeeAddressTabProps {
  employeeId: string | undefined;
}

type LookupKind = "addressType" | "country" | "state" | "city";
const ADDRESS_PERMISSION_SCREEN_NAME = "Address";
const ADDRESS_PERMISSION_PARENT_NAME = "Employee";

const emptyAddress = (): EmployeeAddressRow => ({
  id: "",
  code: "",
  name: "",
  addressTypeId: "",
  countryId: "",
  stateId: "",
  cityId: "",
  address: "",
  primaryAddress: false,
  contactName: "",
  landline: "",
  ext: "",
  mobile: "",
});

const itemsFrom = (payload: any): any[] =>
  payload?.items ??
  payload?.Items ??
  payload?.data?.items ??
  payload?.data?.Items ??
  payload?.data ??
  payload?.Data ??
  (Array.isArray(payload) ? payload : []);

const normalizeLov = (payload: any): LookupOption[] =>
  itemsFrom(payload)
    .map((item: any) => ({
      id: Number(item.id ?? item.Id ?? item.lovid ?? item.Lovid ?? 0),
      code: item.code ?? item.Code ?? item.vtype ?? "",
      name: item.name ?? item.Name ?? item.stxt ?? item.Stxt ?? item.ltxt ?? "",
    }))
    .filter((item: LookupOption) => item.id > 0);

const normalizeCountry = (payload: any): LookupOption[] => {
  const newArray = ToCheckArrayTypeAndLength(payload) ? payload : [];
  return newArray
    .map((item: any) => ({
      id: Number(item.countryid ?? item.countryId ?? item.CountryId ?? item.id ?? item.Id ?? 0),
      code: item.countrycode ?? item.countryCode ?? item.CountryCode ?? item.code ?? item.Code ?? "",
      name: item.countryname ?? item.countryName ?? item.CountryName ?? item.name ?? item.Name ?? "",
    }))
    .filter((item: LookupOption) => item.id > 0);
};

const normalizeState = (payload: any): LookupOption[] => {
  const newArray = ToCheckArrayTypeAndLength(payload) ? payload : [];
  return newArray
    .map((item: any) => ({
      id: Number(item.stateid ?? item.stateId ?? item.StateId ?? item.id ?? item.Id ?? 0),
      code: item.statecode ?? item.stateCode ?? item.StateCode ?? item.code ?? item.Code ?? "",
      name: item.statename ?? item.stateName ?? item.StateName ?? item.stxt ?? item.Stxt ?? item.name ?? item.Name ?? "",
      countryId: Number(item.countryid ?? item.countryId ?? item.CountryId ?? 0) || undefined,
      stateId: Number(item.stateid ?? item.stateId ?? item.StateId ?? item.id ?? item.Id ?? 0) || undefined,
    }))
    .filter((item: LookupOption) => item.id > 0);
};

const normalizeCity = (payload: any): LookupOption[] => {
  const newArray = ToCheckArrayTypeAndLength(payload) ? payload : [];
  return newArray
    .map((item: any) => ({
      id: Number(item.cityid ?? item.cityId ?? item.CityId ?? item.id ?? item.Id ?? 0),
      code: item.citycode ?? item.cityCode ?? item.CityCode ?? item.code ?? item.Code ?? "",
      name: item.cityname ?? item.cityName ?? item.CityName ?? item.citystxt ?? item.Citystxt ?? item.name ?? item.Name ?? "",
      countryId: Number(item.countryid ?? item.countryId ?? item.CountryId ?? item.ctryid ?? item.CtryId ?? 0) || undefined,
      stateId: Number(item.stateid ?? item.stateId ?? item.StateId ?? 0) || undefined,
    }))
    .filter((item: LookupOption) => item.id > 0);
};

const normalizeRow = (item: any): EmployeeAddressRow => ({
  id: item.id ?? item.Id,
  code: item.code ?? item.Code ?? "",
  name: item.name ?? item.Name ?? item.ltxt ?? item.Ltxt ?? "",
  addressTypeId: item.addressTypeId ?? item.AddressTypeId ?? item.adtypeid ?? "",
  countryId: item.countryId ?? item.CountryId ?? item.countryid ?? "",
  stateId: item.stateId ?? item.StateId ?? item.stateid ?? "",
  cityId: item.cityId ?? item.CityId ?? item.cityid ?? "",
  address: item.address ?? item.Address ?? "",
  primaryAddress: Boolean(item.primaryAddress ?? item.PrimaryAddress ?? item.primaryadd),
  contactName: item.contactName ?? item.ContactName ?? item.contactname ?? "",
  landline: item.landline ?? item.Landline ?? "",
  ext: item.ext ?? item.Ext ?? "",
  mobile: item.mobile ?? item.Mobile ?? "",
});

const onlyDigits = (value: string) => value.replace(/\D/g, "");

const formatContactNo = (value: string) => {
  const digits = onlyDigits(value).slice(0, 11);
  if (!digits) return "";

  const firstPart = digits.slice(0, 4).padEnd(4, "_");
  const secondPart = digits.slice(4, 11).padEnd(7, "_");
  return `${firstPart}-${secondPart}`;
};

const LANDLINE_MAX_LENGTH = 5;
const MOBILE_DIGIT_LENGTH = 11;

export const EmployeeAddressTab: React.FC<EmployeeAddressTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [rows, setRows] = useState<EmployeeAddressRow[]>([]);
  const [formData, setFormData] = useState<EmployeeAddressRow>(emptyAddress);
  const [modalMode, setModalMode] = useState<"new" | "edit" | "view">("new");
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIds, setSelectedIds] = useState<any[]>([]);
  const [addressTypes, setAddressTypes] = useState<LookupOption[]>([]);
  const [countries, setCountries] = useState<LookupOption[]>([]);
  const [states, setStates] = useState<LookupOption[]>([]);
  const [cities, setCities] = useState<LookupOption[]>([]);
  const [lookupKind, setLookupKind] = useState<LookupKind | null>(null);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);
  const caretRestoreFrameRef = useRef<number | null>(null);
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({ open: false, messages: [], severity: "info" });
  const canCreate = true;
  const canUpdate = true;
  const canDelete = true;
  const isViewMode = modalMode === "view";

  const stateOptions = useMemo(
    () =>
      formData.countryId
        ? states.filter((item) => item.countryId === Number(formData.countryId))
        : states,
    [formData.countryId, states],
  );

  const cityOptions = useMemo(
    () =>
      cities.filter((item) => {
        if (formData.stateId) return item.stateId === Number(formData.stateId);
        if (formData.countryId) return item.countryId === Number(formData.countryId);
        return true;
      }),
    [cities, formData.countryId, formData.stateId],
  );

  const lookupData = {
    addressType: addressTypes,
    country: countries,
    state: stateOptions,
    city: cityOptions,
  };

  const lookupTitle = {
    addressType: translate("Address Type"),
    country: translate("Country"),
    state: translate("State"),
    city: translate("City"),
  };

  const fetchLookups = useCallback(async () => {
    try {
      setLoading(true);
      const [addressTypeResp, countryResp, stateResp, cityResp] =
        await Promise.all([
          GetService("/api/HumanResource/Lov/v3/ADDTP"),
          GetService("/api/HumanResource/Lookups/v3/countries"),
          GetService("/api/HumanResource/Lookups/v3/states"),
          GetService("/api/HumanResource/Lookups/v3/cities"),
        ]);

      setAddressTypes(normalizeLov(addressTypeResp?.data ?? addressTypeResp));
      setCountries(normalizeCountry(countryResp?.data?.items ?? countryResp));
      setStates(normalizeState(stateResp?.data?.items ?? stateResp));
      setCities(normalizeCity(cityResp?.data?.items ?? cityResp));
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to load address lookups")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [translate]);

  const fetchRows = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `/api/HumanResource/EREmpaddress/v3?employeeId=${employeeId}&pageSize=1000`,
      );
      setRows(itemsFrom(response).map(normalizeRow));
      setSelectedIds([]);
    } catch {
      setRows([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load addresses")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchLookups();
  }, [fetchLookups]);

  useEffect(() => {
    void fetchRows();
  }, [fetchRows]);

  useEffect(() => {
    return () => {
      if (caretRestoreFrameRef.current !== null) {
        window.cancelAnimationFrame(caretRestoreFrameRef.current);
      }
    };
  }, []);

  const getName = (list: LookupOption[], id: number | "") =>
    list.find((item) => item.id === Number(id))?.name ?? "";

  const displayRows = useMemo(
    () =>
      rows.map((row) => ({
        ...row,
        addressType: getName(addressTypes, row.addressTypeId),
        country: getName(countries, row.countryId),
        state: getName(states, row.stateId),
        city: getName(cities, row.cityId),
      })),
    [addressTypes, cities, countries, rows, states],
  );

  const openNew = () => {
    if (!canCreate) return;
    setFormData(emptyAddress());
    setModalMode("new");
    setIsSubmitted(false);
    setModalOpen(true);
  };

  const openEdit = (row: EmployeeAddressRow) => {
    if (!canUpdate) return;
    setFormData(normalizeRow(row));
    setModalMode("edit");
    setIsSubmitted(false);
    setModalOpen(true);
  };

  const openView = (row: EmployeeAddressRow) => {
    setFormData(normalizeRow(row));
    setModalMode("view");
    setIsSubmitted(false);
    setModalOpen(true);
  };

  const updateForm = (patch: Partial<EmployeeAddressRow>) => {
    if (isViewMode) return;
    setFormData((prev) => ({ ...prev, ...patch }));
  };

  const caretPositionForDigitCount = (formattedValue: string, digitCount: number) => {
    if (digitCount <= 0) return 0;
    let seenDigits = 0;
    for (let index = 0; index < formattedValue.length; index += 1) {
      if (/\d/.test(formattedValue[index])) {
        seenDigits += 1;
        if (seenDigits >= digitCount) return index + 1;
      }
    }
    return formattedValue.length;
  };

  const handleMobileChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const input = event.target;
    const caret = input.selectionStart ?? input.value.length;
    const digitsBeforeCaret = onlyDigits(input.value.slice(0, caret)).length;
    const nextValue = formatContactNo(input.value);
    const nextCaret = caretPositionForDigitCount(nextValue, digitsBeforeCaret);

    updateForm({ mobile: nextValue });
    if (caretRestoreFrameRef.current !== null) {
      window.cancelAnimationFrame(caretRestoreFrameRef.current);
    }
    caretRestoreFrameRef.current = window.requestAnimationFrame(() => {
      caretRestoreFrameRef.current = null;
      if (document.activeElement !== input) return;
      if (input.selectionStart === nextCaret && input.selectionEnd === nextCaret) return;
      input.setSelectionRange(nextCaret, nextCaret);
    });
  };

  const validate = () => {
    if (!formData.addressTypeId) return translate("Address type cannot be blank");
    if (!formData.name.trim()) return translate("Name cannot be blank");
    if (!formData.countryId) return translate("Country cannot be blank");
    if (!formData.stateId) return translate("State cannot be blank");
    if (!formData.cityId) return translate("City cannot be blank");
    if (!formData.address.trim()) return translate("Address cannot be blank");
    if (!formData.contactName.trim()) return translate("Contact Person cannot be blank");
    const mobileDigits = onlyDigits(formData.mobile || "");
    if (mobileDigits && mobileDigits.length !== MOBILE_DIGIT_LENGTH) {
      return translate("Phone Number is invalid");
    }
    return null;
  };

  const save = async () => {
    if (isViewMode) return;
    if (!canUpdate && typeof formData.id === "number") return;
    if (!canCreate && typeof formData.id !== "number") return;
    if (!employeeId) return;
    setIsSubmitted(true);

    const error = validate();
    if (error) {
      setToaster({ open: true, messages: [error], severity: "error" });
      return;
    }

    const payload = {
      id: typeof formData.id === "number" ? formData.id : undefined,
      employeeId: Number(employeeId),
      code: formData.code.trim(),
      name: formData.name.trim(),
      addressTypeId: Number(formData.addressTypeId),
      countryId: Number(formData.countryId),
      stateId: Number(formData.stateId),
      cityId: Number(formData.cityId),
      address: formData.address.trim(),
      primaryAddress: formData.primaryAddress,
      contactName: formData.contactName.trim(),
      landline: formData.landline.replace(/\D/g, "").slice(0, LANDLINE_MAX_LENGTH),
      ext: formData.ext.trim(),
      mobile: formData.mobile.trim(),
    };

    try {
      setLoading(true);
      if (typeof formData.id === "number") {
        await PutService("/api/HumanResource/EREmpaddress/v3", payload);
      } else {
        await PostService("/api/HumanResource/EREmpaddress/v3", payload);
      }
      setIsSubmitted(false);
      setModalOpen(false);
      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Address saved successfully")],
        severity: "success",
      });
    } catch (error: any) {
      setToaster({
        open: true,
        messages: [
          error?.response?.data?.error ||
          error?.response?.data ||
          error?.message ||
          translate("Failed to save address"),
        ],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const deleteSelected = async () => {
    if (!canDelete || selectedIds.length === 0) return;

    try {
      setLoading(true);
      for (const row of rows.filter((item) => selectedIds.includes(item.id))) {
        if (typeof row.id === "number") {
          await DeleteService(`/api/HumanResource/EREmpaddress/v3/${row.id}`);
        }
      }
      await fetchRows();
      setToaster({
        open: true,
        messages: [translate("Selected addresses deleted successfully")],
        severity: "success",
      });
    } catch {
      setToaster({
        open: true,
        messages: [translate("Failed to delete selected addresses")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () => [
      {
        id: "code",
        label: translate("Address ID"),
        minWidth: 110,
      },
      {
        id: "primaryAddress",
        label: translate("Primary"),
        minWidth: 110,
        format: (value: boolean) =>
          value ? (
            <Chip label={translate("Yes")} color="primary" size="small" />
          ) : (
            <Chip label={translate("No")} size="small" />
          ),
      },
      { id: "addressType", label: translate("Address Type"), minWidth: 150 },
      { id: "name", label: translate("Name"), minWidth: 160 },
      { id: "country", label: translate("Country"), minWidth: 130 },
      { id: "city", label: translate("City"), minWidth: 130 },
      { id: "address", label: translate("Address"), minWidth: 260 },
      { id: "mobile", label: translate("Mobile No"), minWidth: 130 },
    ],
    [translate],
  );

  const selectedLookupValue = lookupKind ? formData[`${lookupKind}Id` as keyof EmployeeAddressRow] : undefined;

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <SchemaTable
        title={translate("Address")}
        columns={columns}
        rows={displayRows}
        hideTitle
        stickyHeader
        pagination={false}
        filtersEnabled={false}
        actions
        showAddButton
        showDeleteAction={false}
        checkboxSelection
        selectedIds={selectedIds}
        onSelectionChange={setSelectedIds}
        disableRowSelectionOnClick
        showToaster={false}
        onAddClick={openNew}
        onEditClick={openEdit}
        onViewClick={openView}
        onRowDoubleClick={openView}
        extraToolbarItemsLeft={
          <Button
            variant="outlined"
            color="error"
            startIcon={<DeleteIcon />}
            onClick={deleteSelected}
            disabled={!canDelete || selectedIds.length === 0}
            sx={{ textTransform: "none", fontWeight: 700, borderRadius: 2 }}
          >
            <T>Delete Selected</T>
          </Button>
        }
      />

      <Modal
        open={modalOpen}
        onClose={() => {
          setIsSubmitted(false);
          setModalOpen(false);
        }}
        title={translate("Address")}
        maxWidth="md"
        onSave={save}
        showActions={!isViewMode}
        saveLabel={translate("Save")}
        hideResetButton
      >
        <Box sx={{ display: "grid", gap: 2 }}>
          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)" }}>
            <T>Basic Information</T>
          </Typography>
          <Box sx={{ display: "grid", gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" }, gap: 2 }}>
            <FormInput
              label={translate("Address ID")}
              value={formData.code}
              onChange={(event) => updateForm({ code: event.target.value.slice(0, 10) })}
              disabled
            />
            <LookupDropdown
              label={translate("Address Type*")}
              value={formData.addressTypeId}
              options={addressTypes}
              disabled={isViewMode}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) => updateForm({ addressTypeId: option?.id ?? "" })}
              onSearchClick={() => setLookupKind("addressType")}
              placeholder={translate("Select Address Type")}
            />
            <FormInput
              label={translate("Name*")}
              value={formData.name}
              onChange={(event) => updateForm({ name: event.target.value.slice(0, 50) })}
              disabled={isViewMode}
            />
            <LmsCheckbox
              label={translate("Primary")}
              checked={formData.primaryAddress}
              onChange={(checked) => updateForm({ primaryAddress: checked })}
              disabled={isViewMode}
            />
          </Box>

          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", mt: 1 }}>
            <T>Address</T>
          </Typography>
          <Box sx={{ display: "grid", gridTemplateColumns: { xs: "1fr", md: "1fr 1fr 1fr" }, gap: 2 }}>
            <LookupDropdown
              label={translate("Country*")}
              value={formData.countryId}
              options={countries}
              disabled={isViewMode}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) =>
                updateForm({ countryId: option?.id ?? "", stateId: "", cityId: "" })
              }
              onSearchClick={() => setLookupKind("country")}
              placeholder={translate("Select Country")}
            />
            <LookupDropdown
              label={translate("State*")}
              value={formData.stateId}
              options={stateOptions}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) => updateForm({ stateId: option?.id ?? "", cityId: "" })}
              onSearchClick={() => setLookupKind("state")}
              placeholder={translate("Select State")}
              disabled={isViewMode || !formData.countryId}
            />
            <LookupDropdown
              label={translate("City*")}
              value={formData.cityId}
              options={cityOptions}
              getOptionLabel={(option) => option.name || ""}
              onSelect={(option) => updateForm({ cityId: option?.id ?? "" })}
              onSearchClick={() => setLookupKind("city")}
              placeholder={translate("Select City")}
              disabled={isViewMode || !formData.stateId}
            />
            <Box sx={{ gridColumn: { xs: "1", md: "1 / -1" } }}>
              <FormInput
                label={translate("Address*")}
                value={formData.address}
                onChange={(event) => updateForm({ address: event.target.value.slice(0, 500) })}
                multiline
                rows={3}
                disabled={isViewMode}
              />
            </Box>
          </Box>

          <Typography sx={{ fontWeight: 800, color: "var(--lms-text-main)", mt: 1 }}>
            <T>Contact</T>
          </Typography>
          <Box sx={{ display: "grid", gridTemplateColumns: { xs: "1fr", md: "1fr 1fr 1fr" }, gap: 2 }}>
            <FormInput
              label={translate("Contact Person*")}
              value={formData.contactName}
              onChange={(event) => updateForm({ contactName: event.target.value.slice(0, 50) })}
              sx={{ gridColumn: { xs: "1", md: "1 / -1" } }}
              disabled={isViewMode}
            />
            <FormInput
              label={translate("Landline")}
              value={formData.landline}
              debounceMs={0}
              onChange={(event) => updateForm({
                landline: event.target.value.replace(/\D/g, "").slice(0, LANDLINE_MAX_LENGTH),
              })}
              inputProps={{ maxLength: LANDLINE_MAX_LENGTH, inputMode: "numeric", pattern: "[0-9]*" }}
              onKeyDown={(event) => {
                if (
                  !event.ctrlKey &&
                  !event.metaKey &&
                  !event.altKey &&
                  event.key.length === 1 &&
                  !/^[0-9]$/.test(event.key)
                ) {
                  event.preventDefault();
                }
              }}
              disabled={isViewMode}
            />
            <FormInput
              label={translate("Ext")}
              value={formData.ext}
              onChange={(event) => updateForm({ ext: event.target.value.slice(0, 5) })}
              disabled={isViewMode}
            />
            <FormInput
              label={translate("Mobile No")}
              value={formData.mobile}
              debounceMs={0}
              onChange={handleMobileChange}
              error={
                isSubmitted &&
                Boolean(formData.mobile) &&
                onlyDigits(formData.mobile).length !== MOBILE_DIGIT_LENGTH
              }
              helperText={
                isSubmitted &&
                Boolean(formData.mobile) &&
                onlyDigits(formData.mobile).length !== MOBILE_DIGIT_LENGTH
                  ? translate("Phone Number is invalid")
                  : ""
              }
              inputProps={{ inputMode: "numeric" }}
              disabled={isViewMode}
            />
          </Box>
        </Box>
      </Modal>

      {lookupKind && (
        <GenericSearchModal
          open={Boolean(lookupKind)}
          onClose={() => setLookupKind(null)}
          title={lookupTitle[lookupKind]}
          columns={[
            { id: "id", label: translate("Id"), minWidth: 80 },
            { id: "code", label: translate("Code"), minWidth: 110 },
            { id: "name", label: translate("Name"), minWidth: 240 },
          ]}
          data={lookupData[lookupKind]}
          selectedField="id"
          selectedValue={selectedLookupValue as any}
          onSelect={(row: LookupOption) => {
            if (lookupKind === "addressType") updateForm({ addressTypeId: row.id });
            if (lookupKind === "country") updateForm({ countryId: row.id, stateId: "", cityId: "" });
            if (lookupKind === "state") updateForm({ stateId: row.id, cityId: "" });
            if (lookupKind === "city") updateForm({ cityId: row.id });
            setLookupKind(null);
          }}
        />
      )}

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />
    </>
  );
};
