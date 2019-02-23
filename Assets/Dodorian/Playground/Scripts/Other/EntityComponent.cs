using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent
{
    protected Entity _entity;

    private void Init(Entity entity)
    {
        _entity = entity;
    }
}

public class TC : EntityComponent
{
    [SerializeField]
    private string test = "Wakanda";

    private void Init()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Init in tc got entity: " + _entity);
#endif
    }
}

public class TC2 : EntityComponent
{
    [SerializeField]
    private float test = 1.11f;

    private void Update(float deltaTime)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Updating TC2: " + deltaTime);
#endif

        if (Input.GetKeyDown(KeyCode.Q))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Creating copy");
#endif
            UnityEngine.Object.Instantiate(_entity);
        }
    }
}