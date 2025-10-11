using System.Collections;
using UnityEngine;

public class EnemyA_Behavior : IEnemyBehavior
{
    public float Radius => 1.5f;
    public float Range => 3f;

    public IEnumerator Attack(EnemyBase enemy)
    {
        MeleeEnemy melee = enemy as MeleeEnemy;
        if (melee == null)
        {
            Debug.LogError($"{enemy.name}: EnemyA_Behavior는 MeleeEnemy만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.2f);
        melee.HitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        melee.HitBox.enabled = false;
        yield return YieldCache.WaitForSeconds(1.0f);
    }
}
