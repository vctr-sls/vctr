using DatabaseAccessLayer.Models;
using System;
using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class LinkViewModel : EntityModel
    {
        [JsonPropertyName("ident")]
        public string Ident { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("creator")]
        public UserViewModel Creator { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("permanent_redirect")]
        public bool PermanentRedirect { get; set; }

        [JsonPropertyName("password_required")]
        public bool PasswordRequired { get; set; }

        [JsonPropertyName("last_access")]
        public DateTime LastAccess { get; set; }

        [JsonPropertyName("access_count")]
        public int AccessCount { get; set; }

        [JsonPropertyName("unique_access_count")]
        public int UniqueAccessCount { get; set; }

        [JsonPropertyName("total_access_limit")]
        public int TotalAccessLimit { get; set; }

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }

        public LinkViewModel()
        { }

        public LinkViewModel(LinkModel model, bool hydrateUser = false) : base(model) =>
            Hydrate(model, hydrateUser);

        public LinkViewModel(LinkModel model, UserModel authorizedUser) : base(model) =>
            Hydrate(model, authorizedUser.HasPermissions(Permissions.VIEW_USERS) || model.Creator.Guid == authorizedUser.Guid);

        private void Hydrate(LinkModel model, bool hydrateUser)
        {
            Ident = model.Ident;
            Destination = model.Destination;
            Enabled = model.Enabled;
            PermanentRedirect = model.PermanentRedirect;
            PasswordRequired = !string.IsNullOrEmpty(model.PasswordHash);
            LastAccess = model.LastAccess;
            AccessCount = model.AccessCount;
            UniqueAccessCount = model.UniqueAccessCount;
            TotalAccessLimit = model.TotalAccessLimit;
            Expires = model.Expires;

            if (hydrateUser)
                Creator = new UserViewModel(model.Creator);
        }
    }
}
