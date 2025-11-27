public class ChaseState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Chase; } }
    public ChaseState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.MeshAgent.isStopped = false;
    }
    public override void Update()
    {
        monster.MeshAgent.SetDestination(monster.Target.transform.position);

        if (monster.IsTargetInAttackRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Attack);
            return;
        }
        if (monster.Target == null || !monster.IsTargetInChaseRange())
        {
            monster.StateMachine.ChangeState(MonsterStateType.Idle);
        }
    }
    public override void Exit()
    {
        monster.MeshAgent.isStopped = true;
    }
}