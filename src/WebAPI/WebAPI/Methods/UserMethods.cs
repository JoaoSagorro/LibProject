using EFLibrary.Models;

namespace WebAPI.Methods
{
    public class UserMethods
    {
        public class LoginDTO
        {
            public string Email { get; set; }
            public string Password { get; set; }
            
            public LoginDTO(string email, string password)
            {
                Email = email;
                Password = password;
            }
            public static User 

        }
    }
}
