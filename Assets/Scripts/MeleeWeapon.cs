using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [SerializeField] BoxCollider _meleeRange;
    [SerializeField] TrailRenderer _trail;
    static readonly WaitForSeconds W01 = new(0.1f);
    static readonly WaitForSeconds W03 = new(0.3f);

    protected override void PerformAttack()
    {
        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        yield return W01;
        _meleeRange.enabled = true;
        _trail.enabled = true;

        yield return W03;
        _meleeRange.enabled = false;

        yield return W03;
        _trail.enabled = false;
    }
}