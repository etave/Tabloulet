using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.BuilderNS.ComponentPanelsNS;
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
        private Guid _currentPage;

        public Control _blueprint;
        public CreateComponentPanel createComponentPanel;
        public EditComponentPanel editComponentPanel;
        private Button _exitButton;

        private Button _addTextButton;
        private Button _addImageButton;
        private Button _addButtonButton;

        private ScenarioLoader _scenarioLoader;

        private Dictionary<Guid, Control> _page;

        private Timer _saveTimer;

        private OptionButton _pageSelector;
        private Dictionary<int, Guid> _pageSelectorOptions;
        private Button _newPageButton;

        private Panel _newPagePopupPanel;
        private LineEdit _newPageName;
        private Button _newPageCreateButton;
        private Button _newPageCancelButton;

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

            _addTextButton.Pressed += AddTextButtonPressed;
            _addImageButton.Pressed += AddImageButtonPressed;
            _addButtonButton.Pressed += AddButtonButtonPressed;

            _scenarioLoader = new ScenarioLoader(_database, this);

            _page = [];

            _saveTimer = GetNode<Timer>("SaveTimer");
            _saveTimer.Timeout += SaveCurrentPage;

            _pageSelector = GetNode<OptionButton>(
                "PagePanelContainer/MarginContainer/PageOptionButton"
            );

            _pageSelector.ItemSelected += OnPageSelectorItemSelected;

            _newPageButton = GetNode<Button>("NewPageButtonPanel/MarginContainer/NewPageButton");

            _newPageButton.Pressed += NewPageButtonPressed;

            _newPagePopupPanel = GetNode<Panel>("NewPagePopupPanel");
            _newPageName = _newPagePopupPanel.GetNode<LineEdit>(
                "VBoxContainer/MarginContainer/LineEdit"
            );
            _newPageCreateButton = _newPagePopupPanel.GetNode<Button>(
                "VBoxContainer/ButtonsHBoxContainer/CreateButton"
            );
            _newPageCancelButton = _newPagePopupPanel.GetNode<Button>(
                "VBoxContainer/ButtonsHBoxContainer/CancelButton"
            );

            _newPageName.TextChanged += OnNewPageNameTextChanged;
            _newPageCreateButton.Pressed += NewPageCreateButtonPressed;
            _newPageCancelButton.Pressed += NewPageCancelButtonPressed;
        }

        public void Init(Guid idScenario)
        {
            this.idScenario = idScenario;
            _currentPage = _scenarioLoader.LoadScenario(idScenario);
            _saveTimer.Start();
            editComponentPanel.SetCurrentPage(_blueprint.GetNode<Control>(_currentPage.ToString()));
            UpdatePageSelector();
        }

        private void NewPageButtonPressed()
        {
            _newPagePopupPanel.Visible = true;
        }

        private void NewPageCreateButtonPressed()
        {
            Page page =
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = _newPageName.Text,
                    BackgroundColor = "#FFFFFF",
                };
            _database.Insert(page);
            ScenarioPage scenarioPage =
                new()
                {
                    Id = Guid.NewGuid(),
                    ScenarioId = idScenario,
                    PageId = page.Id,
                };
            _database.Insert(scenarioPage);
            _newPagePopupPanel.Visible = false;
            UpdatePageSelector();
        }

        private void UpdatePageSelector()
        {
            _pageSelector.Clear();
            _pageSelectorOptions = [];
            List<Page> pages =
            [
                _database.GetById<Page>(_database.GetById<Scenario>(idScenario).PageId),
                .. _database.GetPagesByScenario(idScenario),
            ];
            for (int i = 0; i < pages.Count; i++)
            {
                _pageSelector.AddItem(pages[i].Name, i);
                _pageSelectorOptions[i] = pages[i].Id;
            }
            _pageSelector.Selected = _pageSelectorOptions
                .FirstOrDefault(x => x.Value == _currentPage)
                .Key;
        }

        private void OnPageSelectorItemSelected(long index)
        {
            ChangePage(_pageSelectorOptions[(int)index]);
        }

        public void ChangePage(Guid idPage)
        {
            SaveCurrentPage();
            FreePage();
            _currentPage = idPage;
            editComponentPanel.isBackgroundCallableSet = false;
            _scenarioLoader.LoadPage(_database.GetById<Page>(_currentPage));
            editComponentPanel.SetCurrentPage(_blueprint.GetNode<Control>(_currentPage.ToString()));
            UpdatePageSelector();
        }

        private void NewPageCancelButtonPressed()
        {
            _newPagePopupPanel.Visible = false;
        }

        private void OnNewPageNameTextChanged(string newText)
        {
            if (newText.Length > 0)
            {
                _newPageCreateButton.Disabled = false;
            }
            else
            {
                _newPageCreateButton.Disabled = true;
            }
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

        private void AddButtonButtonPressed()
        {
            ButtonModel button =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = _currentPage,
                    Content = "Button",
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
            Control page = _blueprint.GetNode<Control>(_currentPage.ToString());
            foreach (Control component in page.GetChildren().Cast<Control>())
            {
                component.QueueFree();
            }
            page.QueueFree();
            _page.Clear();
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
                    Name = _database.GetById<Page>(_currentPage).Name,
                }
            );
            foreach (var component in _page)
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
            // TODO: Change scene before queue free
            SaveCurrentPage();
            QueueFree();
        }
    }
}
