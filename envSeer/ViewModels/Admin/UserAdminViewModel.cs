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
        // list of user account data to populate the table with
        public List<UserAccountData> userAccountData = new List<UserAccountData>();

        // number of pages of entries based on the current 'records per page' value
        public int totalPages;

        // the current page
        public int currentPage;

        // list to populate 'results per page' dropdown
        public List<SelectListItem> resultsPerPageOptions;

        // list to populate 'pages' dropdown
        public List<SelectListItem> pageSelectOptions;
    }
}