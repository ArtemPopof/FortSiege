using Godot;
using System;

public class TimerWidget : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void Timeout();

    private Label label;
    private bool running;

    private bool over;

    private float timeLeft;
    public int TimeLeftInt {private set; get;}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label = GetNode<Label>("Label");

        running = false;
        over = false;
        SetTime(0);
    }

    public void Stop() {
        GD.Print("Stop timer");
        running = false;
    }

    private void SetTime(int time) {
        label.Text = time.ToString() + "s";
    }

    public void Reset(int startTime) {
        GD.Print("Reset TimerWidget " + startTime);
        timeLeft = startTime;
        TimeLeftInt = startTime;
        SetTime(startTime);
        over = false;
        running = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!running) return;

        timeLeft -= delta;
        if (timeLeft <= 0) {
            SetTimeOver();
            return;
        }
        //GD.Print("Timer: " + timeLeft);

        if (TimeLeftInt != (((int)timeLeft))) {
            TimeLeftInt = (int) timeLeft;
            SetTime(TimeLeftInt);
        }
    }

    private void SetTimeOver() {
        running = false;
        over = true;
        SetTime(0);

        EmitSignal("Timeout");
    }
}
