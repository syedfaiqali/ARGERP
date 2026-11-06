import React, { useCallback, useEffect, useMemo, useState } from "react";
import dayjs from "dayjs";
import {
  Box,
  Dialog,
  DialogContent,
  DialogTitle,
  IconButton,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  Close as CloseIcon,
  Visibility as DetailsIcon,
} from "@mui/icons-material";
import { SchemaTable } from "@/components/SchemaTable";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { LmsToaster } from "@/components/LmsToaster";
import { useTranslation } from "@/components/AdaptiveTranslationProvider";
import { GetService } from "@/constants/globalItems";

interface EmployeeAttendanceTabProps {
  employeeId: string | undefined;
}

interface DailyAttendanceRow {
  id: number | string;
  atnDate: string;
  day: string;
  checkIn: string;
  checkOut: string;
  isAbsent: boolean;
  isHoliday: boolean;
  lateCount: number;
  remarks: string;
}

interface MonthlyAttendanceRow {
  id: string;
  monthYear: string;
  sortDate: string;
  workingDays: number;
  presentDays: number;
  absentDays: number;
  holidays: number;
  lateCount: number;
  details: DailyAttendanceRow[];
}

interface AttendanceStats {
  total: number;
  present: number;
  absent: number;
  late: number;
  percent: number;
}

const API = "/api/HumanResource/EmployeeDailyAttendance/v3";
const CURRENT_YEAR_START = dayjs().startOf("year").format("YYYY-MM-DD");
const CURRENT_YEAR_END = dayjs().endOf("year").format("YYYY-MM-DD");

const itemsFrom = (payload: any): any[] =>
  payload?.items ??
  payload?.Items ??
  payload?.data?.items ??
  payload?.data?.Items ??
  payload?.data ??
  payload?.Data ??
  (Array.isArray(payload) ? payload : []);

const formatTime = (value: any) =>
  value && dayjs(value).isValid() ? dayjs(value).format("hh:mm A") : "-";

const normalizeDailyRow = (item: any, index: number): DailyAttendanceRow => ({
  id: item.id ?? item.Id ?? index + 1,
  atnDate: item.atnDate ?? item.AtnDate ?? item.attDate ?? "",
  day: item.day ?? item.Day ?? "",
  checkIn: item.actShiftIn ?? item.ActShiftIn ?? item.checkIn ?? "",
  checkOut: item.actShiftOut ?? item.ActShiftOut ?? item.actShitOut ?? "",
  isAbsent: Boolean(item.isAbsent ?? item.IsAbsent ?? false),
  isHoliday: Boolean(item.isHoliday ?? item.IsHoliday ?? false),
  lateCount: Number(item.lateCount ?? item.LateCount ?? 0),
  remarks: item.remarks ?? item.Remarks ?? "",
});

const buildStats = (rows: DailyAttendanceRow[]): AttendanceStats => {
  const attendanceRows = rows.filter((row) => !row.isHoliday);
  const present = attendanceRows.filter((row) => !!row.checkIn || !!row.checkOut).length;
  const absent = attendanceRows.filter(
    (row) => row.isAbsent || (!row.checkIn && !row.checkOut),
  ).length;
  const late = attendanceRows.reduce((sum, row) => sum + row.lateCount, 0);
  const percent = attendanceRows.length
    ? Math.round((present / attendanceRows.length) * 100)
    : 0;

  return {
    total: attendanceRows.length,
    present,
    absent,
    late,
    percent,
  };
};

