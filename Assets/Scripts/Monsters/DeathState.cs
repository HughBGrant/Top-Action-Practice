using UnityEngine;

public class DeathState : MonsterState
{
    public override MonsterStateType StateType { get { return MonsterStateType.Dead; } }
    private Vector3 hitPoint;
    private bool isHitGrenade;
    public DeathState(MonsterBase monster) : base(monster) { }
    public override void Enter()
    {
        monster.MeshAgent.enabled = false;

        monster.Animator.SetTrigger("Die");

        foreach (MeshRenderer mesh in monster.Meshes)
        {
            mesh.material.color = Color.gray;
        }
        monster.gameObject.layer = LayerMask.NameToLayer("DeadMonster");

        Vector3 hitDir = (monster.transform.position - hitPoint).normalized + (Vector3.up * (isHitGrenade ? 3f : 1f));
        monster.Rigid.AddForce(hitDir * 5f, ForceMode.Impulse);
        monster.Target.Score += monster.RewardScore;
        int randomIndex = Random.Range(0, monster.RewardCoins.Length);
        Object.Instantiate(monster.RewardCoins[randomIndex], monster.transform);

        switch (monster.Type)
        {
            case MonsterType.A:
                GameManager.Instance.MonsterACount--;
                break;
            case MonsterType.B:
                GameManager.Instance.MonsterBCount--;
                break;
            case MonsterType.C:
                GameManager.Instance.MonsterCCount--;
                break;
            case MonsterType.Boss:
                GameManager.Instance.MonsterDCount--;
                break;
        }

        if (isHitGrenade)
        {
            monster.Rigid.AddTorque(hitDir * 15, ForceMode.Impulse);
        }

        Object.Destroy(monster.gameObject, 3f);
    }
    public void SetDeathInfo(Vector3 hitPoint, bool isHitGrenade)
    {
        this.hitPoint = hitPoint;
        this.isHitGrenade = isHitGrenade;
    }
}