using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    private static readonly int SwingHash = Animator.StringToHash("Swing");

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    private BoxCollider attackCollider;
    [SerializeField]
    private TrailRenderer trailEffect;

    private Coroutine swingCo;

    public override int AttackHash { get { return SwingHash; } }

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

        yield return YieldCache.WaitForSeconds(0.3f);
        attackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        swingCo = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage, transform.position);
        }
    }
}