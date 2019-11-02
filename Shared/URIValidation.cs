using slms2asp.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace slms2asp.Shared
{
    public class URIValidation
    {
        public static async Task<bool> Validate(string uri)
        {
            if (uri.IsEmpty() || !uri.StartsWith("http"))
            {
                return false;
            }

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(5);

                try
                {
                    var res = await client.GetAsync(uri);

                    if (!res.IsSuccessStatusCode)
                    {
                        return false;
                    }
                } 
                catch(Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
