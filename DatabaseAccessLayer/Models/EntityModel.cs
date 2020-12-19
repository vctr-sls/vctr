using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DatabaseAccessLayer.Models
{
    public class EntityModel
    {
        [Key]
        [JsonPropertyName("guid")]
        public Guid Guid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        public EntityModel()
        {
            Guid = Guid.NewGuid();
            Created = DateTime.Now;
        }

        public EntityModel(EntityModel model)
        {
            Guid = model.Guid;
            Created = model.Created;
        }
    }
}
