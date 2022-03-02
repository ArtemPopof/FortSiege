using Godot;
using System;

public class main : Node2D
{
    public static MobileCamera camera; 
    
    private Node2D uiLayout;

    private Node2D currentScene;
    private int previousSceneIndex;
    private int currentSceneIndex;

    private long startLoadingTicks;

    ~main() {
        GD.Print("Enter destructor, clearing up...");
        StorageManager.Save();
    }

    main() {
        GD.Print("\n[Main] Init");
        var startTicks = DateTime.Now.Ticks;

        StorageManager.Init();
        //StorageManager.Clear();
        StorageManager.StoreValue(PropertyKeys.COIN_COUNT, 999);
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
        uiLayout = GetNode<Node2D>("UILayout");

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
        startLoadingTicks = DateTime.Now.Ticks;

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
        bool async = false;
        if (screen == Constants.WEAPON_SHOP_SCREEN)
        {
            sceneNode = OpenWeaponsShop();
        } else if (screen == Constants.GLOBAL_MAP_SCREEN)
        {
           sceneNode = OpenGlobalMapScreen();
        } else if (screen == Constants.MAIN_GAME_SCREEN)
        {
            var loadingController = StartLoading();
            loadingController.Connect("Complete", this, "AsyncSceneLoadingComplete");
            async = true;
            sceneNode = OpenGameScreen();
            sceneNode.Connect("ProgressChanged", loadingController, "SetProgress");
        }

        currentScene = sceneNode;

        AddChildBelowNode(uiLayout, sceneNode);
        sceneNode.GlobalPosition = new Vector2(0, 0);
        sceneNode.Visible = true;

        if (async)
        {
            return;
        }

        var time = (DateTime.Now.Ticks - startLoadingTicks) / (TimeSpan.TicksPerMillisecond);

        GD.Print($"\n[Main] New scene {currentScene.Name} ready in {time} ms \n\n\n");
    }

    private Node2D OpenWeaponsShop()
    {

        var scene = ResourceLoader.Load<PackedScene>("res://scenes/WeaponWorkshop.tscn").Instance<WeaponWorkshop>();
        var shopHeader = scene.GetNode<Header>("Header");  

        shopHeader.Connect("BackActionFired", this, "GoToPreviousScreen");
        //scene.Connect("ScreenDone", this, "StartGame");

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

    private LoadingScreen StartLoading()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://scenes/LoadingScreen.tscn").Instance<LoadingScreen>();

        AddChild(scene);

        return scene;
    }

    private void AsyncSceneLoadingComplete()
    {
        var time = (DateTime.Now.Ticks - startLoadingTicks) / (TimeSpan.TicksPerMillisecond);

        GD.Print($"\n[Main] New scene {currentScene.Name} ready in {time} ms \n\n\n");
    }

    private Node2D OpenGameScreen()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://scenes/game.tscn");

        return scene.Instance<Node2D>();
    }

    private void StartGame()
    {
        ChangeScreen(Constants.MAIN_GAME_SCREEN);
    }

    private void StartGame(int levelIndex)
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_LEVEL, levelIndex);
        ChangeScreen(Constants.MAIN_GAME_SCREEN);
    }
}
