using Godot;
using System;

public class GlobalMap : Node2D
{
    [Signal]
    public delegate void LevelSelected(int index);

    public int SelectedLevel {get; private set;} = 0;

    public void Init()
    {
        GD.Print("Open global map");

        var availableLevels = StorageManager.GetString(PropertyKeys.AVAILABLE_LEVELS);
        //availableLevels = null;
        if (availableLevels == null)
        {
            // init with default values
            availableLevels = "0";
            StorageManager.StoreValue(PropertyKeys.AVAILABLE_LEVELS, availableLevels);
            StorageManager.Save();
        }

        GD.Print("Levels: " + availableLevels);

        var possesedArray = Util.ToPossesionArray(availableLevels, Constants.LEVEL_COUNT);

        var levelNodes = GetNode<Node2D>("Levels").GetChildren();
        for (int i = 0; i < possesedArray.Length; i++)
        {
            GD.Print("Level " + i + " passed: " + possesedArray[i]);
            UpdateLevelState(levelNodes[i] as Node2D, possesedArray[i]);
        }
        
        SelectedLevel = GetLastLevel(possesedArray);
        MoveWeaponIconToLevel(levelNodes[SelectedLevel] as Node2D);
    }

    private void UpdateLevelState(Node2D levelNode, bool isPassed)
    {
        var label = (Label) levelNode.GetChildren()[0];
        var passedMark = (Panel) levelNode.GetChildren()[1];
        var enemyIcon = (TextureRect) levelNode.GetChildren()[2];

        if (isPassed)
        {
            label.AddColorOverride("font_color", new Color(1, 1, 1, 1));
            passedMark.Visible = true;
            enemyIcon.Visible = false;
        }
        else
        {
            label.AddColorOverride("font_color", new Color(1, 1, 1, 0.4f));
            passedMark.Visible = false;
            enemyIcon.Visible = true;
        }
    }

    public int GetLastLevel(bool[] levelPassArray)
    {
        int max = 0;
        for (int i = 0; i < levelPassArray.Length; i++)
        {
            if (!levelPassArray[i]) return max;

            max = i;
        }

        return max;
    }

    private void MoveWeaponIconToLevel(Node2D levelNode)
    {
        var weaponNode = GetNode<PropertyBasedActivator>("WeaponIcons");

        weaponNode.GlobalPosition = levelNode.GlobalPosition;
    }
}
