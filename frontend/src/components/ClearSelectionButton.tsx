import React from "react";
import { IconButton } from "@mui/material";
import { Clear as ClearIcon } from "@mui/icons-material";

/** The same clear action used by Employee Profile lookup fields. */
export const ClearSelectionButton = ({ onClear }: { onClear: (event: React.MouseEvent<HTMLButtonElement>) => void }) => (
  <IconButton
    size="small"
    disableRipple
    aria-label="Clear selection"
    sx={{ color: "var(--lms-text-secondary)", p: 0.25 }}
    onMouseDown={(event) => {
      event.preventDefault();
      event.stopPropagation();
    }}
    onKeyDown={(event) => event.stopPropagation()}
    onClick={(event) => {
      event.stopPropagation();
      onClear(event);
    }}
  >
    <ClearIcon sx={{ fontSize: "1.05rem" }} />
  </IconButton>
);
