using System.Collections;
using UnityEngine;

public class BossMonster : MonsterBase
{
    protected static readonly int LaunchMissileHash = Animator.StringToHash("LaunchMissile");
    protected static readonly int ThrowRockHash = Animator.StringToHash("ThrowRock");
    protected static readonly int JumpAttackHash = Animator.StringToHash("JumpAttack");
    [SerializeField]
    protected Collider attackCollider;
    public Collider AttackCollider { get { return attackCollider; } }

    [SerializeField]
    private GameObject projectilePrefab;
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }
    [SerializeField]
    private GameObject rockPrefab;
    [SerializeField]
    private Transform launchPointA;
    [SerializeField]
    private Transform launchPointB;

    private Vector3 jumpTargetPosition;
    private BoxCollider mainCollider;

    private bool isTrackingTarget;
    private Coroutine thinkCo;

    protected override void RegisterStates()
    {
        stateMachine.AddState(new AttackState(this));
        stateMachine.AddState(new DeadState(this));

        stateMachine.ChangeState(MonsterStateType.Attack);
    }
    protected override void Awake()
    {
        base.Awake();
        mainCollider = GetComponent<BoxCollider>();
        meshAgent.isStopped = true;
        thinkCo = StartCoroutine(DecideNextAction());
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
    private IEnumerator DecideNextAction()
    {
        yield return YieldCache.WaitForSeconds(0.2f);

        while (!isDead)
        {
            yield return ExecuteRandomAction();
            yield return YieldCache.WaitForSeconds(0.2f);
        }
    }
    private IEnumerator ExecuteRandomAction()
    {
        float rand = Random.value;

        if (rand < 0.4f)
            yield return PerformMissileAttack();
        else if (rand < 0.8f)
            yield return PerformRockThrow();
        else
            yield return PerformJumpAttack();
    }
    private IEnumerator PerformMissileAttack()
    {
        animator.SetTrigger(LaunchMissileHash);
        yield return YieldCache.WaitForSeconds(0.2f);
        GuidedMissile missileA = Instantiate(projectilePrefab, launchPointA.position, launchPointA.rotation).GetComponent<GuidedMissile>();

        missileA.TargetTransform = TargetTransform;

        yield return YieldCache.WaitForSeconds(0.3f);
        GuidedMissile missileB = Instantiate(projectilePrefab, launchPointB.position, launchPointB.rotation).GetComponent<GuidedMissile>();

        missileB.TargetTransform = TargetTransform;

        yield return YieldCache.WaitForSeconds(2f);
    }
    private IEnumerator PerformRockThrow()
    {
        isTrackingTarget = false;
        animator.SetTrigger(ThrowRockHash);
        Instantiate(rockPrefab, transform.position, transform.rotation);
        yield return YieldCache.WaitForSeconds(3.0f);
        isTrackingTarget = true;
    }
    private IEnumerator PerformJumpAttack()
    {
        jumpTargetPosition = TargetTransform.position;
        meshAgent.SetDestination(jumpTargetPosition);

        isTrackingTarget = false;
        meshAgent.isStopped = false;
        mainCollider.enabled = false;
        animator.SetTrigger(JumpAttackHash);
        yield return YieldCache.WaitForSeconds(1.5f);

        attackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        attackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);

        mainCollider.enabled = true;
        meshAgent.isStopped = true;
        isTrackingTarget = true;
    }
}


//public class BossMonster : MonsterBase
//{
//    protected override void Start()
//    {
//        base.Start();
//        StartCoroutine(BossThinkLoop());
//    }

//    private IEnumerator BossThinkLoop()
//    {
//        yield return new WaitForSeconds(0.5f);

//        while (!isDead)
//        {
//            float rand = Random.value;
//            if (rand < 0.4f)
//            {
//                yield return PerformMissileAttack();
//            }
//            else if (rand < 0.8f)
//            {
//                yield return PerformRockThrow();
//            }
//            else
//            {
//                yield return PerformJumpAttack();
//            }

//            yield return new WaitForSeconds(1.0f);
//        }
//    }

//    private IEnumerator PerformMissileAttack() { yield return new WaitForSeconds(1f); }
//    private IEnumerator PerformRockThrow() { yield return new WaitForSeconds(1f); }
//    private IEnumerator PerformJumpAttack() { yield return new WaitForSeconds(1f); }
//}

//public class BossIdleState : MonsterState
//{
//    private float timer;

//    public BossIdleState(MonsterBase monster) : base(monster) { }
//    public override MonsterStateType StateType => MonsterStateType.Idle;

//    public override void Enter() => timer = 0f;

//    public override void Update()
//    {
//        timer += Time.deltaTime;
//        if (timer > 2f)
//        {
//            monster.stateMachine.ChangeState(MonsterStateType.Attack);
//        }
//    }
//}

//public class BossAttackState : MonsterState
//{
//    public BossAttackState(MonsterBase monster) : base(monster) { }
//    public override MonsterStateType StateType => MonsterStateType.Attack;

//    public override void Enter()
//    {
//        monster.StartCoroutine(AttackRoutine());
//    }

//    private IEnumerator AttackRoutine()
//    {
//        float rand = Random.value;

//        if (rand < 0.4f)
//            Debug.Log("Boss: 미사일 공격!");
//        else if (rand < 0.8f)
//            Debug.Log("Boss: 바위 던지기!");
//        else
//            Debug.Log("Boss: 점프 공격!");

//        yield return new WaitForSeconds(2f);
//        monster.stateMachine.ChangeState(MonsterStateType.Idle);
//    }
//}