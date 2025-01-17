using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.BuilderNS.ComponentPanelsNS;
using Tabloulet.Scenes.BuilderNS.NavigationGraphNS;
using Tabloulet.Scenes.Components.AudioNS;
using Tabloulet.Scenes.HomeNS;
using AudioModel = Tabloulet.DatabaseNS.Models.Audio;
using Base = Tabloulet.Scenes.Components.BaseNS.Base;
using Button = Godot.Button;
using ButtonModel = Tabloulet.DatabaseNS.Models.Button;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextModel = Tabloulet.DatabaseNS.Models.Text;

namespace Tabloulet.Scenes.BuilderNS
{
    public partial class Builder : Control, IDisplay
    {
        private Database _database;

        public Guid idScenario;
        public Guid currentPage;

        public Control _blueprint;
        public CreateComponentPanel createComponentPanel;
        public EditComponentPanel editComponentPanel;
        private Button _exitButton;

        private Button _addTextButton;
        private Button _addImageButton;
        private Button _addButtonButton;
        private Button _addAudioButton;

        private ScenarioLoader _scenarioLoader;

        private Dictionary<Guid, Control> _page;

        private Timer _saveTimer;

        private Button _navigationGraphButton;

        private Button _createTemplateButton;
        private Button _cancelTemplateButton;
        private Button _newTemplateButton;

        private Panel _newTemplatePopUpPanel;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");

            _blueprint = GetNode<Control>("Blueprint");

            createComponentPanel = GetNode<CreateComponentPanel>("CreateComponentPanel");
            editComponentPanel = GetNode<EditComponentPanel>("EditComponentPanel");
            _exitButton = GetNode<Button>("ExitPanel/MarginContainer/Button");

            _exitButton.Pressed += ExitButtonPressed;

