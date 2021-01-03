namespace Gateway
{
    public static class Constants
    {
        public const string RegexUserName = @"[a-zA-Z0-9_\-]+";
        public const string RegexLinkIdent = @"[a-zA-Z0-9_\-]+";
        public const string RegexUrl = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";

        public const string RandomIdentChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
        public const int RandomIdentLength = 6;

        public const string SessionCookieName = "__vctr_session";

        public const string ConfigKeyPasswordHashingDegreeOfParallelism = "PasswordHashing:DegreeOfParallelism";
        public const string ConfigKeyPasswordHashingMemoryPoolKB = "PasswordHashing:MemoryPoolKB";
        public const string ConfigKeyPasswordHashingIterations = "PasswordHashing:Iterations";
        public const string ConfigKeyPasswordHashingSaltLength = "PasswordHashing:SaltLength";
        public const string ConfigKeyPasswordHashingKeyLength = "PasswordHashing:KeyLength";

        public const string ConfigKeyInitializationUserName = "Initialization:RootUserName";
        public const string ConfigKeyInitializationPassword = "Initialization:RootUserPassword";

        public const string ConfigKeySessionsJwtSecret = "Sessions:JWTSecret";
        public const string ConfigKeySessionsBypassSecureCookies = "Sessions:BypassSecureCookies";

        public const string ConfigKeyCacheDurationsLinks = "Caching:Durations:Links";

        public const string ConfigKeyRoutingRoot = "Routing:Root";
        public const string ConfigKeyRoutingNotFound = "Routing:NotFound";
        public const string ConfigKeyRoutingPassword = "Routing:Password";
    }
}
