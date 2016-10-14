using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class DownTimeCategory
    {
        [Key]
        public int DownTimeCatID { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }
    }
}