using System;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using BaseComponent = Tabloulet.Scenes.Components.BaseNS.Base;

namespace Tabloulet.Scenes.ViewerNS
{
    public partial class Viewer : Control, IDisplay
    {
        private Database _database;

        private Guid _currentPage;

        private Guid _idScenario;

        private ScenarioLoader _scenarioLoader;

        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");

            _scenarioLoader = new ScenarioLoader(_database, this);
        }

        public void Init(Guid idScenario)
        {
            _currentPage = _scenarioLoader.LoadScenario(idScenario);
            _idScenario = idScenario;
            InitButtons();
        }

        public void InitButtons()
        {
            HBoxContainer hBoxContainer = new();
            hBoxContainer.SetAnchorsPreset(LayoutPreset.TopLeft);
            hBoxContainer.AddThemeConstantOverride("separation", 20);

            StyleBoxFlat normalStyleBox = new() { BgColor = new Color(1, 1, 1, 0) };

            Godot.Button buttonExit = new();
            Texture2D texture2D = GD.Load<Texture2D>("res://Assets/Viewer/exit.png");
            buttonExit.Icon = texture2D;
            buttonExit.ZIndex = 101;
            buttonExit.AddThemeConstantOverride("icon_max_width", 75);
            buttonExit.AddThemeStyleboxOverride("normal", normalStyleBox);
            buttonExit.AddThemeStyleboxOverride("hover", normalStyleBox);
            buttonExit.AddThemeStyleboxOverride("pressed", normalStyleBox);
            buttonExit.AddThemeStyleboxOverride("focus", normalStyleBox);

            Godot.Button buttonReset = new();
            texture2D = GD.Load<Texture2D>("res://Assets/Viewer/reset.png");
            buttonReset.Icon = texture2D;
            buttonReset.ZIndex = 101;
            buttonReset.AddThemeConstantOverride("icon_max_width", 75);
            buttonReset.AddThemeStyleboxOverride("normal", normalStyleBox);
            buttonReset.AddThemeStyleboxOverride("hover", normalStyleBox);
            buttonReset.AddThemeStyleboxOverride("pressed", normalStyleBox);
            buttonReset.AddThemeStyleboxOverride("focus", normalStyleBox);
            buttonReset.Pressed += ResetScenario;

            hBoxContainer.AddChild(buttonExit);
            hBoxContainer.AddChild(buttonReset);
            AddChild(hBoxContainer);
        }

        private void ResetScenario()
        {
            Scenario scenario = _database.GetById<Scenario>(_idScenario);
            Guid firstPage = scenario.PageId;
            if (firstPage == _currentPage)
            {
                return;
            }
            ChangePage(scenario.PageId);
        }

        public Control GetDisplayRoot()
        {
            return this;
        }

        public void AddPage(Control page)
        {
            AddChild(page);
        }

        public void AddComponent(Guid idPage, Guid _, BaseComponent baseComponent)
        {
            Control page = GetNode<Control>(idPage.ToString());
            page.AddChild(baseComponent);
        }

        public void FreePage()
        {
            Control page = GetNode<Control>(_currentPage.ToString());
            page.QueueFree();
        }

        public void ChangePage(Guid idPage)
        {
            FreePage();
            _currentPage = idPage;
            _scenarioLoader.LoadPage(_database.GetById<Page>(_currentPage));
            InitButtons();
        }
    }
}
