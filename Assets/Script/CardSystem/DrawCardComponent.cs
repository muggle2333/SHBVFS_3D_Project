using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardComponent : MonoBehaviour
{
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject Panel;
    public GameObject DrawBasicCardAndEventCard;
    public GameObject cardObject;

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(AllCardCount[currentPlayer.currentGrid.academy]);
            Debug.Log(PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy].Count);
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

    public void DrawBasicCard()
    {

        DrawBasicCardAndEventCard.SetActive(false);
        if (AllCardCount[AcademyType.Null] > PlayerDeck.AllCardDeck[AcademyType.Null].Count-1)
        {
            PlayerDeck.Shuffle(AcademyType.Null);
            AllCardCount[AcademyType.Null] = 0;
        }
        cardObject = Instantiate(cardPrefab, Panel.transform);
        Card = cardObject.GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[AcademyType.Null][AllCardCount[AcademyType.Null]];
        AllCardCount[AcademyType.Null]++;
        FindObjectOfType<CardSelectManager>().Start();
        FindObjectOfType<CardSelectManager>().UpdateCardPos();
    }
    public void DrawEventCard()
    {
        DrawBasicCardAndEventCard.SetActive(false);
        if (AllCardCount[currentPlayer.currentGrid.academy] > PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy].Count - 1)
        {
            PlayerDeck.Shuffle(currentPlayer.currentGrid.academy);
            AllCardCount[currentPlayer.currentGrid.academy] = 0;
        }
        cardObject = Instantiate(cardPrefab, Panel.transform);
        Card = cardObject.GetComponent<Card>();
        Card.cardSetting = PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]];
        AllCardCount[currentPlayer.currentGrid.academy]++;
        Card.UpdateCardData(PlayerDeck.AllCardDeck[currentPlayer.currentGrid.academy][AllCardCount[currentPlayer.currentGrid.academy]-1]);

        if (currentPlayer.currentGrid.isHasBuilding == false)
        {
            if (Card.cardLevel == CardLevel.Top)
            {
                Destroy(cardObject);
                DrawEventCard();
            }
        }
        FindObjectOfType<CardSelectManager>().Start();
        FindObjectOfType<CardSelectManager>().UpdateCardPos();
    }
}
