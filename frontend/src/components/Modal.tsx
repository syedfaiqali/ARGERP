import React from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  Typography,
  Box,
  Button,
} from "@mui/material";
import {
  Close as CloseIcon,
  Add as AddIcon,
  Save as SaveIcon,
  ChevronRight as ChevronRightIcon,
  Fullscreen as FullscreenIcon,
  FullscreenExit as FullscreenExitIcon,
} from "@mui/icons-material";
import { FormThemeWrapper } from "./FormThemeWrapper";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";

interface ModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  breadcrumbPath?: string;
  maxWidth?: "xs" | "sm" | "md" | "lg" | "xl";
  children: React.ReactNode;
  onSave?: () => void;
  onSaveAndClose?: () => void;
  onReset?: () => void;
  showActions?: boolean;
  showSaveButton?: boolean;
  saveLabel?: string;
  saveAndCloseLabel?: string;
  saveAndCloseVariant?: 'contained' | 'outlined';
  resetLabel?: string;
  resetIcon?: React.ReactNode;
  hideResetIcon?: boolean;
  hideResetButton?: boolean;
  contentPadding?: string | number;
  headerBgColor?: string;
  headerTextColor?: string;
  onMaximize?: () => void;
  isMaximized?: boolean;
  saveDisabled?: boolean;
  saveAndCloseDisabled?: boolean;
  cancelDisabled?: boolean;
  disableCloseIcon?: boolean;
  disableBackdropClose?: boolean;
  disableEscapeKeyDown?: boolean;
  disableContentScroll?: boolean;
  /** Render as a routed, full-page workspace instead of a centered dialog. */
  fullScreen?: boolean;
  /** Remove the close affordance when the header provides its own navigation. */
  hideCloseIcon?: boolean;
  /** Optional controls rendered in the header before the window controls. */
  headerActions?: React.ReactNode;
  /** Optional fixed footer controls. Replaces the standard modal actions. */
  footerActions?: React.ReactNode;
  /** Per-dialog Paper sizing/layout overrides for workspace-sized dialogs. */
  paperSx?: import("@mui/material").SxProps<import("@mui/material").Theme>;
}

