using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    //public List<List<CardSetting>> DeckList = new List<List<CardSetting>>();

    public List<CardSetting> basicCardDeck = new List<CardSetting>();
    public List<CardSetting> BINGCardDeck = new List<CardSetting>();
    public List<CardSetting> DAOCardDeck = new List<CardSetting>();
    public List<CardSetting> FACardDeck = new List<CardSetting>();
    public List<CardSetting> MOCardDeck = new List<CardSetting>();
    public List<CardSetting> RUCardDeck = new List<CardSetting>();
    public List<CardSetting> YICardDeck = new List<CardSetting>();

    private List<CardSetting> basicCardContainer = new List<CardSetting>();
    private List<CardSetting> BINGCardContainer = new List<CardSetting>();
    private List<CardSetting> DAOCardContainer = new List<CardSetting>();
    private List<CardSetting> FACardContainer = new List<CardSetting>();
    private List<CardSetting> MOCardContainer = new List<CardSetting>();
    private List<CardSetting> RUCardContainer = new List<CardSetting>();
    private List<CardSetting> YICardContainer = new List<CardSetting>();
    // Start is called before the first frame update
    void Start()
    {
        basicCardContainer.Add(null);
        BINGCardContainer.Add(null);
        DAOCardContainer.Add(null);
        FACardContainer.Add(null);
        MOCardContainer.Add(null);
        RUCardContainer.Add(null);
        YICardContainer.Add(null);

        

        //Random.Range(0, CardDataBase.basicCardList.Length);
        for (int j = 0; j < CardDataBase.basicCardList.Length; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                basicCardDeck.Add(CardDataBase.basicCardList[j]);
            }
        }
        for(int j = 0; j < CardDataBase.BINGCardList.Length; j++)
        {
            BINGCardDeck.Add(CardDataBase.BINGCardList[j]);
        }
        for (int j = 0; j < CardDataBase.DAOCardList.Length; j++)
        {
            DAOCardDeck.Add(CardDataBase.DAOCardList[j]);
        }
        for (int j = 0; j < CardDataBase.FACardList.Length; j++)
        {
            FACardDeck.Add(CardDataBase.FACardList[j]);
        }
        for (int j = 0; j < CardDataBase.MOCardList.Length; j++)
        {
            MOCardDeck.Add(CardDataBase.MOCardList[j]);
        }
        for (int j = 0; j < CardDataBase.RUCardList.Length; j++)
        {
            RUCardDeck.Add(CardDataBase.RUCardList[j]);
        }
        for (int j = 0; j < CardDataBase.YICardList.Length; j++)
        {
            YICardDeck.Add(CardDataBase.YICardList[j]);
        }
        Shuffle();
        for(int a = 0; a <= 5 ; a++)
        {
            ShuffleEventCard(a);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        for (int i = 0; i< basicCardDeck.Count; i++)
        {
            basicCardContainer[0] = basicCardDeck[i];
            int randomIndex = Random.Range(0, basicCardDeck.Count);
            basicCardDeck[i] = basicCardDeck[randomIndex];
            basicCardDeck[randomIndex] = basicCardContainer[0];
        }
    }
    public void ShuffleEventCard(int a)
    {
        if (a == 0)
        {
            for (int i = 0; i < BINGCardDeck.Count; i++)
            {
                BINGCardContainer[0] = BINGCardDeck[i];
                int randomIndex = Random.Range(0, BINGCardDeck.Count);
                BINGCardDeck[i] = BINGCardDeck[randomIndex];
                BINGCardDeck[randomIndex] = BINGCardContainer[0];
            }
        }
        else if(a == 1)
        {
            for (int i = 0; i < DAOCardDeck.Count; i++)
            {
                DAOCardContainer[0] = DAOCardDeck[i];
                int randomIndex = Random.Range(0, DAOCardDeck.Count);
                DAOCardDeck[i] = DAOCardDeck[randomIndex];
                DAOCardDeck[randomIndex] = DAOCardContainer[0];
            }
        }
        else if (a == 2)
        {
            for (int i = 0; i < FACardDeck.Count; i++)
            {
                FACardContainer[0] = FACardDeck[i];
                int randomIndex = Random.Range(0, FACardDeck.Count);
                FACardDeck[i] = FACardDeck[randomIndex];
                FACardDeck[randomIndex] = FACardContainer[0];
            }
        }
        else if (a == 3)
        {
            for (int i = 0; i < MOCardDeck.Count; i++)
            {
                MOCardContainer[0] = MOCardDeck[i];
                int randomIndex = Random.Range(0, MOCardDeck.Count);
                MOCardDeck[i] = MOCardDeck[randomIndex];
                MOCardDeck[randomIndex] = MOCardContainer[0];
            }
        }
        else if (a == 4)
        {
            for (int i = 0; i < RUCardDeck.Count; i++)
            {
                RUCardContainer[0] = RUCardDeck[i];
                int randomIndex = Random.Range(0, RUCardDeck.Count);
                RUCardDeck[i] = RUCardDeck[randomIndex];
                RUCardDeck[randomIndex] = RUCardContainer[0];
            }
        }
        else if (a == 5)
        {
            for (int i = 0; i < YICardDeck.Count; i++)
            {
                YICardContainer[0] = YICardDeck[i];
                int randomIndex = Random.Range(0, YICardDeck.Count);
                YICardDeck[i] = YICardDeck[randomIndex];
                YICardDeck[randomIndex] = YICardContainer[0];
            }
        }
    }
}
