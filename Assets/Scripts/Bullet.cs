using UnityEngine;

public class Bullet : MonoBehaviour, IDamageSource
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private void OnTriggerEnter(Collider other)         // Bullet, Boss Missile
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            Debug.Log("asdf");
            target.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)  // Casing
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            Destroy(gameObject, 3f);
        }
    }
}
