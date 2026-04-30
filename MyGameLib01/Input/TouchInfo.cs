using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace MyGameLib01.Input;

public class TouchInfo
{
    private TouchCollection _currentTouches;

    public bool IsPressed { get; private set; }
    public bool WasJustPressed { get; private set; }

    public Vector2 ScreenPosition { get; private set; }
    public Vector2 WorldPosition { get; private set; }
    public SwipeDirection Swipe { get; private set; }
    public void Update()
    {
        _currentTouches = TouchPanel.GetState();

        IsPressed = _currentTouches.Count > 0;
        WasJustPressed = false;
        Swipe = SwipeDirection.None;

        if (IsPressed)
        {
            TouchLocation touch = _currentTouches[0];

            ScreenPosition = touch.Position;
            WorldPosition = Core.ScreenToWorld(touch.Position);

            if (touch.State == TouchLocationState.Pressed)
            {
                WasJustPressed = true;
            }
        }

        while (TouchPanel.IsGestureAvailable)
        {
            GestureSample gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.HorizontalDrag)
            {
                if (gesture.Delta.X > 0)
                    Swipe = SwipeDirection.Right;
                else if (gesture.Delta.X < 0)
                    Swipe = SwipeDirection.Left;
            }
            else if (gesture.GestureType == GestureType.VerticalDrag)
            {
                if (gesture.Delta.Y > 0)
                    Swipe = SwipeDirection.Down;
                else if (gesture.Delta.Y < 0)
                    Swipe = SwipeDirection.Up;
            }
        }
    }

    public bool WasJustPressedIn(Rectangle bounds)
    {
        return WasJustPressed && bounds.Contains(WorldPosition);
    }
}
