using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    protected static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    [SerializeField]
    private MonsterType type;
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected Transform targetTransform;

    protected int currentHealth;

    public Rigidbody rigid;
    protected NavMeshAgent meshAgent;
    protected Animator animator;
    protected MeshRenderer[] meshes;

    protected bool isChasing;
    protected bool isAttacking;
    protected bool isDead;

    protected Coroutine attackCo;
    protected Coroutine hitCo;

    protected MonsterBehavior behavior;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        meshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        behavior = new MonsterBehavior(type);

        currentHealth = maxHealth;
    }
    protected virtual void Start()
    {
        if (type != MonsterType.Boss)
        {
            StartCoroutine(BeginChase());
        }
    }
    protected virtual void Update()
    {
        if (meshAgent && targetTransform && type != MonsterType.Boss)
        {
            meshAgent.SetDestination(targetTransform.position);
            meshAgent.isStopped = !isChasing;
        }

        DetectTarget();
    }
    protected virtual void DetectTarget()
    {
        if (behavior == null || isAttacking || type == MonsterType.Boss || isDead) return;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, behavior.AttackRadius, transform.forward, behavior.AttackRange, LayerMask.GetMask("Player"));

        if (hits.Length > 0)
        {
            attackCo ??= StartCoroutine(PerformAttack());
        }
    }
    protected IEnumerator PerformAttack()
    {
        isChasing = false;
        isAttacking = true;
        meshAgent.isStopped = true;
        animator.SetBool(IsAttackingHash, true);

        yield return behavior.ExecuteAttack(this);

        animator.SetBool(IsAttackingHash, false);
        meshAgent.isStopped = false;
        isAttacking = false;
        isChasing = true;
        attackCo = null;
    }
    protected IEnumerator BeginChase()
    {
        yield return YieldCache.WaitForSeconds(2.0f);
        isChasing = true;
        animator.SetBool(IsWalkingHash, true);
    }
    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        Debug.Log($"{name} 체력 {damage} 감소 → 현재 체력 {currentHealth}");

        hitCo ??= StartCoroutine(FlashOnHit());

        if (currentHealth <= 0)
        {
            HandleDeath(hitPoint, isHitGrenade);
        }
    }
    private IEnumerator FlashOnHit()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }
        yield return YieldCache.WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        hitCo = null;
    }
    protected virtual void HandleDeath(Vector3 hitPoint, bool isHitGrenade = false)
    {
        isDead = true;
        isChasing = false;
        meshAgent.enabled = false;
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.gray;
        }
        gameObject.layer = LayerMask.NameToLayer("DeadMonster");
        animator.SetTrigger(DieHash);

        Vector3 hitDir = (transform.position - hitPoint).normalized + Vector3.up * (isHitGrenade ? 3f : 1f);
        rigid.AddForce(hitDir * 5f, ForceMode.Impulse);

        if (isHitGrenade)
        {
            rigid.AddTorque(hitDir * 15, ForceMode.Impulse);
        }

        if (type != MonsterType.Boss)
        {
            Destroy(gameObject, 2f);
        }
    }
    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying || behavior == null) return;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * behavior.AttackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, behavior.AttackRadius);
        Gizmos.DrawWireSphere(end, behavior.AttackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }
}