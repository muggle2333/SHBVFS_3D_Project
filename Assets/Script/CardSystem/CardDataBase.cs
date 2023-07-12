using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static Dictionary<AcademyType, List<CardSetting>> AllCardListDic = new Dictionary<AcademyType, List<CardSetting>>();
    public List<CardSetting> AllCardList = new List<CardSetting>();
    private void Awake()
    {
        for(int i=0;i<=(int)AcademyType.FA;i++)
        {
            if(i==0)
            {
                //Debug.Log(Resources.LoadAll<CardSetting>("Cards/BasicCards").Length);
                AllCardListDic.Add((AcademyType)i, new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards/BasicCards")));
                continue;
            }
            AllCardListDic.Add((AcademyType)i, new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards/EventCards/"+ ((AcademyType)i).ToString())));
        }
        AllCardList = new List<CardSetting>(Resources.LoadAll<CardSetting>("Cards"));

    }
}
