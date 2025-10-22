using UnityEngine;

public static class LayerIndex
{
    public static readonly int Wall = LayerMask.NameToLayer("Wall");
    public static readonly int Ground = LayerMask.NameToLayer("Ground");
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Weapon = LayerMask.NameToLayer("Weapon");
    public static readonly int Item = LayerMask.NameToLayer("Item");
    public static readonly int MonsterHitBox = LayerMask.NameToLayer("MonsterHitBox");
    public static readonly int Melee = LayerMask.NameToLayer("Melee");

    public static bool IsInMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }
}