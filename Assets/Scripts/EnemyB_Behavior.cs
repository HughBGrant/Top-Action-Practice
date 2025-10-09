using System.Collections;
using UnityEngine;

public class EnemyB_Behavior : IEnemyBehavior
{
    public float Radius => 1f;
    public float Range => 12f;

    public IEnumerator Attack(Enemy enemy)
    {
        yield return Enemy.Wait01;
        enemy.Rigidbody.AddForce(enemy.transform.forward * 20, ForceMode.Impulse);
        enemy.HitBox.enabled = true;
        yield return Enemy.Wait05;
        enemy.Rigidbody.velocity = Vector3.zero;
        enemy.HitBox.enabled = false;
        yield return Enemy.Wait20;
    }
}