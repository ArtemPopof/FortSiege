using Godot;
using System;

public class Header : Panel
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void BackActionFired();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<TouchScreenButton>("HBoxContainer/BackButton/TouchScreenButton").Connect("pressed", this, "BackClicked");
    }

    public void BackClicked()
    {
        GD.Print("[Header] clicked back button");
        EmitSignal("BackActionFired");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
