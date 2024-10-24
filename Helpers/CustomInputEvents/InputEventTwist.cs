using Godot;

namespace Tabloulet.Helpers.CustomInputEvents
{
    public partial class InputEventTwist(float angle) : InputEventAction
    {
        public float Angle = angle;
    }
}
