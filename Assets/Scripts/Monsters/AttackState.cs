using System.Collections;
using UnityEngine;

public class AttackState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Attack; } }
    private Coroutine attackCo;

    public AttackState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        attackCo = monster.StartCoroutine(AttackRoutine());
    }
    private IEnumerator AttackRoutine()
    {
        if (monster is not BossMonster)
        {
            monster.Animator.SetBool("IsAttacking", true);
        }
        if (monster.Behavior is IAttackBehavior attackBehavior)
        {
            yield return attackBehavior.ExecuteAttack();
        }
        if (monster is BossMonster)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
        }
        else
        {
            monster.StateMachine.ChangeState(MonsterStateType.Chase);
        }
        attackCo = null;
    }
    public override void Exit()
    {
        if (attackCo != null)
        {
            monster.StopCoroutine(attackCo);
            attackCo = null;
        }
        if (monster is not BossMonster)
        {
            monster.Animator.SetBool("IsAttacking", false);
        }
    }
}