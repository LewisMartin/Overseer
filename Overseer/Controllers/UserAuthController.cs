using System.Linq;
using System.Web.Mvc;
using Overseer.WebApp.ViewModels.UserAuth;
using Overseer.WebApp.DAL;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using Overseer.WebApp.DAL.DomainModels;
using System.Collections.Generic;
using System.Collections;
using System;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.Repositories;

namespace Overseer.WebApp.Controllers
{
    public class UserAuthController : UserAccountController
    {
        // GET: login page
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
        // POST: login page
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
                            // clam : User ID
                            new Claim(ClaimTypes.NameIdentifier, GetUserId(userCreds.Username).ToString()),

                            // claim : Username
                            new Claim(ClaimTypes.Name, userCreds.Username),

                            // claim : User role (name, not role id)
                            new Claim(ClaimTypes.Role, GetUserRole(userCreds.Username)), // note this is just for use with 'Authorize' attribute - if we need to get the user role in the view we need to pass it to the appropriate ViewModel
                        },
                    "UserAuthCookie");

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

        // GET: logout page
        [HttpGet]
        public ActionResult LogOut()
        {
            // log out using the new Microsoft Identity stuff
            HttpContext.GetOwinContext().Authentication.SignOut();

            // redirect to login page
            return RedirectToAction("Login", "UserAuth");
        }

        // GET: registration page
        [HttpGet]
        public ActionResult Register()
        {
            // populate the 'RoleChoices' property with contents of UserRole db
            var RegisterGetModel = new RegisterViewModel() { RoleChoices = GetAllUserRoles() };
            
            // pass viewmodel to view
            return View(RegisterGetModel);
        }
        // POST: registration page
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

                    // generate a new eUserAuth ntity that we'll add to the database
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
    }
}