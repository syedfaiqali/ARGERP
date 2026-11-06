-- Supporting master data for the standalone installation. No source business records are copied.
SET XACT_ABORT ON;
DECLARE @defaults nvarchar(max)=N'';
SELECT @defaults+=N'ALTER TABLE '+QUOTENAME(OBJECT_SCHEMA_NAME(c.object_id))+N'.'+QUOTENAME(OBJECT_NAME(c.object_id))+N' ADD DEFAULT (0) FOR '+QUOTENAME(c.name)+N';'
FROM sys.columns c JOIN sys.tables t ON t.object_id=c.object_id
WHERE c.name IN ('utsysgen','utblock','utispartial','utrecordversion') AND c.default_object_id=0;
EXEC sys.sp_executesql @defaults;
GO
IF OBJECT_ID('dbo.religion','U') IS NULL CREATE TABLE dbo.religion (Id int NOT NULL PRIMARY KEY, religioncode nvarchar(20), stxt nvarchar(100), ltxt nvarchar(500), utblock bit NOT NULL DEFAULT(0));
IF OBJECT_ID('dbo.utnumberhdr','U') IS NULL CREATE TABLE dbo.utnumberhdr (id int NOT NULL PRIMARY KEY, clientid int, fieldid nvarchar(100), modelname nvarchar(100), prefix nvarchar(50), postfix nvarchar(50), counterlength int, totallength int, startvalue int, endvalue int, allowpadding bit, paddingvalue nvarchar(10), isdefault bit, utblock bit NOT NULL DEFAULT(0));
GO
CREATE OR ALTER VIEW dbo.utusers AS SELECT Id userid, Username username, FullName firstname, CAST('' AS nvarchar(100)) lastname, EmployeeId empid, ClientId userclientid, EntityId entityid, Blocked utblock FROM dbo.ArgUsers;
GO
CREATE OR ALTER VIEW dbo.utuserent AS SELECT u.Id*100000+e.id id,u.Id userid,e.id entityid,CAST(0 AS bit) utblock FROM dbo.ArgUsers u JOIN dbo.OrgLegalentity e ON e.clientid=u.ClientId WHERE u.Blocked=0 AND e.utblock=0;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=337) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(337,N'M',N'Male',N'Male',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=338) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(338,N'F',N'Female',N'Female',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=339) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(339,N'O',N'Other',N'Other',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=125) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(125,N'ACT',N'Active',N'Active',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=126) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(126,N'RES',N'Resigned',N'Resigned',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=127) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(127,N'TER',N'Terminated',N'Terminated',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=267) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(267,N'ADMIN',N'Administration',N'Administration',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=268) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(268,N'MGT',N'Management',N'Management',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=269) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(269,N'TEACH',N'Teaching Staff',N'Teaching Staff',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=571) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(571,N'SUPPORT',N'Support Staff',N'Support Staff',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=561) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(561,N'ACT',N'Active',N'Active',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=562) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(562,N'DEV',N'Under Development',N'Under Development',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=563) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(563,N'SUS',N'Suspended',N'Suspended',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=368) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(368,N'QUAL',N'Qualification',N'Qualification',N'QTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2001) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2001,N'ACT',N'Active',N'Active',N'AACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2002) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2002,N'INA',N'Inactive',N'Inactive',N'AACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2010) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2010,N'PERM',N'Permanent',N'Permanent',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2011) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2011,N'CONTRACT',N'Contract',N'Contract',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2012) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2012,N'PROB',N'Probation',N'Probation',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2013) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2013,N'TEMP',N'Temporary',N'Temporary',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2020) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2020,N'FULL',N'Full Time',N'Full Time',N'PSTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2021) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2021,N'PART',N'Part Time',N'Part Time',N'PSTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2030) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2030,N'SINGLE',N'Single',N'Single',N'MRSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2031) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2031,N'MARRIED',N'Married',N'Married',N'MRSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2040) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2040,N'HOME',N'Home',N'Home',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2041) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2041,N'PERM',N'Permanent',N'Permanent',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2042) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2042,N'OFFICE',N'Office',N'Office',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2050) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2050,N'SALARY',N'Salary',N'Salary',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2051) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2051,N'CURRENT',N'Current',N'Current',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2052) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2052,N'SAVINGS',N'Savings',N'Savings',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2060) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2060,N'DAYS',N'Days',N'Days',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2061) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2061,N'HOURS',N'Hours',N'Hours',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2062) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2062,N'MONTHS',N'Months',N'Months',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2070) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2070,N'PEND',N'Pending',N'Pending',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2071) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2071,N'APP',N'Approved',N'Approved',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2072) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2072,N'REJ',N'Rejected',N'Rejected',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2073) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2073,N'CAN',N'Cancelled',N'Cancelled',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2080) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2080,N'PEND',N'Pending',N'Pending',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2081) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2081,N'APP',N'Approved',N'Approved',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2082) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2082,N'REJ',N'Rejected',N'Rejected',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2090) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2090,N'FIXED',N'Fixed',N'Fixed',N'LDT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2091) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2091,N'PERCENT',N'Percentage',N'Percentage',N'LDT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2100) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2100,N'ENTITY',N'Branch',N'Branch',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2101) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2101,N'DEPARTMENT',N'Department',N'Department',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2102) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2102,N'ALL',N'All Employees',N'All Employees',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2110) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2110,N'JOIN',N'Joining',N'Joining',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2111) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2111,N'PROM',N'Promotion',N'Promotion',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2112) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2112,N'TRANSFER',N'Transfer',N'Transfer',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2113) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2113,N'RESIGN',N'Resignation',N'Resignation',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2120) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2120,N'PEND',N'Pending',N'Pending',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2121) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2121,N'APP',N'Approved',N'Approved',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2122) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2122,N'POST',N'Posted',N'Posted',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2130) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2130,N'OPEN',N'Open',N'Open',N'STATS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2131) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2131,N'CLOSED',N'Closed',N'Closed',N'STATS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2140) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2140,N'DAILY',N'Daily',N'Daily',N'ATNTY',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2141) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2141,N'MONTHLY',N'Monthly',N'Monthly',N'ATNTY',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2150) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2150,N'SHIFT',N'Shift Based',N'Shift Based',N'ATPOL',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2151) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2151,N'FLEX',N'Flexible',N'Flexible',N'ATPOL',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2160) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2160,N'PAID',N'Paid',N'Paid',N'LTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2161) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2161,N'UNPAID',N'Unpaid',N'Unpaid',N'LTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2170) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2170,N'ADD',N'Addition',N'Addition',N'ADJP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2171) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2171,N'DEDUCT',N'Deduction',N'Deduction',N'ADJP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2180) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2180,N'HIGH',N'High',N'High',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2181) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2181,N'NORMAL',N'Normal',N'Normal',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2182) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2182,N'LOW',N'Low',N'Low',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2190) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2190,N'BANK',N'Bank',N'Bank',N'PAYTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2191) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2191,N'CASH',N'Cash',N'Cash',N'PAYTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2200) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2200,N'NATIONAL',N'National ID',N'National ID',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2201) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2201,N'PASSPORT',N'Passport',N'Passport',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2202) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2202,N'QUAL',N'Qualification Certificate',N'Qualification Certificate',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2210) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2210,N'PUBLIC',N'Public',N'Public',N'HOTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2211) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2211,N'ORG',N'Organization',N'Organization',N'HOTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2220) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2220,N'RECOGNITION',N'Recognition',N'Recognition',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2221) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2221,N'TRAINING',N'Training',N'Training',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2222) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2222,N'ACHIEVEMENT',N'Achievement',N'Achievement',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2223) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2223,N'DISCIPLINARY',N'Disciplinary',N'Disciplinary',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2224) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2224,N'LEGAL',N'Legal',N'Legal',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2230) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2230,N'HEAD',N'Head Office',N'Head Office',N'ENTITYTYPE',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2231) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2231,N'BRANCH',N'Branch',N'Branch',N'ENTITYTYPE',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2300) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2300,N'A+',N'A+',N'A+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2301) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2301,N'A-',N'A-',N'A-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2302) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2302,N'B+',N'B+',N'B+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2303) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2303,N'B-',N'B-',N'B-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2304) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2304,N'AB+',N'AB+',N'AB+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2305) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2305,N'AB-',N'AB-',N'AB-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2306) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2306,N'O+',N'O+',N'O+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2307) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2307,N'O-',N'O-',N'O-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2401) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2401,N'1',N'January',N'January',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2402) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2402,N'2',N'February',N'February',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2403) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2403,N'3',N'March',N'March',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2404) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2404,N'4',N'April',N'April',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2405) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2405,N'5',N'May',N'May',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2406) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2406,N'6',N'June',N'June',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2407) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2407,N'7',N'July',N'July',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2408) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2408,N'8',N'August',N'August',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2409) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2409,N'9',N'September',N'September',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2410) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2410,N'10',N'October',N'October',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2411) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2411,N'11',N'November',N'November',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2412) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2412,N'12',N'December',N'December',N'M',0);

