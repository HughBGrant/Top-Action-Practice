using System.Collections;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private Projectile bulletPrefab;
    [SerializeField]
    private Transform ejectPoint;
    [SerializeField]
    private Casing casingPrefab;
    [SerializeField]
    private int currentMagazine;
    public override int CurrentMagazine { get { return currentMagazine; } set { currentMagazine = value; } }
    [SerializeField]
    private int maxMagazine;
    public override int MaxMagazine { get { return maxMagazine; } }

    private Coroutine shootCo;
    public override int AttackHash { get { return ShootHash; } }

    public override void Use()
    {
        if (currentMagazine <= 0) { return; }

        currentMagazine--;

        if (shootCo != null)
        {
            StopCoroutine(shootCo);
        }
        shootCo = StartCoroutine(ShootBullet());
    }
    private IEnumerator ShootBullet()
    {
        yield return YieldCache.WaitForFixedUpdate;

        Projectile bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        float bulletSpeed = 50f;
        bullet.Rigid.velocity = firePoint.forward * bulletSpeed;

        Casing casing = Instantiate(casingPrefab, ejectPoint.position, ejectPoint.rotation);
        Vector3 casingVec = ejectPoint.forward * Random.Range(1, 4) * -1 + Vector3.up * Random.Range(1, 4);
        casing.Rigid.AddForce(casingVec, ForceMode.Impulse);
        float casingSpinForce = 10f;
        casing.Rigid.AddTorque(Vector3.up * casingSpinForce, ForceMode.Impulse);

        shootCo = null;
    }
}