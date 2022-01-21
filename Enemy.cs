using Godot;
using System;

public class EnemyType {
    public static EnemyType KNIGHT = new EnemyType(1);
    public static EnemyType REACH_KNIGHT = new EnemyType(2);

    public int coinReward;

    public EnemyType(int coinReward) {
        this.coinReward = coinReward;
    }
}

public class Enemy : RigidBody2D
{
    [Signal]
    public delegate void EnemyDied(Enemy enemy);

    [Export]
    public int coinReward = 1;

    public static String GROUP_NAME = "Enemy";
    
    private bool died = false;
    private int deathSoundCount;

    private DamageController damageController;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        damageController = new DamageController();
        deathSoundCount = GetNode<Node>("Sounds/DeathSounds").GetChildCount();

        damageController.Init(this);
        damageController.Connect("OnDestroyed", this, "OnDestroyed");
    }

    public void OnDestroyed()
    {
        Die();
    }

 public override void _IntegrateForces(Physics2DDirectBodyState physicsState)
 {
     damageController.CalculateDamage(physicsState);
 }

 private void Die() {
     if (died) return;
     died = true;
     
     var soundIndex = ((int)GD.RandRange(0, deathSoundCount));
     var sound = GetNode<AudioStreamPlayer2D>("Sounds/DeathSounds/Death" + (soundIndex + 1));
     sound.Play();
     
     EmitSignal("EnemyDied", this);
 }

}
