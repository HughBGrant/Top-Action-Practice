using UnityEngine;

public class IDamageSource : MonoBehaviour
{
    [SerializeField]
    protected int damage;
    public int Damage { get { return damage; } }

    protected void DealDamageTo(GameObject targetObject, bool destroyAfterHit = true)
    {
        if (targetObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(Damage, transform.position);
            if (destroyAfterHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
