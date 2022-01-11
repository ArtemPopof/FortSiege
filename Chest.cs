using Godot;
using System;

public class Chest : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public const string GROUP_NAME = "CHESTS";

    private const int MAX_COINS = 5;

    [Signal]
    public delegate void ChestDestroyed(int coinsInside, Vector2 position);

    private DamageController damageController;

    [Export]
    public PackedScene brokenObjectScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetParent().AddToGroup(GROUP_NAME);

        damageController = new DamageController();
        damageController.Init(this).WithAccelerationBasedDetectionEnabled(true).WithForceThreshold(45000);
        damageController.Connect("OnDestroyed", this, "OnDestroyed");
    }

    public void OnDestroyed()
    {
        var scene = brokenObjectScene.Instance();
        GetParent().AddChild(scene);

        EmitSignal("ChestDestroyed", GD.RandRange(0, MAX_COINS), this.GlobalPosition);

        Visible = false;
        QueueFree();
    }

    public override void _IntegrateForces(Physics2DDirectBodyState physicsState)
    {
        damageController.CalculateDamage(physicsState);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
