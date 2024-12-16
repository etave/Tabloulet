using System;
using Godot;
using Tabloulet.DatabaseNS.Models;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using TextModel = Tabloulet.DatabaseNS.Models.Text;
using Model3DComponent = Tabloulet.Scenes.Components.Model3DNS.Model3D;
using Model3DModel = Tabloulet.DatabaseNS.Models.Model;

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
                Model3DComponent model3D => CreateModel3DModel(id, model3D),
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

        private static Model3DModel CreateModel3DModel(Guid id, Model3DComponent component)
        {
            return new Model3DModel()
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

    }
}
