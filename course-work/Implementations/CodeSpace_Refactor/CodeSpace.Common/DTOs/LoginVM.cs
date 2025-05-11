using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.Users
{
    public class LoginVM
    {
        [Required(ErrorMessage = "This Field is Required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        [StringLength(64)]
        public string Password { get; set; }
    }
}
