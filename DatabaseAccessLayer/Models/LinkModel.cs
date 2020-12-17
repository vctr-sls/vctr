﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccessLayer.Models
{
    public class LinkModel : EntityModel
    {
        [StringLength(256, MinimumLength = 6)]
        public string Ident { get; set; }

        [StringLength(10240)]
        public string Destination { get; set; }
        public UserModel Creator { get; set; }

        public bool Enabled { get; set; }

        public bool PermanentRedirect { get; set; }

        public string Password { get; set; }

        public DateTime LastAccess { get; set; }

        public int AccessCount { get; set; }

        public int UniqueAccessCount { get; set; }

        public int TotalAccessLimit { get; set; }

        public DateTime Expires { get; set; }
    }
}
