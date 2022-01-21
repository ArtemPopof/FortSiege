using Godot;
using System;

public class Ball : RigidBody2D
{
	private bool Fixed = false;
	private Vector2 FixPosition;

	private float t;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Fire(float xVelocity, float yVelocity) {
		GD.Print("FIREEE! with velocity: " + xVelocity + ", "+ yVelocity);
		this.LinearVelocity = new Vector2(xVelocity, yVelocity);
		this.GravityScale = 1;
		this.Fixed = false;
		t = 0;
	}
	
 	public void Reset(Vector2 position) {
		 this.GravityScale = 0;
		 this.LinearVelocity = new Vector2(0, 0);
		 this.Position = position;
		 this.Fixed = true;
		 this.FixPosition = position;
	 }

	public override void _IntegrateForces(Physics2DDirectBodyState physicsState) {
		if (Fixed) {
			physicsState.Transform = new Transform2D(0, FixPosition);
		}

		Fixed = false;
	}


 public override void _Process(float delta)
 {

 }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

		t+= delta;

		//  1.716666 sec to y
		//  ~ 160 y change and 450 x change
		// from  485.0667 to  439.6025 in 1 sec and  397.735 in 2 sec
		// a = - 0.18 * startVelo
		
		if (1f - t < 0.01 || 2f - t < 0.01) {
		//if (t != 0) {
			//GD.Print("y: " + GlobalPosition.y);
			GD.Print("T: " + t);
			GD.Print("VELOX: " + LinearVelocity.x);
		}
    }
}
