import { GetService } from '@/constants/globalItems';
import { createCrudSlice } from './createCrudSlice';

/**
 * Cached slices for the Organization Setup screens.
 *
 * Each screen previously held its rows in `useState` and refetched on every mount, so
 * moving between screens re-issued the same GETs. These slices hold the rows in the store,
 * fetch once per scope, and are patched in place by the CRUD handlers on each screen.
 *
 * Row shapes mirror exactly what each grid already rendered. `sNo` is deliberately NOT
 * stored: it is a display index and must be derived at render time, otherwise it goes
 * stale as soon as a row is inserted into or removed from the cache.
 */

const num = (v: any, fallback = 0) => Number(v ?? fallback) || fallback;
const str = (v: any, fallback = '') => v ?? fallback;
const byText = (key: string) => (a: any, b: any) =>
  String(a[key] ?? '').localeCompare(String(b[key] ?? ''));

/* ------------------------------------------------------------------ Location */

export interface CountryRow {
  id: number;
  countryid: number;
  countrycode: string;
  countryname: string;
  isocode: string;
  dialcode: string;
  currencycode: string;
  currencyname: string;
  currencysymbol: string;
  isactive: boolean;
}

export const countrySlice = createCrudSlice<CountryRow>({
  name: 'orgCountry',
  url: '/api/organization/Country/v3?pageNumber=1&pageSize=1000&includeInactive=true',
  getId: (r) => r.countryid,
  sort: byText('countryname'),
  normalize: (item) => {
    const countryid = num(item.countryid ?? item.id);
    return {
      id: countryid,
      countryid,
      countrycode: str(item.countrycode),
      countryname: str(item.countryname),
      isocode: str(item.isocode),
      dialcode: str(item.dialcode),
      currencycode: str(item.currencycode),
      currencyname: str(item.currencyname),
      currencysymbol: str(item.currencysymbol),
      isactive: !!item.isactive,
    };
  },
});

export interface RegionRow {
  id: number;
  regionid: number;
  regioncode: string;
  regionname: string;
  countryid: number;
  countryname: string;
  isactive: boolean;
}

export const regionSlice = createCrudSlice<RegionRow>({
  name: 'orgRegion',
  url: '/api/organization/Region/v3?pageNumber=1&pageSize=1000&includeInactive=true',
  getId: (r) => r.regionid,
  sort: byText('regionname'),
  normalize: (item) => {
    const regionid = num(item.regionid ?? item.regionId ?? item.id);
    return {
      id: regionid,
      regionid,
      regioncode: str(item.regioncode ?? item.regionCode ?? item.code),
      regionname: str(item.regionname ?? item.regionName ?? item.region),
      countryid: num(item.countryid ?? item.countryId),
      countryname: str(item.countryname ?? item.countryName),
      isactive: !!(item.isactive ?? item.isActive ?? true),
    };
  },
});

export interface StateRow {
  id: number;
  stateid: number;
  statecode: string;
  statename: string;
  countryid: number;
  countryname: string;
  isactive: boolean;
}

export const stateSlice = createCrudSlice<StateRow>({
  name: 'orgState',
  url: '/api/organization/State/v3?pageNumber=1&pageSize=1000&includeInactive=true',
  getId: (r) => r.stateid,
  sort: byText('statename'),
  normalize: (item) => {
    const stateid = num(item.stateid ?? item.stateId ?? item.id);
    return {
      id: stateid,
      stateid,
      statecode: str(item.statecode ?? item.stateCode),
      statename: str(item.statename ?? item.stateName),
      countryid: num(item.countryid ?? item.countryId),
      countryname: str(item.countryname ?? item.countryName),
      isactive: !!(item.isactive ?? item.isActive ?? true),
    };
  },
});

export interface CityRow {
  id: number;
  cityid: number;
  citycode: string;
  cityname: string;
  stateid: number;
  statename: string;
  countryid: number;
  countryname: string;
  dialcode: string;
  isactive: boolean;
}

