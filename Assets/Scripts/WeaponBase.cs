using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected float _attackSpeed;

    public float AttackSpeed { get => _attackSpeed; }
    public abstract int doAttackHash { get; }

    public abstract void Use();
}
