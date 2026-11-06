-- Explicitly labelled synthetic data. Machine codes are NULL, so demo employees never receive real ZKT punches.
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDepartment WHERE depcode='DEMO-OPS') INSERT dbo.OrgDepartment(Id,depcode,depstxt,clientid,opunitid) SELECT ISNULL(MAX(Id),0)+1,'DEMO-OPS','Operations (Demo)',1,1 FROM dbo.OrgDepartment;
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDesignation WHERE desgcode='DEMOO') INSERT dbo.OrgDesignation(Id,desgcode,stxt,clientid) SELECT ISNULL(MAX(Id),0)+1,'DEMOO','Officer (Demo)',1 FROM dbo.OrgDesignation;
IF NOT EXISTS(SELECT 1 FROM dbo.ERPositiontype WHERE code='DEMO-FT') INSERT dbo.ERPositiontype(id,code,stxt,emptypeid,entityid,clientid) SELECT ISNULL(MAX(id),0)+1,'DEMO-FT','Full Time (Demo)',2010,1,1 FROM dbo.ERPositiontype;
IF NOT EXISTS(SELECT 1 FROM dbo.HRGrade WHERE code='DEMO-G1') INSERT dbo.HRGrade(id,clientid,entityid,code,minsalary,maxsalary,utblock,utrecordversion) SELECT ISNULL(MAX(id),0)+1,1,1,'DEMO-G1',40000,80000,0,1 FROM dbo.HRGrade;
IF NOT EXISTS(SELECT 1 FROM dbo.ETShifthdr WHERE code='DEMO-DAY') INSERT dbo.ETShifthdr(Id,code,stxt,entityid,clientid,wosaturday,wosunday,utblock) SELECT ISNULL(MAX(Id),0)+1,'DEMO-DAY','Day Shift (Demo)',1,1,1,1,0 FROM dbo.ETShifthdr;
DECLARE @shift int=(SELECT TOP 1 Id FROM dbo.ETShifthdr WHERE code='DEMO-DAY');
DECLARE @day int=1;
WHILE @day<=5
BEGIN
IF NOT EXISTS(SELECT 1 FROM dbo.ETShiftdtl WHERE shiftid=@shift AND dayid=@day)
INSERT dbo.ETShiftdtl(Id,shiftid,lineindex,stxt,dayid,intime,outtime,duration,startdate,utblock)
SELECT ISNULL(MAX(Id),0)+1,@shift,@day,'Demo working day',@day,'2000-01-01T09:00:00','2000-01-01T17:00:00',8,'2020-01-01',0 FROM dbo.ETShiftdtl;
SET @day=@day+1;
END;
DECLARE @i int=1,@emp int,@atn int;
WHILE @i<=3
BEGIN
IF NOT EXISTS(SELECT 1 FROM dbo.EREmployeeinfohdr WHERE employeecode=CONCAT('DEMO-',@i))
INSERT dbo.EREmployeeinfohdr(Id,clientid,entityid,employeecode,firstname,lastname,depid,designationid,empstatusid,lvemployeecateg,employeetypeid,lovpositiontypeid,shiftid,joiningdate,dateofbirth,genderid,birthcountryid,birthcityid,nationalityid,machinecode)
SELECT ISNULL(MAX(Id),0)+1,1,1,CONCAT('DEMO-',@i),CASE @i WHEN 1 THEN 'Ayesha' WHEN 2 THEN 'Ali' ELSE 'Sara' END,'Demo',
(SELECT TOP 1 Id FROM dbo.OrgDepartment WHERE depcode='DEMO-OPS'),(SELECT TOP 1 Id FROM dbo.OrgDesignation WHERE desgcode='DEMOO'),125,267,2010,(SELECT TOP 1 Id FROM dbo.ERPositiontype WHERE code='DEMO-FT'),@shift,DATEADD(month,-6,CONVERT(date,GETDATE())),'1995-01-01',CASE WHEN @i=2 THEN 337 ELSE 338 END,1,1,1,NULL FROM dbo.EREmployeeinfohdr;
SELECT @emp=Id FROM dbo.EREmployeeinfohdr WHERE employeecode=CONCAT('DEMO-',@i);
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnintout WHERE empid=@emp AND remarks='Sample attendance (Demo)')
BEGIN
SELECT @atn=ISNULL(MAX(id),0)+1 FROM dbo.HRAtnintout;
INSERT dbo.HRAtnintout(id,empid,atnactivityid,atntypeid,dttime,orgdttime,clientid,entityid,shiftid,remarks,utblock,utsysgen,utispartial,utrecordversion)
VALUES(@atn,@emp,1,1,DATEADD(hour,9,CAST(CONVERT(date,GETDATE()) AS datetime2)),DATEADD(hour,9,CAST(CONVERT(date,GETDATE()) AS datetime2)),1,1,@shift,'Sample attendance (Demo)',0,0,0,1),
(@atn+1,@emp,2,1,DATEADD(hour,17,CAST(CONVERT(date,GETDATE()) AS datetime2)),DATEADD(hour,17,CAST(CONVERT(date,GETDATE()) AS datetime2)),1,1,@shift,'Sample attendance (Demo)',0,0,0,1);
END;
SET @i=@i+1;
END;
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnactivity WHERE Id=1) INSERT dbo.HRAtnactivity(Id,stxt,value,orderid,clientid,utblock) VALUES(1,'Time In',1,1,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnactivity WHERE Id=2) INSERT dbo.HRAtnactivity(Id,stxt,value,orderid,clientid,utblock) VALUES(2,'Time Out',2,2,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.ETLeavetype WHERE code='DEMO-CL') INSERT dbo.ETLeavetype(id,code,stxt,entityid,clientid,leaves,lvunitid,availunitid,allowinprob,quotavalidate,utblock) SELECT ISNULL(MAX(id),0)+1,'DEMO-CL','Casual Leave (Demo)',1,1,10,255,255,1,1,0 FROM dbo.ETLeavetype;
IF NOT EXISTS(SELECT 1 FROM dbo.ETLeaveRequest WHERE requestno='DEMO-LV-001')
INSERT dbo.ETLeaveRequest(Id,requestno,requestdate,employeeid,leavetypeid,leaveunitid,leavedays,fromdate,tilldate,reason,statusid,entityid,clientid)
SELECT ISNULL(MAX(Id),0)+1,'DEMO-LV-001',GETDATE(),(SELECT TOP 1 Id FROM dbo.EREmployeeinfohdr WHERE employeecode='DEMO-1'),(SELECT TOP 1 id FROM dbo.ETLeavetype WHERE code='DEMO-CL'),255,1,DATEADD(day,7,CONVERT(date,GETDATE())),DATEADD(day,7,CONVERT(date,GETDATE())),'Sample leave request (Demo)',2,1,1 FROM dbo.ETLeaveRequest;

UPDATE dbo.ETLeaveRequest SET statusid=2,leaveunitid=255 WHERE requestno='DEMO-LV-001' AND statusid=2070;
UPDATE dbo.ETLeavetype SET lvunitid=255,availunitid=255 WHERE code='DEMO-CL' AND lvunitid=2060;
