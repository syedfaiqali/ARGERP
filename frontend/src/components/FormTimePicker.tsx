import React, { useState, useEffect, useRef } from "react";
import { LocalizationProvider, TimePicker } from "@mui/x-date-pickers";
import { usePickerContext } from "@mui/x-date-pickers/hooks";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useDayjsAdapterLocale } from "@/utils/localeFormat";
import { renderTimeViewClock } from "@mui/x-date-pickers/timeViewRenderers";
import { alpha, useTheme } from "@mui/material/styles";
import { Box, Typography } from "@mui/material";
import dayjs, { Dayjs } from "dayjs";

interface FormTimePickerProps {
  label?: string;
  value: Dayjs | null;
  onChange: (newValue: Dayjs | null) => void;
  required?: boolean;
  hideLabel?: boolean;
  disabled?: boolean;
  error?: boolean;
  helperText?: string;
  defaultTime?: Dayjs;
}

/**
 * A Custom Toolbar component for the TimePicker to ensure 100% control over AM/PM design.
 */
const CustomTimePickerToolbar = (props: { ampm?: boolean; displayValue?: Dayjs | null }) => {
  const { ampm, displayValue } = props;
  const { value, view, setView } = usePickerContext<Dayjs | null>();
  const accentColor = useTheme().palette.primary.main;
  
  if (!ampm) return null;

  const fallbackValue = displayValue ?? value ?? dayjs().hour(12).minute(0).second(0);
  const hasValue =
    (displayValue !== null && displayValue !== undefined) ||
    (value !== null && value !== undefined);
  // Use a slightly dimmer green if no value is present (browsing state)
  const displayColor = hasValue ? accentColor : alpha(accentColor, 0.4);

  const isHoursView = view === "hours";
  const isMinutesView = view === "minutes";

  return (
    <Box sx={{ 
      display: "flex", 
      flexDirection: "column", 
      alignItems: "center", 
      gap: 1.5,
      pt: 3,
      pb: 2,
      borderBottom: "1px solid rgba(0,0,0,0.06)",
      bgcolor: "rgba(var(--lms-primary-rgb), 0.01)"
    }}>
      {/* TIME DIGITS DISPLAY */}
      <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
        <Typography
          onClick={() => setView?.("hours")}
          sx={{
            fontSize: "2.8rem",
            fontWeight: 800,
            color: displayColor,
            cursor: "pointer",
            textDecoration: isHoursView ? "underline" : "none",
          }}
        >
          {fallbackValue.format("hh")}
        </Typography>
        <Typography sx={{ fontSize: "2.40rem", fontWeight: 800, color: displayColor, opacity: 0.3 }}>
          :
        </Typography>
        <Typography
          onClick={() => setView?.("minutes")}
          sx={{
            fontSize: "2.8rem",
            fontWeight: 800,
            color: displayColor,
            cursor: "pointer",
            textDecoration: isMinutesView ? "underline" : "none",
          }}
        >
          {fallbackValue.format("mm")}
        </Typography>
      </Box>

      {/* CUSTOM AM/PM SELECTION ROW */}
      {/* <Box sx={{ display: "flex", flexDirection: "row", gap: 2, justifyContent: "center" }}>
        <Button
          size="small"
          onClick={() => handleAmPmChange(false)}
          sx={{
            minWidth: 68, height: 36, borderRadius: "8px", fontWeight: 700, fontSize: "0.9rem",
            textTransform: "none",
            backgroundColor: !isPM ? `${accentColor} !important` : "#fff !important",
            color: !isPM ? "#fff !important" : `${accentColor} !important`,
            border: `1px solid ${!isPM ? accentColor : alpha(accentColor, 0.2)} !important`,
            boxShadow: !isPM ? `0 4px 10px ${alpha(accentColor, 0.2)}` : "none",
            "&:hover": { backgroundColor: !isPM ? accentColor : "rgba(var(--lms-primary-rgb), 0.05) !important" }
          }}
        >
          AM
        </Button>
        <Button
          size="small"
          onClick={() => handleAmPmChange(true)}
          sx={{
            minWidth: 68, height: 36, borderRadius: "8px", fontWeight: 700, fontSize: "0.9rem",
            textTransform: "none",
            backgroundColor: isPM ? `${accentColor} !important` : "#fff !important",
            color: isPM ? "#fff !important" : `${accentColor} !important`,
            border: `1px solid ${isPM ? accentColor : alpha(accentColor, 0.2)} !important`,
            boxShadow: isPM ? `0 4px 10px ${alpha(accentColor, 0.2)}` : "none",
            "&:hover": { backgroundColor: isPM ? accentColor : "rgba(var(--lms-primary-rgb), 0.05) !important" }
          }}
        >
          PM
        </Button>
      </Box> */}
    </Box>
  );
};

