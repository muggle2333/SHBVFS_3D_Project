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
                AcademyUI.academiesBuff[2].text = "+1 Max HP";
                AcademyUI.academiesBuff[3].text = "+2 Max HP";
                AcademyUI.academiesBuff[4].text = "+2 MaxHP,\n+1 HP/r";
                break;
            case AcademyType.DAO:
                AcademyUI.academiesBuff[2].text = "+1 Range";
                AcademyUI.academiesBuff[3].text = "+2 Range";
                AcademyUI.academiesBuff[4].text = "+3 Range";
                break;
            case AcademyType.MO:
                AcademyUI.academiesBuff[2].text = "+1 random card/r";
                AcademyUI.academiesBuff[3].text = "+2 random card/r";
                AcademyUI.academiesBuff[4].text = "+3 random card/r";
                break;
            case AcademyType.BING:
                AcademyUI.academiesBuff[2].text = "+1 Attack";
                AcademyUI.academiesBuff[3].text = "+2 Attack";
                AcademyUI.academiesBuff[4].text = "+3 Attack";
                break;
            case AcademyType.RU :
                AcademyUI.academiesBuff[2].text = "+1 Defense";
                AcademyUI.academiesBuff[3].text = "+1 Defense";
                AcademyUI.academiesBuff[4].text = "+2 Defense";
                break;
            case AcademyType.FA:
                AcademyUI.academiesBuff[2].text = "+1 AP/r";
                AcademyUI.academiesBuff[3].text = "+1 AP/r";
                AcademyUI.academiesBuff[4].text = "+2 AP/r";
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
