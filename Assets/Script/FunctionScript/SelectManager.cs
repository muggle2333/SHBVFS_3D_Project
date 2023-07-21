using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }

    public List<GridObject> selectGridObject;
    public int selectCount = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSelectObject(GridObject gridObject)
    {
        if(gridObject == null)
        {
            selectGridObject = null;
        }
        else
        {
            if(selectCount>1)
            {

            }
            else
            {
                selectGridObject[0] = gridObject;
            }
        }
    }
}
