import React, { useMemo } from "react";
import { ThemeProvider, createTheme, useTheme } from "@mui/material/styles";

const ReuseFormThemeContext = React.createContext(false);

export const FormThemeWrapper: React.FC<{ children: React.ReactNode; reuseNestedTheme?: boolean }> = ({
  children,
  reuseNestedTheme = false,
}) => {
  const parentTheme = useTheme();
  const reuseParentTheme = React.useContext(ReuseFormThemeContext);

  // Create a custom theme that extends the parent theme but overrides specific components
  // for the "Login-style" form look (White box, Green borders on interaction)
  const formTheme = useMemo(
    () =>
      reuseParentTheme ? parentTheme : createTheme(parentTheme, {
        components: {
      MuiAutocomplete: {
        defaultProps: {
          disableClearable: false,
          forcePopupIcon: true,
        },
        styleOverrides: {
          root: {
            "& .MuiAutocomplete-endAdornment": {
              top: "50%",
              transform: "translateY(-50%)",
              display: "flex",
              alignItems: "center",
            },
            "& .MuiInputBase-root": {
              height: 40,
              cursor: "pointer",
            },
            "& .MuiInputBase-root.Mui-disabled": {
              backgroundColor: "#f5f5f5",
              cursor: "not-allowed",
            },
            "& .MuiInputBase-input": {
              padding: "12px 14px !important",
              cursor: "pointer !important",
              color: "var(--lms-text-main)",
              WebkitTextFillColor: "var(--lms-text-main)",
            },
            "& .MuiInputBase-input.Mui-disabled": {
              cursor: "not-allowed !important",
              color: "var(--lms-text-secondary)",
              WebkitTextFillColor: "var(--lms-text-secondary)",
            },
            "& .MuiAutocomplete-popupIndicator": {
              color: "var(--lms-primary)",
            },
            "& .MuiAutocomplete-popupIndicator.Mui-disabled": {
              color: "var(--lms-text-secondary)",
            },
          },
        },
      },
      MuiIconButton: {
        defaultProps: {
          disableRipple: true,
        },
        styleOverrides: {
          root: {
            "&:focus": {
              outline: "none",
            },
            "&.Mui-focusVisible": {
              outline: "none",
              backgroundColor: "transparent",
            },
          },
        },
      },
      MuiInputLabel: {
        styleOverrides: {
          root: {
            "&.Mui-required": {
              color: "#d32f2f",
              fontWeight: 800,
            },
            "&.Mui-required .MuiFormLabel-asterisk": {
              color: "#d32f2f",
              fontWeight: 900,
            },
          },
        },
      },
      MuiCheckbox: {
        defaultProps: {
          disableRipple: true,
        },
        styleOverrides: {
          root: {
            color: "#ccc",
            transition: "color 0.2s",
            "&.Mui-checked": {
              color: "var(--lms-primary)",
            },
            "&:hover": {
              color: "var(--lms-primary)",
              backgroundColor: "transparent",
            },
          },
        },
      },
      MuiFormControlLabel: {
        styleOverrides: {
          root: {
            marginLeft: "-4px", // Align checkbox better with standard margins
            "&:hover": {
              "& .MuiCheckbox-root": {
                color: "var(--lms-primary)",
                backgroundColor: "transparent",
              },
            },
          },
          label: {
            fontSize: "14px",
            color: "#555",
            userSelect: "none",
          },
        },
      },
      MuiInput: {
        styleOverrides: {
          underline: {
            "&:before": {
              borderBottomColor: "#e0e0e0",
            },
            "&:hover:not(.Mui-disabled):before": {
              borderBottomColor: "var(--lms-primary) !important",
            },
            "&:after": {
              borderBottomColor: "var(--lms-primary) !important",
            },
          },
        },
      },
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            backgroundColor: "#fff",
            borderRadius: "12px",
            "& fieldset": {
              borderColor: "#e0e0e0",
              borderRadius: "12px",
              transition: "border-color 0.2s",
            },
            "&:hover:not(.Mui-disabled) fieldset": {
              borderColor: "var(--lms-primary) !important",
            },
            "&.Mui-focused fieldset": {
              borderColor: "var(--lms-primary) !important",
              borderWidth: "1px",
            },
            "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
              borderColor: "var(--lms-primary) !important",
            },
            "&.Mui-disabled": {
              backgroundColor: "#f5f5f5",
            },
          },
          input: {
            padding: "8px 12px",
            "&:-webkit-autofill": {
              WebkitBoxShadow: "0 0 0 100px #ffffff inset",
              WebkitTextFillColor: "#333",
              borderRadius: "inherit",
            },
          },
        },
      },
      MuiRadio: {
        defaultProps: {
          disableRipple: true,
        },
        styleOverrides: {
          root: {
            color: "#ccc",
            transition: "color 0.2s",
            "&.Mui-checked": {
              color: "var(--lms-primary)",
            },
            "&:hover": {
              color: "var(--lms-primary)",
              backgroundColor: "transparent",
            },
          },
        },
      },
      MuiSelect: {
        styleOverrides: {
          root: {
            backgroundColor: "#fff",
            borderRadius: "12px",
            "& .MuiOutlinedInput-notchedOutline": {
              borderColor: "#e0e0e0",
              borderRadius: "12px",
            },
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: "var(--lms-primary) !important",
            },
            "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
              borderColor: "var(--lms-primary) !important",
              borderWidth: "1px",
            },
          },
          select: {
            padding: "8px 12px",
          },
        },
      },
      MuiSvgIcon: {
        styleOverrides: {
          root: {
            // Target the calendar icon specifically
            '&[data-testid="CalendarIcon"]': {
              color: "var(--lms-primary)",
            },
            // Target any icon that should be primary green
            "&.lms-primary-icon": {
              color: "var(--lms-primary)",
            },
          },
        },
      },
        },
      }),
    [parentTheme, reuseParentTheme],
  );

  if (reuseParentTheme) return <>{children}</>;
  const content = <ThemeProvider theme={formTheme}>{children}</ThemeProvider>;
  return reuseNestedTheme ? <ReuseFormThemeContext.Provider value>{content}</ReuseFormThemeContext.Provider> : content;
};
