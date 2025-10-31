using System.Collections;
using UnityEngine;

public class BossMonsterBehavior
{
    private BossMonster monster;
    public BossMonsterBehavior(BossMonster monster)
    {
        this.monster = monster;
    }
    public IEnumerator ExecuteRandomAttack()
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
        monster.Animator.SetTrigger("LaunchMissile");
        yield return new WaitForSeconds(0.2f);

        GuidedMissile missileA = Object.Instantiate(monster.ProjectilePrefab, monster.LaunchPointA.position, monster.LaunchPointA.rotation).GetComponent<GuidedMissile>();

        missileA.TargetTransform = monster.TargetTransform;

        yield return YieldCache.WaitForSeconds(0.3f);
        GuidedMissile missileB = Object.Instantiate(monster.ProjectilePrefab, monster.LaunchPointB.position, monster.LaunchPointB.rotation).GetComponent<GuidedMissile>();
        missileB.TargetTransform = monster.TargetTransform;

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
        monster.MeshAgent.SetDestination(monster.TargetTransform.position);

        monster.IsTrackingTarget = false;
        monster.MeshAgent.isStopped = false;
        monster.MainCollider.enabled = false;
        monster.Animator.SetTrigger("JumpAttack");
        yield return YieldCache.WaitForSeconds(1.5f);

        monster.AttackCollider.enabled = true;
        yield return YieldCache.WaitForSeconds(0.5f);
        monster.AttackCollider.enabled = false;

        yield return YieldCache.WaitForSeconds(1.0f);

        monster.MainCollider.enabled = true;
        monster.MeshAgent.isStopped = true;
        monster.IsTrackingTarget = true;
    }
}

