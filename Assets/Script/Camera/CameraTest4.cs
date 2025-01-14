using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using static UIPopupList;

public class CameraTest4 : MonoBehaviour
{
    [SerializeField] private Vector2 boundaryX = new Vector2(-100, 100);
    [SerializeField] private Vector2 boundaryZ = new Vector2(-100, 100);
    [SerializeField] private AcademyLookAtComponent[] AcademyTextList;
    [SerializeField] private Vector3 moveSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Vector3 rotateSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Transform cam;
    [SerializeField] private Vector3 shakeOffset = new Vector3(2, 2, 0);
    private Vector3 pos = new Vector3(3, 13, -18);
    private float x = 0f;
    private bool isMoving = false;
    private Vector3 vec = new Vector3();
    private Guid uid;
    private Vector3 offset = new Vector3(0.2f, 0.15f, 0.2f);
    private bool isLocked = false;
    private void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        vec = transform.position;
        AcademyTextList = FindObjectsOfType<AcademyLookAtComponent>();
    }
    void Update()
    {
        
        //if(isMoving && 
        //    (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
        //{
        //    DOTween.Kill(uid);
        //    vec = transform.position;   
        //    isMoving = false;
        //    Debug.LogWarning(111111);
        //}
        MoveCamera();
        ScaleCamera();
        RotateCamera();
        if (Input.GetKeyDown(KeyCode.F))
        {
            FocusGrid();
        }

        if(Input.GetKeyUp(KeyCode.R))
        {
            CameraShake();
        }
    }

    private void MoveCamera()
    {
        if (isLocked) return;
        float x = Input.GetAxis("Horizontal") * 0.05f * moveSpeed.x;
        float z = Input.GetAxis("Forward") * 0.05f * moveSpeed.z;
        transform.Translate(new Vector3(x, 0, z));
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(transform.position.x, boundaryX.x, boundaryX.y);
        newPos.z = Mathf.Clamp(transform.position.z, boundaryZ.x, boundaryZ.y);
        transform.position = newPos;    
    }

    private void ScaleCamera()
    {
        if (isLocked) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(cam.localPosition.z < -10) cam.Translate(new Vector3(0, 0, 3));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(cam.localPosition.z > -50) cam.Translate(new Vector3(0, 0, -3));
        }
    }

    private void RotateCamera()
    {
        if (isLocked) return;
        if (Input.GetMouseButton(2))
        {
            x += Input.GetAxis("Mouse X") * rotateSpeed.x * 0.05f;
            var rotation = Quaternion.Euler(0, x, 0);
            transform.rotation = rotation;
            updateAcademyTextRotation();
        }
    }

    public void FocusGrid()
    {
        var seq = DOTween.Sequence();
        //isMoving = true;
        var selectedGridObject = SelectManager.Instance.GetLatestGridObject();
        if (selectedGridObject != null)
        {
            vec = GridManager.Instance.grid.GetWorldPositionCenter(selectedGridObject.x, selectedGridObject.z);
        }
        else
        {
            vec = GameplayManager.Instance.currentPlayer.transform.position;
        }
        //var uid = System.Guid.NewGuid();
        //seq.id = uid;
        seq.Append(transform.DOMove(vec, 0.5f));
        seq.Join(cam.DOLocalMove(pos, 0.5f));
        //cam.DOMoveZ(-19, 1f);
        //seq.AppendCallback(() => { isMoving = false; });

        if(GameplayManager.Instance.isBluePlayer)
        {
            x = 180f;
        }
        else
        {
            x = 0f;
        }
        var rotation = Quaternion.Euler(0, x, 0);
        //transform.rotation = rotation;
        seq.Join(transform.DORotateQuaternion(rotation, 0.5f));
        seq.AppendCallback(() => updateAcademyTextRotation());
    }

    private void updateAcademyTextRotation()
    {
        int rotateY = (int)transform.localRotation.eulerAngles.y;
        rotateY = ((rotateY + 30) / 60) * 60;
        //Debug.LogWarning(rotateY);
        foreach (AcademyLookAtComponent text in AcademyTextList)
        {
            //text.transform.rotation.y = (float)rotateY;
            text.transform.localRotation = Quaternion.Euler(0.0f, rotateY, 0.0f);
        }
    }
  
    public void FocusEnemy()
    {
        var seq = DOTween.Sequence();
        vec = GameplayManager.Instance.playerList[1].transform.position;
        seq.Append(transform.DOMove(vec, 0.5f));
        seq.Join(cam.DOLocalMove(pos, 0.5f));
    }

    public void FocusPosition(Vector3 position,float zoomScale)
    {
        var seq = DOTween.Sequence();
        vec = position;
        seq.Append(transform.DOMove(vec, 0.5f));
        if(GameplayManager.Instance.isBluePlayer)
        {
            Vector3 backward = new Vector3(-cam.transform.forward.x + offset.x, cam.transform.forward.y +offset.y, -cam.transform.forward.z + offset.z);
            seq.Join(cam.DOLocalMove(pos + backward * zoomScale, 0.5f));
        }
        else
        {
            Vector3 forward = new Vector3(cam.transform.forward.x + offset.x, cam.transform.forward.y +offset.y, cam.transform.forward.z + offset.z);
            seq.Join(cam.DOLocalMove(pos + forward * zoomScale, 0.5f));
        }
    }
    public void FocusGridKeepZoom(GridObject gridObject)
    {
        var dis = cam.transform.localPosition;
        var seq = DOTween.Sequence();
        vec = GridManager.Instance.grid.GetWorldPositionCenter(gridObject.x, gridObject.z);
        seq.Append(transform.DOMove(vec, 0.5f));
        seq.Join(cam.DOLocalMove(dis, 0.5f));
    }
    public void FocusGridCenter(GridObject gridObject1,GridObject gridObject2)
    {
        var dis = cam.transform.localPosition;
        var seq = DOTween.Sequence();
        vec = (GridManager.Instance.grid.GetWorldPositionCenter(gridObject1.x, gridObject1.z) + GridManager.Instance.grid.GetWorldPositionCenter(gridObject2.x, gridObject2.z)) / 2;
        seq.Append(transform.DOMove(vec, 0.5f));
        seq.Join(cam.DOLocalMove(dis, 0.5f));
        //CheckShouldTurnCamera(vec);
    }

    public void FocusGirdCenterZoom(GridObject gridObject1, GridObject gridObject2)
    {
        var dis = cam.transform.localPosition;
        var seq = DOTween.Sequence();
        var length = Vector3.Distance(GridManager.Instance.grid.GetWorldPositionCenter(gridObject1.x, gridObject1.z), GridManager.Instance.grid.GetWorldPositionCenter(gridObject2.x, gridObject2.z));
        
        float zoomScale = -1 * length / 2.5f;
        vec = (GridManager.Instance.grid.GetWorldPositionCenter(gridObject1.x, gridObject1.z) + GridManager.Instance.grid.GetWorldPositionCenter(gridObject2.x, gridObject2.z)) / 2;
        
        seq.Append(transform.DOMove(vec, 0.5f));
        if (GameplayManager.Instance.isBluePlayer)
        {
            Vector3 backward = new Vector3(-cam.transform.forward.x + offset.x, cam.transform.forward.y + offset.y, -cam.transform.forward.z + offset.z);
            seq.Join(cam.DOLocalMove(pos + backward * zoomScale, 0.5f));
        }
        else
        {
            Vector3 forward = new Vector3(cam.transform.forward.x + offset.x, cam.transform.forward.y + offset.y, cam.transform.forward.z + offset.z);
            seq.Join(cam.DOLocalMove(pos + forward * zoomScale, 0.5f));
        }
    }
    public void LockCamera(bool isLocked)
    {
        this.isLocked = isLocked;
    }

    //private bool CheckShouldTurnCamera(Vector3 pos)
    //{
    //    if(pos.x>GridManager.Instance.gridDistance*5 && pos.z> GridManager.Instance.gridDistance * 5)
    //    {
    //        var rotation = Quaternion.Euler(0, 180, 0);
    //        transform.rotation = rotation;
    //        updateAcademyTextRotation();
    //        return true;
    //    }
    //    return false;
    //}

    public void CameraShake()
    {
        StartCoroutine(Shake(0.3f, 0.1f));
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = cam.transform.position;
        float elapsed = 0f;
        while (elapsed < duration/4)
        {
            float x = 1 * magnitude;
            float y = -1 * magnitude;
            cam.transform.position += new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return 0;

        }
        while (elapsed < duration/2)
        {
            float x = -1 * magnitude;
            float y = 1 * magnitude;
            cam.transform.position += new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return 0;

        }
        while (elapsed < duration *0.75)
        {
            float x = -1 * magnitude;
            float y = 1 * magnitude;
            cam.transform.position += new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return 0;

        }
        while (elapsed < duration)
        {
            float x = 1 * magnitude;
            float y = -1 * magnitude;
            cam.transform.position += new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return 0;

        }
        cam.transform.position = orignalPosition;
    }

}
