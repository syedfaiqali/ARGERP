import { FormInput } from "@/components/FormInput";
import React from "react";
import {
  Autocomplete,
  Chip,
  IconButton,
  InputAdornment,
  Popper,
} from "@mui/material";
import {
  Clear as ClearIcon,
  Search as SearchIcon,
  Menu as MenuIcon,
} from "@mui/icons-material";
import { FormThemeWrapper } from "./FormThemeWrapper";
import type { PopperProps } from "@mui/material/Popper";

const LookupDropdownPopper = (props: PopperProps) => (
  <Popper {...props} style={{ ...(props.style ?? {}), zIndex: 3000 }} />
);

// Clearing is available in every module, including lookups outside the HR shell.
const LookupDropdownAutoClearContext = React.createContext(true);

export const LookupDropdownAutoClearProvider: React.FC<
  React.PropsWithChildren
> = ({ children }) => (
  <LookupDropdownAutoClearContext.Provider value>
    {children}
  </LookupDropdownAutoClearContext.Provider>
);

const emptyLookupOptionValues: Record<string, any> = {
  id: 0,
  value: "",
  code: "",
  name: "",
  label: "",
  stxt: "",
  ltxt: "",
  shortName: "",
  description: "",
  employeeId: 0,
  employeeCode: "",
  employeeName: "",
  departmentId: 0,
  departmentName: "",
  designationId: 0,
  designationName: "",
  entityId: 0,
  entityName: "",
  categoryId: 0,
  categoryName: "",
  typeId: 0,
  typeName: "",
  groupId: 0,
  groupName: "",
  shiftId: 0,
  shiftName: "",
  statusId: 0,
  statusName: "",
};

const emptyLookupOption = new Proxy(emptyLookupOptionValues, {
  get: (target, property) => {
    if (typeof property !== "string") return Reflect.get(target, property);
    if (property in target) return target[property];
    return property.toLowerCase().endsWith("id") ? 0 : "";
  },
});

interface LookupDropdownProps {
  maxLength?: number;
  label?: React.ReactNode;
  value: any;
  options: any[];
  getOptionLabel?: (option: any) => string;
  getOptionDescription?: (option: any) => string;
  getOptionStyle?: (option: any) => React.CSSProperties;
  getOptionDisabled?: (option: any) => boolean;
  onSelect?: (option: any) => void;
  onChange?: (value: any) => void;
  onSearchClick?: () => void;
  onDropdownOpen?: () => void;
  showListIcon?: boolean;
  onListClick?: () => void;
  fullWidth?: boolean;
  required?: boolean;
  error?: boolean;
  helperText?: string;
  disabled?: boolean;
  allowSearchWhenDisabled?: boolean;
  hideSearchIcon?: boolean;
  hideLabel?: boolean;
  placeholder?: string;
  sx?: any;
  hideDropdownArrow?: boolean;
  openSearchOnFieldClick?: boolean;
  allowCustomValue?: boolean;
  onChangeInput?: (value: string) => void;
  isLoading?: boolean;
  searchIcon?: React.ReactNode;
  onClear?: () => void;
  searchIconOpensDropdown?: boolean;
  disableKeyboardTypeahead?: boolean;
  /** Search available options by typing (enabled by default); selection still requires an option. */
  searchable?: boolean;
  /** Keep canonical record names from being rewritten by institution terminology. */
  preserveOptionLabels?: boolean;
  multiple?: boolean;
  onMultiSelect?: (options: any[]) => void;
  /** Lets the option list grow wider than a narrow field (for example a grid cell), in px. */
  dropdownMinWidth?: number;
}

