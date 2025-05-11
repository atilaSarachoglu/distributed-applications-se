using Microsoft.AspNetCore.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.Users
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "This Field is Required.")]
        [StringLength(50, ErrorMessage = "Your username is too long.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Needs to be an email address.")]
        [EmailAddress(ErrorMessage = "Needs to be an email address.")]
        [StringLength(255)]
        public string Email { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        [StringLength(64)]
        public string Password { get; set; }
    }
}
