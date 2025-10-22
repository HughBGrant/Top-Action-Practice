using UnityEngine;

public class MissileSpinner : MonoBehaviour
{
    void Update()
    {
        float speed = 30f;
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
}
