import Select from "@/components/ClearableSelect";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Grid, Box, Typography, CircularProgress, IconButton, Button, Collapse, Tooltip, FormControl, FormControlLabel, Radio, RadioGroup, Menu, MenuItem } from "@mui/material";
import {
  Person as PersonIcon,
  Videocam as ZoomIcon,
  Duo as JitsiIcon,
  Google as GoogleMeetIcon,
  Home as AddressIcon,
  FormatListBulleted as RoasterIcon,
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  Assessment as AssessmentIcon,
  Payments as PaymentsIcon,
  Download as DownloadIcon,
  ExpandMore as ExpandMoreIcon,
  PictureAsPdf as PictureAsPdfIcon,
  TableView as TableViewIcon,
  School as TeachingIcon,
} from "@mui/icons-material";
import { ImageUpload } from "@/components/ImageUpload";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormDatePicker } from "@/components/FormDatePicker";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";
import dayjs from "dayjs";
import { FormInput } from "@/components/FormInput";
import { LookupDropdown } from "@/components/LookupDropdown";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { LmsToaster } from "@/components/LmsToaster";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  CheckemptyString,
  DeleteService,
  GetService,
  PostService,
  PutService,
} from "@/constants/globalItems";
import api from "@/services/api";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { toAbsoluteUrl } from "@/utils/toAbsoluteUrl";
import { ReportsPage, type ReportSectionData } from "@/pages/ReportsPage";
import {
  getViewableReportMenuItems,
  normalizeMenuName,
  useHiddenMenuPermissions,
} from "@/hooks/useHiddenMenuPermissions";
import { EmployeeAdditionalInfoPage } from "./EmployeeAdditionalInfoPage";
import { EmployeePayrollTab } from "./components/EmployeePayrollTab";
import { getOnlineMeetingSetup } from "@/services/communication/onlineMeetingSetupService";

