using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class DrawCardComponent :NetworkBehaviour
{
    public Canvas parentCanvas;
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject CardContent;
    public GameObject DrawBasicCardAndEventCard;

    public Card Card;
    public int basicCardCount;

    //public int EventCardCount;
    //public Dictionary<AcademyType, int> AllCardCount = new Dictionary<AcademyType, int>();

    public NetworkList<int> AllCardCount;
    public Player currentPlayer;

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
        DrawCard(GameplayManager.Instance.currentPlayer);

        
    }*/
    public void DrawCard(Player player)
    {
        currentPlayer = player;
        if (player.currentGrid.isHasBuilding == true)
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
        Card = Instantiate(cardPrefab, GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[0]];
        AllCardCountPlusServerRpc(0,AcademyType.Null);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
        Card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Card.transform.DOScale(1, 0.4f);
        PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
        CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, Card.cardId);
    }
    public void DrawEventCard(Player player)
    {
        if (currentPlayer.currentGrid.isHasBuilding == false)
        {
            if (PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[(int)currentPlayer.currentGrid.academy]].cardLevel == CardLevel.Top)
            {
                //AllCardCount[(int)currentPlayer.currentGrid.academy]++;
                AllCardCountPlusServerRpc((int)currentPlayer.currentGrid.academy, currentPlayer.currentGrid.academy);
                DrawEventCard(player);
                return;
            }
        }
        Card = Instantiate(cardPrefab, GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[(int)currentPlayer.currentGrid.academy]];
        AllCardCountPlusServerRpc((int)currentPlayer.currentGrid.academy, currentPlayer.currentGrid.academy);
        Card.UpdateCardData(PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[(int)currentPlayer.currentGrid.academy]-1]);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
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
        if (AllCardCount[i] > PlayerDeck.AllCardDeck[type].Count - 1)
        {
            PlayerDeck.ShuffleServerRpc(type);
            AllCardCount[i] = 0;
        }
        AllCardCount[i]++;
    }
}
