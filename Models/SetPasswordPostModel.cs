namespace slms2asp.Models
{
    /// <summary>
    /// 
    /// HTTP post body model for setting or
    /// resetting the password of a short link.
    /// 
    /// </summary>
    public class SetPasswordPostModel
    {
        public string Password { get; set; }
        public bool Reset { get; set; }
    }
}
