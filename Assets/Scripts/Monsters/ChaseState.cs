using UnityEngine;

public class ChaseState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Chase; } }
    public ChaseState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetBool("IsAttacking", false);
        monster.Animator.SetBool("IsWalking", true);
        monster.MeshAgent.isStopped = false;
    }
    public override void Update()
    {
        monster.MeshAgent.SetDestination(monster.TargetTransform.position);

        float distance = Vector3.Distance(monster.transform.position, monster.TargetTransform.position);

        if (monster.TargetTransform == null || !monster.IsTargetInChaseRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }

        if (monster.IsTargetInAttackRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
        }
    }
    public override void Exit()
    {
        monster.MeshAgent.isStopped = true;
    }
}