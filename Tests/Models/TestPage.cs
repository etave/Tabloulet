using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestPage
    {
        private Page _page;

        [SetUp]
        public void SetUp()
        {
            _page = new Page();
        }

        [Test]
        public void Test_PageIdProperty()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            // Act
            _page.Id = expectedId;

            // Assert
            Assert.AreEqual(expectedId, _page.Id, "The Id property should be set correctly.");
        }

        [Test]
        public void Test_PageNameProperty()
        {
            // Arrange
            var expectedName = "HomePage";

            // Act
            _page.Name = expectedName;

            // Assert
            Assert.AreEqual(expectedName, _page.Name, "The Name property should be set correctly.");
        }

        [Test]
        public void Test_PageBackgroundColorProperty()
        {
            // Arrange
            var expectedBackgroundColor = "#FFFFFF";

            // Act
            _page.BackgroundColor = expectedBackgroundColor;

            // Assert
            Assert.AreEqual(
                expectedBackgroundColor,
                _page.BackgroundColor,
                "The BackgroundColor property should be set correctly."
            );
        }

        [Test]
        public void Test_PageIsTemplateProperty()
        {
            // Arrange
            var expectedIsTemplate = true;

            // Act
            _page.IsTemplate = expectedIsTemplate;

            // Assert
            Assert.AreEqual(
                expectedIsTemplate,
                _page.IsTemplate,
                "The IsTemplate property should be set correctly."
            );
        }
    }
}
