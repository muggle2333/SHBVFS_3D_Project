using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
    public Vector2 tmpVector;


    public void Move(Vector2 dirVector)
    {
        Vector3 dirPos = GridManager.Instance.grid.GetWorldPositionCenter((int)dirVector.x, (int)dirVector.y);

        if(Vector3.Distance(transform.position,dirPos)<= GridManager.Instance.gridDistance)
        {
            transform.position = dirPos;
            tmpVector = dirVector;
        }
    }
}
