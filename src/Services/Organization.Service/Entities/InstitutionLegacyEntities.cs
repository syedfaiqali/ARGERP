using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("OrgLegalentity")]
    public class OrgLegalentityLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("entityno")]
        public string? entityno { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("stxt")]
        public string? stxt { get; set; }

        [Column("shortname")]
        public string? shortname { get; set; }

        [Column("estdate")]
        public DateTime? estdate { get; set; }

        [Column("contactperson")]
        public string? contactperson { get; set; }

        [Column("contactno")]
        public string? contactno { get; set; }

        [Column("address")]
        public string? address { get; set; }

        [Column("countryid")]
        public int? countryid { get; set; }

        [Column("stateid")]
        public int? stateid { get; set; }

        [Column("cityid")]
        public int? cityid { get; set; }

        [Column("regionid")]
        public int? regionid { get; set; }

        [Column("latitude")]
        public double? latitude { get; set; }

        [Column("longitude")]
        public double? longitude { get; set; }

        [Column("lovorgtypeid")]
        public int? lovorgtypeid { get; set; }

        [Column("lovcampustypeid")]
        public int? lovcampustypeid { get; set; }

        [Column("orgstatusid")]
        public int? orgstatusid { get; set; }

        [Column("registerid")]
        public int? registerid { get; set; }

        [Column("slogan")]
        public string? slogan { get; set; }

        [Column("institutiontypeid")]
        public int? institutiontypeid { get; set; }

        [Column("entitytypeid")]
        public int? entitytypeid { get; set; }

        [NotMapped]
        public int? headentity { get; set; }

        [Column("imageid")]
        public long? imageid { get; set; }

        [Column("watermarklogo")]
        public long? watermarklogo { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; } = false;

        [Column("utispartial")]
        public bool utispartial { get; set; } = false;

        [Column("utrecordversion")]
        public int utrecordversion { get; set; } = 0;
    }

    [Table("GNImage")]
    public class GNImageLegacy
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("filename")]
        public string? filename { get; set; }

        [Column("img")]
        public byte[]? img { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("logdate")]
        public DateTime? logdate { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }

        [Column("utispartial")]
        public bool utispartial { get; set; }

        [Column("utrecordversion")]
        public int utrecordversion { get; set; }
    }

    [Table("ORGentitycontact")]
    public class ORGentitycontactLegacy
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("stxt")]
        public string? stxt { get; set; }

        [Column("cntypid")]
        public int? cntypid { get; set; }

        [Column("value")]
        public string? value { get; set; }

        [Column("lineindex")]
        public int? lineindex { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; } = false;

        [Column("utispartial")]
        public bool utispartial { get; set; } = false;

        [Column("utrecordversion")]
        public int utrecordversion { get; set; } = 0;
    }

    [Table("OrgLegalentitydtl")]
    public class OrgLegalentitydtlLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("lineindex")]
        public int? lineindex { get; set; }

        [Column("lovtypeid")]
        public int? lovtypeid { get; set; }

        [Column("employeeid")]
        public int? employeeid { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; } = false;

        [Column("utispartial")]
        public bool utispartial { get; set; } = false;

        [Column("utrecordversion")]
        public int utrecordversion { get; set; } = 0;
    }

    [Table("OrgLegalentitypvdtl")]
    public class OrgLegalentitypvdtlLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("lineindex")]
        public int? lineindex { get; set; }

        [Column("pvendorid")]
        public int? pvendorid { get; set; }

        [Column("prefix")]
        public string? prefix { get; set; }

        [Column("paymethodid")]
        public int? paymethodid { get; set; }

        /// <summary>FK to FIBankaccountsetup (bank account selected for this payment row)</summary>
        [Column("accid")]
        public int? accid { get; set; }

        [Column("isdefault")]
        public bool isdefault { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; } = false;

        [Column("utispartial")]
        public bool utispartial { get; set; } = false;

        [Column("utrecordversion")]
        public int utrecordversion { get; set; } = 0;
    }

    [Table("PaymentVendor")]
    public class PaymentVendorLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("code")]
        public string? code { get; set; }

        [Column("stxt")]
        public string? stxt { get; set; }

        [Column("ltxt")]
        public string? ltxt { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }
    }

    /// <summary>
    /// Bank accounts available for selection on the Online Payment tab.
    /// Referenced by OrgLegalentitypvdtl.accid.
    /// </summary>
    [Table("FIBankaccountsetup")]
    public class BankAccountSetupLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("code")]
        public string? code { get; set; }

        /// <summary>Account title, used as the display name.</summary>
        [Column("title")]
        public string? title { get; set; }

        [Column("accountno")]
        public string? accountno { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("status")]
        public bool? status { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }
    }

    [Table("EREmployeeinfohdr")]
    public class EmployeeinfohdrLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("employeecode")]
        public string? employeecode { get; set; }

        [Column("firstname")]
        public string? firstname { get; set; }

        [Column("lastname")]
        public string? lastname { get; set; }

        [Column("empstatusid")]
        public int? empstatusid { get; set; }

        [Column("utblock")]
        public bool utblock { get; set; }
    }
}
