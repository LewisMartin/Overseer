using Moq;
using System.Web;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class BaseControllerTests
    {
        protected Mock<ControllerContext> MockContextAdminUser;
        protected Mock<ControllerContext> MockContextQAUser;

        [TestInitialize]
        public void Initialize()
        {
            CreateMockContextAdminUser();
            CreateMockContextQAUser();
        }

        private ClaimsPrincipal CreateMockUser(int userId, string userRole)
        {
            IList<Claim> MockClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };

            var Identity = new ClaimsIdentity(MockClaims, "TestAuthType");
            var Principal = new ClaimsPrincipal(Identity);

            return Principal;
        }

        private void CreateMockContextAdminUser()
        {
            // creating mock 'HttpContextBase' and passing fake user
            var MockHttpContext = new Mock<HttpContextBase>();
            MockHttpContext.Setup(t => t.User).Returns(CreateMockUser(1, "Administrator"));

            // providing mock application path
            var MockHttpRequestContext = new Mock<HttpRequestBase>();
            MockHttpRequestContext.Setup(r => r.Url).Returns(new System.Uri("http://localhost/"));
            MockHttpRequestContext.Setup(r => r.ApplicationPath).Returns("/");
            MockHttpContext.Setup(r => r.Request).Returns(MockHttpRequestContext.Object);

            // creating mock 'ControllerContext' and passing mocked 'HttpContext'
            MockContextAdminUser = new Mock<ControllerContext>();
            MockContextAdminUser.Setup(t => t.HttpContext).Returns(MockHttpContext.Object);
        }

        private void CreateMockContextQAUser()
        {
            // creating mock 'HttpContextBase' and passing fake user
            var MockHttpContext = new Mock<HttpContextBase>();
            MockHttpContext.Setup(t => t.User).Returns(CreateMockUser(2, "QA"));

            // providing mock application path
            var MockHttpRequestContext = new Mock<HttpRequestBase>();
            MockHttpRequestContext.Setup(r => r.Url).Returns(new System.Uri("http://localhost/"));
            MockHttpRequestContext.Setup(r => r.ApplicationPath).Returns("/");
            MockHttpContext.Setup(r => r.Request).Returns(MockHttpRequestContext.Object);

            // creating mock 'ControllerContext' and passing mocked 'HttpContext'
            MockContextQAUser = new Mock<ControllerContext>();
            MockContextQAUser.Setup(t => t.HttpContext).Returns(MockHttpContext.Object);
        }
    }
}
