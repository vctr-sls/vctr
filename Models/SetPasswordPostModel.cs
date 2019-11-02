using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Models
{
    public class SetPasswordPostModel
    {
        public string Password { get; set; }
        public bool Reset { get; set; }
    }
}
