using System.Collections;
using UnityEngine;

public class BossMonster : MonsterBase
{
    protected static readonly int LaunchMissileHash = Animator.StringToHash("launchMissile");
    protected static readonly int RollRockHash = Animator.StringToHash("rollRock");
    protected static readonly int JumpAttackHash = Animator.StringToHash("jumpAttack");
    [SerializeField]
    protected Collider hitBox;
    public Collider HitBox => hitBox;

    [SerializeField]
    private GameObject missilePrefab;
    public GameObject MissilePrefab => missilePrefab;
    [SerializeField]
    private GameObject rockPrefab;

    [SerializeField]
    Transform launchPointA;
    [SerializeField]
    Transform launchPointB;

    public Player player;
    private Vector3 lookVec;
    private Vector3 groundSlamVec;

    public bool isLooking;
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Think());
    }
    protected override void Update()
    {
        if (isLooking)
        {
            lookVec = player.moveDirection * 5f;
            transform.LookAt(target.position + lookVec);
            //lookVec = target.position;
            //transform.LookAt(target.position);
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
        BossMissile missileA = Instantiate(missilePrefab, launchPointA.position, launchPointA.rotation).GetComponent<BossMissile>();

        missileA.target = target;

        yield return YieldCache.WaitForSeconds(0.3f);
        BossMissile missileB = Instantiate(missilePrefab, launchPointB.position, launchPointB.rotation).GetComponent<BossMissile>();

        missileB.target = target;

        yield return YieldCache.WaitForSeconds(2f);
        StartCoroutine(Think());
    }
    private IEnumerator RollRock()
    {
        isLooking = false;
        animator.SetTrigger(RollRockHash);
        Instantiate(rockPrefab, transform.position, transform.rotation);
        yield return YieldCache.WaitForSeconds(3.0f);
        isLooking = false;
        StartCoroutine(Think());
    }
    private IEnumerator JumpAttack()
    {
        animator.SetTrigger(JumpAttackHash);
        yield return YieldCache.WaitForSeconds(3.0f);
        StartCoroutine(Think());
    }
}
