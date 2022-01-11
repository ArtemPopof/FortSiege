using Godot;
using System;

public class CoinSpawner : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public PackedScene coinScene;
    [Export]
    public CoinCounter counter;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void SpawnCoins(int count, Vector2 position)
    {
        GD.Print("Spawn " + count + " coins at " + position);
        for (int i = 0; i < count; i++)
        {
            var randomVelX = GD.RandRange(-5, 5);
            var randomVelY = GD.RandRange(-5, 5);

            var newCoin = coinScene.Instance<Coin>();
            newCoin.LinearVelocity = new Vector2((float) randomVelX, (float) randomVelY);
            newCoin.GlobalPosition = position;

            AddChild(newCoin);

            if (counter != null) {
                newCoin.Connect("CollectCoin", this, "CoinCollected");
            }
        }
    }

    private void CoinCollected(int count) {
        counter.SetCount(counter.count + count);
    }


    public void ClearCoins()
    {
        GD.Print("Clean " + GetChildCount() + " Coins");
        
        foreach (Node2D child in GetChildren())
        {
            child.QueueFree();
        }
    }
}
