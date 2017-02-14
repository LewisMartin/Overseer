using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overseer.WebApp.Controllers;
using System.Collections.Generic;
using System.Reflection;
using Overseer.WebApp.ViewModels.Machine;
using System.Web.Mvc;
using System.Linq;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class MachineControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void MachineController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            MachineController controller = new MachineController();
            List<MethodInfo> methodInformation = new List<MethodInfo>();

            // Act:
            Type type = controller.GetType();
            methodInformation.Add(type.GetMethod("MachineConfiguration", new Type[] { typeof(Guid) }));
            methodInformation.Add(type.GetMethod("MachineConfiguration", new Type[] { typeof(MachineConfigurationViewModel) }));
            methodInformation.Add(type.GetMethod("MachineCreation", new Type[] { typeof(int) }));
            methodInformation.Add(type.GetMethod("MachineCreation", new Type[] { typeof(MachineCreationViewModel) }));
            methodInformation.Add(type.GetMethod("MachineDeletion"));

            // Assert:
            foreach (var method in methodInformation)
            {
                var attributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                Assert.IsTrue(attributes.Any());
            }
        }

        [TestMethod]
        public void MachineController_Machineseer_GET()
        {
            // Arrange:
            MachineController controller = new MachineController();
            controller.ControllerContext = MockContextAdminUser.Object;
            Guid exampleMachineGuid = controller.GetExampleMachineGuid();

            // Act:
            ViewResult result = controller.Machineseer(exampleMachineGuid) as ViewResult;
            MachineseerViewModel viewModel = (MachineseerViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(MachineseerViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void MachineController_MachineConfiguration_GET()
        {
            // Arrange:
            MachineController controller = new MachineController();
            controller.ControllerContext = MockContextAdminUser.Object;
            Guid exampleMachineGuid = controller.GetExampleMachineGuid();

            // Act:
            ViewResult result = controller.MachineConfiguration(exampleMachineGuid) as ViewResult;
           MachineConfigurationViewModel viewModel = (MachineConfigurationViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(MachineConfigurationViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void MachineController_MachineCreation_GET()
        {
            // Arrange:
            MachineController controller = new MachineController();
            controller.ControllerContext = MockContextAdminUser.Object;
            Guid exampleMachineGuid = controller.GetExampleMachineGuid();

            // Act:
            ViewResult result = controller.MachineCreation(1) as ViewResult;
            MachineCreationViewModel viewModel = (MachineCreationViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(MachineCreationViewModel)); // ViewResult has correct Model Type
        }
    }
}
