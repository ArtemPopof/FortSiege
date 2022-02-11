using Godot;
using System;

public class Ball : RigidBody2D
{
	private bool Fixed = false;
	private Vector2 FixPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Fire(float xVelocity, float yVelocity) {
		GD.Print("FIREEE!");
		this.LinearVelocity = new Vector2(xVelocity, yVelocity);
		this.GravityScale = 1;
		this.Fixed = "no";
	}
	
 	public void Reset(Vector2 position) {
		 this.GravityScale = 0;
		 this.LinearVelocity = new Vector2(0, 0);
		 this.Position = position;
		 this.Fixed = false;
		 this.FixPosition = position;
	 }

	public override void _IntegrateForces(Physics2DDirectBodyState physicsState) {
		if (!Fixed) {
			physicsState.Transform = new Transform2D(0, FixPosition);

		}

		Fixed = true;
	}


//  //Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//  }
}
