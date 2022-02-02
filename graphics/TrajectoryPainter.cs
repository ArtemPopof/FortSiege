using Godot;
using System;

public class TrajectoryPainter : ColorRect
{
    [Export]
    private Color color = Colors.White;
    [Export]
    private float width = 5.0f;
    [Export]
    private float sublineWidth = 10;

    private float groundY = 0;

    private float linearDamp = 0.1f;

    private Vector2 startVelocity = new Vector2(0, 0);

    public override void _Ready()
    {
        startVelocity = new Vector2(40, -40);
        Color = new Color(0, 0, 0, 0);
        SetGroundY(GetViewportRect().Size.y);
    }

    public void SetGroundY(float groundY)
    {
        this.groundY = groundY - RectGlobalPosition.y;
        GD.Print("GROUNDY: " + this.groundY);
    }

    public void SetStartVelocity(Vector2 velocity)
    {
        //GD.Print("[TrajectoryPainter] New velocity: " + velocity);
        startVelocity = new Vector2(velocity.x * 0.9f, velocity.y * 0.9f);
        Update();
    }


    public override void _Draw()
    {
        if (startVelocity.x == 0 && startVelocity.y == 0) return;

        var h = RectGlobalPosition.y;

        var step = 0.1f;

        var dy = 0f;
        var dy2 = 0f;

        var dx = 0f;
        var dx2 = dx + step;

        var t = 0f;

        var dumpByStep = linearDamp * step;

        var currentVelocityX = startVelocity.x;

        //GD.Print("LinearDump: " + linearDamp);
        while (dy2 < groundY) {
            dx = startVelocity.x * t;
            dy = startVelocity.y * t + 0.5f * 98f * t * t;

            t += step;

            dx2 = startVelocity.x * t;
            dy2 = startVelocity.y * t + 0.5f * 98f * t * t;
            
            DrawLine(new Vector2(dx, dy), new Vector2(dx2, dy2), Colors.RebeccaPurple, width);

            t += step;
        }
    }

    public void SetTrajectoryStart(Vector2 position)
    {
        //GD.Print("[TrajectoryPainter] New position: " + position);

        RectGlobalPosition = position;
    }


}
