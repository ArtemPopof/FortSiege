using Godot;
using System;
using System.Collections.Generic;

public class ShotCounter : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void NoMoreShots();

    private Stack<TextureRect> shotTextures;

    private int maxShots;
    private int shotsLeft;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<TextureRect>("referenceShotTexture").Visible = false;
        shotTextures = new Stack<TextureRect>();
    }

    public void Init(WeaponInfo weaponInfo) {
        maxShots = weaponInfo.shotCount;
        
        GD.Print("Init shot counter " + weaponInfo.shotCount);
        foreach (var texture in shotTextures)
        {
            texture.Visible = false;
            texture.QueueFree();
        }

        this.maxShots = weaponInfo.shotCount;
        this.shotsLeft = weaponInfo.shotCount;

        var referenceTexture = GetNode<TextureRect>("referenceShotTexture");
        referenceTexture.Texture = weaponInfo.projectileTexture;

        for (int i = 0; i < maxShots; i++) {
            var newTexture = referenceTexture.Duplicate() as TextureRect;
            newTexture.RectPosition = new Vector2(i * (referenceTexture.RectSize.x + 20), 0);
            newTexture.Visible = true;
            AddChild(newTexture);

            shotTextures.Push(newTexture);
        }
    }

    public void ShotPerformed() {
        GD.Print("Delete texture from shot counter, shots available: " + shotsLeft);

        if (shotsLeft < 0) return;
        
        GD.Print("Deleting");
        shotsLeft -= 1;
        var shotTexture = shotTextures.Pop();
        GD.Print("Delete " + shotTexture);
        shotTexture.Visible = false;
        shotTexture.QueueFree();

        if (shotsLeft == 0) OnNoMoreShots();
    }

    private void OnNoMoreShots() {
        GD.Print("Emit no more shots signal");
        EmitSignal("NoMoreShots");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
