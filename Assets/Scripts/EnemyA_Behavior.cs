using System.Collections;

public class EnemyA_Behavior : IEnemyBehavior
{
    public float Radius => 1.5f;
    public float Range => 3f;

    public IEnumerator Attack(Enemy enemy)
    {
        yield return Enemy.Wait02;
        enemy.HitBox.enabled = true;
        yield return Enemy.Wait10;
        enemy.HitBox.enabled = false;
        yield return Enemy.Wait10;
    }
}
