using UnityEngine;

public class IdleState : MonsterState
{
    public override MonsterStateType StateType => MonsterStateType.Idle;

    private float timer;
    public IdleState(MonsterBase monster) : base(monster) { }

    public override void Enter()
    {
        timer = Random.Range(1f, 3f);
        monster.GetComponent<Animator>().SetBool("IsWalking", false);
    }

    public override void Update()
    {
        float distance = Vector3.Distance(monster.transform.position, monster.targetTransform.position);

        if (distance < 10f)
        {
            monster.ChangeState(MonsterStateType.Chase);
            return;
        }
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = Random.Range(1f, 3f);
        }
    }
}