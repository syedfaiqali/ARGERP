import React from 'react'
import {
    Paper,
    Box,
    Typography,
    List,
    ListItem,
    ListItemButton,
    ListItemText,
    IconButton,
    Grid
} from '@mui/material'
import {
    Star as StarIcon,
    StarBorder as StarBorderIcon,
    ArrowForwardRounded as ArrowIcon,
    Menu as MenuIcon,
    BuildRounded as SetupIcon,
    LocationOnRounded as LocationIcon,
    MeetingRoomRounded as RoomIcon,
    SettingsRounded as SettingsIcon,
    SchoolRounded as AcademicIcon,
    GroupsRounded as StudentIcon,
    BadgeRounded as HumanResourceIcon,
    ReceiptLongRounded as FeeIcon,
    AccountBalanceRounded as FinanceIcon,
    DirectionsBusRounded as TransportIcon,
    LocalLibraryRounded as LibraryIcon,
    ForumRounded as CommunicationIcon,
    SecurityRounded as SecurityIcon,
    Inventory2Rounded as InventoryIcon,
    ApartmentRounded as OrganizationIcon,
    DashboardRounded as DashboardIcon,
} from '@mui/icons-material'
import { motion } from 'framer-motion'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '@/store/store'
import { fetchFavorites, toggleFavorite } from '@/store/favoritesSlice'
import { useLocation } from 'react-router-dom'
import { T } from './AdaptiveTranslationProvider'
import { normalizeMenuName, useHiddenMenuPermissions } from '@/hooks/useHiddenMenuPermissions'
import { LmsToaster } from './LmsToaster'
import { resolveFavoriteRoute } from '@/utils/favoriteNavigation'

interface MenuCardProps {
    title: string;
    items: string[];
    onItemClick?: (item: string) => void;
    color?: string;
    categoryIcon?: React.ReactElement;
    listHeading?: string;
    permissionsPreFiltered?: boolean;
    getItemLabel?: (item: string) => string;
}

interface MenuCardGridItem extends MenuCardProps {
    gridProps?: any;
    hidden?: boolean;
    permissionsPreFiltered?: boolean;
}

interface MenuCardGridProps {
    cards: MenuCardGridItem[];
    spacing?: number;
    gridItemProps?: any;
}

const routeModuleAliases: Record<string, string[]> = {
    "front-office": ["Front Office"],
    org: ["Organization", "Organization Setup"],
    academics: ["Academics", "Academic"],
    student: ["Student", "Student Information", "Student Management"],
    hr: ["Human Resource", "Human Resources", "HR"],
    fees: ["Fee Management", "Fees Management"],
    transport: ["Transport"],
    library: ["Library"],
    comm: ["Communication"],
    "fixed-asset": ["Fixed Asset"],
    inventory: ["Inventory Management", "Inventory", "Item Management"],
    finance: ["Financial Management", "General Ledger", "Cash Management", "Account Receivable"],
    reports: ["Reports"],
    security: ["System Settings", "Security", "User Authorization"],
};

const getCategoryIcon = (title: string): React.ReactElement => {
    const name = normalizeMenuName(title);
    if (name.includes('location') || name.includes('region')) return <LocationIcon />;
    if (name.includes('room') || name.includes('building') || name.includes('floor')) return <RoomIcon />;
    if (name.includes('setting') || name.includes('policy') || name.includes('control')) return <SettingsIcon />;
    if (name.includes('academic') || name.includes('course') || name.includes('exam')) return <AcademicIcon />;
    if (name.includes('student') || name.includes('guardian')) return <StudentIcon />;
    if (name.includes('human resource') || name === 'hr' || name.includes('employee') || name.includes('staff')) return <HumanResourceIcon />;
    if (name.includes('fee') || name.includes('challan')) return <FeeIcon />;
    if (name.includes('finance') || name.includes('account') || name.includes('bank')) return <FinanceIcon />;
    if (name.includes('transport') || name.includes('vehicle')) return <TransportIcon />;
    if (name.includes('library') || name.includes('book')) return <LibraryIcon />;
    if (name.includes('communication') || name.includes('message')) return <CommunicationIcon />;
    if (name.includes('security') || name.includes('authorization')) return <SecurityIcon />;
    if (name.includes('inventory') || name.includes('item')) return <InventoryIcon />;
    if (name.includes('organization') || name.includes('institution')) return <OrganizationIcon />;
    if (name.includes('setup')) return <SetupIcon />;
    return <DashboardIcon />;
};

