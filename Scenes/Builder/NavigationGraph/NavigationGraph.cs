using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using ButtonModel = Tabloulet.DatabaseNS.Models.Button;
using GDButton = Godot.Button;
using RFIDModel = Tabloulet.DatabaseNS.Models.RFID;

namespace Tabloulet.Scenes.BuilderNS.NavigationGraphNS
{
    public partial class NavigationGraph : Control
    {
        private Database _database;
        private Guid _idScenario;

        GraphArranger _graphArranger;

        private GDButton _closeButton;
        private GraphEdit _graphEdit;
        private List<Page> _pages;
        private Texture2D _buttonTexture;
        private Texture2D _rfidTexture;
        private Dictionary<Guid, List<ButtonModel>> _buttonsByPage;
        private Dictionary<Guid, List<RFIDModel>> _rfidsByPage;
        private Dictionary<Guid, GraphNode> _graphNodes;
        private Dictionary<GraphNode, Dictionary<IDatabaseModel, int>> _slotIndicesByGraphNode;

        private GDButton _newPageButton;
        private GDButton _newRFIDButton;

        private Panel _newPagePopupPanel;
        private LineEdit _newPageName;
        private GDButton _newPageCreateButton;
        private GDButton _newPageCancelButton;

        private Panel _newRFIDPopupPanel;
        private LineEdit _newRFIDName;
        private OptionButton _newRFIDPageSource;
        private OptionButton _newRFIDPageTarget;
        private LineEdit _newRFIDTag;
        private GDButton _newRFIDScanButton;
        private GDButton _newRFIDCreateButton;
        private GDButton _newRFIDCancelButton;

        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");
            _graphArranger = new GraphArranger();

            _closeButton = GetNode<GDButton>(
                "MarginContainer/PanelContainer/VBoxContainer/TopPanelContainer/MarginContainer2/PanelContainer/Button"
            );
            _closeButton.Pressed += () => QueueFree();
            _graphEdit = GetNode<GraphEdit>(
                "MarginContainer/PanelContainer/VBoxContainer/BottomMarginContainer/GraphEdit"
            );
            _buttonTexture = GD.Load<Texture2D>(
                "res://Assets/Builder/NavigationGraph/PressButton.png"
            );
            _rfidTexture = GD.Load<Texture2D>("res://Assets/Builder/NavigationGraph/RFID.png");
            _buttonsByPage = [];
            _rfidsByPage = [];
            _graphNodes = [];
            _slotIndicesByGraphNode = [];

            _newPageButton = GetNode<GDButton>("NewPageButtonPanel/MarginContainer/NewPageButton");
            _newPagePopupPanel = GetNode<Panel>("NewPagePopupPanel");
            _newPageName = _newPagePopupPanel.GetNode<LineEdit>(
                "VBoxContainer/MarginContainer/LineEdit"
            );
            _newPageCreateButton = _newPagePopupPanel.GetNode<GDButton>(
                "VBoxContainer/ButtonsHBoxContainer/CreateButton"
            );
            _newPageCancelButton = _newPagePopupPanel.GetNode<GDButton>(
                "VBoxContainer/ButtonsHBoxContainer/CancelButton"
            );
            _newPageButton.Pressed += NewPageButtonPressed;
            _newPageName.TextChanged += OnNewPageNameTextChanged;
            _newPageCreateButton.Pressed += NewPageCreateButtonPressed;
            _newPageCancelButton.Pressed += NewPageCancelButtonPressed;

            _newRFIDButton = GetNode<GDButton>("NewRFIDButtonPanel/MarginContainer/NewRFIDButton");
            _newRFIDPopupPanel = GetNode<Panel>("NewRFIDPopupPanel");
            _newRFIDName = _newRFIDPopupPanel.GetNode<LineEdit>(
                "VBoxContainer/MarginContainer/LineEdit"
            );
            _newRFIDPageSource = _newRFIDPopupPanel.GetNode<OptionButton>(
                "VBoxContainer/MarginContainer2/VBoxContainer/HBoxContainer/OptionButton"
            );
            _newRFIDPageTarget = _newRFIDPopupPanel.GetNode<OptionButton>(
                "VBoxContainer/MarginContainer2/VBoxContainer/HBoxContainer/OptionButton2"
            );
            _newRFIDTag = _newRFIDPopupPanel.GetNode<LineEdit>(
                "VBoxContainer/MarginContainer3/VBoxContainer/LineEdit"
            );
            _newRFIDScanButton = _newRFIDPopupPanel.GetNode<GDButton>(
                "VBoxContainer/MarginContainer3/VBoxContainer/MarginContainer/ButtonScan"
            );
            _newRFIDCreateButton = _newRFIDPopupPanel.GetNode<GDButton>(
                "VBoxContainer/ButtonsHBoxContainer/CreateButton"
            );
            _newRFIDCancelButton = _newRFIDPopupPanel.GetNode<GDButton>(
                "VBoxContainer/ButtonsHBoxContainer/CancelButton"
            );
            _newRFIDName.TextChanged += OnNewRFIDNameTextChanged;
            _newRFIDButton.Pressed += NewRFIDButtonPressed;
            _newRFIDCreateButton.Pressed += NewRFIDCreateButtonPressed;
            _newRFIDCancelButton.Pressed += NewRFIDCancelButtonPressed;
            _newRFIDScanButton.Pressed += NewRFIDScanButtonPressed;
        }

