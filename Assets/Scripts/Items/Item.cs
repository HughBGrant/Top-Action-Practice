using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemType type;
    public ItemType Type { get { return type; } }

    [SerializeField]
    private int value;
    public int Value { get { return value; } }

    private Rigidbody rigid;
    private SphereCollider groundChecker;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        groundChecker = GetComponent<SphereCollider>();
    }
    void Update()
    {
        float speed = 20f;
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerID.Ground)
        {
            rigid.isKinematic = true;
            groundChecker.enabled = false;
        }
    }
}

