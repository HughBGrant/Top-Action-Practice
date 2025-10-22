using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuidedMissile : Projectile
{
    [NonSerialized]
    public Transform targetTransform;
    private NavMeshAgent navAgent;
    // Start is called before the first frame update
    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        StartCoroutine(TrackTarget());
    }

    private IEnumerator TrackTarget()
    {
        yield return null;
        while (targetTransform != null)
        {
            navAgent.SetDestination(targetTransform.position);
            yield return YieldCache.WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
}
