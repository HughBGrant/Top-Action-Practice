using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [SerializeField]
    protected Collider hitBox;
    public Collider HitBox => hitBox;

    protected override void Awake()
    {
        base.Awake();
        if (hitBox == null)
        {
            Debug.LogWarning($"{name}: MeleeEnemy인데 HitBox가 지정되지 않음");
        }
    }
}