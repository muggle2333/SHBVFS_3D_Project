using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{


    public void Move(GridObject gridObject)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)gridObject.x, (int)gridObject.z);
        transform.position = dirPos;
        GetComponent<Player>().currentGrid = gridObject;
      
    }
}
