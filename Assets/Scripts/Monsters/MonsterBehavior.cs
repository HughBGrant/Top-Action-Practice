using System.Collections;
using UnityEngine;

public class MonsterBehavior
{
    MonsterBase monster;
    protected MonsterType monsterType;

    public MonsterBehavior(MonsterBase monster, MonsterType monsterType)
    {
        this.monster = monster;
        this.monsterType = monsterType;
    }
    public IEnumerator ExecuteAttack()
    {
        switch (monsterType)
        {
            case MonsterType.A:
                yield return AttackTypeA();
                break;
            case MonsterType.B:
                yield return AttackTypeB();
                break;
            case MonsterType.C:
                yield return AttackTypeC();
                break;
        }
    }
    private IEnumerator AttackTypeA()
    {
        yield return YieldCache.WaitForSeconds(0.2f);
        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        monster.AttackCollider.enabled = false;
        yield return YieldCache.WaitForSeconds(1.0f);
    }
    private IEnumerator AttackTypeB()
    {
        yield return YieldCache.WaitForSeconds(0.1f);
        monster.Rigid.AddForce(monster.transform.forward * 20, ForceMode.Impulse);
        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        monster.Rigid.velocity = Vector3.zero;
        monster.AttackCollider.enabled = false;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
    private IEnumerator AttackTypeC()
    {
        yield return YieldCache.WaitForSeconds(0.5f);
        Vector3 spawnPos = monster.transform.position + Vector3.up * 3f;
        GameObject bullet = Object.Instantiate(monster.ProjectilePrefab, spawnPos, monster.transform.rotation);
        Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = monster.transform.forward * 20f;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}