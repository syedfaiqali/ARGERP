import React from "react";
import { Box, Tabs, Tab } from "@mui/material";
import { styled } from "@mui/material/styles";

// Modern, Simple Tabs Container
const StyledTabsList = styled(Tabs)(() => ({
  minHeight: "48px",
  position: "relative",
  borderBottom: "1px solid rgba(0, 0, 0, 0.08)",

  "& .MuiTabs-indicator": {
    height: "3px",
    borderRadius: "3px 3px 0 0",
    backgroundColor: "var(--lms-primary)",
    transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
  },

  "& .MuiTabs-flexContainer": {
    gap: "0px",
    position: "relative",
    zIndex: 1,
  },

}));

// Simple Tab with clean transition
const StyledTabItem = styled(Tab)(() => ({
  textTransform: "none",
  fontWeight: 500,
  fontSize: "0.95rem",
  minHeight: "48px",
  padding: "12px 18px",
  color: "#64748b",
  transition: "all 0.2s ease",
  position: "relative",
  opacity: 0.8,

  "& .MuiTab-iconWrapper": {
    marginRight: "8px",
    fontSize: "1.1rem",
    transition: "transform 0.2s ease",
  },

  "&:hover": {
    color: "var(--lms-primary)",
    opacity: 1,
    backgroundColor: "rgba(var(--lms-primary-rgb), 0.04)",
    "& .MuiTab-iconWrapper": {
      transform: "translateY(-1px)",
    },
  },

  "&.Mui-selected": {
    color: "var(--lms-primary) !important",
    fontWeight: 700,
    opacity: 1,
  },

  "&:active": {
    backgroundColor: "rgba(var(--lms-primary-rgb), 0.08)",
  },

  "&.Mui-disabled": {
    opacity: 0.3,
    cursor: "not-allowed",
  },

  "@media (max-width: 600px)": {
    minHeight: "44px",
    padding: "10px 14px",
    fontSize: "0.85rem",
  },
}));

// Tab Panel component for content with slide animation
interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
  padding?: number | string;
  keepMounted?: boolean;
}

export const TabPanel: React.FC<TabPanelProps> = ({
  children,
  value,
  index,
  padding = 3,
  keepMounted = false,
}) => {
  const [visited, setVisited] = React.useState(value === index);
  React.useEffect(() => {
    if (keepMounted && value === index) setVisited(true);
  }, [keepMounted, value, index]);
  return (
    <Box
      role="tabpanel"
      hidden={value !== index}
      id={`tabpanel-${index}`}
      aria-labelledby={`tab-${index}`}
      sx={{
        p: padding,
        animation:
          value === index
            ? "slideIn 0.4s cubic-bezier(0.34, 1.56, 0.64, 1)"
            : "none",
        "@keyframes slideIn": {
          from: {
            opacity: 0,
            transform: "translateX(-20px)",
          },
          to: {
            opacity: 1,
            transform: "translateX(0)",
          },
        },
      }}
    >
      {(value === index || (keepMounted && visited)) && children}
    </Box>
  );
};

// Main StyledTabs component
export interface StyledTabsProps {
  value: number;
  onChange: (event: React.SyntheticEvent, newValue: number) => void;
  tabs: Array<{
    label: React.ReactNode;
    icon?: React.ReactElement;
    disabled?: boolean;
  }>;
  children?: React.ReactNode;
  variant?: "standard" | "scrollable" | "fullWidth";
  centered?: boolean;
  elevation?: number;
  borderRadius?: string | number;
  scrollButtons?: boolean | "auto";
  allowScrollButtonsMobile?: boolean;
  wrapTabs?: boolean;
  orientation?: "horizontal" | "vertical";
  isCollapsed?: boolean;
  sx?: any;
}

