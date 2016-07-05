using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace envSeer.DAL.DomainModels
{
    public class UserRole
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; }
    }
}