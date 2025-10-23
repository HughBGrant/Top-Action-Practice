using UnityEngine;

public class HitBox : IDamageSource
{
    [SerializeField]
    private bool isJumpAttack;
    private void OnTriggerEnter(Collider other)
    {
        DealDamageTo(other.gameObject, false);

        if (isJumpAttack && other.TryGetComponent(out Rigidbody targetRigid))
        {
            targetRigid.AddForce(other.transform.forward * -250, ForceMode.Impulse);
        }
    }
}
