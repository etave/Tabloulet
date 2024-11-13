using System;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes.Components.ButtonNS
{
    public partial class Button(
        string linkTo,
        string content,
        string color,
        float scaleY,
        float scaleX,
        float sizeX,
        float sizeY,
        float positionX,
        float positionY,
        float rotationDeg,
        int index,
        bool isMovable
    ) : Godot.Button, IComponent
    {
        private InputHandler _inputHandler;

        private Guid? _linkTo = string.IsNullOrEmpty(linkTo) ? null : Guid.Parse(linkTo);
        private string _content = content;
        private string _color = color;
        private float _scaleX = scaleX;
        private float _scaleY = scaleY;
        private float _sizeX = sizeX;
        private float _sizeY = sizeY;
        private float _positionX = positionX;
        private float _positionY = positionY;
        private float _rotationDeg = rotationDeg;
        private int _index = index;
        private bool _isMovable = isMovable;

        public Guid? LinkTo
        {
            get => _linkTo;
            set => _linkTo = value;
        }

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public string Color
        {
            get => _color;
            set => _color = value;
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

            if (!(_linkTo == null))
            {
                Base parent = GetParent<Base>();
                this.Pressed += () => parent.changePage((Guid)_linkTo);
            }

            if (String.IsNullOrEmpty(_content))
            {
                this.Text = "Bouton";
            }
            else
            {
                this.Text = _content;
            }
            StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
            if (String.IsNullOrEmpty(_color))
            {
                styleBoxFlat.BgColor = new Color("#FFFFFF");
            }
            else
            {
                styleBoxFlat.BgColor = new Color(_color);
            }
            styleBoxFlat.CornerRadiusBottomRight = 5;
            styleBoxFlat.CornerRadiusBottomLeft = 5;
            styleBoxFlat.CornerRadiusTopRight = 5;
            styleBoxFlat.CornerRadiusTopLeft = 5;
            this.AddThemeStyleboxOverride("normal", styleBoxFlat);
            this.AddThemeStyleboxOverride("hover", styleBoxFlat);
            this.AddThemeStyleboxOverride("pressed", styleBoxFlat);
            this.AddThemeStyleboxOverride("focus", styleBoxFlat);
            Color textColor = new("#000000");
            this.AddThemeColorOverride("font_color", textColor);
            this.AddThemeColorOverride("font_hover_color", textColor);
            this.AddThemeColorOverride("font_pressed_color", textColor);
            this.AddThemeColorOverride("font_focus_color", textColor);

            Size = new Vector2(_sizeX, _sizeY);
            Scale = new Vector2(_scaleX, _scaleY);
            Position = new Vector2(_positionX, _positionY);
            RotationDegrees = _rotationDeg;
            ZIndex = _index;
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
    }
}
