import React from "react";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useDayjsAdapterLocale } from "@/utils/localeFormat";
import { Dayjs } from "dayjs";
import dayjs from "dayjs";
import {
  Box,
  ClickAwayListener,
  IconButton,
  Paper,
} from "@mui/material";
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
} from "@mui/icons-material";
import type { PickersCalendarHeaderProps } from "@mui/x-date-pickers/PickersCalendarHeader";
import {
  DATE_FORMAT_CHANGED_EVENT,
  getStoredDateFormat,
  loadPolicyDateFormat,
  toDayjsDateFormat,
} from "@/utils/systemPolicyDateFormat";

interface FormDatePickerProps {
  label?: React.ReactNode;
  value?: Dayjs | null;
  onChange?: (value: Dayjs | null) => void;
  required?: boolean;
  format?: string;
  variant?: "outlined" | "filled" | "standard";
  hideLabel?: boolean;
  disabled?: boolean;
  slotProps?: any;
  error?: boolean;
  helperText?: string;
  sx?: any;
  minDate?: Dayjs;
  maxDate?: Dayjs;
  enableAccessibleFieldDOMStructure?: boolean;
}

interface HeaderDropdownOption {
  value: number;
  label: string;
}

interface HeaderDropdownProps {
  value: number;
  options: HeaderDropdownOption[];
  minWidth: number;
  disabled?: boolean;
  onChange: (value: number) => void;
}

const HeaderDropdown: React.FC<HeaderDropdownProps> = ({
  value,
  options,
  minWidth,
  disabled = false,
  onChange,
}) => {
  const [open, setOpen] = React.useState(false);
  const [scrollTop, setScrollTop] = React.useState(0);
  const selected = options.find((opt) => opt.value === value);
  const optionsContainerRef = React.useRef<HTMLDivElement | null>(null);
  const shouldVirtualize = options.length > 120;
  const optionHeight = 34;
  const visibleCount = 10;
  const overscan = 4;
  const firstVisibleIndex = shouldVirtualize
    ? Math.max(0, Math.floor(scrollTop / optionHeight) - overscan)
    : 0;
  const lastVisibleIndex = shouldVirtualize
    ? Math.min(options.length, firstVisibleIndex + visibleCount + overscan * 2)
    : options.length;
  const visibleOptions = options.slice(firstVisibleIndex, lastVisibleIndex);

  React.useEffect(() => {
    if (!open || !optionsContainerRef.current) {
      return;
    }

    const selectedIndex = options.findIndex((opt) => opt.value === value);
    if (selectedIndex < 0) return;

    const nextScrollTop = selectedIndex * optionHeight;
    optionsContainerRef.current.scrollTop = nextScrollTop;
    setScrollTop(nextScrollTop);
  }, [open, value, options]);

  return (
    <ClickAwayListener onClickAway={() => setOpen(false)}>
      <Box sx={{ position: "relative" }}>
        <Box
          component="button"
          type="button"
          disabled={disabled}
          onClick={() => setOpen((prev) => !prev)}
          sx={{
            minWidth,
            height: 32,
            borderRadius: "8px",
            border: "1px solid #d9d9d9",
            backgroundColor: "#fff",
            px: 1,
            fontSize: "0.95rem",
            fontWeight: 700,
            color: "var(--lms-text-main)",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            gap: 0.75,
            cursor: disabled ? "not-allowed" : "pointer",
            "&:focus-visible": {
              outline: "none",
              borderColor: "var(--lms-primary)",
              boxShadow: "0 0 0 2px rgba(var(--lms-primary-rgb), 0.2)",
            },
          }}
        >
          <Box component="span" sx={{ whiteSpace: "nowrap" }}>
            {selected?.label ?? ""}
          </Box>
          <Box component="span" sx={{ fontSize: "0.75rem", opacity: 0.8 }}>
            ▼
          </Box>
        </Box>

        {open && !disabled && (
          <Paper
            elevation={6}
            ref={optionsContainerRef}
            onScroll={(event) => setScrollTop(event.currentTarget.scrollTop)}
            sx={{
              position: "absolute",
              top: "calc(100% + 4px)",
              left: 0,
              minWidth,
              maxHeight: 240,
              overflowY: "auto",
              zIndex: 1700,
              borderRadius: 1,
              border: "1px solid #d9d9d9",
            }}
          >
            {shouldVirtualize && (
              <Box sx={{ height: firstVisibleIndex * optionHeight }} />
            )}
            {visibleOptions.map((option) => {
              const isSelected = option.value === value;
              return (
                <Box
                  key={option.value}
                  component="button"
                  type="button"
                  data-option-value={option.value}
                  onClick={() => {
                    onChange(option.value);
                    setOpen(false);
                  }}
                  sx={{
                    width: "100%",
                    height: optionHeight,
                    textAlign: "left",
                    border: "none",
                    background: isSelected ? "var(--lms-primary)" : "#fff",
                    color: isSelected ? "#fff" : "var(--lms-text-main)",
                    px: 1.25,
                    py: 0.75,
                    fontSize: "0.95rem",
                    cursor: "pointer",
                    "&:hover": {
                      background: isSelected ? "var(--lms-primary-dark, var(--lms-primary))" : "rgba(var(--lms-primary-rgb), 0.12)",
                    },
                  }}
                >
                  {option.label}
                </Box>
              );
            })}
            {shouldVirtualize && (
              <Box sx={{ height: (options.length - lastVisibleIndex) * optionHeight }} />
            )}
          </Paper>
        )}
      </Box>
    </ClickAwayListener>
  );
};

