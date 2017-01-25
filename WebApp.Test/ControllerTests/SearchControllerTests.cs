using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Overseer.WebApp.Controllers;
using System.Web.Mvc;
using System.Linq;
using Overseer.WebApp.ViewModels.Search;

namespace WebApp.Test.ControllerTests
{
    [TestClass]
    public class SearchControllerTests : BaseControllerTests
    {
        [TestMethod]
        public void SearchController_AuthorizeAttributeExists() // use reflection to check for authorize attribute
        {
            // Arrange:
            SearchController controller = new SearchController();

            // Act:
            Type type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [TestMethod]
        public void SearchController_Index_GET()
        {
            // Arrange:
            SearchController controller = new SearchController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            ViewResult result = controller.Index() as ViewResult;
            SearchViewModel viewModel = (SearchViewModel)result.Model;

            // Assert:
            Assert.IsNotNull(result); // ViewResult is not null
            Assert.IsInstanceOfType(result.Model, typeof(SearchViewModel)); // ViewResult has correct Model Type
        }

        [TestMethod]
        public void SearchController_Query_GET()
        {
            // Arrange
           SearchController controller = new SearchController();
            controller.ControllerContext = MockContextAdminUser.Object;

            // Act:
            PartialViewResult result = controller._Query(new SearchViewModel()
            {
                SearchTerm = "Test",
                SearchType = "1",
            }) as PartialViewResult;

            // Assert:
            Assert.IsNotNull(result); // Json is not null
            Assert.IsInstanceOfType(result.Model, typeof(_QueryViewModel)); // ViewResult has correct Model Type
        }
    }
}
