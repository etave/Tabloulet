using System;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;
using IOPath = System.IO.Path;

namespace Tabloulet.Scenes.Components.Model3DNS
{
    public partial class Model3D : TextureRect, IComponent
    {
        private string _path;
        private float _scaleX, _scaleY, _sizeX, _sizeY, _positionX, _positionY, _rotationDeg;
        private int _index;
        private bool _isMovable;
        private InputHandler _inputHandler;

        public Model3D(
            string path,
            float scaleX,
            float scaleY,
            float sizeX,
            float sizeY,
            float positionX,
            float positionY,
            float rotationDeg,
            int index,
            bool isMovable
        )
        {
            _path = path;
            _scaleX = scaleX;
            _scaleY = scaleY;
            _sizeX = sizeX;
            _sizeY = sizeY;

            _positionX = positionX;
            _positionY = positionY;
            _rotationDeg = rotationDeg;
            _index = index;
            _isMovable = isMovable;
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                LoadModel(value);
            }
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

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public bool IsMovable
        {
            get => _isMovable;
            set => _isMovable = value;
        }

        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (!string.IsNullOrEmpty(_path))
            {
                LoadModel(_path);
            }
            else
            {
                SetPlaceholderTexture();
            }

            Position = new Vector2(_positionX, _positionY);
            PivotOffset = Size / 2;
            RotationDegrees = _rotationDeg;
            Size = new Vector2(_sizeX, _sizeY);
            ZIndex = _index;
        }

        private void LoadModel(string path)
        {
            var modelPath = IOPath.Combine(Constants.AppPath, path);
            var meshResource = GD.Load<Mesh>(modelPath);

            if (meshResource != null)
            {
                GD.Print("Model loaded successfully");
            }
            else
            {
                GD.PrintErr($"Error loading model: {modelPath}");
                SetPlaceholderTexture();
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