        public void LoadGraph(Guid scenarioId)
        {
            _idScenario = scenarioId;
            _pages = _database.GetPagesByScenario(scenarioId);
            foreach (ButtonModel button in _database.GetButtonsByScenario(scenarioId))
            {
                if (!_buttonsByPage.TryGetValue(button.PageId, out List<ButtonModel> value))
                {
                    value = [];
                    _buttonsByPage[button.PageId] = value;
                }

                value.Add(button);
            }

            foreach (RFIDModel rfid in _database.GetRFIDsByScenario(scenarioId))
            {
                if (!_rfidsByPage.TryGetValue(rfid.PageId, out List<RFIDModel> value))
                {
                    value = [];
                    _rfidsByPage[rfid.PageId] = value;
                }

                value.Add(rfid);
            }

            foreach (Page page in _pages)
            {
                AddNodeToGraph(page);
            }

            ConnectSlots();
            ArrangeNodes();
        }

        private void ArrangeNodes()
        {
            _graphArranger.ArrangeGraph(_graphEdit);
        }

        private void AddNodeToGraph(Page page)
        {
            GraphNode node =
                new()
                {
                    Name = page.Id.ToString(),
                    Title = page.Name,
                    Size = new Vector2(200, 55),
                    PositionOffset = new Vector2(100, 100),
                };

            Label label = new();
            label.AddThemeFontSizeOverride("font_size", 1);
            node.AddChild(label);
            node.SetSlot(0, true, 0, new Color(1, 1, 1), true, 0, new Color(1, 1, 1, 0));

            int slotIndex = 1;
            List<int> slotIndices = [];

            if (_buttonsByPage.TryGetValue(page.Id, out List<ButtonModel> buttons))
            {
                foreach (ButtonModel button in buttons)
                {
                    AddSlotToNode(node, button, ref slotIndex);
                    slotIndices.Add(slotIndex - 1);
                    if (
                        !_slotIndicesByGraphNode.TryGetValue(
                            node,
                            out Dictionary<IDatabaseModel, int> value
                        )
                    )
                    {
                        value = [];
                        _slotIndicesByGraphNode[node] = value;
                    }
                    value[button] = slotIndex - 1;
                }
            }

            if (_rfidsByPage.TryGetValue(page.Id, out List<RFIDModel> rfids))
            {
                foreach (RFIDModel rfid in rfids)
                {
                    AddSlotToNode(node, rfid, ref slotIndex);
                    slotIndices.Add(slotIndex - 1);
                    if (
                        !_slotIndicesByGraphNode.TryGetValue(
                            node,
                            out Dictionary<IDatabaseModel, int> value
                        )
                    )
                    {
                        value = [];
                        _slotIndicesByGraphNode[node] = value;
                    }
                    value[rfid] = slotIndex - 1;
                }
            }
            _graphEdit.AddChild(node);
            _graphNodes[page.Id] = node;
            node.GuiInput += (InputEvent @event) =>
            {
                if (@event is InputEventScreenTouch touch)
                {
                    if (touch.Pressed && touch.DoubleTap)
                    {
                        Builder builder = GetNode<Builder>("/root/Builder");
                        builder.ChangePage(page.Id);
                        QueueFree();
                    }
                }
            };
        }

        private void AddSlotToNode(GraphNode node, IDatabaseModel model, ref int slotIndex)
        {
            Label label = new() { HorizontalAlignment = HorizontalAlignment.Center };
            switch (model)
            {
                case ButtonModel button:
                    label.Text = button.Content;
                    node.SetSlot(
                        slotIndex,
                        false,
                        0,
                        new Color(1, 1, 1),
                        true,
                        0,
                        new Color(1.0f, 0.3f, 0.3f)
                    );
                    node.SetSlotCustomIconRight(slotIndex, _buttonTexture);
                    break;
                case RFIDModel rfid:
                    label.Text = rfid.Name;
                    node.SetSlot(
                        slotIndex,
                        false,
                        0,
                        new Color(1, 1, 1),
                        true,
                        0,
                        new Color(0.3f, 0.3f, 1.0f)
                    );
                    node.SetSlotCustomIconRight(slotIndex, _rfidTexture);
                    break;
            }
            node.AddChild(label);
            slotIndex++;
        }

