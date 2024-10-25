using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes.Components.TextNS
{
    public partial class Text(
        string Content,
        string FontPath,
        int FontSize,
        float Height,
        float Width,
        float PositionX,
        float PositionY,
        float RotationDeg
    ) : RichTextLabel
    {
        private InputHandler _inputHandler;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (!string.IsNullOrEmpty(FontPath))
            {
                FontFile font = new();
                Error loadError = font.LoadDynamicFont(FontPath);
                if (loadError != Error.Ok || font.Data.Length == 0)
                {
                    GD.PrintErr($"Error loading font: {FontPath}");
                    // TODO: Inform the user about the error in a more user-friendly way
                }
                else
                {
                    AddThemeFontOverride("normal_font", font);
                }
            }

            BbcodeEnabled = true;
            Text = Content;
            AddThemeFontSizeOverride("normal_font_size", FontSize);
            Size = new Vector2(Width, Height);
            Position = new Vector2(PositionX, PositionY);
            PivotOffset = Size / 2;
            RotationDegrees = RotationDeg;
        }

        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            ScrollActive = false;
        }
    }
}
