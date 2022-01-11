using Godot;
using System;

public class Coin : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public const string GROUP_NAME = "Coint_GROUP";

    private int nominal = 1;

    [Signal]
    public delegate void CollectCoin(int count);

    private AudioStreamPlayer2D collectedSoundPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collectedSoundPlayer = GetNode<AudioStreamPlayer2D>("CollectSound");
        collectedSoundPlayer.Connect("finished", this, "CoinJobFinished");

        AddToGroup(GROUP_NAME);
    }

    private void CoinJobFinished()
    {
        QueueFree();
    }

    public void OnClicked()
    {
        if (!Visible) {
            return;
        }

        EmitSignal("CollectCoin", nominal * 20);
        collectedSoundPlayer.Play();
        Visible = false;

        GD.Print("Collected coin");
    }
}
