import { organizationTextErrors } from "@/utils/organizationValidation";
import React, { useCallback, useEffect, useRef, useState } from "react";
import { institutionPhoneError, institutionPhoneExample } from "@/utils/institutionPhone";
import { Box, Grid, Button, Table, TableHead, TableBody, TableRow, TableCell, TableContainer, IconButton, Checkbox, Typography, Dialog, DialogTitle, DialogContent, DialogActions, TextField, List, ListItemButton, ListItemText, MenuItem, CircularProgress, InputAdornment, Switch, FormControlLabel, Divider, Tooltip } from "@mui/material";
import { Add as AddIcon, DeleteOutline as DeleteIcon, Save as SaveIcon, ArrowBack as ArrowBackIcon, LocationOn as LocationOnIcon, MyLocation as MyLocationIcon, Search as SearchIcon, Undo as UndoIcon, InfoOutlined as InfoOutlinedIcon } from "@mui/icons-material";
import dayjs from "dayjs";
import type { Column } from "@/components/DashboardTable";
import { SchemaTable } from "@/components/SchemaTable";
import { FormThemeWrapper } from "@/components/FormThemeWrapper";
import { FormInput } from "@/components/FormInput";
import { FormDatePicker } from "@/components/FormDatePicker";
import { LookupDropdown } from "@/components/LookupDropdown";
import { ImageUpload } from "@/components/ImageUpload";
import { GenericSearchModal } from "@/components/GenericSearchModal";
import { StyledTabs, TabPanel } from "@/components/StyledTabs";
import { T, useTranslation, normalizeInstitutionType, syncExecutiveInstitutionType } from "@/components/AdaptiveTranslationProvider";
import { getInstitutionTerminology, TerminologyMap } from "@/services/institutionTerminologyService";
import { LmsToaster } from "@/components/LmsToaster";
import { LmsOverlayLoader } from "@/components/LmsOverlayLoader";
import { GetService, PostService, PutService, DeleteService, CheckemptyString } from "@/constants/globalItems";
import { fetchBirthdayNotifications } from "@/services/birthdayService";
import { useAppDispatch, useAppSelector } from "@/hooks";
import { setBirthdays } from "@/store/authSlice";
import { InstitutionAiRulesTab, type AiRules } from "./components/InstitutionAiRulesTab";

type Lookup = { id: number; name?: string; code?: string; description?: string };
type LocationPoint = { latitude: number; longitude: number };
type FormRow = {
  id: number;
  code: string;
  entityTypeId: number;
  entityType: string;
  headEntityId: number;
  headCampus: string;
  shortName: string;
  estDate: string;
  instituteName: string;
  academicRegisterId: number;
  academicRegister: string;
  statusId: number;
  status: string;
  institutionTypeId: number;
  institutionType: string;
  instituteTypeId: number;
  instituteType: string;
  campusTypeId: number;
  campusType: string;
  campusName: string;
  slogan: string;
  owner: string;
  contactNo: string;
  address: string;
  countryId: number;
  country: string;
  stateId: number;
  state: string;
  cityId: number;
  city: string;
  regionId: number;
  region: string;
  latitude: number | null;
  longitude: number | null;
  locationPolygon: LocationPoint[];
  imageId: number | null;
  watermarkLogo: number | null;
  logoPreview?: string;
  watermarkPreview?: string;
  logoFile?: File | null;
  watermarkFile?: File | null;
  logoImage?: string | null;
  watermarkImage?: string | null;
  employees: Array<{ id: number; typeId: number; type: string; employeeId: number; employee: string }>;
  contacts: Array<{ id: number; name: string; contactTypeId: number; contactType: string; value: string }>;
  payments: Array<{ id: number; vendorId: number; vendor: string; prefix: string; payMethodId: number; payMethod: string; bankId: number; bank: string; isDefault: boolean }>;
  // AI Rules tab, loaded and saved through its own API; undefined until loaded.
  aiRules?: AiRules;
  aiRulesAvailable?: boolean;
};

type Lookups = {
  entityTypes: Lookup[];
  headCampuses: Lookup[];
  employeeTypes: Lookup[];
  institutionTypes: Lookup[];
  instituteTypes: Lookup[];
  campusTypes: Lookup[];
  statuses: Lookup[];
  contactTypes: Lookup[];
  payMethods: Lookup[];
  academicRegisters: Lookup[];
  regions: Lookup[];
  paymentVendors: Lookup[];
  banks: Lookup[];
  employees: Lookup[];
  countries: Lookup[];
  states: Lookup[];
  cities: Lookup[];
};

const emptyLookups: Lookups = {
  entityTypes: [], headCampuses: [], employeeTypes: [], institutionTypes: [], instituteTypes: [], campusTypes: [], statuses: [], contactTypes: [], payMethods: [],
  academicRegisters: [], regions: [], paymentVendors: [], banks: [], employees: [], countries: [], states: [], cities: [],
};


const makeInitial = (): FormRow => ({
  id: 0,
  code: "",
  entityTypeId: 0,
  entityType: "",
  headEntityId: 0,
  headCampus: "",
  shortName: "",
  estDate: dayjs().format("DD-MMM-YYYY"),
  instituteName: "",
  academicRegisterId: 0,
  academicRegister: "",
  statusId: 0,
  status: "",
  institutionTypeId: 0,
  institutionType: "",
  instituteTypeId: 0,
  instituteType: "",
  campusTypeId: 0,
  campusType: "",
  campusName: "",
  slogan: "",
  owner: "",
  contactNo: "",
  address: "",
  countryId: 0,
  country: "",
  stateId: 0,
  state: "",
  cityId: 0,
  city: "",
  regionId: 0,
  region: "",
  latitude: null,
  longitude: null,
  locationPolygon: [],
  imageId: null,
  watermarkLogo: null,
  logoPreview: "",
  watermarkPreview: "",
  logoFile: null,
  watermarkFile: null,
  logoImage: null,
  watermarkImage: null,
  employees: [{ id: 1, typeId: 0, type: "", employeeId: 0, employee: "" }],
  contacts: [{ id: 1, name: "", contactTypeId: 0, contactType: "", value: "" }],
  payments: [{ id: 1, vendorId: 0, vendor: "", prefix: "", payMethodId: 0, payMethod: "", bankId: 0, bank: "", isDefault: false }],
});

type LocationSearchResult = {
  place_id: number;
  display_name: string;
  lat: string;
  lon: string;
};

interface LocationMapDialogProps {
  open: boolean;
  onClose: () => void;
  onSelect: (latitude: number, longitude: number, locationPolygon: LocationPoint[]) => void;
  countryName: string;
  countryCode?: string;
  latitude: number | null;
  longitude: number | null;
  locationPolygon: LocationPoint[];
  disabled?: boolean;
}

const toCoordinate = (value: unknown): number | null => {
  if (value === null || value === undefined || value === "") return null;
  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : null;
};

const loadLeaflet = async () => {
  const w = window as any;
  if (w.L) return w.L;

  const cssId = "leaflet-map-css";
  if (!document.getElementById(cssId)) {
    const link = document.createElement("link");
    link.id = cssId;
    link.rel = "stylesheet";
    link.href = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css";
    document.head.appendChild(link);
  }

  await new Promise<void>((resolve, reject) => {
    const existingScript = document.getElementById("leaflet-map-js") as HTMLScriptElement | null;
    if (existingScript) {
      existingScript.addEventListener("load", () => resolve(), { once: true });
      existingScript.addEventListener("error", () => reject(new Error("Leaflet failed to load")), { once: true });
      return;
    }

    const script = document.createElement("script");
    script.id = "leaflet-map-js";
    script.src = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js";
    script.async = true;
    script.onload = () => resolve();
    script.onerror = () => reject(new Error("Leaflet failed to load"));
    document.body.appendChild(script);
  });

  return (window as any).L;
};

const normalizeLocationPolygon = (value: unknown): LocationPoint[] => {
  if (!value) return [];
  let raw = value;
  if (typeof value === "string") {
    try {
      raw = JSON.parse(value);
    } catch {
      return [];
    }
  }
  if (!Array.isArray(raw)) return [];
  return raw
    .map((point: any) => ({
      latitude: toCoordinate(point?.latitude ?? point?.lat),
      longitude: toCoordinate(point?.longitude ?? point?.lng ?? point?.lon),
    }))
    .filter((point): point is LocationPoint => point.latitude !== null && point.longitude !== null)
    .map((point) => ({
      latitude: Number(point.latitude.toFixed(7)),
      longitude: Number(point.longitude.toFixed(7)),
    }));
};

const getPolygonCenter = (polygon: LocationPoint[]): LocationPoint | null => {
  if (polygon.length === 0) return null;
  const totals = polygon.reduce(
    (acc, point) => ({
      latitude: acc.latitude + point.latitude,
      longitude: acc.longitude + point.longitude,
    }),
    { latitude: 0, longitude: 0 }
  );
  return {
    latitude: Number((totals.latitude / polygon.length).toFixed(7)),
    longitude: Number((totals.longitude / polygon.length).toFixed(7)),
  };
};

