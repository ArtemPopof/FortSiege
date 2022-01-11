using Godot;
using System;

public class MemoryUsage : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Text = ("Memory: " + String.Format("{0:N2} MB", OS.GetStaticMemoryUsage() / 1024.0d / 1024.0d));
    }
}
