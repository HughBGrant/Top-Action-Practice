using UnityEngine;

public class GrenadeSpinner : MonoBehaviour
{
    void Update()
    {
        float revolution = 20f;
        transform.Rotate(Vector3.up, revolution * Time.deltaTime);
    }
}