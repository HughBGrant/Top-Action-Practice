using UnityEngine;

public class DeadState : MonsterState
{
    public override MonsterStateType StateType => MonsterStateType.Dead;
    public DeadState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        monster.Animator.SetTrigger("Die");
        monster.MeshAgent.enabled = false;

        monster.gameObject.layer = LayerMask.NameToLayer("DeadMonster");
        Object.Destroy(monster.gameObject, 2f);
    }


    //protected virtual void HandleDeath(Vector3 hitPoint, bool isHitGrenade = false)
    //{
    //    isDead = true;
    //    isChasing = false;
    //    meshAgent.enabled = false;
    //    foreach (MeshRenderer mesh in meshes)
    //    {
    //        mesh.material.color = Color.gray;
    //    }
    //    gameObject.layer = LayerMask.NameToLayer("DeadMonster");
    //    animator.SetTrigger(DieHash);

    //    Vector3 hitDir = (transform.position - hitPoint).normalized + Vector3.up * (isHitGrenade ? 3f : 1f);
    //    Rigid.AddForce(hitDir * 5f, ForceMode.Impulse);

    //    if (isHitGrenade)
    //    {
    //        Rigid.AddTorque(hitDir * 15, ForceMode.Impulse);
    //    }

    //    if (type != MonsterType.Boss)
    //    {
    //        Destroy(gameObject, 2f);
    //    }
    //}
}