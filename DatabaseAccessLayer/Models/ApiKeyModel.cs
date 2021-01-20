using System;

namespace DatabaseAccessLayer.Models
{
    public class ApiKeyModel : EntityModel
    {
        public UserModel User { get; set; }

        public string KeyHash { get; set; }

        public DateTime LastAccess { get; set; }

        public int AccessCount { get; set; }
    }
}
