using DatabaseAccessLayer.Models;
using System;
using System.Text.Json.Serialization;

namespace Gateway.Models
{
    public class UserViewModel : EntityModel
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; set; }

        [JsonPropertyName("last_login")]
        public DateTime LastLogin { get; set; }

        public UserViewModel()
        { }

        public UserViewModel(UserModel model) : base(model)
        {
            UserName = model.UserName;
            Permissions = model.Permissions;
            LastLogin = model.LastLogin;
        }
    }

    public class UserLoginViewModel : UserViewModel
    {
        [JsonPropertyName("session_key")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SessionKey { get; set; }

        public UserLoginViewModel() 
        { }

        public UserLoginViewModel(UserModel model) : base(model) 
        { }
    }
}
