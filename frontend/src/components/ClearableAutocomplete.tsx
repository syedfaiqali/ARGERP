import React from "react";
import MuiAutocomplete from "@mui/material/Autocomplete";
import type { AutocompleteProps } from "@mui/material/Autocomplete";
import { Clear as ClearIcon } from "@mui/icons-material";

/** Keep MUI's null/[] clear callbacks and use the Employee Profile clear icon. */
const ClearableAutocomplete = React.forwardRef(function ClearableAutocomplete(
  { onClear, ...props }: AutocompleteProps<any, boolean, boolean, boolean> & { onClear?: () => void },
  ref: React.ForwardedRef<HTMLDivElement>,
) {
  return (
    <MuiAutocomplete
      {...props}
      ref={ref}
      onChange={(event, value, reason, details) => {
        if (reason === "clear" && onClear) onClear();
        else props.onChange?.(event, value, reason, details);
      }}
      disableClearable={props.disableClearable ?? false}
      clearText={props.clearText ?? "Clear selection"}
      clearIcon={props.clearIcon ?? <ClearIcon sx={{ fontSize: "1.05rem", color: "var(--lms-text-secondary)" }} />}
      sx={[
        {
          "& .MuiAutocomplete-clearIndicator": { visibility: "visible" },
          // Custom search adornments already center their children. Override the
          // form theme's absolute-position translation only for nested controls.
          "& .MuiInputAdornment-root .MuiAutocomplete-endAdornment": {
            position: "static",
            top: "auto",
            right: "auto",
            transform: "none",
            display: "flex",
            alignItems: "center",
          },
        },
        ...(Array.isArray(props.sx) ? props.sx : props.sx ? [props.sx] : []),
      ]}
    />
  );
}) as <
  Value,
  Multiple extends boolean | undefined = false,
  DisableClearable extends boolean | undefined = false,
  FreeSolo extends boolean | undefined = false,
>(props: AutocompleteProps<Value, Multiple, DisableClearable, FreeSolo> &
  { onClear?: () => void } & React.RefAttributes<HTMLDivElement>) => React.ReactElement;

export default ClearableAutocomplete;
