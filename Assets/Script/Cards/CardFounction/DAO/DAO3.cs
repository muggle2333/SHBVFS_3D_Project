using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class DAO3 : CardFunction
{
    public List<int> HandCardContainer = new List<int>();
    public List<int> redPlayerHandCardsList = new List<int>();
    public List<int> bluePlayerHandCardsList = new List<int>();
    public Card card;
    // Start is called before the first frame update
    void Start()
    {
        HandCardContainer.Clear();
        redPlayerHandCardsList.Clear();
        bluePlayerHandCardsList.Clear();
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("1");
            for(int i = 0; i < PlayerManager.Instance.redPlayerHandCardsList.Count; i++)
            {
                HandCardContainer.Add(PlayerManager.Instance.redPlayerHandCardsList[i]);
            }
            PlayerManager.Instance.redPlayerHandCardsList.Clear();
            for (int i = 0; i < PlayerManager.Instance.bluePlayerHandCardsList.Count; i++)
            {
                PlayerManager.Instance.redPlayerHandCardsList.Add(PlayerManager.Instance.bluePlayerHandCardsList[i]);
            }
           
            PlayerManager.Instance.bluePlayerHandCardsList.Clear();
            for(int i = 0; i < HandCardContainer.Count; i++)
            {
                PlayerManager.Instance.bluePlayerHandCardsList.Add(HandCardContainer[i]);
            }
        }
        Invoke("AddHandCard", 0.2f);
        Invoke("Function", 2.5f);
    }
    void AddHandCard()
    {
        for (int i = 0; i < PlayerManager.Instance.redPlayerHandCardsList.Count; i++)
        {
            redPlayerHandCardsList.Add(PlayerManager.Instance.redPlayerHandCardsList[i]);
        }
        for (int i = 0; i < PlayerManager.Instance.bluePlayerHandCardsList.Count; i++)
        {
            bluePlayerHandCardsList.Add(PlayerManager.Instance.bluePlayerHandCardsList[i]);
        }
    }
/*    [ServerRpc(RequireOwnership =false)]
    void FunctionServerRpc()
    {
        FunctionClientRpc();
    }
    [ClientRpc]*/
    void Function()
    {
        if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
        {
            for ( int i = 0; i < CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]].Count; i++)
            {
                Destroy(CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]][i].gameObject);
            }
            CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]].Clear();
            for ( int i = 0;i< redPlayerHandCardsList.Count; i++)
            {
                CardSetting cardSetting = CardDataBase.Instance.CardIdToCardSetting(redPlayerHandCardsList[i]);
                card = Instantiate(DrawCardComponent.Instance.cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, DrawCardComponent.Instance.CardContent.transform).GetComponent<Card>();
                card.cardSetting = cardSetting;
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[0]].Add(card);
                card.UpdateCardData(card.cardSetting);
                card.GetComponent<RectTransform>().localPosition = DrawCardComponent.Instance.GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
                card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                card.transform.DOScale(1, 0.5f);
                PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.playerList[0]);
            }
        }
        else
        {
            
            for (int i = 0; i < CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]].Count; i++)
            {
                Destroy(CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]][i].gameObject);
            }
            CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]].Clear();
            for (int i = 0; i < bluePlayerHandCardsList.Count; i++)
            {
                CardSetting cardSetting = CardDataBase.Instance.CardIdToCardSetting(bluePlayerHandCardsList[i]);
                card = Instantiate(DrawCardComponent.Instance.cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, DrawCardComponent.Instance.CardContent.transform).GetComponent<Card>();
                card.cardSetting = cardSetting;
                CardManager.Instance.playerHandCardDict[GameplayManager.Instance.playerList[1]].Add(card);
                card.UpdateCardData(card.cardSetting);
                card.GetComponent<RectTransform>().localPosition = DrawCardComponent.Instance.GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
                card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                card.transform.DOScale(1, 0.5f);
                PlayerManager.Instance.cardSelectManager.UpdateCardPos(GameplayManager.Instance.playerList[1]);
            }
        }
        Invoke("Destroy", 3);
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
}
