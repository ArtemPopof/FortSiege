using Godot;

public class MobileCamera : Node2D
{

    enum CameraState {
        IDLE,
        DRAG,
        ZOOM
    }

    [Signal]
    public delegate void Moved(Vector2 position);

    public bool Enabled { get; set; }

    private Vector2 dragStartPos;
    private Vector2 cameraDragStartPos;

    private CameraState state;

    public override void _Ready()
    {
        Enabled = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Enabled)
        {
            return;
        }

        if (@event is InputEventScreenTouch touch)
		{
            if (touch.IsPressed())
            {
                GD.Print("Start moving camera");
                //state = CameraState.DRAG;
                dragStartPos = touch.Position;
                cameraDragStartPos = Position;
            }
            else
            {
                GD.Print("Finish moving camera");
            }

            return;
		}

        if (!(@event is InputEventScreenDrag drag))
		{
            return;
		}

        var diff = (drag.Position - dragStartPos);
        Position = new Vector2(cameraDragStartPos.x - diff.x, cameraDragStartPos.y + diff.y);

        EmitSignal("Moved", Position);
    }

}
