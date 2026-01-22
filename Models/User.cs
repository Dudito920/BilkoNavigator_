using Microsoft.AspNetCore.Identity;

namespace BilkoNavigator_.Models
{
    public class User : IdentityUser
    {
        public string UserStatus { get; set; } = "Active"; // Set default value
        public int password { get; set; };
    }
}
