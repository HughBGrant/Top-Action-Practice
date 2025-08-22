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
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _ejectPoint;
    [SerializeField] private GameObject _casingPrefab;


    private Coroutine _swingCo;
    private Coroutine _shotCo;


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
            _swingCo = StartCoroutine(SwingRoutine());
        }
        else if (WeaponType == WeaponType.Ranged)
        {
            if (_shotCo != null)
            {
                StopCoroutine(_shotCo);
            }
            _shotCo = StartCoroutine(ShotRoutine());
        }
    }

    IEnumerator SwingRoutine()
    {
        yield return _waitForSeconds01;
        _meleeRange.enabled = true;
        _trailEffect.enabled = true;

        yield return _waitForSeconds03;
        _meleeRange.enabled = false;

        yield return _waitForSeconds03;
        _trailEffect.enabled = false;
    }
    IEnumerator ShotRoutine()
    {
        GameObject bulletInstant = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        Rigidbody bulletRb = bulletInstant.GetComponent<Rigidbody>();
        bulletRb.velocity = _firePoint.forward * 50;/////////////////
        yield return null;

        GameObject casingInstant = Instantiate(_casingPrefab, _ejectPoint.position, _ejectPoint.rotation);
        Rigidbody casingRb = casingInstant.GetComponent<Rigidbody>();
        Vector3 casingVec = _ejectPoint.forward * -Random.Range(1, 4) + Vector3.up * Random.Range(1, 4);
        casingRb.AddForce(casingVec, ForceMode.Impulse);
        casingRb.AddTorque(Vector3.up * 10, ForceMode.Impulse);////////////
    }
}
