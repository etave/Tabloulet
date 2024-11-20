using System;
using Godot;
using Tabloulet.DatabaseNS.Models;
using ButtonComponent = Tabloulet.Scenes.Components.ButtonNS.Button;
using ButtonModel = Tabloulet.DatabaseNS.Models.Button;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using TextModel = Tabloulet.DatabaseNS.Models.Text;

namespace Tabloulet.Helpers
{
    public static class ControlToDatabaseModel
    {
        public static IDatabaseModelComponent ConvertToDatabaseModel(Guid id, Control component)
        {
            return component switch
            {
                TextComponent text => CreateTextModel(id, text),
                ImageComponent image => CreateImageModel(id, image),
                ButtonComponent button => CreateButtonModel(id, button),
                _ => null,
            };
        }

        private static TextModel CreateTextModel(Guid id, TextComponent component)
        {
            return new TextModel()
            {
                Id = id,
                Content = component.Content,
                Font = component.FontPath,
                FontSize = component.FontSize,
                ScaleX = component.ScaleX,
                ScaleY = component.ScaleY,
                SizeX = component.SizeX,
                SizeY = component.SizeY,
                PositionX = component.PositionX,
                PositionY = component.PositionY,
                Rotation = component.RotationDeg,
                ZIndex = component.Index,
                IsMovable = component.IsMovable,
            };
        }

        private static ImageModel CreateImageModel(Guid id, ImageComponent component)
        {
            return new ImageModel()
            {
                Id = id,
                Path = component.Path,
                ScaleX = component.ScaleX,
                ScaleY = component.ScaleY,
                SizeX = component.SizeX,
                SizeY = component.SizeY,
                PositionX = component.PositionX,
                PositionY = component.PositionY,
                Rotation = component.RotationDeg,
                ZIndex = component.Index,
                IsMovable = component.IsMovable,
            };
        }

        private static ButtonModel CreateButtonModel(Guid id, ButtonComponent component)
        {
            return new ButtonModel()
            {
                Id = id,
                LinkTo = component.LinkTo,
                Content = component.Content,
                Color = component.Color,
                ScaleX = component.ScaleX,
                ScaleY = component.ScaleY,
                SizeX = component.SizeX,
                SizeY = component.SizeY,
                PositionX = component.PositionX,
                PositionY = component.PositionY,
                Rotation = component.RotationDeg,
                ZIndex = component.Index,
                IsMovable = component.IsMovable,
            };
        }
    }
}
