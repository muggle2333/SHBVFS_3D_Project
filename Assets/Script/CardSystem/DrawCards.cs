using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public PlayerDeck PlayerDeck;
    public GameObject cardPrefab;
    public GameObject Panel;
    public Card Card;
    public int basicCardCount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(PlayerDeck.basicCardDeck.Count);
        }
    }
    public void DrawCard()
    {
        if(basicCardCount > PlayerDeck.basicCardDeck.Count-1)
        {
            PlayerDeck.Shuffle();
            basicCardCount = 0;
        }
        Card = Instantiate(cardPrefab,Panel.transform).GetComponent<Card>();
        Card.card = PlayerDeck.basicCardDeck[basicCardCount];
        Card.Cardname.text = Card.card.name;
        Card.Description.text = Card.card.Description;
        Card.Head = Card.card.Head;
        Card.Academies = Card.card.Academies;
        Card.Damage = Card.card.Damage;
        Card.LoseHp = Card.card.LoseHp;
        Card.Defense = Card.card.Defense;
        Card.Health = Card.card.Health;
        Card.VisionRange = Card.card.VisionRange;
        Card.Cardtarget = Card.card.Cardtarget;
        Card.cardBuff = Card.card.cardBuff;
        basicCardCount++;
    }
}