const CustomCalendarHeader: React.FC<PickersCalendarHeaderProps & {
  minYear: number;
  maxYear: number;
}> = ({
  currentMonth,
  onMonthChange,
  disabled,
  minYear,
  maxYear,
}) => {
  const monthValue = dayjs(currentMonth as Dayjs);
  const selectedMonth = monthValue.month();
  const selectedYear = monthValue.year();

  const years: number[] = [];
  for (let year = minYear; year <= maxYear; year += 1) {
    years.push(year);
  }

  const handlePreviousMonth = () => {
    onMonthChange(monthValue.subtract(1, "month") as any);
  };

  const handleNextMonth = () => {
    onMonthChange(monthValue.add(1, "month") as any);
  };

  const monthOptions: HeaderDropdownOption[] = Array.from({ length: 12 }).map(
    (_, month) => ({
      value: month,
      // monthValue carries the picker adapter locale, so names follow the UI language.
      label: monthValue.startOf("month").month(month).format("MMMM"),
    }),
  );

  const yearOptions: HeaderDropdownOption[] = years.map((year) => ({
    value: year,
    label: String(year),
  }));

  return (
    <Box
      sx={{
        px: 1.5,
        pt: 1,
        pb: 0.5,
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        gap: 1,
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <HeaderDropdown
          value={selectedMonth}
          onChange={(month) => onMonthChange(monthValue.month(month) as any)}
          disabled={disabled}
          options={monthOptions}
          minWidth={122}
        />
        <HeaderDropdown
          value={selectedYear}
          onChange={(year) => onMonthChange(monthValue.year(year) as any)}
          disabled={disabled}
          options={yearOptions}
          minWidth={92}
        />
      </Box>

      <Box sx={{ display: "flex", alignItems: "center" }}>
        <IconButton size="small" onClick={handlePreviousMonth} disabled={disabled}>
          <ChevronLeftIcon fontSize="small" />
        </IconButton>
        <IconButton size="small" onClick={handleNextMonth} disabled={disabled}>
          <ChevronRightIcon fontSize="small" />
        </IconButton>
      </Box>
    </Box>
  );
};

export const FormDatePicker: React.FC<FormDatePickerProps> = ({
  label = "",
  value,
  onChange,
  required = false,
  format,
  variant = "outlined",
  hideLabel = false,
  disabled = false,
  slotProps = {},
  error,
  helperText,
  sx,
  minDate,
  maxDate,
  enableAccessibleFieldDOMStructure = true,
}) => {
  const resolvedMinDate = minDate ?? dayjs("1900-01-01");
  const resolvedMaxDate = maxDate ?? dayjs("9998-12-31");
  const hasError = !disabled && Boolean(error);
  const minYear = resolvedMinDate.year();
  const maxYear = resolvedMaxDate.year();
  const [policyDateFormat, setPolicyDateFormat] = React.useState(getStoredDateFormat);
  const effectiveFormat = format || toDayjsDateFormat(policyDateFormat);
  const adapterLocale = useDayjsAdapterLocale();

  React.useEffect(() => {
    if (typeof window === "undefined") return;

    let isMounted = true;

    loadPolicyDateFormat().then((value) => {
      if (isMounted) setPolicyDateFormat(value);
    });

    const handlePolicyDateFormatChange = () => {
      setPolicyDateFormat(getStoredDateFormat());
    };

    window.addEventListener(DATE_FORMAT_CHANGED_EVENT, handlePolicyDateFormatChange);

    return () => {
      isMounted = false;
      window.removeEventListener(DATE_FORMAT_CHANGED_EVENT, handlePolicyDateFormatChange);
    };
  }, []);

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale={adapterLocale}>
      <DatePicker
        label={
          hideLabel ? undefined : (
            <>
              {typeof label === "string"
                ? label.replace(/\*+[\s]*$/, "")
                : label}
              {required && <span style={{ color: "#d32f2f" }}> *</span>}
            </>
          )
        }
        value={value}
        onChange={onChange}
        format={effectiveFormat}
        disabled={disabled}
        enableAccessibleFieldDOMStructure={enableAccessibleFieldDOMStructure}
        minDate={resolvedMinDate}
        maxDate={resolvedMaxDate}
        timezone="system"
        sx={{
          "&& .MuiOutlinedInput-root:hover fieldset": {
            borderColor: "var(--lms-primary) !important",
          },
          "&& .MuiOutlinedInput-root:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-primary) !important",
          },
          "&& .MuiOutlinedInput-root.Mui-focused fieldset": {
            borderColor: "var(--lms-primary) !important",
          },
          "&& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-primary) !important",
          },
          "& .MuiInputLabel-root.Mui-focused": {
            color: "var(--lms-primary) !important",
          },
          "&& .MuiPickersOutlinedInput-root:hover .MuiPickersOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-primary) !important",
          },
          "&& .MuiPickersOutlinedInput-root.Mui-focused .MuiPickersOutlinedInput-notchedOutline": {
            borderColor: "var(--lms-primary) !important",
          },
          "&& .MuiPickersOutlinedInput-root.Mui-error:not(.Mui-disabled) .MuiPickersOutlinedInput-notchedOutline": {
            borderColor: "#d32f2f !important",
          },
        }}
        slotProps={{
          ...slotProps,
          day: {
            ...slotProps.day,
            sx: {
              "&.Mui-selected": {
                backgroundColor: "var(--lms-primary) !important",
                color: "#fff !important",
              },
              "&.Mui-selected:hover": {
                backgroundColor: "var(--lms-primary-dark, var(--lms-primary)) !important",
              },
              "&.MuiPickersDay-today": {
                borderColor: "var(--lms-primary)",
              },
              ...slotProps.day?.sx,
            },
          },
          actionBar: {
            actions: ["clear", "today"],
            ...slotProps.actionBar,
            sx: {
              px: 1.5,
              py: 1,
              borderTop: "1px solid #e0e0e0",
              "& .MuiButton-root": {
                color: "var(--lms-primary)",
                fontWeight: 700,
                textTransform: "none",
                borderRadius: "8px",
                minWidth: 64,
              },
              "& .MuiButton-root:hover": {
                backgroundColor: "rgba(var(--lms-primary-rgb), 0.12)",
              },
              ...slotProps.actionBar?.sx,
            },
          },
          textField: {
            size: "small",
            fullWidth: true,
            variant: variant as any,
            error: hasError,
            helperText,
            ...slotProps.textField,
            InputLabelProps: {
              ...slotProps.textField?.InputLabelProps,
              sx: {
                fontWeight: required ? 700 : 600,
                fontSize: "0.85rem",
                "&.Mui-error": {
                  color: "#d32f2f",
                },
                "&.Mui-focused:not(.Mui-error)": {
                  color: "var(--lms-primary)",
                },
                ...slotProps.textField?.InputLabelProps?.sx,
              },
            },
            InputProps: {
              ...slotProps.textField?.InputProps,
              sx: {
                height: "40px",
                borderRadius: "9px",
                backgroundColor: disabled ? "#f5f5f5" : "#fff",
                "& input": {
                  padding: "12px",
                  height: "100%",
                  boxSizing: "border-box",
                  color: "var(--lms-text-main)",
                  WebkitTextFillColor: "var(--lms-text-main)",
                },
                "& .MuiInputBase-input.Mui-disabled": {
                  WebkitTextFillColor: "var(--lms-text-secondary)",
                },
                "& .MuiIconButton-root": {
                  color: "var(--lms-primary)",
                  transition: "all 0.2s ease",
                  "&:hover": {
                    backgroundColor: "rgba(var(--lms-primary-rgb), 0.12)",
                    color: "var(--lms-primary-dark, var(--lms-primary))",
                  },
                },
                "&.Mui-error .MuiIconButton-root": {
                  color: "#d32f2f",
                  "&:hover": {
                    backgroundColor: "rgba(211, 47, 47, 0.08)",
                    color: "#b71c1c",
                  },
                },
                ...slotProps.textField?.InputProps?.sx,
              },
            },
            sx: {
              "& .MuiInputLabel-root": {
                color: "var(--lms-text-secondary)",
              },
              "& .MuiInputLabel-root.Mui-focused:not(.Mui-error)": {
                color: "var(--lms-primary)",
              },
              "& .MuiInputLabel-root.Mui-error": {
                color: "#d32f2f",
              },
              "& .MuiOutlinedInput-root": {
                borderRadius: "9px",
                backgroundColor: disabled ? "#f5f5f5" : "#fff",
              },
              "& .MuiOutlinedInput-root .MuiOutlinedInput-notchedOutline": {
                borderColor: "#e0e0e0 !important",
                borderRadius: "9px",
              },
              "& .MuiOutlinedInput-root:hover .MuiOutlinedInput-notchedOutline": {
                borderColor: "var(--lms-primary) !important",
              },
              "& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline": {
                borderColor: "var(--lms-primary) !important",
                borderWidth: "2px",
              },
              "& .MuiOutlinedInput-root.Mui-error:not(.Mui-disabled) .MuiOutlinedInput-notchedOutline": {
                borderColor: "#d32f2f !important",
              },
              "& .MuiPickersOutlinedInput-root": {
                borderRadius: "9px",
                backgroundColor: disabled ? "#f5f5f5" : "#fff",
              },
              "& .MuiPickersOutlinedInput-root .MuiPickersOutlinedInput-notchedOutline": {
                borderColor: "#e0e0e0 !important",
                borderRadius: "9px",
              },
              "& .MuiPickersOutlinedInput-root:hover .MuiPickersOutlinedInput-notchedOutline": {
                borderColor: "var(--lms-primary) !important",
              },
              "& .MuiPickersOutlinedInput-root.Mui-focused .MuiPickersOutlinedInput-notchedOutline": {
                borderColor: "var(--lms-primary) !important",
                borderWidth: "2px",
              },
              "& .MuiPickersOutlinedInput-root.Mui-error:not(.Mui-disabled) .MuiPickersOutlinedInput-notchedOutline": {
                borderColor: "#d32f2f !important",
              },
              ...sx,
            },
          },
        }}
        slots={{
          ...slotProps?.slots,
          calendarHeader: (headerProps) => (
            <CustomCalendarHeader
              {...headerProps}
              minYear={minYear}
              maxYear={maxYear}
            />
          ),
        }}
      />
    </LocalizationProvider>
  );
};