IF NOT EXISTS(SELECT 1 FROM dbo.Country WHERE Id=1) INSERT dbo.Country(Id,countrycode,countryname,isocode,dialcode) VALUES(1,'PK','Pakistan','PAK','+92');
IF NOT EXISTS(SELECT 1 FROM dbo.State WHERE Id=1) INSERT dbo.State(Id,countryid,stxt,statecode) VALUES(1,1,'Sindh','SD');
IF NOT EXISTS(SELECT 1 FROM dbo.City WHERE Id=1) INSERT dbo.City(Id,ctryid,stateid,citystxt,citycode) VALUES(1,1,1,'Karachi','KHI');
IF NOT EXISTS(SELECT 1 FROM dbo.OrgLegalentity WHERE Id=1) INSERT dbo.OrgLegalentity(id,clientid,entityno,stxt,shortname,campusname,countryid,cityid) VALUES(1,1,'ARG-001','ARG ERP Head Office','ARG ERP','Head Office',1,1);
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDepartment WHERE Id=1) INSERT dbo.OrgDepartment(Id,depcode,depstxt,clientid,opunitid) VALUES(1,'00001','Human Resources',1,1);
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDesignation WHERE Id=1) INSERT dbo.OrgDesignation(Id,stxt,clientid) VALUES(1,'HR Administrator',1);
IF NOT EXISTS(SELECT 1 FROM dbo.EREmployeeinfohdr WHERE Id=1) INSERT dbo.EREmployeeinfohdr(Id,clientid,entityid,employeecode,firstname,lastname,depid,designationid,empstatusid,lvemployeecateg,joiningdate) VALUES(1,1,1,'SYS-ADMIN','System','Administrator',1,1,125,267,CONVERT(date,GETDATE()));
IF NOT EXISTS(SELECT 1 FROM dbo.SMPolicy WHERE Id=1) INSERT dbo.SMPolicy(Id,clientid,entityid,defaultctryid,defaultcityid,utblock) VALUES(1,1,1,1,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.religion WHERE Id=1) INSERT dbo.religion(Id,religioncode,stxt,ltxt) VALUES(1,'ISLAM','Islam','Islam');

