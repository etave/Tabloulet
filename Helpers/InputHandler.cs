using System.Collections.Generic;
using Godot;
using Tabloulet.Helpers.CustomInputEvents;

namespace Tabloulet.Helpers
{
    public partial class InputHandler : Node
    {
        public InputEvent lastEvent;

        private readonly Dictionary<int, Vector2> fingers = [];
        private readonly Dictionary<int, Sprite2D> fingerSprites = [];

        private float lastPinchDistance;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() { }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            HandleFingers();
        }

        private void HandleFingers()
        {
            foreach (var finger in fingers)
            {
                if (!fingerSprites.TryGetValue(finger.Key, out Sprite2D value))
                {
                    Sprite2D fingerSprite =
                        new()
                        {
                            Texture = CreateCircleTexture(25, new Color(1, 1, 1, 0.5f)),
                            Scale = new Vector2(1, 1),
                            ZIndex = 1,
                        };
                    value = fingerSprite;
                    fingerSprites.Add(finger.Key, value);
                    AddChild(fingerSprite);
                }
                value.GlobalPosition = finger.Value;
            }
        }

        private static ImageTexture CreateCircleTexture(int radius, Color color)
        {
            Image image = Image.CreateEmpty(radius * 2, radius * 2, false, Image.Format.Rgba8);

            for (int y = 0; y < radius * 2; y++)
            {
                for (int x = 0; x < radius * 2; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }

            ImageTexture texture = ImageTexture.CreateFromImage(image);
            return texture;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            switch (@event)
            {
                case InputEventScreenDrag drag:
                    HandleDragEvent(drag);
                    lastEvent = @event;
                    break;
                case InputEventScreenTouch touch:
                    HandleTouchEvent(touch);
                    lastEvent = @event;
                    break;
                default:
                    break;
            }

            DetectPinchGesture();
            DetectTwistGesture();
        }

        private void HandleDragEvent(InputEventScreenDrag drag)
        {
            fingers[drag.Index] = drag.Position;
        }

        private void HandleTouchEvent(InputEventScreenTouch touch)
        {
            if (!touch.Pressed)
            {
                fingers.Remove(touch.Index);
                fingerSprites[touch.Index].QueueFree();
                fingerSprites.Remove(touch.Index);
                return;
            }
            fingers[touch.Index] = touch.Position;
        }

        private void DetectPinchGesture()
        {
            if (fingers.Count == 2)
            {
                List<Vector2> fingerList = new(fingers.Values);
                Vector2 finger1 = fingerList[0];
                Vector2 finger2 = fingerList[1];

                float currentDistance = finger1.DistanceTo(finger2);

                if (lastPinchDistance != 0)
                {
                    float pinchFactor = currentDistance / lastPinchDistance;

                    if (pinchFactor != 1)
                    {
                        if (lastPinchDistance > currentDistance)
                        {
                            pinchFactor = 1 - (1 - pinchFactor);
                        }

                        InputEventPinch pinchEvent = new(pinchFactor);
                        Input.ParseInputEvent(pinchEvent);
                        lastEvent = pinchEvent;
                    }
                }

                lastPinchDistance = currentDistance;
            }
            else
            {
                lastPinchDistance = 0;
            }
        }

        private float lastTwistAngle;

        public void DetectTwistGesture()
        {
            if (fingers.Count == 2)
            {
                List<Vector2> fingerList = new(fingers.Values);
                Vector2 finger1 = fingerList[0];
                Vector2 finger2 = fingerList[1];

                float currentAngle = Mathf.Atan2(finger2.Y - finger1.Y, finger2.X - finger1.X);

                if (lastTwistAngle != 0)
                {
                    float twistAngle = Mathf.RadToDeg(currentAngle - lastTwistAngle);

                    if (
                        !float.IsInfinity(twistAngle)
                        && !float.IsNaN(twistAngle)
                        && twistAngle != 0
                    )
                    {
                        InputEventTwist twistEvent = new(twistAngle);
                        Input.ParseInputEvent(twistEvent);
                        lastEvent = twistEvent;
                    }
                }

                lastTwistAngle = currentAngle;
            }
            else
            {
                lastTwistAngle = 0;
            }
        }
    }
}
