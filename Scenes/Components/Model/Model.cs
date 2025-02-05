using System;
using System.IO;
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
        private SubViewport _subViewport;
        private Camera3D _camera;




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

            // Chargement du modele 3D si un chemin est defini
            if (!string.IsNullOrEmpty(_path))
            {
                LoadModel(_path);
            }
            else
            {
                SetPlaceholderModele();
            }

            // Parametres d'affichage
            Position = new Vector2(_positionX, _positionY);
            PivotOffset = Size / 2;
            RotationDegrees = _rotationDeg;
            Size = new Vector2(_sizeX, _sizeY);
            ZIndex = _index;
        }

        private void LoadModel(string path)
        {

            // Creation du SubViewport
            _subViewport = new()
            {
                TransparentBg = true,
                Size = new Vector2I((int)_sizeX, (int)_sizeY)
            };
            AddChild(_subViewport);

            // Creation de la camera 3D
            _camera = new Camera3D
            {
                Position = new Vector3(0, 1, 3),
                Current = true
            };

            // Ajout de la camera au SubViewport
            _subViewport.AddChild(_camera);

            // Ajout de la lumiere directionnelle au SubViewport vers le haut en dessous du model
            DirectionalLight3D directionalLight3D = new DirectionalLight3D();
            _subViewport.AddChild(directionalLight3D);

            // Definition du rendu du SubViewport comme texture du TextureRect
            Texture = _subViewport.GetTexture();

            /*            var gltfDocument = new GltfDocument();
                        var gltfState = new GltfState();

                        var modelPath = IOPath.Combine(Constants.AppPath, path);
                        Error error = gltfDocument.AppendFromFile(modelPath, gltfState);*/

            //file = Fileaccess open

            var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            var bytes = file.GetBuffer((long)file.GetLength());

            var gltfDocument = new Godot.GltfDocument();
            var gltfState = new Godot.GltfState();
            var modelPath = IOPath.Combine(Constants.AppPath, path);
            var error = gltfDocument.AppendFromBuffer(bytes, modelPath, gltfState);

            var model = gltfDocument.GenerateScene(gltfState);

            

            _subViewport.AddChild(model);


            /*
                        if (error != Error.Ok)
                        {
                            GD.PrintErr($"Error loading GLTF model: {modelPath} - {error}");
                            //SetPlaceholderModele();
                            return;
                        }*/
            /*
                        // Generer la scene du modele a partir du fichier GLTF
                        Node3D model = gltfDocument.GenerateScene(gltfState) as Node3D;
            */
            /*            if (model == null)
                        {
                            GD.PrintErr("Failed to generate model scene.");
                            SetPlaceholderModele();
                            return;
                        }*/

            // Ajouter le modele au SubViewport
            _subViewport.AddChild(model);

            GD.Print($"Model loaded: {modelPath}");
        }






        private void SetPlaceholderModele()
        {
            // Definir le chemin du modele 3D par defaut
            string defaultModelPath = "Assets/Components/Box.glb";
            GD.Print($"Default model path: {defaultModelPath}");

            if (!File.Exists(defaultModelPath))
            {
                GD.PrintErr($"Fichier modele 3D par defaut introuvable : {defaultModelPath}");
                return;
            }

            // Essayer de charger ce modele par defaut
            LoadModel(defaultModelPath);


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
