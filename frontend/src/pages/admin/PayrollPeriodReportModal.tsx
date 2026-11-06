import React, { useState, useEffect } from 'react';
import { useTranslation } from '@/components/AdaptiveTranslationProvider';
import { LookupDropdown } from '@/components/LookupDropdown';
import { BaseReportModal, ReportFilterField } from '@/components/reporting/BaseReportModal';
import { Search as SearchIcon } from '@mui/icons-material';

interface PayrollPeriodReportModalProps {
    open: boolean;
    onClose: () => void;
    lookups?: {
        entities: any[];
        payrollRegisters: any[];
        payrollPeriods: any[];
        payrollTypes: any[];
        statuses: any[];
    };
    initialFilters?: any;
    isFullScreenView?: boolean;
}

export const PayrollPeriodReportModal: React.FC<PayrollPeriodReportModalProps> = ({ 
    open, 
    onClose, 
    lookups, 
    initialFilters,
    isFullScreenView = false
}) => {
    const { translate } = useTranslation();
    
    const [filters, setFilters] = useState({
        entityId: null as number | null,
        entityName: '',
        payrollRegisterId: null as number | null,
        payrollRegisterName: '',
        payrollPeriodId: null as number | null,
        payrollPeriodName: '',
        payrollTypeId: null as number | null,
        payrollTypeName: '',
        statusId: null as number | null,
        statusName: '',
        institutionName: '',
        campusName: '',
        address: '',
        city: '',
        country: '',
        imageId: null as number | null,
        reportType: 'Summary' as 'Summary' | 'Detail',
    });

    useEffect(() => {
        if (open && initialFilters) {
            setFilters(prev => ({ ...prev, ...initialFilters }));
        } else if (open && lookups?.entities && lookups.entities.length > 0 && !filters.entityId) {
            const defaultEntity = lookups.entities[0];
            setFilters(prev => ({
                ...prev,
                entityId: defaultEntity.id,
                entityName: defaultEntity.instituteName || defaultEntity.name || '',
                institutionName: defaultEntity.shortName || defaultEntity.instituteName || '',
                campusName: defaultEntity.instituteName || '',
                address: defaultEntity.address || "",
                city: defaultEntity.city || "",
                country: defaultEntity.country || "",
                imageId: defaultEntity.imageId || null
            }));
        }
    }, [open, initialFilters, lookups?.entities]);

    const handleGenerate = () => {
        return {
            EntityId: filters.entityId,
            PayrollRegisterId: filters.payrollRegisterId,
            PayrollPeriodId: filters.payrollPeriodId,
            PayrollTypeId: filters.payrollTypeId,
            StatusId: filters.statusId,
            InstitutionName: filters.institutionName,
            CampusName: filters.campusName,
            Address: filters.address,
            City: filters.city,
            Country: filters.country,
            LogoId: filters.imageId,
        };
    };

    const handleClear = () => {
        setFilters(prev => ({
            ...prev,
            payrollRegisterId: null,
            payrollRegisterName: '',
            payrollPeriodId: null,
            payrollPeriodName: '',
            payrollTypeId: null,
            payrollTypeName: '',
            statusId: null,
            statusName: '',
        }));
    };

    const dropdownSx = {
        flex: 1,
        '& .MuiOutlinedInput-root': {
            bgcolor: '#f8fafc',
            borderRadius: '10px',
            '& fieldset': { borderColor: '#e2e8f0' },
            '&:hover fieldset': { borderColor: '#cbd5e1' },
        }
    };

    const searchIconStyle = { color: 'var(--lms-primary)', fontSize: '1.2rem' };

    return (
        <BaseReportModal
            open={open}
            onClose={onClose}
            title="Payroll Period Report"
            reportName={filters.reportType === 'Summary' ? 'PayrollPeriodSummary' : 'PayrollPeriodDetail'}
            baseUrl="/api/report/payroll-period"
            onGenerate={handleGenerate}
            onClear={handleClear}
            isFullScreenView={isFullScreenView}
        >
            <ReportFilterField label="Entity">
                <LookupDropdown
                    hideLabel
                    value={filters.entityName}
                    placeholder={translate("Select Entity")}
                    options={lookups?.entities || []}
                    getOptionLabel={(option) => option.instituteName || option.name || ""}
                    onSelect={(row: any) => setFilters(prev => ({ 
                        ...prev, 
                        entityId: row.id, 
                        entityName: row.instituteName || row.name,
                        institutionName: row.shortName || row.instituteName,
                        campusName: row.instituteName,
                        address: row.address || "",
                        city: row.city || "",
                        country: row.country || "",
                        imageId: row.imageId || null
                    }))}
                    onSearchClick={() => { }}
                    hideDropdownArrow
                    // suffixIcon={<SearchIcon sx={searchIconStyle} />}
                    sx={dropdownSx}
                />
            </ReportFilterField>

            <ReportFilterField label="Payroll Register">
                <LookupDropdown
                    hideLabel
                    value={filters.payrollRegisterName}
                    placeholder={translate("Select Payroll Register")}
                    options={lookups?.payrollRegisters || []}
                    getOptionLabel={(option) => option.stxt || option.name || ""}
                    onSelect={(row: any) => setFilters(prev => ({ ...prev, payrollRegisterId: row.id, payrollRegisterName: row.stxt || row.name }))}
                    onSearchClick={() => { }}
                    hideDropdownArrow
                    // suffixIcon={<SearchIcon sx={searchIconStyle} />}
                    sx={dropdownSx}
                />
            </ReportFilterField>

            <ReportFilterField label="Payroll Period">
                <LookupDropdown
                    hideLabel
                    value={filters.payrollPeriodName}
                    placeholder={translate("Select Payroll Period")}
                    options={lookups?.payrollPeriods || []}
                    getOptionLabel={(option) => option.stxt || option.name || option.periodText || ""}
                    onSelect={(row: any) => setFilters(prev => ({ ...prev, payrollPeriodId: row.id, payrollPeriodName: row.stxt || row.name || row.periodText }))}
                    onSearchClick={() => { }}
                    hideDropdownArrow
                    // suffixIcon={<SearchIcon sx={searchIconStyle} />}
                    sx={dropdownSx}
                />
            </ReportFilterField>

            <ReportFilterField label="Payroll Type">
                <LookupDropdown
                    hideLabel
                    value={filters.payrollTypeName}
                    placeholder={translate("Select Payroll Type")}
                    options={lookups?.payrollTypes || []}
                    getOptionLabel={(option) => option.stxt || option.name || ""}
                    onSelect={(row: any) => setFilters(prev => ({ ...prev, payrollTypeId: row.id, payrollTypeName: row.stxt || row.name }))}
                    onSearchClick={() => { }}
                    hideDropdownArrow
                    // suffixIcon={<SearchIcon sx={searchIconStyle} />}
                    sx={dropdownSx}
                />
            </ReportFilterField>

            <ReportFilterField label="Status">
                <LookupDropdown
                    hideLabel
                    value={filters.statusName}
                    placeholder={translate("Select Status")}
                    options={lookups?.statuses || []}
                    getOptionLabel={(option) => option.stxt || option.name || ""}
                    onSelect={(row: any) => setFilters(prev => ({ ...prev, statusId: row.id, statusName: row.stxt || row.name }))}
                    onSearchClick={() => { }}
                    hideDropdownArrow
                    // suffixIcon={<SearchIcon sx={searchIconStyle} />}
                    sx={dropdownSx}
                />
            </ReportFilterField>
        </BaseReportModal>
    );
};
