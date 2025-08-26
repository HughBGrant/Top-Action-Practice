using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private BoxCollider meleeRange;
    [SerializeField]
    private TrailRenderer trailEffect;

    private Coroutine swingCo;

    private static readonly int doSwingHash = Animator.StringToHash("doSwing");
    public override int DoAttackHash { get => doSwingHash; }

    private static readonly WaitForSeconds wait01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds wait03 = new WaitForSeconds(0.3f);
    private void Awake()
    {
        //_meleeRange = GetComponent<BoxCollider>();
    }
    public override void Use()
    {
        if (swingCo != null)
        {
            StopCoroutine(swingCo);
        }
        swingCo = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        yield return wait01;
        meleeRange.enabled = true;
        trailEffect.enabled = true;

        yield return wait03;
        meleeRange.enabled = false;

        yield return wait03;
        trailEffect.enabled = false;
    }
}