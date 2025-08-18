using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private float _orbitSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _orbitSpeed * Time.deltaTime);
    }
}