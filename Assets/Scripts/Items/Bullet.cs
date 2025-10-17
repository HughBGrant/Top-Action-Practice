using UnityEngine;

public class Bullet : MonoBehaviour, IDamageSource
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            Destroy(gameObject, 3f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            Debug.Log("sfdasdf");
            target.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
    }
}
