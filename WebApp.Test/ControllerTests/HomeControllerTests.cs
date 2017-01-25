using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using Overseer.WebApp.Controllers;
using Moq;
using System.Web;
using System.Security.Claims;
using Overseer.WebApp.ViewModels.Home;
using System;
using System.Linq;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class HomeControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void HomeController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            HomeController controller = new HomeController();

            // Act:
            Type type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [TestMethod]
        public void HomeController_Index_GET()
        {
            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = MockContextAdminUser.Object;
            
            // Act:
            ViewResult result = controller.Index() as ViewResult;
            HomeViewModel vModel = (HomeViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(HomeViewModel)); // ViewResult has correct Model Type
            Assert.IsTrue(vModel.UserId == 1);  // Correct UserId is returned within ViewModel
            Assert.IsTrue(vModel.UserName == "Admin");
            Assert.IsTrue(vModel.UserRole == "Administrator");
        }
    }
}
