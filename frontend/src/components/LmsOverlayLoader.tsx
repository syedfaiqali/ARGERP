import React, { useEffect, useState } from 'react';
import { Box, Typography, CircularProgress, Theme } from '@mui/material';
import { motion, AnimatePresence } from 'framer-motion';
import {
    CloudSync as SyncIcon,
    CheckCircle as CheckIcon,
    Refresh as RefreshIcon,
} from '@mui/icons-material';
import logoImage from "@/assets/xs_dark_big.png";
import { useAppSelector } from '@/hooks';
import { RootState } from '@/store/store';
import { isRtlLanguage } from '@/constants/languages';
import { useOptionalTranslation } from '@/components/AdaptiveTranslationProvider';

interface LmsOverlayLoaderProps {
    message?: string;
    showProgress?: boolean;
    variant?: "fullscreen" | "screen" | "container" | "button";
    zIndex?: number;
    open?: boolean;
}

const FloatingIcon = ({ delay, Icon }: { delay: number; Icon: any }) => (
    <Box
        component={motion.div}
        initial={{ y: 0, opacity: 0.3 }}
        animate={{
            y: [-20, 20, -20],
            opacity: [0.3, 0.6, 0.3],
            rotate: [0, 360],
        }}
        transition={{
            duration: 4,
            repeat: Infinity,
            delay,
            ease: "easeInOut"
        }}
        sx={{
            position: 'absolute',
            color: 'var(--lms-primary)',
            filter: 'blur(1px)',
        }}
    >
        <Icon sx={{ fontSize: '2rem' }} />
    </Box>
);

