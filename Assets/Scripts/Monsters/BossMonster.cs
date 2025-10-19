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
    protected GameObject bulletPrefab;
    public GameObject BulletPrefab => bulletPrefab;

    [SerializeField]
    Transform launchPointA;
    [SerializeField]
    Transform launchPointB;

    public Player player;
    private Vector3 lookVec;
    private Vector3 groundSlamVec;

    private bool isLooking;
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
        yield return YieldCache.WaitForSeconds(2.5f);
        StartCoroutine(Think());
    }
    private IEnumerator RollRock()
    {
        animator.SetTrigger(RollRockHash);
        yield return YieldCache.WaitForSeconds(3.0f);
        StartCoroutine(Think());
    }
    private IEnumerator JumpAttack()
    {
        animator.SetTrigger(JumpAttackHash);
        yield return YieldCache.WaitForSeconds(3.0f);
        StartCoroutine(Think());
    }
}
