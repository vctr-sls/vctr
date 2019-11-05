using System;
using System.ComponentModel.DataAnnotations;

namespace slms2asp.Database
{
    /// <summary>
    /// 
    /// Database model for access log.
    /// 
    /// </summary>
    public class AccessModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserAgent { get; set; }
        public bool IsUnique { get; set; }
        public string City {get; set; }
        public string Country { get; set; }
        public string Region {get; set; }
        public string Org {get; set; }
        public string Postal {get; set; }
        public string Timezone {get; set; }
    }
}
