using slms2asp.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace slms2asp.Models
{
    public class ShortLinkModel
    {
        [Key]
        public Guid GUID { get; private set; }

        public string RootURL { get; set; }
        public string ShortIdent { get; set; }
        public int MaxUses { get; set; }
        public bool Active { get; set; }
        public DateTime Activates { get; set; }
        public DateTime Expires { get; set; }
        public int AccessCount { get; set; }
        public DateTime CreationDate { get; private set; }
        public DateTime LastAccess { get; set; }
        public DateTime LastModified { get; set; }

        [IgnoreDataMember]
        public string PasswordHash { get; set; }

        public ShortLinkModel() { }

        public ShortLinkModel(
            string rootUrl,
            string shortIdent,
            int maxUses = -1,
            bool active = true,
            DateTime activates = default,
            DateTime expires = default)
        {
            GUID = Guid.NewGuid();
            RootURL = rootUrl;
            ShortIdent = shortIdent;
            MaxUses = maxUses;
            Active = active;
            Activates = activates;
            Expires = expires == default ? DateTime.MaxValue : expires;

            Sanitize();
        }

        public void Sanitize()
        {
            AccessCount = 0;
            CreationDate = DateTime.Now;
            LastAccess = default;
            LastModified = DateTime.Now;
        }

        public async Task<bool> ValidateURI() => 
            await URIValidation.Validate(RootURL);

        public void Update(ShortLinkModel newShortLink)
        {
            RootURL = newShortLink.RootURL;
            ShortIdent = newShortLink.ShortIdent;
            MaxUses = newShortLink.MaxUses;
            Active = newShortLink.Active;
            Activates = newShortLink.Activates;
            Expires = newShortLink.Expires;
        }
        
    }
}
