using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [SerializeField] private int _damage;
    [SerializeField] private BoxCollider _meleeRange;
    [SerializeField] private TrailRenderer _trailEffect;

    private Coroutine _swingCo;
    public override int doAttackHash => _doSwingHash;
    private static readonly int _doSwingHash = Animator.StringToHash("doSwing");

    private static readonly WaitForSeconds _wait01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds _wait03 = new WaitForSeconds(0.3f);
    private void Awake()
    {
        //_meleeRange = GetComponent<BoxCollider>();
    }
    public override void Use()
    {
        if (_swingCo != null)
        {
            StopCoroutine(_swingCo);
        }
        _swingCo = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        yield return _wait01;
        _meleeRange.enabled = true;
        _trailEffect.enabled = true;

        yield return _wait03;
        _meleeRange.enabled = false;

        yield return _wait03;
        _trailEffect.enabled = false;
    }
}