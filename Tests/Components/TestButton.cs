using Godot;
using NUnit.Framework;

namespace Tabloulet.Tests.Scenes.Components.ButtonNS
{
    [TestFixture]
    public class TestButton
    {
        [Test]
        public void Test_IsColorDark()
        {
            // Arrange
            Color darkColor = new Color("#000000"); // Noir
            Color lightColor = new Color("#FFFFFF"); // Blanc
            Color mediumColor = new Color("#808080"); // Gris

            // Act
            bool isDarkColorDark = Tabloulet.Scenes.Components.ButtonNS.Button.IsColorDark(
                darkColor
            );
            bool isLightColorDark = Tabloulet.Scenes.Components.ButtonNS.Button.IsColorDark(
                lightColor
            );
            bool isMediumColorDark = Tabloulet.Scenes.Components.ButtonNS.Button.IsColorDark(
                mediumColor
            );

            // Assert
            Assert.IsTrue(
                isDarkColorDark,
                "La couleur noire devrait être considérée comme sombre."
            );
            Assert.IsFalse(
                isLightColorDark,
                "La couleur blanche ne devrait pas être considérée comme sombre."
            );
            Assert.IsFalse(
                isMediumColorDark,
                "La couleur grise ne devrait pas être considérée comme sombre."
            );
        }
    }
}
