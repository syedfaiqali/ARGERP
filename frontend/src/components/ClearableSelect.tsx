import React from "react";
import MuiSelect from "@mui/material/Select";
import type { SelectChangeEvent, SelectProps } from "@mui/material/Select";
import { useFormControl } from "@mui/material/FormControl";
import { InputAdornment } from "@mui/material";
import { ClearSelectionButton } from "./ClearSelectionButton";

export type ClearableSelectProps<Value = unknown> = SelectProps<Value> & {
  onClear?: () => void;
  clearable?: boolean;
};

/** Preserve the usual Select onChange contract when clearing a controlled value. */
const ClearableSelect = React.forwardRef(function ClearableSelect(
  { onClear, clearable = true, ...props }: ClearableSelectProps,
  ref: React.ForwardedRef<unknown>,
) {
  const formControl = useFormControl();
  const disabled = props.disabled ?? formControl?.disabled;
  const readOnly = props.readOnly || props.inputProps?.readOnly || props.input?.props.readOnly;
  const hasValue = Array.isArray(props.value)
    ? props.value.length > 0
    : props.value !== undefined && props.value !== null && props.value !== "";
  const canClear = clearable && !disabled && !readOnly && hasValue && (onClear || props.onChange);
  const existingAdornment = props.endAdornment ?? props.input?.props.endAdornment;

  return (
    <MuiSelect
      {...props}
      ref={ref}
      endAdornment={canClear ? (
        <InputAdornment position="end" sx={{ mr: 2.5 }}>
          {existingAdornment}
          <ClearSelectionButton onClear={(event) => {
            if (onClear) {
              onClear();
              return;
            }
            const target = { value: props.multiple ? [] : "", name: props.name ?? "" };
            props.onChange?.({ ...event, target, currentTarget: target } as unknown as SelectChangeEvent<unknown>, null);
          }} />
        </InputAdornment>
      ) : existingAdornment}
    />
  );
}) as <Value = unknown>(props: ClearableSelectProps<Value> & React.RefAttributes<unknown>) => React.ReactElement;

export default ClearableSelect;
