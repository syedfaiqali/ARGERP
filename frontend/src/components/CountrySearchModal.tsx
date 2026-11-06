import React, { useMemo } from 'react';
import { GenericSearchModal } from '@/components/GenericSearchModal';
import { useTranslation } from '@/components/AdaptiveTranslationProvider';
import { GetService, ToCheckArrayTypeAndLength } from '@/constants/globalItems';
import { LmsOverlayLoader } from '@/components/LmsOverlayLoader';

interface CountrySearchModalProps {
    open: boolean;
    onClose: () => void;
    onSelect: (row: any) => void;
    title?: string;
}

// Function to get countries from local storage
export const getGlobalCountries = () => {
    const cached = localStorage.getItem('global_countries');
    if (cached) {
        try {
            const parsed = JSON.parse(cached);
            if (ToCheckArrayTypeAndLength(parsed)) {
                return parsed;
            }
        } catch (e) {
            console.error("Failed to parse global_countries from localStorage", e);
        }
    }
    return [];
};

export const fetchGlobalCountries = async () => {
    try {
        const response: any = await GetService("/api/organization/Country/v3?pageNumber=1&pageSize=1000");
        const payload = response.data ?? response ?? {};
        const items = payload.items ?? payload.Items ?? [];
        if (ToCheckArrayTypeAndLength(items)) {
            const formatted = items.map((item: any) => ({
                code: item.countrycode || item.code || "",
                countryname: item.countryname || item.countryName || "",
                dialcode: item.dialcode || item.dialCode || "",
                currencycode: item.currencycode || "",
                currencyname: item.currencyname || "",
                currencysymbol: item.currencysymbol || "",
                countryid: item.countryid || item.id
            }));
            setGlobalCountries(formatted);
            return formatted;
        }
    } catch (e) {
        console.error("Failed to fetch global countries", e);
    }
    return [];
};

export const setGlobalCountries = (countries: any[]) => {
    localStorage.setItem('global_countries', JSON.stringify(countries));
};

export const CountrySearchModal: React.FC<CountrySearchModalProps> = ({
    open,
    onClose,
    onSelect,
    title
}) => {
    const { translate } = useTranslation();

    const columns = [
        { id: 'code', label: translate('Code'), minWidth: 100 },
        { id: 'countryname', label: translate('Country Name'), minWidth: 200 },
        { id: 'dialcode', label: translate('Dial Code'), minWidth: 150 },
    ];

    const [rows, setRows] = React.useState<any[]>(getGlobalCountries());
    const [loading, setLoading] = React.useState(false);

    React.useEffect(() => {
        const loadCountries = async () => {
            if (open) {
                const current = getGlobalCountries();
                if (current.length > 0) {
                    setRows(current);
                }

                setLoading(true);
                const fetched = await fetchGlobalCountries();
                if (fetched.length > 0) {
                    setRows(fetched);
                }
                setLoading(false);
            }
        };
        loadCountries();
    }, [open]);

    const data = useMemo(() => {
        return rows.map((c: any) => ({
            ...c,
            code: c.code || c.countrycode || "",
            countryname: c.countryname || c.countryName || "",
            dialcode: c.dialcode || c.dialCode || ""
        }));
    }, [rows]);

    return (
        <>
            {loading && <LmsOverlayLoader />}
            <GenericSearchModal
                open={open}
                onClose={onClose}
                title={title || translate("Country")}
                columns={columns}
                data={data}
                onSelect={onSelect}
            />
        </>
    );
};
