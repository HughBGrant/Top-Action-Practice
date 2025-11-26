using UnityEngine;

public class Projectile : IDamageSource
{
    private Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layer.Wall)
        {
            Destroy(gameObject);
            return;
        }
        DealDamageTo(other.gameObject);
    }
}
