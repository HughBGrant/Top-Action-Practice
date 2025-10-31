using UnityEngine;

public class BossIdleState : MonsterState
{
    private float timer;

    public BossIdleState(MonsterBase monster) : base(monster) { }
    public override MonsterStateType StateType => MonsterStateType.Idle;

    public override void Enter()
    {
        timer = 1f;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
        }
    }
}
