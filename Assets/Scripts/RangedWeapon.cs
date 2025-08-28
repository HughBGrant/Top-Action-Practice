using System.Collections;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
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
    public override int CurrentMagazine { get => currentMagazine; set => maxMagazine = value; }
    [SerializeField]
    private int maxMagazine;
    public override int MaxMagazine { get => maxMagazine; }

    private float bulletSpeed = 50f;
    private float casingSpinForce = 10f;

    private Coroutine shotCo;
    private static readonly int doShotHash = Animator.StringToHash("doShot");
    public override int DoAttackHash { get => doShotHash; }

    public override void Use()
    {
        if (currentMagazine <= 0) { return; }
        if (shotCo != null)
        {
            StopCoroutine(shotCo);
        }
        currentMagazine--;
        shotCo = StartCoroutine(ShotRoutine());
    }
    
    private IEnumerator ShotRoutine()
    {
        GameObject bulletInstant = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bulletInstant.GetComponent<Rigidbody>();
        bulletRb.velocity = firePoint.forward * bulletSpeed;
        yield return null;

        GameObject casingInstant = Instantiate(casingPrefab, ejectPoint.position, ejectPoint.rotation);
        Rigidbody casingRb = casingInstant.GetComponent<Rigidbody>();
        Vector3 casingVec = ejectPoint.forward * -Random.Range(1, 4) + Vector3.up * Random.Range(1, 4);
        casingRb.AddForce(casingVec, ForceMode.Impulse);
        casingRb.AddTorque(Vector3.up * casingSpinForce, ForceMode.Impulse);
    }
}