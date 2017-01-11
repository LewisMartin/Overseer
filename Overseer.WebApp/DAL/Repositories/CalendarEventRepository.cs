using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class CalendarEventRepository : Repository<CalendarEvent>, ICalendarEventRepository
    {
        public CalendarEventRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public IEnumerable<CalendarEvent> GetEventsByEnvironment(int environmentId)
        {
            return dbContext.CalendarEvents.Where(e => e.AssociatedEnvironment == environmentId).ToList();
        }

        public IEnumerable<CalendarEvent> GetEventsByEnvironmentAndDate(int environmentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CalendarEvent> GetEventsByEnvironmentAndMonth(int environmentId, int month, int year)
        {
            return dbContext.CalendarEvents.Where(e => e.AssociatedEnvironment == environmentId && e.EventDate.Month == month && e.EventDate.Year == year).ToList();
        }
    }
}