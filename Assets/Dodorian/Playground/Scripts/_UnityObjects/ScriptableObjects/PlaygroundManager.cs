using System;
using UnityEngine;
using Dodorian.Singleton;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CreateAssetMenu(fileName = "PlaygroundManager", menuName = "Playground/PlaygroundManager", order = 1)]
public sealed class PlaygroundManager : RSingletonSO<PlaygroundManager>
{
#if UNITY_EDITOR

    [HideInInspector]
    public List<NodeTree> _NodeTrees;

#endif

    [SerializeField, ReadOnly]
    private EntitySetupCollection _setupAssets;

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

        _entityComponentMethods.Add(typeof(EntityNode), new Dictionary<Methods, System.Reflection.MethodInfo>());
        _entityComponentMethods[typeof(EntityNode)].Add(Methods.Init, typeof(EntityNode).GetMethod("Init", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

        for (int i = 0; i < _setupAssets.Length; i++)
        {
            _setups.Add(_setupAssets[i].Item1, _setupAssets[i].Item2);
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

#if UNITY_EDITOR

    public void AddEntitySetup(TextAsset asset)
    {
        _setupAssets.AddEntitySetup(asset);
    }

    /// <summary>
    /// This method is called in the editor automaticaly if this method is in a class which derives from MonoBehaviour or a class which derives from InspectedSO
    /// </summary>
    private void OnInspect()
    {
        var assets = Directory.GetFiles(Application.dataPath + "/Dodorian/Playground/Editor/EntitySetups").Where(s => !s.Contains(".meta")).ToArray();

        _setupAssets.Clear();

        for (int i = 0; i < assets.Length; i++)
        {
            TextAsset ta = null;

            try
            {
                ta = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets" + assets[i].Split(new string[1] { "/Assets" }, StringSplitOptions.None)[1]);
            }
            catch
            {
            }

            if (ta != null)
            {
                _setupAssets.AddEntitySetup(ta);
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }

#endif
}