import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  IconButton,
  InputAdornment,
  Alert,
  Checkbox,
  FormControlLabel,
  Stack,
  Avatar,
} from "@mui/material";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";
import {
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Person as PersonIcon,
  Lock as LockIcon,
  KeyboardArrowRight as ArrowIcon,
  ArrowBack as BackIcon,
  AutoStories as BookIcon,
  School as SchoolIcon,
  Computer as ComputerIcon,
  Science as ScienceIcon,
  LightbulbOutlined as IdeaIcon,
  AssignmentOutlined as TaskIcon,
  BusinessCenter as BusinessIcon,
  AccountTree as WorkflowIcon,
  Groups as TeamsIcon,
  Analytics as AnalyticsIcon,
  Domain as OrganizationIcon,
} from "@mui/icons-material";
import { motion, AnimatePresence } from 'framer-motion';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@/hooks';
import { loginSuccess, setCampus } from '@/store/authSlice';
import api from '@/services/api';
import argLogoImage from '@/assets/ARG.png';
const schema = yup.object({ username:yup.string().required('Username is required'), password:yup.string().required('Password is required') });
interface LoginFormData { username:string;password:string }
const slogans = [
  {
    title: "Learning Meets Operations.",
    sub: "Run campuses, training centers, and corporate teams through one secure ERP workspace.",
    accent: "var(--lms-primary)",
  },
  {
    title: "One Platform. Every Workflow.",
    sub: "Unify LMS, ERP, operations, communication, analytics, and team management across education and corporate environments.",
    accent: "#34d399",
  },
  {
    title: "Built for Every Team.",
    sub: "A unified digital ecosystem for educators, learners, managers, and administrators.",
    accent: "#60a5fa",
  },
];

type LoginBrand = {
  name: string;
  logo?: string;
  isArg: boolean;
};

/**
 * The ARG portal is served from its dedicated IIS binding.  Branding is
 * resolved from the public request address so it is available before a user
 * has authenticated; all other addresses retain the EDXS identity.
 */
const getLoginBrand = (): LoginBrand => ({ name: "Al Rahim Group", logo: argLogoImage, isArg: true });

const getLoginErrorMessage = (err: any) => {
  const status = err?.response?.status;
  const serverMessage =
    err?.response?.data?.message ??
    err?.response?.data?.Message ??
    err?.response?.data?.error ??
    err?.response?.data?.Error;

  if (status === 401) {
    return serverMessage || "Invalid username or password";
  }

  if (!err?.response) {
    return "Something Went Wrong.";
  }

  if (status >= 500) {
    return "Login service is currently unavailable. Please try again shortly.";
  }

  if (status === 404) {
    return "Login service endpoint was not found. Please contact support.";
  }

  if (status === 408 || status === 429) {
    return serverMessage || "Login request could not be completed right now. Please try again.";
  }

  return serverMessage || "Unable to login due to a service issue. Please try again.";
};

// "Meaningful" background element: A subtle grid that represents structure
const BackgroundGrid = () => (
  <Box
    sx={{
      position: "absolute",
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      opacity: 0.03,
      zIndex: 0,
      backgroundImage: "radial-gradient(#fff 1px, transparent 1px)",
      backgroundSize: "30px 30px",
    }}
  />
);

const FloatingIcon = ({ icon: Icon, delay, top, left, size = 40 }: any) => (
  <Box
    component={motion.div}
    animate={{
      y: [0, -15, 0],
      rotate: [0, 5, -5, 0],
      opacity: [0.08, 0.15, 0.08],
    }}
    transition={{ duration: 7, repeat: Infinity, delay, ease: "easeInOut" }}
    sx={{ position: "absolute", top, left, color: "#fff", zIndex: 1 }}
  >
    <Icon sx={{ fontSize: size }} />
  </Box>
);

