using Godot;
using System;

public class Barel : RigidBody2D
{
    private DamageController damageController;
    private AudioStreamPlayer2D explosionSound;
    private ExplosionSphere explosionSphere;

    public override void _Ready()
    {
        damageController = new DamageController();
        damageController.Init(this).WithForceThreshold(85000);
        damageController.Connect("OnDestroyed", this, "OnDestroyed");
                
        explosionSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        explosionSound.Connect("finished", this, "DestructionFinished");

        explosionSphere = GetNode<ExplosionSphere>("ExplosiveSphere");
    }

    private void DestructionFinished()
    {
        //QueueFree();
    }

    private void OnDestroyed()
    {
        GD.Print("[Barel] Destroyed!");

        GetNode<TextureRect>("TextureRect").Visible = false;
        GetNode<Particles2D>("Explosion").Emitting = true;
        explosionSound.Play();

        GetNode<Node2D>("Fragments").Visible = true;

        explosionSphere.Explode();

        GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
    }

    public override void _IntegrateForces(Physics2DDirectBodyState physicsState)
    {
        damageController.CalculateDamage(physicsState);
    }
}
