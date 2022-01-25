using Godot;
using System;

public class MainMapScene : Node2D
{
    [Signal]
    public delegate void ChangeScreen(String screen);

    private MainMenuHeader header;
    private GlobalMap map;

    public override void _Ready()
    {
        header = GetNode<MainMenuHeader>("MainMenuHeader");
        header.Connect("ChangeScreen", this, "OnChangeScreen");
        header.Connect("ChangeWeapon", this, "ChangeWeapon");

        map = GetNode<GlobalMap>("LevelMap");
    }

    public void OnChangeScreen(String screen)
    {
        EmitSignal("ChangeScreen", screen);
    }

    public void ChangeWeapon(int index)
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_WEAPON, index);
        StorageManager.Save();

        map.Update();
    }


}
