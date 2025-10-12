using UnityEngine;

public class BelongingGrenade : MonoBehaviour
{
    void Update()
    {
        float angleSpeed = 20f;
        transform.Rotate(Vector3.up, angleSpeed * Time.deltaTime);
    }
}