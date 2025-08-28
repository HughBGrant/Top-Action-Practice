using UnityEngine;
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected float attackSpeed;
    public float AttackSpeed { get => attackSpeed; }
    public virtual int CurrentMagazine { get; set; }
    public virtual int MaxMagazine { get;}
    public abstract int DoAttackHash { get; }

    public abstract void Use();
}
