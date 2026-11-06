import React from "react";
import { Checkbox, FormControlLabel, Typography } from "@mui/material";
import type { SxProps, Theme } from "@mui/material/styles";

interface LmsCheckboxProps {
  checked?: boolean;
  defaultChecked?: boolean;
  onChange?: (
    checked: boolean,
    event: React.ChangeEvent<HTMLInputElement>,
  ) => void;
  label?: React.ReactNode;
  disabled?: boolean;
  size?: "small" | "medium";
  name?: string;
  icon?: React.ReactNode;
  checkedIcon?: React.ReactNode;
  indeterminate?: boolean;
  hideLabel?: boolean;
  formControlSx?: SxProps<Theme>;
  labelSx?: SxProps<Theme>;
  checkboxSx?: SxProps<Theme>;
}

export const LmsCheckbox: React.FC<LmsCheckboxProps> = ({
  checked,
  defaultChecked,
  onChange,
  label,
  disabled = false,
  size = "small",
  name,
  icon,
  checkedIcon,
  indeterminate = false,
  hideLabel = false,
  formControlSx,
  labelSx,
  checkboxSx,
}) => {
  const checkboxNode = (
    <Checkbox
      name={name}
      size={size}
      checked={checked}
      defaultChecked={defaultChecked}
      disabled={disabled}
      indeterminate={indeterminate}
      icon={icon}
      checkedIcon={checkedIcon}
      onChange={(event) => onChange?.(event.target.checked, event)}
      sx={{
        "& .MuiSvgIcon-root": {
          borderRadius: "4px",
          transition: "all 0.25s cubic-bezier(0.4, 0, 0.2, 1)",
          boxShadow: "none",
          transform: "scale(1)",
          color: "var(--lms-text-main)",
        },
        "&.Mui-checked .MuiSvgIcon-root": {
          color: "var(--lms-primary)",
          boxShadow: "none",
          transform: "scale(1)",
        },
        "&:hover .MuiSvgIcon-root": {
          color: "var(--lms-primary)",
          boxShadow: "0 6px 16px rgba(var(--lms-primary-rgb), 0.68)",
        },
        "&.Mui-checked:hover .MuiSvgIcon-root": {
          boxShadow: "0 6px 16px rgba(var(--lms-primary-rgb), 0.68)",
        },
        ...checkboxSx,
      }}
    />
  );

  if (hideLabel) {
    return checkboxNode;
  }

  return (
    <FormControlLabel
      sx={formControlSx}
      disabled={disabled}
      control={checkboxNode}
      label={
        typeof label === "string" ? (
          <Typography
            sx={{
              fontSize: "0.85rem",
              fontWeight: 600,
              color: "var(--lms-text-main)",
              ...labelSx,
            }}
          >
            {label}
          </Typography>
        ) : (
          label
        )
      }
    />
  );
};

