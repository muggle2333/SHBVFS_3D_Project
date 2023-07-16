using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance;
    public Dictionary<Player, List<Card>> playerHandCardDict = new Dictionary<Player, List<Card>>();
    public Dictionary<Player, List<Card>> playedCardDict = new Dictionary<Player, List<Card>>();

    public NetworkList<int> redPlayerPlayedCards;
    public NetworkList<int> bluePlayerPlayedCards;
    public Calculating calculating;
    public void Awake()
    {
        redPlayerPlayedCards = new NetworkList<int>();
        bluePlayerPlayedCards = new NetworkList<int>();
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (FindObjectOfType<NetworkManager>()) return;
        InitializeCardManager();
    }
    public void InitializeCardManager()
    {
        List<Player> playerList = new List<Player>();
        playerList = GameplayManager.Instance.playerList;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playedCardDict.ContainsKey(playerList[i]))
            {
                playedCardDict.Add(playerList[i], new List<Card>());
            }
            if (!playerHandCardDict.ContainsKey(playerList[i]))
            {
                playerHandCardDict.Add(playerList[i], new List<Card>());
            }
        }
        calculating = FindObjectOfType<Calculating>();
    }
    public void ImmediateCardTakeEffect(Player player, Card card)
    {
        if (card.cardFounction != null)
        {
            Instantiate(card.cardFounction);
        }
        calculating.DelataCardData(card, player);
        calculating.CalculatPlayerBaseData(player);
        calculating.CalaulatPlayerData(player);
    }
    public void S1CardTakeEffect(Player player,Card card)
    {
        if (card.cardFounction != null)
        {
            Instantiate(card.cardFounction);
        }
        calculating.DelataCardData(card, player);
        calculating.CalculatPlayerBaseData(player);
        calculating.CalaulatPlayerData(player);
    }
    [ClientRpc]
    public void CardTakeEffectClientRpc(PlayerId playerId,EffectStage stage)
    {
        if(playerId == PlayerId.RedPlayer)
        {
            CardTakeEffect(GameplayManager.Instance.playerList[0],stage);
        }
        else
        {
            CardTakeEffect(GameplayManager.Instance.playerList[1], stage);
        }
    }
    public void CardTakeEffect(Player player, EffectStage stage)
    {
        for (int i = 0; i < playedCardDict[player].Count; i++)
        {
            if (playedCardDict[player][i].effectStage == stage)
            {
                if (playedCardDict[player][i].cardFounction != null)
                {
                    Instantiate(playedCardDict[player][i].cardFounction);
                }

                playedCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
                calculating.DelataCardData(playedCardDict[player][i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
                playedCardDict[player].RemoveAt(i);
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddCardToPlayerHandServerRpc(PlayerId playerId,int cardId)
    {
        if (playerId == PlayerId.RedPlayer)
        {
            PlayerManager.Instance.redPlayerHandCardsList.Add(cardId);
        }
        else
        {
            PlayerManager.Instance.bluePlayerHandCardsList.Add(cardId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveCardFromPlayerHandServerRpc(PlayerId playerId, int i)
    {
        if (playerId == PlayerId.RedPlayer)
        {
            PlayerManager.Instance.redPlayerHandCardsList.RemoveAt(i);
        }
        else
        {
            PlayerManager.Instance.bluePlayerHandCardsList.RemoveAt(i);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddPlayedCardServerRpc(PlayerId playerId, int cardId)
    {
        if (playerId == PlayerId.RedPlayer)
        {
            redPlayerPlayedCards.Add(cardId);
            RefreshPlayedCardDictClientRpc();
        }
        else
        {
            bluePlayerPlayedCards.Add(cardId);
            RefreshPlayedCardDictClientRpc();
        }
    }
    [ClientRpc]
    public void RefreshPlayedCardDictClientRpc()
    {
        List<Card> redPlayerPlayed = new List<Card>();
        List<Card> bluePlayerPlayed = new List<Card>();
        for(int i = 0; i < redPlayerPlayedCards.Count; i++)
        {
            for(int j = 0; j < CardDataBase.Instance.cards.Count; j++)
            {
                if (redPlayerPlayedCards[i] == CardDataBase.Instance.cards[j].cardId)
                {
                    redPlayerPlayed.Add(CardDataBase.Instance.cards[j]);
                }
            }
        }
        for (int i = 0; i < bluePlayerPlayedCards.Count; i++)
        {
            for (int j = 0; j < CardDataBase.Instance.cards.Count; j++)
            {
                if (bluePlayerPlayedCards[i] == CardDataBase.Instance.cards[j].cardId)
                {
                    bluePlayerPlayed.Add(CardDataBase.Instance.cards[j]);
                }
            }
        }
        playedCardDict[GameplayManager.Instance.playerList[0]] = redPlayerPlayed;
        playedCardDict[GameplayManager.Instance.playerList[1]] = bluePlayerPlayed;
    }
}
