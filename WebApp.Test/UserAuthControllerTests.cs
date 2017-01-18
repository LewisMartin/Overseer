using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Mvc;

using Overseer.WebApp;
using Overseer.WebApp.Controllers;
using Overseer.WebApp.DAL;
using Moq;
using System.Security.Principal;
using System.Web;
using Overseer.WebApp.DAL.Core;
using System.Security.Claims;
using Overseer.WebApp.ViewModels.Home;

namespace WebApp.Test
{
    [TestClass]
    public class UserAuthConrollerTests
    {
        // test unauthenticated user is returned the login page
        [TestMethod]
        public void UserAuthController_UnauthenticatedUserServedLogin()
        {
            // Arrange:
            // Mocking a 'ClaimsIdentity' for an unauthenticated user
            var Identity = new ClaimsIdentity();
            var Principal = new ClaimsPrincipal(Identity);

            // creating mock 'HttpContextBase' and passing fake user
            var MockHttpContext = new Mock<HttpContextBase>();
            MockHttpContext.Setup(t => t.User).Returns(Principal);
            // creating mock 'ControllerContext' and passing mocked 'HttpContext'
            var MockContext = new Mock<ControllerContext>();
            MockContext.Setup(t => t.HttpContext).Returns(MockHttpContext.Object);

            UserAuthController controller = new UserAuthController();
            controller.ControllerContext = MockContext.Object;

            // Act:
            ActionResult result = controller.Login() as ActionResult;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult vResult = (ViewResult)result;
            Assert.IsTrue(vResult.ViewName == "Login"); // assert login page is returned
        }

        // test that authenticated user accessing login page is redirected to homepage
        [TestMethod]
        public void UserAuthController_AuthenticatedUserLoginRedirect()
        {
            // Arrange:
            // Mocking a 'ClaimsIdentity' for a logged in user (simulating admin logging in)
            IList<Claim> MockClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var Identity = new ClaimsIdentity(MockClaims, "ApplicationCookie");
            var Principal = new ClaimsPrincipal(Identity);

            // creating mock 'HttpContextBase' and passing fake user
            var MockHttpContext = new Mock<HttpContextBase>();
            MockHttpContext.Setup(t => t.User).Returns(Principal);
            // creating mock 'ControllerContext' and passing mocked 'HttpContext'
            var MockContext = new Mock<ControllerContext>();
            MockContext.Setup(t => t.HttpContext).Returns(MockHttpContext.Object);

            UserAuthController controller = new UserAuthController();
            controller.ControllerContext = MockContext.Object;

            // Act:
            ActionResult result = controller.Login() as ActionResult;
            //HomeViewModel vModel = (HomeViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ActionResult is not null
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult)); // result is redirect

            RedirectToRouteResult routeResult = result as RedirectToRouteResult; // home page is correctly redirected to
            Assert.AreEqual(routeResult.RouteValues["controller"], "Home");
            Assert.AreEqual(routeResult.RouteValues["action"], "Index");
        }
    }
}
