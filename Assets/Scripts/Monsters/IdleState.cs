public class IdleState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Idle; } }

    public IdleState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetBool("IsWalking", false);
    }
    public override void Update()
    {
        if (monster.IsTargetInChaseRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Chase);
            return;
        }
    }
    public override void Exit()
    {
        monster.Animator.SetBool("IsWalking", true);
    }
}