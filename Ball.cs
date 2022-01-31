using Godot;
using System;

public class Ball : RigidBody2D
{
	private bool Fixed = false;
	private bool Hit = false;
	private Vector2 FixPosition;

	private AudioStreamPlayer2D hitSound;

	private float t;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hitSound = GetNode<AudioStreamPlayer2D>("HitSound");

		ContactMonitor = true;
		ContactsReported = 1;
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
		 this.Hit = false;
		 this.FixPosition = position;
	 }

	public override void _IntegrateForces(Physics2DDirectBodyState physicsState) {
		if (Fixed) {
			physicsState.Transform = new Transform2D(0, FixPosition);
		}

		Fixed = false;

		if (Hit || Fixed)
		{
			return;
		}

		// check collision with prefab
		if (physicsState.GetContactCount() == 0) return;

		GD.Print("[Ball] Hit first time");
		hitSound.Playing = true;
		Hit = true;
	}

}
