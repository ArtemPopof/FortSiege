using Godot;
using System;

public class Coin : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public const string GROUP_NAME = "Coint_GROUP";

    private int nominal = 1;

    [Export]
    public float collectAnimationDuration = 1f;

    [Signal]
    public delegate void CollectCoin(int count);

    private bool collecting;
    private Vector2 collectingStartPos;
    private Vector2 collectingFinishDiff;
    private float animationTime;


    private AudioStreamPlayer2D collectedSoundPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collectedSoundPlayer = GetNode<AudioStreamPlayer2D>("CollectSound");
        collectedSoundPlayer.Connect("finished", this, "CoinJobFinished");

        AddToGroup(GROUP_NAME);
    }

    public void AutoCollect(Vector2 animationFinishPosition)
    {
        GD.Print("Animation end point: " + animationFinishPosition);
        GD.Print("Current pos: " + GlobalPosition);
        collectingStartPos = GlobalPosition;
        animationTime = 0;
        collectingFinishDiff = animationFinishPosition - GlobalPosition;
        collecting = true;
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
        Visible = false;

        GD.Print("Collected coin");
    }

    public override void _Process(float delta)
    {
        if (!collecting || !Visible) return;

        animationTime += delta;
        if (animationTime > collectAnimationDuration) animationTime = collectAnimationDuration;

        GlobalPosition = collectingStartPos + (animationTime / collectAnimationDuration) * collectingFinishDiff;

        if (animationTime == collectAnimationDuration)
        {
            EmitSignal("CollectCoin", nominal * 20);
            collectedSoundPlayer.Play();
            collecting = false;
            Visible = false;
        }
    }
}
