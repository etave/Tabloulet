using System;
using Godot;

namespace Tabloulet.Scenes.BuilderNS.ComponentsPanelNS
{
    public partial class ComponentsPanel : Control
    {
        private Panel _openPanel;
        private Panel _closePanel;
        private Button _closeButton;
        private Button _openButton;

        public bool closeByUser;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _openPanel = GetNode<Panel>("OpenPanel");
            _closePanel = GetNode<Panel>("ClosePanel");

            _closeButton = _openPanel.GetNode<Button>(
                "VBoxContainer/PanelContainer/MarginContainer2/PanelContainer/Button"
            );
            _openButton = _closePanel.GetNode<Button>(
                "VBoxContainer/PanelContainer/MarginContainer/PanelContainer/Button"
            );

            _closeButton.Pressed += () => CloseButtonPressed(true);
            _openButton.Pressed += () => OpenButtonPressed(true);

            closeByUser = false;
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
    }
}