            _addTextButton = createComponentPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/TextMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );
            _addImageButton = createComponentPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/ImageMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );
            _addButtonButton = createComponentPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/ButtonMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );
            _addAudioButton = createComponentPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/AudioMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );

            _addTextButton.Pressed += AddTextButtonPressed;
            _addImageButton.Pressed += AddImageButtonPressed;
            _addButtonButton.Pressed += AddButtonButtonPressed;
            _addAudioButton.Pressed += AddAudioButtonPressed;

            _scenarioLoader = new ScenarioLoader(_database, this);

            _page = [];

            _saveTimer = GetNode<Timer>("SaveTimer");
            _saveTimer.Timeout += SaveCurrentPage;

            _navigationGraphButton = GetNode<Button>(
                "NavigationGraphButtonPanel/MarginContainer/NavigationGraphButton"
            );
            _navigationGraphButton.Pressed += NavigationGraphButtonPressed;

            _createTemplateButton = GetNode<Button>(
                "NewTemplatePopUpPanel/VBoxContainer/ButtonsHBoxContainer/CreateButton"
            );
            _cancelTemplateButton = GetNode<Button>(
                "NewTemplatePopUpPanel/VBoxContainer/ButtonsHBoxContainer/CancelButton"
            );
            _newTemplateButton = GetNode<Button>(
                "NewTemplateButtonPanel/MarginContainer/NewTemplateButton"
            );

            _newTemplateButton.Pressed += () => _newTemplatePopUpPanel.Visible = true;
            _cancelTemplateButton.Pressed += () => _newTemplatePopUpPanel.Visible = false;
            _createTemplateButton.Pressed += CreateTemplateButtonPressed;

            _newTemplatePopUpPanel = GetNode<Panel>("NewTemplatePopUpPanel");
        }

        public void Init(Guid idScenario)
        {
            this.idScenario = idScenario;
            currentPage = _scenarioLoader.LoadScenario(idScenario);
            _saveTimer.Start();
            editComponentPanel.SetCurrentPage(_blueprint.GetNode<Control>(currentPage.ToString()));
        }

        public void ChangePage(Guid idPage)
        {
            SaveCurrentPage();
            FreePage();
            currentPage = idPage;
            editComponentPanel.isBackgroundCallableSet = false;
            _scenarioLoader.LoadPage(_database.GetById<Page>(currentPage));
            editComponentPanel.SetCurrentPage(_blueprint.GetNode<Control>(currentPage.ToString()));
        }

        private void NavigationGraphButtonPressed()
        {
            PackedScene navigationGraphScene = GD.Load<PackedScene>(
                "res://Scenes/Builder/NavigationGraph/NavigationGraph.tscn"
            );
            NavigationGraph navigationGraph = (NavigationGraph)navigationGraphScene.Instantiate();
            GetTree().Root.AddChild(navigationGraph);
            navigationGraph.LoadGraph(idScenario);
        }

        private void AddTextButtonPressed()
        {
            TextModel text =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = currentPage,
                    Content = $"[color=#000000]{LoremNET.Lorem.Words(30, true)}[/color]",
                    Font = null,
                    FontSize = 20,
                    ScaleX = 1,
                    ScaleY = 1,
                    SizeX = 200,
                    SizeY = 200,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    ZIndex = 1,
                    IsMovable = true,
                };
            _scenarioLoader.CreateTextComponent(text);
            _database.Insert(text);
        }

        private void AddImageButtonPressed()
        {
            ImageModel image =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = currentPage,
                    Path = "",
                    ScaleX = 1,
                    ScaleY = 1,
                    SizeX = 200,
                    SizeY = 200,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    ZIndex = 1,
                    IsMovable = true,
                };
            _scenarioLoader.CreateImageComponent(image);
            _database.Insert(image);
        }

        private void AddButtonButtonPressed()
        {
            ButtonModel button =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = currentPage,
                    Content = "Bouton",
                    Color = "#FFFFFF",
                    ScaleX = 1,
                    ScaleY = 1,
                    SizeX = 200,
                    SizeY = 100,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    ZIndex = 1,
                    IsMovable = true,
                };
            _scenarioLoader.CreateButtonComponent(button);
            _database.Insert(button);
        }

        private void AddAudioButtonPressed()
        {
            AudioModel audio =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = currentPage,
                    Path = "",
                    ScaleX = 0.7f,
                    ScaleY = 0.7f,
                    SizeX = Components.AudioNS.Audio.MIN_X_SIZE,
                    SizeY = Components.AudioNS.Audio.MIN_Y_SIZE,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    ZIndex = 1,
                    IsMovable = true,
                };
            _scenarioLoader.CreateAudioComponent(audio);
            _database.Insert(audio);
        }

        public Control GetDisplayRoot()
        {
            return _blueprint;
        }

        public void AddPage(Control page)
        {
            _blueprint.AddChild(page);
        }

        public void AddComponent(Guid idPage, Guid idComponent, Base baseComponent)
        {
            Control page = _blueprint.GetNode<Control>(idPage.ToString());
            page.AddChild(baseComponent);

            if (!_page.ContainsKey(idComponent))
            {
                _page[idComponent] = baseComponent.GetChild<Control>(0);
            }
        }

        public void FreePage()
        {
            editComponentPanel.pageNameLineEdit.Text = "";
            Control page = _blueprint.GetNode<Control>(currentPage.ToString());
            foreach (Control component in page.GetChildren().Cast<Control>())
            {
                component.QueueFree();
            }
            page.QueueFree();
            _page.Clear();
        }

        private void SaveCurrentPage()
        {
            Control blueprintPage = _blueprint.GetNode<Control>(currentPage.ToString());
            _database.Update(
                new Page()
                {
                    Id = currentPage,
                    BackgroundColor = blueprintPage.GetNode<ColorRect>("Background").Color.ToHtml(),
                    Name = editComponentPanel.pageNameLineEdit.Text,
                }
            );
            foreach (var component in _page)
            {
                IDatabaseModelComponent model = ControlToDatabaseModel.ConvertToDatabaseModel(
                    component.Key,
                    component.Value
                );
                model.PageId = currentPage;
                _database.Update(model);
            }
        }

        public void DeleteComponent(Control component)
        {
            Guid guid = _page.FirstOrDefault(x => x.Value == component).Key;
            IDatabaseModelComponent model = ControlToDatabaseModel.ConvertToDatabaseModel(
                guid,
                component
            );
            _database.Delete(model);
            Control parent = component.GetParent<Control>();
            parent.QueueFree();
        }

        private void ExitButtonPressed()
        {
            SaveCurrentPage();
            PackedScene homeScene = GD.Load<PackedScene>("res://Scenes/Home/Home.tscn");
            Home home = (Home)homeScene.Instantiate();
            GetTree().Root.AddChild(home);
            QueueFree();
        }

        public Guid GetCurrentPageId()
        {
            return currentPage;
        }

        private void CreateTemplateButtonPressed()
        {
            LineEdit lineEdit = _newTemplatePopUpPanel.GetNode<LineEdit>(
                "VBoxContainer/MarginContainer/LineEdit"
            );
            String templateName = lineEdit.Text;
            _database.SavePageAsTemplate(currentPage, templateName);
            lineEdit.Text = "Nouveau template";
            _newTemplatePopUpPanel.Visible = false;
        }
    }
}
