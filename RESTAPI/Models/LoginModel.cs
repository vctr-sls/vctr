using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RESTAPI.Models
{
    public class LoginModel
    {
        [Required]
        [JsonPropertyName("ident")]
        public string Ident { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("remember")]
        public bool Remember { get; set; }
    }
}
