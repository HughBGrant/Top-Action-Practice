using System.Collections;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    private static readonly int ShootHash = Animator.StringToHash("shoot");

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform ejectPoint;
    [SerializeField]
    private GameObject casingPrefab;
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
        Rigidbody bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Rigidbody>();
        float bulletSpeed = 50f;
        bullet.velocity = firePoint.forward * bulletSpeed;
        yield return null;

        Rigidbody casing = Instantiate(casingPrefab, ejectPoint.position, ejectPoint.rotation).GetComponent<Rigidbody>();
        Vector3 casingVec = ejectPoint.forward * Random.Range(1, 4) * -1 + Vector3.up * Random.Range(1, 4);
        casing.AddForce(casingVec, ForceMode.Impulse);
        float casingSpinForce = 10f;
        casing.AddTorque(Vector3.up * casingSpinForce, ForceMode.Impulse);

        shootCo = null;
    }
}