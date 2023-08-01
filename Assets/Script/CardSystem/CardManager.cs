using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance;
    public Dictionary<Player, List<Card>> playerHandCardDict = new Dictionary<Player, List<Card>>();
    public Dictionary<Player, List<CardSetting>> playedCardDict = new Dictionary<Player, List<CardSetting>>();
    public PlayedCardUI playedCardUI;
    public NetworkList<int> redPlayerPlayedCards;
    public NetworkList<int> bluePlayerPlayedCards;
    public GameObject CardPrefeb;
    public GameObject CardContent;
    public GameObject smallCard;
    public GameObject cardCemetery;
    public List<SmallCard> smallCards;
    private Calculating calculating;
    private PlayerDeck playerDeck;
    private CardSelectManager cardSelectManager;

    public void Awake()
    {
        playerHandCardDict.Clear();
        playedCardDict.Clear();
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.LogError(playerHandCardDict[GameplayManager.Instance.playerList[1]].Count);
        }
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
    /*[ServerRpc(RequireOwnership = false)]
    public void ImmediateCardTakeEffectServerRpc(PlayerId playerId, int cardId)
    {
        CalculateClientRpc(playerId, cardId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void S1CardTakeEffectServerRpc(PlayerId playerId, int cardId)
    {
        CalculateClientRpc(playerId, cardId);
    }
    [ClientRpc]
    public void CalculateClientRpc(PlayerId playerId, int cardId)
    {
        for (int i = 0; i < CardDataBase.Instance.AllCardList.Count; i++)
        {
            if (cardId == CardDataBase.Instance.AllCardList[i].cardId)
            {
                if (CardDataBase.Instance.AllCardList[i].cardFounction != null)
                {
                    Instantiate(CardDataBase.Instance.AllCardList[i].cardFounction);
                }
                calculating.DelataCardData(CardDataBase.Instance.AllCardList[i], GameplayManager.Instance.playerList[(int)playerId]);
                calculating.CalaulatPlayerData(GameplayManager.Instance.playerList[(int)playerId]);
            }
        }
    }*/
    [ClientRpc]
    public void CardTakeEffectClientRpc(PlayerId playerId,int cardId)
    {
        Player player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        CardTakeEffect(player, CardIdToCardSetting(cardId));
    }
    public CardSetting CardIdToCardSetting(int cardId)
    {
        for (int i = 0; i < CardDataBase.Instance.AllCardList.Count; i++)
        {
            if (cardId == CardDataBase.Instance.AllCardList[i].cardId)
            {
                return CardDataBase.Instance.AllCardList[i];
            }
        }
        return null;
    }
    public void EffectedCards(PlayerId playerId,CardSetting card)
    {
        if (smallCards.Count == 9)
        {
            var Card = smallCards[0];
            smallCards.RemoveAt(0);
            Destroy(Card.gameObject);
        }
        var smallcard = Instantiate(smallCard, cardCemetery.transform).GetComponent<SmallCard>();
        smallcard.cardSetting = card;
        smallcard.transform.SetAsFirstSibling();
        if(playerId == PlayerId.RedPlayer)
        {
            smallcard.gameObject.GetComponent<Image>().sprite = smallcard.gameObject.GetComponent<SmallCard>().redCard;
        }
        else
        {
            smallcard.gameObject.GetComponent<Image>().sprite = smallcard.gameObject.GetComponent<SmallCard>().blueCard;
        }
        smallCards.Add(smallcard);
    }
    public void CardTakeEffect(Player player, CardSetting effectCard)
    {
        var card = Instantiate(CardPrefeb, new Vector3(-800, 500, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        card.gameObject.GetComponent<CardSelectComponent>().Interactable = false;
        card.cardSetting = effectCard;
        card.UpdateCardData(card.cardSetting);
        EffectedCards(player.Id, effectCard);
        if (player == GameplayManager.Instance.currentPlayer)
        {
            card.transform.position = new Vector3(-800, 500, 0);
            card.gameObject.GetComponent<CardSelectComponent>().CardTakeEffectAnimation();
            
        }
        else
        {
            card.transform.position = new Vector3(800, 500, 0);
            card.gameObject.GetComponent<CardSelectComponent>().EnemyCardTakeEffectAnimation();
        }
        if (NetworkManager.Singleton.IsServer)
        {
            RemovePlayedCardServerRpc(player.Id, effectCard.cardId);
        }
        if (effectCard.cardFounction != null)
        {
            var function = Instantiate(effectCard.cardFounction).GetComponent<CardFunction>();
            function.player = player;
            function.cardSetting = effectCard;
        }
        calculating.DelataCardData(effectCard, player);
        calculating.CalaulatPlayerData(player);
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
            Invoke("RefreshPlayedCardDictClientRpc", 1);
        }
        else
        {
            bluePlayerPlayedCards.Add(cardId);
            Invoke("RefreshPlayedCardDictClientRpc", 1);
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
                    break;
                }
            }
            Invoke("RefreshPlayedCardDictClientRpc", 1);
        }
        else
        {
            for (int i = 0; i < bluePlayerPlayedCards.Count; i++)
            {
                if (bluePlayerPlayedCards[i] == cardId)
                {
                    bluePlayerPlayedCards.RemoveAt(i);
                    break;
                }
            }
            Invoke("RefreshPlayedCardDictClientRpc", 1);
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
                    //Debug.LogError(CardDataBase.Instance.AllCardList[j].cardId);
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
                    //Debug.LogError(CardDataBase.Instance.AllCardList[j].cardId);
                    bluePlayerPlayed.Add(CardDataBase.Instance.AllCardList[j]);
                }
            }
        }
        playedCardDict[GameplayManager.Instance.playerList[0]] = redPlayerPlayed;
        playedCardDict[GameplayManager.Instance.playerList[1]] = bluePlayerPlayed;
    }
    public int[] CheckDiscardNum()
    {
        int[] additionCardNum = new int[2];
        additionCardNum[0]= PlayerManager.Instance.redPlayerHandCardsList.Count - GameplayManager.Instance.playerList[0].HP;
        additionCardNum[1]= PlayerManager.Instance.bluePlayerHandCardsList.Count - GameplayManager.Instance.playerList[1].HP;
        return additionCardNum;
    }

    [ClientRpc]
    public void DeselectCardClientRpc()
    {
        DeselectCard();
    }
    public void DeselectCard()
    {
        cardSelectManager.CancelCards(GameplayManager.Instance.currentPlayer);
    }
}
