using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int DieHash = Animator.StringToHash("die");
    private static readonly WaitForSeconds Wait01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds Wait02 = new WaitForSeconds(0.2f);
    private static readonly WaitForSeconds Wait05 = new WaitForSeconds(0.5f);
    private static readonly WaitForSeconds Wait10 = new WaitForSeconds(1.0f);
    private static readonly WaitForSeconds Wait20 = new WaitForSeconds(2.0f);
    private static int deadEnemyLayer;

    [SerializeField]
    private EnemyType enemyType;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private Transform targetPoint;
    [SerializeField]
    private BoxCollider hitBox;
    [SerializeField]
    private GameObject bulletPrefab;

    private bool isChasing;
    private bool isAttacking;

    private Rigidbody rb;
    private Material material;
    private NavMeshAgent navAgent;
    private Animator animator;

    private Coroutine hitCo;

    private const float DeathDestroyDelay = 2f;
    private const float DeathReactionMultiplier = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        material = GetComponentInChildren<MeshRenderer>().material;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
    }
    private void Start()
    {
        currentHealth = maxHealth;

        StartCoroutine(StartChase());
    }
    private void FixedUpdate()
    {
        Target();

    }
    private void Update()
    {
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

        if (hitCo != null)
        {
            StopCoroutine(hitCo);
        }
        hitCo = StartCoroutine(HitFlash());

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

        rb.AddForce(hitDirection * DeathReactionMultiplier, ForceMode.Impulse);

        if (isHitGrenade)
        {
            rb.AddTorque(hitDirection * 15, ForceMode.Impulse);
        }

        Destroy(gameObject, DeathDestroyDelay);
    }
    private void Target()
    {
        float radius = 0f;
        float range = 0f;

        switch (enemyType)
        {
            case EnemyType.A:
                radius = 1.5f;
                range = 3f;
                break;
            case EnemyType.B:
                radius = 1f;
                range = 12f;
                break;
            case EnemyType.C:
                radius = 0.5f;
                range = 25f;
                break;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, range, LayerMask.GetMask("Player"));

        if (hits.Length > 0 && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    private IEnumerator StartChase()
    {
        yield return Wait20;

        isChasing = true;
        animator.SetBool(IsWalkingHash, true);
    }
    private IEnumerator Attack()
    {
        isChasing = false;
        isAttacking = true;
        animator.SetBool(IsAttackingHash, true);

        switch (enemyType)
        {
            case EnemyType.A:

                yield return Wait02;
                hitBox.enabled = true;

                yield return Wait10;
                hitBox.enabled = false;
                yield return Wait10;
                break;
            case EnemyType.B:
                yield return Wait01;
                rb.AddForce(transform.forward * 20, ForceMode.Impulse);//////

                hitBox.enabled = true;

                yield return Wait05;
                rb.velocity = Vector3.zero;

                hitBox.enabled = false;
                yield return Wait20;
                break;

            case EnemyType.C:
                yield return Wait05;
                GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, 3, 0), transform.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.velocity = transform.forward * 20;/////

                yield return Wait20;
                break;
        }
        isChasing = true;
        isAttacking = false;
        animator.SetBool(IsAttackingHash, false);

    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || enemyType == EnemyType.C) return;

        float radius = 0f;
        float range = 0f;

        switch (enemyType)
        {
            case EnemyType.A: radius = 1.5f; range = 3f; break;
            case EnemyType.B: radius = 1f; range = 6f; break;
            case EnemyType.C: radius = 1.5f; range = 3f; break;
        }

        // 구체 위치
        Vector3 start = hitBox.transform.position;
        Vector3 end = start + transform.forward * range;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);

        // 방향선
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }
}
