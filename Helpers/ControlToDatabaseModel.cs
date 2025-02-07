using System;
using Godot;
using Tabloulet.DatabaseNS.Models;
using AudioComponent = Tabloulet.Scenes.Components.AudioNS.Audio;
using AudioModel = Tabloulet.DatabaseNS.Models.Audio;
using ButtonComponent = Tabloulet.Scenes.Components.ButtonNS.Button;
using ButtonModel = Tabloulet.DatabaseNS.Models.Button;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using TextModel = Tabloulet.DatabaseNS.Models.Text;
using VideoComponent = Tabloulet.Scenes.Components.VideoNS.Video;
using VideoModel = Tabloulet.DatabaseNS.Models.Video;

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
                VideoComponent video => CreateVideoModel(id, video),
                ButtonComponent button => CreateButtonModel(id, button),
                AudioComponent audio => CreateAudioModel(id, audio),
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

        private static VideoModel CreateVideoModel(Guid id, VideoComponent component)
        {
            return new VideoModel()
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
                Autoplay = component.AutoplayVideo,
                Loop = component.LoopVideo,
            };
        }

        private static AudioModel CreateAudioModel(Guid id, AudioComponent component)
        {
            return new AudioModel()
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
