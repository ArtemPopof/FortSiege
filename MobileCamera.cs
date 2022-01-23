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
    private Vector2 zoomStartPos;
    private Vector2 dragStartPos;
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
                    GD.Print("Touch second time");
                    state = CameraState.ZOOM;
                    zoomStartPos = touch.Position;
                    zoomStartDistance = zoomStartPos.DistanceSquaredTo(dragStartPos);

                    return;
                }

                GD.Print("Start moving camera");
                state = CameraState.DRAG;
                dragStartPos = touch.Position;
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
            var newDistance = drag.Position.DistanceSquaredTo(dragStartPos);
            var altDistance = drag.Position.DistanceSquaredTo(zoomStartPos);
            var realNewDistance = Mathf.Max(newDistance, altDistance);

            var zoomDiff = (realNewDistance - zoomStartDistance) / touchZoomSpeed;

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

            GD.Print("Zoomed " + zoomDiff);

            // center camera between two touch points
            Vector2 centeredPosition;
            if (realNewDistance == newDistance)
            {
                centeredPosition = (dragStartPos - drag.Position) / 2;
            }
            else
            {
                centeredPosition = (zoomStartPos - drag.Position) / 2;
            }

            Position = GetPositionInBounds(centeredPosition);
            EmitSignal("Moved", Position);
            return;
        }

        var diff = (dragStartPos - drag.Position);
        var cameraPos = cameraDragStartPos + diff;

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
