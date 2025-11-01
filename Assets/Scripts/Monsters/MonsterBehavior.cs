using System.Collections;
using UnityEngine;

public class MonsterBehavior : IAttackBehavior
{
    MonsterBase monster;

    public MonsterBehavior(MonsterBase monster)
    {
        this.monster = monster;
    }
    public IEnumerator ExecuteAttack()
    {
        switch (monster.Type)
        {
            case MonsterType.A:
                yield return MeleeAttack();
                break;
            case MonsterType.B:
                yield return ChargeAttack();
                break;
            case MonsterType.C:
                yield return RangedAttack();
                break;
        }
    }
    private IEnumerator MeleeAttack()
    {
        yield return YieldCache.WaitForSeconds(0.2f);
        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        monster.AttackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);
    }
    private IEnumerator ChargeAttack()
    {
        yield return YieldCache.WaitForSeconds(0.1f);
        monster.Rigid.isKinematic = false;
        monster.Rigid.AddForce(monster.transform.forward * 20, ForceMode.Impulse);
        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        monster.Rigid.velocity = Vector3.zero;
        monster.Rigid.isKinematic = true;
        monster.AttackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(2.0f);
    }
    private IEnumerator RangedAttack()
    {
        yield return YieldCache.WaitForSeconds(0.5f);
        Vector3 spawnPos = monster.transform.position + Vector3.up * 3f;
        Rigidbody bullet = Object.Instantiate(monster.ProjectilePrefab, spawnPos, monster.transform.rotation).GetComponent<Rigidbody>();
        bullet.velocity = monster.transform.forward * 20f;

        yield return YieldCache.WaitForSeconds(2.0f);
    }
}