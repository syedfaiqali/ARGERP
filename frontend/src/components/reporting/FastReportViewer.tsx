import Select from "@/components/ClearableSelect";
import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Box, CircularProgress, Typography, Button, MenuItem } from "@mui/material";
import DownloadIcon from '@mui/icons-material/Download';
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined';
import {hrReportBlob} from '@/services/hrReportService';
import { T } from '@/components/AdaptiveTranslationProvider';

const REPORT_EXPORT_FORMATS = [{label:'PDF',value:'pdf'},{label:'XLSX',value:'xlsx'}] as const;

const NO_DATA_TO_PRINT_MESSAGE = 'No Data To Print';

// React StrictMode mounts a viewer twice in development. Report generation is
// expensive, so viewers for the same request share one in-flight HTTP call.
// The entry is removed after it settles; a later explicit Generate still gets
// a fresh report through its incremented requestKey.
const inFlightReportRequests = new Map<string, Promise<Blob>>();

interface FastReportViewerProps {
    /** Name of the report file without .frx extension */
    reportName: string;
    /** Dynamic parameters to filter the report data */
    parameters?: Record<string, any>;
    /** Optional base URL for the reporting API */
    baseUrl?: string;
    /** Set false for report-specific APIs that expose /view without /{reportName}. */
    includeReportNameInPath?: boolean;
    /** Optional title for the viewer */
    title?: React.ReactNode;
    /** Callback when the viewer should be closed */
    onClose?: () => void;
    /** Increments when the caller explicitly asks to generate the same report again. */
    requestKey?: string | number;
}

/**
 * Professional FastReport Web Viewer Component.
 * Features: Responsive UI, PDF Preview, Robust Blob Handling, Authentication support.
 */
