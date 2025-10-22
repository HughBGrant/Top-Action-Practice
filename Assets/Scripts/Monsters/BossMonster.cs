using System.Collections;
using UnityEngine;

public class BossMonster : MonsterBase
{
    protected static readonly int LaunchMissileHash = Animator.StringToHash("launchMissile");
    protected static readonly int RollRockHash = Animator.StringToHash("rollRock");
    protected static readonly int JumpAttackHash = Animator.StringToHash("jumpAttack");
    [SerializeField]
    protected Collider hitBox;
    public Collider HitBox { get { return hitBox; } }

    [SerializeField]
    private GameObject missilePrefab;
    public GameObject MissilePrefab { get { return missilePrefab; } }
    [SerializeField]
    private GameObject rockPrefab;
    [SerializeField]
    Transform launchPointA;
    [SerializeField]
    Transform launchPointB;

    public Player player;
    private Vector3 lookVec;
    private Vector3 jumpAttackVec;
    private BoxCollider boxCollider;

    public bool isLooking;
    protected override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider>();
        navAgent.isStopped = true;
        StartCoroutine(Think());
    }
    protected override void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (isLooking)
        {
            lookVec = player.moveDirection * 5f;
            transform.LookAt(targetTransform.position + lookVec);
            //lookVec = targetTransform.position;
            //transform.LookAt(targetTransform.position);
        }
        else
        {
            navAgent.SetDestination(jumpAttackVec);
        }
    }
    private IEnumerator Think()
    {
        yield return YieldCache.WaitForSeconds(0.1f);

        int sample = Random.Range(0, 5);

        switch (sample)
        {
            case 0:
            case 1:
                StartCoroutine(LaunchMissile());
                break;
            case 2:
            case 3:
                StartCoroutine(RollRock());
                break;
            case 4:
                StartCoroutine(JumpAttack());
                break;
        }
    }
    private IEnumerator LaunchMissile()
    {
        animator.SetTrigger(LaunchMissileHash);
        yield return YieldCache.WaitForSeconds(0.2f);
        GuidedMissile missileA = Instantiate(missilePrefab, launchPointA.position, launchPointA.rotation).GetComponent<GuidedMissile>();

        missileA.targetTransform = targetTransform;

        yield return YieldCache.WaitForSeconds(0.3f);
        GuidedMissile missileB = Instantiate(missilePrefab, launchPointB.position, launchPointB.rotation).GetComponent<GuidedMissile>();

        missileB.targetTransform = targetTransform;

        yield return YieldCache.WaitForSeconds(2f);
        StartCoroutine(Think());
    }
    private IEnumerator RollRock()
    {
        isLooking = false;
        animator.SetTrigger(RollRockHash);
        Instantiate(rockPrefab, transform.position, transform.rotation);
        yield return YieldCache.WaitForSeconds(3.0f);
        isLooking = true;

        StartCoroutine(Think());
    }
    private IEnumerator JumpAttack()
    {
        jumpAttackVec = targetTransform.position + lookVec;

        isLooking = false;
        navAgent.isStopped = false;
        boxCollider.enabled = false;
        animator.SetTrigger(JumpAttackHash);
        yield return YieldCache.WaitForSeconds(1.5f);

        hitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        hitBox.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);

        boxCollider.enabled = true;
        navAgent.isStopped = true;
        isLooking = true;

        StartCoroutine(Think());
    }
}
