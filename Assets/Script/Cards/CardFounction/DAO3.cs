using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAO3 : CardFunction
{
    public List<int> HandCardContainer = new List<int>();
    public List<int> redPlayerHandCardsList = new List<int>();
    public List<int> bluePlayerHandCardsList = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            for(int i = 0; i < PlayerManager.Instance.redPlayerHandCardsList.Count; i++)
            {
                HandCardContainer[i] = PlayerManager.Instance.redPlayerHandCardsList[i];
                bluePlayerHandCardsList[i] = PlayerManager.Instance.redPlayerHandCardsList[i];
            }
            for (int i = 0; i < PlayerManager.Instance.bluePlayerHandCardsList.Count; i++)
            {
                redPlayerHandCardsList[i] = PlayerManager.Instance.bluePlayerHandCardsList[i];
            }
            PlayerManager.Instance.redPlayerHandCardsList = PlayerManager.Instance.bluePlayerHandCardsList;
            for(int i = 0; i < HandCardContainer.Count; i++)
            {
                PlayerManager.Instance.bluePlayerHandCardsList[i] = HandCardContainer[i];
            }
        }
        Invoke("Function", 0.1f);
    }
    void Function()
    {
        if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
        {
            for( int i = 0; i < CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]].Count; i++)
            {
                Destroy(CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]][i].gameObject);
            }
            for( int i = 0;i< redPlayerHandCardsList.Count; i++)
            {
                CardSetting cardSetting = CardDataBase.Instance.CardIdToCardSetting(redPlayerHandCardsList[i]);
                Card card = Instantiate(DrawCardComponent.Instance.cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, DrawCardComponent.Instance.CardContent.transform).GetComponent<Card>();
                card.cardSetting = cardSetting;
            }
        }
        else
        {
            for (int i = 0; i < CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]].Count; i++)
            {
                Destroy(CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]][i].gameObject);
            }
            for (int i = 0; i < bluePlayerHandCardsList.Count; i++)
            {
                CardSetting cardSetting = CardDataBase.Instance.CardIdToCardSetting(bluePlayerHandCardsList[i]);
                Card card = Instantiate(DrawCardComponent.Instance.cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, DrawCardComponent.Instance.CardContent.transform).GetComponent<Card>();
                card.cardSetting = cardSetting;
            }
        }
        Destroy(gameObject);
    }
}
