using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth(Roles = "Administrator, Manager")]
    public class ManagementController : BaseController
    {
        // GET: Management
        [HttpGet]
        public ActionResult CalendarManagement()
        {
            CalendarManagementViewModel viewModel = new CalendarManagementViewModel()
            {
                EnvironmentOptions = GetEnvironmentOptions(),
                BaseAppUrl = GetBaseApplicationUrl()
            };

            return View(viewModel);
        }

        // POST
        [HttpPost]
        public ActionResult CreateCalendarEvent(CalendarManagementViewModel eventData)
        {
            _unitOfWork.CalendarEvents.Add(new DAL.DomainModels.CalendarEvent()
            {
                AssociatedEnvironment = Int32.Parse(eventData.SelectedEnvironment),
                EventDate = eventData.EventDate,
                Title = eventData.Title,
                Description = eventData.Description,
                DaysEffort = eventData.DaysEffort
            });

            _unitOfWork.Save();

            return Json(new { success = true, successmsg = ("Calendar event added!") }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetEnvironmentOptions()
        {
            var envOptions = new List<SelectListItem>();

            var environments = _unitOfWork.TestEnvironments.GetAll();

            foreach (var env in environments)
            {
                envOptions.Add(new SelectListItem()
                {
                    Value = env.EnvironmentID.ToString(),
                    Text = env.EnvironmentName
                });
            }

            return envOptions;
        }
    }
}