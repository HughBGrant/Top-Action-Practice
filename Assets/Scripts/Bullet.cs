using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            Destroy(gameObject, 3);//////
        }
        else if (collision.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
    }
}
