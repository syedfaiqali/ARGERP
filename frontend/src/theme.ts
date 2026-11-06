import { createTheme, decomposeColor } from '@mui/material/styles';
import { getLanguageFontFamily } from './constants/languages';

export type ApplicationTheme = 'green' | 'blue' | 'purple' | 'orange' | 'pink';

export const applicationThemes: Record<ApplicationTheme, { primary: string; primaryDark: string; primarySoft: string; sidebarBg: string }> = {
    green: {
        primary: '#6F9444',
        primaryDark: '#535F20',
        primarySoft: 'rgba(111, 148, 68, 0.12)',
        sidebarBg: 'linear-gradient(135deg, rgba(111, 148, 68, 0.08), rgba(83, 95, 32, 0.12))',
    },
    blue: {
        primary: '#2563EB',
        primaryDark: '#1D4ED8',
        primarySoft: 'rgba(37, 99, 235, 0.12)',
        sidebarBg: 'linear-gradient(135deg, rgba(37, 99, 235, 0.08), rgba(29, 78, 216, 0.12))',
    },
    purple: {
        primary: '#7C3AED',
        primaryDark: '#6D28D9',
        primarySoft: 'rgba(124, 58, 237, 0.12)',
        sidebarBg: 'linear-gradient(135deg, rgba(124, 58, 237, 0.08), rgba(109, 40, 217, 0.12))',
    },
    orange: {
        primary: '#EA580C',
        primaryDark: '#C2410C',
        primarySoft: 'rgba(234, 88, 12, 0.12)',
        sidebarBg: 'linear-gradient(135deg, rgba(234, 88, 12, 0.08), rgba(194, 65, 12, 0.12))',
    },
    pink: {
        primary: '#DB2777',
        primaryDark: '#BE185D',
        primarySoft: 'rgba(219, 39, 119, 0.12)',
        sidebarBg: 'linear-gradient(135deg, rgba(219, 39, 119, 0.08), rgba(190, 24, 93, 0.12))',
    },
};

export const getThemeVariables = (themeName: ApplicationTheme) => {
    const selectedTheme = applicationThemes[themeName];
    return {
        '--lms-primary': selectedTheme.primary,
        '--lms-primary-rgb': decomposeColor(selectedTheme.primary).values.join(', '),
        '--lms-primary-dark-rgb': decomposeColor(selectedTheme.primaryDark).values.join(', '),
        '--lms-primary-dark': selectedTheme.primaryDark,
        '--lms-primary-soft': selectedTheme.primarySoft,
        '--lms-primary-light': selectedTheme.primarySoft,
        '--lms-sidebar-bg': selectedTheme.sidebarBg,
        '--lms-scrollbar': selectedTheme.primarySoft,
    };
};

export const applyThemeVariables = (themeName: ApplicationTheme) => {
    Object.entries(getThemeVariables(themeName)).forEach(([name, value]) => {
        document.documentElement.style.setProperty(name, value);
    });
};

