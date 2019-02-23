using UnityEngine;
using Dodorian.Singleton;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "PlaygroundManager", menuName = "Playground/PlaygroundManager", order = 1)]
public sealed class PlaygroundManager : RSingletonSO<PlaygroundManager>
{
#if UNITY_EDITOR

    [SerializeField]
    private TextAsset[] _setupAssets;

    public List<NodeTree> _NodeTrees;

#endif

    private Dictionary<string, string> _setups;
    private Dictionary<string, Type> _entityTypes;
    private Dictionary<Type, Dictionary<Methods, System.Reflection.MethodInfo>> _entityComponentMethods;

    public Type this[string type]
    {
        get
        {
            if (!_entityTypes.ContainsKey(type))
            {
                _entityTypes.Add(type, Type.GetType(type));
            }

            return _entityTypes[type];
        }
    }

    public override void OnInstantiated()
    {
        _setups = new Dictionary<string, string>();
        _entityTypes = new Dictionary<string, Type>();
        _entityComponentMethods = new Dictionary<Type, Dictionary<Methods, System.Reflection.MethodInfo>>();

        _entityComponentMethods.Add(typeof(EntityComponent), new Dictionary<Methods, System.Reflection.MethodInfo>());
        _entityComponentMethods[typeof(EntityComponent)].Add(Methods.Init, typeof(EntityComponent).GetMethod("Init", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

        var setupAssets = Resources.LoadAll<TextAsset>("PlaygroundSetups");

#if UNITY_EDITOR
        _setupAssets = setupAssets;
#endif

        for (int i = 0; i < setupAssets.Length; i++)
        {
            _setups.Add(setupAssets[i].name, setupAssets[i].text);
        }
    }

    public string GetSetup(string name)
    {
        return _setups[name];
    }

    public void SetUpEntityComponent(Type componentType)
    {
        if (_entityComponentMethods.ContainsKey(componentType))
        {
            return;
        }

        _entityComponentMethods.Add(componentType, new Dictionary<Methods, System.Reflection.MethodInfo>());

        foreach (Methods methodEnum in Enum.GetValues(typeof(Methods)))
        {
            if (methodEnum != 0)
            {
                var method = componentType.GetMethod(methodEnum.ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (method != null)
                {
                    _entityComponentMethods[componentType].Add(methodEnum, method);
                }
            }
        }
    }

    public System.Reflection.MethodInfo GetCachedComponentMethod(Type targetType, Methods targetMethod)
    {
        if (_entityComponentMethods[targetType].ContainsKey(targetMethod))
        {
            return _entityComponentMethods[targetType][targetMethod];
        }

        return null;
    }
}