using System.Collections;
using UnityEngine;

public class ThrownGrenade : MonoBehaviour
{
    private static readonly WaitForSeconds Wait30 = new WaitForSeconds(3f);

    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private GameObject effect;

    private Rigidbody rb;

    private Coroutine explodeCo;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        explodeCo = StartCoroutine(ExplodeGrenade());
    }
    private IEnumerator ExplodeGrenade()
    {
        yield return Wait30;

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

        explodeCo = null;
    }
}
