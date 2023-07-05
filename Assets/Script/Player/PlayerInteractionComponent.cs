using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerInteractionComponent : NetworkBehaviour
{


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
      
    }
}
