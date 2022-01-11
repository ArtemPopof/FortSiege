using Godot;
using System;

public class menu : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal]
    public delegate void StartGame();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void ExitPressed()
    {
        GetTree().Quit();
    }

    public void OnStartGameButtonPressed()
    {
        EmitSignal("StartGame");
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
