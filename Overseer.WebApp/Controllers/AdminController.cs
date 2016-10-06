using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.Models;
using Overseer.WebApp.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth(Roles = "Administrator")]
    public class AdminController : UserAccountController
    {
        // get request for admin - site page
        [HttpGet]
        public ActionResult Site()
        {
            return View();
        }

        // get request for admin - users page
        [HttpGet, OutputCache(NoStore = true, Duration = 1)] // force controller to reload fresh page rather than use cache
        public ActionResult Users(string notification)
        {
            // getting the top 10 users by default
            IEnumerable<UserAccount> returnedUsers = _unitOfWork.Users.GetRangeUsers(0, 10);

            List<UserDataViewModel> viewModelUsers = new List<UserDataViewModel>();

            // create a UserAuthData object for each user domain object
            foreach(var user in returnedUsers)
            {
                viewModelUsers.Add(new UserDataViewModel
                {
                    UserId = user.UserID,
                    UserName = user.UserName,
                    UserRole = _unitOfWork.UserRoles.Get(user.UserRoleID).RoleName,
                    FullName = (user.FirstName + user.LastName),
                    Email = user.Email,
                });
            }

            int results = 10;

            // create list for 'results per page' drop down
            var resPerPageOps = new List<SelectListItem>
            {
                new SelectListItem { Text = "10", Value = "10", Selected = (results == 10 ? true : false)},
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50"}
            };

            // create list for 'page selection' drop down
            var pageSelectionOps = new List<SelectListItem>();

            for (int i = 1; i <= ((_unitOfWork.Users.CountRows() + 10 - 1) / 10); i++)
            {
                pageSelectionOps.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == 1 ? true : false) });
            };

            // instantiate and assign values to ViewModel
            UserAdminViewModel viewModel = new UserAdminViewModel
            {
                notificationMsg = notification,
                UserAccountData = viewModelUsers,
                resultsPerPageOptions = resPerPageOps,
                pageSelectOptions = pageSelectionOps,
                totalPages = ((_unitOfWork.Users.CountRows() + 10 - 1)/10), // Note: integer division always rounds down
                searchFilter = "",
                currentPage = 1
            };

            return View(viewModel);
        }

        // POST method for 'Users' page
        [HttpPost, OutputCache(NoStore = true, Duration = 1)] // force controller to reload fresh page rather than use cache when back/forward browser operation is used
        public ActionResult Users(string button, int resPerPage, string persistedSearchTerm, string searchTerm, int selectedPage)
        {
            IEnumerable<UserAccount> returnedUsers; // collection of user entities
            var pageSelectionOps = new List<SelectListItem>(); // create list for 'page selection' drop down
            var viewModelUsers = new List<UserDataViewModel>(); // list of user details
            string searchQuery = persistedSearchTerm;
            int pageCount;
            int totalRows;

            // if the search button is pressed we update the search term to use and reset pagination
            if (button == "search")
            {
                searchQuery = searchTerm;
                selectedPage = 1;
            }

            // whether a search filter has been applied or not
            if (searchQuery != null && searchQuery != "" && !String.IsNullOrWhiteSpace(searchQuery))
            {
                totalRows = _unitOfWork.Users.CountUserMatches(searchQuery);

                // ensure 'results per page' doesn't go out of bounds
                if ((resPerPage * (selectedPage - 1)) >= totalRows)
                {
                    selectedPage = 1;
                }

                // get users by search term
                returnedUsers = _unitOfWork.Users.GetRangeUserMatches((resPerPage * (selectedPage - 1)), resPerPage, searchQuery);
            }
            else
            {
                totalRows = _unitOfWork.Users.CountRows();

                // ensure 'results per page' doesn't go out of bounds
                if ((resPerPage * (selectedPage - 1)) >= totalRows)
                {
                    selectedPage = 1;
                }

                returnedUsers = _unitOfWork.Users.GetRangeUsers((resPerPage * (selectedPage - 1)), resPerPage);

                // ensure the search query that we'll pass back to the view is in a specific form
                searchQuery = "";
            }


            // set paging drop down option
            pageSelectionOps.Add(new SelectListItem { Text = "1", Value = "1" });
            pageCount = 1;

            for (int i = 2; i <= ((totalRows + resPerPage - 1) / resPerPage); i++)
            {
                pageSelectionOps.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                pageCount += 1;
            };

            // create a UserAuthData object for each user domain object
            foreach (var user in returnedUsers)
            {
                viewModelUsers.Add(new UserDataViewModel
                {
                    UserId = user.UserID,
                    UserName = user.UserName,
                    UserRole = _unitOfWork.UserRoles.Get(user.UserRoleID).RoleName,
                    FullName = (user.FirstName + user.LastName),
                    Email = user.Email,
                });
            }

            // create list for 'results per page' drop down
            var resPerPageOps = new List<SelectListItem>
            {
                new SelectListItem { Text = "10", Value = "10"},
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50"}
            };

            // instantiate and assign values to ViewModel
            UserAdminViewModel viewModel = new UserAdminViewModel
            {
                UserAccountData = viewModelUsers,
                resultsPerPageOptions = resPerPageOps,
                pageSelectOptions = pageSelectionOps,
                totalPages = pageCount, // Note: integer division always rounds down
                searchFilter = searchQuery,
                currentPage = selectedPage
            };

            return View(viewModel);
        }

        // GET Action for ConfirmUserDeletion page
        [HttpGet, OutputCache(NoStore = true, Duration = 1)] // force controller to reload fresh page rather than use cache
        public ActionResult ConfirmUserDeletion(int? id)
        {
            if (id == null || id == 1)
            {
                return RedirectToAction("Users");
            }
            else
            {
                UserAccount user = _unitOfWork.Users.Get((int)id);

                ConfirmUserDeletionViewModel viewModel = new ConfirmUserDeletionViewModel
                {
                    userToDelete = new UserDataViewModel
                    {
                        UserId = user.UserID,
                        UserName = user.UserName,
                        FullName = (user.FirstName + user.LastName),
                        Email = user.Email,
                        UserRole = _unitOfWork.UserRoles.Get(user.UserRoleID).RoleName
                    }
                };

                return View(viewModel);
            }
        }

        // simply delete the user and redirect to the users page..
        [HttpPost]
        public ActionResult DeleteUser(int UserId)
        {
            UserAccount userToDelete = _unitOfWork.Users.Get(UserId);

            string notificationMsg = "'" + userToDelete.UserName + "' was successfully deleted.";

            _unitOfWork.Users.Delete(userToDelete);

            _unitOfWork.Save();

            return RedirectToAction("Users", new { notification = notificationMsg});
        }

        // GET Action for edit user page
        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Users");
            }
            else
            {
                UserAccount userToEdit = _unitOfWork.Users.Get((int)id);

                EditUserViewModel viewModel = new EditUserViewModel
                {
                    UserId = userToEdit.UserID,
                    FirstName = userToEdit.FirstName,
                    LastName = userToEdit.LastName,
                    UserName = userToEdit.UserName,
                    EmailAddress = userToEdit.Email,
                    RoleChoices = CreateUserRoleSelectList(_unitOfWork.UserRoles.GetNonAdminRoles(), (_unitOfWork.UserRoles.Get(userToEdit.UserRoleID)).RoleName)
                };

                return View(viewModel);
            }
        }

        // POST Action for edit user page
        [HttpPost, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult EditUser(EditUserViewModel editedUserDetails)
        {
            // repopulate the list of role choices for the select box
            editedUserDetails.RoleChoices = CreateUserRoleSelectList(_unitOfWork.UserRoles.GetNonAdminRoles(), (_unitOfWork.UserRoles.Get(int.Parse(editedUserDetails.ChosenRoleID))).RoleName);

            if (ModelState.IsValid)
            {
                UserAccount userToUpdate = _unitOfWork.Users.Get(editedUserDetails.UserId);

                // update the rest of the user's details
                var crypto = new SimpleCrypto.PBKDF2();

                // perform back end validation on password field
                if (editedUserDetails.PasswordChanged)
                {
                    if (string.IsNullOrEmpty(editedUserDetails.Password))
                    {
                        ModelState.AddModelError("UnchangedPassError", "You must set a new password!");
                        return View(editedUserDetails);
                    }
                    else
                    {
                        if (crypto.Compute(editedUserDetails.Password, userToUpdate.PasswordSalt) == userToUpdate.Password)
                        {
                            ModelState.AddModelError("UnchangedPassError", "Password cannot match existing password!");
                            return View(editedUserDetails);
                        }
                        else
                        {
                            crypto.GenerateSalt();
                            userToUpdate.Password = crypto.Compute(editedUserDetails.Password);
                            userToUpdate.PasswordSalt = crypto.Salt;
                        }
                    }
                }

                // redirect if username is duplicate
                if (editedUserDetails.UserName != userToUpdate.UserName)
                {
                    if (!UserNameAvailable(editedUserDetails.UserName))
                    {
                        ModelState.AddModelError("DuplicateUserError", "UserName is not available!");
                        return View(editedUserDetails);
                    }
                }

                userToUpdate.FirstName = editedUserDetails.FirstName;
                userToUpdate.LastName = editedUserDetails.LastName;
                userToUpdate.Email = editedUserDetails.EmailAddress;
                userToUpdate.UserRoleID = Int32.Parse(editedUserDetails.ChosenRoleID);

                _unitOfWork.Save();

                string notificationMsg = "'" + editedUserDetails.UserName + "' was successfully updated.";

                return RedirectToAction("Users", new { notification = notificationMsg });
            }
            else
            {
                return View(editedUserDetails);
            }
        }

        // Dispose Method
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}