using UnityEngine;
using System.Collections.Generic;

public class InputNode : EntityNode
{
    [SerializeField]
    private List<InputCommand> _inputCommands;

    private void Init()
    {
        _entity.AddNode(new TestNode());
    }

    private void Update(float deltaTime)
    {
        for (int i = 0; i < _inputCommands.Count; i++)
        {
            if (_inputCommands[i]._ContinuousInput)
            {
                if (Input.GetKey(_inputCommands[i]._ActivationKey))
                {
                    _entity.transform.Translate(new Vector3(_inputCommands[i]._CommandName == "Right" ? 10 * deltaTime : -10 * deltaTime, 0, 0));
                }
            }
            else
            {
                if (Input.GetKeyDown(_inputCommands[i]._ActivationKey))
                {
                    _entity.transform.Translate(new Vector3(0, 10 * deltaTime, 0));
                }
            }
        }
    }

    private void OnNodeAdd(EntityNode node)
    {
        Debug.Log(node.GetType());
    }
}

public class TestNode : EntityNode { }