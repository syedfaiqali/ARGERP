import { withColorOpacity } from '@/utils/themeColors';
import React from 'react'
import { Box, Typography, Grid, Paper, Link, Breadcrumbs, Card, CardContent, CardActionArea, Dialog, DialogContent, IconButton } from '@mui/material'
import CloseIcon from '@mui/icons-material/Close'
import {
    NavigateNext as NavigateNextIcon,
    Dashboard as DashboardIcon,
    Settings as AdminIcon,
    AccountBalance as OrgIcon,
    School as AcademicsIcon,
    People as HRIcon,
    Payments as FeeIcon,
    AssignmentInd as FrontOfficeIcon,
    BarChart as ReportsIcon,
    CreditCard as StudentIcon,
    Business as BankIcon,
    DirectionsBus as TransportIcon,
    MenuBook as LibraryIcon,
    Campaign as CommIcon,
    Group as FinanceIcon,
    EmojiEvents as AwardsIcon,
    Calculate as MathIcon,
    Palette as ArtIcon,
    SportsSoccer as SportsIcon,
    AutoStories as StoriesIcon,
    RecordVoiceOver as SpeechIcon,
    Science as ScienceIcon,
    HistoryEdu as HistoryIcon,
    MusicNote as MusicIcon,
    Public as GeographyIcon,
    Biotech as BiotechIcon,
    Terminal as ITIcon,
    Translate as LangIcon,
    Code as CodeIcon,
    Psychology as BrainIcon,
    Gavel as LawIcon,
    Architecture as PhysicsIcon,
    FitnessCenter as PEIcon,
    Backpack as SchoolBagIcon,
    Memory as LogicIcon,
    Inventory2 as InventoryIcon,
    FlightTakeoff as FreightIcon,
} from '@mui/icons-material'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Layout } from '@/components/Layout'
import { motion } from 'framer-motion'

import { T, useTranslation } from '@/components/AdaptiveTranslationProvider'
import { getLanguageIntlLocale } from '@/constants/languages'
import { useHiddenMenuPermissions } from "@/hooks/useHiddenMenuPermissions";
import { useAppSelector } from "@/hooks";


const SchoolFloatingIcon = ({ icon: Icon, top, left, delay, duration = 20, size = '6rem', rotate = 360, blur = 0, opacity = 0.02 }: any) => (
    <Box
        component={motion.div}
        animate={{
            y: [0, -60, 20, 0],
            x: [0, 30, -30, 0],
            rotate: [0, rotate * 0.15, -rotate * 0.15, 0],
            opacity: [opacity, opacity * 2, opacity],
            scale: [1, 1.2, 0.9, 1],
        }}
        transition={{
            duration: duration,
            repeat: Infinity,
            delay: delay,
            ease: "easeInOut"
        }}
        sx={{
            position: 'absolute',
            top: top,
            left: left,
            zIndex: 0,
            pointerEvents: 'none',
            color: 'var(--lms-primary)',
            filter: blur > 0 ? `blur(${blur}px)` : 'none',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
        }}
    >
        <Icon sx={{ fontSize: size }} />
    </Box>
);

const Particle = ({ top, left, delay }: any) => (
    <Box
        component={motion.div}
        animate={{
            y: [0, -100],
            opacity: [0, 0.3, 0],
            scale: [0, 1.5, 0]
        }}
        transition={{
            duration: 4 + Math.random() * 4,
            repeat: Infinity,
            delay: delay,
            ease: "linear"
        }}
        sx={{
            position: 'absolute',
            top: top,
            left: left,
            width: 4,
            height: 4,
            borderRadius: '50%',
            bgcolor: 'var(--lms-primary)',
            zIndex: 0,
            pointerEvents: 'none',
        }}
    />
);

