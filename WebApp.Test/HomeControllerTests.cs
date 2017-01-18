using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using Overseer.WebApp.Controllers;
using Moq;
using System.Web;
using System.Security.Claims;
using Overseer.WebApp.ViewModels.Home;

namespace WebApp.Test
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void HomeController_Index()
        {
            // Arrange:
            // Mocking a 'ClaimsIdentity' for a logged in user (simulating admin logging in)
            IList<Claim> MockClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var Identity = new ClaimsIdentity(MockClaims, "TestAuthType");
            var Principal = new ClaimsPrincipal(Identity);

            // creating mock 'HttpContextBase' and passing fake user
            var MockHttpContext = new Mock<HttpContextBase>();
            MockHttpContext.Setup(t => t.User).Returns(Principal);
            // creating mock 'ControllerContext' and passing mocked 'HttpContext'
            var MockContext = new Mock<ControllerContext>();
            MockContext.Setup(t => t.HttpContext).Returns(MockHttpContext.Object);

            HomeController controller = new HomeController();
            controller.ControllerContext = MockContext.Object;

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
