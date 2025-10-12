using UnityEngine;

public class MeleeMonster : MonsterBase
{
    [SerializeField]
    protected Collider hitBox;
    public Collider HitBox => hitBox;

    protected override void Awake()
    {
        base.Awake();
        if (hitBox == null)
        {
            Debug.LogWarning($"{name}: MeleeMonster인데 HitBox가 지정되지 않음");
        }
    }
}