using Godot;

namespace Tabloulet.Helpers.CustomInputEvents
{
    public partial class InputEventPinch(float factor) : InputEventAction
    {
        public float Factor = factor;
    }
}