const Logo = ({ brand }: { brand: LoginBrand }) => (
  <motion.div
    initial={{ opacity: 0, scale: 0.9 }}
    animate={{ opacity: 1, scale: 1 }}
    transition={{ duration: 0.5, ease: "easeOut" }}
  >
    {brand.isArg ? (
      <Box sx={{ mb: 1 }}>
        <Box
          component="img"
          src={brand.logo}
          alt={`${brand.name} logo`}
          sx={{ width: 220, maxWidth: "100%", height: "auto", display: "block" }}
        />
      </Box>
    ) : (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 2,
          mb: 1,
          position: "relative",
        }}
      >
        <Box sx={{ position: "relative" }}>
        <Box
          sx={{
            width: 44,
            height: 44,
            bgcolor: "var(--lms-primary)",
            borderRadius: "12px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            boxShadow: "0 8px 16px rgba(89, 209, 2, 0.25)",
            zIndex: 2,
            position: "relative",
          }}
        >
          <Typography
            variant="h5"
            sx={{
              color: "#fff",
              fontWeight: 950,
              fontSize: "22px",
              letterSpacing: "-0.05em",
            }}
          >
            E
          </Typography>
        </Box>
        <Box
          sx={{
            position: "absolute",
            top: 4,
            left: 4,
            width: 44,
            height: 44,
            bgcolor: "rgba(var(--lms-primary-rgb), 0.2)",
            borderRadius: "12px",
            zIndex: 1,
            transform: "rotate(10deg)",
          }}
        />
        </Box>
        <Box>
        <Typography
          variant="h4"
          sx={{
            fontWeight: 900,
            letterSpacing: "-0.04em",
            color: "var(--lms-text-main)",
            fontFamily: '"Outfit", sans-serif',
            lineHeight: 1,
          }}
        >
          EDXS
        </Typography>
        <Typography
          variant="caption"
          sx={{
            color: "var(--lms-primary)",
            fontWeight: 800,
            letterSpacing: "0.2em",
            fontSize: "9px",
            textTransform: "uppercase",
            display: "block",
            mt: 0.5,
            opacity: 0.8,
          }}
        >
          Solutions
        </Typography>
        </Box>
      </Box>
    )}
    <Typography
      variant="caption"
      sx={{
        color: "var(--lms-text-secondary)",
        fontWeight: 700,
        letterSpacing: "0.12em",
        textTransform: "uppercase",
        ml: 0.5,
        opacity: 0.7,
      }}
    >
      <T>{brand.isArg ? "Al Rahim Group Portal" : "Education & Enterprise Suite"}</T>
    </Typography>
  </motion.div>
);

