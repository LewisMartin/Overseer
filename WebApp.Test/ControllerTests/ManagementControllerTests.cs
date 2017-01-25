using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Overseer.WebApp.Controllers;
using System;
using System.Linq;
using System.Security.Claims;
using Overseer.WebApp.ViewModels.Management;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class ManagementControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void ManagementController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            ManagementController controller = new ManagementController();

            // Act:
            Type type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [TestMethod]
        public void ManagementController_CalendarManagement_GET()
        {
            // Arrange:
            ManagementController controller = new ManagementController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.CalendarManagement() as ViewResult;
            CalendarManagementViewModel viewModel = (CalendarManagementViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(CalendarManagementViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void ManagementController_CreateCalendarEvent_POST()
        {
            // Arrange:
            ManagementController controller = new ManagementController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ActionResult result = controller.CreateCalendarEvent(new CalendarManagementViewModel()
            {
                SelectedEnvironment = "1",
                EventDate = DateTime.Now,
                Title = "Test Event",
                DaysEffort = 3,
                Description = "Some arbitrary description."
            }) as ActionResult;

            JsonResult jsonResult = (JsonResult)result;
            dynamic jsonResultValues = jsonResult.Data;

            // Assert:
            Assert.IsNotNull(result); // Json is not null
            Assert.IsTrue(jsonResultValues.success);
        }
    }
}
