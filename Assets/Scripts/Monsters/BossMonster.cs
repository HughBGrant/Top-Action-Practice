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
