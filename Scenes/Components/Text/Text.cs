using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes.Components.TextNS
{
    public partial class Text(
        string content,
        string fontPath,
        int fontSize,
        float scaleY,
        float scaleX,
        float sizeX,
        float sizeY,
        float positionX,
        float positionY,
        float rotationDeg,
        int index,
        bool isMovable
    ) : RichTextLabel, IComponent
    {
        private InputHandler _inputHandler;

        private string _content = content;
        private string _fontPath = fontPath;
        private int _fontSize = fontSize;
        private float _scaleX = scaleX;
        private float _scaleY = scaleY;
        private float _sizeX = sizeX;
        private float _sizeY = sizeY;
        private float _positionX = positionX;
        private float _positionY = positionY;
        private float _rotationDeg = rotationDeg;
        private int _index = index;
        private bool _isMovable = isMovable;

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public string FontPath
        {
            get => _fontPath;
            set => _fontPath = value;
        }

        public int FontSize
        {
            get => _fontSize;
            set => _fontSize = value;
        }

        public float ScaleX
        {
            get => _scaleY;
            set => _scaleY = value;
        }

        public float ScaleY
        {
            get => _scaleX;
            set => _scaleX = value;
        }

        public float SizeX
        {
            get => _sizeX;
            set => _sizeX = value;
        }

        public float SizeY
        {
            get => _sizeY;
            set => _sizeY = value;
        }

        public float PositionX
        {
            get => _positionX;
            set => _positionX = value;
        }

        public float PositionY
        {
            get => _positionY;
            set => _positionY = value;
        }

        public float RotationDeg
        {
            get => _rotationDeg;
            set => _rotationDeg = value;
        }

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public bool IsMovable
        {
            get => _isMovable;
            set
            {
                _isMovable = value;
                Base parent = GetParent<Base>();
                parent.IsMovable = value;
            }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (!string.IsNullOrEmpty(_fontPath))
            {
                FontFile font = new();
                Error loadError = font.LoadDynamicFont(_fontPath);
                if (loadError != Error.Ok || font.Data.Length == 0)
                {
                    GD.PrintErr($"Error loading font: {_fontPath}");
                    // TODO: Inform the user about the error in a more user-friendly way
                }
                else
                {
                    AddThemeFontOverride("normal_font", font);
                }
            }

            BbcodeEnabled = true;
            Text = _content;
            AddThemeFontSizeOverride("normal_font_size", _fontSize);
            Size = new Vector2(_sizeX, _sizeY);
            Scale = new Vector2(_scaleX, _scaleY);
            Position = new Vector2(_positionX, _positionY);
            PivotOffset = Size / 2;
            RotationDegrees = _rotationDeg;
            ZIndex = _index;
        }

        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            ScrollActive = false;
        }

        public void UpdateSizePositionRotationParameters(
            float scaleX,
            float scaleY,
            float sizeX,
            float sizeY,
            float positionX,
            float positionY,
            float rotationDeg,
            int index
        )
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
            SizeX = sizeX;
            SizeY = sizeY;
            PositionX = positionX;
            PositionY = positionY;
            RotationDeg = rotationDeg;
            Index = index;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            ScrollActive = true;
        }
    }
}
