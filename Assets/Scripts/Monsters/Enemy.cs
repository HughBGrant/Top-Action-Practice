//using System.Collections;
//using UnityEngine;
//using UnityEngine.AI;

//public class Enemy : MonoBehaviour, IDamageable
//{
//    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
//    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
//    private static readonly int DieHash = Animator.StringToHash("die");
//    private static int deadEnemyLayer;

//    [SerializeField]
//    private Collider hitBox;
//    public Collider HitBox => hitBox;

//    public Rigidbody Rigidbody { get; private set; }

//    public GameObject MissilePrefab;

//    [SerializeField]
//    private EnemyType enemyType;
//    [SerializeField]
//    private int maxHealth;
//    [SerializeField]
//    private int currentHealth;
//    [SerializeField]
//    private Transform target;

//    private bool isChasing;
//    private bool isAttacking;

//    private Material material;
//    private NavMeshAgent navAgent;
//    private Animator animator;

//    private Coroutine attackCo;
//    private Coroutine hitCo;


//    private IMonsterBehavior behavior;

//    private void Awake()
//    {
//        Rigidbody = GetComponent<Rigidbody>();
//        navAgent = GetComponent<NavMeshAgent>();
//        animator = GetComponentInChildren<Animator>();
//        material = GetComponentInChildren<MeshRenderer>().material;
//        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");

//        switch (enemyType)
//        {
//            case EnemyType.A:
//                behavior = new MonsterBehaviorA();
//                break;
//            case EnemyType.B:
//                behavior = new MonsterBehaviorB();
//                break;
//            case EnemyType.C:
//                behavior = new MonsterBehaviorC();
//                break;
//        }
//    }
//    private void Start()
//    {
//        currentHealth = maxHealth;

//        StartCoroutine(StartChase());
//    }
//    private void Update()
//    {
//        DetectTarget();

//        if (navAgent.enabled)
//        {
//            navAgent.SetDestination(target.position);
//            navAgent.isStopped = !isChasing;
//        }
//    }
//    public void HitFlash(int damage, Vector3 hitPoint, bool isHitGrenade = false)
//    {
//        currentHealth -= damage;
//        Debug.Log($"체력 {damage} 감소. 현재 체력 {currentHealth}");

//        hitCo ??= StartCoroutine(HitFlash());

//        if (currentHealth <= 0)
//        {
//            Die(hitPoint, isHitGrenade);
//        }

//    }
//    private IEnumerator HitFlash()
//    {
//        material.color = Color.red;
//        yield return YieldCache.WaitForSeconds(0.1f);

//        if (currentHealth > 0)
//        {
//            material.color = Color.white;
//        }

//        hitCo = null;
//    }
//    private void Die(Vector3 hitPoint, bool isHitGrenade = false)
//    {
//        isChasing = false;
//        navAgent.enabled = false;
//        material.color = Color.gray;

//        gameObject.layer = deadEnemyLayer;

//        animator.SetTrigger(DieHash);

//        float hitGrenadeReactionMultiplier = isHitGrenade ? 3f : 1f;
//        Vector3 hitDirection = (transform.position - hitPoint).normalized + Vector3.up * hitGrenadeReactionMultiplier;
//        float DeathReactionMultiplier = 5f;

//        Rigidbody.AddForce(hitDirection * DeathReactionMultiplier, ForceMode.Impulse);

//        if (isHitGrenade)
//        {
//            float torqueMultiplier = 15f;
//            Rigidbody.AddTorque(hitDirection * torqueMultiplier, ForceMode.Impulse);
//        }

//        Destroy(gameObject, 2f);
//    }
//    private void DetectTarget()
//    {
//        if (behavior == null || isAttacking) return;

//        RaycastHit[] hits = Physics.SphereCastAll(transform.position, behavior.Radius, transform.forward, behavior.Range, LayerMask.GetMask("Player"));

//        if (hits.Length > 0)
//        {
//            attackCo ??= StartCoroutine(AttackRoutine());
//        }
//    }
//    private IEnumerator StartChase()
//    {
//        yield return YieldCache.WaitForSeconds(2.0f);

//        isChasing = true;
//        animator.SetBool(IsWalkingHash, true);
//    }
//    private IEnumerator AttackRoutine()
//    {
//        isChasing = false;
//        isAttacking = true;
//        animator.SetBool(IsAttackingHash, true);

//        //yield return behavior.Attack(this);
//        yield return null;

//        animator.SetBool(IsAttackingHash, false);
//        isAttacking = false;
//        isChasing = true;
//        attackCo = null;
//    }
//    private void OnDrawGizmos()
//    {
//        if (!Application.isPlaying || enemyType == EnemyType.C) return;
//        if (behavior == null) return;

//        // 구체 위치
//        Vector3 start = transform.position;
//        Vector3 end = start + transform.forward * behavior.Range;

//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(start, behavior.Radius);
//        Gizmos.DrawWireSphere(end, behavior.Radius);

//        // 방향선
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawLine(start, end);
//    }
//}
