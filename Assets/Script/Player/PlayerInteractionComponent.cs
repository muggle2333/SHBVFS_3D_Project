using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{

    public GameObject playerVfx;
    private GameObject gachaContainer;
    public void Move(GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        if(gridObject.landType==LandType.Mountain)
        {
            dirPos += new Vector3(0, 1.7f, 0);
        }
        else if(gridObject.landType == LandType.Lake)
        {
            dirPos += new Vector3(0, -1f, 0);
        }
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
        if (gridObject.landType == LandType.Mountain)
        {
            dirPos += new Vector3(0, 1.7f, 0);
        }
        else if (gridObject.landType == LandType.Lake)
        {
            dirPos += new Vector3(0, -1f, 0);
        }
        playerVfx.transform.position = dirPos;
        GetComponent<Player>().currentGrid = gridObject;
    }

    public void HideVfxPlayer()
    {
        if (playerVfx == null) return;
        playerVfx.SetActive(false);
    }

    public void TryGacha(GridObject gridObject)
    {
        if (gachaContainer == null)
        {
            gachaContainer = new GameObject("gachaVfxContainer");
        }
        GameObject gachaTmp = Instantiate(Resources.Load<GameObject>("VfxGacha"), gachaContainer.transform);
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        gachaTmp.transform.position = dirPos;
    }

    public void ResetGachaVfx()
    {
        if (gachaContainer == null) return;
        Destroy(gachaContainer);
        gachaContainer= null;
    }

    public void UpdateLinePath(LandType landType)
    {
        LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.positionCount += 1;
        Vector3 offset = new Vector3(0, 0.1f, 0);
        if (landType == LandType.Mountain)
        {
            offset = new Vector3(0, 1.7f, 0);
        }
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerVfx.transform.position + offset);
    }

    public void RefreshLinePath()
    {
        LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.5f, 0), lineRenderer.transform.up);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
    }

    public void DeduceFirstPath()
    {
        LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
        Vector3[] positionArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positionArray);
        var positionList = positionArray.ToList<Vector3>();
        positionList.RemoveAt(0);
        lineRenderer.SetPositions(positionList.ToArray<Vector3>());
    }
}
