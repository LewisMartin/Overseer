using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Calendar
{
    public class _RetrieveCalendarViewModel
    {
        public _RetrieveCalendarViewModel(int days)
        {
            DaysInMonth = days;
            Events = new EventInfo[days];
            EventDetails = new List<EventDetail>();
        }

        public int Month { get; set; }

        public string DisplayMonth { get; set; }

        public int Year { get; set; }

        public int DaysInMonth { get; set; }

        public EventInfo[] Events { get; set; }

        public List<EventDetail> EventDetails { get; set; }

        public int BeginningDay { get; set; }

        public int EndDay { get; set; }
    }

    public class EventInfo
    {
        public string Title { get; set; }

        public int DaysEffort { get; set; }
    }

    public class EventDetail
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }

        public int Day { get; set; }

        public int DaysEffort { get; set; }
    }
}