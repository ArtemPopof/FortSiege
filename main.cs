using Godot;
using System;

public class main : Node2D
{
    public static MobileCamera camera; 

    private Node2D currentScene;
    private int previousSceneIndex;
    private int currentSceneIndex;

    ~main() {
        GD.Print("Enter destructor, clearing up...");
        StorageManager.Save();
    }

    main() {
        GD.Print("\n[Main] Init");
        var startTicks = DateTime.Now.Ticks;

        StorageManager.Init();
        Data.Init();

        previousSceneIndex = -1;
        currentSceneIndex = -1;

        var time = (DateTime.Now.Ticks - startTicks) / (TimeSpan.TicksPerMillisecond);

        GD.Print($"\n[Main] Initialised in {time} ms \n\n\n");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("\n[Main] Ready");

        var startTicks = DateTime.Now.Ticks;

        // erase all cache
        //StorageManager.Clear();

        UIManager.Init();
        ObjectManager.Init();

        camera = GetNode<MobileCamera>("MobileCamera");

        var time = (DateTime.Now.Ticks - startTicks) / (TimeSpan.TicksPerMillisecond);

        GD.Print($"\n[Main] Main menu scene ready in {time} ms \n\n\n");

        ChangeScreen(Constants.GLOBAL_MAP_SCREEN);
    }

    public void GoToPreviousScreen()
    {
        if (previousSceneIndex == -1)
        {
            GD.PrintErr("NO PREVIOUS SCREEN, CAN'T CHANGE");
            return;
        }

        ChangeScreen(previousSceneIndex);
    }

    public void ChangeScreen(int screen)
    {
        var startTicks = DateTime.Now.Ticks;

        previousSceneIndex = currentSceneIndex;
        currentSceneIndex = screen;

        if (currentScene != null)
        {
            currentScene.Visible = false;
            RemoveChild(currentScene);
            currentScene.QueueFree();
            currentScene = null;
            GC.Collect();
        }

        Node2D sceneNode = null;

        if (screen == Constants.WEAPON_SHOP_SCREEN)
        {
            sceneNode = OpenWeaponsShop();
        } else if (screen == Constants.GLOBAL_MAP_SCREEN)
        {
           sceneNode = OpenGlobalMapScreen();
        } else if (screen == Constants.MAIN_GAME_SCREEN)
        {
            sceneNode = OpenGameScreen();
        }

        AddChild(sceneNode);
        sceneNode.GlobalPosition = new Vector2(0, 0);
        sceneNode.Visible = true;
        currentScene = sceneNode;

        var time = (DateTime.Now.Ticks - startTicks) / (TimeSpan.TicksPerMillisecond);

        GD.Print($"\n[Main] New scene {screen} ready in {time} ms \n\n\n");
    }

    private Node2D OpenWeaponsShop()
    {

        var scene = ResourceLoader.Load<PackedScene>("res://WeaponShop.tscn").Instance<WeaponShop>();
        var shopHeader = scene.GetNode<Header>("VBoxContainer/Header");  

        shopHeader.Connect("BackActionFired", this, "GoToPreviousScreen");
        scene.Connect("ScreenDone", this, "StartGame");

        return scene;
    }

    private Node2D OpenGlobalMapScreen()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://ui/Main.tscn").Instance() as Node2D;

        var mainMenu = (scene as MainMapScene);
        mainMenu.Connect("ChangeScreen", this, "ChangeScreen");
        mainMenu.Connect("StartGame", this, "StartGame");

        return scene;
    }

    private Node2D OpenGameScreen()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://scenes/game.tscn");

        return scene.Instance<Node2D>();
    }

    private void StartGame(int levelIndex)
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_LEVEL, levelIndex);
        ChangeScreen(Constants.MAIN_GAME_SCREEN);
    }
}
