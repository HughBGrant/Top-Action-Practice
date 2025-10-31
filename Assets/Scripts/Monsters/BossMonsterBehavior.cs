using System.Collections;
using UnityEngine;

public class BossMonsterBehavior : MonsterBehavior
{
    private BossMonster boss;
    public BossMonsterBehavior(MonsterType monsterType) : base(monsterType)
    {
        this.monsterType = monsterType;
    }
    private IEnumerator ExecuteRandomAttack()
    {
        float rand = Random.value;

        if (rand < 0.4f)
            yield return PerformMissileAttack();
        else if (rand < 0.8f)
            yield return PerformRockThrow();
        else
            yield return PerformJumpAttack();
    }
    private IEnumerator PerformMissileAttack()
    {
        boss.Animator.SetTrigger("LaunchMissile");
        yield return new WaitForSeconds(0.2f);

        GuidedMissile missileA = Object.Instantiate(boss.ProjectilePrefab, boss.LaunchPointA.position, boss.LaunchPointA.rotation).GetComponent<GuidedMissile>();

        missileA.TargetTransform = boss.TargetTransform;

        yield return YieldCache.WaitForSeconds(0.3f);
        GuidedMissile missileB = Object.Instantiate(boss.ProjectilePrefab, boss.LaunchPointB.position, boss.LaunchPointB.rotation).GetComponent<GuidedMissile>();
        missileB.TargetTransform = boss.TargetTransform;

        yield return YieldCache.WaitForSeconds(2f);
    }
    private IEnumerator PerformRockThrow()
    {
        boss.IsTrackingTarget = false;
        boss.Animator.SetTrigger("ThrowRock");
        Object.Instantiate(boss.RockPrefab, boss.transform.position, boss.transform.rotation);
        yield return YieldCache.WaitForSeconds(3.0f);
        boss.IsTrackingTarget = true;
    }
    private IEnumerator PerformJumpAttack()
    {
        boss.MeshAgent.SetDestination(boss.TargetTransform.position);

        boss.IsTrackingTarget = false;
        boss.MeshAgent.isStopped = false;
        boss.MainCollider.enabled = false;
        boss.Animator.SetTrigger("JumpAttack");
        yield return YieldCache.WaitForSeconds(1.5f);

        boss.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        boss.AttackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);

        boss.MainCollider.enabled = true;
        boss.MeshAgent.isStopped = true;
        boss.IsTrackingTarget = true;
    }
}

