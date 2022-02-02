using Godot;
using System;
using System.Collections.Generic;

public class cannon : Weapon
{
    private const float MAX_ROTATION = 0.35f;
    private const float MIN_ROTATION = -0.5019f;

    [Export]
    public Vector2 testVelocity = new Vector2(700, -300);

    [Export]
	public PackedScene BallScene;

    private Ball ball;

    private Vector2 lastLookAtPosition;

    List<Ball> balls = new List<Ball>(3);

    private bool enabled;
    private bool fired;

    private float force;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        fired = false;
        force = 0.1f;
        lastLookAtPosition = new Vector2(Position.x + 100, Position.y);
    }

    public override int GetIndex()
    {
        return 0;
    }

    public void ShaftPressed()
    {
        GD.Print("[Cannon] Start rotating");

        currentState = FireState.SET_TRAJECTORY;
    }

    public override void _Input(InputEvent @event)
	{                
        if (!Visible || !enabled || currentState != FireState.SET_TRAJECTORY) {
            return;
        }

        if (@event is InputEventScreenTouch touch)
		{
            if (!touch.Pressed)
            {
                currentState = FireState.READY;
            }
            return;
		}

		if (!(@event is InputEventScreenDrag drag))
		{
            return;
		}

        RotateCannonToTouchPoint(drag.Position);
	}

    private void RotateCannonToTouchPoint(Vector2 touchPoint) {
        //var limitedPoint = LimitRotationPoints();

        var cannonShaft = GetNode<StaticBody2D>("CannonShaft");

        cannonShaft.LookAt(touchPoint);

        if (cannonShaft.Rotation > MAX_ROTATION) {
            cannonShaft.Rotation = MAX_ROTATION;
        } else if (cannonShaft.Rotation < MIN_ROTATION) {
            cannonShaft.Rotation = MIN_ROTATION;
        }

        lastLookAtPosition = touchPoint;


        EmitSignal("ProjectilePositionChanged", GetNode<Position2D>("CannonShaft/BallPosition").GlobalPosition);
        EmitSignal("FireVelocityChanged", GetBallVelocity());
    }

    private Vector2 LimitRotationPoints(Vector2 touchPoint) {
        var resultPoint = new Vector2();

        return resultPoint;
    }

    private Vector2 GetBallVelocity() {
        var axis = GetNode<Position2D>("CannonShaft/BallPosition").GlobalPosition;

        var rotation = GetNode<Node2D>("CannonShaft").GlobalRotation;

        var xVelocity = 4 * GetViewportRect().Size.x * 0.1f;
        var yVelocity = xVelocity * Mathf.Sin(rotation);

        GD.Print("[Cannon] Rotation: " + rotation);

        return new Vector2(Math.Max(10, xVelocity * force), yVelocity * force);
    }

    public override void Fire() {
        var shape = GetNode<CollisionShape2D>("CannonShaft/CollisionShape2D");
        var ballInstance = (Ball) BallScene.Instance();
        ballInstance.Mass = info.projectileWeight;
        AddChild(ballInstance);
        usedProjectiles.Add(ballInstance);

        var ballPosition = (Position2D) GetNode<Position2D>("CannonShaft/BallPosition");
        ballInstance.GlobalPosition = ballPosition.GlobalPosition;
        ball = ballInstance;

        balls.Add(ballInstance);

        var velocity = GetBallVelocity();
        ball.Fire(velocity.x , velocity.y);

        fired = true;

        var explosion = GetNode<Particles2D>("CannonShaft/Explosion");
        explosion.Emitting = true;
        GetNode<AudioStreamPlayer2D>("ExplosionSound").Play();

        currentState = FireState.READY;

        EmitSignal("Fired");
    }

    public override Vector2 GetProjectileStartPosition()
    {
        return GetNode<Position2D>("CannonShaft/BallPosition").GlobalPosition;
    }

    public override void Reset() {
        GD.Print("Reset cannon state");

        foreach (var cannonBall in balls)
        {
            cannonBall.Visible = false;
            cannonBall.QueueFree();
        }
        balls.Clear();

        enabled = true;
        fired = false;

        force = 0.1f;
    }

    public override void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public override void SetTrajectory(float value)
    {
        // NO IMPLEMENTATION FOR CANNON YET, CONTROLLED BY TOUCHING THE SCREEN
    }

    public override void SetForce(float value)
    {
        GD.Print("[Cannon] Set force to " + value);
        force = value;
        EmitSignal("FireVelocityChanged", GetBallVelocity());

        GD.Print("[Cannon] New velocity: " + GetBallVelocity());
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//  }
}
