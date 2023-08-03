using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDeck : NetworkBehaviour
{
    public Dictionary<AcademyType, List<CardSetting>> AllCardDeck = new Dictionary<AcademyType, List<CardSetting>>();
    public List<CardSetting> CardContainer = new List<CardSetting>();
    public List<CardSetting> cardDeck;
    NetworkVariable<int> randomIndex;
    private void Awake()
    {
        randomIndex = new NetworkVariable<int>(0);
        CardContainer.Add(null);
        
    }
    void Start()
    {
    }
    public void InitializePlayerDeck()
    {
        AllCardDeck.Clear();
        for (int i = 0; i <= (int)AcademyType.FA; i++)
        {
            cardDeck = null;
            if (i == 0)
            {
                CardDataBase.AllCardListDic.TryGetValue((AcademyType)i, out cardDeck);
                for (int j = 0; j < 10; j++)
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
        if (NetworkManager.Singleton.IsHost)
        {
            Invoke("ShuffleAllCard",2f);
        }
    }
    private void ShuffleAllCard()
    {
        for (int i = 0; i <= (int)AcademyType.FA; i++)
        {
            ShuffleServerRpc((AcademyType)i);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void ShuffleServerRpc(AcademyType academyType)
    {
        

        for(int i = 0;i < AllCardDeck[academyType].Count; i++)
        {
            cardDeck = null;
            AllCardDeck.TryGetValue(academyType, out cardDeck);
            randomIndex.Value = Random.Range(0, cardDeck.Count);
            SetCardDeckClientRpc(randomIndex.Value, academyType, i);
        }
    }
    [ClientRpc]
    public void SetCardDeckClientRpc(int randomIndex,AcademyType AcademyType,int i)
    {
        cardDeck = AllCardDeck[AcademyType];
        CardSetting cardsetting = cardDeck[i];
        cardDeck[i] = cardDeck[randomIndex];
        cardDeck[randomIndex] = cardsetting;
        if (i == cardDeck.Count)
        {
            AllCardDeck[AcademyType] = cardDeck;
        }
    }
}
