using System;
using System.Threading.Tasks;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Scenes.HomeNS;
using Tabloulet.Scenes.HomeNS.LoginPanelNS;
using BaseComponent = Tabloulet.Scenes.Components.BaseNS.Base;

namespace Tabloulet.Scenes.ViewerNS
{
    public partial class Viewer : Control, IDisplay
    {
        private Database _database;

        private Guid _currentPage;

        private Guid _idScenario;

        private ScenarioLoader _scenarioLoader;

        private Timer _rfidTimer;

        private LoginPanel _loginPanel;

        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");

            _scenarioLoader = new ScenarioLoader(_database, this);

            _rfidTimer = GetNode<Timer>("RFIDTimer");
            _rfidTimer.Timeout += OnRFIDTimerTimeout;
            _rfidTimer.Start();

            _loginPanel = GetNode<LoginPanel>("LoginPanel");
            _loginPanel.SetProcess(false);
            _loginPanel.viewerMode = true;
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
            buttonExit.Pressed += () => OnExitButtonPressed();

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

        private void OnExitButtonPressed()
        {
            Control page = GetChild(2) as Control;
            page.Visible = false;
            _loginPanel.SetProcess(true);
            _loginPanel.Visible = true;
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

        private HBoxContainer GetFirstHBoxContainer()
        {
            foreach (Node child in GetChildren())
            {
                if (child is HBoxContainer hBoxContainer)
                {
                    return hBoxContainer;
                }
            }
            return null;
        }

        public void FreePage()
        {
            Control page = GetNode<Control>(_currentPage.ToString());
            page.QueueFree();
            HBoxContainer buttons = GetFirstHBoxContainer();
            buttons.QueueFree();
        }

        public void ChangePage(Guid idPage)
        {
            FreePage();
            _currentPage = idPage;
            _scenarioLoader.LoadPage(_database.GetById<Page>(_currentPage));
            InitButtons();
        }

        public void ExitButtonPressed()
        {
            PackedScene homeScene = GD.Load<PackedScene>("res://Scenes/Home/Home.tscn");
            Home home = (Home)homeScene.Instantiate();
            GetTree().Root.AddChild(home);
            QueueFree();
        }

        public void LoginCancelButtonPressed()
        {
            Control page = GetChild(2) as Control;
            page.Visible = true;
            _loginPanel.SetProcess(false);
            _loginPanel.Visible = false;
        }

        private void OnRFIDTimerTimeout()
        {
            Helpers
                .RFID.GetUIDAsync(_idScenario)
                .ContinueWith(
                    task =>
                    {
                        if (
                            task.IsFaulted
                            || task.Result == Guid.Empty
                            || task.Result == _idScenario
                        )
                        {
                            return;
                        }
                        RFID rfid = _database.GetRFIDByTag(task.Result);
                        if (rfid == null)
                        {
                            return;
                        }
                        if (rfid.PageId == _currentPage)
                        {
                            ChangePage(rfid.LinkTo);
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
        }
    }
}
