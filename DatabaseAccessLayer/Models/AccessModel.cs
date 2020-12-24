namespace DatabaseAccessLayer.Models
{
    /// <summary>
    /// Link access database model.
    /// </summary>
    public class AccessModel : EntityModel
    {
        public LinkModel Link { get; set; }

        public string SourceIPHash { get; set; }
    }
}
