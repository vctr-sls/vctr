namespace DatabaseAccessLayer.Models
{
    public enum Permissions
    {
        ADMINISTRATOR               = 1 << 0,
        CAN_CREATE_LINKS            = 1 << 1,
        CAN_UPDATE_LINKS            = 1 << 2,
        CAN_DELETE_LINKS            = 1 << 3,
        CAN_CREATE_USERS            = 1 << 4,
        CAN_UPDATE_USERS            = 1 << 5,
        CAN_DELETE_USERS            = 1 << 6,
        CAN_PERFORM_STATE_CHANGES   = 1 << 7,
    }
}
