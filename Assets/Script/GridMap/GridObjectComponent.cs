using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectComponent : MonoBehaviour,IPointerClickHandler
{
    public GridObject gridObject;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameplayManager.Instance.ShowGirdObjectData(gameObject.transform);

        gridObject = GridManager.Instance.grid.GetGridObject(gameObject.transform.position);

        
    }

    private void OnMouseEnter()
    {
        gridObject = GridManager.Instance.grid.GetGridObject(gameObject.transform.position);
        //Debug.Log(gridObject.x+"+"+gridObject.z);
        SelectManager.Instance.SetCurrentSelectObject(gridObject);
    }

    private void OnMouseExit()
    {
        SelectManager.Instance.RemoveCurrentSelectObject(gridObject);
    }

    private void OnMouseDown()
    {
        SelectManager.Instance.SetSelectObject(gridObject);
    }
}
