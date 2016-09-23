using envSeer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.ViewModels.Admin
{
    public class UserAdminViewModel
    {
        // GET: used only for get requests (populating view with data)
        public string notificationMsg;

        // list of user account data to populate the table with
        public List<UserDataViewModel> UserAccountData = new List<UserDataViewModel>();

        // number of pages of entries based on the current 'records per page' value
        public int totalPages;

        // the current page
        public int currentPage;

        // value of applied search filter
        public string searchFilter;

        // list to populate 'results per page' dropdown
        public List<SelectListItem> resultsPerPageOptions;

        // list to populate 'pages' dropdown
        public List<SelectListItem> pageSelectOptions;
    }
}