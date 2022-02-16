using Godot;
using System;

public class LoadingScreen : Node2D
{
    [Export]
    private float fadingAnimationDuration = 10f;

    [Signal]
    private delegate void Complete();

    private ProgressBar progressBar;
    private Clouds clouds;

    private bool complete = false;
    private float animationLeft;

    public override void _Ready()
    {
        clouds = GetNode<Clouds>("Clouds");
        progressBar = GetNode<ProgressBar>("MarginContainer/ProgressBar");

        clouds.Init();
    }

    public void SetProgress(float value)
    {
        GD.Print("[LoadingScreen] Progress is " + value);
        progressBar.Value = value;

        if (value == 100)
        {
            GD.Print("[LoadingScreen] Loading complete");
            complete = true;
            progressBar.Visible = false;
            clouds.Visible = false;
            animationLeft = fadingAnimationDuration;
        }
    }

    public override void _Process(float delta)
    {
        if (!complete) return;

        animationLeft -= delta;
        if (animationLeft < 0)
        {
            animationLeft = 0;
        }
        this.Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, animationLeft / fadingAnimationDuration);

        if (animationLeft == 0)
        {
            EmitSignal("Complete");
            complete = false;
            QueueFree();
        }
    }
}
