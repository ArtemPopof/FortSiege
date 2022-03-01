using Godot;
using System;

public class YesNoDialog : Dialog
{


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<TouchScreenButton>("VBoxContainer/Body/VBoxContainer/HBoxContainer/NoButton/TouchScreenButton").Connect("pressed", this, "FinishWithResult", new Godot.Collections.Array{false});
        GetNode<TouchScreenButton>("VBoxContainer/Body/VBoxContainer/HBoxContainer/OkButton/TouchScreenButton").Connect("pressed", this, "FinishWithResult", new Godot.Collections.Array{true});
    }

}
