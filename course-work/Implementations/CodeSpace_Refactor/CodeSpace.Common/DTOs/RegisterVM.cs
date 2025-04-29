using Microsoft.AspNetCore.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.Users
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "This Field is Required.")]
        [StringLength(20, ErrorMessage = "Your username is too long.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Needs to be an email address.")]
        [EmailAddress(ErrorMessage = "Needs to be an email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        public string Password { get; set; }
    }
}
