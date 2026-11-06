using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class ETHolidaytypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        [MaxLength(50)]
        public string? Colorcode { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }
}
