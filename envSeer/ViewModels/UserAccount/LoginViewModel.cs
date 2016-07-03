using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace envSeer.ViewModels.UserAccount
{
    public class LoginViewModel
    {
        [MaxLength(20)]
        [Required(ErrorMessage = "Username required.")]
        public string Username { get; set; }

        [MaxLength(20), MinLength(6)]
        [Required(ErrorMessage = "Password required.")]
        public string Password { get; set; }
    }
}