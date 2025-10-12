using UnityEngine;

public class RangedMonster : MonsterBase
{
    [SerializeField]
    protected GameObject bulletPrefab;
    public GameObject BulletPrefab => bulletPrefab;

    protected override void Awake()
    {
        base.Awake();

        if (bulletPrefab == null)
        {
            Debug.LogWarning($"{name}: RangedMonster인데 BulletPrefab이 지정되지 않음");
        }
    }
}