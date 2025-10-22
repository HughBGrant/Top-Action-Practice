using UnityEngine;

public class Projectile : IDamageSource
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerIndex.Wall)
        {
            Destroy(gameObject);
            return;
        }
        DealDamageTo(other.gameObject);
    }
}
