using System.Collections;

public interface IMonsterBehavior
{
    float Radius { get; }
    float Range { get; }

    IEnumerator Attack(MonsterBase monster);
}
