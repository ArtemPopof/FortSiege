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

public class Enemy : Node2D
{
    [Signal]
    public delegate void EnemyDied(Enemy enemy);

    [Export]
    public int coinReward = 1;
    [Export]
    public PackedScene deadEnemy;

    public static String GROUP_NAME = "Enemy";
    
    private bool died = false;
    private bool exploded = false;
    private int deathSoundCount;

    private Skeleton2D skeleton;
    private GameObject body;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup(GROUP_NAME);

        deathSoundCount = GetNode<Node>("Sounds/DeathSounds").GetChildCount();
        body = GetNode<GameObject>("RidigBody");
        //skeleton = GetNode<Skeleton2D>("Skeleton2D");

        var damageController = new DamageController();
        damageController.Init(body);
        damageController.Connect("OnDestroyed", this, "OnDestroyed");
        body.DamageController = damageController;
    }

    public void OnDestroyed()
    {
        Die();
    }

    private void Die() {
        if (died) return;
        died = true;
        
        var soundIndex = ((int)GD.RandRange(0, deathSoundCount));
        var sound = GetNode<AudioStreamPlayer2D>("Sounds/DeathSounds/Death" + (soundIndex + 1));
        sound.Play();
        
        EmitSignal("EnemyDied", this);

        var deadBody = deadEnemy.Instance() as Node2D;
        if (exploded)
        {
            RemoveAllExplodable(deadBody);
        }
        var deadPosition = body.Position;
        deadPosition += new Vector2(0, 50);
        var deadRotation = body.GlobalRotation;
        body.QueueFree();
        GD.Print("deadPosition: " + deadPosition);
        AddChild(deadBody);
        deadBody.Position = deadPosition;
        deadBody.GlobalRotation = deadRotation;
    }

    private void RemoveAllExplodable(Node2D deadBody)
    {
        foreach (Node node in deadBody.GetChildren())
        {
            if (node.IsInGroup("Explodable"))
            {
                node.QueueFree();
            }
        }
    }

    public void Explode(float force)
    {
        if (force < 10)
        {
            GD.Print("Not enough force to explode");
            return;
        }

        exploded = true;
    }
}
