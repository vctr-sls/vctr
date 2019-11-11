using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Models
{
    public class ProtectedPostModel
    {
        public string Password;
        public bool DisableTracking;
    }

    public class ProtectedResponseModel
    {
        public string RootURL;
    }
}
