﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.Models
{
    public class UserDataViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
    }
}