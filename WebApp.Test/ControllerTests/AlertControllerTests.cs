using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overseer.WebApp.Controllers;
using System.Web.Mvc;
using System.Linq;
using Overseer.WebApp.ViewModels.Alert;
using System.Reflection;
using System.Collections.Generic;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class AlertControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void AlertController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            AlertController controller = new AlertController();
            List<MethodInfo> methodInformation = new List<MethodInfo>();

            // Act:
            Type type = controller.GetType();
            methodInformation.Add(type.GetMethod("AlertViewer", new Type[] { typeof(string) }));
            methodInformation.Add(type.GetMethod("AlertViewer", new Type[] { typeof(AlertViewerViewModel) }));
            methodInformation.Add(type.GetMethod("RetrieveMachineOptions"));
            methodInformation.Add(type.GetMethod("_AlertFilter"));
            methodInformation.Add(type.GetMethod("ArchiveAlert"));

            // Assert:
            foreach (var method in methodInformation)
            {
                var attributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                Assert.IsTrue(attributes.Any());
            }
        }

        [TestMethod]
        public void AlertController_AlertViewer_GET()
        {
            // Arrange:
            AlertController controller = new AlertController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.AlertViewer("all") as ViewResult;
            AlertViewerViewModel viewModel = (AlertViewerViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(AlertViewerViewModel)); // ViewResult has correct Model Type
        }
    }
}
