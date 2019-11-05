using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace slms2asp.Shared
{
    [Serializable]
    public class IPInfoModel
    {
        public string IP;
        public string Hostname;
        public string City;
        public string Country;
        public string Region;
        public string Org;
        public string Postal;
        public string Timezone;
    }

    public class IPInfo
    {
        private static HttpClient Client = new HttpClient();

        private const string ROOT_URI = "https://ipinfo.io";

        public static async Task<IPInfoModel> GetInfo(IPAddress address, string token)
        {
            var ipv4addr = address.MapToIPv4().ToString();
            var result = await Client.GetAsync($"{ROOT_URI}/{ipv4addr}?token={token}");

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"request failed with code {result.StatusCode}");
            }

            var body = await result.Content.ReadAsStringAsync();

            var info = JsonConvert.DeserializeObject<IPInfoModel>(body);

            return info;
        }
    }
}
