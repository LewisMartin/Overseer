using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Search
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            AdvancedUserOps = new AdvancedUserOptions();
            AdvancedEnvOps = new AdvancedEnvironmentOptions();
            AdvancedMachineOps = new AdvancedMachineOptions();
        }

        public string SearchTerm { get; set; }

        [Required(ErrorMessage = "Choose a search category.")]
        public string SearchType { get; set; }                // 0: User, 1: Environment, 2: Machine

        public AdvancedUserOptions AdvancedUserOps { get; set; }

        public AdvancedEnvironmentOptions AdvancedEnvOps { get; set; }

        public AdvancedMachineOptions AdvancedMachineOps { get; set; }

        public string BaseAppUrl { get; set; }
    }

    public class AdvancedUserOptions
    {
        public AdvancedUserOptions()
        {
            RoleOptions = new List<SelectListItem>();
        }

        public bool RoleToggle { get; set; }

        public string RoleId { get; set; }

        public IEnumerable<SelectListItem> RoleOptions { get; set; }

        public bool CreatedEnvironmentCountToggle { get; set; }

        public int? CreatedEnvironmentCount { get; set; }
    }

    public class AdvancedEnvironmentOptions
    {
        public AdvancedEnvironmentOptions()
        {
            StatusOptions = new List<SelectListItem>();
            MonitoringSettingsOptions = new List<SelectListItem>();
        }

        public bool SatusToggle { get; set; }

        public string Status { get; set; }

        public IEnumerable<SelectListItem> StatusOptions { get; set; }

        public bool MachineCountToggle { get; set; }

        public int? MachineCount { get; set; }

        public bool MonitoringSettingsToggle { get; set; }

        public string MonitoringSettings { get; set; }

        public IEnumerable<SelectListItem> MonitoringSettingsOptions { get; set; }
    }

    public class AdvancedMachineOptions
    {
        public AdvancedMachineOptions()
        {
            OSBitnessOptions = new List<SelectListItem>();
        }

        public bool EnvironmentToggle { get; set; }

        public int? EnvironmentId { get; set; }

        public IEnumerable<SelectListItem> EnvironmentOptions { get; set; }

        public bool OSBitnessToggle { get; set; }

        public int? OSBitness { get; set; }

        public IEnumerable<SelectListItem> OSBitnessOptions { get; set; }

        public bool CpuCoresToggle { get; set; }

        public int? CpuCores { get; set; }

        public bool MemoryToggle { get; set; }

        public int? Memory { get; set; }
    }
}