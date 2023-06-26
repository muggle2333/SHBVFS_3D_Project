using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static CardSetting[] basicCardList;
    public static CardSetting[] BINGCardList;
    public static CardSetting[] DAOCardList;
    public static CardSetting[] FACardList;
    public static CardSetting[] MOCardList;
    public static CardSetting[] RUCardList;
    public static CardSetting[] YICardList;
    private void Awake()
    {
        basicCardList = Resources.LoadAll<CardSetting>("Cards/BasicCards");
        BINGCardList = Resources.LoadAll<CardSetting>("Cards/EventCards/BING");
        DAOCardList = Resources.LoadAll<CardSetting>("Cards/EventCards/DAO");
        FACardList = Resources.LoadAll<CardSetting>("Cards/EventCards/FA");
        MOCardList = Resources.LoadAll<CardSetting>("Cards/EventCards/MO");
        RUCardList = Resources.LoadAll<CardSetting>("Cards/EventCards/RU");
        YICardList = Resources.LoadAll<CardSetting>("Cards/EventCards/YI");
    }
}
