using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectComponent : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public GridObject gridObject;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameplayManager.Instance.ShowGirdObjectData(gameObject.transform);

        gridObject = GridManager.Instance.grid.GetGridObject(gameObject.transform.position);
        SelectManager.Instance.SetSelectObject(gridObject);

    }

    //private void OnMouseEnter()
    public void OnPointerEnter(PointerEventData eventData)
    {
        gridObject = GridManager.Instance.grid.GetGridObject(gameObject.transform.position);
        //Debug.Log(gridObject.x+"+"+gridObject.z);
        SelectManager.Instance.SetCurrentSelectObject(gridObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectManager.Instance.RemoveCurrentSelectObject(gridObject);
    }

    //private void OnMouseDown()
    //{
    //    SelectManager.Instance.SetSelectObject(gridObject);
    //}
}
