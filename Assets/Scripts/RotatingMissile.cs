using UnityEngine;

public class RotatingMissile : MonoBehaviour
{
    void Update()
    {
        float speed = 30f;
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
}
