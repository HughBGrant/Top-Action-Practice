using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int DieHash = Animator.StringToHash("die");
    public static readonly WaitForSeconds Wait01 = new WaitForSeconds(0.1f);
    public static readonly WaitForSeconds Wait02 = new WaitForSeconds(0.2f);
    public static readonly WaitForSeconds Wait05 = new WaitForSeconds(0.5f);
    public static readonly WaitForSeconds Wait10 = new WaitForSeconds(1.0f);
    public static readonly WaitForSeconds Wait20 = new WaitForSeconds(2.0f);
    private static int deadEnemyLayer;

    [SerializeField]
    private Collider hitBox;
    public Collider HitBox => hitBox;

    public Rigidbody Rigidbody { get; private set; }

    public GameObject BulletPrefab;

    [SerializeField]
    private EnemyType enemyType;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private Transform targetPoint;

    private bool isChasing;
    private bool isAttacking;

    private Material material;
    private NavMeshAgent navAgent;
    private Animator animator;

    private Coroutine attackCo;
    private Coroutine hitCo;

    private const float DeathDestroyDelay = 2f;
    private const float DeathReactionMultiplier = 5f;

    private IEnemyBehavior behavior;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        material = GetComponentInChildren<MeshRenderer>().material;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");

        switch (enemyType)
        {
            case EnemyType.A:
                behavior = new EnemyA_Behavior();
                break;
            case EnemyType.B:
                behavior = new EnemyB_Behavior();
                break;
            case EnemyType.C:
                behavior = new EnemyC_Behavior();
                break;
        }
    }
    private void Start()
    {
        currentHealth = maxHealth;

        StartCoroutine(StartChase());
    }
    private void Update()
    {
        Target();

        if (navAgent.enabled)
        {
            navAgent.SetDestination(targetPoint.position);
            navAgent.isStopped = !isChasing;
        }
    }
    public void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        Debug.Log($"체력 {damage} 감소. 현재 체력 {currentHealth}");

        hitCo ??= StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {

            Die(hitPoint, isHitGrenade);
        }

    }
    private IEnumerator HitFlash()
    {
        material.color = Color.red;
        yield return Wait01;

        if (currentHealth > 0)
        {
            material.color = Color.white;
        }

        hitCo = null;
    }
    private void Die(Vector3 hitPoint, bool isHitGrenade = false)
    {
        isChasing = false;
        navAgent.enabled = false;
        material.color = Color.gray;

        gameObject.layer = deadEnemyLayer;

        animator.SetTrigger(DieHash);

        float hitGrenadeReactionMultiplier = isHitGrenade ? 3f : 1f;
        Vector3 hitDirection = (transform.position - hitPoint).normalized;
        hitDirection += Vector3.up * hitGrenadeReactionMultiplier;

        Rigidbody.AddForce(hitDirection * DeathReactionMultiplier, ForceMode.Impulse);

        if (isHitGrenade)
        {
            Rigidbody.AddTorque(hitDirection * 15, ForceMode.Impulse);
        }

        Destroy(gameObject, DeathDestroyDelay);
    }
    private void Target()
    {
        if (behavior == null || isAttacking) return;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, behavior.Radius, transform.forward, behavior.Range, LayerMask.GetMask("Player"));

        if (hits.Length > 0)
        {
            attackCo ??= StartCoroutine(AttackRoutine());
        }
    }
    private IEnumerator StartChase()
    {
        yield return Wait20;

        isChasing = true;
        animator.SetBool(IsWalkingHash, true);
    }
    private IEnumerator AttackRoutine()
    {
        isChasing = false;
        isAttacking = true;
        animator.SetBool(IsAttackingHash, true);

        yield return behavior.Attack(this);

        animator.SetBool(IsAttackingHash, false);
        isAttacking = false;
        isChasing = true;
        attackCo = null;
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || enemyType == EnemyType.C) return;
        if (behavior == null) return;

        // 구체 위치
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * behavior.Range;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, behavior.Radius);
        Gizmos.DrawWireSphere(end, behavior.Radius);

        // 방향선
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }
}
