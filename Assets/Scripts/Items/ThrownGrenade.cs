using System.Collections;
using UnityEngine;

public class ThrownGrenade : MonoBehaviour
{
    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private GameObject effect;
    private Rigidbody rb;

    private Coroutine ExplodeCo;

    private static readonly WaitForSeconds wait30 = new WaitForSeconds(3f);
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ExplodeCo = StartCoroutine(ExplodeGrenade());
    }
    private IEnumerator ExplodeGrenade()
    {
        yield return wait30;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        effect.SetActive(false);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(100, transform.position, true);
            }
        }
        Destroy(gameObject, 4f);
    }
}
