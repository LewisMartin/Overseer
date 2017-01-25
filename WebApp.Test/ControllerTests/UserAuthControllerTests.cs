using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Overseer.WebApp.Controllers;
using Moq;
using System.Web;
using System.Security.Claims;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class UserAuthConrollerTests : BaseControllerTests
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
            // Arrange
            UserAuthController controller = new UserAuthController();
            controller.ControllerContext = MockContextAdminUser.Object;

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
