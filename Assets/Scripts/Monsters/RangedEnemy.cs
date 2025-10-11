using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [SerializeField] protected GameObject bulletPrefab;
    public GameObject BulletPrefab => bulletPrefab;

    protected override void Awake()
    {
        base.Awake();
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"{name}: RangedEnemy인데 BulletPrefab이 지정되지 않음");
        }
    }
}