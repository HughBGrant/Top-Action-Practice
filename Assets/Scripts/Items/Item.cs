using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemType type;
    public ItemType Type { get { return type; } }

    [SerializeField]
    int value;
    public int Value { get { return value; } }

    private float rotationSpeed = 20f;

    private Rigidbody rb;
    private SphereCollider groundChecker;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = rb.GetComponent<SphereCollider>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.isKinematic = true;
            groundChecker.enabled = false;
        }
    }
}
