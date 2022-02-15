using Godot;
using System;
using System.Collections.Generic;

public class Level : Node2D
{
    [Export]
    public String name = "Default Level";

    [Signal]
    public delegate void LevelEnemyDied(Enemy type);
    [Signal]
    public delegate void AllEnemiesAreDead();
    [Signal]
    public delegate void AllPrefabsAreStill();
    [Signal]
    public delegate void EnemyDead(Enemy enemy);
    [Signal]
    public delegate void CoinCollected(int count);
    [Signal]
    public delegate void ChestDestroyed(int coinCount, Vector2 position);
    
    private int enemiesLeft;
    public int activePrefabs;

    private bool allPrefabsStill = false;

    private float deltaToShot = 0;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Prefab> prefabs = new List<Prefab>();
    private List<Coin> coins = new List<Coin>();
    private List<Chest> chests = new List<Chest>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Init level " + name);

        foreach (Node child in GetChildren())
        {
            if (child.IsInGroup(Enemy.GROUP_NAME)) {
                enemies.Add(child as Enemy);

                child.Connect("EnemyDied", this, "EnemyDied");
            }
            if (child.IsInGroup(Prefab.GROUP_NAME)) {
                prefabs.Add(child as Prefab);
            }
            if (child.IsInGroup(Coin.GROUP_NAME)) {
                coins.Add(child as Coin);
                (child as Coin).Connect("CollectCoin", this, "CollectCoin");
            }
            if (child.IsInGroup(Chest.GROUP_NAME)) {
                chests.Add(child.GetChild(0) as Chest);
                (child.GetChild(0) as Chest).Connect("ChestDestroyed", this, "OnChestDestroyed");
            }
        }

        GD.Print("Enemies count : " + enemies.Count);
        GD.Print("Prefab count: " + prefabs.Count);
        GD.Print("Coin count: " + coins.Count);
        GD.Print("Chest count: " + chests.Count);

        enemiesLeft = enemies.Count;
    }

    private void OnChestDestroyed(int coinCount, Vector2 position)
    {
        GD.Print("\n Chest destroyed, spawn coins " + coinCount + "\n");
        EmitSignal("ChestDestroyed", coinCount, position);
    }

    public void CollectCoin(int count) 
    {
        EmitSignal("CoinCollected", count);
    }

    public void EnemyDied(Enemy enemy) {
        GD.Print("[Level]: Enemy died " + enemy);

        enemiesLeft -= 1;

        EmitSignal("LevelEnemyDied", enemy);

        if (enemiesLeft == 0) {
            GD.Print("[" + name + "] EMIT ALL ENEMIES DIED SIGNAL");
            EmitSignal("AllEnemiesAreDead");
        }
    }

    public List<Enemy> getEnemies() {
        return enemies;
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    var newActivePrefabs = 0;
     foreach (var prefab in prefabs)
     {
         if (prefab.moving) {
             newActivePrefabs++;
         }
     }

     activePrefabs = newActivePrefabs;

     deltaToShot += delta;

     if (deltaToShot > 1) {
        //GD.Print("Prefabs moving: " + activePrefabs);
        deltaToShot = 0;
     }

 }
 
}