export const HomePage: React.FC = () => {
    const navigate = useNavigate()
    const [searchParams] = useSearchParams()
    const [videoOpen, setVideoOpen] = React.useState(false)
    const { isMenuAllowed } = useHiddenMenuPermissions();
    const { user, selectedCampusId, campuses } = useAppSelector((state) => state.auth);
    const selectedEntityId = Number(
        selectedCampusId ||
        localStorage.getItem("selectedEntity") ||
        localStorage.getItem("selectedEntityId") ||
        localStorage.getItem("selectedCampusId") ||
        user?.entityId ||
        0,
    );
    const selectedEntity =
        campuses.find((campus) => Number(campus.id) === selectedEntityId) ||
        (campuses.length === 1 ? campuses[0] : undefined);
    const selectedEntityName = "Al Rahim Group";
    const selectedEntityTitle = `${selectedEntityName}`;
    const { translate, language } = useTranslation();
    // The school name sits inside the sentence, and its position differs by language
    // (Japanese puts it first), so the whole sentence is one key with a {name} slot.
    const welcomeTitle = translate('Welcome to {name}').replace('{name}', selectedEntityTitle);
    // Languages with their own locale get a native date; the others keep the browser's.
    const currentDate = new Date().toLocaleDateString(getLanguageIntlLocale(language), {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    });

    const sections = [{ title: 'Al Rahim Group', modules: [
        { text: 'Human Resource', icon: <HRIcon />, path: '/modules/hr', color: '#f97316', description: 'Staff & Payroll' },
        { text: 'Organization Setup', icon: <OrgIcon />, path: '/modules/organization', color: '#0d9488', description: 'Branch Management' },
        { text: 'Auth', icon: <AdminIcon />, path: '/modules/auth', color: 'var(--lms-primary-dark)', description: 'Users & Account Settings' },
    ] }];

    React.useEffect(() => {
        const view = searchParams.get('view')
        if (view) {
            console.log("Searching from home:", view)
        }
    }, [searchParams])

    return (
        <Layout>
            <Box sx={{
                flexGrow: 1,
                mt: 0,
                position: 'relative',
                overflow: 'hidden',
                minHeight: '100vh',
                background: 'radial-gradient(circle at 50% 50%, rgba(var(--lms-primary-rgb), 0.02) 0%, rgba(255, 255, 255, 1) 100%)',
            }}>

                {/* School-specific Background Elements */}
                <SchoolFloatingIcon icon={MathIcon} top="5%" left="8%" delay={0} size="8rem" opacity={0.12} />
                <SchoolFloatingIcon icon={ScienceIcon} top="15%" left="45%" delay={2} duration={25} size="7rem" opacity={0.10} />
                <SchoolFloatingIcon icon={HistoryIcon} top="12%" left="82%" delay={4} duration={30} size="9rem" opacity={0.10} />
                <SchoolFloatingIcon icon={ArtIcon} top="75%" left="5%" delay={5} duration={18} size="6rem" opacity={0.11} />
                <SchoolFloatingIcon icon={MusicIcon} top="45%" left="92%" delay={1} duration={22} size="7rem" opacity={0.10} />
                <SchoolFloatingIcon icon={GeographyIcon} top="88%" left="15%" delay={3} duration={26} size="8rem" opacity={0.09} />
                <SchoolFloatingIcon icon={BiotechIcon} top="65%" left="78%" delay={7} duration={32} size="10rem" opacity={0.08} />
                <SchoolFloatingIcon icon={LangIcon} top="30%" left="35%" delay={9} duration={20} size="6rem" opacity={0.09} />
                <SchoolFloatingIcon icon={CodeIcon} top="55%" left="12%" delay={11} duration={28} size="7rem" opacity={0.10} />
                <SchoolFloatingIcon icon={SchoolBagIcon} top="82%" left="45%" delay={13} duration={24} size="8rem" opacity={0.09} />

                {/* Additional Right-side Icons */}
                <SchoolFloatingIcon icon={MathIcon} top="25%" left="85%" delay={2} duration={25} size="5rem" opacity={0.10} />
                <SchoolFloatingIcon icon={PhysicsIcon} top="55%" left="94%" delay={5} duration={35} size="6rem" opacity={0.09} />
                <SchoolFloatingIcon icon={LogicIcon} top="80%" left="88%" delay={8} duration={20} size="4rem" opacity={0.08} />

                {/* Midground Layer */}
                <SchoolFloatingIcon icon={AcademicsIcon} top="35%" left="12%" delay={1.5} duration={35} size="5rem" opacity={0.08} />
                <SchoolFloatingIcon icon={SportsIcon} top="85%" left="80%" delay={2.5} duration={22} size="6rem" opacity={0.09} />
                <SchoolFloatingIcon icon={StoriesIcon} top="52%" left="3%" delay={6} duration={28} size="4rem" opacity={0.08} />
                <SchoolFloatingIcon icon={SpeechIcon} top="62%" left="88%" delay={0.5} duration={24} size="5rem" opacity={0.09} />
                <SchoolFloatingIcon icon={AwardsIcon} top="25%" left="65%" delay={8} duration={40} size="7rem" opacity={0.08} />
                <SchoolFloatingIcon icon={ITIcon} top="70%" left="40%" delay={4} duration={38} size="6rem" opacity={0.07} />
                <SchoolFloatingIcon icon={BrainIcon} top="5%" left="60%" delay={14} duration={30} size="5rem" opacity={0.08} />
                <SchoolFloatingIcon icon={PEIcon} top="40%" left="75%" delay={16} duration={26} size="6rem" opacity={0.08} />
                <SchoolFloatingIcon icon={CodeIcon} top="15%" left="90%" delay={10} duration={30} size="4rem" opacity={0.08} />

                {/* Background Layer (Blurred) */}
                <SchoolFloatingIcon icon={MathIcon} top="20%" left="25%" delay={10} duration={50} size="12rem" blur={4} opacity={0.06} />
                <SchoolFloatingIcon icon={ScienceIcon} top="60%" left="35%" delay={12} duration={45} size="10rem" blur={6} opacity={0.05} />
                <SchoolFloatingIcon icon={HistoryIcon} top="80%" left="55%" delay={15} duration={60} size="15rem" blur={8} opacity={0.04} />
                <SchoolFloatingIcon icon={PhysicsIcon} top="10%" left="70%" delay={18} duration={55} size="11rem" blur={5} opacity={0.05} />
                <SchoolFloatingIcon icon={LawIcon} top="85%" left="10%" delay={20} duration={65} size="13rem" blur={7} opacity={0.04} />
                <SchoolFloatingIcon icon={LogicIcon} top="40%" left="85%" delay={22} duration={48} size="9rem" blur={4} opacity={0.05} />
                <SchoolFloatingIcon icon={GeographyIcon} top="50%" left="80%" delay={25} duration={55} size="10rem" blur={5} opacity={0.05} />

                {/* Animated Particles */}
                {[...Array(15)].map((_, i) => (
                    <Particle
                        key={i}
                        top={`${Math.random() * 100}%`}
                        left={`${Math.random() * 100}%`}
                        delay={Math.random() * 5}
                    />
                ))}

                <Paper
                    elevation={0}
                    sx={{
                        py: 0.8,
                        px: { xs: 1.5, sm: 3 },
                        minHeight: 66,
                        mb: 2,
                        background: 'linear-gradient(105deg, color-mix(in srgb, var(--lms-primary) 10%, white) 0%, rgba(255, 255, 255, 0.98) 72%)',
                        backdropFilter: 'blur(12px)',
                        border: '1px solid color-mix(in srgb, var(--lms-primary) 22%, transparent)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        borderRadius: '18px',
                        boxShadow: '0 5px 18px rgba(31, 65, 49, 0.045)',
                        position: 'sticky',
                        top: '0.1% !important',
                        zIndex: 100,
                    }}
                >
                    <Breadcrumbs
                        separator={<NavigateNextIcon fontSize="small" sx={{ color: 'var(--lms-text-secondary)' }} />}
                        sx={{
                            color: 'var(--lms-text-main)',
                            '& .MuiBreadcrumbs-li': { fontSize: '0.82rem', fontWeight: 700 }
                        }}
                    >
                        <Link underline="hover" sx={{ display: 'flex', alignItems: 'center', color: 'var(--lms-text-main)' }}>
                            <DashboardIcon sx={{ mr: 0.5, fontSize: '1rem', color: 'var(--lms-primary)' }} />
                            <T>Home Portal</T>
                        </Link>
                    </Breadcrumbs>

                    {/* <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', pr: 1 }}>
                        <Button
                            size="small"
                            variant="contained"
                            sx={{
                                bgcolor: 'rgba(var(--lms-primary-rgb), 0.1)',
                                color: 'var(--lms-primary-dark)',
                                boxShadow: 'none',
                                fontSize: '10px',
                                fontWeight: 700,
                                px: 1.5,
                                py: 0.5,
                                borderRadius: 1.5,
                                '&:hover': { bgcolor: 'rgba(var(--lms-primary-rgb), 0.2)', boxShadow: 'none' }
                            }}
                        >
                            <T>Support Portal</T>
                        </Button>
                    </Box> */}
                </Paper>

                <Box
                    component={motion.div}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    sx={{ p: 4, width: '100%', position: 'relative', zIndex: 1 }}
                >
                    <Box sx={{ mb: 6 }} component={motion.div} initial={{ y: -20, opacity: 0 }} animate={{ y: 0, opacity: 1 }} transition={{ type: 'spring', damping: 20 }}>
                        <Typography variant="h3" sx={{ fontWeight: 900, color: 'var(--lms-text-main)', mb: 1, letterSpacing: '-0.04em', display: 'flex', alignItems: 'center', gap: 2 }}>
                            <>{welcomeTitle}</>
                            <motion.span
                                animate={{
                                    rotate: [0, 20, 0, 20, 0],
                                    scale: [1, 1.2, 1]
                                }}
                                transition={{ repeat: Infinity, duration: 2.5, repeatDelay: 1.5 }}
                                style={{ display: 'inline-block', originX: 0.7, originY: 0.7 }}
                            >
                                👋
                            </motion.span>
                        </Typography>
                        <Typography variant="h6" sx={{ color: 'var(--lms-primary)', fontWeight: 600, letterSpacing: '0.01em', opacity: 0.8 }}>
                            <T>The unified operating system for learning, operations, and teams</T> - {currentDate}
                        </Typography>
                    </Box>

                    {/* Featured Section */}
                    <Box
                        component={motion.div}
                        initial={{ opacity: 0, y: 40 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.2 }}
                        sx={{ mb: 8, position: 'relative' }}
                    >
                        <Box sx={{
                            position: 'absolute',
                            top: '-20%',
                            left: '50%',
                            transform: 'translateX(-50%)',
                            width: '90%',
                            height: '140%',
                            background: 'radial-gradient(circle, rgba(var(--lms-primary-rgb), 0.1) 0%, rgba(255,255,255,0) 80%)',
                            zIndex: -1,
                            filter: 'blur(60px)'
                        }} />

                        <Card
                            elevation={0}
                            sx={{
                                borderRadius: { xs: 4, md: 6 },
                                overflow: 'hidden',
                                border: '1px solid rgba(255, 255, 255, 0.1)',
                                background: 'linear-gradient(135deg, var(--lms-primary) 0%, var(--lms-primary-dark) 100%)',
                                color: '#fff',
                                position: 'relative',
                                display: 'flex',
                                alignItems: 'center',
                                boxShadow: '0 40px 80px -20px rgba(var(--lms-primary-dark-rgb), 0.35)',
                                transition: 'transform 0.3s ease',
                                '&:hover': { transform: 'translateY(-5px)' }
                            }}
                        >
                            <Box sx={{
                                display: 'flex',
                                flexDirection: { xs: 'column', lg: 'row' },
                                alignItems: 'center',
                                width: '100%',
                                p: { xs: 3, sm: 5, lg: 6 },
                                gap: { xs: 4, lg: 8 }
                            }}>
                                <Box sx={{ flex: { xs: 'none', lg: 1 }, zIndex: 1, textAlign: { xs: 'center', lg: 'left' }, width: '100%' }}>
                                    <motion.div initial={{ x: -30, opacity: 0 }} animate={{ x: 0, opacity: 1 }} transition={{ delay: 0.5 }}>
                                        <Box sx={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: { xs: 'center', lg: 'flex-start' },
                                            gap: 1.5,
                                            mb: 2
                                        }}>
                                            <AwardsIcon sx={{ fontSize: '2rem', opacity: 0.9 }} />
                                            <Typography variant="overline" sx={{ letterSpacing: '0.4em', fontWeight: 950, opacity: 0.8, fontSize: '0.7rem' }}>
                                                <T>Quick Start</T>
                                            </Typography>
                                        </Box>
                                        <Typography variant="h3" sx={{
                                            fontWeight: 900,
                                            mb: 2,
                                            fontSize: { xs: '1.75rem', sm: '2.5rem', lg: '3.5rem' },
                                            letterSpacing: '-0.03em',
                                            lineHeight: 1.1
                                        }}>
                                            <T>Make every workday simpler</T>
                                        </Typography>
                                        <Typography variant="h6" sx={{
                                            opacity: 0.9,
                                            maxWidth: { xs: '100%', lg: 500 },
                                            fontWeight: 500,
                                            lineHeight: 1.6,
                                            fontSize: { xs: '0.9rem', sm: '1.1rem' }
                                        }}>
                                            <T>Take a short guided tour of learning, operations, people, finance, and the tools your team needs.</T>
                                        </Typography>
                                        <Box sx={{
                                            display: 'flex',
                                            flexWrap: 'wrap',
                                            justifyContent: { xs: 'center', lg: 'flex-start' },
                                            gap: 1.25,
                                            mt: 3,
                                        }}>
                                            {['4-minute overview', 'One workspace', 'Built for your team'].map((item) => (
                                                <Box
                                                    key={item}
                                                    sx={{
                                                        px: 1.5,
                                                        py: 0.8,
                                                        borderRadius: 99,
                                                        fontSize: '0.78rem',
                                                        fontWeight: 800,
                                                        letterSpacing: '0.02em',
                                                        bgcolor: 'rgba(255,255,255,0.13)',
                                                        border: '1px solid rgba(255,255,255,0.2)',
                                                        backdropFilter: 'blur(8px)',
                                                    }}
                                                >
                                                    <T>{item}</T>
                                                </Box>
                                            ))}
                                        </Box>
                                    </motion.div>
                                </Box>

                                <Box
                                    component={motion.div}
                                    initial={{ opacity: 0, scale: 0.95, y: 20 }}
                                    animate={{ opacity: 1, scale: 1, y: 0 }}
                                    transition={{ delay: 0.6, type: 'spring', damping: 15 }}
                                    sx={{
                                        flex: { xs: 'none', lg: 1.4 },
                                        width: '100%',
                                        maxWidth: { xs: '100%', lg: '700px' },
                                        aspectRatio: '16/9',
                                        borderRadius: { xs: 3, md: 5 },
                                        overflow: 'hidden',
                                        boxShadow: '0 30px 60px rgba(0,0,0,0.4)',
                                        border: '6px solid rgba(255,255,255,0.15)',
                                        position: 'relative',
                                        bgcolor: '#000',
                                        transform: { lg: 'perspective(1000px) rotateY(-5deg)' },
                                        transition: 'transform 0.5s ease',
                                        '&:hover': {
                                            transform: { lg: 'perspective(1000px) rotateY(0deg) scale(1.02)' }
                                        }
                                    }}
                                >
                                    <iframe
                                        src="https://embed.app.guidde.com/playbooks/5zfn1A8a2HEUMpZpu1uK5H?mode=videoOnly"
                                        title="EDXS ERP Master Guide"
                                        width="100%"
                                        height="100%"
                                        frameBorder="0"
                                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                                        allowFullScreen
                                        style={{ border: 'none' }}
                                    ></iframe>
                                </Box>
                            </Box>
                        </Card>
                    </Box>

                    {sections.map((section, sectionIdx) => (
                        <Box key={section.title} sx={{ mb: 8 }}>
                            <Box
                                component={motion.div}
                                initial={{ opacity: 0, x: -40 }}
                                whileInView={{ opacity: 1, x: 0 }}
                                viewport={{ once: true, margin: "-100px" }}
                                transition={{ duration: 0.7, delay: sectionIdx * 0.1 }}
                                sx={{ display: 'flex', alignItems: 'center', gap: 3, mb: 4 }}
                            >
                                <Typography variant="h5" sx={{ fontWeight: 900, color: 'var(--lms-text-main)', letterSpacing: '0.04em', display: 'flex', alignItems: 'center', gap: 2 }}>
                                    <Box sx={{ width: 12, height: 32, bgcolor: 'var(--lms-primary)', borderRadius: '4px' }} />
                                    <T>{section.title}</T>
                                </Typography>
                                <Box sx={{ flexGrow: 1, height: '2px', background: 'linear-gradient(90deg, var(--lms-border) 0%, transparent 100%)' }} />
                            </Box>

                            <Grid container spacing={4}>
                                {section.modules.map((module, idx) => (
                                    <Grid
                                        item xs={12} sm={6} md={4} key={module.text}
                                        component={motion.div}
                                        initial={{ opacity: 0, y: 50 }}
                                        whileInView={{ opacity: 1, y: 0 }}
                                        viewport={{ once: true, margin: "-50px" }}
                                        transition={{ duration: 0.6, delay: (sectionIdx * 0.1) + (idx * 0.08) }}
                                    >
                                        <Card
                                            elevation={0}
                                            component={motion.div}
                                            whileHover={{
                                                y: -12,
                                                scale: 1.02,
                                                boxShadow: `0 30px 60px -12px ${withColorOpacity(module.color, 0.1451)}`,
                                            }}
                                            sx={{
                                                borderRadius: 4,
                                                border: '1px solid var(--lms-border)',
                                                background: 'rgba(255, 255, 255, 0.8)',
                                                backdropFilter: 'blur(4px)',
                                                transition: 'all 0.4s ease',
                                                height: '100%',
                                                cursor: 'pointer',
                                                overflow: 'hidden',
                                                '&:hover': { borderColor: module.color },
                                                '&:hover .icon-box': {
                                                    transform: 'translateY(-5px) scale(1.1)',
                                                    bgcolor: `${withColorOpacity(module.color, 0.1451)}`
                                                }
                                            }}
                                        >
                                            <CardActionArea onClick={() => navigate(module.path)} sx={{ height: '100%' }}>
                                                <CardContent sx={{ px: 4, py: 4 }}>
                                                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                                                        <Box
                                                            className="icon-box"
                                                            sx={{
                                                                color: module.color,
                                                                display: 'flex',
                                                                p: 2.5,
                                                                width: 'fit-content',
                                                                borderRadius: '20px',
                                                                bgcolor: `${withColorOpacity(module.color, 0.0824)}`,
                                                                transition: 'all 0.3s ease',
                                                            }}
                                                        >
                                                            {React.cloneElement(module.icon as React.ReactElement, { sx: { fontSize: 40 } })}
                                                        </Box>
                                                        <Box sx={{ flex: 1 }}>
                                                            <Typography variant="h5" sx={{ fontWeight: 900, color: 'var(--lms-text-main)', mb: 1 }}>
                                                                <T>{module.text}</T>
                                                            </Typography>
                                                            <Typography variant="body1" sx={{ color: 'var(--lms-text-secondary)', lineHeight: 1.6, fontWeight: 500, opacity: 0.8 }}>
                                                                <T>{module.description}</T>
                                                            </Typography>
                                                        </Box>

                                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: module.color, opacity: 0.6, mt: 1 }}>
                                                            <Typography variant="caption" sx={{ fontWeight: 800, textTransform: 'uppercase' }}>{/* An app module here, not the academic one, so the Module→Course swap must not apply. */}
                                                                <span data-terminology-skip>{translate('Open Module')}</span></Typography>
                                                            <NavigateNextIcon fontSize="small" />
                                                        </Box>
                                                    </Box>
                                                </CardContent>
                                            </CardActionArea>
                                        </Card>
                                    </Grid>
                                ))}
                            </Grid>
                        </Box>
                    ))}
                </Box>
            </Box>

            <Dialog
                open={videoOpen}
                onClose={() => setVideoOpen(false)}
                maxWidth="lg"
                fullWidth
                PaperProps={{
                    sx: {
                        borderRadius: 5,
                        bgcolor: 'var(--lms-card-bg)',
                        backgroundImage: 'none',
                        overflow: 'hidden',
                        position: 'relative',
                        boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.5)',
                        border: '1px solid var(--lms-border)',
                    }
                }}
            >
                <IconButton
                    onClick={() => setVideoOpen(false)}
                    sx={{
                        position: 'absolute',
                        right: 16,
                        top: 16,
                        color: 'var(--lms-text-main)',
                        bgcolor: 'rgba(255, 255, 255, 0.8)',
                        backdropFilter: 'blur(4px)',
                        zIndex: 10,
                    }}
                >
                    <CloseIcon />
                </IconButton>
                <DialogContent sx={{ p: 0, height: '75vh', overflow: 'hidden', bgcolor: '#000' }}>
                    <iframe
                        src="https://embed.app.guidde.com/playbooks/5zfn1A8a2HEUMpZpu1uK5H?mode=videoOnly"
                        title="EDXS Master Guide"
                        width="100%"
                        height="100%"
                        frameBorder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowFullScreen
                    ></iframe>
                </DialogContent>
            </Dialog>
        </Layout>
    )
}
