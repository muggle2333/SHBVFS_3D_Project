using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraTest4 : MonoBehaviour
{
    [SerializeField] private Vector3 moveSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Vector3 rotateSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Transform cam;
    float x = 0f;

    private void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
    }
    void Update()
    {
        MoveCamera();
        ScaleCamera();
        RoateCamera();
    }

    private void MoveCamera()
    {
        float x = Input.GetAxis("Horizontal") * 0.05f * moveSpeed.x;
        float z = Input.GetAxis("Forward") * 0.05f * moveSpeed.z;
        transform.Translate(new Vector3(x, 0, z));
    }

    private void ScaleCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cam.Translate(new Vector3(0, 0, 3));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cam.Translate(new Vector3(0, 0, -3));
        }
    }

    private void RoateCamera()
    {
        if(Input.GetMouseButton(2))
        {
            x += Input.GetAxis("Mouse X") * rotateSpeed.x * 0.05f;
            var rotation = Quaternion.Euler(0, x, 0);
            transform.rotation = rotation;
        }
    }
}
