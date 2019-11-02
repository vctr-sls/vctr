using System;
using System.ComponentModel.DataAnnotations;

namespace slms2asp.Database
{
    public class AccessModel
    {
        [Key]
        public int Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string UserAgent { get; private set; }
    }
}
