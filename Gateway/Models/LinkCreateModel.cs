using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class LinkCreateModel
    {
        [StringLength(256, MinimumLength = 2)]
        [JsonPropertyName("ident")]
        public string Ident { get; set; }

        [Required]
        [StringLength(10240)]
        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("permanent_redirect")]
        public bool PermanentRedirect { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("total_access_limit")]
        public int TotalAccessLimit { get; set; }

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}
