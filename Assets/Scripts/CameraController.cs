using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset;

    void LateUpdate()
    {
        transform.position = _playerTransform.position + _offset;
    }
}