        private void UpdateNodeRFID(Guid node1, Guid node2)
        {
            if (_graphNodes.TryGetValue(node1, out GraphNode node))
            {
                _graphEdit.RemoveChild(node);
                AddNodeToGraph(_database.GetById<Page>(node1));
            }
            if (_graphNodes.TryGetValue(node2, out node))
            {
                _graphEdit.RemoveChild(node);
                AddNodeToGraph(_database.GetById<Page>(node2));
            }
            ConnectSlots();
            ArrangeNodes();
        }

        private void ConnectSlots()
        {
            foreach (var page in _pages)
            {
                if (_graphNodes.TryGetValue(page.Id, out GraphNode node))
                {
                    if (
                        _slotIndicesByGraphNode.TryGetValue(
                            node,
                            out Dictionary<IDatabaseModel, int> value
                        )
                    )
                    {
                        Dictionary<IDatabaseModel, int> keyValuePairs = value;
                        foreach (var keyValuePair in keyValuePairs)
                        {
                            ConnectToTargetNode(node, keyValuePair);
                        }
                    }
                }
            }
        }

        private void ConnectToTargetNode(
            GraphNode node,
            KeyValuePair<IDatabaseModel, int> keyValuePair
        )
        {
            GraphNode targetNode;
            switch (keyValuePair.Key)
            {
                case ButtonModel button:
                    if (
                        button.LinkTo != null
                        && _graphNodes.TryGetValue((Guid)button.LinkTo, out targetNode)
                    )
                    {
                        _graphEdit.ConnectNode(node.Name, keyValuePair.Value, targetNode.Name, 0);
                    }
                    break;
                case RFIDModel rfid:
                    if (_graphNodes.TryGetValue(rfid.LinkTo, out targetNode))
                    {
                        _graphEdit.ConnectNode(node.Name, keyValuePair.Value, targetNode.Name, 0);
                    }
                    break;
            }
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
                    ScenarioId = _idScenario,
                    PageId = page.Id,
                };
            _database.Insert(scenarioPage);
            _newPagePopupPanel.Visible = false;
            _newPageName.Text = "Nouvelle Page";
            AddNodeToGraph(page);
            ConnectSlots();
            ArrangeNodes();
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

        private void OnNewRFIDNameTextChanged(string newText)
        {
            if (newText.Length > 0)
            {
                _newRFIDCreateButton.Disabled = false;
            }
            else
            {
                _newRFIDCreateButton.Disabled = true;
            }
        }

        private void NewRFIDButtonPressed()
        {
            _newRFIDPopupPanel.Visible = true;
            List<Page> pages = _database.GetPagesByScenario(_idScenario);
            foreach (Page page in pages)
            {
                _newRFIDPageSource.AddItem(page.Name);
                _newRFIDPageTarget.AddItem(page.Name);
            }
        }

        private void NewRFIDCreateButtonPressed()
        {
            string name = _newRFIDName.Text;
            List<Page> pages = _database.GetPagesByScenario(_idScenario);
            string sourcePageName = _newRFIDPageSource.GetItemText(_newRFIDPageSource.Selected);
            Guid sourcePageId = pages.Find(page => page.Name == sourcePageName).Id;
            string targetPageName = _newRFIDPageTarget.GetItemText(_newRFIDPageTarget.Selected);
            Guid targetPageId = pages.Find(page => page.Name == targetPageName).Id;
            Guid rfidTag = Guid.Parse(_newRFIDTag.Text);
            RFIDModel rfid =
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    PageId = sourcePageId,
                    LinkTo = targetPageId,
                    RFIDTag = rfidTag,
                };
            _database.Insert(rfid);
            if (!_rfidsByPage.TryGetValue(rfid.PageId, out List<RFIDModel> value))
            {
                value = [];
                _rfidsByPage[rfid.PageId] = value;
            }
            value.Add(rfid);
            UpdateNodeRFID(sourcePageId, targetPageId);

            _newRFIDPageSource.Clear();
            _newRFIDPageTarget.Clear();
            _newRFIDName.Text = "Nouveau lien RFID";
            _newRFIDTag.Text = Guid.Empty.ToString();
            _newRFIDPopupPanel.Visible = false;
        }

        private void NewRFIDCancelButtonPressed()
        {
            _newRFIDPopupPanel.Visible = false;
            _newRFIDPageSource.Clear();
            _newRFIDPageTarget.Clear();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (!_newRFIDPopupPanel.Visible)
            {
                return;
            }

            try
            {
                Guid.Parse(_newRFIDTag.Text);
            }
            catch (FormatException)
            {
                _newRFIDCreateButton.Disabled = true;
                return;
            }

            if (
                _newRFIDPageSource.Selected == _newRFIDPageTarget.Selected
                || _newRFIDName.Text.Length == 0
                || Guid.Parse(_newRFIDTag.Text) == Guid.Empty
            )
            {
                _newRFIDCreateButton.Disabled = true;
            }
            else
            {
                _newRFIDCreateButton.Disabled = false;
            }
        }

        private void NewRFIDScanButtonPressed()
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
                        _newRFIDTag.Text = task.Result.ToString();
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
        }
    }
}
