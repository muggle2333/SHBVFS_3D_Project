using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraTest2 : MonoBehaviour
{
    //?????
    public Transform target;
    //???
    public float instanceDis_Y = 0;
    public float instanceDis_X = 0;
    //????
    public float scaleFactor = 120;
    //??????
    public float scaleFactor_Min = 1;
    public float scaleFactor_Max = 1000;
    //??????
    public float moveSpeed = 1f;
    //??????  
    public float xSpeed = 90.0f;
    public float ySpeed = 40.0f;
    //????????  
    public float yMinLimit = -10;
    public float yMaxLimit = 80;
    //????????
    public float x = 0;
    public float y = 20f;
    //???????
    public bool ISOrthogonality;
    private void Awake()
    {
        if (!target)
        {
            GameObject iteml = new GameObject();
            iteml.name = "cameraTarget";
            iteml.transform.position = new Vector3(-45.6f, 0, -82f);
            target = iteml.transform;
        }
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
    }
    //????????  
    public float Orthogonality_Size_Min = 5;
    public float Orthogonality_Size_Max = 300;
    //??????
    public float zoomDampening = 2.0f;
    public bool ISBound;
    //?? ??
    private Quaternion currentRotation;
    //??
    private Quaternion desiredRotation;
    void Start()
    {
        //Vector3 angles = transform.eulerAngles;
        //x = angles.y;
        //y = angles.x;
    }
    private bool dragging = false;
    private void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!ISOrthogonality)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (!dragging) StartCoroutine(DragTarget(Input.mousePosition));
                }

                if (!ISBound)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (!Input.GetMouseButtonDown(0))
                        {
                            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                            y = ClampAngle(y, yMinLimit, yMaxLimit);
                        }
                    }
                    desiredRotation = Quaternion.Euler(y, x, 0);
                }

                ScrollView();
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    Vector3 p = transform.right;
                    p.y = 0;
                    transform.Translate(p * Input.GetAxis("Mouse X") * moveSpeed * 5, Space.World);
                    transform.Translate(Vector3.right * -Input.GetAxis("Mouse Y") * moveSpeed * 5, Space.World);
                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    //Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor;
                    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    {
                        if (Camera.main.orthographicSize < Orthogonality_Size_Min)
                        {

                        }
                        else
                        {
                            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor * 0.2f;
                        }
                    }
                    else
                    {
                        if (Camera.main.orthographicSize < Orthogonality_Size_Max)
                        {
                            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor * 0.2f;
                        }

                    }
                }
            }

        }
        // target????????????  
        if (!ISOrthogonality)
        {
            if (target)
            {
                // ????????  
                currentRotation = transform.rotation;
                Quaternion rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;
                Vector3 position = rotation * new Vector3(0f, instanceDis_X, instanceDis_Y - FollowDistance) + target.position;
                transform.position = position;
            }
        }
    }
    IEnumerator DragTarget(Vector3 startingHit)
    {
        dragging = true;
        Vector3 startTargetPos = target.position;
        while (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
        {
            Vector3 mouseMove = 0.0001f * moveSpeed * transform.position.y * (Input.mousePosition - startingHit);
            //Vector3 translation = new Vector3(mouseMove.x, 0, mouseMove.y);
            //float clampVal = 0.04f * (transform.position.y - target.position.y);
            Vector3 zDir = transform.up;
            zDir.x = 0;
            target.position = startTargetPos - transform.right * mouseMove.x - zDir.normalized * mouseMove.y;
            yield return null;
        }
        dragging = false;
    }
    //????????
    public float distance = 100f;
    //??
    private float zoomRate = 100;
    //??
    private float FollowDistance = 100;
    void ScrollView()
    {
        float scrollinp = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollinp * Time.deltaTime * zoomRate * Mathf.Abs(distance);
        distance = Mathf.Clamp(distance, scaleFactor_Min, scaleFactor_Max);
        FollowDistance = Mathf.Lerp(FollowDistance, distance, Time.deltaTime * zoomDampening);
    }

    /// <summary>
    /// ???????
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public overlay_Orthogonal set_value = new overlay_Orthogonal();
    //????????
    public void SetCamera_Posi(bool isora, overlay_Orthogonal set_value)
    {
        this.set_value = set_value;
    }
    //??????
    public class overlay_Orthogonal
    {
        public float CameraControlDistance;
        public float CameraControl_X;
        public float CameraControl_Y;
        public Vector3 cameraTargetBeginPos;
        //public float moveSpeed;
    }

}

