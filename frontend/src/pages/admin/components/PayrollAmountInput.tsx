import React, { useState } from "react";
import { InputAdornment } from "@mui/material";
import { FormInput } from "@/components/FormInput";
import { formatPayrollAmount } from "./employeeAllowanceCalculation";

interface Props {
  value: number | "";
  onChange: (value: number | "") => void;
  percentage?: boolean;
  disabled?: boolean;
  label: string;
  hideLabel?: boolean;
  sx?: any;
}

export const PayrollAmountInput: React.FC<Props> = ({ value, onChange, percentage = false,
  disabled, label, hideLabel = false, sx }) => {
  const [focused, setFocused] = useState(false);
  const [draft, setDraft] = useState("");
  return (
    <FormInput
      label={label}
      hideLabel={hideLabel}
      value={focused ? draft : formatPayrollAmount(value, percentage)}
      inputMode="decimal"
      amountDecimalPlaces={2}
      amountMaxDigits={percentage ? 3 : 12}
      disabled={disabled}
      error={percentage && value !== "" && value > 100}
      inputProps={{ "aria-label": label }}
      InputProps={{ endAdornment: percentage ? <InputAdornment position="end">%</InputAdornment> : undefined }}
      onClick={(event: React.MouseEvent) => event.stopPropagation()}
      onFocus={() => { setDraft(value === "" ? "" : String(value)); setFocused(true); }}
      onBlur={() => setFocused(false)}
      onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
        const next = event.target.value;
        setDraft(next);
        onChange(next === "" || next === "." ? "" : Number(next));
      }}
      sx={sx}
    />
  );
};