export const LmsOverlayLoader: React.FC<LmsOverlayLoaderProps> = ({
    message = "Processing...",
    showProgress = true,
    variant = "screen",
    zIndex,
    open = true,
}) => {
    const { translate } = useOptionalTranslation();
    const [dots, setDots] = useState('');
    const sidebarOpen = useAppSelector((state: RootState) => state.layout.sidebarOpen);
    const isRTL = useAppSelector((state: RootState) => isRtlLanguage(state.settings.language));
    const isScreenLoader = variant === "screen";
    const isFullscreenLoader = variant === "fullscreen";
    const sidebarWidth = sidebarOpen ? 240 : 64;

    useEffect(() => {
        const interval = setInterval(() => {
            setDots(prev => prev.length >= 3 ? '' : prev + '.');
        }, 500);
        return () => clearInterval(interval);
    }, []);

    if (!open) return null;

    if (variant === "button") {
        return <CircularProgress size={16} color="inherit" />;
    }

    return (
        <AnimatePresence>
            <Box
                component={motion.div}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                sx={{
                    position: variant === "container" ? "absolute" : "fixed",
                    top: isScreenLoader ? { xs: 58, md: 64 } : 0,
                    bottom: isScreenLoader ? 0 : 'auto',
                    left: isScreenLoader
                        ? (isRTL ? 0 : { xs: 0, md: `${sidebarWidth}px` })
                        : 0,
                    right: isScreenLoader
                        ? (isRTL ? { xs: 0, md: `${sidebarWidth}px` } : 0)
                        : 'auto',
                    width: isScreenLoader ? 'auto' : (isFullscreenLoader ? "100vw" : "100%"),
                    height: isScreenLoader ? 'auto' : (isFullscreenLoader ? "100vh" : "100%"),
                    // Screen loaders should blend with the LMS palette and
                    // leave the header/sidebar available while a page loads.
                    bgcolor: isFullscreenLoader
                        ? 'color-mix(in srgb, var(--lms-primary) 18%, transparent)'
                        : 'color-mix(in srgb, var(--lms-primary) 13%, transparent)',
                    backdropFilter: 'blur(8px) saturate(115%)',
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    alignItems: 'center',
                    zIndex: zIndex ?? (isFullscreenLoader ? 99999999 : isScreenLoader ? ((theme: Theme) => theme.zIndex.appBar - 1) : 3),
                    overflow: 'hidden'
                }}
            >
                {/* Floating Background Icons */}
                <Box sx={{ position: 'absolute', width: '100%', height: '100%', opacity: 0.1 }}>
                    <Box sx={{ position: 'absolute', top: '20%', left: '15%' }}>
                        <FloatingIcon delay={0} Icon={SyncIcon} />
                    </Box>
                    <Box sx={{ position: 'absolute', top: '60%', right: '20%' }}>
                        <FloatingIcon delay={1} Icon={CheckIcon} />
                    </Box>
                    <Box sx={{ position: 'absolute', bottom: '25%', left: '25%' }}>
                        <FloatingIcon delay={2} Icon={RefreshIcon} />
                    </Box>
                </Box>

                {/* Main Loading Container */}
                <Box
                    component={motion.div}
                    initial={{ scale: 0.8, opacity: 0 }}
                    animate={{ scale: 1, opacity: 1 }}
                    transition={{ duration: 0.3, ease: "easeOut" }}
                    sx={{
                        position: 'relative',
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                        gap: 3,
                        p: 5,
                        bgcolor: 'rgba(255, 255, 255, 0.95)',
                        borderRadius: 4,
                        boxShadow: '0 20px 60px rgba(0, 0, 0, 0.15), 0 0 0 1px color-mix(in srgb, var(--lms-primary) 10%, transparent)',
                        backdropFilter: 'blur(20px)',
                        border: '1px solid rgba(255, 255, 255, 0.8)',
                        minWidth: 320,
                        maxWidth: 400,
                    }}
                >
                    {/* Animated Spinner Container */}
                    <Box sx={{ position: 'relative', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        {/* Outer Rotating Ring */}
                        <Box
                            component={motion.div}
                            animate={{ rotate: 360 }}
                            transition={{ duration: 3, repeat: Infinity, ease: "linear" }}
                            sx={{
                                position: 'absolute',
                                width: 100,
                                height: 100,
                                borderRadius: '50%',
                                border: '2px dashed color-mix(in srgb, var(--lms-primary) 20%, transparent)',
                            }}
                        />

                        {/* Pulsing Glow */}
                        <Box
                            component={motion.div}
                            animate={{
                                scale: [1, 1.2, 1],
                                opacity: [0.3, 0.6, 0.3]
                            }}
                            transition={{ duration: 2, repeat: Infinity, ease: "easeInOut" }}
                            sx={{
                                position: 'absolute',
                                width: 80,
                                height: 80,
                                borderRadius: '50%',
                                background: 'radial-gradient(circle, var(--lms-primary) 0%, transparent 70%)',
                                opacity: 0.3,
                            }}
                        />

                        {/* Main Spinner */}
                        {showProgress && (
                            <CircularProgress
                                size={70}
                                thickness={3}
                                sx={{
                                    color: 'var(--lms-primary)',
                                    filter: 'drop-shadow(0 0 8px color-mix(in srgb, var(--lms-primary) 30%, transparent))',
                                }}
                            />
                        )}

                        {/* Center Icon */}
                        {/* Center Logo */}
                        <Box
                            component={motion.div}
                            animate={{
                                scale: [1, 1.1, 1],
                            }}
                            transition={{ duration: 2, repeat: Infinity, ease: "easeInOut" }}
                            sx={{
                                position: 'absolute',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                width: 50,
                                height: 50,
                                borderRadius: '50%',
                                bgcolor: 'white',
                                boxShadow: '0 4px 12px color-mix(in srgb, var(--lms-primary) 20%, transparent)',
                                p: 1
                            }}
                        >
                            <img
                                src={logoImage}
                                alt="Loading"
                                style={{
                                    width: '100%',
                                    height: '100%',
                                    objectFit: 'contain'
                                }}
                            />
                        </Box>
                    </Box>

                    {/* Loading Message */}
                    <Box sx={{ textAlign: 'center' }}>
                        <Typography
                            variant="h6"
                            sx={{
                                fontWeight: 700,
                                color: 'var(--lms-text-main)',
                                letterSpacing: '-0.01em',
                                mb: 0.5,
                            }}
                        >
                            {translate(message)}
                            <Box
                                component="span"
                                sx={{
                                    display: 'inline-block',
                                    width: 30,
                                    textAlign: 'left',
                                    color: 'var(--lms-primary)',
                                }}
                            >
                                {dots}
                            </Box>
                        </Typography>
                        <Typography
                            variant="caption"
                            sx={{
                                color: 'var(--lms-text-secondary)',
                                fontWeight: 600,
                                opacity: 0.7,
                                letterSpacing: 1,
                                textTransform: 'uppercase',
                            }}
                        >
                            {translate("Please wait")}
                        </Typography>
                    </Box>

                    {/* Progress Bar */}
                    <Box
                        sx={{
                            width: '100%',
                            height: 4,
                            bgcolor: 'rgba(0, 0, 0, 0.05)',
                            borderRadius: 2,
                            overflow: 'hidden',
                            border: '1px solid rgba(0, 0, 0, 0.05)',
                        }}
                    >
                        <Box
                            component={motion.div}
                            animate={{
                                x: ['-100%', '100%'],
                            }}
                            transition={{
                                duration: 1.5,
                                repeat: Infinity,
                                ease: "easeInOut"
                            }}
                            sx={{
                                width: '50%',
                                height: '100%',
                                background: 'linear-gradient(90deg, transparent, var(--lms-primary), transparent)',
                                boxShadow: '0 0 8px var(--lms-primary)',
                            }}
                        />
                    </Box>
                </Box>

                {/* Subtle Corner Accents */}
                <Box
                    component={motion.div}
                    animate={{ opacity: [0.3, 0.6, 0.3] }}
                    transition={{ duration: 2, repeat: Infinity, ease: "easeInOut" }}
                    sx={{
                        position: 'absolute',
                        top: 20,
                        right: 20,
                        width: 60,
                        height: 60,
                        borderTop: '2px solid var(--lms-primary)',
                        borderRight: '2px solid var(--lms-primary)',
                        borderRadius: '0 8px 0 0',
                        opacity: 0.2,
                    }}
                />
                <Box
                    component={motion.div}
                    animate={{ opacity: [0.3, 0.6, 0.3] }}
                    transition={{ duration: 2, repeat: Infinity, ease: "easeInOut", delay: 1 }}
                    sx={{
                        position: 'absolute',
                        bottom: 20,
                        left: 20,
                        width: 60,
                        height: 60,
                        borderBottom: '2px solid var(--lms-primary)',
                        borderLeft: '2px solid var(--lms-primary)',
                        borderRadius: '0 0 0 8px',
                        opacity: 0.2,
                    }}
                />
            </Box>
        </AnimatePresence>
    );
};
