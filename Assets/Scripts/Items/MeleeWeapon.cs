using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private BoxCollider attackCollider;
    [SerializeField]
    private TrailRenderer trailEffect;

    private Coroutine swingCo;

    public override int AttackHash { get { return AnimID.SwingHash; } }

    private void Awake()
    {
        attackCollider = GetComponent<BoxCollider>();
    }
    public override void Use()
    {
        if (swingCo != null)
        {
            StopCoroutine(swingCo);
        }
        swingCo = StartCoroutine(MeleeSwing());
    }

    private IEnumerator MeleeSwing()
    {
        yield return YieldCache.WaitForSeconds(0.1f);
        attackCollider.enabled = true;
        trailEffect.enabled = true;

        yield return YieldCache.WaitForSeconds(0.6f);
        trailEffect.enabled = false;
        attackCollider.enabled = false;

        swingCo = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            Debug.Log("sdfsdfsd");
            target.TakeDamage(damage, transform.position);
        }
    }
}