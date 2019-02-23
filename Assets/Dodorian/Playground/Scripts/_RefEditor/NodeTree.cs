#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeTree
{
#pragma warning disable 0649

    [SerializeField]
    private List<Node> _nodes;

#pragma warning restore 0649

    public void Drag(string title, Vector2 delta)
    {
        _nodes.Find(n => n._Title == title).Drag(delta);
    }

    public void Draw()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].Draw();
        }
    }

    public void ProcessEvents(Event e, out bool guiChange)
    {
        guiChange = false;

        for (int i = _nodes.Count - 1; i >= 0; i--)
        {
            var change = _nodes[i].ProcessEvents(e);

            if (change)
            {
                guiChange = true;
            }
        }
    }

    public void AddNode(Node node)
    {
        _nodes.Add(node);
    }
}

#endif