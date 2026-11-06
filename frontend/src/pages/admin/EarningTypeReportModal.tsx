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

interface EarningTypeReportModalProps {
    open: boolean;
    onClose: () => void;
    lookups?: {
        earningTypes: any[];
        uoms: any[];
        payRunTypes: any[];
        entities: any[];
    };
    isFullScreenView?: boolean;
}

export const EarningTypeReportModal: React.FC<EarningTypeReportModalProps> = ({ open, onClose, lookups, isFullScreenView }) => {
    const { translate } = useTranslation();
    const [criteriaExpanded, setCriteriaExpanded] = useState(true);
    const [viewerExpanded, setViewerExpanded] = useState(false);
    
    const [filters, setFilters] = useState({
        earningTypeId: null as number | null,
        earningTypeName: '',
        uomId: null as number | null,
        uomName: '',
        payRunTypeId: null as number | null,
        payRunTypeName: '',
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

    const handleGenerate = () => {
        setReportParams({
            EarningTypeId: filters.earningTypeId || 0,
            UomId: filters.uomId || 0,
            PayRunTypeId: filters.payRunTypeId || 0,
            // Header parameters
            InstitutionName: filters.institutionName,
            CampusName: filters.campusName,
            Address: filters.address,
            City: filters.city,
            Country: filters.country,
            LogoId: filters.imageId,
            ReportName: 'EarningTypeSummary'
        });
        setCriteriaExpanded(false);
        setViewerExpanded(true);
    };

    const handleClear = () => {
        setFilters(prev => ({
            ...prev,
            earningTypeId: null,
            earningTypeName: '',
            uomId: null,
            uomName: '',
            payRunTypeId: null,
            payRunTypeName: ''
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
            title={translate("Earning Type Report")}
            maxWidth="lg"
            onSave={() => handleGenerate()}
            onSaveAndClose={() => {
                handleGenerate();
                setTimeout(onClose, 500);
            }}
            onReset={handleClear}
            saveLabel={translate("Save")}
            saveAndCloseLabel={translate("Save & Close")}
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
                    gap: 2, 
                    mb: 2, 
                    p: 1.5, 
                    bgcolor: '#fff', 
                    borderBottom: '1px solid var(--lms-border)',
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
                            '&:hover': { bgcolor: 'var(--lms-primary-dark)' }
                        }}
                    >
                        <T>Generate</T>
                    </Button>
                    <Button
                        onClick={handleClear}
                        startIcon={<ClearFilterIcon />}
                        sx={{ textTransform: 'none', fontWeight: 700, color: '#64748b' }}
                    >
                        <T>Clear Filter</T>
                    </Button>
                </Box>

                <Box sx={{ border: '1px solid var(--lms-border)', borderRadius: '12px', overflow: 'hidden', bgcolor: '#fff' }}>
                    
                    {/* Section Header: Criteria */}
                    <Box
                        onClick={() => setCriteriaExpanded(!criteriaExpanded)}
                        sx={{
                            p: 1.5,
                            bgcolor: '#f8fafc',
                            borderBottom: criteriaExpanded ? '1px solid var(--lms-border)' : 'none',
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            cursor: 'pointer',
                            '&:hover': { bgcolor: '#f1f5f9' }
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
                                {/* Earning Type */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Earning Type</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.earningTypeName}
                                        placeholder={translate("Select Earning Type")}
                                        options={lookups?.earningTypes || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            earningTypeId: row.id, 
                                            earningTypeName: row.stxt || row.name
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

                                {/* Unit of Measure */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Unit of Measure</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.uomName}
                                        placeholder={translate("Select Unit of Measure")}
                                        options={lookups?.uoms || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            uomId: row.id, 
                                            uomName: row.stxt || row.name
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

                                {/* Payment Run Type */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Payment Run Type</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.payRunTypeName}
                                        placeholder={translate("Select Payment Run Type")}
                                        options={lookups?.payRunTypes || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            payRunTypeId: row.id, 
                                            payRunTypeName: row.stxt || row.name
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
                            p: 1.5,
                            bgcolor: '#f8fafc',
                            borderTop: '1px solid var(--lms-border)',
                            borderBottom: viewerExpanded ? '1px solid var(--lms-border)' : 'none',
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            cursor: 'pointer',
                            '&:hover': { bgcolor: '#f1f5f9' }
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
                        <Box sx={{ height: '700px', bgcolor: '#f1f5f9' }}>
                            {reportParams ? (
                                <FastReportViewer
                                    reportName="EarningTypeSummary"
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
