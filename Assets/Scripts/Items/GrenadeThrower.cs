using System.Collections;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private GameObject effect;

    private Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        StartCoroutine(ExplodeGrenade());
    }
    private IEnumerator ExplodeGrenade()
    {
        yield return YieldCache.WaitForSeconds(3.0f);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        effect.SetActive(false);

        float radius = 15f;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0f, LayerMask.GetMask("Monster"));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent(out MonsterBase monster))
            {
                monster.TakeDamage(damage, transform.position, true);
            }
        }
        Destroy(gameObject, 4f);
    }
}
