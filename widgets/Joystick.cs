using Godot;
using System;

enum JoystickState
{
    IDLE,
    ADJUSTING,
    RELEASE
}
public class Joystick : Node2D
{
    [Signal]
    public delegate void VectorChanged(Vector2 vector);

    private static float secondsToRelease = 0.5f;

    public bool Enabled {get; set;}

    private JoystickState state;
    private Vector2 startPoint;

    private Panel joystickHandle;
    private Vector2 startHandlePosition;
    private Vector2 lastHandleReleasePos;
    private float releaseTimeLeft;

    public override void _Ready()
    {
        Enabled = true;
        state = JoystickState.IDLE;

        joystickHandle = GetNode<Panel>("Panel");
        startHandlePosition = joystickHandle.RectPosition;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Enabled) return;

        if (@event is InputEventScreenTouch touch)
        {
            if (touch.IsPressed())
            {
                GD.Print("[Joystick] Start adjusting");
                state = JoystickState.ADJUSTING;
                startPoint = touch.Position;
            } 
            else
            {
                GD.Print("[Joysting] Finish adjusting");
                releaseTimeLeft = secondsToRelease;
                state = JoystickState.RELEASE;
            }

            return;
        }

		if (@event is InputEventScreenDrag drag)
		{
            var diff = drag.Position - startPoint;

            UpdateVector(diff);
		}
    }

    private void UpdateVector(Vector2 diff)
    {
        var maxed = (diff / (GetViewportRect().Size.x / 6));
        var normalized = new Vector2(Mathf.Clamp(maxed.x, -1, 1), Mathf.Clamp(maxed.y, -1, 1));
        //GD.Print("[Joystick] New vector: " + normalized);
        EmitSignal("VectorChanged", normalized);

        var clamped = diff.Clamped(joystickHandle.RectSize.x / 1.7f);
        lastHandleReleasePos = clamped;

        joystickHandle.RectPosition = startHandlePosition + clamped;
    }

    public override void _Process(float delta)
    {
        if (!Enabled || state != JoystickState.RELEASE)
        {
            return;
        }
        
        releaseTimeLeft -= delta;
        if (releaseTimeLeft < 0) releaseTimeLeft = 0;

        joystickHandle.RectPosition = startHandlePosition + (releaseTimeLeft / secondsToRelease) * lastHandleReleasePos;

        if (releaseTimeLeft == 0) state = JoystickState.IDLE;
    }
}
