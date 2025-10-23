using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuidedMissile : Projectile
{
    [NonSerialized]
    public Transform targetTransform;
    private NavMeshAgent meshAgent;
    // Start is called before the first frame update
    void Awake()
    {
        meshAgent = GetComponent<NavMeshAgent>();
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
            meshAgent.SetDestination(targetTransform.position);
            yield return YieldCache.WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
}
