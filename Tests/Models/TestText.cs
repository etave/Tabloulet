using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestText
    {
        private Text _text;

        [SetUp]
        public void SetUp()
        {
            _text = new Text();
        }

        [Test]
        public void Test_TextContentProperty()
        {
            // Arrange
            var expectedContent = "Hello, World!";

            // Act
            _text.Content = expectedContent;

            // Assert
            Assert.AreEqual(
                expectedContent,
                _text.Content,
                "The Content property should be set correctly."
            );
        }

        [Test]
        public void Test_TextFontProperty()
        {
            // Arrange
            var expectedFont = "Arial";

            // Act
            _text.Font = expectedFont;

            // Assert
            Assert.AreEqual(expectedFont, _text.Font, "The Font property should be set correctly.");
        }

        [Test]
        public void Test_TextFontSizeProperty()
        {
            // Arrange
            var expectedFontSize = 12;

            // Act
            _text.FontSize = expectedFontSize;

            // Assert
            Assert.AreEqual(
                expectedFontSize,
                _text.FontSize,
                "The FontSize property should be set correctly."
            );
        }
    }
}
