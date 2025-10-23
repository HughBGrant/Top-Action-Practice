using UnityEngine;

public class RangedMonster : MonsterBase
{
    [SerializeField]
    private GameObject projectilePrefab;
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    protected override void Awake()
    {
        base.Awake();

        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{name}: RangedMonster인데 BulletPrefab이 지정되지 않음");
        }
    }
}