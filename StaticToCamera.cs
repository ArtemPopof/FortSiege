using Godot;

public class StaticToCamera : Node2D
{
    [Export]
    public MobileCamera camera;

    public void Connect(MobileCamera camera)
    {
        this.camera = camera;
        camera.Connect("Moved", this, "CameraMoved");
        camera.Connect("ZoomChanged", this, "ZoomChanged");
    }

    private void CameraMoved(Vector2 position)
    {
        Position = position;
    }

    private void ZoomChanged(Vector2 zoom)
    {
        Scale = zoom;
    }
}