export const Modal: React.FC<ModalProps> = ({
  open,
  onClose,
  title,
  breadcrumbPath,
  maxWidth = "md",
  children,
  onSave,
  onSaveAndClose,
  onReset,
  showActions = true,
  showSaveButton = true,
  saveLabel,
  saveAndCloseLabel,
  saveAndCloseVariant = 'contained',
  resetLabel,
  resetIcon,
  hideResetIcon = false,
  hideResetButton = false,
  contentPadding = 4,
  headerBgColor = "var(--lms-primary-soft)",
  headerTextColor = "var(--lms-primary-dark)",
  onMaximize,
  isMaximized = false,
  saveDisabled = false,
  saveAndCloseDisabled = false,
  cancelDisabled = false,
  disableCloseIcon = false,
  disableBackdropClose = false,
  disableEscapeKeyDown = false,
  disableContentScroll = false,
  fullScreen = false,
  hideCloseIcon = false,
  headerActions,
  footerActions,
  paperSx,
}) => {
  const { translate } = useTranslation();
  return (
    <Dialog
      open={open}
      disableEscapeKeyDown={disableEscapeKeyDown}
      fullScreen={fullScreen}
      onClose={(_, reason) => {
        if (disableBackdropClose && reason === "backdropClick") return;
        onClose();
      }}
      maxWidth={maxWidth}
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: "24px",
          // The dialog owns its layout; individual content (such as a data table)
          // can opt in to being its single scroll container.
          overflow: "hidden",
          display: "flex",
          flexDirection: "column",
          boxShadow: "0 25px 50px -12px rgba(0, 0, 0, 0.15)",
          transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
          ...(fullScreen && {
            borderRadius: 0,
            width: "100vw",
            height: "100vh",
            maxWidth: "none",
            maxHeight: "none",
          }),
          ...(isMaximized && {
            width: "96vw",
            height: "96vh",
            maxWidth: "none",
            margin: "auto",
            resize: "both",
          }),
          ...paperSx,
        },
      }}
    >
      <DialogTitle
        sx={{
          bgcolor: headerBgColor,
          color: headerTextColor,
          py: 2.5,
          px: 3,
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          borderBottom: "1px solid var(--lms-border)",
        }}
      >
        <Typography variant="h6" component="span" sx={{ fontWeight: 800, fontSize: "1.25rem" }}>
          {title}
        </Typography>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          {headerActions}
          {onMaximize && (
            <IconButton
              size="small"
              onClick={onMaximize}
              sx={{
                color: "var(--lms-text-secondary)",
                bgcolor: "rgba(0,0,0,0.03)",
                "&:hover": { bgcolor: "rgba(0,0,0,0.06)" },
              }}
            >
              {isMaximized ? <FullscreenExitIcon fontSize="small" /> : <FullscreenIcon fontSize="small" />}
            </IconButton>
          )}
          {!hideCloseIcon && <IconButton
            size="small"
            onClick={onClose}
            disabled={disableCloseIcon}
            sx={{
              color: "var(--lms-text-secondary)",
              bgcolor: "rgba(0,0,0,0.03)",
              "&:hover": { bgcolor: "rgba(0,0,0,0.06)" },
            }}
          >
            <CloseIcon fontSize="small" />
          </IconButton>}
        </Box>
      </DialogTitle>

      {breadcrumbPath && (
        <Box
          sx={{
            px: 3,
            py: 1.2,
            borderBottom: "1px solid #F3F4F6",
            bgcolor: "#fff",
          }}
        >
          <Typography
            sx={{
              fontSize: "0.75rem",
              fontWeight: 700,
              color: "var(--lms-primary)",
              letterSpacing: "0.01em",
            }}
          >
            {breadcrumbPath}
          </Typography>
        </Box>
      )}

      <DialogContent sx={{ p: 0, bgcolor: "#fff", display: "flex", flexDirection: "column", flex: 1, minHeight: 0, overflowY: disableContentScroll ? "hidden" : "auto" }}>
        <FormThemeWrapper>
          <Box sx={{ px: contentPadding, py: contentPadding === 0 ? 0 : 3, display: "flex", flexDirection: "column", flex: 1, minHeight: 0 }}>{children}</Box>
        </FormThemeWrapper>
      </DialogContent>

      {footerActions ? (
        <DialogActions
          sx={{
            p: 2,
            px: 4,
            bgcolor: "#fff",
            borderTop: "1px solid var(--lms-border)",
            display: "flex",
            justifyContent: "flex-end",
            gap: 1,
            flexShrink: 0,
          }}
        >
          {footerActions}
        </DialogActions>
      ) : showActions && (
        <DialogActions
          sx={{
            p: 2,
            px: 4,
            bgcolor: "rgba(0,0,0,0.02)",
            borderTop: "1px solid var(--lms-border)",
            display: "flex",
            alignItems: "center",
            justifyContent: hideResetButton ? "flex-end" : "space-between",
            gap: 1.5,
          }}
        >
          {!hideResetButton && (
             <Button
               startIcon={hideResetIcon ? null : (resetIcon || <AddIcon />)}
               variant="outlined"
               onClick={onReset}
               sx={{
                 borderRadius: "10px",
                 textTransform: "none",
                 fontWeight: 700,
                 border: "1px solid var(--lms-primary)",
                 borderColor: "var(--lms-primary)",
                 color: "var(--lms-primary)",
                 px: 2,
                 transition: "all 0.2s ease",
                 "&:hover": {
                   bgcolor: "var(--lms-primary)",
                   border: "1px solid var(--lms-primary)",
                   borderColor: "var(--lms-primary)",
                   color: "#fff",
                   boxShadow: "0 2px 10px var(--lms-primary)",
                 },
               }}
             >
               {resetLabel || translate("Reset Form")}
             </Button>
          )}

          <Box sx={{ display: "flex", gap: 1.5, alignItems: "center" }}>
            <Button
              variant="outlined"
              onClick={onClose}
              disabled={cancelDisabled}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
                border: "1px solid var(--lms-primary)",
                borderColor: "var(--lms-primary)",
                color: "var(--lms-primary)",
                px: 3,
                transition: "all 0.2s ease",
                "&:hover": {
                  border: "1px solid var(--lms-primary)",
                  borderColor: "var(--lms-primary)",
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
                  color: "var(--lms-primary-dark, var(--lms-primary))",
                },
              }}
            >
              <T>Cancel</T>
            </Button>
            {showSaveButton && (
              <Button
                variant="outlined"
                startIcon={<SaveIcon sx={{ fontSize: "1.1rem" }} />}
                onClick={onSave}
                disabled={saveDisabled}
                sx={{
                  borderRadius: "10px",
                  textTransform: "none",
                  fontWeight: 700,
                  border: "1px solid var(--lms-primary)",
                  borderColor: "var(--lms-primary)",
                  color: "var(--lms-primary)",
                  px: 3,
                  transition: "all 0.2s ease",
                  "&:hover": {
                    border: "1px solid var(--lms-primary)",
                    borderColor: "var(--lms-primary)",
                    bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
                    color: "var(--lms-primary-dark, var(--lms-primary))",
                  },
                }}
              >
                {saveLabel || translate("Save")}
              </Button>
            )}
            <Button
              variant={saveAndCloseVariant}
              disableElevation
              startIcon={saveAndCloseVariant === 'contained' ? <ChevronRightIcon /> : undefined}
              onClick={onSaveAndClose || onSave}
              disabled={saveAndCloseDisabled}
              sx={{
                borderRadius: "10px",
                textTransform: "none",
                fontWeight: 700,
                bgcolor: "var(--lms-primary)",
                color: "#fff",
                px: 3,
                transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                boxShadow: "0 2px 4px rgba(var(--lms-primary-rgb), 0.15)",
                "&:hover": {
                  bgcolor: "var(--lms-primary)",
                  // transform: "translateY(-2px)",
                  boxShadow: "0 2px 10px var(--lms-primary)",
                },
                "&:active": {
                  transform: "translateY(0)",
                },
                ...(saveAndCloseVariant === 'outlined' && {
                  bgcolor: 'transparent',
                  color: 'var(--lms-primary)',
                  border: '1px solid var(--lms-primary)',
                  boxShadow: 'none',
                  transition: 'all 0.2s ease',
                  '&:hover': {
                    border: '1px solid var(--lms-primary)',
                    bgcolor: 'rgba(var(--lms-primary-rgb), 0.1)',
                    color: 'var(--lms-primary-dark, var(--lms-primary))',
                    boxShadow: 'none',
                  },
                }),
              }}
            >
              {saveAndCloseLabel || translate("Save & Close")}
            </Button>
          </Box>
        </DialogActions>
      )}
    </Dialog>
  );
};
