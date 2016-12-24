using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.UserProfile
{
    public class ProfileEditorViewModel
    {
        [Required]
        public int UserId { get; set; }

        [MaxLength(25)]
        [Required(ErrorMessage = "Enter user's first name.")]
        public string FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Enter user's last name.")]
        public string LastName { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Enter a Username.")]
        public string UserName { get; set; }

        [MaxLength(40)]
        [Required(ErrorMessage = "Enter user's corporate email address.")]
        public string EmailAddress { get; set; }

        [Required]
        public bool PasswordChanged { get; set; }

        [MaxLength(20), MinLength(6)]
        public string Password { get; set; }

        [MaxLength(20), MinLength(6)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Choose a role.")]
        public string ChosenRoleID { get; set; }

        public IEnumerable<SelectListItem> RoleChoices { get; set; }
    }
}