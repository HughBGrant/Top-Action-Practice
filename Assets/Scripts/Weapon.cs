using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private BoxCollider _meleeRange;
    [SerializeField] private TrailRenderer _trailEffect;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _ejectPoint;
    [SerializeField] private GameObject _casing;


    private Coroutine _swingCo;


    [SerializeField] private float _attackSpeed;
    public float AttackSpeed { get => _attackSpeed; }

    [SerializeField] private WeaponType _weaponType;
    public WeaponType WeaponType { get => _weaponType; }

    private static readonly WaitForSeconds _waitForSeconds01 = new WaitForSeconds(0.1f);
    private static readonly WaitForSeconds _waitForSeconds03 = new WaitForSeconds(0.3f);
    private void Awake()
    {
    }
    public void Use()
    {
        if (WeaponType == WeaponType.Melee)
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
