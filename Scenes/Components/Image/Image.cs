using System;
using Godot;
using Tabloulet.Helpers;
using GDImage = Godot.Image;

namespace Tabloulet.Scenes.Components.ImageNS
{
    public partial class Image(
        string Path,
        float Height,
        float Width,
        float PositionX,
        float PositionY,
        float RotationDeg
    ) : TextureRect
    {
        private InputHandler _inputHandler;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (!string.IsNullOrEmpty(Path))
            {
                LoadImage(Path);
            }
            else
            {
                SetPlaceholderTexture();
            }

            Position = new Vector2(PositionX, PositionY);
            PivotOffset = Size / 2;
            RotationDegrees = RotationDeg;
        }

        private void LoadImage(string path)
        {
            GDImage image = new();
            Error loadError = image.Load(path);
            if (loadError != Error.Ok)
            {
                GD.PrintErr($"Error loading image: {path}");
                // TODO: Inform the user about the error in a more user-friendly way
                SetPlaceholderTexture();
            }
            else
            {
                ImageTexture imageTexture = new();
                imageTexture.SetImage(image);
                Texture = imageTexture;

                ExpandMode = ExpandModeEnum.IgnoreSize;
                float widthRatio = Width / image.GetWidth();
                float heightRatio = Height / image.GetHeight();
                float ratio = Math.Min(widthRatio, heightRatio);
                Size = new Vector2(image.GetWidth() * ratio, image.GetHeight() * ratio);
            }
        }

        private void SetPlaceholderTexture()
        {
            Texture = new PlaceholderTexture2D();
            Size = new Vector2(Width, Height);
        }
    }
}
