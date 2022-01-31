using Godot;
using System;
using System.Collections.Generic;

public class Catapult : Weapon
{

    private bool settingStrength = false;
    private bool firing = false;
    private float firingTime = 0;
    private float firingForce;
    private float currentFiringSpeed;

    private Position2D ballPosition;
    private AudioStreamPlayer2D releaseSound;
    private Node2D neck;
    [Export]
	public PackedScene ballScene;

    private List<Ball> balls;
    private Ball loadedBall;

    private float maxStrengthDistance;
    private float maxTouchX;
    private float maxTouchY;
    private float maxDegreesRotation = 47.2f;
    private float firingTimeoutSeconds = -1;

    private float neckDegreesLoading = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        balls = new List<Ball>(5);

        neck = GetNode<Node2D>("Neck");
        ballPosition = GetNode<Position2D>("Neck/BallPosition");
        releaseSound = GetNode<AudioStreamPlayer2D>("ReleaseSound");

        maxTouchX = GetNode<Position2D>("MaxX").GlobalPosition.x;
        maxTouchY = neck.GlobalPosition.y - 10;
        //maxDegreesRotation = neck.GlobalRotation;
    }

    public override int GetIndex()
    {
        return 0;
    }

    private void LoadWithBall() 
    {
        var ballInstance = (Node2D) ballScene.Instance();
        AddChild(ballInstance);
        ballInstance.GlobalPosition = ballPosition.GlobalPosition;
        usedProjectiles.Add(ballInstance);
        
        balls.Add(ballInstance as Ball);

        loadedBall = ballInstance as Ball;
        (ballInstance as Ball).Mass = info.projectileWeight;
    }

    public override void Reset()
    {
        // don't do anything
    }

    public override void SetEnabled(bool enabled)
    {
        // don't do anything
    }

    public override void Fire()
    {
        NeckReleased();
    }

    public override Vector2 GetProjectileStartPosition()
    {
        return ballPosition.GlobalPosition;
    }

    public override void _Process(float delta)
    {
        if (firingTimeoutSeconds != -1) {
            firingTimeoutSeconds -= delta;
            if (firingTimeoutSeconds <= 0) {
                GetNode<TextureRect>("Neck/BallTexture").Visible = true;
                firingTimeoutSeconds = -1;
            }
        }

        if (!firing) {
            return;
        }

        currentFiringSpeed = firingForce + (firingTime * (firingForce * 82.5f));
        currentFiringSpeed /= 100.0f;

        neck.GlobalRotationDegrees = neck.GlobalRotationDegrees + currentFiringSpeed;
        if (neck.GlobalRotationDegrees > maxDegreesRotation) {
            neck.GlobalRotationDegrees = maxDegreesRotation;
            firing = false;
            FireBall();
        }

        firingTime += delta;
    }

    private void FireBall()
    {
        GD.Print("Release ball, speed: " + GetFireVelocity().x + ", " + GetFireVelocity().y);
        GetNode<TextureRect>("Neck/BallTexture").Visible = false;
        LoadWithBall();
        loadedBall.Fire(GetFireVelocity().x, GetFireVelocity().y);

        EmitSignal("Fired");

        firingTimeoutSeconds = 1f;
    }

    public override void _Input(InputEvent @event)
	{                        
        if (!Visible || !settingStrength) {
            return;
        }

		if (!(@event is InputEventScreenDrag drag))
		{
            return;
		}

        if (drag.Position.x > maxTouchX) {
            GD.Print("MAX X REACHED");
            return;
        }
        if (drag.Position.y > maxTouchY) {
            GD.Print("MAX X REACHED");
            return;
        }

        RotateNeckToPoint(drag.Position);
	}

    private void RotateNeckToPoint(Vector2 touchPosition) {
        neck.LookAt(touchPosition);

        var realDegrees = neck.RotationDegrees - 180;
        neck.RotationDegrees = realDegrees;

        GD.Print("Current rotation: " + neck.RotationDegrees);

        var forceValue = (90 - neck.GlobalRotationDegrees) / 90f;

        neckDegreesLoading = neck.GlobalRotationDegrees;

        //EmitSignal("ProjectilePositionChanged", GetProjectileStartPosition());
        EmitSignal("FireVelocityChanged", GetFireVelocity());  
        EmitSignal("ForceChanged", forceValue);
    }

    private Vector2 GetFireVelocity()
    {
        var force = (90 - neckDegreesLoading) / 90f;
        return new Vector2(force * 600f, force * -300f);
    }

    public void NeckTouched()
    {
        GD.Print("Touched");
        settingStrength = true;
    }

    public void NeckReleased()
    {
        if (firingTimeoutSeconds != -1) {
            return;
        }
        GD.Print("released");
        settingStrength = false;

        releaseSound.Playing = true;

        firing = true;
        firingTime = 0;
        firingForce = (90 - neck.GlobalRotationDegrees) / 90f;
    }

    public override void SetForce(float value)
    {
        GD.Print("Set catapult neck rotation to " + value * 90 + " degrees");
        neck.RotationDegrees = maxDegreesRotation - (value * (90 - maxDegreesRotation));

        neckDegreesLoading = neck.GlobalRotationDegrees;

        //EmitSignal("ProjectilePositionChanged", GetProjectileStartPosition());
        EmitSignal("FireVelocityChanged", GetFireVelocity());
    }

    public override void SetTrajectory(float value)
    {
        // NOTHING TO DO, CATAPULT CAN'T CONTROL TRAJECTORY
    }
}
