using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectComponent : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
        GridManager.Instance.GetSelectedGridObject(gameObject.transform.position);
    }
}
