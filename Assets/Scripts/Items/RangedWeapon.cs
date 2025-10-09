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

    private const float BulletSpeed = 50f;
    private const float CasingSpinForce = 10f;

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
        GameObject bulletInstant = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bulletInstant.GetComponent<Rigidbody>();
        bulletRb.velocity = firePoint.forward * BulletSpeed;
        yield return null;

        GameObject casingInstant = Instantiate(casingPrefab, ejectPoint.position, ejectPoint.rotation);
        Rigidbody casingRb = casingInstant.GetComponent<Rigidbody>();
        Vector3 casingVec = ejectPoint.forward * -Random.Range(1, 4) + Vector3.up * Random.Range(1, 4);
        casingRb.AddForce(casingVec, ForceMode.Impulse);
        casingRb.AddTorque(Vector3.up * CasingSpinForce, ForceMode.Impulse);

        shootCo = null;
    }
}