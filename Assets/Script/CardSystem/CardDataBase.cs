using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static CardComponent[] basicCardList;
    public static CardComponent[] BINGCardList;
    public static CardComponent[] DAOCardList;
    public static CardComponent[] FACardList;
    public static CardComponent[] MOCardList;
    public static CardComponent[] RUCardList;
    public static CardComponent[] YICardList;
    private void Awake()
    {
        basicCardList = Resources.LoadAll<CardComponent>("Cards/BasicCards");
        BINGCardList = Resources.LoadAll<CardComponent>("Cards/EventCards/BING");
        DAOCardList = Resources.LoadAll<CardComponent>("Cards/EventCards/DAO");
        FACardList = Resources.LoadAll<CardComponent>("Cards/EventCards/FA");
        MOCardList = Resources.LoadAll<CardComponent>("Cards/EventCards/MO");
        RUCardList = Resources.LoadAll<CardComponent>("Cards/EventCards/RU");
        YICardList = Resources.LoadAll<CardComponent>("Cards/EventCards/YI");
    }
}
