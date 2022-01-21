using Godot;

public class GameObject : RigidBody2D
{
    public const string BASE_GROUP = "GAME_OBJECT";

    private Vector2 externalVelocity = new Vector2(-1, -1);

    public override void _Ready()
    {
        AddToGroup(BASE_GROUP);
    }

    public void SetVelocity(float x, float y)
    {
        this.externalVelocity = new Vector2(x, y);
        this.Sleeping = false;

        LinearVelocity = new Vector2(x, y);
    }

    // public override void _IntegrateForces(Physics2DDirectBodyState physicsState) {
	// 	if (this.externalVelocity.x == -1) {
    //         return;
	// 	}

	// 	physicsState.LinearVelocity = new Vector2(LinearVelocity.x + externalVelocity.x, LinearVelocity.y + externalVelocity.y);

    //     this.externalVelocity.x = -1;
	// }
}