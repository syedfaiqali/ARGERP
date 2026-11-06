using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("EREmployeeinfohdr")]
    public class EmployeeLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("firstname")]
        public string? FirstName { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }
    }
}
