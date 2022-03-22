using Godot;

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public abstract class Dialog : Node2D
{
    private Dictionary<int, Label> labelsWithPlaceholder;
    private String[] initialText;

    public Vector2 size;

    private Action<bool> resultCallback;

    public bool Show(params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            labelsWithPlaceholder[i].Text = labelsWithPlaceholder[i].Text.Replace("$" + i, args[i].ToString());
        }

        Visible = true;

        return true;
    }

    public void ShowForResult(Action<bool> callback, params object[] args)
    {
        Show(args);

        resultCallback = callback;
    }

    public void FinishWithResult(bool result)
    {
        Close();

        if (resultCallback != null)
        {
            resultCallback.Invoke(result);
            resultCallback = null;
        }
    }

    public void Close()
    {
        Visible = false;
    }

    public void Reset()
    {
        var labels = FindAllLabelChildren(this, new List<Label>());

        for (var i = 0; i < labels.Count; i++)
        {
            labels[i].Text = initialText[i];
        }
    }

    public void Init()
    {
       GD.Print("Init dialog " + Name);
       
       var labels = FindAllLabelChildren(this, new List<Label>());

       initialText = new string[labels.Count];
       labelsWithPlaceholder = new Dictionary<int, Label>(labels.Count);

       for (int i = 0; i < labels.Count; i++)
       {
            var label = labels[i];
            initialText[i] = label.Text;
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