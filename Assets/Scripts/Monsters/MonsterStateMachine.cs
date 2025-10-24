using System.Collections.Generic;

public class MonsterStateMachine
{
    private Dictionary<MonsterStateType, MonsterState> states = new();
    private MonsterState currentState;
    public MonsterStateType CurrentType { get { return currentState?.StateType ?? 0; } }

    public void AddState(MonsterState state)
    {
        states[state.StateType] = state;
    }
    public void ChangeState(MonsterStateType type)
    {
        if (currentState != null && currentState.StateType == type) { return; }

        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }
    public void Update()
    {
        currentState?.Update();
    }
}