export const FormTimePicker: React.FC<FormTimePickerProps> = (props) => {
  const {
    label = "",
    value, 
    onChange: onPersistChange, 
    required = false,
    hideLabel = false,
    disabled = false,
    error = false,
    helperText = "",
    defaultTime,
  } = props;

  const accentColor = useTheme().palette.primary.main;
  const adapterLocale = useDayjsAdapterLocale();
  const defaultReferenceTime = defaultTime ?? dayjs().hour(12).minute(0).second(0);

  const [open, setOpen] = useState(false);
  const [browsingValue, setBrowsingValue] = useState<Dayjs | null>(value);
  const originalValueAtOpen = useRef<Dayjs | null>(value);

  useEffect(() => {
    if (!open) setBrowsingValue(value);
  }, [value, open]);

  const handleOpen = () => {
    originalValueAtOpen.current = value;
    setBrowsingValue(value); 
    setOpen(true);
  };

  const commitToParent = (_val: Dayjs | null) => {
    // Only persist an explicit selection made in this dialog.  MUI can expose
    // its reference time through onAccept even when the original value is null.
    onPersistChange(browsingValue);
    setOpen(false);
  };

  const abortChangesAndClose = () => {
    setBrowsingValue(originalValueAtOpen.current);
    setOpen(false);
  };

  const handleBrowsingChange = (newValue: Dayjs | null) => {
    if (newValue === null) return;
    setBrowsingValue(newValue);
  };

  // Keep an empty field genuinely empty while the dialog is open.  The
  // reference date controls the clock's initial position without becoming a
  // value that is committed when the user merely opens and closes the picker.
  const pickerDisplayValue = browsingValue;

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale={adapterLocale}>
      <TimePicker
        ampm
        format="hh:mm A"
        referenceDate={defaultReferenceTime}
        closeOnSelect={false}
        label={
          hideLabel
            ? undefined
            : `${typeof label === "string" ? label.replace(/\*+[\s]*$/, "") : label}${required ? "*" : ""}`
        }
        value={pickerDisplayValue}
        onChange={handleBrowsingChange}
        onAccept={commitToParent} 
        onClose={abortChangesAndClose} 
        disabled={disabled}
        open={open}
        onOpen={handleOpen}
        viewRenderers={{
          hours: renderTimeViewClock,
          minutes: renderTimeViewClock,
          seconds: renderTimeViewClock,
        }}
        slots={{
          // OVERRIDE ENTIRE TOOLBAR: We now use our custom component to guarantee AM/PM works
          toolbar: CustomTimePickerToolbar,
        }}
        slotProps={{
          toolbar: ({ displayValue: pickerDisplayValue } as any),
          textField: {
            size: "small",
            fullWidth: true,
            variant: "outlined",
            inputProps: { value: value ? value.format("hh:mm A") : "" }, 
            InputLabelProps: { 
              sx: { fontWeight: 600, fontSize: "0.85rem", "&.Mui-focused": { color: accentColor } } 
            },
            error: error,
            helperText: helperText,
            onClick: () => !disabled && handleOpen(),
            onMouseDown: (e) => e.preventDefault(),
            InputProps: { readOnly: true },
            sx: {
              "& .MuiOutlinedInput-root": {
                borderRadius: "10px",
                border: "1px solid var(--lms-border)",
                "& fieldset": { border: "none" },
                "& input": {
                  cursor: disabled ? "default" : "pointer !important",
                  fontWeight: 700, fontSize: "0.9rem",
                  color: (value !== null) ? accentColor : "rgba(0,0,0,0.5)",
                },
                "&:hover": { borderColor: accentColor },
                "&.Mui-focused": { border: `1px solid ${accentColor}` },
              },
            },
          },
          mobilePaper: { sx: { borderRadius: "12px", boxShadow: "0 15px 400px rgba(0,0,0,0.12)", maxWidth: "280px" } },
          desktopPaper: { sx: { borderRadius: "12px", boxShadow: "0 15px 40px rgba(0,0,0,0.12)", maxWidth: "280px" } },
          layout: {
            sx: {
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              "& .MuiPickersLayout-toolbar": {
                width: "100%",
                justifyContent: "center",
              },
              "& .MuiPickersLayout-contentWrapper": {
                width: "100%",
                display: "flex",
                justifyContent: "center",
              },
              "& .MuiTimeClock-root": {
                width: 240,
                height: 240,
                margin: "4px 0 0 0",
                padding: 0,
                alignSelf: "center",
              },
              "& .MuiPickersArrowSwitcher-root": { display: "none !important" },
              "& .MuiClock-pin": { backgroundColor: accentColor },
              "& .MuiClockPointer-root": { backgroundColor: accentColor },
              "& .MuiClockPointer-thumb": { backgroundColor: accentColor, borderColor: accentColor },
              "& .MuiClockNumber-root": {
                fontSize: "0.95rem", fontWeight: 600,
                "&.Mui-selected": { backgroundColor: accentColor, color: "#fff" },
              },
              "& .MuiPickersActionBar-root": {
                padding: "10px 20px", borderTop: "1px solid rgba(0,0,0,0.05)",
                display: "flex", justifyContent: "flex-end", gap: 1.5,
                "& .MuiButton-root": { textTransform: "none", fontWeight: 700, borderRadius: "8px" },
                "& .MuiButton-root:first-of-type": { color: "var(--lms-text-secondary)" },
                "& .MuiButton-root:last-of-type": { 
                   color: "#fff", backgroundColor: accentColor, padding: "8px 20px",
                   "&:hover": { backgroundColor: "var(--lms-primary-dark)" }
                },
              },
            },
          },
        }}
      />
    </LocalizationProvider>
  );
};
