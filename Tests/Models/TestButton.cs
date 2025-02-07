using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestButton
    {
        private Button _button;

        [SetUp]
        public void SetUp()
        {
            _button = new Button();
        }

        [Test]
        public void Test_ButtonLinkToProperty()
        {
            // Arrange
            var expectedLinkTo = Guid.NewGuid();

            // Act
            _button.LinkTo = expectedLinkTo;

            // Assert
            Assert.AreEqual(
                expectedLinkTo,
                _button.LinkTo,
                "The LinkTo property should be set correctly."
            );
        }

        [Test]
        public void Test_ButtonContentProperty()
        {
            // Arrange
            var expectedContent = "Click Me";

            // Act
            _button.Content = expectedContent;

            // Assert
            Assert.AreEqual(
                expectedContent,
                _button.Content,
                "The Content property should be set correctly."
            );
        }

        [Test]
        public void Test_ButtonColorProperty()
        {
            // Arrange
            var expectedColor = "#FF0000";

            // Act
            _button.Color = expectedColor;

            // Assert
            Assert.AreEqual(
                expectedColor,
                _button.Color,
                "The Color property should be set correctly."
            );
        }
    }
}
