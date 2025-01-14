using NUnit.Framework;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Tests.Models
{
    [TestFixture]
    internal class TestAudio
    {
        private Audio _audio;

        [SetUp]
        public void SetUp()
        {
            _audio = new Audio();
        }

        [Test]
        public void Test_AudioPathProperty()
        {
            // Arrange
            var expectedPath = "path/to/audio/file";

            // Act
            _audio.Path = expectedPath;

            // Assert
            Assert.AreEqual(
                expectedPath,
                _audio.Path,
                "The Path property should be set correctly."
            );
        }
    }
}
