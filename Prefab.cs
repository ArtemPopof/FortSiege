using Godot;
using System;

public class Prefab : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public const string GROUP_NAME = "PREFAB";
    private const float CHANGE_DELTA = 0.01f;

    [Signal]
    public delegate void prefabStateChanged(bool moving);

    public bool moving;
    public Vector2 previousLinearVelocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup(GROUP_NAME);
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    moving = (previousLinearVelocity - LinearVelocity).LengthSquared() > CHANGE_DELTA;

    previousLinearVelocity = LinearVelocity;
 }
}
