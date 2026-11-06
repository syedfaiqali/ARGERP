using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class RoomV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("roomtypeid")]
        public int RoomTypeId { get; set; }

        [JsonPropertyName("roomtypename")]
        public string RoomTypeName { get; set; } = string.Empty;

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }

        [JsonPropertyName("buildingname")]
        public string BuildingName { get; set; } = string.Empty;

        [JsonPropertyName("lovfloorid")]
        public int FloorId { get; set; }

        [JsonPropertyName("floorname")]
        public string FloorName { get; set; } = string.Empty;

        [JsonPropertyName("strength")]
        public int? Strength { get; set; }
    }

    public sealed class CreateRoomV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("roomtypeid")]
        public int RoomTypeId { get; set; }

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }

        [JsonPropertyName("lovfloorid")]
        public int FloorId { get; set; }

        [JsonPropertyName("strength")]
        public int? Strength { get; set; }
    }

    public sealed class UpdateRoomV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("roomtypeid")]
        public int RoomTypeId { get; set; }

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }

        [JsonPropertyName("lovfloorid")]
        public int FloorId { get; set; }

        [JsonPropertyName("strength")]
        public int? Strength { get; set; }
    }

    public sealed class RoomLookupDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string Name { get; set; } = string.Empty;
    }
}
