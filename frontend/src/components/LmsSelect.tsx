import Select from "@/components/ClearableSelect";
import React from "react";
import { MenuItem, FormControl, InputLabel, Typography } from "@mui/material";
import type { SelectProps } from "@mui/material";
import { KeyboardArrowDown as ArrowDownIcon } from "@mui/icons-material";
import { FormThemeWrapper } from "./FormThemeWrapper";
import { ToCheckArrayTypeAndLength } from "@/constants/globalItems";

interface LmsSelectProps {
  label?: React.ReactNode;
  value: any;
  options: { label: React.ReactNode; value: any }[];
  onChange: (value: any) => void;
  onClear?: () => void;
  clearable?: boolean;
  fullWidth?: boolean;
  required?: boolean;
  error?: boolean;
  helperText?: string;
  disabled?: boolean;
  hideLabel?: boolean;
  shrinkLabel?: boolean;
  placeholder?: string;
  placeholderInMenu?: boolean;
  renderValue?: SelectProps<any>["renderValue"];
  menuProps?: SelectProps<any>["MenuProps"];
  sx?: any;
}

export const LmsSelect: React.FC<LmsSelectProps> = ({
  label,
  value,
  options,
  onChange,
  onClear,
  clearable = true,
  fullWidth = true,
  required = false,
  error = false,
  helperText = "",
  disabled = false,
  hideLabel = false,
  shrinkLabel,
  placeholder = "",
  placeholderInMenu = true,
  renderValue,
  menuProps,
  sx = {},
}) => {
  const selectId = React.useId();
  const labelId = `lms-select-label-${selectId}`;
  const validOptions = ToCheckArrayTypeAndLength(options) ? options.filter(Boolean) : [];

  return (
    <FormThemeWrapper>
      <FormControl fullWidth={fullWidth} error={error} size="small" sx={sx}>
        {!hideLabel && (
          <InputLabel
            id={labelId}
            shrink={shrinkLabel}
            sx={{
              fontWeight: 600,
              fontSize: "0.85rem",
              "&.Mui-focused": {
                color: "var(--lms-primary)",
              },
            }}
          >
            {label}
            {required && <span style={{ color: "#d32f2f" }}> *</span>}
          </InputLabel>
        )}
        <Select
          notched={shrinkLabel}
          labelId={!hideLabel ? labelId : undefined}
          label={
            !hideLabel ? (
              <>
                {label}
                {required && <span> *</span>}
              </>
            ) : undefined
          }
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onClear={onClear}
          clearable={clearable}
          disabled={disabled}
          displayEmpty
          IconComponent={ArrowDownIcon}
          renderValue={renderValue}
          MenuProps={{
            ...menuProps,
            sx: { zIndex: 3000, ...(menuProps?.sx as object) },
            PaperProps: {
              ...menuProps?.PaperProps,
              sx: {
                borderRadius: "12px",
                marginTop: "2px",
                maxHeight: 250,
                boxShadow: "0 10px 15px -3px rgba(0, 0, 0, 0.1)",
                border: "1px solid #F1F5F9",
                overflowY: "auto",
                "& .MuiMenuItem-root": {
                  borderRadius: "8px",
                  margin: "2px 4px",
                  padding: "8px 12px",
                  fontSize: "0.875rem",
                  fontWeight: 500,
                  "&:hover": {
                    backgroundColor: "var(--lms-primary-soft)",
                    color: "var(--lms-primary)",
                  },
                  "&.Mui-selected": {
                    backgroundColor: "var(--lms-primary-soft) !important",
                    color: "var(--lms-primary)",
                    fontWeight: 700,
                  },
                },
                ...((menuProps?.PaperProps as any)?.sx || {}),
              },
            },
          }}
          sx={{
            borderRadius: "10px",
            backgroundColor: "#fff",
            "& .MuiSelect-select": {
              padding: "11px 14px",
              fontWeight: 600,
              fontSize: "0.875rem",
              color: value ? "inherit" : "#9CA3AF",
            },
            "& .MuiOutlinedInput-notchedOutline": {
              borderColor: "#E2E8F0",
            },
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: error ? "#d32f2f" : "var(--lms-primary)",
            },
            "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
              borderColor: error ? "#d32f2f" : "var(--lms-primary)",
              borderWidth: "1.5px",
            },
            "&.Mui-error .MuiOutlinedInput-notchedOutline": {
              borderColor: "#d32f2f",
            },
            "& .MuiSelect-icon": {
              color: error ? "#d32f2f" : "var(--lms-primary)",
            },
          }}
        >
          {placeholder && placeholderInMenu ? (
            <MenuItem value="" disabled>
              <Typography sx={{ color: "#9CA3AF", fontSize: "0.875rem" }}>
                {placeholder}
              </Typography>
            </MenuItem>
          ) : null}
          {validOptions.map((opt, idx) => (
            <MenuItem key={idx} value={opt.value}>
              {opt.label}
            </MenuItem>
          ))}
        </Select>
        {helperText && (
          <Typography
            variant="caption"
            sx={{ mt: 0.5, color: error ? "#d32f2f" : "#6B7280" }}
          >
            {helperText}
          </Typography>
        )}
      </FormControl>
    </FormThemeWrapper>
  );
};
