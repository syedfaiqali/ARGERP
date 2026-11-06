import React, { useEffect, useState } from "react";
import { Avatar, Box, IconButton, Paper, Tooltip, Typography } from "@mui/material";
import { Person as PersonIcon, Print as PrintIcon, BadgeOutlined, LayersOutlined, WorkOutline, WcOutlined, PeopleOutline, CalendarTodayOutlined, BloodtypeOutlined, PhoneOutlined, MailOutline, LocationOnOutlined, AddIcCallOutlined, ContactEmergencyOutlined } from "@mui/icons-material";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import dayjs from "dayjs";
import { GetService } from "@/constants/globalItems";

type EmployeeHeaderCardProps = {
  employeeData: any;
  onPrint?: () => void;
  refreshKey?: number;
};

const lookupItems = (response: any): any[] => {
  const data = response?.data ?? response;
  return data?.items ?? data?.Items ?? data?.data ?? data?.Data ??
    (Array.isArray(data) ? data : []);
};

export const EmployeeHeaderCard: React.FC<EmployeeHeaderCardProps> = ({
  employeeData,
  onPrint,
  refreshKey = 0,
}) => {
  const { translate } = useTranslation();
  const [emergencyContact, setEmergencyContact] = useState<any>(null);
  const employeeId = employeeData?.id ?? employeeData?.Id;
  useEffect(() => {
    setEmergencyContact(null);
    if (!employeeId) return;
    let cancelled = false;
    void GetService(`/api/HumanResource/EREmpRelation/v3?employeeId=${employeeId}&pageSize=1000`)
      .then((response: any) => {
        const contact = lookupItems(response).find((row: any) => row.emergency ?? row.Emergency);
        if (!cancelled) setEmergencyContact(contact ?? null);
      }).catch(() => { if (!cancelled) setEmergencyContact(null); });
    return () => { cancelled = true; };
  }, [employeeId, refreshKey]);
  const [headerNames, setHeaderNames] = useState({
    department: "",
    designation: "",
    positionType: "",
    status: "",
    gender: "",
    bloodGroup: "",
  });

  useEffect(() => {
    const departmentId = Number(employeeData?.departmentId ?? employeeData?.DepartmentId);
    const designationId = Number(employeeData?.designationId ?? employeeData?.DesignationId);
    const positionTypeId = Number(employeeData?.positionTypeId ?? employeeData?.PositionTypeId);
    const statusId = Number(employeeData?.statusId ?? employeeData?.StatusId);
    const genderId = Number(employeeData?.genderId ?? employeeData?.GenderId);
    const bloodGroupId = Number(employeeData?.bloodGroupId ?? employeeData?.BloodGroupId);

    let cancelled = false;
    void Promise.all([
      GetService("/api/HumanResource/Lookups/v3/departments"),
      GetService("/api/HumanResource/Lookups/v3/designations"),
      GetService("/api/HumanResource/Lov/v3/PSTYP"),
      GetService("/api/HumanResource/Lov/v3/JBSTS"),
      GetService("/api/HumanResource/Lov/v3/GEND"),
      GetService("/api/HumanResource/Lov/v3/BLOOD"),
    ]).then(([
      departmentResponse,
      designationResponse,
      positionTypeResponse,
      statusResponse,
      genderResponse,
      bloodGroupResponse,
    ]) => {
      if (cancelled) return;
      const nameFor = (response: any, id: number) => {
        const item = lookupItems(response).find(
          (row: any) => Number(row?.id ?? row?.Id) === id,
        );
        return String(item?.name ?? item?.Name ?? item?.stxt ?? item?.Stxt ?? item?.depstxt ?? "");
      };
      setHeaderNames({
        department: nameFor(departmentResponse, departmentId),
        designation: nameFor(designationResponse, designationId),
        positionType: nameFor(positionTypeResponse, positionTypeId),
        status: nameFor(statusResponse, statusId),
        gender: nameFor(genderResponse, genderId),
        bloodGroup: nameFor(bloodGroupResponse, bloodGroupId),
      });
    });

    return () => { cancelled = true; };
  }, [
    employeeData?.departmentId,
    employeeData?.designationId,
    employeeData?.positionTypeId,
    employeeData?.statusId,
    employeeData?.genderId,
    employeeData?.bloodGroupId,
  ]);

  const department = employeeData?.department ?? employeeData?.departmentName ?? headerNames.department;
  const designation = employeeData?.designation ?? employeeData?.designationName ?? headerNames.designation;
  const positionType = employeeData?.positionType ?? employeeData?.positionTypeName ?? headerNames.positionType;
  const status = employeeData?.status ?? employeeData?.statusName ?? headerNames.status;
  const gender = employeeData?.gender ?? employeeData?.genderName ?? headerNames.gender;
  const bloodGroup = employeeData?.bloodGroup ?? employeeData?.bloodGroupName ?? headerNames.bloodGroup;

  const emergencyName = emergencyContact
    ? `${emergencyContact.firstName ?? emergencyContact.FirstName ?? ""} ${emergencyContact.lastName ?? emergencyContact.LastName ?? ""}`.trim() : "";
  const emergencyNumber = emergencyContact?.mobileNo || emergencyContact?.MobileNo || emergencyContact?.phoneNo || emergencyContact?.PhoneNo || employeeData?.emergencyNo;
  const unknownBloodGroup = !bloodGroup || String(bloodGroup).toLowerCase() === "unknown";
  const formatDate = (value: string | undefined) => value && dayjs(value).isValid() ? dayjs(value).format("DD MMM YYYY") : "-";
  const details = [
    { label: "Employee ID", value: employeeData?.employeeCode, icon: BadgeOutlined },
    { label: "Department", value: department, icon: LayersOutlined },
    { label: "Designation", value: designation, icon: WorkOutline },
    { label: "Position Type", value: positionType, icon: LayersOutlined },
    { label: "Gender", value: gender, icon: WcOutlined },
    { label: "Father Name", value: employeeData?.parentName, icon: PeopleOutline },
    { label: "Joining Date", value: formatDate(employeeData?.joiningDate), icon: CalendarTodayOutlined },
    { label: "Date of Birth", value: formatDate(employeeData?.dateOfBirth), icon: CalendarTodayOutlined },
    { label: "Blood Group", value: bloodGroup || translate("Unknown"), icon: BloodtypeOutlined, warning: unknownBloodGroup },
    { label: "Contact No.", value: employeeData?.cellNo, icon: PhoneOutlined },
    { label: "Email", value: employeeData?.emailId, icon: MailOutline },
    { label: "Address", value: employeeData?.address, icon: LocationOnOutlined },
    { label: "Emergency Contact No.", value: emergencyNumber, icon: AddIcCallOutlined },
    { label: "Emergency Contact Name", value: emergencyName, icon: ContactEmergencyOutlined, wide: true },
  ];
  return (
    <Paper elevation={0} sx={{
      mb: 1.5, p: { xs: 1.5, md: 1.75, xl: 2 }, position: "relative", overflow: "hidden",
      border: "1px solid var(--lms-border)", borderTop: "3px solid var(--lms-primary)", borderRadius: "18px",
      background: "linear-gradient(115deg, var(--lms-primary-light) -100%, #fff 40%, #fff 75%, var(--lms-primary-light) 180%)",
      boxShadow: "0 8px 24px rgba(34,57,24,0.08), 0 2px 5px rgba(34,57,24,0.05)",
      "&::before": { content: '""', position: "absolute", width: 520, height: 160, right: -100, top: -95, borderRadius: "50%", transform: "rotate(-12deg)", background: "linear-gradient(120deg, transparent, var(--lms-primary-light))", opacity: 0.7, pointerEvents: "none" },
      "@media print": { boxShadow: "none", breakInside: "avoid" },
    }}>
      <Box sx={{ display: "flex", flexDirection: { xs: "column", md: "row" }, gap: { xs: 1.25, md: 2 }, position: "relative" }}>
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "center", pr: { md: 2 }, borderRight: { md: "1px solid var(--lms-border)" }, flexShrink: 0 }}>
          <Box sx={{ p: "4px", borderRadius: "50%", border: "1px solid var(--lms-border)", bgcolor: "#fff", boxShadow: "0 6px 15px rgba(60,85,35,0.14)" }}>
            <Avatar src={employeeData?.imageId ? `/api/HumanResource/v3/image/${employeeData.imageId}` : undefined} sx={{ width: { xs: 64, md: 80, xl: 90 }, height: { xs: 64, md: 80, xl: 90 }, bgcolor: "#fff", border: "3px solid var(--lms-primary-light)" }}>
              <PersonIcon sx={{ fontSize: { xs: 44, md: 56, xl: 62 }, color: "var(--lms-primary)" }} />
            </Avatar>
          </Box>
        </Box>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Box sx={{ display: "flex", alignItems: "flex-start", gap: 1.25, mb: 1 }}>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <Typography component="span" sx={{ display: "inline-block", px: 0.9, py: 0.15, mb: 0.25, borderRadius: "6px", bgcolor: "var(--lms-primary-light)", color: "var(--lms-primary)", fontSize: 10.5, fontWeight: 800 }}># {employeeData?.machineCode || "N/A"}</Typography>
              <Box sx={{ display: "flex", alignItems: "center", flexWrap: "wrap", gap: 1 }}>
                <Typography component="h2" sx={{ fontSize: { xs: 18, lg: 21, xl: 23 }, fontWeight: 900, lineHeight: 1.25, letterSpacing: "-0.035em", color: "var(--lms-text-main)", overflowWrap: "anywhere" }}>{`${employeeData?.firstName || ""} ${employeeData?.lastName || ""}`.trim().toUpperCase() || "-"}</Typography>
                {status && <Typography component="span" sx={{ px: 1.25, py: 0.2, borderRadius: "20px", background: "linear-gradient(180deg, var(--lms-primary), var(--lms-primary-dark, var(--lms-primary)))", color: "#fff", fontSize: 10.5, fontWeight: 700 }}>{status}</Typography>}
              </Box>
            </Box>
            {onPrint && <Tooltip title={translate("Print employee profile")}><IconButton aria-label={translate("Print employee profile")} onClick={onPrint} sx={{ color: "var(--lms-primary)", bgcolor: "rgba(255,255,255,0.9)", border: "1px solid var(--lms-border)", borderRadius: "10px", width: 32, height: 32, boxShadow: "0 3px 8px rgba(34,57,24,0.08)", "&:hover": { bgcolor: "var(--lms-primary-light)" }, "@media print": { display: "none" } }}><PrintIcon fontSize="small" /></IconButton></Tooltip>}
          </Box>
          <Box sx={{ display: "grid", gridTemplateColumns: { xs: "minmax(0,1fr)", sm: "repeat(2,minmax(0,1fr))", md: "repeat(3,minmax(0,1fr))", lg: "repeat(5,minmax(0,1fr))" }, "& > *": { borderBottom: "1px solid var(--lms-border)" }, "@media (min-width: 1200px)": { "& > :nth-of-type(n+11)": { borderBottom: 0 } } }}>
            {details.map(({ label, value, icon: Icon, warning, wide }) => (
              <Box key={label} sx={{ display: "flex", alignItems: "center", gap: 1, minWidth: 0, py: { xs: 0.65, xl: 0.85 }, pr: 1, gridColumn: wide ? { xs: "auto", lg: "span 2" } : "auto" }}>
                <Icon aria-hidden="true" sx={{ flexShrink: 0, fontSize: { xs: 18, xl: 20 }, color: warning ? "#dc3545" : "var(--lms-primary)" }} />
                <Box sx={{ minWidth: 0, flex: 1, pl: 0.85, borderLeft: "1px solid var(--lms-border)" }}>
                  <Typography sx={{ fontSize: { xs: 10.5, xl: 11 }, fontWeight: 600, color: "var(--lms-text-secondary)", lineHeight: 1.4 }}><T>{label}</T></Typography>
                  <Typography component="div" sx={{ fontSize: { xs: 11.5, xl: 12 }, fontWeight: 800, color: "var(--lms-text-main)", lineHeight: 1.5, overflowWrap: "anywhere" }}>{warning ? <Box component="span" sx={{ px: 1, borderRadius: "12px", bgcolor: "#fee2e2", color: "#b91c1c", display: "inline-block" }}>{value}</Box> : value || "-"}</Typography>
                </Box>
              </Box>
            ))}
          </Box>
        </Box>
      </Box>
    </Paper>
  );
};