export default function LoginPage() {
 const dispatch=useAppDispatch(),navigate=useNavigate(),loginBrand=getLoginBrand();
 const {translate}=useTranslation();
 const authenticated=useAppSelector(state=>state.auth.isAuthenticated);
 const [showPassword,setShowPassword]=useState(false),[loading,setLoading]=useState(false),[error,setError]=useState(''),[help,setHelp]=useState(false),[sloganIndex,setSloganIndex]=useState(0);
 const { register,handleSubmit,formState:{errors} }=useForm<LoginFormData>({resolver:yupResolver(schema)});
 useEffect(()=>{const timer=window.setInterval(()=>setSloganIndex(index=>(index+1)%slogans.length),6000);return()=>window.clearInterval(timer)},[]);
 const onSubmit=async(credentials:LoginFormData)=>{setLoading(true);setError('');try{const {data}=await api.post('/api/auth/login',credentials);dispatch(loginSuccess({...data,refreshToken:''}));dispatch(setCampus(data.user.entityId));localStorage.setItem('clientName','Al Rahim Group');localStorage.setItem('clientId',String(data.user.clientId));navigate('/',{replace:true})}catch(e:any){setError(getLoginErrorMessage(e))}finally{setLoading(false)}};
 if(authenticated)return <Navigate to="/" replace/>;
return (<Box sx={{
        minHeight: "100vh",
        display: "flex",
        bgcolor: "#fff",
        overflow: "hidden",
        fontFamily: '"Outfit", sans-serif',
    }}>
      {/* Left Side: Brand & Visuals */}
      <Box sx={{
        flex: 1,
        position: "relative",
        display: { xs: "none", md: "flex" },
        flexDirection: "column",
        justifyContent: "center",
        px: 12,
        background: "linear-gradient(135deg, #0f172a 0%, #1e293b 100%)",
        color: "#fff",
        overflow: "hidden",
    }}>
        <BackgroundGrid />

        <AnimatePresence>
          {null}
        </AnimatePresence>

        {/* Meaningful Floating Icons */}
        <FloatingIcon icon={BookIcon} top="15%" left="15%" delay={0}/>
        <FloatingIcon icon={BusinessIcon} top="25%" left="75%" delay={1} size={50}/>
        <FloatingIcon icon={ComputerIcon} top="70%" left="18%" delay={2}/>
        <FloatingIcon icon={AnalyticsIcon} top="80%" left="70%" delay={3} size={45}/>
        <FloatingIcon icon={WorkflowIcon} top="40%" left="10%" delay={1.5} size={30}/>
        <FloatingIcon icon={TaskIcon} top="55%" left="85%" delay={2.5} size={35}/>
        <FloatingIcon icon={SchoolIcon} top="72%" left="82%" delay={3.5} size={34}/>
        <FloatingIcon icon={OrganizationIcon} top="48%" left="48%" delay={4} size={42}/>
        <FloatingIcon icon={TeamsIcon} top="30%" left="32%" delay={4.5} size={36}/>
        <FloatingIcon icon={ScienceIcon} top="84%" left="42%" delay={5} size={34}/>
        <FloatingIcon icon={IdeaIcon} top="41%" left="76%" delay={5.5} size={30}/>

        {/* Ambient glow */}
        <Box component={motion.div} animate={{
        scale: [1, 1.1, 1],
        opacity: [0.1, 0.15, 0.1],
        backgroundColor: [
            slogans[0].accent,
            slogans[1].accent,
            slogans[2].accent,
            slogans[0].accent,
        ],
    }} transition={{ duration: 15, repeat: Infinity, ease: "linear" }} sx={{
        position: "absolute",
        top: "10%",
        left: "5%",
        width: "500px",
        height: "500px",
        borderRadius: "50%",
        filter: "blur(100px)",
        zIndex: 0,
    }}/>

        <Box sx={{ position: "relative", zIndex: 2 }}>
          <AnimatePresence mode="wait">
            <motion.div key={sloganIndex} initial={{ opacity: 0, y: 15 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -15 }} transition={{ duration: 0.7, ease: "easeOut" }}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 2, mb: 3 }}>
                <motion.div layoutId="accent-bar" style={{
        width: 8,
        height: 48,
        backgroundColor: slogans[sloganIndex].accent,
        borderRadius: 4,
    }}/>
                <Typography variant="h2" sx={{
        fontWeight: 800,
        letterSpacing: "-0.03em",
        lineHeight: 1.1,
    }}>
                  <T>{slogans[sloganIndex].title}</T>
                </Typography>
              </Box>

              <Typography variant="h5" sx={{
        opacity: 0.7,
        fontWeight: 400,
        lineHeight: 1.6,
        maxWidth: 500,
        mb: 6,
        fontSize: "1.25rem",
    }}>
                <T>{slogans[sloganIndex].sub}</T>
              </Typography>
            </motion.div>
          </AnimatePresence>

          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.8 }}>
            <Stack direction="row" spacing={5}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 800, color: "var(--lms-primary)" }}>
                  10k+
                </Typography>
                <Typography variant="caption" sx={{
        opacity: 0.5,
        textTransform: "uppercase",
        fontWeight: 700,
        letterSpacing: "0.05em",
    }}>
                  <T>Active Users</T>
                </Typography>
              </Box>
              <Box sx={{
        width: 1,
        height: 40,
        bgcolor: "rgba(255,255,255,0.1)",
    }}/>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 800, color: "var(--lms-primary)" }}>
                  99%
                </Typography>
                <Typography variant="caption" sx={{
        opacity: 0.5,
        textTransform: "uppercase",
        fontWeight: 700,
        letterSpacing: "0.05em",
    }}>
                  <T>Success Rate</T>
                </Typography>
              </Box>
            </Stack>
          </motion.div>
        </Box>

        <Box sx={{ position: "absolute", bottom: 50, left: 100, opacity: 0.3 }}>
          <Typography variant="caption" sx={{ fontWeight: 500, letterSpacing: "0.05em" }}>
            {loginBrand.isArg
        ? "AL RAHIM GROUP PORTAL"
        : "EDXS PORTAL | EDUCATION & ENTERPRISE SUITE"}
          </Typography>
        </Box>
      </Box>

      {/* Right Side: Login Form */}
      <Box sx={{
        width: { xs: "100%", md: "580px" },
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        px: { xs: 4, sm: 8, md: 10 },
        position: "relative",
        bgcolor: "#fff",
        "&::after": {
            content: '""',
            position: "absolute",
            top: "10%",
            right: "-5%",
            width: "30%",
            height: "30%",
            background: "radial-gradient(circle, rgba(89, 209, 2, 0.03) 0%, transparent 70%)",
            zIndex: 0,
            pointerEvents: "none",
        },
    }}>
        {null}

        <motion.div initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} transition={{ duration: 0.6, ease: "easeOut" }} style={{ position: "relative", zIndex: 1 }}>
          {(<>
              <Box sx={{ mt: { xs: 4, md: 0 }, mb: { xs: 4, md: 6 } }}>
                <Logo brand={loginBrand}/>
              </Box>

              <AnimatePresence mode="wait">
                {error && (<Box component={motion.div} initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }} sx={{ mb: 3 }}>
                    <Alert severity="error" variant="outlined" sx={{
                borderRadius: "12px",
                borderColor: "#fee2e2",
                bgcolor: "#fff1f2",
                color: "#ef4444",
                fontWeight: 600,
                "& .MuiAlert-icon": { color: "#ef4444" },
            }}>
                      {error ? <T>{error}</T> : null}
                    </Alert>
                  </Box>)}
              </AnimatePresence>

              {help && <Alert severity="info" onClose={()=>setHelp(false)} sx={{mb:2}}>Contact your administrator to reset your password.</Alert>}
