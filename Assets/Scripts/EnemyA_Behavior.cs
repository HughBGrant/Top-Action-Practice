using System.Collections;

public class EnemyA_Behavior : IEnemyBehavior
{
    public float Radius => 1.5f;
    public float Range => 3f;

    public IEnumerator Attack(Enemy enemy)
    {
        yield return YieldCache.WaitForSeconds(0.2f);
        enemy.HitBox.enabled = true;
        yield return YieldCache.WaitForSeconds(1.0f);
        enemy.HitBox.enabled = false;
        yield return YieldCache.WaitForSeconds(1.0f);
    }
}
