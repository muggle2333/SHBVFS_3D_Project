using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{

    private GameObject playerVfx;
    [SerializeField] private Canvas canvas;
    //private GameObject gachaContainer;
    [SerializeField] private GameObject vfx_Player_Point;
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private LineRenderer attackLine;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text hpText;

    [SerializeField] private MeshRenderer meshRenderer;
    private bool isPlayingVfx = false;
    private float hitVfxFloat = -1f;
    //private void Start()
    //{
    //    canvas= GetComponent<Canvas>();
    //}
    private void Update()
    {
        hpText.text = GetComponentInParent<Player>().HP.ToString();
        //Canvas follow camera
        Vector3 targetPos = canvas.transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 targetOrientation = Camera.main.transform.rotation * Vector3.up;
        canvas.transform.LookAt(targetPos, targetOrientation);
        if(Input.GetMouseButton(0))
        {
            StartCoroutine("PlayHitVfx");
        }
        if (isPlayingVfx)
        {
            meshRenderer.materials[5].SetFloat("_Float", hitVfxFloat);
        }        
    }
    public void SetPlayerName(bool isSelf)
    {
        playerText.text = isSelf ? "SELF" : "RIVAL";

    }
    public void Move(GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        transform.LookAt(dirPos);

        if (gridObject.landType==LandType.Mountain)
        {
            dirPos += new Vector3(0, 1.7f, 0);
        }
        //else if(gridObject.landType == LandType.Lake)
        //{
        //    dirPos += new Vector3(0, -1f, 0);
        //}
        transform.position = dirPos;
        
        GetComponent<Player>().currentGrid = gridObject;
        GetComponent<Player>().trueGrid = gridObject;

        if(playerVfx!=null&&Vector3.Distance(playerVfx.transform.position, transform.position)<GridManager.Instance.gridDistance*0.5f)
        {
            HideVfxPlayer();
        }
    }

    public void MoveVfxPlayer(GridObject gridObject)
    {
        if (playerVfx == null)
        {
            string path = GetComponent<Player>().Id == PlayerId.RedPlayer? "VfxPlayer_Red":"VfxPlayer_Blue";
            playerVfx = Instantiate(Resources.Load<GameObject>(path));
            playerVfx.transform.position = transform.position;
        }
        playerVfx.SetActive(true);
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        transform.LookAt(dirPos);
        if (gridObject.landType == LandType.Mountain)
        {
            dirPos += new Vector3(0, 1.7f, 0);
        }
        //else if (gridObject.landType == LandType.Lake)
        //{
        //    dirPos += new Vector3(0, -1f, 0);
        //}
        playerVfx.transform.position = dirPos;
        GetComponent<Player>().currentGrid = gridObject;
    }

    public void HideVfxPlayer()
    {
        if (playerVfx == null) return;
        playerVfx.SetActive(false);
    }

    //public void TryGacha(GridObject gridObject)
    //{
    //    if (gachaContainer == null)
    //    {
    //        gachaContainer = new GameObject("gachaVfxContainer");
    //    }
    //    GameObject gachaTmp = Instantiate(Resources.Load<GameObject>("VfxGacha"), gachaContainer.transform);
    //    Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
    //    gachaTmp.transform.position = dirPos;
    //}

    //public void ResetGachaVfx()
    //{
    //    if (gachaContainer == null) return;
    //    Destroy(gachaContainer);
    //    gachaContainer= null;
    //}

    public void UpdateLinePath(LandType landType)
    {
        pathLine.positionCount += 1;
        Vector3 offset = new Vector3(0, 0.1f, 0);
        if (landType == LandType.Mountain)
        {
            offset = new Vector3(0, 1.7f, 0);
        }
        pathLine.SetPosition(pathLine.positionCount - 1, playerVfx.transform.position + offset);
    }

    public void RefreshLinePath()
    {
        pathLine.transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.5f, 0), pathLine.transform.up);
        pathLine.positionCount = 1;
        pathLine.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
    }

    public void DeduceFirstPath()
    {
        Vector3[] positionArray = new Vector3[pathLine.positionCount];
        pathLine.GetPositions(positionArray);
        var positionList = positionArray.ToList<Vector3>();
        positionList.RemoveAt(0);
        pathLine.SetPositions(positionList.ToArray<Vector3>());
    }

    public void SetPlayerPointed(bool isPointed)
    {
        vfx_Player_Point.SetActive(isPointed);
    }

    public void SetAttackPath(Transform start,Transform target)
    {
        attackLine.positionCount = 2;
        attackLine.SetPosition(0,start.position + new Vector3(0, 0.1f, 0));
        attackLine.SetPosition(1,target.position + new Vector3(0, 0.1f, 0));
        Invoke("HideAttackPath", 1f);
    }

    public void HideAttackPath()
    {
        attackLine.positionCount = 0;
    }

    IEnumerator PlayHitVfx()
    {
        hitVfxFloat = -1;
        isPlayingVfx = true;
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, 1, 1f);
        yield return new WaitForSeconds(1f);
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, -1, 1f);
        yield return new WaitForSeconds(1f);
        isPlayingVfx= false;
        yield return null;
    }
}
