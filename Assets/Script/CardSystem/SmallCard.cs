using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmallCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardSetting cardSetting;
    public Text description;
    public GameObject info;
    private void Start()
    {
        UpdateCardData(cardSetting);
    }
    public void UpdateCardData(CardSetting cardSetting)
    {
        description.text = cardSetting.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        info.SetActive(true); 
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        info.SetActive(false);
    }
}
