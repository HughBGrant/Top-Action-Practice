using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine
{
    private Dictionary<MonsterStateType, MonsterState> states = new();
    private MonsterState currentState;
    public MonsterStateType CurrentType { get { return currentState?.StateType ?? 0; } }
    public void Update()
    {
        currentState?.Update();
    }
    public void AddState(MonsterState state)
    {
        states[state.StateType] = state;
    }
    public void ChangeState(MonsterStateType type)
    {
        Debug.Log($"Exit: {currentState}");
        currentState?.Exit();
        currentState = states[type];
        Debug.Log($"Enter: {currentState}");
        currentState.Enter();
    }
    public T GetState<T>() where T : MonsterState
    {
        foreach (MonsterState state in states.Values)
        {
            if (state is T matched)
            {
                return matched;
            }
        }
        return null;
    }
}