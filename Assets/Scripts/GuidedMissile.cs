using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuidedMissile : Projectile
{
    private Transform targetTransform;
    public Transform TargetTransform { get { return targetTransform; } set { targetTransform = value; } }

    private NavMeshAgent meshAgent;

    void Awake()
    {
        meshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        StartCoroutine(TrackTarget());
    }
    private void Update()
    {
        Debug.Log(TargetTransform);
    }
    public IEnumerator TrackTarget()
    {
        while (TargetTransform != null)
        {
            meshAgent.SetDestination(targetTransform.position);
            yield return YieldCache.WaitForSeconds(0.1f);
        }
    }
}
