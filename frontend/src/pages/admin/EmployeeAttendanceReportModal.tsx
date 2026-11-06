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
import { FormDatePicker } from '@/components/FormDatePicker';
import { LookupDropdown } from '@/components/LookupDropdown';
import dayjs from 'dayjs';
import { useAppSelector } from '@/hooks';
import { useAttendanceCampusPermission } from '@/hooks/useAttendanceCampusPermission';

interface EmployeeAttendanceReportModalProps {
    open: boolean;
    onClose: () => void;
    lookups?: {
        employees: any[];
        entities: any[];
        activities: any[];
        categories: any[];
        departments: any[];
    };
    initialFilters?: any;
    isFullScreenView?: boolean;
}

export const EmployeeAttendanceReportModal: React.FC<EmployeeAttendanceReportModalProps> = ({ open, onClose, lookups, initialFilters, isFullScreenView }) => {
    const { translate } = useTranslation();
    const { canAccessCampusDropdown } = useAttendanceCampusPermission();
    const selectedCampusId = useAppSelector(state => state.auth.selectedCampusId);
    const [criteriaExpanded, setCriteriaExpanded] = useState(true);
    const [viewerExpanded, setViewerExpanded] = useState(false);
    
    const [filters, setFilters] = useState({
        employeeId: initialFilters?.employeeId || null as number | null,
        employeeName: initialFilters?.employeeName || '',
        employeeCategoryId: null as number | null,
        employeeCategoryName: '',
        departmentId: null as number | null,
        departmentName: '',
        activityId: null as number | null,
        activityName: '',
        fromDate: initialFilters ? (initialFilters.fromDate ? dayjs(initialFilters.fromDate).format('YYYY-MM-DD') : '') : dayjs().startOf('month').format('YYYY-MM-DD'),
        tillDate: initialFilters ? (initialFilters.tillDate ? dayjs(initialFilters.tillDate).format('YYYY-MM-DD') : '') : dayjs().endOf('month').format('YYYY-MM-DD'),
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
    const [requestKey, setRequestKey] = useState(0);

    // Sync with initialFilters if they change
    useEffect(() => {
        if (open && initialFilters) {
            const newFilters = {
                ...filters,
                employeeId: initialFilters.employeeId || null,
                employeeName: initialFilters.employeeName || '',
                fromDate: initialFilters.fromDate ? dayjs(initialFilters.fromDate).format('YYYY-MM-DD') : '',
                tillDate: initialFilters.tillDate ? dayjs(initialFilters.tillDate).format('YYYY-MM-DD') : '',
            };
            setFilters(newFilters);

            // Auto-generate if we have an employeeId from the row click
            if (initialFilters.employeeId) {
                // We need to wait for filters to update or just use newFilters
                setReportParams({
                    EmployeeId: newFilters.employeeId || 0,
                    FromDate: newFilters.fromDate,
                    TillDate: newFilters.tillDate,
                    EntityId: newFilters.entityId || 0,
                    InstitutionName: newFilters.institutionName,
                    CampusName: newFilters.campusName,
                    Address: newFilters.address,
                    City: newFilters.city,
                    Country: newFilters.country,
                    LogoId: newFilters.imageId,
                    ReportName: 'EmployeeAttendanceSummary'
                });
                setCriteriaExpanded(false);
                setViewerExpanded(true);
            }
        }
    }, [open, initialFilters]);

    const handleGenerate = () => {
        setRequestKey(previous => previous + 1);
        setReportParams({
            EmployeeId: filters.employeeId || 0,
            FromDate: filters.fromDate,
            TillDate: filters.tillDate,
            EntityId: filters.entityId || 0,
            EmployeeCategoryId: filters.employeeCategoryId || 0,
            DepartmentId: filters.departmentId || 0,
            ActivityId: filters.activityId || 0,
            // Header parameters
            InstitutionName: filters.institutionName,
            CampusName: filters.campusName,
            Address: filters.address,
            City: filters.city,
            Country: filters.country,
            LogoId: filters.imageId,
            ReportName: 'EmployeeAttendanceSummary'
        });
        setCriteriaExpanded(false);
        setViewerExpanded(true);
    };

    const handleClear = () => {
        setFilters(prev => ({
            ...prev,
            employeeId: null,
            employeeName: '',
            employeeCategoryId: null,
            employeeCategoryName: '',
            departmentId: null,
            departmentName: '',
            activityId: null,
            activityName: '',
            fromDate: dayjs().startOf('month').format('YYYY-MM-DD'),
            tillDate: dayjs().endOf('month').format('YYYY-MM-DD'),
        }));
        setReportParams(null);
        setViewerExpanded(false);
        setCriteriaExpanded(true);
    };

    // Set default entity when opened
    useEffect(() => {
        if (open && lookups?.entities && lookups.entities.length > 0 && !filters.entityId) {
            const defaultEntity = lookups.entities.find(entity => Number(entity.id) === Number(selectedCampusId)) ?? lookups.entities[0];
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
    }, [open, lookups, selectedCampusId]);

    if (isFullScreenView) {
        return renderContent();
    }

    return (
        <Modal 
            open={open} 
            onClose={onClose} 
            title={'Employee Attendance Summary'}
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
                        <Box sx={{ p: 5 }}>
                            <Grid container spacing={3} sx={{ maxWidth: '850px' }}>
                                {/* Entity / School */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>School</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.entityName}
                                        placeholder={translate("Select School")}
                                        options={lookups?.entities || []}
                                        disabled={!canAccessCampusDropdown}
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
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Employee Category */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Employee Category</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.employeeCategoryName}
                                        placeholder={translate("Select Employee Category")}
                                        options={lookups?.categories || []}
                                        getOptionLabel={(option) => option.stxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            employeeCategoryId: row.id || row.lovid, 
                                            employeeCategoryName: row.stxt || row.name
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Department */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Department</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.departmentName}
                                        placeholder={translate("Select Department")}
                                        options={lookups?.departments || []}
                                        getOptionLabel={(option) => option.departmentName || option.department || option.stxt || option.depStxt || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            departmentId: row.id || row.departmentId || row.depid, 
                                            departmentName: row.departmentName || row.department || row.stxt || row.depStxt || row.name
                                        }))}
                                        onClear={() => setFilters(prev => ({
                                            ...prev,
                                            departmentId: null,
                                            departmentName: ''
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Employee */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Employee</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.employeeName}
                                        placeholder={translate("Select Employee")}
                                        options={lookups?.employees || []}
                                        getOptionLabel={(option) => option.employeeName || option.name || option.employeeCode || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            employeeId: row.id, 
                                            employeeName: row.employeeName || row.name
                                        }))}
                                        onClear={() => setFilters(prev => ({
                                            ...prev,
                                            employeeId: null,
                                            employeeName: ''
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Activity */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Activity</T>
                                    </Typography>
                                    <LookupDropdown
                                        hideLabel
                                        value={filters.activityName}
                                        placeholder={translate("Select Activity")}
                                        options={lookups?.activities || []}
                                        getOptionLabel={(option) => option.stxt || option.activity || option.name || ""}
                                        onSelect={(row: any) => setFilters(prev => ({ 
                                            ...prev, 
                                            activityId: row.id || row.lovid, 
                                            activityName: row.stxt || row.activity || row.name
                                        }))}
                                        onSearchClick={() => {}}
                                        hideDropdownArrow={true}
                                        sx={{
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#f8fafc',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                                '&:hover fieldset': { borderColor: '#cbd5e1' },
                                            }
                                        }}
                                    />
                                </Grid>

                                {/* Date Range */}
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Attendance From</T>
                                    </Typography>
                                    <FormDatePicker
                                        hideLabel
                                        value={filters.fromDate ? dayjs(filters.fromDate) : null}
                                        onChange={(val) => setFilters(prev => ({ ...prev, fromDate: val ? val.format("YYYY-MM-DD") : "" }))}
                                        sx={{ 
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#fff',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
                                            }
                                        }}
                                    />
                                </Grid>
                                <Grid item xs={12} sx={{ display: 'flex', alignItems: 'center' }}>
                                    <Typography sx={{ width: '220px', fontWeight: 600, color: '#334155', fontSize: '1rem' }}>
                                        <T>Attendance Till</T>
                                    </Typography>
                                    <FormDatePicker
                                        hideLabel
                                        value={filters.tillDate ? dayjs(filters.tillDate) : null}
                                        onChange={(val) => setFilters(prev => ({ ...prev, tillDate: val ? val.format("YYYY-MM-DD") : "" }))}
                                        sx={{ 
                                            flex: 1,
                                            '& .MuiOutlinedInput-root': {
                                                bgcolor: '#fff',
                                                borderRadius: '25px',
                                                height: '50px',
                                                '& fieldset': { borderColor: '#e2e8f0' },
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
                                    reportName="EmployeeAttendanceSummary"
                                    parameters={reportParams}
                                    requestKey={requestKey}
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
