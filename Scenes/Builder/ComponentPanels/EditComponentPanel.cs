using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.Components.BaseNS;
using ButtonComponent = Tabloulet.Scenes.Components.ButtonNS.Button;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;

namespace Tabloulet.Scenes.BuilderNS.ComponentPanelsNS
{
    public partial class EditComponentPanel : Control
    {
        private Builder _builder;
        private Database _database;

        private Panel _openPanel;
        private Panel _closePanel;

        private Godot.Button _closeButton;
        private Godot.Button _openButton;
        private Control _currentPage;
        private Components.BaseNS.Base _currentComponent;

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

        private Godot.Button _deleteButton;

        private Dictionary<int, Guid> _pages;

        public override void _Ready()
        {
            base._Ready();

            _builder = GetParent<Builder>();
            _database = GetNode<Database>("/root/Database");

            _openPanel = GetNode<Panel>("OpenPanel");
            _closePanel = GetNode<Panel>("ClosePanel");

            _closeButton = _openPanel.GetNode<Godot.Button>(
                "VBoxContainer/PanelContainer/MarginContainer2/PanelContainer/Button"
            );
            _openButton = _closePanel.GetNode<Godot.Button>("PanelContainer/Button");

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

            _deleteButton = _vboxContainerContent.GetNode<Godot.Button>(
                "DeleteMarginContainer/DeleteButton"
            );
            _deleteButton.Pressed += DeleteComponent;
            _pages = new Dictionary<int, Guid>();
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

        public void SetCurrentComponent(Components.BaseNS.Base component)
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
                case ButtonComponent button:
                    CreateButtonComponentEdit(button);
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
                    Text = !string.IsNullOrEmpty(image.Path)
                        ? Path.Combine(Constants.AppPath, image.Path)
                        : "",
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
                image.Path = Path.Combine(_builder.idScenario.ToString(), Path.GetFileName(path));
            };

            Godot.Button openDialogButton =
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

        private void CreateButtonComponentEdit(ButtonComponent button)
        {
            ResetComponentMarginContainer();

            _pages.Clear();

            VBoxContainer vBoxContainer = new() { Name = "ButtonComponentEdit" };
            Label label =
                new() { Text = "Lien du bouton", HorizontalAlignment = HorizontalAlignment.Center };
            label.AddThemeFontSizeOverride("font_size", 20);
            label.AddThemeColorOverride("font_color", new Color(0, 0, 0));

            // Ajout d'un menu déroulant pour sélectionner une page
            OptionButton pageSelector = new() { SizeFlagsHorizontal = SizeFlags.ExpandFill };
            List<Page> query = _database.GetPagesByScenario(_builder.idScenario);
            int index = 0;
            foreach (Page page in query)
            {
                if (page.Id != _builder.getCurrentPageId())
                {
                    pageSelector.AddItem(page.Name, index);
                    this._pages.Add(index, page.Id);
                    index++;
                }
            }

            pageSelector.ItemSelected += (long index) => onPageSelectorPressed(index, button);

            HBoxContainer hBoxContainer = new();
            hBoxContainer.AddChild(pageSelector);

            vBoxContainer.AddChild(label);
            vBoxContainer.AddChild(hBoxContainer);

            _componentMarginContainer.AddChild(vBoxContainer);
        }

        private void onPageSelectorPressed(long index, ButtonComponent button)
        {
            button.LinkTo = _pages[(int)index];
        }
    }
}
