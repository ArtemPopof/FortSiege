using Godot;

using System.Text.RegularExpressions;
using System.Collections.Generic;

public abstract class Dialog : Node2D
{
    private Dictionary<int, Label> labelsWithPlaceholder;
    public Vector2 size;

    public bool Show(params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            labelsWithPlaceholder[i].Text = labelsWithPlaceholder[i].Text.Replace("$" + i, args[i].ToString());
        }

        Visible = true;

        return true;
    }

    public void Close()
    {
        Visible = false;
    }

    public void Init()
    {
       GD.Print("Init dialog " + Name);
       
       var labels = FindAllLabelChildren(this, new List<Label>());
       labelsWithPlaceholder = new Dictionary<int, Label>(labels.Count);

       foreach (Label label in labels)
       {
            Regex regex = new Regex(@"\$\d");
            MatchCollection matches = regex.Matches(label.Text);
            foreach (Match match in matches)
            {
                var index = int.Parse(match.Value.Substring(1));
                labelsWithPlaceholder[index] = label;
            }
       }

       var container = (Util.getFirstNodeOfGroup("Container", this) as VBoxContainer);
       size = container.RectSize * Scale;

       GD.Print("dialog container scale: " + Scale);

       Visible = false;
    }

    private List<Label> FindAllLabelChildren(Node node, List<Label> labels)
    {
        if (node.IsInGroup("Placeholder")) {
            labels.Add(node as Label);
            return labels;
        }

        var children = node.GetChildren();
        if (children.Count == 0) return labels;
        
        foreach (Node child in children)
        {
            FindAllLabelChildren(child, labels);
        }

        return labels;
    }
}