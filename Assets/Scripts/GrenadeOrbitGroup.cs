using UnityEngine;

public class GrenadeOrbitGroup : MonoBehaviour
{
    [SerializeField] private float orbitSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime);
    }
}