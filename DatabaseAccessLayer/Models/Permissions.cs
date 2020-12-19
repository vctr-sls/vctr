namespace DatabaseAccessLayer.Models
{
    public enum Permissions
    {
        UNSET                   = -1,
        ADMINISTRATOR           = int.MaxValue, // All possible permissions
        VIEW_LINKS              = 1 << 1,
        CREATE_LINKS            = 1 << 2,
        UPDATE_LINKS            = 1 << 3,
        DELETE_LINKS            = 1 << 4,
        VIEW_USERS              = 1 << 5,
        CREATE_USERS            = 1 << 6,
        UPDATE_USERS            = 1 << 7,
        DELETE_USERS            = 1 << 8,
        PERFORM_STATE_CHANGES   = 1 << 9,
    }
}
