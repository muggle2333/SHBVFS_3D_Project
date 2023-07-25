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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        info.SetActive(true);
        switch (academyType)
        {
            case AcademyType.YI:
                AcademyUI.academiesBuff[2].text = "Hp+1";
                AcademyUI.academiesBuff[3].text = "Hp+2";
                AcademyUI.academiesBuff[4].text = "Hp+2 , Hp/R+1";
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
                AcademyUI.academiesBuff[2].text = "Ap/R+1";
                AcademyUI.academiesBuff[3].text = "Ap/R+2";
                AcademyUI.academiesBuff[4].text = "Ap/R+3";
                break;
        }
        switch (AcademyCount.text)
        {
            case "0":
                AcademyUI.academiesBuff[0].color = Color.yellow;
                break;
            case "1":
                AcademyUI.academiesBuff[1].color = Color.yellow;
                break;
            case "2":
                AcademyUI.academiesBuff[2].color = Color.yellow;
                break;
            case "3":
                AcademyUI.academiesBuff[3].color = Color.yellow;
                break;
            case "4":
                AcademyUI.academiesBuff[4].color = Color.yellow;
                break;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        for(int i = 0; i < 5; i++)
        {
            AcademyUI.academiesBuff[i].color = Color.white;
        }
        info.SetActive(false);
    }
}
