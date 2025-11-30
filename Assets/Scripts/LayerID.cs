using UnityEngine;

public static class LayerID
{
    public static readonly int Ground = LayerMask.NameToLayer("Ground");
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Bullet = LayerMask.NameToLayer("Bullet");
    public static readonly int Casing = LayerMask.NameToLayer("Casing");
    public static readonly int Wall = LayerMask.NameToLayer("Wall");
    public static readonly int Monster = LayerMask.NameToLayer("Monster");
    public static readonly int DeadMonster = LayerMask.NameToLayer("DeadMonster");
    public static readonly int MonsterHitBox = LayerMask.NameToLayer("MonsterHitBox");
    public static readonly int Shop = LayerMask.NameToLayer("Shop");
    public static readonly int Weapon = LayerMask.NameToLayer("Weapon");
    public static readonly int Item = LayerMask.NameToLayer("Item");

    public static bool IsInMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }
}