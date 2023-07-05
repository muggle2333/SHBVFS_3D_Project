using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardComponent : MonoBehaviour
{
    public Canvas parentCanvas;
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject CardContent;
    public GameObject DrawBasicCardAndEventCard;

    public Card Card;
    public int basicCardCount;

    //public int EventCardCount;
    public Dictionary<AcademyType, int> AllCardCount = new Dictionary<AcademyType, int>();
    public Player currentPlayer;

    private void Awake()
    {
        for (int i = 0; i < (int)AcademyType.FA; i++)
        {
            AllCardCount.Add((AcademyType)i,0);
        }
    }
    public void DrawCard(Player player)
    {
        currentPlayer = player;
        if (player.CurrentActionPoint < 1)
        {
            Debug.Log("NoActionPoint");
        }
        else
        {
            if (player.currentGrid.isHasBuilding == true)
            {
                DrawBasicCard();
                DrawEventCard();
            }
            else
            {
                DrawBasicCardAndEventCard.SetActive(true);
            }
            player.CurrentActionPoint--;
        }
    }

    public void TryDrawCard()
    {
        
    }
    public void DrawBasicCard()
    {
        DrawBasicCardAndEventCard.SetActive(false);
        if (AllCardCount[AcademyType.Null] > PlayerDeck.AllCardDeck[AcademyType.Null].Count-1)
        {
            PlayerDeck.Shuffle(AcademyType.Null);
            AllCardCount[AcademyType.Null] = 0;
        }
        Card = Instantiate(cardPrefab, GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[AcademyType.Null]];
        AllCardCount[AcademyType.Null]++;
        PlayerManager.Instance.cardSelectManager.Start();
    }
    public void DrawEventCard()
    {
        DrawBasicCardAndEventCard.SetActive(false);
        if (AllCardCount[currentPlayer.currentGrid.academy] > PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy].Count - 1)
        {
            PlayerDeck.Shuffle(currentPlayer.currentGrid.academy);
            AllCardCount[currentPlayer.currentGrid.academy] = 0;
        }
        if (currentPlayer.currentGrid.isHasBuilding == false)
        {
            if (PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]].cardLevel == CardLevel.Top)
            {
                AllCardCount[currentPlayer.currentGrid.academy]++;
                DrawEventCard();
                return;
            }
        }
        Card = Instantiate(cardPrefab, GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject), Quaternion.identity, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]];
        AllCardCount[currentPlayer.currentGrid.academy]++;
        Card.UpdateCardData(PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]-1]);
        PlayerManager.Instance.cardSelectManager.Start();
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
}
