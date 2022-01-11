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
}