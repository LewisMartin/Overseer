using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Overseer.WebApp.Controllers;
using System.Security.Claims;
using System;
using Overseer.WebApp.ViewModels.UserProfile;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class UserProfileControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void UserProfileController_ProfileViewer_GET() // use reflection to check for authorize attribute
        {
            // Arrange:
            UserProfileController controller = new UserProfileController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            var identity = (ClaimsIdentity)controller.HttpContext.User.Identity;
            ViewResult result = controller.ProfileViewer(Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value)) as ViewResult;
            ProfileViewerViewModel viewModel = (ProfileViewerViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(ProfileViewerViewModel)); // ViewResult has correct Model Type
            Assert.IsTrue(viewModel.UserId == 1);  // Correct UserId is returned within ViewModel
            Assert.IsTrue(viewModel.UserName == "Admin");
        }

        [TestMethod]
        public void UserProfileController_ProfileEditor_GET()
        {
            // Arrange:
            UserProfileController controller = new UserProfileController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            var identity = (ClaimsIdentity)controller.HttpContext.User.Identity;
            ViewResult result = controller.ProfileEditor(Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value)) as ViewResult;
            ProfileEditorViewModel viewModel = (ProfileEditorViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(ProfileEditorViewModel)); // ViewResult has correct Model Type
            Assert.IsTrue(viewModel.UserId == 1);  // Correct UserId is returned within ViewModel
            Assert.IsTrue(viewModel.UserName == "Admin");
        }

        [TestMethod]
        public void UserProfileController_ProfileEditor_GET_Unauthorized()
        {
            // Arrange:
            UserProfileController controller = new UserProfileController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            var identity = (ClaimsIdentity)controller.HttpContext.User.Identity;
            ViewResult result = controller.ProfileEditor(Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) + 1) as ViewResult;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsTrue(result.ViewName == "~/Views/UserAuth/Unauthorized.cshtml"); // assert unauthorized view is returned
        }

        [TestMethod]
        public void UserProfileController_ProfileEditor_POST()
        {
            // Arrange:
            UserProfileController controller = new UserProfileController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            var identity = (ClaimsIdentity)controller.HttpContext.User.Identity;
            ActionResult result = controller.ProfileEditor(new ProfileEditorViewModel()
            {
                UserId = Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value),
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "Admin",
                EmailAddress = "admin@Overseer.WebApp.local",
                ChosenRoleID = "1",
                PasswordChanged = false
            }) as ActionResult;

            JsonResult jsonResult = (JsonResult)result;
            dynamic jsonResultValues = jsonResult.Data;

            // Assert:
            Assert.IsNotNull(result); // Json is not null
            Assert.IsTrue(jsonResultValues.success);
        }

        [TestMethod]
        public void UserProfileController_ProfileEditor_POST_Unauthorized()
        {
            // Arrange:
            UserProfileController controller = new UserProfileController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            var identity = (ClaimsIdentity)controller.HttpContext.User.Identity;
            ActionResult result = controller.ProfileEditor(new ProfileEditorViewModel()
            {
                UserId = Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) + 1,
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "Admin",
                EmailAddress = "admin@Overseer.WebApp.local",
                ChosenRoleID = "1",
                PasswordChanged = false
            }) as ActionResult;

            JsonResult jsonResult = (JsonResult)result;
            dynamic jsonResultValues = jsonResult.Data;

            // Assert:
            Assert.IsNotNull(result); // Json is not null
            Assert.IsFalse(jsonResultValues.success);
        }
    }
}
