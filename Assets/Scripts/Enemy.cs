using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Material material;

    private Coroutine hitCo;

    private static int deadEnemyLayer;
    private float deathDestroyDelay = 2f;
    private float deathReactionMultiplier = 5f;
    private static readonly WaitForSeconds hitFlashTime = new WaitForSeconds(0.1f);
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
        currentHealth = maxHealth;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
        if (deadEnemyLayer == -1)
        {
            Debug.LogWarning("DeadEnemy 레이어를 프로젝트에 추가하세요!");
        }
    }
    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        currentHealth -= damage;
        Vector3 hitDir = (transform.position - hitPoint).normalized;

        if (hitCo != null)
        {
            StopCoroutine(hitCo);
        }
        hitCo = StartCoroutine(HitRoutine(hitDir));
    }
    IEnumerator HitRoutine(Vector3 hitDirection)
    {
        material.color = Color.red;
        yield return hitFlashTime;

        if (currentHealth <= 0)
        {
            Die(hitDirection);
        }
        else
        {
            material.color = Color.white;
        }
    }
    private void Die(Vector3 hitDirection)
    {
        material.color = Color.gray;

        if (deadEnemyLayer != -1)
        {
            gameObject.layer = deadEnemyLayer;
        }
        if (rb != null)
        {
            rb.AddForce((hitDirection + Vector3.up) * deathReactionMultiplier, ForceMode.Impulse);
        }
        Destroy(gameObject, deathDestroyDelay);
    }
}
