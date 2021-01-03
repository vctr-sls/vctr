using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class CountResponseModel
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
