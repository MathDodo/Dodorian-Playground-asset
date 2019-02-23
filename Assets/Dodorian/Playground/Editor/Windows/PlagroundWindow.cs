using System;
using UnityEditor;
using UnityEngine;

public class PlagroundWindow : EditorWindow
{
    private int _chosenTree = 0;
    private GUIStyle _nodeStyle;
    private PlaygroundManager _playground;
    private static PlagroundWindow _instance;

    [MenuItem("Window/Playground")]
    private static void OpenWindow()
    {
        _instance = GetWindow<PlagroundWindow>();
        _instance.titleContent = new GUIContent("Playground Editor");
    }

    private void OnEnable()
    {
        _playground = Resources.LoadAll<PlaygroundManager>("Singletons")[0];
        _nodeStyle = new GUIStyle();
        _nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        _nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    private void OnGUI()
    {
        DrawNodes();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void ProcessNodeEvents(Event e)
    {
        if (_playground._NodeTrees.Count > _chosenTree)
        {
            _playground._NodeTrees[_chosenTree].ProcessEvents(e, out bool changed);
            GUI.changed = changed;

            if (changed) EditorUtility.SetDirty(_playground);
        }
    }

    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (_playground._NodeTrees.Count > _chosenTree)
        {
            _playground._NodeTrees[_chosenTree].AddNode(new Node(mousePosition, 200, 50, _nodeStyle));

            EditorUtility.SetDirty(_playground);
        }
    }

    private void DrawNodes()
    {
        if (_playground._NodeTrees.Count > _chosenTree)
        {
            _playground._NodeTrees[_chosenTree].Draw();
        }
    }
}