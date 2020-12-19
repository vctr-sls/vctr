using System;
using System.Text.Json.Serialization;

namespace RESTAPI.Services.Authorization
{
    public class AuthClaims
    {
        [JsonPropertyName("guid")]
        public Guid Guid { get; set; }
    }
}
