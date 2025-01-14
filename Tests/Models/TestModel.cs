using System;
using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestModel
    {
        private Model _model;

        [SetUp]
        public void SetUp()
        {
            _model = new Model();
        }

        [Test]
        public void Test_ModelPathProperty()
        {
            // Arrange
            var expectedPath = "path/to/model/file";

            // Act
            _model.Path = expectedPath;

            // Assert
            Assert.AreEqual(
                expectedPath,
                _model.Path,
                "The Path property should be set correctly."
            );
        }

        [Test]
        public void Test_ModelInheritsBaseProperties()
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
            _model.Id = expectedId;
            _model.PageId = expectedPageId;
            _model.ScaleX = expectedScaleX;
            _model.ScaleY = expectedScaleY;
            _model.SizeX = expectedSizeX;
            _model.SizeY = expectedSizeY;
            _model.PositionX = expectedPositionX;
            _model.PositionY = expectedPositionY;
            _model.Rotation = expectedRotation;
            _model.ZIndex = expectedZIndex;
            _model.IsMovable = expectedIsMovable;

            // Assert
            Assert.AreEqual(expectedId, _model.Id, "The Id property should be set correctly.");
            Assert.AreEqual(
                expectedPageId,
                _model.PageId,
                "The PageId property should be set correctly."
            );
            Assert.AreEqual(
                expectedScaleX,
                _model.ScaleX,
                "The ScaleX property should be set correctly."
            );
            Assert.AreEqual(
                expectedScaleY,
                _model.ScaleY,
                "The ScaleY property should be set correctly."
            );
            Assert.AreEqual(
                expectedSizeX,
                _model.SizeX,
                "The SizeX property should be set correctly."
            );
            Assert.AreEqual(
                expectedSizeY,
                _model.SizeY,
                "The SizeY property should be set correctly."
            );
            Assert.AreEqual(
                expectedPositionX,
                _model.PositionX,
                "The PositionX property should be set correctly."
            );
            Assert.AreEqual(
                expectedPositionY,
                _model.PositionY,
                "The PositionY property should be set correctly."
            );
            Assert.AreEqual(
                expectedRotation,
                _model.Rotation,
                "The Rotation property should be set correctly."
            );
            Assert.AreEqual(
                expectedZIndex,
                _model.ZIndex,
                "The ZIndex property should be set correctly."
            );
            Assert.AreEqual(
                expectedIsMovable,
                _model.IsMovable,
                "The IsMovable property should be set correctly."
            );
        }
    }
}
