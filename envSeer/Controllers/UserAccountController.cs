using System.Linq;
using System.Web.Mvc;
using envSeer.ViewModels.UserAccount;
using envSeer.DAL;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using envSeer.DAL.DomainModels;
using System.Collections.Generic;
using System.Collections;
using System;
using envSeer.DAL.Core;
using envSeer.DAL.Repositories;

namespace envSeer.Controllers
{
    public class UserAccountController : BaseController
    {
        // GET: UserAccount
        public ActionResult Index()
        {
            return View();
        }

        // login page
        [HttpGet]
        public ActionResult Login()
        {
            // if the user is already authenticated we redirect them to the home page of the dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // return login view
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel userCreds)
        {
            // check that the posted viewModel meets the validation criteria (defined via it's data annotations)
            if(ModelState.IsValid)
            {
                // here we will validate the password
                if(ValidatePassword(userCreds.Username, userCreds.Password))
                {
                    // MS Identity stuff here - read up on this again
                    var ident = new ClaimsIdentity(
                        new[]
                        {
                            // add username to claimm
                            new Claim(ClaimTypes.Name, userCreds.Username),

                            // add role to claim (getting it from the database using the username to search)
                            new Claim(ClaimTypes.Role, GetUserRole(userCreds.Username)), // note this is just for use with 'Authorize' attribute - if we need to get the user role in the view we need to pass it to the appropriate ViewModel
                        },
                    DefaultAuthenticationTypes.ApplicationCookie);

                    // and I think here we're 'signing the user in'
                    HttpContext.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = false }, ident);

                    // now we redirect the user to the home page of the application (we will later redirect to the dashboard)
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // username/password was incorrect - add an error message 'LoginError' is a non-existance property of the model but lets us access the message from the view
                    ModelState.AddModelError("LoginError", "Username or password is incorrect.");
                }
            }

            // if the model state passed to the controller was not valid, we'll pass the model back to the login view to display validation errors
            return View(userCreds);
        }

        // logout page
        [HttpGet]
        public ActionResult LogOut()
        {
            // log out using the new Microsoft Identity stuff
            HttpContext.GetOwinContext().Authentication.SignOut();

            // redirect to login page
            return RedirectToAction("Login", "UserAccount");
        }

        // registration page
        [HttpGet]
        public ActionResult Register()
        {
            // populate the 'RoleChoices' property with contents of UserRole db
            var RegisterGetModel = new RegisterViewModel() { RoleChoices = GetAllUserRoles() };
            
            // pass viewmodel to view
            return View(RegisterGetModel);
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel newUserDetails)
        {
            // if the regiter View Model meets it's validation criteria
            if (ModelState.IsValid)
            {
                if (UserNameAvailable(newUserDetails.UserName))
                {
                    // instantiate our hashing provider
                    var crypto = new SimpleCrypto.PBKDF2();

                    // new repository based implementation
                    // generate a new salt to use for this user
                    crypto.GenerateSalt();

                    // generate a new eUserAccount ntity that we'll add to the database
                    UserAccount newUser = new UserAccount();

                    // populate the new user object with the data from our ViewModel
                    newUser.UserName = newUserDetails.UserName;
                    newUser.FirstName = newUserDetails.FirstName;
                    newUser.LastName = newUserDetails.LastName;
                    newUser.Email = newUserDetails.EmailAddress;
                    newUser.UserRoleID = Int32.Parse(newUserDetails.ChosenRoleID);
                    newUser.Password = crypto.Compute(newUserDetails.Password);
                    newUser.PasswordSalt = crypto.Salt;

                    // add the user & save changes via unit of work
                    _unitOfWork.Users.Add(newUser);
                    _unitOfWork.Save();

                    // model validation passed, login complete - clear modelstate, add additional success message to viewdata and return blank registration form
                    ModelState.Clear();
                    ViewData["RegSuccess"] = "Registration Successful for " + newUser.UserName.ToString() + "!";
                    // populate the 'RoleChoices' property with contents of UserRole db
                    var RegisterGetModel = new RegisterViewModel() { RoleChoices = GetAllUserRoles() };
                    return View(RegisterGetModel);
                }
                else
                {
                    ModelState.AddModelError("RegError", "UserName is not available!");
                }
            }

            // model validation failed - pass model back to view   
            newUserDetails.RoleChoices = GetAllUserRoles();
            return View(newUserDetails);
        }

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

        // overriding dispose method to add disposal of UnitOfWork class
        protected override void Dispose(bool disposing)
        {
            // adding the dispose of UnitOfWork, which will in turn dispose the DbContext#
            _unitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}