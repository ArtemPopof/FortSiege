using Godot;

public class MobileCamera : Node2D
{

    enum CameraState {
        IDLE,
        DRAG,
        ZOOM
    }

    [Export]
    public float zoomSpeed = 0.01f;

    [Signal]
    public delegate void Moved(Vector2 position);
    [Signal]
    public delegate void ZoomChanged(Vector2 zoom);

    public bool Enabled { get; set; }

    private Vector2[] touches;
    private Vector2 cameraDragStartPos;
    private float zoomStartDistance;

    private float touchZoomSpeed;

    private CameraState state;

    private Camera2D camera;

    public override void _Ready()
    {
        Enabled = false;
        camera = GetNode<Camera2D>("Camera2D");

        touchZoomSpeed = GetViewportRect().Size.x;
        touchZoomSpeed *= touchZoomSpeed * 10;

        touches = new Vector2[2];
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Enabled)
        {
            return;
        }

        if (@event is InputEventPanGesture panGesture)
        {
            if (panGesture.Delta.y > 0)
            {
                var zoom = new Vector2(camera.Zoom.x - zoomSpeed * panGesture.Delta.LengthSquared() / 10, camera.Zoom.y - zoomSpeed * panGesture.Delta.LengthSquared() / 10);
                if (zoom.x < 0.1)
                {
                    zoom = new Vector2(0.1f, 0.1f);
                }
                camera.Zoom = zoom;
            }
            else
            {
                var zoom = camera.Zoom = new Vector2(camera.Zoom.x + zoomSpeed * panGesture.Delta.LengthSquared() / 10, camera.Zoom.y + zoomSpeed * panGesture.Delta.LengthSquared() / 10);
                if (zoom.x > 1)
                {
                    zoom = new Vector2(1, 1);
                }
                camera.Zoom = zoom;
            }

            Position = GetPositionInBounds(Position);

            EmitSignal("Moved", Position);
            EmitSignal("ZoomChanged", camera.Zoom);

            return;
        }


        if (@event is InputEventScreenTouch touch)
		{
            if (touch.IsPressed())
            {
                if (state == CameraState.DRAG && touch.Index > 0)
                {
                    GD.Print("Touch second time: " + touch.Position);

                    state = CameraState.ZOOM;
                    touches[1] = touch.Position;
                    zoomStartDistance = touches[1].DistanceSquaredTo(touches[0]);

                    return;
                }

                GD.Print("Start moving camera: " + touch.Position);
                state = CameraState.DRAG;
                touches[0] = touch.Position;
                cameraDragStartPos = Position;
            }
            else
            {
                if (state == CameraState.ZOOM)
                {
                    state = CameraState.DRAG;
                }
                else 
                {
                    state = CameraState.IDLE;
                }
                GD.Print("Finish moving camera");
            }

            return;
		}

        if (!(@event is InputEventScreenDrag drag))
		{
            return;
		}

        if (state == CameraState.ZOOM)
        {
            touches[drag.Index] = drag.Position;

            var zoomDiff = ((touches[1].DistanceSquaredTo(touches[0]) - zoomStartDistance) / zoomStartDistance) * zoomSpeed;

            camera.Zoom = new Vector2(camera.Zoom.x - zoomDiff, camera.Zoom.y - zoomDiff);
            if (camera.Zoom.x > 1f)
            {
                camera.Zoom = new Vector2(1f, 1f);
            }
            if (camera.Zoom.x < 0.1f)
            {
                camera.Zoom = new Vector2(0.1f, 0.1f);
            }

            EmitSignal("ZoomChanged", camera.Zoom);

            GD.Print("Zoomed " + camera.Zoom);

            // center camera between two touch points
            var centeredPosition = (touches[0] + touches[1]) / 2;

             //GD.Print("Central point: " + centeredPosition);

            var cameraViewportWidth = camera.Zoom.x * GetViewportRect().Size.x;
            var cameraViewportHeight = camera.Zoom.y * GetViewportRect().Size.y;
            var cameraDiff = (centeredPosition - new Vector2(cameraViewportWidth / 2.0f, cameraViewportHeight / 2.0f) ) * new Vector2(zoomDiff, zoomDiff);

            Position = GetPositionInBounds(Position + cameraDiff);
            EmitSignal("Moved", Position);
            return;
        }

        var diff = (touches[drag.Index] - drag.Position) * camera.Zoom;
        //touches[drag.Index] = drag.Position;

        GD.Print("Diff: " + diff);
        GD.Print("touches[0]: " + touches[drag.Index]);
        var cameraPos = cameraDragStartPos + diff;

        GD.Print(cameraPos);

        Position = GetPositionInBounds(cameraPos);
        EmitSignal("Moved", Position);
    }

    private Vector2 GetPositionInBounds(Vector2 position)
    {
        var cameraViewportWidth = camera.Zoom.x * GetViewportRect().Size.x;
        var cameraViewportHeight = camera.Zoom.y * GetViewportRect().Size.y;

        if (position.x < 0)
        {
            position.x = 0;
        }
        if (position.x + cameraViewportWidth > GetViewportRect().Size.x)
        {
            position.x = GetViewportRect().Size.x - cameraViewportWidth;
        }
        if (position.y < 0)
        {
            position.y = 0;
        }
        if (position.y + cameraViewportHeight > GetViewportRect().Size.y)
        {
            position.y = GetViewportRect().Size.y - cameraViewportHeight;
        }

        return position;
    }

}
