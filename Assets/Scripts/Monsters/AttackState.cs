using UnityEngine;

public class AttackState : MonsterState
{
    private float timer;

    public override MonsterStateType StateType { get { return MonsterStateType.Attack; } }
    public AttackState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetBool("IsAttacking", true);
        timer = 0f;
    }
    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2f)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Chase);
        }
    }
    public override void Exit()
    {
        monster.Animator.SetBool("IsAttacking", false);
    }

    //protected IEnumerator PerformAttack()
    //{
    //    isChasing = false;
    //    isAttacking = true;
    //    meshAgent.isStopped = true;
    //    animator.SetBool(IsAttackingHash, true);

    //    yield return behavior.ExecuteAttack(this);

    //    animator.SetBool(IsAttackingHash, false);
    //    meshAgent.isStopped = false;
    //    isAttacking = false;
    //    isChasing = true;
    //    attackCo = null;
    //}
}
//using System.Collections;
//using UnityEngine;

//public class AttackState : MonsterState
//{
//    private bool isAttacking;

//    public override MonsterStateType StateType => MonsterStateType.Attack;

//    public AttackState(MonsterBase monster) : base(monster) { }

//    public override void Enter()
//    {
//        monster.StartCoroutine(AttackRoutine());
//    }

//    private IEnumerator AttackRoutine()
//    {
//        isAttacking = true;
//        monster.Animator.SetBool("IsAttacking", true);

//        yield return monster.Behavior.ExecuteAttack(monster);

//        monster.Animator.SetBool("IsAttacking", false);
//        isAttacking = false;

//        monster.ChangeState(MonsterStateType.Chase);
//    }
//}