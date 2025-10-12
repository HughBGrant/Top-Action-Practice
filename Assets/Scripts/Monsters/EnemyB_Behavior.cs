using System.Collections;
using UnityEngine;

public class EnemyB_Behavior : IEnemyBehavior
{
    public float Radius => 1f;
    public float Range => 12f;

    public IEnumerator Attack(EnemyBase enemy)
    {
        MeleeEnemy melee = enemy as MeleeEnemy;
        if (melee == null)
        {
            Debug.LogError($"{enemy.name}: EnemyB_Behavior는 MeleeEnemy만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.1f);
        melee.GetComponent<Rigidbody>().AddForce(enemy.transform.forward * 20, ForceMode.Impulse);///////
        melee.HitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        melee.GetComponent<Rigidbody>().velocity = Vector3.zero;
        melee.HitBox.enabled = false;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}