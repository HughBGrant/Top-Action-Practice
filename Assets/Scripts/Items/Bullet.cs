using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    private float groundDestroyDelay = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            Destroy(gameObject, groundDestroyDelay);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable target))
        {
            target.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
    }
}
