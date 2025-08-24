using System.Collections;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _ejectPoint;
    [SerializeField] private GameObject _casingPrefab;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int curAmmo;

    private Coroutine _shotCo;
    public override int doAttackHash => _doShotHash;
    private static readonly int _doShotHash = Animator.StringToHash("doShot");

    public override void Use()
    {
        //if (curAmmo <= 0) { return; }
        if (_shotCo != null)
            StopCoroutine(_shotCo);

        _shotCo = StartCoroutine(ShotRoutine());
    }
    
    private IEnumerator ShotRoutine()
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