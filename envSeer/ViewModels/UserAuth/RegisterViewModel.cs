using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.ViewModels.UserAuth
{
    // contains data annotations for client side validation
    public class RegisterViewModel
    {
        [MaxLength(25)]
        [Required(ErrorMessage = "Enter your first name.")]
        public string FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Enter your last name.")]
        public string LastName { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Choose a user name.")]
        public string UserName { get; set; }

        [MaxLength(40)]
        [Required(ErrorMessage = "Enter your corporate email address.")]
        public string EmailAddress { get; set; }

        [MaxLength(20), MinLength(6)]
        [Required(ErrorMessage = "Choose a password.")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password")]
        [Required(ErrorMessage = "Confirm your password.")]
        public string ConfirmPassword { get; set; }

        // below represents the selected value for, & the possible values of the user's role (the latter stored using the 'SelectListItem' class)
        [Required(ErrorMessage = "Choose a role.")]
        public string ChosenRoleID { get; set; }

        public IEnumerable<SelectListItem> RoleChoices { get; set; }
    }
}