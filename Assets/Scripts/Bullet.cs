using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage;
    private float groundDestroyDelay = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            Destroy(gameObject, groundDestroyDelay);
        }
        else if (collision.gameObject.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
        }
    }
}
