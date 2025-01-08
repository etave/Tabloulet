using System.IO;
using Godot;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using VideoComponent = Tabloulet.Scenes.Components.VideoNS.Video;

namespace Tabloulet.Scenes.BuilderNS.ComponentPanelsNS
{
    public partial class EditComponentPanel : Control
    {
        private Builder _builder;

        private Panel _openPanel;
        private Panel _closePanel;

        private Button _closeButton;
        private Button _openButton;
        private Control _currentPage;
        private Base _currentComponent;

        public bool closeByUser;

        private bool _isEditingPage;
        public bool isBackgroundCallableSet;
        private bool _isEditingComponent;

        private VBoxContainer _vboxContainerContent;

        private MarginContainer _pageMarginContainer;
        private ColorPicker _pageColorPicker;

        private MarginContainer _baseMarginContainer;
        private SpinBox _basePositionX;
        private SpinBox _basePositionY;
        private SpinBox _baseSizeX;
        private SpinBox _baseSizeY;
        private SpinBox _baseZIndex;
        private Slider _baseRotation;
        private CheckBox _baseIsMovable;

        private MarginContainer _componentMarginContainer;

        private Button _deleteButton;

        public override void _Ready()
        {
            base._Ready();

            _builder = GetParent<Builder>();

            _openPanel = GetNode<Panel>("OpenPanel");
            _closePanel = GetNode<Panel>("ClosePanel");

            _closeButton = _openPanel.GetNode<Button>(
                "VBoxContainer/PanelContainer/MarginContainer2/PanelContainer/Button"
            );
            _openButton = _closePanel.GetNode<Button>("PanelContainer/Button");

            _closeButton.Pressed += () => CloseButtonPressed(true);
            _openButton.Pressed += () => OpenButtonPressed(true);

            closeByUser = false;

            _vboxContainerContent = GetNode<VBoxContainer>("OpenPanel/VBoxContainer");

            _pageMarginContainer = _vboxContainerContent.GetNode<MarginContainer>(
                "PageMarginContainer"
            );
            _pageColorPicker = _pageMarginContainer.GetNode<ColorPicker>(
                "VBoxContainer/ColorPicker"
            );

            _isEditingPage = false;
            isBackgroundCallableSet = false;
            _isEditingComponent = false;

            _baseMarginContainer = _vboxContainerContent.GetNode<MarginContainer>(
                "BaseMarginContainer"
            );
            _basePositionX = _baseMarginContainer.GetNode<SpinBox>(
                "VBoxContainer/PositionVBoxContainer/HBoxContainer/XHBoxContainer/XSpinBox"
            );
            _basePositionY = _baseMarginContainer.GetNode<SpinBox>(
                "VBoxContainer/PositionVBoxContainer/HBoxContainer/YHBoxContainer/YSpinBox"
            );
            _baseSizeX = _baseMarginContainer.GetNode<SpinBox>(
                "VBoxContainer/SizeVBoxContainer/HBoxContainer/XHBoxContainer/XSpinBox"
            );
            _baseSizeY = _baseMarginContainer.GetNode<SpinBox>(
                "VBoxContainer/SizeVBoxContainer/HBoxContainer/YHBoxContainer/YSpinBox"
            );
            _baseZIndex = _baseMarginContainer.GetNode<SpinBox>(
                "VBoxContainer/ZIndexIsMovableHBoxContainer/ZIndexVBoxContainer/ZIndexSpinBox"
            );
            _baseRotation = _baseMarginContainer.GetNode<Slider>(
                "VBoxContainer/RotationVBoxContainer/MarginContainer/RotationSlider"
            );
            _baseIsMovable = _baseMarginContainer.GetNode<CheckBox>(
                "VBoxContainer/ZIndexIsMovableHBoxContainer/IsMovableVBoxContainer/IsMovableCheckBox"
            );

            _componentMarginContainer = _vboxContainerContent.GetNode<MarginContainer>(
                "ComponentMarginContainer"
            );

            _deleteButton = _vboxContainerContent.GetNode<Button>(
                "DeleteMarginContainer/DeleteButton"
            );
            _deleteButton.Pressed += DeleteComponent;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (_currentPage == null)
            {
                return;
            }
            if (_currentComponent == null)
            {
                _currentPage.GetNode<ColorRect>("Background").Color = _pageColorPicker.Color;
            }
            else if (_currentComponent != null)
            {
                UpdateComponent();
            }
        }

        public void SetCurrentPage(Control page)
        {
            _currentComponent = null;
            _isEditingComponent = false;
            _currentPage = page;
            _isEditingPage = true;
            _pageMarginContainer.Visible = true;
            _baseMarginContainer.Visible = false;
            _deleteButton.GetParent<Control>().Visible = false;
            ColorRect background = _currentPage.GetNode<ColorRect>("Background");
            _pageColorPicker.Color = background.Color;

            if (!isBackgroundCallableSet)
            {
                background.GuiInput += BackgroundGuiInput;
                isBackgroundCallableSet = true;
            }

            ResetComponentMarginContainer();
        }

