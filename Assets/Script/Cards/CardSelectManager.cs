using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectManager : MonoBehaviour
{
    public CardSelect[] cardsArray; 
    void Start()
    {
        cardsArray = GetComponentsInChildren<CardSelect>();
    }

    public void SelectCards()
    {
        foreach (var card in cardsArray)
        {
            if(card.isSelected)
            {
                Debug.Log("Card " + card.name + " is played.");
                card.EndSelect();
            }

        }
    }

    public void CancelCards()
    {
        foreach (var card in cardsArray)
        {
            if (card.isSelected)
            {
                Debug.Log("Card " + card.name + " is canceled.");
                card.EndSelect();
            }

        }
    }

}
