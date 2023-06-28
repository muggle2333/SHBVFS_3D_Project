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
    public bool IsInOpreationStage;
    public bool isSelected;
    public float targetY;
    public float formerY;
    public float duration;
    public GameObject Info;
    void Start()
    {
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material = Instantiate(Resources.Load<Material>("CardEffects/outline"));
        isSelected = false;
        duration = 0.25f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.yellow);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0.03f);

        Info.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);

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
        if(IsInOpreationStage)
        {
            foreach(var card in FindObjectOfType<CardSelectManager>().cardsList)
            {
                card.EndSelect();
            }
        }
        if (isSelected) EndSelect();
        else OnSelect();
    }

    public void OnSelect()
    {
        //index = transform.GetSiblingIndex();
        //transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
    }
    public void EndSelect()
    {
        //transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
    }
}