interface EmployeeProfileProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const EmployeeProfile: React.FC<EmployeeProfileProps> = ({
  onSubViewChange,
  resetKey,
}) => {
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view" | "report" | "additionalInfo">(
    "list",
  );
  const [additionalInfoEmployeeId, setAdditionalInfoEmployeeId] = useState<string>();
  const [formData, setFormData] = useState<any>({
    id: null,
    employeeCode: "",
    machineCode: "",
    firstName: "",
    lastName: "",
    parentName: "",
    positionTypeId: null,
    joiningDate: "",
    employeeTypeId: null,
    employeeCategoryId: null,
    courseCategoryId: null,
    entityId: null,
    subCampusId: null,
    subCampusFromDate: "",
    subCampusTillDate: "",
    subCampusRemarks: "",
    departmentId: null,
    designationId: null,
    reportingToEmployeeId: null,
    substituteEmployeeId: null,
    statusId: null,
    gradeId: null,
    employmentRemarks: "",
    jobResponsibility: "",
    vacancyPositionId: null,
    promotionEffectiveDate: "",
    promotionRemarks: "",
    resignDate: "",
    dateOfBirth: "",
    genderId: null,
    cnicNo: "",
    bloodGroupId: null,
    maritalStatusId: null,
    birthCountryId: null,
    birthCityId: null,
    nationalityId: null,
    religionId: null,
    emailId: "",
    cellNo: "",
    sessiNo: "",
    eobiNo: "",
    zoomUsername: "",
    zoomPassword: "",
    zoomMeetingId: "",
    zoomMeetingPassword: "",
    zoomInvitePassword: "",
    zoomApiKey: "",
    zoomSdkKey: "",
    zoomApiSecret: "",
    zoomSdkSecret: "",
    zoomToken: "",
    zoomAccountId: "",
    zoomClientSecret: "",
    zoomHostEmail: "",
    zoomUserId: "",
    jitsiDomain: "",
    jitsiAppId: "",
    jitsiAppSecret: "",
    googleServiceAccountJson: "",
    googleHostEmail: "",
    shiftId: null,
    leaveGroupId: null,
    addressTypeId: null,
    address: "",
    addressCountryId: null,
    addressCityId: null,
    imageId: null,
    teachingCourseIds: [],
    teachingClassIds: [],
    teachingCourseCategoryIds: [],
  });
  const vacancyPrefillHandledRef = useRef(false);
  const [isSubmitted, setIsSubmitted] = useState(false);

  // Zoom exposes no API for reading app credentials, so they are typed in by hand.
  // Testing resolves the user id, time zone and licence state by asking Zoom, which
  // doubles as a check that this employee's app is activated. The Client ID comes from
  // the institution; the rest of the credentials belong to this employee.
  const [zoomTest, setZoomTest] = useState<{ busy: boolean; ok: boolean | null; message: string }>({
    busy: false,
    ok: null,
    message: "",
  });

  const testZoomConnection = async () => {
    setZoomTest({ busy: true, ok: null, message: "" });
    try {
      // Zoom's Client ID is stored in the Communication module's per-entity
      // SMOnlineMeetingSetup.configuration JSON, not on the Institution record.
      // The old institution lookup therefore reported it missing even when it
      // had already been saved on the Zoom configuration screen.
      const setup: any = await getOnlineMeetingSetup("zoom");
      const zoomClientId = String(
        (setup?.data ?? setup)?.configuration?.clientId ?? ""
      ).trim();
      if (!zoomClientId) {
        setZoomTest({
          busy: false,
          ok: false,
          message: translate("This organization has no Zoom Client ID. Add it on the Zoom configuration screen first."),
        });
        return;
      }

      const response: any = await PostService("/api/communication/OnlineMeetingSetup/v3/zoom/test-connection", {
        zoomAccountId: formData.zoomAccountId?.trim() || "",
        zoomClientSecret: formData.zoomClientSecret?.trim() || "",
        zoomHostEmail: formData.zoomHostEmail?.trim() || "",
        zoomUserId: formData.zoomUserId?.trim() || "",
      });
      const data = response?.data ?? response ?? {};
      if (data.success) {
        // Fill in whatever Zoom just told us so these never have to be typed.
        setFormData((prev: any) => ({
          ...prev,
          zoomUserId: data.zoomUserId ? String(data.zoomUserId) : prev.zoomUserId,
          zoomHostEmail: data.zoomHostEmail ? String(data.zoomHostEmail) : prev.zoomHostEmail,
        }));
      }
      setZoomTest({ busy: false, ok: !!data.success, message: data.message || "" });
    } catch (e: any) {
      setZoomTest({
        busy: false,
        ok: false,
        message:
          e?.response?.data?.message ||
          e?.response?.data?.error ||
          e?.message ||
          translate("Could not reach the server to test the Zoom connection."),
      });
    }
  };

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(translate("New Employee"));
    else if (viewMode === "edit") onSubViewChange(translate("Edit Employee"));
    else if (viewMode === "view") onSubViewChange(translate("View Employee"));
    else if (viewMode === "report")
      onSubViewChange(translate("Employee Profile"));
    else if (viewMode === "additionalInfo")
      onSubViewChange(translate("Additional Information"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setAdditionalInfoEmployeeId(undefined);
      setViewMode("list");
    }
  }, [resetKey]);

  useEffect(() => {
    if (formData.imageId) {
      setEmployeePhoto(toAbsoluteUrl(`/api/HumanResource/v3/image/${formData.imageId}`));
    } else {
      setEmployeePhoto(null);
    }
  }, [formData.imageId]);

  const [rows, setRows] = useState<any[]>([]);
  const [positionTypes, setPositionTypes] = useState<any[]>([]);
  const [employeeTypes, setEmployeeTypes] = useState<any[]>([]);
  const [employeeCategories, setEmployeeCategories] = useState<any[]>([]);
  const [teacherCategories, setTeacherCategories] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [designations, setDesignations] = useState<any[]>([]);
  const [reportingToEmployees, setReportingToEmployees] = useState<any[]>([]);
  const [entities, setEntities] = useState<any[]>([]);
  const [subCampuses, setSubCampuses] = useState<any[]>([]);
  const [statuses, setStatuses] = useState<any[]>([]);
  const [grades, setGrades] = useState<any[]>([]);
  const [gradeFilterId, setGradeFilterId] = useState<number | null>(null);
  const [exportMenuAnchor, setExportMenuAnchor] = useState<HTMLElement | null>(null);
  const [genders, setGenders] = useState<any[]>([]);
  const [bloodGroups, setBloodGroups] = useState<any[]>([]);
  const [maritalStatuses, setMaritalStatuses] = useState<any[]>([]);
  const [religions, setReligions] = useState<any[]>([]);
  const [countries, setCountries] = useState<any[]>([]);
  const [cities, setCities] = useState<any[]>([]);
  const [addressTypes, setAddressTypes] = useState<any[]>([]);
  const [shifts, setShifts] = useState<any[]>([]);
  const [leaveGroups, setLeaveGroups] = useState<any[]>([]);
  const [teachingCourses, setTeachingCourses] = useState<any[]>([]);
  const [teachingClasses, setTeachingClasses] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [employeeCreationPolicyLoading, setEmployeeCreationPolicyLoading] = useState(true);
  const [canCreateNewEmployee, setCanCreateNewEmployee] = useState(false);
  const [employeeCreationPolicyMessage, setEmployeeCreationPolicyMessage] = useState("");
  const { translate } = useTranslation();
  const { visibleMenuCardGroups } = useHiddenMenuPermissions();
  const employeeProfileFallbackReportName = "Employee Profile";
  const teacherCategoryId = 269;
  const resignedStatusIds = new Set([126, 127]);
  const employeeProfileMaxLength = {
    employeeCode: 50,
    machineCode: 50,
    firstName: 150,
    lastName: 150,
    parentName: 150,
    cellNo: 20,
    cnicNo: 50,
    emailId: 50,
    zoomUsername: 500,
    zoomPassword: 500,
    zoomMeetingId: 50,
    zoomMeetingPassword: 50,
    zoomApiKey: 500,
    zoomApiSecret: 500,
    zoomAccountId: 100,
    zoomClientSecret: 500,
    zoomHostEmail: 200,
    zoomUserId: 100,
    jitsiDomain: 255,
    jitsiAppId: 255,
    jitsiAppSecret: 500,
    googleHostEmail: 200,
    address: 500,
  } as const;
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/i;

  const [activeLookup, setActiveLookup] = useState<string | null>(null);
  const caretRestoreFrameRef = useRef<number | null>(null);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [toasterSeverity, setToasterSeverity] = useState<
    "error" | "success" | "info" | "warning"
  >("error");
  const [employeePhoto, setEmployeePhoto] = useState<string | null>(null);
  const [allowIntegrationTabs, setAllowIntegrationTabs] = useState(false);
  const [activeSubTab, setActiveSubTab] = useState(0);
  // A new value means the user changed designation and wants its saved salary template.
  const [payrollTemplateVersion, setPayrollTemplateVersion] = useState(0);
  // Zoom and Jitsi credentials both live on this employee's SMmeetinghostinfo row,
  // so the Online Meeting tab splits them into one sub-tab per provider.
  const [activeMeetingProviderTab, setActiveMeetingProviderTab] = useState(0);
  // Google Meet accepts either a Workspace service-account key or a personal account
  // connected through OAuth. Both end up on the same row, so the tab offers both.
  const [googleAuthMode, setGoogleAuthMode] = useState<"oauth" | "serviceAccount">("oauth");
  // Show the method this employee actually uses when their profile loads: a stored
  // service-account key means that tab, anything else stays on the connect flow.
  useEffect(() => {
    if (formData.googleServiceAccountJson?.trim()) setGoogleAuthMode("serviceAccount");
  }, [formData.googleServiceAccountJson]);
  const [googleConnect, setGoogleConnect] = useState<{ busy: boolean; message: string; error: boolean }>({
    busy: false,
    message: "",
    error: false,
  });

  // The consent flow runs in a popup and posts back when it is done, so the tab can
  // reload the employee and show the freshly connected account without a full refresh.
  const startGoogleConnect = useCallback(async () => {
    if (!formData.id) {
      setGoogleConnect({ busy: false, error: true, message: translate("Save this employee before connecting a Google account.") });
      return;
    }

    setGoogleConnect({ busy: true, error: false, message: "" });
    try {
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3/${formData.id}/google/connect`,
      );
      const data = response?.data ?? response ?? {};
      if (!data.success || !data.url) {
        setGoogleConnect({
          busy: false,
          error: true,
          message: data.message || translate("Google is not configured on this server."),
        });
        return;
      }

      const popup = window.open(data.url, "_blank", "width=520,height=650");
      if (!popup) {
        setGoogleConnect({ busy: false, error: true, message: translate("Allow pop-ups to connect a Google account.") });
        return;
      }

      // Re-read the employee so the tab shows whichever account Google connected.
      // The credential itself is written by the callback, so this only refreshes
      // what is displayed - it is safe to run more than once.
      const refreshGoogleState = async () => {
        if (!formData.id) return;
        try {
          const res: any = await GetService(`/api/HumanResource/EmployeeProfile/v3/${formData.id}`);
          const employee = res?.data ?? res ?? {};
          setFormData((prev: any) => ({
            ...prev,
            googleHostEmail: employee.googleHostEmail ?? "",
            googleServiceAccountJson: employee.googleServiceAccountJson ?? "",
          }));
          return employee.googleHostEmail ?? "";
        } catch {
          return "";
        }
      };

      let settled = false;
      const finish = async (success: boolean) => {
        if (settled) return;
        settled = true;
        window.removeEventListener("message", onMessage);
        window.clearInterval(pollClosed);
        const email = await refreshGoogleState();
        setGoogleConnect({
          busy: false,
          error: !success && !email,
          message: email
            ? translate("Google account connected.")
            : success
              ? translate("Google account connected.")
              : translate("Connecting to Google failed."),
        });
      };

      const onMessage = (event: MessageEvent) => {
        if (event.data?.source !== "edxs-google-oauth") return;
        void finish(!!event.data.success);
      };
      window.addEventListener("message", onMessage);

      // postMessage can be missed - a blocked opener, a popup closed by hand, or a
      // browser that drops the message. Watching for the window to close means the
      // tab still refreshes instead of sitting on "Connect" after a real connection.
      const pollClosed = window.setInterval(() => {
        if (popup.closed) void finish(true);
      }, 800);
    } catch {
      setGoogleConnect({ busy: false, error: true, message: translate("Could not reach the server to start the Google connection.") });
    }
  }, [formData.id, translate]);

  const disconnectGoogle = useCallback(async () => {
    if (!formData.id) return;
    setGoogleConnect({ busy: true, error: false, message: "" });
    try {
      await DeleteService(`/api/HumanResource/EmployeeProfile/v3/${formData.id}/google/disconnect`);
      setFormData((prev: any) => ({ ...prev, googleHostEmail: "", googleServiceAccountJson: "" }));
      setGoogleConnect({ busy: false, error: false, message: translate("Google account disconnected.") });
    } catch {
      setGoogleConnect({ busy: false, error: true, message: translate("Could not disconnect the Google account.") });
    }
  }, [formData.id, translate]);
  const meetingProviderTabs = useMemo(
    () => [
      { key: "zoom", label: "Zoom", icon: <ZoomIcon /> },
      { key: "jitsi", label: "Jitsi", icon: <JitsiIcon /> },
      { key: "googleMeet", label: "Google Meet", icon: <GoogleMeetIcon /> },
    ],
    [],
  );
  // Only the Teacher category edits a Teaching Specialization; for anyone else the API
  // leaves the subject types alone, since the Subject Rating tab shares those rows.
  const isTeacherEmployee = Number(formData.employeeCategoryId) === teacherCategoryId;
  // A saved course id may be any copy of the course; show it as that name's option.
  const selectedTeachingCourseIds = (ids: any): number[] => {
    const optionIds = (Array.isArray(ids) ? ids : []).map((id: any) => {
      const courseId = Number(id);
      return teachingCourses.find((option: any) => option.ids.includes(courseId))?.id ?? courseId;
    });
    return optionIds.filter((id: number, index: number) => id > 0 && optionIds.indexOf(id) === index);
  };
  const employeeProfileSubTabs = useMemo(
    () => [
      { key: "personal", label: "Personal Information", icon: <PersonIcon /> },
      ...(allowIntegrationTabs
        ? [
            { key: "onlineMeeting", label: "Online Meeting", icon: <ZoomIcon /> },
            { key: "roster", label: "Employee Roster", icon: <RoasterIcon /> },
            { key: "address", label: "Address", icon: <AddressIcon /> },
          ]
        : []),
      ...(isTeacherEmployee
        ? [{ key: "teachingSpecialization", label: "Teaching Specialization", icon: <TeachingIcon /> }]
        : []),
      ...(formData.id
        ? [{ key: "salary", label: "Salary", icon: <PaymentsIcon /> }]
        : []),
    ],
    [allowIntegrationTabs, formData.id, isTeacherEmployee],
  );
  const getEmployeeProfileSubTabIndex = useCallback(
    (key: string) => employeeProfileSubTabs.findIndex((tab) => tab.key === key),
    [employeeProfileSubTabs],
  );

  const employeeProfileReportContext = useMemo(() => {
    const employeeProfileReportNames = new Set([
      "employee profile",
      "employee profile report",
    ]);

    for (const group of visibleMenuCardGroups) {
      const reportItem = getViewableReportMenuItems(group.menuItems).find(
        (item) => employeeProfileReportNames.has(normalizeMenuName(item.name)),
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

  const canViewEmployeeProfileReport = Boolean(
    employeeProfileReportContext?.selectedReport.reportId ||
      employeeProfileReportContext?.selectedReport.reportCode,
  );
  const employeeProfileReportName =
    employeeProfileReportContext?.selectedReport.name ||
    employeeProfileFallbackReportName;

  useEffect(() => {
    if (viewMode === "report" && !canViewEmployeeProfileReport) {
      setViewMode("list");
    }
  }, [canViewEmployeeProfileReport, viewMode]);

  const normalizeLov = (items: any[]) =>
    (items || []).map((item: any) => ({
      id: Number(item.id ?? item.Id ?? 0),
      code: item.code ?? item.Code ?? "",
      name: item.name ?? item.Name ?? "",
      description: item.description ?? item.ltxt ?? item.Ltxt ?? "",
    }));

  const getHeadEntityId = (item: any) => {
    const parsed = Number(
      item?.headEntityId ??
        item?.HeadEntityId ??
        item?.headentity ??
        item?.Headentity ??
        0,
    );
    return Number.isFinite(parsed) && parsed > 0 ? parsed : 0;
  };

  const normalizeEntityOption = (item: any) => ({
    id: Number(item.id ?? item.Id ?? item.entityId ?? item.EntityId ?? 0),
    code: item.code ?? item.Code ?? item.entityNo ?? item.EntityNo ?? "",
    name:
      item.instituteName ??
      item.entityName ??
      item.campusName ??
      item.stxt ??
      item.name ??
      "",
    instituteName:
      item.instituteName ??
      item.entityName ??
      item.campusName ??
      item.stxt ??
      item.name ??
      "",
    shortName: item.shortName ?? item.shortname ?? item.ShortName ?? "",
    address: item.address ?? item.Address ?? "",
    city: item.city ?? item.cityName ?? item.CityName ?? "",
    country: item.country ?? item.countryName ?? item.CountryName ?? "",
    imageId: item.imageId ?? item.ImageId ?? null,
    description: item.description ?? item.ltxt ?? item.Ltxt ?? "",
    headEntityId: getHeadEntityId(item),
  });

  const normalizeNullableNumber = useCallback((value: any) => {
    if (value === null || value === undefined || value === "") return null;
    const parsed = Number(value);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
  }, []);

  const fetchEmployeeProfileDefaults = useCallback(async (entityId?: any) => {
    const parsedEntityId = normalizeNullableNumber(entityId);
    const query = parsedEntityId ? `?entityId=${parsedEntityId}` : "";
    const response: any = await GetService(
      `/api/HumanResource/EmployeeProfile/v3/defaults${query}`,
    );
    const data = response?.data ?? {};
    return {
      birthCountryId: normalizeNullableNumber(
        data.birthCountryId ?? data.BirthCountryId,
      ),
      birthCityId: normalizeNullableNumber(data.birthCityId ?? data.BirthCityId),
      nationalityId: normalizeNullableNumber(
        data.nationalityId ?? data.NationalityId,
      ),
    };
  }, [normalizeNullableNumber]);

  const applyEmployeeProfileDefaults = useCallback(
    async (entityId?: any, overwrite = false) => {
      try {
        const defaults = await fetchEmployeeProfileDefaults(entityId);
        setFormData((prev: any) => ({
          ...prev,
          birthCountryId: overwrite
            ? defaults.birthCountryId
            : prev.birthCountryId || defaults.birthCountryId,
          birthCityId: overwrite
            ? defaults.birthCityId
            : prev.birthCityId || defaults.birthCityId,
          nationalityId:
            overwrite
              ? defaults.nationalityId || defaults.birthCountryId
              : prev.nationalityId ||
                defaults.nationalityId ||
                defaults.birthCountryId,
        }));
      } catch (error) {
        console.error("Failed to fetch employee profile defaults", error);
      }
    },
    [fetchEmployeeProfileDefaults],
  );

  const fetchEmployees = useCallback(async () => {
    setIsLoading(true);
    try {
      const selectedCampusId = normalizeNullableNumber(
        localStorage.getItem("selectedCampusId"),
      );
      const campusQuery = selectedCampusId ? `&entityId=${selectedCampusId}` : "";
      const gradeQuery = gradeFilterId ? `&gradeId=${gradeFilterId}` : "";
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3?pageNumber=1&pageSize=1000${campusQuery}${gradeQuery}`,
      );
      setRows(response?.data?.items ?? response?.data?.Items ?? []);
    } catch (error) {
      console.error("Failed to fetch employees", error);
      setToasterMessages([translate("Failed to fetch employees")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setIsLoading(false);
    }
  }, [gradeFilterId, normalizeNullableNumber, translate]);

  const fetchEmployeeCreationPolicy = useCallback(async () => {
    setEmployeeCreationPolicyLoading(true);
    try {
      const selectedCampusId = normalizeNullableNumber(
        localStorage.getItem("selectedCampusId"),
      );
      const entityQuery = selectedCampusId ? `?entityId=${selectedCampusId}` : "";
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3/creation-policy${entityQuery}`,
      );
      const data = response?.data ?? response ?? {};
      const useVacancyProcess = Boolean(
        data.useVacancyProcess ?? data.UseVacancyProcess ?? false,
      );
      const hasVacancies = Boolean(data.hasVacancies ?? data.HasVacancies ?? false);
      const canCreate = Boolean(
        data.canCreateNewEmployee ??
          data.CanCreateNewEmployee ??
          (!useVacancyProcess || hasVacancies),
      );
      setCanCreateNewEmployee(canCreate);
      setEmployeeCreationPolicyMessage(
        canCreate
          ? ""
          : String(
              data.message ??
                data.Message ??
                translate("Vacancy Process is enabled. Create a vacancy before adding a new employee."),
            ),
      );
    } catch (error) {
      console.error("Failed to verify employee creation policy", error);
      setCanCreateNewEmployee(false);
      setEmployeeCreationPolicyMessage(
        translate("Unable to verify Vacancy Process policy. New employee creation is disabled."),
      );
    } finally {
      setEmployeeCreationPolicyLoading(false);
    }
  }, [normalizeNullableNumber, translate]);

  const generateEmployeeCodeFromNumberConfig = useCallback(async () => {
    try {
      const response: any = await GetService(
        "/api/HumanResource/EmployeeProfile/v3/next-employee-code",
      );
      return String(response?.data?.employeeCode ?? response?.data?.EmployeeCode ?? "");
    } catch (error) {
      console.error("Failed to generate Employee ID from Number Config", error);
      return "";
    }
  }, []);

  const loadReportingToEmployees = useCallback(async (entityId?: any, excludeEmployeeId?: any) => {
    const parsedEntityId = normalizeNullableNumber(entityId);
    if (!parsedEntityId) {
      setReportingToEmployees([]);
      return [];
    }

    try {
      const parsedExcludeEmployeeId = normalizeNullableNumber(excludeEmployeeId);
      const excludeQuery = parsedExcludeEmployeeId
        ? `&excludeEmployeeId=${parsedExcludeEmployeeId}`
        : "";
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3/lookups/reporting-to?entityId=${parsedEntityId}${excludeQuery}`,
      );
      const items = response?.data?.items ?? response?.data?.Items ?? response?.data ?? [];
      const options = (Array.isArray(items) ? items : [])
        .map((item: any) => ({
          id: Number(item.id ?? item.Id ?? 0),
          code: item.code ?? item.Code ?? "",
          name: item.name ?? item.Name ?? item.code ?? item.Code ?? "",
        }))
        .filter((item: any) => item.id > 0 && item.name);
      setReportingToEmployees(options);
      return options;
    } catch (error) {
      console.error("Failed to fetch Reporting To employees", error);
      setReportingToEmployees([]);
      return [];
    }
  }, [normalizeNullableNumber]);

  const loadDepartments = useCallback(async (entityId?: any) => {
    const parsedEntityId = normalizeNullableNumber(entityId);
    if (!parsedEntityId) {
      setDepartments([]);
      return [];
    }

    try {
      const response: any = await GetService(
        `/api/HumanResource/Lookups/v3/departments?entityId=${parsedEntityId}`,
      );
      const items = response?.data?.items ?? response?.data?.Items ?? [];
      const options = (items || []).map((item: any) => ({
        id: Number(item.id ?? item.departmentId ?? item.depid ?? 0),
        code: item.code ?? item.departmentcode ?? item.departmentCode ?? "",
        name: item.departmentName ?? item.department ?? item.depstxt ?? item.name ?? "",
        description: item.description ?? item.ltxt ?? "",
      }));
      setDepartments(options);
      return options;
    } catch (error) {
      console.error("Failed to fetch departments", error);
      setDepartments([]);
      return [];
    }
  }, [normalizeNullableNumber]);

  const fetchLookups = useCallback(async () => {
    setIsLoading(true);
    try {
      const [
        positionResp,
        empTypeResp,
        empCategoryResp,
        teacherCategoryResp,
        statusResp,
        genderResp,
        bloodResp,
        maritalResp,
        addressTypeResp,
        religionResp,
        designationResp,
        entityResp,
        countryResp,
        cityResp,
        leaveGroupResp,
        gradeResp,
      ] = await Promise.all([
        GetService("/api/HumanResource/Lov/v3/PSTYP"),
        GetService("/api/HumanResource/Lov/v3/EMPTP"),
        GetService("/api/HumanResource/Lov/v3/ECAT"),
        GetService("/api/HumanResource/Lov/v3/COTY"),
        GetService("/api/HumanResource/Lov/v3/JBSTS"),
        GetService("/api/HumanResource/Lov/v3/GEND"),
        GetService("/api/HumanResource/Lov/v3/BLOOD"),
        GetService("/api/HumanResource/Lov/v3/MRSTS"),
        GetService("/api/HumanResource/Lov/v3/ADDTP"),
        GetService("/api/HumanResource/Lookups/v3/religions"),
        GetService("/api/HumanResource/Lookups/v3/designations"),
        // Legacy Employee Profile uses Legalentity/UserEntity, which returns
        // only the campuses allocated to the logged-in user.
        GetService("/api/auth/Legalentity/v3/Legalentity"),
        GetService("/api/HumanResource/Lookups/v3/countries"),
        GetService("/api/HumanResource/Lookups/v3/cities"),
        GetService(
          "/api/HumanResource/EmployeeProfile/v3/lookups/leave-groups",
        ),
        GetService("/api/HumanResource/EmployeeProfile/v3/lookups/grades"),
      ]);

      setPositionTypes(normalizeLov(positionResp?.data ?? []));
      setEmployeeTypes(normalizeLov(empTypeResp?.data ?? []));
      setEmployeeCategories(normalizeLov(empCategoryResp?.data ?? []));
      setTeacherCategories(normalizeLov(teacherCategoryResp?.data ?? []));
      setStatuses(normalizeLov(statusResp?.data ?? []));
      setGenders(normalizeLov(genderResp?.data ?? []));
      setBloodGroups(normalizeLov(bloodResp?.data ?? []));
      setMaritalStatuses(normalizeLov(maritalResp?.data ?? []));
      setAddressTypes(normalizeLov(addressTypeResp?.data ?? []));

      const religionItems =
        religionResp?.data?.items ?? religionResp?.data?.Items ?? [];
      setReligions(
        (religionItems || []).map((item: any) => ({
          id: Number(item.id ?? 0),
          code: item.religioncode ?? item.code ?? "",
          name: item.stxt ?? item.religion ?? item.name ?? "",
          description: item.ltxt ?? item.description ?? "",
        })),
      );

      const designationItems =
        designationResp?.data?.items ?? designationResp?.data?.Items ?? [];
      setDesignations(
        (designationItems || []).map((item: any) => ({
          id: Number(item.id ?? item.designationId ?? 0),
          code: item.code ?? item.designationcode ?? item.designationCode ?? "",
          name: item.designationName ?? item.designation ?? item.stxt ?? item.name ?? "",
          description: item.description ?? item.ltxt ?? "",
        })),
      );

      const entityPayload = entityResp?.data ?? [];
      const entityItems = Array.isArray(entityPayload)
        ? entityPayload
        : entityPayload?.items ?? entityPayload?.Items ?? [];
      const normalizedEntities = (entityItems || [])
        .map(normalizeEntityOption)
        .filter((item: any) => item.id > 0 && item.name);
      setEntities(normalizedEntities.filter((item: any) => !item.headEntityId));

      const countryItems =
        countryResp?.data?.items ?? countryResp?.data?.Items ?? [];
      setCountries(
        (countryItems || []).map((item: any) => ({
          id: Number(item.countryid ?? item.id ?? 0),
          code: item.countrycode ?? item.code ?? "",
          name: item.countryname ?? item.countryName ?? item.name ?? "",
        })),
      );

      const cityItems = cityResp?.data?.items ?? cityResp?.data?.Items ?? [];
      setCities(
        (cityItems || []).map((item: any) => ({
          id: Number(item.cityid ?? item.cityId ?? item.id ?? 0),
          code: item.citycode ?? item.cityCode ?? item.code ?? "",
          name: item.cityname ?? item.cityName ?? item.city ?? item.name ?? "",
          countryId: Number(item.countryid ?? item.countryId ?? 0),
        })),
      );

      setLeaveGroups(normalizeLov(leaveGroupResp?.data ?? []));
      setGrades(normalizeLov(gradeResp?.data ?? []));
    } catch (error) {
      console.error("Failed to fetch lookups", error);
      setToasterMessages([translate("Failed to fetch lookup data")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setIsLoading(false);
    }
  }, [translate]);

  useEffect(() => {
    const entityId = normalizeNullableNumber(formData.entityId);
    const departmentId = normalizeNullableNumber(formData.departmentId);
    if (!entityId || !departmentId) {
      setShifts([]);
      return;
    }

    let cancelled = false;
    const employeeId = normalizeNullableNumber(formData.id);
    const employeeQuery = employeeId ? `&employeeId=${employeeId}` : "";
    void GetService(
      `/api/HumanResource/EmployeeProfile/v3/lookups/shifts?entityId=${entityId}&departmentId=${departmentId}${employeeQuery}`,
    )
      .then((response: any) => {
        if (!cancelled) setShifts(normalizeLov(response?.data ?? []));
      })
      .catch(() => {
        if (!cancelled) setShifts([]);
      });

    return () => {
      cancelled = true;
    };
  }, [formData.departmentId, formData.entityId, formData.id]);

  // Teaching Specialization offers the courses of the employee's campus. A course name
  // repeats in every class that has it, so each name is one option: its lowest id stands
  // for all of them, and ids lists the rest so a saved one still finds its option.
  useEffect(() => {
    const entityId = Number(formData.entityId || 0);
    if (!isTeacherEmployee || !entityId) {
      setTeachingCourses([]);
      return;
    }

    let cancelled = false;
    void GetService(`/api/Academics/Subjects/v3?pageNumber=1&pageSize=1000&lightweight=true&entityId=${entityId}`)
      .then((response: any) => {
        if (cancelled) return;
        const payload = response?.data ?? {};
        const items = payload.items ?? payload.Items ?? [];
        const byName = new Map<string, { id: number; name: string; ids: number[]; classes: string[] }>();
        (Array.isArray(items) ? items : []).forEach((item: any) => {
          const id = Number(item.id ?? item.Id ?? 0);
          const name = String(item.name ?? item.Name ?? "").trim();
          if (!id || !name) return;
          const key = name.replace(/\s+/g, " ").toLowerCase();
          const option = byName.get(key) ?? { id, name, ids: [], classes: [] };
          option.id = Math.min(option.id, id);
          option.ids.push(id);
          const className = String(item.courseName ?? item.CourseName ?? "").trim();
          if (className && !option.classes.includes(className)) option.classes.push(className);
          byName.set(key, option);
        });
        setTeachingCourses(
          [...byName.values()]
            .map((option) => ({ ...option, classNames: option.classes.join(", ") }))
            .sort((left, right) => left.name.localeCompare(right.name)),
        );
      })
      .catch(() => {
        if (!cancelled) setTeachingCourses([]);
      });

    return () => {
      cancelled = true;
    };
  }, [isTeacherEmployee, formData.entityId]);

  // Class Range offers the classes of the employee's campus, in their class order.
  useEffect(() => {
    const entityId = Number(formData.entityId || 0);
    if (!isTeacherEmployee || !entityId) {
      setTeachingClasses([]);
      return;
    }

    let cancelled = false;
    void GetService(`/api/Academics/Courses/v3?pageNumber=1&pageSize=1000&entityId=${entityId}`)
      .then((response: any) => {
        if (cancelled) return;
        const payload = response?.data ?? {};
        const items = payload.items ?? payload.Items ?? [];
        setTeachingClasses(
          (Array.isArray(items) ? items : [])
            .map((item: any) => ({
              id: Number(item.id ?? item.Id ?? 0),
              code: item.code ?? item.Code ?? "",
              name: item.courseName ?? item.CourseName ?? "",
            }))
            .filter((item: any) => item.id > 0),
        );
      })
      .catch(() => {
        if (!cancelled) setTeachingClasses([]);
      });

    return () => {
      cancelled = true;
    };
  }, [isTeacherEmployee, formData.entityId]);

  const fetchGeneralPolicy = useCallback(async () => {
    try {
      const response: any = await GetService("/api/Administration/SystemPolicyGeneral/v3");
      const data = response?.data?.data ?? response?.data?.Data ?? response?.data ?? response ?? {};
      setAllowIntegrationTabs(Boolean(data.allowIntegration ?? data.AllowIntegration ?? false));
    } catch (error) {
      console.error("Failed to fetch general policy", error);
      setAllowIntegrationTabs(false);
    }
  }, []);

  useEffect(() => {
    fetchLookups();
    fetchGeneralPolicy();
    void loadDepartments(localStorage.getItem("selectedCampusId"));
  }, [fetchLookups, fetchGeneralPolicy, loadDepartments]);

  useEffect(() => {
    void fetchEmployees();
  }, [fetchEmployees]);

  useEffect(() => {
    void fetchEmployeeCreationPolicy();
  }, [fetchEmployeeCreationPolicy, resetKey]);

  useEffect(() => {
    if (activeSubTab >= employeeProfileSubTabs.length) {
      setActiveSubTab(0);
    }
  }, [activeSubTab, employeeProfileSubTabs.length]);

  useEffect(() => {
    return () => {
      if (caretRestoreFrameRef.current !== null) {
        window.cancelAnimationFrame(caretRestoreFrameRef.current);
      }
    };
  }, []);

  const handleLookupClose = () => {
    setActiveLookup(null);
  };

  const lookupData: Record<string, any> = {
    positionType: {
      title: translate("Position Type"),
      subtitle: translate("Search / Position Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Position Type"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: positionTypes.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    employeeType: {
      title: translate("Employee Type"),
      subtitle: translate("Search / Employee Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Employee Type"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: employeeTypes.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    employeeCategory: {
      title: translate("Employee Category"),
      subtitle: translate("Search / Employee Category"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Employee Category"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: employeeCategories.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    teacherCategory: {
      title: translate("Teaching Level"),
      subtitle: translate("Search / Teaching Level"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Teaching Level"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: teacherCategories.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    department: {
      title: translate("Department"),
      subtitle: translate("Search / Department"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Department"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: departments.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    entity: {
      title: translate("Entity"),
      subtitle: translate("Search / Entity"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Entity"), minWidth: 220 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: entities.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    subCampus: {
      title: translate("Sub Campus"),
      subtitle: translate("Search / Sub Campus"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Sub Campus"), minWidth: 220 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: subCampuses.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    designation: {
      title: translate("Designation"),
      subtitle: translate("Search / Designation"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Designation"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: designations.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    reportingTo: {
      title: translate("Reporting To"),
      subtitle: translate("Search / Reporting To Employee"),
      columns: [
        { id: "code", label: translate("Employee ID"), minWidth: 130 },
        { id: "name", label: translate("Employee Name"), minWidth: 240 },
      ],
      rows: reportingToEmployees,
    },
    substitute: {
      title: translate("Substitute"),
      subtitle: translate("Search / Substitute"),
      columns: [
        { id: "code", label: translate("Employee ID"), minWidth: 130 },
        { id: "name", label: translate("Employee Name"), minWidth: 240 },
      ],
      rows: reportingToEmployees,
    },
    status: {
      title: translate("Status"),
      subtitle: translate("Search / Status"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Status"), minWidth: 180 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: statuses.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    grade: {
      title: translate("Grade"),
      subtitle: translate("Search / Grade"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Grade"), minWidth: 180 },
      ],
      rows: grades.map((item: any) => ({ id: item.id, code: item.code, name: item.name })),
    },
    gender: {
      title: translate("Gender"),
      subtitle: translate("Search / Gender"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Gender"), minWidth: 150 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: genders.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    bloodGroup: {
      title: translate("Blood Group"),
      subtitle: translate("Search / Blood Group"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Blood Group"), minWidth: 150 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: bloodGroups.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    maritalStatus: {
      title: translate("Marital Status"),
      subtitle: translate("Search / Marital Status"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Marital Status"), minWidth: 150 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: maritalStatuses.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    religion: {
      title: translate("Religion"),
      subtitle: translate("Search / Religion"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Religion"), minWidth: 150 },
        { id: "description", label: translate("Description"), minWidth: 200 },
      ],
      rows: religions.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    birthCountry: {
      title: translate("Birth Country"),
      subtitle: translate("Search / Birth Country"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Country"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: countries.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: "",
      })),
    },
    birthCity: {
      title: translate("Birth City"),
      subtitle: translate("Search / Birth City"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("City"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: cities
        .filter(
          (item: any) =>
            !formData.birthCountryId ||
            item.countryId === Number(formData.birthCountryId),
        )
        .map((item: any) => ({
          id: item.id,
          code: item.code,
          name: item.name,
          description: "",
        })),
    },
    nationality: {
      title: translate("Nationality"),
      subtitle: translate("Search / Nationality"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Nationality"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: countries.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: "",
      })),
    },
    shift: {
      title: translate("Shift"),
      subtitle: translate("Search / Shift"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Shift"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: shifts.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    leaveGroup: {
      title: translate("Leave Group"),
      subtitle: translate("Search / Leave Group"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Leave Group"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: leaveGroups.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    addressLookup: {
      title: translate("Address Type"),
      subtitle: translate("Search / Address Type"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Address Type"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: addressTypes.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: item.description ?? "",
      })),
    },
    addressCountry: {
      title: translate("Address Country"),
      subtitle: translate("Search / Address Country"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Country"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: countries.map((item: any) => ({
        id: item.id,
        code: item.code,
        name: item.name,
        description: "",
      })),
    },
    addressCity: {
      title: translate("Address City"),
      subtitle: translate("Search / Address City"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("City"), minWidth: 200 },
        { id: "description", label: translate("Description"), minWidth: 150 },
      ],
      rows: cities
        .filter(
          (item: any) =>
            !formData.addressCountryId ||
            item.countryId === Number(formData.addressCountryId),
        )
        .map((item: any) => ({
          id: item.id,
          code: item.code,
          name: item.name,
          description: "",
        })),
    },
    teachingCourses: {
      title: translate("Teaching Specialization"),
      subtitle: translate("Search / Subject"),
      columns: [
        { id: "name", label: translate("Subject"), minWidth: 220 },
        { id: "classNames", label: translate("Class"), minWidth: 260 },
      ],
      rows: teachingCourses,
    },
    teachingClasses: {
      title: translate("Class Range"),
      subtitle: translate("Search / Class"),
      columns: [
        { id: "code", label: translate("Code"), minWidth: 100 },
        { id: "name", label: translate("Class"), minWidth: 220 },
      ],
      rows: teachingClasses,
    },
  };

  const renderLookupDialog = (formData: any, setFormData: any) => {
    if (!activeLookup || !lookupData[activeLookup]) return null;

    const data = lookupData[activeLookup];
    const selectedLookupValues: Record<string, any> = {
      positionType: formData.positionTypeId,
      employeeType: formData.employeeTypeId,
      employeeCategory: formData.employeeCategoryId,
      department: formData.departmentId,
      entity: formData.entityId,
      subCampus: formData.subCampusId,
      designation: formData.designationId,
      reportingTo: formData.reportingToEmployeeId,
      substitute: formData.substituteEmployeeId,
      status: formData.statusId,
      gender: formData.genderId,
      bloodGroup: formData.bloodGroupId,
      maritalStatus: formData.maritalStatusId,
      religion: formData.religionId,
      birthCountry: formData.birthCountryId,
      birthCity: formData.birthCityId,
      nationality: formData.nationalityId,
      shift: formData.shiftId,
      leaveGroup: formData.leaveGroupId,
      addressLookup: formData.addressTypeId,
      addressCountry: formData.addressCountryId,
      addressCity: formData.addressCityId,
    };

    const handleRowSelect = (row: any) => {
      switch (activeLookup) {
        case "teachingCourses": {
          const selectedIds = selectedTeachingCourseIds(formData.teachingCourseIds);
          const courseId = Number(row.id);
          if (!selectedIds.includes(courseId)) {
            setFormData({ ...formData, teachingCourseIds: [...selectedIds, courseId] });
          }
          break;
        }
        case "teachingClasses": {
          const selectedIds: number[] = formData.teachingClassIds ?? [];
          const classId = Number(row.id);
          if (!selectedIds.includes(classId)) {
            setFormData({ ...formData, teachingClassIds: [...selectedIds, classId] });
          }
          break;
        }
        case "positionType":
          setFormData({ ...formData, positionTypeId: row.id });
          break;
        case "employeeType":
          setFormData({ ...formData, employeeTypeId: row.id });
          break;
        case "employeeCategory":
          setFormData({
            ...formData,
            employeeCategoryId: row.id,
            courseCategoryId: null,
            teachingCourseCategoryIds: [],
          });
          break;
        case "teacherCategory": {
          const selectedIds: number[] = formData.teachingCourseCategoryIds ?? [];
          const categoryId = Number(row.id);
          if (!selectedIds.includes(categoryId)) {
            setFormData({ ...formData, teachingCourseCategoryIds: [...selectedIds, categoryId] });
          }
          break;
        }
        case "department":
          setFormData({ ...formData, departmentId: row.id });
          break;
        case "entity": {
          const previousDepartmentId = formData.departmentId;
          setFormData({
            ...formData,
            entityId: row.id,
            reportingToEmployeeId: null,
            substituteEmployeeId: null,
            subCampusId: 0,
            subCampusFromDate: "",
            subCampusTillDate: "",
            subCampusRemarks: "",
          });
          void loadDepartments(row.id).then((options) => {
            if (previousDepartmentId && !options.some((option: any) => Number(option.id) === Number(previousDepartmentId))) {
              setFormData((previous: any) =>
                Number(previous.entityId) === Number(row.id)
                  ? { ...previous, departmentId: null }
                  : previous,
              );
            }
          });
          void loadSubCampuses(row.id);
          void loadReportingToEmployees(row.id, formData.id);
          if (viewMode === "new") {
            void applyEmployeeProfileDefaults(row.id, true);
          }
          break;
        }
        case "subCampus": {
          const selectedSubCampusId = Number(row.id || 0);
          setFormData({
            ...formData,
            subCampusId: selectedSubCampusId > 0 ? selectedSubCampusId : 0,
            subCampusFromDate: selectedSubCampusId > 0 ? formData.subCampusFromDate : "",
            subCampusTillDate: selectedSubCampusId > 0 ? formData.subCampusTillDate : "",
            subCampusRemarks: selectedSubCampusId > 0 ? formData.subCampusRemarks : "",
          });
          break;
        }
        case "designation":
          if (Number(formData.designationId || 0) !== Number(row.id || 0)) {
            setPayrollTemplateVersion((version) => version + 1);
          }
          setFormData({ ...formData, designationId: row.id });
          break;
        case "reportingTo":
          setFormData({ ...formData, reportingToEmployeeId: row.id || null });
          break;
        case "substitute":
          setFormData({ ...formData, substituteEmployeeId: row.id || null });
          break;
        case "status":
          setFormData({
            ...formData,
            statusId: row.id,
            resignDate: isResignedStatus(row.id) ? formData.resignDate : "",
          });
          break;
        case "gender":
          setFormData({ ...formData, genderId: row.id });
          break;
        case "bloodGroup":
          setFormData({ ...formData, bloodGroupId: row.id });
          break;
        case "maritalStatus":
          setFormData({ ...formData, maritalStatusId: row.id });
          break;
        case "religion":
          setFormData({ ...formData, religionId: row.id });
          break;
        case "birthCountry":
          setFormData({
            ...formData,
            birthCountryId: row.id,
            birthCityId: null,
          });
          break;
        case "birthCity":
          setFormData({ ...formData, birthCityId: row.id });
          break;
        case "nationality":
          setFormData({ ...formData, nationalityId: row.id });
          break;
        case "shift":
          setFormData({ ...formData, shiftId: row.id });
          break;
        case "leaveGroup":
          setFormData({ ...formData, leaveGroupId: row.id });
          break;
        case "addressLookup":
          setFormData({ ...formData, addressTypeId: row.id });
          break;
        case "addressCountry":
          setFormData({
            ...formData,
            addressCountryId: row.id,
            addressCityId: null,
          });
          break;
        case "addressCity":
          setFormData({ ...formData, addressCityId: row.id });
          break;
        default:
          break;
      }
      handleLookupClose();
    };

    return (
      <GenericSearchModal
        open={!!activeLookup}
        onClose={handleLookupClose}
        title={data.title}
        breadcrumbPath={data.subtitle}
        columns={data.columns}
        data={data.rows}
        onSelect={handleRowSelect}
        selectedValue={selectedLookupValues[activeLookup]}
        selectedValues={
          activeLookup === "teachingCourses"
            ? selectedTeachingCourseIds(formData.teachingCourseIds)
            : activeLookup === "teachingClasses"
              ? formData.teachingClassIds ?? []
              : activeLookup === "teacherCategory"
                ? formData.teachingCourseCategoryIds ?? []
                : undefined
        }
        selectedField="id"
        showColumnFilters
        showFooter
        totalRecords={data.rows.length}
      />
    );
  };

  const findLookupName = (list: any[], id: number | null | undefined) => {
    if (!id) return "";
    return list.find((item) => Number(item.id) === Number(id))?.name ?? "";
  };

  const getEmployeesForExport = async () => {
    const selectedCampusId = normalizeNullableNumber(
      localStorage.getItem("selectedCampusId"),
    );
    const campusQuery = selectedCampusId ? `&entityId=${selectedCampusId}` : "";
    const gradeQuery = gradeFilterId ? `&gradeId=${gradeFilterId}` : "";
    const response: any = await GetService(
      `/api/HumanResource/EmployeeProfile/v3?pageNumber=1&pageSize=100000${campusQuery}${gradeQuery}`,
    );
    return response?.data?.items ?? response?.data?.Items ?? [];
  };

  const employeeExportHeaders = [
    translate("S.No"),
    translate("Employee ID"),
    translate("Machine Code"),
    translate("Employee Name"),
    translate("Department"),
    translate("Designation"),
    translate("Grade"),
    translate("Status"),
    translate("Contact No"),
  ];

  const buildEmployeeExportRows = (employees: any[]) =>
    employees.map((employee: any, index: number) => [
      index + 1,
      employee.employeeCode ?? "",
      employee.machineCode ?? "",
      [employee.firstName, employee.lastName].filter(Boolean).join(" "),
      findLookupName(departments, employee.departmentId),
      findLookupName(designations, employee.designationId),
      findLookupName(grades, employee.gradeId),
      findLookupName(statuses, employee.statusId),
      employee.cellNo ?? "",
    ]);

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

  const exportEmployees = async (format: "pdf" | "excel") => {
    setExportMenuAnchor(null);
    setIsLoading(true);
    try {
      const employees = await getEmployeesForExport();
      if (!employees.length) {
        setToasterMessages([translate("No employees found for the selected grade")]);
        setToasterSeverity("warning");
        setShowToaster(true);
        return;
      }

      const exportRows = buildEmployeeExportRows(employees);
      const selectedGrade = gradeFilterId
        ? findLookupName(grades, gradeFilterId)
        : translate("All Grades");
      const generatedAt = new Date();
      const safeGradeName = selectedGrade
        .replace(/[^a-z0-9-_]+/gi, "-")
        .replace(/^-+|-+$/g, "")
        .toLowerCase() || "all-grades";
      const fileBase = `employee-profile-${safeGradeName}`;
      const reportPrimary = [111, 148, 68] as [number, number, number];
      const reportPrimaryDark = [83, 95, 32] as [number, number, number];
      const reportPrimarySoft = [247, 250, 244] as [number, number, number];

      if (format === "pdf") {
        const [{ jsPDF }, { default: autoTable }] = await Promise.all([
          import("jspdf"),
          import("jspdf-autotable"),
        ]);
        const document = new jsPDF({ orientation: "landscape", unit: "pt", format: "a4" });
        const pageWidth = document.internal.pageSize.getWidth();
        document.setFillColor(...reportPrimary);
        document.roundedRect(32, 20, pageWidth - 64, 44, 5, 5, "F");
        document.setTextColor(255, 255, 255);
        document.setFont("helvetica", "bold");
        document.setFontSize(16);
        document.text(translate("Employee Profile Report"), 46, 40);
        document.setFont("helvetica", "normal");
        document.setFontSize(9);
        document.setTextColor(240, 247, 233);
        document.text(`${translate("Grade")}: ${selectedGrade}`, 46, 55);
        document.text(
          `${translate("Generated")}: ${generatedAt.toLocaleString()}`,
          pageWidth - 46,
          55,
          { align: "right" },
        );

        autoTable(document, {
          startY: 76,
          head: [employeeExportHeaders],
          body: exportRows,
          theme: "grid",
          styles: {
            font: "helvetica",
            fontSize: 7.5,
            cellPadding: 4,
            overflow: "linebreak",
            textColor: [31, 41, 55],
            lineColor: [218, 226, 207],
            lineWidth: 0.4,
          },
          headStyles: {
            fillColor: reportPrimaryDark,
            textColor: 255,
            fontStyle: "bold",
            halign: "left",
          },
          alternateRowStyles: { fillColor: reportPrimarySoft },
          columnStyles: {
            0: { halign: "right", cellWidth: 30 },
            1: { cellWidth: 68 },
            2: { cellWidth: 62 },
            3: { cellWidth: 105 },
            4: { cellWidth: 105 },
            5: { cellWidth: 105 },
            6: { cellWidth: 65 },
            7: { cellWidth: 65 },
            8: { cellWidth: 82 },
          },
          margin: { left: 32, right: 32, bottom: 28 },
          didDrawPage: () => {
            const pageNumber = document.getCurrentPageInfo().pageNumber;
            const footerY = document.internal.pageSize.getHeight() - 20;
            document.setDrawColor(...reportPrimary);
            document.setLineWidth(0.7);
            document.line(32, footerY - 7, pageWidth - 32, footerY - 7);
            document.setFontSize(8);
            document.setTextColor(...reportPrimaryDark);
            document.text(
              `${translate("Page")} ${pageNumber}`,
              pageWidth - 32,
              footerY,
              { align: "right" },
            );
          },
        });
        document.save(`${fileBase}.pdf`);
      } else {
        const { default: ExcelJS } = await import("exceljs");
        const workbook = new ExcelJS.Workbook();
        workbook.creator = "EDXS Human Resource";
        workbook.created = generatedAt;
        const worksheet = workbook.addWorksheet("Employees", {
          views: [{ state: "frozen", ySplit: 4 }],
          pageSetup: { orientation: "landscape", fitToPage: true, fitToWidth: 1 },
          properties: { tabColor: { argb: "FF6F9444" } },
        });
        worksheet.columns = [
          { key: "serial", width: 8 },
          { key: "employeeCode", width: 17 },
          { key: "machineCode", width: 16 },
          { key: "employeeName", width: 28 },
          { key: "department", width: 24 },
          { key: "designation", width: 24 },
          { key: "grade", width: 16 },
          { key: "status", width: 16 },
          { key: "contact", width: 18 },
        ];
        worksheet.mergeCells("A1:I1");
        worksheet.getCell("A1").value = translate("Employee Profile Report");
        worksheet.getCell("A1").font = { name: "Calibri", size: 16, bold: true, color: { argb: "FFFFFFFF" } };
        worksheet.getCell("A1").fill = { type: "pattern", pattern: "solid", fgColor: { argb: "FF6F9444" } };
        worksheet.getCell("A1").alignment = { vertical: "middle", horizontal: "left" };
        worksheet.getRow(1).height = 28;
        worksheet.mergeCells("A2:I2");
        worksheet.getCell("A2").value = `${translate("Grade")}: ${selectedGrade} | ${translate("Generated")}: ${generatedAt.toLocaleString()}`;
        worksheet.getCell("A2").font = { name: "Calibri", size: 10, bold: true, color: { argb: "FF535F20" } };
        worksheet.getCell("A2").fill = { type: "pattern", pattern: "solid", fgColor: { argb: "FFF4F8EF" } };
        worksheet.getCell("A2").alignment = { vertical: "middle", horizontal: "left" };
        worksheet.getCell("A2").border = {
          bottom: { style: "thin", color: { argb: "FFB9CCA4" } },
        };
        worksheet.getRow(2).height = 20;
        worksheet.addRow([]);
        const headerRow = worksheet.addRow(employeeExportHeaders);
        headerRow.height = 22;
        headerRow.eachCell((cell) => {
          cell.font = { name: "Calibri", size: 10, bold: true, color: { argb: "FFFFFFFF" } };
          cell.fill = { type: "pattern", pattern: "solid", fgColor: { argb: "FF535F20" } };
          cell.alignment = { vertical: "middle", horizontal: "left" };
          cell.border = {
            bottom: { style: "thin", color: { argb: "FF6F9444" } },
          };
        });
        exportRows.forEach((row, index) => {
          const excelRow = worksheet.addRow(row);
          excelRow.height = 19;
          excelRow.eachCell((cell) => {
            cell.font = { name: "Calibri", size: 10, color: { argb: "FF1F2937" } };
            cell.alignment = { vertical: "middle", horizontal: Number(cell.col) === 1 ? "right" : "left" };
            cell.border = {
              bottom: { style: "hair", color: { argb: "FFDCE6D2" } },
            };
            if (index % 2 === 1) {
              cell.fill = { type: "pattern", pattern: "solid", fgColor: { argb: "FFF7FAF4" } };
            }
          });
        });
        worksheet.autoFilter = `A4:I${exportRows.length + 4}`;
        const buffer = await workbook.xlsx.writeBuffer();
        downloadBlob(
          new Blob([new Uint8Array(buffer)], {
            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
          }),
          `${fileBase}.xlsx`,
        );
      }

      setToasterMessages([
        translate(format === "pdf" ? "PDF report exported successfully" : "Excel report exported successfully"),
      ]);
      setToasterSeverity("success");
      setShowToaster(true);
    } catch (error) {
      console.error("Failed to export employee report", error);
      setToasterMessages([translate("Failed to export employee report")]);
      setToasterSeverity("error");
      setShowToaster(true);
    } finally {
      setIsLoading(false);
    }
  };

  const isTeacherCategory = (categoryId: any) => {
    const categoryName = findLookupName(
      employeeCategories,
      Number(categoryId || 0),
    );
    return (
      Number(categoryId) === teacherCategoryId ||
      categoryName.toLowerCase().includes("teacher")
    );
  };

  const isResignedStatus = (statusId: any) =>
    resignedStatusIds.has(Number(statusId || 0));

  const onlyDigits = (value: string) => value.replace(/\D/g, "");

  const applyDigitMask = (value: string, groups: number[]) => {
    const digits = onlyDigits(value).slice(
      0,
      groups.reduce((sum, group) => sum + group, 0),
    );

    if (!digits) return "";

    let cursor = 0;
    return groups
      .map((length) => {
        const part = digits.slice(cursor, cursor + length);
        cursor += length;
        return part.padEnd(length, "_");
      })
      .join("-");
  };

  const formatCnic = (value: string) => applyDigitMask(value, [5, 7, 1]);

  const formatContactNo = (value: string) => applyDigitMask(value, [4, 7]);

  const columns: Column[] = [
    {
      id: "sNo",
      label: translate("S.No"),
      minWidth: 50,
      format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
    },
    { id: "machineCode", label: translate("Machine Code"), minWidth: 100 },
    { id: "employeeCode", label: translate("Employee ID"), minWidth: 120 },
    { id: "firstName", label: translate("First Name"), minWidth: 150 },
    { id: "lastName", label: translate("Last Name"), minWidth: 120 },
    { id: "parentName", label: translate("Father Name"), minWidth: 150 },
    {
      id: "departmentId",
      label: translate("Department"),
      minWidth: 150,
      format: (val: any) => findLookupName(departments, val),
    },
    {
      id: "designationId",
      label: translate("Designation"),
      minWidth: 150,
      format: (val: any) => findLookupName(designations, val),
    },
    {
      id: "gradeId",
      label: translate("Grade"),
      minWidth: 110,
      format: (val: any) => findLookupName(grades, val),
    },
    {
      id: "statusId",
      label: translate("Status"),
      minWidth: 100,
      format: (val: any) => findLookupName(statuses, val),
    },
    {
      id: "maritalStatusId",
      label: translate("Marital Status"),
      minWidth: 120,
      format: (val: any) => findLookupName(maritalStatuses, val),
    },
    { id: "cellNo", label: translate("Contact No"), minWidth: 120 },
  ];

  const initialFormData = {
    id: null,
    employeeCode: "",
    machineCode: "",
    firstName: "",
    lastName: "",
    parentName: "",
    positionTypeId: null,
    joiningDate: "",
    employeeTypeId: null,
    employeeCategoryId: null,
    courseCategoryId: null,
    entityId: null,
    subCampusId: null,
    subCampusFromDate: "",
    subCampusTillDate: "",
    subCampusRemarks: "",
    departmentId: null,
    designationId: null,
    reportingToEmployeeId: null,
    substituteEmployeeId: null,
    statusId: null,
    gradeId: null,
    employmentRemarks: "",
    jobResponsibility: "",
    vacancyPositionId: null,
    resignDate: "",
    dateOfBirth: "",
    genderId: null,
    cnicNo: "",
    bloodGroupId: null,
    maritalStatusId: null,
    birthCountryId: null,
    birthCityId: null,
    nationalityId: null,
    religionId: null,
    emailId: "",
    cellNo: "",
    sessiNo: "",
    eobiNo: "",
    zoomUsername: "",
    zoomPassword: "",
    zoomMeetingId: "",
    zoomMeetingPassword: "",
    zoomInvitePassword: "",
    zoomApiKey: "",
    zoomSdkKey: "",
    zoomApiSecret: "",
    zoomSdkSecret: "",
    zoomToken: "",
    zoomAccountId: "",
    zoomClientSecret: "",
    zoomHostEmail: "",
    zoomUserId: "",
    jitsiDomain: "",
    jitsiAppId: "",
    jitsiAppSecret: "",
    googleServiceAccountJson: "",
    googleHostEmail: "",
    shiftId: null,
    leaveGroupId: null,
    addressTypeId: null,
    address: "",
    addressCountryId: null,
    addressCityId: null,
    imageId: null,
    teachingCourseIds: [],
    teachingClassIds: [],
    teachingCourseCategoryIds: [],
  };

  const hasRealSubCampusOptions = subCampuses.some((item: any) => Number(item.id || 0) > 0);

  const loadSubCampuses = useCallback(async (entityId?: any) => {
    const parsedEntityId = normalizeNullableNumber(entityId);
    if (!parsedEntityId) {
      setSubCampuses([]);
      return [];
    }

    let options: any[] = [];
    try {
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3/lookups/sub-campuses?entityId=${parsedEntityId}`,
      );
      const items = response?.data?.items ?? response?.data?.Items ?? response?.data ?? [];
      options = (Array.isArray(items) ? items : [])
        .map((item: any) => ({
          id: Number(item.id ?? item.Id ?? 0),
          code: item.code ?? item.Code ?? "",
          name: item.name ?? item.Name ?? "",
        }))
        .filter((item: any) => item.id > 0 && item.name);
    } catch (error) {
      console.error("Failed to fetch sub campuses", error);
    }

    setSubCampuses(options.length ? [{ id: 0, code: "", name: "None" }, ...options] : []);
    if (!options.length) {
      setSubCampuses([]);
      setFormData((prev: any) => ({
        ...prev,
        subCampusId: 0,
        subCampusFromDate: "",
        subCampusTillDate: "",
        subCampusRemarks: "",
      }));
    }
    return options;
  }, [normalizeNullableNumber]);

  // The list rows do not carry Teaching Specialization, so an opened employee fetches it
  // here. Until it arrives teachingCourseIds, teachingClassIds and
  // teachingCourseCategoryIds stay undefined and a save leaves them alone. Employees saved
  // before Teaching Level became multi-select only have courseCategoryId.
  const loadTeachingSubjectTypeIds = useCallback(async (employeeId?: any) => {
    const parsedEmployeeId = normalizeNullableNumber(employeeId);
    if (!parsedEmployeeId) return;

    try {
      const response: any = await GetService(
        `/api/HumanResource/EmployeeProfile/v3/${parsedEmployeeId}/teaching-subject-types`,
      );
      const toIds = (ids: any) =>
        (Array.isArray(ids) ? ids : []).map((id: any) => Number(id)).filter((id: number) => id > 0);
      const teachingCourseIds = toIds(response?.data?.teachingCourseIds);
      const teachingClassIds = toIds(response?.data?.teachingClassIds);
      const storedCourseCategoryIds = toIds(response?.data?.teachingCourseCategoryIds);
      setFormData((prev: any) =>
        Number(prev.id) === parsedEmployeeId
          ? {
              ...prev,
              teachingCourseIds,
              teachingClassIds,
              teachingCourseCategoryIds: storedCourseCategoryIds.length
                ? storedCourseCategoryIds
                : toIds([prev.courseCategoryId]),
            }
          : prev,
      );
    } catch (error) {
      console.error("Failed to fetch teaching specialization", error);
    }
  }, [normalizeNullableNumber]);

  useEffect(() => {
    if (vacancyPrefillHandledRef.current) return;
    const query = new URLSearchParams(window.location.search);
    const positionId = Number(query.get("vacancyPositionId") || 0);
    const promotionEmployeeId = Number(query.get("promotionEmployeeId") || 0);
    if (!positionId) return;
    vacancyPrefillHandledRef.current = true;

    void (async () => {
      try {
        const response: any = await GetService(
          `/api/HumanResource/VacancyManagement/v3/positions`,
        );
        const positions = response?.data ?? response ?? [];
        const position = (Array.isArray(positions) ? positions : []).find(
          (item: any) => Number(item.id) === positionId && item.status === "Vacant",
        );
        if (!position) throw new Error("The selected vacancy position is no longer available.");

        const selectedCampusId = normalizeNullableNumber(position.entityId);
        const existingEmployee = promotionEmployeeId > 0
          ? await GetService(`/api/HumanResource/EmployeeProfile/v3/${promotionEmployeeId}`)
          : null;
        const employeeProfile = existingEmployee?.data ?? existingEmployee;
        const employeeCode = promotionEmployeeId > 0
          ? employeeProfile?.employeeCode ?? ""
          : await generateEmployeeCodeFromNumberConfig();
        setFormData({
          ...(promotionEmployeeId > 0 ? employeeProfile : initialFormData),
          entityId: selectedCampusId,
          employeeCode,
          vacancyPositionId: positionId,
          promotionEffectiveDate: query.get("promotionEffectiveDate") || "",
          promotionRemarks: query.get("promotionRemarks") || "",
          departmentId: position.departmentId,
          designationId: position.designationId,
          gradeId: position.gradeId ?? null,
          employeeTypeId: position.employmentTypeId ?? null,
          jobResponsibility: position.responsibilities ?? "",
        });
        setIsSubmitted(false);
        setViewMode(promotionEmployeeId > 0 ? "edit" : "new");
        void loadDepartments(selectedCampusId);
        void loadSubCampuses(selectedCampusId);
        void loadReportingToEmployees(selectedCampusId, promotionEmployeeId || null);
        void loadTeachingSubjectTypeIds(promotionEmployeeId);
        void applyEmployeeProfileDefaults(selectedCampusId, true);
        setToasterMessages([
          translate(promotionEmployeeId > 0
            ? `Position ${position.positionNumber} loaded. Vacancy-controlled fields are locked; update the remaining employee details and save to schedule the promotion.`
            : `Position ${position.positionNumber} loaded. Complete the employee's personal and joining details.`),
        ]);
        setToasterSeverity("success");
        setShowToaster(true);
      } catch (error: any) {
        const responseMessage = error?.response?.data?.message ?? error?.response?.data ?? error?.message;
        setToasterMessages([
          typeof responseMessage === "string"
            ? responseMessage
            : translate("Failed to load the vacancy position"),
        ]);
        setToasterSeverity("error");
        setShowToaster(true);
      }
    })();
  }, [applyEmployeeProfileDefaults, generateEmployeeCodeFromNumberConfig, loadDepartments, loadReportingToEmployees, loadSubCampuses, loadTeachingSubjectTypeIds, normalizeNullableNumber, translate]);

  const handleSave = async (formData: any) => {
    const errors: string[] = [];
    const employeeCategoryId = Number(formData.employeeCategoryId || 0);
    // Professor Category is multi-select; the first one is also sent as courseCategoryId.
    const teachingCourseCategoryIds: number[] | undefined =
      formData.teachingCourseCategoryIds === undefined
        ? undefined
        : (formData.teachingCourseCategoryIds ?? []).map((id: any) => Number(id));
    const courseCategoryId = teachingCourseCategoryIds
      ? teachingCourseCategoryIds[0] ?? null
      : formData.courseCategoryId
        ? Number(formData.courseCategoryId)
        : null;
    const cnicDigits = onlyDigits(formData.cnicNo || "");
    const contactDigits = onlyDigits(formData.cellNo || "");
    const emailId = formData.emailId?.trim() || "";
    const toNumber = (val: any) =>
      val === null || val === undefined || val === "" ? null : Number(val);

    if (!CheckemptyString(formData.firstName))
      errors.push(translate("First Name is required"));
    if (!formData.positionTypeId)
      errors.push(translate("Position Type is required"));
    if (!formData.joiningDate)
      errors.push(translate("Joining Date is required"));
    if (!formData.employeeTypeId)
      errors.push(translate("Employee Type is required"));
    if (!employeeCategoryId)
      errors.push(translate("Employee Category is required"));
    // Same rule that shows the Teaching Specialization tab, where Teaching Level is picked.
    if (employeeCategoryId === teacherCategoryId && !courseCategoryId)
      errors.push(translate("Teaching Level is required"));
    if (!formData.entityId) errors.push(translate("Entity is required"));
    if (!formData.departmentId)
      errors.push(translate("Department is required"));
    if (!formData.designationId)
      errors.push(translate("Designation is required"));
    if (!formData.statusId) errors.push(translate("Status is required"));
    if (!formData.gradeId) errors.push(translate("Grade is required"));
    if (isResignedStatus(formData.statusId) && !formData.resignDate)
      errors.push(translate("Leaving Date is required"));
    if (!formData.dateOfBirth)
      errors.push(translate("Date of Birth is required"));
    if (!formData.genderId) errors.push(translate("Gender is required"));
    if (!formData.birthCountryId)
      errors.push(translate("Birth Country is required"));
    if (!formData.birthCityId) errors.push(translate("Birth City is required"));
    if (!formData.nationalityId)
      errors.push(translate("Nationality is required"));
    if (!contactDigits)
      errors.push(translate("Contact No is required"));
    else if (contactDigits.length !== 11)
      errors.push(translate("Contact No is invalid"));
    if (cnicDigits && cnicDigits.length !== 13)
      errors.push(translate("CNIC No is invalid"));
    if (emailId && !emailRegex.test(emailId))
      errors.push(translate("Email ID is invalid"));
    if (allowIntegrationTabs && !formData.shiftId) errors.push(translate("Shift is required"));
    // Zoom is optional per employee, but the Server-to-Server pair only works together
    // and the host email must be a real address for Zoom to resolve the account.
    const zoomAccountId = formData.zoomAccountId?.trim();
    const zoomClientSecret = formData.zoomClientSecret?.trim();
    const zoomHostEmail = formData.zoomHostEmail?.trim();
    if (zoomAccountId && !zoomClientSecret)
      errors.push(translate("Zoom Client Secret is required when an Account Id is supplied"));
    if (zoomClientSecret && !zoomAccountId)
      errors.push(translate("Zoom Account Id is required when a Client Secret is supplied"));
    if (zoomHostEmail && !emailRegex.test(zoomHostEmail))
      errors.push(translate("Zoom Host Email is invalid"));

    // Jitsi is optional too, but the JWT pair only works together: a token cannot be
    // signed with one half, and neither half means plain open rooms, which is valid.
    const jitsiAppId = formData.jitsiAppId?.trim();
    const jitsiAppSecret = formData.jitsiAppSecret?.trim();
    if (jitsiAppId && !jitsiAppSecret)
      errors.push(translate("Jitsi App Secret is required when an App ID is supplied"));
    if (jitsiAppSecret && !jitsiAppId)
      errors.push(translate("Jitsi App ID is required when an App Secret is supplied"));

    // Google Meet needs both halves too: the key says who signs, the address says who
    // the meeting belongs to. The JSON is parsed here so a bad paste is caught on save
    // rather than silently failing when a class starts.
    // Only the service-account half is validated here. A connected personal account
    // fills the same address field from Google itself, with no key to check.
    const googleJson = formData.googleServiceAccountJson?.trim();
    const googleHostEmail = formData.googleHostEmail?.trim();
    if (googleJson && !googleHostEmail)
      errors.push(translate("Google Account is required when a Service Account Key is supplied"));
    if (googleHostEmail && googleJson && !emailRegex.test(googleHostEmail))
      errors.push(translate("Google Account is invalid"));
    if (googleJson) {
      try {
        const parsed = JSON.parse(googleJson);
        if (!parsed?.client_email || !parsed?.private_key)
          errors.push(translate("Service Account Key must contain client_email and private_key"));
      } catch {
        errors.push(translate("Service Account Key is not valid JSON"));
      }
    }
    const selectedSubCampusId = hasRealSubCampusOptions ? toNumber(formData.subCampusId) : null;
    if (selectedSubCampusId) {
      if (!formData.subCampusFromDate)
        errors.push(translate("Sub Campus Start Date is required"));
      if (!formData.subCampusTillDate)
        errors.push(translate("Sub Campus Till Date is required"));
    }

    if (errors.length > 0) {
      setToasterMessages(errors);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }

    const payload = {
      id: formData.id ?? 0,
      entityId: toNumber(formData.entityId),
      employeeCode: formData.employeeCode?.trim() || null,
      machineCode: formData.machineCode?.trim() || null,
      firstName: formData.firstName?.trim() || null,
      lastName: formData.lastName?.trim() || null,
      parentName: formData.parentName?.trim() || null,
      positionTypeId: toNumber(formData.positionTypeId),
      employeeTypeId: toNumber(formData.employeeTypeId),
      employeeCategoryId: employeeCategoryId || null,
      courseCategoryId: isTeacherCategory(employeeCategoryId)
        ? courseCategoryId
        : null,
      departmentId: toNumber(formData.departmentId),
      designationId: toNumber(formData.designationId),
      reportingToEmployeeId: toNumber(formData.reportingToEmployeeId),
      substituteEmployeeId: toNumber(formData.substituteEmployeeId),
      statusId: toNumber(formData.statusId),
      gradeId: toNumber(formData.gradeId),
      employmentRemarks: formData.employmentRemarks?.trim() || null,
      jobResponsibility: formData.jobResponsibility?.trim() || null,
      joiningDate: formData.joiningDate || null,
      resignDate: isResignedStatus(formData.statusId)
        ? formData.resignDate || null
        : null,
      dateOfBirth: formData.dateOfBirth || null,
      genderId: toNumber(formData.genderId),
      bloodGroupId: toNumber(formData.bloodGroupId),
      maritalStatusId: toNumber(formData.maritalStatusId),
      birthCountryId: toNumber(formData.birthCountryId),
      birthCityId: toNumber(formData.birthCityId),
      nationalityId: toNumber(formData.nationalityId),
      religionId: toNumber(formData.religionId),
      emailId: emailId || null,
      cellNo: contactDigits.length === 11 ? formatContactNo(contactDigits) : null,
      cnicNo: cnicDigits.length === 13 ? formatCnic(cnicDigits) : null,
      sessiNo: formData.sessiNo?.trim() || null,
      eobiNo: formData.eobiNo?.trim() || null,
      shiftId: toNumber(formData.shiftId),
      leaveGroupId: toNumber(formData.leaveGroupId),
      addressTypeId: toNumber(formData.addressTypeId),
      address: formData.address?.trim() || null,
      addressCountryId: toNumber(formData.addressCountryId),
      addressCityId: toNumber(formData.addressCityId),
      imageId: formData.imageId,
      subCampusId: selectedSubCampusId,
      subCampusFromDate: selectedSubCampusId
        ? formData.subCampusFromDate || null
        : null,
      subCampusTillDate: selectedSubCampusId
        ? formData.subCampusTillDate || null
        : null,
      subCampusRemarks: selectedSubCampusId
        ? formData.subCampusRemarks?.trim() || null
        : null,
      zoomUsername: formData.zoomUsername?.trim() || null,
      zoomPassword: formData.zoomPassword?.trim() || null,
      zoomMeetingId: formData.zoomMeetingId?.trim() || null,
      zoomMeetingPassword: formData.zoomMeetingPassword?.trim() || null,
      zoomInvitePassword: formData.zoomInvitePassword?.trim() || null,
      zoomApiKey: formData.zoomApiKey?.trim() || null,
      zoomSdkKey: formData.zoomSdkKey?.trim() || null,
      zoomApiSecret: formData.zoomApiSecret?.trim() || null,
      zoomSdkSecret: formData.zoomSdkSecret?.trim() || null,
      zoomToken: formData.zoomToken?.trim() || null,
      zoomAccountId: formData.zoomAccountId?.trim() || null,
      zoomClientSecret: formData.zoomClientSecret?.trim() || null,
      zoomHostEmail: formData.zoomHostEmail?.trim() || null,
      zoomUserId: formData.zoomUserId?.trim() || null,
      jitsiDomain: formData.jitsiDomain?.trim() || null,
      jitsiAppId: formData.jitsiAppId?.trim() || null,
      jitsiAppSecret: formData.jitsiAppSecret?.trim() || null,
      googleServiceAccountJson: formData.googleServiceAccountJson?.trim() || null,
      googleHostEmail: formData.googleHostEmail?.trim() || null,
      vacancyPositionId: toNumber(formData.vacancyPositionId),
      promotionEffectiveDate: formData.promotionEffectiveDate || null,
      promotionRemarks: formData.promotionRemarks?.trim() || null,
      // undefined means they have not loaded yet, sent as null so the saved courses
      // are kept; an empty selection is sent as [], which removes them.
      teachingCourseIds:
        formData.teachingCourseIds === undefined
          ? null
          : (formData.teachingCourseIds ?? []).map((id: any) => Number(id)),
      teachingClassIds:
        formData.teachingClassIds === undefined
          ? null
          : (formData.teachingClassIds ?? []).map((id: any) => Number(id)),
      teachingCourseCategoryIds: teachingCourseCategoryIds ?? null,
    };

    try {
      if (payload.id) {
        await PutService("/api/HumanResource/EmployeeProfile/v3", payload);
        setToasterMessages([translate("Record updated successfully")]);
      } else {
        await PostService("/api/HumanResource/EmployeeProfile/v3", payload);
        setToasterMessages([translate("Record saved successfully")]);
      }
      setToasterSeverity("success");
      setShowToaster(true);
      await fetchEmployees();
      setViewMode("list");
      setIsSubmitted(false);
      return true;
    } catch (error: any) {
      console.error("Failed to save employee", error);
      const message =
        error?.response?.data?.error ||
        error?.response?.data ||
        translate("Failed to save employee");
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
      return false;
    }
  };

  const handleDelete = async (row: any) => {
    try {
      await DeleteService(`/api/HumanResource/EmployeeProfile/v3/${row.id}`);
      await fetchEmployees();
    } catch (error: any) {
      const responseData = error?.response?.data;
      const message =
        responseData?.error ??
        responseData?.message ??
        (typeof responseData === "string" ? responseData : null) ??
        error?.message ??
        translate("Failed to delete record");
      setToasterMessages([message]);
      setToasterSeverity("error");
      setShowToaster(true);
    }
  };

  const renderExpandIcon = (expanded: boolean) => (
    <Box
      sx={{
        width: 18,
        height: 18,
        borderRadius: "4px",
        border: "1px solid #E2E8F0",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color: "var(--lms-primary)",
        transition: "all 0.3s ease",
        bgcolor: expanded ? "var(--lms-primary)" : "transparent",
      }}
    >
      <Box
        sx={{
          width: 8,
          height: "2px",
          bgcolor: expanded ? "#fff" : "var(--lms-primary)",
          transition: "transform 0.3s ease",
          transform: expanded ? "rotate(0deg)" : "rotate(90deg)",
        }}
      />
      {!expanded && (
        <Box
          sx={{
            width: "2px",
            height: 8,
            bgcolor: "var(--lms-primary)",
            position: "absolute",
          }}
        />
      )}
    </Box>
  );

  const onImageSelect = async (file: File) => {
    if (viewMode === "view") return;

    // Upload to server
    const data = new FormData();
    data.append("file", file);

    setIsLoading(true);
    try {
      const response: any = await api.post("/api/HumanResource/v3/image/upload", data, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
      const newImageId = Number(response?.data ?? 0);
      if (newImageId > 0) {
        setFormData((prev: any) => ({ ...prev, imageId: newImageId }));
      } else throw new Error("No image ID returned");
    } catch (err) {
      console.error("Image upload failed", err);
      setToasterMessages([translate("Failed to upload image")]);
      setToasterSeverity("error");
      setShowToaster(true);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const onImageRemove = () => {
    if (viewMode === "view") return;
    setFormData((prev: any) => ({ ...prev, imageId: null }));
  };

  const [isBasicExpanded, setIsBasicExpanded] = useState(true);

  const renderForm = (
    formData: any,
    setFormData: any,
    _mode: "view" | "new" | "edit",
    _isReadOnly?: boolean,
    isSubmitted?: boolean,
  ) => {
    const isReadOnly = !!_isReadOnly;
    const inputStyles = {
      "& .MuiOutlinedInput-root": {
        bgcolor: "#fff",
        borderRadius: "8px",
        "& fieldset": { borderColor: "#ced4da" },
        "&:hover fieldset": { borderColor: "var(--lms-primary)" },
        "&.Mui-focused fieldset": { borderColor: "var(--lms-primary)" },
      },
      "& .MuiInputBase-input": { fontSize: "0.85rem", py: 1 },
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

    const handleMaskedChange = (
      event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
      field: "cnicNo" | "cellNo",
      formatter: (value: string) => string,
    ) => {
      const input = event.target;
      const caret = input.selectionStart ?? input.value.length;
      const digitsBeforeCaret = onlyDigits(input.value.slice(0, caret)).length;
      const nextValue = formatter(input.value);
      const nextCaret = caretPositionForDigitCount(nextValue, digitsBeforeCaret);

      setFormData({ ...formData, [field]: nextValue });
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

    const renderLookupField = ({
      label,
      value,
      options,
      lookupKey,
      onSelect,
      required = false,
      error = false,
      disabled = false,
    }: {
      label: string;
      value: any;
      options: any[];
      lookupKey: string;
      onSelect: (option: any) => void;
      required?: boolean;
      error?: boolean;
      disabled?: boolean;
    }) => (
      <LookupDropdown
        label={translate(label)}
        required={required}
        disabled={isReadOnly || disabled}
        value={value ?? ""}
        options={options}
        getOptionLabel={(option: any) => option?.name ?? ""}
        onSelect={onSelect}
        onSearchClick={() => setActiveLookup(lookupKey)}
        error={error}
        hideDropdownArrow
      />
    );

    const isVacancyFulfillment = Number(formData.vacancyPositionId || 0) > 0;
    return (
      <FormThemeWrapper>
        {renderLookupDialog(formData, setFormData)}
        <Box sx={{ p: 0 }}>
          {/* Section: Basic Information */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              mb: 2,
              cursor: "pointer",
              userSelect: "none",
              "&:hover": {
                "& .expand-icon-container": {
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
                },
              },
            }}
            onClick={() => setIsBasicExpanded(!isBasicExpanded)}
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
                sx={{ fontWeight: 800, fontSize: "0.9rem", color: "#333" }}
              >
                Basic Information
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
              {renderExpandIcon(isBasicExpanded)}
            </Box>
          </Box>

          <Collapse in={isBasicExpanded}>
            <Grid container spacing={3}>
              {/* Left Content Area */}
              <Grid item xs={12} md={8.5}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Employee ID")}
                        placeholder={translate("Auto Generated")}
                        disabled
                        value={formData.employeeCode || ""}
                        maxLength={employeeProfileMaxLength.employeeCode}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Machine Code")}
                        placeholder={translate("e.g. 259")}
                        disabled={isReadOnly}
                        value={formData.machineCode || ""}
                        maxLength={employeeProfileMaxLength.machineCode}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            machineCode: e.target.value,
                          })
                        }
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("First Name")}
                        required
                        disabled={isReadOnly}
                        placeholder={translate("Enter First Name")}
                        value={formData.firstName || ""}
                        maxLength={employeeProfileMaxLength.firstName}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            firstName: e.target.value,
                          })
                        }
                        error={isSubmitted && !formData.firstName}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Last Name")}
                        placeholder={translate("Enter Last Name")}
                        disabled={isReadOnly}
                        value={formData.lastName || ""}
                        maxLength={employeeProfileMaxLength.lastName}
                        onChange={(e) =>
                          setFormData({ ...formData, lastName: e.target.value })
                        }
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Father/Husband")}
                        placeholder={translate("Enter Father/Husband Name")}
                        disabled={isReadOnly}
                        value={formData.parentName || ""}
                        maxLength={employeeProfileMaxLength.parentName}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            parentName: e.target.value,
                          })
                        }
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Position Type",
                        value: formData.positionTypeId,
                        options: positionTypes,
                        lookupKey: "positionType",
                        required: true,
                        error: isSubmitted && !formData.positionTypeId,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            positionTypeId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormDatePicker
                        label={translate("Joining Date")}
                        required
                        disabled={isReadOnly}
                        value={
                          formData.joiningDate
                            ? dayjs(formData.joiningDate)
                            : null
                        }
                        onChange={(val) =>
                          setFormData({
                            ...formData,
                            joiningDate: val ? val.format("YYYY-MM-DD") : "",
                          })
                        }
                        format="MM/DD/YYYY"
                        slotProps={{
                          textField: {
                            size: "small",
                            fullWidth: true,
                            error: isSubmitted && !formData.joiningDate,
                            sx: {
                              "& .MuiOutlinedInput-root": {
                                bgcolor: "#fff",
                                borderRadius: "15px",
                                "& fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.joiningDate
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                                "&:hover fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.joiningDate
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                                "&.Mui-focused fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.joiningDate
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                              },
                              "& .MuiInputLabel-root": {
                                color: "var(--lms-primary) !important",
                                fontWeight: 600,
                                fontSize: "0.75rem",
                              },
                              "& .MuiSvgIcon-root": {
                                color: "var(--lms-primary)",
                              },
                            },
                          },
                        }}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Employee Type",
                        value: formData.employeeTypeId,
                        options: employeeTypes,
                        lookupKey: "employeeType",
                        required: true,
                        error: isSubmitted && !formData.employeeTypeId,
                        disabled: isVacancyFulfillment,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            employeeTypeId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Employee Category",
                        value: formData.employeeCategoryId,
                        options: employeeCategories,
                        lookupKey: "employeeCategory",
                        required: true,
                        error: isSubmitted && !formData.employeeCategoryId,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            employeeCategoryId: item.id,
                            courseCategoryId: null,
                            teachingCourseCategoryIds: [],
                          }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Entity",
                        value: formData.entityId,
                        options: entities,
                        lookupKey: "entity",
                        required: true,
                        error: isSubmitted && !formData.entityId,
                        disabled: isVacancyFulfillment,
                        onSelect: (item: any) => {
                          const entityId = normalizeNullableNumber(item.id);
                          const previousDepartmentId = formData.departmentId;
                          setFormData({
                            ...formData,
                            entityId,
                            reportingToEmployeeId: null,
                            substituteEmployeeId: null,
                            subCampusId: 0,
                            subCampusFromDate: "",
                            subCampusTillDate: "",
                            subCampusRemarks: "",
                          });
                          void loadDepartments(entityId).then((options) => {
                            if (previousDepartmentId && !options.some((option: any) => Number(option.id) === Number(previousDepartmentId))) {
                              setFormData((previous: any) =>
                                Number(previous.entityId) === Number(entityId)
                                  ? { ...previous, departmentId: null }
                                  : previous,
                              );
                            }
                          });
                          void loadSubCampuses(entityId);
                          void loadReportingToEmployees(entityId, formData.id);
                          if (_mode === "new") {
                            void applyEmployeeProfileDefaults(entityId, true);
                          }
                        },
                      })}
                    </Box>
                  </Grid>
                  {hasRealSubCampusOptions && (
                    <Grid item xs={12} md={4}>
                      <Box sx={{ mb: 1.5 }}>
                        {renderLookupField({
                          label: "Sub Campus",
                          value: formData.subCampusId,
                          options: subCampuses,
                          lookupKey: "subCampus",
                          onSelect: (item: any) => {
                            const selectedSubCampusId = Number(item.id || 0);
                            const subCampusId = selectedSubCampusId > 0 ? selectedSubCampusId : 0;
                            setFormData({
                              ...formData,
                              subCampusId,
                              subCampusFromDate: selectedSubCampusId > 0 ? formData.subCampusFromDate : "",
                              subCampusTillDate: selectedSubCampusId > 0 ? formData.subCampusTillDate : "",
                              subCampusRemarks: selectedSubCampusId > 0 ? formData.subCampusRemarks : "",
                            });
                          },
                        })}
                      </Box>
                    </Grid>
                  )}
                  {hasRealSubCampusOptions && Number(formData.subCampusId || 0) > 0 && (
                    <>
                      <Grid item xs={12} md={4}>
                        <Box sx={{ mb: 1.5 }}>
                          <FormDatePicker
                            label={translate("Start Date")}
                            disabled={isReadOnly}
                            value={
                              formData.subCampusFromDate
                                ? dayjs(formData.subCampusFromDate)
                                : null
                            }
                            onChange={(val) =>
                              setFormData({
                                ...formData,
                                subCampusFromDate: val ? val.format("YYYY-MM-DD") : "",
                              })
                            }
                            format="MM/DD/YYYY"
                            slotProps={{
                              textField: {
                                size: "small",
                                fullWidth: true,
                                error: isSubmitted && !formData.subCampusFromDate,
                              },
                            }}
                          />
                        </Box>
                      </Grid>
                      <Grid item xs={12} md={4}>
                        <Box sx={{ mb: 1.5 }}>
                          <FormDatePicker
                            label={translate("Till Date")}
                            disabled={isReadOnly}
                            value={
                              formData.subCampusTillDate
                                ? dayjs(formData.subCampusTillDate)
                                : null
                            }
                            onChange={(val) =>
                              setFormData({
                                ...formData,
                                subCampusTillDate: val ? val.format("YYYY-MM-DD") : "",
                              })
                            }
                            format="MM/DD/YYYY"
                            slotProps={{
                              textField: {
                                size: "small",
                                fullWidth: true,
                                error: isSubmitted && !formData.subCampusTillDate,
                              },
                            }}
                          />
                        </Box>
                      </Grid>
                      <Grid item xs={12} md={4}>
                        <Box sx={{ mb: 1.5 }}>
                          <FormInput
                            label={translate("Remarks")}
                            disabled={isReadOnly}
                            value={formData.subCampusRemarks || ""}
                            onChange={(e: any) =>
                              setFormData({
                                ...formData,
                                subCampusRemarks: e.target.value,
                              })
                            }
                            fullWidth
                            size="small"
                            multiline
                            rows={2}
                          />
                        </Box>
                      </Grid>
                    </>
                  )}
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Department",
                        value: formData.departmentId,
                        options: departments,
                        lookupKey: "department",
                        required: true,
                        error: isSubmitted && !formData.departmentId,
                        disabled: isVacancyFulfillment,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            departmentId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Designation",
                        value: formData.designationId,
                        options: designations,
                        lookupKey: "designation",
                        required: true,
                        error: isSubmitted && !formData.designationId,
                        disabled: isVacancyFulfillment,
                        onSelect: (item: any) => {
                          if (Number(formData.designationId || 0) !== Number(item.id || 0)) {
                            setPayrollTemplateVersion((version) => version + 1);
                          }
                          setFormData({
                            ...formData,
                            designationId: item.id,
                          });
                        },
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Reporting To",
                        value: formData.reportingToEmployeeId,
                        options: reportingToEmployees,
                        lookupKey: "reportingTo",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            reportingToEmployeeId: item.id || null,
                          }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Substitute",
                        value: formData.substituteEmployeeId,
                        options: reportingToEmployees,
                        lookupKey: "substitute",
                        onSelect: (item: any) =>
                          setFormData({ ...formData, substituteEmployeeId: item.id || null }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Status",
                        value: formData.statusId,
                        options: statuses,
                        lookupKey: "status",
                        required: true,
                        error: isSubmitted && !formData.statusId,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            statusId: item.id,
                            resignDate: isResignedStatus(item.id)
                              ? formData.resignDate
                              : "",
                          }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Grade",
                        value: formData.gradeId,
                        options: grades,
                        lookupKey: "grade",
                        required: true,
                        error: isSubmitted && !formData.gradeId,
                        disabled: isVacancyFulfillment,
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            gradeId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        label={translate("Employment Remarks")}
                        disabled={isReadOnly}
                        placeholder={translate("Enter remarks for status or grade change")}
                        value={formData.employmentRemarks || ""}
                        maxLength={1500}
                        onChange={(e) =>
                          setFormData({ ...formData, employmentRemarks: e.target.value })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        label={translate("Job Responsibility")}
                        disabled={isReadOnly || isVacancyFulfillment}
                        placeholder={translate("Enter current job responsibilities")}
                        value={formData.jobResponsibility || ""}
                        maxLength={1500}
                        onChange={(e) =>
                          setFormData({ ...formData, jobResponsibility: e.target.value })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  {isResignedStatus(formData.statusId) && (
                    <Grid item xs={12} md={4}>
                      <Box sx={{ mb: 1.5 }}>
                        <FormDatePicker
                          label={translate("Leaving Date *")}
                          disabled={isReadOnly}
                          value={
                            formData.resignDate
                              ? dayjs(formData.resignDate)
                              : null
                          }
                          onChange={(val) =>
                            setFormData({
                              ...formData,
                              resignDate: val ? val.format("YYYY-MM-DD") : "",
                            })
                          }
                          format="MM/DD/YYYY"
                        />
                      </Box>
                    </Grid>
                  )}
                </Grid>
              </Grid>

              {/* Right Side: Image Upload */}
              <Grid
                item
                xs={12}
                md={3}
                sx={{
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  pt: 5,
                }}
              >
                <Box
                  sx={{
                    width: 180,
                    height: "auto",
                    mb: 2,
                  }}
                >
                  <ImageUpload
                    disabled={isReadOnly}
                    currentImage={employeePhoto || ""}
                    onImageSelect={onImageSelect}
                    onImageRemove={onImageRemove}
                  />
                </Box>
              </Grid>
            </Grid>
          </Collapse>

          {/* Sub-Tabs Section */}
          <Box sx={{ mt: 3 }}>
            <StyledTabs
              value={activeSubTab}
              onChange={(_, v) => setActiveSubTab(v)}
              tabs={employeeProfileSubTabs}
            />

            <Box
              sx={{
                mt: -3,
                bgcolor: "#fff",
              }}
            >
              {/* Tab Panel: Personal Information */}
              <TabPanel value={activeSubTab} index={0} padding={3}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormDatePicker
                        label={translate("Date of Birth")}
                        required
                        disabled={isReadOnly}
                        value={
                          formData.dateOfBirth
                            ? dayjs(formData.dateOfBirth)
                            : null
                        }
                        onChange={(val) =>
                          setFormData({
                            ...formData,
                            dateOfBirth: val ? val.format("YYYY-MM-DD") : "",
                          })
                        }
                        format="MM/DD/YYYY"
                        slotProps={{
                          textField: {
                            size: "small",
                            fullWidth: true,
                            error: isSubmitted && !formData.dateOfBirth,
                            sx: {
                              "& .MuiOutlinedInput-root": {
                                bgcolor: "#fff",
                                borderRadius: "15px",
                                "& fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.dateOfBirth
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                                "&:hover fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.dateOfBirth
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                                "&.Mui-focused fieldset": {
                                  borderColor:
                                    isSubmitted && !formData.dateOfBirth
                                      ? "#d32f2f"
                                      : "var(--lms-primary)",
                                },
                              },
                              "& .MuiInputLabel-root": {
                                color: "var(--lms-primary) !important",
                                fontWeight: 600,
                                fontSize: "0.75rem",
                              },
                              "& .MuiSvgIcon-root": {
                                color: "var(--lms-primary)",
                              },
                            },
                          },
                        }}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Gender",
                        value: formData.genderId,
                        options: genders,
                        lookupKey: "gender",
                        required: true,
                        error: isSubmitted && !formData.genderId,
                        onSelect: (item: any) =>
                          setFormData({ ...formData, genderId: item.id }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("CNIC No")}
                        disabled={isReadOnly}
                        placeholder={translate("Enter CNIC Number")}
                        value={formData.cnicNo || ""}
                        debounceMs={0}
                        maxLength={employeeProfileMaxLength.cnicNo}
                        onChange={(e) => handleMaskedChange(e, "cnicNo", formatCnic)}
                        error={
                          isSubmitted &&
                          !!onlyDigits(formData.cnicNo || "") &&
                          onlyDigits(formData.cnicNo || "").length !== 13
                        }
                        inputProps={{ inputMode: "numeric" }}
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Contact No")}
                        required
                        placeholder={translate("Enter Contact No")}
                        value={formData.cellNo || ""}
                        debounceMs={0}
                        maxLength={employeeProfileMaxLength.cellNo}
                        onChange={(e) => handleMaskedChange(e, "cellNo", formatContactNo)}
                        error={
                          isSubmitted &&
                          onlyDigits(formData.cellNo || "").length !== 11
                        }
                        inputProps={{ inputMode: "numeric" }}
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("EOBI No")}
                        placeholder={translate("Enter EOBI No")}
                        value={formData.eobiNo || ""}
                        onChange={(e) =>
                          setFormData({ ...formData, eobiNo: e.target.value })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Blood Group",
                        value: formData.bloodGroupId,
                        options: bloodGroups,
                        lookupKey: "bloodGroup",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            bloodGroupId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Marital Status",
                        value: formData.maritalStatusId,
                        options: maritalStatuses,
                        lookupKey: "maritalStatus",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            maritalStatusId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Birth Country")}
                        required
                        disabled={isReadOnly}
                        value={formData.birthCountryId || ""}
                        options={countries}
                        getOptionLabel={(o: any) => o.name}
                        onSelect={(o: any) =>
                          setFormData({
                            ...formData,
                            birthCountryId: o.id,
                            birthCityId: null,
                          })
                        }
                        onSearchClick={() => setActiveLookup("birthCountry")}
                        error={isSubmitted && !formData.birthCountryId}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Birth City")}
                        required
                        disabled={isReadOnly}
                        value={formData.birthCityId || ""}
                        options={cities.filter(
                          (city: any) =>
                            !formData.birthCountryId ||
                            city.countryId === Number(formData.birthCountryId)
                        )}
                        getOptionLabel={(o: any) => o.name}
                        onSelect={(o: any) =>
                          setFormData({
                            ...formData,
                            birthCityId: o.id,
                          })
                        }
                        onSearchClick={() => setActiveLookup("birthCity")}
                        error={isSubmitted && !formData.birthCityId}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Nationality")}
                        required
                        disabled={isReadOnly}
                        value={formData.nationalityId || ""}
                        options={countries}
                        getOptionLabel={(o: any) => o.name}
                        onSelect={(o: any) =>
                          setFormData({
                            ...formData,
                            nationalityId: o.id,
                          })
                        }
                        onSearchClick={() => setActiveLookup("nationality")}
                        error={isSubmitted && !formData.nationalityId}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Religion",
                        value: formData.religionId,
                        options: religions,
                        lookupKey: "religion",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            religionId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Email ID")}
                        type="email"
                        placeholder={translate("Enter Email")}
                        disabled={isReadOnly}
                        value={formData.emailId || ""}
                        maxLength={employeeProfileMaxLength.emailId}
                        onChange={(e) =>
                          setFormData({ ...formData, emailId: e.target.value })
                        }
                        error={
                          isSubmitted &&
                          !!formData.emailId?.trim() &&
                          !emailRegex.test(formData.emailId.trim())
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Sessi No")}
                        placeholder={translate("Enter sessi no")}
                        disabled={isReadOnly}
                        value={formData.sessiNo || ""}
                        onChange={(e) =>
                          setFormData({ ...formData, sessiNo: e.target.value })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                </Grid>
              </TabPanel>

              {/* Tab Panel: Online Meeting (Zoom / Jitsi credentials) */}
              {getEmployeeProfileSubTabIndex("onlineMeeting") >= 0 && (
              <TabPanel value={activeSubTab} index={getEmployeeProfileSubTabIndex("onlineMeeting")} padding={3}>
                <StyledTabs
                  value={activeMeetingProviderTab}
                  onChange={(_, v) => setActiveMeetingProviderTab(v)}
                  tabs={meetingProviderTabs}
                />

                <Box sx={{ mt: -3, bgcolor: "#fff" }}>
                <TabPanel value={activeMeetingProviderTab} index={0} padding={3}>
                <Typography variant="subtitle2" sx={{ mb: 0.5, fontWeight: 800 }}>
                  {translate("Server-to-Server OAuth App")}
                </Typography>
                <Typography variant="caption" sx={{ display: "block", mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
                  {translate("Required for this employee to create Zoom meetings. The Client ID is taken from the Institution; the Account ID and Client Secret below belong to this employee's own Zoom account, paid or free.")}
                </Typography>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Account Id")}
                        placeholder={translate("Enter Account Id")}
                        disabled={isReadOnly}
                        value={formData.zoomAccountId || ""}
                        maxLength={employeeProfileMaxLength.zoomAccountId}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomAccountId: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        type="password"
                        autoComplete="new-password"
                        label={translate("Client Secret")}
                        placeholder={translate("Enter Client Secret")}
                        disabled={isReadOnly}
                        value={formData.zoomClientSecret || ""}
                        maxLength={employeeProfileMaxLength.zoomClientSecret}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomClientSecret: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Host Email")}
                        placeholder={translate("Enter Host Email")}
                        disabled={isReadOnly}
                        error={isSubmitted && !!formData.zoomHostEmail?.trim() && !emailRegex.test(formData.zoomHostEmail.trim())}
                        value={formData.zoomHostEmail || ""}
                        maxLength={employeeProfileMaxLength.zoomHostEmail}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomHostEmail: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Zoom User Id")}
                        placeholder={translate("Enter Zoom User Id")}
                        disabled={isReadOnly}
                        value={formData.zoomUserId || ""}
                        maxLength={employeeProfileMaxLength.zoomUserId}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomUserId: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                </Grid>

                <Box sx={{ display: "flex", alignItems: "center", gap: 2, mb: 1, flexWrap: "wrap" }}>
                  <Button
                    variant="outlined"
                    onClick={testZoomConnection}
                    disabled={
                      isReadOnly ||
                      zoomTest.busy ||
                      !Number(formData.entityId) ||
                      !formData.zoomAccountId?.trim() ||
                      !formData.zoomClientSecret?.trim()
                    }
                    startIcon={zoomTest.busy ? <CircularProgress size={16} /> : undefined}
                    sx={{
                      height: 40,
                      borderRadius: "9px",
                      fontWeight: 700,
                      textTransform: "none",
                      borderColor: "var(--lms-primary)",
                      color: "var(--lms-primary)",
                      "&:hover": { borderColor: "var(--lms-primary-dark)", bgcolor: "rgba(111, 148, 68, 0.04)" },
                    }}
                  >
                    {zoomTest.busy ? translate("Testing...") : translate("Test Connection")}
                  </Button>
                  {zoomTest.ok !== null && (
                    <Typography
                      variant="body2"
                      sx={{ fontWeight: 600, color: zoomTest.ok ? "var(--lms-primary)" : "#d32f2f" }}
                    >
                      {(zoomTest.ok ? "✓ " : "✕ ") + zoomTest.message}
                    </Typography>
                  )}
                </Box>

                <Typography variant="subtitle2" sx={{ mt: 2, mb: 1.5, fontWeight: 800 }}>
                  {translate("Personal Meeting Room")}
                </Typography>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Username")}
                        placeholder={translate("Enter Username")}
                        disabled={isReadOnly}
                        value={formData.zoomUsername || ""}
                        maxLength={employeeProfileMaxLength.zoomUsername}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomUsername: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Password")}
                        type="password"
                        placeholder={translate("**********")}
                        disabled={isReadOnly}
                        value={formData.zoomPassword || ""}
                        maxLength={employeeProfileMaxLength.zoomPassword}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomPassword: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Meeting Id")}
                        placeholder={translate("Enter Meeting Id")}
                        disabled={isReadOnly}
                        value={formData.zoomMeetingId || ""}
                        maxLength={employeeProfileMaxLength.zoomMeetingId}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomMeetingId: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Meeting Password")}
                        placeholder={translate("Enter Meeting Password")}
                        disabled={isReadOnly}
                        value={formData.zoomMeetingPassword || ""}
                        maxLength={employeeProfileMaxLength.zoomMeetingPassword}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomMeetingPassword: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Invite Password")}
                        placeholder={translate("Enter Invite Password")}
                        disabled={isReadOnly}
                        value={formData.zoomInvitePassword || ""}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomInvitePassword: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Api Key")}
                        placeholder={translate("Enter Api Key")}
                        value={formData.zoomApiKey || ""}
                        maxLength={employeeProfileMaxLength.zoomApiKey}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomApiKey: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("SDK Key")}
                        placeholder={translate("Enter SDK Key")}
                        value={formData.zoomSdkKey || ""}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomSdkKey: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Api Secret")}
                        placeholder={translate("Enter Api Secret")}
                        value={formData.zoomApiSecret || ""}
                        maxLength={employeeProfileMaxLength.zoomApiSecret}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomApiSecret: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("SDK Secret")}
                        placeholder={translate("Enter SDK Secret")}
                        value={formData.zoomSdkSecret || ""}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomSdkSecret: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Zoom Token")}
                        placeholder={translate("Enter Zoom Token")}
                        value={formData.zoomToken || ""}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            zoomToken: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                </Grid>
                </TabPanel>

                {/* Provider Panel: Jitsi */}
                <TabPanel value={activeMeetingProviderTab} index={1} padding={3}>
                  <Typography variant="subtitle2" sx={{ mb: 0.5, fontWeight: 800 }}>
                    {translate("Jitsi Server")}
                  </Typography>
                  <Typography variant="caption" sx={{ display: "block", mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
                    {translate("Leave the App ID and Secret empty to use open rooms on the server below. Filling both in issues a signed token per participant, so this host joins as moderator and everyone else joins as a guest.")}
                  </Typography>
                  <Grid container spacing={2}>
                    <Grid item xs={12} md={4}>
                      <Box sx={{ mb: 1.5 }}>
                        <FormInput
                          fullWidth
                          size="small"
                          label={translate("Server Domain")}
                          placeholder={translate("e.g. meet.example.edu")}
                          disabled={isReadOnly}
                          value={formData.jitsiDomain || ""}
                          maxLength={employeeProfileMaxLength.jitsiDomain}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              jitsiDomain: e.target.value,
                            })
                          }
                          sx={inputStyles}
                        />
                      </Box>
                    </Grid>
                    <Grid item xs={12} md={4}>
                      <Box sx={{ mb: 1.5 }}>
                        <FormInput
                          fullWidth
                          size="small"
                          label={translate("App ID")}
                          placeholder={translate("Enter App ID")}
                          disabled={isReadOnly}
                          value={formData.jitsiAppId || ""}
                          maxLength={employeeProfileMaxLength.jitsiAppId}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              jitsiAppId: e.target.value,
                            })
                          }
                          sx={inputStyles}
                        />
                      </Box>
                    </Grid>
                    <Grid item xs={12} md={4}>
                      <Box sx={{ mb: 1.5 }}>
                        <FormInput
                          fullWidth
                          size="small"
                          label={translate("App Secret")}
                          type="password"
                          placeholder={translate("**********")}
                          disabled={isReadOnly}
                          value={formData.jitsiAppSecret || ""}
                          maxLength={employeeProfileMaxLength.jitsiAppSecret}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              jitsiAppSecret: e.target.value,
                            })
                          }
                          sx={inputStyles}
                        />
                      </Box>
                    </Grid>
                  </Grid>
                </TabPanel>

                {/* Provider Panel: Google Meet */}
                <TabPanel value={activeMeetingProviderTab} index={2} padding={3}>
                  <Typography variant="subtitle2" sx={{ mb: 0.5, fontWeight: 800 }}>
                    {translate("Google Meet")}
                  </Typography>
                  <Typography variant="caption" sx={{ display: "block", mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
                    {translate("Meet links are created on this employee's Google Calendar. Connect a personal Google account for free, or use a Workspace service account if your institution has one.")}
                  </Typography>

                  <RadioGroup
                    row
                    value={googleAuthMode}
                    onChange={(e) => setGoogleAuthMode(e.target.value as "oauth" | "serviceAccount")}
                    sx={{ mb: 2 }}
                  >
                    <FormControlLabel
                      value="oauth"
                      disabled={isReadOnly}
                      control={<Radio size="small" sx={{ "&.Mui-checked": { color: "var(--lms-primary)" } }} />}
                      label={translate("Connect a Google account (free)")}
                    />
                    <FormControlLabel
                      value="serviceAccount"
                      disabled={isReadOnly}
                      control={<Radio size="small" sx={{ "&.Mui-checked": { color: "var(--lms-primary)" } }} />}
                      label={translate("Google Workspace service account")}
                    />
                  </RadioGroup>

                  {googleAuthMode === "oauth" ? (
                    <Box>
                      <Typography variant="caption" sx={{ display: "block", mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
                        {translate("Works with any Google account, including gmail.com. The employee approves once in a browser and can revoke access at any time from their Google account.")}
                      </Typography>
                      <Box sx={{ display: "flex", alignItems: "center", gap: 2, flexWrap: "wrap" }}>
                        <Button
                          variant="outlined"
                          onClick={formData.googleHostEmail && !formData.googleServiceAccountJson ? disconnectGoogle : startGoogleConnect}
                          disabled={isReadOnly || googleConnect.busy || !formData.id}
                          startIcon={googleConnect.busy ? <CircularProgress size={16} /> : <GoogleMeetIcon />}
                          sx={{
                            height: 40,
                            borderRadius: "9px",
                            fontWeight: 700,
                            textTransform: "none",
                            borderColor: "var(--lms-primary)",
                            color: "var(--lms-primary)",
                            "&:hover": { borderColor: "var(--lms-primary-dark)", bgcolor: "rgba(111, 148, 68, 0.04)" },
                          }}
                        >
                          {formData.googleHostEmail && !formData.googleServiceAccountJson
                            ? translate("Disconnect")
                            : translate("Connect Google Account")}
                        </Button>
                        {formData.googleHostEmail && !formData.googleServiceAccountJson && (
                          <Typography variant="body2" sx={{ fontWeight: 600, color: "var(--lms-primary)" }}>
                            {translate("Connected as") + " " + formData.googleHostEmail}
                          </Typography>
                        )}
                        {googleConnect.message && (
                          <Typography
                            variant="body2"
                            sx={{ fontWeight: 600, color: googleConnect.error ? "#d32f2f" : "var(--lms-primary)" }}
                          >
                            {(googleConnect.error ? "✕ " : "✓ ") + googleConnect.message}
                          </Typography>
                        )}
                      </Box>
                      {!formData.id && (
                        <Typography variant="caption" sx={{ display: "block", mt: 1, color: "#d32f2f" }}>
                          {translate("Save this employee before connecting a Google account.")}
                        </Typography>
                      )}
                    </Box>
                  ) : (
                    <Box>
                      <Typography variant="caption" sx={{ display: "block", mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
                        {translate("Requires a Google Workspace domain. The service account needs domain-wide delegation for the Calendar scope, granted once by a Workspace admin.")}
                      </Typography>
                      <Grid container spacing={2}>
                        <Grid item xs={12} md={6}>
                          <Box sx={{ mb: 1.5 }}>
                            <FormInput
                              fullWidth
                              size="small"
                              label={translate("Google Account")}
                              placeholder={translate("teacher@yourschool.edu")}
                              disabled={isReadOnly}
                              error={
                                isSubmitted &&
                                !!formData.googleHostEmail?.trim() &&
                                !emailRegex.test(formData.googleHostEmail.trim())
                              }
                              value={formData.googleHostEmail || ""}
                              maxLength={employeeProfileMaxLength.googleHostEmail}
                              onChange={(e) =>
                                setFormData({
                                  ...formData,
                                  googleHostEmail: e.target.value,
                                })
                              }
                              sx={inputStyles}
                            />
                          </Box>
                        </Grid>
                        <Grid item xs={12}>
                          <Box sx={{ mb: 1.5 }}>
                            <FormInput
                              fullWidth
                              multiline
                              minRows={6}
                              size="small"
                              label={translate("Service Account Key (JSON)")}
                              placeholder={translate("Paste the entire service-account JSON key file here")}
                              disabled={isReadOnly}
                              value={formData.googleServiceAccountJson || ""}
                              onChange={(e) =>
                                setFormData({
                                  ...formData,
                                  googleServiceAccountJson: e.target.value,
                                })
                              }
                              sx={inputStyles}
                            />
                          </Box>
                        </Grid>
                      </Grid>
                    </Box>
                  )}
                </TabPanel>
                </Box>
              </TabPanel>
              )}

              {/* Tab Panel: Employee Roster */}
              {getEmployeeProfileSubTabIndex("roster") >= 0 && (
              <TabPanel value={activeSubTab} index={getEmployeeProfileSubTabIndex("roster")} padding={3}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Shift",
                        value: formData.shiftId,
                        options: shifts,
                        lookupKey: "shift",
                        required: true,
                        error: isSubmitted && !formData.shiftId,
                        onSelect: (item: any) =>
                          setFormData({ ...formData, shiftId: item.id }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Leave Group",
                        value: formData.leaveGroupId,
                        options: leaveGroups,
                        lookupKey: "leaveGroup",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            leaveGroupId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                </Grid>
              </TabPanel>
              )}

              {/* Tab Panel: Address */}
              {getEmployeeProfileSubTabIndex("address") >= 0 && (
              <TabPanel value={activeSubTab} index={getEmployeeProfileSubTabIndex("address")} padding={3}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Address Type",
                        value: formData.addressTypeId,
                        options: addressTypes,
                        lookupKey: "addressLookup",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            addressTypeId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Address Country",
                        value: formData.addressCountryId,
                        options: countries,
                        lookupKey: "addressCountry",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            addressCountryId: item.id,
                            addressCityId: null,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      {renderLookupField({
                        label: "Address City",
                        value: formData.addressCityId,
                        options: cities.filter(
                          (city: any) =>
                            !formData.addressCountryId ||
                            city.countryId ===
                              Number(formData.addressCountryId),
                        ),
                        lookupKey: "addressCity",
                        onSelect: (item: any) =>
                          setFormData({
                            ...formData,
                            addressCityId: item.id,
                          }),
                      })}
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Box sx={{ mb: 1.5 }}>
                      <FormInput
                        fullWidth
                        size="small"
                        label={translate("Address Detail")}
                        disabled={isReadOnly}
                        placeholder={translate("Enter Address")}
                        value={formData.address || ""}
                        maxLength={employeeProfileMaxLength.address}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            address: e.target.value,
                          })
                        }
                        sx={inputStyles}
                      />
                    </Box>
                  </Grid>
                </Grid>
              </TabPanel>
              )}

              {/* Tab Panel: Teaching Specialization */}
              {getEmployeeProfileSubTabIndex("teachingSpecialization") >= 0 && (
              <TabPanel value={activeSubTab} index={getEmployeeProfileSubTabIndex("teachingSpecialization")} padding={3}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={8}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Teaching Level")}
                        multiple
                        required
                        error={isSubmitted && !(formData.teachingCourseCategoryIds ?? []).length}
                        disabled={isReadOnly}
                        value={formData.teachingCourseCategoryIds ?? []}
                        options={teacherCategories}
                        getOptionLabel={(option: any) => option?.name ?? ""}
                        onMultiSelect={(items: any[]) =>
                          setFormData({
                            ...formData,
                            teachingCourseCategoryIds: items.map((item: any) => Number(item.id)),
                          })
                        }
                        onSearchClick={() => setActiveLookup("teacherCategory")}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={8}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Subject")}
                        multiple
                        disabled={isReadOnly}
                        value={selectedTeachingCourseIds(formData.teachingCourseIds)}
                        options={teachingCourses}
                        getOptionLabel={(option: any) => option?.name ?? ""}
                        onMultiSelect={(items: any[]) =>
                          setFormData({
                            ...formData,
                            teachingCourseIds: items.map((item: any) => Number(item.id)),
                          })
                        }
                        onSearchClick={() => setActiveLookup("teachingCourses")}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>
                  <Grid item xs={12} md={8}>
                    <Box sx={{ mb: 1.5 }}>
                      <LookupDropdown
                        label={translate("Class Range")}
                        multiple
                        disabled={isReadOnly}
                        value={formData.teachingClassIds ?? []}
                        options={teachingClasses}
                        getOptionLabel={(option: any) => option?.name ?? ""}
                        onMultiSelect={(items: any[]) =>
                          setFormData({
                            ...formData,
                            teachingClassIds: items.map((item: any) => Number(item.id)),
                          })
                        }
                        onSearchClick={() => setActiveLookup("teachingClasses")}
                        hideDropdownArrow
                      />
                    </Box>
                  </Grid>
                </Grid>
              </TabPanel>
              )}
              {getEmployeeProfileSubTabIndex("salary") >= 0 && formData.id && (
                <TabPanel value={activeSubTab} index={getEmployeeProfileSubTabIndex("salary")} padding={3}>
                  <EmployeePayrollTab
                    employeeId={String(formData.id)}
                    designationId={Number(formData.designationId || 0) || undefined}
                    applyDesignationTemplateKey={payrollTemplateVersion || undefined}
                    readOnly={Number(formData.vacancyPositionId || 0) > 0}
                  />
                </TabPanel>
              )}
            </Box>
          </Box>
        </Box>
      </FormThemeWrapper>
    );
  };

  return (
    <>
      {(isLoading || employeeCreationPolicyLoading) && <LmsOverlayLoader />}
      {viewMode === "report" ? (
        <ReportsPage
          category={employeeProfileReportContext?.category || ""}
          sections={employeeProfileReportContext?.sections || []}
          initialSelectedReport={
            employeeProfileReportContext?.selectedReport || null
          }
          embedded
          onBack={() => setViewMode("list")}
        />
      ) : viewMode === "additionalInfo" ? (
        <EmployeeAdditionalInfoPage
          employeeId={additionalInfoEmployeeId}
          embedded
          onBack={() => {
            setAdditionalInfoEmployeeId(undefined);
            setViewMode("list");
          }}
        />
      ) : viewMode !== "list" ? (
        <Box sx={{ p: 2 }}>
          <Box
            sx={{
                p: 2,
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <Box>
                <Typography
                  variant="h5"
                  sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}
                >
                  {viewMode === "new"
                    ? translate("New Employee Profile")
                    : viewMode === "edit"
                      ? translate("Edit Employee Profile")
                      : translate("View Employee Profile")}
                </Typography>
              </Box>
              <Box sx={{ display: "flex", gap: 2 }}>
                {(viewMode === "new" || viewMode === "edit" || viewMode === "view") && (
                  <Button
                    variant="outlined"
                    startIcon={<PersonIcon />}
                    onClick={() => {
                      const employeeId = formData.id ?? formData.Id;
                      if (!employeeId) {
                        setToasterMessages([
                          translate(
                            "Save employee profile before opening additional information",
                          ),
                        ]);
                        setToasterSeverity("warning");
                        setShowToaster(true);
                        return;
                      }

                      setAdditionalInfoEmployeeId(String(employeeId));
                      setViewMode("additionalInfo");
                    }}
                    sx={{
                      borderRadius: "10px",
                      textTransform: "none",
                      fontWeight: 700,
                    }}
                  >
                    {translate("Additional Information")}
                  </Button>
                )}
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
                      await handleSave(formData);
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
              {renderForm(
                formData,
                setFormData,
                viewMode,
                viewMode === "view",
                isSubmitted,
              )}
            </Box>
        </Box>
      ) : (
        <SchemaTable
          title="Employee Profile"
          addButtonLabel="New"
          breadcrumbPath=""
          columns={columns}
          rows={rows}
          onDelete={handleDelete}
          disableAddAction={employeeCreationPolicyLoading || !canCreateNewEmployee}
          extraToolbarItemsLeft={
            !employeeCreationPolicyLoading && !canCreateNewEmployee ? (
              <Typography
                variant="caption"
                sx={{ color: "#92400e", fontWeight: 700 }}
              >
                {employeeCreationPolicyMessage}
              </Typography>
            ) : undefined
          }
          onAddClick={() => {
            if (!canCreateNewEmployee) {
              setToasterMessages([
                employeeCreationPolicyMessage ||
                  translate("Vacancy Process is enabled. Create a vacancy before adding a new employee."),
              ]);
              setToasterSeverity("warning");
              setShowToaster(true);
              return;
            }
            void (async () => {
            const selectedCampusId = normalizeNullableNumber(
              localStorage.getItem("selectedCampusId"),
            );
            const employeeCode = await generateEmployeeCodeFromNumberConfig();
            setFormData({
              ...initialFormData,
              entityId: selectedCampusId,
              employeeCode,
            });
            setSubCampuses([]);
            void loadReportingToEmployees(selectedCampusId);
            setIsSubmitted(false);
            setViewMode("new");
            void loadSubCampuses(selectedCampusId);
            void applyEmployeeProfileDefaults(selectedCampusId, true);
            })();
          }}
          onEditClick={(row: any) => {
            void loadDepartments(row.entityId);
            void loadSubCampuses(row.entityId);
            void loadReportingToEmployees(row.entityId, row.id);
            setFormData({
              ...row,
              cnicNo: formatCnic(row.cnicNo || ""),
              cellNo: formatContactNo(row.cellNo || ""),
            });
            void loadTeachingSubjectTypeIds(row.id);
            setIsSubmitted(false);
            setViewMode("edit");
          }}
          onViewClick={(row: any) => {
            void loadDepartments(row.entityId);
            void loadSubCampuses(row.entityId);
            void loadReportingToEmployees(row.entityId, row.id);
            setFormData({
              ...row,
              cnicNo: formatCnic(row.cnicNo || ""),
              cellNo: formatContactNo(row.cellNo || ""),
            });
            void loadTeachingSubjectTypeIds(row.id);
            setIsSubmitted(false);
            setViewMode("view");
          }}
          showAddButton={true}
          downloadEnabled={false}
          extraToolbarItems={canViewEmployeeProfileReport ? (
            <Button
              variant="contained"
              startIcon={<AssessmentIcon />}
              sx={{
                minHeight: 36,
                textTransform: "none",
                fontWeight: 700,
                color: "#fff",
                bgcolor: "#0ea5e9",
                borderRadius: "8px",
                px: 1.5,
                py: 0.65,
                boxShadow: "none",
                whiteSpace: "nowrap",
                "&:hover": {
                  bgcolor: "#0284c7",
                  boxShadow: "none",
                },
              }}
              onClick={() => setViewMode("report")}
            >
              {translate(employeeProfileReportName)}
            </Button>
          ) : undefined}
          extraToolbarItemsRight={(
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                flexWrap: "wrap",
                justifyContent: "flex-end",
                gap: 1,
              }}
            >
              <Box
                sx={{
                  height: 36,
                  display: "flex",
                  alignItems: "center",
                  overflow: "hidden",
                  bgcolor: "#fff",
                  border: "1px solid #d7dee8",
                  borderRadius: "8px",
                  transition: "border-color 0.2s ease, box-shadow 0.2s ease",
                  "&:focus-within": {
                    borderColor: "var(--lms-primary)",
                    boxShadow: "0 0 0 3px rgba(var(--lms-primary-rgb), 0.12)",
                  },
                }}
              >
                <Typography
                  component="span"
                  sx={{
                    px: 1.25,
                    color: "#64748b",
                    fontSize: 11,
                    fontWeight: 800,
                    letterSpacing: "0.06em",
                    lineHeight: 1,
                    textTransform: "uppercase",
                    whiteSpace: "nowrap",
                  }}
                >
                  {translate("Grade")}
                </Typography>
                <Box sx={{ width: "1px", alignSelf: "stretch", bgcolor: "#e2e8f0" }} />
                <FormControl size="small" variant="standard" sx={{ minWidth: 170 }}>
                  <Select
                    disableUnderline
                    displayEmpty
                    disabled={isLoading}
                    value={gradeFilterId ?? ""}
                    onChange={(event) => {
                      const value = Number(event.target.value || 0);
                      setGradeFilterId(value > 0 ? value : null);
                    }}
                    inputProps={{ "aria-label": translate("Filter employees by grade") }}
                    sx={{
                      height: 34,
                      color: "#1e293b",
                      fontSize: 13,
                      fontWeight: 700,
                      "& .MuiSelect-select": {
                        py: "7px",
                        pl: 1.25,
                        pr: "34px !important",
                      },
                    }}
                  >
                    <MenuItem value="">{translate("All Grades")}</MenuItem>
                    {grades.map((grade: any) => (
                      <MenuItem key={grade.id} value={grade.id}>
                        {grade.code ? `${grade.code} - ${grade.name}` : grade.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Box>
              <Button
                variant="contained"
                startIcon={<DownloadIcon />}
                endIcon={<ExpandMoreIcon />}
                disabled={isLoading}
                onClick={(event) => setExportMenuAnchor(event.currentTarget)}
                sx={{
                  minHeight: 36,
                  textTransform: "none",
                  fontWeight: 700,
                  borderRadius: "8px",
                  px: 1.5,
                  bgcolor: "var(--lms-primary)",
                  boxShadow: "none",
                  whiteSpace: "nowrap",
                  "&:hover": {
                    bgcolor: "var(--lms-primary)",
                    boxShadow: "none",
                  },
                }}
              >
                {translate("Export")}
              </Button>
              <Menu
                anchorEl={exportMenuAnchor}
                open={Boolean(exportMenuAnchor)}
                onClose={() => setExportMenuAnchor(null)}
                anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                transformOrigin={{ vertical: "top", horizontal: "right" }}
                slotProps={{
                  paper: {
                    sx: {
                      mt: 0.75,
                      minWidth: 190,
                      overflow: "hidden",
                      border: "1px solid rgba(var(--lms-primary-rgb), 0.24)",
                      borderRadius: "10px",
                      boxShadow: "0 12px 28px rgba(var(--lms-primary-dark-rgb), 0.16)",
                    },
                  },
                }}
              >
                <Box
                  sx={{
                    px: 1.5,
                    py: 1,
                    bgcolor: "rgba(var(--lms-primary-rgb), 0.08)",
                    borderBottom: "1px solid rgba(var(--lms-primary-rgb), 0.16)",
                  }}
                >
                  <Typography sx={{ color: "var(--lms-primary-dark)", fontSize: 12, fontWeight: 800 }}>
                    {translate("Download Report")}
                  </Typography>
                </Box>
                <MenuItem
                  onClick={() => void exportEmployees("pdf")}
                  sx={{
                    minHeight: 44,
                    gap: 1.25,
                    color: "#334155",
                    fontSize: 13,
                    fontWeight: 700,
                    "&:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.08)" },
                  }}
                >
                  <Box
                    sx={{
                      width: 28,
                      height: 28,
                      display: "grid",
                      placeItems: "center",
                      color: "var(--lms-primary)",
                      bgcolor: "rgba(var(--lms-primary-rgb), 0.12)",
                      borderRadius: "7px",
                    }}
                  >
                    <PictureAsPdfIcon sx={{ fontSize: 17 }} />
                  </Box>
                  {translate("Export as PDF")}
                </MenuItem>
                <MenuItem
                  onClick={() => void exportEmployees("excel")}
                  sx={{
                    minHeight: 44,
                    gap: 1.25,
                    color: "#334155",
                    fontSize: 13,
                    fontWeight: 700,
                    "&:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.08)" },
                  }}
                >
                  <Box
                    sx={{
                      width: 28,
                      height: 28,
                      display: "grid",
                      placeItems: "center",
                      color: "var(--lms-primary)",
                      bgcolor: "rgba(var(--lms-primary-rgb), 0.12)",
                      borderRadius: "7px",
                    }}
                  >
                    <TableViewIcon sx={{ fontSize: 17 }} />
                  </Box>
                  {translate("Export as Excel")}
                </MenuItem>
              </Menu>
            </Box>
          )}
          extraActions={(row: any) => (
            <Tooltip title={translate("Additional Information")} arrow>
              <IconButton
                size="small"
                onClick={(e) => {
                  e.stopPropagation();
                  setAdditionalInfoEmployeeId(String(row.id));
                  setViewMode("additionalInfo");
                }}
                sx={{ color: "var(--lms-primary)" }}
              >
                <PersonIcon sx={{ fontSize: "1.2rem" }} />
              </IconButton>
            </Tooltip>
          )}
        />
      )}
      <LmsToaster
        open={showToaster}
        onClose={() => setShowToaster(false)}
        messages={toasterMessages}
        severity={toasterSeverity}
        disableDeleteSuccessAutoSeverity={true}
      />
    </>
  );
};

export default EmployeeProfile;
