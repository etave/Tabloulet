using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestScenario
    {
        private Scenario _scenario;

        [SetUp]
        public void SetUp()
        {
            _scenario = new Scenario();
        }

        [Test]
        public void Test_ScenarioIdProperty()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            // Act
            _scenario.Id = expectedId;

            // Assert
            Assert.AreEqual(expectedId, _scenario.Id, "The Id property should be set correctly.");
        }

        [Test]
        public void Test_ScenarioPageIdProperty()
        {
            // Arrange
            var expectedPageId = Guid.NewGuid();

            // Act
            _scenario.PageId = expectedPageId;

            // Assert
            Assert.AreEqual(
                expectedPageId,
                _scenario.PageId,
                "The PageId property should be set correctly."
            );
        }

        [Test]
        public void Test_ScenarioNameProperty()
        {
            // Arrange
            var expectedName = "Scenario1";

            // Act
            _scenario.Name = expectedName;

            // Assert
            Assert.AreEqual(
                expectedName,
                _scenario.Name,
                "The Name property should be set correctly."
            );
        }

        [Test]
        public void Test_ScenarioDescriptionProperty()
        {
            // Arrange
            var expectedDescription = "This is a test scenario.";

            // Act
            _scenario.Description = expectedDescription;

            // Assert
            Assert.AreEqual(
                expectedDescription,
                _scenario.Description,
                "The Description property should be set correctly."
            );
        }
    }
}
