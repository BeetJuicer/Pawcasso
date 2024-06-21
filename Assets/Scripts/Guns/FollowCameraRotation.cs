using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Transform camTransform;

    // Start is called before the first frame update
    void Start()
    {
        cam.TryGetComponent<Transform>(out camTransform);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = camTransform.rotation;
    }
}
