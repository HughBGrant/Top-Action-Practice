using UnityEngine;

public class ChaseState : MonsterState
{
    public override MonsterStateType StateType => MonsterStateType.Chase;
    public ChaseState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetBool("IsWalking", true);
        monster.MeshAgent.isStopped = false;
    }
    public override void Update()
    {
        if (monster.TargetTransform == null)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }
        monster.MeshAgent.SetDestination(monster.TargetTransform.position);

        if (IsTargetInAttackRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
        }
    }
    public override void Exit()
    {
        monster.MeshAgent.isStopped = true;
        monster.Animator.SetBool("IsWalking", false);
    }
    private bool IsTargetInAttackRange()
    {
        if (monster.Behavior == null)
        {
            return false;
        }

        RaycastHit[] hits = Physics.SphereCastAll(
            monster.transform.position,
            monster.Behavior.AttackRadius,
            monster.transform.forward,
            monster.Behavior.AttackRange,
            LayerMask.GetMask("Player")
        );
        return hits.Length > 0;
    }
    //private bool IsTargetInAttackRange()
    //{
    //    float distance = Vector3.Distance(monster.transform.position, monster.TargetTransform.position);

    //    return distance < monster.Behavior.AttackRange;
    //}
}