const LeafletMapPreview: React.FC<{
  latitude: number;
  longitude: number;
  polygon: LocationPoint[];
  onLocationChange: (latitude: number, longitude: number) => void;
  onPolygonChange: (polygon: LocationPoint[]) => void;
  disabled?: boolean;
}> = ({ latitude, longitude, polygon, onLocationChange, onPolygonChange, disabled }) => {
  const mapEl = useRef<HTMLDivElement | null>(null);
  const mapRef = useRef<any>(null);
  const markerRef = useRef<any>(null);
  const polygonRef = useRef<any>(null);
  const pointMarkersRef = useRef<any[]>([]);
  const latestPolygonRef = useRef<LocationPoint[]>(polygon);

  useEffect(() => {
    latestPolygonRef.current = polygon;
  }, [polygon]);

  const redrawPolygon = (L: any, nextPolygon: LocationPoint[]) => {
    if (!mapRef.current) return;
    if (polygonRef.current) {
      polygonRef.current.remove();
      polygonRef.current = null;
    }
    pointMarkersRef.current.forEach((marker) => marker.remove());
    pointMarkersRef.current = [];

    const latLngs = nextPolygon.map((point) => [point.latitude, point.longitude]);
    if (latLngs.length > 0) {
      pointMarkersRef.current = nextPolygon.map((point, index) => {
        const pointMarker = L.circleMarker([point.latitude, point.longitude], {
          radius: 6,
          color: "var(--lms-primary)",
          fillColor: "var(--lms-primary)",
          fillOpacity: 0.95,
          weight: 2,
          draggable: false,
        }).addTo(mapRef.current);
        pointMarker.on("click", () => {
          if (disabled) return;
          onPolygonChange(latestPolygonRef.current.filter((_, itemIndex) => itemIndex !== index));
        });
        return pointMarker;
      });
    }
    if (latLngs.length >= 3) {
      polygonRef.current = L.polygon(latLngs, {
        color: "var(--lms-primary)",
        fillColor: "#8fb665",
        fillOpacity: 0.24,
        weight: 2,
      }).addTo(mapRef.current);
    } else if (latLngs.length >= 2) {
      polygonRef.current = L.polyline(latLngs, {
        color: "var(--lms-primary)",
        weight: 2,
        dashArray: "6 5",
      }).addTo(mapRef.current);
    }
  };

  useEffect(() => {
    let cancelled = false;

    const setupMap = async () => {
      const L = await loadLeaflet();
      if (cancelled || !mapEl.current || mapRef.current) return;

      const map = L.map(mapEl.current, {
        attributionControl: false,
        zoomControl: true,
      }).setView([latitude, longitude], 14);

      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
      }).addTo(map);

      markerRef.current = L.marker([latitude, longitude], { draggable: !disabled }).addTo(map);
      markerRef.current.on("dragend", () => {
        const next = markerRef.current.getLatLng();
        onLocationChange(Number(next.lat.toFixed(7)), Number(next.lng.toFixed(7)));
      });
      map.on("click", (event: any) => {
        if (disabled) return;
        const nextPoint = {
          latitude: Number(event.latlng.lat.toFixed(7)),
          longitude: Number(event.latlng.lng.toFixed(7)),
        };
        onPolygonChange([...latestPolygonRef.current, nextPoint]);
      });
      mapRef.current = map;
      redrawPolygon(L, latestPolygonRef.current);
    };

    setupMap();

    return () => {
      cancelled = true;
      if (mapRef.current) {
        mapRef.current.remove();
        mapRef.current = null;
        markerRef.current = null;
      }
    };
  }, []);

  useEffect(() => {
    if (!mapRef.current || !markerRef.current) return;
    const nextPosition = [latitude, longitude] as [number, number];
    mapRef.current.setView(nextPosition, mapRef.current.getZoom() || 14);
    markerRef.current.setLatLng(nextPosition);
  }, [latitude, longitude]);

  useEffect(() => {
    if (!mapRef.current) return;
    const draw = async () => {
      const L = await loadLeaflet();
      redrawPolygon(L, polygon);
    };
    draw();
  }, [polygon, disabled]);

  return <Box ref={mapEl} sx={{ width: "100%", height: "100%", "& .leaflet-control-zoom": { border: "none" }, "& .leaflet-control-zoom a": { color: "#1f2937" } }} />;
};

