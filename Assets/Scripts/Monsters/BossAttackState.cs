using System.Collections;
using UnityEngine;

public class BossAttackState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Attack; } }
    private Coroutine attackCo;

    public BossAttackState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        attackCo = monster.StartCoroutine(AttackRoutine());
    }
    private IEnumerator AttackRoutine()
    {
        yield return ((BossMonster)monster).BossBehavior.ExecuteRandomAttack();

        monster.StateMachine.ChangeState(MonsterStateType.Idle);

        attackCo = null;
    }
    public override void Update()
    {
        if (monster.TargetTransform == null)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }
    }
    public override void Exit()
    {
        if (attackCo != null)
        {
            monster.StopCoroutine(attackCo);
            attackCo = null;
        }
    }
}