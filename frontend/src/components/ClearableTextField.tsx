import React from "react";
import MuiTextField from "@mui/material/TextField";
import type { TextFieldProps } from "@mui/material/TextField";
import { useFormControl } from "@mui/material/FormControl";
import { InputAdornment } from "@mui/material";
import { ClearSelectionButton } from "./ClearSelectionButton";

/** Add the shared clear icon only when a TextField is being used as a select. */
const ClearableTextField = React.forwardRef(function ClearableTextField(
  { onClear, ...props }: TextFieldProps & { onClear?: () => void },
  ref: React.ForwardedRef<HTMLDivElement>,
) {
  const formControl = useFormControl();
  const value = props.SelectProps?.value ?? props.value;
  const disabled = props.disabled || props.SelectProps?.disabled || formControl?.disabled;
  const readOnly = props.InputProps?.readOnly || props.inputProps?.readOnly || props.SelectProps?.readOnly ||
    props.SelectProps?.inputProps?.readOnly || props.SelectProps?.input?.props.readOnly;
  const hasValue = Array.isArray(value) ? value.length > 0 : value !== undefined && value !== null && value !== "";
  const canClear = props.select && !disabled && !readOnly && hasValue &&
    (onClear || props.SelectProps?.onChange || props.onChange);
  const existingAdornment = props.SelectProps?.endAdornment ?? props.InputProps?.endAdornment;

  return (
    <MuiTextField
      {...props}
      ref={ref}
      SelectProps={canClear ? {
        ...props.SelectProps,
        endAdornment: (
          <InputAdornment position="end" sx={{ mr: 2.5 }}>
            {existingAdornment}
            <ClearSelectionButton onClear={(event) => {
              if (onClear) {
                onClear();
                return;
              }
              const target = { value: props.SelectProps?.multiple ? [] : "", name: props.SelectProps?.name ?? props.name ?? "" };
              const clearedEvent = { ...event, target, currentTarget: target };
              if (props.SelectProps?.onChange) {
                props.SelectProps.onChange(clearedEvent as any, null);
              } else {
                props.onChange?.(clearedEvent as any);
              }
            }} />
          </InputAdornment>
        ),
      } : props.SelectProps}
    />
  );
}) as (props: TextFieldProps & { onClear?: () => void } & React.RefAttributes<HTMLDivElement>) => React.ReactElement;

export default ClearableTextField;