export const MenuCard: React.FC<MenuCardProps> = ({
    title,
    items,
    onItemClick,
    color = "var(--lms-primary)",
    listHeading,
    categoryIcon,
    permissionsPreFiltered = false,
    getItemLabel,
}) => {
    const dispatch = useDispatch();
    const location = useLocation();
    const favorites = useSelector((state: RootState) => state.favorites.items);
    const [toaster, setToaster] = React.useState<{
        open: boolean;
        messages: string[];
        severity: "success" | "error";
    }>({ open: false, messages: [], severity: "success" });
    const { screenPermissionsLoaded, isScreenHidden, visibleScreenMenuItems, visibleMenuCardGroups } = useHiddenMenuPermissions();
    const visibleItems = React.useMemo(
        () => screenPermissionsLoaded
            ? permissionsPreFiltered
                ? items
                : items.filter((item) => !isScreenHidden(item))
            : [],
        [isScreenHidden, items, permissionsPreFiltered, screenPermissionsLoaded]
    );
    const resolvedCategoryIcon = categoryIcon ?? getCategoryIcon(title);

    React.useEffect(() => {
        void dispatch(fetchFavorites() as any);
    }, [dispatch]);

    const getVisibleMenuItem = React.useCallback((name: string) => {
        const normalizedName = normalizeMenuName(name);
        const normalizedTitle = normalizeMenuName(title);
        const routeSegment = location.pathname
            .split("/")
            .map((part) => part.trim())
            .filter(Boolean)[0];
        const normalizedRouteModules = (routeModuleAliases[routeSegment] ?? [])
            .map((moduleName) => normalizeMenuName(moduleName));

        const matchesName = (item: typeof visibleScreenMenuItems[number]) =>
            normalizeMenuName(item.name) === normalizedName || normalizeMenuName(item.screenKey || '') === normalizedName;
        const pathParts = (item: typeof visibleScreenMenuItems[number]) =>
            item.path
                .split("/")
                .map((part) => normalizeMenuName(part));

        return visibleScreenMenuItems.find((item) =>
            matchesName(item) &&
            normalizedRouteModules.some((moduleName) => pathParts(item).includes(moduleName))
        ) ?? visibleScreenMenuItems.find((item) =>
            matchesName(item) &&
            (
                normalizeMenuName(item.parentName) === normalizedTitle ||
                pathParts(item).includes(normalizedTitle)
            )
        ) ?? visibleScreenMenuItems.find(matchesName);
    }, [location.pathname, title, visibleScreenMenuItems]);

    const getFavoritePath = React.useCallback((name: string) => {
        const moduleSegment = location.pathname
            .split("/")
            .map((part) => part.trim())
            .filter(Boolean)[0];
        const basePath = moduleSegment ? `/${moduleSegment}` : location.pathname || "/";
        return `${basePath}/${encodeURIComponent(name)}`;
    }, [location.pathname]);

    const handleToggleFavorite = (name: string) => {
        const menuItem = getVisibleMenuItem(name);
        const wasFavorite = isFavorite(name);

        const caption = menuItem?.name || getItemLabel?.(name) || name;
        const favorite = { menuId: menuItem?.menuId ?? null, name: caption, path: getFavoritePath(name), category: title };
        const path = resolveFavoriteRoute(favorite, visibleScreenMenuItems, visibleMenuCardGroups) || favorite.path;

        const action = dispatch(toggleFavorite({
            menuId: menuItem?.menuId ?? null,
            name: caption,
            path,
            category: title
        }) as any) as any;

        action
            .unwrap()
            .then(() => {
                setToaster({
                    open: true,
                    messages: [wasFavorite ? `${name} removed from favourites` : `${name} added to favourites`],
                    severity: wasFavorite ? "error" : "success",
                });
            })
            .catch((error: any) => {
                setToaster({
                    open: true,
                    messages: [error?.message || `Failed to update ${name} favourite`],
                    severity: "error",
                });
            });
    };

    const isFavorite = (name: string) => {
        const menuItem = getVisibleMenuItem(name);
        return favorites.some(f => (
            menuItem?.menuId
                ? Number(f.menuId) === Number(menuItem.menuId)
                : f.name === name
        ));
    };

    if (!screenPermissionsLoaded || visibleItems.length === 0) {
        return null;
    }

    return (
        <>
            <motion.div
                whileHover={{ y: -3 }}
                transition={{ type: "spring", stiffness: 300, damping: 25 }}
                style={{ display: 'flex', flex: 1 }}
            >
                <Paper
                elevation={0}
                sx={{
                    flex: 1,
                    width: '100%',
                    maxWidth: 365,
                    minHeight: 0,
                    display: 'flex',
                    flexDirection: 'column',
                    border: `1px solid color-mix(in srgb, ${color} 18%, var(--lms-border))`,
                    borderRadius: '18px',
                    overflow: 'hidden',
                    background: `linear-gradient(145deg, color-mix(in srgb, ${color} 4%, white) 0%, #ffffff 32%, #ffffff 100%)`,
                    position: 'relative',
                    transition: 'all 0.3s ease',
                    boxShadow: '0 10px 26px rgba(15, 23, 42, 0.075)',
                    '&:hover': {
                        boxShadow: `0 20px 38px -18px color-mix(in srgb, ${color} 48%, transparent), 0 12px 22px rgba(15, 23, 42, 0.09)`,
                        borderColor: color,
                        '& .menu-card-header-icon': { transform: 'scale(1.08) rotate(-4deg)' },
                    }
                }}
            >
                {/* Visual Accent */}
                <Box sx={{
                    position: 'absolute',
                    top: 0,
                    right: 0,
                    width: '150px',
                    height: '150px',
                    background: `radial-gradient(circle at top right, color-mix(in srgb, ${color} 13%, transparent), transparent 70%)`,
                    zIndex: 0
                }} />

                {/* Header Section */}
                <Box
                    sx={{
                        px: 1.8,
                        py: 1.15,
                        minHeight: 68,
                        position: 'relative',
                        zIndex: 1,
                        borderBottom: `1px solid color-mix(in srgb, ${color} 13%, var(--lms-border))`,
                        background: `linear-gradient(105deg, color-mix(in srgb, ${color} 10%, white) 0%, rgba(255,255,255,.98) 78%)`,
                        display: 'flex',
                        justifyContent: 'flex-start',
                        alignItems: 'center'
                    }}
                >
                    <Typography
                        variant="h6"
                        sx={{
                            fontWeight: 850,
                            color: 'var(--lms-text-main)',
                            fontSize: '1rem',
                            display: 'flex',
                            alignItems: 'center',
                            gap: 1,
                            letterSpacing: 0
                        }}
                    >
                        <Box className="menu-card-header-icon" sx={{ width: 30, height: 30, borderRadius: 2, display: 'grid', placeItems: 'center', color, bgcolor: `color-mix(in srgb, ${color} 14%, white)`, border: `1px solid color-mix(in srgb, ${color} 22%, transparent)`, transition: 'transform .25s ease', '& .MuiSvgIcon-root': { fontSize: '1rem' } }}>{resolvedCategoryIcon}</Box>
                        <T>{title}</T>
                    </Typography>
                </Box>

                {/* Modules List - Showing ALL items as requested */}
                <Box sx={{ px: 1.15, py: 1.1 }}>
                    {listHeading && (
                        <Typography
                            sx={{
                                fontSize: '0.82rem',
                                fontWeight: 700,
                                color: 'var(--lms-text-main)',
                                px: 1.2,
                                pb: 0.8,
                                borderBottom: '1px solid var(--lms-border)',
                                mb: 0.8
                            }}
                        >
                            <T>{listHeading}</T>
                        </Typography>
                    )}
                    <List sx={{ p: 0 }}>
                        {visibleItems.map((item) => (
                            <ListItem
                                key={item}
                                disablePadding
                                sx={{
                                    mb: 0,
                                    // MUI reserves space for one action; these rows have two.
                                    '& > .MuiListItemButton-root': { paddingInlineEnd: '80px' },
                                }}
                                secondaryAction={
                                    <Box sx={{ display: 'flex', gap: 0.5, alignItems: 'center' }}>
                                        <IconButton
                                            size="small"
                                            className="action-star"
                                            aria-pressed={isFavorite(item)}
                                            aria-label={`${isFavorite(item) ? 'Remove from' : 'Add to'} favourites: ${getItemLabel?.(item) || item}`}
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                handleToggleFavorite(item);
                                            }}
                                            sx={{
                                                opacity: isFavorite(item) ? 1 : 0.3,
                                                transition: '0.2s',
                                                color: isFavorite(item) ? '#FFD700' : 'var(--lms-text-secondary)',
                                                p: 0.35,
                                                '&:hover': { opacity: 1, color: '#FFD700' }
                                            }}
                                        >
                                            {isFavorite(item) ? <StarIcon sx={{ fontSize: '0.9rem', color: '#FFD700' }} /> : <StarBorderIcon sx={{ fontSize: '0.9rem' }} />}
                                        </IconButton>
                                        <IconButton size="small" className="action-menu" sx={{ opacity: 0.24, transition: '0.2s', p: 0.35 }}>
                                            <MenuIcon sx={{ fontSize: '0.9rem', color: 'var(--lms-text-secondary)' }} />
                                        </IconButton>
                                    </Box>
                                }
                            >
                                <ListItemButton
                                    onClick={onItemClick ? () => onItemClick(item) : undefined}
                                    sx={{
                                        borderRadius: '10px',
                                        py: 0.7,
                                        px: 1.35,
                                        minHeight: 42,
                                        border: '1px solid transparent',
                                        '&:hover': {
                                            bgcolor: `color-mix(in srgb, ${color} 9%, white)`,
                                            borderColor: `color-mix(in srgb, ${color} 17%, transparent)`,
                                            '& .item-text': { color: color },
                                            '& .action-star, & .action-menu': { opacity: 0.8 },
                                            '& .arrow-icon': { opacity: 1, transform: 'translateX(0)' }
                                        }
                                    }}
                                >
                                    <ListItemText
                                        primary={<T>{getItemLabel?.(item) ?? item}</T>}
                                        primaryTypographyProps={{
                                            className: 'item-text',
                                            fontSize: '0.86rem',
                                            fontWeight: 750,
                                            color: 'var(--lms-text-main)',
                                            sx: { transition: 'color 0.2s ease' }
                                        }}
                                    />
                                    <ArrowIcon
                                        className="arrow-icon"
                                        sx={{
                                            fontSize: '0.9rem',
                                            color: color,
                                            flexShrink: 0,
                                            marginInlineStart: 1,
                                            opacity: 0,
                                            transform: 'translateX(-5px)',
                                            transition: 'all 0.2s ease'
                                        }}
                                    />
                                </ListItemButton>
                            </ListItem>
                        ))}
                    </List>
                </Box>
                </Paper>
            </motion.div>
            <LmsToaster
                open={toaster.open}
                onClose={() => setToaster((current) => ({ ...current, open: false }))}
                messages={toaster.messages}
                severity={toaster.severity}
                disableDeleteSuccessAutoSeverity
            />
        </>
    )
}

export const MenuCardGrid: React.FC<MenuCardGridProps> = ({
    cards,
    spacing = 3,
    gridItemProps = { xs: 12, sm: 6, md: 3 },
}) => {
    const { screenPermissionsLoaded, isScreenHidden } = useHiddenMenuPermissions();

    const visibleCards = React.useMemo(
        () =>
            screenPermissionsLoaded
                ? cards
                    .filter((card) => !card.hidden)
                    .map((card) => ({
                        ...card,
                        items: card.permissionsPreFiltered
                            ? card.items
                            : card.items.filter((item) => !isScreenHidden(item)),
                    }))
                    .filter((card) => card.items.length > 0)
                : [],
        [cards, isScreenHidden, screenPermissionsLoaded],
    );

    if (!screenPermissionsLoaded || visibleCards.length === 0) {
        return null;
    }

    return (
        <Grid container spacing={spacing} alignItems="flex-start">
            {visibleCards.map(({ gridProps, ...card }) => (
                <Grid item key={card.title} {...gridItemProps} {...gridProps}>
                    <MenuCard {...card} />
                </Grid>
            ))}
        </Grid>
    );
};
