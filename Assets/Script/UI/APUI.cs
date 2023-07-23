using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class APUI : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Object;
    public TMP_Text APPerRound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        APPerRound.text = GameplayManager.Instance.currentPlayer.ActionPointPerRound.ToString() + " / R";
        Object.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Object.SetActive(false);
    }
}
