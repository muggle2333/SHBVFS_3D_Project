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
    public Button DrawCardButton;
    public GameObject DrawBasicCardAndEventCard;

    public Card Card;
    public int basicCardCount;

    //public int EventCardCount;

    public int BINGCardCount;
    public int DAOCardCount;
    public int FACardCount;
    public int MOCardCount;
    public int RUCardCount;
    public int YICardCount;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

       
    }
    public void DrawCard(Player player)
    {
        if (player.CurrentActionPoint < 1)
        {
            Debug.Log("NoActionPoint");
        }
        else
        {
            if (player.currentGrid.isHasBuilding == true)
            {
                DrawBasicCard(player);
                DrawEventCard(player);
            }
            else
            {
                DrawBasicCardAndEventCard.SetActive(true);
                DrawCardButton.interactable= false;
            }
            player.CurrentActionPoint--;
            FindObjectOfType<CardSelectManager>().Start();
            FindObjectOfType<CardSelectManager>().UpdateCardPos();
        }
    }

    public void DrawBasicCard(Player player)
    {

        DrawBasicCardAndEventCard.SetActive(false);
        if (basicCardCount > PlayerDeck.basicCardDeck.Count-1)
        {
            PlayerDeck.Shuffle();
            basicCardCount = 0;
        }
        Card = Instantiate(cardPrefab,Panel.transform).GetComponent<Card>();

        Card.cardSetting = PlayerDeck.basicCardDeck[basicCardCount];
        DrawCardButton.interactable = true;

        basicCardCount++;
    }
    public void DrawEventCard(Player player)
    {
        DrawCardButton.interactable = true;
        DrawBasicCardAndEventCard.SetActive(false);
        if (player.currentGrid.academy == AcademyType.BING)
        {
            if (BINGCardCount > PlayerDeck.BINGCardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(0);
                BINGCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.BINGCardDeck[BINGCardCount];

            BINGCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.DAO)
        {
            if (DAOCardCount > PlayerDeck.DAOCardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(1);
                DAOCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.DAOCardDeck[DAOCardCount];

            DAOCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.FA)
        {
            if (FACardCount > PlayerDeck.FACardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(2);
                FACardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.FACardDeck[FACardCount];

            FACardCount++;
        }
        if (player.currentGrid.academy == AcademyType.MO)
        {
            if (MOCardCount > PlayerDeck.MOCardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(3);
                MOCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.MOCardDeck[MOCardCount];

            MOCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.RU)
        {
            if (RUCardCount > PlayerDeck.RUCardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(4);
                RUCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.RUCardDeck[RUCardCount];

            RUCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.YI)
        {
            if (YICardCount > PlayerDeck.YICardDeck.Count - 1)
            {
                PlayerDeck.ShuffleEventCard(5);
                YICardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.cardSetting = PlayerDeck.YICardDeck[YICardCount];

            YICardCount++;
        }
    }
}
