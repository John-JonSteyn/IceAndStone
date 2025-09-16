using Newtonsoft.Json;

namespace IceAndStone.App.Config
{
    public sealed class Config
    {
        [JsonProperty("apiBaseUrl")]
        public string ApiBaseUrl { get; set; } = "http://localhost:8080";

        [JsonProperty("venueId")]
        public long VenueId { get; set; } = 1;

        [JsonProperty("laneId")]
        public long LaneId { get; set; } = 1;

        [JsonProperty("appVersion")]
        public string AppVersion { get; set; } = "0.0.0";
    }
}
