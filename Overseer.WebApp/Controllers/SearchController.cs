using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class SearchController : UserAccountController
    {
        // GET: Search
        [HttpGet]
        public ActionResult Index()
        {
            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchType = "user";

            viewModel.AdvancedUserOps.RoleOptions = GetAllUserRoles();

            viewModel.AdvancedEnvOps.StatusOptions = GetEnvironmentStatusOptions();
            viewModel.AdvancedEnvOps.MonitoringSettingsOptions = GetMonitoringSettingsOptions();

            viewModel.AdvancedMachineOps.EnvironmentOptions = GetEnvironmentOptions();
            viewModel.AdvancedMachineOps.OSBitnessOptions = GetOSBitnessOptions();

            viewModel.BaseAppUrl = GetBaseApplicationUrl();

            return View(viewModel);
        }

        [HttpPost]
        public PartialViewResult _Query(SearchViewModel queryData)
        {
            _QueryViewModel viewModel = new _QueryViewModel();

            int temp;
            switch (queryData.SearchType)
            {
                case "user":
                    var userFilter1 = queryData.AdvancedUserOps.CreatedEnvironmentCount;
                    var userFilter2 = queryData.AdvancedUserOps.RoleId;

                    var userMatches = _unitOfWork.Users.DiscoverySearchQuery(queryData.SearchTerm, userFilter1, (Int32.TryParse(userFilter2, out temp) ? (int?)temp : (int?)null));

                    foreach (var match in userMatches)
                    {
                        viewModel.UserQueryResults.Add(new UserQueryResult() { ResultId = match.UserID, ResultName = match.UserName, MatchedProperties = GetMatchedUserProperties(queryData.SearchTerm, match) });
                    }

                    break;
                case "environment":
                    var envFilter1 = queryData.AdvancedEnvOps.MachineCount;
                    var envFilter2 = queryData.AdvancedEnvOps.MonitoringSettings;
                    var envFilter3 = queryData.AdvancedEnvOps.Status;

                    var envMatches = _unitOfWork.TestEnvironments.DiscoverySearchQuery(queryData.SearchTerm, envFilter1, ConvertEnvMonSettings(envFilter2), ConvertEnvStatus(envFilter3));

                    foreach (var match in envMatches)
                    {
                        viewModel.EnvironmentQueryResults.Add(new EnvironmentQueryResult() { ResultId = match.EnvironmentID, ResultName = match.EnvironmentName, MatchedProperties = GetMatchedEnvironmentProperties(queryData.SearchTerm, match) });
                    }

                    break;
                case "machine":
                    var machineFilter1 = queryData.AdvancedMachineOps.EnvironmentId;
                    var machineFilter2 = queryData.AdvancedMachineOps.OSBitness;
                    var machineFilter3 = queryData.AdvancedMachineOps.CpuCores;
                    var machineFilter4 = queryData.AdvancedMachineOps.Memory;

                    var machineMatches = _unitOfWork.Machines.DiscoverySearchQuery(queryData.SearchTerm, machineFilter1, machineFilter2, machineFilter3, machineFilter4);

                    foreach (var match in machineMatches)
                    {
                        viewModel.MachineQueryResults.Add(new MachineQueryResult() { ResultId = match.MachineID, ResultName = match.DisplayName, MatchedProperties = GetMatchedMachineProperties(queryData.SearchTerm, match) });
                    }

                    break;
            }

            return PartialView(viewModel);
        }


        public List<SelectListItem> GetMonitoringSettingsOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "enabled", Text = "Enabled" },
                new SelectListItem() { Value = "disabled", Text = "Disabled" }
            };
        }

        public List<SelectListItem> GetEnvironmentStatusOptions()
        {
            List<SelectListItem> tempList = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "online", Text = "Online" },
                new SelectListItem() { Value = "offline", Text = "Offline" }
            };

            return tempList;
        }

        public List<SelectListItem> GetEnvironmentOptions()
        {
            List<SelectListItem> tempList = new List<SelectListItem>();

            var environments = _unitOfWork.TestEnvironments.GetAll();

            foreach (var env in environments)
            {
                tempList.Add(new SelectListItem() { Value = env.EnvironmentID.ToString(), Text = env.EnvironmentName });
            }

            return tempList;
        }

        public List<SelectListItem> GetOSBitnessOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "32", Text = "x86" },
                new SelectListItem() { Value = "64", Text = "x64" }
            };
        }

        private List<MatchedProperty> GetMatchedUserProperties(string term, UserAccount user)
        {
            List<MatchedProperty> temp = new List<MatchedProperty>();

            if (user.UserName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(user.UserName, term, "UserName"));
            }
            if (user.FirstName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(user.FirstName, term, "First Name"));
            }
            if (user.LastName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(user.LastName, term, "Last Name"));
            }
            if (user.Email.Contains(term))
            {
                temp.Add(CreateMatchedProperty(user.Email, term, "Email"));
            }

            return temp;
        }

        private List<MatchedProperty> GetMatchedEnvironmentProperties(string term, TestEnvironment env)
        {
            List<MatchedProperty> temp = new List<MatchedProperty>();

            if (env.EnvironmentName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(env.EnvironmentName, term, "Environment Name"));
            }

            return temp;
        }

        private List<MatchedProperty> GetMatchedMachineProperties(string term, Machine machine)
        {
            List<MatchedProperty> temp = new List<MatchedProperty>();

            if (machine.DisplayName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(machine.DisplayName, term, "Display Name"));
            }
            if (machine.ComputerName.Contains(term))
            {
                temp.Add(CreateMatchedProperty(machine.ComputerName, term, "Computer Name"));
            }

            return temp;
        }

        private bool? ConvertEnvStatus(string val)
        {
            if (val == null || val == "")
            {
                return null;
            }
            if (val == "online")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool? ConvertEnvMonSettings(string val)
        {
            if (val == null || val == "")
            {
                return null;
            }
            if (val == "enabled")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private MatchedProperty CreateMatchedProperty(string value, string term, string prop)
        {
            List<string> split = value.Split(new string[] { term }, StringSplitOptions.None).ToList();

            return new MatchedProperty() { PropertyName = prop, PropertyValue = split, MatchedSubstring = term };
        }
    }
}