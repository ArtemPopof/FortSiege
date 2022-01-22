using Godot;

public class StaticToCamera : Node2D
{
    [Export]
    public MobileCamera camera;

    public void Connect(MobileCamera camera)
    {
        this.camera = camera;
        camera.Connect("Moved", this, "CameraMoved");
    }

    private void CameraMoved(Vector2 position)
    {
        Position = position;
    }
}
