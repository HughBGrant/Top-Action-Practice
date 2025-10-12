using System.Collections;
using UnityEngine;

public class MonsterBehavior
{
    private MonsterType type;

    public float Radius { get; private set; }
    public float Range { get; private set; }

    public MonsterBehavior(MonsterType type)
    {
        this.type = type;

        // 몬스터 타입별 기본 공격 범위 설정
        switch (type)
        {
            case MonsterType.A:
                Radius = 1.5f;
                Range = 3f;
                break;
            case MonsterType.B:
                Radius = 1f;
                Range = 12f;
                break;
            case MonsterType.C:
                Radius = 0.5f;
                Range = 25f;
                break;
        }
    }

    public IEnumerator Attack(MonsterBase monster)
    {
        switch (type)
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
        if (monster is MeleeMonster melee)
        {
            yield return YieldCache.WaitForSeconds(0.2f);
            melee.HitBox.enabled = true;
            yield return YieldCache.WaitForSeconds(1.0f);
            melee.HitBox.enabled = false;
            yield return YieldCache.WaitForSeconds(1.0f);
        }
    }

    private IEnumerator AttackTypeB(MonsterBase monster)
    {
        if (monster is MeleeMonster melee)
        {
            yield return YieldCache.WaitForSeconds(0.1f);
            melee.GetComponent<Rigidbody>().AddForce(monster.transform.forward * 20, ForceMode.Impulse);
            melee.HitBox.enabled = true;
            yield return YieldCache.WaitForSeconds(0.5f);
            melee.GetComponent<Rigidbody>().velocity = Vector3.zero;
            melee.HitBox.enabled = false;
            yield return YieldCache.WaitForSeconds(2.0f);
        }
    }

    private IEnumerator AttackTypeC(MonsterBase monster)
    {
        if (monster is RangedMonster ranged)
        {
            yield return YieldCache.WaitForSeconds(0.5f);
            Vector3 spawnPos = ranged.transform.position + Vector3.up * 3f;
            GameObject bullet = Object.Instantiate(ranged.BulletPrefab, spawnPos, ranged.transform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = ranged.transform.forward * 20f;
            yield return YieldCache.WaitForSeconds(2.0f);
        }
    }
}