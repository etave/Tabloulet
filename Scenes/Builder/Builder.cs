using Godot;
using Tabloulet.Scenes.BuilderNS.ComponentsPanelNS;
using Tabloulet.Scenes.Components.BaseNS;
using Tabloulet.Scenes.Components.TextNS;
using ComponentImage = Tabloulet.Scenes.Components.ImageNS.Image;

namespace Tabloulet.Scenes.BuilderNS
{
    public partial class Builder : Control
    {
        private Control _blueprint;
        public ComponentsPanel componentsPanel;

        private Button _addTextButton;
        private Button _addImageButton;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _blueprint = GetNode<Control>("Blueprint");
            componentsPanel = GetNode<ComponentsPanel>("ComponentsPanel");

            _addTextButton = componentsPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/TextMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );
            _addImageButton = componentsPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/ImageMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );

            _addTextButton.Pressed += AddTextButtonPressed;
            _addImageButton.Pressed += AddImageButtonPressed;
        }

        private static Base CreateBase(Control node, bool isMovable, bool inBuilderMode)
        {
            return new Base(node, isMovable, inBuilderMode);
        }

        private void AddTextButtonPressed()
        {
            string textContent = $"[color=#000000]{LoremNET.Lorem.Words(30, true)}[/color]";
            Text text =
                new(
                    textContent,
                    null,
                    20,
                    200,
                    200,
                    _blueprint.GetRect().Size.X / 2,
                    _blueprint.GetRect().Size.Y / 2,
                    0
                );
            Base textBase = CreateBase(text, true, true);
            AddComponent(textBase);
        }

        private void AddImageButtonPressed()
        {
            ComponentImage image =
                new(
                    "",
                    200,
                    200,
                    _blueprint.GetRect().Size.X / 2,
                    _blueprint.GetRect().Size.Y / 2,
                    0
                );
            Base imageBase = CreateBase(image, true, true);
            AddComponent(imageBase);
        }

        private void AddComponent(Base component)
        {
            _blueprint.AddChild(component);
        }
    }
}
