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


//public class BossBehavior
//{
//    private BossMonster boss;

//    public BossBehavior(BossMonster boss)
//    {
//        this.boss = boss;
//    }

//    public IEnumerator ExecuteRandomAttack()
//    {
//        float rand = Random.value;
//        if (rand < 0.4f)
//        {
//            yield return MissileAttack();
//        }
//        else if (rand < 0.8f)
//        {
//            yield return RockThrow();
//        }
//        else
//        {
//            yield return JumpAttack();
//        }
//    }

//    private IEnumerator MissileAttack() { /* 기존 미사일 코드 */ yield break; }
//    private IEnumerator RockThrow() { /* 기존 바위 코드 */ yield break; }
//    private IEnumerator JumpAttack() { /* 기존 점프 코드 */ yield break; }
//}


//public class BossAttackState : MonsterState
//{
//    private Coroutine attackCo;
//    private BossBehavior behavior;

//    public BossAttackState(BossMonster boss) : base(boss)
//    {
//        behavior = new BossBehavior(boss);
//    }

//    public override MonsterStateType StateType => MonsterStateType.Attack;

//    public override void Enter()
//    {
//        attackCo = monster.StartCoroutine(behavior.ExecuteRandomAttack());
//    }

//    public override void Exit()
//    {
//        if (attackCo != null)
//        {
//            monster.StopCoroutine(attackCo);
//            attackCo = null;
//        }
//    }
//}


//public class BossBehavior : MonsterBehavior
//{
//    private BossMonster boss;

//    public BossBehavior(BossMonster boss) : base(MonsterType.Boss)
//    {
//        this.boss = boss;
//    }

//    public IEnumerator ExecuteRandomAttack()
//    {
//        float rand = Random.value;

//        if (rand < 0.4f)
//        {
//            yield return PerformMissileAttack();
//        }
//        else if (rand < 0.8f)
//        {
//            yield return PerformRockThrow();
//        }
//        else
//        {
//            yield return PerformJumpAttack();
//        }
//    }

//    private IEnumerator PerformMissileAttack()
//    {
//        boss.Animator.SetTrigger("LaunchMissile");
//        yield return new WaitForSeconds(0.2f);

//        var missileA = Object.Instantiate(boss.ProjectilePrefab, boss.transform.position + Vector3.up * 2f, boss.transform.rotation)
//            .GetComponent<GuidedMissile>();
//        missileA.targetTransform = boss.TargetTransform;

//        yield return new WaitForSeconds(0.3f);
//        var missileB = Object.Instantiate(boss.ProjectilePrefab, boss.transform.position + Vector3.up * 2f, boss.transform.rotation)
//            .GetComponent<GuidedMissile>();
//        missileB.targetTransform = boss.TargetTransform;

//        yield return new WaitForSeconds(2f);
//    }

//    private IEnumerator PerformRockThrow()
//    {
//        boss.Animator.SetTrigger("ThrowRock");
//        Object.Instantiate(boss.RockPrefab, boss.transform.position + Vector3.up, boss.transform.rotation);
//        yield return new WaitForSeconds(3.0f);
//    }

//    private IEnumerator PerformJumpAttack()
//    {
//        boss.MeshAgent.isStopped = false;
//        boss.Animator.SetTrigger("JumpAttack");

//        yield return new WaitForSeconds(1.5f);
//        boss.AttackCollider.enabled = true;
//        yield return new WaitForSeconds(0.5f);
//        boss.AttackCollider.enabled = false;
//        boss.MeshAgent.isStopped = true;

//        yield return new WaitForSeconds(1.0f);
//    }
//}


//public class BossMonster : MonsterBase
//{
//    private BossBehavior bossBehavior;

//    protected override void Awake()
//    {
//        base.Awake();
//        bossBehavior = new BossBehavior(this);
//    }

//    protected override void Start()
//    {
//        stateMachine.AddState(new IdleState(this));
//        stateMachine.AddState(new ChaseState(this));
//        stateMachine.AddState(new AttackState(this));
//        stateMachine.AddState(new DeadState(this, Vector3.zero));

//        stateMachine.ChangeState(MonsterStateType.Idle);
//    }

//    public BossBehavior BossBehavior => bossBehavior;
//}


//private IEnumerator BossAttackRoutine()
//{
//    isAttacking = true;

//    yield return (monster as BossMonster).BossBehavior.ExecuteRandomAttack();

//    isAttacking = false;
//    monster.ChangeState(MonsterStateType.Idle);
//}
