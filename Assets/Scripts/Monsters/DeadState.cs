using UnityEngine;

public class DeadState : MonsterState
{
    public override MonsterStateType StateType => MonsterStateType.Dead;
    private Vector3 hitPoint;
    private bool isHitGrenade;
    public DeadState(MonsterBase monster) : base(monster) { }
    public override void Enter()
    {
        monster.MeshAgent.enabled = false;

        monster.Animator.SetTrigger("Die");

        foreach (MeshRenderer mesh in monster.Meshes)
        {
            mesh.material.color = Color.gray;
        }
        monster.gameObject.layer = LayerMask.NameToLayer("DeadMonster");

        Vector3 hitDir = (monster.transform.position - hitPoint).normalized + Vector3.up * (isHitGrenade ? 3f : 1f);
        monster.Rigid.AddForce(hitDir * 5f, ForceMode.Impulse);

        if (isHitGrenade)
        {
            monster.Rigid.AddTorque(hitDir * 15, ForceMode.Impulse);
        }

        if (monster.Type != MonsterType.Boss)
        {
            Object.Destroy(monster.gameObject, 2f);
        }
    }
    public void SetDeathInfo(Vector3 hitPoint, bool isHitGrenade)
    {
        this.hitPoint = hitPoint;
        this.isHitGrenade = isHitGrenade;
    }
}