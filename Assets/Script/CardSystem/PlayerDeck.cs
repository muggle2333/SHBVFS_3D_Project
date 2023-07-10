using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public Dictionary<AcademyType, List<CardSetting>> AllCardDeck = new Dictionary<AcademyType, List<CardSetting>>();

    private List<CardSetting> CardContainer = new List<CardSetting>();
    public List<CardSetting> cardDeck;
    private void Awake()
    {
        CardContainer.Add(null);
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            cardDeck = null;
            if (i == 0)
            {
                CardDataBase.AllCardListDic.TryGetValue((AcademyType)i,out cardDeck);
                for(int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        cardDeck.Add(cardDeck[k]);
                    }
                }
                AllCardDeck.Add((AcademyType)i, cardDeck);
                continue;
            }
            CardDataBase.AllCardListDic.TryGetValue((AcademyType)i, out cardDeck);
            AllCardDeck.Add((AcademyType)i, cardDeck);
        }
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            ShuffleServerRpc((AcademyType)i);
        }
    }
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /*public void Shuffle(AcademyType AcademyType)
    {
        List<CardSetting> cardDeck = null;
        AllCardDeck.TryGetValue(AcademyType, out cardDeck);


        for (int i = 0; i< cardDeck.Count; i++)
        {
            CardContainer[0] = cardDeck[i];
            int randomIndex = Random.Range(0, cardDeck.Count);
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = CardContainer[0];
        }
        AllCardDeck[AcademyType] = cardDeck;
    }*/

    [ServerRpc(RequireOwnership =false)]
    public void ShuffleServerRpc(AcademyType AcademyType)
    {
        cardDeck= null;
        AllCardDeck.TryGetValue(AcademyType, out cardDeck);

        for(int i = 0;i< cardDeck.Count; i++)
        {
            int randomIndex = Random.Range(0, cardDeck.Count);
            SetCardDeckClientRpc(randomIndex, AcademyType, i);
        }
    }
    [ClientRpc]
    public void SetCardDeckClientRpc(int randomIndex,AcademyType AcademyType,int i)
    {
        CardContainer[0] = cardDeck[i];
        cardDeck[i] = cardDeck[randomIndex];
        cardDeck[randomIndex] = CardContainer[0];
        if(i == cardDeck.Count)
        {
            AllCardDeck[AcademyType] = cardDeck;
        }
    }
}
