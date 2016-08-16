using envSeer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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


        //POST/GET: used in both/either GET or POST request
        // the current 'results per page' setting
        public int resultsPerPage;

        // a search term to query users by
        public string searchTerm;

        // page number
        public int selectedPage;
    }
}