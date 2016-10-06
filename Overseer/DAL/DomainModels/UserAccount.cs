using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    // contains data annotations to define schema and database constraints
    public class UserAccount
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(40)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        // foreign key for user role
        [ForeignKey("UserRole")]
        public int UserRoleID { get; set; }

        // navigation property to related UserRole data
        public UserRole UserRole { get; set; }

        // navigation property to all test environments this user is 'creator' of
        public ICollection<TestEnvironment> TestEnvironments { get; set; }
    }
}