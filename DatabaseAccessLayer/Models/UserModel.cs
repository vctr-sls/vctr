﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccessLayer.Models
{
    /// <summary>
    /// User database model.
    /// </summary>
    public class UserModel : EntityModel
    {
        [StringLength(128, MinimumLength = 4)]
        public string UserName { get; set; }

        [StringLength(512, MinimumLength = 6)]
        public string MailAddress { get; set; }

        public string PasswordHash { get; set; }

        public Permissions Permissions { get; set; }

        public DateTime LastLogin  { get; set; }

        /// <summary>
        /// Returns true when the user has all of the given
        /// permissions.
        /// </summary>
        /// <param name="permissions">permissions summ</param>
        /// <returns></returns>
        public bool HasPermissions(Permissions permissions) =>
            (Permissions & permissions) == permissions;
    }
}
