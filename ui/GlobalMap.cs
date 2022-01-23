using Godot;
using System;

public class GlobalMap : Node2D
{
    public override void _Ready()
    {
        var passedLevels = StorageManager.GetString(PropertyKeys.PASSED_LEVELS_LIST);
        if (passedLevels == null)
        {
            // init with default values
            passedLevels = "0";
            StorageManager.StoreValue(PropertyKeys.PASSED_LEVELS_LIST, passedLevels);
        }

        var possesedArray = Util.ToPossesionArray(passedLevels, Constants.LEVEL_COUNT);

        var levelNodes = GetNode<Node2D>("Levels").GetChildren();
        for (int i = 0; i < possesedArray.Length; i++)
        {
            UpdateLevelState(levelNodes[i] as Node2D, possesedArray[i]);
        }
    }

    private void UpdateLevelState(Node2D levelNode, bool isPassed)
    {

    }
}
