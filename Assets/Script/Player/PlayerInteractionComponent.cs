using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{

    private GameObject playerVfx;
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

    }

    public void MoveVfxPlayer(GridObject gridObject)
    {
        if (playerVfx == null)
        {
            string path = "VfxPlayer";
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
        if(gachaContainer == null)
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
}
