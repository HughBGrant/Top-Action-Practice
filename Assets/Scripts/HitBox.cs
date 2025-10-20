using UnityEngine;

public class HitBox : MonoBehaviour, IDamageSource
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private void OnTriggerEnter(Collider other)         // Monster Hitbox
    {
        bool isBossAtk = gameObject.name == "Jump Attack Hit Box";
        if (isBossAtk)
        {
            if (other.TryGetComponent(out Rigidbody targetRb))
            {
                targetRb.AddForce(other.transform.forward * 250 * -1, ForceMode.Impulse);
            }
        }
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(Damage, transform.position);
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
