using Godot;
using System;

public class WeaponInfoPanel : Panel
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Panel startButton;
    private Label nameLabel;
    private Label costLabel;
    private bool needToBuy;
    private int cost;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        nameLabel = GetNode<Label>("VBoxContainer/WeaponName");
        costLabel = GetNode<Label>("VBoxContainer/MarginContainer/BuyPanel/VBoxContainer/HBoxContainer/MoneyCount");
        startButton = GetNode<Panel>("VBoxContainer/StartButtonContainer/StartButton");
    }

    public void SetInfo(WeaponInfo info)
    {
        GD.Print("SHOP: Set current weapon to " + info.name);

        nameLabel.Text = info.name;
        costLabel.Text = info.cost.ToString();

        GetNode<Panel>("VBoxContainer/MarginContainer/BuyPanel").Visible = !info.inPossesion;
        startButton.Visible = info.inPossesion;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
