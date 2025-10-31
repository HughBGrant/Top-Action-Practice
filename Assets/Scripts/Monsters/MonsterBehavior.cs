using System.Collections;
using UnityEngine;

public class MonsterBehavior
{
    protected MonsterType monsterType;

    public float AttackRadius { get; private set; }
    public float AttackRange { get; private set; }

    public MonsterBehavior(MonsterType monsterType)
    {
        this.monsterType = monsterType;

        // 몬스터 타입별 기본 공격 범위 설정
        switch (monsterType)
        {
            case MonsterType.A:
                AttackRadius = 1.5f;
                AttackRange = 3f;
                break;
            case MonsterType.B:
                AttackRadius = 1f;
                AttackRange = 12f;
                break;
            case MonsterType.C:
                AttackRadius = 0.5f;
                AttackRange = 25f;
                break;
        }
    }
    public IEnumerator ExecuteAttack(MonsterBase monster)
    {
        switch (monsterType)
        {
            case MonsterType.A:
                yield return AttackTypeA(monster);
                break;
            case MonsterType.B:
                yield return AttackTypeB(monster);
                break;
            case MonsterType.C:
                yield return AttackTypeC(monster);
                break;
        }
    }
    private IEnumerator AttackTypeA(MonsterBase monster)
    {
        if (monster is not MeleeMonster melee) { yield break; }

        yield return YieldCache.WaitForSeconds(0.2f);
        melee.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        melee.AttackCollider.enabled = false;
        yield return YieldCache.WaitForSeconds(1.0f);
    }
    private IEnumerator AttackTypeB(MonsterBase monster)
    {
        if (monster is not MeleeMonster melee) { yield break; }

        yield return YieldCache.WaitForSeconds(0.1f);
        melee.Rigid.AddForce(melee.transform.forward * 20, ForceMode.Impulse);
        melee.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        melee.Rigid.velocity = Vector3.zero;
        melee.AttackCollider.enabled = false;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
    private IEnumerator AttackTypeC(MonsterBase monster)
    {
        if (monster is not RangedMonster ranged) { yield break; }

        yield return YieldCache.WaitForSeconds(0.5f);
        Vector3 spawnPos = ranged.transform.position + Vector3.up * 3f;
        GameObject bullet = Object.Instantiate(ranged.ProjectilePrefab, spawnPos, ranged.transform.rotation);
        Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = ranged.transform.forward * 20f;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}