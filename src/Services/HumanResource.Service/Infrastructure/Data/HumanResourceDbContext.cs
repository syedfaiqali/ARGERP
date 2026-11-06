using HumanResource.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Infrastructure.Data
{
    public sealed partial class HumanResourceDbContext : DbContext
    {
        public DbSet<ZkEmployeeLink> ZkEmployeeLinks => Set<ZkEmployeeLink>();
        public DbSet<ZkDeviceLink> ZkDeviceLinks => Set<ZkDeviceLink>();
        public DbSet<ZkImportReceipt> ZkImportReceipts => Set<ZkImportReceipt>();
        public DbSet<ERQualificationLegacy> Qualifications => Set<ERQualificationLegacy>();
        public DbSet<ERQualificationtypeLegacy> QualificationTypes => Set<ERQualificationtypeLegacy>();
        public DbSet<ERSkillLegacy> Skills => Set<ERSkillLegacy>();
        public DbSet<ERSkilltypeLegacy> SkillTypes => Set<ERSkilltypeLegacy>();
        public DbSet<SMDevicetypeLegacy> DeviceTypes => Set<SMDevicetypeLegacy>();
        public DbSet<ERPositiontypeLegacy> PositionTypes => Set<ERPositiontypeLegacy>();
        public DbSet<EREmploymenttypeLegacy> EmploymentTypes => Set<EREmploymenttypeLegacy>();
        public DbSet<ERSkillratinghdrLegacy> SkillRatings => Set<ERSkillratinghdrLegacy>();
        public DbSet<ERSkillratingdtlLegacy> SkillRatingDetails => Set<ERSkillratingdtlLegacy>();
        public DbSet<HRDocTypeLegacy> DocumentTypes => Set<HRDocTypeLegacy>();
        public DbSet<EREmpAuthorityLegacy> EmployeeAuthorities => Set<EREmpAuthorityLegacy>();
        public DbSet<ETHolidaytypeLegacy> HolidayTypes => Set<ETHolidaytypeLegacy>();
        public DbSet<ETCalholidayhdrLegacy> CalendarHolidays => Set<ETCalholidayhdrLegacy>();
        public DbSet<ETCalholidaydtlLegacy> CalendarHolidayDetails => Set<ETCalholidaydtlLegacy>();
        public DbSet<ETCalholidayapplyondtlLegacy> CalendarHolidayApplyOnDetails => Set<ETCalholidayapplyondtlLegacy>();
        public DbSet<ETCalholidayshareddtlLegacy> CalendarHolidaySharedDetails => Set<ETCalholidayshareddtlLegacy>();
        public DbSet<UtLovLegacy> Lovs => Set<UtLovLegacy>();
        public DbSet<ETPayperiodhdrLegacy> PayrollPeriods => Set<ETPayperiodhdrLegacy>();
        public DbSet<ETPayperioddtlLegacy> PayrollPeriodDetails => Set<ETPayperioddtlLegacy>();
        public DbSet<ETPayrollregisterLegacy> PayrollRegisterLegacy => Set<ETPayrollregisterLegacy>();
        public DbSet<EPEarningtypeLegacy> EarningTypes => Set<EPEarningtypeLegacy>();
        public DbSet<EPDeductiontypeLegacy> DeductionTypes => Set<EPDeductiontypeLegacy>();
        public DbSet<EPLoantypeLegacy> LoanTypes => Set<EPLoantypeLegacy>();
        public DbSet<EPLoanrequestLegacy> LoanRequests => Set<EPLoanrequestLegacy>();
        public DbSet<EPDeductiongrphdrLegacy> DeductionGroups => Set<EPDeductiongrphdrLegacy>();
        public DbSet<EPDeductiongrpdtlLegacy> DeductionGroupDetails => Set<EPDeductiongrpdtlLegacy>();
        public DbSet<EPEarninggrphdrLegacy> EarningGroups => Set<EPEarninggrphdrLegacy>();
        public DbSet<EPEarninggrpdtlLegacy> EarningGroupDetails => Set<EPEarninggrpdtlLegacy>();
        public DbSet<ETPolicyhdrLegacy> AttendancePolicies => Set<ETPolicyhdrLegacy>();
        public DbSet<ETPolicydtlLegacy> AttendancePolicyDetails => Set<ETPolicydtlLegacy>();
        public DbSet<ETAbsentpolicydtlLegacy> AttendancePolicyAbsentDetails => Set<ETAbsentpolicydtlLegacy>();
        public DbSet<EmployeeProfileLegacy> EmployeeProfiles => Set<EmployeeProfileLegacy>();
        public DbSet<MeetingHostInfoLegacy> MeetingHostInfos => Set<MeetingHostInfoLegacy>();
        public DbSet<EmployeeGradeLegacy> EmployeeGrades => Set<EmployeeGradeLegacy>();
        public DbSet<EmployeeAddressLegacy> EmployeeAddresses => Set<EmployeeAddressLegacy>();
        public DbSet<EmployeeRosterLegacy> EmployeeRosters => Set<EmployeeRosterLegacy>();
        public DbSet<EmployeeEducationLegacy> EmployeeEducations => Set<EmployeeEducationLegacy>();
        public DbSet<EmployeeContactInformationLegacy> EmployeeContacts => Set<EmployeeContactInformationLegacy>();
        public DbSet<EmployeeSkillLegacy> EmployeeSkills => Set<EmployeeSkillLegacy>();
        public DbSet<EmployeeSubjectRatingLegacy> EmployeeSubjectRatings => Set<EmployeeSubjectRatingLegacy>();
        public DbSet<EmployeeExperienceLegacy> EmployeeExperiences => Set<EmployeeExperienceLegacy>();
        public DbSet<EmployeeInformationRecord> EmployeeInformationRecords => Set<EmployeeInformationRecord>();
        public DbSet<EmployeeCardLegacy> EmployeeCards => Set<EmployeeCardLegacy>();
        public DbSet<EmployeeDiseaseLegacy> EmployeeDiseases => Set<EmployeeDiseaseLegacy>();
        public DbSet<SMDiseaseHrLegacy> DiseaseMasters => Set<SMDiseaseHrLegacy>();
        public DbSet<SMSubjecttypeHrLegacy> SubjectTypes => Set<SMSubjecttypeHrLegacy>();
        public DbSet<SMCourseHrLegacy> Classes => Set<SMCourseHrLegacy>();
        public DbSet<RelationTypeLegacy> RelationTypes => Set<RelationTypeLegacy>();
        public DbSet<EmployeeEmploymentLegacy> EmployeeEmployments => Set<EmployeeEmploymentLegacy>();
        public DbSet<EREmpPositionLegacy> EmployeePositions => Set<EREmpPositionLegacy>();
        public DbSet<EmployeePayrollLegacy> EmployeePayrolls => Set<EmployeePayrollLegacy>();
        public DbSet<EmployeePayrollBaseSalaryLegacy> EmployeePayrollBaseSalaries => Set<EmployeePayrollBaseSalaryLegacy>();
        public DbSet<EmployeePayrollAllowanceLegacy> EmployeePayrollAllowances => Set<EmployeePayrollAllowanceLegacy>();
        public DbSet<EmployeePayrollDeductionLegacy> EmployeePayrollDeductions => Set<EmployeePayrollDeductionLegacy>();
        public DbSet<EmployeePayrollLoanTypeLegacy> EmployeePayrollLoanTypes => Set<EmployeePayrollLoanTypeLegacy>();
        public DbSet<DesignationPayrollLegacy> DesignationPayrolls => Set<DesignationPayrollLegacy>();
        public DbSet<DesignationPayrollBaseSalaryLegacy> DesignationPayrollBaseSalaries => Set<DesignationPayrollBaseSalaryLegacy>();
        public DbSet<DesignationPayrollAllowanceLegacy> DesignationPayrollAllowances => Set<DesignationPayrollAllowanceLegacy>();
        public DbSet<DesignationPayrollDeductionLegacy> DesignationPayrollDeductions => Set<DesignationPayrollDeductionLegacy>();
        public DbSet<DesignationPayrollLoanTypeLegacy> DesignationPayrollLoanTypes => Set<DesignationPayrollLoanTypeLegacy>();
        public DbSet<EmployeeBankAccountLegacy> EmployeeBankAccounts => Set<EmployeeBankAccountLegacy>();
        public DbSet<EmployeeSubCoordinateLegacy> EmployeeSubCoordinates => Set<EmployeeSubCoordinateLegacy>();
        public DbSet<BankGroupLegacy> BankGroups => Set<BankGroupLegacy>();
        public DbSet<IncomeTaxLegacy> IncomeTaxes => Set<IncomeTaxLegacy>();
        public DbSet<ShiftLegacy> Shifts => Set<ShiftLegacy>();
        public DbSet<SmShiftLegacy> SmShifts => Set<SmShiftLegacy>();
        public DbSet<ScheduleHeaderLegacy> Schedules => Set<ScheduleHeaderLegacy>();
        public DbSet<ScheduleDetailLegacy> ScheduleDetails => Set<ScheduleDetailLegacy>();
        public DbSet<SMCustomShiftHeaderLegacy> CustomShiftHeaders => Set<SMCustomShiftHeaderLegacy>();
        public DbSet<SMCustomShiftDetailLegacy> CustomShiftDetails => Set<SMCustomShiftDetailLegacy>();
        public DbSet<LeaveGroupLegacy> LeaveGroups => Set<LeaveGroupLegacy>();
        public DbSet<LeaveGroupDetailLegacy> LeaveGroupDetails => Set<LeaveGroupDetailLegacy>();
        public DbSet<ETShiftdtlLegacy> ShiftDetails => Set<ETShiftdtlLegacy>();
        public DbSet<ETShiftsubdtlLegacy> ShiftSubDetails => Set<ETShiftsubdtlLegacy>();
        public DbSet<ETShiftshareddtlLegacy> ShiftSharedDetails => Set<ETShiftshareddtlLegacy>();
        public DbSet<ETLeaveRequestLegacy> LeaveRequests => Set<ETLeaveRequestLegacy>();
        public DbSet<HRAtnintoutLegacy> EmployeeAttendances => Set<HRAtnintoutLegacy>();
        public DbSet<HRAtnactivityLegacy> AttendanceActivities => Set<HRAtnactivityLegacy>();
        public DbSet<EmployeeProfileLegacy> Employees => Set<EmployeeProfileLegacy>();
        public DbSet<OrgDepartmentLegacy> Departments => Set<OrgDepartmentLegacy>();
        public DbSet<OrgDesignationLegacy> Designations => Set<OrgDesignationLegacy>();
        public DbSet<OrgLegalentityLegacy> OrgLegalentities => Set<OrgLegalentityLegacy>();
        public DbSet<SMPolicyLegacy> SystemPolicies => Set<SMPolicyLegacy>();
        public DbSet<SMAcadPolicyVacancyLegacy> AcademicPolicies => Set<SMAcadPolicyVacancyLegacy>();
        public DbSet<HRDailyAttendanceLegacy> DailyAttendances => Set<HRDailyAttendanceLegacy>();
        public DbSet<HRMonthlyAttendanceLegacy> MonthlyAttendances => Set<HRMonthlyAttendanceLegacy>();
        public DbSet<ETLeaveadjpolicyLegacy> LeaveAdjustmentPolicies => Set<ETLeaveadjpolicyLegacy>();
        public DbSet<ETLeaveadjpolicydtlLegacy> LeaveAdjustmentPolicyDetails => Set<ETLeaveadjpolicydtlLegacy>();
        public DbSet<ETLeavetypeLegacy> LeaveTypes => Set<ETLeavetypeLegacy>();
        public DbSet<ETLeavetranLegacy> LeaveTransactions => Set<ETLeavetranLegacy>();
        public DbSet<UtinstanceLegacy> UtInstances { get; set; }
        public DbSet<EPLoantranshdrLegacy> LoanIssuances { get; set; }
        public DbSet<EPLoantransdtlLegacy> LoanIssuanceDetails { get; set; }
        public DbSet<HRpayrollprocessLegacy> PayrollProcesses { get; set; }
        public DbSet<HRpayrollprocessdtlLegacy> PayrollProcessDetails { get; set; }
        public DbSet<SMMachineRegistrationLegacy> MachineRegistrations => Set<SMMachineRegistrationLegacy>();
        public DbSet<OrgsiteHrLegacy> Sites => Set<OrgsiteHrLegacy>();
        public DbSet<HRVacancy> Vacancies => Set<HRVacancy>();
        public DbSet<HRVacancyRecruiter> VacancyRecruiters => Set<HRVacancyRecruiter>();
        public DbSet<HRVacancyQualification> VacancyQualifications => Set<HRVacancyQualification>();
        public DbSet<HRVacancySkill> VacancySkills => Set<HRVacancySkill>();
        public DbSet<HRVacancyRevision> VacancyRevisions => Set<HRVacancyRevision>();
        public DbSet<HRVacancyPosition> VacancyPositions => Set<HRVacancyPosition>();
        public DbSet<HRVacancyPositionAssignment> VacancyPositionAssignments => Set<HRVacancyPositionAssignment>();
        public DbSet<HRVacancyNote> VacancyNotes => Set<HRVacancyNote>();
        public DbSet<HRVacancyAudit> VacancyAudits => Set<HRVacancyAudit>();
        public DbSet<HRPosition> Positions => Set<HRPosition>();
        public DbSet<HRPositionAssignment> PositionAssignments => Set<HRPositionAssignment>();
        public DbSet<HRPositionAudit> PositionAudits => Set<HRPositionAudit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ZkEmployeeLink>(entity =>
            {
                entity.ToTable("ARG_ZkEmployeeLinks"); entity.HasKey(x => x.EmployeeId);
                entity.Property(x => x.EmployeeId).ValueGeneratedNever();
                entity.Property(x => x.SourceKey).HasMaxLength(64);
                entity.Property(x => x.MachineCode).HasMaxLength(50);
                entity.HasIndex(x => new { x.SourceKey, x.SourceClientId, x.MachineCode });
            });
            modelBuilder.Entity<ZkDeviceLink>(entity =>
            {
                entity.ToTable("ARG_ZkDeviceLinks"); entity.HasKey(x => x.MachineId);
                entity.Property(x => x.MachineId).ValueGeneratedNever();
                entity.Property(x => x.SourceKey).HasMaxLength(64);
            });
            modelBuilder.Entity<ZkImportReceipt>(entity =>
            {
                entity.ToTable("ARG_ZkImportReceipts");
                entity.HasKey(x => new { x.SourceKey, x.TransactionId });
                entity.Property(x => x.SourceKey).HasMaxLength(64);
            });

            modelBuilder.Entity<HRVacancy>(entity =>
            {
                entity.HasIndex(x => new { x.ClientId, x.VacancyNumber }).IsUnique();
                entity.HasIndex(x => new { x.ClientId, x.EntityId, x.ApprovalStatus, x.LifecycleStatus });
                entity.HasOne(x => x.Entity).WithMany().HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.HiringManager).WithMany().HasForeignKey(x => x.HiringManagerEmployeeId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApproverEmployeeId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne<HRPosition>().WithMany().HasForeignKey(x => x.ReportsToPositionId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne<HRPosition>().WithMany().HasForeignKey(x => x.ReplacementPositionId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRVacancyRecruiter>(entity =>
            {
                entity.HasIndex(x => new { x.VacancyId, x.EmployeeId, x.IsActive });
                entity.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRVacancyQualification>()
                .HasIndex(x => new { x.VacancyId, x.QualificationId }).IsUnique();
            modelBuilder.Entity<HRVacancySkill>()
                .HasIndex(x => new { x.VacancyId, x.SkillId }).IsUnique();
            modelBuilder.Entity<HRVacancyRevision>()
                .HasIndex(x => new { x.VacancyId, x.RevisionNumber }).IsUnique();
            modelBuilder.Entity<HRVacancyPosition>(entity =>
            {
                // A canonical position number can reappear in later replacement vacancy cycles.
                entity.HasIndex(x => x.PositionNumber);
                entity.HasIndex(x => new { x.VacancyId, x.Status });
                entity.HasOne(x => x.CurrentEmployee).WithMany().HasForeignKey(x => x.CurrentEmployeeId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRVacancyPositionAssignment>(entity =>
            {
                // Unlike the other vacancy tables, assignment session columns are NOT NULL.
                entity.Property(x => x.UtAddSessionId).IsRequired();
                entity.Property(x => x.UtEditSessionId).IsRequired();
                entity.HasIndex(x => new { x.PositionId, x.Status, x.EffectiveDate });
                entity.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRVacancyAudit>(entity =>
            {
                entity.HasIndex(x => new { x.VacancyId, x.OccurredAtUtc });
                entity.HasOne(x => x.ActorEmployee).WithMany().HasForeignKey(x => x.ActorEmployeeId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRPosition>(entity =>
            {
                entity.HasIndex(x => new { x.ClientId, x.PositionNumber }).IsUnique();
                entity.HasIndex(x => new { x.ClientId, x.EntityId, x.LovStatusId });
                entity.HasOne(x => x.ReportsToPosition).WithMany().HasForeignKey(x => x.ReportsToPositionId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Entity).WithMany().HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Designation).WithMany().HasForeignKey(x => x.DesignationId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.StatusLov).WithMany().HasForeignKey(x => x.LovStatusId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRPositionAssignment>(entity =>
            {
                entity.HasIndex(x => new { x.PositionId, x.LovStatusId, x.EffectiveTo });
                entity.HasIndex(x => new { x.EmployeeId, x.LovStatusId, x.EffectiveTo });
                entity.HasOne(x => x.Position).WithMany().HasForeignKey(x => x.PositionId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.StatusLov).WithMany().HasForeignKey(x => x.LovStatusId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HRPositionAudit>(entity =>
            {
                entity.HasIndex(x => new { x.PositionId, x.OccurredAtUtc });
                entity.HasOne(x => x.Position).WithMany().HasForeignKey(x => x.PositionId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.ActorEmployee).WithMany().HasForeignKey(x => x.ActorEmployeeId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ERQualificationtypeLegacy>(entity =>
            {
                entity.ToTable("ERQualificationtype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.LovtypeId).HasColumnName("lovtypeid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });


            modelBuilder.Entity<ERQualificationLegacy>(entity =>
            {
                entity.ToTable("ERQualification");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.quatypeid).HasColumnName("quatypeid");
                entity.Property(x => x.stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.entityid).HasColumnName("entityid");
                entity.Property(x => x.clientid).HasColumnName("clientid");
                entity.Property(x => x.lovtypeid).HasColumnName("lovtypeid");
                entity.Property(x => x.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ERSkilltypeLegacy>(entity =>
            {
                entity.ToTable("ERSkilltype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ERSkillLegacy>(entity =>
            {
                entity.ToTable("ERSkill");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.SkillTypeId).HasColumnName("skilltypeid");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.SkillRatingId).HasColumnName("skillratingid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMDevicetypeLegacy>(entity =>
            {
                entity.ToTable("SMDevicetype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMMachineRegistrationLegacy>(entity =>
            {
                entity.ToTable("SMMachineRegistration");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.code).HasColumnName("code").HasMaxLength(50);
                entity.Property(x => x.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(x => x.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.lovmachinetypeid).HasColumnName("lovmachinetypeid");
                entity.Property(x => x.machineip).HasColumnName("machineip").HasMaxLength(20);
                entity.Property(x => x.port).HasColumnName("port");
                entity.Property(x => x.inactive).HasColumnName("inactive");
                entity.Property(x => x.entityid).HasColumnName("entityid");
                entity.Property(x => x.clientid).HasColumnName("clientid");
                entity.Property(x => x.utsysgen).HasColumnName("utsysgen");
                entity.Property(x => x.utblock).HasColumnName("utblock");
                entity.Property(x => x.utispartial).HasColumnName("utispartial");
                entity.Property(x => x.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(x => x.siteid).HasColumnName("siteid");
                entity.Property(x => x.devicetypeid).HasColumnName("devicetypeid");
                entity.Property(x => x.convtytypeid).HasColumnName("convtytypeid");
                entity.Property(x => x.deviceid).HasColumnName("deviceid");
            });

            modelBuilder.Entity<OrgsiteHrLegacy>(entity =>
            {
                entity.ToTable("Orgsite");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.stxt).HasColumnName("stxt").HasMaxLength(500);
                entity.Property(x => x.entityid).HasColumnName("entityid");
                entity.Property(x => x.clientid).HasColumnName("clientid");
                entity.Property(x => x.utblock).HasColumnName("utblock");
            });


            modelBuilder.Entity<ERPositiontypeLegacy>(entity =>
            {
                entity.ToTable("ERPositiontype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.EmpTypeId).HasColumnName("emptypeid");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EREmploymenttypeLegacy>(entity =>
            {
                entity.ToTable("EREmploymenttype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
            });

            modelBuilder.Entity<ERSkillratinghdrLegacy>(entity =>
            {
                entity.ToTable("ERSkillratinghdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasMany(h => h.Details)
                      .WithOne(d => d.Header)
                      .HasForeignKey(d => d.RatingId);
            });

            modelBuilder.Entity<ERSkillratingdtlLegacy>(entity =>
            {
                entity.ToTable("ERSkillratingdtl");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.RatingId).HasColumnName("ratingid");
                entity.Property(x => x.LineIndex).HasColumnName("lineindex");
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(200);
                entity.Property(x => x.Key).HasColumnName("key").HasMaxLength(100);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(255);
                entity.Property(x => x.Factor).HasColumnName("factor");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });
            modelBuilder.Entity<HRDocTypeLegacy>(entity =>
            {
                entity.ToTable("HRDocType");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });
            modelBuilder.Entity<EREmpAuthorityLegacy>(entity =>
            {
                entity.ToTable("EREmpAuthority");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.AuthTypeId).HasColumnName("authtypeid");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(x => x.ContactNo).HasColumnName("contactno").HasMaxLength(100);
                entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(100);
                entity.Property(x => x.Web).HasColumnName("web").HasMaxLength(255);
                entity.Property(x => x.Fax).HasColumnName("fax").HasMaxLength(100);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETHolidaytypeLegacy>(entity =>
            {
                entity.ToTable("ETHolidaytype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.Colorcode).HasColumnName("colorcode").HasMaxLength(50);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETCalholidayhdrLegacy>(entity =>
            {
                entity.ToTable("ETCalholidayhdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(e => e.Payperiodid).HasColumnName("payperiodid");
                entity.Property(e => e.Applyonid).HasColumnName("applyonid");
                entity.Property(e => e.Lovsharedid).HasColumnName("lovsharedid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.Utblock).HasColumnName("utblock");

                entity.HasMany(d => d.Details)
                    .WithOne(p => p.Header)
                    .HasForeignKey(d => d.Calholidayid);

                entity.HasMany(d => d.ApplyOnDetails)
                    .WithOne(p => p.Header)
                    .HasForeignKey(d => d.Calholidayid);

                entity.HasMany(d => d.SharedDetails)
                    .WithOne(p => p.Header)
                    .HasForeignKey(d => d.Calholidayid);
            });

            modelBuilder.Entity<ETCalholidaydtlLegacy>(entity =>
            {
                entity.ToTable("ETCalholidaydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Calholidayid).HasColumnName("calholidayid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.Holidaytypeid).HasColumnName("holidaytypeid");
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(250);
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Enddate).HasColumnName("enddate");
                entity.Property(e => e.Days).HasColumnName("days");
                entity.Property(e => e.Payperioddtlid).HasColumnName("payperioddtlid");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETCalholidayapplyondtlLegacy>(entity =>
            {
                entity.ToTable("ETCalholidayapplyondtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Calholidayid).HasColumnName("calholidayid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.Departid).HasColumnName("departid");
                entity.Property(e => e.Emptypeid).HasColumnName("emptypeid");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETCalholidayshareddtlLegacy>(entity =>
            {
                entity.ToTable("ETCalholidayshareddtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Calholidayid).HasColumnName("calholidayid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.Lovsharedid).HasColumnName("lovsharedid");
                entity.Property(e => e.Pclientid).HasColumnName("pclientid");
                entity.Property(e => e.Pentityid).HasColumnName("pentityid");
                entity.Property(e => e.Regionid).HasColumnName("regionid");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<MeetingHostInfoLegacy>(entity =>
            {
                entity.ToTable("SMmeetinghostinfo");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.LovTypeId).HasColumnName("lovtypeid");
                // The OAuth values reuse the retired JWT columns; see the entity.
                entity.Property(x => x.ZoomHostEmail).HasColumnName("username").HasMaxLength(500);
                entity.Property(x => x.ZoomPassword).HasColumnName("pwd").HasMaxLength(500);
                entity.Property(x => x.ZoomMeetingId).HasColumnName("meetingid").HasMaxLength(50);
                entity.Property(x => x.ZoomMeetingPassword).HasColumnName("meetingpwd").HasMaxLength(50);
                entity.Property(x => x.ZoomInvitePassword).HasColumnName("invitepwd");
                entity.Property(x => x.ZoomToken).HasColumnName("jwttoken");
                entity.Property(x => x.ZoomAccountId).HasColumnName("apiKey").HasMaxLength(500);
                entity.Property(x => x.ZoomClientSecret).HasColumnName("apiSecret").HasMaxLength(500);
                entity.Property(x => x.ZoomSdkKey).HasColumnName("sdkkey");
                entity.Property(x => x.ZoomSdkSecret).HasColumnName("sdksecret");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<EmployeeProfileLegacy>(entity =>
            {
                entity.ToTable("EREmployeeinfohdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.PreviousEntityId).HasColumnName("preventityid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.EmployeeCode).HasColumnName("employeecode").HasMaxLength(50);
                entity.Property(x => x.EmployeeCategoryId).HasColumnName("lvemployeecateg");
                entity.Property(x => x.EmployeeTypeId).HasColumnName("employeetypeid");
                entity.Property(x => x.FirstName).HasColumnName("firstname").HasMaxLength(150);
                entity.Property(x => x.LastName).HasColumnName("lastname").HasMaxLength(150);
                entity.Property(x => x.ParentName).HasColumnName("parentname").HasMaxLength(150);
                entity.Property(x => x.CellNo).HasColumnName("cellno").HasMaxLength(20);
                entity.Property(x => x.DepartmentId).HasColumnName("depid");
                entity.Property(x => x.DesignationId).HasColumnName("designationid");
                entity.Property(x => x.ReportingToEmployeeId).HasColumnName("reportingtoemployeeid");
                entity.Property(x => x.SubstituteEmployeeId).HasColumnName("substituteemployeeid");
                entity.Property(x => x.StatusId).HasColumnName("empstatusid");
                // Grade is held on EREmpPosition (Employee Contract), not the employee header.
                entity.Ignore(x => x.GradeId);
                entity.Property(x => x.DateOfBirth).HasColumnName("dateofbirth");
                entity.Property(x => x.GenderId).HasColumnName("genderid");
                entity.Property(x => x.BloodGroupId).HasColumnName("bloodgroupid");
                entity.Property(x => x.MaritalStatusId).HasColumnName("maritalid");
                entity.Property(x => x.BirthCountryId).HasColumnName("birthcountryid");
                entity.Property(x => x.BirthCityId).HasColumnName("birthcityid");
                entity.Property(x => x.NationalityId).HasColumnName("nationalityid");
                entity.Property(x => x.ReligionId).HasColumnName("religionid");
                entity.Property(x => x.JoiningDate).HasColumnName("joiningdate");
                entity.Property(x => x.ResignDate).HasColumnName("resigndate");
                entity.Property(x => x.ImageId).HasColumnName("imageid");
                entity.Property(x => x.EmailId).HasColumnName("emailid").HasMaxLength(50);
                entity.Property(x => x.PositionTypeId).HasColumnName("lovpositiontypeid");
                entity.Property(x => x.ShiftId).HasColumnName("shiftid");
                entity.Property(x => x.MachineCode).HasColumnName("machinecode").HasMaxLength(50);
                entity.Property(x => x.CourseCategoryId).HasColumnName("coursecategoryid");
                entity.Property(x => x.ZoomUsername).HasColumnName("username").HasMaxLength(500);
                entity.Property(x => x.ZoomPassword).HasColumnName("pwd").HasMaxLength(500);
                entity.Property(x => x.ZoomMeetingId).HasColumnName("meetingid").HasMaxLength(50);
                entity.Property(x => x.ZoomMeetingPassword).HasColumnName("meetingpwd").HasMaxLength(50);
                entity.Property(x => x.ZoomToken).HasColumnName("jwttoken");
                entity.Property(x => x.ZoomApiKey).HasColumnName("apiKey").HasMaxLength(500);
                entity.Property(x => x.ZoomApiSecret).HasColumnName("apiSecret").HasMaxLength(500);
                entity.Property(x => x.ZoomSdkKey).HasColumnName("sdkkey");
                entity.Property(x => x.ZoomSdkSecret).HasColumnName("sdksecret");
                entity.Property(x => x.ZoomInvitePassword).HasColumnName("invitepwd");
                // CNIC is present on the existing employee header and must be mapped
                // for save/edit round-tripping. Keep the optional legacy fields ignored
                // because they are not available in every deployment.
                entity.Property(x => x.CnicNo).HasColumnName("cnicno").HasMaxLength(50);
                entity.Property(x => x.CprNo).HasColumnName("cprno").HasMaxLength(50);
                entity.Property(x => x.CprExpiry).HasColumnName("cprexpiry").HasColumnType("date");
                entity.Property(x => x.PassportNo).HasColumnName("passportno").HasMaxLength(50);
                entity.Property(x => x.PassportExpiry).HasColumnName("passportexpiry").HasColumnType("date");
                entity.Property(x => x.VisaNo).HasColumnName("visano").HasMaxLength(50);
                entity.Property(x => x.VisaExpiry).HasColumnName("visaexpiry").HasColumnType("date");
                entity.Property(x => x.MoeId).HasColumnName("moeid");
                entity.Property(x => x.MoeExpiry).HasColumnName("moeexpiry").HasColumnType("date");
                entity.Property(x => x.HeaderAddress).HasColumnName("address");
                entity.Property(x => x.DrivingLicenseNo).HasColumnName("drivinglicenseno").HasMaxLength(50);
                entity.Property(x => x.LicenseExpiry).HasColumnName("licenseexp");
                entity.Property(x => x.EmergencyNo).HasColumnName("emergencyno").HasMaxLength(50);
                entity.Property(x => x.VehicleId).HasColumnName("vehicleid");
                entity.Property(x => x.HomeStopId).HasColumnName("homestopid");
                entity.Property(x => x.TValidFrom).HasColumnName("tvalidfrom");
                entity.Property(x => x.TValidTill).HasColumnName("tvalidtill");
                entity.Property(x => x.TActive).HasColumnName("tactive");
                entity.Ignore(x => x.EobiNo);
                entity.Ignore(x => x.SessiNo);
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Department)
                    .WithMany()
                    .HasForeignKey(x => x.DepartmentId);

                entity.HasOne(x => x.Designation)
                    .WithMany()
                    .HasForeignKey(x => x.DesignationId);
            });

            modelBuilder.Entity<EmployeeGradeLegacy>(entity =>
            {
                entity.ToTable("HRGrade");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(30);
                entity.Property(x => x.MinimumSalary).HasColumnName("minsalary").HasColumnType("decimal(18,2)");
                entity.Property(x => x.MaximumSalary).HasColumnName("maxsalary").HasColumnType("decimal(18,2)");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(100);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(100);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<EmployeeSubCoordinateLegacy>(entity =>
            {
                entity.ToTable("orgsubcordinate");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("empid");
                entity.Property(x => x.SubCampusId).HasColumnName("subcampus");
                entity.Property(x => x.FromDate).HasColumnName("fromdate");
                entity.Property(x => x.TillDate).HasColumnName("tilldate");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.Remarks).HasColumnName("remarks").HasMaxLength(500);

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.SubCampus)
                    .WithMany()
                    .HasForeignKey(x => x.SubCampusId);
            });

            modelBuilder.Entity<EmployeeAddressLegacy>(entity =>
            {
                entity.ToTable("EREmpaddress");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(50);
                entity.Property(x => x.AddressTypeId).HasColumnName("adtypeid");
                entity.Property(x => x.CountryId).HasColumnName("countryid");
                entity.Property(x => x.StateId).HasColumnName("stateid");
                entity.Property(x => x.CityId).HasColumnName("cityid");
                entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
                entity.Property(x => x.PrimaryAddress).HasColumnName("primaryadd");
                entity.Property(x => x.ContactName).HasColumnName("contactname").HasMaxLength(50);
                entity.Property(x => x.Landline).HasColumnName("landline").HasMaxLength(15);
                entity.Property(x => x.Ext).HasColumnName("ext").HasMaxLength(5);
                entity.Property(x => x.Mobile).HasColumnName("mobile").HasMaxLength(15);
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtBlock1).HasColumnName("utblock1");
            });

            modelBuilder.Entity<EmployeeRosterLegacy>(entity =>
            {
                entity.ToTable("EREmproaster");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ShiftId).HasColumnName("shiftid");
                entity.Property(x => x.LeaveGroupId).HasColumnName("leavegrpid");
                entity.Property(x => x.LateAllowed).HasColumnName("lateallowed");
                entity.Property(x => x.EarlyDeparture).HasColumnName("earlydep");
                entity.Property(x => x.OvertimeAllowed).HasColumnName("otallowed");
                entity.Property(x => x.StartDate).HasColumnName("startdate").HasColumnType("datetime");
                entity.Property(x => x.EndDate).HasColumnName("enddate").HasColumnType("datetime");
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.TaxId).HasColumnName("taxid");
                entity.Property(x => x.TaxExempt).HasColumnName("taxexempt");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.Shift)
                    .WithMany()
                    .HasForeignKey(x => x.ShiftId);

                entity.HasOne(x => x.LeaveGroup)
                    .WithMany()
                    .HasForeignKey(x => x.LeaveGroupId);

                entity.HasOne(x => x.Tax)
                    .WithMany()
                    .HasForeignKey(x => x.TaxId);
            });

            modelBuilder.Entity<EmployeeEducationLegacy>(entity =>
            {
                entity.ToTable("SMAcademic");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.StudentId).HasColumnName("studentid");
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.QualificationTypeId).HasColumnName("qualificationtypeid");
                entity.Property(x => x.QualificationId).HasColumnName("qualificationid");
                entity.Property(x => x.Institute).HasColumnName("insitute").HasMaxLength(50);
                entity.Property(x => x.Year).HasColumnName("year");
                entity.Property(x => x.Grade).HasColumnName("grade").HasMaxLength(3);
                entity.Property(x => x.Percentage).HasColumnName("percentage");
                entity.Property(x => x.Cgpa).HasColumnName("totalmarks");
                entity.Property(x => x.ObtainMarks).HasColumnName("obtainmarks");
                entity.Property(x => x.Division).HasColumnName("division").HasMaxLength(3);
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.QualificationType)
                    .WithMany()
                    .HasForeignKey(x => x.QualificationTypeId);

                entity.HasOne(x => x.Qualification)
                    .WithMany()
                    .HasForeignKey(x => x.QualificationId);
            });

            modelBuilder.Entity<EmployeeContactInformationLegacy>(entity =>
            {
                entity.ToTable("EREmpRelation");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.RelationTypeId).HasColumnName("relationtypeid");
                entity.Property(x => x.GenderId).HasColumnName("genderid");
                entity.Property(x => x.FirstName).HasColumnName("firstname").HasMaxLength(50);
                entity.Property(x => x.LastName).HasColumnName("lastname").HasMaxLength(50);
                entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
                entity.Property(x => x.PhoneNo).HasColumnName("phoneno").HasMaxLength(15);
                entity.Property(x => x.MobileNo).HasColumnName("mobileno").HasMaxLength(15);
                entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(x => x.Emergency).HasColumnName("emergency");
                entity.Property(x => x.NextOfKin).HasColumnName("nextofkin");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock1).HasColumnName("utblock1");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.RelationType)
                    .WithMany()
                    .HasForeignKey(x => x.RelationTypeId);

                entity.HasOne(x => x.Gender)
                    .WithMany()
                    .HasForeignKey(x => x.GenderId);
            });

            modelBuilder.Entity<RelationTypeLegacy>(entity =>
            {
                entity.ToTable("SMRelationtype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.RelationTypeNo).HasColumnName("relationtypeno").HasMaxLength(15);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EmployeeSkillLegacy>(entity =>
            {
                entity.ToTable("EREmpSkill");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.SkillTypeId).HasColumnName("skilltypeid");
                entity.Property(x => x.SkillId).HasColumnName("skillid");
                entity.Property(x => x.Institute).HasColumnName("institute").HasMaxLength(500);
                entity.Property(x => x.LevelId).HasColumnName("levelid");
                entity.Property(x => x.Rating).HasColumnName("rating");
                entity.Property(x => x.PeriodUnitId).HasColumnName("punitid");
                entity.Property(x => x.Period).HasColumnName("period");
                entity.Property(x => x.Year).HasColumnName("year");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock1).HasColumnName("utblock1");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.SkillRatingId).HasColumnName("skillratingid");
                entity.Property(x => x.Remarks).HasColumnName("remarks").HasMaxLength(500);
                entity.Property(x => x.SkillRatingDetailId).HasColumnName("skillratingdtlid");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.SkillType)
                    .WithMany()
                    .HasForeignKey(x => x.SkillTypeId);

                entity.HasOne(x => x.Skill)
                    .WithMany()
                    .HasForeignKey(x => x.SkillId);

                entity.HasOne(x => x.SkillRating)
                    .WithMany()
                    .HasForeignKey(x => x.SkillRatingId);

                entity.HasOne(x => x.SkillRatingDetail)
                    .WithMany()
                    .HasForeignKey(x => x.SkillRatingDetailId);

                entity.HasOne(x => x.PeriodUnit)
                    .WithMany()
                    .HasForeignKey(x => x.PeriodUnitId);
            });

            modelBuilder.Entity<SMSubjecttypeHrLegacy>(entity =>
            {
                entity.ToTable("SMSubjecttype");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMCourseHrLegacy>(entity =>
            {
                entity.ToTable("SMCourse");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EmployeeSubjectRatingLegacy>(entity =>
            {
                entity.ToTable("EREmployeesubjectskill");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.SubjectTypeId).HasColumnName("subjecttypeid");
                entity.Property(x => x.RatingDetailId).HasColumnName("ratingdtlid");
                entity.Property(x => x.RatingValue).HasColumnName("ratingvalue");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.SubjectType)
                    .WithMany()
                    .HasForeignKey(x => x.SubjectTypeId);

                entity.HasOne(x => x.RatingDetail)
                    .WithMany()
                    .HasForeignKey(x => x.RatingDetailId);
            });

            modelBuilder.Entity<EmployeeExperienceLegacy>(entity =>
            {
                entity.ToTable("EREmpExperience");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.BusinessNatureId).HasColumnName("busnatureid");
                entity.Property(x => x.Company).HasColumnName("company").HasMaxLength(150);
                entity.Property(x => x.Department).HasColumnName("department").HasMaxLength(50);
                entity.Property(x => x.Designation).HasColumnName("designation").HasMaxLength(50);
                entity.Property(x => x.StartDate).HasColumnName("startdate");
                entity.Property(x => x.EndDate).HasColumnName("enddate");
                entity.Property(x => x.Salary).HasColumnName("salary");
                entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
                entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(15);
                entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(50);
                entity.Property(x => x.Web).HasColumnName("web").HasMaxLength(50);
                entity.Property(x => x.Responsibility).HasColumnName("responsibility").HasMaxLength(1500);
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.ContactPerson).HasColumnName("contactper").HasMaxLength(150);
                entity.Property(x => x.PersonDesignation).HasColumnName("persondesignation").HasMaxLength(150);
                entity.Property(x => x.PersonAddress).HasColumnName("personaddress").HasMaxLength(150);
                entity.Property(x => x.PersonPhone).HasColumnName("personphone").HasMaxLength(20);
                entity.Property(x => x.PersonEmail).HasColumnName("personemail").HasMaxLength(100);
                entity.Property(x => x.PersonWeb).HasColumnName("personweb").HasMaxLength(100);

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.BusinessNature)
                    .WithMany()
                    .HasForeignKey(x => x.BusinessNatureId);
            });

            modelBuilder.Entity<EmployeeInformationRecord>(entity =>
            {
                entity.ToTable("HREmployeeInformationRecord");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.CategoryId).HasColumnName("categoryid");
                entity.Property(x => x.Category).HasColumnName("category").HasMaxLength(30).IsRequired();
                entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(250);
                entity.Property(x => x.ReferenceNumber).HasColumnName("referencenumber").HasMaxLength(100);
                entity.Property(x => x.RecordDate).HasColumnName("recorddate");
                entity.Property(x => x.EndDate).HasColumnName("enddate");
                entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(x => x.Authority).HasColumnName("authority").HasMaxLength(200);
                entity.Property(x => x.SubCategory).HasColumnName("subcategory").HasMaxLength(100);
                entity.Property(x => x.Mode).HasColumnName("mode").HasMaxLength(50);
                entity.Property(x => x.Details).HasColumnName("details").HasMaxLength(2000);
                entity.Property(x => x.Outcome).HasColumnName("outcome").HasMaxLength(2000);
                entity.Property(x => x.Remarks).HasColumnName("remarks").HasMaxLength(2000);
                entity.Property(x => x.FileName).HasColumnName("filename").HasMaxLength(260);
                entity.Property(x => x.FileContentType).HasColumnName("filecontenttype").HasMaxLength(150);
                entity.Property(x => x.FileData).HasColumnName("filedata");
                entity.Property(x => x.CreatedAt).HasColumnName("createdat");
                entity.Property(x => x.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
                entity.Property(x => x.UpdatedAt).HasColumnName("updatedat");
                entity.Property(x => x.UpdatedBy).HasColumnName("updatedby").HasMaxLength(100);
                entity.Property(x => x.IsDeleted).HasColumnName("isdeleted");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeCardLegacy>(entity =>
            {
                entity.ToTable("EREmpCards");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.CardTypeId).HasColumnName("cardtypeid");
                entity.Property(x => x.AuthorityId).HasColumnName("authorityid");
                entity.Property(x => x.CardNo).HasColumnName("cardno").HasMaxLength(25);
                entity.Property(x => x.ValidFrom).HasColumnName("validfrom");
                entity.Property(x => x.ValidTill).HasColumnName("validtill");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock1).HasColumnName("utblock1");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.CrdTypeId).HasColumnName("crdtypeid");
                entity.Property(x => x.AuthTypeId).HasColumnName("authtypeid");
                entity.Property(x => x.AuthId).HasColumnName("authid");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.CardType)
                    .WithMany()
                    .HasForeignKey(x => x.CardTypeId);

                entity.HasOne(x => x.Authority)
                    .WithMany()
                    .HasForeignKey(x => x.AuthorityId);
            });

            modelBuilder.Entity<EmployeeDiseaseLegacy>(entity =>
            {
                entity.ToTable("EREmployeedisease");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.LineIndex).HasColumnName("lineindex");
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.DiseaseId).HasColumnName("diseasesid");
                entity.Property(x => x.DiseaseCategoryId).HasColumnName("discatid");
                entity.Property(x => x.DiseaseTypeId).HasColumnName("distypeid");
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.Disease)
                    .WithMany()
                    .HasForeignKey(x => x.DiseaseId);

                entity.HasOne(x => x.DiseaseCategory)
                    .WithMany()
                    .HasForeignKey(x => x.DiseaseCategoryId);

                entity.HasOne(x => x.DiseaseType)
                    .WithMany()
                    .HasForeignKey(x => x.DiseaseTypeId);
            });

            modelBuilder.Entity<SMDiseaseHrLegacy>(entity =>
            {
                entity.ToTable("SMDisease");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.DiseaseCategoryId).HasColumnName("lovdiseasecat");
                entity.Property(x => x.DiseaseTypeId).HasColumnName("lovdiseasetype");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<IncomeTaxLegacy>(entity =>
            {
                entity.ToTable("orgincomtaxhdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(500);
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<BankGroupLegacy>(entity =>
            {
                entity.ToTable("FMBankgrp");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EmployeeBankAccountLegacy>(entity =>
            {
                entity.ToTable("EREmpBank");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.BankGroupId).HasColumnName("bankgrpid");
                entity.Property(x => x.BranchCode).HasColumnName("branchcode").HasMaxLength(10);
                entity.Property(x => x.BranchName).HasColumnName("branchname").HasMaxLength(100);
                entity.Property(x => x.AccountNo).HasColumnName("accountno").HasMaxLength(30);
                entity.Property(x => x.IbanNo).HasColumnName("ibanno").HasMaxLength(30);
                entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(100);
                entity.Property(x => x.PrimaryAccount).HasColumnName("primaryacct");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock1).HasColumnName("utblock1");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.BankGroup)
                    .WithMany()
                    .HasForeignKey(x => x.BankGroupId);
            });

            modelBuilder.Entity<EmployeePayrollLegacy>(entity =>
            {
                entity.ToTable("EREmppayroll");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.LoanAllowedId).HasColumnName("loanallowedid");
                entity.Property(x => x.AdvanceAllowedId).HasColumnName("advanceallowedid");
                entity.Property(x => x.OvertimeAllowedId).HasColumnName("ovtallowedid");
                entity.Property(x => x.OvertimePolicyId).HasColumnName("ovtpolicyid");
                entity.Property(x => x.PayFullSalary).HasColumnName("payfullsalary");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.IsLoanAllowed).HasColumnName("isloanallowed");
                entity.Property(x => x.IsAdvanceAllowed).HasColumnName("isadvallowed");
                entity.Property(x => x.IsOvertimeAllowed).HasColumnName("isovtallowed");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasMany(x => x.BaseSalaries)
                    .WithOne(x => x.Payroll)
                    .HasForeignKey(x => x.PayrollId);

                entity.HasMany(x => x.Allowances)
                    .WithOne(x => x.Payroll)
                    .HasForeignKey(x => x.PayrollId);

                entity.HasMany(x => x.Deductions)
                    .WithOne(x => x.Payroll)
                    .HasForeignKey(x => x.PayrollId);

                entity.HasMany(x => x.LoanTypes)
                    .WithOne(x => x.Payroll)
                    .HasForeignKey(x => x.PayrollId);
            });

            modelBuilder.Entity<EmployeePayrollBaseSalaryLegacy>(entity =>
            {
                entity.ToTable("EREmppaybasesalarydtl");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.PayrollId).HasColumnName("emppayrollid");
                entity.Property(x => x.LineIndex).HasColumnName("lineindex");
                entity.Property(x => x.EarningTypeId).HasColumnName("earntypeid");
                entity.Property(x => x.Value).HasColumnName("value");
                entity.Property(x => x.ToValue).HasColumnName("tovalue");
                entity.Property(x => x.StartDate).HasColumnName("startdate");
                entity.Property(x => x.EndDate).HasColumnName("enddate");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.HasOne(x => x.EarningType)
                    .WithMany()
                    .HasForeignKey(x => x.EarningTypeId);
            });

            modelBuilder.Entity<EmployeePayrollAllowanceLegacy>(entity =>
            {
                entity.ToTable("EREmppayallowancedtl");
                MapPayrollAmountDetail(entity);
                entity.Property(x => x.EarningTypeId).HasColumnName("earntypeid");
                entity.Property(x => x.IsPercentage).HasColumnName("ispercentage").HasDefaultValue(false);
                entity.HasOne(x => x.EarningType)
                    .WithMany()
                    .HasForeignKey(x => x.EarningTypeId);
            });

            modelBuilder.Entity<EmployeePayrollDeductionLegacy>(entity =>
            {
                entity.ToTable("EREmppaydeductiondtl");
                MapPayrollAmountDetail(entity);
                entity.Property(x => x.DeductionTypeId).HasColumnName("deductiontypeid");
                entity.HasOne(x => x.DeductionType)
                    .WithMany()
                    .HasForeignKey(x => x.DeductionTypeId);
            });

            modelBuilder.Entity<EmployeePayrollLoanTypeLegacy>(entity =>
            {
                entity.ToTable("EREmppayrollLoanType");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.PayrollId).HasColumnName("emppayrollid");
                entity.Property(x => x.LoanTypeId).HasColumnName("loantypeid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.HasOne(x => x.LoanType).WithMany().HasForeignKey(x => x.LoanTypeId);
            });

            modelBuilder.Entity<DesignationPayrollLegacy>(entity =>
            {
                entity.ToTable("ERDesgPayroll");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.DesignationId).HasColumnName("designationid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.LoanAllowed).HasColumnName("loanallowed");
                entity.Property(x => x.AdvanceAllowed).HasColumnName("advanceallowed");
                entity.Property(x => x.PayFullSalary).HasColumnName("payfullsalary");
                entity.Property(x => x.OvertimeAllowed).HasColumnName("overtimeallowed");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.HasMany(x => x.BaseSalaries).WithOne(x => x.Payroll).HasForeignKey(x => x.PayrollId);
                entity.HasMany(x => x.Allowances).WithOne(x => x.Payroll).HasForeignKey(x => x.PayrollId);
                entity.HasMany(x => x.Deductions).WithOne(x => x.Payroll).HasForeignKey(x => x.PayrollId);
                entity.HasMany(x => x.LoanTypes).WithOne(x => x.Payroll).HasForeignKey(x => x.PayrollId);
            });

            modelBuilder.Entity<DesignationPayrollBaseSalaryLegacy>(entity =>
            {
                entity.ToTable("ERDesgPayBaseSalaryDtl");
                MapDesignationPayrollAmountDetail(entity);
                entity.Property(x => x.EarningTypeId).HasColumnName("earntypeid");
                entity.Property(x => x.ToValue).HasColumnName("tovalue");
                entity.HasOne(x => x.EarningType).WithMany().HasForeignKey(x => x.EarningTypeId);
            });

            modelBuilder.Entity<DesignationPayrollAllowanceLegacy>(entity =>
            {
                entity.ToTable("ERDesgPayAllowanceDtl");
                MapDesignationPayrollAmountDetail(entity);
                entity.Property(x => x.EarningTypeId).HasColumnName("earntypeid");
                entity.Property(x => x.IsPercentage).HasColumnName("ispercentage").HasDefaultValue(false);
                entity.HasOne(x => x.EarningType).WithMany().HasForeignKey(x => x.EarningTypeId);
            });

            modelBuilder.Entity<DesignationPayrollDeductionLegacy>(entity =>
            {
                entity.ToTable("ERDesgPayDeductionDtl");
                MapDesignationPayrollAmountDetail(entity);
                entity.Property(x => x.DeductionTypeId).HasColumnName("deductiontypeid");
                entity.HasOne(x => x.DeductionType).WithMany().HasForeignKey(x => x.DeductionTypeId);
            });

            modelBuilder.Entity<DesignationPayrollLoanTypeLegacy>(entity =>
            {
                entity.ToTable("ERDesgPayLoanType");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.PayrollId).HasColumnName("designationpayrollid");
                entity.Property(x => x.LoanTypeId).HasColumnName("loantypeid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.HasOne(x => x.LoanType).WithMany().HasForeignKey(x => x.LoanTypeId);
            });

            modelBuilder.Entity<EmployeeEmploymentLegacy>(entity =>
            {
                entity.ToTable("EREmployment");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.EmploymentTypeId).HasColumnName("employmenttypeid");
                entity.Property(x => x.StartDate).HasColumnName("startdate");
                entity.Property(x => x.EndDate).HasColumnName("enddate");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EREmpPositionLegacy>(entity =>
            {
                entity.ToTable("EREmpPosition");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.EmployeeId).HasColumnName("employeeid");
                entity.Property(x => x.DepartmentId).HasColumnName("deptid");
                entity.Property(x => x.DesignationId).HasColumnName("desgid");
                entity.Property(x => x.GradeId).HasColumnName("gradeid");
                entity.Property(x => x.ActionId).HasColumnName("actionid");
                entity.Property(x => x.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(x => x.FromDate).HasColumnName("fromdate").HasColumnType("datetime");
                entity.Property(x => x.TillDate).HasColumnName("tilldate").HasColumnType("datetime");
                entity.Property(x => x.Responsibility).HasColumnName("responsibility").HasMaxLength(1500);
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);

                entity.HasOne(x => x.Department)
                    .WithMany()
                    .HasForeignKey(x => x.DepartmentId);

                entity.HasOne(x => x.Designation)
                    .WithMany()
                    .HasForeignKey(x => x.DesignationId);
            });

            modelBuilder.Entity<ShiftLegacy>(entity =>
            {
                entity.ToTable("ETShifthdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(x => x.ShiftTypeId).HasColumnName("shifttypeid");
                entity.Property(x => x.WoMonday).HasColumnName("womonday");
                entity.Property(x => x.WoTuesday).HasColumnName("wotuesday");
                entity.Property(x => x.WoWednesday).HasColumnName("wowednesday");
                entity.Property(x => x.WoThuresday).HasColumnName("wothuresday");
                entity.Property(x => x.WoFriday).HasColumnName("wofriday");
                entity.Property(x => x.WoSaturday).HasColumnName("wosaturday");
                entity.Property(x => x.WoSunday).HasColumnName("wosunday");
                entity.Property(x => x.OtPolicyId).HasColumnName("otpolicyid");
                entity.Property(x => x.LateArrivPolicyId).HasColumnName("latearivpolicyid");
                entity.Property(x => x.EarlyDepPolicyId).HasColumnName("earlydeppolicyid");
                entity.Property(x => x.AbsPolicyId).HasColumnName("abspolicyid");
                entity.Property(x => x.LovShiftClassId).HasColumnName("lovshiftclassid");
                entity.Property(x => x.LovSharedId).HasColumnName("lovsharedid");
                entity.Property(x => x.OtPolicyMnt).HasColumnName("otpolicymnt");
                entity.Property(x => x.LateArrivPolicyMnt).HasColumnName("latearivpolicymnt");
                entity.Property(x => x.EarlyDepPolicyMnt).HasColumnName("earlydeppolicymnt");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SmShiftLegacy>(entity =>
            {
                entity.ToTable("SMShifthdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(x => x.StartTime).HasColumnName("starttime");
                entity.Property(x => x.EndTime).HasColumnName("endtime");
                entity.Property(x => x.GraceTime).HasColumnName("gracetime");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.WoMonday).HasColumnName("womonday");
                entity.Property(x => x.WoTuesday).HasColumnName("wotuesday");
                entity.Property(x => x.WoWednesday).HasColumnName("wowednesday");
                entity.Property(x => x.WoThuresday).HasColumnName("wothuresday");
                entity.Property(x => x.WoFriday).HasColumnName("wofriday");
                entity.Property(x => x.WoSaturday).HasColumnName("wosaturday");
                entity.Property(x => x.WoSunday).HasColumnName("wosunday");
            });

            modelBuilder.Entity<ScheduleHeaderLegacy>(entity =>
            {
                entity.ToTable("ETSchedulehdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.FromDate).HasColumnName("fromdate");
                entity.Property(x => x.TillDate).HasColumnName("tilldate");
                entity.Property(x => x.ScheduleFor).HasColumnName("schedulefor");
                entity.Property(x => x.ShiftId).HasColumnName("shiftid");
                entity.Property(x => x.AssignTo).HasColumnName("assignto");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ScheduleDetailLegacy>(entity =>
            {
                entity.ToTable("ETScheduledtl");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.ScheduleId).HasColumnName("scheduleid");
                entity.Property(x => x.LineIndex).HasColumnName("lineindex");
                entity.Property(x => x.SchDate).HasColumnName("schdate");
                entity.Property(x => x.Holiday).HasColumnName("holiday");
                entity.Property(x => x.ShiftId).HasColumnName("shiftid");
                entity.Property(x => x.TimeIn).HasColumnName("timein");
                entity.Property(x => x.TimeOut).HasColumnName("timeout");
                entity.Property(x => x.Duration).HasColumnName("duration");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMCustomShiftHeaderLegacy>(entity =>
            {
                entity.ToTable("SMCustomshifthdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(x => x.StartTime).HasColumnName("starttime");
                entity.Property(x => x.EndTime).HasColumnName("endtime");
                entity.Property(x => x.GraceTime).HasColumnName("gracetime");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.ShiftTypeId).HasColumnName("shifttypeid");
                entity.Property(x => x.WoMonday).HasColumnName("womonday");
                entity.Property(x => x.WoTuesday).HasColumnName("wotuesday");
                entity.Property(x => x.WoWednesday).HasColumnName("wowednesday");
                entity.Property(x => x.WoThuresday).HasColumnName("wothuresday");
                entity.Property(x => x.WoFriday).HasColumnName("wofriday");
                entity.Property(x => x.WoSaturday).HasColumnName("wosaturday");
                entity.Property(x => x.WoSunday).HasColumnName("wosunday");
                entity.Property(x => x.GraceTimeEd).HasColumnName("gracetimeed");
            });

            modelBuilder.Entity<OrgLegalentityLegacy>(entity =>
            {
                entity.ToTable("OrgLegalentity");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityNo).HasColumnName("entityno").HasMaxLength(20);
                entity.Property(x => x.ShortName).HasColumnName("shortname").HasMaxLength(50);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(x => x.CampusName).HasColumnName("campusname").HasMaxLength(150);
                entity.Property(x => x.HeadEntityId).HasColumnName("headentity");
                entity.Property(x => x.CampusTypeId).HasColumnName("lovcampustypeid");
                entity.Property(x => x.CountryId).HasColumnName("countryid");
                entity.Property(x => x.CityId).HasColumnName("cityid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMPolicyLegacy>(entity =>
            {
                entity.ToTable("SMPolicy");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.DefaultCountryId).HasColumnName("defaultctryid");
                entity.Property(x => x.DefaultCityId).HasColumnName("defaultcityid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMAcadPolicyVacancyLegacy>(entity =>
            {
                entity.ToTable("SMAcadpolicy");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.UseVacancyProcess).HasColumnName("useVacancyProcess");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMCustomShiftDetailLegacy>(entity =>
            {
                entity.ToTable("SMCustomshiftdtl");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.ShiftId).HasColumnName("shiftid");
                entity.Property(x => x.LineIndex).HasColumnName("lineindex");
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(x => x.StartTime).HasColumnName("starttime");
                entity.Property(x => x.EndTime).HasColumnName("endtime");
                entity.Property(x => x.Duration).HasColumnName("duration");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtAddTime).HasColumnName("utaddtime");
                entity.Property(x => x.UtAddUser).HasColumnName("utadduser").HasMaxLength(25);
                entity.Property(x => x.UtAddSystem).HasColumnName("utaddsystem").HasMaxLength(25);
                entity.Property(x => x.UtAddIp).HasColumnName("utaddip").HasMaxLength(25);
                entity.Property(x => x.UtAddAppVersion).HasColumnName("utaddappversion").HasMaxLength(10);
                entity.Property(x => x.UtAddDbVersion).HasColumnName("utadddbversion").HasMaxLength(10);
                entity.Property(x => x.UtAddId).HasColumnName("utaddid");
                entity.Property(x => x.UtAddSessionId).HasColumnName("utaddsessionid");
                entity.Property(x => x.UtEditTime).HasColumnName("utedittime");
                entity.Property(x => x.UtEditUser).HasColumnName("utedituser").HasMaxLength(25);
                entity.Property(x => x.UtEditSystem).HasColumnName("uteditsystem").HasMaxLength(25);
                entity.Property(x => x.UtEditIp).HasColumnName("uteditip").HasMaxLength(25);
                entity.Property(x => x.UtEditAppVersion).HasColumnName("uteditappversion").HasMaxLength(10);
                entity.Property(x => x.UtEditDbVersion).HasColumnName("uteditdbversion").HasMaxLength(10);
                entity.Property(x => x.UtEditId).HasColumnName("uteditid");
                entity.Property(x => x.UtEditSessionId).HasColumnName("uteditsessionid");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<LeaveGroupLegacy>(entity =>
            {
                entity.ToTable("ETLeavegrouphdr");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(x => x.Stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(x => x.AdjustmentPolicyId).HasColumnName("adjpolicyid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.ClientId).HasColumnName("clientid");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");

                entity.HasMany(x => x.Details)
                    .WithOne()
                    .HasForeignKey(x => x.LeaveGroupId);
            });

            modelBuilder.Entity<LeaveGroupDetailLegacy>(entity =>
            {
                entity.ToTable("ETLeavegroupdtl");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(x => x.LeaveGroupId).HasColumnName("leavegrpid");
                entity.Property(x => x.LeaveTypeId).HasColumnName("leavetypeid");
                entity.Property(x => x.LeaveDays).HasColumnName("leavedays");
                entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
                entity.Property(x => x.UtDocType).HasColumnName("utdoctype");
                entity.Property(x => x.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
                entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<UtLovLegacy>(entity =>
            {
                entity.ToTable("lov");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.Vtype).HasColumnName("vtype").HasMaxLength(20);
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETPayperiodhdrLegacy>(entity =>
            {
                entity.ToTable("ETPayperiodhdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Atntypeid).HasColumnName("atntypeid");
                entity.Property(e => e.Fromyear).HasColumnName("fromyear");
                entity.Property(e => e.Toyear).HasColumnName("toyear");
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(250);
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.Startfrom).HasColumnName("startfrom");
                entity.Property(e => e.Statusid).HasColumnName("statusid");
                entity.Property(e => e.Lovsharedid).HasColumnName("lovsharedid");
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Payrollregisterid).HasColumnName("payrollregisterid");
                entity.Property(e => e.Noofperiod).HasColumnName("noofperiod");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.Utblock).HasColumnName("utblock");

                entity.HasMany(e => e.Details)
                    .WithOne(d => d.Header)
                    .HasForeignKey(d => d.Payperiodid);
            });

            modelBuilder.Entity<ETPayperioddtlLegacy>(entity =>
            {
                entity.ToTable("ETPayperioddtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Payperiodid).HasColumnName("payperiodid");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.Monthid).HasColumnName("monthid");
                entity.Property(e => e.Periodstxt).HasColumnName("periodstxt").HasMaxLength(200);
                entity.Property(e => e.Startdate).HasColumnName("startdate");
                entity.Property(e => e.Enddate).HasColumnName("enddate");
                entity.Property(e => e.Noofdays).HasColumnName("noofdays");
                entity.Property(e => e.Disbdate).HasColumnName("disbdate");
                entity.Property(e => e.Statusid).HasColumnName("statusid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETPayrollregisterLegacy>(entity =>
            {
                entity.ToTable("ETPayrollregister");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(250);
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EPEarningtypeLegacy>(entity =>
            {
                entity.ToTable("EPEarningtype");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(250);
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.PayRunTypeId).HasColumnName("payruntypeid");
                entity.Property(e => e.TaxMethodId).HasColumnName("taxmethodid");
                entity.Property(e => e.UomId).HasColumnName("uomid");
                entity.Property(e => e.RateBasisId).HasColumnName("ratebasisid");
                entity.Property(e => e.BsSalary).HasColumnName("bssalary");
                entity.Property(e => e.EarningTypeId).HasColumnName("earningtypeid");
                entity.Property(e => e.PaySlipIndex).HasColumnName("paysliindex").HasMaxLength(100);
                entity.Property(e => e.PaySlipDescription).HasColumnName("payslipdesc").HasMaxLength(500);
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EPDeductiontypeLegacy>(entity =>
            {
                entity.ToTable("EPDeductiontype");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.Ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.PayRunTypeId).HasColumnName("payruntypeid");
                entity.Property(e => e.Reimbursed).HasColumnName("reimbursed");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtDocType).HasColumnName("utdoctype");
                entity.Property(e => e.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<EPLoantypeLegacy>(entity =>
            {
                entity.ToTable("EPLoantype");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.loannatureid).HasColumnName("loannatureid");
                entity.Property(e => e.limitid).HasColumnName("limitid");
                entity.Property(e => e.limit).HasColumnName("limit");
                entity.Property(e => e.availid).HasColumnName("availid");
                entity.Property(e => e.loaninterestid).HasColumnName("loaninterestid");
                entity.Property(e => e.jobduration).HasColumnName("jobduration");
                entity.Property(e => e.noofloan).HasColumnName("noofloan");
                entity.Property(e => e.noofinstallments).HasColumnName("noofinstallments");
                entity.Property(e => e.flexible).HasColumnName("flexible");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<EPDeductiongrphdrLegacy>(entity =>
            {
                entity.ToTable("EPDeductiongrphdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utdoctype).HasColumnName("utdoctype");
                entity.Property(e => e.utsubdoctype).HasColumnName("utsubdoctype");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasMany(e => e.details)
                    .WithOne(d => d.header)
                    .HasForeignKey(d => d.dedgrpid);
            });

            modelBuilder.Entity<EPDeductiongrpdtlLegacy>(entity =>
            {
                entity.ToTable("EPDeductiongrpdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.dedgrpid).HasColumnName("dedgrpid");
                entity.Property(e => e.lineindex).HasColumnName("lineindex");
                entity.Property(e => e.dedtypeid).HasColumnName("dedtypeid");
                entity.Property(e => e.ratebasisid).HasColumnName("ratebasisid");
                entity.Property(e => e.basisearntypeid).HasColumnName("basisearntypeid");
                entity.Property(e => e.value).HasColumnName("value");
                entity.Property(e => e.payslipindex).HasColumnName("payslipindex");
                entity.Property(e => e.paysliptxt).HasColumnName("paysliptxt").HasMaxLength(100);
                entity.Property(e => e.statusid).HasColumnName("statusid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<EPEarninggrphdrLegacy>(entity =>
            {
                entity.ToTable("EPEarninggrphdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utdoctype).HasColumnName("utdoctype");
                entity.Property(e => e.utsubdoctype).HasColumnName("utsubdoctype");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasMany(e => e.details)
                    .WithOne(d => d.header)
                    .HasForeignKey(d => d.earngrpid);
            });

            modelBuilder.Entity<EPEarninggrpdtlLegacy>(entity =>
            {
                entity.ToTable("EPEarninggrpdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.earngrpid).HasColumnName("earngrpid");
                entity.Property(e => e.lineindex).HasColumnName("lineindex");
                entity.Property(e => e.earntypeid).HasColumnName("earntypeid");
                entity.Property(e => e.ratebasisid).HasColumnName("ratebasisid");
                entity.Property(e => e.basisearntypeid).HasColumnName("basisearntypeid");
                entity.Property(e => e.value).HasColumnName("value");
                entity.Property(e => e.absnoteffective).HasColumnName("absnoteffective");
                entity.Property(e => e.levnoteffective).HasColumnName("levnoteffective");
                entity.Property(e => e.isotpart).HasColumnName("isotpart");
                entity.Property(e => e.payslipindex).HasColumnName("payslipindex");
                entity.Property(e => e.paysliptxt).HasColumnName("paysliptxt").HasMaxLength(100);
                entity.Property(e => e.statusid).HasColumnName("statusid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<HRAtnintoutLegacy>(entity =>
            {
                entity.ToTable("HRAtnintout");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.EmpId).HasColumnName("empid");
                entity.Property(e => e.MachineId).HasColumnName("machineid");
                entity.Property(e => e.AtnTypeId).HasColumnName("atntypeid");
                entity.Property(e => e.AtnActivityId).HasColumnName("atnactivityid");
                entity.Property(e => e.Dttime).HasColumnName("dttime");
                entity.Property(e => e.OrgDttime).HasColumnName("orgdttime");
                entity.Property(e => e.ShiftId).HasColumnName("shiftid");
                entity.Property(e => e.ShiftDtlId).HasColumnName("shiftdtlid");
                entity.Property(e => e.DayId).HasColumnName("dayid");
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(e => e.FilePath).HasColumnName("filepath").HasMaxLength(1500);
                entity.Property(e => e.PostTransId).HasColumnName("posttransid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ShiftIn).HasColumnName("shiftin");
                entity.Property(e => e.ShiftOut).HasColumnName("shiftout");
                entity.Property(e => e.SchTimeIn).HasColumnName("schtimein");
                entity.Property(e => e.SchTimeOut).HasColumnName("schtimeout");
                entity.Property(e => e.LateMin).HasColumnName("latemin");
                entity.Property(e => e.LateCount).HasColumnName("latcount");
                entity.Property(e => e.EdMin).HasColumnName("edmin");
                entity.Property(e => e.EdCount).HasColumnName("edcount");
                entity.Property(e => e.OtMin).HasColumnName("otmin");
                entity.Property(e => e.OtCount).HasColumnName("otcount");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<HRAtnactivityLegacy>(entity =>
            {
                entity.ToTable("HRAtnactivity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.OrderId).HasColumnName("orderid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OrgDepartmentLegacy>(entity =>
            {
                entity.ToTable("OrgDepartment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.DepCode).HasColumnName("depcode").HasMaxLength(50);
                entity.Property(e => e.DepStxt).HasColumnName("depstxt").HasMaxLength(50);
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.OpUnitId).HasColumnName("opunitid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OrgDesignationLegacy>(entity =>
            {
                entity.ToTable("OrgDesignation");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Code).HasColumnName("desgcode").HasMaxLength(50);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });



            modelBuilder.Entity<ETPolicyhdrLegacy>(entity =>
            {
                entity.ToTable("ETPolicyhdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.Policytypeid).HasColumnName("policytypeid");
                entity.Property(e => e.Applyoffdays).HasColumnName("applyoffdays");
                entity.Property(e => e.Noofexcuse).HasColumnName("noofexcuse");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.Utblock).HasColumnName("utblock");

                entity.HasMany(e => e.PolicyDetails)
                    .WithOne(d => d.Header)
                    .HasForeignKey(d => d.Policyid);

                entity.HasMany(e => e.AbsentDetails)
                    .WithOne(d => d.Header)
                    .HasForeignKey(d => d.Policyid);
            });

            modelBuilder.Entity<ETPolicydtlLegacy>(entity =>
            {
                entity.ToTable("ETPolicydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Policyid).HasColumnName("policyid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.Starttime).HasColumnName("starttime");
                entity.Property(e => e.Endtime).HasColumnName("endtime");
                entity.Property(e => e.Periodtypeid).HasColumnName("periodtypeid");
                entity.Property(e => e.Valtypeid).HasColumnName("valtypeid");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETAbsentpolicydtlLegacy>(entity =>
            {
                entity.ToTable("ETAbsentpolicydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Policyid).HasColumnName("policyid");
                entity.Property(e => e.Lineindex).HasColumnName("lineindex");
                entity.Property(e => e.Periodtypeid).HasColumnName("periodtypeid");
                entity.Property(e => e.Policycount).HasColumnName("policycount");
                entity.Property(e => e.Absentcount).HasColumnName("absentcount");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.Utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<HRDailyAttendanceLegacy>(entity =>
            {
                entity.ToTable("HRdailyattendance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
                entity.Property(e => e.AtnDate).HasColumnName("atndate");
                entity.Property(e => e.SchTimeIn).HasColumnName("schtimein");
                entity.Property(e => e.SchTimeOut).HasColumnName("schtimeout");
                entity.Property(e => e.ActShiftIn).HasColumnName("actshiftin");
                entity.Property(e => e.ActShitOut).HasColumnName("actshitout");
                entity.Property(e => e.ShiftId).HasColumnName("shiftid");
                entity.Property(e => e.ShiftDtlId).HasColumnName("shiftdtlid");
                entity.Property(e => e.DayId).HasColumnName("dayid");
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.LateMin).HasColumnName("latemin");
                entity.Property(e => e.LateCount).HasColumnName("latecount");
                entity.Property(e => e.LateHalfDays).HasColumnName("lathalfdays");
                entity.Property(e => e.LateAbsent).HasColumnName("lateabsent");
                entity.Property(e => e.LateExcuse).HasColumnName("lateexcuse");
                entity.Property(e => e.EdMin).HasColumnName("edmin");
                entity.Property(e => e.EdCount).HasColumnName("edcount");
                entity.Property(e => e.EdHalfDays).HasColumnName("edhalfdays");
                entity.Property(e => e.EdAbsent).HasColumnName("edabsent");
                entity.Property(e => e.EdExcuse).HasColumnName("edexcuse");
                entity.Property(e => e.OtMin).HasColumnName("otmin");
                entity.Property(e => e.OtCount).HasColumnName("otcount");
                entity.Property(e => e.OtHalfDays).HasColumnName("othalfdays");
                entity.Property(e => e.DepId).HasColumnName("depid");
                entity.Property(e => e.DesignationId).HasColumnName("designationid");
                entity.Property(e => e.IsAbsent).HasColumnName("isabsent");
                entity.Property(e => e.IsHoliday).HasColumnName("isholiday");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETShiftdtlLegacy>(entity =>
            {
                entity.ToTable("ETShiftdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.ShiftId).HasColumnName("shiftid");
                entity.Property(e => e.LineIndex).HasColumnName("lineindex");
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.DayId).HasColumnName("dayid");
                entity.Property(e => e.InTime).HasColumnName("intime");
                entity.Property(e => e.OutTime).HasColumnName("outtime");
                entity.Property(e => e.GraceTime).HasColumnName("gracetime");
                entity.Property(e => e.GraceTimeEnd).HasColumnName("gracetimeend");
                entity.Property(e => e.LatePolicyApplicable).HasColumnName("latepolicyapplicable");
                entity.Property(e => e.EarlyPolicyApplicable).HasColumnName("earlypolicyapplicable");
                entity.Property(e => e.StartDate).HasColumnName("startdate");
                entity.Property(e => e.EndDate).HasColumnName("endate");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OrgLegalentityLegacy>(entity =>
            {
                entity.ToTable("OrgLegalentity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityNo).HasColumnName("entityno");
                entity.Property(e => e.ShortName).HasColumnName("shortname");
                entity.Property(e => e.Stxt).HasColumnName("stxt");
                entity.Property(e => e.CampusName).HasColumnName("campusname");
                entity.Property(e => e.CampusTypeId).HasColumnName("lovcampustypeid");
                entity.Property(e => e.CountryId).HasColumnName("countryid");
                entity.Property(e => e.CityId).HasColumnName("cityid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETShiftsubdtlLegacy>(entity =>
            {
                entity.ToTable("ETShiftsubdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.ShiftDtlId).HasColumnName("shiftdtlid");
                entity.Property(e => e.LineIndex).HasColumnName("lineindex");
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(e => e.StartTime).HasColumnName("starttime");
                entity.Property(e => e.EndTime).HasColumnName("endtime");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETShiftshareddtlLegacy>(entity =>
            {
                entity.ToTable("ETShiftshareddtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.LineIndex).HasColumnName("lineindex");
                entity.Property(e => e.ShiftId).HasColumnName("shiftid");
                entity.Property(e => e.LovSharedId).HasColumnName("lovsharedid");
                entity.Property(e => e.PClientId).HasColumnName("pclientid");
                entity.Property(e => e.PEntityId).HasColumnName("pentityid");
                entity.Property(e => e.PDepartId).HasColumnName("pdepartid");
                entity.Property(e => e.PEmpId).HasColumnName("pempid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<ETLeaveRequestLegacy>(entity =>
            {
                entity.ToTable("ETLeaveRequest");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.RequestNo).HasColumnName("requestno").HasMaxLength(20);
                entity.Property(e => e.RequestDate).HasColumnName("requestdate");
                entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
                entity.Property(e => e.LeaveTypeId).HasColumnName("leavetypeid");
                entity.Property(e => e.LeaveUnitId).HasColumnName("leaveunitid");
                entity.Property(e => e.LeaveDays).HasColumnName("leavedays");
                entity.Property(e => e.DOB).HasColumnName("DOB");
                entity.Property(e => e.InLieuDate).HasColumnName("inleuedate");
                entity.Property(e => e.FromDate).HasColumnName("fromdate");
                entity.Property(e => e.TillDate).HasColumnName("tilldate");
                entity.Property(e => e.FromTime).HasColumnName("fromtime");
                entity.Property(e => e.TillTime).HasColumnName("tilltime");
                entity.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(1500);
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(e => e.StatusId).HasColumnName("statusid");
                entity.Property(e => e.ApprovalDate).HasColumnName("approvaldate");
                entity.Property(e => e.ApprovedById).HasColumnName("approvedbyid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId);

                entity.HasOne(e => e.LeaveType)
                    .WithMany()
                    .HasForeignKey(e => e.LeaveTypeId);
            });

            modelBuilder.Entity<EPLoanrequestLegacy>(entity =>
            {
                entity.ToTable("EPLoanrequest");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.requestno).HasColumnName("requestno").HasMaxLength(50);
                entity.Property(e => e.requestdate).HasColumnName("requestdate");
                entity.Property(e => e.requiredate).HasColumnName("requiredate");
                entity.Property(e => e.employeeid).HasColumnName("employeeid");
                entity.Property(e => e.loannatureid).HasColumnName("loannatureid");
                entity.Property(e => e.loantypeid).HasColumnName("loantypeid");
                entity.Property(e => e.appliedamount).HasColumnName("appliedamount");
                entity.Property(e => e.reasontypeid).HasColumnName("reasontypeid");
                entity.Property(e => e.reason).HasColumnName("reason").HasMaxLength(1500);
                entity.Property(e => e.dedtypeid).HasColumnName("dedtypeid");
                entity.Property(e => e.instalamt).HasColumnName("instalamt");
                entity.Property(e => e.noofinstal).HasColumnName("noofinstal");
                entity.Property(e => e.approvenoofinstallments).HasColumnName("approvenoofinstallments");
                entity.Property(e => e.approveamounts).HasColumnName("approveamounts");
                entity.Property(e => e.installmentstartdate).HasColumnName("installmentstartdate");
                entity.Property(e => e.HRComments).HasColumnName("HRComments").HasMaxLength(1500);
                entity.Property(e => e.LineManagerComments).HasColumnName("LineManagerComments").HasMaxLength(1500);
                entity.Property(e => e.statusid).HasColumnName("statusid");
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utdoctype).HasColumnName("utdoctype");
                entity.Property(e => e.utsubdoctype).HasColumnName("utsubdoctype");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion").IsConcurrencyToken();

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.employeeid);

                entity.HasOne(e => e.LoanType)
                    .WithMany()
                    .HasForeignKey(e => e.loantypeid);

            });

            modelBuilder.Entity<HRMonthlyAttendanceLegacy>(entity =>
            {
                entity.ToTable("HRMonthlyAttendance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
                entity.Property(e => e.RegisterId).HasColumnName("registerid");
                entity.Property(e => e.PayPeriodId).HasColumnName("payperiodid");
                entity.Property(e => e.PayPeriodDetailId).HasColumnName("payperiodtlid");
                entity.Property(e => e.StartDate).HasColumnName("startdate");
                entity.Property(e => e.EndDate).HasColumnName("enddate");
                entity.Property(e => e.DisbDate).HasColumnName("disbdate");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.Days).HasColumnName("days");
                entity.Property(e => e.PresentsCount).HasColumnName("presentscount");
                entity.Property(e => e.AbsentsCount).HasColumnName("absentscount");
                entity.Property(e => e.HalfDaysCount).HasColumnName("halfdayscount");
                entity.Property(e => e.LateCount).HasColumnName("latecount");
                entity.Property(e => e.EarlyCounts).HasColumnName("earlycounts");
                entity.Property(e => e.ActualDays).HasColumnName("actualdays");
                entity.Property(e => e.ActualPresent).HasColumnName("actualpresent");
                entity.Property(e => e.Holidays).HasColumnName("holidays");
                entity.Property(e => e.Leaves).HasColumnName("leaves");
                entity.Ignore(e => e.Deptid);
                entity.Ignore(e => e.Desgid);
                entity.Ignore(e => e.Salary);
                entity.Ignore(e => e.Allowance);
                entity.Ignore(e => e.Deduction);
                entity.Ignore(e => e.Loanddeduction);
                entity.Ignore(e => e.Grosssalary);
                entity.Ignore(e => e.Tax);
                entity.Ignore(e => e.Department);
                entity.Ignore(e => e.Designation);
                entity.Property(e => e.UtBlock).HasColumnName("utblock");

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId);
            });

            modelBuilder.Entity<ETLeaveadjpolicyLegacy>(entity =>
            {
                entity.ToTable("ETLeaveadjpolicy");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");

                entity.HasMany(e => e.Details)
                    .WithOne(d => d.Header)
                    .HasForeignKey(d => d.leaveadjid);
            });

            modelBuilder.Entity<ETLeaveadjpolicydtlLegacy>(entity =>
            {
                entity.ToTable("ETLeaveadjpolicydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.leaveadjid).HasColumnName("leaveadjid");
                entity.Property(e => e.leavetypeid).HasColumnName("leavetypeid");
                entity.Property(e => e.adjtypeid).HasColumnName("adjtypeid");
                entity.Property(e => e.basedonid).HasColumnName("basedonid");
                entity.Property(e => e.priorityid).HasColumnName("priorityid");
                entity.Property(e => e.days).HasColumnName("days");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
            });

            modelBuilder.Entity<ETLeavetypeLegacy>(entity =>
            {
                entity.ToTable("ETLeavetype");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.leaves).HasColumnName("leaves");
                entity.Property(e => e.renewonid).HasColumnName("renewonid");
                entity.Property(e => e.lvunitid).HasColumnName("lvunitid");
                entity.Property(e => e.availunitid).HasColumnName("availunitid");
                entity.Property(e => e.maxavail).HasColumnName("maxavail");
                entity.Property(e => e.maritalid).HasColumnName("maritalid");
                entity.Property(e => e.carrylv).HasColumnName("carrylv");
                entity.Property(e => e.carrylvid).HasColumnName("carrylvid");
                entity.Property(e => e.encash).HasColumnName("encash");
                entity.Property(e => e.encashid).HasColumnName("encashid");
                entity.Property(e => e.genderid).HasColumnName("genderid");
                entity.Property(e => e.entitleid).HasColumnName("entitleid");
                entity.Property(e => e.accrualunitid).HasColumnName("accrualunitid");
                entity.Property(e => e.accrualleaves).HasColumnName("accrualleaves");
                entity.Property(e => e.allowinprob).HasColumnName("allowinprob");
                entity.Property(e => e.quotavalidate).HasColumnName("quotavalidate");
                entity.Property(e => e.lateadjust).HasColumnName("lateadjust");
                entity.Property(e => e.earlydepadjust).HasColumnName("earlydepadjust");
                entity.Property(e => e.halfadjust).HasColumnName("halfadjust");
                entity.Property(e => e.incholidays).HasColumnName("incholidays");
                entity.Property(e => e.exclweeklyholidays).HasColumnName("exclweeklyholidays");
                entity.Property(e => e.ispaid).HasColumnName("ispaid");
                entity.Property(e => e.requestbefdays).HasColumnName("requestbefdays");
                entity.Property(e => e.requestunitid).HasColumnName("requestunitid");
                entity.Property(e => e.nom).HasColumnName("nom");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
            });

            modelBuilder.Entity<ETLeavetranLegacy>(entity =>
            {
                entity.ToTable("ETLeavetrans");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.TransNo).HasColumnName("transno").HasMaxLength(50);
                entity.Property(e => e.TransDate).HasColumnName("transdate");
                entity.Property(e => e.RequestId).HasColumnName("requestid");
                entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
                entity.Property(e => e.LeaveTypeId).HasColumnName("leavetypeid");
                entity.Property(e => e.LeaveUnitId).HasColumnName("leaveunitid");
                entity.Property(e => e.LeaveDays).HasColumnName("leavedays");
                entity.Property(e => e.FromDate).HasColumnName("fromdate");
                entity.Property(e => e.TillDate).HasColumnName("tilldate");
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(e => e.StatusId).HasColumnName("statusid");
                entity.Property(e => e.AdmApproval).HasColumnName("admApproval");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.IsPaid).HasColumnName("ispaid");
                entity.Property(e => e.UtDocType).HasColumnName("utdoctype");
                entity.Property(e => e.UtSubDocType).HasColumnName("utsubdoctype");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId);

                entity.HasOne(e => e.LeaveType)
                    .WithMany()
                    .HasForeignKey(e => e.LeaveTypeId);

                entity.HasOne(e => e.Request)
                    .WithMany()
                    .HasForeignKey(e => e.RequestId);
            });

            modelBuilder.Entity<EPLoanrequestLegacy>(entity =>
            {
                entity.ToTable("EPLoanrequest");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(x => x.requestno).HasColumnName("requestno");
                entity.Property(x => x.requestdate).HasColumnName("requestdate");
                entity.Property(x => x.requiredate).HasColumnName("requiredate");
                entity.Property(x => x.employeeid).HasColumnName("employeeid");
                entity.Property(x => x.loannatureid).HasColumnName("loannatureid");
                entity.Property(x => x.loantypeid).HasColumnName("loantypeid");
                entity.Property(x => x.appliedamount).HasColumnName("appliedamount");
                entity.Property(x => x.reasontypeid).HasColumnName("reasontypeid");
                entity.Property(x => x.reason).HasColumnName("reason");
                entity.Property(x => x.dedtypeid).HasColumnName("dedtypeid");
                entity.Property(x => x.instalamt).HasColumnName("instalamt");
                entity.Property(x => x.noofinstal).HasColumnName("noofinstal");
                entity.Property(x => x.approvenoofinstallments).HasColumnName("approvenoofinstallments");
                entity.Property(x => x.approveamounts).HasColumnName("approveamounts");
                entity.Property(x => x.HRComments).HasColumnName("HRComments").HasMaxLength(1500);
                entity.Property(x => x.LineManagerComments).HasColumnName("LineManagerComments").HasMaxLength(1500);
                entity.Property(x => x.statusid).HasColumnName("statusid");
                entity.Property(x => x.entityid).HasColumnName("entityid");
                entity.Property(x => x.clientid).HasColumnName("clientid");
                entity.Property(x => x.utblock).HasColumnName("utblock");
                entity.Property(x => x.utsysgen).HasColumnName("utsysgen");
                entity.Property(x => x.utispartial).HasColumnName("utispartial");
                entity.Property(x => x.utrecordversion).HasColumnName("utrecordversion").IsConcurrencyToken();

                entity.HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.employeeid);

                entity.HasOne(x => x.LoanType)
                    .WithMany()
                    .HasForeignKey(x => x.loantypeid);
            });

            modelBuilder.Entity<EPLoantranshdrLegacy>(entity =>
            {
                entity.ToTable("EPLoantranshdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.loanno).HasColumnName("loanno").HasMaxLength(20);
                entity.Property(e => e.loandate).HasColumnName("loandate");
                entity.Property(e => e.postingdate).HasColumnName("postingdate");
                entity.Property(e => e.typeid).HasColumnName("typeid");
                entity.Property(e => e.requestid).HasColumnName("requestid");
                entity.Property(e => e.employeeid).HasColumnName("employeeid");
                entity.Property(e => e.loantypeid).HasColumnName("loantypeid");
                entity.Property(e => e.appliedamount).HasColumnName("appliedamount");
                entity.Property(e => e.loanamount).HasColumnName("loanamount");
                entity.Property(e => e.instalamt).HasColumnName("instalamt");
                entity.Property(e => e.noofinstal).HasColumnName("noofinstal");
                entity.Property(e => e.recoveryyear).HasColumnName("recoveryyear");
                entity.Property(e => e.recoverymonth).HasColumnName("recoverymonth");
                entity.Property(e => e.dedtypeid).HasColumnName("dedtypeid");
                entity.Property(e => e.payperiodid).HasColumnName("payperiodid");
                entity.Property(e => e.perioddtlid).HasColumnName("perioddtlid");
                entity.Property(e => e.reason).HasColumnName("reason").HasMaxLength(1500);
                entity.Property(e => e.statusid).HasColumnName("statusid");
                entity.Property(e => e.TrusteeStatusId).HasColumnName("TrusteeStatusId").IsRequired();
                entity.Ignore(e => e.entityid);
                entity.Ignore(e => e.clientid);
                entity.Property(e => e.utdoctype).HasColumnName("utdoctype");
                entity.Property(e => e.utsubdoctype).HasColumnName("utsubdoctype");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.Property(e => e.utaddtime).HasColumnName("utaddtime");
                entity.Property(e => e.utadduser).HasColumnName("utadduser").HasMaxLength(25);

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.employeeid);

                entity.HasOne(e => e.LoanRequest)
                    .WithMany()
                    .HasForeignKey(e => e.requestid);

                entity.HasOne(e => e.TrusteeStatus)
                    .WithMany()
                    .HasForeignKey(e => e.TrusteeStatusId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EPLoantransdtlLegacy>(entity =>
            {
                entity.ToTable("EPLoantransdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.loanid).HasColumnName("loanid");
                entity.Property(e => e.year).HasColumnName("year");
                entity.Property(e => e.monthid).HasColumnName("monthid");
                entity.Property(e => e.installment).HasColumnName("installment");
                entity.Property(e => e.balance).HasColumnName("balance");
                entity.Property(e => e.recovered).HasColumnName("recovered");
                entity.Property(e => e.merged).HasColumnName("merged");
                entity.Property(e => e.notdeduct).HasColumnName("notdeduct");
                entity.Property(e => e.mergeloanid).HasColumnName("mergeloanid");
                entity.Property(e => e.reason).HasColumnName("reason").HasMaxLength(500);
                entity.Ignore(e => e.entityid);
                entity.Ignore(e => e.clientid);
                entity.HasOne(e => e.LoanIssuance)
                    .WithMany(h => h.EPLoantransdtls)
                    .HasForeignKey(e => e.loanid);
            });

            modelBuilder.Entity<HRpayrollprocessLegacy>(entity =>
            {
                entity.ToTable("HRpayrollprocess");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.transno).HasColumnName("transno").HasMaxLength(50);
                entity.Property(e => e.transdate).HasColumnName("transdate");
                entity.Property(e => e.registerid).HasColumnName("registerid");
                entity.Property(e => e.periodid).HasColumnName("periodid");
                entity.Property(e => e.salarytypeid).HasColumnName("salarytypeid");
                entity.Property(e => e.processtypeid).HasColumnName("processtypeid");
                entity.Property(e => e.employeeid).HasColumnName("employeeid");
                entity.Property(e => e.deptid).HasColumnName("deptid");
                entity.Property(e => e.desgid).HasColumnName("desgid");
                entity.Property(e => e.statusid).HasColumnName("statusid");
                entity.Property(e => e.days).HasColumnName("days");
                entity.Property(e => e.present).HasColumnName("present");
                entity.Property(e => e.absent).HasColumnName("absent");
                entity.Property(e => e.leaves).HasColumnName("leaves");
                entity.Property(e => e.holidays).HasColumnName("holidays");
                entity.Property(e => e.salary).HasColumnName("salary");
                entity.Property(e => e.allowance).HasColumnName("allowance");
                entity.Property(e => e.deduction).HasColumnName("deduction");
                entity.Property(e => e.tax).HasColumnName("tax");
                entity.Property(e => e.grosssalary).HasColumnName("grosssalary");
                entity.Property(e => e.remarks).HasColumnName("remarks").HasMaxLength(500);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.perioddtlid).HasColumnName("perioddtlid");
                entity.Property(e => e.loanddeduction).HasColumnName("loanddeduction");
                entity.Property(e => e.latededuction).HasColumnName("latededuction");
                entity.Property(e => e.fixedded).HasColumnName("fixedded");
                entity.Property(e => e.absded).HasColumnName("absded");
                entity.Property(e => e.overtime).HasColumnName("overtime");
                entity.Property(e => e.feededuction).HasColumnName("feededuction");
                entity.Ignore(e => e.arrears);
                entity.Ignore(e => e.othded);
                entity.Property(e => e.tilldate).HasColumnName("tilldate");
                entity.Property(e => e.fromdate).HasColumnName("fromdate");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.employeeid);
            });

            modelBuilder.Entity<HRpayrollprocessdtlLegacy>(entity =>
            {
                entity.ToTable("HRpayrollprocessdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.processid).HasColumnName("processid");
                entity.Property(e => e.lovtypeid).HasColumnName("lovtypeid");
                entity.Property(e => e.typeid).HasColumnName("typeid");
                entity.Property(e => e.fromdate).HasColumnName("fromdate");
                entity.Property(e => e.tilldate).HasColumnName("tilldate");
                entity.Property(e => e.value).HasColumnName("value");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");

                entity.HasOne(e => e.PayrollProcess)
                    .WithMany(p => p.HRpayrollprocessdtls)
                    .HasForeignKey(e => e.processid);
            });

            modelBuilder.Entity<HRAtnintoutLegacy>(entity =>
            {
                entity.ToTable("HRAtnintout");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.EmpId).HasColumnName("empid");
                entity.Property(e => e.MachineId).HasColumnName("machineid");
                entity.Property(e => e.AtnTypeId).HasColumnName("atntypeid");
                entity.Property(e => e.AtnActivityId).HasColumnName("atnactivityid");
                entity.Property(e => e.Dttime).HasColumnName("dttime");
                entity.Property(e => e.OrgDttime).HasColumnName("orgdttime");
                entity.Property(e => e.ShiftId).HasColumnName("shiftid");
                entity.Property(e => e.ShiftDtlId).HasColumnName("shiftdtlid");
                entity.Property(e => e.DayId).HasColumnName("dayid");
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1500);
                entity.Property(e => e.FilePath).HasColumnName("filepath").HasMaxLength(500);
                entity.Property(e => e.PostTransId).HasColumnName("posttransid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ShiftIn).HasColumnName("shiftin");
                entity.Property(e => e.ShiftOut).HasColumnName("shiftout");
                entity.Property(e => e.SchTimeIn).HasColumnName("schtimein");
                entity.Property(e => e.SchTimeOut).HasColumnName("schtimeout");
                entity.Property(e => e.LateMin).HasColumnName("latemin");
                entity.Property(e => e.LateCount).HasColumnName("latcount");
                entity.Property(e => e.EdMin).HasColumnName("edmin");
                entity.Property(e => e.EdCount).HasColumnName("edcount");
                entity.Property(e => e.OtMin).HasColumnName("otmin");
                entity.Property(e => e.OtCount).HasColumnName("otcount");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<HRAtnactivityLegacy>(entity =>
            {
                entity.ToTable("HRAtnactivity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Stxt).HasColumnName("stxt").HasMaxLength(250);
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.OrderId).HasColumnName("orderid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
            });

            modelBuilder.Entity<UtinstanceLegacy>(entity =>
            {
                entity.ToTable("utinstance");
                entity.HasKey(e => e.instanceid);
                entity.Property(e => e.instanceid).HasColumnName("instanceid").ValueGeneratedNever();
                entity.Property(e => e.dbserver).HasColumnName("dbserver").HasMaxLength(150);
                entity.Property(e => e.dbuserid).HasColumnName("dbuserid").HasMaxLength(150);
                entity.Property(e => e.dbpassword).HasColumnName("dbpassword").HasMaxLength(150);
                entity.Property(e => e.dbname).HasColumnName("dbname").HasMaxLength(150);
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });
        
            ConfigureAuditModel(modelBuilder);
            modelBuilder.Entity<OrgDepartmentLegacy>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<OrgDesignationLegacy>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<UtLovLegacy>().Property(x => x.Id).ValueGeneratedNever();

            // Legacy databases allow NULL in text columns
            // that are declared non-nullable on the entity. EF then strips the `?? ""`
            // from projections and materialisation throws SqlNullValueException.
            // Marking every non-key string optional keeps reads safe; the database
            // still enforces its own NOT NULL constraints on write.
            foreach (var __entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var __property in __entityType.GetProperties())
                {
                    if (__property.ClrType == typeof(string) && !__property.IsKey())
                    {
                        __property.IsNullable = true;
                    }
                }
            }
        }

        private static void MapPayrollAmountDetail<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
            where TEntity : EmployeePayrollAmountDetailLegacy
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.PayrollId).HasColumnName("emppayrollid");
            entity.Property(x => x.LineIndex).HasColumnName("lineindex");
            entity.Property(x => x.Value).HasColumnName("value");
            entity.Property(x => x.StartDate).HasColumnName("startdate");
            entity.Property(x => x.EndDate).HasColumnName("enddate");
            entity.Property(x => x.UtSysGen).HasColumnName("utsysgen");
            entity.Property(x => x.UtBlock).HasColumnName("utblock");
            entity.Property(x => x.UtIsPartial).HasColumnName("utispartial");
            entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
        }

        private static void MapDesignationPayrollAmountDetail<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
            where TEntity : DesignationPayrollAmountDetailLegacy
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.PayrollId).HasColumnName("designationpayrollid");
            entity.Property(x => x.LineIndex).HasColumnName("lineindex");
            entity.Property(x => x.Value).HasColumnName("value");
            entity.Property(x => x.StartDate).HasColumnName("startdate");
            entity.Property(x => x.EndDate).HasColumnName("enddate");
            entity.Property(x => x.UtBlock).HasColumnName("utblock");
            entity.Property(x => x.UtRecordVersion).HasColumnName("utrecordversion");
        }
    }
}

