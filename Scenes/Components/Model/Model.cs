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
        private bool _hasModel;
        private Node3D _model;





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

            _subViewport = new SubViewport
            {
                TransparentBg = true,
                Size = new Vector2I((int)_sizeX, (int)_sizeY),
                OwnWorld3D = true,
            };
            AddChild(_subViewport);

            // Cr�ation de la cam�ra 3D
            _camera = new Camera3D
            {
                Position = new Vector3(0, 2, 5),
                Current = true
            };

            // Ajout de la cam�ra au SubViewport
            _subViewport.AddChild(_camera);

            // Ajout des lumi�res directionnelles autour du mod�le
            AddDirectionalLight(new Vector3(0, 2, 0), new Vector3(-Mathf.Pi / 4, 0, 0), 1.0f); // Haut
            AddDirectionalLight(new Vector3(0, -2, 0), new Vector3(Mathf.Pi / 4, 0, 0), 1.0f); // Bas
            AddDirectionalLight(new Vector3(2, 0, 0), new Vector3(0, -Mathf.Pi / 4, 0), 1.0f); // Droite
            AddDirectionalLight(new Vector3(-2, 0, 0), new Vector3(0, Mathf.Pi / 4, 0), 1.0f); // Gauche
            AddDirectionalLight(new Vector3(0, 0, 2), new Vector3(0, 0, -Mathf.Pi / 4), 1.0f); // Devant
            AddDirectionalLight(new Vector3(0, 0, -2), new Vector3(0, 0, Mathf.Pi / 4), 1.0f); // Derri�re

            Texture = _subViewport.GetTexture();

            LoadModel(_path);

            // Param�tres d'affichage
            Position = new Vector2(_positionX, _positionY);
            PivotOffset = Size / 2;
            RotationDegrees = _rotationDeg;
            Size = new Vector2(_sizeX, _sizeY);
            ZIndex = _index;
        }

        private void AddDirectionalLight(Vector3 position, Vector3 rotation, float intensity)
        {
            DirectionalLight3D light = new DirectionalLight3D
            {
                Position = position,
                Rotation = rotation,
                LightEnergy = intensity // Ajuster l'intensit� de la lumi�re
            };
            _subViewport.AddChild(light);
        }


        private void LoadModel(string path)
        {
            if (_hasModel)
            {
                _subViewport.GetChild(_subViewport.GetChildCount() - 1).QueueFree();
            }

            if (string.IsNullOrEmpty(path))
            {
                path = "res://Assets/Components/Box.glb";
            }
            else
            {
                path = IOPath.Combine(Constants.AppPath, path);
            }

            GD.Print($"Loading model: {path}");

            var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Erreur d'ouverture du fichier: {path}");
                return;
            }

            var bytes = file.GetBuffer((long)file.GetLength());

            var gltfDocument = new Godot.GltfDocument();
            var gltfState = new Godot.GltfState();
            var error = gltfDocument.AppendFromBuffer(bytes, path, gltfState);

            if (error != Error.Ok)
            {
                GD.PrintErr($"Error loading GLTF model: {path} - {error}");
                return;
            }

            var model = gltfDocument.GenerateScene(gltfState) as Node3D;
            if (model == null)
            {
                GD.PrintErr("Failed to generate model scene.");
                return;
            }

            _model = model;

            _subViewport.AddChild(model);
            _hasModel = true;

            AdjustModelView(model);

            GD.Print($"Model loaded: {path}");
        }

        private void AdjustModelView(Node3D model)
        {
            Aabb boundingBox = new Aabb();

            bool hasMesh = false;

            foreach (Node child in model.GetChildren())
            {
                if (child is MeshInstance3D meshInstance)
                {
                    if (!hasMesh)
                    {
                        boundingBox = meshInstance.GetAabb();
                        hasMesh = true;
                    }
                    else
                    {
                        boundingBox = boundingBox.Merge(meshInstance.GetAabb());
                    }
                }
            }

            if (!hasMesh)
            {
                GD.PrintErr("No MeshInstance3D found in the model.");
                return;
            }

            Vector3 modelSize = boundingBox.Size;
            float maxSize = Mathf.Max(modelSize.X, modelSize.Y);
            maxSize = Mathf.Max(maxSize, modelSize.Z);

            // V�rifier si le mod�le est trop grand
            if (maxSize > 2.0f) // Seuil arbitraire (peut �tre ajust�)
            {
                // Reculer la cam�ra proportionnellement � la taille
                float distance = maxSize * 1.5f;
                _camera.Position = new Vector3(0, maxSize / 2.0f, distance);
                _camera.LookAt(Vector3.Zero);
            }
            else if (maxSize < 1.0f)
            {
                // Si le mod�le est trop petit, on l'agrandit
                float scaleFactor = 2.0f / maxSize;
                model.Scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
            else
            {
                // Ajuster la cam�ra pour que le mod�le soit bien cadr�
                float distance = maxSize * 1.5f;
                _camera.Position = new Vector3(0, maxSize / 2.0f, distance);
                _camera.LookAt(Vector3.Zero);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            if (_model != null)
            {
                // Ici, rotationSpeed est en radians par seconde.
                float rotationSpeed = 0.5f;
                _model.RotateY(rotationSpeed * (float)delta);
            }
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
