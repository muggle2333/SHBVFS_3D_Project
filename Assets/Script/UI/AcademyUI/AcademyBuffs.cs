using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AcademyBuffs : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject info;
    public AcademyType academyType;
    public TMP_Text AcademyCount;
    public AcademyUI AcademyUI;
    public void OnPointerEnter(PointerEventData eventData)
    {
        info.SetActive(true);
        switch (academyType)
        {
            case AcademyType.YI:
                AcademyUI.academiesBuff[2].text = "MAXHp+1";
                AcademyUI.academiesBuff[3].text = "MAXHp+2";
                AcademyUI.academiesBuff[4].text = "MAXHp+2 , Hp/Round+1";
                break;
            case AcademyType.DAO:
                AcademyUI.academiesBuff[2].text = "Range+1";
                AcademyUI.academiesBuff[3].text = "Range+2";
                AcademyUI.academiesBuff[4].text = "Range+3";
                break;
            case AcademyType.MO:
                AcademyUI.academiesBuff[2].text = "1 basic card/R";
                AcademyUI.academiesBuff[3].text = "2 basic card/R";
                AcademyUI.academiesBuff[4].text = "3 basic card/R";
                break;
            case AcademyType.BING:
                AcademyUI.academiesBuff[2].text = "Attack+1";
                AcademyUI.academiesBuff[3].text = "Attack+2";
                AcademyUI.academiesBuff[4].text = "Attack+3";
                break;
            case AcademyType.RU :
                AcademyUI.academiesBuff[2].text = "Defense+1";
                AcademyUI.academiesBuff[3].text = "Defense+1";
                AcademyUI.academiesBuff[4].text = "Defense+2";
                break;
            case AcademyType.FA:
                AcademyUI.academiesBuff[2].text = "Ap/Round+1";
                AcademyUI.academiesBuff[3].text = "Ap/Round+1";
                AcademyUI.academiesBuff[4].text = "Ap/Round+2";
                break;
        }
        if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
        {
            switch (AcademyCount.text)
            {
                case "0":
                    AcademyUI.academiesBuff[0].color = new Color32(255, 119, 100, 255);
                    break;
                case "1":
                    AcademyUI.academiesBuff[1].color = new Color32(255, 119, 100, 255);
                    break;
                case "2":
                    AcademyUI.academiesBuff[2].color = new Color32(255, 119, 100, 255);
                    break;
                case "3":
                    AcademyUI.academiesBuff[3].color = new Color32(255, 119, 100, 255);
                    break;
                case "4":
                    AcademyUI.academiesBuff[4].color = new Color32(255, 119, 100, 255);
                    break;
            }
        }
        else
        {
            switch (AcademyCount.text)
            {
                case "0":
                    AcademyUI.academiesBuff[0].color = new Color32(78, 156, 168, 255);
                    break;
                case "1":
                    AcademyUI.academiesBuff[1].color = new Color32(78, 156, 168, 255);
                    break;
                case "2":
                    AcademyUI.academiesBuff[2].color = new Color32(78, 156, 168, 255);
                    break;
                case "3":
                    AcademyUI.academiesBuff[3].color = new Color32(78, 156, 168, 255);
                    break;
                case "4":
                    AcademyUI.academiesBuff[4].color = new Color32(78, 156, 168, 255);
                    break;
            }
        }
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        for(int i = 0; i < 5; i++)
        {
            AcademyUI.academiesBuff[i].color = new Color32(237, 203, 121, 255);
        }
        info.SetActive(false);
    }
}
