using Microsoft.AspNetCore.Identity;

namespace TaskManager.APIs.DTOs
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public IList<string> RoleName { get; set; }
    }
    
}
