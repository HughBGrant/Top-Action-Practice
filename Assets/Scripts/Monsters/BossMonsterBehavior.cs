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
        float randomValue = Random.value;

        if (randomValue < 0.4f)
            yield return PerformMissileAttack();
        else if (randomValue < 0.8f)
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
        monster.IsTrackingTarget = false;
        monster.MainCollider.enabled = false;
        yield return null;
        monster.MeshAgent.isStopped = false;
        monster.MeshAgent.SetDestination(monster.Target.transform.position);
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

