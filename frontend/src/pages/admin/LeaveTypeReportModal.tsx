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

interface LeaveTypeReportModalProps {
    open: boolean;
    onClose: () => void;
    lookups?: {
        leaveTypes: any[];
        entities: any[];
        maritalStatus: any[];
        gender: any[];
        renewOn: any[];
    };
    isFullScreenView?: boolean;
}

export const LeaveTypeReportModal: React.FC<LeaveTypeReportModalProps> = ({ open, onClose, lookups, isFullScreenView }) => {
    const { translate } = useTranslation();
    const [criteriaExpanded, setCriteriaExpanded] = useState(true);
    const [viewerExpanded, setViewerExpanded] = useState(false);
    
    const [filters, setFilters] = useState({
        leaveTypeId: null as number | null,
        leaveTypeName: '',
        maritalStatusId: null as number | null,
        maritalStatusName: '',
        genderId: null as number | null,
        genderName: '',
        renewOnId: null as number | null,
        renewOnName: '',
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
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        const clientId = user?.clientId || 0;

        setReportParams({
            ClientId: clientId,
            LeaveTypeId: filters.leaveTypeId || 0,
            MaritalStatusId: filters.maritalStatusId || 0,
            GenderId: filters.genderId || 0,
            RenewOnId: filters.renewOnId || 0,
            EntityId: filters.entityId || 0,
            // Header parameters
            InstitutionName: filters.institutionName,
            CampusName: filters.campusName,
            Address: filters.address,
            City: filters.city,
            Country: filters.country,
            LogoId: filters.imageId,
            ReportName: 'LeaveTypeSummary'
        });
        setCriteriaExpanded(false);
        setViewerExpanded(true);
    };

    const handleClear = () => {
        setFilters(prev => ({
            ...prev,
            leaveTypeId: null,
            leaveTypeName: '',
            maritalStatusId: null,
            maritalStatusName: '',
            genderId: null,
            genderName: '',
            renewOnId: null,
            renewOnName: '',
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

    const renderContent = () => {
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
                                {/* School (Entity) */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>School</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.entityName}
                                        placeholder={translate("Select School")}
                                        options={lookups?.entities || []}
                                        getOptionLabel={(option) => option.instituteName || option.name || option.stxt || ""}
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

                                {/* Marital Status */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Marital Status</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.maritalStatusName}
                                        placeholder={translate("Select Marital Status")}
                                        options={lookups?.maritalStatus || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            maritalStatusId: row.id, 
                                            maritalStatusName: row.stxt || row.name
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

                                {/* Gender */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Gender</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.genderName}
                                        placeholder={translate("Select Gender")}
                                        options={lookups?.gender || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            genderId: row.id, 
                                            genderName: row.stxt || row.name
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

                                {/* Renew On */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Renew On</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.renewOnName}
                                        placeholder={translate("Select Renew On")}
                                        options={lookups?.renewOn || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            renewOnId: row.id, 
                                            renewOnName: row.stxt || row.name
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

                                {/* Leave Type */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '180px', fontWeight: 600, color: '#475569', fontSize: '0.9rem' }}>
                                        <T>Leave Type</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.leaveTypeName}
                                        placeholder={translate("Select Leave Type")}
                                        options={lookups?.leaveTypes || []}
                                        getOptionLabel={(option) => option.stxt || option.name || option.leaveType || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            leaveTypeId: row.id, 
                                            leaveTypeName: row.stxt || row.name || row.leaveType
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
                                    reportName="LeaveTypeSummary"
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
    };

    if (isFullScreenView) {
        return renderContent();
    }

    return (
        <Modal 
            open={open} 
            onClose={onClose} 
            title={translate("Leave Type Report")}
            maxWidth="lg"
            onSave={() => handleGenerate()}
            onSaveAndClose={() => {
                handleGenerate();
                setTimeout(onClose, 500);
            }}
            onReset={handleClear}
            saveLabel={translate("Generate")}
        >
            {renderContent()}
        </Modal>
    );
};
