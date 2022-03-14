using Godot;
using System;

public class MainMapScene : Node2D
{
    [Signal]
    public delegate void ChangeScreen(int screen);
    [Signal]
    public delegate void StartGame(int levelIndex);

    private GlobalMap map;

    private int selectedLevelIndex;

    public override void _Ready()
    {
        map = GetNode<GlobalMap>("LevelMap");

        map.Init();

        selectedLevelIndex = map.SelectedLevel;

        map.Connect("LaunchSelectedLevel", this, "OnStartLevel");
    }

    public void OnChangeScreen(int screen)
    {
        EmitSignal("ChangeScreen", screen);
    }

    public void GoToWorkshop()
    {
        OnChangeScreen(Constants.WEAPON_SHOP_SCREEN);
    }

    public void OnStartLevel(int level)
    {
        GD.Print("[MainMapScene] Start level " + level);

        var music = GetNode<AudioStreamPlayer2D>("BackgroundMusic");
        music.Autoplay = false;
        music.Playing = false;

        EmitSignal("StartGame", level);
    }


}
