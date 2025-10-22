using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemType type;
    public ItemType Type { get { return type; } }

    [SerializeField]
    int value;
    public int Value { get { return value; } }

    private Rigidbody rb;
    private SphereCollider groundChecker;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<SphereCollider>();
    }
    void Update()
    {
        float speed = 20f;
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tag.Ground))
        {
            rb.isKinematic = true;
            groundChecker.enabled = false;
        }
    }
}

