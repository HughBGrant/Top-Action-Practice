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

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