export const StyledTabs: React.FC<StyledTabsProps> = ({
  value,
  onChange,
  tabs,
  children,
  variant = "standard",
  centered = false,
  elevation = 0,
  borderRadius = "20px",
  scrollButtons = false,
  allowScrollButtonsMobile = false,
  wrapTabs = false,
  orientation = "horizontal",
  isCollapsed = false,
  sx: customSx = {},
}) => {
  const isVertical = orientation === "vertical";

  return (
    <Box
      sx={{
        display: isVertical ? "flex" : "block",
        flexDirection: isVertical ? "row" : "column",
        mb: isVertical ? 0 : 3,
        width: "100%",
        maxWidth: "100%",
        minWidth: 0,
        minHeight: isVertical ? 0 : undefined,
        overflow: isVertical ? "hidden" : "visible",
        ...customSx
      }}
    >
      <StyledTabsList
        value={value}
        onChange={onChange}
        variant={variant}
        centered={centered}
        orientation={orientation}
        scrollButtons={scrollButtons}
        allowScrollButtonsMobile={allowScrollButtonsMobile}
        sx={{
          ...(wrapTabs && !isVertical
            ? {
              minHeight: "auto",
              borderBottom: "none",
              "& .MuiTabs-indicator": {
                display: "none"
              },
              "& .MuiTabs-flexContainer, & .MuiTabs-list": {
                flexWrap: "wrap",
                gap: "8px",
              },
            }
            : {}),
          ...(isVertical
            ? {
              minHeight: "auto",
              height: "100%",
              maxHeight: "100%",
              background: "transparent",
              borderRight: "1px solid rgba(0, 0, 0, 0.08)",
              borderBottom: "none",
              borderRadius: 0,
              padding: isCollapsed ? "10px 4px" : "10px",
              width: isCollapsed ? 64 : { xs: 76, md: 280 },
              maxWidth: isCollapsed ? 64 : { xs: 76, md: 280 },
              flexShrink: 0,
              "& .MuiTabs-indicator": {
                left: "auto",
                right: 0,
                width: "3px",
                height: "auto",
                borderRadius: "3px 0 0 3px",
                display: isCollapsed ? "none" : "block", // Optional: hide indicator when collapsed
              },
              "& .MuiTabs-flexContainer": {
                flexDirection: "column",
                gap: "4px",
              },
              "& .MuiTabs-scroller": {
                overflowX: "hidden !important",
                overflowY: "auto !important",
                margin: 0,
                padding: 0,
                "&::-webkit-scrollbar": {
                  width: "4px",
                },
                "&::-webkit-scrollbar-thumb": {
                  backgroundColor: "rgba(var(--lms-primary-rgb), 0.3)",
                  borderRadius: "4px",
                },
                "&::-webkit-scrollbar-track": {
                  backgroundColor: "transparent",
                },
              }
            }
            : {}),
        }}
      >
        {tabs.map((tab, index) => (
          <StyledTabItem
            key={index}
            label={isCollapsed ? undefined : tab.label}
            icon={tab.icon}
            iconPosition="start"
            disabled={tab.disabled}
            id={`tab-${index}`}
            aria-controls={`tabpanel-${index}`}
            sx={isVertical ? {
              justifyContent: isCollapsed ? "center" : { xs: "center", md: "flex-start" },
              textAlign: isCollapsed ? "center" : "left",
              minHeight: "48px",
              minWidth: isCollapsed ? "48px" : 0,
              width: "100%",
              px: isCollapsed ? 1 : 2,
              borderRadius: "8px",
              "& .MuiTab-iconWrapper": {
                marginRight: isCollapsed ? 0 : { xs: 0, md: "8px" },
                fontSize: "1.2rem",
              },
              "& .MuiTab-wrapper, & .MuiTab-iconWrapper + *": {
                minWidth: 0,
              },
              "& .MuiTab-iconWrapper ~ span, & .MuiTab-iconWrapper + span": {
                display: { xs: "none", md: "inline-flex" },
              },
              "& .MuiTab-root": {
                 minWidth: 0,
              },
              "&.Mui-selected": {
                backgroundColor: "rgba(var(--lms-primary-rgb), 0.06)",
              },
            } : {
              borderRadius: "8px 8px 0 0",
            }}
          />
        ))}
      </StyledTabsList>
      {isVertical ? <Box sx={{ flexGrow: 1, minWidth: 0, maxWidth: "100%", minHeight: 0, bgcolor: '#fff', display: 'flex', flexDirection: 'column', overflow: "hidden" }}>{children}</Box> : children}
    </Box>
  );
};

// Compact variant for smaller spaces
export const CompactStyledTabs: React.FC<StyledTabsProps> = (props) => {
  return <StyledTabs {...props} borderRadius="16px" />;
};

// Card variant with elevation
export const CardStyledTabs: React.FC<StyledTabsProps> = (props) => {
  return <StyledTabs {...props} elevation={3} />;
};
