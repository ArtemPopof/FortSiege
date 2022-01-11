using Godot;
using System;

public class Background : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Clouds clouds;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        clouds = GetNode<Clouds>("Clouds");
    }

    public void Init()
    {
        clouds.Init();
    }

    public void Reset()
    {
        clouds.Reset();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
