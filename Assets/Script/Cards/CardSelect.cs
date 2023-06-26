using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private int index;
    public bool isSelected;
    public float targetY;
    public float formerY;
    public float duration;
    public GameObject Info;
    void Start()
    {
        transform.gameObject.GetComponent<Image>().material = Instantiate(Resources.Load<Material>("CardEffects/outline"));
        isSelected = false;
        targetY = 30;
        formerY = 0;
        duration = 0.25f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        transform.gameObject.GetComponent<Image>().material.SetColor("_EdgeColor", Color.yellow);
        transform.gameObject.GetComponent<Image>().material.SetFloat("_Edge", 0.03f);
        //index = transform.GetSiblingIndex();
        //transform.SetAsLastSibling();
        Info.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.gameObject.GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        transform.gameObject.GetComponent<Image>().material.SetFloat("_Edge", 0);
        //transform.SetSiblingIndex(index);
        Info.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) EndSelect();
        else OnSelect();
    }

    public void OnSelect()
    {
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
    }
    public void EndSelect()
    {
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
    }

}
