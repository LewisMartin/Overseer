using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class UserProfileController : UserAccountController
    {
        // GET: Profile Viewer
        [HttpGet]
        public ActionResult ProfileViewer(int userId)
        {
            var user = _unitOfWork.Users.Get(userId);

            ProfileViewerViewModel viewModel = new ProfileViewerViewModel()
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                EmailAddress = user.Email,
                Role = GetUserRole(user.UserName),
                AllowEdit = user.UserID == GetLoggedInUserId() ? true : false
            };

            var environments = _unitOfWork.TestEnvironments.GetEnvironmentsAndChildMachinesByCreator(userId);

            foreach (var environment in environments)
            {
                if (environment.IsPrivate == false)
                {
                    viewModel.CreatedEnvironments.Add(new EnvironmentStats()
                    {
                        EnvironmentId = environment.EnvironmentID,
                        EnvironmentName = environment.EnvironmentName,
                        MachineCount = environment.Machines.Count(),
                        Status = environment.Status == true ? "Online" : environment.DownTimeCategory.Name
                    });
                }
            }

            return View(viewModel);
        }

        // GET: Profile Editor
        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ProfileEditor(int userId)
        {
            if (userId == GetLoggedInUserId())
            {
                var user = _unitOfWork.Users.GetWithUserRole(userId);

                ProfileEditorViewModel viewModel = new ProfileEditorViewModel()
                {
                    UserId = user.UserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                    PasswordChanged = false,
                    RoleChoices = CreateUserRoleSelectList(_unitOfWork.UserRoles.GetAll(), user.UserRole.RoleName)
                };

                return View(viewModel);
            }
            else
            {
                return View("~/Views/UserAuth/Unauthorized.cshtml");
            }
        }

        // POST: Profile Editor
        [HttpPost, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ProfileEditor(ProfileEditorViewModel viewModel)
        {
            if (viewModel.UserId == GetLoggedInUserId())
            {
                // if the regiter View Model meets it's validation criteria
                if (ModelState.IsValid)
                {
                    var user = _unitOfWork.Users.Get(viewModel.UserId);

                    // return error early if username is duplicate
                    if (viewModel.UserName != user.UserName)
                    {
                        if (!UserNameAvailable(viewModel.UserName))
                        {
                            return Json(new { success = false, responsemsg = ("UserName is not available!") }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    user.UserName = viewModel.UserName;
                    user.FirstName = viewModel.FirstName;
                    user.LastName = viewModel.LastName;
                    user.Email = viewModel.EmailAddress;
                    user.UserRoleID = Int32.Parse(viewModel.ChosenRoleID);

                    // perform back end validation on password field
                    if (viewModel.PasswordChanged)
                    {
                        if (viewModel.Password != viewModel.ConfirmPassword)
                        {
                            return Json(new { success = false, responsemsg = ("Password & confirmation must match!") }, JsonRequestBehavior.AllowGet);
                        }

                        var crypto = new SimpleCrypto.PBKDF2();     // instantiate our hashing provider

                        if (string.IsNullOrEmpty(viewModel.Password))
                        {
                            return Json(new { success = false, responsemsg = ("You must set a new password!") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (crypto.Compute(viewModel.Password, user.PasswordSalt) == user.Password)
                            {
                                return Json(new { success = false, responsemsg = ("Password cannot match existing password!") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                crypto.GenerateSalt();
                                user.Password = crypto.Compute(viewModel.Password);
                                user.PasswordSalt = crypto.Salt;
                            }
                        }
                    }

                    _unitOfWork.Save();

                    return Json(new { success = true, responsemsg = ("Changes made successfully!") }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, responsemsg = ("Validation failed!") }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, responsemsg = ("Unauthorized Request!") }, JsonRequestBehavior.AllowGet);
        }
    }
}