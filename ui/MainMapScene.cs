using Godot;
using System;

public class MainMapScene : Node2D
{
    [Signal]
    public delegate void ChangeScreen(String screen);
    [Signal]
    public delegate void StartGame(int levelIndex);

    private MainMenuHeader header;
    private GlobalMap map;

    private int selectedLevelIndex;

    public override void _Ready()
    {
        header = GetNode<MainMenuHeader>("MainMenuHeader");
        header.Connect("ChangeScreen", this, "OnChangeScreen");
        header.Connect("ChangeWeapon", this, "ChangeWeapon");

        map = GetNode<GlobalMap>("LevelMap");

        map.Init();

        selectedLevelIndex = map.SelectedLevel;
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

    public void OnStartButtonPressed()
    {
        GD.Print("[MainMapScene] Start level " + selectedLevelIndex);
        EmitSignal("StartGame", selectedLevelIndex);
    }


}
