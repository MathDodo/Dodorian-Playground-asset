using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntitySetupCollection
{
#if UNITY_EDITOR

    [SerializeField]
    private TextAsset[] _entitySetupAssets;

#endif

    [SerializeField]
    private string[] _entityNames;

    [SerializeField]
    private string[] _entitySetups;

    public int Length { get { return _entitySetups.Length; } }

    public Tuple<string, string> this[int index]
    {
        get { return new Tuple<string, string>(_entityNames[index], _entitySetups[index]); }
    }

#if UNITY_EDITOR

    public void AddEntitySetup(TextAsset asset)
    {
        var newSize = _entitySetups.Length + 1;

        Array.Resize(ref _entitySetupAssets, newSize);
        Array.Resize(ref _entityNames, newSize);
        Array.Resize(ref _entitySetups, newSize);

        _entitySetupAssets[newSize - 1] = asset;
        _entityNames[newSize - 1] = asset.name;
        _entitySetups[newSize - 1] = asset.text;
    }

    public void Clear()
    {
        _entitySetupAssets = new TextAsset[0];
        _entityNames = new string[0];
        _entitySetups = new string[0];
    }

#endif
}