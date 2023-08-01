using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class DrawCardComponent : NetworkBehaviour
{
    public int cardId;
    public static DrawCardComponent Instance;
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
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    public void Start()
    {
        if (NetworkManager.Singleton.IsServer)
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
        SoundManager.Instance.PlaySound(Sound.DrawCard);
        if (gridObject.isHasBuilding == true)
        {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() => { DrawEventCard(player); });
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() => { DrawEventCard(player); });
            PlayDrawCardAnimationServerRpc(player.Id, 2);
        }
        else
        {
            DrawEventCard(player);
            PlayDrawCardAnimationServerRpc(player.Id, 1);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void PlayDrawCardAnimationServerRpc(PlayerId playerId,int drawCount)
    {
        PlayDrawCardAnimationClientRpc(playerId, drawCount);
    }
    [ClientRpc]
    public void PlayDrawCardAnimationClientRpc(PlayerId playerId, int drawCount)
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && GameplayManager.Instance.PlayerIdToPlayer(playerId) != GameplayManager.Instance.currentPlayer)
        {
            return;
        }
        var player = GameplayManager.Instance.PlayerIdToPlayer(playerId);
        if (drawCount > 0)
        {
            player.headCardText.text = "+" + drawCount;
        }
        else if(drawCount < 0)
        {
            player.headCardText.text = drawCount.ToString();
        }
        else if(drawCount == 0)
        {
            return;
        }
        player.headCard.SetActive(true);
        player.headCard.transform.localPosition = new Vector3(0, 6.6f, 0);
        var seq = DOTween.Sequence();
        seq.Append(player.headCard.transform.DOLocalMoveY(8.5f, 1f));
        //seq.AppendInterval(0.8f);
        seq.AppendCallback(() => { player.headCard.SetActive(false); });
    }
    public void DrawBasicCard(Player player)
    {
        Card = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[0]];
        Card.UpdateCardData(Card.cardSetting);
        AllCardCountPlusServerRpc(0,AcademyType.Null);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
        Card.GetComponent<RectTransform>().localPosition = GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
        Card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Card.transform.DOScale(1, 0.5f);
        PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
        CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, Card.cardId);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DrawEventCardForTest(cardId);
        }
    }
    public void DrawEventCardForTest(int cardId)
    {
        Card = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = CardManager.Instance.CardIdToCardSetting(cardId);
        Card.UpdateCardData(Card.cardSetting);
        CardManager.Instance.playerHandCardDict[player].Add(Card);
        Card.GetComponent<RectTransform>().localPosition = GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
        Card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Card.transform.DOScale(1, 0.5f);
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
            int randomIndex = Random.Range(0, 1000);
            int randomIndex_ = Random.Range(0, 2);
            if (randomIndex <= 99)
            {
                DrawBasicCard(player);
                return;
            }
            else if(randomIndex <= 299)
            {
                Card.cardSetting = CardDataBase.Instance.AllTopCardListDic[currentAcedemy][randomIndex_];
            }
            else if(randomIndex <= 999)
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
        Card.GetComponent<RectTransform>().localPosition = GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
        Card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Card.transform.DOScale(1, 0.5f);
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
        pos.x += 50;
        pos.y += 100;
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
