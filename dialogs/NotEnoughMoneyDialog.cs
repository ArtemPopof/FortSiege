using Godot;
using System;

public class NotEnoughMoneyDialog : Dialog
{
    public override void _Ready()
    {
        GetNode<TouchScreenButton>("VBoxContainer/Body/VBoxContainer/MarginContainer/StartButton/TouchScreenButton").Connect("pressed", this, "Close");
    }

}
