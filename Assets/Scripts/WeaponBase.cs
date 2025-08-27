using UnityEngine;
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected float attackSpeed;
    public float AttackSpeed { get => attackSpeed; }
    public virtual int CurrentMagazine { get => 0; }
    public virtual int MaxMagazine { get => 0; }
    public abstract int DoAttackHash { get; }

    public abstract void Use();
}