export const citySlice = createCrudSlice<CityRow>({
  name: 'orgCity',
  url: '/api/organization/City/v3?pageNumber=1&pageSize=1000&includeInactive=true',
  getId: (r) => r.cityid,
  sort: byText('cityname'),
  normalize: (item) => {
    const cityid = num(item.cityid ?? item.cityId ?? item.id);
    return {
      id: cityid,
      cityid,
      citycode: str(item.citycode ?? item.cityCode ?? item.code),
      cityname: str(item.cityname ?? item.cityName ?? item.city),
      stateid: num(item.stateid ?? item.stateId),
      statename: str(item.statename ?? item.stateName ?? item.state),
      countryid: num(item.countryid ?? item.countryId),
      countryname: str(item.countryname ?? item.countryName ?? item.country),
      dialcode: str(item.dialcode ?? item.dialCode),
      isactive: !!(item.isactive ?? item.isActive ?? true),
    };
  },
});

export const orgSiteSlice = createCrudSlice<any>({
  name: 'orgSite',
  url: '/api/organization/Site/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r?.id,
  lookups: {
    siteTypes: '/api/organization/Site/v3/lookups/site-types',
    parentSites: '/api/organization/Site/v3/lookups/parent-sites',
    siteFor: '/api/organization/Site/v3/lookups/site-for',
    regions: '/api/organization/Region/v3?pageNumber=1&pageSize=1000&includeInactive=true',
  },
  normalize: (item) => item,
});

/* --------------------------------------------------------------- Room Setup */

export interface BuildingRow {
  id: number;
  code: string;
  stxt: string;
}

export const buildingSlice = createCrudSlice<BuildingRow>({
  name: 'orgBuilding',
  url: '/api/organization/Building/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r.id,
  sort: byText('stxt'),
  normalize: (item) => ({
    id: num(item.id),
    code: str(item.code),
    stxt: str(item.stxt),
  }),
});

export interface FloorRow {
  id: number;
  code: string;
  stxt: string;
  buildingid: number;
  buildingname: string;
}

export const floorSlice = createCrudSlice<FloorRow>({
  name: 'orgFloor',
  url: '/api/organization/Floor/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r.id,
  sort: byText('stxt'),
  lookups: {
    buildings: '/api/organization/Room/v3/lookups/buildings',
  },
  normalize: (item) => ({
    id: num(item.id),
    code: str(item.code),
    stxt: str(item.stxt),
    buildingid: num(item.buildingid),
    buildingname: str(item.buildingname),
  }),
});

export interface RoomRow {
  id: number;
  code: string;
  stxt: string;
  roomtypeid: number;
  roomtypename: string;
  buildingid: number;
  buildingname: string;
  lovfloorid: number;
  floorname: string;
  strength: string;
}

export const roomSlice = createCrudSlice<RoomRow>({
  name: 'orgRoom',
  url: '/api/organization/Room/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r.id,
  sort: byText('stxt'),
  lookups: {
    roomTypes: '/api/organization/Room/v3/lookups/room-types',
    buildings: '/api/organization/Room/v3/lookups/buildings',
  },
  normalize: (item) => ({
    id: num(item.id),
    code: str(item.code),
    stxt: str(item.stxt),
    roomtypeid: num(item.roomtypeid),
    roomtypename: str(item.roomtypename),
    buildingid: num(item.buildingid),
    buildingname: str(item.buildingname),
    lovfloorid: num(item.lovfloorid),
    floorname: str(item.floorname),
    strength: str(item.strength),
  }),
});

/* -------------------------------------------------------------------- Setup */

/*
 * The screens below map API fields at render time rather than at fetch time, so these
 * slices keep the raw record. Reshaping here would change what their grids display.
 */

export const departmentSlice = createCrudSlice<any>({
  name: 'orgDepartment',
  url: '/api/organization/Department/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r?.id ?? r?.departmentid,
  lookups: {
    ficoa: '/api/organization/Department/v3/ficoa-lookup',
  },
  normalize: (item) => item,
});

export const designationSlice = createCrudSlice<any>({
  name: 'orgDesignation',
  url: '/api/organization/Designation/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r?.id ?? r?.designationid,
  normalize: (item) => item,
});

export const occupationSlice = createCrudSlice<any>({
  name: 'orgOccupation',
  url: '/api/organization/Occupation/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r?.id ?? r?.occupationid,
  normalize: (item) => item,
});



export const institutionSlice = createCrudSlice<any>({
  name: 'orgInstitution',
  url: '/api/organization/Institution/v3?pageNumber=1&pageSize=1000',
  getId: (r) => r?.id ?? r?.institutionid,
  lookups: {
    all: '/api/organization/Institution/v3/lookups',
  },
  normalize: (item) => item,
});
