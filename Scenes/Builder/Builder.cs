using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.BuilderNS.ComponentPanelsNS;
using Base = Tabloulet.Scenes.Components.BaseNS.Base;
using Button = Godot.Button;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextModel = Tabloulet.DatabaseNS.Models.Text;

namespace Tabloulet.Scenes.BuilderNS
{
    public partial class Builder : Control, IDisplay
    {
        private Database _database;

        public Guid idScenario;
        private Guid _currentPage;

        public Control _blueprint;
        public CreateComponentPanel createComponentPanel;
        public EditComponentPanel editComponentPanel;
        private Button _exitButton;

        private Button _addTextButton;
        private Button _addImageButton;

        private ScenarioLoader _scenarioLoader;

        private Dictionary<Guid, Dictionary<Guid, Control>> _pages;

        private Timer _saveTimer;

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

            _addTextButton.Pressed += AddTextButtonPressed;
            _addImageButton.Pressed += AddImageButtonPressed;

            _scenarioLoader = new ScenarioLoader(_database, this);

            _pages = [];

            _saveTimer = GetNode<Timer>("SaveTimer");
            _saveTimer.Timeout += SaveCurrentPage;
        }

        public void Init(Guid idScenario)
        {
            this.idScenario = idScenario;
            _currentPage = _scenarioLoader.LoadScenario(idScenario);
            _saveTimer.Start();
            editComponentPanel.SetCurrentPage(_blueprint.GetNode<Control>(_currentPage.ToString()));
        }

        private void AddTextButtonPressed()
        {
            TextModel text =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = _currentPage,
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
                    PageId = _currentPage,
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
            if (!_pages.TryGetValue(idPage, out Dictionary<Guid, Control> value))
            {
                value = ([]);
                _pages[idPage] = value;
            }

            value[idComponent] = baseComponent.GetChild<Control>(0);
        }

        private void SaveCurrentPage()
        {
            _database.Update(
                new Page()
                {
                    Id = _currentPage,
                    BackgroundColor = _blueprint
                        .GetNode<Control>(_currentPage.ToString())
                        .GetNode<ColorRect>("Background")
                        .Color.ToHtml(),
                }
            );
            foreach (var component in _pages[_currentPage])
            {
                IDatabaseModelComponent model = ControlToDatabaseModel.ConvertToDatabaseModel(
                    component.Key,
                    component.Value
                );
                model.PageId = _currentPage;
                _database.Update(model);
            }
        }

        public void DeleteComponent(Control component)
        {
            Guid guid = _pages[_currentPage].FirstOrDefault(x => x.Value == component).Key;
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
            // TODO: Save and change scene before queue free
            QueueFree();
        }
    }
}
