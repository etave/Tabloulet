using Godot;
using Tabloulet.Helpers;
using Tabloulet.Helpers.CustomInputEvents;
using Tabloulet.Scenes.BuilderNS;

namespace Tabloulet.Scenes.Components.BaseNS
{
    public partial class Base(Control node, bool isMovable, bool inBuilderMode, Builder builder)
        : Control
    {
        private Control _child = node;
        private InputHandler _inputHandler;

        private InputEvent _pinch;
        private InputEvent _twist;

        private Builder _builder;

        private bool _isMovable = isMovable;

        private bool _isBeingPressed = false;

        public bool IsMovable
        {
            get => _isMovable;
            set => _isMovable = value;
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _inputHandler = GetNode<InputHandler>("/root/InputHandler");

            if (inBuilderMode)
            {
                _builder = builder;
            }

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
                    TouchChild(touch);
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

        public void TouchChild(InputEventScreenTouch touch)
        {
            _isBeingPressed = touch.Pressed;
            if (!_isBeingPressed)
            {
                if (inBuilderMode && !_builder.componentsPanel.closeByUser)
                {
                    _builder.componentsPanel.OpenButtonPressed(false);
                }
            }
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
            if (!_isMovable)
            {
                return;
            }
            if (inBuilderMode)
            {
                _builder.componentsPanel.CloseButtonPressed(false);
                switch (_child)
                {
                    case IComponent component:
                        component.UpdateSizePositionRotationParameters(
                            _child.Scale.Y,
                            _child.Scale.X,
                            _child.Size.X,
                            _child.Size.Y,
                            _child.Position.X,
                            _child.Position.Y,
                            _child.RotationDegrees
                        );
                        break;
                }
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
            if (!_isMovable)
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
            else if (inBuilderMode && !_builder.componentsPanel.closeByUser)
            {
                _builder.componentsPanel.OpenButtonPressed(false);
            }
        }
    }
}