export const getTheme = (direction: 'ltr' | 'rtl', language: string, themeName: ApplicationTheme = 'green') => {
    const selectedTheme = applicationThemes[themeName];

    return createTheme({
        direction,
        palette: {
            primary: {
                main: selectedTheme.primary,
                dark: selectedTheme.primaryDark,
                contrastText: '#ffffff',
            },
            secondary: {
                main: '#6B7280', // Muted Gray
            },
            background: {
                default: '#FFFFFF',
                paper: '#FFFFFF',
            },
            text: {
                primary: '#1F2937', // Dark Charcoal
                secondary: '#6B7280', // Muted Gray
            },
            divider: '#F3F4F6', // Light Gray Border
        },
        typography: {
            fontFamily: getLanguageFontFamily(language),
            h4: {
                fontWeight: 700,
                color: '#1F2937',
            },
            subtitle1: {
                fontWeight: 700,
                color: '#1F2937',
            },
            subtitle2: {
                fontWeight: 600,
                color: '#1F2937',
            },
        },
        shape: {
            borderRadius: 8,
        },
        components: {
            MuiButton: {
                styleOverrides: {
                    root: {
                        textTransform: 'none',
                        fontWeight: 700,
                        borderRadius: 8,
                        transition: 'all 0.2s ease-in-out',
                    },
                    containedPrimary: {
                        backgroundColor: selectedTheme.primary,
                        boxShadow: 'none',
                        '&:hover': {
                            backgroundColor: selectedTheme.primaryDark,
                            boxShadow: `0 4px 12px ${selectedTheme.primarySoft}`,
                        },
                    },
                    outlinedPrimary: {
                        color: selectedTheme.primaryDark,
                        borderColor: selectedTheme.primary,
                        '&:hover': {
                            borderColor: selectedTheme.primaryDark,
                            backgroundColor: selectedTheme.primarySoft,
                        },
                    },
                },
            },
            MuiPaper: {
                styleOverrides: {
                    root: {
                        boxShadow: 'none',
                        border: '1px solid #F3F4F6',
                        borderRadius: 8,
                    },
                },
            },
            MuiDialog: {
                styleOverrides: {
                    paper: {
                        borderRadius: 12,
                        boxShadow: '0 10px 40px rgba(0,0,0,0.1)',
                    },
                },
            },
            MuiDialogTitle: {
                styleOverrides: {
                    root: {
                        padding: '16px 24px',
                        background: `linear-gradient(90deg, ${selectedTheme.primarySoft} 0%, rgba(255, 255, 255, 0) 100%)`,
                        borderBottom: '1px solid #F3F4F6',
                        '& .MuiTypography-root': {
                            fontSize: '1.1rem',
                            fontWeight: 700,
                            color: '#1F2937',
                        },
                    },
                },
            },
            MuiDialogActions: {
                styleOverrides: {
                    root: {
                        padding: '16px 24px',
                        backgroundColor: '#F9FAFB',
                        borderTop: '1px solid #F3F4F6',
                    },
                },
            },
            MuiTableHead: {
                styleOverrides: {
                    root: {
                        '& .MuiTableCell-root': {
                            backgroundColor: '#F9FAFB',
                            color: '#1F2937',
                            fontWeight: 700,
                            borderBottom: '2px solid #F3F4F6',
                        },
                    },
                },
            },
            MuiTableRow: {
                styleOverrides: {
                    root: {
                        '&:nth-of-type(even)': {
                            backgroundColor: '#FAFAFA',
                        },
                        '&:hover': {
                            backgroundColor: `${selectedTheme.primarySoft} !important`,
                        },
                    },
                },
            },
            MuiTableCell: {
                styleOverrides: {
                    root: {
                        borderColor: '#F3F4F6',
                        color: '#1F2937',
                        padding: '12px 16px',
                        textAlign: direction === 'rtl' ? 'right' : 'inherit',
                    },
                },
            },
            MuiTextField: { defaultProps: { InputLabelProps: { shrink: true } } },
            MuiOutlinedInput: {
                styleOverrides: {
                    root: {
                        borderRadius: 8,
                        backgroundColor: '#FAFAFA',
                        fontSize: '0.85rem',
                        transition: 'all 0.2s ease-in-out',
                        '& fieldset': {
                            borderColor: '#E5E7EB',
                        },
                        '&:hover fieldset': {
                            borderColor: `${selectedTheme.primary} !important`,
                        },
                        '&.Mui-focused fieldset': {
                            borderColor: `${selectedTheme.primary} !important`,
                            borderWidth: '1.5px !important',
                        },
                    },
                    input: {
                        padding: '10px 14px',
                        fontWeight: 500,
                    },
                },
                defaultProps: {
                    size: 'small',
                },
            },
            MuiSelect: {
                defaultProps: {
                    size: 'small',
                },
                styleOverrides: {
                    root: {
                        borderRadius: 8,
                        backgroundColor: '#FAFAFA',
                    },
                },
            },
        },
    });
};

