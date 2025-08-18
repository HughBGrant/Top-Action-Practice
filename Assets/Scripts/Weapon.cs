using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private int _damage;
    public float AttackSpeed;
    [SerializeField] private BoxCollider _meleeRange;
    [SerializeField] private TrailRenderer _trailEffect;

    private static readonly WaitForSeconds _waitForSeconds01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds _waitForSeconds03 = new WaitForSeconds(0.3f);


    private Coroutine _swingCo;

    private void Awake()
    {
        //_meleeRange = GetComponent<BoxCollider>();
        //_trailEffect = GetComponent<TrailRenderer>();
    }
    public void Use()
    {
        if (_weaponType == WeaponType.Melee)
        {
            if (_swingCo != null)
            {
                StopCoroutine(_swingCo);
            }
            _swingCo = StartCoroutine(Swing());
        }
    }
    IEnumerator Swing()
    {
        yield return _waitForSeconds01;
        _meleeRange.enabled = true;
        _trailEffect.enabled = true;

        yield return _waitForSeconds03;
        _meleeRange.enabled = false;

        yield return _waitForSeconds03;
        _trailEffect.enabled = false;
    }
}
