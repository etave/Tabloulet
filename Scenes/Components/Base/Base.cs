using Godot;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Helpers.CustomInputEvents;

namespace Tabloulet.Scenes.Components.BaseNS
{
    public partial class Base(Control node, bool IsMovable) : Node
    {
        private Control _child = node;
        private InputHandler _inputHandler;

        private InputEvent _pinch;
        private InputEvent _twist;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (_child != null)
            {
                _child.GuiInput += ChildGuiInput;
                AddChild(_child);
            }
        }

        public void UpdateChildFromInputEvent(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventScreenDrag drag:
                    DragChild(drag);
                    break;
                case InputEventScreenTouch touch:
                    break;
                case InputEventPinch pinch:
                    PinchChild(pinch);
                    break;
                case InputEventTwist twist:
                    TwistChild(twist);
                    break;
                default:
                    break;
            }
        }

        public void DragChild(InputEventScreenDrag drag)
        {
            _child.Position += drag.ScreenRelative;
        }

        public void PinchChild(InputEventPinch pinch)
        {
            float newScaleX = _child.Scale.X * pinch.Factor;
            float newScaleY = _child.Scale.Y * pinch.Factor;

            newScaleX = Mathf.Max(newScaleX, 0.25f);
            newScaleY = Mathf.Max(newScaleY, 0.25f);

            _child.PivotOffset = _child.Size / 2;

            _child.Scale = new Vector2(newScaleX, newScaleY);
        }

        public void TwistChild(InputEventTwist twist)
        {
            _child.PivotOffset = _child.Size / 2;

            _child.RotationDegrees += twist.Angle * Mathf.RadToDeg(0.05f);
        }

        private void ChildGuiInput(InputEvent @event)
        {
            if (!IsMovable)
            {
                return;
            }
            if (_pinch != null)
            {
                UpdateChildFromInputEvent(_pinch);
                _pinch = null;
                return;
            }
            else if (_twist != null)
            {
                UpdateChildFromInputEvent(_twist);
                _twist = null;
                return;
            }
            UpdateChildFromInputEvent(@event);
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (!IsMovable)
            {
                return;
            }
            if (@event is InputEventPinch)
            {
                _pinch = @event;
            }
            else if (@event is InputEventTwist)
            {
                _twist = @event;
            }
        }
    }
}
