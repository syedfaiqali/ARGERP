import React, { useState, useEffect } from 'react';
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
} from '@mui/icons-material';
import { Modal } from '@/components/Modal';
import { T, useTranslation } from '@/components/AdaptiveTranslationProvider';
import { FastReportViewer } from '@/components/reporting/FastReportViewer';
import { LookupDropdown } from '@/components/LookupDropdown';

interface AttendancePolicyReportModalProps {
    open: boolean;
    onClose: () => void;
    lookups?: {
        entities: any[];
        policyTypes: any[];
        policies: any[];
    };
    isFullScreenView?: boolean;
}

export const AttendancePolicyReportModal: React.FC<AttendancePolicyReportModalProps> = ({ open, onClose, lookups, isFullScreenView }) => {
    const { translate } = useTranslation();
    const [criteriaExpanded, setCriteriaExpanded] = useState(true);
    const [viewerExpanded, setViewerExpanded] = useState(false);
    
    const [filters, setFilters] = useState({
        policyTypeId: null as number | null,
        policyTypeName: '',
        policyId: null as number | null,
        policyName: '',
        entityId: null as number | null,
        entityName: '',
        institutionName: '',
        campusName: '',
        address: '',
        city: '',
        country: '',
        imageId: null as any,
    });

    const [reportParams, setReportParams] = useState<any>(null);

    useEffect(() => {
        if (open && lookups?.entities && lookups.entities.length > 0 && !filters.entityId) {
            const defaultEntity = lookups.entities[0];
            setFilters(prev => ({
                ...prev,
                entityId: defaultEntity.id,
                entityName: defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt,
                institutionName: defaultEntity.shortName || defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt,
                campusName: defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt,
                address: defaultEntity.address || "",
                city: defaultEntity.city || "",
                country: defaultEntity.country || "",
                imageId: defaultEntity.imageId || null
            }));
        }
    }, [open, lookups?.entities, filters.entityId]);

    const handleGenerate = () => {
        setReportParams({
            PolicyTypeId: filters.policyTypeId || 0,
            PolicyId: filters.policyId || 0,
            EntityId: filters.entityId || 0,
            // Header parameters
            InstitutionName: filters.institutionName,
            CampusName: filters.campusName,
            Address: filters.address,
            City: filters.city,
            Country: filters.country,
            LogoId: filters.imageId,
            ReportName: 'AttendancePolicySummary'
        });
        setCriteriaExpanded(false);
        setViewerExpanded(true);
    };

    const handleClear = () => {
        setFilters(prev => ({
            ...prev,
            policyTypeId: null,
            policyTypeName: '',
            policyId: null,
            policyName: '',
        }));
        setReportParams(null);
        setViewerExpanded(false);
        setCriteriaExpanded(true);
    };

    // Set default entity when opened
    useEffect(() => {
        if (open && lookups?.entities && lookups.entities.length > 0 && !filters.entityId) {
            const defaultEntity = lookups.entities[0];
            setFilters(prev => ({
                ...prev,
                entityId: defaultEntity.id,
                entityName: defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt || defaultEntity.ltxt,
                institutionName: defaultEntity.shortName || defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt,
                campusName: defaultEntity.instituteName || defaultEntity.name || defaultEntity.stxt,
                address: defaultEntity.address || "",
                city: defaultEntity.city || "",
                country: defaultEntity.country || "",
                imageId: defaultEntity.imageId || null
            }));
        }
    }, [open, lookups]);

    if (isFullScreenView) {
        return renderContent();
    }

    return (
        <Modal 
            open={open} 
            onClose={onClose} 
            title={"Attendance Policy Summary"}
            maxWidth="lg"
            showActions={false}
        >
            {renderContent()}
        </Modal>
    );

    function renderContent() {
        return (
            <Box sx={{ minHeight: '500px' }}>
                {/* Custom Toolbar */}
                <Box sx={{ 
                    display: 'flex', 
                    gap: 3, 
                    mb: 3, 
                    p: 0, 
                    alignItems: 'center'
                }}>
                    <Button
                        variant="contained"
                        onClick={handleGenerate}
                        startIcon={<SettingsIcon />}
                        sx={{ 
                            textTransform: 'none', 
                            fontWeight: 700, 
                            bgcolor: 'var(--lms-primary)',
                            borderRadius: '8px',
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
                            '&:hover': { bgcolor: 'transparent', opacity: 0.8 }
                        }}
                    >
                        <T>Clear Filter</T>
                    </Button>
                </Box>

                <Box sx={{ border: '1px solid #f1f5f9', borderRadius: '8px', overflow: 'hidden', bgcolor: '#fff', mb: 3 }}>
                    
                    {/* Section Header: Criteria */}
                    <Box
                        onClick={() => setCriteriaExpanded(!criteriaExpanded)}
                        sx={{
                            p: 2,
                            bgcolor: '#f8fafc',
                            borderBottom: criteriaExpanded ? '1px solid #f1f5f9' : 'none',
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            cursor: 'pointer',
                        }}
                    >
                        <Typography sx={{ fontSize: '1rem', fontWeight: 700, color: '#334155' }}>
                            <T>Report Criteria</T>
                        </Typography>
                        <IconButton size="small" sx={{ color: '#64748b' }}>
                            {criteriaExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}
                        </IconButton>
                    </Box>

                    <Collapse in={criteriaExpanded}>
                        <Box sx={{ p: 4 }}>
                            <Grid container spacing={3} sx={{ maxWidth: '800px' }}>
                                {/* Entity / School */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>School</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.entityName}
                                        placeholder={translate("Select School")}
                                        options={lookups?.entities || []}
                                        getOptionLabel={(option) => option.instituteName || option.name || option.stxt || option.ltxt || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            entityId: row.id, 
                                            entityName: row.instituteName || row.name || row.stxt,
                                            institutionName: row.shortName || row.instituteName || row.name || row.stxt,
                                            campusName: row.instituteName || row.name || row.stxt,
                                            address: row.address || "",
                                            city: row.city || "",
                                            country: row.country || "",
                                            imageId: row.imageId || null
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '10px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Policy Name */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Policy Name</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.policyName}
                                        placeholder={translate("Select Policy Name")}
                                        options={lookups?.policies || []}
                                        getOptionLabel={(option) => option.policyName || option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            policyId: row.id, 
                                            policyName: row.policyName || row.stxt || row.name
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '10px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Policy Type */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Policy Type</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.policyTypeName}
                                        placeholder={translate("Select Policy Type")}
                                        options={lookups?.policyTypes || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            policyTypeId: row.id || row.lovid, 
                                            policyTypeName: row.stxt || row.name
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '10px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>
                            </Grid>
                        </Box>
                    </Collapse>

                    {/* Section Header: Viewer */}
                    <Box
                        onClick={() => setViewerExpanded(!viewerExpanded)}
                        sx={{
                            p: 2,
                            bgcolor: '#f8fafc',
                            borderTop: '1px solid #f1f5f9',
                            borderBottom: viewerExpanded ? '1px solid #f1f5f9' : 'none',
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            cursor: 'pointer',
                        }}
                    >
                        <Typography sx={{ fontSize: '1rem', fontWeight: 700, color: '#334155' }}>
                            <T>Report Viewer</T>
                        </Typography>
                        <IconButton size="small" sx={{ color: '#64748b' }}>
                            {viewerExpanded ? <RemoveIcon fontSize="small" /> : <AddIcon fontSize="small" />}
                        </IconButton>
                    </Box>

                    <Collapse in={viewerExpanded}>
                        <Box sx={{ height: '700px', bgcolor: '#f1f5f9' }}>
                            {reportParams ? (
                                <FastReportViewer
                                    reportName="AttendancePolicySummary"
                                    parameters={reportParams}
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
                                    <AssessmentIcon sx={{ fontSize: '4rem', opacity: 0.2 }} />
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
    }
};
