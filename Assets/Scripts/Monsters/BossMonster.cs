using UnityEngine;

public class BossMonster : MonsterBase
{
    [SerializeField]
    private GameObject rockPrefab;
    [SerializeField]
    private Transform launchPointA;
    [SerializeField]
    private Transform launchPointB;

    private BoxCollider mainCollider;

    private bool isTrackingTarget = true;

    private IAttackBehavior bossBehavior;
    public GameObject RockPrefab { get { return rockPrefab; } }
    public Transform LaunchPointA { get { return launchPointA; } }
    public Transform LaunchPointB { get { return launchPointB; } }
    public BoxCollider MainCollider { get { return mainCollider; } }
    public bool IsTrackingTarget { get { return isTrackingTarget; } set { isTrackingTarget = value; } }
    public override IAttackBehavior Behavior { get { return bossBehavior; } set { bossBehavior = value; } }

    protected override void Awake()
    {
        base.Awake();

        bossBehavior = new BossMonsterBehavior(this);
        mainCollider = GetComponent<BoxCollider>();

        meshAgent.isStopped = true;
    }
    protected override void RegisterStates()
    {
        stateMachine.AddState(new AttackState(this));
        stateMachine.AddState(new DeadState(this));

        stateMachine.ChangeState(MonsterStateType.Attack);
    }
    protected override void Update()
    {
        StateMachine.Update();

        if (isTrackingTarget)
        {
            transform.LookAt(TargetTransform.position);
        }
    }
}
