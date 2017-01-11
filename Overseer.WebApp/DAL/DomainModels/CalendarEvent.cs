using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("CalendarEvents")]
    public class CalendarEvent
    {
        [Key]
        public int EventID { get; set; }

        [ForeignKey("TestEnvironment")]
        public int AssociatedEnvironment { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime EventDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int DaysEffort { get; set; }

        // navigation properties
        public TestEnvironment TestEnvironment { get; set; }
    }
}