export const LookupDropdown: React.FC<LookupDropdownProps> = ({
  maxLength,
  label,
  value,
  options = [],
  getOptionLabel,
  getOptionDescription,
  getOptionStyle,
  getOptionDisabled,
  onSelect,
  onChange,
  onSearchClick,
  onDropdownOpen,
  showListIcon = false,
  onListClick,
  fullWidth = true,
  required = false,
  error = false,
  helperText = "",
  disabled = false,
  allowSearchWhenDisabled = false,
  hideSearchIcon = false,
  hideLabel = false,
  placeholder = "",
  sx = {},
  hideDropdownArrow = true,
  openSearchOnFieldClick = false,
  allowCustomValue = false,
  onChangeInput,
  isLoading = false,
  searchIcon,
  onClear,
  searchIconOpensDropdown = false,
  disableKeyboardTypeahead = false,
  searchable = true,
  preserveOptionLabels = false,
  multiple = false,
  onMultiSelect,
  dropdownMinWidth,
}) => {
  const autoClearEnabled = React.useContext(
    LookupDropdownAutoClearContext,
  );
  // Every lookup exposes both selection paths: its dropdown arrow and a search
  // icon.  Where a screen has no dedicated search dialog, the search icon opens
  // the same, filterable lookup list instead of becoming a dead control.
  const canShowSearchIcon = !hideSearchIcon;
  const hasClearAction =
    typeof onClear === "function" ||
    (autoClearEnabled &&
      (typeof onSelect === "function" || typeof onChange === "function" ||
        (multiple && typeof onMultiSelect === "function")));
  const resolveLabel = (option: any) => {
    if (typeof option === "string") return option;
    if (option && typeof option === "object" && "__fallbackValue" in option) {
      return String(option.__fallbackValue ?? "");
    }
    if (typeof getOptionLabel === "function") return getOptionLabel(option);
    if (option?.label !== undefined) return String(option.label);
    if (option?.title !== undefined) return String(option.title);
    if (option?.stxt !== undefined) return String(option.stxt);
    if (option?.ltxt !== undefined) return String(option.ltxt);
    if (option?.name !== undefined) return String(option.name);
    if (option?.code !== undefined) return String(option.code);
    return String(option ?? "");
  };

  const resolveValueKey = (option: any) => {
    if (option && typeof option === "object") {
      if (option.value !== undefined && option.value !== null) return String(option.value);
      if (option.id !== undefined && option.id !== null) return String(option.id);
    }
    return String(resolveLabel(option));
  };

  // Lookup data may arrive through an API envelope while a screen is loading.
  // A dropdown must remain usable in that state rather than throwing from
  // Array.prototype methods during render.
  const optionList = Array.isArray(options) ? options : [];

  // Keep selected value visible even when options are not yet loaded.
  const matchedOption = optionList.find((opt) => {
    const normalizedValue = String(value).trim().toLowerCase();
    return (
      String(resolveValueKey(opt)).trim().toLowerCase() === normalizedValue ||
      String(resolveLabel(opt)).trim().toLowerCase() === normalizedValue
    );
  });
  const hasValue =
    value !== null && value !== undefined && String(value).trim() !== "";
  // Many lookup handlers use zero for an empty ID. Keep a real zero-valued
  // option clearable, but do not leave a clear button on an empty lookup.
  const canShowClearIcon = !disabled && hasClearAction && hasValue &&
    (!Array.isArray(value) || value.length > 0) &&
    (String(value).trim() !== "0" || !!matchedOption);
  const canShowEndIcons = canShowClearIcon || canShowSearchIcon || showListIcon;
  const isNumericValue = hasValue && !Number.isNaN(Number(String(value)));
  // Keep a not-yet-loaded selection stable while the user types a search query.
  const selectedOption = React.useMemo(() =>
    matchedOption || (hasValue && !isNumericValue ? { __fallbackValue: value } : null),
    [matchedOption, hasValue, isNumericValue, value],
  );
  const selectedOptions = React.useMemo(() => {
    if (!multiple) return [];
    const selectedValues = new Set(
      (Array.isArray(value) ? value : String(value ?? "").split(","))
        .map((item) => String(item).trim().toLowerCase())
        .filter(Boolean),
    );

    return optionList.filter((opt) => {
      const valueKey = resolveValueKey(opt).trim().toLowerCase();
      const label = resolveLabel(opt).trim().toLowerCase();
      return selectedValues.has(valueKey) || selectedValues.has(label);
    });
  }, [multiple, optionList, value]);
  const [dropdownOpen, setDropdownOpen] = React.useState(false);
  const [typeaheadIndex, setTypeaheadIndex] = React.useState<number | null>(null);
  const typeaheadQueryRef = React.useRef("");
  const typeaheadResetTimerRef = React.useRef<number | null>(null);
  const listboxRef = React.useRef<HTMLUListElement | null>(null);
  // Autocomplete sizes the popper to the field. With dropdownMinWidth the list keeps at least
  // that width, anchored to the field's left edge, so long codes and names are readable.
  const popperComponent = React.useMemo(() => {
    if (!dropdownMinWidth) return LookupDropdownPopper;
    return (props: PopperProps) => (
      <Popper
        {...props}
        placement="bottom-start"
        style={{
          ...(props.style ?? {}),
          zIndex: 3000,
          width: "auto",
          minWidth: Math.max(Number(props.style?.width) || 0, dropdownMinWidth),
        }}
      />
    );
  }, [dropdownMinWidth]);

  const openDropdown = () => {
    if (!dropdownOpen) onDropdownOpen?.();
    setDropdownOpen(true);
  };

  const toggleDropdown = () => {
    if (dropdownOpen) {
      setDropdownOpen(false);
      return;
    }

    onDropdownOpen?.();
    setDropdownOpen(true);
  };

  const clearTypeaheadQuery = () => {
    typeaheadQueryRef.current = "";
    if (typeaheadResetTimerRef.current !== null) {
      window.clearTimeout(typeaheadResetTimerRef.current);
      typeaheadResetTimerRef.current = null;
    }
  };

  const selectOption = (newValue: any) => {
    if (multiple) {
      onMultiSelect?.(Array.isArray(newValue) ? newValue : []);
      return;
    }

    if (!newValue || getOptionDisabled?.(newValue)) return;

    if (typeof newValue === "string") {
      onSelect?.({ id: undefined, name: newValue, __custom: true });
      onChange?.(newValue);
      return;
    }

    onSelect?.(newValue);
    onChange?.(resolveValueKey(newValue));
  };

  const clearSelection = () => {
    if (disabled) return;
    setDropdownOpen(false);
    clearTypeaheadQuery();
    setTypeaheadIndex(null);
    if (onClear) {
      onClear();
    } else if (multiple) {
      onMultiSelect?.([]);
    } else {
      onSelect?.(emptyLookupOption);
      onChange?.("");
      if (allowCustomValue) onChangeInput?.("");
    }
  };

  const findTypeaheadIndex = (query: string) => {
    const normalizedQuery = query.trim().toLowerCase();
    if (!normalizedQuery) return -1;

    return optionList.findIndex((option) =>
      !getOptionDisabled?.(option) && resolveLabel(option).trim().toLowerCase().startsWith(normalizedQuery),
    );
  };

  const handleTypeaheadKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
    if (disabled || allowCustomValue || searchable || multiple || disableKeyboardTypeahead) return;

    if (event.key === "Enter" && typeaheadIndex !== null && dropdownOpen) {
      event.preventDefault();
      selectOption(optionList[typeaheadIndex]);
      setDropdownOpen(false);
      clearTypeaheadQuery();
      return;
    }

    if (event.key === "Escape") {
      clearTypeaheadQuery();
      setTypeaheadIndex(null);
      return;
    }

    const isPrintableKey =
      event.key.length === 1 && !event.altKey && !event.ctrlKey && !event.metaKey;
    if (!isPrintableKey) return;

    event.preventDefault();
    if (!dropdownOpen) openDropdown();

    const nextQuery = `${typeaheadQueryRef.current}${event.key}`.toLowerCase();
    let nextIndex = findTypeaheadIndex(nextQuery);
    let acceptedQuery = nextQuery;

    if (nextIndex === -1) {
      acceptedQuery = event.key.toLowerCase();
      nextIndex = findTypeaheadIndex(acceptedQuery);
    }

    typeaheadQueryRef.current = acceptedQuery;
    setTypeaheadIndex(nextIndex >= 0 ? nextIndex : null);

    if (typeaheadResetTimerRef.current !== null) {
      window.clearTimeout(typeaheadResetTimerRef.current);
    }
    typeaheadResetTimerRef.current = window.setTimeout(() => {
      typeaheadQueryRef.current = "";
      typeaheadResetTimerRef.current = null;
    }, 900);
  };

  React.useEffect(() => {
    if (!dropdownOpen || typeaheadIndex === null) return;

    const optionElement = listboxRef.current?.querySelector(
      `[data-lookup-typeahead-index="${typeaheadIndex}"]`,
    );
    optionElement?.scrollIntoView({ block: "nearest" });
  }, [dropdownOpen, typeaheadIndex]);

  React.useEffect(
    () => () => {
      if (typeaheadResetTimerRef.current !== null) {
        window.clearTimeout(typeaheadResetTimerRef.current);
      }
    },
    [],
  );

  return (
    <FormThemeWrapper>
      <Autocomplete
        multiple={multiple}
        disableCloseOnSelect={multiple}
        options={optionList}
        getOptionLabel={resolveLabel}
        getOptionDisabled={getOptionDisabled}
        // Compare by id/value when present to avoid multiple rows appearing "selected"
        // when several options share the same label text.
        isOptionEqualToValue={(option, selected) =>
          resolveValueKey(option) === resolveValueKey(selected)
        }
        value={multiple ? selectedOptions : selectedOption}
        freeSolo={allowCustomValue}
        open={dropdownOpen}
        onOpen={openDropdown}
        onClose={() => {
          setDropdownOpen(false);
          clearTypeaheadQuery();
          setTypeaheadIndex(null);
        }}
        loading={isLoading}
        onChange={(_, newValue, reason) => {
          if (reason === "clear") clearSelection();
          else selectOption(newValue);
        }}
        onInputChange={(_, newInputValue) => {
          if (allowCustomValue && onChangeInput) {
            onChangeInput(newInputValue);
          }
        }}
        renderOption={(props, option, { index }) => {
          const label = resolveLabel(option);
          const key = option?.id
            ? `${option.id}-${label}`
            : `${label}-${index}`;
          return (
            <li
              {...props}
              key={key}
              style={{ ...props.style, ...getOptionStyle?.(option) }}
              data-lookup-typeahead-index={index}
              data-terminology-skip={preserveOptionLabels ? true : undefined}
              className={`${props.className ?? ""}${index === typeaheadIndex ? " Mui-focused" : ""}`.trim()}
            >
              {getOptionDescription?.(option) ? `${label} — ${getOptionDescription(option)}` : label}
            </li>
          );
        }}
        PopperComponent={popperComponent}
        openOnFocus={multiple || searchable}
        disableClearable
        noOptionsText="No options"
        fullWidth={fullWidth}
        disabled={disabled}
        // The form theme fixes lookups at 40px. A multi-select grows instead, so its
        // chips wrap onto new rows inside the field rather than spilling below it.
        sx={
          multiple
            ? {
                "& .MuiInputBase-root": { height: "auto", minHeight: 40 },
                "& .MuiAutocomplete-inputRoot .MuiAutocomplete-input": {
                  padding: "2.5px 4px 2.5px 8px !important",
                },
              }
            : undefined
        }
        ListboxProps={{
          ref: listboxRef,
          sx: {
            // Keep long lookup lists (for example Buyer Type and Buyer Name)
            // inside a dedicated, reliably scrollable listbox.
            maxHeight: "min(320px, 45vh)",
            overflowY: "auto",
            overscrollBehavior: "contain",
            "& .MuiAutocomplete-option": {
              fontWeight: 600,
              color: "var(--lms-text-main)",
            },
            "& .MuiAutocomplete-option.Mui-focused": {
              backgroundColor: "rgba(var(--lms-primary-rgb), 0.12)",
            },
            "& .MuiAutocomplete-option[aria-selected='true']": {
              backgroundColor: "rgba(var(--lms-primary-rgb), 0.2)",
              color: "var(--lms-primary-dark, var(--lms-primary))",
            },
            "& .MuiAutocomplete-option[aria-selected='true'].Mui-focused": {
              backgroundColor: "rgba(var(--lms-primary-rgb), 0.26)",
            },
          },
        }}
        renderInput={(params) => (
          <FormInput
            {...params}
            // Autocomplete keeps its text in `params.inputProps.value` rather
            // than in a top-level TextField value. FormInput explicitly
            // controls the top-level value, so forwarding it is essential for
            // existing Program lookup selections to remain visible on edit.
            value={params.inputProps.value ?? ""}
            onChange={params.inputProps.onChange}
            debounceMs={0}
            onKeyDown={handleTypeaheadKeyDown}
            onMouseDown={(event) => {
              if (disabled) return;
              // If allowing custom values, let the user click into it.
              // If not, we do manual toggle.
              if (!allowCustomValue && !searchable) {
                if (multiple && (event.target as HTMLElement).closest("input")) {
                  return;
                }
                event.preventDefault();
                if (multiple) {
                  toggleDropdown();
                  return;
                }
                if (openSearchOnFieldClick) {
                  setDropdownOpen(false);
                  onSearchClick?.();
                  return;
                }
                toggleDropdown();
              }
            }}
            onClick={() => {
              if (disabled) return;
              if (allowCustomValue || searchable) {
                openDropdown();
              } else if (openSearchOnFieldClick) {
                onSearchClick?.();
              }
            }}
            label={
              !hideLabel ? (
                <>
                  {typeof label === "string"
                    ? label.replace(/\*+[\s]*$/, "")
                    : label}
                  {required && <span style={{ color: "#d32f2f" }}> *</span>}
                </>
              ) : undefined
            }
            hideLabel={hideLabel}
            variant="outlined"
            size="small"
            error={error}
            helperText={helperText}
            placeholder={placeholder}
            inputProps={{
              ...params.inputProps,
              ...(maxLength === undefined ? {} : { maxLength }),
              readOnly: !allowCustomValue && !searchable && !multiple,
              style: {
                cursor: disabled
                  ? "not-allowed"
                  : !allowCustomValue && !searchable && !multiple
                    ? "pointer"
                    : "text",
                color: disabled
                  ? "var(--lms-text-secondary)"
                  : "var(--lms-text-main)",
                WebkitTextFillColor: disabled
                  ? "var(--lms-text-secondary)"
                  : "var(--lms-text-main)",
                fontWeight: 600,
              },
            }}
            InputLabelProps={{
              sx: {
                fontWeight: 700,
                fontSize: "0.85rem",
                "&.Mui-focused": {
                  color: "var(--lms-primary)",
                },
              },
            }}
            InputProps={{
              ...params.InputProps,
              endAdornment: (
                <>
                  {!hideDropdownArrow && params.InputProps.endAdornment}
                  {canShowEndIcons && (
                    <InputAdornment
                      position="end"
                      sx={{
                        position: "absolute",
                        right: hideDropdownArrow ? 9 : 35,
                        // Keep the icons centred when a multi-select grows taller.
                        ...(multiple ? { top: "50%", transform: "translateY(-50%)" } : {}),
                        gap: 0.25,
                        alignItems: "center",
                      }}
                    >
                      {canShowClearIcon && (
                        <IconButton
                          size="small"
                          disableRipple
                          aria-label="Clear selection"
                          sx={{
                            color: "var(--lms-text-secondary)",
                            p: 0.25,
                          }}
                          onClick={(e) => {
                            e.stopPropagation();
                            clearSelection();
                          }}
                          onMouseDown={(e) => {
                            e.preventDefault();
                            e.stopPropagation();
                          }}
                        >
                          <ClearIcon sx={{ fontSize: "1.05rem" }} />
                        </IconButton>
                      )}
                      {canShowSearchIcon && (
                        <IconButton
                          size="small"
                          disableRipple
                          disabled={disabled && !allowSearchWhenDisabled}
                          sx={{
                            color:
                              disabled && !allowSearchWhenDisabled
                                ? "var(--lms-text-secondary)"
                                : "var(--lms-primary)",
                            p: 0.25,
                            backgroundColor: (disabled && allowSearchWhenDisabled) ? "#fff" : "transparent",
                            borderRadius: "4px",
                            boxShadow: (disabled && allowSearchWhenDisabled) ? "0px 1px 3px rgba(0,0,0,0.1)" : "none",
                            cursor:
                              disabled && !allowSearchWhenDisabled
                                ? "not-allowed"
                                : "pointer",
                          }}
                          onClick={(e) => {
                            e.stopPropagation();
                            if (disabled && !allowSearchWhenDisabled) return;
                            if (searchIconOpensDropdown || !onSearchClick) {
                              toggleDropdown();
                              return;
                            }
                            setDropdownOpen(false); // Close dropdown immediately
                            onSearchClick?.();
                          }}
                          onMouseDown={(e) => {
                            e.stopPropagation(); // Prevent Autocomplete from reacting
                          }}
                        >
                          {searchIcon || (
                            <SearchIcon
                              sx={{
                                fontSize: "1.1rem",
                                cursor:
                                  disabled && !allowSearchWhenDisabled
                                    ? "not-allowed"
                                    : "pointer",
                              }}
                            />
                          )}
                        </IconButton>
                      )}
                      {showListIcon && (
                        <IconButton
                          size="small"
                          disableRipple
                          disabled={disabled}
                          sx={{
                            color: disabled
                              ? "var(--lms-text-secondary)"
                              : "var(--lms-primary)",
                            ml: 0.25,
                            p: 0.25,
                            cursor: disabled ? "not-allowed" : "pointer",
                          }}
                          onClick={(e) => {
                            e.stopPropagation();
                            if (disabled) return;
                            setDropdownOpen(false);
                            if (onListClick) {
                              onListClick();
                            } else {
                              toggleDropdown();
                            }
                          }}
                          onMouseDown={(e) => {
                            e.stopPropagation();
                          }}
                        >
                          <MenuIcon sx={{ fontSize: "1.15rem" }} />
                        </IconButton>
                      )}
                    </InputAdornment>
                  )}
                </>
              ),
            }}
            sx={sx}
          />
        )}
        renderTags={(selectedRows, getTagProps) =>
          selectedRows.map((option, index) => {
            const { key, ...tagProps } = getTagProps({ index });
            return (
              <Chip
                key={key}
                label={resolveLabel(option)}
                size="small"
                sx={{
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.12)",
                  border: "1px solid rgba(var(--lms-primary-rgb), 0.28)",
                  color: "var(--lms-primary)",
                  fontWeight: 700,
                  "& .MuiChip-deleteIcon": {
                    color: "rgba(var(--lms-primary-rgb), 0.72)",
                    "&:hover": {
                      color: "var(--lms-primary-dark)",
                    },
                  },
                }}
                {...tagProps}
              />
            );
          })
        }
      />
    </FormThemeWrapper>
  );
};