export const FastReportViewer: React.FC<FastReportViewerProps> = ({
    reportName,
    parameters = {},
    baseUrl = '/api/HumanResource/Reports/v3',
    includeReportNameInPath = true,
    requestKey,
}) => {
    const [loading, setLoading] = useState(true);
    const [exporting, setExporting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [exportError, setExportError] = useState<string | null>(null);
    const [noDataMessage, setNoDataMessage] = useState<string | null>(null);
    const [reportUrl, setReportUrl] = useState<string | null>(null);
    const [exportFormat, setExportFormat] = useState<(typeof REPORT_EXPORT_FORMATS)[number]['value']>('pdf');
    const fetchedRequestRef = useRef<string | null>(null);
    const parameterSignature = JSON.stringify(parameters ?? {});

    const buildQueryParams = useCallback(() => {
        const queryParams: Record<string, any> = {};
        if (parameters) {
            Object.entries(parameters).forEach(([key, value]) => {
                if (value !== undefined && value !== null && value !== '') {
                    queryParams[`parameters[${key}]`] = value;
                }
            });
        }
        return queryParams;
    // The object itself is often recreated by parent renders.  Its serialized
    // content is the actual request dependency, preventing duplicate GETs.
    }, [parameterSignature]);

    const readBlobMessage = async (blob: Blob) => {
        const text = await blob.text();
        if (!text) return '';

        if (blob.type.includes('json')) {
            try {
                const json = JSON.parse(text);
                if (json.isNoData === true) {
                    return NO_DATA_TO_PRINT_MESSAGE;
                }
                if (isNoDataMessage(String(json.detail || json.message || json.error || ''))) {
                    return NO_DATA_TO_PRINT_MESSAGE;
                }
                return json.detail || json.message || json.error || text;
            } catch {
                return text;
            }
        }

        return text;
    };

    const isNoDataMessage = (message: string) =>
        message.toLowerCase().includes(NO_DATA_TO_PRINT_MESSAGE.toLowerCase());

    const fetchReport = useCallback(async () => {
        if (!reportName) return;

        setLoading(true);
        setError(null);
        setNoDataMessage(null);
        try {
            // Mapping parameters to the format expected by the backend
            const queryParams = buildQueryParams();

            const trimmedBaseUrl = baseUrl.replace(/\/+$/, '');
            const reportPath = includeReportNameInPath ? `/view/${encodeURIComponent(reportName)}` : '/view';
            const requestUrl = `${trimmedBaseUrl}${reportPath}`;
            const requestCacheKey = `${requestUrl}|${JSON.stringify(queryParams)}|${requestKey ?? ''}`;
            let request = inFlightReportRequests.get(requestCacheKey);

            if (!request) {
                request = hrReportBlob(reportName, queryParams);
                inFlightReportRequests.set(requestCacheKey, request);
                void request.then(
                    () => inFlightReportRequests.delete(requestCacheKey),
                    () => inFlightReportRequests.delete(requestCacheKey),
                );
            }

            // CRITICAL: Check if the response is actually a PDF. 
            // If the server returns HTML (like a redirect to home page), the type will be text/html.
            // If it returns JSON, it's likely an error message from the controller.
            const blob = await request;
            
            if (blob.type.includes('json') || blob.type.startsWith('text/')) {
                const message = await readBlobMessage(blob);
                if (isNoDataMessage(message)) {
                    setNoDataMessage(NO_DATA_TO_PRINT_MESSAGE);
                    return;
                }
                throw new Error(message || 'Server error during report generation');
            }

            if (blob.type === 'text/html' || blob.type.includes('html')) {
                console.error('Expected PDF but received HTML content. This usually indicates a redirect or 404.');
                throw new Error('Failed to generate report: Server returned HTML instead of PDF. Please check your parameters.');
            }

            const finalBlob = blob.type === 'application/pdf' ? blob : new Blob([blob], { type: 'application/pdf' });

            // Revoke old URL if it exists to prevent memory leaks
            if (reportUrl && reportUrl.startsWith('blob:')) {
                URL.revokeObjectURL(reportUrl.split('#')[0]);
            }

            const url = URL.createObjectURL(finalBlob);
            // Append PDF viewer parameters: zoom=100 and pagemode=thumbnails (to open sidebar)
            setReportUrl(`${url}#toolbar=1&navpanes=1&zoom=100&pagemode=thumbnails`);
        } catch (err: any) {
            console.error('FastReport Viewer Error:', err);
            const responseBlob = err?.response?.data;
            if (responseBlob instanceof Blob) {
                const message = await readBlobMessage(responseBlob);
                if (isNoDataMessage(message)) {
                    setNoDataMessage(NO_DATA_TO_PRINT_MESSAGE);
                    return;
                }
                setError(message || 'Failed to load report');
                return;
            }
            setError(err.message || 'Failed to load report');
        } finally {
            setLoading(false);
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [reportName, parameterSignature, baseUrl, includeReportNameInPath, buildQueryParams, requestKey]);

    const downloadReport = useCallback(async () => {
        if (!reportName) return;

        setExporting(true);
        setExportError(null);
        try {
            const trimmedBaseUrl = baseUrl.replace(/\/+$/, '');
            const reportPath = includeReportNameInPath
                ? `/export/${encodeURIComponent(reportName)}`
                : '/export';
            const blob = await hrReportBlob(reportName, buildQueryParams(), exportFormat);
            if (blob?.type?.includes('json') || blob?.type?.startsWith('text/')) {
                const message = await readBlobMessage(blob);
                throw new Error(isNoDataMessage(message) ? NO_DATA_TO_PRINT_MESSAGE : message || 'Server error during report export');
            }

            const downloadUrl = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.download = `${reportName}.${exportFormat}`;
            document.body.appendChild(link);
            link.click();
            link.remove();
            URL.revokeObjectURL(downloadUrl);
        } catch (err: any) {
            console.error('FastReport Export Error:', err);
            setExportError(err.message || 'Failed to download report');
        } finally {
            setExporting(false);
        }
    }, [baseUrl, buildQueryParams, exportFormat, includeReportNameInPath, reportName]);

    useEffect(() => {
        if (!parameters) return;

        // React StrictMode intentionally re-runs effects in development.  A
        // report generation is costly, so only issue one request for an
        // identical report invocation.  requestKey still permits an explicit
        // Generate click with the same filters to refresh the report.
        const key = `${baseUrl}|${includeReportNameInPath}|${reportName}|${parameterSignature}|${requestKey ?? ''}`;
        if (fetchedRequestRef.current === key) return;

        fetchedRequestRef.current = key;
        void fetchReport();
    }, [baseUrl, fetchReport, includeReportNameInPath, parameterSignature, parameters, reportName, requestKey]);

    useEffect(() => {
        return () => {
            if (reportUrl && reportUrl.startsWith('blob:')) {
                URL.revokeObjectURL(reportUrl.split('#')[0]);
            }
        };
    }, [reportUrl]);

    if (loading) {
        return (
            <Box sx={{ height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: '#f8fafc' }}>
                <Box sx={{ textAlign: 'center' }}>
                    <CircularProgress size={40} thickness={4} sx={{ color: 'var(--lms-primary)', mb: 2 }} />
                    <Typography sx={{ color: '#64748b', fontWeight: 600, fontSize: '0.9rem' }}>
                        <T>Generating report, please wait...</T>
                    </Typography>
                </Box>
            </Box>
        );
    }

    if (noDataMessage) {
        return (
            <Box sx={{
                height: '100%',
                display: 'grid',
                placeItems: 'center',
                bgcolor: '#f6f8fb',
                p: 3,
                textAlign: 'center',
                backgroundImage: `
                    radial-gradient(circle at 50% 44%, rgba(var(--lms-primary-rgb), 0.08) 0 1px, transparent 1px),
                    linear-gradient(135deg, rgba(var(--lms-primary-rgb), 0.05), rgba(255, 255, 255, 0) 45%)
                `,
                backgroundSize: '22px 22px, 100% 100%'
            }}>
                <Box sx={{
                    width: 'min(520px, 100%)',
                    px: { xs: 3, sm: 5 },
                    py: { xs: 4, sm: 5 },
                    position: 'relative',
                    overflow: 'hidden',
                    border: '1px solid rgba(var(--lms-primary-rgb), 0.18)',
                    borderRadius: '14px',
                    bgcolor: 'rgba(255, 255, 255, 0.96)',
                    boxShadow: '0 24px 60px rgba(15, 23, 42, 0.10)',
                    '&:before': {
                        content: '""',
                        position: 'absolute',
                        inset: 12,
                        border: '1px dashed rgba(var(--lms-primary-rgb), 0.28)',
                        borderRadius: '10px',
                        pointerEvents: 'none'
                    },
                    '&:after': {
                        content: '""',
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        right: 0,
                        height: 4,
                        background: 'linear-gradient(90deg, var(--lms-primary), rgba(var(--lms-primary-rgb), 0.289), var(--lms-primary))'
                    }
                }}>
                    <Box sx={{
                        width: 86,
                        height: 86,
                        mx: 'auto',
                        mb: 2.5,
                        position: 'relative',
                        borderRadius: '50%',
                        display: 'grid',
                        placeItems: 'center',
                        bgcolor: 'rgba(var(--lms-primary-rgb), 0.111)',
                        color: 'var(--lms-primary-dark)',
                        boxShadow: 'inset 0 0 0 10px rgba(255, 255, 255, 0.72), 0 14px 30px rgba(var(--lms-primary-rgb), 0.18)',
                        '&:before, &:after': {
                            content: '""',
                            position: 'absolute',
                            borderRadius: '999px',
                            border: '1px solid rgba(var(--lms-primary-rgb), 0.24)'
                        },
                        '&:before': { inset: -10 },
                        '&:after': { inset: 9, borderStyle: 'dashed' }
                    }}>
                        <DescriptionOutlinedIcon sx={{ fontSize: 42, position: 'relative', zIndex: 1 }} />
                    </Box>
                    <Box sx={{
                        display: 'inline-flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        px: 1.5,
                        py: 0.45,
                        mb: 1.5,
                        borderRadius: '999px',
                        bgcolor: 'rgba(var(--lms-primary-rgb), 0.10)',
                        color: 'var(--lms-primary-dark)',
                        fontSize: '0.72rem',
                        fontWeight: 900,
                        textTransform: 'uppercase',
                        letterSpacing: 0
                    }}>
                        <T>Selection Result</T>
                    </Box>
                    <Typography sx={{ fontSize: { xs: '1.25rem', sm: '1.45rem' }, fontWeight: 900, color: '#17231a', mb: 0.75, letterSpacing: 0 }}>
                        <T>{noDataMessage}</T>
                    </Typography>
                    <Typography sx={{ maxWidth: 360, mx: 'auto', fontSize: '0.9rem', lineHeight: 1.65, fontWeight: 650, color: '#5f6f82' }}>
                        <T>No records match the selected criteria. Adjust the filters and generate the report again.</T>
                    </Typography>
                </Box>
            </Box>
        );
    }

    if (error) {
        return (
            <Box sx={{ height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: '#fff', color: 'error.main', p: 3, textAlign: 'center' }}>
                <Box sx={{ maxWidth: '500px' }}>
                    <Typography variant="h6" gutterBottom sx={{ fontWeight: 800 }}>Report Rendering Error</Typography>
                    <Typography variant="body2" sx={{ mb: 3, color: '#64748b' }}>{error}</Typography>
                    <Button 
                        variant="contained" 
                        color="primary" 
                        onClick={fetchReport}
                        sx={{ borderRadius: '8px', textTransform: 'none', fontWeight: 700, bgcolor: 'var(--lms-primary)', '&:hover': { bgcolor: 'var(--lms-primary-dark)' } }}
                    >
                        Try Again
                    </Button>
                </Box>
            </Box>
        );
    }

    if (!reportUrl) return null;

    return (
        <Box sx={{
            height: '100%',
            width: '100%',
            minWidth: 0,
            flex: '1 1 auto',
            bgcolor: '#525659', // Dark PDF viewer background
            position: 'relative',
            display: 'flex',
            flexDirection: 'column',
            overflow: 'hidden',
            borderRadius: '8px'
        }}>
            <Box sx={{
                minHeight: 48,
                px: 1.5,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'flex-end',
                gap: 1,
                bgcolor: '#f8fafc',
                borderBottom: '1px solid #d6d9dc'
            }}>
                {exportError && (
                    <Typography sx={{ color: 'error.main', fontSize: '0.78rem', fontWeight: 700, mr: 'auto' }}>
                        {exportError}
                    </Typography>
                )}
                <Select
                    size="small"
                    value={exportFormat}
                    onChange={(event) => setExportFormat(event.target.value as (typeof REPORT_EXPORT_FORMATS)[number]['value'])}
                    sx={{
                        minWidth: 112,
                        bgcolor: '#fff',
                        fontSize: '0.82rem',
                        '& .MuiSelect-select': { py: 0.65 }
                    }}
                >
                    {REPORT_EXPORT_FORMATS.map((format) => (
                        <MenuItem key={format.value} value={format.value}>
                            {format.label}
                        </MenuItem>
                    ))}
                </Select>
                <Button
                    variant="contained"
                    size="small"
                    startIcon={<DownloadIcon />}
                    disabled={exporting}
                    onClick={downloadReport}
                    sx={{
                        minWidth: 104,
                        textTransform: 'none',
                        fontWeight: 800,
                        bgcolor: 'var(--lms-primary)',
                        '&:hover': { bgcolor: 'var(--lms-primary-dark)' }
                    }}
                >
                    {exporting ? <T>Downloading</T> : <T>Download</T>}
                </Button>
            </Box>
            <embed
                src={reportUrl}
                type="application/pdf"
                width="100%"
                height="100%"
                style={{ 
                    border: 'none', 
                    display: 'block',
                    background: '#525659'
                }}
            />
        </Box>
    );
};