IF COL_LENGTH('dbo.utnumberhdr','controllerid') IS NULL ALTER TABLE dbo.utnumberhdr ADD controllerid nvarchar(100) NULL;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.utnumberhdr WHERE id=1) INSERT dbo.utnumberhdr(id,clientid,fieldid,modelname,controllerid,counterlength,startvalue,allowpadding,paddingvalue,isdefault) VALUES(1,1,'employeecode','employee','EmployeeProfile',6,1,1,'0',1);

IF OBJECT_ID('dbo.WF_ExecutionReceipts','U') IS NULL CREATE TABLE dbo.WF_ExecutionReceipts (ServiceName nvarchar(100) NOT NULL, ExecutionId uniqueidentifier NOT NULL, RequestHash nvarchar(64) NOT NULL, ResultEnvelope nvarchar(max) NOT NULL, CompletedAtUtc datetime2 NOT NULL, CONSTRAINT PK_WF_ExecutionReceipts PRIMARY KEY (ServiceName, ExecutionId));

GO
-- Supporting tables used by raw SQL attachment operations rather than EF entities.
IF OBJECT_ID('dbo.SMAttachments','U') IS NULL CREATE TABLE dbo.SMAttachments(id int NOT NULL PRIMARY KEY,attchmentno nvarchar(50) NULL,refdocid int NULL,doctypeid int NULL,docname nvarchar(500) NULL,browsepath nvarchar(2000) NULL,uploadpath nvarchar(2000) NULL,lovreftypeid int NULL,externallink nvarchar(2000) NULL,typeid int NULL,entityid int NULL,clientid int NULL,utdoctype int NULL,utsubdoctype int NULL,utsysgen bit NOT NULL DEFAULT(0),utispartial bit NOT NULL DEFAULT(0),utblock bit NOT NULL DEFAULT(0),utrecordversion int NOT NULL DEFAULT(1),utaddtime datetime NULL,utadduser varchar(25) NULL,utaddsystem varchar(25) NULL,utaddip varchar(25) NULL,utaddappversion varchar(10) NULL,utadddbversion varchar(10) NULL,utaddid bigint NULL,utaddsessionid int NULL,utedittime datetime NULL,utedituser varchar(25) NULL,uteditsystem varchar(25) NULL,uteditip varchar(25) NULL,uteditappversion varchar(10) NULL,uteditdbversion varchar(10) NULL,uteditid bigint NULL,uteditsessionid int NULL);
IF OBJECT_ID('dbo.EREmpDocument','U') IS NULL CREATE TABLE dbo.EREmpDocument(Id int NOT NULL PRIMARY KEY,documenttypeid int NULL,employeeid int NULL,utdoctype int NULL,utsubdoctype int NULL,utsysgen bit NOT NULL DEFAULT(0),utispartial bit NOT NULL DEFAULT(0),utblock bit NOT NULL DEFAULT(0),utrecordversion int NOT NULL DEFAULT(1),utaddtime datetime NULL,utadduser varchar(25) NULL,utaddsystem varchar(25) NULL,utaddip varchar(25) NULL,utaddappversion varchar(10) NULL,utadddbversion varchar(10) NULL,utaddid bigint NULL,utaddsessionid int NULL,utedittime datetime NULL,utedituser varchar(25) NULL,uteditsystem varchar(25) NULL,uteditip varchar(25) NULL,uteditappversion varchar(10) NULL,uteditdbversion varchar(10) NULL,uteditid bigint NULL,uteditsessionid int NULL);
IF OBJECT_ID('dbo.HREmployeeDocumentFile','U') IS NULL CREATE TABLE dbo.HREmployeeDocumentFile(attachmentid int NOT NULL,clientid int NOT NULL,filename nvarchar(260) NOT NULL,contenttype nvarchar(150) NOT NULL,filedata varbinary(max) NOT NULL,createdat datetime2 NOT NULL,updatedat datetime2 NOT NULL,PRIMARY KEY(attachmentid,clientid));
IF COL_LENGTH('dbo.EREmploymenttype','utblock') IS NULL ALTER TABLE dbo.EREmploymenttype ADD utblock bit NOT NULL DEFAULT(0);
IF COL_LENGTH('dbo.EREmployeeinfohdr','teachingclassids') IS NULL ALTER TABLE dbo.EREmployeeinfohdr ADD teachingclassids nvarchar(max) NULL;
IF COL_LENGTH('dbo.SMCourse','code') IS NULL ALTER TABLE dbo.SMCourse ADD code nvarchar(20) NULL;
IF COL_LENGTH('dbo.SMCourse','entityid') IS NULL ALTER TABLE dbo.SMCourse ADD entityid int NULL;
IF OBJECT_ID('dbo.SMSubject','U') IS NULL CREATE TABLE dbo.SMSubject(Id int NOT NULL PRIMARY KEY,code nvarchar(20) NULL,stxt nvarchar(150) NULL,clientid int NULL,entityid int NULL,utblock bit NOT NULL DEFAULT(0));

IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=255) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(255,'DAY','Days','Days','LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=256) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(256,'HOUR','Hours','Hours','LVUNT',0);
