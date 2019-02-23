#if UNITY_EDITOR

using System;
using UnityEngine;

[Serializable]
public class Node
{
    public Rect _Rect;
    public string _Title;
    public GUIStyle _Style;

    [HideInInspector]
    public bool _IsDragged;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle)
    {
        _Rect = new Rect(position, new Vector2(width, height));
        _Style = nodeStyle;
    }

    public void Drag(Vector2 delta)
    {
        _Rect.position += delta;
    }

    public void Draw()
    {
        GUI.Box(_Rect, _Title, _Style);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (_Rect.Contains(e.mousePosition))
                    {
                        _IsDragged = true;
                        GUI.changed = true;
                    }
                    else
                    {
                        GUI.changed = true;
                    }
                }
                break;

            case EventType.MouseUp:
                _IsDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && _IsDragged)
                {
                    Drag(e.delta);
                    e.Use();

                    return true;
                }
                break;
        }
        return false;
    }
}

#endif