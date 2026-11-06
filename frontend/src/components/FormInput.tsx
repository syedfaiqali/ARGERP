import TextField from "@/components/ClearableTextField";
import React, {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { TextFieldProps } from "@mui/material";
import {
  GetNextAmountInputValue,
  IsValidAmountInput,
  RemoveAmountCommas,
} from "@/constants/globalItems";

type FormInputProps = TextFieldProps & {
  label?: React.ReactNode;
  required?: boolean;
  hideLabel?: boolean;
  debounceMs?: number;
  maxLength?: number;
  sanitizeInput?: (value: string) => string;
  amountMaxDigits?: number;
  amountDecimalPlaces?: number;
  [key: string]: any;
};

const DEFAULT_TEXT_INPUT_DEBOUNCE_MS = 300;

const FormInputImpl: React.FC<FormInputProps> = ({
  label,
  required,
  disabled,
  multiline,
  rows,
  hideLabel = false,
  debounceMs,
  value,
  onChange,
  onBlur,
  onFocus,
  onCompositionStart,
  onCompositionEnd,
  fullWidth = true,
  size = "small",
  variant = "outlined",
  sx,
  InputProps,
  inputProps,
  InputLabelProps,
  onKeyDown,
  inputMode,
  maxLength,
  sanitizeInput,
  amountMaxDigits,
  amountDecimalPlaces,
  ...props
}) => {
  const externalValue = value == null ? "" : String(value);
  const [localValue, setLocalValue] = useState(externalValue);
  const isComposingRef = useRef(false);
  const isFocusedRef = useRef(false);
  const shouldSelectAmountOnFocusRef = useRef(false);
  const lastEmittedValueRef = useRef(externalValue);
  const inputType = (props.type ?? "text").toLowerCase();
  const isTextLikeType = [
    "text",
    "search",
    "email",
    "password",
    "tel",
    "url",
  ].includes(inputType);
  const isAmountInput =
    inputMode === "decimal" || inputProps?.inputMode === "decimal";
  const nativeInputMode = (isAmountInput ? "decimal" : inputMode) as
    | React.HTMLAttributes<HTMLInputElement>["inputMode"]
    | undefined;
  const shouldAutoDebounce =
    value !== undefined &&
    !!onChange &&
    !disabled &&
    !props.select &&
    !isAmountInput &&
    isTextLikeType;
  const effectiveDebounceMs =
    debounceMs === undefined
      ? shouldAutoDebounce
        ? DEFAULT_TEXT_INPUT_DEBOUNCE_MS
        : 0
      : debounceMs;
  const isBufferedInput = effectiveDebounceMs > 0;

  const hasVisibleLabel =
    !hideLabel && label !== undefined && label !== null && label !== "";
  const resolvedLabel = useMemo(
    () =>
      hasVisibleLabel ? (
        <>
          {typeof label === "string" ? label.replace(/\*+[\s]*$/, "") : label}
          {required && <span style={{ color: "#d32f2f" }}> *</span>}
        </>
      ) : undefined,
    [hasVisibleLabel, label, required],
  );

  useEffect(() => {
    if (!isBufferedInput) {
      setLocalValue(externalValue);
      return;
    }

    if (
      isFocusedRef.current &&
      externalValue !== lastEmittedValueRef.current
    ) {
      return;
    }

    setLocalValue((currentValue) =>
      currentValue === externalValue ? currentValue : externalValue,
    );
  }, [externalValue, isBufferedInput]);

  const emitChange = useCallback(
    (nextValue: string) => {
      if (!onChange) return;
      lastEmittedValueRef.current = nextValue;
      onChange({
        target: { value: nextValue },
        currentTarget: { value: nextValue },
      } as React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>);
    },
    [onChange],
  );

  useEffect(() => {
    if (
      !isBufferedInput ||
      isComposingRef.current ||
      localValue === externalValue
    ) {
      return;
    }

    const timer = window.setTimeout(() => {
      emitChange(localValue);
    }, effectiveDebounceMs);

    return () => window.clearTimeout(timer);
  }, [effectiveDebounceMs, emitChange, externalValue, isBufferedInput, localValue]);

  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      if (isAmountInput) {
        const nextValue = RemoveAmountCommas(e.target.value);
        if (!IsValidAmountInput(nextValue, amountDecimalPlaces, amountMaxDigits)) return;

        onChange?.({
          ...e,
          target: { ...e.target, value: nextValue },
          currentTarget: { ...e.currentTarget, value: nextValue },
        });
        return;
      }

      const nextValue = sanitizeInput ? sanitizeInput(e.target.value) : e.target.value;

      if (isBufferedInput) {
        setLocalValue(nextValue);
        return;
      }

      // Masked fields need the actual input for selection and caret restoration.
      if (!sanitizeInput) {
        onChange?.(e);
        return;
      }

      onChange?.({
        ...e,
        target: { ...e.target, value: nextValue },
        currentTarget: { ...e.currentTarget, value: nextValue },
      });
    },
    [isAmountInput, isBufferedInput, onChange, sanitizeInput],
  );

  const handleKeyDown = useCallback(
    (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (isAmountInput && !event.ctrlKey && !event.metaKey && !event.altKey && event.key.length === 1) {
        const input = event.target as HTMLInputElement;
        const nextValue = GetNextAmountInputValue(
          input.value,
          input.selectionStart,
          input.selectionEnd,
          event.key,
        );

        if (!IsValidAmountInput(nextValue, amountDecimalPlaces, amountMaxDigits)) {
          event.preventDefault();
          return;
        }
      }

      onKeyDown?.(event);
    },
    [amountDecimalPlaces, amountMaxDigits, isAmountInput, onKeyDown],
  );

  const handleBlur = useCallback(
    (e: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      isFocusedRef.current = false;

      if (isBufferedInput) {
        const target = e.target as HTMLInputElement | HTMLTextAreaElement;
        emitChange(target.value);
      }

      onBlur?.(e);
    },
    [emitChange, isBufferedInput, onBlur],
  );

  const handleCompositionStart = useCallback(
    (e: React.CompositionEvent<HTMLDivElement>) => {
      if (isBufferedInput) {
        isComposingRef.current = true;
      }

      onCompositionStart?.(e);
    },
    [isBufferedInput, onCompositionStart],
  );

  const handleCompositionEnd = useCallback(
    (e: React.CompositionEvent<HTMLDivElement>) => {
      if (isBufferedInput) {
        isComposingRef.current = false;
        const target = e.target as HTMLInputElement | HTMLTextAreaElement;
        const nextValue = target.value;
        setLocalValue(nextValue);
        emitChange(nextValue);
      }

      onCompositionEnd?.(e);
    },
    [emitChange, isBufferedInput, onCompositionEnd],
  );

  const mergedInputLabelProps = useMemo(
    () => ({
      shrink:
        hasVisibleLabel && (isBufferedInput ? localValue : value)
          ? true
          : undefined,
      ...InputLabelProps,
      sx: {
        fontWeight: required ? 700 : 600,
        fontSize: "0.85rem",
        "&.Mui-focused": {
          color: "var(--lms-primary)",
        },
        ...InputLabelProps?.sx,
      },
    }),
    [InputLabelProps, hasVisibleLabel, isBufferedInput, localValue, required, value],
  );

  const mergedInputProps = useMemo(
    () => ({
      ...InputProps,
      sx: {
        height: multiline ? "auto" : "40px",
        "& input": {
          padding: "12px",
          textAlign: isAmountInput ? "right" : undefined,
          color: "var(--lms-text-main)",
          WebkitTextFillColor: "var(--lms-text-main)",
          cursor: disabled ? "not-allowed" : undefined,
          "&::selection": {
            backgroundColor: "rgba(var(--lms-primary-rgb), 0.2)",
            color: "var(--lms-primary)",
          },
          "&::-webkit-datetime-edit-hour-field:focus, &::-webkit-datetime-edit-minute-field:focus, &::-webkit-datetime-edit-ampm-field:focus":
            {
              backgroundColor: "rgba(var(--lms-primary-rgb), 0.2)",
              color: "var(--lms-primary)",
            },
        },
        "& .MuiInputBase-input.Mui-disabled": {
          WebkitTextFillColor: "var(--lms-text-secondary)",
          cursor: "not-allowed",
        },
        ...InputProps?.sx,
      },
    }),
    [InputProps, disabled, multiline],
  );

  const mergedNativeInputProps = useMemo(
    () => ({
      ...inputProps,
      ...(nativeInputMode ? { inputMode: nativeInputMode } : {}),
      ...(maxLength !== undefined ? { maxLength } : {}),
      onPaste: (event: React.ClipboardEvent<HTMLInputElement>) => {
        if (isAmountInput) {
          const input = event.currentTarget;
          const nextValue = GetNextAmountInputValue(
            input.value,
            input.selectionStart,
            input.selectionEnd,
            event.clipboardData.getData("text"),
          );

          if (!IsValidAmountInput(nextValue, amountDecimalPlaces, amountMaxDigits)) {
            event.preventDefault();
            return;
          }
        }

        inputProps?.onPaste?.(event);
      },
      onMouseUp: (event: React.MouseEvent<HTMLInputElement>) => {
        if (isAmountInput && shouldSelectAmountOnFocusRef.current) {
          event.preventDefault();
          shouldSelectAmountOnFocusRef.current = false;
        }

        inputProps?.onMouseUp?.(event);
      },
    }),
    [amountDecimalPlaces, amountMaxDigits, inputProps, isAmountInput, maxLength, nativeInputMode],
  );

  const mergedSx = useMemo(
    () => ({
      "& .MuiOutlinedInput-root": {
        borderRadius: "9px",
        backgroundColor: disabled ? "#f5f5f5" : "#fff",
        cursor: disabled ? "not-allowed" : undefined,
        "&.Mui-disabled": {
          cursor: "not-allowed",
        },
        "& fieldset": {
          borderColor: "#e0e0e0",
          borderRadius: "9px",
        },
        "&:hover:not(.Mui-disabled) fieldset": {
          borderColor: "var(--lms-primary)",
        },
        "&.Mui-focused fieldset": {
          borderColor: "var(--lms-primary)",
          borderWidth: "2px",
        },
        "&.Mui-error fieldset": {
          borderColor: "#d32f2f !important",
        },
      },
      "& .MuiInputBase-root.Mui-disabled, & .MuiInputBase-input.Mui-disabled": {
        cursor: "not-allowed",
      },
      ...sx,
    }),
    [disabled, sx],
  );

  return (
    <TextField
      label={resolvedLabel}
      fullWidth={fullWidth}
      onKeyDown={handleKeyDown}
      onFocus={(e: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        isFocusedRef.current = true;
        if (isAmountInput && e.target.value.includes(",")) {
          emitChange(RemoveAmountCommas(e.target.value));
        }
        if (isAmountInput) {
          shouldSelectAmountOnFocusRef.current = true;
          window.setTimeout(() => {
            if (!shouldSelectAmountOnFocusRef.current) return;
            e.target.select();
            shouldSelectAmountOnFocusRef.current = false;
          }, 0);
        }
        onFocus?.(e);
      }}
      size={size}
      variant={variant}
      disabled={disabled}
      multiline={multiline}
      rows={rows}
      value={isBufferedInput ? localValue : value}
      onChange={handleChange}
      onBlur={handleBlur}
      onCompositionStart={handleCompositionStart}
      onCompositionEnd={handleCompositionEnd}
      InputLabelProps={mergedInputLabelProps}
      InputProps={mergedInputProps}
      inputProps={mergedNativeInputProps}
      sx={mergedSx}
      {...props}
    />
  );
};

