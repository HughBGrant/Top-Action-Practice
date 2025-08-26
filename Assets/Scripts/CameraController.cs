using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Vector3 offset;

    void LateUpdate()
    {
        transform.position = playerTransform.position + offset;
    }
}
