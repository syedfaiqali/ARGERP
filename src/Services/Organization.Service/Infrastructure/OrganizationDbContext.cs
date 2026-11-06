using Microsoft.EntityFrameworkCore;
using Organization.Service.Entities;

namespace Organization.Service.Infrastructure.Data
{
    public partial class OrganizationDbContext : DbContext
    {
        // Geographical entities
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<District> Districts { get; set; }

        // Organizational entities
        public DbSet<OrgEntity> OrgEntities { get; set; }
        public DbSet<OrgCampus> OrgCampuses { get; set; }
        public DbSet<OrgBuilding> OrgBuildings { get; set; }
        public DbSet<OrgFloor> OrgFloors { get; set; }
        public DbSet<OrgRoom> OrgRooms { get; set; }
        public DbSet<OrgDepartment> OrgDepartments { get; set; }
        public DbSet<OrgDesignation> OrgDesignations { get; set; }
        public DbSet<OrgCostCenter> OrgCostCenters { get; set; }
        public DbSet<OrgBusinessUnit> OrgBusinessUnits { get; set; }
        public DbSet<OrgRegion> OrgRegion { get; set; }
        public DbSet<OrgSite> OrgSites { get; set; }
        public DbSet<SignupClientHashKey> SignupClientHashKeys { get; set; }
        public DbSet<OrgRoomLegacy> OrgRoomLegacy { get; set; }
        public DbSet<OrgBuildingLegacy> OrgBuildingLegacy { get; set; }
        public DbSet<OrgFloorLegacy> OrgFloorLegacy { get; set; }
        public DbSet<AcademicRegisterLegacy> AcademicRegisterLegacy { get; set; }
        public DbSet<AcademicYearLegacy> AcademicYearLegacy { get; set; }
        public DbSet<AcademicYearDtlLegacy> AcademicYearDtlLegacy { get; set; }
        public DbSet<OrgDepartmentLegacy> OrgDepartmentLegacy { get; set; }
        public DbSet<OrgDesignationLegacy> OrgDesignationLegacy { get; set; }
        public DbSet<OrgDonorLegacy> OrgDonorLegacy { get; set; }
        public DbSet<OccupationLegacy> OccupationLegacy { get; set; }
        public DbSet<FicoaLegacy> FicoaLegacy { get; set; }
        public DbSet<ClientFeeTypeAccountLegacy> ClientFeeTypeAccounts => Set<ClientFeeTypeAccountLegacy>();
        public DbSet<Lov> Lovs { get; set; }
        public DbSet<SMAdditionalregistrationLegacy> SMAdditionalregistrationLegacy { get; set; }
        public DbSet<SMAdditionalinfoconfigLegacy> SMAdditionalinfoconfigLegacy { get; set; }
        public DbSet<OrgLegalentityLegacy> OrgLegalentityLegacy { get; set; }
        public DbSet<GNImageLegacy> GNImages { get; set; }
        public DbSet<ORGentitycontactLegacy> ORGentitycontactLegacy { get; set; }
        public DbSet<OrgLegalentitydtlLegacy> OrgLegalentitydtlLegacy { get; set; }
        public DbSet<OrgLegalentitypvdtlLegacy> OrgLegalentitypvdtlLegacy { get; set; }
        public DbSet<PaymentVendorLegacy> PaymentVendorLegacy { get; set; }
        public DbSet<BankAccountSetupLegacy> BankAccountSetupLegacy { get; set; }
        public DbSet<EmployeeinfohdrLegacy> EmployeeinfohdrLegacy { get; set; }
        public DbSet<FOPurposeLegacy> FOPurposeLegacy { get; set; }
        public DbSet<FOCalllogLegacy> FOCalllogLegacy { get; set; }
        public DbSet<FOPostaldispatchLegacy> FOPostaldispatchLegacy { get; set; }
        public DbSet<EmployeeLegacy> EmployeeLegacy { get; set; }
        public DbSet<PublishDocumentHdrLegacy> PublishDocumentHdrLegacy { get; set; }
        public DbSet<PublishDocumentDtlLegacy> PublishDocumentDtlLegacy { get; set; }
        public DbSet<CampusStudentInfoLegacy> CampusStudents { get; set; }
        public DbSet<CampusStudentWithdrawalLegacy> CampusStudentWithdrawals { get; set; }
        public DbSet<CampusGuardianLegacy> CampusGuardians { get; set; }
        public DbSet<CampusStudentGuardianLegacy> CampusStudentGuardians { get; set; }
        public DbSet<CampusAdmissionEnquiryLegacy> CampusAdmissionEnquiries { get; set; }
        public DbSet<CampusAdmissionEnquiryDetailLegacy> CampusAdmissionEnquiryDetails { get; set; }
        public DbSet<CampusAdmissionApplicationLegacy> CampusAdmissionApplications { get; set; }
        public DbSet<CampusCourseLegacy> CampusCourses { get; set; }
        public DbSet<CampusCourseExtendLegacy> CampusCourseExtends { get; set; }
        public DbSet<ClientStudentAttendanceLegacy> ClientStudentAttendance { get; set; }
        public DbSet<ClientEmployeeAttendanceLegacy> ClientEmployeeAttendance { get; set; }
        public DbSet<ClientFeeCollectionLegacy> ClientFeeCollections { get; set; }
        public DbSet<ClientFeeCollectionDetailLegacy> ClientFeeCollectionDetails { get; set; }
        public DbSet<ClientFeeChallanLegacy> ClientFeeChallans { get; set; }
        public DbSet<ClientFinancialTransactionMasterLegacy> ClientFinancialTransactions { get; set; }
        public DbSet<ClientFinancialTransactionDetailLegacy> ClientFinancialTransactionDetails { get; set; }


