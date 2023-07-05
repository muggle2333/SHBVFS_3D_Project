using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardComponent : MonoBehaviour
{
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
        Card = Instantiate(cardPrefab, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[AcademyType.Null]];
        AllCardCount[AcademyType.Null]++;
        FindObjectOfType<CardSelectManager>().Start();
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
        Card = Instantiate(cardPrefab, CardContent.transform).GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]];
        AllCardCount[currentPlayer.currentGrid.academy]++;
        Card.UpdateCardData(PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]-1]);

        
        FindObjectOfType<CardSelectManager>().Start();
    }
}
