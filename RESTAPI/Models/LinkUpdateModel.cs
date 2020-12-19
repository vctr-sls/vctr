using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RESTAPI.Models
{
    public class LinkUpdateModel : LinkCreateModel
    {
        [StringLength(10240)]
        [JsonPropertyName("destination")]
        public new string Destination { get; set; }
    }
}
