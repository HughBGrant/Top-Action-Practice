using System.Collections;
using UnityEngine;

public class EnemyC_Behavior : IEnemyBehavior
{
    public float Radius => 0.5f;
    public float Range => 25f;

    public IEnumerator Attack(Enemy enemy)
    {
        yield return YieldCache.WaitForSeconds(0.5f);
        GameObject bullet = Object.Instantiate(enemy.BulletPrefab, enemy.transform.position + new Vector3(0, 3, 0), enemy.transform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = enemy.transform.forward * 20;
        yield return YieldCache.WaitForSeconds(2.0f);
    }
}