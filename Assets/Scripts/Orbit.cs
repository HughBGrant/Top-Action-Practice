using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField]
    private float orbitSpeed;

    void Update()
    {
        transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime);
    }
}