﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("SupportedOS")]
    public class OperatingSys
    {
        [Key]
        public int OperatingSysID { get; set; }

        public string OSName { get; set; }

        public int Bitness { get; set; }
    }
}