namespace DatabaseAccessLayer.Models
{
    public class AccessModel : EntityModel
    {
        public LinkModel Link { get; set; }

        public string SourceIPHash { get; set; }
    }
}
