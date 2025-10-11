using System.Collections;
using UnityEngine;

public class EnemyC_Behavior : IEnemyBehavior
{
    public float Radius => 0.5f;
    public float Range => 25f;

    public IEnumerator Attack(EnemyBase enemy)
    {
        RangedEnemy ranged = enemy as RangedEnemy;
        if (ranged == null)
        {
            Debug.LogError($"{enemy.name}: EnemyC_Behavior는 RangedEnemy만 지원합니다.");
            yield break;
        }
        yield return YieldCache.WaitForSeconds(0.5f);
        Vector3 spawnPos = ranged.transform.position + Vector3.up * 3f;
        GameObject bullet = Object.Instantiate(ranged.BulletPrefab, spawnPos, ranged.transform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = ranged.transform.forward * 20;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}