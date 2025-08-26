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

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
