using System;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Helpers.CustomInputEvents;
using Tabloulet.Scenes.Components.BaseNS;
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
        private Base _base;
        private InputHandler _inputHandler;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _base = GetParent<Base>();
            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (Path != null)
            {
                GDImage image = new();
                Error loadError = image.Load(Path);
                if (loadError != Error.Ok)
                {
                    GD.PrintErr($"Error loading image: {loadError}");
                    // TODO: Inform the user about the error in a more user-friendly way
                    QueueFree();
                    return;
                }
                ImageTexture imageTexture = new();
                imageTexture.SetImage(image);
                Texture = imageTexture;

                ExpandMode = ExpandModeEnum.IgnoreSize;
                float widthRatio = Width / image.GetWidth();
                float heightRatio = Height / image.GetHeight();
                float ratio = Math.Min(widthRatio, heightRatio);
                Size = new Vector2(image.GetWidth() * ratio, image.GetHeight() * ratio);
            }
            else
            {
                Texture = new PlaceholderTexture2D();
                Size = new Vector2(Width, Height);
            }
            Position = new Vector2(PositionX, PositionY);
            RotationDegrees = RotationDeg;
        }
    }
}
