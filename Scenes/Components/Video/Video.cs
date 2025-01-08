using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes.Components.VideoNS
{
    public partial class Video() : VideoStreamPlayer, IComponent
    {
        private InputHandler _inputHandler;
        private HSlider _timerSlider;
        private Button _resetButton;
        private CompressedTexture2D _pauseIcon;
        private CompressedTexture2D _playIcon;
        private Button _playPauseButton;
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
        private bool _autoplay;
        private bool _loop;
        private bool _isDragging = false;
        private bool _isPlaying = false;
        private bool _isEnded = false;
        private float _maxPosition = 0f;
        private float _targetPlaybackPosition = 0f;
        private StyleBoxFlat _styleBoxPlayPauseBtnNormal;
        private StyleBoxFlat _styleBoxPlayPauseBtnHover;
        private StyleBoxFlat _styleBoxPlayPauseBtnPressed;

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                LoadVideo(value);
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
            set => _sizeX = _sizeX;
        }

        public float SizeY
        {
            get => _sizeY;
            set => _sizeY = _sizeY;
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

        public bool AutoplayVideo
        {
            get => _autoplay;
            set => _autoplay = value;
        }

        public bool LoopVideo
        {
            get => _loop;
            set => _loop = value;
        }

        public override void _Ready()
        {
            base._Ready();
            _timerSlider = GetNode<HSlider>("ControlMarginContainer/HBoxContainer/HSlider");
            StreamPosition = (float)_timerSlider.Value;
            _timerSlider.GuiInput += OnProgressionSliderGuiInput;




            _resetButton = GetNode<Button>("ControlMarginContainer/HBoxContainer/ResetButton");
            _playIcon = GD.Load<CompressedTexture2D>(
                "res://Assets/Components/bouton-de-lecture.png"
            );
            _playPauseButton = GetNode<Button>(
                "ControlMarginContainer/HBoxContainer/PlayPauseButton"
            );
            StreamPosition = (float)_timerSlider.Value;

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");
            _pauseIcon = GD.Load<CompressedTexture2D>("res://Assets/Components/pause.png");
            Autoplay = _autoplay;
            Loop = _loop;
            _resetButton.Pressed += () => OnRestartButtonPressed(_pauseIcon);
            _styleBoxPlayPauseBtnNormal =
                _playPauseButton.GetThemeStylebox("normal") as StyleBoxFlat;
            _styleBoxPlayPauseBtnHover = _playPauseButton.GetThemeStylebox("hover") as StyleBoxFlat;
            _styleBoxPlayPauseBtnPressed =
                _playPauseButton.GetThemeStylebox("pressed") as StyleBoxFlat;

            _playPauseButton.Pressed += () => OnPlayPauseButtonPressed(_playIcon, _pauseIcon);

            Position = new Vector2(_positionX, _positionY);
            RotationDegrees = _rotationDeg;
            Size = new Vector2(_sizeX, _sizeY);
            ZIndex = _index;
            Scale = new Vector2(_scaleX, _scaleY);
            if (!string.IsNullOrEmpty(_path))
            {
                LoadVideo(_path);
            }

        }

        private void LoadVideo(string path)
        {
            if (path.StartsWith("res://"))
            {
                Stream = GD.Load<VideoStream>(path);
            }
            else
            {
                string fullPath = System
                    .IO.Path.Combine(Constants.AppPath, path)
                    .Replace("\\", "/");
                if (!System.IO.File.Exists(fullPath))
                {
                    GD.PrintErr("File not found: " + fullPath);
                    return;
                }
                Stream = GD.Load<VideoStream>(fullPath);
            }
            if (Autoplay)
            {
                Play();
                _isPlaying = true;
                _playPauseButton.Icon = _pauseIcon;
                SetBtnIcon();
            }

            if (_timerSlider != null && Stream != null)
            {
                _timerSlider.MinValue = 0;
                _timerSlider.MaxValue = GetStreamLength();
                _timerSlider.Step = 0.1;
            }
        }

        public void SetBtnIcon()
        {
            if (_playPauseButton.Icon == _pauseIcon)
            {
                _styleBoxPlayPauseBtnNormal.ContentMarginLeft = 1;
                _styleBoxPlayPauseBtnHover.ContentMarginLeft = 1;
                _styleBoxPlayPauseBtnPressed.ContentMarginLeft = 1;
            }
            else if (_playPauseButton.Icon == _playIcon)
            {
                _styleBoxPlayPauseBtnNormal.ContentMarginLeft = 3;
                _styleBoxPlayPauseBtnHover.ContentMarginLeft = 3;
                _styleBoxPlayPauseBtnPressed.ContentMarginLeft = 3;
            }
        }
        public override void _Process(double delta)
        {
            if (_timerSlider != null && !_isDragging && Stream != null && !Paused)
            {
                _timerSlider.Value = StreamPosition;
            }
        }

        private void OnRestartButtonPressed(CompressedTexture2D _pauseIcon)
        {
            Play();
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
            GD.Print("PlayPauseButton pressed");

            if (_isPlaying)
            {
                Paused = true;
                _isPlaying = false;
                _playPauseButton.Icon = iconPlay;
            }
            else
            {
                Paused = false;
                _isPlaying = true;
                _playPauseButton.Icon = iconPause;
            }

            SetBtnIcon();
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

        private void OnProgressionSliderChanged(double value)
        {
            _targetPlaybackPosition = (float)value;

            if (_targetPlaybackPosition < _maxPosition)
            {
                _isEnded = false;
            }
        }

        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            var marginContainer = GetNode<MarginContainer>("ControlMarginContainer");
            marginContainer.Visible = true;

            var timer = new Timer { WaitTime = 2.0f, OneShot = true };
            timer.Timeout += OnHideMarginContainer;
            AddChild(timer);
            timer.Start();
        }

        private void OnHideMarginContainer()
        {
            var marginContainer = GetNode<MarginContainer>("ControlMarginContainer");
            marginContainer.Visible = false;
        }

        private void OnProgressionSliderGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    if (mouseEvent.Pressed)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        _isDragging = false;
                        StreamPosition = (float)_timerSlider.Value;
                    }
                }
            }
        }
    }
}
