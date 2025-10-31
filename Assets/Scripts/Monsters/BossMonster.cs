using UnityEngine;

public class BossMonster : MonsterBase
{
    [SerializeField]
    protected Collider attackCollider;

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private GameObject rockPrefab;
    [SerializeField]
    private Transform launchPointA;
    [SerializeField]
    private Transform launchPointB;

    private BoxCollider mainCollider;

    private bool isTrackingTarget = true;
    public Collider AttackCollider { get { return attackCollider; } }
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }
    public GameObject RockPrefab { get { return rockPrefab; } }
    public Transform LaunchPointA { get { return launchPointA; } }
    public Transform LaunchPointB { get { return launchPointB; } }
    public BoxCollider MainCollider { get { return mainCollider; } }
    public bool IsTrackingTarget { get { return isTrackingTarget; } set { isTrackingTarget = value; } }


    protected override void Awake()
    {
        base.Awake();

        behavior = new BossMonsterBehavior(type);
        mainCollider = GetComponent<BoxCollider>();

        meshAgent.isStopped = true;
    }
    protected override void RegisterStates()
    {
        stateMachine.AddState(new BossIdleState(this));
        stateMachine.AddState(new BossAttackState(this));
        //stateMachine.AddState(new BossDeadState(this));

        stateMachine.ChangeState(MonsterStateType.Idle);
    }
    protected override void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (isTrackingTarget)
        {
            transform.LookAt(TargetTransform.position);
        }
    }
}
