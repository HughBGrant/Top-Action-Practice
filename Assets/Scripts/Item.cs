using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemType type;
    public ItemType Type { get => type; }

    [SerializeField]
    int value;
    public int Value { get => value; }
    private float rotationSpeed = 20f;

    private Rigidbody rb;
    private SphereCollider sphereCollider;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = rb.GetComponent<SphereCollider>();
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
            sphereCollider.enabled = false;
        }
    }
}
