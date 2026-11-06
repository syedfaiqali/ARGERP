using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance.Data;

/// <summary>
/// Posting engine's own view of the FI tables. Never registered in DI: it is created per call by
/// <see cref="IJvPostingContextFactory"/> on the caller's connection and transaction.
/// </summary>
public sealed class JvPostingDbContext : DbContext
{
	public JvPostingDbContext(DbContextOptions<JvPostingDbContext> options) : base(options)
	{
	}

	public DbSet<FiTransactionMaster> TransactionMasters => Set<FiTransactionMaster>();
	public DbSet<FiTransactionDetail> TransactionDetails => Set<FiTransactionDetail>();
	public DbSet<FiChartOfAccount> Accounts => Set<FiChartOfAccount>();
	public DbSet<FiFiscalYear> FiscalYears => Set<FiFiscalYear>();
	public DbSet<FiPolicyHeader> Policies => Set<FiPolicyHeader>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<FiTransactionMaster>(e =>
		{
			e.ToTable("FITransactionMaster");
			e.HasKey(x => x.Id);
			e.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
			e.Property(x => x.Code).HasColumnName("code").HasMaxLength(50);
			e.Property(x => x.TransactionDate).HasColumnName("transdate");
			e.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
			e.Property(x => x.TotalAmount).HasColumnName("totalamount");
			e.Property(x => x.VoucherTypeId).HasColumnName("vouchertypeid");
			e.Property(x => x.ClientId).HasColumnName("clientid");
			e.Property(x => x.EntityId).HasColumnName("entityid");
			e.Property(x => x.FiscalYearId).HasColumnName("fiscalyearid");
			e.Property(x => x.ReferenceNo).HasColumnName("referenceno");
			e.Property(x => x.SystemGenerated).HasColumnName("utsysgen");
			e.Property(x => x.Blocked).HasColumnName("utblock");
			e.Property(x => x.IsPartial).HasColumnName("utispartial");
			e.Property(x => x.RecordVersion).HasColumnName("utrecordversion");
			e.Property(x => x.AddedOn).HasColumnName("utaddtime");
			e.Property(x => x.AddedBy).HasColumnName("utadduser").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.AddedIp).HasColumnName("utaddip").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.EditedOn).HasColumnName("utedittime");
			e.Property(x => x.EditedBy).HasColumnName("utedituser").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.EditedIp).HasColumnName("uteditip").HasMaxLength(25).IsUnicode(false);
			e.HasMany(x => x.Details).WithOne().HasForeignKey(x => x.TransactionId);
		});

		modelBuilder.Entity<FiTransactionDetail>(e =>
		{
			e.ToTable("FITransactionDetail");
			e.HasKey(x => x.Id);
			e.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
			e.Property(x => x.TransactionId).HasColumnName("transactionid");
			e.Property(x => x.ClientId).HasColumnName("clientid");
			e.Property(x => x.EntityId).HasColumnName("entityid");
			e.Property(x => x.AccountId).HasColumnName("ficoaid");
			e.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
			e.Property(x => x.Debit).HasColumnName("debit");
			e.Property(x => x.Credit).HasColumnName("credit");
			e.Property(x => x.TransType).HasColumnName("transtype");
			e.Property(x => x.SystemGenerated).HasColumnName("utsysgen");
			e.Property(x => x.Blocked).HasColumnName("utblock");
			e.Property(x => x.IsPartial).HasColumnName("utispartial");
			e.Property(x => x.RecordVersion).HasColumnName("utrecordversion");
			e.Property(x => x.AddedOn).HasColumnName("utaddtime");
			e.Property(x => x.AddedBy).HasColumnName("utadduser").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.AddedIp).HasColumnName("utaddip").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.EditedOn).HasColumnName("utedittime");
			e.Property(x => x.EditedBy).HasColumnName("utedituser").HasMaxLength(25).IsUnicode(false);
			e.Property(x => x.EditedIp).HasColumnName("uteditip").HasMaxLength(25).IsUnicode(false);
		});

		modelBuilder.Entity<FiChartOfAccount>(e =>
		{
			e.ToTable("FICOA_New");
			e.HasKey(x => x.Id);
			e.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
			e.Property(x => x.Title).HasColumnName("Title");
			e.Property(x => x.AccountCode).HasColumnName("accountcode");
			e.Property(x => x.Subsidiary).HasColumnName("Subsidiary");
			e.Property(x => x.Active).HasColumnName("Active");
			e.Property(x => x.ClientId).HasColumnName("clientid");
			e.Property(x => x.Blocked).HasColumnName("utblock");
		});

		modelBuilder.Entity<FiFiscalYear>(e =>
		{
			e.ToTable("FIFiscalYear");
			e.HasKey(x => x.Id);
			e.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
			e.Property(x => x.Code).HasColumnName("code");
			e.Property(x => x.Title).HasColumnName("title");
			e.Property(x => x.Status).HasColumnName("status");
			e.Property(x => x.StartDate).HasColumnName("startdate");
			e.Property(x => x.EndDate).HasColumnName("enddate");
			e.Property(x => x.ClientId).HasColumnName("clientid");
			e.Property(x => x.EntityId).HasColumnName("entityid");
			e.Property(x => x.Blocked).HasColumnName("utblock");
		});

		modelBuilder.Entity<FiPolicyHeader>(e =>
		{
			e.ToTable("FIPolicyhdr");
			e.HasKey(x => x.Id);
			e.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
			e.Property(x => x.ClientId).HasColumnName("clientid");
			e.Property(x => x.EntityId).HasColumnName("entityid");
			e.Property(x => x.TransactionAllowInBackDate).HasColumnName("transallowinbackdate");
			e.Property(x => x.CanChangeTransactionDate).HasColumnName("canchangetransdate");
		});
	}
}