        public void SetCurrentComponent(Base component)
        {
            _currentComponent = component;
            _isEditingComponent = true;
            _isEditingPage = false;
            _baseMarginContainer.Visible = true;
            _deleteButton.GetParent<Control>().Visible = true;
            _pageMarginContainer.Visible = false;
            UpdatePanel();
        }

        public void RemoveCurrentComponent()
        {
            SetCurrentPage(_currentPage);
        }

        private void BackgroundGuiInput(InputEvent @event)
        {
            if (@event is InputEventScreenTouch touch && touch.Pressed)
            {
                SetCurrentPage(_currentPage);
            }
        }

        public void CloseButtonPressed(bool closeByUser)
        {
            if (closeByUser)
            {
                this.closeByUser = true;
            }
            _openPanel.Visible = false;
            _closePanel.Visible = true;
        }

        public void OpenButtonPressed(bool openByUser)
        {
            if (openByUser)
            {
                this.closeByUser = false;
            }
            _closePanel.Visible = false;
            _openPanel.Visible = true;
        }

        private void DeleteComponent()
        {
            _builder.DeleteComponent(_currentComponent.GetChild<Control>(0));
            SetCurrentPage(_currentPage);
        }

        private void UpdatePanel()
        {
            Control child = _currentComponent.GetChild<Control>(0);
            _basePositionX.Value = child.Position.X;
            _basePositionY.Value = child.Position.Y;
            _baseSizeX.Value = child.Size.X;
            _baseSizeY.Value = child.Size.Y;
            _baseZIndex.Value = child.ZIndex;
            _baseRotation.Value = NormalizeRotation(child.RotationDegrees);
            _baseIsMovable.ButtonPressed = _currentComponent.IsMovable;

            switch (child)
            {
                case TextComponent text:
                    break;
                case ImageComponent image:
                    CreateImageComponentEdit(image);
                    break;
                default:
                    break;
            }
        }

        private void UpdateComponent()
        {
            Control child = _currentComponent.GetChild<Control>(0);
            child.PivotOffset = child.Size / 2;
            child.Position = new Vector2((float)_basePositionX.Value, (float)_basePositionY.Value);

            if (child is not VideoComponent)
            {
                child.Size = new Vector2((float)_baseSizeX.Value, (float)_baseSizeY.Value);
            }

            child.Size = new Vector2((float)_baseSizeX.Value, (float)_baseSizeY.Value);
            child.ZIndex = (int)_baseZIndex.Value;
            child.RotationDegrees = (float)_baseRotation.Value;
            _currentComponent.IsMovable = _baseIsMovable.ButtonPressed;
        }

        private static float NormalizeRotation(float rotation)
        {
            float normalizedRotation = rotation % 360;
            if (normalizedRotation < 0)
            {
                normalizedRotation += 360;
            }
            return normalizedRotation;
        }

        private void ResetComponentMarginContainer()
        {
            foreach (Node child in _componentMarginContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        private void CreateImageComponentEdit(ImageComponent image)
        {
            ResetComponentMarginContainer();

            VBoxContainer vBoxContainer = new() { Name = "ImageComponentEdit" };
            Label label =
                new()
                {
                    Text = "Chemin de l'image",
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
            label.AddThemeFontSizeOverride("font_size", 20);
            label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
            LineEdit lineEdit =
                new()
                {
                    Text = image.Path,
                    Editable = false,
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                };
            lineEdit.AddThemeColorOverride("font_uneditable_color", new Color(0, 0, 0));
            FileDialog fileDialog =
                new()
                {
                    FileMode = FileDialog.FileModeEnum.OpenFile,
                    Access = FileDialog.AccessEnum.Filesystem,
                    Filters = ["*.png", "*.jpg", "*.jpeg"],
                };

            fileDialog.FileSelected += (string path) =>
            {
                lineEdit.Text = path;
                string directoryPath = Path.Combine(
                    Constants.AppPath,
                    _builder.idScenario.ToString()
                );
                string newFilePath = Path.Combine(directoryPath, Path.GetFileName(path));
                File.Copy(path, newFilePath, true);
                image.Path = newFilePath;
            };

            Button openDialogButton =
                new() { Text = "📂", SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
            openDialogButton.Pressed += () => fileDialog.PopupCenteredRatio();

            HBoxContainer hBoxContainer = new();

            hBoxContainer.AddChild(lineEdit);
            hBoxContainer.AddChild(openDialogButton);
            hBoxContainer.AddChild(fileDialog);

            vBoxContainer.AddChild(label);
            vBoxContainer.AddChild(hBoxContainer);

            _componentMarginContainer.AddChild(vBoxContainer);
        }
    }
}