<form onSubmit={handleSubmit(onSubmit)} autoComplete="on">
                <Stack spacing={3}>
                  <Box>
                    <Typography variant="caption" sx={{
            fontWeight: 800,
            mb: 1,
            display: "block",
            ml: 0.5,
            color: "var(--lms-text-main)",
            textTransform: "uppercase",
            letterSpacing: "0.04em",
            fontSize: "10px",
        }}>
                      <T>Identity</T>
                    </Typography>
                    <TextField fullWidth autoComplete="username" placeholder={translate("Username or Email")} {...register("username")} inputProps={{ "aria-label": "Username" }} error={!!errors.username} helperText={errors.username?.message ? (<T>{errors.username.message}</T>) : ("")} InputProps={{
            startAdornment: (<InputAdornment position="start">
                            <PersonIcon sx={{
                    color: "var(--lms-text-secondary)",
                    fontSize: 20,
                }}/>
                          </InputAdornment>),
            sx: {
                borderRadius: "14px",
                bgcolor: "rgba(0,0,0,0.02)",
                height: "56px",
                "& fieldset": { border: "1px solid transparent" },
                "&:hover": { bgcolor: "rgba(0,0,0,0.04)" },
                "&.Mui-focused": {
                    bgcolor: "#fff",
                    boxShadow: "0 8px 16px rgba(89, 209, 2, 0.08)",
                    "& fieldset": {
                        borderColor: "rgba(89, 209, 2, 0.3)",
                    },
                },
                "& input:-webkit-autofill": {
                    WebkitBoxShadow: "0 0 0 1000px #fff inset",
                    WebkitTextFillColor: "var(--lms-text-main)",
                    borderRadius: "inherit",
                },
            },
        }}/>
                  </Box>

                  <Box>
                    <Typography variant="caption" sx={{
            fontWeight: 800,
            mb: 1,
            display: "block",
            ml: 0.5,
            color: "var(--lms-text-main)",
            textTransform: "uppercase",
            letterSpacing: "0.04em",
            fontSize: "10px",
        }}>
                      <T>Secret Pin</T>
                    </Typography>
                    <TextField fullWidth type={showPassword ? "text" : "password"} autoComplete="current-password" placeholder="••••••••" {...register("password")} inputProps={{ "aria-label": "Password" }} error={!!errors.password} helperText={errors.password?.message ? (<T>{errors.password.message}</T>) : ("")} InputProps={{
            startAdornment: (<InputAdornment position="start">
                            <LockIcon sx={{
                    color: "var(--lms-text-secondary)",
                    fontSize: 20,
                }}/>
                          </InputAdornment>),
            endAdornment: (<InputAdornment position="end">
                            <IconButton aria-label={showPassword ? "Hide password" : "Show password"} onClick={() => setShowPassword(!showPassword)} size="small">
                              {showPassword ? (<VisibilityOffIcon fontSize="small"/>) : (<VisibilityIcon fontSize="small"/>)}
                            </IconButton>
                          </InputAdornment>),
            sx: {
                borderRadius: "14px",
                bgcolor: "rgba(0,0,0,0.02)",
                height: "56px",
                "& fieldset": { border: "1px solid transparent" },
                "&:hover": { bgcolor: "rgba(0,0,0,0.04)" },
                "&.Mui-focused": {
                    bgcolor: "#fff",
                    boxShadow: "0 8px 16px rgba(89, 209, 2, 0.08)",
                    "& fieldset": {
                        borderColor: "rgba(89, 209, 2, 0.3)",
                    },
                },
                "& input:-webkit-autofill": {
                    WebkitBoxShadow: "0 0 0 1000px #fff inset",
                    WebkitTextFillColor: "var(--lms-text-main)",
                    borderRadius: "inherit",
                },
            },
        }}/>
                  </Box>

                  <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ px: 0.5 }}>
                    <FormControlLabel control={<Checkbox size="small" disableRipple sx={{
                color: "var(--lms-primary)",
                "&.Mui-checked": { color: "var(--lms-primary)" },
            }}/>} label={<Typography variant="body2" sx={{
                fontWeight: 600,
                color: "var(--lms-text-secondary)",
                fontSize: "0.8rem",
            }}>
                          <T>Remember Me</T>
                        </Typography>}/>
                    <Typography variant="body2" sx={{
            color: "var(--lms-primary)",
            fontWeight: 800,
            cursor: "pointer",
            fontSize: "0.8rem",
            "&:hover": { opacity: 0.7 },
        }} onClick={() => {
            setHelp(true);
        }}>
                      <T>Forgot Password?</T>
                    </Typography>
                  </Stack>

                  <Button fullWidth variant="contained" size="large" type="submit" disabled={loading} endIcon={!loading && <ArrowIcon />} sx={{
            py: 1.8,
            mt: 1,
            borderRadius: "14px",
            bgcolor: "var(--lms-primary)",
            backgroundImage: "linear-gradient(45deg, var(--lms-primary), #6ee7b7)",
            color: "#fff",
            fontSize: "15px",
            fontWeight: 700,
            textTransform: "none",
            boxShadow: "0 8px 24px rgba(89, 209, 2, 0.25)",
            position: "relative",
            overflow: "hidden",
            transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
            "&:hover": {
                boxShadow: "0 12px 32px rgba(89, 209, 2, 0.35)",
                transform: "translateY(-2px)",
                filter: "brightness(1.05)",
            },
            "&:active": {
                transform: "translateY(0) scale(0.98)",
            },
            "&::after": loading
                ? {
                    content: '""',
                    position: "absolute",
                    bottom: 0,
                    left: 0,
                    height: "4px",
                    width: "100%",
                    bgcolor: "rgba(255,255,255,0.3)",
                    animation: "loading-bar 2s linear infinite",
                }
                : {},
        }}>
                    {loading ? <T>Validating...</T> : <T>Secure Login</T>}
                  </Button>
                </Stack>
              </form>

              <Box sx={{ mt: 8, textAlign: "center" }}>
                <Typography variant="body2" sx={{
            color: "var(--lms-text-secondary)",
            fontWeight: 600,
            fontSize: "0.85rem",
        }}>
                  <T>Questions?</T>{" "}
                  <Box component="span" sx={{
            color: "var(--lms-primary)",
            fontWeight: 800,
            cursor: "pointer",
            borderBottom: "1.5px solid rgba(89, 209, 2, 0.2)",
            pb: 0.2,
            ml: 1,
            "&:hover": { borderBottomColor: "var(--lms-primary)" },
        }}>
                    <T>Contact Hub</T>
                  </Box>
                </Typography>
              </Box>
            </>)}
        </motion.div>
      </Box>

      {/* Full Screen Success Overlay */}
      <AnimatePresence>
        {null}
      </AnimatePresence>

      <React.Fragment />

      <style>
        {`
                    @keyframes loading-bar {
                        0% { transform: translateX(-100%); }
                        100% { transform: translateX(100%); }
                    }
                `}
      </style>
    </Box>);
}
