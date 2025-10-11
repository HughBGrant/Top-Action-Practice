using System.Collections;

public interface IEnemyBehavior
{
    float Radius { get; }
    float Range { get; }

    IEnumerator Attack(EnemyBase enemy);
}
