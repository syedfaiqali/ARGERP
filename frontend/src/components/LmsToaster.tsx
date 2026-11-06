import React from "react";
import { Snackbar, Alert, Box } from "@mui/material";
import type { SxProps, Theme } from "@mui/material/styles";

interface LmsToasterProps {
  open: boolean;
  onClose: () => void;
  messages: string[];
  severity?: "error" | "success" | "info" | "warning";
  alertSx?: SxProps<Theme>;
  disableDeleteSuccessAutoSeverity?: boolean;
}

export const LmsToaster: React.FC<LmsToasterProps> = ({
  open,
  onClose,
  messages = [],
  severity,
  alertSx,
  disableDeleteSuccessAutoSeverity = false,
}) => {
  const baseAlertSx: SxProps<Theme> = {
    width: "100%",
    boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
  };
  const safeMessages = Array.isArray(messages) ? messages : [];
  const normalizedMessages = safeMessages.map((m) =>
    typeof m === "string" ? m.toLowerCase() : "",
  );
  const hasSuccessMessage = normalizedMessages.some(
    (m) =>
      m &&
      !m.includes("deleted") &&
      !m.includes("delete") &&
      (m.includes("successfully") ||
        m.includes("successful") ||
        m.includes("created") ||
        m.includes("updated") ||
        m.includes("added") ||
        m.includes("saved")),
  );

  // Message wording must never override an explicit validation/API severity.
  const resolvedSeverity = severity ??
    (!disableDeleteSuccessAutoSeverity && hasSuccessMessage ? "success" : "error");

  return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      onClose={onClose}
      anchorOrigin={{ vertical: "top", horizontal: "right" }}
    >
      <Alert
        onClose={onClose}
        severity={resolvedSeverity}
        sx={alertSx ? ([baseAlertSx, alertSx] as any) : baseAlertSx}
      >
        <Box component="ul" sx={{ m: 0, pl: 2 }}>
          {safeMessages.map((msg, idx) => (
            <li key={idx} style={{ fontSize: "0.85rem" }}>
              {typeof msg === "string" ? msg : String(msg)}
            </li>
          ))}
        </Box>
      </Alert>
    </Snackbar>
  );
};