const LocationMapDialog: React.FC<LocationMapDialogProps> = ({
  open,
  onClose,
  onSelect,
  countryName,
  countryCode,
  latitude,
  longitude,
  locationPolygon,
  disabled,
}) => {
  const { translate } = useTranslation();
  const [searchText, setSearchText] = useState("");
  const [results, setResults] = useState<LocationSearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [draftLatitude, setDraftLatitude] = useState<number | null>(latitude);
  const [draftLongitude, setDraftLongitude] = useState<number | null>(longitude);
  const [draftPolygon, setDraftPolygon] = useState<LocationPoint[]>(locationPolygon);
  const selectedCountryCode = (countryCode || "").trim().toLowerCase();
  const canSearch = !!countryName;
  const mapLat = draftLatitude ?? 30.3753;
  const mapLon = draftLongitude ?? 69.3451;

  useEffect(() => {
    if (open) {
      setDraftLatitude(latitude);
      setDraftLongitude(longitude);
      setDraftPolygon(locationPolygon);
    } else {
      setSearchText("");
      setResults([]);
      setMessage("");
      setLoading(false);
    }
  }, [latitude, longitude, locationPolygon, open]);

  const searchLocation = async () => {
    if (!canSearch || !searchText.trim()) return;
    setLoading(true);
    setMessage("");
    try {
      const params = new URLSearchParams({
        format: "jsonv2",
        q: `${searchText.trim()}, ${countryName}`,
        limit: "8",
        addressdetails: "1",
      });
      if (selectedCountryCode) params.set("countrycodes", selectedCountryCode);
      const response = await fetch(`https://nominatim.openstreetmap.org/search?${params.toString()}`);
      if (!response.ok) throw new Error("Location search failed");
      const data = await response.json();
      setResults(Array.isArray(data) ? data : []);
      if (!Array.isArray(data) || data.length === 0) setMessage(translate("No location found in selected country"));
    } catch {
      setMessage(translate("Failed to search location"));
    } finally {
      setLoading(false);
    }
  };

  const useCurrentLocation = () => {
    if (!navigator.geolocation) {
      setMessage(translate("Current location is not available in this browser"));
      return;
    }
    setLoading(true);
    setMessage("");
    navigator.geolocation.getCurrentPosition(
      async (position) => {
        const nextLat = position.coords.latitude;
        const nextLon = position.coords.longitude;
        if (selectedCountryCode) {
          try {
            const params = new URLSearchParams({ format: "jsonv2", lat: String(nextLat), lon: String(nextLon), addressdetails: "1" });
            const response = await fetch(`https://nominatim.openstreetmap.org/reverse?${params.toString()}`);
            const data = await response.json();
            const currentCountry = (data?.address?.country_code || "").toLowerCase();
            if (currentCountry && currentCountry !== selectedCountryCode) {
              setMessage(translate("Current location is outside selected country"));
              setLoading(false);
              return;
            }
          } catch {
            setMessage(translate("Could not verify current location country"));
            setLoading(false);
            return;
          }
        }
        setDraftLatitude(Number(nextLat.toFixed(7)));
        setDraftLongitude(Number(nextLon.toFixed(7)));
        setLoading(false);
      },
      () => {
        setMessage(translate("Unable to access current location"));
        setLoading(false);
      },
      { enableHighAccuracy: true, timeout: 10000 }
    );
  };

  const selectResult = (item: LocationSearchResult) => {
    setDraftLatitude(Number(Number(item.lat).toFixed(7)));
    setDraftLongitude(Number(Number(item.lon).toFixed(7)));
  };

  const commitLocation = () => {
    if (draftLatitude === null || draftLongitude === null) return;
    onSelect(draftLatitude, draftLongitude, draftPolygon);
    onClose();
  };

  const updateDraftPolygon = (nextPolygon: LocationPoint[]) => {
    setDraftPolygon(nextPolygon);
    const center = getPolygonCenter(nextPolygon);
    if (!center) return;
    setDraftLatitude(center.latitude);
    setDraftLongitude(center.longitude);
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      maxWidth="lg"
      PaperProps={{ sx: { borderRadius: "16px", overflow: "hidden" } }}
    >
      <DialogTitle sx={{ px: 3, py: 2.25, fontWeight: 900, color: "#1f2937", bgcolor: "rgba(var(--lms-primary-rgb), 0.034)", borderBottom: "1px solid rgba(var(--lms-primary-rgb), 0.161)" }}>
        <T>Select Location</T>
      </DialogTitle>
      <DialogContent sx={{ p: 0 }}>
        <Grid container sx={{ minHeight: 440 }}>
          <Grid item xs={12} md={4} sx={{ p: 3, borderRight: { md: "1px solid rgba(var(--lms-primary-rgb), 0.161)" }, bgcolor: "#fff" }}>
            <Box sx={{ mb: 1.5 }}>
              <TextField
                fullWidth
                size="small"
                label={translate("Search Location")}
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
                onKeyDown={(e) => { if (e.key === "Enter") searchLocation(); }}
                disabled={disabled || !canSearch || loading}
                placeholder={translate("Search Location")}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={searchLocation}
                        disabled={disabled || !canSearch || !searchText.trim() || loading}
                        edge="end"
                      >
                        {loading ? <CircularProgress size={18} /> : <SearchIcon fontSize="small" />}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
                sx={{
                  "& .MuiOutlinedInput-root": { borderRadius: "10px", bgcolor: "#fff" },
                }}
              />
            </Box>
            <Button
              variant="outlined"
              startIcon={<MyLocationIcon />}
              onClick={useCurrentLocation}
              disabled={disabled || loading || !canSearch}
              fullWidth
              sx={{
                mb: 2,
                height: 40,
                borderRadius: "10px",
                borderColor: "rgba(var(--lms-primary-rgb), 0.265)",
                color: "var(--lms-primary-dark)",
                fontWeight: 800,
                textTransform: "none",
                bgcolor: "rgba(var(--lms-primary-rgb), 0.050)",
              }}
            >
              <T>Use Current Location</T>
            </Button>
            <Button
              variant="outlined"
              startIcon={<UndoIcon />}
              onClick={() => setDraftPolygon([])}
              disabled={disabled || draftPolygon.length === 0}
              fullWidth
              sx={{
                mb: 2,
                height: 40,
                borderRadius: "10px",
                borderColor: "rgba(var(--lms-primary-rgb), 0.265)",
                color: "var(--lms-primary-dark)",
                fontWeight: 800,
                textTransform: "none",
                bgcolor: "#fff",
              }}
            >
              <T>Clear Boundary</T>
            </Button>
            {message && <Typography sx={{ color: "var(--lms-danger, #b42318)", fontSize: "0.85rem", mb: 1 }}>{message}</Typography>}
            {results.length > 0 && (
              <List dense sx={{ maxHeight: 260, overflow: "auto", border: "1px solid rgba(var(--lms-primary-rgb), 0.161)", borderRadius: "10px", p: 0 }}>
                {results.map((item) => {
                const itemLat = Number(Number(item.lat).toFixed(7));
                const itemLon = Number(Number(item.lon).toFixed(7));
                const selected = draftLatitude === itemLat && draftLongitude === itemLon;
                return (
                  <ListItemButton
                    key={item.place_id}
                    onClick={() => selectResult(item)}
                    disabled={disabled}
                    selected={selected}
                    sx={{
                      alignItems: "flex-start",
                      borderBottom: "1px solid rgba(var(--lms-primary-rgb), 0.114)",
                      "&.Mui-selected": { bgcolor: "rgba(var(--lms-primary-rgb), 0.104)" },
                      "&.Mui-selected:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.151)" },
                    }}
                  >
                    <ListItemText
                      primary={item.display_name}
                      secondary={`${itemLat}, ${itemLon}`}
                      primaryTypographyProps={{ sx: { fontWeight: 700, color: "#344054", fontSize: "0.86rem" } }}
                      secondaryTypographyProps={{ sx: { color: "var(--lms-primary-dark)", fontWeight: 700, mt: 0.4 } }}
                    />
                  </ListItemButton>
                );
                })}
              </List>
            )}
          </Grid>
          <Grid item xs={12} md={8} sx={{ p: 3, bgcolor: "rgba(var(--lms-primary-rgb), 0.057)" }}>
            <Box sx={{ height: 360, border: "1px solid rgba(var(--lms-primary-rgb), 0.218)", borderRadius: "14px", overflow: "hidden", bgcolor: "#fff", boxShadow: "0 12px 30px rgba(16, 24, 40, 0.08)" }}>
              <LeafletMapPreview
                latitude={mapLat}
                longitude={mapLon}
                polygon={draftPolygon}
                disabled={disabled}
                onLocationChange={(nextLat, nextLon) => {
                  setDraftLatitude(nextLat);
                  setDraftLongitude(nextLon);
                }}
                onPolygonChange={updateDraftPolygon}
              />
            </Box>
            <Typography sx={{ mt: 0.75, textAlign: "right", color: "#667085", fontSize: "0.7rem" }}>
              Map data (c) OpenStreetMap contributors
            </Typography>
            <Box sx={{ mt: 2, display: "grid", gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" }, gap: 1.5 }}>
              <Box sx={{ p: 1.5, borderRadius: "10px", bgcolor: "#fff", border: "1px solid rgba(var(--lms-primary-rgb), 0.161)" }}>
                <Typography sx={{ color: "#667085", fontSize: "0.75rem", fontWeight: 800, textTransform: "uppercase" }}><T>Latitude</T></Typography>
                <Typography sx={{ fontWeight: 900, color: "#1f2937" }}>{draftLatitude ?? ""}</Typography>
              </Box>
              <Box sx={{ p: 1.5, borderRadius: "10px", bgcolor: "#fff", border: "1px solid rgba(var(--lms-primary-rgb), 0.161)" }}>
                <Typography sx={{ color: "#667085", fontSize: "0.75rem", fontWeight: 800, textTransform: "uppercase" }}><T>Longitude</T></Typography>
                <Typography sx={{ fontWeight: 900, color: "#1f2937" }}>{draftLongitude ?? ""}</Typography>
              </Box>
            </Box>
            <Typography sx={{ mt: 1, color: "#667085", fontSize: "0.78rem", fontWeight: 700 }}>
              {translate("Boundary Points")}: {draftPolygon.length}
            </Typography>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions sx={{ px: 3, py: 2, bgcolor: "#fff", borderTop: "1px solid rgba(var(--lms-primary-rgb), 0.161)" }}>
        <Button onClick={onClose} sx={{ color: "#667085", fontWeight: 800, textTransform: "none" }}><T>Close</T></Button>
        <Button
          variant="contained"
          onClick={commitLocation}
          disabled={disabled || draftLatitude === null || draftLongitude === null}
          sx={{ px: 4, borderRadius: "10px", bgcolor: "var(--lms-primary)", fontWeight: 800, textTransform: "none", "&:hover": { bgcolor: "var(--lms-primary-dark)" } }}
        >
          <T>Done</T>
        </Button>
      </DialogActions>
    </Dialog>
  );
};

interface InstitutionFormProps {
  formData: FormRow;
  setFormData: React.Dispatch<React.SetStateAction<FormRow>>;
  viewMode: "new" | "edit" | "view";
  lookups: Lookups;
  locationLoading: boolean;
  isSubmitted: boolean;
}

const InstitutionForm: React.FC<InstitutionFormProps> = ({ formData, setFormData, viewMode, lookups, locationLoading, isSubmitted }) => {
  const { translate } = useTranslation();
  const [tab, setTab] = useState(0);
  const [mapDialogOpen, setMapDialogOpen] = useState(false);
  const isReadOnly = viewMode === "view";
  const [phoneTouched, setPhoneTouched] = useState(false);
  const phoneCountry = lookups.countries.find((country) => Number(country.id) === Number(formData.countryId));
  const phoneExample = institutionPhoneExample(phoneCountry?.code);
  const phoneError = institutionPhoneError(formData.contactNo || "", phoneCountry?.code);
  const selectCountry = (country: Lookup) => {
    setPhoneTouched(Boolean(formData.contactNo));
    setFormData((previous) => ({ ...previous, countryId: country.id, country: country.name || "", stateId: 0, state: "", cityId: 0, city: "", latitude: null, longitude: null, locationPolygon: [] }));
  };
  const selectState = (state: Lookup) => {
    setFormData((previous) => ({ ...previous, stateId: state.id, state: state.name || "", cityId: 0, city: "" }));
  };
  const isCompanyEntity = (formData.entityType || "").trim().toLowerCase() === "company";
  // Academic institutions can be registered under a Company entity as well.
  // Use the selected institution type, rather than the entity's legal category.
  const academicInstitutionTypes = ['school', 'university', 'college', 'coachingcenter', 'daycare'];
  const selectedInstitutionType = lookups.institutionTypes.find(option => Number(option.id) === Number(formData.institutionTypeId));
  const showAcademicRegister = academicInstitutionTypes.includes(normalizeInstitutionType(selectedInstitutionType?.name || formData.institutionType)) || !isCompanyEntity;
  // This form must react before saving: choosing Company changes its own labels
  // immediately, independently of the selected campus's saved terminology.
  const entityTypeLabel = (value: string) => translate(
    isCompanyEntity
      ? value.replace(/\bInstitution\b/g, "Organization").replace(/\bCampus\b/g, "Branch")
      : value,
  );
  const [lookupModal, setLookupModal] = useState<{ open: boolean; key: string | null; rowId?: number }>({ open: false, key: null });
  const tabs = [{ label: <T>Entity Setup</T> }, { label: <T>Address</T> }, { label: <T>Contact</T> }, { label: <T>Online Payment</T> }, { label: <T>Template</T> }, { label: <T>AI Rules</T> }];
  const addressTabIndex = 1;
  const contactTabIndex = 2;
  const paymentTabIndex = 3;
  const templateTabIndex = 4;
  const aiRulesTabIndex = 5;
  // AI Rules are saved by their own API after the institution (see handleSave), so a
  // new institution gets them once it exists. undefined means they have not loaded.
  const aiRulesNotice = !formData.id
    ? entityTypeLabel("Save the Institution first, then add its AI Rules.")
    : formData.aiRulesAvailable === false
      ? translate("AI Rules cannot be saved yet: table OrgAiRule has not been created in this database.")
      : undefined;
  useEffect(() => {
    const id = Number(formData.id || 0);
    if (!id) return;
    let cancelled = false;
    GetService(`/api/organization/Institution/v3/${id}/ai-rules`)
      .then((response: any) => {
        if (cancelled) return;
        const data = response?.data ?? {};
        const aiRules: AiRules = {};
        (Array.isArray(data.rules) ? data.rules : []).forEach((rule: any) => {
          if (rule?.featureCode) aiRules[String(rule.featureCode)] = String(rule.prompt ?? "");
        });
        setFormData((prev: any) => Number(prev.id) === id ? { ...prev, aiRules, aiRulesAvailable: data.available !== false } : prev);
      })
      .catch((error: any) => console.error("Failed to load AI Rules", error));
    return () => { cancelled = true; };
  }, [formData.id, setFormData]);


  // Read-only preview of the keyword swaps applied for the selected institution type.
  const [terminologyMap, setTerminologyMap] = useState<TerminologyMap>({});
  useEffect(() => { getInstitutionTerminology().then(setTerminologyMap); }, []);
  const templateMappings = terminologyMap[normalizeInstitutionType(formData.institutionType)] ?? [];

  const openLookup = (key: string, rowId?: number) => {
    if (isReadOnly) return;
    if ((key === "state" && (!formData.countryId || locationLoading)) ||
        (key === "city" && (!formData.stateId || locationLoading))) return;
    setLookupModal({ open: true, key, rowId });
  };

  const closeLookup = () => setLookupModal({ open: false, key: null, rowId: undefined });

  const pick = (fieldId: string, fieldName: string, o: Lookup) => {
    if (isReadOnly) return;
    setFormData((prev: any) => ({ ...prev, [fieldId]: o.id, [fieldName]: o.name || "" }));
  };

  const contacts = Array.isArray(formData.contacts) ? formData.contacts : [];
  const payments = Array.isArray(formData.payments) ? formData.payments : [];
  const employees = Array.isArray(formData.employees) ? formData.employees : [];
  const academicRegisterOptions = [{ id: 0, name: "", code: "" }, ...lookups.academicRegisters];
  const headCampusOptions = lookups.headCampuses.filter((x) => Number(x.id || 0) !== Number(formData.id || 0));

  const updateContacts = (next: any[]) => setFormData((prev: any) => ({ ...prev, contacts: next }));
  const updatePayments = (next: any[]) => setFormData((prev: any) => ({ ...prev, payments: next }));
  const updateEmployees = (next: any[]) => setFormData((prev: any) => ({ ...prev, employees: next }));

  const [selectedContactIds, setSelectedContactIds] = useState<number[]>([]);
  const [selectedPaymentIds, setSelectedPaymentIds] = useState<number[]>([]);

  const addContact = () => { if (!isReadOnly) { const m = contacts.reduce((a: number, b: any) => Math.max(a, Number(b.id || 0)), 0); updateContacts([...contacts, { id: m + 1, name: "", contactTypeId: 0, contactType: "", value: "" }]); } };
  const updateContact = (id: number, patch: any) => { if (!isReadOnly) updateContacts(contacts.map((x: any) => x.id === id ? { ...x, ...patch } : x)); };
  const deleteSelectedContacts = () => { if (!isReadOnly && selectedContactIds.length) { const next = contacts.filter((x: any) => !selectedContactIds.includes(Number(x.id))); updateContacts(next.length ? next : [{ id: 1, name: "", contactTypeId: 0, contactType: "", value: "" }]); setSelectedContactIds([]); } };

  const addPayment = () => { if (!isReadOnly) { const m = payments.reduce((a: number, b: any) => Math.max(a, Number(b.id || 0)), 0); updatePayments([...payments, { id: m + 1, vendorId: 0, vendor: "", prefix: "", payMethodId: 0, payMethod: "", bankId: 0, bank: "", isDefault: false }]); } };
  const updatePayment = (id: number, patch: any) => { if (!isReadOnly) updatePayments(payments.map((x: any) => x.id === id ? { ...x, ...patch } : x)); };
  const setDefaultPayment = (id: number, checked: boolean) => { if (!isReadOnly) updatePayments(payments.map((x: any) => ({ ...x, isDefault: checked ? x.id === id : (x.id === id ? false : x.isDefault) }))); };
  const deleteSelectedPayments = () => { if (!isReadOnly && selectedPaymentIds.length) { const next = payments.filter((x: any) => !selectedPaymentIds.includes(Number(x.id))); updatePayments(next.length ? next : [{ id: 1, vendorId: 0, vendor: "", prefix: "", payMethodId: 0, payMethod: "", bankId: 0, bank: "", isDefault: false }]); setSelectedPaymentIds([]); } };

  const addEmployee = () => { if (!isReadOnly) { const m = employees.reduce((a: number, b: any) => Math.max(a, Number(b.id || 0)), 0); updateEmployees([...employees, { id: m + 1, typeId: 0, type: "", employeeId: 0, employee: "" }]); } };
  const updateEmployee = (id: number, patch: any) => { if (!isReadOnly) updateEmployees(employees.map((x: any) => x.id === id ? { ...x, ...patch } : x)); };
  const removeEmployee = (id: number) => { if (!isReadOnly) { const next = employees.filter((x: any) => x.id !== id); updateEmployees(next.length ? next : [{ id: 1, typeId: 0, type: "", employeeId: 0, employee: "" }]); } };

  const readImageFile = (file: File) => new Promise<string>((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(typeof reader.result === "string" ? reader.result : "");
    reader.onerror = () => reject(reader.error || new Error("Failed to read image"));
    reader.readAsDataURL(file);
  });

  const onLogoSelect = async (file: File) => {
    if (isReadOnly) return;
    const image = await readImageFile(file);
    setFormData((prev: any) => ({ ...prev, logoFile: file, logoImage: image, logoPreview: image }));
  };

  const onWatermarkSelect = async (file: File) => {
    if (isReadOnly) return;
    const image = await readImageFile(file);
    setFormData((prev: any) => ({ ...prev, watermarkFile: file, watermarkImage: image, watermarkPreview: image }));
  };

  const lookupConfigs: Record<string, { title: string; data: Lookup[]; selectedValue: string }> = {
    entityType: { title: translate("Entity Type"), data: lookups.entityTypes, selectedValue: formData.entityType || "" },
    headCampus: { title: entityTypeLabel("Head Campus"), data: headCampusOptions, selectedValue: formData.headCampus || "" },
    academicRegister: { title: translate("Academic Register"), data: academicRegisterOptions, selectedValue: formData.academicRegister || "" },
    status: { title: translate("Status"), data: lookups.statuses, selectedValue: formData.status || "" },
    institutionType: { title: entityTypeLabel("Institution Type"), data: lookups.institutionTypes, selectedValue: formData.institutionType || "" },
    instituteType: { title: translate("Institute Type"), data: lookups.instituteTypes, selectedValue: formData.instituteType || "" },
    campusType: { title: entityTypeLabel("Campus Type"), data: lookups.campusTypes, selectedValue: formData.campusType || "" },
    country: { title: translate("Country"), data: lookups.countries, selectedValue: formData.country || "" },
    state: { title: translate("State"), data: lookups.states, selectedValue: formData.state || "" },
    city: { title: translate("City"), data: lookups.cities, selectedValue: formData.city || "" },
    region: { title: translate("Region"), data: lookups.regions, selectedValue: formData.region || "" },
    employeeType: { title: translate("Type"), data: lookups.employeeTypes, selectedValue: employees.find((x: any) => x.id === lookupModal.rowId)?.type || "" },
    employee: { title: translate("Employee"), data: lookups.employees, selectedValue: employees.find((x: any) => x.id === lookupModal.rowId)?.employee || "" },
    contactType: { title: translate("Contact Type"), data: lookups.contactTypes, selectedValue: contacts.find((x: any) => x.id === lookupModal.rowId)?.contactType || "" },
    paymentVendor: { title: translate("Payment Vendor"), data: lookups.paymentVendors, selectedValue: payments.find((x: any) => x.id === lookupModal.rowId)?.vendor || "" },
    payMethod: { title: translate("Pay Method"), data: lookups.payMethods, selectedValue: payments.find((x: any) => x.id === lookupModal.rowId)?.payMethod || "" },
    bank: { title: translate("Bank"), data: lookups.banks, selectedValue: payments.find((x: any) => x.id === lookupModal.rowId)?.bank || "" },
  };

  const currentLookup = lookupModal.key ? lookupConfigs[lookupModal.key] : undefined;

  const handleLookupSelect = async (row: any) => {
    if (!lookupModal.key) return;
    const s: Lookup = { id: Number(row?.id || 0), name: row?.name || row?.stxt || "", code: row?.code || "" };
    const key = lookupModal.key;
    const mid = lookupModal.rowId;

    if (key === "country") selectCountry(s);
    else if (key === "state") selectState(s);
    else if (key === "city") pick("cityId", "city", s);
    else if (key === "region") pick("regionId", "region", s);
    else if (key === "entityType") {
      const isCompany = (s.name || "").trim().toLowerCase() === "company";
      setFormData((p: any) => ({
        ...p,
        entityTypeId: s.id,
        entityType: s.name || "",
        ...(isCompany ? { campusTypeId: 0, campusType: "" } : {}),
      }));
    }
    else if (key === "headCampus") pick("headEntityId", "headCampus", s);
    else if (key === "academicRegister") pick("academicRegisterId", "academicRegister", s);
    else if (key === "status") pick("statusId", "status", s);
    else if (key === "institutionType") pick("institutionTypeId", "institutionType", s);
    else if (key === "instituteType") pick("instituteTypeId", "instituteType", s);
    else if (key === "campusType") pick("campusTypeId", "campusType", s);
    else if (key === "employeeType" && mid) updateEmployee(mid, { typeId: s.id, type: s.name || "" });
    else if (key === "employee" && mid) updateEmployee(mid, { employeeId: s.id, employee: s.name || "" });
    else if (key === "contactType" && mid) updateContact(mid, { contactTypeId: s.id, contactType: s.name || "" });
    else if (key === "paymentVendor" && mid) updatePayment(mid, { vendorId: s.id, vendor: s.name || "" });
    else if (key === "payMethod" && mid) updatePayment(mid, { payMethodId: s.id, payMethod: s.name || "" });
    else if (key === "bank" && mid) updatePayment(mid, { bankId: s.id, bank: s.name || "" });

    closeLookup();
  };

  return (
    <FormThemeWrapper>
      {currentLookup && (
        <GenericSearchModal
          open={lookupModal.open}
          onClose={closeLookup}
          title={currentLookup.title}
          columns={[{ id: "code", label: translate("Code"), minWidth: 100 }, { id: "name", label: translate("Name"), minWidth: 200,
            ...(lookupModal.key === 'institutionType' ? { format: (value: any) => <span data-terminology-skip>{value}</span> } : {}),
          }]}
          data={currentLookup.data.map((x: any, i) => ({ id: x.id || i, code: x.code || "", name: x.name || x.stxt || "" }))}
          onSelect={handleLookupSelect}
          selectedValue={currentLookup.selectedValue}
        />
      )}
      <Box sx={{ mb: 3 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 3 }}>
          <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
          <Typography sx={{ fontWeight: 800, fontSize: "1rem", color: "#333" }}>
            {entityTypeLabel("Institution Information")}
          </Typography>
        </Box>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={4}><FormInput disabled={isReadOnly} label={<T>Code*</T>} value={formData.code || ""} onChange={(e) => setFormData((p: any) => ({ ...p, code: e.target.value }))} error={!!isSubmitted && !formData.code} /></Grid>
          <Grid item xs={12} sm={4}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("entityType")} label={<T>Entity Type*</T>} value={formData.entityType || ""} options={lookups.entityTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => { const isCompany = (o.name || "").trim().toLowerCase() === "company"; setFormData((p: any) => ({ ...p, entityTypeId: o.id, entityType: o.name || "", ...(isCompany ? { campusTypeId: 0, campusType: "" } : {}) })); }} error={!!isSubmitted && !formData.entityTypeId} /></Grid>
          <Grid item xs={12} sm={4}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("headCampus")} label={entityTypeLabel("Head Campus")} value={formData.headCampus || ""} options={headCampusOptions} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("headEntityId", "headCampus", o)} /></Grid>
          <Grid item xs={12} sm={4}><FormInput disabled={isReadOnly} label={<T>Short Name*</T>} value={formData.shortName || ""} onChange={(e) => setFormData((p: any) => ({ ...p, shortName: e.target.value }))} error={!!isSubmitted && !formData.shortName} /></Grid>
          <Grid item xs={12} sm={4}><FormDatePicker disabled={isReadOnly} label={<T>Established Date*</T>} value={formData.estDate ? dayjs(formData.estDate, "DD-MMM-YYYY") : null} onChange={(v) => setFormData((p: any) => ({ ...p, estDate: v ? v.format("DD-MMM-YYYY") : "" }))} /></Grid>
          <Grid item xs={12} sm={4}><FormInput disabled={isReadOnly} label={<T>Institute Name*</T>} value={formData.instituteName || ""} onChange={(e) => setFormData((p: any) => ({ ...p, instituteName: e.target.value }))} error={!!isSubmitted && !formData.instituteName} /></Grid>
          {showAcademicRegister && (
            <Grid item xs={12} sm={4}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("academicRegister")} label={<T>Academic Register</T>} value={formData.academicRegister || ""} options={academicRegisterOptions} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("academicRegisterId", "academicRegister", o)} /></Grid>
          )}
          <Grid item xs={12} sm={4}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("status")} label={<T>Status*</T>} value={formData.status || ""} options={lookups.statuses} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("statusId", "status", o)} error={!!isSubmitted && !formData.statusId} /></Grid>
        </Grid>
      </Box>

      <Box sx={{ mb: 3 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 2 }}>
          <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
          <Typography sx={{ fontWeight: 800, fontSize: "1rem", color: "#333" }}>
            <T>Identity & Assets</T>
          </Typography>
        </Box>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={3}>
            <Box>
              <Box sx={{ fontWeight: 700, fontSize: "0.85rem", mb: 0.5 }}><T>Logo</T></Box>
              <ImageUpload imageFit="contain" disabled={isReadOnly} currentImage={formData.logoPreview || ""} onImageSelect={onLogoSelect} onImageRemove={() => setFormData((p: any) => ({ ...p, logoFile: null, logoImage: null, logoPreview: "", imageId: null }))} />
            </Box>
          </Grid>
          <Grid item xs={12} sm={3}>
            <Box>
              <Box sx={{ fontWeight: 700, fontSize: "0.85rem", mb: 0.5 }}><T>Water Mark</T></Box>
              <ImageUpload imageFit="contain" disabled={isReadOnly} currentImage={formData.watermarkPreview || ""} onImageSelect={onWatermarkSelect} onImageRemove={() => setFormData((p: any) => ({ ...p, watermarkFile: null, watermarkImage: null, watermarkPreview: "", watermarkLogo: null }))} />
            </Box>
          </Grid>
        </Grid>
      </Box>

      <Box sx={{ mt: 2 }}>
        <StyledTabs value={tab} onChange={(_, v) => setTab(v)} tabs={tabs} />
        <TabPanel value={tab} index={0}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={4}><LookupDropdown preserveOptionLabels error={!!isSubmitted && !formData.institutionTypeId} disabled={isReadOnly} onSearchClick={() => openLookup("institutionType")} label={entityTypeLabel("Institution Type*")} value={formData.institutionType || ""} options={lookups.institutionTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("institutionTypeId", "institutionType", o)} /></Grid>
            <Grid item xs={12} sm={4}><LookupDropdown error={!!isSubmitted && !formData.instituteTypeId} disabled={isReadOnly} onSearchClick={() => openLookup("instituteType")} label={<T>Institute Type*</T>} value={formData.instituteType || ""} options={lookups.instituteTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("instituteTypeId", "instituteType", o)} /></Grid>
            {!isCompanyEntity && <Grid item xs={12} sm={4}><LookupDropdown error={!!isSubmitted && !formData.campusTypeId} disabled={isReadOnly} onSearchClick={() => openLookup("campusType")} label={entityTypeLabel("Campus Type*")} value={formData.campusType || ""} options={lookups.campusTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("campusTypeId", "campusType", o)} /></Grid>}
            <Grid item xs={12} sm={4}><FormInput disabled={isReadOnly} label={entityTypeLabel("Campus Name")} value={formData.campusName || ""} onChange={(e) => setFormData((p: any) => ({ ...p, campusName: e.target.value }))} /></Grid>
            <Grid item xs={12} sm={isCompanyEntity ? 12: 8}><FormInput disabled={isReadOnly} label={<T>Slogan</T>} value={formData.slogan || ""} onChange={(e) => setFormData((p: any) => ({ ...p, slogan: e.target.value }))} /></Grid>
          </Grid>
          <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 1, mt: 2 }}>
            <Button variant="outlined" startIcon={<AddIcon />} onClick={addEmployee} disabled={isReadOnly}><T>Add</T></Button>
          </Box>
          <TableContainer sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5 }}>
            <Table size="small">
              <TableHead><TableRow><TableCell sx={{ width: 80 }}><T>S.No</T></TableCell><TableCell><T>Type</T></TableCell><TableCell><T>Employee</T></TableCell><TableCell align="right"><T>Action</T></TableCell></TableRow></TableHead>
              <TableBody>
                {employees.map((row: any, i: number) => (
                  <TableRow key={row.id}>
                    <TableCell>{i + 1}</TableCell>
                    <TableCell sx={{ minWidth: 220 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("employeeType", row.id)} label={<T>Type</T>} value={row.type || ""} options={lookups.employeeTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updateEmployee(row.id, { typeId: o.id, type: o.name || "" })} /></TableCell>
                    <TableCell sx={{ minWidth: 260 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("employee", row.id)} label={<T>Employee</T>} value={row.employee || ""} options={lookups.employees} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updateEmployee(row.id, { employeeId: o.id, employee: o.name || "" })} /></TableCell>
                    <TableCell align="right"><IconButton color="error" onClick={() => removeEmployee(row.id)} disabled={isReadOnly}><DeleteIcon /></IconButton></TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </TabPanel>
        <TabPanel value={tab} index={addressTabIndex}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={4}>
              <LookupDropdown error={!!isSubmitted && !formData.countryId} disabled={isReadOnly} onSearchClick={() => openLookup("country")} label={<T>Country*</T>} value={formData.country || ""} options={lookups.countries} getOptionLabel={(o) => o.name || ""} onSelect={selectCountry} />
            </Grid>
            <Grid item xs={12} sm={4}>
              <LookupDropdown error={!!isSubmitted && !formData.stateId} disabled={isReadOnly || !formData.countryId || locationLoading} isLoading={locationLoading && !!formData.countryId} onSearchClick={() => openLookup("state")} label={<T>State/Province*</T>} value={formData.state || ""} options={lookups.states} getOptionLabel={(o) => o.name || ""} onSelect={selectState} />
            </Grid>
            <Grid item xs={12} sm={4}>
              <LookupDropdown error={!!isSubmitted && !formData.cityId} disabled={isReadOnly || !formData.stateId || locationLoading} isLoading={locationLoading && !!formData.stateId} onSearchClick={() => openLookup("city")} label={<T>City*</T>} value={formData.city || ""} options={lookups.cities} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("cityId", "city", o)} />
            </Grid>
            <Grid item xs={12} sm={4}><FormInput error={!!isSubmitted && !formData.owner} disabled={isReadOnly} label={<T>Owner*</T>} value={formData.owner || ""} onChange={(e) => setFormData((p: any) => ({ ...p, owner: e.target.value }))} /></Grid>
            <Grid item xs={12} sm={4}>
              <FormInput
                error={!isReadOnly && (isSubmitted || phoneTouched) && !!phoneError}
                disabled={isReadOnly || !formData.countryId}
                type="tel"
                debounceMs={0}
                label={<T>Contact No*</T>}
                value={formData.contactNo || ""}
                placeholder={phoneExample}
                helperText={!isReadOnly && (isSubmitted || phoneTouched) && phoneError ? translate(phoneError) : undefined}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <Tooltip arrow enterTouchDelay={0} title={phoneExample ? `${translate("Example")}: ${phoneExample}` : translate("Select Country first")}>
                        <IconButton size="small" aria-label={translate("Contact number format")} sx={{ color: "var(--lms-text-secondary)" }}>
                          <InfoOutlinedIcon sx={{ fontSize: 18 }} />
                        </IconButton>
                      </Tooltip>
                    </InputAdornment>
                  ),
                }}
                inputProps={{ maxLength: 50, inputMode: "tel" }}
                onBlur={() => setPhoneTouched(true)}
                onChange={(e) => setFormData((previous) => ({ ...previous, contactNo: e.target.value }))}
              />
            </Grid>
            <Grid item xs={12} sm={4}><LookupDropdown error={!!isSubmitted && !formData.regionId} disabled={isReadOnly} onSearchClick={() => openLookup("region")} label={<T>Region*</T>} value={formData.region || ""} options={lookups.regions} getOptionLabel={(o) => o.name || ""} onSelect={(o) => pick("regionId", "region", o)} /></Grid>
            <Grid item xs={12}><FormInput error={!!isSubmitted && !formData.address} disabled={isReadOnly} label={<T>Address*</T>} multiline rows={3} value={formData.address || ""} onChange={(e) => setFormData((p: any) => ({ ...p, address: e.target.value }))} /></Grid>
            <Grid item xs={12} sm={4}><FormInput disabled label={<T>Latitude</T>} value={formData.latitude ?? ""} /></Grid>
            <Grid item xs={12} sm={4}><FormInput disabled label={<T>Longitude</T>} value={formData.longitude ?? ""} /></Grid>
            <Grid item xs={12} sm={4}>
              <Button
                variant="outlined"
                startIcon={
                  <LocationOnIcon sx={{ fontSize: 18 }} />
                }
                onClick={() => setMapDialogOpen(true)}
                disabled={!formData.countryId}
                fullWidth
                sx={{
                  height: 40,
                  justifyContent: "center",
                  borderRadius: "9px",
                  borderColor: formData.latitude !== null && formData.longitude !== null ? "var(--lms-primary)" : "#e0e0e0",
                  color: formData.latitude !== null && formData.longitude !== null ? "var(--lms-primary)" : "var(--lms-text-main)",
                  bgcolor: "#fff",
                  fontSize: "0.85rem",
                  fontWeight: 700,
                  textTransform: "none",
                  boxShadow: "none",
                  "&:hover": {
                    borderColor: "var(--lms-primary)",
                    bgcolor: "rgba(var(--lms-primary-rgb), 0.04)",
                    boxShadow: "none",
                  },
                  "&.Mui-disabled": {
                    color: "var(--lms-text-secondary)",
                    borderColor: "#e0e0e0",
                    bgcolor: "#f5f5f5",
                  },
                  "& .MuiButton-startIcon": { mr: 0.75, color: "inherit" },
                }}
              >
                <T>Location Map</T>
              </Button>
            </Grid>
          </Grid>
          <LocationMapDialog
            open={mapDialogOpen}
            onClose={() => setMapDialogOpen(false)}
            onSelect={(latitude, longitude, locationPolygon) => setFormData((p: any) => ({ ...p, latitude, longitude, locationPolygon }))}
            countryName={formData.country || ""}
            countryCode={lookups.countries.find((x) => Number(x.id) === Number(formData.countryId))?.code}
            latitude={toCoordinate(formData.latitude)}
            longitude={toCoordinate(formData.longitude)}
            locationPolygon={normalizeLocationPolygon(formData.locationPolygon)}
            disabled={isReadOnly}
          />
        </TabPanel>
        <TabPanel value={tab} index={contactTabIndex}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1 }}>
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button variant="text" onClick={addContact} disabled={isReadOnly}><T>+ Add</T></Button>
              <Button variant="text" color="error" onClick={deleteSelectedContacts} disabled={isReadOnly || selectedContactIds.length === 0}><T>× Delete</T></Button>
            </Box>
          </Box>
          <TableContainer sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5 }}>
            <Table size="small">
              <TableHead><TableRow><TableCell sx={{ width: 52 }}><Checkbox size="small" checked={contacts.length > 0 && selectedContactIds.length === contacts.length} indeterminate={selectedContactIds.length > 0 && selectedContactIds.length < contacts.length} onChange={(e) => { if (!e.target.checked) setSelectedContactIds([]); else setSelectedContactIds(contacts.map((x: any) => Number(x.id))); }} disabled={isReadOnly} /></TableCell><TableCell><T>Name</T></TableCell><TableCell><T>Contact Type</T></TableCell><TableCell><T>Value</T></TableCell></TableRow></TableHead>
              <TableBody>
                {contacts.map((row: any) => (
                  <TableRow key={row.id}>
                    <TableCell><Checkbox size="small" checked={selectedContactIds.includes(Number(row.id))} onChange={(e) => { const id = Number(row.id); setSelectedContactIds(prev => e.target.checked ? [...prev, id] : prev.filter(x => x !== id)); }} disabled={isReadOnly} /></TableCell>
                    <TableCell><FormInput disabled={isReadOnly} value={row.name || ""} onChange={(e) => updateContact(row.id, { name: e.target.value })} /></TableCell>
                    <TableCell sx={{ minWidth: 200 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("contactType", row.id)} label={<T>Contact Type</T>} value={row.contactType || ""} options={lookups.contactTypes} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updateContact(row.id, { contactTypeId: o.id, contactType: o.name || "" })} /></TableCell>
                    <TableCell><FormInput disabled={isReadOnly} value={row.value || ""} onChange={(e) => updateContact(row.id, { value: e.target.value })} /></TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </TabPanel>
        <TabPanel value={tab} index={paymentTabIndex}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1 }}>
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button variant="text" onClick={addPayment} disabled={isReadOnly}><T>+ Add</T></Button>
              <Button variant="text" color="error" onClick={deleteSelectedPayments} disabled={isReadOnly || selectedPaymentIds.length === 0}><T>× Delete</T></Button>
            </Box>
          </Box>
          <TableContainer sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5 }}>
            <Table size="small">
              <TableHead><TableRow><TableCell sx={{ width: 52 }}><Checkbox size="small" checked={payments.length > 0 && selectedPaymentIds.length === payments.length} indeterminate={selectedPaymentIds.length > 0 && selectedPaymentIds.length < payments.length} onChange={(e) => { if (!e.target.checked) setSelectedPaymentIds([]); else setSelectedPaymentIds(payments.map((x: any) => Number(x.id))); }} disabled={isReadOnly} /></TableCell><TableCell><T>Vendor</T></TableCell><TableCell><T>Prefix</T></TableCell><TableCell><T>Pay Method</T></TableCell><TableCell><T>Bank</T></TableCell><TableCell align="center"><T>Is Default</T></TableCell></TableRow></TableHead>
              <TableBody>
                {payments.map((row: any) => (
                  <TableRow key={row.id}>
                    <TableCell><Checkbox size="small" checked={selectedPaymentIds.includes(Number(row.id))} onChange={(e) => { const id = Number(row.id); setSelectedPaymentIds(prev => e.target.checked ? [...prev, id] : prev.filter(x => x !== id)); }} disabled={isReadOnly} /></TableCell>
                    <TableCell sx={{ minWidth: 200 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("paymentVendor", row.id)} label={<T>Vendor</T>} value={row.vendor || ""} options={lookups.paymentVendors} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updatePayment(row.id, { vendorId: o.id, vendor: o.name || "" })} /></TableCell>
                    <TableCell><FormInput disabled={isReadOnly} value={row.prefix || ""} onChange={(e) => updatePayment(row.id, { prefix: e.target.value })} /></TableCell>
                    <TableCell sx={{ minWidth: 200 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("payMethod", row.id)} label={<T>Pay Method</T>} value={row.payMethod || ""} options={lookups.payMethods} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updatePayment(row.id, { payMethodId: o.id, payMethod: o.name || "" })} /></TableCell>
                    <TableCell sx={{ minWidth: 200 }}><LookupDropdown disabled={isReadOnly} onSearchClick={() => openLookup("bank", row.id)} label={<T>Bank</T>} value={row.bank || ""} options={lookups.banks} getOptionLabel={(o) => o.name || ""} onSelect={(o) => updatePayment(row.id, { bankId: o.id, bank: o.name || "" })} /></TableCell>
                    <TableCell align="center"><Checkbox size="small" checked={!!row.isDefault} onChange={(e) => setDefaultPayment(row.id, e.target.checked)} disabled={isReadOnly} /></TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </TabPanel>
        <TabPanel value={tab} index={templateTabIndex}>
          <Typography variant="body2" sx={{ mb: 1.5, color: "var(--lms-muted, #6b7280)" }}>
            {entityTypeLabel("These words are automatically replaced across the app for the selected Institution Type.")}
          </Typography>
          {!formData.institutionType ? (
            <Typography variant="body2" sx={{ p: 2 }}>{entityTypeLabel("Select an Institution Type to preview its terminology template.")}</Typography>
          ) : templateMappings.length === 0 ? (
            <Typography variant="body2" sx={{ p: 2 }}>{entityTypeLabel("No terminology overrides are configured for this Institution Type.")}</Typography>
          ) : (
            <TableContainer sx={{ border: "1px solid var(--lms-border)", borderRadius: 1.5 }}>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ width: 80 }}><T>S.No</T></TableCell>
                    <TableCell><T>Original Term</T></TableCell>
                    <TableCell><T>Replaced With</T></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {templateMappings.map((m, i) => (
                    <TableRow key={`${m.from}-${i}`}>
                      <TableCell>{i + 1}</TableCell>
                      <TableCell>{m.from}</TableCell>
                      <TableCell>{translate(m.to?.en || "")}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}
        </TabPanel>
        <TabPanel value={tab} index={aiRulesTabIndex}>
          <InstitutionAiRulesTab
            rules={formData.aiRules ?? {}}
            onChange={(aiRules) => setFormData((p: any) => ({ ...p, aiRules }))}
            disabled={isReadOnly || !!aiRulesNotice || formData.aiRules === undefined}
            notice={aiRulesNotice}
          />
        </TabPanel>
      </Box>
    </FormThemeWrapper>
  );
};

interface InstitutionProps {
  onSubViewChange?: (subView: string | null) => void;
  resetKey?: number;
}

const Institution: React.FC<InstitutionProps> = ({ onSubViewChange, resetKey }) => {
  const dispatch = useAppDispatch();
  const selectedCampusId = useAppSelector((state) => state.auth.selectedCampusId);
  const { translate } = useTranslation();
  const [rows, setRows] = useState<any[]>([]);
  const [lookups, setLookups] = useState<Lookups>(emptyLookups);
  const [loading, setLoading] = useState(false);
  const [showToaster, setShowToaster] = useState(false);
  const [toasterMessages, setToasterMessages] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<"list" | "new" | "edit" | "view">("list");
  const [formData, setFormData] = useState<FormRow>(makeInitial());
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [locationLoading, setLocationLoading] = useState(false);
  const lookupRequestId = useRef(0);
  const isCompanyEntity = (formData.entityType || "").trim().toLowerCase() === "company";
  const formPageLabel = (value: string) => translate(
    isCompanyEntity ? value.replace(/\bInstitution\b/g, "Organization").replace(/\bCampus\b/g, "Branch") : value,
  );
  const activeEntityId = () => Number(
    selectedCampusId || localStorage.getItem("selectedEntity") || localStorage.getItem("selectedEntityId") || 0,
  );

  useEffect(() => {
    if (!onSubViewChange) return;
    if (viewMode === "new") onSubViewChange(formPageLabel("New Institution"));
    else if (viewMode === "edit") onSubViewChange(formPageLabel("Edit Institution"));
    else if (viewMode === "view") onSubViewChange(formPageLabel("View Institution"));
    else onSubViewChange(null);
  }, [onSubViewChange, viewMode, formData.entityType, translate]);

  useEffect(() => {
    if (resetKey && resetKey > 0) {
      setViewMode("list");
    }
  }, [resetKey]);

  const loadLookups = useCallback(async (countryId?: number, stateId?: number) => {
    const requestId = ++lookupRequestId.current;
    setLocationLoading(true);
    setLookups((previous) => ({ ...previous, states: [], cities: [] }));
    const q = new URLSearchParams();
    if (countryId) q.set("countryId", String(countryId));
    if (stateId) q.set("stateId", String(stateId));
    const endpoint = q.toString() ? `/api/organization/Institution/v3/lookups?${q.toString()}` : "/api/organization/Institution/v3/lookups";
    try {
      const response: any = await GetService(endpoint);
      if (requestId !== lookupRequestId.current) return;
      const data = response?.data ?? {};
      setLookups({
        entityTypes: data.entityTypes ?? [], headCampuses: data.headCampuses ?? [], employeeTypes: data.employeeTypes ?? [], institutionTypes: data.institutionTypes ?? [], instituteTypes: data.instituteTypes ?? [],
        campusTypes: data.campusTypes ?? [], statuses: data.statuses ?? [], contactTypes: data.contactTypes ?? [],
        payMethods: data.payMethods ?? [], academicRegisters: data.academicRegisters ?? [], regions: data.regions ?? [],
        paymentVendors: data.paymentVendors ?? [], banks: data.banks ?? [], employees: data.employees ?? [], countries: data.countries ?? [],
        states: countryId ? data.states ?? [] : [], cities: countryId && stateId ? data.cities ?? [] : [],
      });
    } catch (e) {
      console.error("Lookup error", e);
    } finally {
      if (requestId === lookupRequestId.current) setLocationLoading(false);
    }
  }, []);

  // Reload dependent options for new forms, edits and both picker entry points.
  // Only the newest request can publish options after a quick selection change.
  useEffect(() => {
    if (viewMode === "list") return;
    void loadLookups(formData.countryId, formData.stateId);
    return () => { lookupRequestId.current += 1; };
  }, [viewMode, formData.countryId, formData.stateId, loadLookups]);

  const fetchRows = async (forceRefresh = false) => {
    const refreshParam = forceRefresh ? `&refresh=${Date.now()}` : "";
    const response: any = await GetService(`/api/organization/Institution/v3?pageNumber=1&pageSize=1000${refreshParam}`);
    const items = response?.data?.items ?? response?.data?.Items ?? [];
    setRows(items.map((x: any, i: number) => ({ ...x, sNo: i + 1, entityName: x.instituteName, institute: x.instituteType })));
  };

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        await Promise.all([loadLookups(formData.countryId, formData.stateId), fetchRows()]);
      } catch (e: any) {
        fail(e?.response?.data?.error || translate("Failed to load institutions"));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [translate]);

  const fail = (msg: string | string[]) => {
    setToasterMessages(Array.isArray(msg) ? msg : [msg]);
    setShowToaster(true);
  };

  const validate = (d: FormRow) => {
    const errors = organizationTextErrors(d, rows, [
      {field: "code", label: "Code", maxLength: 20, duplicate: true},
      {field: "instituteName", label: "Institute Name", maxLength: 500, duplicate: true},
      {field: "shortName", label: "Short Name", maxLength: 50},
      {field: "owner", label: "Owner", maxLength: 200},
      {field: "contactNo", label: "Contact No", maxLength: 50},
      {field: "address", label: "Address", maxLength: 500},
      {field: "slogan", label: "Slogan", maxLength: 500},
    ]).map(message => translate(message));
    const isCompanyEntity = (d.entityType || "").trim().toLowerCase() === "company";
    const isSubCampusEntity = ["sub campus", "subcampus"].some((x) => (d.entityType || "").trim().toLowerCase().includes(x));
    if (!CheckemptyString(d.code)) errors.push(translate("Code is required"));
    if (!d.entityTypeId) errors.push(translate("Entity Type is required"));
    if (!CheckemptyString(d.shortName)) errors.push(translate("Short Name is required"));
    if (!CheckemptyString(d.estDate)) errors.push(translate("Established Date is required"));
    if (!CheckemptyString(d.instituteName)) errors.push(translate("Institute Name is required"));
    if (!d.statusId) errors.push(translate("Status is required"));
    if (!isCompanyEntity && !d.institutionTypeId) errors.push(translate("Institution Type is required"));
    if (!isCompanyEntity && !d.instituteTypeId) errors.push(translate("Institute Type is required"));
    if (!isCompanyEntity && !d.campusTypeId) errors.push(translate("Campus Type is required"));
    if (isSubCampusEntity && !d.headEntityId) errors.push(translate("Head Campus is required"));
    if (!CheckemptyString(d.owner)) errors.push(translate("Owner is required"));
    const phoneError = institutionPhoneError(d.contactNo || "", lookups.countries.find((country) => Number(country.id) === Number(d.countryId))?.code);
    if (phoneError) errors.push(translate(phoneError));
    if (!CheckemptyString(d.address)) errors.push(translate("Address is required"));
    if (!d.countryId) errors.push(translate("Country is required"));
    if (!d.stateId) errors.push(translate("State is required"));
    if (!d.cityId) errors.push(translate("City is required"));
    if (!d.regionId) errors.push(translate("Region is required"));
    // A blank row is kept in the grid for data entry, but online payment itself
    // is optional for an institution. Only validate rows where payment details
    // have actually been supplied.
    const configuredPayments = (d.payments || []).filter(
      (p) => p.vendorId || p.payMethodId || p.prefix?.trim() || p.bankId || p.isDefault,
    );
    const defaults = configuredPayments.filter((p) => p.isDefault).length;
    if (configuredPayments.length > 0 && defaults !== 1) errors.push(translate("Online Payment must contain exactly one default row"));

    return errors;
  };


  const handleSave = async (data: FormRow) => {
    setIsSubmitted(true);
    const errors = validate(data);
    if (errors.length) {
      fail(errors);
      return false;
    }

    setLoading(true);
    const isCompanyEntity = (data.entityType || "").trim().toLowerCase() === "company";
    const payload: any = {
      id: Number(data.id || 0),
      code: data.code?.trim(),
      entityTypeId: data.entityTypeId,
      headEntityId: data.headEntityId || null,
      shortName: data.shortName?.trim(),
      estDate: data.estDate,
      instituteName: data.instituteName?.trim(),
      academicRegisterId: data.academicRegisterId,
      statusId: data.statusId,
      institutionTypeId: data.institutionTypeId,
      instituteTypeId: data.instituteTypeId,
      campusTypeId: isCompanyEntity ? null : data.campusTypeId,
      campusName: data.campusName || "",
      slogan: data.slogan || "",
      owner: data.owner?.trim(),
      contactNo: data.contactNo?.trim(),
      address: data.address?.trim(),
      countryId: data.countryId,
      stateId: data.stateId,
      cityId: data.cityId,
      regionId: data.regionId,
      latitude: toCoordinate(data.latitude),
      longitude: toCoordinate(data.longitude),
      locationPolygon: normalizeLocationPolygon(data.locationPolygon),
      imageId: data.imageId || null,
      watermarkLogo: data.watermarkLogo || null,
      logoImage: data.logoImage || null,
      watermarkImage: data.watermarkImage || null,
      employees: (data.employees || []).filter((e) => e.typeId || e.employeeId).map((e) => ({ id: e.id || 0, typeId: e.typeId || 0, employeeId: e.employeeId || 0 })),
      contacts: (data.contacts || []).filter((c) => c.name || c.contactTypeId || c.value).map((c) => ({ id: c.id || 0, name: c.name || "", contactTypeId: c.contactTypeId || 0, value: c.value || "" })),
      payments: (data.payments || []).filter((p) => p.vendorId || p.payMethodId || p.prefix || p.bankId).map((p) => ({ id: p.id || 0, vendorId: p.vendorId || 0, prefix: p.prefix || "", payMethodId: p.payMethodId || 0, bankId: p.bankId || 0, isDefault: !!p.isDefault })),
    };

    try {
      if (payload.id > 0) await PutService(`/api/organization/Institution/v3/${payload.id}`, payload);
      else await PostService("/api/organization/Institution/v3", payload);
      if (payload.id > 0 && data.aiRules && data.aiRulesAvailable) {
        try {
          await PutService(`/api/organization/Institution/v3/${payload.id}/ai-rules`, {
            rules: Object.entries(data.aiRules).map(([featureCode, prompt]) => ({ featureCode, prompt })),
          });
        } catch (e: any) {
          fail(e?.response?.data?.error || translate("Institution saved, but its AI Rules could not be saved"));
          return false;
        }
      }
      if (Number(payload.id) === activeEntityId()) {
        // Make terminology react immediately to a type change. The campus list is
        // populated at login and can otherwise retain the previous institution type.
        syncExecutiveInstitutionType(data.institutionType, data.institutionTypeId, data.entityType, data.entityTypeId);
        try {
          const { students, employees } = await fetchBirthdayNotifications(payload.id, { useCache: false });
          dispatch(setBirthdays({ students, employees }));
        } catch (error) {
          console.error("Failed to refresh executive data after saving the institution:", error);
        }
      }
      await fetchRows(true);
      setViewMode("list");
      return true;
    } catch (e: any) {
      fail(e?.response?.data?.error || translate("Failed to save institution"));
      return false;
    } finally {
      setLoading(false);
    }
  };

  const toImagePreview = (imageId?: number | null) => imageId ? `/api/HumanResource/v3/image/${imageId}` : "";
  const toFormRow = (row: any): FormRow => ({
    ...row,
    sNo: undefined,
    entityName: undefined,
    institute: undefined,
    logoFile: null,
    watermarkFile: null,
  logoImage: null,
  watermarkImage: null,
    logoPreview: row.logoPreview || toImagePreview(row.imageId ?? row.ImageId ?? row.imageid),
    watermarkPreview: row.watermarkPreview || toImagePreview(row.watermarkLogo ?? row.WatermarkLogo),
  });

  const handleAddClick = () => { setFormData(makeInitial()); setViewMode("new"); setIsSubmitted(false); };
  const handleEditClick = (row: any) => {
    const next = toFormRow(row);
    setFormData(next);
    if (Number(next.id) === activeEntityId()) {
      syncExecutiveInstitutionType(next.institutionType, next.institutionTypeId, next.entityType, next.entityTypeId);
    }
    setViewMode("edit");
    setIsSubmitted(false);
  };
  const handleViewClick = (row: any) => { setFormData(toFormRow(row)); setViewMode("view"); setIsSubmitted(false); };
  const handleBackToList = () => { setViewMode("list"); setFormData(makeInitial()); setIsSubmitted(false); };

  const handleDelete = async (row: any) => {
    const id = Number(row.id || 0);
    if (!id) return;
    setLoading(true);
    try {
      await DeleteService(`/api/organization/Institution/v3/${id}`);
      await fetchRows(true);
    } catch (e: any) { fail(e?.response?.data?.error || translate("Failed to delete institution")); }
    finally { setLoading(false); }
  };

  const columns: Column[] = [
    { id: "sNo", label: translate("S.No"), minWidth: 60 },
    { id: "code", label: translate("Code"), minWidth: 110 },
    { id: "estDate", label: translate("Established Date"), minWidth: 130 },
    { id: "entityName", label: translate("Institute Name"), minWidth: 220 },
    { id: "owner", label: translate("Owner"), minWidth: 160 },
    { id: "contactNo", label: translate("Contact No"), minWidth: 160 },
    { id: "address", label: translate("Address"), minWidth: 220 },
    { id: "country", label: translate("Country"), minWidth: 140 },
    { id: "state", label: translate("State"), minWidth: 140 },
    { id: "city", label: translate("City"), minWidth: 140 },
    { id: "region", label: translate("Region"), minWidth: 140 },
    { id: "institute", label: translate("Institute Type"), minWidth: 160 },
  ];

  return (
    <>
      {viewMode === "list" ? (
        <SchemaTable showToaster={false}
          title={translate("Institution")}
          addButtonLabel={translate("New Institution")}
          breadcrumbPath=""
          columns={columns}
          rows={rows}
          initialFormData={makeInitial()}
          onSave={handleSave}
          onDelete={handleDelete}
          onAddClick={handleAddClick}
          onEditClick={handleEditClick}
          onViewClick={handleViewClick}
          hideTitle={true}
          hideBreadcrumbs={true}
          mandatoryFields={[]}
        />
      ) : (
        <Box sx={{ mt: -1 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
            <Typography variant="h4" sx={{ fontWeight: 900, color: "var(--lms-text-main)" }}>
              {viewMode === "new" ? formPageLabel("New Institution") : viewMode === "edit" ? formPageLabel("Edit Institution") : formPageLabel("View Institution")}
            </Typography>
            <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
              <Button
                variant="text"
                startIcon={<ArrowBackIcon sx={{ fontSize: "1.2rem" }} />}
                onClick={handleBackToList}
                sx={{ color: "var(--lms-primary-dark)", fontWeight: 700, textTransform: "none", "&:hover": { bgcolor: "rgba(var(--lms-primary-rgb), 0.05)" } }}
              >
                <T>Back to List</T>
              </Button>
              {viewMode !== "view" && (
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={() => handleSave(formData)}
                  sx={{
                    bgcolor: "var(--lms-primary)", color: "#fff", px: 4, py: 1, borderRadius: "10px", fontWeight: 700, textTransform: "none",
                    boxShadow: "0 4px 12px rgba(var(--lms-primary-rgb), 0.2)", "&:hover": { bgcolor: "var(--lms-primary-dark)", boxShadow: "0 6px 16px rgba(var(--lms-primary-rgb), 0.3)" }
                  }}
                >
                  <T>Save</T>
                </Button>
              )}
            </Box>
          </Box>
          <Box sx={{ bgcolor: "#fff", p: 0, borderRadius: "16px" }}>
            <InstitutionForm
              formData={formData}
              setFormData={setFormData}
              viewMode={viewMode}
              lookups={lookups}
              locationLoading={locationLoading}
              isSubmitted={isSubmitted}
            />
          </Box>
        </Box>
      )}

      {loading && <LmsOverlayLoader />}
      <LmsToaster open={showToaster} onClose={() => setShowToaster(false)} messages={toasterMessages} severity="error" disableDeleteSuccessAutoSeverity={true} />
    </>
  );
};

export default Institution;
