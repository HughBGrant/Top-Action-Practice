using System.Collections;
using UnityEngine;

public class ThrownGrenade : MonoBehaviour
{
    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private GameObject effect;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        StartCoroutine(ExplodeGrenade());
    }
    private IEnumerator ExplodeGrenade()
    {
        yield return YieldCache.WaitForSeconds(3.0f);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        effect.SetActive(false);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent(out EnemyBase enemy))
            {
                enemy.TakeDamage(100, transform.position, true);
            }
        }
        Destroy(gameObject, 4f);
    }
}
