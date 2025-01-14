using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestScenarioPage
    {
        private ScenarioPage _scenarioPage;

        [SetUp]
        public void SetUp()
        {
            _scenarioPage = new ScenarioPage();
        }

        [Test]
        public void Test_ScenarioPageIdProperty()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            // Act
            _scenarioPage.Id = expectedId;

            // Assert
            Assert.AreEqual(
                expectedId,
                _scenarioPage.Id,
                "The Id property should be set correctly."
            );
        }

        [Test]
        public void Test_ScenarioPagePageIdProperty()
        {
            // Arrange
            var expectedPageId = Guid.NewGuid();

            // Act
            _scenarioPage.PageId = expectedPageId;

            // Assert
            Assert.AreEqual(
                expectedPageId,
                _scenarioPage.PageId,
                "The PageId property should be set correctly."
            );
        }

        [Test]
        public void Test_ScenarioPageScenarioIdProperty()
        {
            // Arrange
            var expectedScenarioId = Guid.NewGuid();

            // Act
            _scenarioPage.ScenarioId = expectedScenarioId;

            // Assert
            Assert.AreEqual(
                expectedScenarioId,
                _scenarioPage.ScenarioId,
                "The ScenarioId property should be set correctly."
            );
        }
    }
}
