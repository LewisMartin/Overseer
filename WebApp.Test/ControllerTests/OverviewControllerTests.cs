using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overseer.WebApp.Controllers;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web.Mvc;
using Overseer.WebApp.ViewModels.Overview;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class OverviewControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void OverviewController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            OverviewController controller = new OverviewController();

            // Act:
            Type type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [TestMethod]
        public void OverviewController_Overseer_GET()
        {
            // Arrange:
            OverviewController controller = new OverviewController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.Overseer() as ViewResult;
            OverseerViewModel viewModel = (OverseerViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(OverseerViewModel)); // ViewResult has correct Model Type
        }
    }
}
