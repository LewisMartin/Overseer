using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
{
    public class UserAccountController : BaseController
    {
        // method to validate password against what is stored in our user DB
        public bool ValidatePassword(string userName, string userSecret)
        {
            // bool to store whether password is validated/not
            bool valid = false;

            // create and instance of our hashing provider
            var crypto = new SimpleCrypto.PBKDF2();

            // getting user via user repository
            var matchedUser = _unitOfWork.Users.GetUserByUsername(userName);

            // if a user is matched in the db
            if (matchedUser != null)
            {
                // if the password within the database matches that posted via ViewModel
                if (matchedUser.Password == crypto.Compute(userSecret, matchedUser.PasswordSalt))
                {
                    // the password has been validated
                    valid = true;
                }
                else
                {
                    // adding error message to modelstate to be returned to view (arg 1 could link the error to a particular property of the model if we wanted)
                    ModelState.AddModelError("", "Username of password is not correct");
                }
            }

            return valid;
        }

        // check if username already exists
        public bool UserNameAvailable(string username)
        {
            UserAccount user = _unitOfWork.Users.GetUserByUsername(username);

            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // method to get the role of a user during login we will take the username to find the user record, find their roleId and then query the userrole table using role id to find the role name
        public string GetUserRole(string userName)
        {
            // get the user from the database
            var user = _unitOfWork.Users.GetUserByUsername(userName);

            // get their role using the UserRoleID foreign key
            var role = _unitOfWork.UserRoles.Get(user.UserRoleID);

            // return the name of the role
            return role.RoleName.ToString();
        }

        // method to get userId of a user during login 
        public int GetUserId(string userName)
        {
            var user = _unitOfWork.Users.GetUserByUsername(userName);

            return user.UserID;
        }

        // method to get all roles from the database
        public IEnumerable<SelectListItem> GetAllUserRoles()
        {
            // an IEnumerable list of SelectList items
            List<SelectListItem> UserRolesList = new List<SelectListItem>();

            // get all user role database records
            var UserRoles = _unitOfWork.UserRoles.GetNonAdminRoles();

            // for each role in the db create a new list item using the roleID & roleName
            foreach (UserRole role in UserRoles)
            {
                UserRolesList.Add(new SelectListItem() { Value = role.RoleID.ToString(), Text = role.RoleName });
            }

            IEnumerable<SelectListItem> UserRoleListItems = UserRolesList;

            // return the list
            return UserRoleListItems;
        }

        // create a select list from a list of values, set default value if it exists in the list
        public IEnumerable<SelectListItem> CreateUserRoleSelectList(IEnumerable<UserRole> userRoles, string defaultValue)
        {
            // an IEnumerable list of SelectList items
            List<SelectListItem> UserRolesList = new List<SelectListItem>();

            // for each role in the db create a new list item using the roleID & roleName
            foreach (UserRole role in userRoles)
            {
                UserRolesList.Add(new SelectListItem() {
                    Value = role.RoleID.ToString(),
                    Text = role.RoleName,
                    Selected = (role.RoleName == defaultValue ? true : false) });
            }

            return UserRolesList;
        }

        // Dispose Method
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}