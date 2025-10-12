using UnityEngine;

public class BelongingGrenade : MonoBehaviour
{
    void Update()
    {
        float revolution = 20f;
        transform.Rotate(Vector3.up, revolution * Time.deltaTime);
    }
}