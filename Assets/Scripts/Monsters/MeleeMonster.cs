using UnityEngine;

public class MeleeMonster : MonsterBase
{
    [SerializeField]
    protected Collider attackCollider;
    public Collider AttackCollider { get { return attackCollider; } }

    protected override void Awake()
    {
        base.Awake();
        behavior = new MonsterBehavior(this, type);

        if (attackCollider == null)
        {
            Debug.LogWarning($"{name}: MeleeMonster인데 HitBox가 지정되지 않음");
        }
    }
}
