using UnityEngine;

public class HitBox : MonoBehaviour, IDamageSource
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private void OnTriggerEnter(Collider other)         // Monster
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(Damage, transform.position);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)  // BossRock
    {
        if (collision.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(Damage, transform.position);
            Destroy(gameObject);
        }
    }
}