        // Reference entities
        public DbSet<OrgLanguage> OrgLanguages { get; set; }
        public DbSet<OrgCaste> OrgCastes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Country
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.CountryName).HasColumnName("countryname").IsRequired().HasMaxLength(100);
                entity.Property(e => e.CountryCode).HasColumnName("countrycode").IsRequired().HasMaxLength(2);
                entity.Property(e => e.CountryCode3).HasColumnName("isocode").HasMaxLength(5);
                entity.Property(e => e.DialCode).HasColumnName("dialcode").HasMaxLength(10);
                entity.Property(e => e.UtBlock).HasColumnName("utblock");

                entity.HasIndex(e => e.CountryCode).IsUnique();
            });

            // Configure State
            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.stxt).HasColumnName("stxt").IsRequired().HasMaxLength(150);
                entity.Property(e => e.StateCode).HasColumnName("statecode").HasMaxLength(5);
                entity.Property(e => e.CountryId).HasColumnName("countryid");

                entity.HasOne(e => e.Country)
                    .WithMany(c => c.States)
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CountryId);
            });

            // Configure City
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Citystxt).HasColumnName("citystxt").IsRequired().HasMaxLength(100);
                entity.Property(e => e.CityCode).HasColumnName("citycode").HasMaxLength(10);
                entity.Property(e => e.DialCode).HasColumnName("dialcode").HasMaxLength(10);
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.StateId).HasColumnName("stateid");
                entity.Property(e => e.CountryId).HasColumnName("ctryid");

                entity.HasOne(e => e.State)
                    .WithMany(s => s.Cities)
                    .HasForeignKey(e => e.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Country)
                    .WithMany()
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.StateId);
                entity.HasIndex(e => e.CountryId);
            });

            // Configure District
            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.DistrictId);
                entity.Property(e => e.DistrictName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DistrictCode).HasMaxLength(10);

                entity.HasOne(e => e.City)
                    .WithMany(c => c.Districts)
                    .HasForeignKey(e => e.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CityId);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure OrgEntity
            modelBuilder.Entity<OrgEntity>(entity =>
            {
                entity.HasKey(e => e.EntityId);
                entity.Property(e => e.EntityCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.EntityType).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.EmailAddress).HasMaxLength(100);
                entity.Property(e => e.WebsiteUrl).HasMaxLength(200);
                entity.Property(e => e.LogoUrl).HasMaxLength(200);

                entity.HasIndex(e => new { e.ClientId, e.EntityCode }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Country).WithMany().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.State).WithMany().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.City).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrgCampus
            modelBuilder.Entity<OrgCampus>(entity =>
            {
                entity.HasKey(e => e.CampusId);
                entity.Property(e => e.CampusCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CampusName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.CampusType).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.EmailAddress).HasMaxLength(100);
                entity.Property(e => e.PrincipalName).HasMaxLength(100);
                entity.Property(e => e.PrincipalEmail).HasMaxLength(100);
                entity.Property(e => e.PrincipalPhone).HasMaxLength(20);

                entity.HasIndex(e => new { e.ClientId, e.CampusCode }).IsUnique();
                entity.HasIndex(e => e.EntityId);
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Entity)
                    .WithMany(e => e.Campuses)
                    .HasForeignKey(e => e.EntityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Country).WithMany().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.State).WithMany().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.City).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrgBuilding
            modelBuilder.Entity<OrgBuilding>(entity =>
            {
                entity.HasKey(e => e.BuildingId);
                entity.Property(e => e.BuildingCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BuildingName).IsRequired().HasMaxLength(150);

                entity.HasIndex(e => new { e.CampusId, e.BuildingCode }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Campus)
                    .WithMany(c => c.Buildings)
                    .HasForeignKey(e => e.CampusId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrgFloor
            modelBuilder.Entity<OrgFloor>(entity =>
            {
                entity.HasKey(e => e.FloorId);
                entity.Property(e => e.FloorName).HasMaxLength(100);

                entity.HasIndex(e => new { e.BuildingId, e.FloorNumber }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Building)
                    .WithMany(b => b.Floors)
                    .HasForeignKey(e => e.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrgRoom
            modelBuilder.Entity<OrgRoom>(entity =>
            {
                entity.HasKey(e => e.RoomId);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RoomName).HasMaxLength(150);
                entity.Property(e => e.RoomType).HasMaxLength(50);

                entity.HasIndex(e => new { e.FloorId, e.RoomNumber }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Floor)
                    .WithMany(f => f.Rooms)
                    .HasForeignKey(e => e.FloorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrgDepartment
            modelBuilder.Entity<OrgDepartment>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);
                entity.Property(e => e.DepartmentCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DepartmentName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DepartmentType).HasMaxLength(50);
                entity.Property(e => e.HeadName).HasMaxLength(100);
                entity.Property(e => e.HeadEmail).HasMaxLength(100);
                entity.Property(e => e.HeadPhone).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasIndex(e => new { e.ClientId, e.DepartmentCode }).IsUnique();
                entity.HasIndex(e => e.CampusId);
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Campus)
                    .WithMany(c => c.Departments)
                    .HasForeignKey(e => e.CampusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrgDesignation
            modelBuilder.Entity<OrgDesignation>(entity =>
            {
                entity.HasKey(e => e.DesignationId);
                entity.Property(e => e.DesignationCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DesignationName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DesignationType).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasIndex(e => new { e.ClientId, e.DesignationCode }).IsUnique();
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Designations)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReportingToDesignation)
                    .WithMany()
                    .HasForeignKey(e => e.ReportingToDesignationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrgCostCenter
            modelBuilder.Entity<OrgCostCenter>(entity =>
            {
                entity.HasKey(e => e.CostCenterId);
                entity.Property(e => e.CostCenterCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CostCenterName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.BudgetAmount).HasPrecision(18, 2);
                entity.Property(e => e.ManagerName).HasMaxLength(100);
                entity.Property(e => e.ManagerEmail).HasMaxLength(100);

                entity.HasIndex(e => new { e.ClientId, e.CostCenterCode }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure OrgBusinessUnit
            modelBuilder.Entity<OrgBusinessUnit>(entity =>
            {
                entity.HasKey(e => e.BusinessUnitId);
                entity.Property(e => e.BusinessUnitCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BusinessUnitName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.BusinessUnitType).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.HeadName).HasMaxLength(100);
                entity.Property(e => e.HeadEmail).HasMaxLength(100);

                entity.HasIndex(e => new { e.ClientId, e.BusinessUnitCode }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure OrgRegion
            modelBuilder.Entity<OrgRegion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.stxt).IsRequired().HasMaxLength(150);
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");
                //entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasIndex(e => new { e.ClientId, e.Code }).IsUnique();
                entity.HasIndex(e => e.CountryId);
                //entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.Country)
                    .WithMany()
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrgSite (legacy table: Orgsite)
            modelBuilder.Entity<OrgSite>(entity =>
            {
                entity.ToTable("Orgsite");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.SiteName).HasColumnName("stxt").HasMaxLength(500);
                entity.Property(e => e.SiteTypeId).HasColumnName("sitetypeid");
                entity.Property(e => e.RegionId).HasColumnName("regionid");
                entity.Property(e => e.SiteForId).HasColumnName("siteforid");
                entity.Property(e => e.ForId).HasColumnName("forid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.ParentSiteId).HasColumnName("siteid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");

                entity.HasIndex(e => new { e.ClientId, e.EntityId, e.Code });
                entity.HasIndex(e => new { e.ClientId, e.EntityId, e.SiteName });
                entity.HasIndex(e => e.RegionId);

                entity.HasOne(e => e.Region)
                    .WithMany()
                    .HasForeignKey(e => e.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ParentSite)
                    .WithMany()
                    .HasForeignKey(e => e.ParentSiteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SignupClientHashKey>(entity =>
            {
                entity.ToTable("SignupClientHashKeys");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.HashKey).IsRequired().HasMaxLength(128);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasIndex(e => e.ClientId).IsUnique();
                entity.HasIndex(e => e.HashKey).IsUnique();
            });

            // Configure legacy Room/Building/Floor tables used by v3 Room APIs
            modelBuilder.Entity<OrgRoomLegacy>(entity =>
            {
                entity.ToTable("Orgroom");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.lovfloorid).HasColumnName("lovfloorid");
                entity.Property(e => e.roomtypeid).HasColumnName("roomtypeid");
                entity.Property(e => e.buildingid).HasColumnName("buildingid");
                entity.Property(e => e.strength).HasColumnName("strength");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");

                entity.HasIndex(e => new { e.clientid, e.entityid, e.code });
                entity.HasIndex(e => new { e.clientid, e.entityid, e.buildingid, e.lovfloorid, e.stxt });
            });

            modelBuilder.Entity<OrgBuildingLegacy>(entity =>
            {
                entity.ToTable("OrgBuilding");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<OrgFloorLegacy>(entity =>
            {
                entity.ToTable("OrgFloor");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.buildingid).HasColumnName("buildingid");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<OrgDepartmentLegacy>(entity =>
            {
                entity.ToTable("OrgDepartment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.depcode).HasColumnName("depcode").HasMaxLength(10);
                entity.Property(e => e.depstxt).HasColumnName("depstxt").HasMaxLength(50);
                entity.Property(e => e.depltxt).HasColumnName("depltxt").HasMaxLength(500);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.ficoaid).HasColumnName("ficoaid");
                entity.Property(e => e.ficoaid_rec).HasColumnName("ficoaid_rec");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OrgDesignationLegacy>(entity =>
            {
                entity.ToTable("OrgDesignation");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.desgcode).HasColumnName("desgcode").HasMaxLength(5);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.blank).HasColumnName("blank");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OrgDonorLegacy>(entity =>
            {
                entity.ToTable("OrgDonor");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(50);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(50);
                entity.Property(e => e.contactno).HasColumnName("contactno").HasMaxLength(50);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<SMAdditionalregistrationLegacy>(entity =>
            {
                entity.ToTable("SMAdditionalregistration");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.caption).HasColumnName("caption").HasMaxLength(200);
                entity.Property(e => e.configid).HasColumnName("configid");
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.hide).HasColumnName("hide");
                entity.Property(e => e.infoforid).HasColumnName("infoforid");
                entity.Property(e => e.sequance).HasColumnName("sequance");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<SMAdditionalinfoconfigLegacy>(entity =>
            {
                entity.ToTable("SMAdditionalinfoconfig");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.indx).HasColumnName("indx");
                entity.Property(e => e.webicon).HasColumnName("webicon").HasMaxLength(100);
                entity.Property(e => e.route).HasColumnName("route").HasMaxLength(200);
                entity.Property(e => e.visible).HasColumnName("visible");
                entity.Property(e => e._class).HasColumnName("class").HasMaxLength(100);
                entity.Property(e => e.grponid).HasColumnName("grponid");
                entity.Property(e => e.infoforid).HasColumnName("infoforid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<OccupationLegacy>(entity =>
            {
                entity.ToTable("Occupation");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(150);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<GNImageLegacy>(entity =>
            {
                entity.ToTable("GNImage");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.filename).HasColumnName("filename").HasMaxLength(500);
                entity.Property(e => e.img).HasColumnName("img").HasColumnType("image");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.logdate).HasColumnName("logdate");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
            });

            modelBuilder.Entity<OrgLegalentityLegacy>(entity =>
            {
                entity.ToTable("OrgLegalentity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.entityno).HasColumnName("entityno").HasMaxLength(20);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(500);
                entity.Property(e => e.shortname).HasColumnName("shortname").HasMaxLength(50);
                entity.Property(e => e.estdate).HasColumnName("estdate");
                entity.Property(e => e.contactperson).HasColumnName("contactperson").HasMaxLength(200);
                entity.Property(e => e.contactno).HasColumnName("contactno").HasMaxLength(50);
                entity.Property(e => e.address).HasColumnName("address").HasMaxLength(500);
                entity.Property(e => e.countryid).HasColumnName("countryid");
                entity.Property(e => e.stateid).HasColumnName("stateid");
                entity.Property(e => e.cityid).HasColumnName("cityid");
                entity.Property(e => e.regionid).HasColumnName("regionid");
                entity.Property(e => e.latitude).HasColumnName("latitude");
                entity.Property(e => e.longitude).HasColumnName("longitude");
                entity.Property(e => e.lovorgtypeid).HasColumnName("lovorgtypeid");
                entity.Property(e => e.lovcampustypeid).HasColumnName("lovcampustypeid");
                entity.Property(e => e.orgstatusid).HasColumnName("orgstatusid");
                entity.Property(e => e.registerid).HasColumnName("registerid");
                entity.Property(e => e.slogan).HasColumnName("slogan").HasMaxLength(500);
                entity.Property(e => e.institutiontypeid).HasColumnName("institutiontypeid");
                entity.Property(e => e.entitytypeid).HasColumnName("entitytypeid");
                entity.Property(e => e.imageid).HasColumnName("imageid");
                entity.Property(e => e.watermarklogo).HasColumnName("watermarklogo");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasIndex(e => new { e.clientid, e.entityno });
                entity.HasIndex(e => new { e.clientid, e.stxt });
            });

            modelBuilder.Entity<ORGentitycontactLegacy>(entity =>
            {
                entity.ToTable("ORGentitycontact");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.cntypid).HasColumnName("cntypid");
                entity.Property(e => e.value).HasColumnName("value").HasMaxLength(200);
                entity.Property(e => e.lineindex).HasColumnName("lineindex");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasIndex(e => e.entityid);
            });

            modelBuilder.Entity<OrgLegalentitydtlLegacy>(entity =>
            {
                entity.ToTable("OrgLegalentitydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.lineindex).HasColumnName("lineindex");
                entity.Property(e => e.lovtypeid).HasColumnName("lovtypeid");
                entity.Property(e => e.employeeid).HasColumnName("employeeid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasIndex(e => e.entityid);
            });

            modelBuilder.Entity<OrgLegalentitypvdtlLegacy>(entity =>
            {
                entity.ToTable("OrgLegalentitypvdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.lineindex).HasColumnName("lineindex");
                entity.Property(e => e.pvendorid).HasColumnName("pvendorid");
                entity.Property(e => e.prefix).HasColumnName("prefix").HasMaxLength(50);
                entity.Property(e => e.paymethodid).HasColumnName("paymethodid");
                entity.Property(e => e.accid).HasColumnName("accid");
                entity.Property(e => e.isdefault).HasColumnName("isdefault");
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.utsysgen).HasColumnName("utsysgen");
                entity.Property(e => e.utispartial).HasColumnName("utispartial");
                entity.Property(e => e.utrecordversion).HasColumnName("utrecordversion");
                entity.HasIndex(e => e.entityid);
            });

            modelBuilder.Entity<PaymentVendorLegacy>(entity =>
            {
                entity.ToTable("PaymentVendor");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<BankAccountSetupLegacy>(entity =>
            {
                entity.ToTable("FIBankaccountsetup");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(100);
                entity.Property(e => e.title).HasColumnName("title").HasMaxLength(100);
                entity.Property(e => e.accountno).HasColumnName("accountno").HasMaxLength(100);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            modelBuilder.Entity<EmployeeinfohdrLegacy>(entity =>
            {
                entity.ToTable("EREmployeeinfohdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.entityid).HasColumnName("entityid");
                entity.Property(e => e.firstname).HasColumnName("firstname").HasMaxLength(100);
                entity.Property(e => e.lastname).HasColumnName("lastname").HasMaxLength(100);
                entity.Property(e => e.empstatusid).HasColumnName("empstatusid");
                entity.Property(e => e.utblock).HasColumnName("utblock");
            });

            // EmployeeLegacy shares the same legacy table as EmployeeinfohdrLegacy.
            // Configure explicit table-splitting link to avoid EF shared-table mapping errors.
            modelBuilder.Entity<EmployeeLegacy>(entity =>
            {
                entity.ToTable("EREmployeeinfohdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.FirstName).HasColumnName("firstname").HasMaxLength(100);
                entity.Property(e => e.ClientId).HasColumnName("clientid");

                entity.HasOne<EmployeeinfohdrLegacy>()
                    .WithOne()
                    .HasForeignKey<EmployeeLegacy>(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CampusStudentInfoLegacy>(entity =>
            {
                entity.ToTable("SMStudentinfo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.RegDate).HasColumnName("regdate");
                entity.Property(e => e.StatusId).HasColumnName("lovstatusid");
                entity.Property(e => e.GenderId).HasColumnName("genderid");
                entity.Property(e => e.CurrentCourseId).HasColumnName("curcourseid");
                entity.Property(e => e.AppId).HasColumnName("appid");
            });

            modelBuilder.Entity<CampusStudentWithdrawalLegacy>(entity =>
            {
                entity.ToTable("SMStudentwithdrawal");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StudentId).HasColumnName("studentid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.LeavingDate).HasColumnName("leavingdate");
                entity.Property(e => e.StatusId).HasColumnName("lovstatusid");
            });

            modelBuilder.Entity<CampusGuardianLegacy>(entity =>
            {
                entity.ToTable("SMGuardian");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
            });

            modelBuilder.Entity<CampusStudentGuardianLegacy>(entity =>
            {
                entity.ToTable("SMStudentguardian");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StudentId).HasColumnName("studentid");
                entity.Property(e => e.GuardianId).HasColumnName("guardianid");
            });

            modelBuilder.Entity<CampusAdmissionEnquiryLegacy>(entity =>
            {
                entity.ToTable("FOAdmEnquiry");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
            });

            modelBuilder.Entity<CampusAdmissionEnquiryDetailLegacy>(entity =>
            {
                entity.ToTable("FOAdmEnquirydtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.EnquiryId).HasColumnName("enquiryid");
            });

            modelBuilder.Entity<CampusAdmissionApplicationLegacy>(entity =>
            {
                entity.ToTable("SMAdmApplication");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EnquiryDetailId).HasColumnName("enquirydtlid");
                entity.Property(e => e.AppDate).HasColumnName("appdate");
            });

            modelBuilder.Entity<CampusCourseLegacy>(entity =>
            {
                entity.ToTable("SMCourse");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("stxt");
                entity.Property(e => e.Index).HasColumnName("indx");
            });

            modelBuilder.Entity<CampusCourseExtendLegacy>(entity =>
            {
                entity.ToTable("SMCourseextend");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.CourseId).HasColumnName("courseid");
            });

            // Configure LOV (legacy lookup table)
            modelBuilder.Entity<Lov>(entity =>
            {
                entity.ToTable("lov");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(200);
                entity.Property(e => e.ltxt).HasColumnName("ltxt").HasMaxLength(500);
                entity.Property(e => e.plovid).HasColumnName("plovid");
                entity.Property(e => e.vtype).HasColumnName("vtype").HasMaxLength(20);
                entity.Property(e => e.utblock).HasColumnName("utblock");
                entity.Property(e => e.indx).HasColumnName("indx");
                entity.HasIndex(e => e.vtype);
            });

            // Configure OrgLanguage
            modelBuilder.Entity<OrgLanguage>(entity =>
            {
                entity.HasKey(e => e.LanguageId);
                entity.Property(e => e.LanguageName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(5);

                entity.HasIndex(e => e.LanguageCode).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure OrgCaste
            modelBuilder.Entity<OrgCaste>(entity =>
            {
                entity.HasKey(e => e.CasteId);
                entity.Property(e => e.CasteName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CasteCode).HasMaxLength(20);

                entity.HasIndex(e => e.CasteCode).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure FOPostaldispatchLegacy (legacy table: FOPostaldispatch)
            // shared by Postal Dispatch (postaltypeid=226) and Postal Receive (postaltypeid=227)
            modelBuilder.Entity<FOPostaldispatchLegacy>(entity =>
            {
                entity.ToTable("FOPostaldispatch");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PostalNo).HasColumnName("postalno").HasMaxLength(50);
                entity.Property(e => e.PostalDate).HasColumnName("postaldate");
                entity.Property(e => e.PostalTypeId).HasColumnName("postaltypeid");
                entity.Property(e => e.CourierId).HasColumnName("courierid");
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(250);
                entity.Property(e => e.FromPerson).HasColumnName("fromperson").HasMaxLength(250);
                entity.Property(e => e.ToPerson).HasColumnName("toperson").HasMaxLength(250);
                entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
                entity.Property(e => e.RefNo).HasColumnName("refno").HasMaxLength(100);
                entity.Property(e => e.Attention).HasColumnName("attention").HasMaxLength(250);
                entity.Property(e => e.CellNo).HasColumnName("Cellno").HasMaxLength(20);
                entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(1500);
                entity.Property(e => e.EntityId).HasColumnName("entityid");
                entity.Property(e => e.ClientId).HasColumnName("clientid");
                entity.Property(e => e.UtBlock).HasColumnName("utblock");
                entity.Property(e => e.UtIsPartial).HasColumnName("utispartial");
                entity.Property(e => e.UtSysGen).HasColumnName("utsysgen");
                entity.Property(e => e.UtRecordVersion).HasColumnName("utrecordversion");

                entity.HasOne(e => e.Courier)
                    .WithMany()
                    .HasForeignKey(e => e.CourierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ClientId);
                entity.HasIndex(e => e.PostalTypeId);
                entity.HasIndex(e => e.PostalDate);
            });

            // Configure PublishDocumentHdrLegacy
            modelBuilder.Entity<PublishDocumentHdrLegacy>(entity =>
            {
                entity.ToTable("SMPublishdocumenthdr");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.code).HasColumnName("code").HasMaxLength(10);
                entity.Property(e => e.stxt).HasColumnName("stxt").HasMaxLength(100);
                entity.Property(e => e.clientid).HasColumnName("clientid");
                entity.Property(e => e.lovsharedid).HasColumnName("lovsharedid");
                entity.Property(e => e.doctypeid).HasColumnName("doctypeid");
                entity.Property(e => e.lovdoccategoryid).HasColumnName("lovdoccategoryid");
                entity.Property(e => e.validfrom).HasColumnName("validfrom");
                entity.Property(e => e.validtill).HasColumnName("validtill");
                entity.Property(e => e.publishdate).HasColumnName("publishdate");
                entity.Property(e => e.subjectid).HasColumnName("subjectid");
                entity.Property(e => e.courseid).HasColumnName("courseid");
                entity.Property(e => e.acdyearid).HasColumnName("acdyearid");

                entity.HasMany(e => e.SMPublishdocumentdtls)
                    .WithOne(d => d.PublishDocumentHdr)
                    .HasForeignKey(d => d.publishid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PublishDocumentDtlLegacy
            modelBuilder.Entity<PublishDocumentDtlLegacy>(entity =>
            {
                entity.ToTable("SMPublishdocumentdtl");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.publishid).HasColumnName("publishid");
                entity.Property(e => e.lovshareid).HasColumnName("lovshareid");
                entity.Property(e => e.pclientid).HasColumnName("pclientid");
                entity.Property(e => e.pentityid).HasColumnName("pentityid");
                entity.Property(e => e.regionid).HasColumnName("regionid");
                entity.Property(e => e.camplvelid).HasColumnName("camplvelid");
                entity.Property(e => e.courseid).HasColumnName("courseid");
            });
        
            ConfigureAuditModel(modelBuilder);
            foreach (var type in modelBuilder.Model.GetEntityTypes())
                foreach (var property in type.GetProperties().Where(p => p.IsPrimaryKey() && p.ClrType == typeof(int)))
                    property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;

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
    }
}


