using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance;
    public Dictionary<Player, List<Card>> playerHandCardDict = new Dictionary<Player, List<Card>>();
    public Dictionary<Player, List<CardSetting>> playedCardDict = new Dictionary<Player, List<CardSetting>>();

    public NetworkList<int> redPlayerPlayedCards;
    public NetworkList<int> bluePlayerPlayedCards;
    public GameObject CardPrefeb;
    public GameObject CardContent;
    private Calculating calculating;
    private PlayerDeck playerDeck;
    private CardSelectManager cardSelectManager;
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
                playedCardDict.Add(playerList[i], new List<CardSetting>());
            }
            if (!playerHandCardDict.ContainsKey(playerList[i]))
            {
                playerHandCardDict.Add(playerList[i], new List<Card>());
            }
        }
        calculating = FindObjectOfType<Calculating>();
        cardSelectManager = GetComponentInChildren<CardSelectManager>();
        cardSelectManager.InitializeCardSelectManager();
        playerDeck = GetComponentInChildren<PlayerDeck>();
        playerDeck.InitializePlayerDeck();
    }
    public void ImmediateCardTakeEffect(Player player, Card card)
    {
        if (card.cardFounction != null)
        {
            Instantiate(card.cardFounction);
        }
        calculating.DelataCardData(card.cardSetting, player);
        calculating.CalculatPlayerBaseData(player);
        calculating.CalaulatPlayerData(player);
    }
    public void S1CardTakeEffect(Player player,Card card)
    {
        if (card.cardFounction != null)
        {
            Instantiate(card.cardFounction);
        }
        calculating.DelataCardData(card.cardSetting, player);
        calculating.CalculatPlayerBaseData(player);
        calculating.CalaulatPlayerData(player);
    }
    [ClientRpc]
    public void CardTakeEffectClientRpc(PlayerId playerId,int cardId)
    {
        if(playerId == PlayerId.RedPlayer)
        {
            CardTakeEffect(GameplayManager.Instance.playerList[0], cardId);
        }
        else
        {
            CardTakeEffect(GameplayManager.Instance.playerList[1], cardId);
        }
    }
    public void CardTakeEffect(Player player, int cardId)
    {
        //playedCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();

        for(int i = 0; i < CardDataBase.Instance.AllCardList.Count; i++)
        {
            if(cardId == CardDataBase.Instance.AllCardList[i].cardId)
            {
                for (int j = 0; j < playedCardDict[player].Count; j++)
                {
                    if (playedCardDict[player][j].cardId == cardId)
                    {
                        if (player == GameplayManager.Instance.currentPlayer)
                        {
                            //Self Card TakeEffect Animation
                            //playedCardDict[player][j].gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
                            var card = Instantiate(CardPrefeb, new Vector3(-800, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
                            card.cardSetting = CardDataBase.Instance.AllCardList[i];
                            card.gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
                            playedCardDict[player].RemoveAt(j);
                        }
                        else
                        {
                            //Enemy Card TakeEffect Animation
                            //playedCardDict[player][j].gameObject.GetComponent<CardSelectComponent>().EnemyCardTakeEffectAnimation();
                            var card = Instantiate(CardPrefeb, new Vector3(-800, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
                            card.cardSetting = CardDataBase.Instance.AllCardList[i];
                            card.gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
                            playedCardDict[player].RemoveAt(j);
                        }
                    }
                }
                if (CardDataBase.Instance.AllCardList[i].cardFounction != null)
                {
                    Instantiate(CardDataBase.Instance.AllCardList[i].cardFounction);
                }
                calculating.DelataCardData(CardDataBase.Instance.AllCardList[i], player);
                calculating.CalculatPlayerBaseData(player);
                calculating.CalaulatPlayerData(player);
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
    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayedCardServerRpc(PlayerId playerId,int cardId)
    {
        if(playerId == PlayerId.RedPlayer)
        {
            for(int i = 0; i < redPlayerPlayedCards.Count; i++)
            {
                if (redPlayerPlayedCards[i] == cardId)
                {
                    redPlayerPlayedCards.RemoveAt(i);
                }
            }
            RefreshPlayedCardDictClientRpc();
        }
        else
        {
            for (int i = 0; i < bluePlayerPlayedCards.Count; i++)
            {
                if (bluePlayerPlayedCards[i] == cardId)
                {
                    bluePlayerPlayedCards.RemoveAt(i);
                }
            }
            RefreshPlayedCardDictClientRpc();
        }
    }
    [ClientRpc]
    public void RefreshPlayedCardDictClientRpc()
    {
        List<CardSetting> redPlayerPlayed = new List<CardSetting>();
        List<CardSetting> bluePlayerPlayed = new List<CardSetting>();
        for(int i = 0; i < redPlayerPlayedCards.Count; i++)
        {
            for(int j = 0; j < CardDataBase.Instance.AllCardList.Count; j++)
            {
                if (redPlayerPlayedCards[i] == CardDataBase.Instance.AllCardList[j].cardId)
                {
                    redPlayerPlayed.Add(CardDataBase.Instance.AllCardList[j]);
                }
            }
        }
        for (int i = 0; i < bluePlayerPlayedCards.Count; i++)
        {
            for (int j = 0; j < CardDataBase.Instance.AllCardList.Count; j++)
            {
                if (bluePlayerPlayedCards[i] == CardDataBase.Instance.AllCardList[j].cardId)
                {
                    bluePlayerPlayed.Add(CardDataBase.Instance.AllCardList[j]);
                }
            }
        }
        playedCardDict[GameplayManager.Instance.playerList[0]] = redPlayerPlayed;
        playedCardDict[GameplayManager.Instance.playerList[1]] = bluePlayerPlayed;
    }
}
