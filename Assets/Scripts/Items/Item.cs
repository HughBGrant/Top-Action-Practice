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

    private const float RotationSpeed = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = rb.GetComponent<SphereCollider>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.isKinematic = true;
            groundChecker.enabled = false;
        }
    }
}
