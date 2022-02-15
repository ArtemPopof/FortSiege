using Godot;
using System;

public class Prefab : GameObject
{
    public static string GROUP_NAME = "PREFAB";
    private const float CHANGE_DELTA = 0.01f;

    [Signal]
    public delegate void prefabStateChanged(bool moving);

    public bool moving;
    public Vector2 previousLinearVelocity;

    public override void _Ready()
    {
        base._Ready();

        AddToGroup(GROUP_NAME);
    }

    public override void _Process(float delta)
    {
        moving = (previousLinearVelocity - LinearVelocity).LengthSquared() > CHANGE_DELTA;

        previousLinearVelocity = LinearVelocity;
    }
}
