using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float zoomSize;
    private int index;
    //????
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(1);
        transform.localScale = new Vector3(zoomSize, zoomSize, zoomSize);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }
    //????
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        transform.SetSiblingIndex(index);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
