using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject Panel;
    public Player player;

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
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(player.currentGrid.academy);
        }
    }
    public void DrawBasicCard()
    {
        if(basicCardCount > PlayerDeck.basicCardDeck.Count-1)
        {
            PlayerDeck.Shuffle();
            basicCardCount = 0;
        }
        Card = Instantiate(cardPrefab,Panel.transform).GetComponent<Card>();

        Card.card = PlayerDeck.basicCardDeck[basicCardCount];
        
        basicCardCount++;
    }
    public void DrawEventCard()
    {
        if (player.currentGrid.academy == AcademyType.BING)
        {
            if (BINGCardCount > PlayerDeck.BINGCardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                BINGCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.BINGCardDeck[BINGCardCount];

            BINGCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.DAO)
        {
            if (DAOCardCount > PlayerDeck.DAOCardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                DAOCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.DAOCardDeck[DAOCardCount];

            DAOCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.FA)
        {
            if (FACardCount > PlayerDeck.FACardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                FACardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.FACardDeck[FACardCount];

            FACardCount++;
        }
        if (player.currentGrid.academy == AcademyType.MO)
        {
            if (MOCardCount > PlayerDeck.MOCardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                MOCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.MOCardDeck[MOCardCount];

            MOCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.RU)
        {
            if (RUCardCount > PlayerDeck.RUCardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                RUCardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.RUCardDeck[RUCardCount];

            RUCardCount++;
        }
        if (player.currentGrid.academy == AcademyType.YI)
        {
            if (YICardCount > PlayerDeck.YICardDeck.Count - 1)
            {
                PlayerDeck.Shuffle();
                YICardCount = 0;
            }
            Card = Instantiate(cardPrefab, Panel.transform).GetComponent<Card>();

            Card.card = PlayerDeck.YICardDeck[YICardCount];

            YICardCount++;
        }
    }
}
