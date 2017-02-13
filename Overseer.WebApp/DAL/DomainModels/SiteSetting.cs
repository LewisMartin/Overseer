using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("SiteSettings")]
    public class SiteSetting
    {
        [DefaultValue(1)]
        [Key]
        public int SiteID { get; set; }

        [Range(1, 10)]
        public int EnvironmentLimit { get; set; }

        [Range(1, 20)]
        public int MachineLimit { get; set; }

        public bool AllowMonitoring { get; set; }

        public bool EnableUserRoleChange { get; set; }

        public bool EnableUsernameChange { get; set; }
    }
}