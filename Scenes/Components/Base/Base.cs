using Godot;

namespace Tabloulet.Scenes.Components.BaseNS
{
    public partial class Base(Control node) : Node
    {
        private Node _inputEventHelper;
        private Control _child = node;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _inputEventHelper = GetNode<Node>("/root/InputEventH");
            if (_child != null)
            {
                AddChild(_child);
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta) { }

        public void UpdateFromInputEvent(InputEvent @event)
        {
            if (_child == null)
            {
                return;
            }
            if (!_child.GetRect().HasPoint(_child.GetGlobalMousePosition()))
            {
                return;
            }
            int eventType = (int)_inputEventHelper.Call("_get_event_type", @event);
            switch (eventType)
            {
                case 0:
                    GD.Print(0);
                    break;
                case 1:
                    GD.Print(1);
                    break;
                case 2:
                    GD.Print(2);
                    break;
                case 3:
                    GD.Print(3);
                    break;
                case 4:
                    GD.Print(4);
                    break;
                case 5:
                    GD.Print(5);
                    break;
                case 6:
                    GD.Print(6);
                    break;
                case 7:
                    GD.Print(7);
                    break;
                case 8:
                    GD.Print(8);
                    break;
                case 9:
                    GD.Print(9);
                    break;
                case 10:
                    GD.Print(10);
                    break;
                case 11:
                    GD.Print(11);
                    break;
                case -1:
                    break;
            }
        }
    }
}
