using Overseer.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Admin
{
    public class ConfirmUserDeletionViewModel
    {
        public bool Exists { get; set; }

        public UserDataViewModel userToDelete;
    }
}