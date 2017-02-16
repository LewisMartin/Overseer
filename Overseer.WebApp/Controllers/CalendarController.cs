using Overseer.WebApp.ViewModels.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    public class CalendarController : BaseController
    {
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }

        // GET: EnvironmentCalendar
        [HttpGet]
        public ActionResult CalendarViewer(int environmentId)
        {
            var environment = _unitOfWork.TestEnvironments.Get(environmentId);

            CalendarViewerViewModel viewModel = new CalendarViewerViewModel()
            {
                EnvironmentId = environment.EnvironmentID,
                EnvironmentName = environment.EnvironmentName,
                MonthOptions = GetMonthOptions(),
                YearOptions = GetYearOptions(),
                BaseAppUrl = GetBaseApplicationUrl()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CalendarViewer(CalendarViewerViewModel calendarOptions)
        {
            return RedirectToAction("_RetrieveCalendar", new { environmentId = calendarOptions.EnvironmentId, month = Int32.Parse(calendarOptions.ChosenMonth), year = Int32.Parse(calendarOptions.ChosenYear) });
        }
        
        [HttpGet]
        public PartialViewResult _RetrieveCalendar(int environmentId, int month, int year)
        {
            int envId = environmentId;

            int selectedMonth = month == 0 ? DateTime.Now.Month : month;
            int selectedYear = year == 0 ? DateTime.Now.Year : year;
            DateTime firstDay = new DateTime(selectedYear, selectedMonth, 1);
            DateTime lastDay = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));

            _RetrieveCalendarViewModel viewModel = new _RetrieveCalendarViewModel(DateTime.DaysInMonth(selectedYear, selectedMonth))
            {
                Month = selectedMonth,
                DisplayMonth = new DateTime(2017, selectedMonth, 1).ToString("MMMM"),
                Year = selectedYear,
                BeginningDay = (firstDay.DayOfWeek > 0) ? (int)firstDay.DayOfWeek : 7,
                EndDay = (lastDay.DayOfWeek > 0) ? (int)lastDay.DayOfWeek : 7
            };

            var events = _unitOfWork.CalendarEvents.GetEventsByEnvironmentAndMonth(environmentId, selectedMonth, selectedYear);

            if (events != null)
            {
                for (int i = 0; i < events.Count(); i++)
                {
                    viewModel.Events[(events.ElementAt(i).EventDate.Day)-1] = new EventInfo()
                    {
                        Title = events.ElementAt(i).Title,
                        DaysEffort = events.ElementAt(i).DaysEffort
                    };

                    viewModel.EventDetails.Add(new EventDetail()
                    {
                        Title = events.ElementAt(i).Title,
                        Date = events.ElementAt(i).EventDate.ToString("dd/MM/yyyy"),
                        Day = (events.ElementAt(i).EventDate.Day)-1,
                        Description = events.ElementAt(i).Description,
                        DaysEffort = events.ElementAt(i).DaysEffort
                    });
                }
            }

            return PartialView(viewModel);
        }

        private List<SelectListItem> GetMonthOptions()
        {
            int month = DateTime.Now.Month;

            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "1", Text = "January", Selected = (month == 1 ? true : false) },
                new SelectListItem() { Value = "2", Text = "February", Selected = (month == 2 ? true : false) },
                new SelectListItem() { Value = "3", Text = "March", Selected = (month == 3 ? true : false) },
                new SelectListItem() { Value = "4", Text = "April", Selected = (month == 4 ? true : false) },
                new SelectListItem() { Value = "5", Text = "May", Selected = (month == 5 ? true : false) },
                new SelectListItem() { Value = "6", Text = "June", Selected = (month == 6 ? true : false) },
                new SelectListItem() { Value = "7", Text = "July", Selected = (month == 7 ? true : false) },
                new SelectListItem() { Value = "8", Text = "August", Selected = (month == 8 ? true : false) },
                new SelectListItem() { Value = "9", Text = "September", Selected = (month == 9 ? true : false) },
                new SelectListItem() { Value = "10", Text = "October", Selected = (month == 10 ? true : false) },
                new SelectListItem() { Value = "11", Text = "November", Selected = (month == 11 ? true : false) },
                new SelectListItem() { Value = "12", Text = "December", Selected = (month == 12 ? true : false) },
            };
        }

        private List<SelectListItem> GetYearOptions()
        {
            int year = DateTime.Now.Year;

            var yearOptions = new List<SelectListItem>();

            for(int i = year; i <= year+5; i++)
            {
                yearOptions.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = (i == year ? true : false) });
            }

            return yearOptions;
        }
    }
}