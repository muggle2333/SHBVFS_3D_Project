using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class DrawCardComponent :NetworkBehaviour
{
    public Canvas parentCanvas;
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject CardContent;
    public GameObject DrawBasicCardAndEventCard;
    public Card Card;
    public int basicCardCount;
    public Camera cam;
    //public int EventCardCount;
    //public Dictionary<AcademyType, int> AllCardCount = new Dictionary<AcademyType, int>();

    public NetworkList<int> AllCardCount;
    public Player player;

    private void Awake()
    {
        AllCardCount = new NetworkList<int>();
    }
    public void Start()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i <= (int)AcademyType.FA; i++)
            {
                AllCardCount.Add(0);
            }
        }

    }
    /*[ServerRpc(RequireOwnership = false)]
    public void DrawCardServerRpc(PlayerId playerId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams{
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerId }
            }
        };
        DrawCardClientRpc(playerId,clientRpcParams);

    }
    [ClientRpc]
    public void DrawCardClientRpc(PlayerId playerId,ClientRpcParams clientRpcParams=default)
    {
        Debug.LogError(playerId);
        DrawCard(GameplayManager.Instance.player);

        
    }*/
    public void DrawCard(Player player)
    {
        this.player = player;
        GridObject gridObject = GridManager.Instance.GetCurrentGridObject(player.currentGrid);
        if (gridObject.isHasBuilding == true)
        {
            DrawBasicCard(player);
            DrawEventCard(player);
        }
        else
        {
            DrawEventCard(player);
        }
    }

    public void DrawBasicCard(Player player)
    {
        Card = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[0]];
        AllCardCountPlusServerRpc(0,AcademyType.Null);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
        Card.GetComponent<RectTransform>().position = GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
        Card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Card.transform.DOScale(1, 0.4f);
        PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
        CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, Card.cardId);
    }
    public void DrawEventCard(Player player)
    {
        AcademyType currentAcedemy = player.currentGrid.academy;
        GridObject currentGridObject = GridManager.Instance.GetCurrentGridObject(player.currentGrid);
        Card = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        if (currentGridObject.isHasBuilding)
        {
            int randomIndex = Random.Range(0, 3);
            int randomIndex_ = Random.Range(0, 2);
            if (randomIndex == 0)
            {
                Card.cardSetting = CardDataBase.Instance.AllTopCardListDic[currentAcedemy][randomIndex_];
            }
            else
            {
                Card.cardSetting = PlayerDeck.AllCardDeck[currentAcedemy][AllCardCount[(int)currentAcedemy]];
            }
        }
        else
        {
            Card.cardSetting = PlayerDeck.AllCardDeck[currentAcedemy][AllCardCount[(int)currentAcedemy]];
        }
        AllCardCountPlusServerRpc((int)currentAcedemy, currentAcedemy);
        //Card.UpdateCardData(PlayerDeck.AllCardDeck[currentAcedemy][AllCardCount[(int)currentAcedemy] -1]);
        Card.UpdateCardData(Card.cardSetting);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
        Card.GetComponent<RectTransform>().position = GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
        Debug.LogWarning(GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject));
        Card.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Card.transform.DOScale(1, 0.4f);
        PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
        CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, Card.cardId);
    }

    public Vector3 GetScreenPosition(GameObject target)
    {
        RectTransform canvasRtm = parentCanvas.GetComponent<RectTransform>();
        float width = canvasRtm.sizeDelta.x;
        float height = canvasRtm.sizeDelta.y;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
        pos.x *= width / Screen.width;
        pos.y *= height / Screen.height;
        pos.x -= width * 0.5f;
        pos.y -= height * 0.5f;
        pos.x += 950;
        pos.y += 450;
        return pos;
    }


    [ServerRpc(RequireOwnership = false)]
    public void AllCardCountPlusServerRpc(int i,AcademyType type)
    {
        if (AllCardCount[i] == PlayerDeck.AllCardDeck[type].Count - 1)
        {
            PlayerDeck.ShuffleServerRpc(type);
            AllCardCount[i] = 0;
            return;
        }
        AllCardCount[i]++;
    }
}
