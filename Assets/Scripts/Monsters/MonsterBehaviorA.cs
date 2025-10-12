using System.Collections;
using UnityEngine;

public class MonsterBehaviorA : IMonsterBehavior
{
    public float Radius => 1.5f;
    public float Range => 3f;

    public IEnumerator Attack(MonsterBase monsterBase)
    {
        MeleeMonster monster = monsterBase as MeleeMonster;
        if (monster == null)
        {
            Debug.LogError($"{monsterBase.name}: MonsterBehaviorA는 MeleeMonster만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.2f);
        monster.HitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        monster.HitBox.enabled = false;
        yield return YieldCache.WaitForSeconds(1.0f);
    }
}
