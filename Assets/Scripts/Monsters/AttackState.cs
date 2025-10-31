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

        monster.Animator.SetBool("IsAttacking", false);
        monster.MeshAgent.isStopped = false;
    }
    private IEnumerator AttackRoutine()
    {
        monster.MeshAgent.isStopped = true;
        monster.Animator.SetBool("IsAttacking", true);

        yield return monster.Behavior.ExecuteAttack(monster);

        monster.StateMachine.ChangeState(ShouldReturnToChase() ? MonsterStateType.Idle : MonsterStateType.Attack);

        attackCo = null;
    }
    private bool ShouldReturnToChase()
    {
        float distance = Vector3.Distance(monster.transform.position, monster.TargetTransform.position);

        return distance > monster.Behavior.AttackRange;
    }
}