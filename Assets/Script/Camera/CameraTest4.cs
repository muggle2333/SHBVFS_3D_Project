using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CameraTest4 : MonoBehaviour
{
    [SerializeField] private AcademyLookAtComponent[] AcademyTextList;
    [SerializeField] private Vector3 moveSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Vector3 rotateSpeed = new Vector3(10f, 10f, 10f);
    [SerializeField] private Transform cam;
    private Vector3 pos = new Vector3(3, 13, -18);
    private float x = 0f;
    private bool isMoving = false;
    private Vector3 vec = new Vector3();
    private Guid uid;
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

    private void RotateCamera()
    {
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
    }

    public void FocusEnemy()
    {
        var seq = DOTween.Sequence();
        vec = GameplayManager.Instance.playerList[1].transform.position;
        seq.Append(transform.DOMove(vec, 0.5f));
        seq.Join(cam.DOLocalMove(pos, 0.5f));
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
}
