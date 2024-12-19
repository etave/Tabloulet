using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes.Components.AudioNS
{
    public partial class Audio() : Panel, IComponent
    {
        public const float MIN_X_SIZE = 712;
        public const float MIN_Y_SIZE = 82;

        private InputHandler _inputHandler;

        private string _path;
        private float _scaleX;
        private float _scaleY;
        private float _sizeX;
        private float _sizeY;
        private float _positionX;
        private float _positionY;
        private float _rotationDeg;
        private int _index;
        private bool _isMovable;

        private AudioStreamPlayer _audioStreamPlayer = new();
        private Button _resetButton;
        private Button _playPauseButton;
        private CompressedTexture2D _playIcon;
        private CompressedTexture2D _pauseIcon;
        private StyleBoxFlat _styleBoxPlayPauseBtnNormal;
        private StyleBoxFlat _styleBoxPlayPauseBtnHover;
        private StyleBoxFlat _styleBoxPlayPauseBtnPressed;
        private HSlider _volumeSlider;
        private HSlider _progressionSlider = new();
        private bool _isPlaying = false;
        private bool _isEnded = false;
        private bool _isDragging = false;
        private float _maxPosition = 0f; // Durée totale de l'audio
        private float _targetPlaybackPosition = 0f; // Position de lecture souhaitée

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                LoadAudio(value);
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
            set
            {
                if (value < MIN_X_SIZE)
                {
                    _sizeX = MIN_X_SIZE;
                }
                else
                {
                    _sizeX = value;
                }
            }
        }

        public float SizeY
        {
            get => _sizeY;
            set
            {
                if (value < MIN_Y_SIZE)
                {
                    _sizeY = MIN_Y_SIZE;
                }
                else
                {
                    _sizeY = value;
                }
            }
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

        public override void _Ready()
        {
            // Références aux nœuds
            _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
            _resetButton = GetNode<Button>("HBoxContainer/ResetButton");
            _playPauseButton = GetNode<Button>("HBoxContainer/PlayPauseButton");
            _volumeSlider = GetNode<HSlider>("HBoxContainer/MarginContainer/VBoxContainer/Volume");
            _progressionSlider = GetNode<HSlider>(
                "HBoxContainer/MarginContainer/VBoxContainer/Progression"
            );

            _progressionSlider.SizeFlagsHorizontal = SizeFlags.ShrinkCenter; // Centre l'élément sans étirement

            _playIcon = GD.Load<CompressedTexture2D>(
                "res://Assets/Components/bouton-de-lecture.png"
            );
            _pauseIcon = GD.Load<CompressedTexture2D>("res://Assets/Components/pause.png");

            // StyleBox pour les boutons de lecture/pause
            _styleBoxPlayPauseBtnNormal =
                _playPauseButton.GetThemeStylebox("normal") as StyleBoxFlat;
            _styleBoxPlayPauseBtnHover = _playPauseButton.GetThemeStylebox("hover") as StyleBoxFlat;
            _styleBoxPlayPauseBtnPressed =
                _playPauseButton.GetThemeStylebox("pressed") as StyleBoxFlat;

            // Connecter les signaux
            _resetButton.Pressed += () => OnRestartButtonPressed(_pauseIcon);
            _playPauseButton.Pressed += () => OnPlayPauseButtonPressed(_playIcon, _pauseIcon);
            _volumeSlider.ValueChanged += OnVolumeSliderChanged;
            _progressionSlider.ValueChanged += OnProgressionSliderChanged;
            _progressionSlider.GuiInput += OnProgressionSliderGuiInput;

            _audioStreamPlayer.Finished += () => OnAudioFinished(_playIcon);

            // Initialisation des sliders
            _volumeSlider.MinValue = 0;
            _volumeSlider.MaxValue = 100;
            _volumeSlider.Value = 100; // Volume au maximum au démarrage
            _audioStreamPlayer.VolumeDb = 0; // Volume maximal au départ

            _inputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
            if (_inputHandler == null)
            {
                GD.PrintErr("InputHandler node not found in the scene tree.");
            }

            // Charger l'audio si un chemin est spécifié
            if (!string.IsNullOrEmpty(_path))
            {
                LoadAudio(_path);
            }

            // Configurer la position, la rotation et les autres propriétés de l'Audio
            Position = new Vector2(_positionX, _positionY); // Position dans la scène
            PivotOffset = new Vector2(_sizeX / 2, _sizeY / 2); // Centrer le pivot
            RotationDegrees = _rotationDeg; // Rotation en degrés
            Size = new Vector2(_sizeX, _sizeY); // Taille de l'élément
            Scale = new Vector2(_scaleX, _scaleY); // Échelle de l'élément
            ZIndex = _index; // Ordre d'affichage
        }

        public override void _Process(double delta)
        {
            // Mise à jour de la barre de progression pendant la lecture
            if (_audioStreamPlayer.Playing && !_isDragging)
            {
                _progressionSlider.Value = _audioStreamPlayer.GetPlaybackPosition();
            }
        }

        private void LoadAudio(string path)
        {
            string fullPath = System.IO.Path.Combine(Constants.AppPath, path);

            if (string.IsNullOrEmpty(fullPath))
            {
                GD.PrintErr("Le chemin est vide ou null.");
                return;
            }

            // Normaliser le chemin (remplacer les backslashes par des slashes pour compatibilité)
            string normalizedPath = fullPath.Replace("\\", "/");

            // Ouvrir le fichier audio
            var file = FileAccess.Open(normalizedPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Impossible d'ouvrir le fichier : {normalizedPath}");
                return;
            }

            // Obtenir l'extension
            string extension = System.IO.Path.GetExtension(normalizedPath)?.ToLower();

            AudioStream audioStream = null;
            if (extension == ".mp3")
            {
                try
                {
                    byte[] myMp3Data = file.GetBuffer((long)file.GetLength());
                    audioStream = new AudioStreamMP3 { Data = myMp3Data };
                }
                catch (System.Exception ex)
                {
                    GD.PrintErr($"Erreur lors du chargement du fichier audio : {ex.Message}");
                    return;
                }
                finally
                {
                    file.Close();
                }

                // Assigner et jouer l'audio
                if (audioStream != null)
                {
                    SetAudio(audioStream);
                }
            }
        }

        private void OnProgressionSliderGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    _isDragging = true; // L'utilisateur commence à manipuler
                }
                else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    _isDragging = false; // L'utilisateur a terminé
                    _audioStreamPlayer.Seek((float)_progressionSlider.Value);
                }
            }
        }

        public void SetBtnIcon()
        {
            // Vérifier si l'icône est la pause et appliquer les marges en temps réel
            if (_playPauseButton.Icon == _pauseIcon)
            {
                // Appliquer les marges si l'icône est celle de la pause
                _styleBoxPlayPauseBtnNormal.ContentMarginLeft = 1;
                _styleBoxPlayPauseBtnHover.ContentMarginLeft = 1;
                _styleBoxPlayPauseBtnPressed.ContentMarginLeft = 1;
            }
            else if (_playPauseButton.Icon == _playIcon)
            {
                // Appliquer d'autres marges si l'icône est celle de la lecture
                _styleBoxPlayPauseBtnNormal.ContentMarginLeft = 3; // Exemple de marge différente
                _styleBoxPlayPauseBtnHover.ContentMarginLeft = 3;
                _styleBoxPlayPauseBtnPressed.ContentMarginLeft = 3;
            }
        }

        public void SetAudio(AudioStream audioStream)
        {
            // Vérifie si _audioStreamPlayer est initialisé
            if (_audioStreamPlayer == null)
            {
                GD.PrintErr("L'AudioStreamPlayer (_audioStreamPlayer) n'est pas initialisé.");
                return;
            }

            // Assigne le flux audio à l'AudioStreamPlayer
            _audioStreamPlayer.Stream = audioStream;

            // Si le flux audio est valide, met à jour la durée et la barre de progression
            if (audioStream != null)
            {
                float audioLength = (float)audioStream.GetLength(); // Durée totale en secondes
                _maxPosition = audioLength;

                // Vérifie si _progressionSlider est initialisé
                if (_progressionSlider != null)
                {
                    _progressionSlider.MinValue = 0;
                    _progressionSlider.MaxValue = audioLength; // Définit la durée de l'audio
                    _progressionSlider.Value = 0; // Réinitialise la progression
                }
                else
                {
                    GD.PrintErr(
                        "Le Slider de progression (_progressionSlider) n'est pas initialisé."
                    );
                }
            }
            else
            {
                GD.PrintErr("AudioStream nul, réinitialisation des valeurs.");

                // Réinitialise les valeurs de la barre de progression
                _maxPosition = 0;

                if (_progressionSlider != null)
                {
                    _progressionSlider.MinValue = 0;
                    _progressionSlider.MaxValue = 0;
                    _progressionSlider.Value = 0;
                }
            }
        }

        private void OnRestartButtonPressed(CompressedTexture2D _pauseIcon)
        {
            // Remet à zéro
            _audioStreamPlayer.Play();
            _isPlaying = true;
            if (_playPauseButton.Icon != _pauseIcon)
            {
                _playPauseButton.Icon = _pauseIcon;
                SetBtnIcon();
            }
        }

        private void OnPlayPauseButtonPressed(
            CompressedTexture2D iconPlay,
            CompressedTexture2D iconPause
        )
        {
            if (!_isPlaying)
            {
                _audioStreamPlayer.Play();
                if (_isEnded)
                {
                    _isEnded = false;
                }
                else
                {
                    _audioStreamPlayer.Seek(_targetPlaybackPosition); // Reprend à la position souhaitée
                }
                _playPauseButton.Icon = iconPause;
                SetBtnIcon();

                _isPlaying = true;
            }
            else if (!_audioStreamPlayer.StreamPaused)
            {
                _audioStreamPlayer.StreamPaused = true;
                _playPauseButton.Icon = iconPlay;
                SetBtnIcon();
            }
            else
            {
                _audioStreamPlayer.StreamPaused = false;
                _playPauseButton.Icon = iconPause;
                SetBtnIcon();
                _audioStreamPlayer.Seek(_targetPlaybackPosition); // Reprend à la position souhaitée
            }
        }

        private void OnAudioFinished(CompressedTexture2D _playIcon)
        {
            _playPauseButton.Icon = _playIcon;
            SetBtnIcon();
            _isPlaying = false;
            _isEnded = true;
        }

        private void OnVolumeSliderChanged(double value)
        {
            // Plage linéaire du slider (exemple : de 0 à 100)
            float linearValue = (float)value;

            // Définir les limites de décibels
            float minDb = -40; // Niveau sonore très faible
            float maxDb = 0; // Volume maximal

            if (linearValue <= 0)
            {
                _audioStreamPlayer.VolumeDb = -80; // Valeur suffisamment basse pour couper complètement le son
            }
            else
            {
                // Convertir la valeur linéaire (0 à 100) en décibels
                float volumeDb = Mathf.Lerp(minDb, maxDb, linearValue / 100f);

                // Appliquer au lecteur audio
                _audioStreamPlayer.VolumeDb = volumeDb;
            }
        }

        private void OnProgressionSliderChanged(double value)
        {
            // Met à jour la position cible
            _targetPlaybackPosition = (float)value;

            if (_targetPlaybackPosition < _maxPosition)
            {
                _isEnded = false;
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
