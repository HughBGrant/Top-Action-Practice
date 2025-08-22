using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [SerializeField] Transform _firePoint;
    [SerializeField] GameObject _bulletPrefab; // (풀 사용 권장)

    protected override void PerformAttack()
    {
        // 객체 풀을 쓰고 있으면 여기서 Pull; 없으면 Instantiate
        var go = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        // go.GetComponent<Bullet>().Init(Damage, ...);
    }
}
