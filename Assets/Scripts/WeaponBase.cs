using UnityEngine;
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected float attackSpeed;
    public float AttackSpeed { get => attackSpeed; }
    public abstract int DoAttackHash { get; }

    public abstract void Use();
}
