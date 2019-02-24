using System;
using UnityEngine;
using System.Collections.Generic;

public sealed class Entity : MonoBehaviour
{
    private Action<float> _updates;
    private Action<float> _fixedUpdates;
    private List<EntityNode> _components;
    private Action<EntityNode> _onNodeAdd;
    private Action<EntityNode> _onNodeRemove;

    [ReadOnly]
    public string _Name;

    [NonSerialized]
    public Transform _Transform;

    [NonSerialized]
    public GameObject _GameObject;

    private void Awake()
    {
        _Transform = transform;
        _GameObject = gameObject;

        _components = new List<EntityNode>();

        var manager = PlaygroundManager.Instance;

        ReadComponentData(manager);

        InvokeBaseInit(manager);

        AssignMethodCalls(manager);

        InvokeInit(manager);
    }

    private void ReadComponentData(PlaygroundManager manager)
    {
        var data = manager.GetSetup(_Name).Split('|');
        Type componentType = manager[data[0]];

        for (int i = 1; i < data.Length; i++)
        {
            if (i % 2 == 0)
            {
                componentType = manager[data[i]];
            }
            else
            {
                _components.Add((EntityNode)JsonUtility.FromJson(data[i], componentType));
            }
        }
    }

    private void InvokeBaseInit(PlaygroundManager manager)
    {
        var baseInit = manager.GetCachedComponentMethod(typeof(EntityNode), Methods.Init);

        for (int i = 0; i < _components.Count; i++)
        {
            manager.SetUpEntityComponent(_components[i].GetType());

            baseInit.Invoke(_components[i], new object[1] { this });
        }
    }

    private void InvokeInit(PlaygroundManager manager)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            var init = manager.GetCachedComponentMethod(_components[i].GetType(), Methods.Init);

            if (init != null)
            {
                init.Invoke(_components[i], null);
            }
        }
    }

    private void AssignMethodCalls(PlaygroundManager manager)
    {
        var methodsList = new List<Methods>();

        foreach (Methods methodEnum in Enum.GetValues(typeof(Methods)))
        {
            methodsList.Add(methodEnum);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            for (int t = 0; t < methodsList.Count; t++)
            {
                switch (methodsList[t])
                {
                    case Methods.FixedUpdate:
                        var fixedUpdate = manager.GetCachedComponentMethod(_components[i].GetType(), Methods.FixedUpdate);

                        if (fixedUpdate != null)
                        {
                            _fixedUpdates += (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), _components[i], fixedUpdate);
                        }
                        break;

                    case Methods.Update:
                        var update = manager.GetCachedComponentMethod(_components[i].GetType(), Methods.Update);

                        if (update != null)
                        {
                            _updates += (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), _components[i], update);
                        }
                        break;

                    case Methods.OnNodeAdd:
                        var add = manager.GetCachedComponentMethod(_components[i].GetType(), Methods.OnNodeAdd);

                        if (add != null)
                        {
                            _onNodeAdd += (Action<EntityNode>)Delegate.CreateDelegate(typeof(Action<EntityNode>), _components[i], add);
                        }
                        break;

                    case Methods.OnNodeRemove:
                        var remove = manager.GetCachedComponentMethod(_components[i].GetType(), Methods.OnNodeRemove);

                        if (remove != null)
                        {
                            _onNodeRemove += (Action<EntityNode>)Delegate.CreateDelegate(typeof(Action<EntityNode>), _components[i], remove);
                        }

                        break;
                }
            }
        }
    }

    private void Update()
    {
        _updates?.Invoke(Time.deltaTime);
    }

    public void AddNode(EntityNode node)
    {
        _onNodeAdd?.Invoke(node);
    }
}