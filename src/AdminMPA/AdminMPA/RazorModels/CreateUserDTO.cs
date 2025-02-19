using Microsoft.AspNetCore.Mvc;

namespace AdminMPA.RazorModels
{
        [BindProperties]
    public class CreateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public DateTime Birth { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
