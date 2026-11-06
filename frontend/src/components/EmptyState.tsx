import React from "react";
import { Box, Typography } from "@mui/material";
import { SearchOffOutlined as SearchOffIcon } from "@mui/icons-material";
import { T } from "@/components/AdaptiveTranslationProvider";

interface EmptyStateProps {
  title?: string;
  description?: string;
  icon?: React.ReactNode;
  compact?: boolean;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  title = "No Records Found",
  description = "No records are available at the moment.",
  icon,
  compact = false,
}) => {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        gap: compact ? 1 : 1.4,
        px: 3,
        py: compact ? 1 : 2,
      }}
    >
      {icon ?? (
        <SearchOffIcon sx={{ fontSize: compact ? "3rem" : "4rem", color: "var(--lms-primary)" }} />
      )}
      <Typography
        sx={{
          fontSize: compact ? "0.95rem" : "1rem",
          fontWeight: 700,
          color: "var(--lms-primary)",
        }}
      >
        <T>{title}</T>
      </Typography>
      <Typography
        sx={{
          fontSize: "0.8rem",
          fontWeight: 500,
          color: "var(--lms-primary)",
          textAlign: "center",
        }}
      >
        <T>{description}</T>
      </Typography>
    </Box>
  );
};

