using Godot;

public class Util
{
    public static Node getFirstNodeOfGroup(string group, Node parentNode)
    {
        var children = parentNode.GetChildren();
        foreach (Node child in children)
        {
            if (child.IsInGroup(group))
            {
                return child;
            }
        }

        return null;
    }

    public static bool[] ToPossesionArray(string dataString, int entityCount)
    {
        var strings = dataString.Split(';');

        var array = new bool[entityCount];
        
        for (var i = 0; i < strings.Length; i++)
        {
            array[int.Parse(strings[i])] = true;
        }

        return array;
    }
}