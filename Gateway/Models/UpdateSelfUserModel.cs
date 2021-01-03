using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class UpdateSelfUserModel
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("current_password")]
        public string CurrentPassword { get; set; }

        [JsonPropertyName("new_password")]
        public string NewPassword { get; set; }
    }
}
