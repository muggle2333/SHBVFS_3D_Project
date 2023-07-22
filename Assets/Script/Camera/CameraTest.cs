using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.ProBuilder.Shapes;
using DG.Tweening;

public class CameraTest : MonoBehaviour
{
    public Transform target;//rotate center
    public float CameraMax_X;
    public float CameraMin_X;
    public float CameraMax_Y;
    public float CameraMin_Y;
    public float CameraMax_Z;
    public float CameraMin_Z;

    public Vector2 rotationSpeed = new Vector2(250.0f, 120.0f);
    public Vector2 moveSpeed = new Vector2(20.0f, 20.0f);

    float yMinLimit = -20;
    float yMaxLimit = 80;

    private float distance;
    private float x = 0.0f;
    private float y = 0.0f;

    private bool isTargetMoving = false;

    void Start()
    {
        distance = (transform.position - target.position).magnitude;
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        if (target)
        {
            UpdateRotate();
        }
    }

    void LateUpdate()
    {
        if (isTargetMoving)
        {
            //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 24, 60 * Time.deltaTime);
            UpdateRotate();
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FocusOnPlayer();
        }
        DragRotation();
        DragMove();
        Scale();
        var position = gameObject.transform.position;
        position = new Vector3(Mathf.Clamp(position.x, CameraMin_X, CameraMax_X),
                               Mathf.Clamp(position.y, CameraMin_Y, CameraMax_Y),
                               Mathf.Clamp(position.z, CameraMin_Z, CameraMax_Z));

        gameObject.transform.position = position;
    }

    void DragRotation()
    {
        if (Input.GetMouseButton(2))
        {
            if (target)
            {
                UpdateRotate();
            }
        }
    }

    void DragMove()
    {
        if (Input.GetMouseButton(1))
        {
            UpdateMove();
        }
    }

    void Scale()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void UpdateRotate()
    {
        if (!isTargetMoving)
        {
            x += Input.GetAxis("Mouse X") * rotationSpeed.x * 0.02f;
            y -= Input.GetAxis("Mouse Y") * rotationSpeed.y * 0.02f;
        }

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        target.rotation = rotation;
        transform.rotation = rotation;
        transform.position = position;
    }

    private void UpdateMove()
    {
        float xx = -Input.GetAxis("Mouse X") * moveSpeed.x * 0.02f;
        float yy = -Input.GetAxis("Mouse Y") * moveSpeed.y * 0.02f;
        Vector3 pos = new Vector3(xx, yy, 0.0f);
        transform.Translate(pos);
        target.transform.Translate(pos);
    }

    public void FocusOnPlayer()
    {
        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, 24, 0.8f);
        var targetPosition = GameplayManager.Instance.currentPlayer.gameObject.transform.position;
        target.DOMove(targetPosition, 1);
        isTargetMoving = true;
        Invoke("MovingStop", 1f);

        //target.position = GameplayManager.Instance.player.gameObject.transform.position;
        //UpdateRotate();
    }

    private void MovingStop()
    {
        isTargetMoving = false;
    }
}
