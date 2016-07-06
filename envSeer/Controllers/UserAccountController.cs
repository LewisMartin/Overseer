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
    public class UserAccountController : Controller
    {
        // instantiating a private unit of work for any database work carried out within this controller
        private readonly IUnitOfWork _unitOfWork;

        // User Account Controller Constructor - simply instantiates the UnitOfWork class (need to look into dependancy injection at some point)
        public UserAccountController()
        {
            _unitOfWork = new UnitOfWork(new envSeerDBContext());
        }

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

                            // add roles to claim (reading these from the database)
                            new Claim(ClaimTypes.Role, GetUserRole()),
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
                // instantiate our hashing provider
                var crypto = new SimpleCrypto.PBKDF2();

                // THIS SHOULD BE REPLACED BY USE OF A REPOSITORY
                // add the new user to our user database
                using(envSeerDBContext db = new envSeerDBContext())
                {
                    // generate a new salt to use for this user
                    crypto.GenerateSalt();

                    // generate a new entity that we'll persist to the database
                    var newUser = db.Users.Create();

                    // populate the new user object with the data from our ViewModel
                    newUser.UserName = newUserDetails.UserName;
                    newUser.FirstName = newUserDetails.FirstName;
                    newUser.LastName = newUserDetails.LastName;
                    newUser.Email = newUserDetails.EmailAddress;
                    newUser.UserRoleID = Int32.Parse(newUserDetails.ChosenRoleID);
                    newUser.Password = crypto.Compute(newUserDetails.Password);
                    newUser.PasswordSalt = crypto.Salt;

                    // add & persist the new user to the user db
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    // model validation passed, login complete - clear modelstate, add additional success message to viewdata and return blank registration form
                    ModelState.Clear();
                    ViewData["RegSuccess"] = "Registration Successful for " + newUser.UserName.ToString() + "!";
                    // populate the 'RoleChoices' property with contents of UserRole db
                    var RegisterGetModel = new RegisterViewModel() { RoleChoices = GetAllUserRoles() };
                    return View(RegisterGetModel);
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

            // THIS NEEDS TO BE REPLACED WITH A REPOSITORY
            // pull the user data from the database

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


        // MOVE TO REPOSITORY method to get the role of a user during login we will take the username to find the user record, find their roleId and then query the userrole table using role id to find the role name
        public string GetUserRole()
        {
            return "QA";
        }

        // MOVE TO REPOSITORY method to get all roles from the database
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


        // overriding dispose method to dispose of UnitOfWork
        protected override void Dispose(bool disposing)
        {
            // adding the dispose of UnitOfWork, which will in turn dispose the DbContext#
            _unitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}