using UnityEngine;

public class Projectile : IDamageSource
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Wall))
        {
            Destroy(gameObject);
            return;
        }
        DealDamageTo(other.gameObject);
    }
}
