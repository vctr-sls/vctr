using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class ResponseErrorModel
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        public ResponseErrorModel(string error) => Error = error;
    }
}
