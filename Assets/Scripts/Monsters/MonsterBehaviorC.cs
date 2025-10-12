using System.Collections;
using UnityEngine;

public class MonsterBehaviorC : IMonsterBehavior
{
    public float Radius => 0.5f;
    public float Range => 25f;

    public IEnumerator Attack(MonsterBase monsterBase)
    {
        RangedMonster monster = monsterBase as RangedMonster;
        if (monster == null)
        {
            Debug.LogError($"{monsterBase.name}: MonsterBehaviorC RangedMonster만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.5f);
        Vector3 spawnPos = monster.transform.position + Vector3.up * 3f;
        GameObject bullet = Object.Instantiate(monster.BulletPrefab, spawnPos, monster.transform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        float speed = 20f;
        bulletRb.velocity = monster.transform.forward * speed;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}