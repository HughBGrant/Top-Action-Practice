using UnityEngine;

public class ChaseState : MonsterState
{
    public override MonsterStateType StateType => MonsterStateType.Chase;
    public ChaseState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetBool("IsWalking", true);

        //monster.MeshAgent.isStopped = false;
    }
    public override void Update()
    {
        if (monster.IsDead) { return; }

        if (monster.TargetTransform == null)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }
        monster.MeshAgent.SetDestination(monster.TargetTransform.position);

        float distance = Vector3.Distance(monster.transform.position, monster.TargetTransform.position);
        if (distance < monster.Behavior.AttackRange)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
        }
    }
    public override void Exit()
    {
        monster.Animator.SetBool("IsWalking", false);

        //monster.MeshAgent.isStopped = true;
    }

    //protected IEnumerator BeginChase()
    //{
    //    yield return YieldCache.WaitForSeconds(2.0f);
    //    isChasing = true;
    //    animator.SetBool(IsWalkingHash, true);
    //}

    //protected virtual void DetectTarget()
    //{
    //    if (behavior == null || isAttacking || type == MonsterType.Boss || isDead) return;

    //    RaycastHit[] hits = Physics.SphereCastAll(transform.position, behavior.AttackRadius, transform.forward, behavior.AttackRange, LayerMask.GetMask("Player"));

    //    if (hits.Length > 0)
    //    {
    //        attackCo ??= StartCoroutine(PerformAttack());
    //    }
    //}
}