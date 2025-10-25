using UnityEngine;

public class IdleState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Idle; } }

    public IdleState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.GetComponent<Animator>().SetBool("IsWalking", false);
    }
    public override void Update()
    {
        float distance = Vector3.Distance(monster.transform.position, monster.TargetTransform.position);

        if (distance < 10f)
        {
            monster.StateMachine.ChangeState(MonsterStateType.Chase);
            return;
        }
    }
}