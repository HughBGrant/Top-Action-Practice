using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private Transform targetPoint;

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Material material;
    NavMeshAgent navAgent;

    private Coroutine hitCo;
    private Coroutine reactCo;

    private static int deadEnemyLayer;
    private const float deathDestroyDelay = 2f;
    private const float deathReactionMultiplier = 5f;
    private static readonly WaitForSeconds hitFlashTime = new WaitForSeconds(0.1f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        navAgent = GetComponent<NavMeshAgent>();
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
    }
    private void Update()
    {
        navAgent.SetDestination(targetPoint.position);
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
            Vector3 hitDir = (transform.position - hitPoint).normalized;

            Die(hitDir, isHitGrenade);
        }

    }
    private IEnumerator HitFlash()
    {
        material.color = Color.red;
        yield return hitFlashTime;

        if (currentHealth > 0)
        {
            material.color = Color.white;
        }

    }
    private void Die(Vector3 hitDirection, bool isHitGrenade = false)
    {
        material.color = Color.gray;

        gameObject.layer = deadEnemyLayer;

        float hitGrenadeReactionMultiplier = isHitGrenade ? 3f : 1f;
        hitDirection += Vector3.up * hitGrenadeReactionMultiplier;

        rb.AddForce(hitDirection * deathReactionMultiplier, ForceMode.Impulse);

        if (isHitGrenade)
        {
            rb.AddTorque(hitDirection * 15, ForceMode.Impulse);
        }

        Destroy(gameObject, deathDestroyDelay);
    }
}