export const EmployeeAttendanceTab: React.FC<EmployeeAttendanceTabProps> = ({
  employeeId,
}) => {
  const { translate } = useTranslation();
  const [dailyRows, setDailyRows] = useState<DailyAttendanceRow[]>([]);
  const [selectedMonth, setSelectedMonth] = useState<MonthlyAttendanceRow | null>(
    null,
  );
  const [loading, setLoading] = useState(false);
  const [toaster, setToaster] = useState<{
    open: boolean;
    messages: string[];
    severity: "error" | "success" | "info" | "warning";
  }>({ open: false, messages: [], severity: "info" });

  const fetchAttendance = useCallback(async () => {
    if (!employeeId) return;

    try {
      setLoading(true);
      const response: any = await GetService(
        `${API}?employeeId=${employeeId}&fromDate=${CURRENT_YEAR_START}&tillDate=${CURRENT_YEAR_END}`,
      );
      setDailyRows(itemsFrom(response).map(normalizeDailyRow));
    } catch {
      setDailyRows([]);
      setToaster({
        open: true,
        messages: [translate("Failed to load attendance")],
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  }, [employeeId, translate]);

  useEffect(() => {
    void fetchAttendance();
  }, [fetchAttendance]);

  const monthlyRows = useMemo(() => {
    const groups = new Map<string, DailyAttendanceRow[]>();

    dailyRows.forEach((row) => {
      if (!row.atnDate || !dayjs(row.atnDate).isValid()) return;
      const key = dayjs(row.atnDate).format("YYYY-MM");
      groups.set(key, [...(groups.get(key) ?? []), row]);
    });

    return Array.from(groups.entries())
      .map(([key, rows]) => {
        const orderedDetails = rows
          .slice()
          .sort((a, b) => dayjs(a.atnDate).valueOf() - dayjs(b.atnDate).valueOf());

        return {
          id: key,
          monthYear: dayjs(`${key}-01`).format("MMMM YYYY"),
          sortDate: `${key}-01`,
          workingDays: rows.filter((row) => !row.isHoliday).length,
          presentDays: rows.filter((row) => !!row.checkIn || !!row.checkOut).length,
          absentDays: rows.filter(
            (row) => row.isAbsent || (!row.isHoliday && !row.checkIn && !row.checkOut),
          ).length,
          holidays: rows.filter((row) => row.isHoliday).length,
          lateCount: rows.reduce((sum, row) => sum + row.lateCount, 0),
          details: orderedDetails,
        };
      })
      .sort((a, b) => dayjs(b.sortDate).valueOf() - dayjs(a.sortDate).valueOf());
  }, [dailyRows]);

  const attendanceStats = useMemo(() => {
    const currentYear = dayjs().year();
    const currentYearRows = dailyRows.filter(
      (row) => row.atnDate && dayjs(row.atnDate).isValid() && dayjs(row.atnDate).year() === currentYear,
    );

    return {
      overall: buildStats(dailyRows),
      currentYear: buildStats(currentYearRows),
    };
  }, [dailyRows]);

  const columns = useMemo(
    () => [
      {
        id: "sno",
        label: translate("S.No"),
        minWidth: 70,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      { id: "monthYear", label: translate("Month Year"), minWidth: 180 },
      { id: "workingDays", label: translate("Working Days"), minWidth: 130 },
      { id: "presentDays", label: translate("Present"), minWidth: 110 },
      { id: "absentDays", label: translate("Absent"), minWidth: 110 },
      { id: "holidays", label: translate("Holidays"), minWidth: 110 },
      {
        id: "details",
        label: translate("Details"),
        minWidth: 90,
        align: "center" as const,
        format: (_: any, row?: MonthlyAttendanceRow) =>
          row ? (
            <Tooltip title={translate("View Details")}>
              <IconButton
                size="small"
                onClick={(event) => {
                  event.stopPropagation();
                  setSelectedMonth(row);
                }}
                sx={{ color: "var(--lms-primary)" }}
              >
                <DetailsIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          ) : null,
      },
    ],
    [translate],
  );

  const detailColumns = useMemo(
    () => [
      {
        id: "sno",
        label: translate("S.No"),
        minWidth: 70,
        format: (_: any, __: any, index?: number) => (index ?? 0) + 1,
      },
      {
        id: "atnDate",
        label: translate("Date"),
        minWidth: 140,
        format: (value: any) =>
          value && dayjs(value).isValid() ? dayjs(value).format("DD MMM YYYY") : "-",
      },
      { id: "day", label: translate("Day"), minWidth: 120 },
      {
        id: "checkIn",
        label: translate("Check In"),
        minWidth: 120,
        format: formatTime,
      },
      {
        id: "checkOut",
        label: translate("Check Out"),
        minWidth: 120,
        format: formatTime,
      },
    ],
    [translate],
  );

  const renderGauge = (label: string, value: number) => {
    const normalizedValue = Math.max(0, Math.min(100, value));

    return (
      <Box
        sx={{
          flex: "1 1 260px",
          minHeight: 124,
          px: 2.25,
          py: 1.75,
          borderRight: { xs: 0, md: "1px solid rgba(17, 24, 39, 0.12)" },
        }}
      >
        <Typography
          sx={{
            fontSize: "0.95rem",
            fontWeight: 900,
            color: "var(--lms-primary-dark)",
            mb: 1.25,
          }}
        >
          {translate(label)}
        </Typography>
        <Box
          sx={{
            width: 136,
            height: 70,
            mx: "auto",
            position: "relative",
            overflow: "hidden",
          }}
        >
          <Box
            sx={{
              width: 136,
              height: 136,
              borderRadius: "50%",
              background: `conic-gradient(from 270deg, var(--lms-primary) 0deg, var(--lms-primary) ${normalizedValue * 1.8}deg, #e8eaee ${normalizedValue * 1.8}deg, #e8eaee 180deg, transparent 180deg, transparent 360deg)`,
              boxShadow: "inset 0 5px 12px rgba(15, 23, 42, 0.14)",
              position: "absolute",
              left: 0,
              top: 0,
              "&::after": {
                content: '""',
                position: "absolute",
                width: 82,
                height: 82,
                borderRadius: "50%",
                bgcolor: "#fff",
                left: 27,
                top: 27,
                boxShadow: "0 -2px 8px rgba(15, 23, 42, 0.08)",
              },
            }}
          />
          <Typography
            sx={{
              position: "absolute",
              left: 0,
              right: 0,
              bottom: 0,
              textAlign: "center",
              fontSize: "1.05rem",
              fontWeight: 900,
              color: "var(--lms-primary)",
            }}
          >
            {normalizedValue}
          </Typography>
        </Box>
      </Box>
    );
  };

  return (
    <>
      {loading && <LmsOverlayLoader />}
      <Box
        sx={{
          mb: 2,
          border: "1px solid rgba(17, 24, 39, 0.12)",
          borderRadius: 2,
          bgcolor: "#fff",
          boxShadow: "0 10px 24px rgba(15, 23, 42, 0.06)",
          display: "flex",
          flexWrap: "wrap",
          overflow: "hidden",
        }}
      >
        {renderGauge("Over All Attendance (%)", attendanceStats.overall.percent)}
        {renderGauge("Current Year Attendance (%)", attendanceStats.currentYear.percent)}
        <Box
          sx={{
            flex: "1.2 1 300px",
            minHeight: 124,
            px: 2.25,
            py: 1.75,
          }}
        >
          <Typography
            sx={{
              fontSize: "0.95rem",
              fontWeight: 900,
              color: "var(--lms-primary-dark)",
              mb: 1.25,
            }}
          >
            {translate("Current Year Summary")}
          </Typography>
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
              gap: 1.25,
              alignItems: "start",
            }}
          >
            <Box sx={{ borderRight: { xs: 0, sm: "1px solid rgba(17, 24, 39, 0.12)" } }}>
              <Typography sx={{ fontSize: "0.85rem", fontWeight: 800, color: "#111827" }}>
                {translate("Out Of")} : {attendanceStats.currentYear.total}
              </Typography>
              <Typography sx={{ fontSize: "0.85rem", fontWeight: 900, color: "#1c8a27", mt: 0.75 }}>
                {translate("Present")} : {attendanceStats.currentYear.present}
              </Typography>
            </Box>
            <Box>
              <Typography sx={{ fontSize: "0.85rem", fontWeight: 900, color: "#f5a400" }}>
                {translate("Late")} : {attendanceStats.currentYear.late}
              </Typography>
              <Typography sx={{ fontSize: "0.85rem", fontWeight: 900, color: "#e02828", mt: 0.75 }}>
                {translate("Absent")} : {attendanceStats.currentYear.absent}
              </Typography>
            </Box>
          </Box>
        </Box>
      </Box>
      <SchemaTable
        title={translate("Attendance")}
        columns={columns}
        rows={monthlyRows}
        hideTitle
        stickyHeader
        pagination={false}
        filtersEnabled={false}
        actions={false}
        showAddButton={false}
        showToaster={false}
      />

      <Dialog
        open={Boolean(selectedMonth)}
        onClose={() => setSelectedMonth(null)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            fontWeight: 900,
          }}
        >
          <Typography sx={{ fontWeight: 900 }}>
            {selectedMonth
              ? `${translate("Attendance Details")} - ${selectedMonth.monthYear}`
              : translate("Attendance Details")}
          </Typography>
          <IconButton size="small" onClick={() => setSelectedMonth(null)}>
            <CloseIcon fontSize="small" />
          </IconButton>
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pb: 1 }}>
            <SchemaTable
              title={translate("Attendance Details")}
              columns={detailColumns}
              rows={selectedMonth?.details ?? []}
              hideTitle
              stickyHeader
              pagination={false}
              filtersEnabled={false}
              actions={false}
              showAddButton={false}
              showToaster={false}
            />
          </Box>
        </DialogContent>
      </Dialog>

      <LmsToaster
        open={toaster.open}
        messages={toaster.messages}
        severity={toaster.severity}
        onClose={() => setToaster((prev) => ({ ...prev, open: false }))}
      />
    </>
  );
};
