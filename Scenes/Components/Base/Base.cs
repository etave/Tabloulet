using System;
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

        public bool IsMovable
        {
            get => _isMovable;
            set
            {
                _isMovable = value;
                if (_child is IComponent component && component.IsMovable != value)
                {
                    component.IsMovable = value;
                }
            }
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
            _builder.editComponentPanel.RemoveCurrentComponent();

            _child.Position += drag.ScreenRelative;
        }

        public void TouchChild(InputEventScreenTouch touch)
        {
            if (!inBuilderMode)
            {
                return;
            }
            if (!touch.Pressed)
            {
                if (!_builder.createComponentPanel.closeByUser)
                {
                    _builder.createComponentPanel.OpenButtonPressed(false);
                }
                if (!_builder.editComponentPanel.closeByUser)
                {
                    _builder.editComponentPanel.OpenButtonPressed(false);
                }
            }
            else if (touch.Pressed)
            {
                _builder.editComponentPanel.SetCurrentComponent(this);
            }
        }

        public void PinchChild(InputEventPinch pinch)
        {
            _builder.editComponentPanel.RemoveCurrentComponent();

            float newScaleX = _child.Scale.X * pinch.Factor;
            float newScaleY = _child.Scale.Y * pinch.Factor;

            newScaleX = Mathf.Max(newScaleX, 0.25f);
            newScaleY = Mathf.Max(newScaleY, 0.25f);

            _child.PivotOffset = _child.Size / 2;

            _child.Scale = new Vector2(newScaleX, newScaleY);
        }

        public void TwistChild(InputEventTwist twist)
        {
            _builder.editComponentPanel.RemoveCurrentComponent();

            _child.PivotOffset = _child.Size / 2;

            _child.RotationDegrees += twist.Angle * Mathf.RadToDeg(0.05f);
        }

        private void ChildGuiInput(InputEvent @event)
        {
            if (!_isMovable)
            {
                if (@event is InputEventScreenTouch touch && touch.Pressed)
                {
                    _builder.editComponentPanel.SetCurrentComponent(this);
                }
                return;
            }
            if (inBuilderMode)
            {
                _builder.createComponentPanel.CloseButtonPressed(false);
                _builder.editComponentPanel.CloseButtonPressed(false);
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
                            _child.RotationDegrees,
                            _child.ZIndex
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
            if (inBuilderMode)
            {
                if (!_builder.createComponentPanel.closeByUser)
                {
                    _builder.createComponentPanel.OpenButtonPressed(false);
                }
                if (!builder.editComponentPanel.closeByUser)
                {
                    _builder.editComponentPanel.OpenButtonPressed(false);
                }
            }
        }

        public void changePage(Guid linkTo)
        {
            if (inBuilderMode)
            {
                this._builder.ChangePage(linkTo);
            }
        }
    }
}
