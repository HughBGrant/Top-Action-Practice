using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _type;
    public ItemType Type {  get => _type; }

    [SerializeField] int _value;
    public int Value { get => _value; }


    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);///////////
    }
}
