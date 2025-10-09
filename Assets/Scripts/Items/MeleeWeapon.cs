using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    private static readonly int SwingHash = Animator.StringToHash("swing");
    private static readonly WaitForSeconds Wait01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds Wait03 = new WaitForSeconds(0.3f);

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    private BoxCollider hitBox;
    [SerializeField]
    private TrailRenderer trailEffect;

    private Coroutine swingCo;

    public override int AttackHash { get { return SwingHash; } }

    private void Awake()
    {
        hitBox = GetComponent<BoxCollider>();
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
        yield return Wait01;
        hitBox.enabled = true;
        trailEffect.enabled = true;

        yield return Wait03;
        hitBox.enabled = false;

        yield return Wait03;
        trailEffect.enabled = false;

        swingCo = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable target))
        {
            target.TakeDamage(damage, transform.position);
        }
    }
}