using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestImage
    {
        private Image _image;

        [SetUp]
        public void SetUp()
        {
            _image = new Image();
        }

        [Test]
        public void Test_ImagePathProperty()
        {
            // Arrange
            var expectedPath = "path/to/image/file";

            // Act
            _image.Path = expectedPath;

            // Assert
            Assert.AreEqual(
                expectedPath,
                _image.Path,
                "The Path property should be set correctly."
            );
        }

        [Test]
        public void Test_ImageInheritsBaseProperties()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var expectedPageId = Guid.NewGuid();
            var expectedScaleX = 1.0f;
            var expectedScaleY = 1.0f;
            var expectedSizeX = 100.0f;
            var expectedSizeY = 100.0f;
            var expectedPositionX = 50.0f;
            var expectedPositionY = 50.0f;
            var expectedRotation = 0.0f;
            var expectedZIndex = 1;
            var expectedIsMovable = true;

            // Act
            _image.Id = expectedId;
            _image.PageId = expectedPageId;
            _image.ScaleX = expectedScaleX;
            _image.ScaleY = expectedScaleY;
            _image.SizeX = expectedSizeX;
            _image.SizeY = expectedSizeY;
            _image.PositionX = expectedPositionX;
            _image.PositionY = expectedPositionY;
            _image.Rotation = expectedRotation;
            _image.ZIndex = expectedZIndex;
            _image.IsMovable = expectedIsMovable;

            // Assert
            Assert.AreEqual(expectedId, _image.Id, "The Id property should be set correctly.");
            Assert.AreEqual(
                expectedPageId,
                _image.PageId,
                "The PageId property should be set correctly."
            );
            Assert.AreEqual(
                expectedScaleX,
                _image.ScaleX,
                "The ScaleX property should be set correctly."
            );
            Assert.AreEqual(
                expectedScaleY,
                _image.ScaleY,
                "The ScaleY property should be set correctly."
            );
            Assert.AreEqual(
                expectedSizeX,
                _image.SizeX,
                "The SizeX property should be set correctly."
            );
            Assert.AreEqual(
                expectedSizeY,
                _image.SizeY,
                "The SizeY property should be set correctly."
            );
            Assert.AreEqual(
                expectedPositionX,
                _image.PositionX,
                "The PositionX property should be set correctly."
            );
            Assert.AreEqual(
                expectedPositionY,
                _image.PositionY,
                "The PositionY property should be set correctly."
            );
            Assert.AreEqual(
                expectedRotation,
                _image.Rotation,
                "The Rotation property should be set correctly."
            );
            Assert.AreEqual(
                expectedZIndex,
                _image.ZIndex,
                "The ZIndex property should be set correctly."
            );
            Assert.AreEqual(
                expectedIsMovable,
                _image.IsMovable,
                "The IsMovable property should be set correctly."
            );
        }
    }
}
