using Godot;

public class GameObject : RigidBody2D
{
    public const string BASE_GROUP = "GAME_OBJECT";

    private Vector2 externalVelocity = new Vector2(-1, -1);

    public DamageController DamageController {set; private get;}

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

    public override void _IntegrateForces(Physics2DDirectBodyState physicsState) {
		if (this.DamageController == null) {
            return;
		}

        DamageController.CalculateDamage(physicsState);
    }
}