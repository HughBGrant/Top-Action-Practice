using System.Collections;
using UnityEngine;

public class BossMonsterBehavior : IAttackBehavior
{
    private BossMonster monster;
    public BossMonsterBehavior(BossMonster monster)
    {
        this.monster = monster;
    }
    public IEnumerator ExecuteAttack()
    {
        yield return YieldCache.WaitForSeconds(0.1f);
        float rand = Random.value;

        if (rand < 0.01f)
            yield return PerformMissileAttack();
        else if (rand < 0.02f)
            yield return PerformRockThrow();
        else
            yield return PerformJumpAttack();
    }
    private IEnumerator PerformMissileAttack()
    {
        monster.Animator.SetTrigger("LaunchMissile");
        yield return new WaitForSeconds(0.2f);
        GuidedMissile missileA = (GuidedMissile)Object.Instantiate(monster.ProjectilePrefab, monster.LaunchPointA.position, monster.LaunchPointA.rotation);
        missileA.TargetTransform = monster.Target.transform;

        yield return YieldCache.WaitForSeconds(0.3f);
        GuidedMissile missileB = (GuidedMissile)Object.Instantiate(monster.ProjectilePrefab, monster.LaunchPointB.position, monster.LaunchPointB.rotation);
        missileB.TargetTransform = monster.Target.transform;

        yield return YieldCache.WaitForSeconds(2f);
    }
    private IEnumerator PerformRockThrow()
    {
        monster.IsTrackingTarget = false;
        monster.Animator.SetTrigger("ThrowRock");
        Object.Instantiate(monster.RockPrefab, monster.transform.position, monster.transform.rotation);

        yield return YieldCache.WaitForSeconds(3.0f);
        monster.IsTrackingTarget = true;
    }
    private IEnumerator PerformJumpAttack()
    {
        monster.MeshAgent.SetDestination(monster.Target.transform.position);

        monster.IsTrackingTarget = false;
        monster.MeshAgent.isStopped = false;
        monster.MainCollider.enabled = false;
        monster.Animator.SetTrigger("JumpAttack");
        yield return YieldCache.WaitForSeconds(1.5f);

        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        monster.AttackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);
        monster.IsTrackingTarget = true;
        monster.MainCollider.enabled = true;
        monster.MeshAgent.isStopped = true;
    }
}

