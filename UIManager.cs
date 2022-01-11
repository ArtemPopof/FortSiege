using Godot;
using System;
using System.Collections.Generic;



public class UIManager : Node2D
{
    private class MaybeLoadedDialog {
        public PackedScene scene;
        public Dialog instancedDialog;

        public MaybeLoadedDialog(PackedScene scene)
        {
            this.scene = scene;
        }

        public Dialog GetDialogOrInitFirst(Node2D parent)
        {
            if (instancedDialog == null)
            {
                instancedDialog = scene.Instance<Dialog>();
                instancedDialog.Init();
                parent.AddChild(instancedDialog);
            }

            return instancedDialog;
        }
    }

    public const int NOT_ENOUGH_MONEY_TO_BUY_DIALOG = 0;

    public static UIManager instance;

    private Dictionary<int, MaybeLoadedDialog> dialogScenes;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instance = this;
    }

    public static void Init()
    {
        GD.Print("UIManager: Init");

        instance.dialogScenes = new Dictionary<int, MaybeLoadedDialog>(5);

        var dialogDirectory = new Directory();
        dialogDirectory.Open("res://dialogs");
        dialogDirectory.ListDirBegin(true, true);
        
        var nextFile = dialogDirectory.GetNext();
        var dialogId = 0;
        while (nextFile != "")
        {
            GD.Print("Init " + nextFile + " dialog");
            var dialog = ResourceLoader.Load("res://dialogs/" + nextFile) as PackedScene;
            if (dialog == null)
            {
                nextFile = dialogDirectory.GetNext();
                continue;
            }
            instance.dialogScenes.Add(dialogId, new MaybeLoadedDialog(dialog));

            dialogId++;
            nextFile = dialogDirectory.GetNext();
        }

        GD.Print("UIManager: loaded " + instance.dialogScenes.Count + " dialogs");
    }

    public bool ShowDialog(int dialogId, float x = -1, float y = -1, params object[] args)
    {
        var dialog = dialogScenes[dialogId].GetDialogOrInitFirst(this);

        GD.Print("[UIManager] Show dialog " + dialog.Name);

        if (x == -1 || y == -1)
        {
            // center the dialog
            var screenX = (GetViewportRect().Size.x - dialog.size.x) / 2;
            var screenY = (GetViewportRect().Size.y - dialog.size.y) / 2;
            dialog.GlobalPosition = new Vector2(screenX, screenY);
        } else {
            dialog.GlobalPosition = new Vector2(x, y);
        }

        dialog.Show(args);

        return true;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
