using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    //public List<List<CardComponent>> DeckList = new List<List<CardComponent>>();

    public List<CardComponent> basicCardDeck = new List<CardComponent>();
    public List<CardComponent> BINGCardDeck = new List<CardComponent>();
    public List<CardComponent> DAOCardDeck = new List<CardComponent>();
    public List<CardComponent> FACardDeck = new List<CardComponent>();
    public List<CardComponent> MOCardDeck = new List<CardComponent>();
    public List<CardComponent> RUCardDeck = new List<CardComponent>();
    public List<CardComponent> YICardDeck = new List<CardComponent>();

    private List<CardComponent> basicCardContainer = new List<CardComponent>();
    private List<CardComponent> BINGCardContainer = new List<CardComponent>();
    private List<CardComponent> DAOCardContainer = new List<CardComponent>();
    private List<CardComponent> FACardContainer = new List<CardComponent>();
    private List<CardComponent> MOCardContainer = new List<CardComponent>();
    private List<CardComponent> RUCardContainer = new List<CardComponent>();
    private List<CardComponent> YICardContainer = new List<CardComponent>();
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
}
