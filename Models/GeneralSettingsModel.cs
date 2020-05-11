﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace slms2asp.Models
{
    /// <summary>
    /// 
    /// Database model for general application settings.
    /// 
    /// </summary>
    public class GeneralSettingsModel
    {
        [Key]
        [IgnoreDataMember]
        public int Key { get; set; } = 0;

        [IgnoreDataMember]
        public string PasswordHash { get; set; }

        public string DefaultRedirect { get; set; }
    }

    /// <summary>
    /// 
    /// HTTP post body model for setting general
    /// application configuration.
    /// 
    /// </summary>
    public class GeneralSettingsPostModel
    {
        public string CurrentPassword;
        public string Password;
        public string DefaultRedirect;
    }
}
