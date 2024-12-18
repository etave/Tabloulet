using System;
using System.Threading.Tasks;
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

        private Timer _rfidTimer;

        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");

            _scenarioLoader = new ScenarioLoader(_database, this);

            _rfidTimer = GetNode<Timer>("RFIDTimer");
            _rfidTimer.Timeout += OnRFIDTimerTimeout;
            _rfidTimer.Start();
        }

        public void Init(Guid idScenario)
        {
            _currentPage = _scenarioLoader.LoadScenario(idScenario);
            _idScenario = idScenario;
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
