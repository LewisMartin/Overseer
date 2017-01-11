using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface ICalendarEventRepository : IRepository<CalendarEvent>
    {
        IEnumerable<CalendarEvent> GetEventsByEnvironment(int environmentId);

        IEnumerable<CalendarEvent> GetEventsByEnvironmentAndMonth(int environmentId, int month, int year);

        IEnumerable<CalendarEvent> GetEventsByEnvironmentAndDate(int environmentId);
    }
}
