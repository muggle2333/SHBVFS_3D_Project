using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest3 : MonoBehaviour
{
    public class CameraController : MonoBehaviour
    {
        private bool _isF;

        // [Header("???????")][SerializeField] private GameObject _camera;    //????
        [Header("????????")][SerializeField] private GameObject _target;
        [Header("?????")][SerializeField] private float _sensitivity = 2.0f;
        [Header("????")][SerializeField] private float _speed = 0.1f;

        private void Update()
        {
            W_A_S_D();
            //?????F ???????????????
            if (Input.GetKeyUp(KeyCode.F))
            {
                _isF = !_isF;
                Debug.Log("isF=" + _isF);
            }

            if (!_isF)
            {
                if (Input.GetMouseButton(0))
                {
                    Around();
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    LookAround();
                }
            }
        }

        //?????W?A?S?D????????????
        private void W_A_S_D()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * _speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * _speed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * _speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * _speed);
            }
        }

        //????????????????
        private void LookAround()
        {
            float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;
            transform.RotateAround(_target.transform.position, Vector3.up, mouseX);
            transform.RotateAround(_target.transform.position, transform.right, -mouseY);
            transform.LookAt(_target.transform);
        }

        //??????????????
        private void Around()
        {
            float rotateX = 0;
            float rotateY = 0;
            rotateX = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * _sensitivity;
            rotateY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _sensitivity;

            transform.localEulerAngles = new Vector3(rotateX, rotateY, 0);
        }
    }
}
