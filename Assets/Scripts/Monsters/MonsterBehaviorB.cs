using System.Collections;
using UnityEngine;

public class MonsterBehaviorB : IMonsterBehavior
{
    public float Radius => 1f;
    public float Range => 12f;

    public IEnumerator Attack(MonsterBase monsterBase)
    {
        MeleeMonster monster = monsterBase as MeleeMonster;
        if (monster == null)
        {
            Debug.LogError($"{monsterBase.name}: MonsterBehaviorB MeleeMonster만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.1f);
        monster.GetComponent<Rigidbody>().AddForce(monsterBase.transform.forward * 20, ForceMode.Impulse);///////
        monster.HitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        monster.GetComponent<Rigidbody>().velocity = Vector3.zero;
        monster.HitBox.enabled = false;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}