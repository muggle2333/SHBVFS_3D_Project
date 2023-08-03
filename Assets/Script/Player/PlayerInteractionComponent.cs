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
    [SerializeField] private TMP_Text cardText;

    [SerializeField] private GameObject craft;

    [SerializeField] private GameObject playerModel;
    private MeshRenderer meshRenderer;
    private bool isPlayingVfx = false;
    private float hitVfxFloat = -1f;
    private void Start()
    {
        //canvas = GetComponent<Canvas>();
        meshRenderer = playerModel.GetComponentInChildren<MeshRenderer>();
    }
    private void Update()
    {
        hpText.text = GetComponentInParent<Player>().HP.ToString();
        //cardText.text = CardManager.Instance.playerHandCardDict[GetComponentInParent<Player>()].Count.ToString();
        //Canvas follow camera
        Vector3 targetPos = canvas.transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 targetOrientation = Camera.main.transform.rotation * Vector3.up;
        canvas.transform.LookAt(targetPos, targetOrientation);
        if(Input.GetMouseButton(0))
        {
            //StartCoroutine("PlayHitVfx");
            //PlayHitVfxRed();
        }
        //if (isPlayingVfx)
        //{
        //    meshRenderer.materials[1].SetFloat("_Float", hitVfxFloat);
        //}        
    }

    public void UpdateCardNum(int num)
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && playerText.text== "ENEMY")
        {
            cardText.text = "--";
            return;
        }
        cardText.text = num.ToString();
    }
    public void SetPlayerName(bool isSelf)
    {
        playerText.text = isSelf ? "SELF" : "ENEMY";

    }
    public void Move(GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        transform.LookAt(dirPos);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        if (gridObject.landType==LandType.Mountain)
        {
            dirPos += new Vector3(0, 1.7f, 0);
        }
        if(gridObject.landType == LandType.Lake)
        {
            craft.SetActive(true);
        }
        else
        {
            craft.SetActive(false);
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
        if(GetComponent<Player>().Id == (PlayerId)NetworkManager.Singleton.LocalClientId)
        {
            if(GameManager.Instance.wholeGameState.Value != GameManager.WholeGameState.GamePlaying)
            {
                Invoke("UpdateSelectableGrid", 3f);
            }else
            {
                UpdateSelectableGrid();
            }
        }
        
    }
    public void UpdateSelectableGrid()
    {
        SelectManager.Instance.UpdateSelectableGridObject();
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
        playerVfx.transform.LookAt(dirPos);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        playerVfx.transform.localEulerAngles = new Vector3(0, playerVfx.transform.localEulerAngles.y, playerVfx.transform.localEulerAngles.z);
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

        if (GetComponent<Player>().Id == (PlayerId)NetworkManager.Singleton.LocalClientId)
        {
            if (GameManager.Instance.wholeGameState.Value != GameManager.WholeGameState.GamePlaying)
            {
                Invoke("UpdateSelectableGrid", 3f);
            }
            else
            {
                UpdateSelectableGrid();
            }
        }
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
        //pathLine.transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.5f, 0), pathLine.transform.up);
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
        //attackLine.positionCount = 2;
        //attackLine.SetPosition(0,start.position + new Vector3(0, 0.1f, 0));
        //attackLine.SetPosition(1,target.position + new Vector3(0, 0.1f, 0));
        int lineNum = 10;
        attackLine.positionCount = lineNum;
        Vector3 startPos = start.position + new Vector3(0, 3.1f, 0);
        Vector3 endPos = target.position + new Vector3(0, 3.1f, 0);
        float step = 1f / lineNum;
        for(int i=1; i <= lineNum; i++)
        {
            float  t = step* i;
            Vector3 position = CalculateParabola(startPos, endPos, t);
            attackLine.SetPosition(i-1, position);
        }
        
        Invoke("HideAttackPath", 2f);
    }

    public void HideAttackPath()
    {
        attackLine.positionCount = 0;
    }

    IEnumerator PlayHitVfx()
    {
        hitVfxFloat = -1;
        isPlayingVfx = true;
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, -1, 0.2f);
        yield return new WaitForSeconds(0.2f);
        isPlayingVfx= false;
        yield return null;
    }

    public void PlayHitVfxRed()
    {
        //Debug.LogError(this.gameObject + "Hited");
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, new Color(1, 0.5f, 0.5f), 0.2f);});
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, Color.white, 0.2f); });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, new Color(1, 0.5f, 0.5f), 0.2f); });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, Color.white, 0.2f); });
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, new Color(1, 0.5f, 0.5f), 0.2f); });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { DOTween.To(() => meshRenderer.material.color, x => meshRenderer.material.color = x, Color.white, 0.2f); });

    }

    public void PlayRangeVfx(Vector3 pos)
    {
        int range = GetComponentInChildren<Player>().Range;
        GameObject visionVfx = Pool.Instance.GetObj("Vfx_VisionRange");
        visionVfx.transform.position = pos;
        visionVfx.transform.DOScale(new Vector3(3.2f*range*2, 3.2f *  range * 2, 0.5f), 1f);
        var sq = DOTween.Sequence();
        sq.AppendInterval(1f);
        sq.AppendCallback(() => { visionVfx.transform.localScale = new Vector3(3.2f,3.2f,0.5f); Pool.Instance.SetObj("Vfx_VisionRange", visionVfx); });
    }
    private Vector3 CalculateParabola(Vector3 start, Vector3 end, float t)
    {
        Vector3 height = Vector3.up * (end - start).magnitude * 0.3f;
        Vector3 midPoint = (start + end) * 0.5f + height;

        Vector3 P0 = start;
        Vector3 P1 = midPoint;
        Vector3 P2 = end;
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * P0;
        p += 2 * u * t * P1;
        p += tt * P2;
        return p;
    }
}
