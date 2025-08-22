using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected float _attackSpeed;
    bool _onCooldown;

    public float AttackSpeed => _attackSpeed;
    public int Damage => _damage;

    public void Use()
    {
        if (_onCooldown) return;
        StartCoroutine(CooldownRoutine());
        PerformAttack();
    }

    protected abstract void PerformAttack();

    IEnumerator CooldownRoutine()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_attackSpeed);
        _onCooldown = false;
    }
}
