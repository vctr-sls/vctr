using slms2asp.Extensions;
using slms2asp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace slms2asp.Models
{
    public class ShortLinkModel
    {
        // ------------------------------------------------
        // -- Statics

        private static readonly List<string> ShortIdentBlacklist = new List<string>
        {
            "ui",
            "manage"
        };

        public static readonly Regex ShortIdentPattern = new Regex(@"^[\w-]+$");

        // ------------------------------------------------
        // -- Database Model Properties

        [Key]
        public Guid GUID { get; private set; }

        // User-Settable Properties
        public string RootURL { get; set; }
        public string ShortIdent { get; set; }
        public int MaxUses { get; set; }
        public bool IsActive { get; set; }
        public DateTime Activates { get; set; }
        public DateTime Expires { get; set; }
        public bool IsPermanentRedirect { get; set; }

        // Not User-Settable variables
        public bool IsPasswordProtected { get; set; }
        public int AccessCount { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime LastAccess { get; private set; }
        public DateTime LastModified { get; private set; }

        [IgnoreDataMember]
        public string PasswordHash { get; set; }

        // ------------------------------------------------
        // -- Initializers

        public ShortLinkModel() { }

        public ShortLinkModel(
            string rootUrl,
            string shortIdent,
            int maxUses = -1,
            bool active = true,
            DateTime activates = default,
            DateTime expires = default,
            bool permanentRedirect = true)
        {
            GUID = Guid.NewGuid();
            RootURL = rootUrl;
            ShortIdent = shortIdent;
            MaxUses = maxUses;
            IsActive = active;
            Activates = activates;
            Expires = expires == default ? DateTime.MaxValue : expires;
            IsPermanentRedirect = permanentRedirect;

            Sanitize(asNew: true);
        }

        // ------------------------------------------------
        // -- Model Functions

        /// <summary>
        /// 
        /// Sanitizes the model when it is getting passed
        /// as request body model to be saved in the
        /// database.
        /// 
        /// On invokation, the set <i>ShortIdent</i>
        /// will be lowercased.
        /// 
        /// If <i>asNew</i> is set to true, properties
        /// which are not settable by the user will be
        /// set to a default value.
        /// 
        /// </summary>
        /// <param name="asNew"></param>
        public void Sanitize(bool asNew = false)
        {
            if (asNew)
            {
                AccessCount = 0;
                CreationDate = DateTime.Now;
                LastAccess = default;
                LastModified = DateTime.Now;
            }

            ShortIdent = ShortIdent.ToLower();
        }

        /// <summary>
        /// 
        /// Returns if the set <i>RootURL</i> is
        /// a valid HTTP URI and requests to it
        /// returns a valid success response code.
        /// 
        /// </summary>
        /// <returns>is valid state</returns>
        public async Task<bool> ValidateURI() => 
            await URIValidation.Validate(RootURL);

        /// <summary>
        /// 
        /// Returns if the set <i>ShortIdent</i>
        /// is not mepty, does not consists of a
        /// blacklisted ident or matches the
        /// defined pattern of a valid short ident.
        /// 
        /// </summary>
        /// <returns>is valid state</returns>
        public bool ValidateIdent() =>
            !ShortIdent.IsEmpty() &&
            !ShortIdentBlacklist.Contains(ShortIdent) &&
            ShortIdentPattern.IsMatch(ShortIdent);

        /// <summary>
        /// 
        /// Updates user-settable properties of 
        /// the current short link model object 
        /// from another short link model object.
        /// 
        /// </summary>
        /// <param name="newShortLink">
        ///     model object to set properties from
        /// </param>
        public void Update(ShortLinkModel newShortLink)
        {
            RootURL = newShortLink.RootURL;
            ShortIdent = newShortLink.ShortIdent;
            MaxUses = newShortLink.MaxUses;
            IsActive = newShortLink.IsActive;
            Activates = newShortLink.Activates;
            Expires = newShortLink.Expires;

            LastModified = DateTime.Now;
        }
     
        /// <summary>
        /// 
        /// Increases <i>AccessCount</i> by one and
        /// sets <i>LastAccess</i> to now.
        /// 
        /// </summary>
        public void Access()
        {
            AccessCount++;
            LastAccess = DateTime.Now;
        }
    }
}
