using System;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;
using GDImage = Godot.Image;

namespace Tabloulet.Scenes.Components.ImageNS
{
    public partial class Image(
        string path,
        float scaleX,
        float scaleY,
        float sizeX,
        float sizeY,
        float positionX,
        float positionY,
        float rotationDeg,
        bool isMovable
    ) : TextureRect, IComponent
    {
        private InputHandler _inputHandler;

        private string _path = path;
        private float _scaleX = scaleX;
        private float _scaleY = scaleY;
        private float _sizeX = sizeX;
        private float _sizeY = sizeY;
        private float _positionX = positionX;
        private float _positionY = positionY;
        private float _rotationDeg = rotationDeg;
        private bool _isMovable = isMovable;

        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public float ScaleX
        {
            get => _scaleX;
            set => _scaleX = value;
        }

        public float ScaleY
        {
            get => _scaleY;
            set => _scaleY = value;
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

            if (!string.IsNullOrEmpty(_path))
            {
                LoadImage(_path);
            }
            else
            {
                SetPlaceholderTexture();
            }

            Position = new Vector2(_positionX, _positionY);
            PivotOffset = Size / 2;
            RotationDegrees = _rotationDeg;
            Size = new Vector2(_sizeX, _sizeY);
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
                Scale = new Vector2(_scaleX, _scaleY);
            }
            else
            {
                ImageTexture imageTexture = new();
                imageTexture.SetImage(image);
                Texture = imageTexture;

                ExpandMode = ExpandModeEnum.IgnoreSize;
                float widthRatio = _scaleX / image.GetWidth();
                float heightRatio = _scaleY / image.GetHeight();
                float ratio = Math.Min(widthRatio, heightRatio);
                Scale = new Vector2(image.GetWidth() * ratio, image.GetHeight() * ratio);
            }
        }

        private void SetPlaceholderTexture()
        {
            Texture = new PlaceholderTexture2D();
            Scale = new Vector2(_scaleX, ScaleY);
        }

        public void UpdateSizePositionRotationParameters(
            float scaleX,
            float scaleY,
            float sizeX,
            float sizeY,
            float positionX,
            float positionY,
            float rotationDeg
        )
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
            SizeX = sizeX;
            SizeY = sizeY;
            PositionX = positionX;
            PositionY = positionY;
            RotationDeg = rotationDeg;
        }
    }
}
