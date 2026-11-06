import React, { useEffect, useState } from 'react';
import {
    Box,
    Grid,
    Button,
    Typography,
    Collapse,
    IconButton,
} from '@mui/material';
import {
    Settings as SettingsIcon,
    ClearAll as ClearFilterIcon,
    Add as AddIcon,
    Remove as RemoveIcon,
    Assessment as AssessmentIcon,
    ArrowBack as ArrowBackIcon,
} from '@mui/icons-material';
import { T, useTranslation } from '@/components/AdaptiveTranslationProvider';
import { FastReportViewer } from '@/components/reporting/FastReportViewer';

interface BaseReportModalProps {
    open: boolean;
    onClose: () => void;
    title: string;
    reportName: string;
    parameters?: any;
    baseUrl?: string;
    onGenerate?: () => any;
    onClear?: () => void;
    children: React.ReactNode;
    hideHeader?: boolean;
    isFullScreenView?: boolean;
    /** Start with filters hidden when a report is opened for one known record. */
    defaultCriteriaExpanded?: boolean;
}

export const BaseReportModal: React.FC<BaseReportModalProps> = ({
    open,
    onClose,
    title,
    reportName,
    baseUrl,
    onGenerate,
    onClear,
    children,
    hideHeader = false,
    isFullScreenView = false,
    defaultCriteriaExpanded = true,
}) => {
    const { translate } = useTranslation();
    const [criteriaExpanded, setCriteriaExpanded] = useState(defaultCriteriaExpanded);
    const [viewerExpanded, setViewerExpanded] = useState(false);
    const [reportParams, setReportParams] = useState<any>(null);
    const [reportRequestKey, setReportRequestKey] = useState(0);

    useEffect(() => {
        if (open) setCriteriaExpanded(defaultCriteriaExpanded);
    }, [open, defaultCriteriaExpanded]);

    const handleGenerate = () => {
        const params = onGenerate ? onGenerate() : {};
        if (params) {
            setReportParams(params);
            setReportRequestKey((current) => current + 1);
            setCriteriaExpanded(false);
            setViewerExpanded(true);
        }
    };

    const handleClear = () => {
        if (onClear) onClear();
        setReportParams(null);
        setViewerExpanded(false);
        setCriteriaExpanded(true);
    };

    const renderHeader = () => (
        <Box sx={{
            p: 2,
            px: 3,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            bgcolor: 'transparent'
        }}>
            <Typography variant="h4" sx={{ fontWeight: 900, color: '#1e293b' }}>
                <T>{title}</T>
            </Typography>
            <Button
                onClick={onClose}
                variant="text"
                startIcon={<ArrowBackIcon />}
                sx={{
                    color: 'var(--lms-primary)',
                    textTransform: 'none',
                    fontWeight: 800,
                    fontSize: '0.95rem',
                    '&:hover': { bgcolor: 'transparent', color: 'var(--lms-primary-dark)' }
                }}
            >
                <T>Back to List</T>
            </Button>
        </Box>
    );

    const renderMainContent = () => (
        <Box sx={{ p: 3, pt: 1 }}>
            <Box sx={{
                display: 'flex',
                gap: 2,
                mb: 3,
                p: 2,
                bgcolor: '#fff',
                borderRadius: '16px',
                border: '1px solid #e2e8f0',
                alignItems: 'center',
                boxShadow: '0 1px 3px 0 rgb(0 0 0 / 0.05)'
            }}>
                <Button
                    variant="contained"
                    onClick={handleGenerate}
                    startIcon={<SettingsIcon />}
                    sx={{
                        textTransform: 'none',
                        fontWeight: 700,
                        bgcolor: 'var(--lms-primary)',
                        borderRadius: '10px',
                        px: 3,
                        py: 1,
                        '&:hover': { bgcolor: 'var(--lms-primary-dark)' }
                    }}
                >
                    <T>Generate</T>
                </Button>
                <Button
                    onClick={handleClear}
                    startIcon={<ClearFilterIcon />}
                    sx={{
                        textTransform: 'none',
                        fontWeight: 700,
                        color: '#64748b',
                        '&:hover': { bgcolor: '#f1f5f9' },
                        borderRadius: '10px'
                    }}
                >
                    <T>Clear Filter</T>
                </Button>
            </Box>
            <Box sx={{ border: '1px solid #e2e8f0', borderRadius: '12px', overflow: 'hidden', bgcolor: '#fff' }}>
                <Box
                    onClick={() => setCriteriaExpanded(!criteriaExpanded)}
                    sx={{
                        p: 2,
                        bgcolor: '#f8fafc',
                        borderBottom: criteriaExpanded ? '1px solid #e2e8f0' : 'none',
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        cursor: 'pointer'
                    }}
                >
                    <Typography sx={{ fontSize: '0.9rem', fontWeight: 800, color: '#334155' }}>
                        <T>Report Criteria</T>
                    </Typography>
                    <IconButton size="small">
                        {criteriaExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}
                    </IconButton>
                </Box>

                <Collapse in={criteriaExpanded}>
                    <Box sx={{ p: 4 }}>
                        <Grid container spacing={3} sx={{ maxWidth: '800px' }}>
                            {children}
                        </Grid>
                    </Box>
                </Collapse>

                <Box
                    onClick={() => setViewerExpanded(!viewerExpanded)}
                    sx={{
                        p: 2,
                        bgcolor: '#f8fafc',
                        borderTop: '1px solid #e2e8f0',
                        borderBottom: viewerExpanded ? '1px solid #e2e8f0' : 'none',
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        cursor: 'pointer'
                    }}
                >
                    <Typography sx={{ fontSize: '0.9rem', fontWeight: 800, color: '#334155' }}>
                        <T>Report Viewer</T>
                    </Typography>
                    <IconButton size="small">
                        {viewerExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}
                    </IconButton>
                </Box>

                <Collapse in={viewerExpanded}>
                    <Box sx={{ height: '800px', bgcolor: '#f1f5f9', overflowX: 'auto', overflowY: 'hidden' }}>
                        {reportParams ? (
                            <FastReportViewer
                                reportName={reportName}
                                parameters={reportParams}
                                baseUrl={baseUrl}
                                requestKey={reportRequestKey}
                            />
                        ) : (
                            <Box sx={{
                                height: '100%',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                color: '#94a3b8',
                                flexDirection: 'column',
                                gap: 2
                            }}>
                                <AssessmentIcon sx={{ fontSize: '4rem', opacity: 0.1 }} />
                                <Typography sx={{ fontWeight: 600 }}>
                                    <T>Click Generate to preview report</T>
                                </Typography>
                            </Box>
                        )}
                    </Box>
                </Collapse>
            </Box>
        </Box>
    );

    return (
        <Box sx={{ minHeight: isFullScreenView ? '100%' : 'auto', bgcolor: '#f1f5f9' }}>
            {!hideHeader && renderHeader()}
            {renderMainContent()}
        </Box>
    );
};

export const ReportFilterField: React.FC<{ label: string; children: React.ReactNode }> = ({ label, children }) => {
    return (
        <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
            <Typography sx={{
                width: '110px',
                flexShrink: 0,
                mr: 2,
                fontWeight: 600,
                color: '#475569',
                fontSize: '0.9rem'
            }}>
                <T>{label}</T>
            </Typography>
            {children}
        </Grid>
    );
};
