using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overseer.WebApp.Controllers;
using System.Collections.Generic;
using System.Reflection;
using Overseer.WebApp.ViewModels.Environment;
using System.Web.Mvc;
using System.Linq;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class EnvironmentControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void EnvironmentController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            EnvironmentController controller = new EnvironmentController();
            List<MethodInfo> methodInformation = new List<MethodInfo>();

            // Act:
            Type type = controller.GetType();
            methodInformation.Add(type.GetMethod("EnvironmentConfiguration", new Type[] { typeof(int) }));
            methodInformation.Add(type.GetMethod("EnvironmentConfiguration", new Type[] { typeof(EnvironmentConfigurationViewModel) }));
            methodInformation.Add(type.GetMethod("EnvironmentCreation", new Type[0]));
            methodInformation.Add(type.GetMethod("EnvironmentCreation", new Type[] { typeof(EnvironmentCreationViewModel) }));
            methodInformation.Add(type.GetMethod("EnvironmentDeletion"));

            // Assert:
            foreach (var method in methodInformation)
            {
                var attributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                Assert.IsTrue(attributes.Any());
            }
        }

        [TestMethod]
        public void EnvironmentController_Environmentseer_GET()
        {
            // Arrange:
            EnvironmentController controller = new EnvironmentController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.Environmentseer(1) as ViewResult;
            EnvironmentseerViewModel viewModel = (EnvironmentseerViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(EnvironmentseerViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void EnvironmentController_EnvironmentConfiguration_GET()
        {
            // Arrange:
            EnvironmentController controller = new EnvironmentController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.EnvironmentConfiguration(1) as ViewResult;
            EnvironmentConfigurationViewModel viewModel = (EnvironmentConfigurationViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(EnvironmentConfigurationViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void EnvironmentController_EnvironmentCreation_GET()
        {
            // Arrange:
            EnvironmentController controller = new EnvironmentController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.EnvironmentCreation() as ViewResult;
            EnvironmentCreationViewModel viewModel = (EnvironmentCreationViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(EnvironmentCreationViewModel)); // ViewResult has correct Model Type
        }
    }
}
