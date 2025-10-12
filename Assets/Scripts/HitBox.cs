using UnityEngine;

public class HitBox : MonoBehaviour, IDamageSource
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}
