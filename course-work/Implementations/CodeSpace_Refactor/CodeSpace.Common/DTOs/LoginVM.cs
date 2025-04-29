using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.Users
{
    public class LoginVM
    {
        [Required(ErrorMessage = "This Field is Required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        public string Password { get; set; }
    }
}
