using Godot;
using System;

public class ExplosionSphere : Node2D
{
    private Area2D area;

    [Export]
    private float power = 20.0f;

    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
    }

    public void Explode()
    {
        var bodies = area.GetOverlappingBodies();

        GD.Print("[ExplosionSphere] exploding and affecting " + bodies.Count + " bodies");

        int count = 0;
        foreach (PhysicsBody2D body in bodies)
        {
            if (!body.IsInGroup(GameObject.BASE_GROUP))
            {
                GD.Print(body.GetParent().Name);
                continue;
            }

            count++;

            var toBody = area.Position.DirectionTo(body.Position);
            var distance = (1 / area.Position.DistanceTo(body.Position));

            var gameObject = body as GameObject;
            if (body.GetParent().IsInGroup(Enemy.GROUP_NAME))
            {
                (body.GetParent() as Enemy).Explode(power * distance);
            }
            gameObject.SetVelocity(-1 * toBody.x * power * 2000 * distance, -1 * toBody.y * power * 2000 * distance);

            GD.Print("Set velocity for " + gameObject.GetParent().Name + ": " + toBody.x * power *  2000 * distance);
        }

        GD.Print("OCOUT: " + count);
    }
}
