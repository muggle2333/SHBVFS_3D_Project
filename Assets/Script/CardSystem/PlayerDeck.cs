using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> container = new List<Card>();
    public int x;
    public int cardSize;
    // Start is called before the first frame update
    void Start()
    {
        x = 0;

        for(int i = 0; i < deck.Count; i++)
        {
            x = Random.Range(0, cardSize);
            deck[i] = CardDataBase.cardList[x];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        for(int i = 0; i< deck.Count; i++)
        {
            container[0] = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = container[0];
        }
    }
}
