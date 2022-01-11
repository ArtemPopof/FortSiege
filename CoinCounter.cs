using Godot;
using System;

public class CoinCounter : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Label counterText;

    public int count;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        counterText = GetNode<Label>("Label");
    }

    public void SetCount(int count)
    {
        counterText.Text = count.ToString();
        this.count = count;

        //StorageManager.StoreValue(PropertyKeys.COIN_COUNT, count);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
