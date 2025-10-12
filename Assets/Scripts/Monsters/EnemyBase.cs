using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    protected static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    protected static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    protected static readonly int DieHash = Animator.StringToHash("die");

    [SerializeField]
    private EnemyType enemyType;
    [SerializeField]
    protected int maxHealth = 100;
    protected int currentHealth;

    [SerializeField]
    protected Transform targetPoint;
    protected Rigidbody rb;
    protected NavMeshAgent navAgent;
    protected Animator animator;
    protected Material material;

    protected bool isChasing;
    protected bool isAttacking;
    protected Coroutine attackCo;
    protected Coroutine hitCo;

    protected IEnemyBehavior behavior;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        material = GetComponentInChildren<MeshRenderer>().material;
        GetBehavior();

        currentHealth = maxHealth;

    }

    protected virtual void Start()
    {
        StartCoroutine(StartChase());
    }

    protected virtual void Update()
    {
        if (navAgent && targetPoint)
        {
            navAgent.SetDestination(targetPoint.position);
            navAgent.isStopped = !isChasing;
        }

        Target();
    }
    protected virtual void Target()
    {
        if (behavior == null || isAttacking) return;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, behavior.Radius, transform.forward, behavior.Range, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            attackCo ??= StartCoroutine(AttackRoutine());
        }
    }

    protected IEnumerator AttackRoutine()
    {
        isChasing = false;
        isAttacking = true;
        navAgent.isStopped = true;
        animator.SetBool(IsAttackingHash, true);

        yield return behavior.Attack(this);

        animator.SetBool(IsAttackingHash, false);
        navAgent.isStopped = false;
        isAttacking = false;
        isChasing = true;
        attackCo = null;
    }

    protected IEnumerator StartChase()
    {
        yield return YieldCache.WaitForSeconds(2.0f);
        isChasing = true;
        animator.SetBool(IsWalkingHash, true);
    }

    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        Debug.Log($"{name} 체력 {damage} 감소 → 현재 체력 {currentHealth}");

        hitCo ??= StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die(hitPoint, isHitGrenade);
        }
    }

    private IEnumerator HitFlash()
    {
        material.color = Color.red;
        yield return YieldCache.WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            material.color = Color.white;
        }

        hitCo = null;
    }

    protected virtual void Die(Vector3 hitPoint, bool isHitGrenade = false)
    {
        isChasing = false;
        navAgent.enabled = false;
        material.color = Color.gray;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        animator.SetTrigger(DieHash);

        Vector3 hitDir = (transform.position - hitPoint).normalized + Vector3.up * (isHitGrenade ? 3f : 1f);
        rb.AddForce(hitDir * 5f, ForceMode.Impulse);
        if (isHitGrenade)
        {
            rb.AddTorque(hitDir * 15, ForceMode.Impulse);
        }

        Destroy(gameObject, 2f);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying || behavior == null) return;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * behavior.Range;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, behavior.Radius);
        Gizmos.DrawWireSphere(end, behavior.Radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }
    void GetBehavior()
    {
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
}