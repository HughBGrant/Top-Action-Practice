using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private Vector3 offset;

    void LateUpdate()
    {
        transform.position = targetTransform.position + offset;
    }
}
