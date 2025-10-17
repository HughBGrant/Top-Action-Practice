using UnityEngine;

public class BossMonster : MonsterBase
{
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    Transform launchPointA;
    [SerializeField]
    Transform launchPointB;
}
