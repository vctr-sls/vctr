using DatabaseAccessLayer.Models;
using System;
using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class ApiKeyViewModel : EntityModel
    {
        [JsonPropertyName("last_access")]
        public DateTime LastAccess { get; set; }

        [JsonPropertyName("access_count")]
        public int AccessCount { get; set; }

        public ApiKeyViewModel()
        { }

        public ApiKeyViewModel(ApiKeyModel model) : base(model)
        {
            LastAccess = model.LastAccess;
            AccessCount = model.AccessCount;
        }
    }

    public class ApiKeyCreatedViewModel : ApiKeyViewModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        public ApiKeyCreatedViewModel()
        { }

        public ApiKeyCreatedViewModel(ApiKeyModel model, string key) : base(model)
        {
            Key = key;
        }
    }
}
