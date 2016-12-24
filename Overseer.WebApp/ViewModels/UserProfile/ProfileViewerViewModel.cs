using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.UserProfile
{
    public class ProfileViewerViewModel
    {
        public ProfileViewerViewModel()
        {
            CreatedEnvironments = new List<EnvironmentStats>();
        }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string Role { get; set; }

        public List<EnvironmentStats> CreatedEnvironments { get; set; }

        public bool Online { get; set; }    // need to implement database changes for this..

        public bool AllowEdit { get; set; }
    }

    public class EnvironmentStats
    {
        public int EnvironmentId { get; set; }

        public string EnvironmentName { get; set; }

        public int MachineCount { get; set; }

        public string Status { get; set; }
    }
}