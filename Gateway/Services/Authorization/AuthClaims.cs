using System;
using System.Text.Json.Serialization;

namespace Gateway.Services.Authorization
{
    public class AuthClaims
    {
        [JsonPropertyName("guid")]
        public Guid Guid { get; set; }
    }
}