const MemoizedFormInput = React.memo(FormInputImpl);

MemoizedFormInput.displayName = "MemoizedFormInput";

export const FormInput: React.FC<FormInputProps> = (props) => {
  const onChangeRef = useRef(props.onChange);
  const onBlurRef = useRef(props.onBlur);
  const onFocusRef = useRef(props.onFocus);
  const onCompositionStartRef = useRef(props.onCompositionStart);
  const onCompositionEndRef = useRef(props.onCompositionEnd);
  const onKeyDownRef = useRef(props.onKeyDown);

  useEffect(() => {
    onChangeRef.current = props.onChange;
  }, [props.onChange]);

  useEffect(() => {
    onBlurRef.current = props.onBlur;
  }, [props.onBlur]);

  useEffect(() => {
    onFocusRef.current = props.onFocus;
  }, [props.onFocus]);

  useEffect(() => {
    onCompositionStartRef.current = props.onCompositionStart;
  }, [props.onCompositionStart]);

  useEffect(() => {
    onCompositionEndRef.current = props.onCompositionEnd;
  }, [props.onCompositionEnd]);

  useEffect(() => {
    onKeyDownRef.current = props.onKeyDown;
  }, [props.onKeyDown]);

  const stableOnChange = useCallback(
    (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      onChangeRef.current?.(event);
    },
    [],
  );

  const stableOnBlur = useCallback(
    (event: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      onBlurRef.current?.(event);
    },
    [],
  );

  const stableOnFocus = useCallback(
    (event: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      onFocusRef.current?.(event);
    },
    [],
  );

  const stableOnCompositionStart = useCallback(
    (event: React.CompositionEvent<HTMLDivElement>) => {
      onCompositionStartRef.current?.(event);
    },
    [],
  );

  const stableOnCompositionEnd = useCallback(
    (event: React.CompositionEvent<HTMLDivElement>) => {
      onCompositionEndRef.current?.(event);
    },
    [],
  );

  const stableOnKeyDown = useCallback(
    (event: React.KeyboardEvent<HTMLDivElement>) => {
      onKeyDownRef.current?.(event);
    },
    [],
  );

  return (
    <MemoizedFormInput
      {...props}
      onChange={stableOnChange}
      onBlur={stableOnBlur}
      onFocus={stableOnFocus}
      onCompositionStart={stableOnCompositionStart}
      onCompositionEnd={stableOnCompositionEnd}
      onKeyDown={stableOnKeyDown}
    />
  );
};

FormInput.displayName = "FormInput";
