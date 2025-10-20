using UnityEngine;
using UnityEngine.Serialization;

public class RangedMonster : MonsterBase
{
    [SerializeField]
    [FormerlySerializedAs("bulletPrefab")]
    protected GameObject missilePrefab;
    public GameObject MissilePrefab => missilePrefab;

    protected override void Awake()
    {
        base.Awake();

        if (missilePrefab == null)
        {
            Debug.LogWarning($"{name}: RangedMonster인데 BulletPrefab이 지정되지 않음");
        }
    }
}