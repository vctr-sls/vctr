using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gateway.Util
{
    public static class LinksUtil
    {
        public static readonly Regex UrlRegex = new Regex(Constants.RegexUrl);

        public static async Task<bool> ValidateDestination(string destination)
        {
            if (!UrlRegex.IsMatch(destination))
                return false;

            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            try
            {
                var res = await client.GetAsync(destination, HttpCompletionOption.ResponseHeadersRead);
                return (int)res.StatusCode < 400;
            }
            catch
            {
                return false;
            }
        }
    }
}
