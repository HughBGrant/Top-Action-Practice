using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    private BoxCollider hitBox;
    [SerializeField]
    private TrailRenderer trailEffect;

    private Coroutine swingCo;

    private static readonly int doSwingHash = Animator.StringToHash("swing");
    public override int DoAttackHash { get { return doSwingHash; } }

    private static readonly WaitForSeconds wait01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds wait03 = new WaitForSeconds(0.3f);
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
        yield return wait01;
        hitBox.enabled = true;
        trailEffect.enabled = true;

        yield return wait03;
        hitBox.enabled = false;

        yield return wait03;
        trailEffect.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable target))
        {
            target.TakeDamage(damage, transform.position);
        }
    }
}