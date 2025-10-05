using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int DieHash = Animator.StringToHash("die");
    private static readonly WaitForSeconds HitFlashTime = new WaitForSeconds(0.1f);
    private static int deadEnemyLayer;

    [SerializeField]
    private bool isChasing;
    [SerializeField]
    private bool isAttacking;

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private Transform targetPoint;
    [SerializeField]
    private BoxCollider bullet;

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
        if (deadEnemyLayer == -1)
        {
            Debug.LogWarning("DeadEnemy 레이어를 프로젝트에 추가하세요!");
        }

    }
    private void Start()
    {
        currentHealth = maxHealth;

        Invoke("StartChase", 2f);////////
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
        yield return HitFlashTime;

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
        float radius = 1.5f;
        float range = 3f;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, range, LayerMask.GetMask("Player"));
    }
    private void StartChase()
    {
        isChasing = true;
        animator.SetBool(IsWalkingHash, true);
    }
    private IEnumerator Attack()
    {
        isChasing = false;
        isAttacking = true;

        animator.SetBool(IsAttackingHash, true);
        yield return null;

    }
}
